using DungeonKnight.Player;
using UnityEngine;

namespace DungeonKnight.Level
{
    [RequireComponent(typeof(CharacterController))]
    public class DungeonEnemy3D : MonoBehaviour
    {
        [SerializeField] private int maxHealth = 55;
        [SerializeField] private int touchDamage = 12;
        [SerializeField] private float moveSpeed = 2.6f;
        [SerializeField] private float attackRange = 1.85f;
        [SerializeField] private float preferredAttackDistance = 1.55f;
        [SerializeField] private float attackSpacingCorrection = 3.4f;
        [SerializeField] private float aggroRange = 10f;
        [SerializeField] private float attackWindup = 0.28f;
        [SerializeField] private float attackRecovery = 0.72f;
        [SerializeField] private float attackCooldownPadding = 0.18f;
        [SerializeField] private Color hitColor = new Color(1f, 0.45f, 0.28f);

        private CharacterController controller;
        private PlayerController3D player;
        private DungeonEnemyDamageFlash3D damageFlash;
        private Vector3 velocity;
        private int health;
        private float attackTimer;
        private float attackWindupTimer;
        private float activeAttackDuration;
        private bool attackImpactDone;
        private bool dropsKey;
        private float weaveOffset;
        private SkeletonEnemyVisual3D skeletonVisual;
        private RiggedSkeletonEnemyVisual3D riggedSkeletonVisual;

        public bool IsAlive => health > 0;

        public void Configure(PlayerController3D target, int hp, int damage, float speed, bool keyCarrier)
        {
            player = target;
            maxHealth = hp;
            touchDamage = damage;
            moveSpeed = speed;
            dropsKey = keyCarrier;
            health = maxHealth;
        }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            skeletonVisual = GetComponentInChildren<SkeletonEnemyVisual3D>();
            riggedSkeletonVisual = GetComponentInChildren<RiggedSkeletonEnemyVisual3D>();
            damageFlash = new DungeonEnemyDamageFlash3D(GetComponentsInChildren<Renderer>(), hitColor);

            health = maxHealth;
            weaveOffset = Random.value * 100f;
        }

        private void Update()
        {
            if (!player || health <= 0) return;

            attackTimer = Mathf.Max(0f, attackTimer - Time.deltaTime);
            attackWindupTimer = Mathf.Max(0f, attackWindupTimer - Time.deltaTime);
            damageFlash.Tick();

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
                        player.TakeDamage(touchDamage);
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
            if (health <= 0) return;

            health = Mathf.Max(0, health - amount);
            damageFlash.Flash(0.16f);
            Vector3 away = transform.position - sourcePosition;
            away.y = 0f;
            if (away.sqrMagnitude > 0.001f)
            {
                controller.Move(away.normalized * 0.28f);
            }

            if (health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            if (dropsKey && player) player.GiveGateKey();
            Destroy(gameObject);
        }
    }
}
