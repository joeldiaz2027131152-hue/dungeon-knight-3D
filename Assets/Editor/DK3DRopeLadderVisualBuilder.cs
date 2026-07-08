using System.IO;
using UnityEditor;
using UnityEngine;

public static class DK3DRopeLadderVisualBuilder
{
    private const string MenuPath = "Dungeon Knight 3D/Build Proper Rope Ladder Visual";
    private const string OriginalName = "Manual Rope Ladder To Upper Opening";
    private const string OldImageFaceName = "Manual Rope Ladder Image Face";
    private const string VisualRootName = "Manual Rope Ladder 3D Game Visual";
    private const string MaterialsFolder = "Assets/Art/Materials/Ladders";
    private const string WoodMaterialPath = MaterialsFolder + "/DK3D_RopeLadder_GameWood.mat";
    private const string RopeMaterialPath = MaterialsFolder + "/DK3D_RopeLadder_GameRope.mat";
    private const string WoodTexturePath = "Assets/Art/Textures/Ladders/rope_ladder_wood_albedo.png";
    private const string RopeTexturePath = "Assets/Art/Textures/Ladders/rope_ladder_rope_albedo.png";

    [MenuItem(MenuPath)]
    public static void Build()
    {
        var original = GameObject.Find(OriginalName);
        if (original == null)
        {
            Debug.LogError($"Could not find {OriginalName}.");
            return;
        }

        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName("Build proper rope ladder visual");
        var undoGroup = Undo.GetCurrentGroup();

        DestroyIfPresent(OldImageFaceName);
        DestroyIfPresent(VisualRootName);

        var originalRenderer = original.GetComponent<MeshRenderer>();
        if (originalRenderer != null)
        {
            Undo.RecordObject(originalRenderer, "Hide old rope ladder renderer");
            originalRenderer.enabled = false;
            EditorUtility.SetDirty(originalRenderer);
        }

        var wood = LoadOrCreateMaterial(WoodMaterialPath, "DK3D_RopeLadder_GameWood", new Color(0.72f, 0.56f, 0.38f), 0.22f, WoodTexturePath);
        var rope = LoadOrCreateMaterial(RopeMaterialPath, "DK3D_RopeLadder_GameRope", new Color(0.66f, 0.58f, 0.46f), 0.34f, RopeTexturePath);

        var root = new GameObject(VisualRootName);
        Undo.RegisterCreatedObjectUndo(root, "Create rope ladder visual");
        root.transform.SetPositionAndRotation(original.transform.position + new Vector3(0f, 0f, -0.02f), original.transform.rotation);
        root.transform.localScale = Vector3.one;

        const int rungCount = 18;
        const float height = 14.3f;
        const float width = 1.38f;
        const float railX = width * 0.5f;
        const float plankThickness = 0.18f;
        const float plankDepth = 0.28f;
        const float ropeDiameter = 0.045f;
        const float startY = -height * 0.5f;
        const float spacing = height / (rungCount - 1);

        CreateCylinder(root.transform, "Left Long Rope Rail", new Vector3(-railX, 0f, -0.015f), Quaternion.identity, ropeDiameter, height + 0.9f, rope);
        CreateCylinder(root.transform, "Right Long Rope Rail", new Vector3(railX, 0f, -0.015f), Quaternion.identity, ropeDiameter, height + 0.9f, rope);

        for (var i = 0; i < rungCount; i++)
        {
            var y = startY + spacing * i;
            var wobble = Mathf.Sin(i * 1.73f) * 0.025f;
            var rungLength = width + 0.32f + Mathf.Sin(i * 0.9f) * 0.04f;
            var angle = Mathf.Sin(i * 0.65f) * 2.5f;

            CreateBox(root.transform, $"Squared Wooden Step {i + 1:00}", new Vector3(wobble, y, -0.05f), Quaternion.Euler(0f, 0f, angle), new Vector3(rungLength, plankThickness, plankDepth), wood);
        }

        Selection.activeGameObject = root;
        EditorUtility.SetDirty(root);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(root.scene);
        Undo.CollapseUndoOperations(undoGroup);
        Debug.Log("Built proper 3D rope ladder visual. Original collider object kept; old flat image face removed.");
    }

    private static GameObject CreateBox(Transform parent, string name, Vector3 localPosition, Quaternion localRotation, Vector3 localScale, Material material)
    {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Undo.RegisterCreatedObjectUndo(obj, "Create ladder box");
        obj.name = name;
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = localPosition;
        obj.transform.localRotation = localRotation;
        obj.transform.localScale = localScale;
        AssignMaterial(obj, material);
        Object.DestroyImmediate(obj.GetComponent<Collider>());
        return obj;
    }

    private static GameObject CreateCylinder(Transform parent, string name, Vector3 localPosition, Quaternion localRotation, float diameter, float length, Material material)
    {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Undo.RegisterCreatedObjectUndo(obj, "Create ladder cylinder");
        obj.name = name;
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = localPosition;
        obj.transform.localRotation = localRotation;
        obj.transform.localScale = new Vector3(diameter, length * 0.5f, diameter);
        AssignMaterial(obj, material);
        Object.DestroyImmediate(obj.GetComponent<Collider>());
        return obj;
    }

    private static void AssignMaterial(GameObject obj, Material material)
    {
        var renderer = obj.GetComponent<MeshRenderer>();
        if (renderer == null)
        {
            return;
        }

        Undo.RecordObject(renderer, "Assign ladder material");
        renderer.sharedMaterial = material;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        renderer.receiveShadows = true;
        EditorUtility.SetDirty(renderer);
    }

    private static void DestroyIfPresent(string objectName)
    {
        var existing = GameObject.Find(objectName);
        if (existing != null)
        {
            Undo.DestroyObjectImmediate(existing);
        }
    }

    private static Material LoadOrCreateMaterial(string path, string materialName, Color color, float smoothness, string texturePath)
    {
        Directory.CreateDirectory(MaterialsFolder);
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
        var texture = string.IsNullOrEmpty(texturePath) ? null : AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
        material.SetTexture("_MainTex", texture);
        material.SetTextureScale("_MainTex", texturePath == RopeTexturePath ? new Vector2(1f, 1.65f) : new Vector2(1.45f, 1f));
        EditorUtility.SetDirty(material);
        return material;
    }
}
