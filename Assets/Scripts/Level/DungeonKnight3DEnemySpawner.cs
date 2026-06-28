using DungeonKnight.Player;
using UnityEngine;

namespace DungeonKnight.Level
{
    internal sealed class DungeonKnight3DEnemySpawner
    {
        private readonly DungeonKnight3DAssets assets;

        public DungeonKnight3DEnemySpawner(DungeonKnight3DAssets assets)
        {
            this.assets = assets;
        }

        public void CreateWorldOneOneEnemies(PlayerController3D player)
        {
            CreateEnemy("Skeleton Guard", new Vector3(0f, 1.05f, -2.6f), player, 45, 9, 2.35f, false);
            CreateEnemy("Skeleton Archer Stand-in", new Vector3(-3.2f, 2.68f, 8f), player, 38, 8, 2.1f, false);
            CreateEnemy("Key Guardian 3D", new Vector3(0f, 1.05f, 16.5f), player, 110, 18, 2.6f, true);
        }

        public GameObject CreateEnemy(string name, Vector3 position, PlayerController3D player, int hp, int damage, float speed, bool dropsKey)
        {
            GameObject enemyObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            enemyObject.name = name;
            enemyObject.transform.position = position;
            enemyObject.GetComponent<Renderer>().sharedMaterial = assets.Enemy;
            CapsuleCollider capsuleCollider = enemyObject.GetComponent<CapsuleCollider>();
            if (Application.isPlaying) Object.Destroy(capsuleCollider);
            else Object.DestroyImmediate(capsuleCollider);

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

            enemyObject.AddComponent<DungeonEnemy3D>().Configure(player, hp, damage, speed, dropsKey);
            return enemyObject;
        }

        private void AttachSkeletonSprite(Transform enemyTransform)
        {
            if (!assets.SkeletonEnemySprite) return;

            Renderer capsuleRenderer = enemyTransform.GetComponent<Renderer>();
            if (capsuleRenderer) capsuleRenderer.enabled = false;

            GameObject visual = new GameObject("Skeleton Sprite Visual");
            visual.transform.SetParent(enemyTransform, false);
            visual.transform.localPosition = new Vector3(0f, 0.05f, 0f);
            visual.transform.localScale = Vector3.one * 1.3f;

            SpriteRenderer spriteRenderer = visual.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = assets.SkeletonEnemySprite;
            spriteRenderer.sortingOrder = 2;
            spriteRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            spriteRenderer.receiveShadows = false;
            visual.AddComponent<SkeletonEnemyVisual3D>().Configure(assets.SkeletonEnemySprite, assets.SkeletonAttackSprites, assets.SkeletonWalkFrontSprites, assets.SkeletonWalkLeftSprites, assets.SkeletonWalkRightSprites);
        }
    }
}
