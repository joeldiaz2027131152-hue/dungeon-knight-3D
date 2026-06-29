using DungeonKnight.Player;
using DungeonKnight.UI;
using UnityEngine;

namespace DungeonKnight.Level
{
    public class DungeonKnight3DBootstrap : MonoBehaviour
    {
        public const string GeneratedRootName = "Dungeon Knight 3D Generated World";
        public static readonly Vector3 PlayerSpawn = new Vector3(0f, 1.05f, -16f);
        private const bool EnemyEncountersEnabled = false;
        private const bool EnemySetDressingEnabled = false;

        private static bool built;
        [SerializeField] private bool quietEditorPreview;
        private Transform generatedRoot;
        private Material stone;
        private Material darkStone;
        private Material brass;
        private Material ember;
        private Material enemy;
        private Material playerBody;
        private Material potion;
        private Material exteriorStone;
        private Material floorStone;
        private Material wallStone;
        private Material vineWallStone;
        private Material shadowStone;
        private Material hazard;
        private Material wood;
        private Material ashStone;
        private Material charcoal;
        private Material mist;
        private Material chestWood;
        private Material chestGold;
        private Material chestIron;
        private Material chestTrim;
        private Material chestGlow;
        private Material gothicDoorWood;
        private Material gothicDoorWoodDark;
        private Material gothicDoorIron;
        private Material gothicDoorIronHighlight;
        private Material runestoneFace;
        private Material runestoneEdge;
        private Material runestoneRune;
        private Material wallTorchIron;
        private Material wallTorchWood;
        private Material wallTorchStone;
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
            PlayerController3D existingPlayer = Object.FindAnyObjectByType<PlayerController3D>();
            if (existingPlayer)
            {
                built = true;
                RepairPlayerRuntimeSetup(existingPlayer, false);
                return;
            }

            if (built) return;
            built = true;
            CreateBootstrap().Build();
        }

        public static PlayerController3D RepairPlayerRuntimeSetup(bool resetHeightToSpawn)
        {
            PlayerController3D player = Object.FindAnyObjectByType<PlayerController3D>();
            if (!player) return null;

            RepairPlayerRuntimeSetup(player, resetHeightToSpawn);
            return player;
        }

        public static void BuildEditableScene()
        {
            ClearEditableScene();
            built = true;
            DungeonKnight3DBootstrap bootstrap = CreateBootstrap();
            bootstrap.quietEditorPreview = true;
            bootstrap.Build();
        }

        public static void ClearEditableScene()
        {
            built = false;
            GameObject existingRoot = GameObject.Find(GeneratedRootName);
            if (!existingRoot) return;

            if (Application.isPlaying) DestroySafely(existingRoot);
            else Object.DestroyImmediate(existingRoot);
        }

        public static void ApplyQuietEditorPreviewToScene()
        {
            GameObject existingRoot = GameObject.Find(GeneratedRootName);
            if (!existingRoot) return;

            foreach (DungeonKnight3DEditorPreviewRenderer previewRenderer in existingRoot.GetComponentsInChildren<DungeonKnight3DEditorPreviewRenderer>(true))
            {
                previewRenderer.RestoreNow();
            }

            foreach (AnimatedWallTorch3D torch in existingRoot.GetComponentsInChildren<AnimatedWallTorch3D>(true))
            {
                torch.SetEditorPreviewHidden(true);
            }

            foreach (Light light in existingRoot.GetComponentsInChildren<Light>(true))
            {
                if (light.type == LightType.Directional) continue;
                DungeonKnight3DEditorPreviewLight previewLight = light.GetComponent<DungeonKnight3DEditorPreviewLight>();
                if (!previewLight) previewLight = light.gameObject.AddComponent<DungeonKnight3DEditorPreviewLight>();
                previewLight.Configure(light);
            }
        }

        public static void ApplySelectionRootsToScene()
        {
            GameObject existingRoot = GameObject.Find(GeneratedRootName);
            if (!existingRoot) return;

            foreach (Transform child in existingRoot.GetComponentsInChildren<Transform>(true))
            {
                if (child == existingRoot.transform || child.childCount == 0) continue;
                if (child.GetComponent<DungeonKnight3DSelectionRoot>()) continue;

                string childName = child.name;
                bool isUsefulGroup =
                    childName.Contains("Chest") ||
                    childName.Contains("Bonfire") ||
                    childName.Contains("Torch") ||
                    childName.Contains("Stair") ||
                    childName.Contains("Railing") ||
                    childName.Contains("Door") ||
                    childName.Contains("Metal Bar Row") ||
                    childName.Contains("Elevator");

                if (isUsefulGroup)
                {
                    child.gameObject.AddComponent<DungeonKnight3DSelectionRoot>();
                }
            }
        }

        private static DungeonKnight3DBootstrap CreateBootstrap()
        {
            GameObject root = new GameObject(GeneratedRootName);
            GameObject bootstrapObject = new GameObject("Dungeon Knight 3D Bootstrap");
            bootstrapObject.transform.SetParent(root.transform, false);

            DungeonKnight3DBootstrap bootstrap = bootstrapObject.AddComponent<DungeonKnight3DBootstrap>();
            bootstrap.generatedRoot = root.transform;
            return bootstrap;
        }

        private static void RepairPlayerRuntimeSetup(PlayerController3D player, bool resetHeightToSpawn)
        {
            player.enabled = true;
            if (!player.GetComponent<PlayerInventory>()) player.gameObject.AddComponent<PlayerInventory>();

            CharacterController controller = player.GetComponent<CharacterController>();
            if (!controller) controller = player.gameObject.AddComponent<CharacterController>();
            controller.enabled = true;
            controller.height = 2.1f;
            controller.radius = 0.42f;
            controller.center = Vector3.zero;
            controller.stepOffset = 0.3f;

            foreach (CapsuleCollider capsuleCollider in player.GetComponents<CapsuleCollider>())
            {
                if (Application.isPlaying) DestroySafely(capsuleCollider);
                else Object.DestroyImmediate(capsuleCollider);
            }

            Vector3 position = player.transform.position;
            bool heightLooksBroken = position.y < 0.35f || position.y > 2.5f;
            if (resetHeightToSpawn || heightLooksBroken)
            {
                player.transform.position = new Vector3(position.x, PlayerSpawn.y, position.z);
            }

            player.transform.localScale = new Vector3(0.82f, 1.05f, 0.82f);
            RepairPlayerVisual(player);
            MeshRenderer playerRootRenderer = player.GetComponent<MeshRenderer>();
            if (playerRootRenderer && player.GetComponentInChildren<PlayerModelVisual3D>(true)) playerRootRenderer.enabled = false;
            ConfigureCameraForPlayer(player.transform);
            EnsureRuntimeCloseRoomCameraZones();
            EnsureHudForPlayer(player);
            RepairEnemiesRuntimeSetup(player);
        }

        private static void RepairEnemiesRuntimeSetup(PlayerController3D player)
        {
            if (!player) return;
            if (!EnemyEncountersEnabled)
            {
                RemoveEnemyActorsFromScene();
                RemoveOrphanEnemySwords();
                return;
            }

            RemoveOrphanEnemySwords();

            foreach (DungeonEnemy3D enemy in Object.FindObjectsByType<DungeonEnemy3D>(FindObjectsInactive.Include))
            {
                enemy.RepairRuntimeSetup(player);
            }
        }

        private static void RemoveEnemyActorsFromScene()
        {
            foreach (DungeonEnemy3D enemy in Object.FindObjectsByType<DungeonEnemy3D>(FindObjectsInactive.Include))
            {
                if (Application.isPlaying) DestroySafely(enemy.gameObject);
                else Object.DestroyImmediate(enemy.gameObject);
            }
        }

        private static void RemoveOrphanEnemySwords()
        {
            foreach (Transform transform in Object.FindObjectsByType<Transform>(FindObjectsInactive.Include))
            {
                if (transform.name != "Rusty Sword") continue;
                if (transform.GetComponentInParent<RiggedSkeletonEnemyVisual3D>(true)) continue;

                if (Application.isPlaying) DestroySafely(transform.gameObject);
                else Object.DestroyImmediate(transform.gameObject);
            }
        }

        private static void RepairPlayerVisual(PlayerController3D player)
        {
            Animator animator = player.GetComponentInChildren<Animator>(true);
            if (!animator) return;

            animator.enabled = true;
            animator.applyRootMotion = false;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

            PlayerModelVisual3D visual = player.GetComponentInChildren<PlayerModelVisual3D>(true);
            if (!visual) visual = animator.gameObject.AddComponent<PlayerModelVisual3D>();
            visual.enabled = true;
            visual.Configure(player, animator);

            foreach (SkinnedMeshRenderer renderer in player.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                renderer.enabled = true;
                renderer.updateWhenOffscreen = true;
            }
        }

        private static void ConfigureCameraForPlayer(Transform target)
        {
            Camera camera = Camera.main;
            if (!camera)
            {
                GameObject cameraObject = new GameObject("Main Camera");
                camera = cameraObject.AddComponent<Camera>();
                cameraObject.tag = "MainCamera";
            }

            camera.tag = "MainCamera";
            camera.transform.position = target.position + new Vector3(0f, 7.4f, -8.6f);
            camera.transform.rotation = Quaternion.Euler(48f, 0f, 0f);
            camera.fieldOfView = 58f;
            camera.nearClipPlane = 0.08f;

            CameraFollow3D follow = camera.gameObject.GetComponent<CameraFollow3D>();
            if (!follow) follow = camera.gameObject.AddComponent<CameraFollow3D>();
            follow.SetTarget(target);
        }

        private static void EnsureHudForPlayer(PlayerController3D player)
        {
            GameHud3D hud = Object.FindAnyObjectByType<GameHud3D>();
            if (!hud) hud = new GameObject("Game HUD 3D").AddComponent<GameHud3D>();
            hud.enabled = true;
            hud.Bind(player);
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
            Texture2D floorStoneTexture = Resources.Load<Texture2D>("Art/Textures/Dungeon/floor_moss_stone_tiles");
            Texture2D wallStoneTexture = Resources.Load<Texture2D>("Art/Textures/Dungeon/wall_damp_moss_stone");
            Texture2D vineWallStoneTexture = Resources.Load<Texture2D>("Art/Textures/Dungeon/wall_damp_vines_stone");
            stone = NewTexturedMaterial("DK3D Moss Stone", new Color(0.72f, 0.74f, 0.72f), dungeonStoneTexture);
            darkStone = NewTexturedMaterial("DK3D Dark Moss Stone", new Color(0.42f, 0.46f, 0.46f), dungeonStoneTexture);
            brass = NewMaterial("DK3D Old Brass", new Color(0.86f, 0.62f, 0.28f));
            ember = NewMaterial("DK3D Ember", new Color(1f, 0.26f, 0.08f));
            enemy = NewMaterial("DK3D Bone Enemy", new Color(0.82f, 0.78f, 0.66f));
            playerBody = NewMaterial("DK3D Knight Steel", new Color(0.52f, 0.58f, 0.68f));
            potion = NewMaterial("DK3D Potion", new Color(0.9f, 0.08f, 0.22f));
            exteriorStone = NewTexturedMaterial("DK3D Exterior Stone", new Color(0.54f, 0.57f, 0.56f), dungeonStoneTexture);
            floorStone = NewTexturedMaterial("DK3D Floor Stone Large", Color.white, floorStoneTexture ? floorStoneTexture : dungeonStoneTexture);
            floorStone.mainTextureScale = Vector2.one;
            wallStone = NewTexturedMaterial("DK3D Damp Moss Wall", Color.white, wallStoneTexture ? wallStoneTexture : dungeonStoneTexture);
            wallStone.mainTextureScale = Vector2.one;
            vineWallStone = NewTexturedMaterial("DK3D Damp Vine Wall", Color.white, vineWallStoneTexture ? vineWallStoneTexture : wallStoneTexture ? wallStoneTexture : dungeonStoneTexture);
            vineWallStone.mainTextureScale = Vector2.one;
            shadowStone = NewTexturedMaterial("DK3D Shadow Stone", new Color(0.16f, 0.17f, 0.19f), dungeonStoneTexture);
            hazard = NewMaterial("DK3D Hazard Iron", new Color(0.68f, 0.18f, 0.08f));
            wood = NewMaterial("DK3D Charred Wood", new Color(0.34f, 0.16f, 0.07f));
            ashStone = NewMaterial("DK3D Ash Ring Stone", new Color(0.22f, 0.22f, 0.21f));
            charcoal = NewMaterial("DK3D Hot Charcoal", new Color(0.08f, 0.045f, 0.035f));
            mist = NewTransparentMaterial("DK3D White Fog Gate", new Color(0.78f, 0.82f, 0.9f, 0.48f));
            chestWood = NewMaterial("DK3D Blue Iron Chest Shell", new Color(0.16f, 0.24f, 0.29f));
            chestGold = NewMaterial("DK3D Worn Chest Edge Metal", new Color(0.42f, 0.5f, 0.55f));
            chestIron = NewMaterial("DK3D Dark Riveted Chest Iron", new Color(0.045f, 0.06f, 0.075f));
            chestTrim = NewMaterial("DK3D Pale Chest Rivets", new Color(0.62f, 0.72f, 0.78f));
            chestGlow = NewMaterial("DK3D Chest Soul Glow", new Color(0.74f, 0.94f, 1f));
            gothicDoorWood = NewMaterial("DK3D Gothic Door Aged Wood", new Color(0.18f, 0.105f, 0.065f));
            gothicDoorWoodDark = NewMaterial("DK3D Gothic Door Dark Grain", new Color(0.075f, 0.048f, 0.035f));
            gothicDoorIron = NewMaterial("DK3D Gothic Door Black Iron", new Color(0.018f, 0.022f, 0.026f));
            gothicDoorIronHighlight = NewMaterial("DK3D Gothic Door Worn Iron Edge", new Color(0.22f, 0.25f, 0.27f));
            runestoneFace = NewTexturedMaterial("DK3D Weathered Rune Stone", new Color(0.48f, 0.49f, 0.45f), dungeonStoneTexture);
            runestoneEdge = NewTexturedMaterial("DK3D Rune Stone Dark Edge", new Color(0.18f, 0.2f, 0.19f), dungeonStoneTexture);
            runestoneRune = NewMaterial("DK3D Rune Carving Shadow", new Color(0.035f, 0.04f, 0.035f));
            wallTorchIron = NewMaterial("DK3D Black Torch Iron", new Color(0.025f, 0.025f, 0.022f));
            wallTorchWood = NewMaterial("DK3D Dark Torch Wood", new Color(0.19f, 0.105f, 0.055f));
            wallTorchStone = NewTexturedMaterial("DK3D Torch Backing Stone", new Color(0.5f, 0.48f, 0.42f), dungeonStoneTexture);
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
            SetGeneratedParent(lightObject);
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
            SetGeneratedParent(player);
            player.transform.position = PlayerSpawn;
            player.transform.localScale = new Vector3(0.82f, 1.05f, 0.82f);
            CapsuleCollider capsuleCollider = player.GetComponent<CapsuleCollider>();
            if (Application.isPlaying) DestroySafely(capsuleCollider);
            else Object.DestroyImmediate(capsuleCollider);
            player.GetComponent<Renderer>().sharedMaterial = playerBody;

            CharacterController controller = player.AddComponent<CharacterController>();
            controller.height = 2.1f;
            controller.radius = 0.42f;
            controller.center = Vector3.zero;

            player.AddComponent<PlayerInventory>();
            PlayerController3D playerController = player.AddComponent<PlayerController3D>();
            bool attachedVisual = AttachPlayerModel(player.transform, playerController);
            if (!attachedVisual) attachedVisual = AttachPlayerSprite(player.transform, playerController);

            if (!attachedVisual)
            {
                CreateWeaponVisual(player.transform);
                CreateShieldVisual(player.transform);
            }

            return playerController;
        }

        private void CreateCamera(Transform target)
        {
            ConfigureCameraForPlayer(target);
        }

        private void CreateHud(PlayerController3D player)
        {
            EnsureHudForPlayer(player);
            GameHud3D hud = Object.FindAnyObjectByType<GameHud3D>();
            if (hud) SetGeneratedParent(hud.gameObject);
        }

        private void CreateDungeonShell()
        {
            CreateBox("Main Stone Floor", new Vector3(0f, -0.25f, 0f), new Vector3(15f, 0.5f, 44f), floorStone);
            CreateBox("Left Wall", new Vector3(-7.75f, 2.5f, 0.12f), new Vector3(0.5f, 5.5f, 44.35f), vineWallStone);
            CreateBox("Right Wall", new Vector3(7.75f, 2.5f, 0.12f), new Vector3(0.5f, 5.5f, 44.35f), wallStone);
            CreateBox("Left Wall Upper Seal", new Vector3(-7.75f, 5.65f, 0.12f), new Vector3(0.7f, 0.55f, 44.35f), wallStone);
            CreateBox("Right Wall Upper Seal", new Vector3(7.75f, 5.65f, 0.12f), new Vector3(0.7f, 0.55f, 44.35f), wallStone);
            CreateBox("Rear Wall Left", new Vector3(-5.2f, 2.5f, 22.03f), new Vector3(4.9f, 5.5f, 0.72f), wallStone);
            CreateBox("Rear Wall Right", new Vector3(5.2f, 2.5f, 22.03f), new Vector3(4.9f, 5.5f, 0.72f), wallStone);
            CreateBox("Rear Wall Above Door", new Vector3(0f, 4.2f, 22.03f), new Vector3(5.95f, 2.1f, 0.72f), vineWallStone);
            CreateBox("Start Arch", new Vector3(0f, 3.8f, -20f), new Vector3(15f, 0.8f, 0.7f), wallStone);
            CreateFloorInlays("World 1-1", -18f, 20f, 12.8f, 0.025f);

            for (int i = 0; i < 8; i++)
            {
                float z = -17.5f + i * 5f;
                if (Mathf.Abs(z - 7.5f) > 0.01f)
                {
                    CreateColumn(new Vector3(-6.7f, 1.25f, z));
                }

                CreateColumn(new Vector3(6.7f, 1.25f, z));
            }
        }

        private void CreateTraversal()
        {
            CreateBox("Left Wall Tutorial Chest Platform", new Vector3(-5.75f, 2.0f, 7.8f), new Vector3(3.25f, 0.45f, 3.1f), exteriorStone);

            Transform stairGroup = CreateGroup("World 1-1 Tutorial Chest Stairs", new Vector3(-5.75f, 0.14f, 4.25f));
            for (int i = 0; i < 13; i++)
            {
                Vector3 position = new Vector3(-5.75f, 0.14f + i * 0.155f, 4.25f + i * 0.28f);
                GameObject step = CreateBox($"Tutorial Chest Stair Step {i + 1}", position, new Vector3(1.55f, 0.22f, 0.52f), exteriorStone);
                step.transform.SetParent(stairGroup, true);
            }

            CreateTutorialSigns();
            CreateSpikeRow(new Vector3(0f, 0.15f, -7.2f), 9);
            CreateSpikeRow(new Vector3(0f, 0.15f, 5.2f), 7);
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
            CreateRubbleCluster("World 1-1 Statue", new Vector3(-4.1f, 0.08f, -11.9f), 9);
            CreateRubbleCluster("World 1-1 Gate", new Vector3(3.2f, 0.08f, 18.2f), 7);
            CreateBox("Blue Banner", new Vector3(7.45f, 2.2f, -4.4f), new Vector3(0.08f, 2f, 1.1f), NewMaterial("DK3D Blue Banner", new Color(0.1f, 0.22f, 0.5f)));
            CreateBox("Red Tutorial Banner", new Vector3(-7.45f, 3.05f, 5.85f), new Vector3(0.08f, 2.15f, 0.9f), NewMaterial("DK3D Red Banner", new Color(0.48f, 0.04f, 0.08f)));
        }

        private void CreateInteractables()
        {
            CreateBonfire("Bonfire", new Vector3(-3.3f, 0.45f, -15.1f));
            CreateChest("Treasure Chest", new Vector3(-5.75f, 2.68f, 7.8f), 12, true, true);

            Transform gate = CreateGothicDoor("Castle Passage Door", new Vector3(0f, 1.65f, 21.78f), 6.2f, 3.25f);
            CreateInvisibleInteractableBox("Gate Lock", new Vector3(0f, 1f, 20.85f), new Vector3(1.35f, 1.5f, 0.75f), brass).ConfigureUnlockedGate(gate);
            CreateInvisibleInteractableBox("Castle Passage Door Interaction", new Vector3(0f, 1.25f, 20.8f), new Vector3(4.8f, 2.35f, 1.7f), brass).ConfigureUnlockedGate(gate);

            CreatePickup("Potion", new Vector3(5.35f, 0.65f, 14.4f), false);
        }

        private void CreateEnemies(PlayerController3D player)
        {
            CreateEnemy("Key Guardian 3D", new Vector3(0f, 1.05f, 16.5f), player, 110, 18, 2.6f, true);
        }

        private void CreateBonfire(string name, Vector3 position)
        {
            DungeonInteractable3D bonfire = CreateInteractableBox(name, position, new Vector3(1.8f, 0.7f, 1.8f), ember);
            bonfire.ConfigureBonfire();
            Renderer triggerRenderer = bonfire.GetComponent<Renderer>();
            if (triggerRenderer) triggerRenderer.enabled = false;

            GameObject visualRoot = CreateBonfireVisual(name, position);
            bonfire.transform.SetParent(visualRoot.transform, true);
        }

        private GameObject CreateBonfireVisual(string name, Vector3 position)
        {
            Vector3 floorPosition = new Vector3(position.x, 0.02f, position.z);
            GameObject root = new GameObject($"{name} Visual");
            SetGeneratedParent(root);
            MarkSelectionRoot(root);
            root.transform.position = floorPosition;

            GameObject ash = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ash.name = $"{name} Ash Bed";
            ash.transform.SetParent(root.transform, false);
            ash.transform.localPosition = new Vector3(0f, 0.015f, 0f);
            ash.transform.localScale = new Vector3(1.35f, 0.035f, 1.35f);
            ash.GetComponent<Renderer>().sharedMaterial = charcoal;
            DestroySafely(ash.GetComponent<Collider>());

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
            ConfigureEditorPreviewLight(light);
            return root;
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
            stoneObject.GetComponent<Renderer>().sharedMaterial = ashStone;
            DestroySafely(stoneObject.GetComponent<Collider>());
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
            log.GetComponent<Renderer>().sharedMaterial = wood;
            DestroySafely(log.GetComponent<Collider>());
        }

        private void CreateBonfireCoal(Transform parent, Vector3 localPosition, Vector3 scale)
        {
            GameObject coal = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            coal.name = "Bonfire Ember Core";
            coal.transform.SetParent(parent, false);
            coal.transform.localPosition = localPosition;
            coal.transform.localScale = scale;
            coal.GetComponent<Renderer>().sharedMaterial = ember;
            DestroySafely(coal.GetComponent<Collider>());
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
            flame.AddComponent<AnimatedWallTorch3D>().Configure(light, wind, quietEditorPreview);
        }

        private void CreateWorldOneTwo(PlayerController3D player)
        {
            CreateBox("World 1-2 Courtyard Floor", new Vector3(0f, -0.25f, 43f), new Vector3(22f, 0.5f, 42f), floorStone);
            CreateBox("World 1-2 Left Parapet", new Vector3(-11.25f, 1.25f, 45.38f), new Vector3(0.5f, 3f, 46.95f), wallStone);
            CreateBox("World 1-2 Right Parapet", new Vector3(11.25f, 1.25f, 45.38f), new Vector3(0.5f, 3f, 46.95f), vineWallStone);
            CreateBox("World 1-2 Left Upper Wall Seal", new Vector3(-11.25f, 3.05f, 45.38f), new Vector3(0.7f, 0.5f, 46.95f), wallStone);
            CreateBox("World 1-2 Right Upper Wall Seal", new Vector3(11.25f, 3.05f, 45.38f), new Vector3(0.7f, 0.5f, 46.95f), wallStone);
            CreateBox("World 1-2 Entry Left Seal", new Vector3(-7.15f, 1.9f, 22.05f), new Vector3(8.55f, 3.4f, 0.75f), wallStone);
            CreateBox("World 1-2 Entry Right Seal", new Vector3(7.15f, 1.9f, 22.05f), new Vector3(8.55f, 3.4f, 0.75f), wallStone);
            CreateBox("World 1-2 Far Arch Left", new Vector3(-6.12f, 2.2f, 68.35f), new Vector3(4.75f, 4.6f, 0.72f), wallStone);
            CreateBox("World 1-2 Far Arch Right", new Vector3(6.12f, 2.2f, 68.35f), new Vector3(4.75f, 4.6f, 0.72f), wallStone);
            CreateBox("World 1-2 Far Arch Top", new Vector3(0f, 4.6f, 68.35f), new Vector3(12.95f, 0.9f, 0.78f), vineWallStone);
            CreateBox("Tower Threshold Stone Link", new Vector3(0f, -0.25f, 66.7f), new Vector3(12f, 0.5f, 4.3f), exteriorStone);
            CreateFloorInlays("World 1-2 Courtyard", 24f, 62f, 18f, 0.026f);

            CreateBox("World 1-2 Mid Walkway Main", new Vector3(-5f, 1.85f, 40.75f), new Vector3(5.6f, 0.45f, 4.7f), exteriorStone);
            CreateBox("World 1-2 Mid Walkway Left Lip", new Vector3(-6.8f, 1.85f, 36.85f), new Vector3(2f, 0.45f, 3.1f), exteriorStone);
            CreateBox("World 1-2 Mid Walkway Right Lip", new Vector3(-2.8f, 1.85f, 36.85f), new Vector3(1.2f, 0.45f, 3.1f), exteriorStone);
            CreateBox("World 1-2 Mid Stair Landing", new Vector3(-4.55f, 1.85f, 38.15f), new Vector3(1.75f, 0.45f, 0.55f), exteriorStone);
            CreateBox("World 1-2 Upper Walkway Main", new Vector3(5.2f, 2.9f, 51.5f), new Vector3(5.8f, 0.45f, 4.2f), exteriorStone);
            CreateBox("World 1-2 Upper Walkway Side Lip", new Vector3(6.1f, 2.9f, 47.6f), new Vector3(4f, 0.45f, 3.6f), exteriorStone);
            CreateMovingPlatform("World 1-2 Moving Bridge", new Vector3(0.05f, 2.39f, 46.13f), new Vector3(3.2f, 0.42f, 2.2f), new Vector3(1.3f, 1.05f, 10.75f), 0.65f);

            CreateStairRun("World 1-2 Lower Stair", new Vector3(-4.55f, 0f, 34.1f), 12, new Vector3(0f, 0.17f, 0.37f), 1.55f);

            CreateTorch(new Vector3(-10.85f, 2.0f, 30f));
            CreateTorch(new Vector3(10.85f, 2.0f, 35.5f));
            CreateTorch(new Vector3(-10.85f, 2.0f, 47f));
            CreateTorch(new Vector3(10.85f, 2.0f, 56f));
            CreateBox("World 1-2 Moon Banner", new Vector3(-10.95f, 2.2f, 42f), new Vector3(0.08f, 2.1f, 1.25f), NewMaterial("DK3D Purple Banner", new Color(0.28f, 0.12f, 0.42f)));
            CreateBox("World 1-2 Crown Banner", new Vector3(10.95f, 2.2f, 53f), new Vector3(0.08f, 2.1f, 1.25f), NewMaterial("DK3D Gold Banner", new Color(0.62f, 0.42f, 0.1f)));
            CreateRubbleCluster("World 1-2 Courtyard Left", new Vector3(-8.2f, 0.08f, 36.4f), 10);
            CreateRubbleCluster("World 1-2 Courtyard Right", new Vector3(8.1f, 0.08f, 55.4f), 10);
            CreateWorldOneTwoCourtyardDressing();

            CreateRunestoneLoreTablet("Courtyard Lore Tablet", new Vector3(-8.9f, 0.82f, 33.2f), "El patio estira el riesgo: pasarelas, ruinas y enemigos en altura antes de la torre.", Quaternion.Euler(0f, 90f, 0f));
            CreateChest("World 1-2 Supply Chest", new Vector3(5.2f, 3.47f, 51.6f), 18);

            CreatePickup("World 1-2 Potion", new Vector3(7.2f, 0.65f, 58f), false);

            CreateMiniBossGateArena(player);

            CreateEnemy("World 1-2 Gate Guard", new Vector3(4.3f, 1.05f, 31.5f), player, 55, 10, 2.55f, false);
            CreateEnemy("World 1-2 Walkway Guard", new Vector3(-5.2f, 3.13f, 40.2f), player, 60, 11, 2.45f, false);
            CreateEnemy("World 1-2 Upper Guard", new Vector3(5.6f, 4.13f, 49.2f), player, 60, 12, 2.35f, false);
        }

        private void CreateWorldOneTwoCourtyardDressing()
        {
            CreateFloorCrack("World 1-2 South Floor Crack", new Vector3(-0.9f, 0.032f, 35.85f), 3.25f, -18f);
            CreateFloorCrack("World 1-2 East Floor Crack", new Vector3(5.95f, 0.033f, 43.8f), 2.6f, 31f);
            CreateFloorCrack("World 1-2 North Floor Crack", new Vector3(-3.4f, 0.034f, 55.6f), 3.4f, 8f);

            CreateRubbleCluster("World 1-2 Broken Platform Left", new Vector3(-8.45f, 0.08f, 43.2f), 8);
            CreateRubbleCluster("World 1-2 Broken Platform Right", new Vector3(8.35f, 0.08f, 47.4f), 7);
            CreateRubbleCluster("World 1-2 Stair Rubble", new Vector3(-6.15f, 0.08f, 34.4f), 6);

            CreateBrokenCrates("World 1-2 Left Wall Crates", new Vector3(-9.2f, 0.16f, 39.6f), -12f);
            CreateBrokenCrates("World 1-2 Right Wall Crates", new Vector3(9.05f, 0.16f, 49.3f), 16f);
            CreateBrokenStatue("World 1-2 Fallen Sentinel", new Vector3(-9.1f, 0.1f, 51.7f), 22f);
            CreateBattleRemnants("World 1-2 Central Battle Remnants", new Vector3(1.9f, 0.04f, 44.5f), 28f);
            CreateBattleRemnants("World 1-2 North Battle Remnants", new Vector3(-4.9f, 0.04f, 52.7f), -18f);
            CreateFallenSkeleton("World 1-2 Fallen Skeleton A", new Vector3(4.75f, 0.09f, 38.8f), -35f);
            CreateFallenSkeleton("World 1-2 Fallen Skeleton B", new Vector3(-6.7f, 0.09f, 48.9f), 27f);

            CreateFloorBrazier("World 1-2 Lower Brazier", new Vector3(-8.4f, 0.12f, 45.4f));
            CreateFloorBrazier("World 1-2 Upper Brazier", new Vector3(8.25f, 0.12f, 39.8f));
        }

        private void CreateBuriedSpikeTrap(string name, Vector3 center, int count)
        {
            GameObject damage = CreateHazardBox(name + " Damage", center, new Vector3(8.2f, 0.32f, 1.35f), 22, "Los pinchos atraviesan la armadura.");
            Renderer damageRenderer = damage.GetComponent<Renderer>();
            if (damageRenderer) damageRenderer.enabled = false;

            Transform group = CreateGroup(name + " Tips", center);
            for (int i = 0; i < count; i++)
            {
                float x = (i - (count - 1) * 0.5f) * 0.62f;
                GameObject spike = CreateSpikeTip("Buried Spike Tip", center + new Vector3(x, -0.08f, 0f), 0.36f, 0.46f, chestIron);
                spike.transform.SetParent(group, true);
            }
        }

        private void CreateWorldOneThree(PlayerController3D player)
        {
            CreateBox("World 1-3 Tower Lower Floor", new Vector3(0f, -0.25f, 77.5f), new Vector3(16f, 0.5f, 25f), floorStone);
            CreateBox("World 1-3 Tower Mid Floor", new Vector3(0f, 2.95f, 87f), new Vector3(13f, 0.45f, 16f), floorStone);
            CreateBox("World 1-3 Tower Top Floor", new Vector3(0f, 6.15f, 97f), new Vector3(12f, 0.45f, 14f), floorStone);
            CreateBox("World 1-3 Left Wall", new Vector3(-8.25f, 3f, 87f), new Vector3(0.5f, 8f, 34f), wallStone);
            CreateBox("World 1-3 Right Wall", new Vector3(8.25f, 3f, 87f), new Vector3(0.5f, 8f, 34f), wallStone);
            CreateBox("World 1-3 Rear Wall", new Vector3(0f, 5.4f, 104f), new Vector3(16.5f, 10f, 0.5f), vineWallStone);
            CreateBox("World 1-3 Entry Seal Left", new Vector3(-8.25f, 2.4f, 66.65f), new Vector3(0.5f, 4.2f, 4.8f), wallStone);
            CreateBox("World 1-3 Entry Seal Right", new Vector3(8.25f, 2.4f, 66.65f), new Vector3(0.5f, 4.2f, 4.8f), wallStone);
            CreateBox("World 1-3 Mid Floor Left Rail", new Vector3(-6.75f, 3.55f, 87f), new Vector3(0.28f, 1.0f, 16f), darkStone);
            CreateBox("World 1-3 Mid Floor Right Rail", new Vector3(6.75f, 3.55f, 87f), new Vector3(0.28f, 1.0f, 16f), darkStone);
            CreateBox("World 1-3 Top Floor Left Rail", new Vector3(-6.25f, 6.75f, 97f), new Vector3(0.28f, 1.0f, 14f), darkStone);
            CreateBox("World 1-3 Top Floor Right Rail", new Vector3(6.25f, 6.75f, 97f), new Vector3(0.28f, 1.0f, 14f), darkStone);
            CreateStairRun("World 1-3 Lower Tower Stair", new Vector3(-5.5f, 0f, 80.8f), 13, new Vector3(0.45f, 0.25f, 0.48f), 1.55f, true);
            CreateStairRun("World 1-3 Upper Tower Stair", new Vector3(5.2f, 3.1f, 89.1f), 13, new Vector3(-0.45f, 0.25f, 0.48f), 1.55f, true);
            CreateBox("World 1-3 High Tower Floor", new Vector3(0f, 9.35f, 108f), new Vector3(11.2f, 0.45f, 13.5f), floorStone);
            CreateBox("World 1-3 Spire Floor", new Vector3(0f, 12.55f, 119f), new Vector3(9.4f, 0.45f, 10.5f), floorStone);
            CreateBox("World 1-3 High Left Wall", new Vector3(-5.85f, 9.5f, 108f), new Vector3(0.45f, 6.2f, 13.5f), wallStone);
            CreateBox("World 1-3 High Right Wall", new Vector3(5.85f, 9.5f, 108f), new Vector3(0.45f, 6.2f, 13.5f), vineWallStone);
            CreateBox("World 1-3 Spire Rear Wall", new Vector3(0f, 12.8f, 124.4f), new Vector3(10f, 4.6f, 0.45f), wallStone);
            CreateBox("World 1-3 High Front Rail", new Vector3(0f, 9.95f, 101.15f), new Vector3(11.2f, 1f, 0.28f), darkStone);
            CreateBox("World 1-3 Spire Left Rail", new Vector3(-4.9f, 13.15f, 119f), new Vector3(0.25f, 1f, 10.5f), darkStone);
            CreateBox("World 1-3 Spire Right Rail", new Vector3(4.9f, 13.15f, 119f), new Vector3(0.25f, 1f, 10.5f), darkStone);
            CreateStairRun("World 1-3 High Tower Stair", new Vector3(-4.9f, 6.28f, 101.4f), 13, new Vector3(0.42f, 0.25f, 0.48f), 1.55f, true);
            CreateStairRun("World 1-3 Spire Stair", new Vector3(4.35f, 9.45f, 112.2f), 13, new Vector3(-0.38f, 0.25f, 0.43f), 1.55f, true);
            CreateMovingPlatform("World 1-3 Lift Stone", new Vector3(-4.6f, 4.4f, 94f), new Vector3(2.8f, 0.42f, 2.8f), new Vector3(0f, 2.6f, 0f), 0.7f);
            CreateFloorInlays("World 1-3 Lower", 68f, 86f, 12f, 0.026f);
            CreateFloorInlays("World 1-3 High", 102f, 123f, 8f, 12.83f);

            for (int i = 0; i < 5; i++)
            {
                float z = 73.5f + i * 7f;
                CreateColumn(new Vector3(-6.8f, 2.1f, z));
                CreateColumn(new Vector3(6.8f, 2.1f, z));
                CreateTorch(new Vector3(i % 2 == 0 ? -7.85f : 7.85f, 3.2f, z + 2.5f));
            }
            CreateRubbleCluster("World 1-3 Lower Tower", new Vector3(-5.4f, 0.08f, 76.5f), 8);
            CreateRubbleCluster("World 1-3 High Tower", new Vector3(3.5f, 9.68f, 107.2f), 8);
            CreateDecorationBox("World 1-3 Crown Seal Bar", new Vector3(0f, 12.83f, 119f), new Vector3(6.8f, 0.04f, 0.1f), brass);
            CreateDecorationBox("World 1-3 Crown Seal Spine", new Vector3(0f, 12.84f, 119f), new Vector3(0.1f, 0.04f, 6.8f), brass);

            CreateBonfire("Tower Bonfire", new Vector3(5.7f, 0.45f, 73.3f));
            CreateRunestoneLoreTablet("Tower Lore Tablet", new Vector3(-5.6f, 0.82f, 74.3f), "La torre sube mucho mas de lo que parece. Arriba hay un mecanismo que baja un atajo.", Quaternion.Euler(0f, 90f, 0f));
            CreateChest("World 1-3 Tower Chest", new Vector3(-4.8f, 13.02f, 119.5f), 38);
            CreateInteractableBox("World 1-3 Exit Door", new Vector3(0f, 13.95f, 123.6f), new Vector3(2.4f, 2.6f, 0.4f), brass).ConfigureExit();

            CreatePickup("World 1-3 Potion", new Vector3(4.2f, 13.05f, 121f), false);

            CreateEnemy("World 1-3 Tower Guard A", new Vector3(-2.5f, 1.05f, 78f), player, 70, 13, 2.4f, false);
            CreateEnemy("World 1-3 Tower Guard B", new Vector3(3.5f, 4.18f, 89.4f), player, 75, 14, 2.35f, false);
            CreateEnemy("World 1-3 High Guard", new Vector3(-2.7f, 10.38f, 109.8f), player, 85, 16, 2.35f, false);
            CreateEnemy("World 1-3 Crown Warden", new Vector3(0f, 13.78f, 119.5f), player, 150, 22, 2.45f, false);
            CreateShortcutElevator();
        }

        private void CreateMiniBossGateArena(PlayerController3D player)
        {
            GameObject miniBoss = CreateEnemy("Tower Key Mini Boss", new Vector3(0f, 1.05f, 64.2f), player, 280, 26, 2.15f, false, true, 12, 18, true);

            CreateTowerKeyChamber();
            Transform entryDoor = CreateGothicDoor("Tower Chamber Entry Door", new Vector3(0f, 1.65f, 56.45f), 6.2f, 3.35f);
            if (miniBoss && miniBoss.TryGetComponent(out DungeonEnemy3D miniBossEnemy))
            {
                miniBossEnemy.ConfigureWakeGate(entryDoor.GetComponent<GothicDoubleDoor3D>());
            }

            Transform towerGate = CreateGothicDoor("Tower Gothic Door", new Vector3(0f, 1.65f, 68.85f), 6.2f, 3.35f, false);
            DungeonInteractable3D towerGateLock = CreateInvisibleInteractableBox("Tower Gate Lock", new Vector3(0f, 1.0f, 67.75f), new Vector3(1.55f, 1.45f, 0.75f), brass);
            DungeonInteractable3D towerDoorInteraction = CreateInvisibleInteractableBox("Tower Gothic Door Interaction", new Vector3(0f, 1.25f, 67.65f), new Vector3(4.8f, 2.35f, 1.85f), brass);
            if (EnemyEncountersEnabled)
            {
                towerGateLock.ConfigureTowerGate(towerGate);
                towerDoorInteraction.ConfigureTowerGate(towerGate);
            }
            else
            {
                towerGateLock.ConfigureUnlockedGate(towerGate);
                towerDoorInteraction.ConfigureUnlockedGate(towerGate);
            }

            CreateCloseRoomCameraZone("Tower Key Chamber Close Camera Zone", new Vector3(0f, 2.05f, 63.25f), new Vector3(14.9f, 5.2f, 8.5f));
        }

        private void CreateTowerKeyChamber()
        {
            CreateBox("Tower Key Chamber Left Wall", new Vector3(-8.25f, 2.45f, 62.75f), new Vector3(0.7f, 5.6f, 13.1f), wallStone);
            CreateBox("Tower Key Chamber Right Wall", new Vector3(8.25f, 2.45f, 62.75f), new Vector3(0.7f, 5.6f, 13.1f), wallStone);
            CreateBox("Tower Key Chamber Outer Left Gap Seal", new Vector3(-9.68f, 2.15f, 68.35f), new Vector3(3.25f, 4.8f, 0.76f), wallStone);
            CreateBox("Tower Key Chamber Outer Right Gap Seal", new Vector3(9.68f, 2.15f, 68.35f), new Vector3(3.25f, 4.8f, 0.76f), wallStone);
            CreateBox("Tower Key Chamber Front Left Return", new Vector3(-5.62f, 2.15f, 56.45f), new Vector3(6.18f, 4.8f, 1.08f), wallStone);
            CreateBox("Tower Key Chamber Front Right Return", new Vector3(5.62f, 2.15f, 56.45f), new Vector3(6.18f, 4.8f, 1.08f), wallStone);
            CreateBox("Tower Key Chamber Front Header", new Vector3(0f, 4.05f, 56.45f), new Vector3(17.05f, 2.0f, 1.08f), wallStone);
            CreateBox("Tower Key Chamber Front Left Gap Seal", new Vector3(-5.62f, 2.15f, 56.25f), new Vector3(6.18f, 4.8f, 1.12f), wallStone);
            CreateBox("Tower Key Chamber Front Right Gap Seal", new Vector3(5.62f, 2.15f, 56.25f), new Vector3(6.18f, 4.8f, 1.12f), wallStone);
            CreateBox("Tower Key Chamber Front Top Gap Seal", new Vector3(0f, 4.05f, 56.25f), new Vector3(17.1f, 2.05f, 1.12f), wallStone);
            CreateBox("Tower Key Chamber Front Left Jamb Seal", new Vector3(-2.92f, 2.15f, 56.22f), new Vector3(0.5f, 4.8f, 1.18f), wallStone);
            CreateBox("Tower Key Chamber Front Right Jamb Seal", new Vector3(2.92f, 2.15f, 56.22f), new Vector3(0.5f, 4.8f, 1.18f), wallStone);
            CreateBox("Tower Key Chamber Rear Left Fill", new Vector3(-5.62f, 2.15f, 68.85f), new Vector3(6.18f, 4.8f, 1.08f), vineWallStone);
            CreateBox("Tower Key Chamber Rear Right Fill", new Vector3(5.62f, 2.15f, 68.85f), new Vector3(6.18f, 4.8f, 1.08f), vineWallStone);
            CreateBox("Tower Key Chamber Rear Header", new Vector3(0f, 4.05f, 68.85f), new Vector3(17.1f, 2.05f, 1.08f), wallStone);
            CreateBox("Tower Key Chamber Rear Left Gap Seal", new Vector3(-5.62f, 2.15f, 69.08f), new Vector3(6.18f, 4.8f, 1.18f), vineWallStone);
            CreateBox("Tower Key Chamber Rear Right Gap Seal", new Vector3(5.62f, 2.15f, 69.08f), new Vector3(6.18f, 4.8f, 1.18f), vineWallStone);
            CreateBox("Tower Key Chamber Rear Top Gap Seal", new Vector3(0f, 4.05f, 69.08f), new Vector3(17.1f, 2.05f, 1.18f), wallStone);
            CreateBox("Tower Key Chamber Rear Left Jamb Seal", new Vector3(-2.92f, 2.15f, 69.08f), new Vector3(0.5f, 4.8f, 1.18f), wallStone);
            CreateBox("Tower Key Chamber Rear Right Jamb Seal", new Vector3(2.92f, 2.15f, 69.08f), new Vector3(0.5f, 4.8f, 1.18f), wallStone);
            CreateBox("Tower Key Chamber Unified Floor", new Vector3(0f, -0.255f, 62.8f), new Vector3(17.15f, 0.5f, 13.35f), floorStone);
            CreateBox("Tower Key Chamber Ceiling", new Vector3(0f, 4.95f, 62.8f), new Vector3(17.35f, 0.7f, 13.65f), darkStone);
            CreateBox("Tower Key Chamber Left Upper Side Seal", new Vector3(-8.25f, 4.75f, 62.75f), new Vector3(1.1f, 1.1f, 13.65f), wallStone);
            CreateBox("Tower Key Chamber Right Upper Side Seal", new Vector3(8.25f, 4.75f, 62.75f), new Vector3(1.1f, 1.1f, 13.65f), wallStone);

            CreateTorch(new Vector3(-7.85f, 3.15f, 62.75f));
            CreateTorch(new Vector3(7.85f, 3.15f, 62.75f));
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
            SetGeneratedParent(npc);
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
                fallback.GetComponent<Renderer>().sharedMaterial = NewMaterial("DK3D Anciano Cloak", new Color(0.16f, 0.19f, 0.24f));
                DestroySafely(fallback.GetComponent<CapsuleCollider>());

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
                DestroySafely(modelCollider);
            }

            Renderer[] renderers = visual.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
            {
                DestroySafely(visual);
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
                Material[] materials = renderer.sharedMaterials;
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

                renderer.sharedMaterials = materials;
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
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            renderer.receiveShadows = true;

            Collider collider = accent.GetComponent<Collider>();
            if (collider) DestroySafely(collider);
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
            ConfigureEditorPreviewLight(light);
            return fog;
        }

        private GameObject CreateEnemy(string name, Vector3 position, PlayerController3D player, int hp, int damage, float speed, bool dropsKey)
        {
            return CreateEnemy(name, position, player, hp, damage, speed, dropsKey, false, 5, 8);
        }

        private GameObject CreateEnemy(string name, Vector3 position, PlayerController3D player, int hp, int damage, float speed, bool dropsKey, bool dropsTowerKey, int minCoinReward, int maxCoinReward, bool miniBossVisual = false)
        {
            if (!EnemyEncountersEnabled) return null;

            GameObject enemyObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            enemyObject.name = name;
            SetGeneratedParent(enemyObject);
            enemyObject.transform.position = position;
            enemyObject.GetComponent<Renderer>().sharedMaterial = enemy;
            CapsuleCollider capsuleCollider = enemyObject.GetComponent<CapsuleCollider>();
            if (Application.isPlaying) DestroySafely(capsuleCollider);
            else Object.DestroyImmediate(capsuleCollider);
            CharacterController controller = enemyObject.AddComponent<CharacterController>();
            controller.height = 2f;
            controller.radius = 0.42f;
            controller.center = Vector3.zero;
            bool hasRiggedSkeleton = miniBossVisual ? RiggedSkeletonEnemyVisual3D.TryAttachMiniBoss(enemyObject.transform) : RiggedSkeletonEnemyVisual3D.TryAttach(enemyObject.transform);
            if (!hasRiggedSkeleton)
            {
                AttachSkeletonSprite(enemyObject.transform);
            }
            else
            {
                Renderer capsuleRenderer = enemyObject.GetComponent<Renderer>();
                RiggedSkeletonEnemyVisual3D riggedVisual = enemyObject.GetComponentInChildren<RiggedSkeletonEnemyVisual3D>(true);
                if (capsuleRenderer) capsuleRenderer.enabled = !riggedVisual || !riggedVisual.HasVisibleBodyRenderer();
            }

            enemyObject.AddComponent<DungeonEnemy3D>().Configure(player, hp, damage, speed, dropsKey, dropsTowerKey, minCoinReward, maxCoinReward);
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
            visual.transform.localScale = Vector3.one * 1.3f;

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
                DestroySafely(visual);
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

        private void CreateStairRun(string name, Vector3 start, int steps, Vector3 stepOffset, float width, bool alignStepsToRun = false)
        {
            Transform group = CreateGroup(name, start);
            Vector3 alignedStepOffset = stepOffset;
            Quaternion stepRotation = alignStepsToRun
                ? Quaternion.Euler(0f, Mathf.Atan2(stepOffset.x, stepOffset.z) * Mathf.Rad2Deg, 0f)
                : Quaternion.identity;
            for (int i = 0; i < steps; i++)
            {
                Vector3 position = start + alignedStepOffset * i;
                Vector3 scale = Mathf.Abs(alignedStepOffset.x) > Mathf.Abs(alignedStepOffset.z)
                    ? new Vector3(Mathf.Abs(alignedStepOffset.x) + 0.35f, 0.28f, width)
                    : new Vector3(width, 0.28f, Mathf.Abs(alignedStepOffset.z) + 0.35f);
                GameObject step = CreateBox($"{name} {i + 1}", position, scale, exteriorStone);
                step.transform.rotation = stepRotation;
                step.transform.SetParent(group, true);
            }
        }

        private void CreateBridgeRailing(Vector3 origin, int count)
        {
            Transform group = CreateGroup("Stone Railing", origin);
            for (int i = 0; i < count; i++)
            {
                GameObject post = CreateBox("Stone Railing Post", origin + Vector3.forward * (i * 1.15f), new Vector3(0.28f, 1.15f, 0.28f), darkStone);
                post.transform.SetParent(group, true);
            }
        }

        private void CreateTutorialSigns()
        {
            CreateTutorialSign("Tutorial Sign Movement", new Vector3(-4.3f, 0.86f, -1.8f), "WASD mueve al caballero. Usa la camara para orientar el movimiento.");
            CreateTutorialSign("Tutorial Sign Inventory", new Vector3(-1.45f, 0.86f, -1.8f), "E interactua con cofres, letreros y puertas. I abre el inventario para equipar espada y escudo.");
            CreateTutorialSign("Tutorial Sign Combat", new Vector3(1.45f, 0.86f, -1.8f), "J ataca. Mantener J carga un golpe fuerte. K levanta el escudo y puede hacer parry al inicio.");
            CreateTutorialSign("Tutorial Sign Actions", new Vector3(4.3f, 0.86f, -1.8f), "Space salta. L rueda para esquivar. Q usa una pocion cuando te falta vida.");
        }

        private void CreateTutorialSign(string name, Vector3 position, string label)
        {
            CreateRunestoneLoreTablet(name, position, label, Quaternion.Euler(0f, 180f, 0f));
        }

        private void CreateRunestoneLoreTablet(string name, Vector3 position, string label, Quaternion rotation)
        {
            GameObject root = new GameObject(name);
            SetGeneratedParent(root);
            MarkSelectionRoot(root);
            root.transform.position = position;
            root.transform.rotation = rotation;

            BoxCollider trigger = root.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.size = new Vector3(2.4f, 2.25f, 2.2f);
            trigger.center = new Vector3(0f, 0.18f, 0.42f);
            root.AddComponent<DungeonInteractable3D>().ConfigureLore(label);
            CreateInvisibleSolidBlocker(root.transform, $"{name} Solid Stone Blocker", new Vector3(0f, -0.02f, 0.02f), new Vector3(1.08f, 2.05f, 0.58f));

            const float frontZ = -0.095f;
            CreateLocalBox(root.transform, $"{name} Broken Stone Base", new Vector3(0f, -0.76f, 0.03f), new Vector3(1.45f, 0.22f, 0.58f), runestoneEdge);
            CreateLocalBox(root.transform, $"{name} Mossy Base Slab", new Vector3(0.04f, -0.66f, 0.02f), new Vector3(1.18f, 0.18f, 0.42f), runestoneFace);
            CreateLocalBox(root.transform, $"{name} Tall Rune Stone", new Vector3(0f, 0.15f, 0f), new Vector3(0.92f, 1.82f, 0.22f), runestoneFace);
            CreateLocalBox(root.transform, $"{name} Rounded Top Center", new Vector3(0f, 1.09f, 0f), new Vector3(0.74f, 0.22f, 0.23f), runestoneFace);
            CreateLocalBox(root.transform, $"{name} Dark Left Edge", new Vector3(-0.49f, 0.1f, -0.01f), new Vector3(0.09f, 1.72f, 0.24f), runestoneEdge);
            CreateLocalBox(root.transform, $"{name} Dark Right Edge", new Vector3(0.49f, 0.04f, -0.01f), new Vector3(0.08f, 1.62f, 0.24f), runestoneEdge);
            CreateLocalBox(root.transform, $"{name} Chipped Top Left", new Vector3(-0.35f, 1.18f, frontZ), new Vector3(0.24f, 0.11f, 0.035f), runestoneEdge);
            CreateLocalBox(root.transform, $"{name} Chipped Lower Right", new Vector3(0.44f, -0.5f, frontZ), new Vector3(0.12f, 0.24f, 0.035f), runestoneEdge);

            CreateRunestoneCracks(root.transform, name, frontZ);
            CreateRunestoneGlyphs(root.transform, name, frontZ);
        }

        private void CreateRunestoneCracks(Transform parent, string name, float frontZ)
        {
            CreateLocalBox(parent, $"{name} Stone Crack A", new Vector3(-0.18f, 0.46f, frontZ - 0.02f), new Vector3(0.035f, 0.58f, 0.026f), runestoneRune).transform.localRotation = Quaternion.Euler(0f, 0f, -28f);
            CreateLocalBox(parent, $"{name} Stone Crack B", new Vector3(0.12f, -0.28f, frontZ - 0.02f), new Vector3(0.028f, 0.48f, 0.026f), runestoneRune).transform.localRotation = Quaternion.Euler(0f, 0f, 24f);
            CreateLocalBox(parent, $"{name} Stone Crack C", new Vector3(-0.29f, -0.1f, frontZ - 0.02f), new Vector3(0.026f, 0.28f, 0.026f), runestoneRune).transform.localRotation = Quaternion.Euler(0f, 0f, 67f);
        }

        private void CreateRunestoneGlyphs(Transform parent, string name, float frontZ)
        {
            float[] runeYs = { 0.86f, 0.67f, 0.48f, -0.07f, -0.27f, -0.48f };
            for (int i = 0; i < runeYs.Length; i++)
            {
                CreateRuneGlyph(parent, $"{name} Rune Left {i + 1}", -0.31f, runeYs[i], frontZ, i);
                CreateRuneGlyph(parent, $"{name} Rune Right {i + 1}", 0.31f, runeYs[i] - 0.03f, frontZ, i + 2);
            }

            Vector3 center = new Vector3(0f, 0.18f, frontZ - 0.03f);
            float radius = 0.18f;
            for (int i = 0; i < 10; i++)
            {
                float angle = i * 32f;
                float radians = angle * Mathf.Deg2Rad;
                float segmentRadius = radius * (1f - i * 0.055f);
                Vector3 local = center + new Vector3(Mathf.Cos(radians) * segmentRadius, Mathf.Sin(radians) * segmentRadius, 0f);
                GameObject mark = CreateLocalBox(parent, $"{name} Rune Spiral Mark", local, new Vector3(0.075f, 0.026f, 0.026f), runestoneRune);
                mark.transform.localRotation = Quaternion.Euler(0f, 0f, angle + 92f);
            }
        }

        private void CreateRuneGlyph(Transform parent, string name, float x, float y, float frontZ, int variant)
        {
            CreateLocalBox(parent, $"{name} Spine", new Vector3(x, y, frontZ - 0.03f), new Vector3(0.026f, 0.15f, 0.026f), runestoneRune);
            if (variant % 3 != 1)
            {
                GameObject slash = CreateLocalBox(parent, $"{name} Slash", new Vector3(x + 0.035f, y + 0.035f, frontZ - 0.032f), new Vector3(0.024f, 0.12f, 0.026f), runestoneRune);
                slash.transform.localRotation = Quaternion.Euler(0f, 0f, -38f);
            }
            if (variant % 2 == 0)
            {
                GameObject cross = CreateLocalBox(parent, $"{name} Cross", new Vector3(x + 0.02f, y - 0.028f, frontZ - 0.034f), new Vector3(0.1f, 0.022f, 0.026f), runestoneRune);
                cross.transform.localRotation = Quaternion.Euler(0f, 0f, variant % 4 == 0 ? 0f : 24f);
            }
            if (variant % 3 == 1)
            {
                GameObject fork = CreateLocalBox(parent, $"{name} Fork", new Vector3(x - 0.028f, y + 0.042f, frontZ - 0.036f), new Vector3(0.024f, 0.09f, 0.026f), runestoneRune);
                fork.transform.localRotation = Quaternion.Euler(0f, 0f, 42f);
            }
        }

        private void CreateMovingPlatform(string name, Vector3 position, Vector3 scale, Vector3 travel, float speed)
        {
            GameObject platform = CreateBox(name, position, scale, exteriorStone);
            platform.AddComponent<DungeonMovingPlatform3D>().Configure(travel, speed);
        }

        private void CreateFireTrap(string name, Vector3 position, Vector3 scale, bool visible = true)
        {
            GameObject fire = CreateHazardBox(name, position, scale, 16, "Llamas del castillo.");
            fire.GetComponent<Renderer>().sharedMaterial = ember;
            fire.GetComponent<Renderer>().enabled = visible;

            Light light = fire.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1f, 0.34f, 0.12f);
            light.intensity = visible ? 2f : 0f;
            light.range = 5f;
            ConfigureEditorPreviewLight(light);
        }

        private void CreateBladeTrap(string name, Vector3 position, Vector3 spin, Vector3 bob)
        {
            GameObject blade = CreateHazardBox(name, position, new Vector3(3.6f, 0.18f, 0.42f), 22, "Una cuchilla te alcanza.");
            blade.GetComponent<Renderer>().sharedMaterial = hazard;
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

        private static void EnsureRuntimeCloseRoomCameraZones()
        {
            GameObject oldZone = GameObject.Find("Tower Key Chamber Camera Zone");
            if (oldZone)
            {
                DestroySafely(oldZone);
            }

            GameObject zone = GameObject.Find("Tower Key Chamber Close Camera Zone");
            if (!zone)
            {
                zone = new GameObject("Tower Key Chamber Close Camera Zone");
                GameObject generatedRootObject = GameObject.Find(GeneratedRootName);
                if (generatedRootObject) zone.transform.SetParent(generatedRootObject.transform, false);
            }

            ConfigureTowerKeyCloseCameraZone(zone);
        }

        private static void ConfigureTowerKeyCloseCameraZone(GameObject zone)
        {
            zone.transform.position = new Vector3(0f, 2.05f, 63.25f);
            BoxCollider collider = zone.GetComponent<BoxCollider>();
            if (!collider)
            {
                collider = zone.AddComponent<BoxCollider>();
            }

            collider.isTrigger = true;
            collider.size = new Vector3(14.9f, 5.2f, 8.5f);

            CloseRoomCameraZone3D roomZone = zone.GetComponent<CloseRoomCameraZone3D>();
            if (!roomZone)
            {
                roomZone = zone.AddComponent<CloseRoomCameraZone3D>();
            }

            ConfigureTowerKeyCloseCamera(roomZone);
        }

        private GameObject CreateCloseRoomCameraZone(string name, Vector3 position, Vector3 scale)
        {
            GameObject zone = new GameObject(name);
            SetGeneratedParent(zone);
            zone.transform.position = position;
            BoxCollider collider = zone.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.size = scale;

            CloseRoomCameraZone3D roomZone = zone.AddComponent<CloseRoomCameraZone3D>();
            ConfigureTowerKeyCloseCamera(roomZone);
            return zone;
        }

        private static void ConfigureTowerKeyCloseCamera(CloseRoomCameraZone3D roomZone)
        {
            roomZone.Configure(
                3.65f,
                4.15f,
                2.45f,
                1.25f,
                60f,
                0.08f,
                57.35f,
                Vector3.forward);
        }

        private Transform CreateGothicDoor(string name, Vector3 position, float width, float height, bool addUnlockedInteraction = true)
        {
            GameObject root = new GameObject(name);
            SetGeneratedParent(root);
            MarkSelectionRoot(root);
            root.transform.position = position;

            GameObject blocker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            blocker.name = $"{name} Solid Blocker";
            blocker.transform.SetParent(root.transform, false);
            blocker.transform.localPosition = Vector3.zero;
            blocker.transform.localScale = new Vector3(width * 0.9f, height * 0.96f, 0.48f);
            Renderer blockerRenderer = blocker.GetComponent<Renderer>();
            if (blockerRenderer) blockerRenderer.enabled = false;
            Collider blockerCollider = blocker.GetComponent<Collider>();

            float leafWidth = width * 0.45f;
            float leafHeight = height * 0.96f;
            Transform leftPivot = CreateDoorPivot(root.transform, "Left Door Pivot", new Vector3(-leafWidth, -0.08f, -0.1f));
            Transform rightPivot = CreateDoorPivot(root.transform, "Right Door Pivot", new Vector3(leafWidth, -0.08f, -0.1f));

            CreateLocalBox(leftPivot, "Left Wood Door", new Vector3(leafWidth * 0.5f, 0f, 0f), new Vector3(leafWidth, leafHeight, 0.16f), gothicDoorWood);
            CreateLocalBox(rightPivot, "Right Wood Door", new Vector3(-leafWidth * 0.5f, 0f, 0f), new Vector3(leafWidth, leafHeight, 0.16f), gothicDoorWood);
            CreateLocalBox(root.transform, "Door Center Gap", new Vector3(0f, -0.08f, -0.2f), new Vector3(0.075f, leafHeight, 0.06f), gothicDoorIron);

            DecorateGothicDoorLeaf(leftPivot, true, leafWidth, leafHeight);
            DecorateGothicDoorLeaf(rightPivot, false, leafWidth, leafHeight);

            root.AddComponent<GothicDoubleDoor3D>().Configure(leftPivot, rightPivot, blockerCollider);
            if (addUnlockedInteraction)
            {
                BoxCollider interactionCollider = root.AddComponent<BoxCollider>();
                interactionCollider.isTrigger = true;
                interactionCollider.center = new Vector3(0f, 0f, -0.18f);
                interactionCollider.size = new Vector3(width, height, 2.2f);
                root.AddComponent<DungeonInteractable3D>().ConfigureUnlockedGate(root.transform);
            }

            return root.transform;
        }

        private void DecorateGothicDoorLeaf(Transform pivot, bool leftLeaf, float leafWidth, float leafHeight)
        {
            float side = leftLeaf ? 1f : -1f;
            float frontZ = -0.18f;
            int plankCount = 5;
            float plankWidth = leafWidth / plankCount;

            for (int i = 0; i < plankCount; i++)
            {
                float x = side * (plankWidth * (i + 0.5f));
                Material plankMaterial = i % 2 == 0 ? gothicDoorWood : NewMaterial($"DK3D Gothic Door Plank Shade {pivot.name} {i}", new Color(0.14f + i * 0.009f, 0.082f + i * 0.005f, 0.052f));
                CreateLocalBox(pivot, "Gothic Door Vertical Plank", new Vector3(x, 0f, frontZ), new Vector3(plankWidth * 0.92f, leafHeight * 0.98f, 0.035f), plankMaterial);

                float grainOffset = (i - 2) * 0.018f;
                CreateLocalBox(pivot, "Gothic Door Wood Grain", new Vector3(x + side * plankWidth * 0.18f, grainOffset, frontZ - 0.026f), new Vector3(0.025f, leafHeight * 0.78f, 0.018f), gothicDoorWoodDark);
                CreateLocalBox(pivot, "Gothic Door Wood Grain", new Vector3(x - side * plankWidth * 0.22f, -grainOffset, frontZ - 0.027f), new Vector3(0.018f, leafHeight * 0.58f, 0.018f), gothicDoorWoodDark);

                if (i > 0)
                {
                    float seamX = side * (plankWidth * i);
                    CreateLocalBox(pivot, "Gothic Door Plank Seam", new Vector3(seamX, 0f, frontZ - 0.035f), new Vector3(0.035f, leafHeight * 0.98f, 0.025f), gothicDoorWoodDark);
                }
            }

            float hingeX = side * leafWidth * 0.14f;
            float handleX = side * leafWidth * 0.82f;
            CreateDoorIronStrap(pivot, side, hingeX, leafHeight * 0.32f, leafWidth, frontZ);
            CreateDoorIronStrap(pivot, side, hingeX, -leafHeight * 0.32f, leafWidth, frontZ);

            CreateLocalBox(pivot, "Gothic Door Pull Backplate", new Vector3(handleX, -leafHeight * 0.04f, frontZ - 0.08f), new Vector3(0.18f, 0.72f, 0.055f), gothicDoorIron);
            CreateLocalBox(pivot, "Gothic Door Pull Grip", new Vector3(handleX - side * 0.06f, -leafHeight * 0.04f, frontZ - 0.14f), new Vector3(0.075f, 0.52f, 0.075f), gothicDoorIronHighlight);
            CreateLocalSphere(pivot, "Gothic Door Lion Knocker", new Vector3(handleX - side * 0.18f, leafHeight * 0.14f, frontZ - 0.13f), Vector3.one * 0.16f, gothicDoorIronHighlight);

            if (leftLeaf)
            {
                CreateLocalBox(pivot, "Gothic Door Lock Plate", new Vector3(leafWidth * 0.94f, -leafHeight * 0.08f, frontZ - 0.12f), new Vector3(0.28f, 0.36f, 0.07f), gothicDoorIron);
                CreateLocalBox(pivot, "Gothic Door Keyhole", new Vector3(leafWidth * 0.96f, -leafHeight * 0.1f, frontZ - 0.17f), new Vector3(0.055f, 0.15f, 0.035f), gothicDoorWoodDark);
            }
        }

        private void CreateDoorIronStrap(Transform parent, float side, float hingeX, float y, float leafWidth, float frontZ)
        {
            CreateLocalBox(parent, "Gothic Door Iron Strap", new Vector3(hingeX + side * leafWidth * 0.38f, y, frontZ - 0.075f), new Vector3(leafWidth * 0.72f, 0.09f, 0.07f), gothicDoorIron);
            CreateLocalBox(parent, "Gothic Door Iron Hinge Plate", new Vector3(hingeX, y, frontZ - 0.09f), new Vector3(0.2f, 0.38f, 0.08f), gothicDoorIron);
            CreateLocalBox(parent, "Gothic Door Spear Tip", new Vector3(hingeX + side * leafWidth * 0.76f, y, frontZ - 0.085f), new Vector3(0.18f, 0.18f, 0.07f), gothicDoorIron);

            for (int i = 0; i < 5; i++)
            {
                float rivetX = hingeX + side * (leafWidth * (0.14f + i * 0.13f));
                CreateLocalSphere(parent, "Gothic Door Iron Rivet", new Vector3(rivetX, y, frontZ - 0.135f), Vector3.one * 0.07f, gothicDoorIronHighlight);
            }

            CreateLocalBox(parent, "Gothic Door Scroll Upper", new Vector3(hingeX + side * leafWidth * 0.22f, y + 0.23f, frontZ - 0.08f), new Vector3(leafWidth * 0.34f, 0.055f, 0.055f), gothicDoorIron);
            CreateLocalBox(parent, "Gothic Door Scroll Lower", new Vector3(hingeX + side * leafWidth * 0.22f, y - 0.23f, frontZ - 0.08f), new Vector3(leafWidth * 0.34f, 0.055f, 0.055f), gothicDoorIron);
            CreateLocalSphere(parent, "Gothic Door Scroll Curl", new Vector3(hingeX + side * leafWidth * 0.42f, y + 0.28f, frontZ - 0.12f), Vector3.one * 0.13f, gothicDoorIron);
            CreateLocalSphere(parent, "Gothic Door Scroll Curl", new Vector3(hingeX + side * leafWidth * 0.42f, y - 0.28f, frontZ - 0.12f), Vector3.one * 0.13f, gothicDoorIron);
        }

        private static Transform CreateDoorPivot(Transform parent, string name, Vector3 localPosition)
        {
            GameObject pivot = new GameObject(name);
            pivot.transform.SetParent(parent, false);
            pivot.transform.localPosition = localPosition;
            return pivot.transform;
        }

        private void CreateChest(string name, Vector3 position, int souls, bool givesStarterSword = false, bool givesStarterShield = false)
        {
            GameObject root = new GameObject(name);
            SetGeneratedParent(root);
            MarkSelectionRoot(root);
            root.transform.position = position;

            DungeonInteractable3D trigger = CreateInteractableBox($"{name} Trigger", position, new Vector3(1.6f, 1.05f, 1.25f), brass);
            trigger.transform.SetParent(root.transform, true);
            Renderer triggerRenderer = trigger.GetComponent<Renderer>();
            if (triggerRenderer) triggerRenderer.enabled = false;
            CreateInvisibleSolidBlocker(root.transform, $"{name} Solid Chest Blocker", new Vector3(0f, 0.72f, 0f), new Vector3(1.42f, 3.05f, 0.98f));

            if (quietEditorPreview)
            {
                CreateLocalBox(root.transform, "Chest Unified Blue Iron Body", new Vector3(0f, 0.03f, 0f), new Vector3(1.36f, 0.72f, 0.9f), chestWood);
                CreateLocalBox(root.transform, "Chest Unified Dark Band", new Vector3(0f, 0.06f, -0.49f), new Vector3(1.42f, 0.14f, 0.08f), chestIron);
                CreateLocalBox(root.transform, "Chest Unified Lock", new Vector3(0f, 0.02f, -0.56f), new Vector3(0.28f, 0.32f, 0.08f), chestGold);
                CreateRivetRow(root.transform, 0.16f, -0.56f, 7);
                GameObject[] revealObjects = CreateChestEquipmentPickups(root.transform, givesStarterSword, givesStarterShield);
                trigger.ConfigureChest(souls, null, givesStarterSword, givesStarterShield, revealObjects);
                return;
            }

            CreateLocalBox(root.transform, "Chest Lower Blue Iron Case", new Vector3(0f, -0.14f, 0f), new Vector3(1.38f, 0.5f, 0.88f), chestWood);
            CreateLocalBox(root.transform, "Chest Front Iron Strap", new Vector3(0f, 0.03f, -0.47f), new Vector3(1.46f, 0.12f, 0.07f), chestIron);
            CreateLocalBox(root.transform, "Chest Back Iron Strap", new Vector3(0f, 0.03f, 0.47f), new Vector3(1.46f, 0.12f, 0.07f), chestIron);
            CreateLocalBox(root.transform, "Chest Left Iron Strap", new Vector3(-0.71f, 0.03f, 0f), new Vector3(0.07f, 0.12f, 0.9f), chestIron);
            CreateLocalBox(root.transform, "Chest Right Iron Strap", new Vector3(0.71f, 0.03f, 0f), new Vector3(0.07f, 0.12f, 0.9f), chestIron);
            CreateLocalBox(root.transform, "Chest Front Bottom Lip", new Vector3(0f, -0.41f, -0.47f), new Vector3(1.5f, 0.1f, 0.08f), chestIron);
            CreateLocalBox(root.transform, "Chest Lock Plate", new Vector3(0f, -0.06f, -0.54f), new Vector3(0.28f, 0.34f, 0.08f), chestGold);
            CreateLocalBox(root.transform, "Chest Lock Keyhole", new Vector3(0f, -0.08f, -0.595f), new Vector3(0.08f, 0.18f, 0.035f), chestIron);

            GameObject lidPivot = new GameObject("Chest Lid Pivot");
            lidPivot.transform.SetParent(root.transform, false);
            lidPivot.transform.localPosition = new Vector3(0f, 0.16f, 0.43f);
            CreateLocalBox(lidPivot.transform, "Chest Curved Lid Core", new Vector3(0f, 0.14f, -0.42f), new Vector3(1.42f, 0.28f, 0.88f), chestWood);
            CreateLocalBox(lidPivot.transform, "Chest Lid Front Band", new Vector3(0f, 0.26f, -0.88f), new Vector3(1.5f, 0.09f, 0.08f), chestIron);
            CreateLocalBox(lidPivot.transform, "Chest Lid Crown Band", new Vector3(0f, 0.31f, -0.42f), new Vector3(1.5f, 0.08f, 0.12f), chestGold);
            CreateLocalBox(lidPivot.transform, "Chest Left Lid Hinge Band", new Vector3(-0.42f, 0.26f, -0.42f), new Vector3(0.08f, 0.18f, 0.92f), chestIron);
            CreateLocalBox(lidPivot.transform, "Chest Right Lid Hinge Band", new Vector3(0.42f, 0.26f, -0.42f), new Vector3(0.08f, 0.18f, 0.92f), chestIron);

            CreateRivetRow(root.transform, -0.53f, -0.47f, 7);
            CreateRivetRow(root.transform, 0.11f, -0.47f, 7);
            CreateRivetRow(lidPivot.transform, 0.34f, -0.88f, 7);

            GameObject treasure = new GameObject("Chest Treasure");
            treasure.transform.SetParent(root.transform, false);
            treasure.transform.localPosition = new Vector3(0f, 0.18f, -0.06f);
            CreateCoinPile(treasure.transform);
            CreateSoulGlow(treasure.transform);

            Light light = treasure.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(0.78f, 0.95f, 1f);
            light.range = 4.2f;
            light.intensity = 1.4f;
            ConfigureEditorPreviewLight(light);

            DungeonChestVisual3D visual = root.AddComponent<DungeonChestVisual3D>();
            visual.Configure(lidPivot.transform, treasure, light);
            GameObject[] equipmentRevealObjects = CreateChestEquipmentPickups(root.transform, givesStarterSword, givesStarterShield);
            trigger.ConfigureChest(souls, visual, givesStarterSword, givesStarterShield, equipmentRevealObjects);
        }

        private GameObject[] CreateChestEquipmentPickups(Transform chestRoot, bool givesStarterSword, bool givesStarterShield)
        {
            int count = (givesStarterSword ? 1 : 0) + (givesStarterShield ? 1 : 0);
            if (count == 0) return null;

            GameObject[] pickups = new GameObject[count];
            int index = 0;
            if (givesStarterSword)
            {
                pickups[index++] = CreateStarterEquipmentPickup(chestRoot, "Starter Sword Pickup", new Vector3(-0.34f, 0.62f, -0.08f), true);
            }

            if (givesStarterShield)
            {
                pickups[index] = CreateStarterEquipmentPickup(chestRoot, "Starter Shield Pickup", new Vector3(0.34f, 0.62f, -0.08f), false);
            }

            return pickups;
        }

        private GameObject CreateStarterEquipmentPickup(Transform parent, string name, Vector3 localPosition, bool sword)
        {
            GameObject pickup = new GameObject(name);
            pickup.transform.SetParent(parent, false);
            pickup.transform.localPosition = localPosition;
            pickup.transform.localRotation = Quaternion.identity;
            BoxCollider collider = pickup.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.size = new Vector3(0.9f, 0.8f, 0.8f);
            collider.center = new Vector3(0f, 0.1f, 0f);

            DungeonInteractable3D interactable = pickup.AddComponent<DungeonInteractable3D>();
            if (sword)
            {
                interactable.ConfigureStarterSwordPickup();
                GameObject blade = CreateLocalBox(pickup.transform, "Pickup Sword Blade", new Vector3(0f, 0.18f, 0f), new Vector3(0.08f, 0.62f, 0.04f), chestIron);
                blade.transform.localRotation = Quaternion.Euler(0f, 0f, -32f);
                GameObject guard = CreateLocalBox(pickup.transform, "Pickup Sword Guard", new Vector3(0f, -0.12f, 0f), new Vector3(0.42f, 0.06f, 0.06f), chestGold);
                guard.transform.localRotation = Quaternion.Euler(0f, 0f, -32f);
            }
            else
            {
                interactable.ConfigureStarterShieldPickup();
                CreateLocalBox(pickup.transform, "Pickup Shield Body", Vector3.zero, new Vector3(0.48f, 0.62f, 0.08f), darkStone);
                CreateLocalBox(pickup.transform, "Pickup Shield Stripe", new Vector3(0f, 0f, -0.055f), new Vector3(0.08f, 0.66f, 0.035f), chestGold);
            }

            pickup.SetActive(false);
            return pickup;
        }

        private GameObject CreateLocalBox(Transform parent, string name, Vector3 localPosition, Vector3 localScale, Material material)
        {
            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.name = name;
            box.transform.SetParent(parent, false);
            box.transform.localPosition = localPosition;
            box.transform.localScale = localScale;
            box.GetComponent<Renderer>().sharedMaterial = material;
            DestroySafely(box.GetComponent<Collider>());
            return box;
        }

        private GameObject CreateLocalSphere(Transform parent, string name, Vector3 localPosition, Vector3 localScale, Material material)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = name;
            sphere.transform.SetParent(parent, false);
            sphere.transform.localPosition = localPosition;
            sphere.transform.localScale = localScale;
            sphere.GetComponent<Renderer>().sharedMaterial = material;
            DestroySafely(sphere.GetComponent<Collider>());
            return sphere;
        }

        private void CreateRivetRow(Transform parent, float y, float z, int count)
        {
            for (int i = 0; i < count; i++)
            {
                float x = -0.54f + i * (1.08f / (count - 1));
                GameObject rivet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                rivet.name = "Chest Raised Rivet";
                rivet.transform.SetParent(parent, false);
                rivet.transform.localPosition = new Vector3(x, y, z);
                rivet.transform.localScale = Vector3.one * 0.055f;
                rivet.GetComponent<Renderer>().sharedMaterial = chestTrim;
                DestroySafely(rivet.GetComponent<Collider>());
            }
        }

        private void CreateSoulGlow(Transform parent)
        {
            GameObject soul = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            soul.name = "Chest Soul Glow";
            soul.transform.SetParent(parent, false);
            soul.transform.localPosition = new Vector3(0f, 0.1f, -0.02f);
            soul.transform.localScale = Vector3.one * 0.28f;
            soul.GetComponent<Renderer>().sharedMaterial = chestGlow;
            DestroySafely(soul.GetComponent<Collider>());

            GameObject wispA = CreateLocalBox(soul.transform, "Chest Soul Wisp A", new Vector3(-0.16f, 0.42f, 0f), new Vector3(0.05f, 0.42f, 0.05f), chestGlow);
            wispA.transform.localRotation = Quaternion.Euler(0f, 0f, -22f);
            GameObject wispB = CreateLocalBox(soul.transform, "Chest Soul Wisp B", new Vector3(0.12f, 0.6f, 0f), new Vector3(0.04f, 0.48f, 0.04f), chestGlow);
            wispB.transform.localRotation = Quaternion.Euler(0f, 0f, 18f);
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
                coin.GetComponent<Renderer>().sharedMaterial = brass;
                DestroySafely(coin.GetComponent<Collider>());
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
            gem.GetComponent<Renderer>().sharedMaterial = material;
            DestroySafely(gem.GetComponent<Collider>());
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
            SetGeneratedParent(pickup);
            pickup.transform.position = position;
            pickup.transform.localScale = coin ? new Vector3(0.45f, 0.08f, 0.45f) : new Vector3(0.55f, 0.55f, 0.55f);
            pickup.GetComponent<Renderer>().sharedMaterial = coin ? brass : potion;
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
            SetGeneratedParent(column);
            column.transform.position = position;
            column.transform.localScale = new Vector3(0.55f, 1.7f, 0.55f);
            column.GetComponent<Renderer>().sharedMaterial = darkStone;
        }

        private void CreateTorch(Vector3 position)
        {
            float side = position.x < 0f ? 1f : -1f;
            Vector3 inward = Vector3.right * side;
            Vector3 lateral = Vector3.forward;

            GameObject torch = new GameObject("Animated Wall Torch");
            SetGeneratedParent(torch);
            MarkSelectionRoot(torch);
            torch.transform.position = position;

            GameObject backStone = CreateBox("Wall Torch Stone Backplate", position - inward * 0.075f + Vector3.up * 0.02f, new Vector3(0.18f, 1.55f, 0.72f), wallTorchStone);
            backStone.transform.SetParent(torch.transform, true);
            DestroySafely(backStone.GetComponent<Collider>());
            GameObject topStone = CreateBox("Wall Torch Chipped Top Stone", position - inward * 0.09f + Vector3.up * 0.78f + lateral * 0.04f, new Vector3(0.19f, 0.22f, 0.62f), runestoneEdge);
            topStone.transform.SetParent(torch.transform, true);
            DestroySafely(topStone.GetComponent<Collider>());
            GameObject lowerStone = CreateBox("Wall Torch Lower Stone Block", position - inward * 0.095f - Vector3.up * 0.58f - lateral * 0.05f, new Vector3(0.2f, 0.3f, 0.58f), runestoneFace);
            lowerStone.transform.SetParent(torch.transform, true);
            DestroySafely(lowerStone.GetComponent<Collider>());

            Vector3 bracketCenter = position + inward * 0.18f - Vector3.up * 0.05f;
            CreateTorchCylinder(torch.transform, "Wall Torch Iron Bracket Bar", bracketCenter, inward, 0.035f, 0.5f, wallTorchIron);
            CreateTorchCylinder(torch.transform, "Wall Torch Curled Support", position + inward * 0.11f + Vector3.up * 0.22f, Vector3.up + inward * 0.34f, 0.024f, 0.34f, wallTorchIron);
            CreateTorchCylinder(torch.transform, "Wall Torch Lower Curled Support", position + inward * 0.11f - Vector3.up * 0.28f, Vector3.up - inward * 0.3f, 0.024f, 0.36f, wallTorchIron);
            CreateLocalSphere(torch.transform, "Wall Torch Upper Iron Curl", inward * 0.23f + Vector3.up * 0.4f, new Vector3(0.09f, 0.09f, 0.09f), wallTorchIron);
            CreateLocalSphere(torch.transform, "Wall Torch Lower Iron Curl", inward * 0.22f - Vector3.up * 0.48f, new Vector3(0.08f, 0.08f, 0.08f), wallTorchIron);
            CreateTorchCylinder(torch.transform, "Wall Torch Iron Mount Pin Top", position - inward * 0.18f + Vector3.up * 0.43f, lateral, 0.03f, 0.22f, wallTorchIron);
            CreateTorchCylinder(torch.transform, "Wall Torch Iron Mount Pin Bottom", position - inward * 0.18f - Vector3.up * 0.55f, lateral, 0.03f, 0.22f, wallTorchIron);

            Vector3 handleDirection = (inward * 0.46f + Vector3.up * 0.72f).normalized;
            Vector3 handleCenter = position + inward * 0.36f - Vector3.up * 0.18f;
            CreateTorchCylinder(torch.transform, "Wall Torch Dark Wooden Handle", handleCenter, handleDirection, 0.065f, 0.85f, wallTorchWood);
            CreateTorchCylinder(torch.transform, "Wall Torch Lower Iron Band", handleCenter - handleDirection * 0.28f, handleDirection, 0.074f, 0.08f, wallTorchIron);
            CreateTorchCylinder(torch.transform, "Wall Torch Upper Iron Band", handleCenter + handleDirection * 0.2f, handleDirection, 0.074f, 0.08f, wallTorchIron);

            GameObject coal = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            coal.name = "Wall Torch Ember Core";
            coal.transform.SetParent(torch.transform, true);
            coal.transform.position = position + inward * 0.56f + Vector3.up * 0.32f;
            coal.transform.localScale = new Vector3(0.24f, 0.16f, 0.24f);
            coal.GetComponent<Renderer>().sharedMaterial = ember;
            DestroySafely(coal.GetComponent<Collider>());

            Vector3 basketCenter = position + inward * 0.56f + Vector3.up * 0.36f;
            CreateTorchBasket(torch.transform, basketCenter, inward, lateral);

            GameObject flame = new GameObject("Torch Flame");
            flame.transform.SetParent(torch.transform, true);
            flame.transform.position = position + inward * 0.57f + Vector3.up * 0.45f;
            flame.transform.localScale = Vector3.one * 1.18f;
            flame.AddComponent<SpriteRenderer>();

            Light light = flame.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1f, 0.48f, 0.18f);
            light.intensity = 2.1f;
            light.range = 6f;

            flame.AddComponent<AnimatedWallTorch3D>().Configure(light, inward, quietEditorPreview);
        }

        private void CreateTorchBasket(Transform parent, Vector3 center, Vector3 inward, Vector3 lateral)
        {
            CreateTorchCylinder(parent, "Wall Torch Basket Top Band Front", center + Vector3.up * 0.21f, lateral, 0.026f, 0.42f, wallTorchIron);
            CreateTorchCylinder(parent, "Wall Torch Basket Top Band Side", center + Vector3.up * 0.21f, inward, 0.026f, 0.36f, wallTorchIron);
            CreateTorchCylinder(parent, "Wall Torch Basket Lower Band Front", center - Vector3.up * 0.02f, lateral, 0.026f, 0.34f, wallTorchIron);
            CreateTorchCylinder(parent, "Wall Torch Basket Lower Band Side", center - Vector3.up * 0.02f, inward, 0.026f, 0.3f, wallTorchIron);

            Vector3[] rodOffsets =
            {
                lateral * 0.18f,
                -lateral * 0.18f,
                inward * 0.15f,
                -inward * 0.1f,
                lateral * 0.12f + inward * 0.12f,
                -lateral * 0.12f + inward * 0.12f
            };

            for (int i = 0; i < rodOffsets.Length; i++)
            {
                Vector3 lean = Vector3.up + rodOffsets[i].normalized * 0.18f;
                CreateTorchCylinder(parent, $"Wall Torch Basket Iron Rib {i + 1}", center + rodOffsets[i] + Vector3.up * 0.08f, lean, 0.022f, 0.5f, wallTorchIron);
            }
        }

        private GameObject CreateTorchCylinder(Transform parent, string name, Vector3 position, Vector3 direction, float radius, float length, Material material)
        {
            GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cylinder.name = name;
            cylinder.transform.SetParent(parent, true);
            cylinder.transform.position = position;
            cylinder.transform.rotation = Quaternion.FromToRotation(Vector3.up, direction.normalized);
            cylinder.transform.localScale = new Vector3(radius, length * 0.5f, radius);
            cylinder.GetComponent<Renderer>().sharedMaterial = material;
            DestroySafely(cylinder.GetComponent<Collider>());
            return cylinder;
        }

        private void CreateSpikeRow(Vector3 origin, int count)
        {
            Transform group = CreateGroup("Metal Bar Row", origin);
            for (int i = 0; i < count; i++)
            {
                GameObject spike = CreateSpikeTip("Buried Spike Tip", origin + new Vector3((i - count * 0.5f) * 0.62f, 0.22f, 0f), 0.36f, 0.46f, chestIron);
                spike.transform.SetParent(group, true);
            }
        }

        private GameObject CreateSpikeTip(string name, Vector3 position, float baseWidth, float height, Material material)
        {
            GameObject spike = new GameObject(name);
            SetGeneratedParent(spike);
            spike.transform.position = position;

            float half = baseWidth * 0.5f;
            Mesh mesh = new Mesh
            {
                name = name + " Mesh",
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
            spike.AddComponent<MeshRenderer>().sharedMaterial = material;
            return spike;
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
            SetGeneratedParent(box);
            box.transform.position = position;
            box.transform.localScale = scale;
            Renderer renderer = box.GetComponent<Renderer>();
            renderer.sharedMaterial = material;
            ApplyBoxTextureTiling(renderer, scale);
            return box;
        }

        private GameObject CreateInvisibleSolidBlocker(Transform parent, string name, Vector3 localCenter, Vector3 size)
        {
            GameObject blocker = new GameObject(name);
            blocker.transform.SetParent(parent, false);
            blocker.transform.localPosition = Vector3.zero;
            blocker.transform.localRotation = Quaternion.identity;
            BoxCollider collider = blocker.AddComponent<BoxCollider>();
            collider.isTrigger = false;
            collider.center = localCenter;
            collider.size = size;
            return blocker;
        }

        private GameObject CreateDecorationBox(string name, Vector3 position, Vector3 scale, Material material)
        {
            GameObject box = CreateBox(name, position, scale, material);
            Collider collider = box.GetComponent<Collider>();
            if (collider)
            {
                if (Application.isPlaying) DestroySafely(collider);
                else Object.DestroyImmediate(collider);
            }

            return box;
        }

        private void CreateSideParapets(string prefix, float xExtent, float y, float zCenter, float zLength, Material material)
        {
            CreateBox($"{prefix} Left Parapet Cap", new Vector3(-xExtent, y, zCenter), new Vector3(0.45f, 1.25f, zLength), material);
            CreateBox($"{prefix} Right Parapet Cap", new Vector3(xExtent, y, zCenter), new Vector3(0.45f, 1.25f, zLength), material);
        }

        private void CreateFloorInlays(string prefix, float zStart, float zEnd, float width, float y)
        {
            Material inlayMaterial = NewMaterial($"{prefix} Dark Floor Inlay", new Color(0.12f, 0.13f, 0.14f));
            for (float z = zStart; z <= zEnd; z += 4f)
            {
                CreateDecorationBox($"{prefix} Cross Floor Joint", new Vector3(0f, y, z), new Vector3(width, 0.035f, 0.045f), inlayMaterial);
            }

            CreateDecorationBox($"{prefix} Left Long Floor Joint", new Vector3(-width * 0.28f, y + 0.004f, (zStart + zEnd) * 0.5f), new Vector3(0.045f, 0.035f, zEnd - zStart), inlayMaterial);
            CreateDecorationBox($"{prefix} Right Long Floor Joint", new Vector3(width * 0.28f, y + 0.004f, (zStart + zEnd) * 0.5f), new Vector3(0.045f, 0.035f, zEnd - zStart), inlayMaterial);
        }

        private void CreateRubbleCluster(string prefix, Vector3 center, int count)
        {
            for (int i = 0; i < count; i++)
            {
                float angle = i * 137.5f * Mathf.Deg2Rad;
                float radius = 0.25f + (i % 4) * 0.18f;
                Vector3 position = center + new Vector3(Mathf.Cos(angle) * radius, 0.08f + (i % 3) * 0.025f, Mathf.Sin(angle) * radius);
                Vector3 scale = new Vector3(0.22f + (i % 3) * 0.08f, 0.14f + (i % 2) * 0.05f, 0.2f + (i % 4) * 0.05f);
                GameObject rubble = CreateDecorationBox($"{prefix} Rubble Stone", position, scale, i % 2 == 0 ? darkStone : ashStone);
                rubble.transform.rotation = Quaternion.Euler(0f, i * 31f, 0f);
            }
        }

        private void CreateFloorCrack(string name, Vector3 center, float length, float angle)
        {
            GameObject crack = CreateDecorationBox(name, center, new Vector3(0.065f, 0.025f, length), shadowStone);
            crack.transform.rotation = Quaternion.Euler(0f, angle, 0f);

            GameObject branchA = CreateDecorationBox($"{name} Branch A", center + new Vector3(0.35f, 0.003f, -0.28f), new Vector3(0.045f, 0.025f, length * 0.36f), shadowStone);
            branchA.transform.rotation = Quaternion.Euler(0f, angle + 42f, 0f);

            GameObject branchB = CreateDecorationBox($"{name} Branch B", center + new Vector3(-0.28f, 0.006f, 0.36f), new Vector3(0.04f, 0.025f, length * 0.28f), shadowStone);
            branchB.transform.rotation = Quaternion.Euler(0f, angle - 38f, 0f);
        }

        private void CreateBrokenCrates(string prefix, Vector3 center, float yaw)
        {
            Vector3 right = Quaternion.Euler(0f, yaw, 0f) * Vector3.right;
            Vector3 forward = Quaternion.Euler(0f, yaw, 0f) * Vector3.forward;

            CreateReinforcedCrate(prefix, center + Vector3.up * 0.38f, yaw);
            CreateWoodenBarrel($"{prefix} Barrel", center + right * 1.05f + Vector3.up * 0.5f + forward * 0.08f, yaw - 8f);

            GameObject plankA = CreateDecorationBox($"{prefix} Loose Plank A", center - right * 0.7f + forward * 0.58f + Vector3.up * 0.05f, new Vector3(0.92f, 0.08f, 0.22f), wallTorchWood);
            plankA.transform.rotation = Quaternion.Euler(0f, yaw + 31f, 8f);
            GameObject plankB = CreateDecorationBox($"{prefix} Loose Plank B", center + right * 0.45f - forward * 0.72f + Vector3.up * 0.04f, new Vector3(0.68f, 0.07f, 0.18f), wallTorchWood);
            plankB.transform.rotation = Quaternion.Euler(0f, yaw - 25f, -6f);
        }

        private void CreateReinforcedCrate(string prefix, Vector3 center, float yaw)
        {
            Quaternion rotation = Quaternion.Euler(0f, yaw, 0f);
            Vector3 right = rotation * Vector3.right;
            Vector3 up = Vector3.up;
            Vector3 forward = rotation * Vector3.forward;

            GameObject body = CreateDecorationBox($"{prefix} Reinforced Crate Body", center, new Vector3(0.95f, 0.76f, 0.78f), wallTorchWood);
            body.transform.rotation = rotation;

            for (int i = -1; i <= 1; i++)
            {
                Vector3 plankCenter = center + up * (i * 0.22f) - forward * 0.405f;
                GameObject plank = CreateDecorationBox($"{prefix} Front Wood Plank", plankCenter, new Vector3(0.9f, 0.055f, 0.035f), i == 0 ? wood : wallTorchWood);
                plank.transform.rotation = rotation;
            }

            CreateMetalPlate($"{prefix} Top Iron Band", center + up * 0.42f - forward * 0.41f, new Vector3(0.98f, 0.07f, 0.045f), rotation);
            CreateMetalPlate($"{prefix} Bottom Iron Band", center - up * 0.42f - forward * 0.41f, new Vector3(0.98f, 0.07f, 0.045f), rotation);
            CreateMetalPlate($"{prefix} Left Iron Spine", center - right * 0.52f - forward * 0.41f, new Vector3(0.07f, 0.86f, 0.045f), rotation);
            CreateMetalPlate($"{prefix} Right Iron Spine", center + right * 0.52f - forward * 0.41f, new Vector3(0.07f, 0.86f, 0.045f), rotation);

            Vector3[] corners =
            {
                center - right * 0.52f + up * 0.42f - forward * 0.44f,
                center + right * 0.52f + up * 0.42f - forward * 0.44f,
                center - right * 0.52f - up * 0.42f - forward * 0.44f,
                center + right * 0.52f - up * 0.42f - forward * 0.44f
            };

            for (int i = 0; i < corners.Length; i++)
            {
                CreateMetalPlate($"{prefix} Iron Corner Plate {i + 1}", corners[i], new Vector3(0.24f, 0.22f, 0.055f), rotation);
                CreateLocalSphereOnWorld($"{prefix} Corner Rivet {i + 1}", corners[i] - up * 0.035f, new Vector3(0.055f, 0.055f, 0.055f), chestIron);
            }

            CreateWorldCylinder($"{prefix} Front Iron Handle", center - forward * 0.51f - up * 0.05f, right, 0.026f, 0.44f, chestIron);
            CreateMetalPlate($"{prefix} Lock Plate", center - forward * 0.54f + up * 0.25f, new Vector3(0.18f, 0.24f, 0.06f), rotation);
        }

        private void CreateWoodenBarrel(string name, Vector3 center, float yaw)
        {
            Quaternion rotation = Quaternion.Euler(0f, yaw, 0f);
            CreateBarrelCylinder(name, center, rotation, 0.72f, 1.0f, wallTorchWood);
            CreateBarrelCylinder($"{name} Top Iron Band", center + Vector3.up * 0.34f, rotation, 0.76f, 0.075f, chestIron);
            CreateBarrelCylinder($"{name} Middle Iron Band", center, rotation, 0.78f, 0.07f, chestIron);
            CreateBarrelCylinder($"{name} Bottom Iron Band", center - Vector3.up * 0.34f, rotation, 0.76f, 0.075f, chestIron);

            for (int i = 0; i < 8; i++)
            {
                float angle = i * 45f;
                Vector3 side = rotation * (Quaternion.Euler(0f, angle, 0f) * Vector3.forward);
                GameObject stave = CreateDecorationBox($"{name} Wood Stave", center + side * 0.37f, new Vector3(0.035f, 0.92f, 0.04f), i % 2 == 0 ? wallTorchWood : wood);
                stave.transform.rotation = Quaternion.LookRotation(side, Vector3.up);
            }
        }

        private void CreateMetalPlate(string name, Vector3 position, Vector3 scale, Quaternion rotation)
        {
            GameObject plate = CreateDecorationBox(name, position, scale, chestIron);
            plate.transform.rotation = rotation;
        }

        private void CreateLocalSphereOnWorld(string name, Vector3 position, Vector3 scale, Material material)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = name;
            SetGeneratedParent(sphere);
            sphere.transform.position = position;
            sphere.transform.localScale = scale;
            sphere.GetComponent<Renderer>().sharedMaterial = material;
            DestroySafely(sphere.GetComponent<Collider>());
        }

        private void CreateBarrelCylinder(string name, Vector3 center, Quaternion rotation, float diameter, float height, Material material)
        {
            GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cylinder.name = name;
            SetGeneratedParent(cylinder);
            cylinder.transform.position = center;
            cylinder.transform.rotation = rotation;
            cylinder.transform.localScale = new Vector3(diameter, height * 0.5f, diameter);
            cylinder.GetComponent<Renderer>().sharedMaterial = material;
            DestroySafely(cylinder.GetComponent<Collider>());
        }

        private void CreateWorldCylinder(string name, Vector3 position, Vector3 direction, float radius, float length, Material material)
        {
            GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cylinder.name = name;
            SetGeneratedParent(cylinder);
            cylinder.transform.position = position;
            cylinder.transform.rotation = Quaternion.FromToRotation(Vector3.up, direction.normalized);
            cylinder.transform.localScale = new Vector3(radius, length * 0.5f, radius);
            cylinder.GetComponent<Renderer>().sharedMaterial = material;
            DestroySafely(cylinder.GetComponent<Collider>());
        }

        private void CreateBrokenStatue(string prefix, Vector3 center, float yaw)
        {
            GameObject baseBlock = CreateDecorationBox($"{prefix} Base", center + Vector3.up * 0.18f, new Vector3(1.15f, 0.36f, 1.0f), darkStone);
            baseBlock.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
            GameObject torso = CreateDecorationBox($"{prefix} Broken Torso", center + new Vector3(0f, 0.77f, 0f), new Vector3(0.58f, 0.9f, 0.42f), ashStone);
            torso.transform.rotation = Quaternion.Euler(-8f, yaw + 6f, 4f);
            GameObject shoulder = CreateDecorationBox($"{prefix} Fallen Shoulder", center + new Vector3(0.56f, 0.42f, 0.22f), new Vector3(0.58f, 0.24f, 0.32f), ashStone);
            shoulder.transform.rotation = Quaternion.Euler(0f, yaw + 34f, -12f);
            CreateRubbleCluster($"{prefix} Rubble", center + new Vector3(0.68f, 0f, -0.44f), 7);
        }

        private void CreateBattleRemnants(string prefix, Vector3 center, float yaw)
        {
            Quaternion rotation = Quaternion.Euler(0f, yaw, 0f);
            Vector3 right = rotation * Vector3.right;
            Vector3 forward = rotation * Vector3.forward;

            CreateFloorStain($"{prefix} Dark Scorch", center, 1.25f, 0.82f, yaw + 12f);
            CreateFloorCrack($"{prefix} Thin Crack", center + right * 0.45f + forward * 0.15f, 1.75f, yaw - 24f);
            CreateBrokenSword($"{prefix} Broken Sword", center - right * 0.25f + forward * 0.28f + Vector3.up * 0.08f, yaw + 58f);
            CreateBrokenSpear($"{prefix} Broken Spear", center + right * 0.62f - forward * 0.35f + Vector3.up * 0.08f, yaw - 12f);
            CreateFallenShield($"{prefix} Fallen Shield", center - right * 0.74f - forward * 0.42f + Vector3.up * 0.08f, yaw + 18f);
            CreateRubbleCluster($"{prefix} Small Stones", center + right * 0.85f + forward * 0.62f, 5);
        }

        private void CreateFallenSkeleton(string prefix, Vector3 center, float yaw)
        {
            if (!EnemySetDressingEnabled) return;

            Quaternion rotation = Quaternion.Euler(0f, yaw, 0f);
            Vector3 right = rotation * Vector3.right;
            Vector3 forward = rotation * Vector3.forward;
            Material bone = NewMaterial($"{prefix} Old Bone", new Color(0.66f, 0.61f, 0.49f));
            Material oldBoneDark = NewMaterial($"{prefix} Weathered Bone Shadow", new Color(0.42f, 0.38f, 0.3f));

            CreateFloorStain($"{prefix} Old Ash Stain", center + forward * 0.05f, 1.58f, 1.05f, yaw - 8f);
            CreateRibCage(prefix, center + Vector3.up * 0.12f, right, forward, bone);
            CreateBone($"{prefix} Spine", center + Vector3.up * 0.13f, forward, 0.04f, 0.88f, bone);
            CreateBone($"{prefix} Shoulder Bone", center + forward * 0.34f + Vector3.up * 0.16f, right, 0.038f, 0.72f, bone);
            CreateBone($"{prefix} Hip Bone", center - forward * 0.37f + Vector3.up * 0.13f, right, 0.046f, 0.58f, bone);
            CreateLocalSphereOnWorld($"{prefix} Left Pelvis Plate", center - forward * 0.37f - right * 0.26f + Vector3.up * 0.13f, new Vector3(0.18f, 0.055f, 0.14f), bone);
            CreateLocalSphereOnWorld($"{prefix} Right Pelvis Plate", center - forward * 0.37f + right * 0.26f + Vector3.up * 0.13f, new Vector3(0.18f, 0.055f, 0.14f), bone);

            CreateBone($"{prefix} Left Upper Arm Bone", center + forward * 0.22f - right * 0.48f + Vector3.up * 0.11f, -right + forward * 0.26f, 0.035f, 0.58f, bone);
            CreateBone($"{prefix} Left Forearm Bone", center + forward * 0.03f - right * 0.85f + Vector3.up * 0.1f, -right - forward * 0.18f, 0.028f, 0.5f, bone);
            CreateBone($"{prefix} Right Upper Arm Bone", center + forward * 0.16f + right * 0.52f + Vector3.up * 0.11f, right + forward * 0.1f, 0.035f, 0.56f, bone);
            CreateBone($"{prefix} Right Forearm Bone", center - forward * 0.03f + right * 0.86f + Vector3.up * 0.1f, right - forward * 0.24f, 0.028f, 0.48f, bone);
            CreateHandBones($"{prefix} Left Hand", center - forward * 0.1f - right * 1.12f + Vector3.up * 0.09f, right, forward, bone);
            CreateHandBones($"{prefix} Right Hand", center - forward * 0.14f + right * 1.1f + Vector3.up * 0.09f, -right, forward, bone);

            CreateBone($"{prefix} Left Thigh Bone", center - forward * 0.64f - right * 0.21f + Vector3.up * 0.1f, -forward - right * 0.16f, 0.041f, 0.72f, bone);
            CreateBone($"{prefix} Left Shin Bone", center - forward * 1.11f - right * 0.34f + Vector3.up * 0.09f, -forward + right * 0.23f, 0.034f, 0.64f, bone);
            CreateBone($"{prefix} Right Thigh Bone", center - forward * 0.62f + right * 0.2f + Vector3.up * 0.1f, -forward + right * 0.2f, 0.041f, 0.7f, bone);
            CreateBone($"{prefix} Right Shin Bone", center - forward * 1.11f + right * 0.42f + Vector3.up * 0.09f, -forward - right * 0.18f, 0.034f, 0.62f, bone);
            CreateFootBones($"{prefix} Left Foot", center - forward * 1.44f - right * 0.18f + Vector3.up * 0.08f, right, forward, bone);
            CreateFootBones($"{prefix} Right Foot", center - forward * 1.42f + right * 0.22f + Vector3.up * 0.08f, right, forward, bone);

            GameObject skull = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            skull.name = $"{prefix} Skull";
            SetGeneratedParent(skull);
            skull.transform.position = center + forward * 0.82f + right * 0.12f + Vector3.up * 0.17f;
            skull.transform.localScale = new Vector3(0.25f, 0.2f, 0.23f);
            skull.transform.rotation = Quaternion.Euler(0f, yaw + 18f, 14f);
            skull.GetComponent<Renderer>().sharedMaterial = bone;
            DestroySafely(skull.GetComponent<Collider>());

            CreateLocalSphereOnWorld($"{prefix} Eye Hollow Left", skull.transform.position + right * 0.06f + forward * 0.1f + Vector3.up * 0.025f, new Vector3(0.043f, 0.026f, 0.024f), shadowStone);
            CreateLocalSphereOnWorld($"{prefix} Eye Hollow Right", skull.transform.position - right * 0.045f + forward * 0.1f + Vector3.up * 0.02f, new Vector3(0.04f, 0.024f, 0.022f), shadowStone);
            GameObject jaw = CreateDecorationBox($"{prefix} Broken Jaw", skull.transform.position + forward * 0.08f - Vector3.up * 0.11f, new Vector3(0.2f, 0.035f, 0.07f), bone);
            jaw.transform.rotation = Quaternion.Euler(0f, yaw + 18f, 0f);

            CreateBoneShard($"{prefix} Loose Rib A", center + forward * 0.58f - right * 0.82f + Vector3.up * 0.08f, right + forward * 0.18f, 0.026f, 0.48f, bone);
            CreateBoneShard($"{prefix} Loose Rib B", center - forward * 0.18f + right * 0.95f + Vector3.up * 0.08f, right - forward * 0.14f, 0.024f, 0.42f, bone);
            CreateBoneShard($"{prefix} Loose Femur", center - forward * 1.05f - right * 0.9f + Vector3.up * 0.08f, right + forward * 0.34f, 0.04f, 0.74f, bone);
            CreateBoneShard($"{prefix} Broken Bone Splinter A", center + forward * 0.68f + right * 0.78f + Vector3.up * 0.07f, forward - right * 0.22f, 0.018f, 0.28f, oldBoneDark);
            CreateBoneShard($"{prefix} Broken Bone Splinter B", center - forward * 0.62f + right * 0.78f + Vector3.up * 0.07f, right + forward * 0.5f, 0.016f, 0.34f, oldBoneDark);

            CreateBrokenSword($"{prefix} Rusted Sword", center + right * 0.78f + forward * 0.2f + Vector3.up * 0.08f, yaw - 42f);
            CreateFallenShield($"{prefix} Split Shield", center - right * 0.86f - forward * 0.1f + Vector3.up * 0.08f, yaw + 50f);
            CreateRubbleCluster($"{prefix} Bone Dust", center + forward * 0.08f - right * 0.08f, 7);
        }

        private void CreateFloorStain(string name, Vector3 center, float width, float depth, float yaw)
        {
            GameObject stain = CreateDecorationBox(name, center + Vector3.up * 0.006f, new Vector3(width, 0.018f, depth), charcoal);
            stain.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        }

        private void CreateBone(string name, Vector3 center, Vector3 direction, float radius, float length, Material material)
        {
            Vector3 normalized = direction.normalized;
            CreateWorldCylinder(name, center, normalized, radius, length, material);
            CreateLocalSphereOnWorld($"{name} Knuckle A", center - normalized * (length * 0.5f), Vector3.one * radius * 2.35f, material);
            CreateLocalSphereOnWorld($"{name} Knuckle B", center + normalized * (length * 0.5f), Vector3.one * radius * 2.35f, material);
        }

        private void CreateBoneShard(string name, Vector3 center, Vector3 direction, float radius, float length, Material material)
        {
            CreateWorldCylinder(name, center, direction.normalized, radius, length, material);
        }

        private void CreateRibCage(string prefix, Vector3 center, Vector3 right, Vector3 forward, Material bone)
        {
            for (int i = 0; i < 6; i++)
            {
                float forwardOffset = 0.25f - i * 0.085f;
                float sideReach = 0.28f + i * 0.045f;
                float length = 0.38f + i * 0.025f;
                Vector3 ribCenter = center + forward * forwardOffset + Vector3.up * (0.022f - i * 0.003f);
                CreateBoneShard($"{prefix} Left Rib {i + 1}", ribCenter - right * (sideReach * 0.5f), -right - forward * 0.24f, 0.018f, length, bone);
                CreateBoneShard($"{prefix} Right Rib {i + 1}", ribCenter + right * (sideReach * 0.5f), right - forward * 0.24f, 0.018f, length, bone);
            }
        }

        private void CreateHandBones(string prefix, Vector3 center, Vector3 outward, Vector3 forward, Material bone)
        {
            CreateLocalSphereOnWorld($"{prefix} Palm", center, new Vector3(0.075f, 0.025f, 0.055f), bone);
            for (int i = 0; i < 4; i++)
            {
                float spread = (i - 1.5f) * 0.045f;
                Vector3 fingerCenter = center + outward * 0.12f + forward * spread;
                CreateBoneShard($"{prefix} Finger {i + 1}", fingerCenter, outward + forward * spread * 0.8f, 0.011f, 0.2f, bone);
            }
        }

        private void CreateFootBones(string prefix, Vector3 center, Vector3 right, Vector3 forward, Material bone)
        {
            CreateLocalSphereOnWorld($"{prefix} Heel", center, new Vector3(0.095f, 0.032f, 0.065f), bone);
            for (int i = 0; i < 5; i++)
            {
                float spread = (i - 2f) * 0.055f;
                Vector3 toeCenter = center - forward * 0.13f + right * spread;
                CreateBoneShard($"{prefix} Toe {i + 1}", toeCenter, -forward + right * spread * 0.55f, 0.012f, 0.18f, bone);
            }
        }

        private void CreateBrokenSword(string name, Vector3 center, float yaw)
        {
            Quaternion rotation = Quaternion.Euler(0f, yaw, 0f);
            Vector3 forward = rotation * Vector3.forward;
            Vector3 right = rotation * Vector3.right;

            CreateWorldCylinder($"{name} Grip", center - forward * 0.34f, forward, 0.035f, 0.28f, wallTorchWood);
            CreateWorldCylinder($"{name} Guard", center - forward * 0.18f, right, 0.027f, 0.42f, chestIron);
            GameObject bladeA = CreateDecorationBox($"{name} Broken Blade A", center + forward * 0.13f, new Vector3(0.08f, 0.035f, 0.58f), chestIron);
            bladeA.transform.rotation = rotation;
            GameObject bladeB = CreateDecorationBox($"{name} Broken Blade Tip", center + forward * 0.62f + right * 0.08f, new Vector3(0.07f, 0.03f, 0.24f), chestIron);
            bladeB.transform.rotation = Quaternion.Euler(0f, yaw + 18f, 0f);
        }

        private void CreateBrokenSpear(string name, Vector3 center, float yaw)
        {
            Quaternion rotation = Quaternion.Euler(0f, yaw, 0f);
            Vector3 forward = rotation * Vector3.forward;
            Vector3 right = rotation * Vector3.right;

            CreateWorldCylinder($"{name} Shaft A", center - forward * 0.28f, forward, 0.025f, 0.72f, wallTorchWood);
            CreateWorldCylinder($"{name} Shaft B", center + forward * 0.5f + right * 0.06f, forward, 0.023f, 0.48f, wallTorchWood);
            GameObject tip = CreateDecorationBox($"{name} Iron Tip", center + forward * 0.82f + right * 0.08f, new Vector3(0.13f, 0.04f, 0.22f), chestIron);
            tip.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        }

        private void CreateFallenShield(string name, Vector3 center, float yaw)
        {
            Quaternion rotation = Quaternion.Euler(0f, yaw, 0f);
            GameObject shield = CreateDecorationBox($"{name} Shield Body", center, new Vector3(0.56f, 0.055f, 0.72f), darkStone);
            shield.transform.rotation = rotation;
            GameObject boss = CreateDecorationBox($"{name} Iron Boss", center + Vector3.up * 0.035f, new Vector3(0.2f, 0.05f, 0.2f), chestIron);
            boss.transform.rotation = rotation;
            GameObject crack = CreateDecorationBox($"{name} Split Mark", center + Vector3.up * 0.07f, new Vector3(0.045f, 0.03f, 0.62f), shadowStone);
            crack.transform.rotation = Quaternion.Euler(0f, yaw + 12f, 0f);
        }

        private void CreateFloorBrazier(string name, Vector3 center)
        {
            GameObject root = new GameObject(name);
            SetGeneratedParent(root);
            MarkSelectionRoot(root);
            root.transform.position = center;

            CreateBrazierCylinder(root.transform, $"{name} Round Stone Base", center + Vector3.up * 0.08f, 1.15f, 0.16f, runestoneEdge);
            CreateBrazierCylinder(root.transform, $"{name} Chipped Inner Stone", center + Vector3.up * 0.18f, 0.82f, 0.16f, runestoneFace);
            CreateBrazierStoneBlock(root.transform, $"{name} Front Stone Support", center + new Vector3(0f, 0.38f, -0.46f), new Vector3(0.42f, 0.42f, 0.24f), 0f);
            CreateBrazierStoneBlock(root.transform, $"{name} Back Stone Support", center + new Vector3(0f, 0.38f, 0.46f), new Vector3(0.42f, 0.42f, 0.24f), 0f);
            CreateBrazierStoneBlock(root.transform, $"{name} Left Stone Support", center + new Vector3(-0.46f, 0.38f, 0f), new Vector3(0.24f, 0.42f, 0.42f), 0f);
            CreateBrazierStoneBlock(root.transform, $"{name} Right Stone Support", center + new Vector3(0.46f, 0.38f, 0f), new Vector3(0.24f, 0.42f, 0.42f), 0f);

            CreateBrazierCylinder(root.transform, $"{name} Dark Iron Bowl", center + Vector3.up * 0.68f, 0.98f, 0.18f, wallTorchIron);
            CreateBrazierCylinder(root.transform, $"{name} Rusted Bowl Lip", center + Vector3.up * 0.78f, 1.08f, 0.06f, chestIron);
            CreateBrazierCylinder(root.transform, $"{name} Charcoal Bed", center + Vector3.up * 0.83f, 0.72f, 0.06f, charcoal);

            for (int i = 0; i < 10; i++)
            {
                float angle = i * 36f;
                float radius = 0.16f + (i % 3) * 0.06f;
                Vector3 coalPosition = center + Quaternion.Euler(0f, angle, 0f) * new Vector3(radius, 0.89f, 0f);
                CreateLocalSphere(root.transform, $"{name} Hot Coal", coalPosition - center, new Vector3(0.11f, 0.055f, 0.11f), i % 2 == 0 ? ember : charcoal);
            }

            CreateBrazierLog(root.transform, $"{name} Charred Log A", center + Vector3.up * 0.96f, 28f);
            CreateBrazierLog(root.transform, $"{name} Charred Log B", center + Vector3.up * 1.0f, 112f);
            CreateBrazierLog(root.transform, $"{name} Charred Log C", center + Vector3.up * 1.04f, -34f);

            CreateBrazierFlame(root.transform, $"{name} Flame Core", center + new Vector3(0f, 1.16f, 0f), 0.78f, Vector3.forward);
            CreateBrazierFlame(root.transform, $"{name} Flame Left", center + new Vector3(-0.18f, 1.05f, 0.05f), 0.46f, Vector3.left + Vector3.forward * 0.3f);
            CreateBrazierFlame(root.transform, $"{name} Flame Right", center + new Vector3(0.2f, 1.08f, -0.03f), 0.5f, Vector3.right + Vector3.back * 0.25f);

            Light light = root.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1f, 0.42f, 0.14f);
            light.intensity = 1.9f;
            light.range = 5.6f;
            ConfigureEditorPreviewLight(light);
        }

        private void CreateBrazierCylinder(Transform parent, string name, Vector3 position, float diameter, float height, Material material)
        {
            GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cylinder.name = name;
            cylinder.transform.SetParent(parent, true);
            cylinder.transform.position = position;
            cylinder.transform.localScale = new Vector3(diameter, height * 0.5f, diameter);
            cylinder.GetComponent<Renderer>().sharedMaterial = material;
            DestroySafely(cylinder.GetComponent<Collider>());
        }

        private void CreateBrazierStoneBlock(Transform parent, string name, Vector3 position, Vector3 scale, float yaw)
        {
            GameObject block = CreateDecorationBox(name, position, scale, runestoneFace);
            block.transform.SetParent(parent, true);
            block.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        }

        private void CreateBrazierLog(Transform parent, string name, Vector3 center, float yaw)
        {
            Vector3 direction = Quaternion.Euler(0f, yaw, 0f) * Vector3.forward;
            CreateTorchCylinder(parent, name, center, direction + Vector3.up * 0.08f, 0.075f, 0.86f, wood);
            CreateTorchCylinder(parent, $"{name} Ember Seam", center + Vector3.up * 0.015f, direction + Vector3.up * 0.08f, 0.025f, 0.88f, ember);
        }

        private void CreateBrazierFlame(Transform parent, string name, Vector3 position, float scale, Vector3 wind)
        {
            GameObject flame = new GameObject(name);
            flame.transform.SetParent(parent, true);
            flame.transform.position = position;
            flame.transform.localScale = Vector3.one * scale;
            flame.AddComponent<SpriteRenderer>();

            Light light = flame.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1f, 0.45f, 0.13f);
            light.intensity = 1.15f;
            light.range = 3.4f;
            flame.AddComponent<AnimatedWallTorch3D>().Configure(light, wind, quietEditorPreview);
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
            if (!renderer || !renderer.sharedMaterial || !renderer.sharedMaterial.mainTexture) return;

            Material material = GetWritableMaterial(renderer);
            string materialName = material.name.Replace(" (Instance)", string.Empty);
            bool isWallTexture = materialName == "DK3D Damp Moss Wall" || materialName == "DK3D Damp Vine Wall";

            Vector2 tiling = isWallTexture
                ? new Vector2(
                    Mathf.Max(1f, Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.z)) / 2.6f),
                    Mathf.Max(1f, Mathf.Abs(scale.y) / 5.2f)
                )
                : new Vector2(
                    Mathf.Max(1f, Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.z)) * 0.45f),
                    Mathf.Max(1f, Mathf.Abs(scale.y) * 0.65f)
                );

            if (!isWallTexture && Mathf.Abs(scale.y) <= Mathf.Abs(scale.x) * 0.2f && Mathf.Abs(scale.y) <= Mathf.Abs(scale.z) * 0.2f)
            {
                tiling = new Vector2(Mathf.Max(1f, Mathf.Abs(scale.x) * 0.45f), Mathf.Max(1f, Mathf.Abs(scale.z) * 0.45f));
            }

            material.mainTextureScale = tiling;
        }

        private static Material GetWritableMaterial(Renderer renderer)
        {
            if (Application.isPlaying) return renderer.material;

            Material source = renderer.sharedMaterial;
            Material material = new Material(source)
            {
                name = source.name
            };
            renderer.sharedMaterial = material;
            return material;
        }

        private void SetGeneratedParent(GameObject target)
        {
            if (!generatedRoot || !target || target.transform == generatedRoot) return;
            target.transform.SetParent(generatedRoot, true);
        }

        private Transform CreateGroup(string name, Vector3 position)
        {
            GameObject group = new GameObject(name);
            SetGeneratedParent(group);
            MarkSelectionRoot(group);
            group.transform.position = position;
            return group.transform;
        }

        private void MarkSelectionRoot(GameObject target)
        {
            if (!target || target.GetComponent<DungeonKnight3DSelectionRoot>()) return;
            target.AddComponent<DungeonKnight3DSelectionRoot>();
        }

        private void ConfigureEditorPreviewLight(Light light)
        {
            if (!quietEditorPreview || !light) return;
            light.gameObject.AddComponent<DungeonKnight3DEditorPreviewLight>().Configure(light);
        }

        private static void DestroySafely(Object target)
        {
            if (!target) return;

            if (Application.isPlaying)
            {
                Object.Destroy(target);
            }
            else
            {
                Object.DestroyImmediate(target);
            }
        }
    }
}
