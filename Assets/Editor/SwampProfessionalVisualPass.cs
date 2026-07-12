using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SwampProfessionalVisualPass
{
    private const string ParentPath = "Linear Swamp Approach";
    private const string PassName = "Swamp Professional Atmosphere Pass";
    private const string MaterialFolder = "Assets/Art/Materials/SwampAtmosphere/ProfessionalPass";

    [MenuItem("Dungeon Knight 3D/Swamp/Add Professional Visual Pass")]
    public static void Generate()
    {
        var parent = GameObject.Find(ParentPath);
        if (!parent)
        {
            Debug.LogError($"Could not find {ParentPath}");
            return;
        }

        EnsureFolder("Assets/Art/Materials/SwampAtmosphere", "ProfessionalPass");
        ImproveExistingSwampMaterials();

        var pass = FindOrCreate(PassName, parent.transform);
        pass.transform.localPosition = Vector3.zero;
        pass.transform.localRotation = Quaternion.identity;
        pass.transform.localScale = Vector3.one;

        var mats = CreateMaterials();
        CreateLowFog(pass.transform, mats.fog);
        CreateMirePatches(pass.transform, mats.deepMud, mats.algae, mats.waterSheen);
        CreateRootSilhouettes(pass.transform, mats.root);
        CreateColdCandleShrines(pass.transform, mats.candleWax, mats.soulFlame, mats.rustedMetal, mats.bone);
        CreateSwampMotes(pass.transform, mats.soulFlame, mats.warmEmber);
        CreateStoryRemnants(pass.transform, mats.rustedMetal, mats.bone, mats.deadLeaves);
        CreateRiverScum(pass.transform, mats.algae);

        EditorUtility.SetDirty(pass);
        var scene = pass.scene;
        if (scene.IsValid())
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Added additive professional swamp atmosphere pass. Existing swamp objects were preserved.");
    }

    private static SwampMaterials CreateMaterials()
    {
        return new SwampMaterials
        {
            fog = CreateTransparentMaterial("SwampLowFogVeil", new Color(0.22f, 0.34f, 0.32f, 0.19f), 0.02f, 3020),
            deepMud = CreateStandardMaterial("SwampDeepMudPatch", new Color(0.035f, 0.05f, 0.04f, 1f), 0.2f),
            algae = CreateTransparentMaterial("SwampRotAlgaeFilm", new Color(0.08f, 0.18f, 0.055f, 0.5f), 0.14f, 3005),
            waterSheen = CreateTransparentMaterial("SwampBlackWaterSheen", new Color(0.018f, 0.08f, 0.075f, 0.44f), 0.72f, 3010),
            root = CreateStandardMaterial("SwampAncientRoot", new Color(0.034f, 0.026f, 0.021f, 1f), 0.09f),
            candleWax = CreateStandardMaterial("SwampBoneWax", new Color(0.58f, 0.54f, 0.42f, 1f), 0.22f),
            rustedMetal = CreateStandardMaterial("SwampRustedIron", new Color(0.25f, 0.095f, 0.045f, 1f), 0.28f),
            bone = CreateStandardMaterial("SwampOldBone", new Color(0.42f, 0.39f, 0.31f, 1f), 0.18f),
            deadLeaves = CreateStandardMaterial("SwampDeadLeafMat", new Color(0.11f, 0.075f, 0.035f, 1f), 0.12f),
            soulFlame = CreateEmissiveMaterial("SwampSoulFlame", new Color(0.12f, 0.88f, 0.72f, 1f), 1.9f),
            warmEmber = CreateEmissiveMaterial("SwampDyingEmber", new Color(1f, 0.36f, 0.08f, 1f), 1.4f)
        };
    }

    private static void ImproveExistingSwampMaterials()
    {
        ConfigureTexturedMaterial("Assets/Art/Materials/SwampApproach/DK3D_Swamp_Mud.mat", "Assets/Art/Textures/SwampApproach/swamp_mud_albedo.png", "Assets/Art/Textures/SwampApproach/swamp_mud_height.png", 0.035f, 4f);
        ConfigureTexturedMaterial("Assets/Art/Materials/SwampApproach/DK3D_Swamp_Moss.mat", "Assets/Art/Textures/SwampApproach/swamp_moss_albedo.png", "Assets/Art/Textures/SwampApproach/swamp_moss_height.png", 0.025f, 3f);
        ConfigureTexturedMaterial("Assets/Art/Materials/SwampApproach/DK3D_Ruined_Wet_Stone.mat", "Assets/Art/Textures/SwampApproach/wet_stone_albedo.png", "Assets/Art/Textures/SwampApproach/wet_stone_height.png", 0.02f, 2f);
        ConfigureTexturedMaterial("Assets/Art/Materials/SwampApproach/DK3D_Dead_Bark.mat", "Assets/Art/Textures/SwampApproach/dead_bark_albedo.png", "Assets/Art/Textures/SwampApproach/dead_bark_height.png", 0.03f, 2.5f);
        ConfigureTexturedMaterial("Assets/Art/Materials/SwampApproach/DK3D_Swamp_Water.mat", "Assets/Art/Textures/SwampApproach/swamp_water_albedo.png", "Assets/Art/Textures/SwampApproach/swamp_water_height.png", 0.012f, 5f);
    }

    private static void ConfigureTexturedMaterial(string materialPath, string albedoPath, string heightPath, float parallax, float tiling)
    {
        var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if (!material) return;

        var albedo = AssetDatabase.LoadAssetAtPath<Texture2D>(albedoPath);
        if (albedo)
        {
            material.SetTexture("_MainTex", albedo);
            material.SetTextureScale("_MainTex", new Vector2(tiling, tiling));
        }

        var height = AssetDatabase.LoadAssetAtPath<Texture2D>(heightPath);
        if (height)
        {
            material.SetTexture("_ParallaxMap", height);
            material.SetFloat("_Parallax", parallax);
            material.EnableKeyword("_PARALLAXMAP");
        }

        EditorUtility.SetDirty(material);
    }

    private static void CreateLowFog(Transform root, Material fog)
    {
        var group = FindOrCreate("Layered Low Fog", root);
        var bands = new[]
        {
            new FogBand("Low Fog Backwater 01", -8f, 83.1f, 142f, 34f, 0.05f, 13f, 0f),
            new FogBand("Low Fog Backwater 02", 7f, 83.18f, 165f, 30f, 0.05f, 16f, -7f),
            new FogBand("Low Fog Mid Causeway 01", -3f, 83.14f, 194f, 38f, 0.05f, 11f, 6f),
            new FogBand("Low Fog Around Center Tree", -11f, 84.0f, 220f, 32f, 0.06f, 18f, -18f),
            new FogBand("Low Fog Front Pool", 4f, 83.15f, 246f, 36f, 0.05f, 14f, 9f),
            new FogBand("Low Fog Exit Sluice", -5f, 83.12f, 284f, 35f, 0.05f, 13f, -5f),
            new FogBand("Low Fog Far Exit", 6f, 83.16f, 315f, 28f, 0.05f, 12f, 4f)
        };

        foreach (var band in bands)
        {
            var fogObject = CreateBox(band.name, group.transform, new Vector3(band.x, band.y, band.z), new Vector3(band.width, band.height, band.depth), fog);
            fogObject.transform.rotation = Quaternion.Euler(0f, band.yaw, 0f);
        }
    }

    private static void CreateMirePatches(Transform root, Material mud, Material algae, Material water)
    {
        var group = FindOrCreate("Mire Surface Detail", root);
        var patches = new[]
        {
            new Patch("Black Water Pocket 01", -12f, 82.86f, 134f, 7.2f, 4.8f, -12f, water),
            new Patch("Rot Algae Shelf 01", 10.5f, 82.9f, 151f, 5.8f, 3.4f, 21f, algae),
            new Patch("Deep Mud Pull 01", -8f, 82.88f, 176f, 8.5f, 3.8f, -9f, mud),
            new Patch("Black Water Pocket 02", 12f, 82.89f, 199f, 7.8f, 4.9f, 17f, water),
            new Patch("Rot Algae Shelf 02", -22f, 82.93f, 211f, 8f, 5.2f, 8f, algae),
            new Patch("Deep Mud Pull 02", 11f, 82.88f, 232f, 9f, 4.2f, -24f, mud),
            new Patch("Tree Root Water Mirror", -12.5f, 83.02f, 221f, 12f, 7.2f, 35f, water),
            new Patch("Rot Algae Shelf 03", -13f, 82.91f, 259f, 6f, 3.8f, -18f, algae),
            new Patch("Deep Mud Pull 03", 9f, 82.87f, 284f, 8.2f, 4.6f, 13f, mud),
            new Patch("Black Water Pocket 03", -8f, 82.9f, 308f, 7.2f, 4.4f, -30f, water)
        };

        foreach (var patch in patches)
        {
            var obj = FindOrCreate(patch.name, group.transform);
            obj.transform.position = new Vector3(patch.x, patch.y, patch.z);
            obj.transform.rotation = Quaternion.Euler(0f, patch.yaw, 0f);
            obj.transform.localScale = Vector3.one;
            EnsureIrregularDisc(obj, patch.width, patch.depth, 14);
            obj.GetComponent<MeshRenderer>().sharedMaterial = patch.material;
        }
    }

    private static void CreateRootSilhouettes(Transform root, Material material)
    {
        var group = FindOrCreate("Ancient Root Silhouettes", root);
        CreateCylinderBetween("Center Root Rib 01", group.transform, new Vector3(-13f, 83.2f, 219f), new Vector3(-25f, 83.05f, 209f), 0.38f, material);
        CreateCylinderBetween("Center Root Rib 02", group.transform, new Vector3(-12f, 83.25f, 222f), new Vector3(3f, 83.08f, 229f), 0.32f, material);
        CreateCylinderBetween("Center Root Rib 03", group.transform, new Vector3(-15f, 83.25f, 216f), new Vector3(-8f, 83.02f, 196f), 0.28f, material);
        CreateCylinderBetween("Sunken Root Bridge 01", group.transform, new Vector3(7f, 83.05f, 181f), new Vector3(-6f, 82.98f, 189f), 0.24f, material);
        CreateCylinderBetween("Sunken Root Bridge 02", group.transform, new Vector3(-6f, 83.06f, 254f), new Vector3(10f, 83.0f, 243f), 0.26f, material);
        CreateCylinderBetween("Exit Root Claw 01", group.transform, new Vector3(-12f, 83.05f, 303f), new Vector3(2f, 83.0f, 315f), 0.22f, material);

        for (var i = 0; i < 10; i++)
        {
            var z = 142f + i * 17.5f;
            var x = i % 2 == 0 ? -18f : 17f;
            CreateCylinderBetween($"Hanging Vine Silhouette {i + 1:00}", group.transform, new Vector3(x, 87.5f, z), new Vector3(x + (i % 3 - 1) * 0.7f, 83.4f, z + 1.6f), 0.055f, material);
        }
    }

    private static void CreateColdCandleShrines(Transform root, Material wax, Material flame, Material rust, Material bone)
    {
        var group = FindOrCreate("Cold Candle Shrines", root);
        var centers = new[]
        {
            new Vector3(-19.5f, 83.05f, 211.5f),
            new Vector3(4.4f, 83.05f, 233.2f),
            new Vector3(-7.5f, 83.04f, 248.5f),
            new Vector3(9.5f, 83.04f, 176.5f)
        };

        for (var shrine = 0; shrine < centers.Length; shrine++)
        {
            var shrineRoot = FindOrCreate($"Cold Shrine {shrine + 1:00}", group.transform);
            shrineRoot.transform.position = centers[shrine];

            for (var i = 0; i < 5; i++)
            {
                var angle = i / 5f * Mathf.PI * 2f + shrine * 0.7f;
                var offset = new Vector3(Mathf.Cos(angle) * (0.42f + i * 0.05f), 0f, Mathf.Sin(angle) * (0.35f + i * 0.04f));
                var candle = CreateCylinder($"Cold Shrine {shrine + 1:00} Candle {i + 1:00}", shrineRoot.transform, centers[shrine] + offset + Vector3.up * 0.16f, 0.07f, 0.32f + (i % 3) * 0.08f, wax);
                candle.transform.localRotation = Quaternion.Euler((i % 2) * 3f, 0f, (i % 3 - 1) * 4f);

                var wick = CreateSphere($"Cold Shrine {shrine + 1:00} Soul Wick {i + 1:00}", shrineRoot.transform, candle.transform.position + Vector3.up * (0.22f + (i % 3) * 0.04f), 0.08f, flame);
                ConfigureOptionalLight(wick, i == 2, new Color(0.08f, 0.85f, 0.66f), 0.55f, 3.2f);
            }

            CreateCylinderBetween($"Cold Shrine {shrine + 1:00} Rusted Blade", shrineRoot.transform, centers[shrine] + new Vector3(-0.55f, 0.12f, -0.2f), centers[shrine] + new Vector3(0.32f, 0.78f, 0.18f), 0.055f, rust);
            CreateCylinderBetween($"Cold Shrine {shrine + 1:00} Half Buried Bone", shrineRoot.transform, centers[shrine] + new Vector3(0.3f, 0.09f, -0.58f), centers[shrine] + new Vector3(0.9f, 0.08f, -0.18f), 0.07f, bone);
        }
    }

    private static void CreateSwampMotes(Transform root, Material soul, Material ember)
    {
        var group = FindOrCreate("Drifting Soul Motes", root);
        for (var i = 0; i < 28; i++)
        {
            var t = i / 27f;
            var z = Mathf.Lerp(138f, 318f, t);
            var x = Mathf.Sin(i * 1.73f) * 14f + Mathf.Cos(i * 0.61f) * 4f;
            var y = 84.4f + Mathf.Sin(i * 0.83f) * 1.6f + (i % 5) * 0.12f;
            var material = i % 7 == 0 ? ember : soul;
            var mote = CreateSphere($"Drifting Mote {i + 1:00}", group.transform, new Vector3(x, y, z), i % 7 == 0 ? 0.09f : 0.065f, material);
            ConfigureOptionalLight(mote, i % 6 == 0, i % 7 == 0 ? new Color(1f, 0.35f, 0.08f) : new Color(0.08f, 0.82f, 0.68f), i % 7 == 0 ? 0.18f : 0.14f, i % 7 == 0 ? 1.4f : 1.2f);
        }
    }

    private static void CreateStoryRemnants(Transform root, Material rust, Material bone, Material leaves)
    {
        var group = FindOrCreate("Half Sunken Story Remnants", root);
        var positions = new[]
        {
            new Vector3(-10f, 83.03f, 157f),
            new Vector3(13f, 83.03f, 203f),
            new Vector3(-19f, 83.04f, 233f),
            new Vector3(7f, 83.03f, 276f),
            new Vector3(-11f, 83.03f, 304f)
        };

        for (var i = 0; i < positions.Length; i++)
        {
            var p = positions[i];
            CreateCylinderBetween($"Sunken Rusted Spear {i + 1:00}", group.transform, p + new Vector3(-0.5f, 0.08f, -0.3f), p + new Vector3(1.1f, 0.12f, 0.65f), 0.045f, rust);
            CreateCylinderBetween($"Sunken Rib Bone {i + 1:00}", group.transform, p + new Vector3(0.15f, 0.11f, -0.42f), p + new Vector3(0.92f, 0.12f, 0.05f), 0.06f, bone);
            CreateCylinderBetween($"Sunken Rib Bone {i + 1:00} B", group.transform, p + new Vector3(0.02f, 0.11f, 0.1f), p + new Vector3(0.72f, 0.1f, 0.48f), 0.052f, bone);

            for (var leaf = 0; leaf < 6; leaf++)
            {
                var angle = (leaf * 59f + i * 21f) * Mathf.Deg2Rad;
                var leafObject = CreateBox($"Dead Leaf Cluster {i + 1:00}-{leaf + 1:00}", group.transform, p + new Vector3(Mathf.Cos(angle) * 1.1f, 0.035f, Mathf.Sin(angle) * 0.75f), new Vector3(0.42f, 0.018f, 0.13f), leaves);
                leafObject.transform.rotation = Quaternion.Euler(0f, leaf * 37f + i * 9f, 0f);
            }
        }
    }

    private static void CreateRiverScum(Transform root, Material algae)
    {
        var group = FindOrCreate("River Edge Algae Scum", root);
        var strips = new[]
        {
            new Patch("Algae Scum Back Left", -5.5f, 82.98f, 151f, 9f, 1.2f, -8f, algae),
            new Patch("Algae Scum Back Right", 1.8f, 82.99f, 171f, 10f, 1.1f, -35f, algae),
            new Patch("Algae Scum Loop North", -18f, 83.02f, 204f, 8.5f, 1.2f, 18f, algae),
            new Patch("Algae Scum Loop East", 8f, 83.03f, 222f, 8f, 1.15f, 82f, algae),
            new Patch("Algae Scum Loop South", -10f, 83.03f, 242f, 9f, 1.2f, -14f, algae),
            new Patch("Algae Scum Exit", 4.8f, 82.99f, 291f, 10f, 1.2f, 4f, algae)
        };

        foreach (var strip in strips)
        {
            var obj = FindOrCreate(strip.name, group.transform);
            obj.transform.position = new Vector3(strip.x, strip.y, strip.z);
            obj.transform.rotation = Quaternion.Euler(0f, strip.yaw, 0f);
            EnsureIrregularDisc(obj, strip.width, strip.depth, 10);
            obj.GetComponent<MeshRenderer>().sharedMaterial = strip.material;
        }
    }

    private static GameObject CreateBox(string name, Transform parent, Vector3 position, Vector3 scale, Material material)
    {
        var obj = FindOrCreatePrimitive(name, parent, PrimitiveType.Cube);
        obj.transform.position = position;
        obj.transform.localScale = scale;
        AssignMaterial(obj, material);
        RemoveCollider(obj);
        return obj;
    }

    private static GameObject CreateSphere(string name, Transform parent, Vector3 position, float radius, Material material)
    {
        var obj = FindOrCreatePrimitive(name, parent, PrimitiveType.Sphere);
        obj.transform.position = position;
        obj.transform.localScale = Vector3.one * radius;
        AssignMaterial(obj, material);
        RemoveCollider(obj);
        return obj;
    }

    private static GameObject CreateCylinder(string name, Transform parent, Vector3 position, float radius, float height, Material material)
    {
        var obj = FindOrCreatePrimitive(name, parent, PrimitiveType.Cylinder);
        obj.transform.position = position;
        obj.transform.localScale = new Vector3(radius, height * 0.5f, radius);
        AssignMaterial(obj, material);
        RemoveCollider(obj);
        return obj;
    }

    private static GameObject CreateCylinderBetween(string name, Transform parent, Vector3 start, Vector3 end, float radius, Material material)
    {
        var obj = FindOrCreatePrimitive(name, parent, PrimitiveType.Cylinder);
        var midpoint = (start + end) * 0.5f;
        var direction = end - start;
        obj.transform.position = midpoint;
        obj.transform.rotation = Quaternion.FromToRotation(Vector3.up, direction.normalized);
        obj.transform.localScale = new Vector3(radius, direction.magnitude * 0.5f, radius);
        AssignMaterial(obj, material);
        RemoveCollider(obj);
        return obj;
    }

    private static GameObject FindOrCreatePrimitive(string name, Transform parent, PrimitiveType primitive)
    {
        var existing = parent.Find(name);
        if (existing) return existing.gameObject;

        var obj = GameObject.CreatePrimitive(primitive);
        obj.name = name;
        obj.transform.SetParent(parent, true);
        Undo.RegisterCreatedObjectUndo(obj, $"Create {name}");
        return obj;
    }

    private static GameObject FindOrCreate(string name, Transform parent)
    {
        var existing = parent.Find(name);
        if (existing) return existing.gameObject;

        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        Undo.RegisterCreatedObjectUndo(obj, $"Create {name}");
        return obj;
    }

    private static void EnsureIrregularDisc(GameObject obj, float width, float depth, int segments)
    {
        var filter = obj.GetComponent<MeshFilter>();
        if (!filter) filter = obj.AddComponent<MeshFilter>();
        var renderer = obj.GetComponent<MeshRenderer>();
        if (!renderer) renderer = obj.AddComponent<MeshRenderer>();

        var vertices = new List<Vector3> { Vector3.zero };
        var uvs = new List<Vector2> { new Vector2(0.5f, 0.5f) };
        var triangles = new List<int>();

        for (var i = 0; i < segments; i++)
        {
            var angle = i / (float)segments * Mathf.PI * 2f;
            var wobble = 0.82f + Mathf.Sin(i * 1.71f) * 0.12f + Mathf.Cos(i * 0.83f) * 0.07f;
            var x = Mathf.Cos(angle) * width * 0.5f * wobble;
            var z = Mathf.Sin(angle) * depth * 0.5f * (0.9f + Mathf.Cos(i * 1.37f) * 0.08f);
            vertices.Add(new Vector3(x, 0f, z));
            uvs.Add(new Vector2(0.5f + x / width, 0.5f + z / depth));
        }

        for (var i = 1; i <= segments; i++)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i == segments ? 1 : i + 1);
        }

        var mesh = new Mesh
        {
            name = obj.name + " Mesh"
        };
        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        filter.sharedMesh = mesh;
        RemoveCollider(obj);
    }

    private static void AssignMaterial(GameObject obj, Material material)
    {
        var renderer = obj.GetComponent<Renderer>();
        if (renderer) renderer.sharedMaterial = material;
    }

    private static void ConfigureOptionalLight(GameObject obj, bool enabled, Color color, float intensity, float range)
    {
        var light = obj.GetComponent<Light>();
        if (!light) light = obj.AddComponent<Light>();
        light.enabled = enabled;
        light.type = LightType.Point;
        light.color = color;
        light.intensity = intensity;
        light.range = range;
        light.shadows = LightShadows.None;
    }

    private static void RemoveCollider(GameObject obj)
    {
        var collider = obj.GetComponent<Collider>();
        if (collider) Object.DestroyImmediate(collider);
    }

    private static Material CreateStandardMaterial(string name, Color color, float smoothness)
    {
        var material = LoadOrCreateMaterial(name, "Standard");
        material.SetColor("_Color", color);
        material.SetFloat("_Glossiness", smoothness);
        material.SetFloat("_Metallic", 0f);
        SetOpaque(material);
        EditorUtility.SetDirty(material);
        return material;
    }

    private static Material CreateTransparentMaterial(string name, Color color, float smoothness, int renderQueue)
    {
        var material = LoadOrCreateMaterial(name, "Standard");
        material.SetColor("_Color", color);
        material.SetFloat("_Glossiness", smoothness);
        material.SetFloat("_Metallic", 0f);
        SetTransparent(material, renderQueue);
        EditorUtility.SetDirty(material);
        return material;
    }

    private static Material CreateEmissiveMaterial(string name, Color color, float intensity)
    {
        var material = LoadOrCreateMaterial(name, "Standard");
        material.SetColor("_Color", color);
        material.SetColor("_EmissionColor", color * intensity);
        material.SetFloat("_Glossiness", 0.35f);
        material.SetFloat("_Metallic", 0f);
        material.EnableKeyword("_EMISSION");
        material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        SetOpaque(material);
        EditorUtility.SetDirty(material);
        return material;
    }

    private static Material LoadOrCreateMaterial(string name, string shaderName)
    {
        var path = $"{MaterialFolder}/{name}.mat";
        var material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material) return material;

        material = new Material(Shader.Find(shaderName))
        {
            name = name
        };
        AssetDatabase.CreateAsset(material, path);
        return material;
    }

    private static void SetOpaque(Material material)
    {
        material.SetFloat("_Mode", 0f);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        material.SetInt("_ZWrite", 1);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 2000;
    }

    private static void SetTransparent(Material material, int renderQueue)
    {
        material.SetFloat("_Mode", 3f);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = renderQueue;
    }

    private static void EnsureFolder(string parentFolder, string childName)
    {
        var fullPath = parentFolder + "/" + childName;
        if (AssetDatabase.IsValidFolder(fullPath)) return;
        if (!AssetDatabase.IsValidFolder(parentFolder))
        {
            Directory.CreateDirectory(parentFolder);
            AssetDatabase.Refresh();
        }
        AssetDatabase.CreateFolder(parentFolder, childName);
    }

    private readonly struct FogBand
    {
        public readonly string name;
        public readonly float x;
        public readonly float y;
        public readonly float z;
        public readonly float width;
        public readonly float height;
        public readonly float depth;
        public readonly float yaw;

        public FogBand(string name, float x, float y, float z, float width, float height, float depth, float yaw)
        {
            this.name = name;
            this.x = x;
            this.y = y;
            this.z = z;
            this.width = width;
            this.height = height;
            this.depth = depth;
            this.yaw = yaw;
        }
    }

    private readonly struct Patch
    {
        public readonly string name;
        public readonly float x;
        public readonly float y;
        public readonly float z;
        public readonly float width;
        public readonly float depth;
        public readonly float yaw;
        public readonly Material material;

        public Patch(string name, float x, float y, float z, float width, float depth, float yaw, Material material)
        {
            this.name = name;
            this.x = x;
            this.y = y;
            this.z = z;
            this.width = width;
            this.depth = depth;
            this.yaw = yaw;
            this.material = material;
        }
    }

    private sealed class SwampMaterials
    {
        public Material fog;
        public Material deepMud;
        public Material algae;
        public Material waterSheen;
        public Material root;
        public Material candleWax;
        public Material soulFlame;
        public Material warmEmber;
        public Material rustedMetal;
        public Material bone;
        public Material deadLeaves;
    }
}
