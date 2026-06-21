using UnityEngine;

namespace DungeonKnight.Player
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerVisual3D : MonoBehaviour
    {
        [SerializeField] private float metersPerWalkFrame = 0.58f;
        [SerializeField] private float metersPerRollFrame = 0.42f;
        [SerializeField] private float targetWorldHeight = 6.9f;
        [SerializeField] private float moveBob = 0.06f;

        private enum DirectionBucket
        {
            Front,
            Back,
            Left,
            Right
        }

        private PlayerController3D controller;
        private SpriteRenderer spriteRenderer;
        private Transform cameraTransform;
        private Vector3 baseLocalPosition;
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
        private float frameDistance;
        private int locomotionFrame;

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
            Sprite[] blocks,
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
            blockSprites = blocks;
            blockWalkFrontSprites = blockWalkFront;
            blockWalkLeftSprites = blockWalkLeft;
            blockWalkRightSprites = blockWalkRight;
            blockWalkBackSprites = blockWalkBack;

            spriteRenderer = GetComponent<SpriteRenderer>();
            baseLocalPosition = transform.localPosition;
            SetSprite(idleSprite);
        }

        private void LateUpdate()
        {
            if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
            if (!controller) return;

            UpdateBillboard();

            Vector3 direction = controller.IsAttacking || controller.IsCharging
                ? controller.FacingDirection
                : controller.IsMoving ? controller.MoveDirection : controller.FacingDirection;
            Sprite nextSprite = ChooseSprite(direction);
            SetSprite(nextSprite);

            float bob = controller.IsMoving && !controller.IsDashing ? Mathf.Sin(Time.time * 10f) * moveBob : 0f;
            transform.localPosition = baseLocalPosition + Vector3.up * bob;
        }

        private Sprite ChooseSprite(Vector3 direction)
        {
            if (controller.IsDashing)
            {
                return DistanceLoopFrame(ChooseDirectionalFrames(direction, rollFrontSprites, rollBackSprites, rollLeftSprites, rollRightSprites), metersPerRollFrame);
            }

            if (controller.IsBlocking)
            {
                if (controller.IsMoving)
                {
                    return DistanceLoopFrame(ChooseDirectionalFrames(direction, blockWalkFrontSprites, blockWalkBackSprites, blockWalkLeftSprites, blockWalkRightSprites), metersPerWalkFrame);
                }

                return ChooseDirectionalSprite(direction, blockSprites);
            }

            if (controller.IsAttacking)
            {
                return ProgressFrame(ChooseDirectionalFrames(direction, attackFrontSprites, attackBackSprites, attackLeftSprites, attackRightSprites), controller.AttackProgress);
            }

            if (controller.IsCharging)
            {
                Sprite[] windupFrames = ChooseDirectionalFrames(direction, attackFrontSprites, attackBackSprites, attackLeftSprites, attackRightSprites);
                return HasSprites(windupFrames) && windupFrames[0] ? windupFrames[0] : idleSprite;
            }

            if (controller.IsMoving)
            {
                return DistanceLoopFrame(ChooseDirectionalFrames(direction, walkFrontSprites, walkBackSprites, walkLeftSprites, walkRightSprites), metersPerWalkFrame);
            }

            frameDistance = 0f;
            locomotionFrame = 0;
            return idleSprite;
        }

        private Sprite[] ChooseDirectionalFrames(Vector3 direction, Sprite[] front, Sprite[] back, Sprite[] left, Sprite[] right)
        {
            DirectionBucket bucket = DirectionFor(direction);
            if (bucket == DirectionBucket.Back && HasSprites(back)) return back;
            if (bucket == DirectionBucket.Left && HasSprites(left)) return left;
            if (bucket == DirectionBucket.Right && HasSprites(right)) return right;
            return front;
        }

        private Sprite ChooseDirectionalSprite(Vector3 direction, Sprite[] sprites)
        {
            if (!HasSprites(sprites)) return idleSprite;

            DirectionBucket bucket = DirectionFor(direction);
            if (bucket == DirectionBucket.Back && sprites.Length > 1 && sprites[1]) return sprites[1];
            if (bucket == DirectionBucket.Right && sprites.Length > 2 && sprites[2]) return sprites[2];
            if (bucket == DirectionBucket.Left && sprites.Length > 3 && sprites[3]) return sprites[3];
            return sprites[0] ? sprites[0] : idleSprite;
        }

        private Sprite DistanceLoopFrame(Sprite[] sprites, float metersPerFrame)
        {
            if (!HasSprites(sprites)) return idleSprite;

            float speed = controller ? controller.PlanarSpeed : 0f;
            if (speed <= 0.08f)
            {
                frameDistance = 0f;
                locomotionFrame = 0;
            }
            else
            {
                frameDistance += speed * Time.deltaTime;
                float stepDistance = Mathf.Max(0.01f, metersPerFrame);
                while (frameDistance >= stepDistance)
                {
                    frameDistance -= stepDistance;
                    locomotionFrame++;
                }
            }

            int frame = Mathf.Abs(locomotionFrame) % sprites.Length;
            return sprites[frame] ? sprites[frame] : idleSprite;
        }

        private Sprite ProgressFrame(Sprite[] sprites, float progress)
        {
            if (!HasSprites(sprites)) return idleSprite;

            int frame = Mathf.Clamp(Mathf.FloorToInt(Mathf.Clamp01(progress) * sprites.Length), 0, sprites.Length - 1);
            return sprites[frame] ? sprites[frame] : idleSprite;
        }

        private void SetSprite(Sprite sprite)
        {
            if (!sprite || !spriteRenderer) return;

            spriteRenderer.sprite = sprite;
            Vector2 size = sprite.bounds.size;
            if (size.y <= 0.001f) return;

            float scale = targetWorldHeight / size.y;
            transform.localScale = new Vector3(scale, scale, scale);
        }

        private void CameraBasis(out Vector3 viewForward, out Vector3 viewRight)
        {
            if (!cameraTransform && Camera.main) cameraTransform = Camera.main.transform;

            viewForward = cameraTransform ? Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized : Vector3.forward;
            viewRight = cameraTransform ? Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized : Vector3.right;

            if (viewForward.sqrMagnitude < 0.001f) viewForward = Vector3.forward;
            if (viewRight.sqrMagnitude < 0.001f) viewRight = Vector3.right;
        }

        private DirectionBucket DirectionFor(Vector3 direction)
        {
            CameraBasis(out Vector3 viewForward, out Vector3 viewRight);
            Vector3 flatDirection = FlattenDirection(direction);
            float forwardDot = Vector3.Dot(flatDirection, viewForward);
            float rightDot = Vector3.Dot(flatDirection, viewRight);

            if (Mathf.Abs(forwardDot) >= Mathf.Abs(rightDot))
            {
                return forwardDot > 0f ? DirectionBucket.Back : DirectionBucket.Front;
            }

            return rightDot > 0f ? DirectionBucket.Right : DirectionBucket.Left;
        }

        private static Vector3 FlattenDirection(Vector3 direction)
        {
            Vector3 flatDirection = Vector3.ProjectOnPlane(direction, Vector3.up);
            if (flatDirection.sqrMagnitude < 0.001f) return Vector3.forward;
            return flatDirection.normalized;
        }

        private static bool HasSprites(Sprite[] sprites)
        {
            return sprites != null && sprites.Length > 0;
        }

        private void UpdateBillboard()
        {
            if (!cameraTransform && Camera.main)
            {
                cameraTransform = Camera.main.transform;
            }

            if (!cameraTransform) return;

            Vector3 toCamera = transform.position - cameraTransform.position;
            toCamera.y = 0f;
            if (toCamera.sqrMagnitude < 0.001f) return;

            transform.rotation = Quaternion.LookRotation(toCamera.normalized, Vector3.up);
        }
    }
}
