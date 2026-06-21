using UnityEngine;

namespace DungeonKnight.Level
{
    internal static class RiggedSkeletonModelSetup3D
    {
        private static readonly Color BoneColor = new Color(0.86f, 0.82f, 0.68f);

        public static Renderer[] PrepareRenderers(GameObject visual)
        {
            Renderer[] renderers = visual.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = true;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                renderer.receiveShadows = true;
                if (renderer.material.HasProperty("_Color"))
                {
                    renderer.material.color = BoneColor;
                }

                if (renderer is SkinnedMeshRenderer skinnedMeshRenderer)
                {
                    skinnedMeshRenderer.updateWhenOffscreen = true;
                }
            }

            return renderers;
        }

        public static bool FitToEnemyRoot(Transform visualTransform, Transform enemyRoot, Renderer[] renderers, float targetHeight)
        {
            Bounds bounds = CalculateBounds(renderers);
            if (bounds.size.y <= 0.001f)
            {
                return false;
            }

            float scaleMultiplier = targetHeight / bounds.size.y;
            visualTransform.localScale *= scaleMultiplier;

            bounds = CalculateBounds(renderers);
            Vector3 offset = visualTransform.position - bounds.center;
            offset.y = enemyRoot.position.y - 1f - bounds.min.y;
            visualTransform.position += offset;
            return true;
        }

        public static Bounds CalculateBounds(Renderer[] renderers)
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
    }
}
