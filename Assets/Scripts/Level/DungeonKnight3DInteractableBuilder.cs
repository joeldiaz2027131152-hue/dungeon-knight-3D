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
            CreateFirstBonfireNpc();
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

        public DungeonInteractable3D CreateInvisibleInteractableBox(string name, Vector3 position, Vector3 scale, Material material)
        {
            DungeonInteractable3D interactable = CreateInteractableBox(name, position, scale, material);
            Renderer renderer = interactable.GetComponent<Renderer>();
            if (renderer) renderer.enabled = false;
            return interactable;
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

        private void CreateFirstBonfireNpc()
        {
            Vector3 position = new Vector3(-5.35f, 0f, -14.7f);
            GameObject npc = new GameObject("Anciano del Principio NPC");
            npc.transform.position = position;
            npc.transform.rotation = Quaternion.LookRotation(new Vector3(2f, 0f, -0.4f).normalized, Vector3.up);

            if (!AttachAncianoNpcModel(npc.transform))
            {
                GameObject fallback = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                fallback.name = "Anciano del Principio Fallback";
                fallback.transform.SetParent(npc.transform, false);
                fallback.transform.localPosition = Vector3.up;
                fallback.transform.localRotation = Quaternion.identity;
                fallback.transform.localScale = new Vector3(0.65f, 0.9f, 0.65f);
                fallback.GetComponent<Renderer>().material = DungeonKnight3DAssets.NewMaterial("DK3D Anciano Cloak", new Color(0.025f, 0.026f, 0.024f));
                Object.Destroy(fallback.GetComponent<CapsuleCollider>());
            }

            Vector3 talkPosition = position + Vector3.up * 0.95f + npc.transform.forward * 0.55f;
            DungeonInteractable3D talk = CreateInvisibleInteractableBox("Anciano del Principio Talk", talkPosition, new Vector3(2.7f, 2.1f, 2.7f), assets.Brass);
            talk.ConfigureNpc("El fuego no te salva del miedo, solo te da otra oportunidad. Si cruzas la niebla, termina el trabajo antes de buscar la torre.");
        }

        private bool AttachAncianoNpcModel(Transform npcTransform)
        {
            const string modelPath = "Characters/NPC/AncianoDelPrincipio/anciano_del_principio";
            GameObject modelPrefab = Resources.Load<GameObject>(modelPath);
            if (!modelPrefab) return false;

            GameObject visual = Object.Instantiate(modelPrefab, npcTransform);
            visual.name = "Anciano del Principio Model";
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.identity;
            visual.transform.localScale = Vector3.one;

            Collider[] modelColliders = visual.GetComponentsInChildren<Collider>(true);
            foreach (Collider modelCollider in modelColliders)
            {
                Object.Destroy(modelCollider);
            }

            Renderer[] renderers = visual.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0)
            {
                Object.Destroy(visual);
                return false;
            }

            FitModelToHeightOnFloor(visual, renderers, 1.82f, npcTransform.position);
            ApplyAncianoPalette(visual, npcTransform);

            Animator animator = visual.GetComponentInChildren<Animator>();
            if (animator)
            {
                animator.applyRootMotion = false;
                animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            }

            foreach (Renderer renderer in renderers)
            {
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                renderer.receiveShadows = true;
                if (renderer is SkinnedMeshRenderer skinnedMeshRenderer)
                {
                    skinnedMeshRenderer.updateWhenOffscreen = true;
                }
            }

            return true;
        }

        private void ApplyAncianoPalette(GameObject visual, Transform npcTransform)
        {
            Material robe = DungeonKnight3DAssets.NewMaterial("DK3D Anciano Deep Black Robe", new Color(0.025f, 0.026f, 0.024f));
            Material skin = DungeonKnight3DAssets.NewMaterial("DK3D Anciano Warm Skin", new Color(0.72f, 0.52f, 0.4f));
            Material beard = DungeonKnight3DAssets.NewMaterial("DK3D Anciano Silver Beard", new Color(0.58f, 0.56f, 0.52f));
            Material sash = DungeonKnight3DAssets.NewMaterial("DK3D Anciano Dark Sash", new Color(0.11f, 0.095f, 0.12f));

            foreach (Renderer renderer in visual.GetComponentsInChildren<Renderer>(true))
            {
                renderer.material = robe;
            }

            Transform head = FindModelBone(visual.transform, "head");
            Transform leftHand = FindModelBone(visual.transform, "lefthand");
            Transform rightHand = FindModelBone(visual.transform, "righthand");

            Vector3 forward = npcTransform.forward;
            Vector3 root = npcTransform.position;
            Vector3 headPosition = head ? head.position + Vector3.up * 0.1f + forward * 0.025f : root + Vector3.up * 1.62f + forward * 0.04f;
            Vector3 beardPosition = head ? head.position + Vector3.down * 0.13f + forward * 0.08f : root + Vector3.up * 1.42f + forward * 0.08f;
            Vector3 leftHandPosition = leftHand ? leftHand.position : root + npcTransform.right * -0.72f + Vector3.up * 1.18f;
            Vector3 rightHandPosition = rightHand ? rightHand.position : root + npcTransform.right * 0.72f + Vector3.up * 1.18f;

            CreateAncianoAccent("Anciano Bald Head Color", PrimitiveType.Sphere, npcTransform, headPosition, Quaternion.identity, new Vector3(0.2f, 0.24f, 0.19f), skin);
            CreateAncianoAccent("Anciano Left Hand Color", PrimitiveType.Sphere, npcTransform, leftHandPosition, Quaternion.identity, new Vector3(0.13f, 0.08f, 0.1f), skin);
            CreateAncianoAccent("Anciano Right Hand Color", PrimitiveType.Sphere, npcTransform, rightHandPosition, Quaternion.identity, new Vector3(0.13f, 0.08f, 0.1f), skin);
            CreateAncianoAccent("Anciano Long Silver Beard", PrimitiveType.Capsule, npcTransform, beardPosition, Quaternion.identity, new Vector3(0.16f, 0.33f, 0.11f), beard);
            CreateAncianoAccent("Anciano Cloth Sash", PrimitiveType.Cube, npcTransform, root + Vector3.up * 0.88f + forward * 0.2f, npcTransform.rotation, new Vector3(0.72f, 0.12f, 0.08f), sash);
        }

        private static Transform FindModelBone(Transform root, string boneName)
        {
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                string childName = child.name.ToLowerInvariant();
                if (childName == boneName || childName.EndsWith(":" + boneName) || childName.EndsWith("_" + boneName))
                {
                    return child;
                }
            }

            return null;
        }

        private static void CreateAncianoAccent(string name, PrimitiveType primitiveType, Transform parent, Vector3 worldPosition, Quaternion worldRotation, Vector3 worldScale, Material material)
        {
            GameObject accent = GameObject.CreatePrimitive(primitiveType);
            accent.name = name;
            accent.transform.position = worldPosition;
            accent.transform.rotation = worldRotation;
            accent.transform.localScale = worldScale;
            accent.transform.SetParent(parent, true);
            accent.GetComponent<Renderer>().material = material;
            Object.Destroy(accent.GetComponent<Collider>());
        }

        private static void FitModelToHeightOnFloor(GameObject visual, Renderer[] renderers, float targetHeight, Vector3 floorPosition)
        {
            Bounds bounds = GetRendererBounds(renderers);
            if (bounds.size.y > 0.001f)
            {
                visual.transform.localScale *= targetHeight / bounds.size.y;
            }

            bounds = GetRendererBounds(renderers);
            Vector3 correction = new Vector3(floorPosition.x - bounds.center.x, floorPosition.y - bounds.min.y, floorPosition.z - bounds.center.z);
            visual.transform.position += correction;
        }

        private static Bounds GetRendererBounds(Renderer[] renderers)
        {
            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            return bounds;
        }
    }
}
