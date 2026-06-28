using UnityEngine;

namespace DungeonKnight.Level
{
    [ExecuteAlways]
    [RequireComponent(typeof(SpriteRenderer))]
    public class AnimatedWallTorch3D : MonoBehaviour
    {
        private const int FrameCount = 8;
        private const int TextureWidth = 32;
        private const int TextureHeight = 48;
        private const float PixelsPerUnit = 64f;

        private static Sprite[] frames;
        private static Sprite glowSprite;

        private SpriteRenderer flameRenderer;
        private SpriteRenderer glowRenderer;
        private Light torchLight;
        private Vector3 windDirection = Vector3.right;
        private Vector3 baseLocalPosition;
        private Vector3 baseScale;
        private Vector3 glowBaseScale;
        private float seed;
        [SerializeField] private bool hideWhenNotPlaying;

        public void Configure(Light lightSource, Vector3 worldWindDirection, bool hideInEditorPreview = false)
        {
            torchLight = lightSource;
            windDirection = worldWindDirection.sqrMagnitude > 0.001f ? worldWindDirection.normalized : Vector3.right;
            hideWhenNotPlaying = hideInEditorPreview;
            ApplyEditorPreviewVisibility();
        }

        public void SetEditorPreviewHidden(bool hidden)
        {
            hideWhenNotPlaying = hidden;
            ApplyEditorPreviewVisibility();
        }

        private void Awake()
        {
            EnsureSprites();

            flameRenderer = GetComponent<SpriteRenderer>();
            flameRenderer.sprite = frames[0];
            flameRenderer.sortingOrder = 6;
            flameRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            flameRenderer.receiveShadows = false;

            Transform existingGlow = transform.Find("Torch Flame Glow");
            GameObject glow = existingGlow ? existingGlow.gameObject : new GameObject("Torch Flame Glow");
            if (!existingGlow) glow.transform.SetParent(transform, false);
            glow.transform.localPosition = new Vector3(0f, 0.12f, 0.03f);
            glow.transform.localScale = new Vector3(0.78f, 0.7f, 1f);
            glowRenderer = glow.GetComponent<SpriteRenderer>();
            if (!glowRenderer) glowRenderer = glow.AddComponent<SpriteRenderer>();
            glowRenderer.sprite = glowSprite;
            glowRenderer.sortingOrder = 5;
            glowRenderer.color = new Color(1f, 0.42f, 0.08f, 0.18f);
            glowRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            glowRenderer.receiveShadows = false;

            baseLocalPosition = transform.localPosition;
            baseScale = transform.localScale;
            glowBaseScale = glow.transform.localScale;
            seed = Random.Range(0f, 10f);
            ApplyEditorPreviewVisibility();
        }

        private void OnEnable()
        {
            ApplyEditorPreviewVisibility();
        }

        private void Update()
        {
            if (frames == null || frames.Length == 0) return;
            ApplyEditorPreviewVisibility();
            if (!Application.isPlaying) return;

            float time = Time.time + seed;
            int frameIndex = Mathf.FloorToInt(time * 11f) % frames.Length;
            flameRenderer.sprite = frames[frameIndex];

            float gust = Mathf.Sin(time * 3.8f) * 0.45f + Mathf.Sin(time * 7.3f) * 0.25f + Mathf.Sin(time * 11.6f) * 0.14f;
            float flicker = 0.82f + Mathf.Sin(time * 13.5f) * 0.07f + Mathf.Sin(time * 21.1f) * 0.04f;
            transform.localPosition = Vector3.Lerp(transform.localPosition, baseLocalPosition + windDirection * (gust * 0.026f), Time.deltaTime * 10f);
            transform.localRotation = Quaternion.Euler(0f, 0f, -gust * 6f);
            transform.localScale = baseScale * flicker;

            if (glowRenderer)
            {
                glowRenderer.color = new Color(1f, 0.42f, 0.08f, 0.12f + Mathf.Abs(gust) * 0.035f);
                glowRenderer.transform.localScale = glowBaseScale * (1f + Mathf.Abs(gust) * 0.05f);
            }

            if (torchLight)
            {
                torchLight.intensity = 1.35f + flicker * 0.38f + Mathf.Abs(gust) * 0.12f;
                torchLight.range = 4.6f + flicker * 0.35f;
            }
        }

        private void ApplyEditorPreviewVisibility()
        {
            bool visible = Application.isPlaying || !hideWhenNotPlaying;
            if (flameRenderer) flameRenderer.enabled = visible;
            if (glowRenderer) glowRenderer.enabled = visible;
            if (torchLight) torchLight.enabled = visible;
        }

        private static void EnsureSprites()
        {
            if (frames != null && frames.Length == FrameCount && glowSprite) return;

            frames = new Sprite[FrameCount];
            for (int i = 0; i < FrameCount; i++)
            {
                Texture2D texture = new Texture2D(TextureWidth, TextureHeight, TextureFormat.RGBA32, false);
                texture.name = $"Torch Wind Flame {i + 1}";
                texture.filterMode = FilterMode.Point;
                texture.wrapMode = TextureWrapMode.Clamp;

                Color[] pixels = new Color[TextureWidth * TextureHeight];
                for (int p = 0; p < pixels.Length; p++)
                {
                    pixels[p] = Color.clear;
                }

                float phase = i / (float)FrameCount * Mathf.PI * 2f;
                float lean = Mathf.Sin(phase) * 4.2f + 2.2f;
                DrawFlame(pixels, TextureWidth, TextureHeight, 16f, 4f, 39f, 11.5f, lean, new Color(1f, 0.23f, 0.02f, 1f), new Color(1f, 0.62f, 0.02f, 1f), phase);
                DrawFlame(pixels, TextureWidth, TextureHeight, 15.5f, 7f, 30f, 7.2f, lean * 0.72f, new Color(1f, 0.77f, 0.08f, 1f), new Color(1f, 0.95f, 0.38f, 1f), phase + 0.8f);
                DrawFlame(pixels, TextureWidth, TextureHeight, 14.5f, 9f, 20f, 3.8f, lean * 0.38f, new Color(1f, 0.96f, 0.62f, 1f), new Color(1f, 1f, 0.9f, 1f), phase + 1.4f);
                DrawEmbers(pixels, TextureWidth, TextureHeight, i, phase);

                texture.SetPixels(pixels);
                texture.Apply();
                frames[i] = Sprite.Create(texture, new Rect(0f, 0f, TextureWidth, TextureHeight), new Vector2(0.5f, 0.08f), PixelsPerUnit);
                frames[i].name = texture.name;
            }

            glowSprite = CreateGlowSprite();
        }

        private static void DrawFlame(Color[] pixels, int width, int height, float centerX, float baseY, float flameHeight, float maxWidth, float lean, Color edgeColor, Color hotColor, float phase)
        {
            for (int y = Mathf.FloorToInt(baseY); y < Mathf.CeilToInt(baseY + flameHeight); y++)
            {
                float n = (y - baseY) / flameHeight;
                if (n < 0f || n > 1f) continue;

                float taper = Mathf.Sin(n * Mathf.PI);
                float halfWidth = Mathf.Max(0.8f, maxWidth * taper * (1f - n * 0.28f));
                float center = centerX + lean * n * n + Mathf.Sin(n * 9f + phase) * 1.3f;
                for (int x = Mathf.FloorToInt(center - halfWidth - 1f); x <= Mathf.CeilToInt(center + halfWidth + 1f); x++)
                {
                    if (x < 0 || x >= width || y < 0 || y >= height) continue;

                    float distance = Mathf.Abs(x - center) / Mathf.Max(0.01f, halfWidth);
                    if (distance > 1f) continue;

                    float edge = Mathf.Clamp01(1f - distance);
                    Color color = Color.Lerp(edgeColor, hotColor, edge * 0.9f);
                    color.a = Mathf.Clamp01(edge * 1.4f);
                    BlendPixel(pixels, width, x, y, color);
                }
            }
        }

        private static void DrawEmbers(Color[] pixels, int width, int height, int frame, float phase)
        {
            for (int i = 0; i < 3; i++)
            {
                int x = Mathf.RoundToInt(13f + Mathf.Sin(phase + i * 1.7f) * 10f + frame % 2);
                int y = Mathf.RoundToInt(34f + i * 4f + Mathf.Cos(phase + i) * 3f);
                Color color = i == 0 ? new Color(1f, 0.72f, 0.12f, 0.9f) : new Color(1f, 0.34f, 0.05f, 0.72f);
                BlendPixel(pixels, width, x, y, color);
                BlendPixel(pixels, width, x + 1, y, color * 0.82f);
            }
        }

        private static Sprite CreateGlowSprite()
        {
            const int size = 48;
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.name = "Torch Soft Orange Glow";
            texture.filterMode = FilterMode.Bilinear;
            texture.wrapMode = TextureWrapMode.Clamp;

            Color[] pixels = new Color[size * size];
            Vector2 center = new Vector2(size * 0.5f, size * 0.45f);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center) / (size * 0.5f);
                    float alpha = Mathf.Clamp01(1f - distance);
                    alpha *= alpha * 0.65f;
                    pixels[y * size + x] = new Color(1f, 0.36f, 0.04f, alpha);
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();
            Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), PixelsPerUnit);
            sprite.name = texture.name;
            return sprite;
        }

        private static void BlendPixel(Color[] pixels, int width, int x, int y, Color color)
        {
            if (x < 0 || x >= width || y < 0) return;

            int index = y * width + x;
            if (index < 0 || index >= pixels.Length) return;

            Color current = pixels[index];
            float alpha = color.a + current.a * (1f - color.a);
            if (alpha <= 0.001f)
            {
                pixels[index] = Color.clear;
                return;
            }

            pixels[index] = new Color(
                (color.r * color.a + current.r * current.a * (1f - color.a)) / alpha,
                (color.g * color.a + current.g * current.a * (1f - color.a)) / alpha,
                (color.b * color.a + current.b * current.a * (1f - color.a)) / alpha,
                alpha);
        }
    }
}
