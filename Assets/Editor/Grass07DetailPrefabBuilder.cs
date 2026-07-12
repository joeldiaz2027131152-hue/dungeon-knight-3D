using System.IO;
using UnityEditor;
using UnityEngine;

public static class Grass07DetailPrefabBuilder
{
    private const string SourceModelPath = "Assets/Art/Models/SwampApproach/Grass07/grass_07.FBX";
    private const string DiffusePath = "Assets/Art/Models/SwampApproach/Grass07/diffus.tga";
    private const string NormalPath = "Assets/Art/Models/SwampApproach/Grass07/normal.tga";
    private const string MaterialPath = "Assets/Art/Models/SwampApproach/Grass07/Grass07_Detail.mat";
    private const string CombinedMeshPath = "Assets/Art/Models/SwampApproach/Grass07/Grass07_DetailMesh.asset";
    private const string LargeMeshPath = "Assets/Art/Models/SwampApproach/Grass07/Grass07_DetailMesh_Large.asset";
    private const string PrefabPath = "Assets/Art/Models/SwampApproach/Grass07/Grass07_Detail.prefab";
    private const string LargePrefabPath = "Assets/Art/Models/SwampApproach/Grass07/Grass07_Detail_Large.prefab";

    [MenuItem("Dungeon Knight 3D/Swamp/Build Grass07 Detail Prefab")]
    public static void Build()
    {
        ConfigureTextureImporters();

        Material material = CreateOrUpdateMaterial();
        GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>(SourceModelPath);
        if (source == null)
        {
            Debug.LogError($"Grass07 source model not found at {SourceModelPath}");
            return;
        }

        Mesh combinedMesh = CreateOrUpdateCombinedMesh(source, CombinedMeshPath, "Grass07_DetailMesh", Vector3.one);
        if (combinedMesh == null)
        {
            Debug.LogError($"Grass07 source model has no mesh filters at {SourceModelPath}");
            return;
        }

        Mesh largeMesh = CreateOrUpdateCombinedMesh(
            source,
            LargeMeshPath,
            "Grass07_DetailMesh_Large",
            new Vector3(2.15f, 1.85f, 2.15f));

        BuildPrefab("Grass07_Detail", combinedMesh, material, PrefabPath);
        BuildPrefab("Grass07_Detail_Large", largeMesh, material, LargePrefabPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Built Grass07 terrain detail prefabs at {PrefabPath} and {LargePrefabPath}");
    }

    private static void BuildPrefab(string name, Mesh mesh, Material material, string prefabPath)
    {
        GameObject instance = new GameObject(name);
        MeshFilter meshFilter = instance.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = instance.AddComponent<MeshRenderer>();

        meshFilter.sharedMesh = mesh;
        meshRenderer.sharedMaterial = material;
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = true;

        Directory.CreateDirectory(Path.GetDirectoryName(prefabPath));
        PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
        Object.DestroyImmediate(instance);
    }

    private static void ConfigureTextureImporters()
    {
        TextureImporter diffuseImporter = AssetImporter.GetAtPath(DiffusePath) as TextureImporter;
        if (diffuseImporter != null)
        {
            diffuseImporter.textureType = TextureImporterType.Default;
            diffuseImporter.alphaIsTransparency = true;
            diffuseImporter.mipmapEnabled = true;
            diffuseImporter.SaveAndReimport();
        }

        TextureImporter normalImporter = AssetImporter.GetAtPath(NormalPath) as TextureImporter;
        if (normalImporter != null)
        {
            normalImporter.textureType = TextureImporterType.NormalMap;
            normalImporter.mipmapEnabled = true;
            normalImporter.SaveAndReimport();
        }
    }

    private static Material CreateOrUpdateMaterial()
    {
        Material material = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
        if (material == null)
        {
            material = new Material(Shader.Find("Standard"));
            AssetDatabase.CreateAsset(material, MaterialPath);
        }

        Texture2D diffuse = AssetDatabase.LoadAssetAtPath<Texture2D>(DiffusePath);
        Texture2D normal = AssetDatabase.LoadAssetAtPath<Texture2D>(NormalPath);

        material.shader = Shader.Find("Standard");
        material.SetTexture("_MainTex", diffuse);
        material.SetTexture("_BumpMap", normal);
        material.SetColor("_Color", new Color(0.78f, 0.72f, 0.56f, 1f));
        material.SetFloat("_Cutoff", 0.35f);
        material.SetFloat("_Glossiness", 0.08f);
        material.SetFloat("_Metallic", 0f);
        material.SetFloat("_BumpScale", 0.45f);
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

        Mesh combined = new Mesh
        {
            name = meshName
        };
        combined.CombineMeshes(combines, true, true, false);
        combined.RecalculateBounds();
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
}
