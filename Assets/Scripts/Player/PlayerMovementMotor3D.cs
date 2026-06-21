using UnityEngine;

namespace DungeonKnight.Player
{
    internal sealed class PlayerMovementMotor3D
    {
        private readonly float moveSpeed;
        private readonly float dashSpeed;
        private readonly float jumpSpeed;
        private readonly float gravity;
        private readonly float turnSpeed;
        private readonly float blockMoveMultiplier;

        private Vector3 velocity;
        private float dashTimer;

        public PlayerMovementMotor3D(
            float moveSpeed,
            float dashSpeed,
            float jumpSpeed,
            float gravity,
            float turnSpeed,
            float blockMoveMultiplier)
        {
            this.moveSpeed = moveSpeed;
            this.dashSpeed = dashSpeed;
            this.jumpSpeed = jumpSpeed;
            this.gravity = gravity;
            this.turnSpeed = turnSpeed;
            this.blockMoveMultiplier = blockMoveMultiplier;
        }

        public Vector3 LastMoveDirection { get; private set; } = Vector3.forward;
        public bool IsBlocking { get; private set; }
        public bool IsDashing => dashTimer > 0f;

        public void Tick(CharacterController controller, Transform playerTransform, Transform cameraPivot, ref float stamina)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 input = new Vector3(horizontal, 0f, vertical);
            input = Vector3.ClampMagnitude(input, 1f);

            Vector3 forward = Vector3.forward;
            Vector3 right = Vector3.right;
            if (cameraPivot)
            {
                forward = Vector3.ProjectOnPlane(cameraPivot.forward, Vector3.up).normalized;
                right = Vector3.ProjectOnPlane(cameraPivot.right, Vector3.up).normalized;
            }

            Vector3 move = (right * input.x + forward * input.z).normalized;
            if (move.sqrMagnitude > 0.001f)
            {
                LastMoveDirection = move;
                Quaternion targetRotation = Quaternion.LookRotation(LastMoveDirection, Vector3.up);
                playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }

            if (controller.isGrounded && velocity.y < 0f) velocity.y = -1.5f;
            if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
            {
                velocity.y = jumpSpeed;
            }

            if (Input.GetKeyDown(KeyCode.L) && dashTimer <= 0f && stamina >= 24f)
            {
                stamina -= 24f;
                dashTimer = 0.24f;
            }

            IsBlocking = Input.GetKey(KeyCode.K) && stamina > 0f && dashTimer <= 0f;
            if (IsBlocking) stamina = Mathf.Max(0f, stamina - 16f * Time.deltaTime);

            float speed = IsBlocking ? moveSpeed * blockMoveMultiplier : moveSpeed;
            if (dashTimer > 0f)
            {
                dashTimer -= Time.deltaTime;
                move = LastMoveDirection;
                speed = dashSpeed;
            }

            velocity.y += gravity * Time.deltaTime;
            controller.Move((move * speed + velocity) * Time.deltaTime);
        }

        public void Reset()
        {
            velocity = Vector3.zero;
            dashTimer = 0f;
            IsBlocking = false;
            LastMoveDirection = Vector3.forward;
        }
    }
}
