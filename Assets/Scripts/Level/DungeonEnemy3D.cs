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
        [SerializeField] private int minCoinReward = 5;
        [SerializeField] private int maxCoinReward = 8;
        [SerializeField] private GothicDoubleDoor3D wakeGate;
        [SerializeField] private bool waitForWakeGate;

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
        private float deathFinishAt;
        private float flashTimer;
        private float staggerTimer;
        private bool attackImpactDone;
        private bool dying;
        private bool miniBossVisualChecked;
        private bool wakeGateResolved;
        private bool awakened;
        [SerializeField] private bool dropsKey;
        [SerializeField] private bool dropsTowerKey;
        private float weaveOffset;
        private SkeletonEnemyVisual3D skeletonVisual;
        private RiggedSkeletonEnemyVisual3D riggedSkeletonVisual;
        public bool IsAlive => health > 0 && !dying;
        public float HealthFraction => maxHealth > 0 ? Mathf.Clamp01(health / (float)maxHealth) : 0f;
        public float PostureFraction => maxPosture > 0.001f ? Mathf.Clamp01(posture / maxPosture) : 0f;
        public bool IsStaggered => staggerTimer > 0f;
        public bool IsTowerMiniBossEnemy => IsTowerMiniBoss();
        public string DisplayName => IsTowerMiniBoss() ? "Prueba de la Torre" : name;

        public void Configure(PlayerController3D target, int hp, int damage, float speed, bool keyCarrier)
        {
            Configure(target, hp, damage, speed, keyCarrier, false, 5, 8);
        }

        public void Configure(PlayerController3D target, int hp, int damage, float speed, bool keyCarrier, bool towerKeyCarrier, int minReward, int maxReward)
        {
            player = target;
            maxHealth = hp;
            touchDamage = damage;
            moveSpeed = speed;
            dropsKey = keyCarrier;
            dropsTowerKey = towerKeyCarrier;
            minCoinReward = Mathf.Max(0, minReward);
            maxCoinReward = Mathf.Max(minCoinReward, maxReward);
            if (towerKeyCarrier)
            {
                ApplyTowerMiniBossTrialProfile();
            }

            health = maxHealth;
            posture = maxPosture;
            waitForWakeGate = dropsTowerKey || waitForWakeGate;
            EnsureTowerMiniBossVisual();
        }

        public void ConfigureWakeGate(GothicDoubleDoor3D gate)
        {
            wakeGate = gate;
            waitForWakeGate = gate != null;
            wakeGateResolved = gate != null;
            awakened = gate && gate.IsOpen;
        }

        public void ConfigureTowerKeyMiniBoss(PlayerController3D target)
        {
            if (target) player = target;
            dropsKey = false;
            dropsTowerKey = true;
            ApplyTowerMiniBossTrialProfile();
            health = maxHealth;
            posture = maxPosture;
            waitForWakeGate = true;
            miniBossVisualChecked = false;
            EnsureTowerMiniBossVisual();
        }

        private void ApplyTowerMiniBossTrialProfile()
        {
            maxHealth = 220;
            touchDamage = 18;
            moveSpeed = 1.55f;
            attackRange = 2.05f;
            preferredAttackDistance = 1.82f;
            attackSpacingCorrection = 1.1f;
            aggroRange = 9.5f;
            attackWindup = 0.72f;
            attackRecovery = 1.18f;
            attackCooldownPadding = 0.64f;
            maxPosture = 95f;
            minCoinReward = Mathf.Max(minCoinReward, 16);
            maxCoinReward = Mathf.Max(maxCoinReward, 24);
        }

        public void RepairRuntimeSetup(PlayerController3D target)
        {
            player = target;
            transform.localScale = Vector3.one;
            controller = GetComponent<CharacterController>();
            if (!controller) controller = gameObject.AddComponent<CharacterController>();
            controller.enabled = true;
            controller.height = 2f;
            controller.radius = 0.42f;
            controller.center = Vector3.zero;

            foreach (CapsuleCollider capsuleCollider in GetComponents<CapsuleCollider>())
            {
                if (Application.isPlaying) Destroy(capsuleCollider);
                else DestroyImmediate(capsuleCollider);
            }

            renderers = GetComponentsInChildren<Renderer>(true);
            skeletonVisual = GetComponentInChildren<SkeletonEnemyVisual3D>(true);
            EnsureTowerMiniBossVisual();
            if (IsTowerMiniBoss()) ApplyTowerMiniBossControllerProfile();
            riggedSkeletonVisual = GetComponentInChildren<RiggedSkeletonEnemyVisual3D>(true);
            riggedSkeletonVisual?.RepairRuntimeSetup();
            MeshRenderer rootRenderer = GetComponent<MeshRenderer>();
            if (rootRenderer && riggedSkeletonVisual) rootRenderer.enabled = riggedSkeletonVisual.HasVisibleBodyRenderer() == false;

            if (baseColors == null || baseColors.Length != renderers.Length)
            {
                baseColors = new Color[renderers.Length];
                for (int i = 0; i < renderers.Length; i++)
                {
                    baseColors[i] = ReadRendererColor(renderers[i]);
                }
            }

            if (health <= 0)
            {
                health = maxHealth;
                posture = maxPosture;
            }
        }

        private void Awake()
        {
            transform.localScale = Vector3.one;
            controller = GetComponent<CharacterController>();
            if (controller)
            {
                controller.height = 2f;
                controller.radius = 0.42f;
                controller.center = Vector3.zero;
            }

            spawnPosition = transform.position;
            spawnRotation = transform.rotation;
            renderers = GetComponentsInChildren<Renderer>();
            skeletonVisual = GetComponentInChildren<SkeletonEnemyVisual3D>();
            if (IsTowerMiniBoss())
            {
                ApplyTowerMiniBossTrialProfile();
            }

            EnsureTowerMiniBossVisual();
            riggedSkeletonVisual = GetComponentInChildren<RiggedSkeletonEnemyVisual3D>();
            riggedSkeletonVisual?.RepairRuntimeSetup();
            baseColors = new Color[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                baseColors[i] = ReadRendererColor(renderers[i]);
            }

            health = maxHealth;
            posture = maxPosture;
            weaveOffset = Random.value * 100f;
        }

        private void OnEnable()
        {
            EnsureTowerMiniBossVisual();
        }

        private void Start()
        {
            EnsureTowerMiniBossVisual();
        }

        private void Update()
        {
            if (dying)
            {
                SetVisualMovement(false, Vector3.zero);
                UpdateFlash();
                if (Time.time >= deathFinishAt)
                {
                    FinishDeath();
                }

                return;
            }

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

            if (ShouldWaitForWakeGate())
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

        private bool IsTowerMiniBoss()
        {
            return dropsTowerKey || name.Contains("Tower Key Mini Boss");
        }

        private void EnsureTowerMiniBossVisual()
        {
            if (!IsTowerMiniBoss() || miniBossVisualChecked) return;

            miniBossVisualChecked = true;
            if (!RiggedSkeletonEnemyVisual3D.EnsureMiniBossVisual(transform))
            {
                Debug.LogWarning($"{name}: could not attach mini boss visual.");
                return;
            }

            ApplyTowerMiniBossControllerProfile();
            renderers = GetComponentsInChildren<Renderer>(true);
            skeletonVisual = GetComponentInChildren<SkeletonEnemyVisual3D>(true);
            riggedSkeletonVisual = GetComponentInChildren<RiggedSkeletonEnemyVisual3D>(true);
            MeshRenderer rootRenderer = GetComponent<MeshRenderer>();
            if (rootRenderer) rootRenderer.enabled = riggedSkeletonVisual && riggedSkeletonVisual.HasVisibleBodyRenderer() ? false : true;
            RebuildBaseColors();
        }

        private void ApplyTowerMiniBossControllerProfile()
        {
            if (!controller) controller = GetComponent<CharacterController>();
            if (!controller) return;

            controller.height = 3.25f;
            controller.radius = 0.54f;
            controller.center = new Vector3(0f, 0.6f, 0f);
        }

        private bool ShouldWaitForWakeGate()
        {
            if (awakened) return false;
            if (!waitForWakeGate && !IsTowerMiniBoss()) return false;

            if (!wakeGateResolved)
            {
                wakeGate = FindWakeGate();
                wakeGateResolved = true;
            }

            if (!wakeGate) return false;
            if (!wakeGate.IsOpen && !PlayerHasCrossedWakeGate()) return true;

            awakened = true;
            player.QueueMessage("Prueba de la Torre: fija objetivo, rueda y castiga la pausa.", 2.6f, HudMessageIcon.Shield);
            return false;
        }

        private bool PlayerHasCrossedWakeGate()
        {
            if (!player || !wakeGate) return false;

            float gateZ = wakeGate.transform.position.z;
            return player.transform.position.z > gateZ + 1.15f;
        }

        private GothicDoubleDoor3D FindWakeGate()
        {
            GothicDoubleDoor3D namedGate = FindGateByName("Tower Chamber Entry Door");
            if (namedGate) return namedGate;

            GothicDoubleDoor3D bestGate = null;
            float bestDistance = float.MaxValue;
            foreach (GothicDoubleDoor3D gate in Object.FindObjectsByType<GothicDoubleDoor3D>(FindObjectsInactive.Include))
            {
                if (!gate || gate.transform.position.z > transform.position.z) continue;

                float distance = Vector3.Distance(transform.position, gate.transform.position);
                if (distance > 14f || distance >= bestDistance) continue;

                bestGate = gate;
                bestDistance = distance;
            }

            return bestGate;
        }

        private static GothicDoubleDoor3D FindGateByName(string gateName)
        {
            GameObject gateObject = GameObject.Find(gateName);
            return gateObject ? gateObject.GetComponent<GothicDoubleDoor3D>() : null;
        }

        private void RebuildBaseColors()
        {
            if (renderers == null)
            {
                baseColors = System.Array.Empty<Color>();
                return;
            }

            baseColors = new Color[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                baseColors[i] = ReadRendererColor(renderers[i]);
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
            if (health <= 0 || dying) return;

            awakened = true;
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
                BeginDeath();
                return;
            }

            if (posture <= 0f)
            {
                Stagger(charged ? 1.15f : 0.85f, sourcePosition, false);
            }
        }

        public void Stagger(float duration, Vector3 sourcePosition, bool parried)
        {
            if (health <= 0 || dying) return;

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
            dying = false;
            deathFinishAt = 0f;
            velocity = Vector3.zero;
            controller.enabled = false;
            transform.position = spawnPosition;
            transform.rotation = spawnRotation;
            controller.enabled = true;
            SetVisualMovement(false, Vector3.zero);
        }

        private void BeginDeath()
        {
            dying = true;
            health = 0;
            attackWindupTimer = 0f;
            attackTimer = 0f;
            staggerTimer = 0f;
            attackImpactDone = true;
            velocity = Vector3.zero;
            SetVisualMovement(false, Vector3.zero);
            float deathDuration = riggedSkeletonVisual ? riggedSkeletonVisual.PlayDeath() : 1.2f;
            deathFinishAt = Time.time + Mathf.Clamp(deathDuration, 0.65f, 4.5f);
            if (controller) controller.enabled = false;
        }

        private void FinishDeath()
        {
            if (player && maxCoinReward > 0) player.AddCoins(Random.Range(minCoinReward, maxCoinReward + 1));
            if (dropsKey && player) player.GiveGateKey();
            if (dropsTowerKey)
            {
                player?.QueueMessage("Prueba superada. La llave de la torre queda libre.", 2.6f, HudMessageIcon.Sword);
                SpawnTowerKeyPickup();
            }
            Destroy(gameObject);
        }

        private void SpawnTowerKeyPickup()
        {
            Vector3 position = new Vector3(transform.position.x, 0.62f, transform.position.z);
            GameObject pickup = new GameObject("Tower Ornate Key Pickup");
            pickup.transform.position = position;

            SphereCollider trigger = pickup.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.radius = 0.85f;

            DungeonPickup3D pickupScript = pickup.AddComponent<DungeonPickup3D>();
            pickupScript.ConfigureTowerKey();
            CreateTowerKeyVisual(pickup.transform);
        }

        private static void CreateTowerKeyVisual(Transform parent)
        {
            Material gold = NewMaterial("Tower Key Aged Gold", new Color(0.86f, 0.62f, 0.22f), 0.55f);
            Material darkGold = NewMaterial("Tower Key Dark Engraving", new Color(0.36f, 0.22f, 0.075f), 0.25f);
            Material brightGold = NewMaterial("Tower Key Bright Worn Edge", new Color(1f, 0.82f, 0.38f), 0.35f);

            GameObject visual = new GameObject("Ornate Tower Key Visual");
            visual.transform.SetParent(parent, false);
            visual.transform.localPosition = new Vector3(0f, 0.2f, 0f);
            visual.transform.localRotation = Quaternion.Euler(0f, 0f, -3f);
            visual.transform.localScale = Vector3.one * 0.82f;

            AddKeyCylinder("Long Golden Shaft", visual.transform, new Vector3(0f, 0f, 0f), new Vector3(0.055f, 0.62f, 0.055f), gold);
            AddKeyCylinder("Lower Knob", visual.transform, new Vector3(0f, -0.72f, 0f), new Vector3(0.1f, 0.055f, 0.1f), brightGold);
            AddKeyCylinder("Lower Collar", visual.transform, new Vector3(0f, -0.48f, 0f), new Vector3(0.13f, 0.055f, 0.13f), brightGold);
            AddKeyCylinder("Upper Collar", visual.transform, new Vector3(0f, 0.46f, 0f), new Vector3(0.15f, 0.065f, 0.15f), brightGold);

            AddKeyBox("Key Bit Stem", visual.transform, new Vector3(0.24f, -0.48f, 0f), new Vector3(0.42f, 0.08f, 0.08f), gold);
            AddKeyBox("Key Bit Upper Tooth", visual.transform, new Vector3(0.48f, -0.36f, 0f), new Vector3(0.22f, 0.08f, 0.08f), gold);
            AddKeyBox("Key Bit Lower Tooth", visual.transform, new Vector3(0.48f, -0.62f, 0f), new Vector3(0.18f, 0.08f, 0.08f), gold);
            AddKeyBox("Key Bit Notch Dark", visual.transform, new Vector3(0.37f, -0.5f, -0.045f), new Vector3(0.1f, 0.14f, 0.02f), darkGold);

            AddKeyCylinder("Ornate Head Center", visual.transform, new Vector3(0f, 0.83f, 0f), new Vector3(0.18f, 0.05f, 0.18f), gold);
            AddKeyCylinder("Ornate Head Left Loop", visual.transform, new Vector3(-0.28f, 0.87f, 0f), new Vector3(0.26f, 0.045f, 0.26f), gold);
            AddKeyCylinder("Ornate Head Right Loop", visual.transform, new Vector3(0.28f, 0.87f, 0f), new Vector3(0.26f, 0.045f, 0.26f), gold);
            AddKeyCylinder("Ornate Head Top Loop", visual.transform, new Vector3(0f, 1.13f, 0f), new Vector3(0.18f, 0.04f, 0.18f), gold);
            AddKeyBox("Filigree Left Curl", visual.transform, new Vector3(-0.24f, 0.91f, -0.05f), new Vector3(0.26f, 0.045f, 0.025f), darkGold);
            AddKeyBox("Filigree Right Curl", visual.transform, new Vector3(0.24f, 0.91f, -0.05f), new Vector3(0.26f, 0.045f, 0.025f), darkGold);
            AddKeyBox("Filigree Center Spear", visual.transform, new Vector3(0f, 0.95f, -0.05f), new Vector3(0.045f, 0.34f, 0.025f), brightGold);

            Light glow = parent.gameObject.AddComponent<Light>();
            glow.type = LightType.Point;
            glow.color = new Color(1f, 0.72f, 0.24f);
            glow.intensity = 1.1f;
            glow.range = 3.6f;
        }

        private static void AddKeyBox(string name, Transform parent, Vector3 localPosition, Vector3 localScale, Material material)
        {
            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.name = name;
            box.transform.SetParent(parent, false);
            box.transform.localPosition = localPosition;
            box.transform.localScale = localScale;
            box.GetComponent<Renderer>().sharedMaterial = material;
            DestroySafely(box.GetComponent<Collider>());
        }

        private static void AddKeyCylinder(string name, Transform parent, Vector3 localPosition, Vector3 localScale, Material material)
        {
            GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cylinder.name = name;
            cylinder.transform.SetParent(parent, false);
            cylinder.transform.localPosition = localPosition;
            cylinder.transform.localScale = localScale;
            cylinder.GetComponent<Renderer>().sharedMaterial = material;
            DestroySafely(cylinder.GetComponent<Collider>());
        }

        private static Material NewMaterial(string name, Color color, float metallic)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (!shader) shader = Shader.Find("Standard");
            Material material = new Material(shader);
            material.name = name;
            material.color = color;
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", metallic);
            if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", 0.28f);
            if (material.HasProperty("_Glossiness")) material.SetFloat("_Glossiness", 0.28f);
            return material;
        }

        private void UpdateFlash()
        {
            if (flashTimer > 0f)
            {
                flashTimer -= Time.deltaTime;
                foreach (Renderer renderer in renderers)
                {
                    SetRendererColor(renderer, hitColor);
                }

                return;
            }

            if (staggerTimer > 0f)
            {
                foreach (Renderer renderer in renderers)
                {
                    SetRendererColor(renderer, staggerColor);
                }

                return;
            }

            if (attackWindupTimer > 0f && !attackImpactDone)
            {
                float pulse = Mathf.PingPong(Time.time * 9f, 1f);
                Color color = Color.Lerp(telegraphColor, hitColor, pulse * 0.45f);
                foreach (Renderer renderer in renderers)
                {
                    SetRendererColor(renderer, color);
                }

                return;
            }

            if (renderers == null || baseColors == null) return;

            int colorCount = Mathf.Min(renderers.Length, baseColors.Length);
            for (int i = 0; i < colorCount; i++)
            {
                SetRendererColor(renderers[i], baseColors[i]);
            }
        }

        private static Color ReadRendererColor(Renderer renderer)
        {
            if (!renderer) return Color.white;

            Material material = Application.isPlaying ? renderer.material : renderer.sharedMaterial;
            return material ? material.color : Color.white;
        }

        private static void SetRendererColor(Renderer renderer, Color color)
        {
            if (!renderer) return;

            Material material = Application.isPlaying ? renderer.material : renderer.sharedMaterial;
            if (material) material.color = color;
        }

        private static void DestroySafely(Object target)
        {
            if (!target) return;

            if (Application.isPlaying)
            {
                Object.Destroy(target);
            }
            else
            {
                Object.DestroyImmediate(target);
            }
        }
    }
}
