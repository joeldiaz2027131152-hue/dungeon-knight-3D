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
            Door,
            Exit
        }

        [SerializeField] private InteractionKind kind;
        [SerializeField] private string text;
        [SerializeField] private int coinReward;
        [SerializeField] private bool used;
        [SerializeField] private Transform linkedObject;
        private Vector3 doorDestination;
        private Vector3 doorFacing = Vector3.forward;

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
            kind = InteractionKind.Chest;
            coinReward = coins;
            text = "Cofre abierto";
        }

        public void ConfigureGate(Transform gate)
        {
            kind = InteractionKind.Gate;
            linkedObject = gate;
            text = "El porton exige la llave del guardian.";
        }

        public void ConfigureDoor(Vector3 destination, Vector3 facing, string travelText)
        {
            kind = InteractionKind.Door;
            doorDestination = destination;
            doorFacing = facing.sqrMagnitude > 0.001f ? facing.normalized : Vector3.forward;
            text = travelText;
        }

        public void ConfigureExit()
        {
            kind = InteractionKind.Exit;
            text = "Demo 3D completada. La salida queda lista para el siguiente nivel.";
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
                    transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 0.55f, transform.localScale.z);
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
                case InteractionKind.Door:
                    player.transform.position = doorDestination;
                    player.transform.rotation = Quaternion.LookRotation(doorFacing, Vector3.up);
                    player.ShowMessage(text, 2f);
                    break;
                case InteractionKind.Exit:
                    player.ShowMessage(text, 3.2f);
                    break;
            }
        }
    }
}
