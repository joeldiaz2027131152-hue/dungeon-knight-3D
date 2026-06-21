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

        public static void Shake(float amount, float duration)
        {
            if (!activeCamera) return;

            activeCamera.shakeAmount = Mathf.Max(activeCamera.shakeAmount, amount);
            activeCamera.shakeTimer = Mathf.Max(activeCamera.shakeTimer, duration);
        }

        private void Awake()
        {
            activeCamera = this;
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            player = target ? target.GetComponent<PlayerController3D>() : null;
        }

        private void LateUpdate()
        {
            if (!target) return;

            Vector3 ahead = target.forward * lookAhead;
            Transform lockTarget = player && player.LockOnTarget ? player.LockOnTarget.transform : null;
            Vector3 desired = target.position + offset + ahead;
            if (lockTarget)
            {
                Vector3 toEnemy = lockTarget.position - target.position;
                toEnemy.y = 0f;
                Vector3 side = Vector3.Cross(Vector3.up, toEnemy.normalized);
                desired = target.position + Vector3.up * 4.9f - toEnemy.normalized * 5.7f + side * 0.9f;
            }

            transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
            if (shakeTimer > 0f)
            {
                shakeTimer -= Time.deltaTime;
                Vector3 shake = Random.insideUnitSphere * shakeAmount * Mathf.Clamp01(shakeTimer / 0.2f);
                shake.y *= 0.45f;
                transform.position += shake;
            }

            Vector3 lookTarget = lockTarget
                ? Vector3.Lerp(target.position + Vector3.up * 1.15f, lockTarget.position + Vector3.up * 1.0f, 0.56f)
                : target.position + Vector3.up * 1.35f + ahead * 0.55f;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookTarget - transform.position), 10f * Time.deltaTime);
        }
    }
}
