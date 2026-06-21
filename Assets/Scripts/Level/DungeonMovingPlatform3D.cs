using UnityEngine;

namespace DungeonKnight.Level
{
    public class DungeonMovingPlatform3D : MonoBehaviour
    {
        [SerializeField] private Vector3 travel = new Vector3(0f, 0f, 4f);
        [SerializeField] private float speed = 1f;

        private Vector3 startPosition;

        public void Configure(Vector3 newTravel, float newSpeed)
        {
            travel = newTravel;
            speed = newSpeed;
        }

        private void Awake()
        {
            startPosition = transform.position;
        }

        private void Update()
        {
            float t = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;
            transform.position = Vector3.Lerp(startPosition - travel * 0.5f, startPosition + travel * 0.5f, t);
        }
    }
}
