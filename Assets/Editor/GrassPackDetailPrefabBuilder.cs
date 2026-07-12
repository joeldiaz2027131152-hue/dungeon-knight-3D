using System.IO;
using UnityEditor;
using UnityEngine;

public static class GrassPackDetailPrefabBuilder
{
    private const string SourceModelPath = "Assets/Art/Models/SwampApproach/Grass01Pack/grass.FBX";
    private const string OutputFolder = "Assets/Art/Models/SwampApproach/GrassDetails";

    [MenuItem("Dungeon Knight 3D/Swamp/Build Grass Pack Detail Prefabs")]
    public static void Build()
    {
        Directory.CreateDirectory(OutputFolder);

        BuildVariant("Grass01", "Assets/Art/Models/SwampApproach/Grass01Pack/grass_01", new Vector3(1.8f, 3.8f, 1.8f));
        BuildVariant("Grass02", "Assets/Art/Models/SwampApproach/Grass01Pack/grass_02", new Vector3(2f, 4.2f, 2f));
        BuildVariant("Grass03", "Assets/Art/Models/SwampApproach/Grass01Pack/grass_03", new Vector3(2.2f, 4.6f, 2.2f));
        BuildVariant("Grass04", "Assets/Art/Models/SwampApproach/Grass02Pack/grass_04", new Vector3(1.9f, 4f, 1.9f));
        BuildVariant("Grass05", "Assets/Art/Models/SwampApproach/Grass02Pack/grass_05", new Vector3(2.25f, 4.8f, 2.25f));
        BuildVariant("Grass06", "Assets/Art/Models/SwampApproach/Grass02Pack/grass_06", new Vector3(2.4f, 5.2f, 2.4f));

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Built swamp grass detail prefabs in {OutputFolder}");
    }

    private static void BuildVariant(string name, string textureFolder, Vector3 scale)
    {
        GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>(SourceModelPath);
        if (source == null)
        {
            Debug.LogError($"Grass source model not found at {SourceModelPath}");
            return;
        }

        ConfigureTextureImporters(textureFolder);

        string materialPath = $"{OutputFolder}/{name}_Detail.mat";
        string meshPath = $"{OutputFolder}/{name}_DetailMesh.asset";
        string prefabPath = $"{OutputFolder}/{name}_Detail.prefab";

        Material material = CreateOrUpdateMaterial(name, textureFolder, materialPath);
        Mesh mesh = CreateOrUpdateCombinedMesh(source, meshPath, $"{name}_DetailMesh", scale);
        if (mesh == null)
        {
            Debug.LogError($"Grass source model has no mesh filters at {SourceModelPath}");
            return;
        }

        GameObject instance = new GameObject($"{name}_Detail");
        MeshFilter meshFilter = instance.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = instance.AddComponent<MeshRenderer>();
        meshFilter.sharedMesh = mesh;
        meshRenderer.sharedMaterial = material;
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = true;

        PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
        Object.DestroyImmediate(instance);
    }

    private static void ConfigureTextureImporters(string textureFolder)
    {
        TextureImporter diffuseImporter = AssetImporter.GetAtPath($"{textureFolder}/diffus.tga") as TextureImporter;
        if (diffuseImporter != null)
        {
            diffuseImporter.textureType = TextureImporterType.Default;
            diffuseImporter.alphaIsTransparency = true;
            diffuseImporter.mipmapEnabled = true;
            diffuseImporter.SaveAndReimport();
        }

        TextureImporter normalImporter = AssetImporter.GetAtPath($"{textureFolder}/normal.tga") as TextureImporter;
        if (normalImporter != null)
        {
            normalImporter.textureType = TextureImporterType.NormalMap;
            normalImporter.mipmapEnabled = true;
            normalImporter.SaveAndReimport();
        }
    }

    private static Material CreateOrUpdateMaterial(string name, string textureFolder, string materialPath)
    {
        Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if (material == null)
        {
            material = new Material(Shader.Find("Standard"));
            AssetDatabase.CreateAsset(material, materialPath);
        }

        Texture2D diffuse = AssetDatabase.LoadAssetAtPath<Texture2D>($"{textureFolder}/diffus.tga");
        Texture2D normal = AssetDatabase.LoadAssetAtPath<Texture2D>($"{textureFolder}/normal.tga");

        material.name = $"{name}_Detail";
        material.shader = Shader.Find("Standard");
        material.SetTexture("_MainTex", diffuse);
        material.SetTexture("_BumpMap", normal);
        material.SetColor("_Color", new Color(0.76f, 0.7f, 0.54f, 1f));
        material.SetFloat("_Cutoff", 0.35f);
        material.SetFloat("_Glossiness", 0.06f);
        material.SetFloat("_Metallic", 0f);
        material.SetFloat("_BumpScale", 0.35f);
        material.SetFloat("_Mode", 1f);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        material.SetInt("_ZWrite", 1);
        material.EnableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_NORMALMAP");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;

        EditorUtility.SetDirty(material);
        return material;
    }

    private static Mesh CreateOrUpdateCombinedMesh(GameObject source, string meshPath, string meshName, Vector3 scale)
    {
        GameObject sourceInstance = Object.Instantiate(source);
        sourceInstance.transform.position = Vector3.zero;
        sourceInstance.transform.rotation = Quaternion.identity;
        sourceInstance.transform.localScale = Vector3.one;

        MeshFilter[] meshFilters = sourceInstance.GetComponentsInChildren<MeshFilter>(true);
        if (meshFilters.Length == 0)
        {
            Object.DestroyImmediate(sourceInstance);
            return null;
        }

        CombineInstance[] combines = new CombineInstance[meshFilters.Length];
        for (int i = 0; i < meshFilters.Length; i++)
        {
            combines[i] = new CombineInstance
            {
                mesh = meshFilters[i].sharedMesh,
                transform = Matrix4x4.Scale(scale) * meshFilters[i].transform.localToWorldMatrix
            };
        }

        Mesh combined = new Mesh { name = meshName };
        combined.CombineMeshes(combines, true, true, false);
        combined.RecalculateBounds();
        MovePivotToGroundCenter(combined);
        combined.RecalculateNormals();

        Object.DestroyImmediate(sourceInstance);

        Mesh existing = AssetDatabase.LoadAssetAtPath<Mesh>(meshPath);
        if (existing == null)
        {
            AssetDatabase.CreateAsset(combined, meshPath);
            return combined;
        }

        EditorUtility.CopySerialized(combined, existing);
        Object.DestroyImmediate(combined);
        EditorUtility.SetDirty(existing);
        return existing;
    }

    private static void MovePivotToGroundCenter(Mesh mesh)
    {
        Bounds bounds = mesh.bounds;
        Vector3 offset = new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] -= offset;
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
    }
}
