using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace DungeonKnight.Level
{
    public class RiggedSkeletonEnemyVisual3D : MonoBehaviour
    {
        public const string ResourcePath = "Characters/Enemies/SkeletonEnemy/Rigged/SwordAndShieldSlashEsqueleto";
        private const string WalkResourcePath = "Characters/Enemies/SkeletonEnemy/Rigged/WalkingEsqueleto";
        private const string MiniBossModelResourcePath = "Characters/Enemies/MiniBoss2/Great Sword Casting";
        private const string MiniBossAttackResourcePath = "Characters/Enemies/MiniBoss2/Great Sword Slash";
        private const string MiniBossWalkResourcePath = "Characters/Enemies/MiniBoss2/Great Sword Walk";
        private const string MiniBossDeathResourcePath = "Characters/Enemies/MiniBoss2/Dying";
        private const int WalkInput = 0;
        private const int AttackInput = 1;
        private const int DeathInput = 2;

        [SerializeField] private float attackPlaybackSpeed = 1f;
        [SerializeField] private float walkPlaybackSpeed = 1f;
        [SerializeField] private float targetHeight = 1.9f;
        [SerializeField] private float moveBob = 0.04f;
        [SerializeField] private float moveSway = 5f;
        [SerializeField] private bool miniBossVisual;
        private static readonly Color BoneColor = new Color(0.78f, 0.73f, 0.58f);
        private static readonly Color ArmorColor = new Color(0.17f, 0.15f, 0.13f);
        private static readonly Color ArmorTrimColor = new Color(0.45f, 0.3f, 0.16f);
        private static readonly Color ClothColor = new Color(0.16f, 0.16f, 0.12f);
        private static readonly Color ZoneRustColor = new Color(0.34f, 0.13f, 0.07f);
        private static readonly Color RustBladeColor = new Color(0.38f, 0.35f, 0.3f);
        private static readonly Color RustPatchColor = new Color(0.42f, 0.16f, 0.08f);
        private static readonly Color DarkGripColor = new Color(0.14f, 0.1f, 0.07f);
        private static readonly Color EyeGlowColor = new Color(1f, 0.08f, 0.03f);

        private Animator animator;
        private AnimationClip attackClip;
        private AnimationClip walkClip;
        private AnimationClip deathClip;
        private PlayableGraph graph;
        private AnimationMixerPlayable mixer;
        private AnimationClipPlayable attackPlayable;
        private AnimationClipPlayable walkPlayable;
        private AnimationClipPlayable deathPlayable;
        private Vector3 baseLocalPosition;
        private Transform rightHand;
        private Transform rustySword;
        private Transform hipsRoot;
        private Vector3 hipsBaseLocalPosition;
        private float attackUntil;
        private float deathUntil;
        private bool moving;
        private string attackResourcePath = ResourcePath;
        private string walkResourcePath = WalkResourcePath;
        private string deathResourcePath = string.Empty;

        public static bool TryAttach(Transform enemyRoot)
        {
            return TryAttach(enemyRoot, ResourcePath, ResourcePath, WalkResourcePath, string.Empty, 1.9f, false);
        }

        public static bool TryAttachMiniBoss(Transform enemyRoot)
        {
            return TryAttach(enemyRoot, MiniBossModelResourcePath, MiniBossAttackResourcePath, MiniBossWalkResourcePath, MiniBossDeathResourcePath, 3.4f, true);
        }

        private static bool TryAttach(Transform enemyRoot, string modelResourcePath, string attackResourcePath, string walkResourcePath, string deathResourcePath, float targetHeight, bool miniBoss)
        {
            GameObject prefab = Resources.Load<GameObject>(modelResourcePath);
            if (!prefab)
            {
                Debug.LogWarning($"RiggedSkeletonEnemyVisual3D: missing Resources/{modelResourcePath}.fbx");
                return false;
            }

            GameObject visual = Instantiate(prefab, enemyRoot);
            visual.name = miniBoss ? "Mini Boss Rigged Visual" : "Rigged Skeleton Visual";
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.identity;
            visual.transform.localScale = Vector3.one;

            RiggedSkeletonEnemyVisual3D controller = visual.AddComponent<RiggedSkeletonEnemyVisual3D>();
            controller.miniBossVisual = miniBoss;
            controller.attackResourcePath = attackResourcePath;
            controller.walkResourcePath = walkResourcePath;
            controller.deathResourcePath = deathResourcePath;
            controller.targetHeight = targetHeight;
            controller.moveBob = miniBoss ? 0.015f : controller.moveBob;
            controller.moveSway = miniBoss ? 3.2f : controller.moveSway;
            controller.walkPlaybackSpeed = miniBoss ? 0.9f : controller.walkPlaybackSpeed;

            Renderer[] renderers = visual.GetComponentsInChildren<Renderer>(true);
            if (miniBoss) ConfigureRendererBasics(renderers);
            else ApplyRendererPalette(renderers);

            int rendererCount = renderers.Length;
            if (rendererCount == 0)
            {
                Object.Destroy(visual);
                return false;
            }

            if (!controller.FitToEnemyRoot(enemyRoot, renderers))
            {
                Object.Destroy(visual);
                return false;
            }

            controller.Configure();
            if (!miniBoss)
            {
                controller.AttachRustySword();
            }

            int clipCount = Resources.LoadAll<AnimationClip>(attackResourcePath).Length;
            int walkClipCount = Resources.LoadAll<AnimationClip>(walkResourcePath).Length;
            Bounds fittedBounds = CalculateBounds(renderers);
            Debug.Log($"RiggedSkeletonEnemyVisual3D: attached {prefab.name}. MiniBoss={miniBoss}, Renderers={rendererCount}, AttackClips={clipCount}, WalkClips={walkClipCount}, Height={fittedBounds.size.y:0.00}, Scale={visual.transform.localScale.x:0.00}");
            return true;
        }

        public static bool EnsureMiniBossVisual(Transform enemyRoot)
        {
            RiggedSkeletonEnemyVisual3D existing = enemyRoot.GetComponentInChildren<RiggedSkeletonEnemyVisual3D>(true);
            if (existing)
            {
                if (!existing.miniBossVisual)
                {
                    DestroySafely(existing.gameObject);
                    return TryAttachMiniBoss(enemyRoot);
                }

                existing.RepairRuntimeSetup();
                return existing.HasVisibleBodyRenderer();
            }

            return TryAttachMiniBoss(enemyRoot);
        }

        private static void ApplyRendererPalette(Renderer[] renderers)
        {
            ConfigureRendererBasics(renderers);
            foreach (Renderer renderer in renderers)
            {
                renderer.material = NewSkeletonZoneMaterial(renderer);
            }
        }

        private static void ConfigureRendererBasics(Renderer[] renderers)
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = true;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                renderer.receiveShadows = true;

                if (renderer is SkinnedMeshRenderer skinnedMeshRenderer)
                {
                    skinnedMeshRenderer.updateWhenOffscreen = true;
                }
            }
        }

        public void SetMovement(bool value, Vector3 direction)
        {
            moving = value;
            if (value && direction.sqrMagnitude > 0.001f)
            {
                transform.localRotation = Quaternion.identity;
            }

            if (!IsAttackActive())
            {
                UpdateLocomotionPlayable();
            }
        }

        public float PlayAttack(Vector3 directionToPlayer)
        {
            if (!attackClip || !graph.IsValid()) return 0f;

            attackPlayable.SetTime(0d);
            attackPlayable.SetSpeed(attackPlaybackSpeed);
            float duration = attackClip.length / Mathf.Max(0.01f, attackPlaybackSpeed);
            attackUntil = Time.time + duration;
            if (mixer.IsValid())
            {
                mixer.SetInputWeight(WalkInput, 0f);
                mixer.SetInputWeight(AttackInput, 1f);
            }

            if (walkPlayable.IsValid())
            {
                walkPlayable.SetSpeed(0d);
            }

            return duration;
        }

        public void RepairRuntimeSetup()
        {
            if (miniBossVisual)
            {
                attackResourcePath = MiniBossAttackResourcePath;
                walkResourcePath = MiniBossWalkResourcePath;
                deathResourcePath = MiniBossDeathResourcePath;
                targetHeight = 3.4f;
            }

            if (!animator) animator = GetComponentInChildren<Animator>(true);
            if (animator)
            {
                animator.applyRootMotion = false;
                animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            }

            foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true))
            {
                renderer.enabled = true;
                if (renderer is SkinnedMeshRenderer skinnedMeshRenderer)
                {
                    skinnedMeshRenderer.updateWhenOffscreen = true;
                }
            }

            RemoveGlowingEyes();

            if (!graph.IsValid())
            {
                Configure();
            }

            if (miniBossVisual) return;

            if (!rightHand)
            {
                rightHand = FindDeepChild(transform, "mixamorig:RightHand") ?? FindDeepChild(transform, "RightHand");
            }

            if (!rustySword)
            {
                Transform owner = transform.parent ? transform.parent : transform;
                rustySword = FindDeepChild(owner, "Rusty Sword");
            }

            if (!rustySword && rightHand)
            {
                AttachRustySword();
            }
        }

        public float PlayDeath()
        {
            moving = false;
            if (!deathClip || !graph.IsValid() || !deathPlayable.IsValid()) return 1.2f;

            deathPlayable.SetTime(0d);
            deathPlayable.SetSpeed(1d);
            float duration = Mathf.Max(0.35f, deathClip.length);
            deathUntil = Time.time + duration;
            attackUntil = 0f;
            if (mixer.IsValid())
            {
                mixer.SetInputWeight(WalkInput, 0f);
                mixer.SetInputWeight(AttackInput, 0f);
                mixer.SetInputWeight(DeathInput, 1f);
            }

            if (walkPlayable.IsValid()) walkPlayable.SetSpeed(0d);
            if (attackPlayable.IsValid()) attackPlayable.SetSpeed(0d);
            return duration;
        }

        public bool HasVisibleBodyRenderer()
        {
            foreach (Renderer renderer in GetComponentsInChildren<Renderer>(true))
            {
                if (!renderer || !renderer.enabled) continue;
                if (rustySword && (renderer.transform == rustySword || renderer.transform.IsChildOf(rustySword))) continue;
                return true;
            }

            return false;
        }

        private void Configure()
        {
            baseLocalPosition = transform.localPosition;
            animator = GetComponentInChildren<Animator>();
            if (!animator) animator = gameObject.AddComponent<Animator>();
            animator.applyRootMotion = false;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            CacheAnimatedRoot();

            attackClip = ChooseClip(Resources.LoadAll<AnimationClip>(attackResourcePath));
            walkClip = ChooseClip(Resources.LoadAll<AnimationClip>(walkResourcePath));
            deathClip = string.IsNullOrEmpty(deathResourcePath) ? null : ChooseClip(Resources.LoadAll<AnimationClip>(deathResourcePath));
            if (walkClip) walkClip.wrapMode = WrapMode.Loop;
            if (!attackClip && !walkClip && !deathClip) return;

            graph = PlayableGraph.Create($"{name} Animation Graph");
            graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            mixer = AnimationMixerPlayable.Create(graph, 3);
            AnimationPlayableOutput output = AnimationPlayableOutput.Create(graph, "Rigged Skeleton Animation", animator);
            output.SetSourcePlayable(mixer);

            if (walkClip)
            {
                walkPlayable = AnimationClipPlayable.Create(graph, walkClip);
                graph.Connect(walkPlayable, 0, mixer, WalkInput);
                mixer.SetInputWeight(WalkInput, 1f);
                walkPlayable.SetSpeed(0d);
            }

            if (attackClip)
            {
                attackPlayable = AnimationClipPlayable.Create(graph, attackClip);
                graph.Connect(attackPlayable, 0, mixer, AttackInput);
                mixer.SetInputWeight(AttackInput, 0f);
                attackPlayable.SetSpeed(0d);
            }

            if (deathClip)
            {
                deathPlayable = AnimationClipPlayable.Create(graph, deathClip);
                graph.Connect(deathPlayable, 0, mixer, DeathInput);
                mixer.SetInputWeight(DeathInput, 0f);
                deathPlayable.SetSpeed(0d);
            }

            graph.Play();
        }

        private void AttachRustySword()
        {
            rightHand = FindDeepChild(transform, "mixamorig:RightHand") ?? FindDeepChild(transform, "RightHand");
            if (!rightHand)
            {
                Debug.LogWarning("RiggedSkeletonEnemyVisual3D: RightHand bone not found, rusty sword skipped.");
                return;
            }

            Material bladeMaterial = NewMaterial("Skeleton Rusty Blade", RustBladeColor, 0.2f);
            Material rustMaterial = NewMaterial("Skeleton Rust Patches", RustPatchColor, 0.05f);
            Material gripMaterial = NewMaterial("Skeleton Dark Grip", DarkGripColor, 0.15f);

            GameObject swordRootObject = new GameObject("Rusty Sword");
            rustySword = swordRootObject.transform;
            rustySword.SetParent(transform.parent != null ? transform.parent : transform, true);
            rustySword.localScale = Vector3.one;
            UpdateRustySwordTransform();

            GameObject blade = GameObject.CreatePrimitive(PrimitiveType.Cube);
            blade.name = "Pitted Blade";
            blade.transform.SetParent(rustySword, false);
            blade.transform.localPosition = new Vector3(0f, 0.43f, 0f);
            blade.transform.localScale = new Vector3(0.075f, 0.78f, 0.035f);
            blade.GetComponent<Renderer>().material = bladeMaterial;
            Object.Destroy(blade.GetComponent<Collider>());

            GameObject tip = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tip.name = "Chipped Tip";
            tip.transform.SetParent(rustySword, false);
            tip.transform.localPosition = new Vector3(0f, 0.84f, 0f);
            tip.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
            tip.transform.localScale = new Vector3(0.058f, 0.058f, 0.034f);
            tip.GetComponent<Renderer>().material = bladeMaterial;
            Object.Destroy(tip.GetComponent<Collider>());

            GameObject guard = GameObject.CreatePrimitive(PrimitiveType.Cube);
            guard.name = "Rusty Guard";
            guard.transform.SetParent(rustySword, false);
            guard.transform.localPosition = new Vector3(0f, 0.02f, 0f);
            guard.transform.localScale = new Vector3(0.34f, 0.05f, 0.06f);
            guard.GetComponent<Renderer>().material = rustMaterial;
            Object.Destroy(guard.GetComponent<Collider>());

            GameObject grip = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            grip.name = "Worn Grip";
            grip.transform.SetParent(rustySword, false);
            grip.transform.localPosition = new Vector3(0f, -0.13f, 0f);
            grip.transform.localScale = new Vector3(0.045f, 0.18f, 0.045f);
            grip.GetComponent<Renderer>().material = gripMaterial;
            Object.Destroy(grip.GetComponent<Collider>());

            AddRustPatch(rustySword, new Vector3(-0.024f, 0.28f, -0.019f), new Vector3(0.044f, 0.12f, 0.006f), rustMaterial);
            AddRustPatch(rustySword, new Vector3(0.022f, 0.58f, -0.019f), new Vector3(0.04f, 0.1f, 0.006f), rustMaterial);
        }

        private void RemoveGlowingEyes()
        {
            foreach (Transform child in GetComponentsInChildren<Transform>(true))
            {
                if (child == transform) continue;
                if (child.name == "Left Red Eye" || child.name == "Right Red Eye")
                {
                    DestroySafely(child.gameObject);
                }
            }

            Transform head = FindDeepChild(transform, "mixamorig:Head") ?? FindDeepChild(transform, "Head");
            if (!head) return;

            foreach (Light light in head.GetComponents<Light>())
            {
                if (ApproximatelySameColor(light.color, EyeGlowColor))
                {
                    DestroySafely(light);
                }
            }
        }

        private bool FitToEnemyRoot(Transform enemyRoot, Renderer[] renderers)
        {
            Bounds bounds = CalculateBounds(renderers);
            if (bounds.size.y <= 0.001f)
            {
                return false;
            }

            float scaleMultiplier = targetHeight / bounds.size.y;
            transform.localScale *= scaleMultiplier;

            bounds = CalculateBounds(renderers);
            Vector3 offset = transform.position - bounds.center;
            offset.y = enemyRoot.position.y - 1f - bounds.min.y;
            transform.position += offset;
            return true;
        }

        private void LateUpdate()
        {
            if (IsAttackActive())
            {
                StabilizeAnimatedRoot();
            }

            if (graph.IsValid() && Time.time >= attackUntil && attackPlayable.IsValid() && attackPlayable.GetSpeed() > 0d)
            {
                attackPlayable.SetSpeed(0d);
                attackPlayable.SetTime(0d);
                UpdateLocomotionPlayable();
            }

            LoopWalkPlayable();
            float bob = moving ? Mathf.Sin(Time.time * moveSway) * moveBob : 0f;
            transform.localPosition = baseLocalPosition + Vector3.up * bob;
            StabilizeAnimatedRoot();
            UpdateRustySwordTransform();
        }

        private void OnDestroy()
        {
            if (graph.IsValid()) graph.Destroy();
            if (rustySword) Object.Destroy(rustySword.gameObject);
        }

        private static AnimationClip ChooseClip(AnimationClip[] clips)
        {
            if (clips == null) return null;

            AnimationClip fallback = null;
            foreach (AnimationClip clip in clips)
            {
                if (!clip || clip.length <= 0.05f) continue;
                if (!clip.name.StartsWith("__preview__", System.StringComparison.OrdinalIgnoreCase)) return clip;
                fallback ??= clip;
            }

            return fallback;
        }

        private static Bounds CalculateBounds(Renderer[] renderers)
        {
            Bounds bounds = default;
            bool hasBounds = false;
            foreach (Renderer renderer in renderers)
            {
                if (!renderer) continue;

                if (!hasBounds)
                {
                    bounds = renderer.bounds;
                    hasBounds = true;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            return hasBounds ? bounds : new Bounds(Vector3.zero, Vector3.zero);
        }

        private static void AddRustPatch(Transform parent, Vector3 position, Vector3 scale, Material material)
        {
            GameObject patch = GameObject.CreatePrimitive(PrimitiveType.Cube);
            patch.name = "Rust Patch";
            patch.transform.SetParent(parent, false);
            patch.transform.localPosition = position;
            patch.transform.localScale = scale;
            patch.GetComponent<Renderer>().material = material;
            Object.Destroy(patch.GetComponent<Collider>());
        }

        private static Material NewSkeletonZoneMaterial(Renderer renderer)
        {
            Shader shader = Shader.Find("DungeonKnight/SkeletonZoneTint");
            if (!shader)
            {
                return NewMaterial("Skeleton Aged Bone", BoneColor, 0f);
            }

            Material material = new Material(shader);
            material.name = "Skeleton Zone Armor Tint";
            material.SetColor("_Color", Color.white);
            Bounds bounds = GetRendererLocalBounds(renderer);
            float minY = bounds.min.y;
            float maxY = bounds.max.y;
            if (maxY - minY < 0.001f)
            {
                minY = -1f;
                maxY = 1f;
            }

            material.SetColor("_BoneColor", BoneColor);
            material.SetColor("_ArmorColor", ArmorColor);
            material.SetColor("_TrimColor", ArmorTrimColor);
            material.SetColor("_RustColor", ZoneRustColor);
            material.SetColor("_ClothColor", ClothColor);
            material.SetFloat("_MinY", minY);
            material.SetFloat("_MaxY", maxY);
            material.SetFloat("_ArmorStart", 0.44f);
            material.SetFloat("_ArmorEnd", 0.72f);
            material.SetFloat("_HelmetStart", 0.81f);
            material.SetFloat("_BeltCenter", 0.39f);
            return material;
        }

        private static Bounds GetRendererLocalBounds(Renderer renderer)
        {
            if (renderer is SkinnedMeshRenderer skinnedMeshRenderer)
            {
                return skinnedMeshRenderer.localBounds;
            }

            MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
            if (meshFilter && meshFilter.sharedMesh)
            {
                return meshFilter.sharedMesh.bounds;
            }

            return new Bounds(Vector3.zero, new Vector3(1f, 2f, 1f));
        }

        private static Material NewMaterial(string name, Color color, float metallic)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (!shader) shader = Shader.Find("Standard");
            if (!shader) shader = Shader.Find("Diffuse");
            Material material = new Material(shader);
            material.name = name;
            SetMaterialColor(material, color);
            if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", metallic);
            if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", 0.18f);
            if (material.HasProperty("_Glossiness")) material.SetFloat("_Glossiness", 0.18f);
            return material;
        }

        private static void SetMaterialColor(Material material, Color color)
        {
            if (!material) return;

            material.color = color;
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Color")) material.SetColor("_Color", color);
        }

        private static bool ApproximatelySameColor(Color a, Color b)
        {
            return Mathf.Abs(a.r - b.r) < 0.02f &&
                Mathf.Abs(a.g - b.g) < 0.02f &&
                Mathf.Abs(a.b - b.b) < 0.02f;
        }

        private static Transform FindDeepChild(Transform root, string childName)
        {
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == childName) return child;
            }

            return null;
        }

        private static void DestroySafely(Object target)
        {
            if (!target) return;

            if (Application.isPlaying) Object.Destroy(target);
            else Object.DestroyImmediate(target);
        }

        private void UpdateRustySwordTransform()
        {
            if (!rightHand || !rustySword) return;

            rustySword.position = rightHand.position;
            rustySword.rotation = rightHand.rotation * Quaternion.Euler(8f, 88f, -88f);
        }

        private bool IsAttackActive()
        {
            return graph.IsValid() && attackPlayable.IsValid() && Time.time < attackUntil && attackPlayable.GetSpeed() > 0d;
        }

        private void UpdateLocomotionPlayable()
        {
            if (!graph.IsValid() || !mixer.IsValid() || !walkPlayable.IsValid()) return;

            mixer.SetInputWeight(WalkInput, 1f);
            mixer.SetInputWeight(AttackInput, 0f);
            walkPlayable.SetSpeed(moving ? walkPlaybackSpeed : 0d);
            if (!moving)
            {
                walkPlayable.SetTime(0d);
            }
        }

        private void LoopWalkPlayable()
        {
            if (!moving || !walkClip || !walkPlayable.IsValid() || walkClip.length <= 0.01f) return;

            double time = walkPlayable.GetTime();
            if (time >= walkClip.length)
            {
                walkPlayable.SetTime(time % walkClip.length);
            }
        }

        private void CacheAnimatedRoot()
        {
            hipsRoot = FindDeepChild(transform, "mixamorig:Hips") ?? FindDeepChild(transform, "Hips");
            if (!hipsRoot) return;

            hipsBaseLocalPosition = hipsRoot.localPosition;
        }

        private void StabilizeAnimatedRoot()
        {
            if (!hipsRoot) return;

            Vector3 localPosition = hipsRoot.localPosition;
            localPosition.x = hipsBaseLocalPosition.x;
            localPosition.z = hipsBaseLocalPosition.z;
            hipsRoot.localPosition = localPosition;
        }
    }
}
