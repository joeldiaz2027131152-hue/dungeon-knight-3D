using DungeonKnight.Player;
using UnityEngine;

namespace DungeonKnight.Level
{
    public class PlayerVisual3D : MonoBehaviour
    {
        [SerializeField] private float walkFrameRate = 8f;
        [SerializeField] private float actionFrameRate = 12f;

        private PlayerController3D controller;
        private SpriteRenderer spriteRenderer;
        private Sprite idleSprite;
        private Sprite[] walkFrontSprites;
        private Sprite[] walkLeftSprites;
        private Sprite[] walkRightSprites;
        private Sprite[] walkBackSprites;
        private Sprite[] rollFrontSprites;
        private Sprite[] rollLeftSprites;
        private Sprite[] rollRightSprites;
        private Sprite[] rollBackSprites;
        private Sprite[] attackFrontSprites;
        private Sprite[] attackBackSprites;
        private Sprite[] attackLeftSprites;
        private Sprite[] attackRightSprites;
        private Sprite[] blockSprites;
        private Sprite[] blockWalkFrontSprites;
        private Sprite[] blockWalkLeftSprites;
        private Sprite[] blockWalkRightSprites;
        private Sprite[] blockWalkBackSprites;
        private Vector3 previousPosition;
        private Vector3 moveDirection = Vector3.forward;
        private float frameTimer;
        private int frameIndex;

        public void Configure(
            PlayerController3D playerController,
            Sprite idle,
            Sprite[] walkFront,
            Sprite[] walkLeft,
            Sprite[] walkRight,
            Sprite[] walkBack,
            Sprite[] rollFront,
            Sprite[] rollLeft,
            Sprite[] rollRight,
            Sprite[] rollBack,
            Sprite[] attackFront,
            Sprite[] attackBack,
            Sprite[] attackLeft,
            Sprite[] attackRight,
            Sprite[] block,
            Sprite[] blockWalkFront,
            Sprite[] blockWalkLeft,
            Sprite[] blockWalkRight,
            Sprite[] blockWalkBack)
        {
            controller = playerController;
            idleSprite = idle;
            walkFrontSprites = walkFront;
            walkLeftSprites = walkLeft;
            walkRightSprites = walkRight;
            walkBackSprites = walkBack;
            rollFrontSprites = rollFront;
            rollLeftSprites = rollLeft;
            rollRightSprites = rollRight;
            rollBackSprites = rollBack;
            attackFrontSprites = attackFront;
            attackBackSprites = attackBack;
            attackLeftSprites = attackLeft;
            attackRightSprites = attackRight;
            blockSprites = block;
            blockWalkFrontSprites = blockWalkFront;
            blockWalkLeftSprites = blockWalkLeft;
            blockWalkRightSprites = blockWalkRight;
            blockWalkBackSprites = blockWalkBack;
            spriteRenderer = GetComponent<SpriteRenderer>();
            previousPosition = transform.parent ? transform.parent.position : transform.position;
            if (spriteRenderer) spriteRenderer.sprite = idleSprite;
        }

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            previousPosition = transform.parent ? transform.parent.position : transform.position;
        }

        private void LateUpdate()
        {
            if (!spriteRenderer || !controller) return;

            FaceCamera();
            Vector3 parentPosition = transform.parent ? transform.parent.position : transform.position;
            Vector3 delta = parentPosition - previousPosition;
            delta.y = 0f;
            previousPosition = parentPosition;
            bool moving = delta.sqrMagnitude > 0.0001f;
            if (moving)
            {
                moveDirection = delta.normalized;
            }

            Sprite[] frames = PickFrames(moving);
            float rate = controller.IsDashing || controller.IsCharging ? actionFrameRate : walkFrameRate;
            spriteRenderer.sprite = PickFrame(frames, moving || controller.IsDashing || controller.IsCharging || controller.IsBlocking, rate);
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

        private Sprite[] PickFrames(bool moving)
        {
            if (controller.IsCharging)
            {
                return PickDirectional(attackFrontSprites, attackBackSprites, attackLeftSprites, attackRightSprites);
            }

            if (controller.IsDashing)
            {
                return PickDirectional(rollFrontSprites, rollBackSprites, rollLeftSprites, rollRightSprites);
            }

            if (controller.IsBlocking)
            {
                if (moving)
                {
                    return PickDirectional(blockWalkFrontSprites, blockWalkBackSprites, blockWalkLeftSprites, blockWalkRightSprites);
                }

                return blockSprites;
            }

            return moving ? PickDirectional(walkFrontSprites, walkBackSprites, walkLeftSprites, walkRightSprites) : null;
        }

        private Sprite[] PickDirectional(Sprite[] front, Sprite[] back, Sprite[] left, Sprite[] right)
        {
            if (Mathf.Abs(moveDirection.x) > Mathf.Abs(moveDirection.z))
            {
                return moveDirection.x < 0f ? left : right;
            }

            return moveDirection.z > 0f ? back : front;
        }

        private Sprite PickFrame(Sprite[] frames, bool animate, float rate)
        {
            if (frames == null || frames.Length == 0) return idleSprite;

            if (!animate) return frames[0] ? frames[0] : idleSprite;

            frameTimer += Time.deltaTime;
            if (frameTimer >= 1f / Mathf.Max(1f, rate))
            {
                frameTimer = 0f;
                frameIndex = (frameIndex + 1) % frames.Length;
            }

            return frames[frameIndex] ? frames[frameIndex] : idleSprite;
        }
    }
}
