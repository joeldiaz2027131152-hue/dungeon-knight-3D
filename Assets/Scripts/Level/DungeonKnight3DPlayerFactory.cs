using DungeonKnight.Player;
using DungeonKnight.UI;
using UnityEngine;

namespace DungeonKnight.Level
{
    internal sealed class DungeonKnight3DPlayerFactory
    {
        private readonly DungeonKnight3DAssets assets;

        public DungeonKnight3DPlayerFactory(DungeonKnight3DAssets assets)
        {
            this.assets = assets;
        }

        public PlayerController3D CreatePlayer()
        {
            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Knight 3D";
            player.transform.position = DungeonKnight3DBootstrap.PlayerSpawn;
            player.transform.localScale = new Vector3(0.82f, 1.05f, 0.82f);
            Object.Destroy(player.GetComponent<CapsuleCollider>());
            player.GetComponent<Renderer>().material = assets.PlayerBody;

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

        public void CreateCamera(Transform target)
        {
            Camera camera = Camera.main;
            if (!camera)
            {
                GameObject cameraObject = new GameObject("Main Camera");
                camera = cameraObject.AddComponent<Camera>();
                cameraObject.tag = "MainCamera";
            }

            camera.transform.position = DungeonKnight3DBootstrap.PlayerSpawn + CameraFollow3D.DefaultOffset;
            camera.transform.rotation = Quaternion.Euler(48f, 0f, 0f);
            camera.fieldOfView = 58f;
            camera.nearClipPlane = 0.08f;
            CameraFollow3D follow = camera.gameObject.AddComponent<CameraFollow3D>();
            follow.SetTarget(target);
        }

        public void CreateHud(PlayerController3D player)
        {
            new GameObject("Game HUD 3D").AddComponent<GameHud3D>().Bind(player);
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
            if (!assets.PlayerIdleSprite) return false;

            Renderer capsuleRenderer = playerTransform.GetComponent<Renderer>();
            if (capsuleRenderer) capsuleRenderer.enabled = false;

            GameObject visual = new GameObject("Dark Knight Sprite Visual");
            visual.transform.SetParent(playerTransform, false);
            visual.transform.localPosition = new Vector3(0f, 0.1f, 0f);

            SpriteRenderer spriteRenderer = visual.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = assets.PlayerIdleSprite;
            spriteRenderer.sortingOrder = 4;
            spriteRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            spriteRenderer.receiveShadows = false;
            visual.AddComponent<PlayerVisual3D>().Configure(
                controller,
                assets.PlayerIdleSprite,
                assets.PlayerWalkFrontSprites,
                assets.PlayerWalkLeftSprites,
                assets.PlayerWalkRightSprites,
                assets.PlayerWalkBackSprites,
                assets.PlayerRollFrontSprites,
                assets.PlayerRollLeftSprites,
                assets.PlayerRollRightSprites,
                assets.PlayerRollBackSprites,
                assets.PlayerAttackFrontSprites,
                assets.PlayerAttackBackSprites,
                assets.PlayerAttackLeftSprites,
                assets.PlayerAttackRightSprites,
                assets.PlayerBlockSprites,
                assets.PlayerBlockWalkFrontSprites,
                assets.PlayerBlockWalkLeftSprites,
                assets.PlayerBlockWalkRightSprites,
                assets.PlayerBlockWalkBackSprites);

            return true;
        }

        private void CreateWeaponVisual(Transform parent)
        {
            GameObject sword = CreateBox("Knight Sword", new Vector3(0.48f, 1.12f, 0.42f), new Vector3(0.12f, 1.1f, 0.12f), assets.Brass);
            sword.transform.SetParent(parent, false);
            sword.transform.localRotation = Quaternion.Euler(24f, 0f, -22f);
        }

        private void CreateShieldVisual(Transform parent)
        {
            GameObject shield = CreateBox("Knight Shield", new Vector3(-0.48f, 1.08f, 0.34f), new Vector3(0.15f, 0.85f, 0.65f), assets.DarkStone);
            shield.transform.SetParent(parent, false);
        }

        private static GameObject CreateBox(string name, Vector3 position, Vector3 scale, Material material)
        {
            return DungeonKnight3DGeometryBuilder.CreateBox(name, position, scale, material);
        }
    }
}
