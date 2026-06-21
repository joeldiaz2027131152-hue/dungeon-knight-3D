using DungeonKnight.Player;
using UnityEngine;

namespace DungeonKnight.Level
{
    public class DungeonSoulRemnant3D : MonoBehaviour
    {
        private int souls;
        private Vector3 basePosition;
        private float bobSeed;

        public static void Create(Vector3 position, int amount)
        {
            if (amount <= 0) return;

            GameObject remnant = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            remnant.name = "Lost Souls";
            remnant.transform.position = position;
            remnant.transform.localScale = Vector3.one * 0.48f;
            Collider collider = remnant.GetComponent<Collider>();
            collider.isTrigger = true;

            Renderer renderer = remnant.GetComponent<Renderer>();
            renderer.material = NewSoulMaterial();

            Light light = remnant.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(0.52f, 0.78f, 1f);
            light.intensity = 1.4f;
            light.range = 3.8f;

            remnant.AddComponent<DungeonSoulRemnant3D>().Configure(amount);
        }

        private void Configure(int amount)
        {
            souls = amount;
            basePosition = transform.position;
            bobSeed = Random.Range(0f, 10f);
        }

        private void Update()
        {
            transform.Rotate(Vector3.up * (72f * Time.deltaTime), Space.World);
            transform.position = basePosition + Vector3.up * (Mathf.Sin(Time.time * 2.5f + bobSeed) * 0.12f);
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerController3D player = other.GetComponentInParent<PlayerController3D>();
            if (!player) return;

            player.RecoverSouls(souls);
            CombatFeedback3D.Spawn(transform.position + Vector3.up * 0.4f, new Color(0.52f, 0.78f, 1f), 12);
            Destroy(gameObject);
        }

        private static Material NewSoulMaterial()
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (!shader) shader = Shader.Find("Standard");
            if (!shader) shader = Shader.Find("Diffuse");

            Material material = new Material(shader);
            Color color = new Color(0.38f, 0.74f, 1f, 0.82f);
            material.name = "Lost Souls Glow";
            material.color = color;
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            if (material.HasProperty("_EmissionColor"))
            {
                material.SetColor("_EmissionColor", color * 1.8f);
                material.EnableKeyword("_EMISSION");
            }

            return material;
        }
    }
}
