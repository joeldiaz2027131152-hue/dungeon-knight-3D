using DungeonKnight.Player;
using UnityEngine;

namespace DungeonKnight.UI
{
    public class GameHud3D : MonoBehaviour
    {
        private PlayerController3D player;
        private GUIStyle titleStyle;
        private GUIStyle smallStyle;
        private GUIStyle messageStyle;

        public void Bind(PlayerController3D target)
        {
            player = target;
        }

        private void OnGUI()
        {
            if (!player) return;

            titleStyle ??= new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.93f, 0.88f, 0.76f) }
            };
            smallStyle ??= new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                normal = { textColor = new Color(0.8f, 0.83f, 0.88f) }
            };
            messageStyle ??= new GUIStyle(titleStyle)
            {
                fontSize = 17,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                normal = { textColor = new Color(1f, 0.87f, 0.46f) }
            };

            GUI.Label(new Rect(22, 18, 240, 28), "Dungeon Knight 3D", titleStyle);
            DrawBar(new Rect(22, 50, 230, 18), player.Health / (float)player.MaxHealth, new Color(0.38f, 0.04f, 0.06f), new Color(0.9f, 0.12f, 0.12f), $"HP {player.Health}/{player.MaxHealth}");
            DrawBar(new Rect(22, 76, 230, 18), player.Stamina / player.MaxStamina, new Color(0.07f, 0.3f, 0.17f), new Color(0.28f, 0.9f, 0.42f), $"ST {Mathf.RoundToInt(player.Stamina)}/{Mathf.RoundToInt(player.MaxStamina)}");
            GUI.Label(new Rect(22, 102, 260, 24), $"Monedas: {player.Coins}   Pociones: {player.Potions}   Llave: {(player.HasGateKey ? "si" : "no")}", smallStyle);

            string status = player.StatusMessage;
            if (!string.IsNullOrEmpty(status))
            {
                Rect box = new Rect(Screen.width * 0.5f - 220f, Screen.height - 118f, 440f, 54f);
                DrawBox(box, new Color(0.03f, 0.035f, 0.05f, 0.78f), new Color(0.72f, 0.54f, 0.26f, 0.8f));
                GUI.Label(new Rect(box.x + 14f, box.y + 7f, box.width - 28f, box.height - 14f), status, messageStyle);
            }

            Rect help = new Rect(18, Screen.height - 38, 760, 26);
            DrawBox(help, new Color(0.035f, 0.04f, 0.055f, 0.78f), new Color(0.52f, 0.42f, 0.24f, 0.72f));
            GUI.Label(
                new Rect(help.x + 12, help.y + 4, help.width - 24, help.height),
                "WASD: moverte   Space: saltar   Tab: fijar enemigo   J: atacar/cargar   K: escudo   L: rodar   E: usar   Q: pocion",
                smallStyle);
        }

        private static void DrawBar(Rect rect, float fill, Color back, Color front, string label)
        {
            DrawBox(rect, new Color(0.02f, 0.02f, 0.028f, 0.9f), new Color(0.52f, 0.42f, 0.24f, 0.75f));
            Color old = GUI.color;
            GUI.color = back;
            GUI.DrawTexture(new Rect(rect.x + 2, rect.y + 2, rect.width - 4, rect.height - 4), Texture2D.whiteTexture);
            GUI.color = front;
            GUI.DrawTexture(new Rect(rect.x + 2, rect.y + 2, (rect.width - 4) * Mathf.Clamp01(fill), rect.height - 4), Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUI.Label(new Rect(rect.x + 8, rect.y - 1, rect.width - 16, rect.height + 4), label);
            GUI.color = old;
        }

        private static void DrawBox(Rect rect, Color fill, Color border)
        {
            Color previous = GUI.color;
            GUI.color = fill;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = border;
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, 1.5f), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.yMax - 1.5f, rect.width, 1.5f), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.y, 1.5f, rect.height), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.xMax - 1.5f, rect.y, 1.5f, rect.height), Texture2D.whiteTexture);
            GUI.color = previous;
        }
    }
}
