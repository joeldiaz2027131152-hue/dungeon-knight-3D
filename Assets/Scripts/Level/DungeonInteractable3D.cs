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
            Npc
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
            kind = InteractionKind.Chest;
            coinReward = coins;
            chestVisual = visual;
            text = "Cofre abierto";
        }

        public void ConfigureGate(Transform gate)
        {
            kind = InteractionKind.Gate;
            linkedObject = gate;
            text = "El porton exige la llave del guardian.";
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
                        linkedObject.gameObject.SetActive(false);
                    }

                    player.ShowMessage("La llave gira. El porton se abre.", 2f);
                    break;
                case InteractionKind.TowerGate:
                    if (!player.HasTowerKey)
                    {
                        player.ShowMessage(text, 2.3f);
                        return;
                    }

                    if (linkedObject)
                    {
                        linkedObject.gameObject.SetActive(false);
                    }

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
            }
        }
    }
}
