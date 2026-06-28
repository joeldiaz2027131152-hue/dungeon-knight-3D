using UnityEngine;

namespace DungeonKnight.Level
{
    internal sealed class DungeonKnight3DGeometryBuilder
    {
        private readonly DungeonKnight3DAssets assets;

        public DungeonKnight3DGeometryBuilder(DungeonKnight3DAssets assets)
        {
            this.assets = assets;
        }

        public static GameObject CreateBox(string name, Vector3 position, Vector3 scale, Material material)
        {
            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.name = name;
            box.transform.position = position;
            box.transform.localScale = scale;
            Renderer renderer = box.GetComponent<Renderer>();
            renderer.sharedMaterial = material;
            DungeonKnight3DAssets.ApplyBoxTextureTiling(renderer, scale);
            return box;
        }

        public void CreateColumn(Vector3 position)
        {
            GameObject column = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            column.name = "Stone Column";
            column.transform.position = position;
            column.transform.localScale = new Vector3(0.55f, 1.7f, 0.55f);
            column.GetComponent<Renderer>().sharedMaterial = assets.DarkStone;
        }

        public void CreateTorch(Vector3 position)
        {
            CreateBox("Wall Torch", position, new Vector3(0.18f, 0.55f, 0.18f), assets.Brass);
            GameObject flame = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            flame.name = "Torch Flame";
            flame.transform.position = position + Vector3.up * 0.42f;
            flame.transform.localScale = new Vector3(0.35f, 0.45f, 0.35f);
            flame.GetComponent<Renderer>().sharedMaterial = assets.Ember;

            Light light = flame.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1f, 0.48f, 0.18f);
            light.intensity = 2.1f;
            light.range = 6f;
        }

        public void CreateSpikeRow(Vector3 origin, int count)
        {
            for (int i = 0; i < count; i++)
            {
                CreateSpikeTip(origin + new Vector3((i - count * 0.5f) * 0.62f, 0.22f, 0f), 0.36f, 0.46f);
            }
        }

        private void CreateSpikeTip(Vector3 position, float baseWidth, float height)
        {
            GameObject spike = new GameObject("Buried Spike Tip");
            spike.transform.position = position;

            float half = baseWidth * 0.5f;
            Mesh mesh = new Mesh
            {
                name = "Buried Spike Tip Mesh",
                vertices = new[]
                {
                    new Vector3(-half, -height * 0.5f, -half),
                    new Vector3(half, -height * 0.5f, -half),
                    new Vector3(half, -height * 0.5f, half),
                    new Vector3(-half, -height * 0.5f, half),
                    new Vector3(0f, height * 0.5f, 0f)
                },
                triangles = new[]
                {
                    0, 4, 1,
                    1, 4, 2,
                    2, 4, 3,
                    3, 4, 0,
                    0, 1, 2,
                    0, 2, 3
                }
            };
            mesh.RecalculateNormals();
            spike.AddComponent<MeshFilter>().sharedMesh = mesh;
            spike.AddComponent<MeshRenderer>().sharedMaterial = assets.ChestIron;
        }
    }
}
