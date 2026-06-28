using DungeonKnight.Player;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonKnight.UI
{
    public class GameHud3D : MonoBehaviour
    {
        private PlayerController3D player;
        private PlayerInventory inventory;
        [SerializeField] private bool useImmediateModeHud;
        private bool inventoryOpen;
        private EquipmentListMode listMode;
        private int hoveredSlot = -1;
        private GUIStyle titleStyle;
        private GUIStyle smallStyle;
        private GUIStyle messageStyle;
        private GUIStyle slotTitleStyle;
        private GUIStyle statStyle;
        private Canvas overlayCanvas;
        private Text titleText;
        private Text statsText;
        private Text hpText;
        private Text staminaText;
        private Text equipmentText;
        private Text helpText;
        private Text messageText;
        private Text messageIconText;
        private Image hpFill;
        private Image staminaFill;
        private Image equipmentPanel;
        private Image helpPanel;
        private Image messagePanel;
        private Transform worldHudRoot;
        private TextMesh worldTitleText;
        private TextMesh worldStatsText;
        private TextMesh worldEquipmentText;
        private TextMesh worldMessageText;
        private TextMesh worldHelpText;
        private Transform worldHpFill;
        private Transform worldStaminaFill;
        private Material worldPanelMaterial;
        private Material worldHpMaterial;
        private Material worldStaminaMaterial;
        private Material worldTextMaterial;

        public void Bind(PlayerController3D target)
        {
            player = target;
            inventory = player ? player.GetComponent<PlayerInventory>() : null;
        }

        private void Awake()
        {
            EnsureOverlayCanvas();
        }

        private void Update()
        {
            if (!player)
            {
                Bind(Object.FindAnyObjectByType<PlayerController3D>());
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                inventoryOpen = !inventoryOpen;
                if (!inventoryOpen) listMode = EquipmentListMode.None;
            }

            UpdateOverlayCanvas();
            HideWorldHud();
        }

        private void OnGUI()
        {
            if (!player)
            {
                Bind(Object.FindAnyObjectByType<PlayerController3D>());
            }

            if (!player) return;
            if (!inventory) inventory = player.GetComponent<PlayerInventory>();

            titleStyle ??= new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.93f, 0.88f, 0.76f) }
            };
            smallStyle ??= new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                wordWrap = true,
                normal = { textColor = new Color(0.8f, 0.83f, 0.88f) }
            };
            messageStyle ??= new GUIStyle(titleStyle)
            {
                fontSize = 15,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                normal = { textColor = new Color(1f, 0.87f, 0.46f) }
            };
            slotTitleStyle ??= new GUIStyle(titleStyle)
            {
                fontSize = 13,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                normal = { textColor = new Color(0.9f, 0.88f, 0.8f) }
            };
            statStyle ??= new GUIStyle(smallStyle)
            {
                fontSize = 11,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.67f, 0.7f, 0.76f) }
            };

            if (!useImmediateModeHud)
            {
                if (inventoryOpen)
                {
                    DrawInventoryPanel();
                }

                return;
            }

            const float hudX = 34f;
            const float hudY = 18f;
            const float hudWidth = 230f;
            GUI.Label(new Rect(hudX, hudY, hudWidth, 24f), "Dungeon Knight 3D", titleStyle);
            DrawBar(new Rect(hudX, hudY + 30f, hudWidth, 15f), player.Health / (float)player.MaxHealth, new Color(0.38f, 0.04f, 0.06f), new Color(0.9f, 0.12f, 0.12f), $"HP {player.Health}/{player.MaxHealth}");
            DrawBar(new Rect(hudX, hudY + 51f, hudWidth, 15f), player.Stamina / player.MaxStamina, new Color(0.07f, 0.3f, 0.17f), new Color(0.28f, 0.9f, 0.42f), $"ST {Mathf.RoundToInt(player.Stamina)}/{Mathf.RoundToInt(player.MaxStamina)}");
            GUI.Label(new Rect(hudX, hudY + 72f, 520f, 22f), $"Almas: {player.Coins}   Pociones: {player.Potions}   Porton: {(player.HasGateKey ? "si" : "no")}   Torre: {(player.HasTowerKey ? "si" : "no")}   Lock-on: {(player.HasLockOn ? "si" : "no")}", smallStyle);

            if (player.LockOnTarget)
            {
                Rect targetBox = new Rect(Screen.width * 0.5f - 150f, 22f, 300f, 56f);
                DrawBox(targetBox, new Color(0.025f, 0.02f, 0.025f, 0.82f), new Color(0.7f, 0.48f, 0.22f, 0.75f));
                GUI.Label(new Rect(targetBox.x + 12f, targetBox.y + 5f, targetBox.width - 24f, 18f), "Objetivo fijado", smallStyle);
                DrawBar(new Rect(targetBox.x + 12f, targetBox.y + 25f, targetBox.width - 24f, 10f), player.LockOnTarget.HealthFraction, new Color(0.24f, 0.03f, 0.04f), new Color(0.84f, 0.1f, 0.1f), "");
                DrawBar(new Rect(targetBox.x + 12f, targetBox.y + 39f, targetBox.width - 24f, 9f), player.LockOnTarget.PostureFraction, new Color(0.22f, 0.18f, 0.07f), new Color(0.95f, 0.68f, 0.18f), "");
            }

            string status = player.StatusMessage;
            string prompt = player.CurrentInteractionPrompt;
            string centerMessage = !string.IsNullOrEmpty(status) ? status : prompt;
            if (!string.IsNullOrEmpty(centerMessage))
            {
                float messageWidth = Mathf.Min(460f, Screen.width - 48f);
                Rect box = new Rect(Screen.width * 0.5f - messageWidth * 0.5f, Screen.height - 92f, messageWidth, 52f);
                DrawBox(box, new Color(0.03f, 0.035f, 0.05f, 0.78f), new Color(0.72f, 0.54f, 0.26f, 0.8f));
                HudMessageIcon icon = !string.IsNullOrEmpty(status) ? player.StatusMessageIcon : HudMessageIcon.None;
                float textInset = icon == HudMessageIcon.None ? 14f : 70f;
                if (icon != HudMessageIcon.None)
                {
                    DrawMessageIcon(new Rect(box.x + 19f, box.y + 9f, 38f, 36f), icon);
                }

                GUI.Label(new Rect(box.x + textInset, box.y + 7f, box.width - textInset - 14f, box.height - 14f), centerMessage, messageStyle);
            }

            if (inventoryOpen)
            {
                DrawInventoryPanel();
            }
        }

        private void HideWorldHud()
        {
            if (worldHudRoot)
            {
                worldHudRoot.gameObject.SetActive(false);
            }
        }

        private void HideOverlayCanvas()
        {
            if (overlayCanvas)
            {
                overlayCanvas.enabled = false;
            }
        }

        private void EnsureOverlayCanvas()
        {
            if (overlayCanvas && titleText && hpFill && staminaFill && hpText && staminaText && statsText && messagePanel && messageText) return;

            if (overlayCanvas)
            {
                Destroy(overlayCanvas.gameObject);
            }

            GameObject canvasObject = new GameObject("Runtime HUD Canvas");
            overlayCanvas = canvasObject.AddComponent<Canvas>();
            overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            overlayCanvas.sortingOrder = 5000;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObject.AddComponent<GraphicRaycaster>();

            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (!font) font = Resources.GetBuiltinResource<Font>("Arial.ttf");

            titleText = CreateText("Title", overlayCanvas.transform, font, 18, FontStyle.Bold, new Color(0.93f, 0.88f, 0.76f), TextAnchor.MiddleLeft);
            SetRect(titleText.rectTransform, new Vector2(22f, -18f), new Vector2(260f, 26f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f));

            hpFill = CreateBar("HP Bar", new Vector2(22f, -50f), new Color(0.9f, 0.12f, 0.12f));
            hpText = CreateText("HP Text", hpFill.transform.parent, font, 13, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter);
            SetRect(hpText.rectTransform, Vector2.zero, new Vector2(300f, 20f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));

            staminaFill = CreateBar("Stamina Bar", new Vector2(22f, -78f), new Color(0.28f, 0.9f, 0.42f));
            staminaText = CreateText("Stamina Text", staminaFill.transform.parent, font, 13, FontStyle.Bold, Color.white, TextAnchor.MiddleCenter);
            SetRect(staminaText.rectTransform, Vector2.zero, new Vector2(300f, 20f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));

            statsText = CreateText("Stats", overlayCanvas.transform, font, 12, FontStyle.Normal, new Color(0.8f, 0.83f, 0.88f), TextAnchor.MiddleLeft);
            SetRect(statsText.rectTransform, new Vector2(22f, -108f), new Vector2(560f, 24f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f));

            equipmentPanel = CreatePanel("Equipment Panel", overlayCanvas.transform, new Color(0.02f, 0.023f, 0.03f, 0.72f));
            SetRect(equipmentPanel.rectTransform, new Vector2(22f, -118f), new Vector2(250f, 58f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f));
            equipmentText = CreateText("Equipment", equipmentPanel.transform, font, 12, FontStyle.Normal, new Color(0.82f, 0.84f, 0.88f), TextAnchor.UpperLeft);
            SetRect(equipmentText.rectTransform, new Vector2(0f, 0f), new Vector2(250f, 58f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));

            messagePanel = CreatePanel("Message Panel", overlayCanvas.transform, new Color(0.03f, 0.035f, 0.05f, 0.82f));
            SetRect(messagePanel.rectTransform, new Vector2(0f, 58f), new Vector2(560f, 58f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));
            messageText = CreateText("Message Text", messagePanel.transform, font, 16, FontStyle.Bold, new Color(1f, 0.87f, 0.46f), TextAnchor.MiddleCenter);
            SetRect(messageText.rectTransform, new Vector2(0f, 0f), new Vector2(530f, 48f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            messageIconText = CreateText("Message Icon", messagePanel.transform, font, 11, FontStyle.Bold, new Color(0.95f, 0.8f, 0.35f), TextAnchor.MiddleCenter);
            SetRect(messageIconText.rectTransform, new Vector2(34f, 0f), new Vector2(48f, 36f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0.5f, 0.5f));

            helpPanel = CreatePanel("Help Panel", overlayCanvas.transform, new Color(0.035f, 0.04f, 0.055f, 0.68f));
            SetRect(helpPanel.rectTransform, new Vector2(18f, 12f), new Vector2(720f, 26f), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f));
            helpText = CreateText("Help", helpPanel.transform, font, 12, FontStyle.Normal, new Color(0.8f, 0.83f, 0.88f), TextAnchor.MiddleLeft);
            SetRect(helpText.rectTransform, new Vector2(0f, 0f), new Vector2(720f, 26f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
        }

        private void EnsureWorldHud()
        {
            if (worldHudRoot) return;

            GameObject root = new GameObject("Runtime World HUD");
            worldHudRoot = root.transform;

            worldPanelMaterial = NewHudMaterial("DK3D HUD Panel", new Color(0.015f, 0.018f, 0.024f, 0.92f));
            worldHpMaterial = NewHudMaterial("DK3D HUD HP", new Color(0.9f, 0.08f, 0.08f, 1f));
            worldStaminaMaterial = NewHudMaterial("DK3D HUD Stamina", new Color(0.18f, 0.9f, 0.35f, 1f));
            worldTextMaterial = NewHudMaterial("DK3D HUD Text", new Color(1f, 0.88f, 0.58f, 1f));

            worldTitleText = CreateWorldText("World HUD Title", 32, TextAnchor.MiddleLeft);
            worldStatsText = CreateWorldText("World HUD Stats", 22, TextAnchor.MiddleLeft);
            worldEquipmentText = CreateWorldText("World HUD Equipment", 20, TextAnchor.MiddleLeft);
            worldMessageText = CreateWorldText("World HUD Message", 26, TextAnchor.MiddleCenter);
            worldHelpText = CreateWorldText("World HUD Help", 18, TextAnchor.MiddleLeft);

            CreateWorldPanel("World HUD HP Back", out _);
            worldHpFill = CreateWorldPanel("World HUD HP Fill", out _);
            CreateWorldPanel("World HUD Stamina Back", out _);
            worldStaminaFill = CreateWorldPanel("World HUD Stamina Fill", out _);
            CreateWorldPanel("World HUD Equipment Back", out _);
            CreateWorldPanel("World HUD Message Back", out _);
            CreateWorldPanel("World HUD Help Back", out _);
        }

        private void UpdateWorldHud()
        {
            EnsureWorldHud();
            if (!player)
            {
                worldHudRoot.gameObject.SetActive(false);
                return;
            }

            Camera camera = Camera.main;
            if (!camera)
            {
                worldHudRoot.gameObject.SetActive(false);
                return;
            }

            worldHudRoot.gameObject.SetActive(true);
            if (!inventory) inventory = player.GetComponent<PlayerInventory>();

            worldTitleText.text = "Dungeon Knight 3D";
            worldStatsText.text = $"HP {player.Health}/{player.MaxHealth}   ST {Mathf.RoundToInt(player.Stamina)}/{Mathf.RoundToInt(player.MaxStamina)}";
            string weapon = inventory ? inventory.EquippedWeaponName : "Sin arma";
            string shield = inventory && !inventory.EquippedShieldItem.IsEmpty ? inventory.EquippedShieldItem.Name : "Sin escudo";
            worldEquipmentText.text = $"{weapon}\n{shield}";

            string status = player.StatusMessage;
            string prompt = player.CurrentInteractionPrompt;
            string centerMessage = !string.IsNullOrEmpty(status) ? status : prompt;
            worldMessageText.text = centerMessage;
            worldHelpText.text = "WASD mover | E usar | I inventario | J atacar | K bloquear | L rodar | Q pocion";

            worldHudRoot.SetParent(camera.transform, false);
            worldHudRoot.localPosition = Vector3.zero;
            worldHudRoot.localRotation = Quaternion.identity;

            const float hudDepth = 2.6f;
            float barWidth = CameraViewportWidth(camera, hudDepth) * 0.18f;
            float barHeight = CameraViewportHeight(camera, hudDepth) * 0.018f;
            PositionWorldText(worldTitleText, CameraLocalFromViewport(camera, 0.08f, 0.93f, hudDepth), 0.0115f);
            PositionWorldText(worldStatsText, CameraLocalFromViewport(camera, 0.08f, 0.875f, hudDepth), 0.0085f);
            worldEquipmentText.gameObject.SetActive(false);
            worldHelpText.gameObject.SetActive(false);

            bool showMessage = !string.IsNullOrEmpty(centerMessage);
            worldMessageText.gameObject.SetActive(showMessage);
            if (showMessage)
            {
                PositionWorldText(worldMessageText, CameraLocalFromViewport(camera, 0.5f, 0.09f, hudDepth), 0.0105f);
            }

            PositionWorldBar("World HUD HP Back", CameraLocalFromViewport(camera, 0.17f, 0.845f, hudDepth - 0.02f), barWidth, barHeight, worldPanelMaterial, 1f);
            PositionWorldBar("World HUD HP Fill", CameraLocalFromViewport(camera, 0.17f, 0.845f, hudDepth - 0.04f), barWidth, barHeight, worldHpMaterial, player.Health / (float)player.MaxHealth);
            PositionWorldBar("World HUD Stamina Back", CameraLocalFromViewport(camera, 0.17f, 0.805f, hudDepth - 0.02f), barWidth, barHeight, worldPanelMaterial, 1f);
            PositionWorldBar("World HUD Stamina Fill", CameraLocalFromViewport(camera, 0.17f, 0.805f, hudDepth - 0.04f), barWidth, barHeight, worldStaminaMaterial, player.Stamina / player.MaxStamina);
            PositionWorldBar("World HUD Equipment Back", Vector3.zero, 0f, 0f, worldPanelMaterial, 0f);
            PositionWorldBar("World HUD Help Back", Vector3.zero, 0f, 0f, worldPanelMaterial, 0f);
            PositionWorldBar("World HUD Message Back", CameraLocalFromViewport(camera, 0.5f, 0.08f, hudDepth - 0.02f), CameraViewportWidth(camera, hudDepth) * 0.42f, CameraViewportHeight(camera, hudDepth) * 0.065f, worldPanelMaterial, showMessage ? 1f : 0f);
        }

        private TextMesh CreateWorldText(string name, int fontSize, TextAnchor anchor)
        {
            GameObject textObject = new GameObject(name);
            textObject.transform.SetParent(worldHudRoot, false);
            TextMesh textMesh = textObject.AddComponent<TextMesh>();
            textMesh.fontSize = fontSize;
            textMesh.anchor = anchor;
            textMesh.alignment = anchor == TextAnchor.MiddleCenter ? TextAlignment.Center : TextAlignment.Left;
            textMesh.color = new Color(1f, 0.88f, 0.58f, 1f);
            MeshRenderer renderer = textObject.GetComponent<MeshRenderer>();
            renderer.sortingOrder = 32767;
            return textMesh;
        }

        private Transform CreateWorldPanel(string name, out MeshRenderer renderer)
        {
            GameObject panel = GameObject.CreatePrimitive(PrimitiveType.Quad);
            panel.name = name;
            panel.transform.SetParent(worldHudRoot, false);
            renderer = panel.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = worldPanelMaterial;
            renderer.sortingOrder = 32760;
            Collider collider = panel.GetComponent<Collider>();
            if (collider) Destroy(collider);
            return panel.transform;
        }

        private static Material NewHudMaterial(string name, Color color)
        {
            Shader shader = Shader.Find("Unlit/Color");
            if (!shader) shader = Shader.Find("Sprites/Default");
            if (!shader) shader = Shader.Find("Standard");
            Material material = new Material(shader);
            material.name = name;
            material.color = color;
            material.renderQueue = 5000;
            return material;
        }

        private static void PositionWorldText(TextMesh text, Vector3 localPosition, float scale)
        {
            Transform transform = text.transform;
            transform.localPosition = localPosition;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one * scale;
        }

        private static Vector3 CameraLocalFromViewport(Camera camera, float x, float y, float depth)
        {
            float height = CameraViewportHeight(camera, depth);
            float width = height * camera.aspect;
            return new Vector3((x - 0.5f) * width, (y - 0.5f) * height, depth);
        }

        private static float CameraViewportHeight(Camera camera, float depth)
        {
            if (camera.orthographic) return camera.orthographicSize * 2f;
            return 2f * depth * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        }

        private static float CameraViewportWidth(Camera camera, float depth)
        {
            return CameraViewportHeight(camera, depth) * camera.aspect;
        }

        private void PositionWorldBar(string name, Vector3 localPosition, float width, float height, Material material, float fill)
        {
            Transform panel = worldHudRoot.Find(name);
            if (!panel) return;

            fill = Mathf.Clamp01(fill);
            bool alwaysVisibleBack = name == "World HUD HP Back" || name == "World HUD Stamina Back";
            panel.gameObject.SetActive(fill > 0.001f || alwaysVisibleBack);
            panel.localPosition = localPosition;
            panel.localRotation = Quaternion.identity;
            panel.localScale = new Vector3(width * fill, height, 1f);
            panel.GetComponent<MeshRenderer>().sharedMaterial = material;
        }

        private void UpdateOverlayCanvas()
        {
            EnsureOverlayCanvas();
            if (!player)
            {
                overlayCanvas.enabled = false;
                return;
            }

            overlayCanvas.enabled = true;
            overlayCanvas.sortingOrder = 5000;
            overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            overlayCanvas.worldCamera = null;
            if (!inventory) inventory = player.GetComponent<PlayerInventory>();

            titleText.text = "Dungeon Knight 3D";
            hpFill.rectTransform.localScale = new Vector3(Mathf.Clamp01(player.Health / (float)player.MaxHealth), 1f, 1f);
            staminaFill.rectTransform.localScale = new Vector3(Mathf.Clamp01(player.Stamina / player.MaxStamina), 1f, 1f);
            hpText.text = $"HP {player.Health}/{player.MaxHealth}";
            staminaText.text = $"ST {Mathf.RoundToInt(player.Stamina)}/{Mathf.RoundToInt(player.MaxStamina)}";
            statsText.text = $"Almas: {player.Coins}   Pociones: {player.Potions}   Porton: {(player.HasGateKey ? "si" : "no")}   Torre: {(player.HasTowerKey ? "si" : "no")}";

            string weapon = inventory ? inventory.EquippedWeaponName : "Sin arma";
            string shield = inventory && !inventory.EquippedShieldItem.IsEmpty ? inventory.EquippedShieldItem.Name : "Sin escudo";
            equipmentText.text = $"  {weapon}\n  {shield}";
            equipmentPanel.gameObject.SetActive(false);

            string status = player.StatusMessage;
            string prompt = player.CurrentInteractionPrompt;
            string centerMessage = !string.IsNullOrEmpty(status) ? status : prompt;
            bool hasMessage = !string.IsNullOrEmpty(centerMessage);
            messagePanel.gameObject.SetActive(hasMessage);
            if (hasMessage)
            {
                HudMessageIcon icon = !string.IsNullOrEmpty(status) ? player.StatusMessageIcon : HudMessageIcon.None;
                messageIconText.text = icon == HudMessageIcon.Sword ? "Sword" : icon == HudMessageIcon.Shield ? "Shield" : string.Empty;
                messageText.rectTransform.anchoredPosition = icon == HudMessageIcon.None ? Vector2.zero : new Vector2(32f, 0f);
                messageText.rectTransform.sizeDelta = icon == HudMessageIcon.None ? new Vector2(530f, 48f) : new Vector2(460f, 48f);
                messageText.text = centerMessage;
            }

            helpText.text = "  WASD: moverte   Tab: fijar enemigo   Space: saltar   J: atacar/cargar   K: escudo/parry   L: rodar   E: usar   Q: pocion   I: inventario";
            helpPanel.gameObject.SetActive(false);
        }

        private Image CreateBar(string name, Vector2 anchoredPosition, Color fillColor)
        {
            Image back = CreatePanel($"{name} Back", overlayCanvas.transform, new Color(0.02f, 0.02f, 0.028f, 0.9f));
            SetRect(back.rectTransform, anchoredPosition, new Vector2(300f, 20f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f));
            Image fill = CreatePanel($"{name} Fill", back.transform, fillColor);
            SetRect(fill.rectTransform, new Vector2(2f, -2f), new Vector2(296f, 16f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f));
            fill.rectTransform.pivot = new Vector2(0f, 1f);
            return fill;
        }

        private static Text CreateText(string name, Transform parent, Font font, int size, FontStyle style, Color color, TextAnchor alignment)
        {
            GameObject textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);
            Text text = textObject.AddComponent<Text>();
            text.font = font;
            text.fontSize = size;
            text.fontStyle = style;
            text.color = color;
            text.alignment = alignment;
            text.raycastTarget = false;
            return text;
        }

        private static Image CreatePanel(string name, Transform parent, Color color)
        {
            GameObject panelObject = new GameObject(name);
            panelObject.transform.SetParent(parent, false);
            Image image = panelObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private static void SetRect(RectTransform rect, Vector2 anchoredPosition, Vector2 size, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;
        }

        private void DrawEquipmentQuickView()
        {
            if (!inventory) return;

            Rect panel = new Rect(22f, 118f, 250f, 58f);
            DrawBox(panel, new Color(0.02f, 0.023f, 0.03f, 0.72f), new Color(0.48f, 0.42f, 0.28f, 0.7f));
            GUI.Label(new Rect(panel.x + 12f, panel.y + 7f, panel.width - 24f, 18f), inventory.EquippedWeaponName, smallStyle);
            GUI.Label(new Rect(panel.x + 12f, panel.y + 30f, panel.width - 24f, 18f), inventory.EquippedShieldItem.IsEmpty ? "Sin escudo" : inventory.EquippedShieldItem.Name, smallStyle);
        }

        private void DrawInventoryPanel()
        {
            if (!inventory) return;

            float width = Mathf.Min(760f, Screen.width - 44f);
            float height = 438f;
            Rect panel = new Rect(Screen.width * 0.5f - width * 0.5f, Screen.height * 0.5f - height * 0.5f, width, height);
            DrawBox(panel, new Color(0.018f, 0.02f, 0.025f, 0.95f), new Color(0.64f, 0.54f, 0.34f, 0.9f));

            GUI.Label(new Rect(panel.x + 22f, panel.y + 18f, panel.width - 44f, 26f), "Equipo", titleStyle);
            GUI.Label(new Rect(panel.x + 22f, panel.y + 45f, panel.width - 44f, 20f), "Manos", smallStyle);

            if (DrawEquippedCard(new Rect(panel.x + 22f, panel.y + 82f, 220f, 120f), "MANO DERECHA", inventory.EquippedWeapon, listMode == EquipmentListMode.Weapons))
            {
                listMode = EquipmentListMode.Weapons;
            }

            if (DrawEquippedCard(new Rect(panel.x + 22f, panel.y + 218f, 220f, 120f), "MANO IZQUIERDA", inventory.EquippedShieldItem, listMode == EquipmentListMode.Shields))
            {
                listMode = EquipmentListMode.Shields;
            }

            hoveredSlot = -1;
            float gridX = panel.x + 270f;
            float gridY = panel.y + 82f;
            if (listMode == EquipmentListMode.None)
            {
                Rect emptyPanel = new Rect(gridX, gridY, panel.xMax - gridX - 24f, 210f);
                DrawBox(emptyPanel, new Color(0.028f, 0.031f, 0.038f, 0.92f), new Color(0.38f, 0.36f, 0.28f, 0.85f));
                GUI.Label(new Rect(emptyPanel.x + 18f, emptyPanel.y + 26f, emptyPanel.width - 36f, 28f), "Armas / Escudos", slotTitleStyle);
                return;
            }

            string listTitle = listMode == EquipmentListMode.Weapons ? "Inventario de armas" : "Inventario de escudos";
            GUI.Label(new Rect(gridX, panel.y + 52f, panel.xMax - gridX - 24f, 22f), listTitle, slotTitleStyle);

            float slotSize = Mathf.Min(82f, (panel.xMax - gridX - 24f) / 5f - 8f);
            for (int i = 0; i < PlayerInventory.EquipmentSlotCount; i++)
            {
                int column = i % 5;
                int row = i / 5;
                Rect slot = new Rect(gridX + column * (slotSize + 8f), gridY + row * (slotSize + 54f), slotSize, slotSize + 42f);
                DrawInventorySlot(slot, i, listMode);
            }

            Rect details = new Rect(gridX, panel.y + 318f, panel.xMax - gridX - 24f, 82f);
            int fallbackSlot = listMode == EquipmentListMode.Weapons ? inventory.EquippedWeaponSlot : inventory.EquippedShieldInventorySlot;
            DrawItemDetails(details, hoveredSlot >= 0 ? hoveredSlot : fallbackSlot, listMode);
        }

        private bool DrawEquippedCard(Rect rect, string label, PlayerInventory.EquipmentItem item, bool selected)
        {
            Color border = selected ? new Color(0.95f, 0.76f, 0.32f, 1f) : new Color(0.45f, 0.4f, 0.28f, 0.85f);
            DrawBox(rect, new Color(0.035f, 0.037f, 0.045f, 0.92f), border);
            GUI.Label(new Rect(rect.x + 12f, rect.y + 10f, rect.width - 24f, 18f), label, statStyle);
            GUI.Label(new Rect(rect.x + 12f, rect.y + 36f, rect.width - 24f, 30f), item.IsEmpty ? "Vacio" : item.Name, slotTitleStyle);
            GUI.Label(new Rect(rect.x + 12f, rect.y + 78f, rect.width - 24f, 28f), ItemStats(item), statStyle);
            return GUI.Button(rect, GUIContent.none, GUIStyle.none);
        }

        private void DrawInventorySlot(Rect rect, int slotIndex, EquipmentListMode mode)
        {
            PlayerInventory.EquipmentItem item = mode == EquipmentListMode.Weapons ? inventory.GetWeaponItem(slotIndex) : inventory.GetShieldItem(slotIndex);
            bool equipped = mode == EquipmentListMode.Weapons ? inventory.IsEquippedWeaponSlot(slotIndex) : inventory.IsEquippedShieldSlot(slotIndex);
            bool hover = rect.Contains(Event.current.mousePosition);
            if (hover) hoveredSlot = slotIndex;

            Color border = equipped ? new Color(0.95f, 0.76f, 0.32f, 1f) : hover ? new Color(0.7f, 0.74f, 0.82f, 0.95f) : new Color(0.28f, 0.3f, 0.36f, 0.85f);
            Color fill = item.IsEmpty ? new Color(0.028f, 0.03f, 0.036f, 0.9f) : new Color(0.055f, 0.058f, 0.068f, 0.92f);
            DrawBox(rect, fill, border);

            Rect icon = new Rect(rect.x + 14f, rect.y + 10f, rect.width - 28f, rect.width - 28f);
            DrawEquipmentIcon(icon, item);

            string itemName = item.IsEmpty ? "Vacio" : ShortName(item.Name);
            GUI.Label(new Rect(rect.x + 4f, rect.yMax - 34f, rect.width - 8f, 16f), itemName, statStyle);
            GUI.Label(new Rect(rect.x + 4f, rect.yMax - 18f, rect.width - 8f, 14f), equipped ? "EQUIPADO" : ItemTypeLabel(item), statStyle);

            if (!item.IsEmpty && GUI.Button(rect, GUIContent.none, GUIStyle.none))
            {
                if (mode == EquipmentListMode.Weapons) inventory.EquipWeaponSlot(slotIndex);
                else inventory.EquipShieldSlot(slotIndex);
            }
        }

        private static void DrawEquipmentIcon(Rect rect, PlayerInventory.EquipmentItem item)
        {
            Color previous = GUI.color;
            if (item.Type == EquipmentItemType.Sword)
            {
                GUI.color = new Color(0.78f, 0.8f, 0.76f, 1f);
                GUI.DrawTexture(new Rect(rect.center.x - 4f, rect.y + 4f, 8f, rect.height - 8f), Texture2D.whiteTexture);
                GUI.color = new Color(0.72f, 0.5f, 0.24f, 1f);
                GUI.DrawTexture(new Rect(rect.x + 10f, rect.yMax - 17f, rect.width - 20f, 7f), Texture2D.whiteTexture);
            }
            else if (item.Type == EquipmentItemType.Shield)
            {
                GUI.color = item.Shield == ShieldKind.Tower ? new Color(0.45f, 0.48f, 0.52f, 1f) : new Color(0.34f, 0.36f, 0.4f, 1f);
                GUI.DrawTexture(new Rect(rect.x + 12f, rect.y + 8f, rect.width - 24f, rect.height - 16f), Texture2D.whiteTexture);
                GUI.color = new Color(0.84f, 0.62f, 0.25f, 1f);
                GUI.DrawTexture(new Rect(rect.center.x - 3f, rect.y + 12f, 6f, rect.height - 24f), Texture2D.whiteTexture);
            }
            GUI.color = previous;
        }

        private void DrawItemDetails(Rect rect, int slotIndex, EquipmentListMode mode)
        {
            PlayerInventory.EquipmentItem item = mode == EquipmentListMode.Weapons ? inventory.GetWeaponItem(slotIndex) : inventory.GetShieldItem(slotIndex);
            DrawBox(rect, new Color(0.028f, 0.031f, 0.038f, 0.92f), new Color(0.38f, 0.36f, 0.28f, 0.85f));

            if (item.IsEmpty)
            {
                GUI.Label(new Rect(rect.x + 14f, rect.y + 12f, rect.width - 28f, 22f), "Espacio vacio", slotTitleStyle);
                GUI.Label(new Rect(rect.x + 14f, rect.y + 42f, rect.width - 28f, 20f), "Aqui podras guardar otro equipo mas adelante.", smallStyle);
                return;
            }

            GUI.Label(new Rect(rect.x + 14f, rect.y + 10f, rect.width - 28f, 24f), item.Name, slotTitleStyle);
            GUI.Label(new Rect(rect.x + 14f, rect.y + 38f, rect.width - 28f, 18f), item.Description, smallStyle);
            GUI.Label(new Rect(rect.x + 14f, rect.y + 60f, rect.width - 28f, 18f), ItemStats(item), statStyle);
        }

        private static string ItemStats(PlayerInventory.EquipmentItem item)
        {
            if (item.Type == EquipmentItemType.Sword) return $"Dano {item.LightDamage} / Cargado {item.ChargedDamage}";
            if (item.Type == EquipmentItemType.Shield) return $"Stamina bloqueo x{item.BlockStaminaMultiplier:0.00}";
            return string.Empty;
        }

        private static string ItemTypeLabel(PlayerInventory.EquipmentItem item)
        {
            if (item.Type == EquipmentItemType.Sword) return "ESPADA";
            if (item.Type == EquipmentItemType.Shield) return "ESCUDO";
            return string.Empty;
        }

        private static string ShortName(string itemName)
        {
            return itemName
                .Replace("Espada de ", string.Empty)
                .Replace("Escudo de ", string.Empty);
        }

        private static void DrawMessageIcon(Rect rect, HudMessageIcon icon)
        {
            Color previous = GUI.color;
            DrawBox(rect, new Color(0.055f, 0.058f, 0.066f, 0.96f), new Color(0.86f, 0.68f, 0.32f, 0.95f));

            if (icon == HudMessageIcon.Sword)
            {
                GUI.color = new Color(0.82f, 0.84f, 0.82f, 1f);
                GUI.DrawTexture(new Rect(rect.center.x - 3f, rect.y + 7f, 6f, rect.height - 14f), Texture2D.whiteTexture);
                GUI.color = new Color(0.76f, 0.55f, 0.25f, 1f);
                GUI.DrawTexture(new Rect(rect.x + 8f, rect.yMax - 13f, rect.width - 16f, 5f), Texture2D.whiteTexture);
            }
            else if (icon == HudMessageIcon.Shield)
            {
                GUI.color = new Color(0.34f, 0.37f, 0.42f, 1f);
                GUI.DrawTexture(new Rect(rect.x + 9f, rect.y + 7f, rect.width - 18f, rect.height - 13f), Texture2D.whiteTexture);
                GUI.color = new Color(0.86f, 0.62f, 0.25f, 1f);
                GUI.DrawTexture(new Rect(rect.center.x - 3f, rect.y + 10f, 6f, rect.height - 20f), Texture2D.whiteTexture);
            }

            GUI.color = previous;
        }

        private enum EquipmentListMode
        {
            None,
            Weapons,
            Shields
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
