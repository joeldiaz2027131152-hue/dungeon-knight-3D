using DungeonKnight.Player;
using UnityEngine;

namespace DungeonKnight.Level
{
    public class PlayerModelVisual3D : MonoBehaviour
    {
        private PlayerController3D controller;
        private Animator animator;

        public void Configure(PlayerController3D playerController, Animator modelAnimator)
        {
            controller = playerController;
            animator = modelAnimator;
        }

        private void LateUpdate()
        {
            if (!controller || !animator) return;

            animator.speed = controller.Health <= 0 ? 0f : 1f;
        }
    }
}
