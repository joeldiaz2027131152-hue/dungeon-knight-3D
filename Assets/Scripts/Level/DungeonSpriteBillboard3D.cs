using UnityEngine;

namespace DungeonKnight.Level
{
    public class DungeonSpriteBillboard3D : MonoBehaviour
    {
        private Transform cameraTransform;

        private void LateUpdate()
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
