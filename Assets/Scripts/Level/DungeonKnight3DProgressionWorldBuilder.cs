using DungeonKnight.Player;
using UnityEngine;

namespace DungeonKnight.Level
{
    internal sealed class DungeonKnight3DProgressionWorldBuilder
    {
        private readonly DungeonKnight3DAssets assets;
        private readonly DungeonKnight3DInteractableBuilder interactables;
        private readonly DungeonKnight3DEnemySpawner enemies;
        private readonly DungeonKnight3DGeometryBuilder geometry;
        private readonly DungeonKnight3DTraversalBuilder traversal;

        public DungeonKnight3DProgressionWorldBuilder(
            DungeonKnight3DAssets assets,
            DungeonKnight3DInteractableBuilder interactables,
            DungeonKnight3DEnemySpawner enemies)
        {
            this.assets = assets;
            this.interactables = interactables;
            this.enemies = enemies;
            geometry = new DungeonKnight3DGeometryBuilder(assets);
            traversal = new DungeonKnight3DTraversalBuilder(assets);
        }

        public void BuildWorldOneTwo(PlayerController3D player)
        {
            CreateBox("World 1-2 Courtyard Floor", new Vector3(0f, -0.25f, 43f), new Vector3(22f, 0.5f, 42f), assets.ExteriorStone);
            CreateBox("World 1-2 Left Parapet", new Vector3(-11.25f, 1.25f, 43f), new Vector3(0.5f, 3f, 42f), assets.DarkStone);
            CreateBox("World 1-2 Right Parapet", new Vector3(11.25f, 1.25f, 43f), new Vector3(0.5f, 3f, 42f), assets.DarkStone);
            CreateBox("World 1-2 Far Arch Left", new Vector3(-4.6f, 2.2f, 64.2f), new Vector3(4.6f, 4.6f, 0.5f), assets.DarkStone);
            CreateBox("World 1-2 Far Arch Right", new Vector3(4.6f, 2.2f, 64.2f), new Vector3(4.6f, 4.6f, 0.5f), assets.DarkStone);
            CreateBox("World 1-2 Far Arch Top", new Vector3(0f, 4.6f, 64.2f), new Vector3(9.5f, 0.9f, 0.55f), assets.DarkStone);
            interactables.CreateDoor("Door 1-2 Back To 1-1", new Vector3(0f, 1.35f, 26.75f), new Vector3(2.35f, 2.7f, 0.35f), new Vector3(0f, 1.05f, 18.2f), Vector3.back, "Regresaste al World 1-1.");
            interactables.CreateDoor("Door 1-2 To 1-3", new Vector3(0f, 1.35f, 64.7f), new Vector3(2.55f, 2.8f, 0.35f), new Vector3(0f, 1.05f, 70.8f), Vector3.forward, "Subiste hacia la torre World 1-3.");

            traversal.CreateBridgeRailing(new Vector3(-4.6f, 0.78f, 29.5f), 10);
            traversal.CreateBridgeRailing(new Vector3(4.6f, 0.78f, 29.5f), 10);
            traversal.CreateBridgeRailing(new Vector3(-8.8f, 0.78f, 48f), 13);
            traversal.CreateBridgeRailing(new Vector3(8.8f, 0.78f, 48f), 13);

            CreateBox("World 1-2 Mid Walkway Main", new Vector3(-5f, 1.18f, 40.75f), new Vector3(5.6f, 0.45f, 4.7f), assets.Stone);
            CreateBox("World 1-2 Mid Walkway Left Lip", new Vector3(-6.8f, 1.18f, 36.85f), new Vector3(2f, 0.45f, 3.1f), assets.Stone);
            CreateBox("World 1-2 Mid Walkway Right Lip", new Vector3(-2.8f, 1.18f, 36.85f), new Vector3(1.2f, 0.45f, 3.1f), assets.Stone);
            CreateBox("World 1-2 Mid Stair Landing", new Vector3(-4.55f, 1.18f, 38.15f), new Vector3(1.75f, 0.45f, 0.55f), assets.Stone);
            CreateBox("World 1-2 Upper Walkway Main", new Vector3(5.2f, 2.15f, 51.5f), new Vector3(5.8f, 0.45f, 4.2f), assets.Stone);
            CreateBox("World 1-2 Upper Walkway Side Lip", new Vector3(6.1f, 2.15f, 47.6f), new Vector3(4f, 0.45f, 3.6f), assets.Stone);
            CreateBox("World 1-2 Upper Stair Landing", new Vector3(3.15f, 2.15f, 49.05f), new Vector3(1.7f, 0.45f, 0.7f), assets.Stone);
            traversal.CreateMovingPlatform("World 1-2 Moving Bridge", new Vector3(0f, 1.42f, 46.1f), new Vector3(3.2f, 0.42f, 2.2f), new Vector3(4.8f, 0f, 0f), 0.8f);

            traversal.CreateStairRun("World 1-2 Lower Stair", new Vector3(-1.5f, 0f, 34.1f), 8, new Vector3(-0.45f, 0.16f, 0.48f), 1.55f);
            traversal.CreateStairRun("World 1-2 Upper Stair", new Vector3(-1.3f, 1.15f, 45.3f), 8, new Vector3(0.55f, 0.14f, 0.45f), 1.55f);

            geometry.CreateTorch(new Vector3(-10.85f, 2.0f, 30f));
            geometry.CreateTorch(new Vector3(10.85f, 2.0f, 35.5f));
            geometry.CreateTorch(new Vector3(-10.85f, 2.0f, 47f));
            geometry.CreateTorch(new Vector3(10.85f, 2.0f, 56f));
            CreateBox("World 1-2 Moon Banner", new Vector3(-10.95f, 2.2f, 42f), new Vector3(0.08f, 2.1f, 1.25f), DungeonKnight3DAssets.NewMaterial("DK3D Purple Banner", new Color(0.28f, 0.12f, 0.42f)));
            CreateBox("World 1-2 Crown Banner", new Vector3(10.95f, 2.2f, 53f), new Vector3(0.08f, 2.1f, 1.25f), DungeonKnight3DAssets.NewMaterial("DK3D Gold Banner", new Color(0.62f, 0.42f, 0.1f)));

            interactables.CreateBonfire("World 1-2 Bonfire", new Vector3(-7.6f, 0.45f, 30.8f));
            interactables.CreateInteractableBox("World 1-2 Lore Tablet", new Vector3(-8.9f, 0.75f, 33.2f), new Vector3(1f, 1.35f, 0.26f), assets.Brass)
                .ConfigureLore("El 1-2 abre el castillo: pasarelas, ruinas y enemigos en altura.");
            interactables.CreateChest("World 1-2 Supply Chest", new Vector3(5.2f, 2.72f, 51.6f), 18);

            interactables.CreatePickup("World 1-2 Potion", new Vector3(7.2f, 0.65f, 58f), false);

            enemies.CreateEnemy("World 1-2 Gate Guard", new Vector3(4.3f, 1.05f, 31.5f), player, 55, 10, 2.55f, false);
            enemies.CreateEnemy("World 1-2 Walkway Guard", new Vector3(-5.2f, 2.46f, 40.2f), player, 60, 11, 2.45f, false);
            enemies.CreateEnemy("World 1-2 Upper Guard", new Vector3(5.6f, 3.38f, 49.2f), player, 60, 12, 2.35f, false);
            enemies.CreateEnemy("World 1-2 Door Knight", new Vector3(0f, 1.05f, 61f), player, 85, 16, 2.45f, false);
        }

        public void BuildWorldOneThree(PlayerController3D player)
        {
            CreateBox("World 1-3 Tower Lower Floor", new Vector3(0f, -0.25f, 77.5f), new Vector3(16f, 0.5f, 25f), assets.Stone);
            CreateBox("World 1-3 Tower Mid Floor", new Vector3(0f, 2.95f, 87f), new Vector3(13f, 0.45f, 16f), assets.Stone);
            CreateBox("World 1-3 Tower Top Floor", new Vector3(0f, 6.15f, 97f), new Vector3(12f, 0.45f, 14f), assets.Stone);
            CreateBox("World 1-3 Left Wall", new Vector3(-8.25f, 3f, 87f), new Vector3(0.5f, 8f, 34f), assets.ShadowStone);
            CreateBox("World 1-3 Right Wall", new Vector3(8.25f, 3f, 87f), new Vector3(0.5f, 8f, 34f), assets.ShadowStone);
            CreateBox("World 1-3 Rear Wall", new Vector3(0f, 5.4f, 104f), new Vector3(16.5f, 10f, 0.5f), assets.ShadowStone);
            interactables.CreateDoor("Door 1-3 Back To 1-2", new Vector3(0f, 1.35f, 70.15f), new Vector3(2.35f, 2.7f, 0.35f), new Vector3(0f, 1.05f, 61.8f), Vector3.back, "Regresaste al World 1-2.");

            traversal.CreateStairRun("World 1-3 Lower Tower Stair", new Vector3(-5.5f, 0f, 80.8f), 13, new Vector3(0.45f, 0.25f, 0.48f), 1.55f, true);
            traversal.CreateStairRun("World 1-3 Upper Tower Stair", new Vector3(5.2f, 3.1f, 89.1f), 13, new Vector3(-0.45f, 0.25f, 0.48f), 1.55f, true);
            traversal.CreateMovingPlatform("World 1-3 Lift Stone", new Vector3(-4.6f, 4.4f, 94f), new Vector3(2.8f, 0.42f, 2.8f), new Vector3(0f, 2.6f, 0f), 0.7f);

            for (int i = 0; i < 5; i++)
            {
                float z = 73.5f + i * 7f;
                geometry.CreateColumn(new Vector3(-6.8f, 2.1f, z));
                geometry.CreateColumn(new Vector3(6.8f, 2.1f, z));
                geometry.CreateTorch(new Vector3(i % 2 == 0 ? -7.85f : 7.85f, 3.2f, z + 2.5f));
            }

            interactables.CreateBonfire("World 1-3 Tower Bonfire", new Vector3(5.7f, 0.45f, 73.3f));
            interactables.CreateInteractableBox("World 1-3 Lore Tablet", new Vector3(-5.6f, 0.8f, 74.3f), new Vector3(1f, 1.35f, 0.26f), assets.Brass)
                .ConfigureLore("La torre del 1-3 sube en espiral. Las plataformas moviles reemplazan los saltos del prototipo 2D.");
            interactables.CreateChest("World 1-3 Tower Chest", new Vector3(-4.8f, 6.72f, 99.5f), 28);
            interactables.CreateInteractableBox("World 1-3 Exit Door", new Vector3(0f, 7.35f, 103f), new Vector3(2.4f, 2.6f, 0.4f), assets.Brass).ConfigureExit();

            interactables.CreatePickup("World 1-3 Potion", new Vector3(4.6f, 6.75f, 101f), false);

            enemies.CreateEnemy("World 1-3 Tower Guard A", new Vector3(-2.5f, 1.05f, 78f), player, 70, 13, 2.4f, false);
            enemies.CreateEnemy("World 1-3 Tower Guard B", new Vector3(3.5f, 4.18f, 89.4f), player, 75, 14, 2.35f, false);
            enemies.CreateEnemy("World 1-3 Crown Warden", new Vector3(0f, 7.38f, 100.2f), player, 130, 20, 2.55f, false);
        }

        private GameObject CreateBox(string name, Vector3 position, Vector3 scale, Material material)
        {
            return DungeonKnight3DGeometryBuilder.CreateBox(name, position, scale, material);
        }
    }
}
