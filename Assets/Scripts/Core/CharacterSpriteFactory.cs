using System;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonKnight
{
    public static class CharacterSpriteFactory
    {
        private const int Width = 96;
        private const int Height = 128;
        private const int PixelsPerUnit = 100;
        private const string KnightReferenceResource = "Characters/dungeon_knight_hatch_base";
        private const string KnightIdleResourceA = "Characters/DungeonKnight/idle_0";
        private const string KnightIdleResourceB = "Characters/DungeonKnight/idle_0";
        private const string KnightActionResourceRoot = "Characters/DungeonKnight/Actions/";
        private static readonly Dictionary<string, Sprite> Cache = new();

        public static Sprite KnightIdleA() => KnightResource("KnightIdleA", KnightIdleResourceA) ?? KnightReference("KnightIdleA") ?? Knight("KnightIdleA", 0, false, false, false, false, false);
        public static Sprite KnightIdleB() => KnightResource("KnightIdleB", KnightIdleResourceB) ?? KnightReference("KnightIdleB") ?? Knight("KnightIdleB", 1, false, false, false, false, false);
        public static Sprite KnightRunA() => KnightReference("KnightRunA") ?? Knight("KnightRunA", 0, true, false, false, false, false);
        public static Sprite KnightRunB() => KnightReference("KnightRunB") ?? Knight("KnightRunB", 1, true, false, false, false, false);
        public static Sprite KnightRunC() => KnightReference("KnightRunC") ?? Knight("KnightRunC", 2, true, false, false, false, false);
        public static Sprite KnightRunD() => KnightReference("KnightRunD") ?? Knight("KnightRunD", 3, true, false, false, false, false);
        public static Sprite KnightJump() => KnightReference("KnightJump") ?? Knight("KnightJump", 0, false, true, false, false, false);
        public static Sprite KnightAttack() => KnightAction("KnightAttack", "attack", 0) ?? KnightReference("KnightAttack") ?? Knight("KnightAttack", 0, false, false, true, false, false);
        public static Sprite KnightAttackFrame(int frame) => KnightAttack();
        public static Sprite KnightBlock() => KnightAction("KnightBlock", "block", 0) ?? KnightReference("KnightBlock") ?? Knight("KnightBlock", 0, false, false, false, true, false);
        public static Sprite KnightBlockFrame(int frame) => KnightBlock();
        public static Sprite KnightCrouchFrame(int frame) => KnightAction("KnightCrouch" + frame, "crouch", frame) ?? KnightReference("KnightCrouch") ?? Knight("KnightCrouch", frame, false, false, false, false, false);
        public static Sprite KnightCharge() => KnightReference("KnightCharge") ?? Knight("KnightCharge", 0, false, false, false, false, true);
        public static Sprite KnightRollA() => KnightAction("KnightRollA", "roll", 0) ?? KnightReference("KnightRollA") ?? KnightRoll("KnightRollA", 0);
        public static Sprite KnightRollB() => KnightAction("KnightRollB", "roll", 2) ?? KnightReference("KnightRollB") ?? KnightRoll("KnightRollB", 1);
        public static Sprite KnightRollFrame(int frame) => KnightAction("KnightRoll" + frame, "roll", frame) ?? (frame % 2 == 0 ? KnightRollA() : KnightRollB());

        public static Sprite GuardIdleA() => Guard("GuardIdleA", 0, false, false);
        public static Sprite GuardIdleB() => Guard("GuardIdleB", 1, false, false);
        public static Sprite GuardRunA() => Guard("GuardRunA", 0, true, false);
        public static Sprite GuardRunB() => Guard("GuardRunB", 1, true, false);
        public static Sprite GuardRunC() => Guard("GuardRunC", 2, true, false);
        public static Sprite GuardRunD() => Guard("GuardRunD", 3, true, false);
        public static Sprite GuardAttack() => Guard("GuardAttack", 0, false, true);
        public static Sprite ArcherIdleA() => Archer("ArcherIdleA", 0);
        public static Sprite ArcherIdleB() => Archer("ArcherIdleB", 1);
        public static Sprite ArcherAim() => Archer("ArcherAim", 2);
        public static Sprite ArcherRelease() => Archer("ArcherRelease", 3);

        private static Sprite KnightReference(string name)
        {
            return KnightResource(name, KnightReferenceResource);
        }

        private static Sprite KnightAction(string name, string action, int frame)
        {
            return KnightResource(name, KnightActionResourceRoot + action + "_" + Mathf.Abs(frame % 4));
        }

        private static Sprite KnightResource(string name, string resourcePath)
        {
            if (Cache.TryGetValue(name, out Sprite cached)) return cached;

            Texture2D texture = Resources.Load<Texture2D>(resourcePath);
            if (!texture) return null;

            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), PixelsPerUnit);
            sprite.name = name;
            Cache[name] = sprite;
            return sprite;
        }

        private static Sprite Knight(string name, int frame, bool running, bool airborne, bool attacking, bool blocking, bool charging)
        {
            return Create(name, tex =>
            {
                Color outline = Hex("080a10");
                Color armorDark = Hex("121821");
                Color armor = Hex("2c343d");
                Color armorMid = Hex("46515d");
                Color armorLight = Hex("87929f");
                Color gold = Hex("c8983f");
                Color goldBright = Hex("f2d579");
                Color capeDark = Hex("0f1f31");
                Color cape = Hex("1e3e5f");
                Color capeLight = Hex("3c668f");
                Color skin = Hex("e7ad83");
                Color leather = Hex("211819");
                Color steel = Hex("d1dae2");
                Color steelDark = Hex("68747f");
                Color shadow = Hex("05070b");

                int runFrame = frame % 4;
                float bob = running ? (runFrame is 0 or 2 ? 2f : -1f) : frame == 1 ? 1f : 0f;
                float armSwing = running ? runFrame switch { 0 => 8f, 1 => 3f, 2 => -8f, _ => -3f } : 0f;
                float legSwing = running ? runFrame switch { 0 => 9f, 1 => 4f, 2 => -9f, _ => -4f } : 0f;

                if (airborne)
                {
                    legSwing = -4f;
                    armSwing = 4f;
                    bob = 1f;
                }

                float capeWave = running ? runFrame switch { 0 => 4f, 1 => 1f, 2 => -3f, _ => 0f } : charging ? -3f : frame == 1 ? 1.5f : 0f;

                DrawEllipse(tex, new Vector2(48, 14), 35, 6, new Color(0f, 0f, 0f, 0.38f));
                DrawPolygon(tex, outline, new Vector2(39, 91 + bob), new Vector2(24, 84 + bob), new Vector2(8, 58 + capeWave), new Vector2(3, 35 + capeWave), new Vector2(16, 45 + capeWave), new Vector2(20, 26), new Vector2(31, 41), new Vector2(41, 28), new Vector2(50, 86 + bob));
                DrawPolygon(tex, capeDark, new Vector2(40, 90 + bob), new Vector2(25, 82 + bob), new Vector2(10, 59 + capeWave), new Vector2(6, 39 + capeWave), new Vector2(19, 48 + capeWave), new Vector2(22, 30), new Vector2(32, 44), new Vector2(40, 32), new Vector2(50, 84 + bob));
                DrawPolygon(tex, outline, new Vector2(50, 88 + bob), new Vector2(66, 78 + bob), new Vector2(93, 56 + capeWave), new Vector2(84, 42 + capeWave), new Vector2(74, 49 + capeWave), new Vector2(76, 28), new Vector2(61, 43), new Vector2(54, 29), new Vector2(43, 85 + bob));
                DrawPolygon(tex, cape, new Vector2(50, 86 + bob), new Vector2(67, 76 + bob), new Vector2(90, 56 + capeWave), new Vector2(82, 45 + capeWave), new Vector2(72, 52 + capeWave), new Vector2(73, 32), new Vector2(61, 46), new Vector2(54, 33), new Vector2(44, 83 + bob));
                DrawCapsule(tex, new Vector2(19, 57 + capeWave), new Vector2(36, 76 + bob), 2, capeLight);
                DrawCapsule(tex, new Vector2(62, 59 + capeWave), new Vector2(49, 81 + bob), 2, capeLight);
                DrawCapsule(tex, new Vector2(74, 49 + capeWave), new Vector2(89, 56 + capeWave), 1.4f, outline);
                DrawCapsule(tex, new Vector2(18, 47 + capeWave), new Vector2(6, 38 + capeWave), 1.4f, outline);
                DrawCapsule(tex, new Vector2(71, 40 + capeWave), new Vector2(78, 32 + capeWave), 1.3f, shadow);
                DrawCapsule(tex, new Vector2(26, 36 + capeWave), new Vector2(33, 47 + capeWave), 1.3f, shadow);

                Vector2 leftKnee = new Vector2(31 - legSwing * 0.25f, 43 + bob);
                Vector2 leftFoot = new Vector2(27 - legSwing * 0.35f, 19);
                Vector2 rightKnee = new Vector2(57 + legSwing * 0.25f, 43 + bob);
                Vector2 rightFoot = new Vector2(62 + legSwing * 0.35f, 19);
                DrawCapsule(tex, new Vector2(38, 60 + bob), leftKnee, 8, outline);
                DrawCapsule(tex, new Vector2(38, 60 + bob), leftKnee, 5.5f, armorDark);
                DrawCapsule(tex, leftKnee, leftFoot, 7, outline);
                DrawCapsule(tex, leftKnee, leftFoot, 4.5f, armor);
                DrawCapsule(tex, new Vector2(56, 60 + bob), rightKnee, 8, outline);
                DrawCapsule(tex, new Vector2(56, 60 + bob), rightKnee, 5.5f, armorDark);
                DrawCapsule(tex, rightKnee, rightFoot, 7, outline);
                DrawCapsule(tex, rightKnee, rightFoot, 4.5f, armor);
                DrawEllipse(tex, leftFoot + Vector2.left * 2, 10, 5, outline);
                DrawEllipse(tex, leftFoot + Vector2.left * 1, 7, 3, armorMid);
                DrawEllipse(tex, rightFoot + Vector2.right * 2, 10, 5, outline);
                DrawEllipse(tex, rightFoot + Vector2.right * 1, 7, 3, armorMid);
                DrawEllipse(tex, leftKnee + new Vector2(0, 1), 7, 6, outline);
                DrawEllipse(tex, leftKnee + new Vector2(0, 1), 5, 4, armorMid);
                DrawEllipse(tex, rightKnee + new Vector2(0, 1), 7, 6, outline);
                DrawEllipse(tex, rightKnee + new Vector2(0, 1), 5, 4, armorMid);
                DrawCapsule(tex, leftKnee + new Vector2(-4, 3), leftKnee + new Vector2(5, 5), 2, gold);
                DrawCapsule(tex, rightKnee + new Vector2(-5, 3), rightKnee + new Vector2(4, 5), 2, gold);
                DrawCapsule(tex, leftFoot + new Vector2(-4, 2), leftFoot + new Vector2(5, 2), 1.5f, gold);
                DrawCapsule(tex, rightFoot + new Vector2(-5, 2), rightFoot + new Vector2(4, 2), 1.5f, gold);

                DrawCapsule(tex, new Vector2(47, 54 + bob), new Vector2(48, 88 + bob), 21, outline);
                DrawCapsule(tex, new Vector2(47, 56 + bob), new Vector2(48, 86 + bob), 17, armorDark);
                DrawPolygon(tex, armor, new Vector2(35, 83 + bob), new Vector2(60, 86 + bob), new Vector2(66, 66 + bob), new Vector2(50, 54 + bob), new Vector2(32, 62 + bob));
                DrawPolygon(tex, armorMid, new Vector2(40, 81 + bob), new Vector2(56, 83 + bob), new Vector2(60, 67 + bob), new Vector2(49, 58 + bob), new Vector2(38, 64 + bob));
                DrawCapsule(tex, new Vector2(38, 84 + bob), new Vector2(61, 83 + bob), 3, gold);
                DrawCapsule(tex, new Vector2(38, 61 + bob), new Vector2(59, 63 + bob), 3, gold);
                DrawCapsule(tex, new Vector2(48, 57 + bob), new Vector2(51, 82 + bob), 2, goldBright);
                DrawCapsule(tex, new Vector2(40, 78 + bob), new Vector2(55, 64 + bob), 1.5f, gold);
                DrawCapsule(tex, new Vector2(58, 79 + bob), new Vector2(42, 64 + bob), 1.2f, gold);
                DrawCapsule(tex, new Vector2(34, 70 + bob), new Vector2(61, 72 + bob), 1.2f, goldBright);
                DrawEllipse(tex, new Vector2(34, 85 + bob), 13, 10, outline);
                DrawEllipse(tex, new Vector2(34, 85 + bob), 9, 7, armor);
                DrawCapsule(tex, new Vector2(27, 86 + bob), new Vector2(39, 91 + bob), 2, gold);
                DrawEllipse(tex, new Vector2(64, 84 + bob), 13, 10, outline);
                DrawEllipse(tex, new Vector2(64, 84 + bob), 9, 7, armor);
                DrawCapsule(tex, new Vector2(58, 91 + bob), new Vector2(70, 86 + bob), 2, gold);
                if (blocking)
                {
                    DrawCapsule(tex, new Vector2(31, 86 + bob), new Vector2(46, 84 + bob), 7, outline);
                    DrawCapsule(tex, new Vector2(31, 86 + bob), new Vector2(46, 84 + bob), 5, armorDark);
                    DrawCapsule(tex, new Vector2(46, 84 + bob), new Vector2(72, 75 + bob), 6, outline);
                    DrawCapsule(tex, new Vector2(46, 84 + bob), new Vector2(72, 75 + bob), 4, armorMid);
                    DrawEllipse(tex, new Vector2(75, 74 + bob), 5, 5, outline);
                    DrawEllipse(tex, new Vector2(75, 74 + bob), 3, 3, leather);
                    DrawCapsule(tex, new Vector2(66, 76 + bob), new Vector2(78, 72 + bob), 2, gold);
                }
                else
                {
                    DrawCapsule(tex, new Vector2(31, 86 + bob), new Vector2(28 - armSwing * 0.28f, 60 + bob), 6, outline);
                    DrawCapsule(tex, new Vector2(31, 86 + bob), new Vector2(28 - armSwing * 0.28f, 60 + bob), 4, armorDark);
                    DrawCapsule(tex, new Vector2(27 - armSwing * 0.28f, 61 + bob), new Vector2(31 - armSwing * 0.2f, 50 + bob), 5, outline);
                    DrawCapsule(tex, new Vector2(27 - armSwing * 0.28f, 61 + bob), new Vector2(31 - armSwing * 0.2f, 50 + bob), 3, armorMid);
                    DrawCapsule(tex, new Vector2(29 - armSwing * 0.25f, 55 + bob), new Vector2(33 - armSwing * 0.18f, 52 + bob), 1.3f, gold);
                }

                Vector2 rightShoulder = new Vector2(63, 83 + bob);
                Vector2 hand = charging ? new Vector2(54, 92 + bob) : attacking ? new Vector2(84, 77 + bob) : new Vector2(69 + armSwing * 0.22f, 55 + bob);
                DrawCapsule(tex, rightShoulder, hand, 7, outline);
                DrawCapsule(tex, rightShoulder, hand, 5, armorDark);
                DrawEllipse(tex, hand, 5, 5, outline);
                DrawEllipse(tex, hand, 3, 3, leather);

                Vector2 hilt = charging ? new Vector2(51, 91 + bob) : attacking ? new Vector2(78, 76 + bob) : new Vector2(65 + armSwing * 0.22f, 54 + bob);
                Vector2 bladeTip = charging ? new Vector2(18, 116 + bob) : attacking ? new Vector2(95, 79 + bob) : new Vector2(95, 22 + bob * 0.2f);
                if (charging)
                {
                    DrawCapsule(tex, hilt + new Vector2(-1, 1), bladeTip + new Vector2(-2, 2), 7, new Color(0.32f, 0.82f, 1f, 0.28f));
                    DrawEllipse(tex, bladeTip, 8, 8, new Color(0.55f, 0.92f, 1f, 0.24f));
                }
                DrawCapsule(tex, hilt, bladeTip, 6, outline);
                DrawCapsule(tex, hilt, bladeTip, 4, steelDark);
                DrawCapsule(tex, hilt + new Vector2(2, 1), bladeTip - new Vector2(3, 2), 1.8f, steel);
                DrawCapsule(tex, hilt + new Vector2(-7, -4), hilt + new Vector2(8, 4), 3, outline);
                DrawCapsule(tex, hilt + new Vector2(-6, -3), hilt + new Vector2(7, 3), 2, gold);
                DrawEllipse(tex, hilt + new Vector2(-2, 1), 3, 3, goldBright);

                DrawEllipse(tex, new Vector2(48, 101 + bob), 16, 18, outline);
                DrawEllipse(tex, new Vector2(54, 100 + bob), 7, 13, skin);
                DrawPolygon(tex, armorDark, new Vector2(33, 106 + bob), new Vector2(42, 121 + bob), new Vector2(58, 120 + bob), new Vector2(68, 105 + bob), new Vector2(58, 91 + bob), new Vector2(40, 91 + bob));
                DrawPolygon(tex, armor, new Vector2(39, 112 + bob), new Vector2(48, 119 + bob), new Vector2(60, 114 + bob), new Vector2(57, 104 + bob), new Vector2(44, 101 + bob));
                DrawPolygon(tex, gold, new Vector2(39, 111 + bob), new Vector2(47, 118 + bob), new Vector2(59, 114 + bob), new Vector2(56, 106 + bob), new Vector2(44, 102 + bob));
                DrawPolygon(tex, shadow, new Vector2(42, 104 + bob), new Vector2(54, 109 + bob), new Vector2(59, 100 + bob), new Vector2(45, 96 + bob));
                DrawCapsule(tex, new Vector2(42, 109 + bob), new Vector2(58, 114 + bob), 1.2f, goldBright);
                DrawEllipse(tex, new Vector2(56, 102 + bob), 2.4f, 2.2f, goldBright);
                DrawCapsule(tex, new Vector2(39, 116 + bob), new Vector2(30, 125 + bob), 5, outline);
                DrawCapsule(tex, new Vector2(39, 116 + bob), new Vector2(31, 125 + bob), 3, armorLight);
                DrawCapsule(tex, new Vector2(57, 116 + bob), new Vector2(67, 125 + bob), 5, outline);
                DrawCapsule(tex, new Vector2(57, 116 + bob), new Vector2(66, 125 + bob), 3, armorLight);
                DrawCapsule(tex, new Vector2(40, 119 + bob), new Vector2(43, 127 + bob), 3, outline);
                DrawCapsule(tex, new Vector2(56, 119 + bob), new Vector2(55, 127 + bob), 3, outline);
                DrawCapsule(tex, new Vector2(40, 119 + bob), new Vector2(43, 127 + bob), 1.5f, armorLight);
                DrawCapsule(tex, new Vector2(56, 119 + bob), new Vector2(55, 127 + bob), 1.5f, armorLight);
            });
        }

        private static Sprite Guard(string name, int frame, bool running, bool attacking)
        {
            return Create(name, tex =>
            {
                Color outline = Hex("17131f");
                Color bone = Hex("e7dcc0");
                Color boneDark = Hex("a89b7e");
                Color armor = Hex("7b746d");
                Color armorDark = Hex("3c3637");
                Color rust = Hex("8b4a2f");
                Color cloth = Hex("2b272c");
                Color leather = Hex("4b3428");
                Color weapon = Hex("c0c5c6");
                Color weaponDark = Hex("687071");
                Color plume = Hex("6f665a");

                int runFrame = frame % 4;
                float bob = running ? (runFrame is 0 or 2 ? 1.5f : -1f) : frame == 1 ? 0.8f : 0f;
                float step = running ? runFrame switch { 0 => 9f, 1 => 3f, 2 => -9f, _ => -3f } : 0f;

                Vector2 leftKnee = new Vector2(32 - step * 0.32f, 44 + bob);
                Vector2 leftFoot = new Vector2(24 - step * 0.35f, 18);
                Vector2 rightKnee = new Vector2(58 + step * 0.32f, 44 + bob);
                Vector2 rightFoot = new Vector2(68 + step * 0.35f, 18);
                DrawCapsule(tex, new Vector2(39, 58 + bob), leftKnee, 5, outline);
                DrawCapsule(tex, new Vector2(39, 58 + bob), leftKnee, 3, bone);
                DrawCapsule(tex, leftKnee, leftFoot, 6, outline);
                DrawCapsule(tex, leftKnee, leftFoot, 4, bone);
                DrawCapsule(tex, new Vector2(55, 58 + bob), rightKnee, 5, outline);
                DrawCapsule(tex, new Vector2(55, 58 + bob), rightKnee, 3, bone);
                DrawCapsule(tex, rightKnee, rightFoot, 6, outline);
                DrawCapsule(tex, rightKnee, rightFoot, 4, bone);
                DrawEllipse(tex, leftKnee, 7, 5, armorDark);
                DrawEllipse(tex, rightKnee, 7, 5, armorDark);
                DrawCapsule(tex, leftKnee + new Vector2(-2, -4), leftKnee + new Vector2(5, -11), 4, armor);
                DrawCapsule(tex, rightKnee + new Vector2(-4, -4), rightKnee + new Vector2(4, -12), 4, armor);
                DrawEllipse(tex, leftFoot, 9, 4, outline);
                DrawEllipse(tex, leftFoot + Vector2.right * 1.5f, 6, 2.4f, bone);
                DrawEllipse(tex, rightFoot, 9, 4, outline);
                DrawEllipse(tex, rightFoot + Vector2.right * 1.5f, 6, 2.4f, bone);

                DrawPolygon(tex, outline, new Vector2(38, 60 + bob), new Vector2(58, 60 + bob), new Vector2(63, 38 + bob), new Vector2(53, 29), new Vector2(46, 40), new Vector2(38, 29), new Vector2(30, 42 + bob));
                DrawPolygon(tex, cloth, new Vector2(39, 58 + bob), new Vector2(57, 58 + bob), new Vector2(60, 41 + bob), new Vector2(54, 34), new Vector2(48, 44), new Vector2(40, 34), new Vector2(33, 43 + bob));
                DrawCapsule(tex, new Vector2(47, 56 + bob), new Vector2(48, 86 + bob), 15, outline);
                DrawCapsule(tex, new Vector2(47, 58 + bob), new Vector2(48, 84 + bob), 12, boneDark);
                DrawCapsule(tex, new Vector2(37, 78 + bob), new Vector2(58, 79 + bob), 2, bone);
                DrawCapsule(tex, new Vector2(38, 72 + bob), new Vector2(58, 72 + bob), 2, bone);
                DrawCapsule(tex, new Vector2(39, 66 + bob), new Vector2(57, 66 + bob), 2, bone);
                DrawCapsule(tex, new Vector2(49, 61 + bob), new Vector2(50, 82 + bob), 2, bone);
                DrawCapsule(tex, new Vector2(39, 55 + bob), new Vector2(58, 54 + bob), 4, leather);
                DrawEllipse(tex, new Vector2(34, 85 + bob), 10, 9, outline);
                DrawEllipse(tex, new Vector2(34, 85 + bob), 7, 6, armor);
                DrawEllipse(tex, new Vector2(60, 84 + bob), 10, 9, outline);
                DrawEllipse(tex, new Vector2(60, 84 + bob), 7, 6, armor);

                Vector2 swordHand = attacking ? new Vector2(22, 78 + bob) : new Vector2(27 - step * 0.2f, 62 + bob);
                DrawCapsule(tex, new Vector2(36, 82 + bob), swordHand, 5, outline);
                DrawCapsule(tex, new Vector2(36, 82 + bob), swordHand, 3, bone);
                DrawEllipse(tex, swordHand, 4, 4, outline);
                DrawEllipse(tex, swordHand, 2.6f, 2.6f, bone);
                Vector2 bladeTip = attacking ? new Vector2(4, 91 + bob) : new Vector2(15, 103 + bob);
                DrawCapsule(tex, swordHand, bladeTip, 5, outline);
                DrawCapsule(tex, swordHand + Vector2.left * 1.5f, bladeTip + new Vector2(1, -2), 3, weaponDark);
                DrawCapsule(tex, swordHand + Vector2.left * 2.5f, bladeTip + new Vector2(0, -4), 1.4f, weapon);
                DrawCapsule(tex, swordHand + new Vector2(-5, -4), swordHand + new Vector2(6, 5), 2, rust);

                DrawCapsule(tex, new Vector2(60, 82 + bob), new Vector2(72 + step * 0.08f, 65 + bob), 5, outline);
                DrawCapsule(tex, new Vector2(60, 82 + bob), new Vector2(72 + step * 0.08f, 65 + bob), 3, bone);
                DrawEllipse(tex, new Vector2(75, 67 + bob), 18, 18, outline);
                DrawEllipse(tex, new Vector2(75, 67 + bob), 15, 15, armorDark);
                DrawEllipse(tex, new Vector2(75, 67 + bob), 12, 12, armor);
                DrawCapsule(tex, new Vector2(63, 67 + bob), new Vector2(87, 67 + bob), 2, rust);
                DrawCapsule(tex, new Vector2(75, 55 + bob), new Vector2(75, 79 + bob), 2, rust);
                DrawCapsule(tex, new Vector2(66, 58 + bob), new Vector2(84, 76 + bob), 1.5f, boneDark);
                DrawCapsule(tex, new Vector2(84, 58 + bob), new Vector2(66, 76 + bob), 1.5f, boneDark);
                DrawEllipse(tex, new Vector2(75, 67 + bob), 5, 5, outline);
                DrawEllipse(tex, new Vector2(75, 67 + bob), 3, 3, boneDark);

                DrawEllipse(tex, new Vector2(48, 101 + bob), 16, 17, outline);
                DrawEllipse(tex, new Vector2(48, 100 + bob), 13, 14, bone);
                DrawEllipse(tex, new Vector2(43, 101 + bob), 3.4f, 3.8f, outline);
                DrawEllipse(tex, new Vector2(55, 101 + bob), 3.4f, 3.8f, outline);
                DrawEllipse(tex, new Vector2(55, 101 + bob), 1.4f, 1.4f, Hex("d4512f"));
                DrawCapsule(tex, new Vector2(43, 92 + bob), new Vector2(55, 92 + bob), 2, outline);
                DrawCapsule(tex, new Vector2(43, 92 + bob), new Vector2(55, 92 + bob), 1, boneDark);
                DrawCapsule(tex, new Vector2(38, 112 + bob), new Vector2(61, 113 + bob), 7, outline);
                DrawCapsule(tex, new Vector2(40, 112 + bob), new Vector2(59, 113 + bob), 5, armorDark);
                DrawEllipse(tex, new Vector2(48, 117 + bob), 9, 4, armor);
                DrawCapsule(tex, new Vector2(45, 116 + bob), new Vector2(36, 123 + bob), 2, plume);
                DrawCapsule(tex, new Vector2(49, 117 + bob), new Vector2(47, 126 + bob), 2, plume);
                DrawCapsule(tex, new Vector2(52, 116 + bob), new Vector2(61, 123 + bob), 2, plume);
            });
        }

        private static Sprite KnightRoll(string name, int frame)
        {
            return Create(name, tex =>
            {
                Color outline = Hex("080a10");
                Color armorDark = Hex("121821");
                Color armor = Hex("2c343d");
                Color armorMid = Hex("46515d");
                Color armorLight = Hex("87929f");
                Color gold = Hex("c8983f");
                Color goldBright = Hex("f2d579");
                Color capeDark = Hex("0f1f31");
                Color cape = Hex("1e3e5f");
                Color leather = Hex("2b2020");
                Color steel = Hex("d1dae2");
                Color steelDark = Hex("68747f");
                Color skin = Hex("e7ad83");

                float spin = frame == 0 ? -1f : 1f;
                DrawEllipse(tex, new Vector2(48, 16), 30, 5, new Color(0f, 0f, 0f, 0.34f));
                DrawPolygon(tex, outline, new Vector2(13, 48), new Vector2(27, 27), new Vector2(53, 18), new Vector2(82, 34), new Vector2(89, 56), new Vector2(71, 79), new Vector2(42, 82), new Vector2(20, 69));
                DrawPolygon(tex, capeDark, new Vector2(16, 48), new Vector2(29, 30), new Vector2(53, 22), new Vector2(79, 36), new Vector2(85, 54), new Vector2(69, 75), new Vector2(43, 78), new Vector2(23, 67));
                DrawPolygon(tex, cape, new Vector2(22, 48), new Vector2(38, 32), new Vector2(62, 32), new Vector2(78, 48), new Vector2(68, 66), new Vector2(46, 72), new Vector2(28, 63));
                DrawCapsule(tex, new Vector2(71, 44), new Vector2(85, 54), 1.3f, outline);
                DrawCapsule(tex, new Vector2(21, 45), new Vector2(13, 38), 1.3f, outline);

                DrawEllipse(tex, new Vector2(48, 53), 28, 25, outline);
                DrawEllipse(tex, new Vector2(48, 53), 24, 21, armorDark);
                DrawCapsule(tex, new Vector2(29, 57 + spin * 3f), new Vector2(65, 37 - spin * 3f), 7, armorMid);
                DrawCapsule(tex, new Vector2(31, 60 + spin * 3f), new Vector2(67, 41 - spin * 3f), 3, gold);
                DrawCapsule(tex, new Vector2(33, 42 - spin * 2f), new Vector2(65, 65 + spin * 2f), 5, armorLight);
                DrawCapsule(tex, new Vector2(34, 43 - spin * 2f), new Vector2(64, 64 + spin * 2f), 2, goldBright);

                DrawEllipse(tex, new Vector2(58 + spin * 5f, 71 - spin * 4f), 13, 12, outline);
                DrawEllipse(tex, new Vector2(58 + spin * 5f, 71 - spin * 4f), 9, 8, armorDark);
                DrawEllipse(tex, new Vector2(61 + spin * 4f, 70 - spin * 4f), 4, 6, skin);
                DrawCapsule(tex, new Vector2(52 + spin * 5f, 79 - spin * 4f), new Vector2(43 + spin * 8f, 88 - spin * 2f), 4.5f, outline);
                DrawCapsule(tex, new Vector2(53 + spin * 5f, 79 - spin * 4f), new Vector2(45 + spin * 8f, 88 - spin * 2f), 2.8f, armorLight);
                DrawCapsule(tex, new Vector2(52 + spin * 5f, 80 - spin * 4f), new Vector2(43 + spin * 7f, 89 - spin * 2f), 4, outline);
                DrawCapsule(tex, new Vector2(52 + spin * 5f, 80 - spin * 4f), new Vector2(44 + spin * 7f, 89 - spin * 2f), 2.5f, gold);

                DrawCapsule(tex, new Vector2(30, 54), new Vector2(17, 42 + spin * 4f), 6, outline);
                DrawCapsule(tex, new Vector2(30, 54), new Vector2(17, 42 + spin * 4f), 4, armor);
                DrawCapsule(tex, new Vector2(66, 55), new Vector2(82, 62 - spin * 4f), 6, outline);
                DrawCapsule(tex, new Vector2(66, 55), new Vector2(82, 62 - spin * 4f), 4, armor);
                DrawCapsule(tex, new Vector2(29, 52), new Vector2(17, 42 + spin * 4f), 1.6f, gold);
                DrawCapsule(tex, new Vector2(67, 55), new Vector2(82, 62 - spin * 4f), 1.6f, gold);
                DrawEllipse(tex, new Vector2(17, 42 + spin * 4f), 7, 5, outline);
                DrawEllipse(tex, new Vector2(82, 62 - spin * 4f), 7, 5, outline);

                DrawCapsule(tex, new Vector2(25, 35 + spin * 5f), new Vector2(87, 22 - spin * 2f), 5, outline);
                DrawCapsule(tex, new Vector2(27, 35 + spin * 5f), new Vector2(84, 23 - spin * 2f), 3, steelDark);
                DrawCapsule(tex, new Vector2(33, 34 + spin * 5f), new Vector2(85, 23 - spin * 2f), 1.4f, steel);
                DrawCapsule(tex, new Vector2(24, 36 + spin * 5f), new Vector2(39, 34 + spin * 4f), 3, gold);
                DrawEllipse(tex, new Vector2(24, 36 + spin * 5f), 3, 3, leather);
            });
        }

        private static Sprite Archer(string name, int frame)
        {
            return Create(name, tex =>
            {
                Color outline = Hex("17131f");
                Color bone = Hex("e7dcc0");
                Color boneDark = Hex("a89b7e");
                Color armor = Hex("6f6b66");
                Color armorDark = Hex("343033");
                Color cloth = Hex("2b272c");
                Color leather = Hex("4b3428");
                Color wood = Hex("8a5634");
                Color woodLight = Hex("c08a52");
                Color stringColor = Hex("d9d0b7");
                Color arrowTip = Hex("c0c5c6");
                Color plume = Hex("6f665a");

                bool aiming = frame == 2 || frame == 3;
                bool releasing = frame == 3;
                float bob = frame == 1 ? 0.8f : aiming ? -0.4f : 0f;

                DrawEllipse(tex, new Vector2(48, 15), 28, 5, new Color(0f, 0f, 0f, 0.28f));

                DrawPolygon(tex, outline, new Vector2(32, 86 + bob), new Vector2(43, 91 + bob), new Vector2(50, 55 + bob), new Vector2(37, 51 + bob));
                DrawPolygon(tex, leather, new Vector2(34, 84 + bob), new Vector2(42, 88 + bob), new Vector2(47, 57 + bob), new Vector2(38, 54 + bob));
                for (int i = 0; i < 4; i++)
                {
                    float x = 36 + i * 3f;
                    DrawCapsule(tex, new Vector2(x, 87 + bob), new Vector2(x + 8, 108 + bob), 1.4f, outline);
                    DrawCapsule(tex, new Vector2(x, 87 + bob), new Vector2(x + 8, 108 + bob), 0.8f, stringColor);
                    DrawPolygon(tex, arrowTip, new Vector2(x + 7, 108 + bob), new Vector2(x + 12, 111 + bob), new Vector2(x + 8, 104 + bob));
                }

                DrawCapsule(tex, new Vector2(38, 58 + bob), new Vector2(31, 19), 6, outline);
                DrawCapsule(tex, new Vector2(38, 58 + bob), new Vector2(31, 19), 4, bone);
                DrawCapsule(tex, new Vector2(57, 58 + bob), new Vector2(64, 19), 6, outline);
                DrawCapsule(tex, new Vector2(57, 58 + bob), new Vector2(64, 19), 4, bone);
                DrawEllipse(tex, new Vector2(31, 19), 8, 4, outline);
                DrawEllipse(tex, new Vector2(32, 19), 5.5f, 2.4f, bone);
                DrawEllipse(tex, new Vector2(64, 19), 8, 4, outline);
                DrawEllipse(tex, new Vector2(65, 19), 5.5f, 2.4f, bone);

                DrawPolygon(tex, outline, new Vector2(38, 59 + bob), new Vector2(58, 59 + bob), new Vector2(63, 38 + bob), new Vector2(54, 30), new Vector2(48, 42), new Vector2(40, 30), new Vector2(31, 42 + bob));
                DrawPolygon(tex, cloth, new Vector2(39, 58 + bob), new Vector2(57, 58 + bob), new Vector2(60, 41 + bob), new Vector2(54, 34), new Vector2(48, 44), new Vector2(41, 34), new Vector2(34, 43 + bob));
                DrawCapsule(tex, new Vector2(47, 56 + bob), new Vector2(48, 84 + bob), 14, outline);
                DrawCapsule(tex, new Vector2(47, 58 + bob), new Vector2(48, 82 + bob), 11, boneDark);
                DrawCapsule(tex, new Vector2(38, 76 + bob), new Vector2(58, 77 + bob), 2, bone);
                DrawCapsule(tex, new Vector2(40, 68 + bob), new Vector2(56, 68 + bob), 2, bone);
                DrawCapsule(tex, new Vector2(40, 55 + bob), new Vector2(58, 54 + bob), 4, leather);

                DrawEllipse(tex, new Vector2(34, 84 + bob), 9, 8, outline);
                DrawEllipse(tex, new Vector2(34, 84 + bob), 6, 5, armor);
                DrawEllipse(tex, new Vector2(60, 84 + bob), 9, 8, outline);
                DrawEllipse(tex, new Vector2(60, 84 + bob), 6, 5, armor);

                Vector2 bowHand = releasing ? new Vector2(81, 72 + bob) : aiming ? new Vector2(76, 72 + bob) : new Vector2(73, 70 + bob);
                DrawCapsule(tex, new Vector2(61, 82 + bob), bowHand, 5, outline);
                DrawCapsule(tex, new Vector2(61, 82 + bob), bowHand, 3, bone);
                DrawEllipse(tex, bowHand, 4, 4, outline);
                DrawEllipse(tex, bowHand, 2.5f, 2.5f, bone);
                Vector2 drawHand = releasing ? new Vector2(58, 72 + bob) : aiming ? new Vector2(41, 73 + bob) : new Vector2(50, 72 + bob);
                DrawCapsule(tex, new Vector2(31, 82 + bob), drawHand, 5, outline);
                DrawCapsule(tex, new Vector2(31, 82 + bob), drawHand, 3, bone);
                DrawEllipse(tex, drawHand + Vector2.right, 4, 4, outline);
                DrawEllipse(tex, drawHand + Vector2.right, 2.5f, 2.5f, bone);

                DrawCapsule(tex, new Vector2(77, 105 + bob), new Vector2(85, 86 + bob), 3, outline);
                DrawCapsule(tex, new Vector2(85, 86 + bob), new Vector2(83, 45 + bob), 3, outline);
                DrawCapsule(tex, new Vector2(83, 45 + bob), new Vector2(74, 28 + bob), 3, outline);
                DrawCapsule(tex, new Vector2(77, 105 + bob), new Vector2(85, 86 + bob), 1.8f, woodLight);
                DrawCapsule(tex, new Vector2(85, 86 + bob), new Vector2(83, 45 + bob), 1.8f, wood);
                DrawCapsule(tex, new Vector2(83, 45 + bob), new Vector2(74, 28 + bob), 1.8f, woodLight);
                Vector2 stringPull = aiming && !releasing ? drawHand : new Vector2(74, 28 + bob);
                Vector2 arrowStart = aiming && !releasing ? drawHand : new Vector2(52, 72 + bob);
                Vector2 arrowEnd = releasing ? new Vector2(92, 72 + bob) : new Vector2(81, 72 + bob);
                DrawCapsule(tex, new Vector2(77, 105 + bob), stringPull, 0.9f, stringColor);
                DrawCapsule(tex, arrowStart, arrowEnd, 1.5f, outline);
                DrawCapsule(tex, arrowStart + Vector2.right, arrowEnd - Vector2.right, 0.8f, stringColor);
                DrawPolygon(tex, arrowTip, arrowEnd + new Vector2(1, 0), arrowEnd + new Vector2(7, 3), arrowEnd + new Vector2(7, -3));

                DrawEllipse(tex, new Vector2(48, 101 + bob), 16, 17, outline);
                DrawEllipse(tex, new Vector2(48, 100 + bob), 13, 14, bone);
                DrawEllipse(tex, new Vector2(43, 101 + bob), 3.3f, 3.6f, outline);
                DrawEllipse(tex, new Vector2(55, 101 + bob), 3.3f, 3.6f, outline);
                DrawEllipse(tex, new Vector2(55, 101 + bob), 1.3f, 1.3f, Hex("d4512f"));
                DrawCapsule(tex, new Vector2(43, 92 + bob), new Vector2(55, 92 + bob), 2, outline);
                DrawCapsule(tex, new Vector2(43, 92 + bob), new Vector2(55, 92 + bob), 1, boneDark);
                DrawCapsule(tex, new Vector2(38, 112 + bob), new Vector2(61, 113 + bob), 7, outline);
                DrawCapsule(tex, new Vector2(40, 112 + bob), new Vector2(59, 113 + bob), 5, armorDark);
                DrawEllipse(tex, new Vector2(48, 117 + bob), 9, 4, armor);
                DrawCapsule(tex, new Vector2(44, 116 + bob), new Vector2(36, 123 + bob), 2, plume);
                DrawCapsule(tex, new Vector2(50, 117 + bob), new Vector2(50, 126 + bob), 2, plume);
                DrawCapsule(tex, new Vector2(54, 116 + bob), new Vector2(62, 123 + bob), 2, plume);
            });
        }

        private static Sprite Create(string name, Action<Texture2D> draw)
        {
            if (Cache.TryGetValue(name, out Sprite cached)) return cached;

            Texture2D texture = new Texture2D(Width, Height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };

            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
            {
                texture.SetPixel(x, y, Color.clear);
            }

            draw(texture);
            texture.Apply();
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, Width, Height), new Vector2(0.5f, 0.5f), PixelsPerUnit);
            sprite.name = name;
            Cache[name] = sprite;
            return sprite;
        }

        private static void DrawEllipse(Texture2D texture, Vector2 center, float radiusX, float radiusY, Color color)
        {
            int minX = Mathf.Max(0, Mathf.FloorToInt(center.x - radiusX - 1));
            int maxX = Mathf.Min(Width - 1, Mathf.CeilToInt(center.x + radiusX + 1));
            int minY = Mathf.Max(0, Mathf.FloorToInt(center.y - radiusY - 1));
            int maxY = Mathf.Min(Height - 1, Mathf.CeilToInt(center.y + radiusY + 1));

            for (int y = minY; y <= maxY; y++)
            for (int x = minX; x <= maxX; x++)
            {
                float dx = (x + 0.5f - center.x) / radiusX;
                float dy = (y + 0.5f - center.y) / radiusY;
                if (dx * dx + dy * dy <= 1f) Blend(texture, x, y, color);
            }
        }

        private static void DrawCapsule(Texture2D texture, Vector2 a, Vector2 b, float radius, Color color)
        {
            int minX = Mathf.Max(0, Mathf.FloorToInt(Mathf.Min(a.x, b.x) - radius - 1));
            int maxX = Mathf.Min(Width - 1, Mathf.CeilToInt(Mathf.Max(a.x, b.x) + radius + 1));
            int minY = Mathf.Max(0, Mathf.FloorToInt(Mathf.Min(a.y, b.y) - radius - 1));
            int maxY = Mathf.Min(Height - 1, Mathf.CeilToInt(Mathf.Max(a.y, b.y) + radius + 1));

            Vector2 ab = b - a;
            float lengthSquared = Mathf.Max(0.001f, ab.sqrMagnitude);
            for (int y = minY; y <= maxY; y++)
            for (int x = minX; x <= maxX; x++)
            {
                Vector2 p = new Vector2(x + 0.5f, y + 0.5f);
                float t = Mathf.Clamp01(Vector2.Dot(p - a, ab) / lengthSquared);
                Vector2 closest = a + ab * t;
                if ((p - closest).sqrMagnitude <= radius * radius) Blend(texture, x, y, color);
            }
        }

        private static void DrawPolygon(Texture2D texture, Color color, params Vector2[] points)
        {
            if (points == null || points.Length < 3) return;

            float minPointX = points[0].x;
            float maxPointX = points[0].x;
            float minPointY = points[0].y;
            float maxPointY = points[0].y;

            for (int i = 1; i < points.Length; i++)
            {
                minPointX = Mathf.Min(minPointX, points[i].x);
                maxPointX = Mathf.Max(maxPointX, points[i].x);
                minPointY = Mathf.Min(minPointY, points[i].y);
                maxPointY = Mathf.Max(maxPointY, points[i].y);
            }

            int minX = Mathf.Max(0, Mathf.FloorToInt(minPointX));
            int maxX = Mathf.Min(Width - 1, Mathf.CeilToInt(maxPointX));
            int minY = Mathf.Max(0, Mathf.FloorToInt(minPointY));
            int maxY = Mathf.Min(Height - 1, Mathf.CeilToInt(maxPointY));

            for (int y = minY; y <= maxY; y++)
            for (int x = minX; x <= maxX; x++)
            {
                Vector2 sample = new Vector2(x + 0.5f, y + 0.5f);
                if (PointInPolygon(sample, points)) Blend(texture, x, y, color);
            }
        }

        private static bool PointInPolygon(Vector2 point, Vector2[] points)
        {
            bool inside = false;
            for (int i = 0, j = points.Length - 1; i < points.Length; j = i++)
            {
                bool crosses = (points[i].y > point.y) != (points[j].y > point.y);
                if (!crosses) continue;

                float slopeX = (points[j].x - points[i].x) * (point.y - points[i].y) / (points[j].y - points[i].y + 0.0001f) + points[i].x;
                if (point.x < slopeX) inside = !inside;
            }

            return inside;
        }

        private static void Blend(Texture2D texture, int x, int y, Color color)
        {
            Color previous = texture.GetPixel(x, y);
            float alpha = color.a + previous.a * (1f - color.a);
            if (alpha <= 0f)
            {
                texture.SetPixel(x, y, Color.clear);
                return;
            }

            Color blended = new Color
            {
                r = (color.r * color.a + previous.r * previous.a * (1f - color.a)) / alpha,
                g = (color.g * color.a + previous.g * previous.a * (1f - color.a)) / alpha,
                b = (color.b * color.a + previous.b * previous.a * (1f - color.a)) / alpha,
                a = alpha
            };
            texture.SetPixel(x, y, blended);
        }

        private static Color Hex(string value)
        {
            ColorUtility.TryParseHtmlString("#" + value, out Color color);
            return color;
        }
    }
}
