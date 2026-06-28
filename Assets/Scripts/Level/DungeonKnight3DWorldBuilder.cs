using UnityEngine;

namespace DungeonKnight.Level
{
    internal sealed class DungeonKnight3DWorldBuilder
    {
        private readonly DungeonKnight3DAssets assets;
        private readonly DungeonKnight3DGeometryBuilder geometry;

        public DungeonKnight3DWorldBuilder(DungeonKnight3DAssets assets)
        {
            this.assets = assets;
            geometry = new DungeonKnight3DGeometryBuilder(assets);
        }

        public void BuildWorldOneOneBase()
        {
            CreateDungeonShell();
            CreateTraversal();
            CreateSetDressing();
        }

        private void CreateDungeonShell()
        {
            CreateBox("Main Stone Floor", new Vector3(0f, -0.25f, 0f), new Vector3(15f, 0.5f, 44f), assets.Stone);
            CreateBox("Left Wall", new Vector3(-7.75f, 2.5f, 0f), new Vector3(0.5f, 5.5f, 44f), assets.DarkStone);
            CreateBox("Right Wall", new Vector3(7.75f, 2.5f, 0f), new Vector3(0.5f, 5.5f, 44f), assets.DarkStone);
            CreateBox("Rear Wall Left", new Vector3(-5.15f, 2.5f, 22f), new Vector3(4.7f, 5.5f, 0.5f), assets.DarkStone);
            CreateBox("Rear Wall Right", new Vector3(5.15f, 2.5f, 22f), new Vector3(4.7f, 5.5f, 0.5f), assets.DarkStone);
            CreateBox("Rear Wall Above Door", new Vector3(0f, 4.2f, 22f), new Vector3(5.6f, 2.1f, 0.5f), assets.DarkStone);
            CreateBox("Start Arch", new Vector3(0f, 3.8f, -20f), new Vector3(15f, 0.8f, 0.7f), assets.DarkStone);

            for (int i = 0; i < 8; i++)
            {
                float z = -17.5f + i * 5f;
                geometry.CreateColumn(new Vector3(-6.7f, 1.25f, z));
                geometry.CreateColumn(new Vector3(6.7f, 1.25f, z));
            }
        }

        private void CreateTraversal()
        {
            CreateBox("Raised Walkway", new Vector3(0f, 0.5f, -1f), new Vector3(5.4f, 0.55f, 8.5f), assets.Stone);
            CreateBox("Left Side Platform", new Vector3(-4.1f, 1.4f, 7.8f), new Vector3(3.6f, 0.45f, 4.5f), assets.Stone);
            CreateBox("Right Side Platform", new Vector3(4.1f, 2.25f, 14.4f), new Vector3(3.6f, 0.45f, 4.5f), assets.Stone);
            CreateBox("Gate Landing", new Vector3(0f, 0.3f, 18.6f), new Vector3(7.2f, 0.45f, 3.2f), assets.Stone);

            for (int i = 0; i < 7; i++)
            {
                CreateBox($"Stair Step {i + 1}", new Vector3(-2.8f + i * 0.46f, 0.05f + i * 0.16f, 4.2f + i * 0.42f), new Vector3(1.6f, 0.32f, 0.55f), assets.Stone);
            }

            geometry.CreateSpikeRow(new Vector3(0f, 0.15f, -7.2f), 9);
            geometry.CreateSpikeRow(new Vector3(0f, 0.15f, 5.2f), 7);
        }

        private void CreateSetDressing()
        {
            for (int i = 0; i < 6; i++)
            {
                float z = -14f + i * 6f;
                geometry.CreateTorch(new Vector3(-7.35f, 2.1f, z));
                geometry.CreateTorch(new Vector3(7.35f, 2.1f, z + 2.4f));
            }

            CreateBox("Broken Statue Base", new Vector3(-4.8f, 0.35f, -12.8f), new Vector3(1.5f, 0.7f, 1.5f), assets.DarkStone);
            CreateBox("Broken Statue Torso", new Vector3(-4.8f, 1.15f, -12.8f), new Vector3(0.75f, 1.2f, 0.55f), assets.DarkStone);
            CreateBox("Blue Banner", new Vector3(7.45f, 2.2f, -4.4f), new Vector3(0.08f, 2f, 1.1f), DungeonKnight3DAssets.NewMaterial("DK3D Blue Banner", new Color(0.1f, 0.22f, 0.5f)));
            CreateBox("Red Banner", new Vector3(-7.45f, 2.2f, 8.5f), new Vector3(0.08f, 2f, 1.1f), DungeonKnight3DAssets.NewMaterial("DK3D Red Banner", new Color(0.48f, 0.04f, 0.08f)));
        }

        private static GameObject CreateBox(string name, Vector3 position, Vector3 scale, Material material)
        {
            return DungeonKnight3DGeometryBuilder.CreateBox(name, position, scale, material);
        }
    }
}
