using UnityEngine;

namespace DungeonKnight.Level
{
    internal sealed class DungeonKnight3DInteractableBuilder
    {
        private readonly DungeonKnight3DAssets assets;

        public DungeonKnight3DInteractableBuilder(DungeonKnight3DAssets assets)
        {
            this.assets = assets;
        }

        public void BuildWorldOneOneInteractables()
        {
            CreateInteractableBox("Bonfire", new Vector3(-3.3f, 0.45f, -15.1f), new Vector3(1.1f, 0.9f, 1.1f), assets.Ember).ConfigureBonfire();
            CreateInteractableBox("Lore Tablet", new Vector3(2.7f, 0.6f, -13.4f), new Vector3(1.1f, 1.2f, 0.28f), assets.Brass)
                .ConfigureLore("J: atacar. Mantener J: golpe cargado. K bloquea, L rueda, E interactua.");
            CreateInteractableBox("Treasure Chest", new Vector3(-4.1f, 1.95f, 7.8f), new Vector3(1.2f, 0.55f, 0.75f), assets.Brass).ConfigureChest(12);

            Transform gate = CreateBox("Locked Gate", new Vector3(0f, 1.65f, 20.7f), new Vector3(5.8f, 3.2f, 0.38f), assets.Brass).transform;
            CreateInteractableBox("Gate Lock", new Vector3(0f, 1f, 19.85f), new Vector3(1.2f, 1.4f, 0.45f), assets.Brass).ConfigureGate(gate);
            CreateDoor("Door 1-1 To 1-2", new Vector3(0f, 1.35f, 22.65f), new Vector3(2.35f, 2.7f, 0.35f), new Vector3(0f, 1.05f, 28.2f), Vector3.forward, "Entraste al World 1-2.");
            CreateInteractableBox("World 1-2 Marker", new Vector3(2.7f, 1f, 22.8f), new Vector3(1.1f, 1.4f, 0.3f), assets.Brass)
                .ConfigureLore("La llave abre el porton. Mas alla empieza el camino exterior del 1-2.");

            CreatePickup("Coin A", new Vector3(2.4f, 0.65f, -6.2f), true);
            CreatePickup("Coin B", new Vector3(-2.4f, 0.65f, -4.6f), true);
            CreatePickup("Potion", new Vector3(4.2f, 2.95f, 14.4f), false);
        }

        public void CreateBonfire(string name, Vector3 position)
        {
            DungeonInteractable3D bonfire = CreateInteractableBox(name, position, new Vector3(1.1f, 0.9f, 1.1f), assets.Ember);
            bonfire.ConfigureBonfire();

            Light light = bonfire.gameObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1f, 0.42f, 0.16f);
            light.intensity = 2.4f;
            light.range = 7f;
        }

        public DungeonInteractable3D CreateInteractableBox(string name, Vector3 position, Vector3 scale, Material material)
        {
            GameObject interactable = CreateBox(name, position, scale, material);
            interactable.GetComponent<BoxCollider>().isTrigger = true;
            return interactable.AddComponent<DungeonInteractable3D>();
        }

        public void CreateDoor(string name, Vector3 position, Vector3 scale, Vector3 destination, Vector3 facing, string travelText)
        {
            DungeonInteractable3D door = CreateInteractableBox(name, position, scale, assets.DarkStone);
            door.ConfigureDoor(destination, facing, travelText);
            CreateBox($"{name} Frame Top", position + Vector3.up * (scale.y * 0.5f + 0.22f), new Vector3(scale.x + 0.55f, 0.35f, scale.z + 0.2f), assets.Brass);
            CreateBox($"{name} Frame Left", position + Vector3.left * (scale.x * 0.5f + 0.18f), new Vector3(0.28f, scale.y + 0.45f, scale.z + 0.2f), assets.Brass);
            CreateBox($"{name} Frame Right", position + Vector3.right * (scale.x * 0.5f + 0.18f), new Vector3(0.28f, scale.y + 0.45f, scale.z + 0.2f), assets.Brass);
        }

        public void CreatePickup(string name, Vector3 position, bool coin)
        {
            GameObject pickup = GameObject.CreatePrimitive(coin ? PrimitiveType.Cylinder : PrimitiveType.Sphere);
            pickup.name = name;
            pickup.transform.position = position;
            pickup.transform.localScale = coin ? new Vector3(0.45f, 0.08f, 0.45f) : new Vector3(0.55f, 0.55f, 0.55f);
            pickup.GetComponent<Renderer>().material = coin ? assets.Brass : assets.Potion;
            Collider collider = pickup.GetComponent<Collider>();
            collider.isTrigger = true;
            DungeonPickup3D pickupScript = pickup.AddComponent<DungeonPickup3D>();
            if (coin) pickupScript.ConfigureCoin(3);
            else pickupScript.ConfigurePotion();
        }

        private static GameObject CreateBox(string name, Vector3 position, Vector3 scale, Material material)
        {
            return DungeonKnight3DGeometryBuilder.CreateBox(name, position, scale, material);
        }
    }
}
