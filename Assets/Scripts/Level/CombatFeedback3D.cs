using UnityEngine;

namespace DungeonKnight.Level
{
    public class CombatFeedback3D : MonoBehaviour
    {
        private Vector3 velocity;
        private float lifetime = 0.35f;
        private float maxLifetime = 0.35f;
        private Renderer feedbackRenderer;

        public static void Spawn(Vector3 position, Color color, int count)
        {
            int finalCount = Mathf.Clamp(count, 1, 14);
            Material material = NewFeedbackMaterial(color);
            for (int i = 0; i < finalCount; i++)
            {
                GameObject spark = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                spark.name = "Combat Spark";
                spark.transform.position = position + Random.insideUnitSphere * 0.12f;
                spark.transform.localScale = Vector3.one * Random.Range(0.025f, 0.055f);
                spark.GetComponent<Renderer>().material = material;
                Object.Destroy(spark.GetComponent<Collider>());

                CombatFeedback3D feedback = spark.AddComponent<CombatFeedback3D>();
                Vector3 direction = Random.onUnitSphere;
                direction.y = Mathf.Abs(direction.y) + 0.25f;
                feedback.velocity = direction.normalized * Random.Range(0.9f, 2.4f);
            }
        }

        private void Awake()
        {
            feedbackRenderer = GetComponent<Renderer>();
        }

        private void Update()
        {
            lifetime -= Time.deltaTime;
            transform.position += velocity * Time.deltaTime;
            velocity += Physics.gravity * 0.35f * Time.deltaTime;

            float t = Mathf.Clamp01(lifetime / maxLifetime);
            transform.localScale *= 1f - Time.deltaTime * 1.7f;
            if (feedbackRenderer)
            {
                Color color = feedbackRenderer.material.color;
                color.a = t;
                feedbackRenderer.material.color = color;
            }

            if (lifetime <= 0f)
            {
                Destroy(gameObject);
            }
        }

        private static Material NewFeedbackMaterial(Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (!shader) shader = Shader.Find("Standard");
            if (!shader) shader = Shader.Find("Diffuse");

            Material material = new Material(shader);
            material.name = "Combat Feedback Spark";
            material.color = color;
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            if (material.HasProperty("_EmissionColor"))
            {
                material.SetColor("_EmissionColor", color * 1.6f);
                material.EnableKeyword("_EMISSION");
            }

            return material;
        }
    }
}
