using System.IO;
using UnityEditor;
using UnityEngine;

public static class Grass07DetailPrefabBuilder
{
    private const string SourceModelPath = "Assets/Art/Models/SwampApproach/Grass07/grass_07.FBX";
    private const string DiffusePath = "Assets/Art/Models/SwampApproach/Grass07/diffus.tga";
    private const string NormalPath = "Assets/Art/Models/SwampApproach/Grass07/normal.tga";
    private const string MaterialPath = "Assets/Art/Models/SwampApproach/Grass07/Grass07_Detail.mat";
    private const string PrefabPath = "Assets/Art/Models/SwampApproach/Grass07/Grass07_Detail.prefab";

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

        GameObject instance = Object.Instantiate(source);
        instance.name = "Grass07_Detail";
        instance.transform.position = Vector3.zero;
        instance.transform.rotation = Quaternion.identity;
        instance.transform.localScale = Vector3.one;

        foreach (Renderer renderer in instance.GetComponentsInChildren<Renderer>(true))
        {
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = true;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(PrefabPath));
        PrefabUtility.SaveAsPrefabAsset(instance, PrefabPath);
        Object.DestroyImmediate(instance);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Built Grass07 terrain detail prefab at {PrefabPath}");
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
}
