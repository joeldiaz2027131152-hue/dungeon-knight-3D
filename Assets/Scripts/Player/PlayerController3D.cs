using DungeonKnight.Level;
using UnityEngine;

namespace DungeonKnight.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController3D : MonoBehaviour
    {
        public const float SharedFocusDistance = 24f;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 6.2f;
        [SerializeField] private float dashSpeed = 11f;
        [SerializeField] private float jumpSpeed = 7.4f;
        [SerializeField] private float gravity = -22f;
        [SerializeField] private float turnSpeed = 14f;

        [Header("Combat")]
        [SerializeField] private int lightAttackDamage = 20;
        [SerializeField] private int chargedAttackDamage = 44;
        [SerializeField] private float attackReach = 1.55f;
        [SerializeField] private float attackRadius = 0.95f;
        [SerializeField] private float attackCooldown = 0.36f;
        [SerializeField] private float chargedAttackThreshold = 0.58f;
        [SerializeField] private float blockMoveMultiplier = 0.42f;
        [SerializeField] private float lockOnRange = SharedFocusDistance;

        private readonly PlayerAttackResolver3D attackResolver = new PlayerAttackResolver3D();
        private readonly PlayerInteractionScanner3D interactionScanner = new PlayerInteractionScanner3D();
        private readonly PlayerStatusMessenger3D statusMessenger = new PlayerStatusMessenger3D();
        private readonly PlayerState3D state = new PlayerState3D();
        private PlayerMovementMotor3D movementMotor;
        private CharacterController controller;
        private Transform cameraPivot;
        private Transform lockOnTarget;
        private float stamina = 100f;
        private float attackTimer;
        private float chargeTimer;

        public int MaxHealth => state.MaxHealth;
        public int Health => state.Health;
        public int Coins => state.Coins;
        public int Potions => state.Potions;
        public bool HasGateKey => state.HasGateKey;
        public float Stamina => stamina;
        public float MaxStamina => 100f;
        public bool IsBlocking => movementMotor != null && movementMotor.IsBlocking;
        public bool IsDashing => movementMotor != null && movementMotor.IsDashing;
        public bool IsCharging => chargeTimer > 0f;
        public string StatusMessage => statusMessenger.Current;
        public Transform LockOnTarget => IsValidLockOnTarget(lockOnTarget) ? lockOnTarget : null;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            cameraPivot = Camera.main ? Camera.main.transform : null;
            movementMotor = new PlayerMovementMotor3D(moveSpeed, dashSpeed, jumpSpeed, gravity, turnSpeed, blockMoveMultiplier);
        }

        private void Update()
        {
            if (!cameraPivot && Camera.main) cameraPivot = Camera.main.transform;

            if (state.IsDead)
            {
                ShowMessage("Has caido. Pulsa R para volver a la hoguera.", 0.2f);
                if (Input.GetKeyDown(KeyCode.R)) Respawn();
                return;
            }

            attackTimer = Mathf.Max(0f, attackTimer - Time.deltaTime);

            UpdateLockOn();
            UpdateMovement();
            UpdateCombat();
            UpdateInteraction();

            if (!IsBlocking && stamina < MaxStamina)
            {
                stamina = Mathf.Min(MaxStamina, stamina + 24f * Time.deltaTime);
            }
        }

        public void AddCoins(int amount)
        {
            int awarded = state.AddCoins(amount);
            ShowMessage($"+{awarded} monedas antiguas", 1.8f);
        }

        public void AddPotion()
        {
            state.AddPotion();
            ShowMessage("Pocion recuperada", 1.8f);
        }

        public void RestAtBonfire(Vector3 bonfirePosition)
        {
            state.RestAtBonfire(bonfirePosition);
            stamina = MaxStamina;
            ShowMessage("Hoguera encendida. Punto de regreso actualizado.", 2.4f);
        }

        public void GiveGateKey()
        {
            state.GiveGateKey();
            ShowMessage("Llave del guardian obtenida", 2.4f);
        }

        public void TakeDamage(int amount)
        {
            if (state.IsDead || IsDashing) return;

            int finalAmount = state.TakeDamage(amount, IsBlocking);
            ShowMessage(IsBlocking ? "Bloqueo firme" : $"-{finalAmount} HP", 0.85f);
        }

        public void ShowMessage(string text, float duration)
        {
            statusMessenger.Show(text, duration);
        }

        private void UpdateMovement()
        {
            movementMotor.Tick(controller, transform, cameraPivot, ref stamina);
            FaceLockOnTarget();
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

            if (Input.GetKeyUp(KeyCode.J) && chargeTimer > 0f)
            {
                bool charged = chargeTimer >= chargedAttackThreshold && stamina >= 30f;
                if (charged) stamina -= 30f;
                Attack(charged);
                chargeTimer = 0f;
            }

            if (Input.GetKeyDown(KeyCode.Q) && state.UsePotion())
            {
                ShowMessage("Pocion usada", 1.4f);
            }
        }

        private void Attack(bool charged)
        {
            if (attackTimer > 0f) return;

            attackTimer = charged ? attackCooldown * 1.4f : attackCooldown;
            int damage = charged ? chargedAttackDamage : lightAttackDamage;
            Vector3 attackDirection = GetAttackDirection();
            Vector3 center = transform.position + Vector3.up * 0.9f + attackDirection * attackReach;
            bool hitSomething = attackResolver.TryHitEnemies(center, attackRadius, damage, transform.position);

            ShowMessage(hitSomething ? (charged ? "Golpe cargado" : "Corte limpio") : "La espada corta el aire", 0.75f);
        }

        private void UpdateLockOn()
        {
            if (!IsValidLockOnTarget(lockOnTarget))
            {
                lockOnTarget = null;
            }

            if (!Input.GetKeyDown(KeyCode.Tab)) return;

            if (lockOnTarget)
            {
                lockOnTarget = null;
                ShowMessage("Objetivo liberado", 0.9f);
                return;
            }

            lockOnTarget = FindClosestLockOnTarget();
            ShowMessage(lockOnTarget ? $"Fijado: {lockOnTarget.name}" : "No hay enemigos cerca", 1.1f);
        }

        private Transform FindClosestLockOnTarget()
        {
            DungeonEnemy3D[] enemies = Object.FindObjectsByType<DungeonEnemy3D>(FindObjectsInactive.Exclude);
            Transform closest = null;
            float closestDistance = lockOnRange;

            for (int i = 0; i < enemies.Length; i++)
            {
                DungeonEnemy3D enemy = enemies[i];
                if (!enemy || !enemy.IsAlive) continue;

                float distance = HorizontalDistanceTo(enemy.transform);
                if (distance >= closestDistance) continue;

                closest = enemy.transform;
                closestDistance = distance;
            }

            Collider[] nearbyHits = Physics.OverlapBox(transform.position + Vector3.up * 0.9f, new Vector3(lockOnRange, lockOnRange, lockOnRange));
            for (int i = 0; i < nearbyHits.Length; i++)
            {
                Transform candidate = nearbyHits[i].attachedRigidbody ? nearbyHits[i].attachedRigidbody.transform : nearbyHits[i].transform.root;
                if (!IsEnemyTarget(candidate)) continue;

                float distance = HorizontalDistanceTo(candidate);
                if (distance >= closestDistance) continue;

                closest = candidate;
                closestDistance = distance;
            }

            return closest;
        }

        private void FaceLockOnTarget()
        {
            Transform target = LockOnTarget;
            if (!target || IsDashing) return;

            Vector3 direction = target.position - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude <= 0.001f) return;

            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 18f * Time.deltaTime);
        }

        private Vector3 GetAttackDirection()
        {
            Transform target = LockOnTarget;
            if (!target) return movementMotor.LastMoveDirection;

            Vector3 direction = target.position - transform.position;
            direction.y = 0f;
            return direction.sqrMagnitude > 0.001f ? direction.normalized : movementMotor.LastMoveDirection;
        }

        private bool IsValidLockOnTarget(Transform target)
        {
            if (!target || !IsEnemyTarget(target)) return false;

            Vector3 delta = target.position - transform.position;
            float verticalDistance = Mathf.Abs(delta.y);
            delta.y = 0f;
            return verticalDistance <= lockOnRange && delta.sqrMagnitude <= lockOnRange * lockOnRange;
        }

        private bool IsEnemyTarget(Transform target)
        {
            if (!target) return false;

            DungeonEnemy3D enemy = target.GetComponentInParent<DungeonEnemy3D>();
            if (enemy) return enemy.IsAlive;

            return target.CompareTag("Enemy") || target.gameObject.layer == LayerMask.NameToLayer("Enemy");
        }

        private float HorizontalDistanceTo(Transform target)
        {
            Vector3 delta = target.position - transform.position;
            delta.y = 0f;
            return delta.magnitude;
        }

        private void UpdateInteraction()
        {
            if (!Input.GetKeyDown(KeyCode.E)) return;

            DungeonInteractable3D closest = interactionScanner.FindClosest(transform.position + Vector3.up * 0.8f, transform.position, 1.8f);

            if (closest) closest.Interact(this);
            else ShowMessage("No hay nada cerca para usar.", 1.1f);
        }

        private void Respawn()
        {
            state.RestoreVitals();
            stamina = MaxStamina;
            lockOnTarget = null;
            movementMotor.Reset();
            controller.enabled = false;
            transform.position = state.RespawnPoint;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
            controller.enabled = true;
            ShowMessage("La hoguera te devuelve al combate.", 2f);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Vector3 direction = Application.isPlaying && movementMotor != null ? movementMotor.LastMoveDirection : transform.forward;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.9f + direction * attackReach, attackRadius);
        }
    }
}
