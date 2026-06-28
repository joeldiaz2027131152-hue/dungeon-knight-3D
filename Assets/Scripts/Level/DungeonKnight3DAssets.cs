using UnityEngine;

namespace DungeonKnight.Level
{
    internal sealed class DungeonKnight3DAssets
    {
        public Material Stone { get; private set; }
        public Material DarkStone { get; private set; }
        public Material Brass { get; private set; }
        public Material ChestShell { get; private set; }
        public Material ChestIron { get; private set; }
        public Material ChestTrim { get; private set; }
        public Material ChestGlow { get; private set; }
        public Material Ember { get; private set; }
        public Material Enemy { get; private set; }
        public Material PlayerBody { get; private set; }
        public Material Potion { get; private set; }
        public Material ExteriorStone { get; private set; }
        public Material ShadowStone { get; private set; }
        public Material Hazard { get; private set; }
        public Sprite SkeletonEnemySprite { get; private set; }
        public Sprite[] SkeletonAttackSprites { get; private set; }
        public Sprite[] SkeletonWalkFrontSprites { get; private set; }
        public Sprite[] SkeletonWalkLeftSprites { get; private set; }
        public Sprite[] SkeletonWalkRightSprites { get; private set; }
        public Sprite PlayerIdleSprite { get; private set; }
        public Sprite[] PlayerWalkFrontSprites { get; private set; }
        public Sprite[] PlayerWalkLeftSprites { get; private set; }
        public Sprite[] PlayerWalkRightSprites { get; private set; }
        public Sprite[] PlayerWalkBackSprites { get; private set; }
        public Sprite[] PlayerRollFrontSprites { get; private set; }
        public Sprite[] PlayerRollLeftSprites { get; private set; }
        public Sprite[] PlayerRollRightSprites { get; private set; }
        public Sprite[] PlayerRollBackSprites { get; private set; }
        public Sprite[] PlayerAttackFrontSprites { get; private set; }
        public Sprite[] PlayerAttackBackSprites { get; private set; }
        public Sprite[] PlayerAttackLeftSprites { get; private set; }
        public Sprite[] PlayerAttackRightSprites { get; private set; }
        public Sprite[] PlayerBlockSprites { get; private set; }
        public Sprite[] PlayerBlockWalkFrontSprites { get; private set; }
        public Sprite[] PlayerBlockWalkLeftSprites { get; private set; }
        public Sprite[] PlayerBlockWalkRightSprites { get; private set; }
        public Sprite[] PlayerBlockWalkBackSprites { get; private set; }

        public static DungeonKnight3DAssets Load()
        {
            DungeonKnight3DAssets assets = new DungeonKnight3DAssets();
            Texture2D dungeonStoneTexture = Resources.Load<Texture2D>("Art/Textures/Dungeon/dark_moss_stone_tiles");
            assets.Stone = NewTexturedMaterial("DK3D Moss Stone", new Color(0.72f, 0.74f, 0.72f), dungeonStoneTexture);
            assets.DarkStone = NewTexturedMaterial("DK3D Dark Moss Stone", new Color(0.42f, 0.46f, 0.46f), dungeonStoneTexture);
            assets.Brass = NewMaterial("DK3D Old Brass", new Color(0.86f, 0.62f, 0.28f));
            assets.ChestShell = NewMaterial("DK3D Blue Iron Chest Shell", new Color(0.16f, 0.24f, 0.29f));
            assets.ChestIron = NewMaterial("DK3D Dark Riveted Chest Iron", new Color(0.045f, 0.06f, 0.075f));
            assets.ChestTrim = NewMaterial("DK3D Worn Chest Edge Metal", new Color(0.42f, 0.5f, 0.55f));
            assets.ChestGlow = NewMaterial("DK3D Chest Soul Glow", new Color(0.74f, 0.94f, 1f));
            assets.Ember = NewMaterial("DK3D Ember", new Color(1f, 0.26f, 0.08f));
            assets.Enemy = NewMaterial("DK3D Bone Enemy", new Color(0.82f, 0.78f, 0.66f));
            assets.PlayerBody = NewMaterial("DK3D Knight Steel", new Color(0.52f, 0.58f, 0.68f));
            assets.Potion = NewMaterial("DK3D Potion", new Color(0.9f, 0.08f, 0.22f));
            assets.ExteriorStone = NewMaterial("DK3D Exterior Stone", new Color(0.34f, 0.36f, 0.39f));
            assets.ShadowStone = NewMaterial("DK3D Shadow Stone", new Color(0.07f, 0.075f, 0.09f));
            assets.Hazard = NewMaterial("DK3D Hazard Iron", new Color(0.68f, 0.18f, 0.08f));

            const string skeletonPath = "Characters/Enemies/SkeletonEnemy/Normalized/";
            assets.SkeletonEnemySprite = LoadSpriteAsset(skeletonPath + "skeleton_enemy_idle_normalized");
            assets.SkeletonAttackSprites = new[]
            {
                LoadSpriteAsset(skeletonPath + "skeleton_enemy_attack_front_slash_normalized"),
                LoadSpriteAsset(skeletonPath + "skeleton_enemy_attack_back_slash_normalized"),
                LoadSpriteAsset(skeletonPath + "skeleton_enemy_attack_right_slash_normalized"),
                LoadSpriteAsset(skeletonPath + "skeleton_enemy_attack_left_slash_normalized"),
                LoadSpriteAsset(skeletonPath + "skeleton_enemy_attack_front_left_slash_normalized"),
                LoadSpriteAsset(skeletonPath + "skeleton_enemy_attack_front_right_slash_normalized")
            };
            assets.SkeletonWalkFrontSprites = LoadSpriteSequence(skeletonPath + "skeleton_enemy_walk_front_", 4, "_normalized");
            assets.SkeletonWalkLeftSprites = LoadSpriteSequence(skeletonPath + "skeleton_enemy_walk_left_", 4, "_normalized");
            assets.SkeletonWalkRightSprites = LoadSpriteSequence(skeletonPath + "skeleton_enemy_walk_right_", 4, "_normalized");

            const string playerPath = "Characters/Player/DarkKnight/Normalized/";
            assets.PlayerIdleSprite = LoadSpriteAsset(playerPath + "dark_knight_idle_base_normalized");
            assets.PlayerWalkFrontSprites = LoadSpriteSequence(playerPath + "dark_knight_walk_front_", 4, "_normalized");
            assets.PlayerWalkLeftSprites = LoadSpriteSequence(playerPath + "dark_knight_walk_left_", 4, "_normalized");
            assets.PlayerWalkRightSprites = LoadSpriteSequence(playerPath + "dark_knight_walk_right_", 4, "_normalized");
            assets.PlayerWalkBackSprites = LoadSpriteSequence(playerPath + "dark_knight_walk_back_", 4, "_normalized");
            assets.PlayerRollFrontSprites = LoadSpriteSequence(playerPath + "dark_knight_roll_front_", 4, "_normalized");
            assets.PlayerRollLeftSprites = LoadSpriteSequence(playerPath + "dark_knight_roll_left_", 4, "_normalized");
            assets.PlayerRollRightSprites = LoadSpriteSequence(playerPath + "dark_knight_roll_right_", 4, "_normalized");
            assets.PlayerRollBackSprites = LoadSpriteSequence(playerPath + "dark_knight_roll_back_", 4, "_normalized");
            assets.PlayerBlockWalkFrontSprites = LoadSpriteSequence(playerPath + "dark_knight_blockwalk_front_", 4, "_normalized");
            assets.PlayerBlockWalkLeftSprites = LoadSpriteSequence(playerPath + "dark_knight_blockwalk_left_", 4, "_normalized");
            assets.PlayerBlockWalkRightSprites = LoadSpriteSequence(playerPath + "dark_knight_blockwalk_right_", 4, "_normalized");
            assets.PlayerBlockWalkBackSprites = LoadSpriteSequence(playerPath + "dark_knight_blockwalk_back_", 4, "_normalized");
            assets.PlayerAttackFrontSprites = LoadSpriteSequence(playerPath + "dark_knight_attackanim_front_", 4, "_normalized");
            assets.PlayerAttackBackSprites = LoadSpriteSequence(playerPath + "dark_knight_attackanim_back_", 4, "_normalized");
            assets.PlayerAttackLeftSprites = LoadSpriteSequence(playerPath + "dark_knight_attackanim_left_", 4, "_normalized");
            assets.PlayerAttackRightSprites = LoadSpriteSequence(playerPath + "dark_knight_attackanim_right_", 4, "_normalized");
            assets.PlayerBlockSprites = new[]
            {
                LoadSpriteAsset(playerPath + "dark_knight_block_front_block_normalized"),
                LoadSpriteAsset(playerPath + "dark_knight_block_back_block_normalized"),
                LoadSpriteAsset(playerPath + "dark_knight_block_right_block_normalized"),
                LoadSpriteAsset(playerPath + "dark_knight_block_left_block_normalized"),
                LoadSpriteAsset(playerPath + "dark_knight_block_front_left_block_normalized"),
                LoadSpriteAsset(playerPath + "dark_knight_block_front_right_block_normalized")
            };

            return assets;
        }

        public static Material NewMaterial(string name, Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (!shader) shader = Shader.Find("Standard");
            if (!shader) shader = Shader.Find("Diffuse");
            Material material = new Material(shader);
            material.name = name;
            material.color = color;
            return material;
        }

        public static void ApplyBoxTextureTiling(Renderer renderer, Vector3 scale)
        {
            if (!renderer || !renderer.sharedMaterial || !renderer.sharedMaterial.mainTexture) return;

            Material material = GetWritableMaterial(renderer);
            Vector2 tiling = new Vector2(
                Mathf.Max(1f, Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.z)) * 0.45f),
                Mathf.Max(1f, Mathf.Abs(scale.y) * 0.65f)
            );

            if (Mathf.Abs(scale.y) <= Mathf.Abs(scale.x) * 0.2f && Mathf.Abs(scale.y) <= Mathf.Abs(scale.z) * 0.2f)
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
    }
}
