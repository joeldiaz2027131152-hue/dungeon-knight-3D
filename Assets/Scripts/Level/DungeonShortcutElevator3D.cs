using DungeonKnight.Player;
using UnityEngine;

namespace DungeonKnight.Level
{
    public class DungeonShortcutElevator3D : MonoBehaviour
    {
        [SerializeField] private Transform platform;
        [SerializeField] private Vector3 lowerPosition;
        [SerializeField] private Vector3 upperPosition;
        [SerializeField] private float rideDuration = 2.4f;

        private PlayerController3D passenger;
        private Vector3 startPosition;
        private Vector3 targetPosition;
        private float rideTimer;
        private bool unlocked;
        private bool moving;
        private bool atUpper;

        public void Configure(Transform elevatorPlatform, Vector3 lower, Vector3 upper, float duration)
        {
            platform = elevatorPlatform;
            lowerPosition = lower;
            upperPosition = upper;
            rideDuration = Mathf.Max(0.4f, duration);
            if (platform) platform.position = lowerPosition;
        }

        public void TryUse(PlayerController3D player, bool fromUpperSwitch)
        {
            if (!platform || moving) return;

            if (!unlocked && !fromUpperSwitch)
            {
                player.ShowMessage("El elevador no responde. Hay que activarlo desde arriba.", 2.3f);
                return;
            }

            if (!unlocked && fromUpperSwitch)
            {
                unlocked = true;
                atUpper = true;
                platform.position = upperPosition;
                player.ShowMessage("Atajo desbloqueado. El elevador ya responde desde ambos lados.", 2.6f);
            }

            passenger = player;
            startPosition = platform.position;
            targetPosition = atUpper ? lowerPosition : upperPosition;
            rideTimer = 0f;
            moving = true;
        }

        private void Update()
        {
            if (!platform || !moving) return;

            rideTimer += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(rideTimer / rideDuration));
            platform.position = Vector3.Lerp(startPosition, targetPosition, t);

            if (passenger)
            {
                passenger.TeleportTo(platform.position + Vector3.up * 1.05f, passenger.FacingDirection, string.Empty);
            }

            if (t < 1f) return;

            moving = false;
            atUpper = Vector3.Distance(platform.position, upperPosition) < 0.2f;
            if (passenger)
            {
                passenger.ShowMessage(atUpper ? "El elevador sube hasta la torre." : "El elevador baja hasta la entrada.", 1.7f);
            }

            passenger = null;
        }
    }
}
