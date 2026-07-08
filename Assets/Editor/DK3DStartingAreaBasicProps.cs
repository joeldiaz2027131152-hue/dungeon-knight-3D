using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class DK3DStartingAreaBasicProps
{
    private const string MenuPath = "Tools/Dungeon Knight 3D/Place Starting Area Basic Props";
    private const string RootName = "Starting Area Basic Props";
    private const string MaterialsFolder = "Assets/Art/Materials/SetDressing";

    [MenuItem(MenuPath)]
    public static void Place()
    {
        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName("Place starting area basic props");
        var undoGroup = Undo.GetCurrentGroup();

        DestroyIfPresent(RootName);

        var root = new GameObject(RootName);
        Undo.RegisterCreatedObjectUndo(root, "Create starting area basic props");

        var stone = LoadOrCreateMaterial("DK3D_Prop_RubbleStone", new Color(0.28f, 0.31f, 0.30f), 0.18f);
        var darkStone = LoadOrCreateMaterial("DK3D_Prop_DarkWetStone", new Color(0.08f, 0.11f, 0.10f), 0.08f);
        var moss = LoadOrCreateMaterial("DK3D_Prop_Moss", new Color(0.12f, 0.28f, 0.13f), 0.12f);
        var wood = LoadOrCreateMaterial("DK3D_Prop_OldWood", new Color(0.36f, 0.23f, 0.14f), 0.2f);
        var iron = LoadOrCreateMaterial("DK3D_Prop_DarkIron", new Color(0.07f, 0.07f, 0.08f), 0.25f);
        var ember = LoadOrCreateMaterial("DK3D_Prop_TorchEmber", new Color(1f, 0.42f, 0.1f), 0.45f);
        var web = LoadOrCreateMaterial("DK3D_Prop_Cobweb", new Color(0.74f, 0.76f, 0.72f, 0.55f), 0.05f);

        PlaceTorches(root.transform, wood, iron, ember);
        PlaceCornerClutter(root.transform, wood, stone);
        PlaceRubble(root.transform, stone, darkStone);
        PlaceMossAndDamp(root.transform, moss, darkStone);
        PlaceCobwebs(root.transform, web);
        PlaceStaticSpikes(root.transform, iron);

        Selection.activeGameObject = root;
        EditorUtility.SetDirty(root);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(root.scene);
        Undo.CollapseUndoOperations(undoGroup);

        Debug.Log("[Dungeon Knight 3D] Starting area basic props placed.");
    }

    private static void PlaceTorches(Transform root, Material wood, Material iron, Material ember)
    {
        CreateTorch(root, "Left Wall Guide Torch 01", new Vector3(-7.18f, 2.35f, -16.2f), 90f, wood, iron, ember);
        CreateTorch(root, "Right Wall Guide Torch 01", new Vector3(7.18f, 2.35f, -9.5f), -90f, wood, iron, ember);
        CreateTorch(root, "Left Wall Guide Torch 02", new Vector3(-7.18f, 2.35f, 1.8f), 90f, wood, iron, ember);
        CreateTorch(root, "Right Wall Guide Torch 02", new Vector3(7.18f, 2.35f, 12.8f), -90f, wood, iron, ember);
        CreateTorch(root, "Gate Approach Torch", new Vector3(-7.18f, 2.35f, 20.0f), 90f, wood, iron, ember);
    }

    private static void PlaceCornerClutter(Transform root, Material wood, Material stone)
    {
        CreateCrateStack(root, "Broken Crate Corner A", new Vector3(-6.35f, 0.18f, -18.0f), wood, -8f);
        CreateCrateStack(root, "Broken Crate Corner B", new Vector3(6.25f, 0.18f, -13.2f), wood, 13f);
        CreateBarrelCluster(root, "Old Barrel Corner A", new Vector3(-6.15f, 0.35f, 7.2f), wood, stone);
        CreateBarrelCluster(root, "Old Barrel Corner B", new Vector3(6.2f, 0.35f, 16.4f), wood, stone);
    }

    private static void PlaceRubble(Transform root, Material stone, Material darkStone)
    {
        CreateRubbleCluster(root, "Left Wall Rubble 01", new Vector3(-6.55f, 0.06f, -7.0f), stone, darkStone, 0);
        CreateRubbleCluster(root, "Right Wall Rubble 01", new Vector3(6.55f, 0.06f, -1.2f), stone, darkStone, 1);
        CreateRubbleCluster(root, "Left Wall Rubble 02", new Vector3(-6.5f, 0.06f, 11.0f), stone, darkStone, 2);
        CreateRubbleCluster(root, "Gate Side Rubble", new Vector3(5.75f, 0.06f, 20.0f), stone, darkStone, 3);
    }

    private static void PlaceMossAndDamp(Transform root, Material moss, Material darkStone)
    {
        CreatePatch(root, "Floor Moss Patch 01", new Vector3(-5.25f, 0.035f, -14.2f), Quaternion.Euler(0f, 24f, 0f), new Vector3(1.35f, 0.018f, 0.72f), moss);
        CreatePatch(root, "Floor Moss Patch 02", new Vector3(5.15f, 0.035f, -5.0f), Quaternion.Euler(0f, -11f, 0f), new Vector3(1.0f, 0.018f, 0.55f), moss);
        CreatePatch(root, "Floor Damp Patch 01", new Vector3(-5.7f, 0.032f, 5.5f), Quaternion.Euler(0f, -32f, 0f), new Vector3(1.55f, 0.015f, 0.65f), darkStone);
        CreatePatch(root, "Floor Moss Patch 03", new Vector3(5.75f, 0.035f, 15.8f), Quaternion.Euler(0f, 18f, 0f), new Vector3(1.25f, 0.018f, 0.6f), moss);

        CreateWallStain(root, "Left Wall Damp Stain 01", new Vector3(-7.44f, 1.55f, -11.5f), 90f, new Vector3(0.02f, 1.35f, 1.0f), darkStone);
        CreateWallStain(root, "Right Wall Moss Stain 01", new Vector3(7.46f, 1.4f, 2.0f), -90f, new Vector3(0.02f, 1.1f, 1.35f), moss);
        CreateWallStain(root, "Left Wall Moss Stain 02", new Vector3(-7.44f, 1.6f, 17.8f), 90f, new Vector3(0.02f, 1.15f, 1.2f), moss);
    }

    private static void PlaceCobwebs(Transform root, Material web)
    {
        CreateCobweb(root, "Start Arch Left Cobweb", new Vector3(-6.45f, 4.55f, -21.08f), 0f, web);
        CreateCobweb(root, "Start Arch Right Cobweb", new Vector3(6.35f, 4.55f, -21.08f), 0f, web);
        CreateCobweb(root, "Gate Corner Cobweb", new Vector3(-6.35f, 4.7f, 20.75f), 0f, web);
        CreateCobweb(root, "Right Wall High Cobweb", new Vector3(7.32f, 4.35f, 10.4f), -90f, web);
    }

    private static void PlaceStaticSpikes(Transform root, Material iron)
    {
        CreateSpikeCluster(root, "Static Spike Warning Left", new Vector3(-5.6f, 0.05f, -2.3f), iron);
        CreateSpikeCluster(root, "Static Spike Warning Right", new Vector3(5.55f, 0.05f, 8.8f), iron);
    }

    private static void CreateTorch(Transform parent, string name, Vector3 position, float yaw, Material wood, Material iron, Material ember)
    {
        var torch = CreateEmpty(parent, name, position, Quaternion.Euler(0f, yaw, 0f));
        CreateBox(torch.transform, "Wall Plate", new Vector3(0f, 0f, 0f), Quaternion.identity, new Vector3(0.12f, 0.45f, 0.35f), iron);
        CreateCylinder(torch.transform, "Torch Handle", new Vector3(0.24f, -0.12f, 0f), Quaternion.Euler(0f, 0f, 62f), 0.07f, 0.78f, wood);
        CreateCylinder(torch.transform, "Iron Band", new Vector3(0.48f, 0.16f, 0f), Quaternion.Euler(0f, 0f, 62f), 0.09f, 0.16f, iron);
        CreateSphere(torch.transform, "Small Flame Glow", new Vector3(0.58f, 0.32f, 0f), new Vector3(0.22f, 0.32f, 0.22f), ember);

        var lightObject = new GameObject("Warm Torch Light");
        Undo.RegisterCreatedObjectUndo(lightObject, "Create torch light");
        lightObject.transform.SetParent(torch.transform, false);
        lightObject.transform.localPosition = new Vector3(0.55f, 0.35f, 0f);
        var light = lightObject.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(1f, 0.55f, 0.22f);
        light.range = 4.2f;
        light.intensity = 1.35f;
    }

    private static void CreateCrateStack(Transform parent, string name, Vector3 position, Material wood, float yaw)
    {
        var root = CreateEmpty(parent, name, position, Quaternion.Euler(0f, yaw, 0f));
        CreateBox(root.transform, "Cracked Crate Main", Vector3.zero, Quaternion.Euler(0f, 0f, 2f), new Vector3(0.75f, 0.55f, 0.7f), wood);
        CreateBox(root.transform, "Broken Crate Side", new Vector3(0.44f, -0.06f, 0.18f), Quaternion.Euler(0f, 18f, -12f), new Vector3(0.12f, 0.42f, 0.58f), wood);
        CreateBox(root.transform, "Loose Plank A", new Vector3(-0.18f, 0.36f, 0.45f), Quaternion.Euler(11f, 22f, -8f), new Vector3(0.58f, 0.08f, 0.12f), wood);
        CreateBox(root.transform, "Loose Plank B", new Vector3(0.1f, -0.31f, -0.48f), Quaternion.Euler(0f, -33f, 0f), new Vector3(0.78f, 0.07f, 0.1f), wood);
    }

    private static void CreateBarrelCluster(Transform parent, string name, Vector3 position, Material wood, Material stone)
    {
        var root = CreateEmpty(parent, name, position, Quaternion.identity);
        CreateCylinder(root.transform, "Standing Barrel", new Vector3(0f, 0.2f, 0f), Quaternion.identity, 0.42f, 0.75f, wood);
        CreateCylinder(root.transform, "Fallen Barrel", new Vector3(0.62f, -0.1f, 0.18f), Quaternion.Euler(0f, 0f, 90f), 0.34f, 0.72f, wood);
        CreateBox(root.transform, "Broken Barrel Slat", new Vector3(-0.34f, -0.08f, 0.42f), Quaternion.Euler(0f, 27f, 0f), new Vector3(0.5f, 0.08f, 0.12f), wood);
        CreateSphere(root.transform, "Small Corner Stone", new Vector3(0.12f, -0.23f, -0.52f), new Vector3(0.28f, 0.14f, 0.22f), stone);
    }

    private static void CreateRubbleCluster(Transform parent, string name, Vector3 position, Material stone, Material darkStone, int seed)
    {
        var root = CreateEmpty(parent, name, position, Quaternion.Euler(0f, seed * 17f, 0f));
        var offsets = new[]
        {
            new Vector3(0f, 0f, 0f),
            new Vector3(0.38f, 0.01f, 0.18f),
            new Vector3(-0.32f, 0.02f, 0.2f),
            new Vector3(0.16f, 0.01f, -0.34f),
            new Vector3(-0.52f, 0f, -0.18f)
        };

        for (var i = 0; i < offsets.Length; i++)
        {
            var scale = new Vector3(0.28f + i * 0.03f, 0.12f + (i % 2) * 0.04f, 0.22f + i * 0.02f);
            CreateSphere(root.transform, $"Rubble Stone {i + 1:00}", offsets[i], scale, i % 2 == 0 ? stone : darkStone);
        }
    }

    private static void CreatePatch(Transform parent, string name, Vector3 position, Quaternion rotation, Vector3 scale, Material material)
    {
        CreateBox(parent, name, position, rotation, scale, material);
    }

    private static void CreateWallStain(Transform parent, string name, Vector3 position, float yaw, Vector3 scale, Material material)
    {
        CreateBox(parent, name, position, Quaternion.Euler(0f, yaw, 0f), scale, material);
    }

    private static void CreateCobweb(Transform parent, string name, Vector3 position, float yaw, Material material)
    {
        var root = CreateEmpty(parent, name, position, Quaternion.Euler(0f, yaw, 0f));
        CreateCylinderBetween(root.transform, "Web Top Strand", new Vector3(0f, 0f, 0f), new Vector3(1.0f, -0.04f, 0f), 0.012f, material);
        CreateCylinderBetween(root.transform, "Web Side Strand", new Vector3(0f, 0f, 0f), new Vector3(0.02f, -0.85f, 0f), 0.012f, material);
        CreateCylinderBetween(root.transform, "Web Diagonal Strand", new Vector3(0f, 0f, 0f), new Vector3(0.92f, -0.78f, 0f), 0.01f, material);
        CreateCylinderBetween(root.transform, "Web Sag Strand", new Vector3(0.2f, -0.18f, 0f), new Vector3(0.86f, -0.72f, 0f), 0.008f, material);
        CreateCylinderBetween(root.transform, "Web Inner Strand", new Vector3(0.06f, -0.54f, 0f), new Vector3(0.72f, -0.2f, 0f), 0.008f, material);
    }

    private static void CreateSpikeCluster(Transform parent, string name, Vector3 position, Material material)
    {
        var root = CreateEmpty(parent, name, position, Quaternion.identity);
        for (var i = 0; i < 4; i++)
        {
            var x = (i - 1.5f) * 0.34f;
            var z = i % 2 == 0 ? -0.12f : 0.16f;
            CreateCone(root.transform, $"Static Iron Spike {i + 1:00}", new Vector3(x, 0.28f, z), Quaternion.Euler(0f, i * 11f, 0f), 0.16f, 0.72f, material);
        }

        CreateBox(root.transform, "Spike Base Plate", new Vector3(0f, 0.03f, 0.02f), Quaternion.identity, new Vector3(1.55f, 0.08f, 0.55f), material);
    }

    private static GameObject CreateEmpty(Transform parent, string name, Vector3 localPosition, Quaternion localRotation)
    {
        var obj = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(obj, "Create starting prop");
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = localPosition;
        obj.transform.localRotation = localRotation;
        return obj;
    }

    private static GameObject CreateBox(Transform parent, string name, Vector3 localPosition, Quaternion localRotation, Vector3 localScale, Material material)
    {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Undo.RegisterCreatedObjectUndo(obj, "Create starting prop box");
        obj.name = name;
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = localPosition;
        obj.transform.localRotation = localRotation;
        obj.transform.localScale = localScale;
        AssignMaterial(obj, material);
        return obj;
    }

    private static GameObject CreateCylinder(Transform parent, string name, Vector3 localPosition, Quaternion localRotation, float diameter, float length, Material material)
    {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Undo.RegisterCreatedObjectUndo(obj, "Create starting prop cylinder");
        obj.name = name;
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = localPosition;
        obj.transform.localRotation = localRotation;
        obj.transform.localScale = new Vector3(diameter, length * 0.5f, diameter);
        AssignMaterial(obj, material);
        return obj;
    }

    private static GameObject CreateSphere(Transform parent, string name, Vector3 localPosition, Vector3 localScale, Material material)
    {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Undo.RegisterCreatedObjectUndo(obj, "Create starting prop sphere");
        obj.name = name;
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = localPosition;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = localScale;
        AssignMaterial(obj, material);
        return obj;
    }

    private static void CreateCylinderBetween(Transform parent, string name, Vector3 start, Vector3 end, float diameter, Material material)
    {
        var midpoint = (start + end) * 0.5f;
        var direction = end - start;
        var length = direction.magnitude;
        var rotation = Quaternion.FromToRotation(Vector3.up, direction.normalized);
        CreateCylinder(parent, name, midpoint, rotation, diameter, length, material);
    }

    private static void CreateCone(Transform parent, string name, Vector3 localPosition, Quaternion localRotation, float radius, float height, Material material)
    {
        var obj = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(obj, "Create starting prop cone");
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = localPosition;
        obj.transform.localRotation = localRotation;

        var mesh = new Mesh { name = name + " Mesh" };
        var vertices = new List<Vector3> { new Vector3(0f, height * 0.5f, 0f) };
        const int sides = 12;
        for (var i = 0; i < sides; i++)
        {
            var angle = Mathf.PI * 2f * i / sides;
            vertices.Add(new Vector3(Mathf.Cos(angle) * radius, -height * 0.5f, Mathf.Sin(angle) * radius));
        }

        vertices.Add(new Vector3(0f, -height * 0.5f, 0f));

        var triangles = new List<int>();
        for (var i = 1; i <= sides; i++)
        {
            var next = i == sides ? 1 : i + 1;
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(next);

            triangles.Add(sides + 1);
            triangles.Add(next);
            triangles.Add(i);
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        obj.AddComponent<MeshFilter>().sharedMesh = mesh;
        var renderer = obj.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = material;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        renderer.receiveShadows = true;
        var collider = obj.AddComponent<MeshCollider>();
        collider.sharedMesh = mesh;
    }

    private static void AssignMaterial(GameObject obj, Material material)
    {
        var renderer = obj.GetComponent<MeshRenderer>();
        if (renderer == null)
        {
            return;
        }

        renderer.sharedMaterial = material;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        renderer.receiveShadows = true;
        EditorUtility.SetDirty(renderer);
    }

    private static Material LoadOrCreateMaterial(string materialName, Color color, float smoothness)
    {
        Directory.CreateDirectory(MaterialsFolder);
        var path = $"{MaterialsFolder}/{materialName}.mat";
        var material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            material = new Material(Shader.Find("Standard"))
            {
                name = materialName
            };
            AssetDatabase.CreateAsset(material, path);
        }

        material.shader = Shader.Find("Standard");
        material.SetColor("_Color", color);
        material.SetFloat("_Metallic", 0f);
        material.SetFloat("_Glossiness", smoothness);
        EditorUtility.SetDirty(material);
        return material;
    }

    private static void DestroyIfPresent(string objectName)
    {
        var existing = GameObject.Find(objectName);
        if (existing != null)
        {
            Undo.DestroyObjectImmediate(existing);
        }
    }
}
