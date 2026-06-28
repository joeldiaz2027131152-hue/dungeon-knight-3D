using UnityEngine;

namespace DungeonKnight.Level
{
    internal static class RiggedSkeletonModelSetup3D
    {
        private static readonly Color BoneColor = new Color(0.63f, 0.57f, 0.45f);
        private static readonly Color ArmorColor = new Color(0.105f, 0.11f, 0.115f);
        private static readonly Color ArmorTrimColor = new Color(0.43f, 0.39f, 0.32f);
        private static readonly Color ClothColor = new Color(0.045f, 0.043f, 0.04f);
        private static readonly Color RustColor = new Color(0.38f, 0.16f, 0.075f);

        public static Renderer[] PrepareRenderers(GameObject visual)
        {
            Renderer[] renderers = visual.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = true;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                renderer.receiveShadows = true;
                renderer.material = NewSkeletonZoneMaterial(renderer);

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

        private static Material NewSkeletonZoneMaterial(Renderer renderer)
        {
            Shader shader = Shader.Find("DungeonKnight/SkeletonZoneTint");
            if (!shader)
            {
                return DungeonKnight3DAssets.NewMaterial("Skeleton Aged Bone", BoneColor);
            }

            Material material = new Material(shader);
            material.name = "Skeleton Zone Armor Tint";
            material.SetColor("_Color", Color.white);
            material.SetColor("_BoneColor", BoneColor);
            material.SetColor("_ArmorColor", ArmorColor);
            material.SetColor("_TrimColor", ArmorTrimColor);
            material.SetColor("_RustColor", RustColor);
            material.SetColor("_ClothColor", ClothColor);

            Bounds bounds = GetRendererLocalBounds(renderer);
            float minY = bounds.min.y;
            float maxY = bounds.max.y;
            if (maxY - minY < 0.001f)
            {
                minY = -1f;
                maxY = 1f;
            }

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
    }
}
