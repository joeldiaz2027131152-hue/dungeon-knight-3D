using DungeonKnight.Player;
using UnityEngine;

namespace DungeonKnight.Level
{
    [RequireComponent(typeof(BoxCollider))]
    public class CloseRoomCameraZone3D : MonoBehaviour
    {
        [SerializeField] private float cameraHeight = 3.65f;
        [SerializeField] private float cameraDistance = 4.15f;
        [SerializeField] private float lookAhead = 2.45f;
        [SerializeField] private float lookHeight = 1.25f;
        [SerializeField] private float fieldOfView = 60f;
        [SerializeField] private float smoothTime = 0.08f;
        [SerializeField] private float minCameraZ = 57.35f;
        [SerializeField] private Vector3 lookDirection = Vector3.forward;

        private void Awake()
        {
            BoxCollider boxCollider = GetComponent<BoxCollider>();
            boxCollider.isTrigger = true;
        }

        public void Configure(
            float newCameraHeight,
            float newCameraDistance,
            float newLookAhead,
            float newLookHeight,
            float newFieldOfView,
            float newSmoothTime,
            float newMinCameraZ,
            Vector3 newLookDirection)
        {
            cameraHeight = newCameraHeight;
            cameraDistance = newCameraDistance;
            lookAhead = newLookAhead;
            lookHeight = newLookHeight;
            fieldOfView = newFieldOfView;
            smoothTime = newSmoothTime;
            minCameraZ = newMinCameraZ;
            lookDirection = newLookDirection.sqrMagnitude > 0.01f ? newLookDirection.normalized : Vector3.forward;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.GetComponentInParent<PlayerController3D>()) return;

            ApplyToCamera();
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.GetComponentInParent<PlayerController3D>()) return;

            ApplyToCamera();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.GetComponentInParent<PlayerController3D>()) return;

            CameraFollow3D.ClearCloseRoomOverride(this);
        }

        private void ApplyToCamera()
        {
            CameraFollow3D.SetCloseRoomOverride(this, cameraHeight, cameraDistance, lookAhead, lookHeight, smoothTime, fieldOfView, minCameraZ, lookDirection);
        }
    }
}
