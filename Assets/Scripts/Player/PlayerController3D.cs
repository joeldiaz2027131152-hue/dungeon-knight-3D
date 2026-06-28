using System.Collections.Generic;
using DungeonKnight.Level;
using UnityEngine;

namespace DungeonKnight.Player
{
    public enum HudMessageIcon
    {
        None,
        Sword,
        Shield
    }

    [RequireComponent(typeof(CharacterController))]
    public class PlayerController3D : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 4.2f;
        [SerializeField] private float dashSpeed = 5.9f;
        [SerializeField] private float jumpSpeed = 7.4f;
        [SerializeField] private float gravity = -22f;
        [SerializeField] private float turnSpeed = 14f;

        [Header("Combat")]
        [SerializeField] private int lightAttackDamage = 20;
        [SerializeField] private int chargedAttackDamage = 44;
        [SerializeField] private float attackReach = 1.55f;
        [SerializeField] private float attackRadius = 0.95f;
        [SerializeField] private float attackCooldown = 0.68f;
        [SerializeField] private float chargedAttackThreshold = 0.68f;
        [SerializeField] private float chargedAttackDurationMultiplier = 2.35f;
        [SerializeField] private float lightAttackDamageWindow = 0.44f;
        [SerializeField] private float chargedAttackDamageWindow = 0.58f;
        [SerializeField] private float blockMoveMultiplier = 0.42f;
        [SerializeField] private float blockStaminaRecoveryThreshold = 18f;
        [SerializeField] private float parryWindow = 0.16f;
        [SerializeField] private float parryStaminaReward = 24f;
        [SerializeField] private float rollInvulnerableDuration = 0.38f;
        [SerializeField] private float lockOnRange = 42f;
        [SerializeField] private float lightAttackStaminaCost = 16f;
        [SerializeField] private float chargedAttackStaminaCost = 38f;
        [SerializeField] private float rollStaminaCost = 31f;
        [SerializeField] private float blockStaminaDrainPerSecond = 13f;
        [SerializeField] private float staminaRecoveryPerSecond = 19f;
        [SerializeField] private float interactionRange = 3.25f;

        private readonly Collider[] attackHits = new Collider[12];
        private readonly Collider[] interactHits = new Collider[24];
        private readonly Collider[] lockOnHits = new Collider[32];
        private CharacterController controller;
        private PlayerInventory inventory;
        private Transform cameraPivot;
        private Vector3 velocity;
        private Vector3 lastMoveDirection = Vector3.forward;
        private float stamina = 100f;
        private float attackTimer;
        private float attackDuration;
        private float chargeTimer;
        private float dashTimer;
        private float hurtTimer;
        private float invulnerableTimer;
        private float parryTimer;
        private readonly Queue<StatusMessageEntry> queuedMessages = new Queue<StatusMessageEntry>();
        private float messageUntil;
        private string message = string.Empty;
        private HudMessageIcon messageIcon;
        private Vector3 respawnPoint = DungeonKnight3DBootstrap.PlayerSpawn;
        private bool blockExhausted;
        private bool attackCharged;
        private bool attackHitResolved;
        private bool deathHandled;
        private int pendingAttackDamage;
        private int attackSequence;
        private DungeonEnemy3D lockOnTarget;
        private DungeonInteractable3D currentInteractable;

        public int MaxHealth { get; private set; } = 120;
        public int Health { get; private set; } = 120;
        public int Coins { get; private set; }
        public int Potions { get; private set; } = 2;
        public bool HasGateKey { get; private set; }
        public bool HasTowerKey { get; private set; }
        public float Stamina => stamina;
        public float MaxStamina => 100f;
        public bool IsBlocking { get; private set; }
        public bool IsDashing => dashTimer > 0f;
        public bool IsCharging => chargeTimer > 0f;
        public bool IsAttacking => attackTimer > 0f;
        public bool IsChargedAttack => attackTimer > 0f && attackCharged;
        public bool IsInvulnerable => invulnerableTimer > 0f;
        public int AttackSequence => attackSequence;
        public float CurrentAttackDuration => attackDuration;
        public float AttackProgress => attackDuration > 0.001f ? 1f - Mathf.Clamp01(attackTimer / attackDuration) : 1f;
        public bool IsGrounded => controller && controller.isGrounded;
        public bool IsMoving { get; private set; }
        public Vector3 MoveDirection { get; private set; } = Vector3.zero;
        public Vector3 FacingDirection => lastMoveDirection;
        public Vector3 PlanarVelocity { get; private set; }
        public float PlanarSpeed => PlanarVelocity.magnitude;
        public DungeonEnemy3D LockOnTarget => lockOnTarget && lockOnTarget.IsAlive ? lockOnTarget : null;
        public bool HasLockOn => LockOnTarget;
        public string CurrentInteractionPrompt => currentInteractable ? currentInteractable.GetPrompt(this) : string.Empty;
        public string StatusMessage => Time.time < messageUntil ? message : string.Empty;
        public HudMessageIcon StatusMessageIcon => Time.time < messageUntil ? messageIcon : HudMessageIcon.None;

        private void Awake()
        {
            lockOnRange = Mathf.Max(lockOnRange, 42f);
            controller = GetComponent<CharacterController>();
            inventory = GetComponent<PlayerInventory>();
            cameraPivot = Camera.main ? Camera.main.transform : null;
        }

        private void Update()
        {
            if (!cameraPivot && Camera.main) cameraPivot = Camera.main.transform;

            if (Health <= 0)
            {
                ShowMessage("Has caido. Pulsa R para volver a la hoguera.", 0.2f);
                if (Input.GetKeyDown(KeyCode.R)) Respawn();
                return;
            }

            attackTimer = Mathf.Max(0f, attackTimer - Time.deltaTime);
            if (attackTimer <= 0f)
            {
                attackCharged = false;
                attackHitResolved = true;
            }

            hurtTimer = Mathf.Max(0f, hurtTimer - Time.deltaTime);
            invulnerableTimer = Mathf.Max(0f, invulnerableTimer - Time.deltaTime);
            parryTimer = Mathf.Max(0f, parryTimer - Time.deltaTime);

            UpdateMovement();
            UpdateCombat();
            UpdateInteraction();
            UpdateMessageQueue();

            if (!IsBlocking && !IsAttacking && stamina < MaxStamina)
            {
                stamina = Mathf.Min(MaxStamina, stamina + staminaRecoveryPerSecond * Time.deltaTime);
            }
        }

        public void AddCoins(int amount)
        {
            Coins += Mathf.Max(0, amount);
            ShowMessage($"+{amount} almas", 1.8f);
        }

        public void RecoverSouls(int amount)
        {
            Coins += Mathf.Max(0, amount);
            ShowMessage($"Recuperaste {amount} almas", 2f);
        }

        public bool AddPotion()
        {
            if (Potions >= 5)
            {
                ShowMessage("Bolsa de pociones llena", 1.1f);
                return false;
            }

            Potions++;
            ShowMessage("Pocion recuperada", 1.8f);
            return true;
        }

        public void RestAtBonfire(Vector3 bonfirePosition)
        {
            respawnPoint = bonfirePosition + Vector3.back * 1.35f + Vector3.up * 0.6f;
            Health = MaxHealth;
            stamina = MaxStamina;
            Potions = Mathf.Max(Potions, 2);
            foreach (DungeonEnemy3D enemy in Object.FindObjectsByType<DungeonEnemy3D>(FindObjectsInactive.Exclude))
            {
                enemy.RestoreAtBonfire();
            }

            ShowMessage("Hoguera encendida. Vida, stamina y enemigos activos restaurados.", 2.4f);
        }

        public void GiveGateKey()
        {
            HasGateKey = true;
            ShowMessage("Llave del guardian obtenida", 2.4f);
        }

        public void GiveTowerKey()
        {
            HasTowerKey = true;
            ShowMessage("Llave de la torre obtenida", 2.4f);
        }

        public void TakeDamage(int amount)
        {
            TakeEnemyHit(amount, null, transform.position);
        }

        public bool TakeEnemyHit(int amount, DungeonEnemy3D source, Vector3 sourcePosition)
        {
            if (Health <= 0) return false;

            if (IsInvulnerable)
            {
                ShowMessage("Esquiva limpia", 0.55f);
                CombatFeedback3D.Spawn(transform.position + Vector3.up * 0.85f, new Color(0.46f, 0.72f, 1f), 9);
                return false;
            }

            if (IsBlocking && source && parryTimer > 0f)
            {
                stamina = Mathf.Min(MaxStamina, stamina + parryStaminaReward);
                source.Stagger(1.35f, transform.position, true);
                CombatFeedback3D.Spawn(transform.position + Vector3.up * 1f + lastMoveDirection * 0.45f, new Color(0.55f, 0.9f, 1f), 18);
                ShowMessage("Parry perfecto", 0.9f);
                return false;
            }

            int finalAmount = IsBlocking ? Mathf.CeilToInt(amount * 0.35f) : amount;
            if (IsBlocking)
            {
                stamina = Mathf.Max(0f, stamina - amount * 1.4f * EffectiveBlockStaminaMultiplier);
                if (stamina <= 0.001f)
                {
                    blockExhausted = true;
                    IsBlocking = false;
                }
            }

            Health = Mathf.Max(0, Health - finalAmount);
            hurtTimer = 0.18f;
            CameraFollow3D.Shake(IsBlocking ? 0.08f : 0.18f, 0.16f);
            CombatFeedback3D.Spawn(transform.position + Vector3.up * 0.9f, IsBlocking ? new Color(0.42f, 0.94f, 1f) : new Color(1f, 0.28f, 0.16f), IsBlocking ? 8 : 14);
            ShowMessage(IsBlocking ? "Bloqueo firme" : $"-{finalAmount} HP", 0.85f);
            if (Health <= 0)
            {
                HandleDeath();
            }

            return true;
        }

        public void ShowMessage(string text, float duration)
        {
            ShowMessage(text, duration, HudMessageIcon.None);
        }

        public void ShowMessage(string text, float duration, HudMessageIcon icon)
        {
            message = text;
            messageIcon = icon;
            messageUntil = Time.time + duration;
        }

        public void QueueMessage(string text, float duration, HudMessageIcon icon)
        {
            if (Time.time >= messageUntil)
            {
                ShowMessage(text, duration, icon);
                return;
            }

            queuedMessages.Enqueue(new StatusMessageEntry(text, duration, icon));
        }

        private void UpdateMessageQueue()
        {
            if (Time.time < messageUntil || queuedMessages.Count == 0) return;

            StatusMessageEntry next = queuedMessages.Dequeue();
            ShowMessage(next.Text, next.Duration, next.Icon);
        }

        private readonly struct StatusMessageEntry
        {
            public readonly string Text;
            public readonly float Duration;
            public readonly HudMessageIcon Icon;

            public StatusMessageEntry(string text, float duration, HudMessageIcon icon)
            {
                Text = text;
                Duration = duration;
                Icon = icon;
            }
        }

        public void TeleportTo(Vector3 destination, Vector3 facingDirection, string statusText)
        {
            velocity = Vector3.zero;
            PlanarVelocity = Vector3.zero;
            dashTimer = 0f;
            chargeTimer = 0f;
            attackTimer = 0f;
            attackCharged = false;
            attackHitResolved = true;
            IsBlocking = false;

            Vector3 flatFacing = Vector3.ProjectOnPlane(facingDirection, Vector3.up);
            if (flatFacing.sqrMagnitude < 0.001f) flatFacing = Vector3.forward;
            flatFacing.Normalize();

            controller.enabled = false;
            transform.position = destination;
            transform.rotation = Quaternion.LookRotation(flatFacing, Vector3.up);
            controller.enabled = true;
            lastMoveDirection = flatFacing;
            ShowMessage(statusText, 1.8f);
        }

        private void UpdateMovement()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 input = new Vector3(horizontal, 0f, vertical);
            input = Vector3.ClampMagnitude(input, 1f);

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ToggleLockOn();
            }

            ValidateLockOnTarget();

            Vector3 forward = Vector3.forward;
            Vector3 right = Vector3.right;
            if (cameraPivot)
            {
                forward = Vector3.ProjectOnPlane(cameraPivot.forward, Vector3.up).normalized;
                right = Vector3.ProjectOnPlane(cameraPivot.right, Vector3.up).normalized;
            }

            Vector3 move = (right * input.x + forward * input.z).normalized;
            MoveDirection = move;
            IsMoving = move.sqrMagnitude > 0.001f;
            Vector3 lockDirection = Vector3.zero;
            if (LockOnTarget)
            {
                lockDirection = LockOnTarget.transform.position - transform.position;
                lockDirection.y = 0f;
            }

            if (lockDirection.sqrMagnitude > 0.01f && dashTimer <= 0f)
            {
                lastMoveDirection = lockDirection.normalized;
                Quaternion targetRotation = Quaternion.LookRotation(lastMoveDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * 1.25f * Time.deltaTime);
            }
            else if (move.sqrMagnitude > 0.001f)
            {
                lastMoveDirection = move;
                Quaternion targetRotation = Quaternion.LookRotation(lastMoveDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }

            if (controller.isGrounded && velocity.y < 0f) velocity.y = -1.5f;
            if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
            {
                velocity.y = jumpSpeed;
            }

            if (Input.GetKeyDown(KeyCode.L) && dashTimer <= 0f && stamina >= rollStaminaCost)
            {
                stamina -= rollStaminaCost;
                dashTimer = 0.78f;
                invulnerableTimer = rollInvulnerableDuration;
            }

            bool wantsBlock = Input.GetKey(KeyCode.K) && dashTimer <= 0f;
            if (Input.GetKeyDown(KeyCode.K) && dashTimer <= 0f && stamina > 0f)
            {
                parryTimer = parryWindow;
            }

            if (!wantsBlock || stamina >= blockStaminaRecoveryThreshold)
            {
                blockExhausted = false;
            }

            IsBlocking = wantsBlock && !blockExhausted && stamina > 0f;
            if (IsBlocking)
            {
                stamina = Mathf.Max(0f, stamina - blockStaminaDrainPerSecond * EffectiveBlockStaminaMultiplier * Time.deltaTime);
                if (stamina <= 0.001f)
                {
                    blockExhausted = true;
                    IsBlocking = false;
                }
            }

            float speed = IsBlocking ? moveSpeed * blockMoveMultiplier : moveSpeed;
            if (dashTimer > 0f)
            {
                dashTimer -= Time.deltaTime;
                move = lastMoveDirection;
                MoveDirection = move;
                IsMoving = true;
                speed = dashSpeed;
            }

            PlanarVelocity = move * speed;
            velocity.y += gravity * Time.deltaTime;
            controller.Move((PlanarVelocity + velocity) * Time.deltaTime);
        }

        private void UpdateCombat()
        {
            if (Input.GetKeyDown(KeyCode.J) && !IsBlocking)
            {
                chargeTimer = 0.01f;
            }

            if (Input.GetKey(KeyCode.J) && chargeTimer > 0f && !IsBlocking)
            {
                chargeTimer += Time.deltaTime;
            }
            else if (chargeTimer > 0f && IsBlocking)
            {
                chargeTimer = 0f;
            }

            if (Input.GetKeyUp(KeyCode.J) && chargeTimer > 0f)
            {
                if (attackTimer <= 0f)
                {
                    bool charged = chargeTimer >= chargedAttackThreshold;
                    float staminaCost = charged ? chargedAttackStaminaCost : lightAttackStaminaCost;
                    if (stamina >= staminaCost)
                    {
                        stamina -= staminaCost;
                        Attack(charged);
                    }
                    else
                    {
                        ShowMessage("Sin stamina", 0.65f);
                    }
                }

                chargeTimer = 0f;
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                TryUsePotion();
            }

            ResolveAttackWindow();
        }

        private void Attack(bool charged)
        {
            if (attackTimer > 0f) return;

            attackDuration = charged ? attackCooldown * chargedAttackDurationMultiplier : attackCooldown;
            attackTimer = attackDuration;
            attackCharged = charged;
            attackHitResolved = false;
            pendingAttackDamage = charged ? EffectiveChargedAttackDamage : EffectiveLightAttackDamage;
            attackSequence++;
        }

        public bool TryUsePotion()
        {
            if (Potions <= 0)
            {
                ShowMessage("No tienes pociones", 0.9f);
                return false;
            }

            if (Health >= MaxHealth)
            {
                ShowMessage("Vida llena", 0.8f);
                return false;
            }

            Potions--;
            Health = Mathf.Min(MaxHealth, Health + 45);
            CombatFeedback3D.Spawn(transform.position + Vector3.up * 0.9f, new Color(0.95f, 0.12f, 0.22f), 12);
            ShowMessage("Pocion usada", 1.4f);
            return true;
        }

        private PlayerInventory Inventory
        {
            get
            {
                if (!inventory) inventory = GetComponent<PlayerInventory>();
                return inventory;
            }
        }

        private int EffectiveLightAttackDamage => Inventory ? Inventory.LightAttackDamage : lightAttackDamage;
        private int EffectiveChargedAttackDamage => Inventory ? Inventory.ChargedAttackDamage : chargedAttackDamage;
        private float EffectiveBlockStaminaMultiplier => Inventory ? Inventory.BlockStaminaMultiplier : 1f;

        private void ResolveAttackWindow()
        {
            if (!IsAttacking || attackHitResolved) return;

            float hitWindow = attackCharged ? chargedAttackDamageWindow : lightAttackDamageWindow;
            if (AttackProgress < hitWindow) return;

            attackHitResolved = true;
            ApplyAttackDamage(pendingAttackDamage, attackCharged);
        }

        private void ApplyAttackDamage(int damage, bool charged)
        {
            Vector3 center = transform.position + Vector3.up * 0.9f + lastMoveDirection * attackReach;
            int count = Physics.OverlapSphereNonAlloc(center, attackRadius, attackHits);
            bool hitSomething = false;

            for (int i = 0; i < count; i++)
            {
                DungeonEnemy3D enemy = attackHits[i].GetComponentInParent<DungeonEnemy3D>();
                if (!enemy) continue;

                bool critical = enemy.IsStaggered;
                enemy.TakeDamage(critical ? damage * 2 : damage, transform.position, charged);
                hitSomething = true;
                if (critical)
                {
                    CombatFeedback3D.Spawn(enemy.transform.position + Vector3.up * 1.1f, new Color(0.9f, 0.82f, 1f), 14);
                    CameraFollow3D.Shake(0.22f, 0.2f);
                }
            }

            CombatFeedback3D.Spawn(center, hitSomething ? new Color(1f, 0.68f, 0.18f) : new Color(0.65f, 0.72f, 0.84f), hitSomething ? 16 : 7);
            if (hitSomething) CameraFollow3D.Shake(charged ? 0.16f : 0.09f, 0.12f);
            ShowMessage(hitSomething ? (charged ? "Golpe cargado" : "Corte limpio") : "La espada corta el aire", 0.75f);
        }

        private void ToggleLockOn()
        {
            if (LockOnTarget)
            {
                lockOnTarget = null;
                ShowMessage("Lock-on desactivado", 0.65f);
                return;
            }

            lockOnTarget = FindNearestLockOnTarget();
            ShowMessage(lockOnTarget ? "Objetivo fijado" : "No hay objetivo cerca", 0.75f);
        }

        private void ValidateLockOnTarget()
        {
            if (!lockOnTarget) return;

            float distance = Vector3.Distance(transform.position, lockOnTarget.transform.position);
            if (!lockOnTarget.IsAlive || distance > lockOnRange * 1.35f)
            {
                lockOnTarget = null;
            }
        }

        private DungeonEnemy3D FindNearestLockOnTarget()
        {
            int count = Physics.OverlapSphereNonAlloc(transform.position + Vector3.up * 0.8f, lockOnRange, lockOnHits);
            DungeonEnemy3D best = null;
            float bestScore = float.MaxValue;
            Vector3 referenceForward = cameraPivot ? Vector3.ProjectOnPlane(cameraPivot.forward, Vector3.up).normalized : transform.forward;

            for (int i = 0; i < count; i++)
            {
                DungeonEnemy3D enemy = lockOnHits[i].GetComponentInParent<DungeonEnemy3D>();
                ConsiderLockOnCandidate(enemy, referenceForward, ref best, ref bestScore);
            }

            if (!best)
            {
                foreach (DungeonEnemy3D enemy in Object.FindObjectsByType<DungeonEnemy3D>(FindObjectsInactive.Exclude))
                {
                    ConsiderLockOnCandidate(enemy, referenceForward, ref best, ref bestScore);
                }
            }

            return best;
        }

        private void ConsiderLockOnCandidate(DungeonEnemy3D enemy, Vector3 referenceForward, ref DungeonEnemy3D best, ref float bestScore)
        {
            if (!enemy || !enemy.IsAlive) return;

            Vector3 toEnemy = enemy.transform.position - transform.position;
            toEnemy.y = 0f;
            float distance = toEnemy.magnitude;
            if (distance < 0.05f || distance > lockOnRange) return;

            Vector3 direction = toEnemy / distance;
            float angle = referenceForward.sqrMagnitude > 0.001f ? Vector3.Angle(referenceForward, direction) : 0f;
            float score = distance + angle * 0.035f;
            if (score >= bestScore) return;

            best = enemy;
            bestScore = score;
        }

        private void UpdateInteraction()
        {
            currentInteractable = FindClosestInteractable();
            if (!Input.GetKeyDown(KeyCode.E)) return;

            if (currentInteractable) currentInteractable.Interact(this);
            else ShowMessage("No hay nada cerca para usar.", 1.1f);
        }

        private DungeonInteractable3D FindClosestInteractable()
        {
            int count = Physics.OverlapSphereNonAlloc(transform.position + Vector3.up * 0.8f, interactionRange, interactHits, ~0, QueryTriggerInteraction.Collide);
            DungeonInteractable3D closest = null;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                DungeonInteractable3D interactable = interactHits[i].GetComponentInParent<DungeonInteractable3D>();
                if (!interactable) continue;
                if (string.IsNullOrEmpty(interactable.GetPrompt(this))) continue;

                float distance = Vector3.Distance(transform.position, interactable.transform.position);
                if (distance >= closestDistance) continue;

                closest = interactable;
                closestDistance = distance;
            }

            return closest;
        }

        private void Respawn()
        {
            Health = MaxHealth;
            stamina = MaxStamina;
            deathHandled = false;
            velocity = Vector3.zero;
            PlanarVelocity = Vector3.zero;
            attackTimer = 0f;
            attackCharged = false;
            attackHitResolved = true;
            invulnerableTimer = 0f;
            parryTimer = 0f;
            lockOnTarget = null;
            controller.enabled = false;
            transform.position = respawnPoint;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
            controller.enabled = true;
            ShowMessage("La hoguera te devuelve al combate.", 2f);
        }

        private void HandleDeath()
        {
            if (deathHandled) return;

            deathHandled = true;
            if (Coins > 0)
            {
                DungeonSoulRemnant3D.Create(transform.position + Vector3.up * 0.2f, Coins);
                Coins = 0;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Vector3 direction = Application.isPlaying ? lastMoveDirection : transform.forward;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.9f + direction * attackReach, attackRadius);
        }
    }
}
