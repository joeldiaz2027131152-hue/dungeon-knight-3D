using UnityEngine;

namespace DungeonKnight.Level
{
    [ExecuteAlways]
    public class DungeonKnight3DEditorPreviewLight : MonoBehaviour
    {
        [SerializeField] private Light targetLight;

        public void Configure(Light light)
        {
            targetLight = light;
            ApplyVisibility();
        }

        private void OnEnable()
        {
            if (!targetLight) targetLight = GetComponent<Light>();
            ApplyVisibility();
        }

        private void Update()
        {
            ApplyVisibility();
        }

        private void ApplyVisibility()
        {
            if (!targetLight) return;
            targetLight.enabled = Application.isPlaying;
        }
    }
}
