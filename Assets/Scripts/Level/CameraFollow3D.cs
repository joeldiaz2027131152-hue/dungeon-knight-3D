using DungeonKnight.Player;
using UnityEngine;

namespace DungeonKnight.Level
{
    public class CameraFollow3D : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 7.8f, -8.9f);
        [SerializeField] private float smoothTime = 0.14f;
        [SerializeField] private float lookAhead = 1.05f;

        private Vector3 velocity;
        private PlayerController3D player;
        private static CameraFollow3D activeCamera;
        private float shakeTimer;
        private float shakeAmount;
        private Camera attachedCamera;
        private float defaultFieldOfView;
        private Object closeRoomSource;
        private float closeRoomHeight;
        private float closeRoomDistance;
        private float closeRoomLookAhead;
        private float closeRoomLookHeight;
        private float closeRoomSmoothTime;
        private float closeRoomFieldOfView;
        private float closeRoomMinZ;
        private Vector3 closeRoomLookDirection = Vector3.forward;
        private bool hasCloseRoomOverride;

        public static void Shake(float amount, float duration)
        {
            if (!activeCamera) return;

            activeCamera.shakeAmount = Mathf.Max(activeCamera.shakeAmount, amount);
            activeCamera.shakeTimer = Mathf.Max(activeCamera.shakeTimer, duration);
        }

        private void Awake()
        {
            activeCamera = this;
            attachedCamera = GetComponent<Camera>();
            defaultFieldOfView = attachedCamera ? attachedCamera.fieldOfView : 58f;
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            player = target ? target.GetComponent<PlayerController3D>() : null;
        }

        public static void SetCloseRoomOverride(
            Object source,
            float height,
            float distance,
            float newLookAhead,
            float lookHeight,
            float newSmoothTime,
            float fieldOfView,
            float minCameraZ,
            Vector3 lookDirection)
        {
            if (!activeCamera) return;

            activeCamera.closeRoomSource = source;
            activeCamera.closeRoomHeight = Mathf.Max(1.4f, height);
            activeCamera.closeRoomDistance = Mathf.Max(0.8f, distance);
            activeCamera.closeRoomLookAhead = Mathf.Max(0f, newLookAhead);
            activeCamera.closeRoomLookHeight = Mathf.Max(0.8f, lookHeight);
            activeCamera.closeRoomSmoothTime = Mathf.Max(0.04f, newSmoothTime);
            activeCamera.closeRoomFieldOfView = Mathf.Clamp(fieldOfView, 42f, 78f);
            activeCamera.closeRoomMinZ = minCameraZ;
            activeCamera.closeRoomLookDirection = lookDirection.sqrMagnitude > 0.01f ? lookDirection.normalized : Vector3.forward;
            activeCamera.hasCloseRoomOverride = true;
        }

        public static void ClearCloseRoomOverride(Object source)
        {
            if (!activeCamera || activeCamera.closeRoomSource != source) return;

            activeCamera.hasCloseRoomOverride = false;
            activeCamera.closeRoomSource = null;
        }

        private void LateUpdate()
        {
            if (!target) return;

            Vector3 flatForward = target.forward;
            flatForward.y = 0f;
            if (flatForward.sqrMagnitude < 0.01f) flatForward = Vector3.forward;
            flatForward.Normalize();
            if (hasCloseRoomOverride)
            {
                flatForward = closeRoomLookDirection;
                flatForward.y = 0f;
                flatForward.Normalize();
            }

            Vector3 ahead = flatForward * (hasCloseRoomOverride ? closeRoomLookAhead : lookAhead);
            Transform lockTarget = player && player.LockOnTarget ? player.LockOnTarget.transform : null;
            Vector3 desired = hasCloseRoomOverride
                ? target.position + Vector3.up * closeRoomHeight - flatForward * closeRoomDistance
                : target.position + offset + ahead;

            if (hasCloseRoomOverride)
            {
                desired.z = Mathf.Max(desired.z, closeRoomMinZ);
            }
            else if (lockTarget)
            {
                Vector3 toEnemy = lockTarget.position - target.position;
                toEnemy.y = 0f;
                Vector3 side = Vector3.Cross(Vector3.up, toEnemy.normalized);
                desired = target.position + Vector3.up * 4.9f - toEnemy.normalized * 5.7f + side * 0.9f;
            }

            float activeSmoothTime = hasCloseRoomOverride ? closeRoomSmoothTime : smoothTime;
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, activeSmoothTime);
            if (shakeTimer > 0f)
            {
                shakeTimer -= Time.deltaTime;
                Vector3 shake = Random.insideUnitSphere * shakeAmount * Mathf.Clamp01(shakeTimer / 0.2f);
                shake.y *= 0.45f;
                transform.position += shake;
            }

            if (attachedCamera)
            {
                float targetFov = hasCloseRoomOverride ? closeRoomFieldOfView : defaultFieldOfView;
                attachedCamera.fieldOfView = Mathf.Lerp(attachedCamera.fieldOfView, targetFov, 8f * Time.deltaTime);
            }

            Vector3 roomLookTarget = target.position + Vector3.up * closeRoomLookHeight + ahead;
            if (lockTarget && hasCloseRoomOverride)
            {
                roomLookTarget = Vector3.Lerp(roomLookTarget, lockTarget.position + Vector3.up * 1.0f, 0.25f);
            }

            Vector3 lookTarget = hasCloseRoomOverride
                ? roomLookTarget
                : lockTarget
                ? Vector3.Lerp(target.position + Vector3.up * 1.15f, lockTarget.position + Vector3.up * 1.0f, 0.56f)
                : target.position + Vector3.up * 1.35f + ahead * 0.55f;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookTarget - transform.position), 10f * Time.deltaTime);
        }
    }
}
