using DungeonKnight.Player;
using UnityEngine;

namespace DungeonKnight.Level
{
    public class DungeonInteractable3D : MonoBehaviour
    {
        private enum InteractionKind
        {
            Lore,
            Bonfire,
            Chest,
            Gate,
            TowerGate,
            Door,
            Exit,
            Elevator,
            Npc,
            StarterSwordPickup,
            StarterShieldPickup,
            UnlockedGate
        }

        [SerializeField] private InteractionKind kind;
        [SerializeField] private string text;
        [SerializeField] private int coinReward;
        [SerializeField] private bool used;
        [SerializeField] private Transform linkedObject;
        [SerializeField] private DungeonShortcutElevator3D linkedElevator;
        [SerializeField] private DungeonChestVisual3D chestVisual;
        [SerializeField] private Vector3 destination;
        [SerializeField] private Vector3 destinationFacing = Vector3.forward;
        [SerializeField] private bool upperElevatorSwitch;
        [SerializeField] private bool chestGivesStarterSword;
        [SerializeField] private bool chestGivesStarterShield;
        [SerializeField] private GameObject[] chestRevealObjects;

        private void Awake()
        {
            if (kind == InteractionKind.Chest && !chestVisual)
            {
                chestVisual = DungeonChestVisualFactory3D.EnsureVisual(this);
            }
        }

        public void ConfigureLore(string lore)
        {
            kind = InteractionKind.Lore;
            text = lore;
        }

        public void ConfigureBonfire()
        {
            kind = InteractionKind.Bonfire;
            text = "La hoguera crepita y restaura tus fuerzas.";
        }

        public void ConfigureChest(int coins)
        {
            ConfigureChest(coins, null);
        }

        public void ConfigureChest(int coins, DungeonChestVisual3D visual)
        {
            ConfigureChest(coins, visual, false, false);
        }

        public void ConfigureChest(int coins, DungeonChestVisual3D visual, bool givesStarterSword, bool givesStarterShield)
        {
            ConfigureChest(coins, visual, givesStarterSword, givesStarterShield, null);
        }

        public void ConfigureChest(int coins, DungeonChestVisual3D visual, bool givesStarterSword, bool givesStarterShield, GameObject[] revealObjects)
        {
            kind = InteractionKind.Chest;
            coinReward = coins;
            chestVisual = visual;
            chestGivesStarterSword = givesStarterSword;
            chestGivesStarterShield = givesStarterShield;
            chestRevealObjects = revealObjects;
            text = givesStarterSword || givesStarterShield ? "Equipo encontrado. Recoge cada pieza con E." : "Cofre abierto";
        }

        public void ConfigureGate(Transform gate)
        {
            kind = InteractionKind.Gate;
            linkedObject = gate;
            text = "El porton exige la llave del guardian.";
        }

        public void ConfigureUnlockedGate(Transform gate)
        {
            kind = InteractionKind.UnlockedGate;
            linkedObject = gate;
            text = "El porton se abre.";
        }

        public void ConfigureTowerGate(Transform gate)
        {
            kind = InteractionKind.TowerGate;
            linkedObject = gate;
            text = "La puerta de la torre exige la llave del guardian de la niebla.";
        }

        public void ConfigureDoor(Vector3 newDestination, Vector3 newFacing, string travelText)
        {
            kind = InteractionKind.Door;
            destination = newDestination;
            destinationFacing = newFacing;
            text = travelText;
        }

        public void ConfigureExit()
        {
            kind = InteractionKind.Exit;
            text = "Demo 3D completada. La salida queda lista para el siguiente nivel.";
        }

        public void ConfigureElevator(DungeonShortcutElevator3D elevator, bool fromUpperSwitch)
        {
            kind = InteractionKind.Elevator;
            linkedElevator = elevator;
            upperElevatorSwitch = fromUpperSwitch;
            text = fromUpperSwitch ? "Activar elevador de atajo." : "Usar elevador.";
        }

        public void ConfigureNpc(string dialogue)
        {
            kind = InteractionKind.Npc;
            text = dialogue;
        }

        public void ConfigureStarterSwordPickup()
        {
            kind = InteractionKind.StarterSwordPickup;
            text = "Obtuviste una espada";
        }

        public void ConfigureStarterShieldPickup()
        {
            kind = InteractionKind.StarterShieldPickup;
            text = "Obtuviste un escudo";
        }

        public string GetPrompt(PlayerController3D player)
        {
            switch (kind)
            {
                case InteractionKind.Bonfire:
                    return "E - Descansar en hoguera";
                case InteractionKind.Lore:
                    return "E - Leer";
                case InteractionKind.Chest:
                    return used ? string.Empty : "E - Abrir cofre";
                case InteractionKind.Gate:
                    if (used) return string.Empty;
                    return player && player.HasGateKey ? "E - Abrir porton" : "E - Porton cerrado";
                case InteractionKind.UnlockedGate:
                    return used ? string.Empty : "E - Abrir porton";
                case InteractionKind.TowerGate:
                    if (used) return string.Empty;
                    return player && player.HasTowerKey ? "E - Abrir puerta de torre" : "E - Puerta cerrada";
                case InteractionKind.Door:
                    return "E - Entrar";
                case InteractionKind.Exit:
                    return "E - Salida";
                case InteractionKind.Elevator:
                    return "E - Usar elevador";
                case InteractionKind.Npc:
                    return "E - Hablar";
                case InteractionKind.StarterSwordPickup:
                    return "E - Recoger espada";
                case InteractionKind.StarterShieldPickup:
                    return "E - Recoger escudo";
                default:
                    return "E - Usar";
            }
        }

        public void Interact(PlayerController3D player)
        {
            switch (kind)
            {
                case InteractionKind.Bonfire:
                    player.RestAtBonfire(transform.position);
                    break;
                case InteractionKind.Lore:
                    player.ShowMessage(text, 3.5f);
                    break;
                case InteractionKind.Chest:
                    if (used)
                    {
                        player.ShowMessage("El cofre ya esta vacio.", 1.4f);
                        return;
                    }

                    used = true;
                    player.AddCoins(coinReward);
                    if (chestRevealObjects != null)
                    {
                        foreach (GameObject revealObject in chestRevealObjects)
                        {
                            if (revealObject) revealObject.SetActive(true);
                        }
                    }

                    if (chestVisual) chestVisual.Open();
                    else transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 0.55f, transform.localScale.z);
                    CameraFollow3D.Shake(0.08f, 0.12f);
                    player.ShowMessage(text, 1.8f);
                    break;
                case InteractionKind.Gate:
                    if (!player.HasGateKey)
                    {
                        player.ShowMessage(text, 2f);
                        return;
                    }

                    if (linkedObject)
                    {
                        OpenLinkedGate();
                    }

                    used = true;
                    player.ShowMessage("La llave gira. El porton se abre.", 2f);
                    break;
                case InteractionKind.UnlockedGate:
                    if (linkedObject)
                    {
                        OpenLinkedGate();
                    }

                    used = true;
                    player.ShowMessage(text, 1.7f);
                    break;
                case InteractionKind.TowerGate:
                    if (!player.HasTowerKey)
                    {
                        player.ShowMessage(text, 2.3f);
                        return;
                    }

                    if (linkedObject)
                    {
                        OpenLinkedGate();
                    }

                    used = true;
                    player.ShowMessage("La llave de la torre abre el camino.", 2.2f);
                    CameraFollow3D.Shake(0.16f, 0.25f);
                    break;
                case InteractionKind.Door:
                    player.TeleportTo(destination, destinationFacing, text);
                    break;
                case InteractionKind.Exit:
                    player.ShowMessage(text, 3.2f);
                    break;
                case InteractionKind.Elevator:
                    if (linkedElevator) linkedElevator.TryUse(player, upperElevatorSwitch);
                    else player.ShowMessage("El mecanismo esta roto.", 1.6f);
                    break;
                case InteractionKind.Npc:
                    player.ShowMessage(text, 5f);
                    break;
                case InteractionKind.StarterSwordPickup:
                    player.GetComponent<PlayerInventory>()?.AddStarterSword();
                    player.ShowMessage(text, 1.75f, HudMessageIcon.Sword);
                    gameObject.SetActive(false);
                    break;
                case InteractionKind.StarterShieldPickup:
                    player.GetComponent<PlayerInventory>()?.AddStarterShield();
                    player.ShowMessage(text, 1.75f, HudMessageIcon.Shield);
                    gameObject.SetActive(false);
                    break;
            }
        }

        private void OpenLinkedGate()
        {
            GothicDoubleDoor3D doubleDoor = linkedObject.GetComponent<GothicDoubleDoor3D>();
            if (!doubleDoor) doubleDoor = linkedObject.gameObject.AddComponent<GothicDoubleDoor3D>();

            if (!doubleDoor.Open())
            {
                linkedObject.gameObject.SetActive(false);
            }
        }
    }
}
