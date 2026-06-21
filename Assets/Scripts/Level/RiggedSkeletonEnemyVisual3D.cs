using UnityEngine;

namespace DungeonKnight.Level
{
    public class RiggedSkeletonEnemyVisual3D : MonoBehaviour
    {
        public const string ResourcePath = "Characters/Enemies/SkeletonEnemy/Rigged/SwordAndShieldSlashEsqueleto";
        private const string WalkResourcePath = "Characters/Enemies/SkeletonEnemy/Rigged/WalkingEsqueleto";

        [SerializeField] private float attackPlaybackSpeed = 1f;
        [SerializeField] private float walkPlaybackSpeed = 1f;
        [SerializeField] private float targetHeight = 1.9f;
        [SerializeField] private float moveBob = 0.04f;
        [SerializeField] private float moveSway = 5f;

        private Animator animator;
        private RiggedSkeletonAnimationGraph3D animationGraph;
        private Vector3 baseLocalPosition;
        private RiggedSkeletonSwordVisual3D swordVisual;
        private Transform hipsRoot;
        private Vector3 hipsBaseLocalPosition;
        private bool moving;

        public static bool TryAttach(Transform enemyRoot)
        {
            GameObject prefab = Resources.Load<GameObject>(ResourcePath);
            if (!prefab)
            {
                Debug.LogWarning($"RiggedSkeletonEnemyVisual3D: missing Resources/{ResourcePath}.fbx");
                return false;
            }

            GameObject visual = Instantiate(prefab, enemyRoot);
            visual.name = "Rigged Skeleton Visual";
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.identity;
            visual.transform.localScale = Vector3.one;

            RiggedSkeletonEnemyVisual3D controller = visual.AddComponent<RiggedSkeletonEnemyVisual3D>();

            Renderer[] renderers = RiggedSkeletonModelSetup3D.PrepareRenderers(visual);
            int rendererCount = renderers.Length;
            if (rendererCount == 0)
            {
                Object.Destroy(visual);
                return false;
            }

            if (!RiggedSkeletonModelSetup3D.FitToEnemyRoot(visual.transform, enemyRoot, renderers, controller.targetHeight))
            {
                Object.Destroy(visual);
                return false;
            }

            controller.Configure();
            controller.AttachRustySword();

            int clipCount = Resources.LoadAll<AnimationClip>(ResourcePath).Length;
            int walkClipCount = Resources.LoadAll<AnimationClip>(WalkResourcePath).Length;
            Bounds fittedBounds = RiggedSkeletonModelSetup3D.CalculateBounds(renderers);
            Debug.Log($"RiggedSkeletonEnemyVisual3D: attached {prefab.name}. Renderers={rendererCount}, AttackClips={clipCount}, WalkClips={walkClipCount}, Height={fittedBounds.size.y:0.00}, Scale={visual.transform.localScale.x:0.00}");
            return true;
        }

        public void SetMovement(bool value, Vector3 direction)
        {
            moving = value;
            if (value && direction.sqrMagnitude > 0.001f)
            {
                transform.localRotation = Quaternion.identity;
            }

            animationGraph?.SetMovement(value);
        }

        public float PlayAttack(Vector3 directionToPlayer)
        {
            return animationGraph?.PlayAttack() ?? 0f;
        }

        private void Configure()
        {
            baseLocalPosition = transform.localPosition;
            animator = GetComponentInChildren<Animator>();
            if (!animator) animator = gameObject.AddComponent<Animator>();
            animator.applyRootMotion = false;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            CacheAnimatedRoot();
            animationGraph = new RiggedSkeletonAnimationGraph3D($"{name} Animation Graph", animator, ResourcePath, WalkResourcePath, attackPlaybackSpeed, walkPlaybackSpeed);
        }

        private void AttachRustySword()
        {
            swordVisual = RiggedSkeletonSwordVisual3D.Attach(transform);
        }

        private void LateUpdate()
        {
            if (animationGraph is { IsAttackActive: true })
            {
                StabilizeAnimatedRoot();
            }

            animationGraph?.Tick();
            float bob = moving ? Mathf.Sin(Time.time * moveSway) * moveBob : 0f;
            transform.localPosition = baseLocalPosition + Vector3.up * bob;
            StabilizeAnimatedRoot();
            swordVisual?.Tick();
        }

        private void OnDestroy()
        {
            animationGraph?.Destroy();
            swordVisual?.Destroy();
        }

        private static Transform FindDeepChild(Transform root, string childName)
        {
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == childName) return child;
            }

            return null;
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
