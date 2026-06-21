using UnityEngine;

namespace DungeonKnight.Level
{
    public class SkeletonEnemyVisual3D : MonoBehaviour
    {
        [SerializeField] private float walkFrameRate = 7f;
        [SerializeField] private float attackFrameDuration = 0.14f;

        private SpriteRenderer spriteRenderer;
        private Sprite idleSprite;
        private Sprite[] attackSprites;
        private Sprite[] walkFrontSprites;
        private Sprite[] walkLeftSprites;
        private Sprite[] walkRightSprites;
        private float frameTimer;
        private float attackUntil;
        private int frameIndex;
        private bool moving;
        private Vector3 movementDirection;

        public void Configure(Sprite idle, Sprite[] attacks, Sprite[] walkFront, Sprite[] walkLeft, Sprite[] walkRight)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            idleSprite = idle;
            attackSprites = attacks;
            walkFrontSprites = walkFront;
            walkLeftSprites = walkLeft;
            walkRightSprites = walkRight;
            if (spriteRenderer) spriteRenderer.sprite = idleSprite;
        }

        public void SetMovement(bool value, Vector3 direction)
        {
            moving = value;
            if (direction.sqrMagnitude > 0.001f)
            {
                movementDirection = direction.normalized;
            }
        }

        public void PlayAttack(Vector3 directionToPlayer)
        {
            Sprite attackSprite = PickAttackSprite(directionToPlayer);
            if (spriteRenderer && attackSprite)
            {
                spriteRenderer.sprite = attackSprite;
            }

            attackUntil = Time.time + attackFrameDuration;
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (!spriteRenderer) return;
            FaceCamera();

            if (Time.time < attackUntil) return;

            if (!moving)
            {
                spriteRenderer.sprite = idleSprite;
                return;
            }

            Sprite[] frames = PickWalkFrames(movementDirection);
            if (frames == null || frames.Length == 0)
            {
                spriteRenderer.sprite = idleSprite;
                return;
            }

            frameTimer += Time.deltaTime;
            if (frameTimer >= 1f / Mathf.Max(1f, walkFrameRate))
            {
                frameTimer = 0f;
                frameIndex = (frameIndex + 1) % frames.Length;
            }

            if (frames[frameIndex])
            {
                spriteRenderer.sprite = frames[frameIndex];
            }
        }

        private void FaceCamera()
        {
            Camera camera = Camera.main;
            if (!camera) return;

            Vector3 direction = transform.position - camera.transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }

        private Sprite[] PickWalkFrames(Vector3 direction)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
            {
                return direction.x < 0f ? walkLeftSprites : walkRightSprites;
            }

            return walkFrontSprites;
        }

        private Sprite PickAttackSprite(Vector3 direction)
        {
            if (attackSprites == null || attackSprites.Length == 0) return idleSprite;

            int index = 0;
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
            {
                index = direction.x < 0f ? 3 : 2;
            }
            else if (direction.z > 0f)
            {
                index = 1;
            }

            index = Mathf.Clamp(index, 0, attackSprites.Length - 1);
            return attackSprites[index] ? attackSprites[index] : idleSprite;
        }
    }
}
