using UnityEngine;

namespace DungeonKnight.Level
{
    internal sealed class DungeonKnight3DTraversalBuilder
    {
        private readonly DungeonKnight3DAssets assets;

        public DungeonKnight3DTraversalBuilder(DungeonKnight3DAssets assets)
        {
            this.assets = assets;
        }

        public void CreateStairRun(string name, Vector3 start, int steps, Vector3 stepOffset, float width)
        {
            for (int i = 0; i < steps; i++)
            {
                Vector3 position = start + stepOffset * i;
                Vector3 scale = Mathf.Abs(stepOffset.x) > Mathf.Abs(stepOffset.z)
                    ? new Vector3(Mathf.Abs(stepOffset.x) + 0.35f, 0.28f, width)
                    : new Vector3(width, 0.28f, Mathf.Abs(stepOffset.z) + 0.35f);
                DungeonKnight3DGeometryBuilder.CreateBox($"{name} {i + 1}", position, scale, assets.Stone);
            }
        }

        public void CreateBridgeRailing(Vector3 origin, int count)
        {
            for (int i = 0; i < count; i++)
            {
                DungeonKnight3DGeometryBuilder.CreateBox("Stone Railing Post", origin + Vector3.forward * (i * 1.15f), new Vector3(0.28f, 1.15f, 0.28f), assets.DarkStone);
            }
        }

        public void CreateMovingPlatform(string name, Vector3 position, Vector3 scale, Vector3 travel, float speed)
        {
            GameObject platform = DungeonKnight3DGeometryBuilder.CreateBox(name, position, scale, assets.Stone);
            platform.AddComponent<DungeonMovingPlatform3D>().Configure(travel, speed);
        }

        public void CreateFireTrap(string name, Vector3 position, Vector3 scale)
        {
            GameObject fire = CreateHazardBox(name, position, scale, 16, "Llamas del castillo.");
            fire.GetComponent<Renderer>().sharedMaterial = assets.Ember;

            Light light = fire.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1f, 0.34f, 0.12f);
            light.intensity = 2f;
            light.range = 5f;
        }

        public void CreateBladeTrap(string name, Vector3 position, Vector3 spin, Vector3 bob)
        {
            GameObject blade = CreateHazardBox(name, position, new Vector3(3.6f, 0.18f, 0.42f), 22, "Una cuchilla te alcanza.");
            blade.GetComponent<Renderer>().sharedMaterial = assets.Hazard;
            blade.GetComponent<DungeonHazard3D>().Configure(22, "Una cuchilla te alcanza.", spin, bob, 2.1f);
        }

        public GameObject CreateHazardBox(string name, Vector3 position, Vector3 scale, int damage, string message)
        {
            GameObject box = DungeonKnight3DGeometryBuilder.CreateBox(name, position, scale, assets.Hazard);
            BoxCollider collider = box.GetComponent<BoxCollider>();
            collider.isTrigger = true;
            box.AddComponent<DungeonHazard3D>().Configure(damage, message, Vector3.zero, Vector3.zero, 1f);
            return box;
        }
    }
}
