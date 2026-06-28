using UnityEngine;

namespace DungeonKnight.Level
{
    internal static class DungeonChestVisualFactory3D
    {
        public static DungeonChestVisual3D EnsureVisual(DungeonInteractable3D interactable)
        {
            Transform trigger = interactable.transform;
            Transform chestRoot = trigger.parent ? trigger.parent : trigger;

            DungeonChestVisual3D existing = chestRoot.GetComponent<DungeonChestVisual3D>();
            if (existing) return existing;

            HideLegacyChestParts(chestRoot, trigger);

            Material shell = NewMaterial("Runtime Blue Iron Chest Shell", new Color(0.16f, 0.24f, 0.29f));
            Material iron = NewMaterial("Runtime Dark Riveted Chest Iron", new Color(0.045f, 0.06f, 0.075f));
            Material trim = NewMaterial("Runtime Worn Chest Edge Metal", new Color(0.5f, 0.6f, 0.66f));
            Material glow = NewMaterial("Runtime Chest Soul Glow", new Color(0.74f, 0.94f, 1f));
            Material inner = NewMaterial("Runtime Chest Deep Interior", new Color(0.015f, 0.02f, 0.026f));

            GameObject visualRoot = new GameObject("Blue Iron Chest Runtime Visual");
            visualRoot.transform.SetParent(chestRoot, false);
            visualRoot.transform.localPosition = Vector3.zero;
            visualRoot.transform.localRotation = Quaternion.identity;
            visualRoot.transform.localScale = Vector3.one;

            CreateLocalBox(visualRoot.transform, "Chest Lower Blue Iron Case", new Vector3(0f, -0.16f, 0f), new Vector3(1.42f, 0.46f, 0.9f), shell);
            CreateLocalBox(visualRoot.transform, "Chest Dark Open Interior", new Vector3(0f, 0.11f, -0.03f), new Vector3(1.22f, 0.12f, 0.68f), inner);
            CreateLocalBox(visualRoot.transform, "Chest Front Raised Panel", new Vector3(0f, -0.15f, -0.515f), new Vector3(1.12f, 0.3f, 0.045f), shell);
            CreateLocalBox(visualRoot.transform, "Chest Left Side Panel", new Vector3(-0.74f, -0.15f, 0f), new Vector3(0.05f, 0.34f, 0.76f), shell);
            CreateLocalBox(visualRoot.transform, "Chest Right Side Panel", new Vector3(0.74f, -0.15f, 0f), new Vector3(0.05f, 0.34f, 0.76f), shell);
            CreateLocalBox(visualRoot.transform, "Chest Front Iron Strap", new Vector3(0f, 0.03f, -0.47f), new Vector3(1.46f, 0.12f, 0.07f), iron);
            CreateLocalBox(visualRoot.transform, "Chest Back Iron Strap", new Vector3(0f, 0.03f, 0.47f), new Vector3(1.46f, 0.12f, 0.07f), iron);
            CreateLocalBox(visualRoot.transform, "Chest Left Iron Strap", new Vector3(-0.71f, 0.03f, 0f), new Vector3(0.07f, 0.12f, 0.9f), iron);
            CreateLocalBox(visualRoot.transform, "Chest Right Iron Strap", new Vector3(0.71f, 0.03f, 0f), new Vector3(0.07f, 0.12f, 0.9f), iron);
            CreateLocalBox(visualRoot.transform, "Chest Front Bottom Lip", new Vector3(0f, -0.41f, -0.47f), new Vector3(1.5f, 0.1f, 0.08f), iron);
            CreateLocalBox(visualRoot.transform, "Chest Lock Plate", new Vector3(0f, -0.06f, -0.54f), new Vector3(0.28f, 0.34f, 0.08f), trim);
            CreateLocalBox(visualRoot.transform, "Chest Lock Keyhole", new Vector3(0f, -0.08f, -0.595f), new Vector3(0.08f, 0.18f, 0.035f), iron);
            CreateHandle(visualRoot.transform, trim);
            CreateSideChain(visualRoot.transform, iron, trim);

            GameObject lidPivot = new GameObject("Chest Lid Pivot");
            lidPivot.transform.SetParent(visualRoot.transform, false);
            lidPivot.transform.localPosition = new Vector3(0f, 0.13f, 0.43f);
            CreateArchedLid(lidPivot.transform, "Chest Rounded Lid Shell", new Vector3(0f, 0.02f, -0.42f), new Vector3(1.44f, 0.32f, 0.88f), shell);
            CreateLocalBox(lidPivot.transform, "Chest Lid Front Band", new Vector3(0f, 0.04f, -0.88f), new Vector3(1.52f, 0.12f, 0.08f), iron);
            CreateLocalBox(lidPivot.transform, "Chest Lid Crown Band", new Vector3(0f, 0.26f, -0.42f), new Vector3(1.5f, 0.08f, 0.12f), trim);
            CreateLocalBox(lidPivot.transform, "Chest Left Lid Hinge Band", new Vector3(-0.42f, 0.1f, -0.42f), new Vector3(0.08f, 0.22f, 0.92f), iron);
            CreateLocalBox(lidPivot.transform, "Chest Right Lid Hinge Band", new Vector3(0.42f, 0.1f, -0.42f), new Vector3(0.08f, 0.22f, 0.92f), iron);

            CreateRivetRow(visualRoot.transform, -0.53f, -0.47f, 7, trim);
            CreateRivetRow(visualRoot.transform, 0.11f, -0.47f, 7, trim);
            CreateRivetRow(lidPivot.transform, 0.12f, -0.88f, 7, trim);

            GameObject treasure = new GameObject("Chest Treasure");
            treasure.transform.SetParent(visualRoot.transform, false);
            treasure.transform.localPosition = new Vector3(0f, 0.12f, -0.05f);
            treasure.transform.localScale = Vector3.one;
            CreateCoinPile(treasure.transform, trim);
            CreateSoulGlow(treasure.transform, glow);

            Light light = treasure.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(0.78f, 0.95f, 1f);
            light.range = 2.7f;
            light.intensity = 0.8f;

            DungeonChestVisual3D visual = chestRoot.gameObject.AddComponent<DungeonChestVisual3D>();
            visual.Configure(lidPivot.transform, treasure, light);
            return visual;
        }

        private static void HideLegacyChestParts(Transform chestRoot, Transform trigger)
        {
            foreach (Transform child in chestRoot.GetComponentsInChildren<Transform>(true))
            {
                if (child == chestRoot || child == trigger || child.IsChildOf(trigger)) continue;
                if (child.name.Contains("Blocker")) continue;
                if (!child.name.Contains("Chest")) continue;

                foreach (Renderer renderer in child.GetComponents<Renderer>())
                {
                    renderer.enabled = false;
                }

                foreach (Collider collider in child.GetComponents<Collider>())
                {
                    collider.enabled = false;
                }
            }
        }

        private static GameObject CreateLocalBox(Transform parent, string name, Vector3 localPosition, Vector3 localScale, Material material)
        {
            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.name = name;
            box.transform.SetParent(parent, false);
            box.transform.localPosition = localPosition;
            box.transform.localScale = localScale;
            box.GetComponent<Renderer>().material = material;
            Object.Destroy(box.GetComponent<Collider>());
            return box;
        }

        private static void CreateRivetRow(Transform parent, float y, float z, int count, Material material)
        {
            for (int i = 0; i < count; i++)
            {
                float x = -0.54f + i * (1.08f / (count - 1));
                GameObject rivet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                rivet.name = "Chest Raised Rivet";
                rivet.transform.SetParent(parent, false);
                rivet.transform.localPosition = new Vector3(x, y, z);
                rivet.transform.localScale = Vector3.one * 0.055f;
                rivet.GetComponent<Renderer>().material = material;
                Object.Destroy(rivet.GetComponent<Collider>());
            }
        }

        private static void CreateHandle(Transform parent, Material material)
        {
            CreateLocalBox(parent, "Chest Handle Left Mount", new Vector3(-0.22f, -0.03f, -0.59f), new Vector3(0.07f, 0.14f, 0.035f), material);
            CreateLocalBox(parent, "Chest Handle Right Mount", new Vector3(0.22f, -0.03f, -0.59f), new Vector3(0.07f, 0.14f, 0.035f), material);
            GameObject leftArm = CreateLocalBox(parent, "Chest Handle Left Arm", new Vector3(-0.14f, -0.11f, -0.62f), new Vector3(0.045f, 0.16f, 0.035f), material);
            leftArm.transform.localRotation = Quaternion.Euler(0f, 0f, -18f);
            GameObject rightArm = CreateLocalBox(parent, "Chest Handle Right Arm", new Vector3(0.14f, -0.11f, -0.62f), new Vector3(0.045f, 0.16f, 0.035f), material);
            rightArm.transform.localRotation = Quaternion.Euler(0f, 0f, 18f);
            CreateLocalBox(parent, "Chest Handle Bottom Grip", new Vector3(0f, -0.18f, -0.62f), new Vector3(0.26f, 0.045f, 0.035f), material);
        }

        private static void CreateSideChain(Transform parent, Material iron, Material trim)
        {
            for (int i = 0; i < 5; i++)
            {
                GameObject link = CreateLocalBox(parent, "Chest Curled Side Chain", Vector3.zero, new Vector3(0.12f, 0.035f, 0.045f), i % 2 == 0 ? trim : iron);
                float angle = i * 72f;
                float radius = 0.11f;
                link.transform.localPosition = new Vector3(0.78f, -0.16f + Mathf.Sin(angle * Mathf.Deg2Rad) * radius, -0.02f + Mathf.Cos(angle * Mathf.Deg2Rad) * radius);
                link.transform.localRotation = Quaternion.Euler(0f, 90f, angle);
            }
        }

        private static void CreateArchedLid(Transform parent, string name, Vector3 localPosition, Vector3 size, Material material)
        {
            const int segments = 8;
            float halfWidth = size.x * 0.5f;
            float halfDepth = size.z * 0.5f;
            float height = size.y;

            Vector3[] vertices = new Vector3[(segments + 1) * 2 + 4];
            for (int i = 0; i <= segments; i++)
            {
                float t = i / (float)segments;
                float z = Mathf.Lerp(-halfDepth, halfDepth, t);
                float y = Mathf.Sin(t * Mathf.PI) * height;
                vertices[i * 2] = new Vector3(-halfWidth, y, z);
                vertices[i * 2 + 1] = new Vector3(halfWidth, y, z);
            }

            int bottomStart = (segments + 1) * 2;
            vertices[bottomStart] = new Vector3(-halfWidth, 0f, -halfDepth);
            vertices[bottomStart + 1] = new Vector3(halfWidth, 0f, -halfDepth);
            vertices[bottomStart + 2] = new Vector3(-halfWidth, 0f, halfDepth);
            vertices[bottomStart + 3] = new Vector3(halfWidth, 0f, halfDepth);

            int[] triangles = new int[segments * 6 + 18];
            int tri = 0;
            for (int i = 0; i < segments; i++)
            {
                int a = i * 2;
                int b = a + 1;
                int c = a + 2;
                int d = a + 3;
                triangles[tri++] = a;
                triangles[tri++] = c;
                triangles[tri++] = b;
                triangles[tri++] = b;
                triangles[tri++] = c;
                triangles[tri++] = d;
            }

            triangles[tri++] = bottomStart;
            triangles[tri++] = bottomStart + 1;
            triangles[tri++] = bottomStart + 2;
            triangles[tri++] = bottomStart + 1;
            triangles[tri++] = bottomStart + 3;
            triangles[tri++] = bottomStart + 2;

            triangles[tri++] = 0;
            triangles[tri++] = 1;
            triangles[tri++] = bottomStart;
            triangles[tri++] = 1;
            triangles[tri++] = bottomStart + 1;
            triangles[tri++] = bottomStart;

            int lastLeft = segments * 2;
            int lastRight = lastLeft + 1;
            triangles[tri++] = lastLeft;
            triangles[tri++] = bottomStart + 2;
            triangles[tri++] = lastRight;
            triangles[tri++] = lastRight;
            triangles[tri++] = bottomStart + 2;
            triangles[tri++] = bottomStart + 3;

            GameObject lid = new GameObject(name);
            lid.transform.SetParent(parent, false);
            lid.transform.localPosition = localPosition;
            Mesh mesh = new Mesh { name = name };
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            lid.AddComponent<MeshFilter>().mesh = mesh;
            lid.AddComponent<MeshRenderer>().material = material;
        }

        private static void CreateCoinPile(Transform parent, Material material)
        {
            for (int i = 0; i < 7; i++)
            {
                GameObject coin = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                coin.name = "Chest Small Coin";
                coin.transform.SetParent(parent, false);
                coin.transform.localPosition = new Vector3(-0.28f + (i % 4) * 0.18f, -0.02f + i * 0.006f, -0.12f + (i / 4) * 0.18f);
                coin.transform.localRotation = Quaternion.Euler(90f, 0f, i * 11f);
                coin.transform.localScale = new Vector3(0.075f, 0.012f, 0.075f);
                coin.GetComponent<Renderer>().material = material;
                Object.Destroy(coin.GetComponent<Collider>());
            }
        }

        private static void CreateSoulGlow(Transform parent, Material material)
        {
            GameObject soul = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            soul.name = "Chest Soul Glow";
            soul.transform.SetParent(parent, false);
            soul.transform.localPosition = new Vector3(0f, 0.16f, -0.04f);
            soul.transform.localScale = Vector3.one * 0.12f;
            soul.GetComponent<Renderer>().material = material;
            Object.Destroy(soul.GetComponent<Collider>());

            GameObject wispA = CreateLocalBox(soul.transform, "Chest Soul Wisp A", new Vector3(-0.08f, 0.22f, 0f), new Vector3(0.025f, 0.2f, 0.025f), material);
            wispA.transform.localRotation = Quaternion.Euler(0f, 0f, -22f);
            GameObject wispB = CreateLocalBox(soul.transform, "Chest Soul Wisp B", new Vector3(0.07f, 0.32f, 0f), new Vector3(0.022f, 0.22f, 0.022f), material);
            wispB.transform.localRotation = Quaternion.Euler(0f, 0f, 18f);
        }

        private static Material NewMaterial(string name, Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (!shader) shader = Shader.Find("Standard");
            if (!shader) shader = Shader.Find("Diffuse");
            Material material = new Material(shader);
            material.name = name;
            material.color = color;
            return material;
        }
    }
}
