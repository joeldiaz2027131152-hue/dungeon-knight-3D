using UnityEngine;

namespace DungeonKnight.Level
{
    internal sealed class DungeonEnemyDamageFlash3D
    {
        private readonly Renderer[] renderers;
        private readonly Color[] baseColors;
        private readonly Color hitColor;
        private float flashTimer;

        public DungeonEnemyDamageFlash3D(Renderer[] renderers, Color hitColor)
        {
            this.renderers = renderers;
            this.hitColor = hitColor;
            baseColors = new Color[renderers.Length];

            for (int i = 0; i < renderers.Length; i++)
            {
                baseColors[i] = renderers[i].material.color;
            }
        }

        public void Flash(float duration)
        {
            flashTimer = duration;
        }

        public void Tick()
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

            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.color = baseColors[i];
            }
        }
    }
}
