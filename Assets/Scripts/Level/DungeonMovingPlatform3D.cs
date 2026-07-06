using UnityEngine;

namespace DungeonKnight.Level
{
    public class DungeonMovingPlatform3D : MonoBehaviour
    {
        [SerializeField] private Vector3 travel = new Vector3(0f, 0f, 4f);
        [SerializeField] private float speed = 1f;
        [SerializeField] private bool startAtLowerEndpoint;
        [SerializeField] private Vector3[] routePoints;
        [SerializeField] private float routeStopDuration = 0.65f;

        private Vector3 startPosition;
        private BoxCollider platformCollider;
        private float phaseOffset;
        private int routeTargetIndex = 1;
        private float routeStopTimer;
        private readonly Collider[] riderHits = new Collider[8];
        private readonly CharacterController[] riders = new CharacterController[4];

        public void Configure(Vector3 newTravel, float newSpeed)
        {
            Configure(newTravel, newSpeed, false);
        }

        public void Configure(Vector3 newTravel, float newSpeed, bool newStartAtLowerEndpoint)
        {
            travel = newTravel;
            speed = newSpeed;
            startAtLowerEndpoint = newStartAtLowerEndpoint;
            routePoints = null;
        }

        public void ConfigureRoute(Vector3[] newRoutePoints, float newSpeed)
        {
            ConfigureRoute(newRoutePoints, newSpeed, routeStopDuration);
        }

        public void ConfigureRoute(Vector3[] newRoutePoints, float newSpeed, float newRouteStopDuration)
        {
            routePoints = newRoutePoints;
            speed = newSpeed;
            startAtLowerEndpoint = false;
            routeStopDuration = Mathf.Max(0f, newRouteStopDuration);
            routeTargetIndex = 1;
            routeStopTimer = 0f;
        }

        private void Awake()
        {
            if (HasRoute)
            {
                transform.position = routePoints[0];
            }

            startPosition = startAtLowerEndpoint ? transform.position + travel * 0.5f : transform.position;
            phaseOffset = startAtLowerEndpoint ? -Mathf.PI * 0.5f : 0f;
            platformCollider = GetComponent<BoxCollider>();
        }

        private void Update()
        {
            Bounds previousBounds = platformCollider ? platformCollider.bounds : new Bounds(transform.position, transform.lossyScale);
            Vector3 previousPosition = transform.position;
            Vector3 nextPosition = HasRoute ? CalculateRoutePosition(previousPosition) : CalculateTravelPosition();
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

        private bool HasRoute => routePoints != null && routePoints.Length > 1;

        private Vector3 CalculateTravelPosition()
        {
            float t = (Mathf.Sin(Time.time * speed + phaseOffset) + 1f) * 0.5f;
            return Vector3.Lerp(startPosition - travel * 0.5f, startPosition + travel * 0.5f, t);
        }

        private Vector3 CalculateRoutePosition(Vector3 previousPosition)
        {
            if (routeStopTimer > 0f)
            {
                routeStopTimer = Mathf.Max(0f, routeStopTimer - Time.deltaTime);
                return previousPosition;
            }

            routeTargetIndex = Mathf.Clamp(routeTargetIndex, 0, routePoints.Length - 1);
            Vector3 target = routePoints[routeTargetIndex];
            Vector3 nextPosition = Vector3.MoveTowards(previousPosition, target, Mathf.Max(0.05f, speed) * Time.deltaTime);

            if ((nextPosition - target).sqrMagnitude <= 0.0004f)
            {
                routeTargetIndex = (routeTargetIndex + 1) % routePoints.Length;
                routeStopTimer = routeStopDuration;
            }

            return nextPosition;
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
