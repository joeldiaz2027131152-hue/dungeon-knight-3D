using UnityEngine;

namespace DungeonKnight.Level
{
    [ExecuteAlways]
    public class DungeonKnight3DEditorPreviewRenderer : MonoBehaviour
    {
        [SerializeField] private Renderer[] targetRenderers;
        [SerializeField] private bool visibleDuringPlay = true;

        public void Configure(Renderer[] renderers, bool showWhenPlaying)
        {
            targetRenderers = renderers;
            visibleDuringPlay = showWhenPlaying;
            ApplyVisibility();
        }

        public void RestoreNow()
        {
            if (targetRenderers != null)
            {
                foreach (Renderer targetRenderer in targetRenderers)
                {
                    if (targetRenderer) targetRenderer.enabled = true;
                }
            }

            if (Application.isPlaying) Destroy(this);
            else DestroyImmediate(this);
        }

        private void OnEnable()
        {
            if (targetRenderers == null || targetRenderers.Length == 0)
            {
                targetRenderers = GetComponentsInChildren<Renderer>(true);
            }

            ApplyVisibility();
        }

        private void Update()
        {
            ApplyVisibility();
        }

        private void ApplyVisibility()
        {
            if (targetRenderers == null) return;

            bool visible = Application.isPlaying ? visibleDuringPlay : !visibleDuringPlay;
            foreach (Renderer targetRenderer in targetRenderers)
            {
                if (targetRenderer) targetRenderer.enabled = visible;
            }
        }
    }
}
