using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace DungeonKnight.Visuals
{
    public class SkeletonEnemyVisual : MonoBehaviour
    {
        public const string ResourcePath = "Characters/SkeletonEnemy/SwordAndShieldSlashEsqueleto";

        private Animator animator;
        private AnimationClip attackClip;
        private PlayableGraph graph;
        private AnimationClipPlayable attackPlayable;
        private float attackUntil;

        public static bool TryAttach(Transform enemyRoot)
        {
            if (enemyRoot.GetComponentInChildren<SkeletonEnemyVisual>()) return true;

            GameObject prefab = Resources.Load<GameObject>(ResourcePath);
            if (!prefab)
            {
                Debug.LogWarning($"SkeletonEnemyVisual: missing Resources/{ResourcePath}.fbx for {enemyRoot.name}");
                return false;
            }

            GameObject visual = Instantiate(prefab, enemyRoot);
            visual.name = "Skeleton Sword Slash Visual";
            visual.transform.localPosition = new Vector3(0f, -1f, 0f);
            visual.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            visual.transform.localScale = Vector3.one * 2f;

            SkeletonEnemyVisual controller = visual.AddComponent<SkeletonEnemyVisual>();
            controller.Configure();

            int rendererCount = visual.GetComponentsInChildren<Renderer>(true).Length;
            int clipCount = Resources.LoadAll<AnimationClip>(ResourcePath).Length;
            Debug.Log($"SkeletonEnemyVisual: attached {prefab.name} to {enemyRoot.name}. Renderers={rendererCount}, Clips={clipCount}");
            return true;
        }

        public bool HasClip => attackClip;

        public void PlayAttack()
        {
            if (!HasClip || !graph.IsValid()) return;

            attackPlayable.SetTime(0d);
            attackPlayable.SetSpeed(1d);
            attackUntil = Time.time + attackClip.length;
        }

        private void Configure()
        {
            animator = GetComponentInChildren<Animator>();
            if (!animator) animator = gameObject.AddComponent<Animator>();
            animator.applyRootMotion = false;
            animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;

            AnimationClip[] clips = Resources.LoadAll<AnimationClip>(ResourcePath);
            attackClip = ChooseClip(clips);
            if (!attackClip) return;

            graph = PlayableGraph.Create($"{name} Attack Graph");
            graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            attackPlayable = AnimationClipPlayable.Create(graph, attackClip);
            AnimationPlayableOutput output = AnimationPlayableOutput.Create(graph, "Skeleton Attack", animator);
            output.SetSourcePlayable(attackPlayable);
            graph.Play();
            attackPlayable.SetSpeed(0d);
        }

        private void Update()
        {
            if (!HasClip || !graph.IsValid()) return;

            if (Time.time >= attackUntil && attackPlayable.GetSpeed() > 0d)
            {
                attackPlayable.SetSpeed(0d);
                attackPlayable.SetTime(0d);
            }
        }

        private void OnDestroy()
        {
            if (graph.IsValid()) graph.Destroy();
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
    }
}
