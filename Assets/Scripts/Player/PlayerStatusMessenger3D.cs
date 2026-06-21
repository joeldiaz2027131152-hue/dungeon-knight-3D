using UnityEngine;

namespace DungeonKnight.Player
{
    internal sealed class PlayerStatusMessenger3D
    {
        private float messageUntil;
        private string message = string.Empty;

        public string Current => Time.time < messageUntil ? message : string.Empty;

        public void Show(string text, float duration)
        {
            message = text;
            messageUntil = Time.time + duration;
        }
    }
}
