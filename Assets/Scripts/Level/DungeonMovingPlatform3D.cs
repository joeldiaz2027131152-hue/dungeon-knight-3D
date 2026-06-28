using UnityEngine;

namespace DungeonKnight.Level
{
    public class DungeonMovingPlatform3D : MonoBehaviour
    {
        [SerializeField] private Vector3 travel = new Vector3(0f, 0f, 4f);
        [SerializeField] private float speed = 1f;

        private Vector3 startPosition;
        private BoxCollider platformCollider;
        private readonly Collider[] riderHits = new Collider[8];
        private readonly CharacterController[] riders = new CharacterController[4];

        public void Configure(Vector3 newTravel, float newSpeed)
        {
            travel = newTravel;
            speed = newSpeed;
        }

        private void Awake()
        {
            startPosition = transform.position;
            platformCollider = GetComponent<BoxCollider>();
        }

        private void Update()
        {
            Bounds previousBounds = platformCollider ? platformCollider.bounds : new Bounds(transform.position, transform.lossyScale);
            Vector3 previousPosition = transform.position;
            float t = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;
            Vector3 nextPosition = Vector3.Lerp(startPosition - travel * 0.5f, startPosition + travel * 0.5f, t);
            Vector3 delta = nextPosition - previousPosition;
            int riderCount = delta.sqrMagnitude > 0.000001f ? CollectRiders(previousBounds) : 0;

            transform.position = nextPosition;

            for (int i = 0; i < riderCount; i++)
            {
                if (riders[i] && riders[i].enabled)
                {
                    riders[i].Move(delta);
                    riders[i] = null;
                }
            }
        }

        private int CollectRiders(Bounds platformBounds)
        {
            Vector3 center = new Vector3(platformBounds.center.x, platformBounds.max.y + 0.3f, platformBounds.center.z);
            Vector3 halfExtents = new Vector3(platformBounds.extents.x + 0.35f, 0.38f, platformBounds.extents.z + 0.35f);
            int hitCount = Physics.OverlapBoxNonAlloc(center, halfExtents, riderHits, Quaternion.identity, ~0, QueryTriggerInteraction.Ignore);
            int riderCount = 0;

            for (int i = 0; i < hitCount && riderCount < riders.Length; i++)
            {
                CharacterController controller = riderHits[i].GetComponentInParent<CharacterController>();
                riderHits[i] = null;
                if (!controller || !controller.enabled || !IsStandingOnPlatform(controller, platformBounds)) continue;

                bool alreadyAdded = false;
                for (int j = 0; j < riderCount; j++)
                {
                    if (riders[j] == controller)
                    {
                        alreadyAdded = true;
                        break;
                    }
                }

                if (!alreadyAdded)
                {
                    riders[riderCount++] = controller;
                }
            }

            return riderCount;
        }

        private static bool IsStandingOnPlatform(CharacterController controller, Bounds platformBounds)
        {
            Vector3 position = controller.transform.position;
            float bottom = position.y + controller.center.y - controller.height * 0.5f;
            if (bottom < platformBounds.max.y - 0.12f || bottom > platformBounds.max.y + 0.5f) return false;

            float radius = controller.radius + 0.18f;
            bool insideX = position.x >= platformBounds.min.x - radius && position.x <= platformBounds.max.x + radius;
            bool insideZ = position.z >= platformBounds.min.z - radius && position.z <= platformBounds.max.z + radius;
            return insideX && insideZ;
        }
    }
}
