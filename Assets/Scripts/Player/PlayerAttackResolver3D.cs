using DungeonKnight.Level;
using UnityEngine;

namespace DungeonKnight.Player
{
    internal sealed class PlayerAttackResolver3D
    {
        private readonly Collider[] attackHits = new Collider[12];

        public bool TryHitEnemies(Vector3 center, float radius, int damage, Vector3 attackerPosition)
        {
            int count = Physics.OverlapSphereNonAlloc(center, radius, attackHits);
            bool hitSomething = false;

            for (int i = 0; i < count; i++)
            {
                DungeonEnemy3D enemy = attackHits[i].GetComponentInParent<DungeonEnemy3D>();
                if (!enemy) continue;

                enemy.TakeDamage(damage, attackerPosition);
                hitSomething = true;
            }

            return hitSomething;
        }
    }
}
