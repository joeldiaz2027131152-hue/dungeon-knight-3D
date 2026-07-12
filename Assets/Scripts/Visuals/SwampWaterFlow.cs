using UnityEngine;

namespace DungeonKnight.Visuals
{
    [ExecuteAlways]
    [RequireComponent(typeof(Renderer))]
    public sealed class SwampWaterFlow : MonoBehaviour
    {
        [SerializeField] private int materialIndex = 1;
        [SerializeField] private Vector2 mainTiling = new Vector2(3f, 6f);
        [SerializeField] private Vector2 normalTiling = new Vector2(3.5f, 7f);
        [SerializeField] private Vector2 flowSpeed = new Vector2(0.012f, 0.035f);
        [SerializeField] private float normalFlowMultiplier = 1.35f;

        private static readonly int MainTexST = Shader.PropertyToID("_MainTex_ST");
        private static readonly int BumpMapST = Shader.PropertyToID("_BumpMap_ST");

        private Renderer targetRenderer;
        private MaterialPropertyBlock propertyBlock;
        private Vector2 offset;
        private float lastTime;

        private void OnEnable()
        {
            targetRenderer = GetComponent<Renderer>();
            propertyBlock = new MaterialPropertyBlock();
            lastTime = Time.realtimeSinceStartup;
            ApplyOffset();
        }

        private void Update()
        {
            float now = Time.realtimeSinceStartup;
            float deltaTime = Mathf.Max(0f, now - lastTime);
            lastTime = now;

            offset += flowSpeed * deltaTime;
            offset.x = Mathf.Repeat(offset.x, 1f);
            offset.y = Mathf.Repeat(offset.y, 1f);

            ApplyOffset();
        }

        private void OnValidate()
        {
            materialIndex = Mathf.Max(0, materialIndex);
            mainTiling.x = Mathf.Max(0.01f, mainTiling.x);
            mainTiling.y = Mathf.Max(0.01f, mainTiling.y);
            normalTiling.x = Mathf.Max(0.01f, normalTiling.x);
            normalTiling.y = Mathf.Max(0.01f, normalTiling.y);
            normalFlowMultiplier = Mathf.Max(0f, normalFlowMultiplier);
        }

        private void ApplyOffset()
        {
            if (targetRenderer == null)
            {
                return;
            }

            targetRenderer.GetPropertyBlock(propertyBlock, materialIndex);
            propertyBlock.SetVector(MainTexST, new Vector4(mainTiling.x, mainTiling.y, offset.x, offset.y));
            propertyBlock.SetVector(
                BumpMapST,
                new Vector4(
                    normalTiling.x,
                    normalTiling.y,
                    offset.x * normalFlowMultiplier,
                    offset.y * normalFlowMultiplier));
            targetRenderer.SetPropertyBlock(propertyBlock, materialIndex);
        }
    }
}
