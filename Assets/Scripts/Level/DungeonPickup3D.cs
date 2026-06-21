using DungeonKnight.Player;
using UnityEngine;

namespace DungeonKnight.Level
{
    public class DungeonPickup3D : MonoBehaviour
    {
        private enum PickupKind
        {
            Coin,
            Potion
        }

        [SerializeField] private PickupKind kind;
        [SerializeField] private int amount = 1;
        [SerializeField] private float spinSpeed = 95f;

        public void ConfigureCoin(int coinAmount)
        {
            kind = PickupKind.Coin;
            amount = coinAmount;
        }

        public void ConfigurePotion()
        {
            kind = PickupKind.Potion;
            amount = 1;
        }

        private void Update()
        {
            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerController3D player = other.GetComponentInParent<PlayerController3D>();
            if (!player) return;

            if (kind == PickupKind.Coin) player.AddCoins(amount);
            else player.AddPotion();

            Destroy(gameObject);
        }
    }
}
