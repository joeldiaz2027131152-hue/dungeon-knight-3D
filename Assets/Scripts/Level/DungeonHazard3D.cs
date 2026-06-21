using DungeonKnight.Player;
using UnityEngine;

namespace DungeonKnight.Level
{
    public class DungeonHazard3D : MonoBehaviour
    {
        [SerializeField] private int damage = 12;
        [SerializeField] private float cooldown = 0.8f;
        [SerializeField] private string hitMessage = "El calabozo muerde.";
        [SerializeField] private Vector3 rotationPerSecond;
        [SerializeField] private Vector3 bobAmplitude;
        [SerializeField] private float bobSpeed = 1f;

        private Vector3 startPosition;
        private float nextHitTime;

        public void Configure(int newDamage, string newMessage, Vector3 spin, Vector3 bob, float speed)
        {
            damage = newDamage;
            hitMessage = newMessage;
            rotationPerSecond = spin;
            bobAmplitude = bob;
            bobSpeed = speed;
        }

        private void Awake()
        {
            startPosition = transform.position;
        }

        private void Update()
        {
            if (rotationPerSecond != Vector3.zero)
            {
                transform.Rotate(rotationPerSecond * Time.deltaTime, Space.Self);
            }

            if (bobAmplitude != Vector3.zero)
            {
                transform.position = startPosition + bobAmplitude * Mathf.Sin(Time.time * bobSpeed);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (Time.time < nextHitTime) return;

            PlayerController3D player = other.GetComponentInParent<PlayerController3D>();
            if (!player) return;

            player.TakeDamage(damage);
            player.ShowMessage(hitMessage, 1.1f);
            nextHitTime = Time.time + cooldown;
        }
    }
}
