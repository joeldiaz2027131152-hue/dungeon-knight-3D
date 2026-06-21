using DungeonKnight.Player;
using UnityEngine;

namespace DungeonKnight.Level
{
    public class DungeonKnight3DBootstrap : MonoBehaviour
    {
        public static readonly Vector3 PlayerSpawn = new Vector3(0f, 1.05f, -16f);

        private static bool built;
        private DungeonKnight3DAssets assets;
        private DungeonKnight3DInteractableBuilder interactables;
        private DungeonKnight3DEnemySpawner enemies;

        public static void BuildOnPlayScene()
        {
            if (built || Object.FindAnyObjectByType<PlayerController3D>()) return;
            built = true;
            new GameObject("Dungeon Knight 3D Bootstrap").AddComponent<DungeonKnight3DBootstrap>().Build();
        }

        private void Build()
        {
            assets = DungeonKnight3DAssets.Load();
            interactables = new DungeonKnight3DInteractableBuilder(assets);
            enemies = new DungeonKnight3DEnemySpawner(assets);
            ConfigureWorld();

            DungeonKnight3DPlayerFactory playerFactory = new DungeonKnight3DPlayerFactory(assets);
            PlayerController3D player = playerFactory.CreatePlayer();
            playerFactory.CreateCamera(player.transform);
            playerFactory.CreateHud(player);

            new DungeonKnight3DWorldBuilder(assets).BuildWorldOneOneBase();
            interactables.BuildWorldOneOneInteractables();
            enemies.CreateWorldOneOneEnemies(player);
            DungeonKnight3DProgressionWorldBuilder progressionWorlds = new DungeonKnight3DProgressionWorldBuilder(assets, interactables, enemies);
            progressionWorlds.BuildWorldOneTwo(player);
            progressionWorlds.BuildWorldOneThree(player);

            Debug.Log("Dungeon Knight 3D: World 1-1, 1-2 and tower prototype built in the copied 3D project.");
        }

        private void ConfigureWorld()
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.22f, 0.25f, 0.32f);
            RenderSettings.ambientEquatorColor = new Color(0.12f, 0.13f, 0.17f);
            RenderSettings.ambientGroundColor = new Color(0.04f, 0.035f, 0.04f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.035f, 0.04f, 0.06f);
            RenderSettings.fogDensity = 0.022f;

            GameObject lightObject = new GameObject("Moon Directional Light");
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(0.65f, 0.72f, 1f);
            light.intensity = 1.1f;
            lightObject.transform.rotation = Quaternion.Euler(48f, -34f, 0f);
        }

    }
}
