using UnityEngine;

namespace DungeonKnight.Player
{
    public enum EquipmentItemType
    {
        Empty,
        Sword,
        Shield
    }

    public enum SwordKind
    {
        Knight
    }

    public enum ShieldKind
    {
        Steel,
        Tower
    }

    public class PlayerInventory : MonoBehaviour
    {
        public const int EquipmentSlotCount = 10;

        private readonly EquipmentItem[] weaponSlots =
        {
            EquipmentItem.Empty,
            EquipmentItem.Empty,
            EquipmentItem.Empty,
            EquipmentItem.Empty,
            EquipmentItem.Empty,
            EquipmentItem.Empty,
            EquipmentItem.Empty,
            EquipmentItem.Empty,
            EquipmentItem.Empty,
            EquipmentItem.Empty
        };

        private readonly EquipmentItem[] shieldEquipmentSlots =
        {
            EquipmentItem.Empty,
            EquipmentItem.Empty,
            EquipmentItem.Empty,
            EquipmentItem.Empty,
            EquipmentItem.Empty,
            EquipmentItem.Empty,
            EquipmentItem.Empty,
            EquipmentItem.Empty,
            EquipmentItem.Empty,
            EquipmentItem.Empty
        };

        public int Coins { get; private set; }
        public int MinorPotions { get; private set; } = 1;
        public int MaxMinorPotions { get; private set; } = 3;
        public bool HasGateKey { get; private set; }
        public int EquippedShieldSlot { get; private set; } = -1;
        public int EquippedWeaponSlot { get; private set; } = -1;
        public int EquippedShieldInventorySlot { get; private set; } = -1;
        public ShieldKind EquippedShield => EquippedShieldItem.Shield;
        public bool HasTowerShield => shieldEquipmentSlots[1].Type == EquipmentItemType.Shield;
        public bool IsTowerShieldEquipped => EquippedShield == ShieldKind.Tower;
        public string EquippedShieldName => EquippedShieldItem.IsEmpty ? "Sin escudo" : GetShieldName(EquippedShieldSlot);
        public EquipmentItem EquippedWeapon => GetWeaponItem(EquippedWeaponSlot);
        public EquipmentItem EquippedShieldItem => GetShieldItem(EquippedShieldInventorySlot);
        public string EquippedWeaponName => EquippedWeapon.IsEmpty ? "Sin arma" : EquippedWeapon.Name;
        public int LightAttackDamage => Mathf.Max(1, EquippedWeapon.LightDamage);
        public int ChargedAttackDamage => Mathf.Max(LightAttackDamage, EquippedWeapon.ChargedDamage);
        public float BlockStaminaMultiplier => Mathf.Clamp(EquippedShieldItem.BlockStaminaMultiplier, 0.35f, 1.25f);

        public void AddCoins(int amount)
        {
            Coins += Mathf.Max(0, amount);
            Debug.Log($"Coins: {Coins}");
        }

        public void UnlockLateGamePotionBag()
        {
            MaxMinorPotions = 5;
        }

        public bool AddMinorPotion()
        {
            if (MinorPotions >= MaxMinorPotions) return false;
            MinorPotions++;
            return true;
        }

        public bool ConsumeMinorPotion()
        {
            if (MinorPotions <= 0) return false;
            MinorPotions--;
            return true;
        }

        public void AddGateKey()
        {
            HasGateKey = true;
            Debug.Log("Gate key collected.");
        }

        public void AddTowerShield()
        {
            shieldEquipmentSlots[1] = new EquipmentItem(EquipmentItemType.Shield, "Escudo Torre", "Consume menos stamina", 0, 0, 0.58f, SwordKind.Knight, ShieldKind.Tower);
            EquipShieldSlot(1);
            Debug.Log("Tower shield collected.");
        }

        public void AddStarterSword()
        {
            weaponSlots[0] = new EquipmentItem(EquipmentItemType.Sword, "Espada de Caballero", "Corte balanceado", 20, 44, 1f, SwordKind.Knight, ShieldKind.Steel);
            Debug.Log("Starter sword collected.");
        }

        public void AddStarterShield()
        {
            shieldEquipmentSlots[0] = new EquipmentItem(EquipmentItemType.Shield, "Escudo de Acero", "Bloqueo estable", 0, 0, 1f, SwordKind.Knight, ShieldKind.Steel);
            Debug.Log("Starter shield collected.");
        }

        public EquipmentItem GetWeaponItem(int slot)
        {
            if (slot < 0 || slot >= weaponSlots.Length) return EquipmentItem.Empty;
            return weaponSlots[slot];
        }

        public EquipmentItem GetShieldItem(int slot)
        {
            if (slot < 0 || slot >= shieldEquipmentSlots.Length) return EquipmentItem.Empty;
            return shieldEquipmentSlots[slot];
        }

        public bool EquipWeaponSlot(int slot)
        {
            EquipmentItem item = GetWeaponItem(slot);
            if (item.IsEmpty) return false;
            EquippedWeaponSlot = slot;
            return true;
        }

        public bool EquipShieldSlot(int slot)
        {
            EquipmentItem item = GetShieldItem(slot);
            if (item.IsEmpty) return false;
            EquippedShieldInventorySlot = slot;
            EquippedShieldSlot = slot;
            return true;
        }

        public bool IsEquippedWeaponSlot(int slot)
        {
            return slot == EquippedWeaponSlot;
        }

        public bool IsEquippedShieldSlot(int slot)
        {
            return slot == EquippedShieldInventorySlot;
        }

        public bool HasShieldAt(int slot)
        {
            return !GetShieldItem(slot).IsEmpty;
        }

        public string GetShieldName(int slot)
        {
            if (!HasShieldAt(slot)) return "Vacio";
            return GetShieldItem(slot).Shield == ShieldKind.Tower ? "Torre" : "Acero";
        }

        public string GetShieldValue(int slot)
        {
            if (!HasShieldAt(slot)) return "";
            return slot == EquippedShieldSlot ? "USO" : "x1";
        }

        public bool SelectShieldSlot(int slot)
        {
            return EquipShieldSlot(slot);
        }

        public readonly struct EquipmentItem
        {
            public static readonly EquipmentItem Empty = new EquipmentItem(EquipmentItemType.Empty, string.Empty, string.Empty, 0, 0, 1f, SwordKind.Knight, ShieldKind.Steel);

            public readonly EquipmentItemType Type;
            public readonly string Name;
            public readonly string Description;
            public readonly int LightDamage;
            public readonly int ChargedDamage;
            public readonly float BlockStaminaMultiplier;
            public readonly SwordKind Sword;
            public readonly ShieldKind Shield;

            public EquipmentItem(EquipmentItemType type, string name, string description, int lightDamage, int chargedDamage, float blockStaminaMultiplier, SwordKind sword, ShieldKind shield)
            {
                Type = type;
                Name = name;
                Description = description;
                LightDamage = lightDamage;
                ChargedDamage = chargedDamage;
                BlockStaminaMultiplier = blockStaminaMultiplier;
                Sword = sword;
                Shield = shield;
            }

            public bool IsEmpty => Type == EquipmentItemType.Empty;
        }
    }
}
