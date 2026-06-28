using UnityEngine;

namespace DungeonKnight.Level
{
    public class GothicDoubleDoor3D : MonoBehaviour
    {
        [SerializeField] private Transform leftPivot;
        [SerializeField] private Transform rightPivot;
        [SerializeField] private Collider blocker;
        [SerializeField] private float openAngle = 105f;
        [SerializeField] private float openSpeed = 4.8f;

        private Quaternion leftClosedRotation;
        private Quaternion rightClosedRotation;
        private bool open;

        public bool IsOpen => open;

        private void Awake()
        {
            EnsureRuntimeSetup();
        }

        private void Update()
        {
            if (!open) return;

            Quaternion leftOpenRotation = leftClosedRotation * Quaternion.Euler(0f, -openAngle, 0f);
            Quaternion rightOpenRotation = rightClosedRotation * Quaternion.Euler(0f, openAngle, 0f);
            leftPivot.localRotation = Quaternion.Slerp(leftPivot.localRotation, leftOpenRotation, openSpeed * Time.deltaTime);
            rightPivot.localRotation = Quaternion.Slerp(rightPivot.localRotation, rightOpenRotation, openSpeed * Time.deltaTime);
        }

        public void Configure(Transform newLeftPivot, Transform newRightPivot, Collider newBlocker)
        {
            leftPivot = newLeftPivot;
            rightPivot = newRightPivot;
            blocker = newBlocker;
            CacheClosedRotations();
        }

        public bool Open()
        {
            EnsureRuntimeSetup();
            if (!leftPivot || !rightPivot) return false;

            open = true;
            if (blocker) blocker.enabled = false;
            return true;
        }

        private void EnsureRuntimeSetup()
        {
            if (!leftPivot) leftPivot = transform.Find("Left Door Pivot");
            if (!rightPivot) rightPivot = transform.Find("Right Door Pivot");
            if (!blocker)
            {
                Transform blockerTransform = transform.Find($"{name} Solid Blocker");
                if (blockerTransform) blocker = blockerTransform.GetComponent<Collider>();
            }
            if (blocker && blocker.transform.localScale.x < 5.6f)
            {
                Vector3 scale = blocker.transform.localScale;
                blocker.transform.localScale = new Vector3(5.6f, scale.y, Mathf.Max(scale.z, 0.48f));
            }

            if ((!leftPivot || !rightPivot) && TryFindLegacyDoorLeaves(out Transform leftDoor, out Transform rightDoor))
            {
                CreatePivotsForLegacyLeaves(leftDoor, rightDoor);
            }

            CacheClosedRotations();
        }

        private bool TryFindLegacyDoorLeaves(out Transform leftDoor, out Transform rightDoor)
        {
            leftDoor = transform.Find("Left Wood Door");
            rightDoor = transform.Find("Right Wood Door");
            return leftDoor && rightDoor;
        }

        private void CreatePivotsForLegacyLeaves(Transform leftDoor, Transform rightDoor)
        {
            Bounds leftBounds = GetRendererBounds(leftDoor);
            Bounds rightBounds = GetRendererBounds(rightDoor);

            leftPivot = CreatePivot("Left Door Pivot", new Vector3(leftBounds.min.x, leftBounds.center.y, leftBounds.center.z));
            rightPivot = CreatePivot("Right Door Pivot", new Vector3(rightBounds.max.x, rightBounds.center.y, rightBounds.center.z));

            ParentToPivot(leftDoor, leftPivot);
            ParentToPivot(rightDoor, rightPivot);
        }

        private Transform CreatePivot(string pivotName, Vector3 worldPosition)
        {
            GameObject pivot = new GameObject(pivotName);
            Transform pivotTransform = pivot.transform;
            pivotTransform.SetParent(transform, false);
            pivotTransform.position = worldPosition;
            pivotTransform.rotation = transform.rotation;
            return pivotTransform;
        }

        private static void ParentToPivot(Transform doorLeaf, Transform pivot)
        {
            Vector3 worldPosition = doorLeaf.position;
            Quaternion worldRotation = doorLeaf.rotation;
            Vector3 localScale = doorLeaf.localScale;
            doorLeaf.SetParent(pivot, true);
            doorLeaf.position = worldPosition;
            doorLeaf.rotation = worldRotation;
            doorLeaf.localScale = localScale;
        }

        private static Bounds GetRendererBounds(Transform target)
        {
            Renderer renderer = target.GetComponent<Renderer>();
            if (renderer) return renderer.bounds;

            return new Bounds(target.position, target.lossyScale);
        }

        private void CacheClosedRotations()
        {
            if (leftPivot) leftClosedRotation = leftPivot.localRotation;
            if (rightPivot) rightClosedRotation = rightPivot.localRotation;
        }
    }
}
