using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class DK3DManualTorchPolisher
{
    private const string MaterialFolder = "Assets/Art/Materials/Torches";
    private const string MeshFolder = "Assets/Art/Meshes/Torches";

    [MenuItem("Tools/Dungeon Knight 3D/Polish Manual Torch Flames")]
    public static void PolishManualTorchFlames()
    {
        Material outerFlame = LoadOrCreateMaterial("DK3D_TorchFlame_Outer", new Color(1f, 0.24f, 0.02f, 0.78f), new Color(1f, 0.32f, 0.04f), 2.4f);
        Material midFlame = LoadOrCreateMaterial("DK3D_TorchFlame_Mid", new Color(1f, 0.58f, 0.03f, 0.9f), new Color(1f, 0.62f, 0.08f), 3.0f);
        Material hotCore = LoadOrCreateMaterial("DK3D_TorchFlame_Core", new Color(1f, 0.92f, 0.35f, 0.95f), new Color(1f, 0.86f, 0.32f), 3.4f);
        Material coal = LoadOrCreateMaterial("DK3D_TorchCoal_DarkEmber", new Color(0.24f, 0.055f, 0.035f, 1f), new Color(1f, 0.16f, 0.04f), 0.55f);
        Mesh outerMesh = LoadOrCreateFlameMesh("DK3D_TorchFlame_OuterMesh", 0.25f, 0.72f, 0.04f);
        Mesh midMesh = LoadOrCreateFlameMesh("DK3D_TorchFlame_MidMesh", 0.17f, 0.58f, 0.015f);
        Mesh coreMesh = LoadOrCreateFlameMesh("DK3D_TorchFlame_CoreMesh", 0.085f, 0.42f, -0.005f);

        int polished = 0;
        foreach (GameObject torch in Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (!IsManualTorchRoot(torch.name)) continue;
            PolishTorch(torch.transform, outerFlame, midFlame, hotCore, coal, outerMesh, midMesh, coreMesh);
            polished++;
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log($"Polished {polished} manual torch flame(s).");
    }

    private static bool IsManualTorchRoot(string name)
    {
        return name.StartsWith("Manual Upper Stair Wall Torch") ||
               name.StartsWith("Manual Upper Stair Opposite Wall Torch");
    }

    private static void PolishTorch(Transform torch, Material outerFlame, Material midFlame, Material hotCore, Material coalMaterial, Mesh outerMesh, Mesh midMesh, Mesh coreMesh)
    {
        Undo.RegisterFullObjectHierarchyUndo(torch.gameObject, "Polish manual torch flame");

        Transform ember = torch.Find("Wall Torch Ember Core");
        if (ember)
        {
            ember.localPosition = new Vector3(0.5f, 0.32f, 0f);
            ember.localScale = new Vector3(0.13f, 0.07f, 0.13f);
            AssignMaterial(ember, coalMaterial);
        }

        Transform flame = torch.Find("Torch Flame");
        if (!flame)
        {
            flame = new GameObject("Torch Flame").transform;
            flame.SetParent(torch, false);
        }

        flame.localPosition = new Vector3(0.58f, 0.48f, 0f);
        flame.localRotation = Quaternion.identity;
        flame.localScale = Vector3.one;

        SpriteRenderer sprite = flame.GetComponent<SpriteRenderer>();
        if (sprite)
        {
            sprite.color = new Color(1f, 0.78f, 0.24f, 0.92f);
            sprite.sortingOrder = 8;
            sprite.enabled = true;
        }

        Light light = flame.GetComponent<Light>();
        if (!light) light = flame.gameObject.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(1f, 0.46f, 0.13f);
        light.intensity = 2.45f;
        light.range = 5.2f;

        ReplaceFlameMesh(flame, "Torch Flame Outer Lobe", outerFlame, outerMesh, new Vector3(0.02f, -0.05f, 0f));
        ReplaceFlameMesh(flame, "Torch Flame Middle Lobe", midFlame, midMesh, new Vector3(0.01f, 0.01f, -0.015f));
        ReplaceFlameMesh(flame, "Torch Flame Hot Core", hotCore, coreMesh, new Vector3(0.0f, 0.07f, 0.02f));

        Transform glow = flame.Find("Torch Flame Glow");
        if (glow)
        {
            glow.localPosition = new Vector3(0f, 0.18f, 0.02f);
            glow.localScale = new Vector3(1.05f, 1.0f, 1f);
        }

        TightenBasket(torch);
    }

    private static void ReplaceFlameMesh(Transform flame, string name, Material material, Mesh mesh, Vector3 localOffset)
    {
        Transform existing = flame.Find(name);
        GameObject lobe = existing ? existing.gameObject : new GameObject(name);
        lobe.transform.SetParent(flame, false);
        lobe.transform.localPosition = localOffset;
        lobe.transform.localRotation = Quaternion.identity;
        lobe.transform.localScale = Vector3.one;

        MeshFilter filter = lobe.GetComponent<MeshFilter>();
        if (!filter) filter = lobe.AddComponent<MeshFilter>();
        MeshRenderer renderer = lobe.GetComponent<MeshRenderer>();
        if (!renderer) renderer = lobe.AddComponent<MeshRenderer>();

        filter.sharedMesh = mesh;
        renderer.sharedMaterial = material;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;
    }

    private static Mesh CreateFlameMesh(float radius, float height, float lean)
    {
        const int segments = 12;
        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 6];

        vertices[0] = new Vector3(lean, height, 0f);
        vertices[1] = new Vector3(0f, 0f, 0f);
        for (int i = 0; i < segments; i++)
        {
            float angle = Mathf.PI * 2f * i / segments;
            float x = Mathf.Cos(angle) * radius * 0.62f;
            float z = Mathf.Sin(angle) * radius;
            vertices[i + 2] = new Vector3(x, 0f, z);
        }

        int t = 0;
        for (int i = 0; i < segments; i++)
        {
            int current = i + 2;
            int next = (i + 1) % segments + 2;
            triangles[t++] = 0;
            triangles[t++] = current;
            triangles[t++] = next;
            triangles[t++] = 1;
            triangles[t++] = next;
            triangles[t++] = current;
        }

        Mesh mesh = new Mesh { name = "DK3D Torch Flame Lobe Mesh" };
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    private static Mesh LoadOrCreateFlameMesh(string name, float radius, float height, float lean)
    {
        if (!Directory.Exists(MeshFolder))
        {
            Directory.CreateDirectory(MeshFolder);
        }

        string path = $"{MeshFolder}/{name}.asset";
        Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
        if (!mesh)
        {
            mesh = CreateFlameMesh(radius, height, lean);
            mesh.name = name;
            AssetDatabase.CreateAsset(mesh, path);
            AssetDatabase.SaveAssets();
        }

        return mesh;
    }

    private static void TightenBasket(Transform torch)
    {
        SetLocal(torch, "Wall Torch Basket Top Band Front", new Vector3(0.56f, 0.57f, 0f), new Vector3(0.024f, 0.16f, 0.024f));
        SetLocal(torch, "Wall Torch Basket Top Band Side", new Vector3(0.56f, 0.57f, 0f), new Vector3(0.024f, 0.14f, 0.024f));
        SetLocal(torch, "Wall Torch Basket Lower Band Front", new Vector3(0.54f, 0.34f, 0f), new Vector3(0.026f, 0.14f, 0.026f));
        SetLocal(torch, "Wall Torch Basket Lower Band Side", new Vector3(0.54f, 0.34f, 0f), new Vector3(0.026f, 0.12f, 0.026f));

        for (int i = 1; i <= 6; i++)
        {
            Transform rib = torch.Find($"Wall Torch Basket Iron Rib {i}");
            if (!rib) continue;
            rib.localScale = new Vector3(0.018f, 0.16f, 0.018f);
            rib.localPosition = new Vector3(0.56f, 0.46f, rib.localPosition.z * 0.75f);
        }
    }

    private static void SetLocal(Transform parent, string childName, Vector3 localPosition, Vector3 localScale)
    {
        Transform child = parent.Find(childName);
        if (!child) return;
        child.localPosition = localPosition;
        child.localScale = localScale;
    }

    private static void AssignMaterial(Transform target, Material material)
    {
        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer) renderer.sharedMaterial = material;
    }

    private static Material LoadOrCreateMaterial(string name, Color color, Color emission, float intensity)
    {
        if (!Directory.Exists(MaterialFolder))
        {
            Directory.CreateDirectory(MaterialFolder);
        }

        string path = $"{MaterialFolder}/{name}.mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (!material)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (!shader) shader = Shader.Find("Standard");
            material = new Material(shader) { name = name };
            AssetDatabase.CreateAsset(material, path);
        }

        material.color = color;
        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", emission * intensity);
        EditorUtility.SetDirty(material);
        AssetDatabase.SaveAssets();
        return material;
    }
}
