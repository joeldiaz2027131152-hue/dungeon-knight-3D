using UnityEngine;

namespace DungeonKnight.Level
{
    internal sealed class RiggedSkeletonSwordVisual3D
    {
        private static readonly Color RustBladeColor = new Color(0.38f, 0.35f, 0.3f);
        private static readonly Color RustPatchColor = new Color(0.42f, 0.16f, 0.08f);
        private static readonly Color DarkGripColor = new Color(0.14f, 0.1f, 0.07f);

        private readonly Transform rightHand;
        private readonly Transform rustySword;

        private RiggedSkeletonSwordVisual3D(Transform rightHand, Transform rustySword)
        {
            this.rightHand = rightHand;
            this.rustySword = rustySword;
        }

        public static RiggedSkeletonSwordVisual3D Attach(Transform skeletonTransform)
        {
            Transform rightHand = FindDeepChild(skeletonTransform, "mixamorig:RightHand") ?? FindDeepChild(skeletonTransform, "RightHand");
            if (!rightHand)
            {
                Debug.LogWarning("RiggedSkeletonEnemyVisual3D: RightHand bone not found, rusty sword skipped.");
                return null;
            }

            Material bladeMaterial = NewMaterial("Skeleton Rusty Blade", RustBladeColor, 0.2f);
            Material rustMaterial = NewMaterial("Skeleton Rust Patches", RustPatchColor, 0.05f);
            Material gripMaterial = NewMaterial("Skeleton Dark Grip", DarkGripColor, 0.15f);

            GameObject swordRootObject = new GameObject("Rusty Sword");
            Transform rustySword = swordRootObject.transform;
            rustySword.SetParent(skeletonTransform.parent != null ? skeletonTransform.parent : skeletonTransform, true);
            rustySword.localScale = Vector3.one;

            RiggedSkeletonSwordVisual3D swordVisual = new RiggedSkeletonSwordVisual3D(rightHand, rustySword);
            swordVisual.Tick();
            CreateBlade(rustySword, bladeMaterial);
            CreateTip(rustySword, bladeMaterial);
            CreateGuard(rustySword, rustMaterial);
            CreateGrip(rustySword, gripMaterial);
            AddRustPatch(rustySword, new Vector3(-0.024f, 0.28f, -0.019f), new Vector3(0.044f, 0.12f, 0.006f), rustMaterial);
            AddRustPatch(rustySword, new Vector3(0.022f, 0.58f, -0.019f), new Vector3(0.04f, 0.1f, 0.006f), rustMaterial);

            return swordVisual;
        }

        public void Tick()
        {
            if (!rightHand || !rustySword) return;

            rustySword.position = rightHand.position;
            rustySword.rotation = rightHand.rotation * Quaternion.Euler(8f, 88f, -88f);
        }

        public void Destroy()
        {
            if (rustySword) Object.Destroy(rustySword.gameObject);
        }

        private static void CreateBlade(Transform parent, Material material)
        {
            GameObject blade = GameObject.CreatePrimitive(PrimitiveType.Cube);
            blade.name = "Pitted Blade";
            blade.transform.SetParent(parent, false);
            blade.transform.localPosition = new Vector3(0f, 0.43f, 0f);
            blade.transform.localScale = new Vector3(0.075f, 0.78f, 0.035f);
            blade.GetComponent<Renderer>().material = material;
            Object.Destroy(blade.GetComponent<Collider>());
        }

        private static void CreateTip(Transform parent, Material material)
        {
            GameObject tip = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tip.name = "Chipped Tip";
            tip.transform.SetParent(parent, false);
            tip.transform.localPosition = new Vector3(0f, 0.84f, 0f);
            tip.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
            tip.transform.localScale = new Vector3(0.058f, 0.058f, 0.034f);
            tip.GetComponent<Renderer>().material = material;
            Object.Destroy(tip.GetComponent<Collider>());
        }

        private static void CreateGuard(Transform parent, Material material)
        {
            GameObject guard = GameObject.CreatePrimitive(PrimitiveType.Cube);
            guard.name = "Rusty Guard";
            guard.transform.SetParent(parent, false);
            guard.transform.localPosition = new Vector3(0f, 0.02f, 0f);
            guard.transform.localScale = new Vector3(0.34f, 0.05f, 0.06f);
            guard.GetComponent<Renderer>().material = material;
            Object.Destroy(guard.GetComponent<Collider>());
        }

        private static void CreateGrip(Transform parent, Material material)
        {
            GameObject grip = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            grip.name = "Worn Grip";
            grip.transform.SetParent(parent, false);
            grip.transform.localPosition = new Vector3(0f, -0.13f, 0f);
            grip.transform.localScale = new Vector3(0.045f, 0.18f, 0.045f);
            grip.GetComponent<Renderer>().material = material;
            Object.Destroy(grip.GetComponent<Collider>());
        }

        private static void AddRustPatch(Transform parent, Vector3 position, Vector3 scale, Material material)
        {
            GameObject patch = GameObject.CreatePrimitive(PrimitiveType.Cube);
            patch.name = "Rust Patch";
            patch.transform.SetParent(parent, false);
            patch.transform.localPosition = position;
            patch.transform.localScale = scale;
            patch.GetComponent<Renderer>().material = material;
            Object.Destroy(patch.GetComponent<Collider>());
        }

        private static Material NewMaterial(string name, Color color, float metallic)
        {
            Material material = new Material(Shader.Find("Standard"));
            material.name = name;
            material.color = color;
            material.SetFloat("_Metallic", metallic);
            material.SetFloat("_Glossiness", 0.18f);
            return material;
        }

        private static Transform FindDeepChild(Transform root, string childName)
        {
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == childName) return child;
            }

            return null;
        }
    }
}
