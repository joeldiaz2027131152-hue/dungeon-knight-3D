using DungeonKnight.Player;
using DungeonKnight.UI;
using UnityEngine;

namespace DungeonKnight.Level
{
    public class DungeonKnight3DBootstrap : MonoBehaviour
    {
        public static readonly Vector3 PlayerSpawn = new Vector3(0f, 1.05f, -16f);

        private static bool built;
        private Material stone;
        private Material darkStone;
        private Material brass;
        private Material ember;
        private Material enemy;
        private Material playerBody;
        private Material potion;
        private Material exteriorStone;
        private Material shadowStone;
        private Material hazard;
        private Material wood;
        private Material ashStone;
        private Material charcoal;
        private Material mist;
        private Material chestWood;
        private Material chestGold;
        private Material gemBlue;
        private Material gemRed;
        private Sprite skeletonEnemySprite;
        private Sprite[] skeletonAttackSprites;
        private Sprite[] skeletonWalkFrontSprites;
        private Sprite[] skeletonWalkLeftSprites;
        private Sprite[] skeletonWalkRightSprites;
        private Sprite playerIdleSprite;
        private Sprite[] playerWalkFrontSprites;
        private Sprite[] playerWalkLeftSprites;
        private Sprite[] playerWalkRightSprites;
        private Sprite[] playerWalkBackSprites;
        private Sprite[] playerRollFrontSprites;
        private Sprite[] playerRollLeftSprites;
        private Sprite[] playerRollRightSprites;
        private Sprite[] playerRollBackSprites;
        private Sprite[] playerAttackFrontSprites;
        private Sprite[] playerAttackBackSprites;
        private Sprite[] playerAttackLeftSprites;
        private Sprite[] playerAttackRightSprites;
        private Sprite[] playerBlockSprites;
        private Sprite[] playerBlockWalkFrontSprites;
        private Sprite[] playerBlockWalkLeftSprites;
        private Sprite[] playerBlockWalkRightSprites;
        private Sprite[] playerBlockWalkBackSprites;

        public static void BuildOnPlayScene()
        {
            if (built || Object.FindAnyObjectByType<PlayerController3D>()) return;
            built = true;
            new GameObject("Dungeon Knight 3D Bootstrap").AddComponent<DungeonKnight3DBootstrap>().Build();
        }

        private void Build()
        {
            CreateMaterials();
            ConfigureWorld();

            PlayerController3D player = CreatePlayer();
            CreateCamera(player.transform);
            CreateHud(player);

            CreateDungeonShell();
            CreateTraversal();
            CreateSetDressing();
            CreateInteractables();
            CreateEnemies(player);
            CreateWorldOneTwo(player);
            CreateWorldOneThree(player);
            CreateFirstBonfireNpc();

            Debug.Log("Dungeon Knight 3D: continuous castle passage and tower prototype built in the copied 3D project.");
        }

        private void CreateMaterials()
        {
            Texture2D dungeonStoneTexture = Resources.Load<Texture2D>("Art/Textures/Dungeon/dark_moss_stone_tiles");
            stone = NewTexturedMaterial("DK3D Moss Stone", new Color(0.72f, 0.74f, 0.72f), dungeonStoneTexture);
            darkStone = NewTexturedMaterial("DK3D Dark Moss Stone", new Color(0.42f, 0.46f, 0.46f), dungeonStoneTexture);
            brass = NewMaterial("DK3D Old Brass", new Color(0.86f, 0.62f, 0.28f));
            ember = NewMaterial("DK3D Ember", new Color(1f, 0.26f, 0.08f));
            enemy = NewMaterial("DK3D Bone Enemy", new Color(0.82f, 0.78f, 0.66f));
            playerBody = NewMaterial("DK3D Knight Steel", new Color(0.52f, 0.58f, 0.68f));
            potion = NewMaterial("DK3D Potion", new Color(0.9f, 0.08f, 0.22f));
            exteriorStone = NewMaterial("DK3D Exterior Stone", new Color(0.34f, 0.36f, 0.39f));
            shadowStone = NewMaterial("DK3D Shadow Stone", new Color(0.07f, 0.075f, 0.09f));
            hazard = NewMaterial("DK3D Hazard Iron", new Color(0.68f, 0.18f, 0.08f));
            wood = NewMaterial("DK3D Charred Wood", new Color(0.34f, 0.16f, 0.07f));
            ashStone = NewMaterial("DK3D Ash Ring Stone", new Color(0.22f, 0.22f, 0.21f));
            charcoal = NewMaterial("DK3D Hot Charcoal", new Color(0.08f, 0.045f, 0.035f));
            mist = NewTransparentMaterial("DK3D White Fog Gate", new Color(0.78f, 0.82f, 0.9f, 0.48f));
            chestWood = NewMaterial("DK3D Chest Dark Wood", new Color(0.42f, 0.2f, 0.07f));
            chestGold = NewMaterial("DK3D Chest Gold", new Color(1f, 0.68f, 0.16f));
            gemBlue = NewMaterial("DK3D Gem Blue", new Color(0.12f, 0.46f, 1f));
            gemRed = NewMaterial("DK3D Gem Red", new Color(1f, 0.12f, 0.2f));
            const string skeletonPath = "Characters/Enemies/SkeletonEnemy/Normalized/";
            skeletonEnemySprite = LoadSpriteAsset(skeletonPath + "skeleton_enemy_idle_normalized");
            skeletonAttackSprites = new[]
            {
                LoadSpriteAsset(skeletonPath + "skeleton_enemy_attack_front_slash_normalized"),
                LoadSpriteAsset(skeletonPath + "skeleton_enemy_attack_back_slash_normalized"),
                LoadSpriteAsset(skeletonPath + "skeleton_enemy_attack_right_slash_normalized"),
                LoadSpriteAsset(skeletonPath + "skeleton_enemy_attack_left_slash_normalized"),
                LoadSpriteAsset(skeletonPath + "skeleton_enemy_attack_front_left_slash_normalized"),
                LoadSpriteAsset(skeletonPath + "skeleton_enemy_attack_front_right_slash_normalized")
            };
            skeletonWalkFrontSprites = LoadSpriteSequence(skeletonPath + "skeleton_enemy_walk_front_", 4, "_normalized");
            skeletonWalkLeftSprites = LoadSpriteSequence(skeletonPath + "skeleton_enemy_walk_left_", 4, "_normalized");
            skeletonWalkRightSprites = LoadSpriteSequence(skeletonPath + "skeleton_enemy_walk_right_", 4, "_normalized");

            const string playerPath = "Characters/Player/DarkKnight/Normalized/";
            playerIdleSprite = LoadSpriteAsset(playerPath + "dark_knight_idle_base_normalized");
            playerWalkFrontSprites = LoadSpriteSequence(playerPath + "dark_knight_walk_front_", 4, "_normalized");
            playerWalkLeftSprites = LoadSpriteSequence(playerPath + "dark_knight_walk_left_", 4, "_normalized");
            playerWalkRightSprites = LoadSpriteSequence(playerPath + "dark_knight_walk_right_", 4, "_normalized");
            playerWalkBackSprites = LoadSpriteSequence(playerPath + "dark_knight_walk_back_", 4, "_normalized");
            playerRollFrontSprites = LoadSpriteSequence(playerPath + "dark_knight_roll_front_", 4, "_normalized");
            playerRollLeftSprites = LoadSpriteSequence(playerPath + "dark_knight_roll_left_", 4, "_normalized");
            playerRollRightSprites = LoadSpriteSequence(playerPath + "dark_knight_roll_right_", 4, "_normalized");
            playerRollBackSprites = LoadSpriteSequence(playerPath + "dark_knight_roll_back_", 4, "_normalized");
            playerBlockWalkFrontSprites = LoadSpriteSequence(playerPath + "dark_knight_blockwalk_front_", 4, "_normalized");
            playerBlockWalkLeftSprites = LoadSpriteSequence(playerPath + "dark_knight_blockwalk_left_", 4, "_normalized");
            playerBlockWalkRightSprites = LoadSpriteSequence(playerPath + "dark_knight_blockwalk_right_", 4, "_normalized");
            playerBlockWalkBackSprites = LoadSpriteSequence(playerPath + "dark_knight_blockwalk_back_", 4, "_normalized");
            playerAttackFrontSprites = LoadSpriteSequence(playerPath + "dark_knight_attackanim_front_", 4, "_normalized");
            playerAttackBackSprites = LoadSpriteSequence(playerPath + "dark_knight_attackanim_back_", 4, "_normalized");
            playerAttackLeftSprites = LoadSpriteSequence(playerPath + "dark_knight_attackanim_left_", 4, "_normalized");
            playerAttackRightSprites = LoadSpriteSequence(playerPath + "dark_knight_attackanim_right_", 4, "_normalized");
            playerBlockSprites = new[]
            {
                LoadSpriteAsset(playerPath + "dark_knight_block_front_block_normalized"),
                LoadSpriteAsset(playerPath + "dark_knight_block_back_block_normalized"),
                LoadSpriteAsset(playerPath + "dark_knight_block_right_block_normalized"),
                LoadSpriteAsset(playerPath + "dark_knight_block_left_block_normalized"),
                LoadSpriteAsset(playerPath + "dark_knight_block_front_left_block_normalized"),
                LoadSpriteAsset(playerPath + "dark_knight_block_front_right_block_normalized")
            };
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

        private PlayerController3D CreatePlayer()
        {
            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Knight 3D";
            player.transform.position = PlayerSpawn;
            player.transform.localScale = new Vector3(0.82f, 1.05f, 0.82f);
            Object.Destroy(player.GetComponent<CapsuleCollider>());
            player.GetComponent<Renderer>().material = playerBody;

            CharacterController controller = player.AddComponent<CharacterController>();
            controller.height = 2.1f;
            controller.radius = 0.42f;
            controller.center = Vector3.zero;

            PlayerController3D playerController = player.AddComponent<PlayerController3D>();
            if (!AttachPlayerModel(player.transform, playerController) && !AttachPlayerSprite(player.transform, playerController))
            {
                CreateWeaponVisual(player.transform);
                CreateShieldVisual(player.transform);
            }

            return playerController;
        }

        private void CreateCamera(Transform target)
        {
            Camera camera = Camera.main;
            if (!camera)
            {
                GameObject cameraObject = new GameObject("Main Camera");
                camera = cameraObject.AddComponent<Camera>();
                cameraObject.tag = "MainCamera";
            }

            camera.transform.position = PlayerSpawn + new Vector3(0f, 7.4f, -8.6f);
            camera.transform.rotation = Quaternion.Euler(48f, 0f, 0f);
            camera.fieldOfView = 58f;
            camera.nearClipPlane = 0.08f;
            CameraFollow3D follow = camera.gameObject.AddComponent<CameraFollow3D>();
            follow.SetTarget(target);
        }

        private static void CreateHud(PlayerController3D player)
        {
            new GameObject("Game HUD 3D").AddComponent<GameHud3D>().Bind(player);
        }

        private void CreateDungeonShell()
        {
            CreateBox("Main Stone Floor", new Vector3(0f, -0.25f, 0f), new Vector3(15f, 0.5f, 44f), stone);
            CreateBox("Left Wall", new Vector3(-7.75f, 2.5f, 0f), new Vector3(0.5f, 5.5f, 44f), darkStone);
            CreateBox("Right Wall", new Vector3(7.75f, 2.5f, 0f), new Vector3(0.5f, 5.5f, 44f), darkStone);
            CreateBox("Rear Wall Left", new Vector3(-5.25f, 2.5f, 22f), new Vector3(5f, 5.5f, 0.5f), darkStone);
            CreateBox("Rear Wall Right", new Vector3(5.25f, 2.5f, 22f), new Vector3(5f, 5.5f, 0.5f), darkStone);
            CreateBox("Rear Gate Arch", new Vector3(0f, 4.35f, 22f), new Vector3(5.8f, 1.2f, 0.55f), darkStone);
            CreateBox("Start Arch", new Vector3(0f, 3.8f, -20f), new Vector3(15f, 0.8f, 0.7f), darkStone);

            for (int i = 0; i < 8; i++)
            {
                float z = -17.5f + i * 5f;
                CreateColumn(new Vector3(-6.7f, 1.25f, z));
                CreateColumn(new Vector3(6.7f, 1.25f, z));
            }
        }

        private void CreateTraversal()
        {
            CreateBox("Raised Walkway", new Vector3(0f, 0.5f, -1f), new Vector3(5.4f, 0.55f, 8.5f), stone);
            CreateBox("Left Side Platform", new Vector3(-4.1f, 1.4f, 7.8f), new Vector3(3.6f, 0.45f, 4.5f), stone);
            CreateBox("Right Side Platform", new Vector3(4.1f, 2.25f, 14.4f), new Vector3(3.6f, 0.45f, 4.5f), stone);
            CreateBox("Gate Landing", new Vector3(0f, 0.3f, 18.6f), new Vector3(7.2f, 0.45f, 3.2f), stone);

            for (int i = 0; i < 7; i++)
            {
                CreateBox($"Stair Step {i + 1}", new Vector3(-2.8f + i * 0.46f, 0.05f + i * 0.16f, 4.2f + i * 0.42f), new Vector3(1.6f, 0.32f, 0.55f), stone);
            }

            CreateSpikeRow(new Vector3(0f, 0.15f, -7.2f), 9);
            CreateSpikeRow(new Vector3(0f, 0.15f, 10.8f), 7);
        }

        private void CreateSetDressing()
        {
            for (int i = 0; i < 6; i++)
            {
                float z = -14f + i * 6f;
                CreateTorch(new Vector3(-7.35f, 2.1f, z));
                CreateTorch(new Vector3(7.35f, 2.1f, z + 2.4f));
            }

            CreateBox("Broken Statue Base", new Vector3(-4.8f, 0.35f, -12.8f), new Vector3(1.5f, 0.7f, 1.5f), darkStone);
            CreateBox("Broken Statue Torso", new Vector3(-4.8f, 1.15f, -12.8f), new Vector3(0.75f, 1.2f, 0.55f), darkStone);
            CreateBox("Blue Banner", new Vector3(7.45f, 2.2f, -4.4f), new Vector3(0.08f, 2f, 1.1f), NewMaterial("DK3D Blue Banner", new Color(0.1f, 0.22f, 0.5f)));
            CreateBox("Red Banner", new Vector3(-7.45f, 2.2f, 8.5f), new Vector3(0.08f, 2f, 1.1f), NewMaterial("DK3D Red Banner", new Color(0.48f, 0.04f, 0.08f)));
        }

        private void CreateInteractables()
        {
            CreateBonfire("Bonfire", new Vector3(-3.3f, 0.45f, -15.1f));
            CreateInteractableBox("Controls Lore Tablet", new Vector3(2.7f, 0.6f, -13.4f), new Vector3(1.1f, 1.2f, 0.28f), brass)
                .ConfigureLore("J: atacar. Mantener J: golpe cargado. K bloquea, L rueda, E interactua.");
            CreateChest("Treasure Chest", new Vector3(-4.1f, 1.95f, 7.8f), 12);

            Transform gate = CreateGothicDoor("Castle Passage Door", new Vector3(0f, 1.65f, 20.7f), 5.8f, 3.25f);
            CreateInvisibleInteractableBox("Gate Lock", new Vector3(0f, 1f, 19.85f), new Vector3(1.35f, 1.5f, 0.75f), brass).ConfigureGate(gate);
            CreateInteractableBox("Castle Passage Marker", new Vector3(2.7f, 0.82f, 22.8f), new Vector3(0.85f, 1.05f, 0.22f), darkStone)
                .ConfigureLore("El porton abre un solo camino: patio, pasarelas y torre. No hay descanso hasta la siguiente hoguera.");

            CreatePickup("Coin A", new Vector3(2.4f, 0.65f, -6.2f), true);
            CreatePickup("Coin B", new Vector3(-2.4f, 0.65f, -4.6f), true);
            CreatePickup("Potion", new Vector3(4.2f, 2.95f, 14.4f), false);
        }

        private void CreateEnemies(PlayerController3D player)
        {
            CreateEnemy("Skeleton Guard", new Vector3(0f, 1.05f, -2.6f), player, 45, 9, 2.35f, false);
            CreateEnemy("Skeleton Archer Stand-in", new Vector3(-3.2f, 2.68f, 8f), player, 38, 8, 2.1f, false);
            CreateEnemy("Key Guardian 3D", new Vector3(0f, 1.05f, 16.5f), player, 110, 18, 2.6f, true).transform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
        }

        private void CreateBonfire(string name, Vector3 position)
        {
            DungeonInteractable3D bonfire = CreateInteractableBox(name, position, new Vector3(1.8f, 0.7f, 1.8f), ember);
            bonfire.ConfigureBonfire();
            Renderer triggerRenderer = bonfire.GetComponent<Renderer>();
            if (triggerRenderer) triggerRenderer.enabled = false;

            CreateBonfireVisual(name, position);
        }

        private void CreateBonfireVisual(string name, Vector3 position)
        {
            Vector3 floorPosition = new Vector3(position.x, 0.02f, position.z);
            GameObject root = new GameObject($"{name} Visual");
            root.transform.position = floorPosition;

            GameObject ash = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ash.name = $"{name} Ash Bed";
            ash.transform.SetParent(root.transform, false);
            ash.transform.localPosition = new Vector3(0f, 0.015f, 0f);
            ash.transform.localScale = new Vector3(1.35f, 0.035f, 1.35f);
            ash.GetComponent<Renderer>().material = charcoal;
            Object.Destroy(ash.GetComponent<Collider>());

            const int stoneCount = 12;
            for (int i = 0; i < stoneCount; i++)
            {
                CreateBonfireStone(root.transform, i, stoneCount);
            }

            CreateBonfireLog(root.transform, 28f, 0.18f);
            CreateBonfireLog(root.transform, 118f, 0.21f);
            CreateBonfireLog(root.transform, -32f, 0.25f);
            CreateBonfireLog(root.transform, -122f, 0.28f);

            CreateBonfireCoal(root.transform, new Vector3(0f, 0.16f, 0f), new Vector3(0.34f, 0.14f, 0.34f));
            CreateBonfireFlame(root.transform, new Vector3(0f, 0.22f, 0.02f), 1.52f, Vector3.right);
            CreateBonfireFlame(root.transform, new Vector3(-0.18f, 0.19f, -0.04f), 0.94f, Vector3.left + Vector3.forward * 0.25f);
            CreateBonfireFlame(root.transform, new Vector3(0.2f, 0.18f, -0.03f), 0.84f, Vector3.right + Vector3.back * 0.2f);

            Light light = root.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1f, 0.44f, 0.16f);
            light.intensity = 1.55f;
            light.range = 7.5f;
        }

        private void CreateBonfireStone(Transform parent, int index, int count)
        {
            float angle = index / (float)count * Mathf.PI * 2f;
            float radius = 0.74f + Mathf.Sin(index * 2.17f) * 0.04f;
            GameObject stoneObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stoneObject.name = "Bonfire Ring Stone";
            stoneObject.transform.SetParent(parent, false);
            stoneObject.transform.localPosition = new Vector3(Mathf.Cos(angle) * radius, 0.08f, Mathf.Sin(angle) * radius);
            stoneObject.transform.localRotation = Quaternion.Euler(0f, -angle * Mathf.Rad2Deg + 8f, 0f);
            stoneObject.transform.localScale = new Vector3(0.34f + Mathf.Sin(index * 1.9f) * 0.04f, 0.18f, 0.22f + Mathf.Cos(index * 1.4f) * 0.035f);
            stoneObject.GetComponent<Renderer>().material = ashStone;
            Object.Destroy(stoneObject.GetComponent<Collider>());
        }

        private void CreateBonfireLog(Transform parent, float angleDegrees, float height)
        {
            Vector3 direction = Quaternion.Euler(0f, angleDegrees, 0f) * Vector3.forward;
            GameObject log = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            log.name = "Bonfire Charred Log";
            log.transform.SetParent(parent, false);
            log.transform.localPosition = new Vector3(0f, height, 0f);
            log.transform.localRotation = Quaternion.FromToRotation(Vector3.up, direction) * Quaternion.Euler(0f, 0f, 8f);
            log.transform.localScale = new Vector3(0.12f, 0.68f, 0.12f);
            log.GetComponent<Renderer>().material = wood;
            Object.Destroy(log.GetComponent<Collider>());
        }

        private void CreateBonfireCoal(Transform parent, Vector3 localPosition, Vector3 scale)
        {
            GameObject coal = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            coal.name = "Bonfire Ember Core";
            coal.transform.SetParent(parent, false);
            coal.transform.localPosition = localPosition;
            coal.transform.localScale = scale;
            coal.GetComponent<Renderer>().material = ember;
            Object.Destroy(coal.GetComponent<Collider>());
        }

        private void CreateBonfireFlame(Transform parent, Vector3 localPosition, float scale, Vector3 wind)
        {
            GameObject flame = new GameObject("Bonfire Flame");
            flame.transform.SetParent(parent, false);
            flame.transform.localPosition = localPosition;
            flame.transform.localScale = Vector3.one * scale;
            flame.AddComponent<SpriteRenderer>();

            Light light = flame.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1f, 0.45f, 0.13f);
            light.intensity = 1.8f;
            light.range = 5.2f;
            flame.AddComponent<AnimatedWallTorch3D>().Configure(light, wind);
        }

        private void CreateWorldOneTwo(PlayerController3D player)
        {
            CreateBox("World 1-2 Courtyard Floor", new Vector3(0f, -0.25f, 43f), new Vector3(22f, 0.5f, 42f), exteriorStone);
            CreateBox("World 1-2 Left Parapet", new Vector3(-11.25f, 1.25f, 43f), new Vector3(0.5f, 3f, 42f), darkStone);
            CreateBox("World 1-2 Right Parapet", new Vector3(11.25f, 1.25f, 43f), new Vector3(0.5f, 3f, 42f), darkStone);
            CreateBox("World 1-2 Far Arch Left", new Vector3(-4.6f, 2.2f, 64.2f), new Vector3(4.6f, 4.6f, 0.5f), darkStone);
            CreateBox("World 1-2 Far Arch Right", new Vector3(4.6f, 2.2f, 64.2f), new Vector3(4.6f, 4.6f, 0.5f), darkStone);
            CreateBox("World 1-2 Far Arch Top", new Vector3(0f, 4.6f, 64.2f), new Vector3(9.5f, 0.9f, 0.55f), darkStone);
            CreateBox("Tower Threshold Stone Link", new Vector3(0f, -0.25f, 64.55f), new Vector3(9.2f, 0.5f, 1.3f), stone);
            CreateBridgeRailing(new Vector3(-4.6f, 0.78f, 29.5f), 10);
            CreateBridgeRailing(new Vector3(4.6f, 0.78f, 29.5f), 10);
            CreateBridgeRailing(new Vector3(-8.8f, 0.78f, 48f), 13);
            CreateBridgeRailing(new Vector3(8.8f, 0.78f, 48f), 13);

            CreateBox("World 1-2 Mid Walkway Main", new Vector3(-5f, 1.18f, 40.75f), new Vector3(5.6f, 0.45f, 4.7f), stone);
            CreateBox("World 1-2 Mid Walkway Left Lip", new Vector3(-6.8f, 1.18f, 36.85f), new Vector3(2f, 0.45f, 3.1f), stone);
            CreateBox("World 1-2 Mid Walkway Right Lip", new Vector3(-2.8f, 1.18f, 36.85f), new Vector3(1.2f, 0.45f, 3.1f), stone);
            CreateBox("World 1-2 Mid Stair Landing", new Vector3(-4.55f, 1.18f, 38.15f), new Vector3(1.75f, 0.45f, 0.55f), stone);
            CreateBox("World 1-2 Upper Walkway Main", new Vector3(5.2f, 2.15f, 51.5f), new Vector3(5.8f, 0.45f, 4.2f), stone);
            CreateBox("World 1-2 Upper Walkway Side Lip", new Vector3(6.1f, 2.15f, 47.6f), new Vector3(4f, 0.45f, 3.6f), stone);
            CreateBox("World 1-2 Upper Stair Landing", new Vector3(3.15f, 2.15f, 49.05f), new Vector3(1.7f, 0.45f, 0.7f), stone);
            CreateMovingPlatform("World 1-2 Moving Bridge", new Vector3(0f, 1.42f, 46.1f), new Vector3(3.2f, 0.42f, 2.2f), new Vector3(4.8f, 0f, 0f), 0.8f);

            CreateStairRun("World 1-2 Lower Stair", new Vector3(-1.5f, 0f, 34.1f), 8, new Vector3(-0.45f, 0.16f, 0.48f), 1.55f);
            CreateStairRun("World 1-2 Upper Stair", new Vector3(-1.3f, 1.15f, 45.3f), 8, new Vector3(0.55f, 0.14f, 0.45f), 1.55f);

            CreateTorch(new Vector3(-10.85f, 2.0f, 30f));
            CreateTorch(new Vector3(10.85f, 2.0f, 35.5f));
            CreateTorch(new Vector3(-10.85f, 2.0f, 47f));
            CreateTorch(new Vector3(10.85f, 2.0f, 56f));
            CreateBox("World 1-2 Moon Banner", new Vector3(-10.95f, 2.2f, 42f), new Vector3(0.08f, 2.1f, 1.25f), NewMaterial("DK3D Purple Banner", new Color(0.28f, 0.12f, 0.42f)));
            CreateBox("World 1-2 Crown Banner", new Vector3(10.95f, 2.2f, 53f), new Vector3(0.08f, 2.1f, 1.25f), NewMaterial("DK3D Gold Banner", new Color(0.62f, 0.42f, 0.1f)));

            CreateInteractableBox("Courtyard Lore Tablet", new Vector3(-8.9f, 0.75f, 33.2f), new Vector3(1f, 1.35f, 0.26f), brass)
                .ConfigureLore("El patio estira el riesgo: pasarelas, trampas de fuego y enemigos en altura antes de la torre.");
            CreateChest("World 1-2 Supply Chest", new Vector3(5.2f, 2.72f, 51.6f), 18);

            CreatePickup("World 1-2 Coin A", new Vector3(4.6f, 0.65f, 31.2f), true);
            CreatePickup("World 1-2 Coin B", new Vector3(-5.2f, 1.9f, 39.1f), true);
            CreatePickup("World 1-2 Potion", new Vector3(7.2f, 0.65f, 58f), false);

            CreateFireTrap("World 1-2 Fire Trap A", new Vector3(-2.8f, 0.35f, 35.8f), new Vector3(1.2f, 0.7f, 1.2f));
            CreateFireTrap("World 1-2 Fire Trap B", new Vector3(2.6f, 0.35f, 53.6f), new Vector3(1.3f, 0.7f, 1.3f));
            CreateBladeTrap("World 1-2 Ceiling Blade", new Vector3(0f, 2.9f, 42.4f), new Vector3(0f, 0f, 165f), new Vector3(0f, 0.55f, 0f));
            CreateSpikeRow(new Vector3(0f, 0.15f, 57.2f), 11);
            CreateHazardBox("World 1-2 Spike Damage", new Vector3(0f, 0.35f, 57.2f), new Vector3(8.2f, 0.7f, 1.35f), 18, "Pinchos oxidados.");
            CreateMiniBossGateArena(player);

            CreateEnemy("World 1-2 Gate Guard", new Vector3(4.3f, 1.05f, 31.5f), player, 55, 10, 2.55f, false);
            CreateEnemy("World 1-2 Walkway Guard", new Vector3(-5.2f, 2.46f, 40.2f), player, 60, 11, 2.45f, false);
            CreateEnemy("World 1-2 Upper Guard", new Vector3(5.6f, 3.38f, 49.2f), player, 60, 12, 2.35f, false);
        }

        private void CreateWorldOneThree(PlayerController3D player)
        {
            CreateBox("World 1-3 Tower Lower Floor", new Vector3(0f, -0.25f, 77.5f), new Vector3(16f, 0.5f, 25f), stone);
            CreateBox("World 1-3 Tower Mid Floor", new Vector3(0f, 2.95f, 87f), new Vector3(13f, 0.45f, 16f), stone);
            CreateBox("World 1-3 Tower Top Floor", new Vector3(0f, 6.15f, 97f), new Vector3(12f, 0.45f, 14f), stone);
            CreateBox("World 1-3 Left Wall", new Vector3(-8.25f, 3f, 87f), new Vector3(0.5f, 8f, 34f), shadowStone);
            CreateBox("World 1-3 Right Wall", new Vector3(8.25f, 3f, 87f), new Vector3(0.5f, 8f, 34f), shadowStone);
            CreateBox("World 1-3 Rear Wall", new Vector3(0f, 5.4f, 104f), new Vector3(16.5f, 10f, 0.5f), shadowStone);
            CreateStairRun("World 1-3 Lower Tower Stair", new Vector3(-5.5f, 0f, 80.8f), 13, new Vector3(0.45f, 0.25f, 0.48f), 1.35f);
            CreateStairRun("World 1-3 Upper Tower Stair", new Vector3(5.2f, 3.1f, 89.1f), 13, new Vector3(-0.45f, 0.25f, 0.48f), 1.35f);
            CreateBox("World 1-3 High Tower Floor", new Vector3(0f, 9.35f, 108f), new Vector3(11.2f, 0.45f, 13.5f), stone);
            CreateBox("World 1-3 Spire Floor", new Vector3(0f, 12.55f, 119f), new Vector3(9.4f, 0.45f, 10.5f), stone);
            CreateBox("World 1-3 High Left Wall", new Vector3(-5.85f, 9.5f, 108f), new Vector3(0.45f, 6.2f, 13.5f), shadowStone);
            CreateBox("World 1-3 High Right Wall", new Vector3(5.85f, 9.5f, 108f), new Vector3(0.45f, 6.2f, 13.5f), shadowStone);
            CreateBox("World 1-3 Spire Rear Wall", new Vector3(0f, 12.8f, 124.4f), new Vector3(10f, 4.6f, 0.45f), shadowStone);
            CreateStairRun("World 1-3 High Tower Stair", new Vector3(-4.9f, 6.28f, 101.4f), 13, new Vector3(0.42f, 0.25f, 0.48f), 1.25f);
            CreateStairRun("World 1-3 Spire Stair", new Vector3(4.35f, 9.45f, 112.2f), 13, new Vector3(-0.38f, 0.25f, 0.43f), 1.15f);
            CreateMovingPlatform("World 1-3 Lift Stone", new Vector3(-4.6f, 4.4f, 94f), new Vector3(2.8f, 0.42f, 2.8f), new Vector3(0f, 2.6f, 0f), 0.7f);

            for (int i = 0; i < 5; i++)
            {
                float z = 73.5f + i * 7f;
                CreateColumn(new Vector3(-6.8f, 2.1f, z));
                CreateColumn(new Vector3(6.8f, 2.1f, z));
                CreateTorch(new Vector3(i % 2 == 0 ? -7.85f : 7.85f, 3.2f, z + 2.5f));
            }

            CreateBonfire("Tower Bonfire", new Vector3(5.7f, 0.45f, 73.3f));
            CreateInteractableBox("Tower Lore Tablet", new Vector3(-5.6f, 0.8f, 74.3f), new Vector3(1f, 1.35f, 0.26f), brass)
                .ConfigureLore("La torre sube mucho mas de lo que parece. Arriba hay un mecanismo que baja un atajo.");
            CreateChest("World 1-3 Tower Chest", new Vector3(-4.8f, 13.02f, 119.5f), 38);
            CreateInteractableBox("World 1-3 Exit Door", new Vector3(0f, 13.95f, 123.6f), new Vector3(2.4f, 2.6f, 0.4f), brass).ConfigureExit();

            CreatePickup("World 1-3 Coin A", new Vector3(3.8f, 3.55f, 87.2f), true);
            CreatePickup("World 1-3 Coin B", new Vector3(-3.2f, 6.75f, 96f), true);
            CreatePickup("World 1-3 Coin C", new Vector3(3.6f, 10.02f, 112.5f), true);
            CreatePickup("World 1-3 Potion", new Vector3(4.2f, 13.05f, 121f), false);

            CreateBladeTrap("World 1-3 Lower Blade", new Vector3(0f, 2.25f, 80f), new Vector3(0f, 155f, 0f), new Vector3(0.9f, 0f, 0f));
            CreateBladeTrap("World 1-3 Upper Blade", new Vector3(0f, 5.45f, 93f), new Vector3(0f, 170f, 0f), new Vector3(1.15f, 0f, 0f));
            CreateFireTrap("World 1-3 Tower Fire", new Vector3(0f, 3.55f, 88.8f), new Vector3(1.2f, 0.7f, 1.2f));

            CreateEnemy("World 1-3 Tower Guard A", new Vector3(-2.5f, 1.05f, 78f), player, 70, 13, 2.4f, false);
            CreateEnemy("World 1-3 Tower Guard B", new Vector3(3.5f, 4.18f, 89.4f), player, 75, 14, 2.35f, false);
            CreateEnemy("World 1-3 High Guard", new Vector3(-2.7f, 10.38f, 109.8f), player, 85, 16, 2.35f, false);
            CreateEnemy("World 1-3 Crown Warden", new Vector3(0f, 13.78f, 119.5f), player, 150, 22, 2.45f, false, false, 42).transform.localScale = new Vector3(1.32f, 1.32f, 1.32f);
            CreateShortcutElevator();
        }

        private void CreateMiniBossGateArena(PlayerController3D player)
        {
            GameObject entranceMist = CreateFogWall("Boss Mist Entrance", new Vector3(0f, 1.7f, 58.85f), new Vector3(6.4f, 3.4f, 0.32f), false);
            GameObject entrySeal = CreateFogWall("Boss Mist Exit Seal", new Vector3(0f, 1.7f, 58.45f), new Vector3(6.4f, 3.4f, 0.42f), true);
            entrySeal.SetActive(false);

            GameObject bossObject = CreateEnemy("Mist Gate Warden", new Vector3(0f, 1.05f, 61.65f), player, 210, 24, 2.25f, false, true, 75);
            bossObject.transform.localScale = new Vector3(1.62f, 1.62f, 1.62f);
            DungeonEnemy3D boss = bossObject.GetComponent<DungeonEnemy3D>();

            entranceMist.AddComponent<DungeonMiniBossArena3D>().Configure(boss, entrySeal);

            Transform towerGate = CreateGothicDoor("Tower Gothic Door", new Vector3(0f, 1.65f, 64.95f), 5.7f, 3.25f);
            CreateInvisibleInteractableBox("Tower Gate Lock", new Vector3(0f, 1.0f, 63.9f), new Vector3(1.35f, 1.45f, 0.75f), brass).ConfigureTowerGate(towerGate);
            CreateInteractableBox("Mist Warning Tablet", new Vector3(-4.8f, 0.82f, 58.2f), new Vector3(1f, 1.25f, 0.25f), brass)
                .ConfigureLore("Tras la niebla no se retrocede. El guardian guarda la llave de la torre.");
        }

        private void CreateShortcutElevator()
        {
            Vector3 lower = new Vector3(-5.85f, 0.15f, 74.4f);
            Vector3 upper = new Vector3(-4.15f, 12.9f, 116.6f);
            GameObject platform = CreateBox("Tower Shortcut Elevator", lower, new Vector3(2.15f, 0.3f, 2.15f), darkStone);
            DungeonShortcutElevator3D elevator = platform.AddComponent<DungeonShortcutElevator3D>();
            elevator.Configure(platform.transform, lower, upper, 2.8f);

            CreateBox("Elevator Lower Shaft Left", lower + new Vector3(-1.25f, 3.4f, 0f), new Vector3(0.18f, 6.8f, 0.18f), brass);
            CreateBox("Elevator Lower Shaft Right", lower + new Vector3(1.25f, 3.4f, 0f), new Vector3(0.18f, 6.8f, 0.18f), brass);
            CreateBox("Elevator Upper Shaft Left", upper + new Vector3(-1.25f, -3.4f, 0f), new Vector3(0.18f, 6.8f, 0.18f), brass);
            CreateBox("Elevator Upper Shaft Right", upper + new Vector3(1.25f, -3.4f, 0f), new Vector3(0.18f, 6.8f, 0.18f), brass);

            CreateInteractableBox("Lower Elevator Lever", lower + new Vector3(1.72f, 0.72f, -0.95f), new Vector3(0.45f, 1.15f, 0.45f), brass).ConfigureElevator(elevator, false);
            CreateInteractableBox("Upper Elevator Lever", upper + new Vector3(1.72f, 0.72f, -0.95f), new Vector3(0.45f, 1.15f, 0.45f), brass).ConfigureElevator(elevator, true);
        }

        private void CreateFirstBonfireNpc()
        {
            Vector3 position = new Vector3(-5.35f, 0f, -14.7f);
            GameObject npc = new GameObject("Anciano del Principio NPC");
            npc.transform.position = position;
            npc.transform.rotation = Quaternion.LookRotation(new Vector3(2f, 0f, -0.4f).normalized, Vector3.up);

            if (!AttachAncianoNpcModel(npc.transform))
            {
                GameObject fallback = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                fallback.name = "Anciano del Principio Fallback";
                fallback.transform.SetParent(npc.transform, false);
                fallback.transform.localPosition = Vector3.up;
                fallback.transform.localRotation = Quaternion.identity;
                fallback.transform.localScale = new Vector3(0.65f, 0.9f, 0.65f);
                fallback.GetComponent<Renderer>().material = NewMaterial("DK3D Anciano Cloak", new Color(0.16f, 0.19f, 0.24f));
                Object.Destroy(fallback.GetComponent<CapsuleCollider>());

                Transform staff = CreateBox("Anciano del Principio Staff Rest", position + new Vector3(0.45f, 0.9f, 0.2f), new Vector3(0.08f, 1.35f, 0.08f), brass).transform;
                staff.rotation = Quaternion.Euler(0f, 0f, -24f);
            }

            Vector3 talkPosition = position + Vector3.up * 0.95f + npc.transform.forward * 0.55f;
            DungeonInteractable3D talk = CreateInvisibleInteractableBox("Anciano del Principio Talk", talkPosition, new Vector3(2.7f, 2.1f, 2.7f), brass);
            Renderer triggerRenderer = talk.GetComponent<Renderer>();
            if (triggerRenderer) triggerRenderer.enabled = false;
            talk.ConfigureNpc("El fuego no te salva del miedo, solo te da otra oportunidad. Si cruzas la niebla, termina el trabajo antes de buscar la torre.");
        }

        private bool AttachAncianoNpcModel(Transform npcTransform)
        {
            const string modelPath = "Characters/NPC/AncianoDelPrincipio/anciano_del_principio";
            GameObject modelPrefab = Resources.Load<GameObject>(modelPath);
            if (!modelPrefab) return false;

            GameObject visual = Object.Instantiate(modelPrefab, npcTransform);
            visual.name = "Anciano del Principio Model";
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.identity;
            visual.transform.localScale = Vector3.one;

            Collider[] modelColliders = visual.GetComponentsInChildren<Collider>(true);
            foreach (Collider modelCollider in modelColliders)
            {
                Object.Destroy(modelCollider);
            }

            Renderer[] renderers = visual.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
            {
                Object.Destroy(visual);
                return false;
            }

            FitModelToHeightOnFloor(visual, renderers, 1.82f, npcTransform.position);
            ApplyAncianoPalette(visual, npcTransform);

            Animator animator = visual.GetComponentInChildren<Animator>();
            if (animator)
            {
                animator.applyRootMotion = false;
                animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            }

            foreach (Renderer renderer in renderers)
            {
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                renderer.receiveShadows = true;
                if (renderer is SkinnedMeshRenderer skinnedMeshRenderer)
                {
                    skinnedMeshRenderer.updateWhenOffscreen = true;
                }
            }

            return true;
        }

        private void ApplyAncianoPalette(GameObject visual, Transform npcTransform)
        {
            Material robe = NewMaterial("DK3D Anciano Deep Black Robe", new Color(0.025f, 0.026f, 0.024f));
            Material skin = NewMaterial("DK3D Anciano Warm Skin", new Color(0.72f, 0.52f, 0.4f));
            Material beard = NewMaterial("DK3D Anciano Silver Beard", new Color(0.58f, 0.56f, 0.52f));
            Material sash = NewMaterial("DK3D Anciano Dark Sash", new Color(0.11f, 0.095f, 0.12f));

            Renderer[] renderers = visual.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                Material[] materials = renderer.materials;
                for (int i = 0; i < materials.Length; i++)
                {
                    string materialName = materials[i].name.ToLowerInvariant();
                    if (materialName.Contains("skin") || materialName.Contains("head") || materialName.Contains("hand") || materialName.Contains("face"))
                    {
                        SetMaterialColor(materials[i], skin.color);
                    }
                    else if (materialName.Contains("hair") || materialName.Contains("beard") || materialName.Contains("brow"))
                    {
                        SetMaterialColor(materials[i], beard.color);
                    }
                    else if (materialName.Contains("belt") || materialName.Contains("sash"))
                    {
                        SetMaterialColor(materials[i], sash.color);
                    }
                    else
                    {
                        SetMaterialColor(materials[i], robe.color);
                    }
                }

                renderer.materials = materials;
            }

            Transform head = FindModelBone(visual.transform, "head");
            Transform leftHand = FindModelBone(visual.transform, "lefthand");
            Transform rightHand = FindModelBone(visual.transform, "righthand");

            Vector3 forward = npcTransform.forward;
            Vector3 root = npcTransform.position;
            Vector3 headPosition = head ? head.position + Vector3.up * 0.1f + forward * 0.025f : root + Vector3.up * 1.62f + forward * 0.04f;
            Vector3 beardPosition = head ? head.position + Vector3.down * 0.13f + forward * 0.08f : root + Vector3.up * 1.42f + forward * 0.08f;
            Vector3 leftHandPosition = leftHand ? leftHand.position : root + npcTransform.right * -0.72f + Vector3.up * 1.18f;
            Vector3 rightHandPosition = rightHand ? rightHand.position : root + npcTransform.right * 0.72f + Vector3.up * 1.18f;

            CreateAncianoAccent("Anciano Bald Head Color", PrimitiveType.Sphere, npcTransform, headPosition, Quaternion.identity, new Vector3(0.2f, 0.24f, 0.19f), skin);
            CreateAncianoAccent("Anciano Left Hand Color", PrimitiveType.Sphere, npcTransform, leftHandPosition, Quaternion.identity, new Vector3(0.13f, 0.08f, 0.1f), skin);
            CreateAncianoAccent("Anciano Right Hand Color", PrimitiveType.Sphere, npcTransform, rightHandPosition, Quaternion.identity, new Vector3(0.13f, 0.08f, 0.1f), skin);
            CreateAncianoAccent("Anciano Long Silver Beard", PrimitiveType.Capsule, npcTransform, beardPosition, Quaternion.identity, new Vector3(0.16f, 0.33f, 0.11f), beard);
            CreateAncianoAccent("Anciano Cloth Sash", PrimitiveType.Cube, npcTransform, root + Vector3.up * 0.88f + forward * 0.2f, npcTransform.rotation, new Vector3(0.72f, 0.12f, 0.08f), sash);
        }

        private static void SetMaterialColor(Material material, Color color)
        {
            material.color = color;
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", 0.18f);
            if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", 0f);
        }

        private static Transform FindModelBone(Transform root, string boneName)
        {
            Transform[] transforms = root.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in transforms)
            {
                string childName = child.name.ToLowerInvariant();
                if (childName == boneName || childName.EndsWith(":" + boneName) || childName.EndsWith("_" + boneName))
                {
                    return child;
                }
            }

            return null;
        }

        private static void CreateAncianoAccent(string name, PrimitiveType primitiveType, Transform parent, Vector3 worldPosition, Quaternion worldRotation, Vector3 worldScale, Material material)
        {
            GameObject accent = GameObject.CreatePrimitive(primitiveType);
            accent.name = name;
            accent.transform.position = worldPosition;
            accent.transform.rotation = worldRotation;
            accent.transform.localScale = worldScale;
            accent.transform.SetParent(parent, true);

            Renderer renderer = accent.GetComponent<Renderer>();
            renderer.material = material;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            renderer.receiveShadows = true;

            Collider collider = accent.GetComponent<Collider>();
            if (collider) Object.Destroy(collider);
        }

        private static void FitModelToHeightOnFloor(GameObject visual, Renderer[] renderers, float targetHeight, Vector3 floorPosition)
        {
            Bounds bounds = GetRendererBounds(renderers);
            if (bounds.size.y > 0.001f)
            {
                visual.transform.localScale *= targetHeight / bounds.size.y;
            }

            bounds = GetRendererBounds(renderers);
            Vector3 correction = new Vector3(floorPosition.x - bounds.center.x, floorPosition.y - bounds.min.y, floorPosition.z - bounds.center.z);
            visual.transform.position += correction;
        }

        private static Bounds GetRendererBounds(Renderer[] renderers)
        {
            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            return bounds;
        }

        private GameObject CreateFogWall(string name, Vector3 position, Vector3 scale, bool solid)
        {
            GameObject fog = CreateBox(name, position, scale, mist);
            BoxCollider collider = fog.GetComponent<BoxCollider>();
            collider.isTrigger = !solid;
            Light light = fog.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(0.62f, 0.68f, 1f);
            light.intensity = solid ? 0.7f : 0.45f;
            light.range = 4.2f;
            return fog;
        }

        private GameObject CreateEnemy(string name, Vector3 position, PlayerController3D player, int hp, int damage, float speed, bool dropsKey)
        {
            return CreateEnemy(name, position, player, hp, damage, speed, dropsKey, false, Mathf.Max(4, hp / 6));
        }

        private GameObject CreateEnemy(string name, Vector3 position, PlayerController3D player, int hp, int damage, float speed, bool dropsKey, bool dropsTowerKey, int soulReward)
        {
            GameObject enemyObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            enemyObject.name = name;
            enemyObject.transform.position = position;
            enemyObject.GetComponent<Renderer>().material = enemy;
            Object.Destroy(enemyObject.GetComponent<CapsuleCollider>());
            CharacterController controller = enemyObject.AddComponent<CharacterController>();
            controller.height = 2f;
            controller.radius = 0.42f;
            controller.center = Vector3.zero;
            bool hasRiggedSkeleton = RiggedSkeletonEnemyVisual3D.TryAttach(enemyObject.transform);
            if (!hasRiggedSkeleton)
            {
                AttachSkeletonSprite(enemyObject.transform);
            }
            else
            {
                Renderer capsuleRenderer = enemyObject.GetComponent<Renderer>();
                if (capsuleRenderer) capsuleRenderer.enabled = false;
            }

            enemyObject.AddComponent<DungeonEnemy3D>().Configure(player, hp, damage, speed, dropsKey, dropsTowerKey, soulReward);
            return enemyObject;
        }

        private void AttachSkeletonSprite(Transform enemyTransform)
        {
            if (!skeletonEnemySprite) return;

            Renderer capsuleRenderer = enemyTransform.GetComponent<Renderer>();
            if (capsuleRenderer) capsuleRenderer.enabled = false;

            GameObject visual = new GameObject("Skeleton Sprite Visual");
            visual.transform.SetParent(enemyTransform, false);
            visual.transform.localPosition = new Vector3(0f, 0.05f, 0f);
            visual.transform.localScale = Vector3.one * 1.68f;

            SpriteRenderer spriteRenderer = visual.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = skeletonEnemySprite;
            spriteRenderer.sortingOrder = 2;
            spriteRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            spriteRenderer.receiveShadows = false;
            visual.AddComponent<SkeletonEnemyVisual3D>().Configure(skeletonEnemySprite, skeletonAttackSprites, skeletonWalkFrontSprites, skeletonWalkLeftSprites, skeletonWalkRightSprites);
        }

        private bool AttachPlayerModel(Transform playerTransform, PlayerController3D controller)
        {
            const string modelPath = "Characters/Player/DarkKnight3D/Rigged/dark_knight_tpose_mixamo_rigged";
            GameObject modelPrefab = Resources.Load<GameObject>(modelPath);
            if (!modelPrefab) return false;

            Renderer capsuleRenderer = playerTransform.GetComponent<Renderer>();
            if (capsuleRenderer) capsuleRenderer.enabled = false;

            GameObject visual = Object.Instantiate(modelPrefab, playerTransform);
            visual.name = "Dark Knight 3D Model Visual";
            visual.transform.localPosition = new Vector3(0f, -1f, 0f);
            visual.transform.localRotation = Quaternion.identity;
            visual.transform.localScale = Vector3.one * 100f;

            Renderer[] renderers = visual.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
            {
                Object.Destroy(visual);
                return false;
            }

            Animator animator = visual.GetComponentInChildren<Animator>();
            if (!animator)
            {
                animator = visual.AddComponent<Animator>();
            }

            animator.applyRootMotion = false;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            visual.AddComponent<PlayerModelVisual3D>().Configure(controller, animator);

            foreach (Renderer renderer in renderers)
            {
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                renderer.receiveShadows = true;
                if (renderer is SkinnedMeshRenderer skinnedMeshRenderer)
                {
                    skinnedMeshRenderer.updateWhenOffscreen = true;
                }
            }

            return true;
        }

        private bool AttachPlayerSprite(Transform playerTransform, PlayerController3D controller)
        {
            if (!playerIdleSprite) return false;

            Renderer capsuleRenderer = playerTransform.GetComponent<Renderer>();
            if (capsuleRenderer) capsuleRenderer.enabled = false;

            GameObject visual = new GameObject("Dark Knight Sprite Visual");
            visual.transform.SetParent(playerTransform, false);
            visual.transform.localPosition = new Vector3(0f, 0.1f, 0f);

            SpriteRenderer spriteRenderer = visual.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = playerIdleSprite;
            spriteRenderer.sortingOrder = 4;
            spriteRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            spriteRenderer.receiveShadows = false;
            visual.AddComponent<PlayerVisual3D>().Configure(
                controller,
                playerIdleSprite,
                playerWalkFrontSprites,
                playerWalkLeftSprites,
                playerWalkRightSprites,
                playerWalkBackSprites,
                playerRollFrontSprites,
                playerRollLeftSprites,
                playerRollRightSprites,
                playerRollBackSprites,
                playerAttackFrontSprites,
                playerAttackBackSprites,
                playerAttackLeftSprites,
                playerAttackRightSprites,
                playerBlockSprites,
                playerBlockWalkFrontSprites,
                playerBlockWalkLeftSprites,
                playerBlockWalkRightSprites,
                playerBlockWalkBackSprites);

            return true;
        }

        private static Sprite LoadSpriteAsset(string resourcePath)
        {
            Sprite sprite = Resources.Load<Sprite>(resourcePath);
            if (sprite) return sprite;

            Texture2D texture = Resources.Load<Texture2D>(resourcePath);
            if (!texture) return null;

            return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 220f);
        }

        private static Sprite[] LoadSpriteSequence(string resourcePrefix, int count, string resourceSuffix = "")
        {
            Sprite[] sprites = new Sprite[count];
            for (int i = 0; i < count; i++)
            {
                sprites[i] = LoadSpriteAsset($"{resourcePrefix}{i + 1:00}{resourceSuffix}");
            }

            return sprites;
        }

        private void CreateStairRun(string name, Vector3 start, int steps, Vector3 stepOffset, float width)
        {
            for (int i = 0; i < steps; i++)
            {
                Vector3 position = start + stepOffset * i;
                Vector3 scale = Mathf.Abs(stepOffset.x) > Mathf.Abs(stepOffset.z)
                    ? new Vector3(Mathf.Abs(stepOffset.x) + 0.35f, 0.28f, width)
                    : new Vector3(width, 0.28f, Mathf.Abs(stepOffset.z) + 0.35f);
                CreateBox($"{name} {i + 1}", position, scale, stone);
            }
        }

        private void CreateBridgeRailing(Vector3 origin, int count)
        {
            for (int i = 0; i < count; i++)
            {
                CreateBox("Stone Railing Post", origin + Vector3.forward * (i * 1.15f), new Vector3(0.28f, 1.15f, 0.28f), darkStone);
            }
        }

        private void CreateMovingPlatform(string name, Vector3 position, Vector3 scale, Vector3 travel, float speed)
        {
            GameObject platform = CreateBox(name, position, scale, stone);
            platform.AddComponent<DungeonMovingPlatform3D>().Configure(travel, speed);
        }

        private void CreateFireTrap(string name, Vector3 position, Vector3 scale)
        {
            GameObject fire = CreateHazardBox(name, position, scale, 16, "Llamas del castillo.");
            fire.GetComponent<Renderer>().material = ember;

            Light light = fire.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1f, 0.34f, 0.12f);
            light.intensity = 2f;
            light.range = 5f;
        }

        private void CreateBladeTrap(string name, Vector3 position, Vector3 spin, Vector3 bob)
        {
            GameObject blade = CreateHazardBox(name, position, new Vector3(3.6f, 0.18f, 0.42f), 22, "Una cuchilla te alcanza.");
            blade.GetComponent<Renderer>().material = hazard;
            blade.GetComponent<DungeonHazard3D>().Configure(22, "Una cuchilla te alcanza.", spin, bob, 2.1f);
        }

        private GameObject CreateHazardBox(string name, Vector3 position, Vector3 scale, int damage, string message)
        {
            GameObject box = CreateBox(name, position, scale, hazard);
            BoxCollider collider = box.GetComponent<BoxCollider>();
            collider.isTrigger = true;
            box.AddComponent<DungeonHazard3D>().Configure(damage, message, Vector3.zero, Vector3.zero, 1f);
            return box;
        }

        private DungeonInteractable3D CreateInteractableBox(string name, Vector3 position, Vector3 scale, Material material)
        {
            GameObject interactable = CreateBox(name, position, scale, material);
            interactable.GetComponent<BoxCollider>().isTrigger = true;
            return interactable.AddComponent<DungeonInteractable3D>();
        }

        private DungeonInteractable3D CreateInvisibleInteractableBox(string name, Vector3 position, Vector3 scale, Material material)
        {
            DungeonInteractable3D interactable = CreateInteractableBox(name, position, scale, material);
            Renderer renderer = interactable.GetComponent<Renderer>();
            if (renderer) renderer.enabled = false;
            return interactable;
        }

        private Transform CreateGothicDoor(string name, Vector3 position, float width, float height)
        {
            GameObject root = new GameObject(name);
            root.transform.position = position;

            GameObject blocker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            blocker.name = $"{name} Solid Blocker";
            blocker.transform.SetParent(root.transform, false);
            blocker.transform.localPosition = Vector3.zero;
            blocker.transform.localScale = new Vector3(width * 0.82f, height * 0.92f, 0.36f);
            Renderer blockerRenderer = blocker.GetComponent<Renderer>();
            if (blockerRenderer) blockerRenderer.enabled = false;

            CreateLocalBox(root.transform, "Door Darkness", new Vector3(0f, -0.1f, 0.06f), new Vector3(width * 0.56f, height * 0.78f, 0.08f), shadowStone);
            CreateLocalBox(root.transform, "Left Stone Pillar", new Vector3(-width * 0.42f, -0.08f, 0f), new Vector3(0.42f, height * 0.9f, 0.52f), darkStone);
            CreateLocalBox(root.transform, "Right Stone Pillar", new Vector3(width * 0.42f, -0.08f, 0f), new Vector3(0.42f, height * 0.9f, 0.52f), darkStone);
            CreateLocalBox(root.transform, "Stone Threshold", new Vector3(0f, -height * 0.49f, -0.02f), new Vector3(width * 0.86f, 0.24f, 0.58f), darkStone);

            GameObject leftArch = CreateLocalBox(root.transform, "Left Pointed Arch", new Vector3(-width * 0.19f, height * 0.38f, 0f), new Vector3(width * 0.47f, 0.28f, 0.52f), darkStone);
            leftArch.transform.localRotation = Quaternion.Euler(0f, 0f, 32f);
            GameObject rightArch = CreateLocalBox(root.transform, "Right Pointed Arch", new Vector3(width * 0.19f, height * 0.38f, 0f), new Vector3(width * 0.47f, 0.28f, 0.52f), darkStone);
            rightArch.transform.localRotation = Quaternion.Euler(0f, 0f, -32f);

            CreateLocalBox(root.transform, "Left Wood Door", new Vector3(-width * 0.14f, -0.18f, -0.08f), new Vector3(width * 0.27f, height * 0.72f, 0.16f), chestWood);
            CreateLocalBox(root.transform, "Right Wood Door", new Vector3(width * 0.14f, -0.18f, -0.08f), new Vector3(width * 0.27f, height * 0.72f, 0.16f), chestWood);
            CreateLocalBox(root.transform, "Door Center Gap", new Vector3(0f, -0.18f, -0.19f), new Vector3(0.08f, height * 0.72f, 0.05f), shadowStone);

            CreateLocalBox(root.transform, "Door Top Iron Strap", new Vector3(0f, height * 0.08f, -0.2f), new Vector3(width * 0.58f, 0.08f, 0.08f), shadowStone);
            CreateLocalBox(root.transform, "Door Bottom Iron Strap", new Vector3(0f, -height * 0.29f, -0.2f), new Vector3(width * 0.58f, 0.08f, 0.08f), shadowStone);
            CreateLocalBox(root.transform, "Door Lock Plate", new Vector3(0f, -0.12f, -0.24f), new Vector3(0.32f, 0.42f, 0.08f), chestGold);
            CreateLocalBox(root.transform, "Left Door Handle", new Vector3(-0.23f, -0.1f, -0.29f), new Vector3(0.18f, 0.08f, 0.08f), shadowStone);
            CreateLocalBox(root.transform, "Right Door Handle", new Vector3(0.23f, -0.1f, -0.29f), new Vector3(0.18f, 0.08f, 0.08f), shadowStone);

            return root.transform;
        }

        private void CreateChest(string name, Vector3 position, int souls)
        {
            DungeonInteractable3D trigger = CreateInteractableBox($"{name} Trigger", position, new Vector3(1.6f, 1.05f, 1.25f), brass);
            Renderer triggerRenderer = trigger.GetComponent<Renderer>();
            if (triggerRenderer) triggerRenderer.enabled = false;

            GameObject root = new GameObject(name);
            root.transform.position = position;

            CreateLocalBox(root.transform, "Chest Body", new Vector3(0f, -0.12f, 0f), new Vector3(1.28f, 0.48f, 0.86f), chestWood);
            CreateLocalBox(root.transform, "Chest Front Gold Band", new Vector3(0f, 0.03f, -0.45f), new Vector3(1.34f, 0.12f, 0.06f), chestGold);
            CreateLocalBox(root.transform, "Chest Back Gold Band", new Vector3(0f, 0.03f, 0.45f), new Vector3(1.34f, 0.12f, 0.06f), chestGold);
            CreateLocalBox(root.transform, "Chest Left Gold Band", new Vector3(-0.67f, 0.03f, 0f), new Vector3(0.06f, 0.12f, 0.86f), chestGold);
            CreateLocalBox(root.transform, "Chest Right Gold Band", new Vector3(0.67f, 0.03f, 0f), new Vector3(0.06f, 0.12f, 0.86f), chestGold);
            CreateLocalBox(root.transform, "Chest Lock", new Vector3(0f, 0.02f, -0.51f), new Vector3(0.24f, 0.28f, 0.08f), chestGold);

            GameObject lidPivot = new GameObject("Chest Lid Pivot");
            lidPivot.transform.SetParent(root.transform, false);
            lidPivot.transform.localPosition = new Vector3(0f, 0.18f, 0.43f);
            CreateLocalBox(lidPivot.transform, "Chest Lid", new Vector3(0f, 0.12f, -0.42f), new Vector3(1.34f, 0.24f, 0.88f), chestWood);
            CreateLocalBox(lidPivot.transform, "Chest Lid Front Trim", new Vector3(0f, 0.25f, -0.88f), new Vector3(1.36f, 0.08f, 0.06f), chestGold);
            CreateLocalBox(lidPivot.transform, "Chest Lid Top Trim", new Vector3(0f, 0.27f, -0.42f), new Vector3(1.38f, 0.07f, 0.1f), chestGold);

            GameObject treasure = new GameObject("Chest Treasure");
            treasure.transform.SetParent(root.transform, false);
            treasure.transform.localPosition = new Vector3(0f, 0.18f, -0.06f);
            CreateCoinPile(treasure.transform);

            Light light = treasure.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1f, 0.72f, 0.2f);
            light.range = 3.8f;
            light.intensity = 1.2f;

            DungeonChestVisual3D visual = root.AddComponent<DungeonChestVisual3D>();
            visual.Configure(lidPivot.transform, treasure, light);
            trigger.ConfigureChest(souls, visual);
        }

        private GameObject CreateLocalBox(Transform parent, string name, Vector3 localPosition, Vector3 localScale, Material material)
        {
            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.name = name;
            box.transform.SetParent(parent, false);
            box.transform.localPosition = localPosition;
            box.transform.localScale = localScale;
            box.GetComponent<Renderer>().material = material;
            Object.Destroy(box.GetComponent<Collider>());
            return box;
        }

        private void CreateCoinPile(Transform parent)
        {
            for (int i = 0; i < 9; i++)
            {
                float x = -0.36f + (i % 3) * 0.24f;
                float z = -0.2f + (i / 3) * 0.17f;
                GameObject coin = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                coin.name = "Chest Coin";
                coin.transform.SetParent(parent, false);
                coin.transform.localPosition = new Vector3(x, 0.02f + i * 0.006f, z);
                coin.transform.localRotation = Quaternion.Euler(90f, 0f, Random.Range(-8f, 8f));
                coin.transform.localScale = new Vector3(0.085f, 0.018f, 0.085f);
                coin.GetComponent<Renderer>().material = chestGold;
                Object.Destroy(coin.GetComponent<Collider>());
            }

            CreateGem(parent, "Chest Blue Gem", new Vector3(-0.16f, 0.14f, -0.04f), gemBlue);
            CreateGem(parent, "Chest Red Gem", new Vector3(0.22f, 0.12f, 0.08f), gemRed);
        }

        private void CreateGem(Transform parent, string name, Vector3 localPosition, Material material)
        {
            GameObject gem = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            gem.name = name;
            gem.transform.SetParent(parent, false);
            gem.transform.localPosition = localPosition;
            gem.transform.localRotation = Quaternion.Euler(18f, 35f, 0f);
            gem.transform.localScale = Vector3.one * 0.12f;
            gem.GetComponent<Renderer>().material = material;
            Object.Destroy(gem.GetComponent<Collider>());
        }

        private void CreateDoor(string name, Vector3 position, Vector3 scale, Vector3 destination, Vector3 facing, string travelText)
        {
            DungeonInteractable3D door = CreateInteractableBox(name, position, scale, darkStone);
            door.ConfigureDoor(destination, facing, travelText);
            CreateBox($"{name} Frame Top", position + Vector3.up * (scale.y * 0.5f + 0.22f), new Vector3(scale.x + 0.55f, 0.35f, scale.z + 0.2f), brass);
            CreateBox($"{name} Frame Left", position + Vector3.left * (scale.x * 0.5f + 0.18f), new Vector3(0.28f, scale.y + 0.45f, scale.z + 0.2f), brass);
            CreateBox($"{name} Frame Right", position + Vector3.right * (scale.x * 0.5f + 0.18f), new Vector3(0.28f, scale.y + 0.45f, scale.z + 0.2f), brass);
        }

        private void CreatePickup(string name, Vector3 position, bool coin)
        {
            GameObject pickup = GameObject.CreatePrimitive(coin ? PrimitiveType.Cylinder : PrimitiveType.Sphere);
            pickup.name = name;
            pickup.transform.position = position;
            pickup.transform.localScale = coin ? new Vector3(0.45f, 0.08f, 0.45f) : new Vector3(0.55f, 0.55f, 0.55f);
            pickup.GetComponent<Renderer>().material = coin ? brass : potion;
            Collider collider = pickup.GetComponent<Collider>();
            collider.isTrigger = true;
            DungeonPickup3D pickupScript = pickup.AddComponent<DungeonPickup3D>();
            if (coin) pickupScript.ConfigureCoin(3);
            else pickupScript.ConfigurePotion();
        }

        private void CreateColumn(Vector3 position)
        {
            GameObject column = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            column.name = "Stone Column";
            column.transform.position = position;
            column.transform.localScale = new Vector3(0.55f, 1.7f, 0.55f);
            column.GetComponent<Renderer>().material = darkStone;
        }

        private void CreateTorch(Vector3 position)
        {
            float side = position.x < 0f ? 1f : -1f;
            Vector3 inward = Vector3.right * side;

            GameObject torch = new GameObject("Animated Wall Torch");
            torch.transform.position = position;

            GameObject plate = CreateBox("Wall Torch Plate", position - inward * 0.04f, new Vector3(0.12f, 0.68f, 0.5f), brass);
            plate.transform.SetParent(torch.transform, true);
            Object.Destroy(plate.GetComponent<Collider>());

            GameObject bracket = CreateBox("Wall Torch Bracket", position + inward * 0.16f - Vector3.up * 0.02f, new Vector3(0.42f, 0.11f, 0.11f), brass);
            bracket.transform.SetParent(torch.transform, true);
            Object.Destroy(bracket.GetComponent<Collider>());

            GameObject handle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            handle.name = "Wall Torch Handle";
            handle.transform.SetParent(torch.transform, true);
            handle.transform.position = position + inward * 0.32f - Vector3.up * 0.26f;
            handle.transform.rotation = Quaternion.FromToRotation(Vector3.up, (inward * 0.46f + Vector3.down * 0.68f).normalized);
            handle.transform.localScale = new Vector3(0.055f, 0.42f, 0.055f);
            handle.GetComponent<Renderer>().material = brass;
            Object.Destroy(handle.GetComponent<Collider>());

            GameObject coal = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            coal.name = "Wall Torch Ember Core";
            coal.transform.SetParent(torch.transform, true);
            coal.transform.position = position + inward * 0.5f + Vector3.up * 0.17f;
            coal.transform.localScale = new Vector3(0.24f, 0.16f, 0.24f);
            coal.GetComponent<Renderer>().material = ember;
            Object.Destroy(coal.GetComponent<Collider>());

            GameObject flame = new GameObject("Torch Flame");
            flame.transform.SetParent(torch.transform, true);
            flame.transform.position = position + inward * 0.5f + Vector3.up * 0.22f;
            flame.transform.localScale = Vector3.one * 1.08f;
            flame.AddComponent<SpriteRenderer>();

            Light light = flame.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1f, 0.48f, 0.18f);
            light.intensity = 2.1f;
            light.range = 6f;

            flame.AddComponent<AnimatedWallTorch3D>().Configure(light, inward);
        }

        private void CreateSpikeRow(Vector3 origin, int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject spike = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                spike.name = "Floor Spike";
                spike.transform.position = origin + new Vector3((i - count * 0.5f) * 0.62f, 0.35f, 0f);
                spike.transform.localScale = new Vector3(0.26f, 0.52f, 0.26f);
                spike.GetComponent<Renderer>().material = brass;
            }
        }

        private void CreateWeaponVisual(Transform parent)
        {
            GameObject sword = CreateBox("Knight Sword", new Vector3(0.48f, 1.12f, 0.42f), new Vector3(0.12f, 1.1f, 0.12f), brass);
            sword.transform.SetParent(parent, false);
            sword.transform.localRotation = Quaternion.Euler(24f, 0f, -22f);
        }

        private void CreateShieldVisual(Transform parent)
        {
            GameObject shield = CreateBox("Knight Shield", new Vector3(-0.48f, 1.08f, 0.34f), new Vector3(0.15f, 0.85f, 0.65f), darkStone);
            shield.transform.SetParent(parent, false);
        }

        private GameObject CreateBox(string name, Vector3 position, Vector3 scale, Material material)
        {
            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.name = name;
            box.transform.position = position;
            box.transform.localScale = scale;
            Renderer renderer = box.GetComponent<Renderer>();
            renderer.material = material;
            ApplyBoxTextureTiling(renderer, scale);
            return box;
        }

        private static Material NewMaterial(string name, Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (!shader) shader = Shader.Find("Standard");
            if (!shader) shader = Shader.Find("Diffuse");
            Material material = new Material(shader);
            material.name = name;
            material.color = color;
            return material;
        }

        private static Material NewTexturedMaterial(string name, Color color, Texture2D texture)
        {
            Material material = NewMaterial(name, color);
            if (!texture) return material;

            texture.wrapMode = TextureWrapMode.Repeat;
            texture.filterMode = FilterMode.Bilinear;
            material.mainTexture = texture;
            material.mainTextureScale = Vector2.one;
            return material;
        }

        private static Material NewTransparentMaterial(string name, Color color)
        {
            Material material = NewMaterial(name, color);
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Surface")) material.SetFloat("_Surface", 1f);
            if (material.HasProperty("_Blend")) material.SetFloat("_Blend", 0f);
            if (material.HasProperty("_SrcBlend")) material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            if (material.HasProperty("_DstBlend")) material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            if (material.HasProperty("_ZWrite")) material.SetFloat("_ZWrite", 0f);
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            return material;
        }

        private static void ApplyBoxTextureTiling(Renderer renderer, Vector3 scale)
        {
            if (!renderer || !renderer.material || !renderer.material.mainTexture) return;

            Vector2 tiling = new Vector2(
                Mathf.Max(1f, Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.z)) * 0.45f),
                Mathf.Max(1f, Mathf.Abs(scale.y) * 0.65f)
            );

            if (Mathf.Abs(scale.y) <= Mathf.Abs(scale.x) * 0.2f && Mathf.Abs(scale.y) <= Mathf.Abs(scale.z) * 0.2f)
            {
                tiling = new Vector2(Mathf.Max(1f, Mathf.Abs(scale.x) * 0.45f), Mathf.Max(1f, Mathf.Abs(scale.z) * 0.45f));
            }

            renderer.material.mainTextureScale = tiling;
        }
    }
}
