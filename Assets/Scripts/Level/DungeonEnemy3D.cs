using DungeonKnight.Player;
using UnityEngine;

namespace DungeonKnight.Level
{
    [RequireComponent(typeof(CharacterController))]
    public class DungeonEnemy3D : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 55;
        [SerializeField] private int touchDamage = 14;
        [SerializeField] private float moveSpeed = 2.25f;
        [SerializeField] private float attackRange = 1.9f;
        [SerializeField] private float preferredAttackDistance = 1.65f;
        [SerializeField] private float attackSpacingCorrection = 2.8f;
        [SerializeField] private float aggroRange = 10f;
        [SerializeField] private float attackWindup = 0.42f;
        [SerializeField] private float attackRecovery = 0.86f;
        [SerializeField] private float attackCooldownPadding = 0.28f;
        [SerializeField] private float maxPosture = 115f;
        [SerializeField] private Color hitColor = new Color(1f, 0.45f, 0.28f);
        [SerializeField] private Color telegraphColor = new Color(1f, 0.72f, 0.18f);
        [SerializeField] private Color staggerColor = new Color(0.44f, 0.82f, 1f);
        [SerializeField] private int soulsReward = 8;

        private CharacterController controller;
        private PlayerController3D player;
        private Renderer[] renderers;
        private Color[] baseColors;
        private Vector3 spawnPosition;
        private Quaternion spawnRotation;
        private Vector3 velocity;
        private float posture;
        private int health;
        private float attackTimer;
        private float attackWindupTimer;
        private float activeAttackDuration;
        private float flashTimer;
        private float staggerTimer;
        private bool attackImpactDone;
        private bool dropsKey;
        private bool dropsTowerKey;
        private float weaveOffset;
        private SkeletonEnemyVisual3D skeletonVisual;
        private RiggedSkeletonEnemyVisual3D riggedSkeletonVisual;
        public bool IsAlive => health > 0;
        public float HealthFraction => maxHealth > 0 ? Mathf.Clamp01(health / (float)maxHealth) : 0f;
        public float PostureFraction => maxPosture > 0.001f ? Mathf.Clamp01(posture / maxPosture) : 0f;
        public bool IsStaggered => staggerTimer > 0f;

        public void Configure(PlayerController3D target, int hp, int damage, float speed, bool keyCarrier)
        {
            Configure(target, hp, damage, speed, keyCarrier, false, Mathf.Max(4, hp / 6));
        }

        public void Configure(PlayerController3D target, int hp, int damage, float speed, bool keyCarrier, bool towerKeyCarrier, int reward)
        {
            player = target;
            maxHealth = hp;
            touchDamage = damage;
            moveSpeed = speed;
            dropsKey = keyCarrier;
            dropsTowerKey = towerKeyCarrier;
            soulsReward = Mathf.Max(0, reward);
            health = maxHealth;
            posture = maxPosture;
        }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            spawnPosition = transform.position;
            spawnRotation = transform.rotation;
            renderers = GetComponentsInChildren<Renderer>();
            skeletonVisual = GetComponentInChildren<SkeletonEnemyVisual3D>();
            riggedSkeletonVisual = GetComponentInChildren<RiggedSkeletonEnemyVisual3D>();
            baseColors = new Color[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                baseColors[i] = renderers[i].material.color;
            }

            health = maxHealth;
            posture = maxPosture;
            weaveOffset = Random.value * 100f;
        }

        private void Update()
        {
            if (!player || health <= 0) return;

            attackTimer = Mathf.Max(0f, attackTimer - Time.deltaTime);
            attackWindupTimer = Mathf.Max(0f, attackWindupTimer - Time.deltaTime);
            staggerTimer = Mathf.Max(0f, staggerTimer - Time.deltaTime);
            UpdateFlash();

            if (staggerTimer > 0f)
            {
                velocity.y += Physics.gravity.y * Time.deltaTime;
                if (controller.isGrounded && velocity.y < 0f) velocity.y = -1f;
                SetVisualMovement(false, Vector3.zero);
                controller.Move(velocity * Time.deltaTime);
                return;
            }

            Vector3 toPlayer = player.transform.position - transform.position;
            toPlayer.y = 0f;
            float distance = toPlayer.magnitude;
            if (distance > aggroRange) return;

            Vector3 direction = distance > 0.05f ? toPlayer / distance : Vector3.zero;
            if (direction.sqrMagnitude > 0f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 8f * Time.deltaTime);
            }

            velocity.y += Physics.gravity.y * Time.deltaTime;
            if (controller.isGrounded && velocity.y < 0f) velocity.y = -1f;

            if (attackWindupTimer > 0f)
            {
                if (!attackImpactDone && attackWindupTimer <= activeAttackDuration - attackWindup)
                {
                    attackImpactDone = true;
                    if (distance <= attackRange + 0.35f)
                    {
                        player.TakeEnemyHit(touchDamage, this, transform.position);
                    }
                }

                Vector3 spacingMotion = Vector3.zero;
                if (distance < preferredAttackDistance && direction.sqrMagnitude > 0f)
                {
                    float spacingStrength = Mathf.Clamp01(preferredAttackDistance - distance);
                    spacingMotion = -direction * (attackSpacingCorrection * spacingStrength);
                }

                SetVisualMovement(false, Vector3.zero);
                controller.Move((spacingMotion + velocity) * Time.deltaTime);
                return;
            }

            Vector3 motion = distance > preferredAttackDistance ? GetPursuitDirection(direction, distance) * moveSpeed : Vector3.zero;
            controller.Move((motion + velocity) * Time.deltaTime);
            SetVisualMovement(motion.sqrMagnitude > 0.01f, motion);

            if (distance <= attackRange && attackTimer <= 0f)
            {
                StartAttack(direction);
            }
        }

        private void StartAttack(Vector3 direction)
        {
            float clipDuration = riggedSkeletonVisual ? riggedSkeletonVisual.PlayAttack(direction) : 0f;
            activeAttackDuration = Mathf.Max(attackWindup + attackRecovery, clipDuration);
            attackTimer = activeAttackDuration + attackCooldownPadding;
            attackWindupTimer = activeAttackDuration;
            attackImpactDone = false;
            skeletonVisual?.PlayAttack(direction);
        }

        private void SetVisualMovement(bool moving, Vector3 motion)
        {
            riggedSkeletonVisual?.SetMovement(moving, motion);
            skeletonVisual?.SetMovement(moving, motion);
        }

        private Vector3 GetPursuitDirection(Vector3 direction, float distance)
        {
            if (distance < attackRange + 1.8f)
            {
                Vector3 side = Vector3.Cross(Vector3.up, direction);
                float weave = Mathf.Sin(Time.time * 2.6f + weaveOffset) * 0.28f;
                return (direction + side * weave).normalized;
            }

            return direction;
        }

        public void TakeDamage(int amount, Vector3 sourcePosition)
        {
            TakeDamage(amount, sourcePosition, false);
        }

        public void TakeDamage(int amount, Vector3 sourcePosition, bool charged)
        {
            if (health <= 0) return;

            health = Mathf.Max(0, health - amount);
            flashTimer = 0.16f;
            posture = Mathf.Max(0f, posture - (charged ? 58f : 30f));
            Vector3 away = transform.position - sourcePosition;
            away.y = 0f;
            if (away.sqrMagnitude > 0.001f)
            {
                controller.Move(away.normalized * (charged ? 0.42f : 0.28f));
            }

            if (health <= 0)
            {
                Die();
                return;
            }

            if (posture <= 0f)
            {
                Stagger(charged ? 1.15f : 0.85f, sourcePosition, false);
            }
        }

        public void Stagger(float duration, Vector3 sourcePosition, bool parried)
        {
            if (health <= 0) return;

            posture = maxPosture;
            staggerTimer = Mathf.Max(staggerTimer, duration);
            attackWindupTimer = 0f;
            attackTimer = Mathf.Max(attackTimer, duration + 0.25f);
            attackImpactDone = true;
            flashTimer = Mathf.Max(flashTimer, 0.12f);
            Vector3 away = transform.position - sourcePosition;
            away.y = 0f;
            if (away.sqrMagnitude > 0.001f)
            {
                controller.Move(away.normalized * (parried ? 0.55f : 0.34f));
            }
        }

        public void RestoreAtBonfire()
        {
            if ((dropsKey || dropsTowerKey) && health <= 0) return;

            health = maxHealth;
            posture = maxPosture;
            attackTimer = 0f;
            attackWindupTimer = 0f;
            staggerTimer = 0f;
            flashTimer = 0f;
            attackImpactDone = false;
            velocity = Vector3.zero;
            controller.enabled = false;
            transform.position = spawnPosition;
            transform.rotation = spawnRotation;
            controller.enabled = true;
            SetVisualMovement(false, Vector3.zero);
        }

        private void Die()
        {
            if (player && soulsReward > 0) player.AddCoins(soulsReward);
            if (dropsKey && player) player.GiveGateKey();
            if (dropsTowerKey && player) player.GiveTowerKey();
            Destroy(gameObject);
        }

        private void UpdateFlash()
        {
            if (flashTimer > 0f)
            {
                flashTimer -= Time.deltaTime;
                foreach (Renderer renderer in renderers)
                {
                    renderer.material.color = hitColor;
                }

                return;
            }

            if (staggerTimer > 0f)
            {
                foreach (Renderer renderer in renderers)
                {
                    renderer.material.color = staggerColor;
                }

                return;
            }

            if (attackWindupTimer > 0f && !attackImpactDone)
            {
                float pulse = Mathf.PingPong(Time.time * 9f, 1f);
                Color color = Color.Lerp(telegraphColor, hitColor, pulse * 0.45f);
                foreach (Renderer renderer in renderers)
                {
                    renderer.material.color = color;
                }

                return;
            }

            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.color = baseColors[i];
            }
        }
    }
}
