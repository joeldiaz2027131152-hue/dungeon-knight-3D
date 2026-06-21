using DungeonKnight.Enemies;
using DungeonKnight.Player;
using UnityEngine;

namespace DungeonKnight.Visuals
{
    [RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D))]
    public class CharacterFrameAnimator : MonoBehaviour
    {
        public enum VisualKind
        {
            Knight,
            Guard,
            Archer
        }

        [SerializeField] private VisualKind kind;

        private SpriteRenderer spriteRenderer;
        private SpriteRenderer targetRenderer;
        private Rigidbody2D body;
        private PlayerController2D player;
        private SkeletonMinionAI guard;
        private SkeletonArcherAI archer;
        private float frameTimer;
        private int frame;

        public void Configure(VisualKind visualKind)
        {
            kind = visualKind;
        }

        public void Configure(VisualKind visualKind, SpriteRenderer visualTarget)
        {
            kind = visualKind;
            targetRenderer = visualTarget;
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            body = GetComponent<Rigidbody2D>();
            player = GetComponent<PlayerController2D>();
            guard = GetComponent<SkeletonMinionAI>();
            archer = GetComponent<SkeletonArcherAI>();
            targetRenderer ??= kind == VisualKind.Knight && player ? EnsureKnightVisual() : spriteRenderer;
        }

        private void Update()
        {
            frameTimer += Time.deltaTime;
            float frameDuration = player && player.IsRolling ? 0.07f : 0.16f;
            if (frameTimer >= frameDuration)
            {
                frameTimer = 0f;
                frame = (frame + 1) % 4;
            }

            Sprite nextSprite = kind switch
            {
                VisualKind.Knight => KnightFrame(),
                VisualKind.Archer => ArcherFrame(),
                _ => GuardFrame()
            };

            targetRenderer.sprite = nextSprite;

            if (player)
            {
                HideDuplicateKnightRenderers();
                targetRenderer.flipX = player.Facing < 0;
                ApplyKnightVisualTransform(targetRenderer.transform, nextSprite);
            }
        }

        private Sprite KnightFrame()
        {
            if (player && player.IsRolling) return CharacterSpriteFactory.KnightRollFrame(frame);
            if (player && player.IsBlocking) return CharacterSpriteFactory.KnightBlockFrame(frame);
            if (player && player.IsCharging) return CharacterSpriteFactory.KnightCharge();
            if (player && player.IsAttacking) return CharacterSpriteFactory.KnightAttackFrame(frame);
            if (player && player.IsCrouching) return CharacterSpriteFactory.KnightCrouchFrame(frame);
            if (player && !player.IsGrounded) return CharacterSpriteFactory.KnightJump();
            if (Mathf.Abs(body.linearVelocity.x) > 0.15f)
            {
                return frame switch
                {
                    0 => CharacterSpriteFactory.KnightRunA(),
                    1 => CharacterSpriteFactory.KnightRunB(),
                    2 => CharacterSpriteFactory.KnightRunC(),
                    _ => CharacterSpriteFactory.KnightRunD()
                };
            }

            return frame % 4 < 2 ? CharacterSpriteFactory.KnightIdleA() : CharacterSpriteFactory.KnightIdleB();
        }

        private Sprite GuardFrame()
        {
            if (guard && guard.IsAttacking) return CharacterSpriteFactory.GuardAttack();
            if (Mathf.Abs(body.linearVelocity.x) > 0.15f)
            {
                return frame switch
                {
                    0 => CharacterSpriteFactory.GuardRunA(),
                    1 => CharacterSpriteFactory.GuardRunB(),
                    2 => CharacterSpriteFactory.GuardRunC(),
                    _ => CharacterSpriteFactory.GuardRunD()
                };
            }

            return frame % 4 < 2 ? CharacterSpriteFactory.GuardIdleA() : CharacterSpriteFactory.GuardIdleB();
        }

        private Sprite ArcherFrame()
        {
            if (archer && archer.IsAiming)
            {
                return archer.AimProgress > 0.72f ? CharacterSpriteFactory.ArcherRelease() : CharacterSpriteFactory.ArcherAim();
            }

            return frame % 4 < 2 ? CharacterSpriteFactory.ArcherIdleA() : CharacterSpriteFactory.ArcherIdleB();
        }

        private SpriteRenderer EnsureKnightVisual()
        {
            Sprite idle = CharacterSpriteFactory.KnightIdleA();
            SpriteRenderer existingRenderer = FindPrimaryKnightVisual();
            if (existingRenderer)
            {
                if (spriteRenderer) spriteRenderer.enabled = false;
                existingRenderer.sprite = idle;
                ApplyKnightVisualTransform(existingRenderer.transform, idle);
                HideDuplicateKnightRenderers(existingRenderer);
                return existingRenderer;
            }

            if (spriteRenderer) spriteRenderer.enabled = false;

            GameObject visual = new GameObject("Knight Visual 2D Active");
            visual.transform.SetParent(transform, false);
            SpriteRenderer renderer = visual.AddComponent<SpriteRenderer>();
            renderer.sprite = idle;
            renderer.sortingOrder = 6;
            ApplyKnightVisualTransform(visual.transform, idle);
            HideDuplicateKnightRenderers(renderer);

            return renderer;
        }

        private SpriteRenderer FindPrimaryKnightVisual()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.name != "Knight Visual 2D Active" || !child.gameObject.activeInHierarchy) continue;
                if (!child.TryGetComponent(out SpriteRenderer childRenderer)) continue;

                return childRenderer;
            }

            return null;
        }

        private void HideDuplicateKnightRenderers(SpriteRenderer primary = null)
        {
            primary ??= targetRenderer;
            if (spriteRenderer) spriteRenderer.enabled = false;

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (!child.name.StartsWith("Knight Visual 2D")) continue;

                bool isPrimary = primary && child.gameObject == primary.gameObject;
                if (child.TryGetComponent(out SpriteRenderer childRenderer))
                {
                    childRenderer.enabled = isPrimary;
                }

                child.gameObject.SetActive(isPrimary);
            }
        }

        private void ApplyKnightVisualTransform(Transform visual, Sprite sprite)
        {
            Vector2 spriteSize = sprite.bounds.size;
            Vector2 visualWorldSize = new Vector2(2.35f, 2.3f);
            Vector2 visualWorldOffset = new Vector2(0.12f, 0.42f);
            Vector3 parentScale = transform.localScale;
            visual.localScale = new Vector3(
                visualWorldSize.x / spriteSize.x / parentScale.x,
                visualWorldSize.y / spriteSize.y / parentScale.y,
                1f);
            visual.localPosition = new Vector3(
                visualWorldOffset.x / parentScale.x,
                visualWorldOffset.y / parentScale.y,
                0f);
        }
    }
}
