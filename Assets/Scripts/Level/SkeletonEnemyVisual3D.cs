using UnityEngine;

namespace DungeonKnight.Level
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SkeletonEnemyVisual3D : MonoBehaviour
    {
        [SerializeField] private float attackFrameRate = 12f;
        [SerializeField] private float walkFrameRate = 8f;
        [SerializeField] private float moveBob = 0.08f;
        [SerializeField] private float moveSway = 4.5f;

        private SpriteRenderer spriteRenderer;
        private Transform cameraTransform;
        private Sprite idleSprite;
        private Sprite[] attackSprites;
        private Sprite[] walkFrontSprites;
        private Sprite[] walkLeftSprites;
        private Sprite[] walkRightSprites;
        private Vector3 baseLocalPosition;
        private Vector3 movementDirection = Vector3.forward;
        private float attackUntil;
        private float attackStartedAt;
        private bool moving;

        public void Configure(Sprite idle, Sprite[] attacks, Sprite[] walkFront, Sprite[] walkLeft, Sprite[] walkRight)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            idleSprite = idle;
            attackSprites = attacks;
            walkFrontSprites = walkFront;
            walkLeftSprites = walkLeft;
            walkRightSprites = walkRight;
            baseLocalPosition = transform.localPosition;
            if (idleSprite) spriteRenderer.sprite = idleSprite;
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
            if (attackSprites == null || attackSprites.Length == 0) return;

            attackStartedAt = Time.time;
            attackUntil = Time.time + 0.55f;
            spriteRenderer.sprite = ChooseAttackSprite(directionToPlayer);
            transform.localPosition = baseLocalPosition + Vector3.up * 0.05f;
        }

        private void LateUpdate()
        {
            if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();

            UpdateBillboard();

            if (Time.time < attackUntil)
            {
                float elapsed = Time.time - attackStartedAt;
                int frame = Mathf.FloorToInt(elapsed * attackFrameRate);
                if (attackSprites != null && attackSprites.Length > 0)
                {
                    spriteRenderer.sprite = attackSprites[Mathf.Clamp(frame, 0, attackSprites.Length - 1)];
                }

                return;
            }

            if (moving)
            {
                Sprite[] walkSprites = ChooseWalkSprites(movementDirection);
                if (walkSprites != null && walkSprites.Length > 0)
                {
                    int frame = Mathf.FloorToInt(Time.time * walkFrameRate) % walkSprites.Length;
                    spriteRenderer.sprite = walkSprites[frame];
                }
            }
            else if (idleSprite && spriteRenderer.sprite != idleSprite)
            {
                spriteRenderer.sprite = idleSprite;
            }

            float bob = moving ? Mathf.Sin(Time.time * moveSway) * moveBob : 0f;
            transform.localPosition = baseLocalPosition + Vector3.up * bob;
        }

        private Sprite ChooseAttackSprite(Vector3 directionToPlayer)
        {
            if (attackSprites == null || attackSprites.Length == 0) return idleSprite;

            Vector3 localDirection = transform.parent ? transform.parent.InverseTransformDirection(directionToPlayer.normalized) : directionToPlayer.normalized;
            if (localDirection.z < -0.45f && attackSprites.Length > 1) return attackSprites[1];
            if (localDirection.x > 0.45f && attackSprites.Length > 2) return attackSprites[2];
            if (localDirection.x < -0.45f && attackSprites.Length > 3) return attackSprites[3];
            if (localDirection.x < -0.15f && attackSprites.Length > 4) return attackSprites[4];
            if (localDirection.x > 0.15f && attackSprites.Length > 5) return attackSprites[5];
            return attackSprites[0];
        }

        private Sprite[] ChooseWalkSprites(Vector3 direction)
        {
            Vector3 localDirection = transform.parent ? transform.parent.InverseTransformDirection(direction.normalized) : direction.normalized;
            if (localDirection.x < -0.35f && walkLeftSprites != null && walkLeftSprites.Length > 0) return walkLeftSprites;
            if (localDirection.x > 0.35f && walkRightSprites != null && walkRightSprites.Length > 0) return walkRightSprites;
            return walkFrontSprites;
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
