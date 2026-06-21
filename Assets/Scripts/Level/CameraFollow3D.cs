using DungeonKnight.Player;
using UnityEngine;

namespace DungeonKnight.Level
{
    public class CameraFollow3D : MonoBehaviour
    {
        public static readonly Vector3 DefaultOffset = new Vector3(0f, 7.4f, -8.6f).normalized * PlayerController3D.SharedFocusDistance;

        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = DefaultOffset;
        [SerializeField] private float smoothTime = 0.12f;
        [SerializeField] private float lookAhead = 1.35f;

        private Vector3 velocity;
        private PlayerController3D player;

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            player = target ? target.GetComponent<PlayerController3D>() : null;
        }

        private void LateUpdate()
        {
            if (!target) return;

            Transform lockOnTarget = player ? player.LockOnTarget : null;
            Vector3 ahead = lockOnTarget ? DirectionToLockOn(lockOnTarget) * lookAhead : target.forward * lookAhead;
            Vector3 desired = target.position + offset + ahead;
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
            Vector3 lookTarget = lockOnTarget
                ? Vector3.Lerp(target.position + Vector3.up * 1.35f, lockOnTarget.position + Vector3.up * 1.1f, 0.65f)
                : target.position + Vector3.up * 1.35f + ahead * 0.55f;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookTarget - transform.position), 10f * Time.deltaTime);
        }

        private Vector3 DirectionToLockOn(Transform lockOnTarget)
        {
            Vector3 direction = lockOnTarget.position - target.position;
            direction.y = 0f;
            return direction.sqrMagnitude > 0.001f ? direction.normalized : target.forward;
        }
    }
}
