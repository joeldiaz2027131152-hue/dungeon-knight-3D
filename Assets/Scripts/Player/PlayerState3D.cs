using DungeonKnight.Level;
using UnityEngine;

namespace DungeonKnight.Player
{
    internal sealed class PlayerState3D
    {
        public int MaxHealth { get; private set; } = 120;
        public int Health { get; private set; } = 120;
        public int Coins { get; private set; }
        public int Potions { get; private set; } = 2;
        public bool HasGateKey { get; private set; }
        public Vector3 RespawnPoint { get; private set; } = DungeonKnight3DBootstrap.PlayerSpawn;

        public bool IsDead => Health <= 0;

        public int AddCoins(int amount)
        {
            int awarded = Mathf.Max(0, amount);
            Coins += awarded;
            return awarded;
        }

        public void AddPotion()
        {
            Potions = Mathf.Min(5, Potions + 1);
        }

        public void RestAtBonfire(Vector3 bonfirePosition)
        {
            RespawnPoint = bonfirePosition + Vector3.back * 1.35f + Vector3.up * 0.6f;
            RestoreVitals();
            Potions = Mathf.Max(Potions, 2);
        }

        public void GiveGateKey()
        {
            HasGateKey = true;
        }

        public int TakeDamage(int amount, bool isBlocking)
        {
            if (IsDead) return 0;

            int finalAmount = isBlocking ? Mathf.CeilToInt(amount * 0.35f) : amount;
            Health = Mathf.Max(0, Health - finalAmount);
            return finalAmount;
        }

        public bool UsePotion()
        {
            if (Potions <= 0 || Health >= MaxHealth) return false;

            Potions--;
            Health = Mathf.Min(MaxHealth, Health + 45);
            return true;
        }

        public void RestoreVitals()
        {
            Health = MaxHealth;
        }
    }
}
