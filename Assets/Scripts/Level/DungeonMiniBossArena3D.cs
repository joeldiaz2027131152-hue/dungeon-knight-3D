using DungeonKnight.Player;
using UnityEngine;

namespace DungeonKnight.Level
{
    public class DungeonMiniBossArena3D : MonoBehaviour
    {
        [SerializeField] private DungeonEnemy3D boss;
        [SerializeField] private GameObject entrySeal;
        [SerializeField] private string enterMessage = "La niebla se cierra. El guardian de la torre despierta.";
        [SerializeField] private string clearMessage = "El guardian cae. La salida hacia la torre puede abrirse.";

        private bool started;
        private bool cleared;

        public void Configure(DungeonEnemy3D arenaBoss, GameObject seal)
        {
            boss = arenaBoss;
            entrySeal = seal;
            if (entrySeal) entrySeal.SetActive(false);
        }

        private void Update()
        {
            if (!started || cleared) return;

            PlayerController3D player = Object.FindAnyObjectByType<PlayerController3D>();
            if (player && player.Health <= 0)
            {
                started = false;
                if (entrySeal) entrySeal.SetActive(false);
                if (boss && boss.IsAlive) boss.RestoreAtBonfire();
                return;
            }

            if (boss && boss.IsAlive) return;

            cleared = true;
            if (entrySeal) entrySeal.SetActive(false);

            if (player) player.ShowMessage(clearMessage, 3f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (started || cleared) return;

            PlayerController3D player = other.GetComponentInParent<PlayerController3D>();
            if (!player) return;

            started = true;
            if (entrySeal) entrySeal.SetActive(true);
            player.ShowMessage(enterMessage, 3f);
            CameraFollow3D.Shake(0.18f, 0.28f);
        }
    }
}
