using DungeonKnight.Level;
using UnityEngine;

namespace DungeonKnight.Player
{
    internal sealed class PlayerInteractionScanner3D
    {
        private readonly Collider[] interactHits = new Collider[8];

        public DungeonInteractable3D FindClosest(Vector3 origin, Vector3 playerPosition, float radius)
        {
            int count = Physics.OverlapSphereNonAlloc(origin, radius, interactHits);
            DungeonInteractable3D closest = null;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                DungeonInteractable3D interactable = interactHits[i].GetComponentInParent<DungeonInteractable3D>();
                if (!interactable) continue;

                float distance = Vector3.Distance(playerPosition, interactable.transform.position);
                if (distance >= closestDistance) continue;

                closest = interactable;
                closestDistance = distance;
            }

            return closest;
        }
    }
}
