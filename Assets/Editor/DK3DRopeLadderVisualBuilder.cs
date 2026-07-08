using System.IO;
using UnityEditor;
using UnityEngine;

public static class DK3DRopeLadderVisualBuilder
{
    private const string MenuPath = "Dungeon Knight 3D/Build Proper Rope Ladder Visual";
    private const string ToolMenuPath = "Dungeon Knight 3D/Rope Ladder Tool";
    private const string OriginalName = "Manual Rope Ladder To Upper Opening";
    private const string OldImageFaceName = "Manual Rope Ladder Image Face";
    private const string VisualRootName = "Manual Rope Ladder 3D Game Visual";
    private const string MaterialsFolder = "Assets/Art/Materials/Ladders";
    private const string WoodMaterialPath = MaterialsFolder + "/DK3D_RopeLadder_GameWood.mat";
    private const string RopeMaterialPath = MaterialsFolder + "/DK3D_RopeLadder_GameRope.mat";
    private const string WoodTexturePath = "Assets/Art/Textures/Ladders/rope_ladder_wood_albedo.png";
    private const string RopeTexturePath = "Assets/Art/Textures/Ladders/rope_ladder_rope_albedo.png";
    private const int DefaultStepCount = 18;
    private const float DefaultHeight = 14.3f;
    private const float DefaultWidth = 1.38f;
    private const float DefaultPlankThickness = 0.18f;
    private const float DefaultPlankDepth = 0.28f;
    private const float DefaultRopeDiameter = 0.045f;

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

        var root = CreateLadderRoot(VisualRootName, original.transform.position + new Vector3(0f, 0f, -0.02f), original.transform.rotation);
        BuildInto(root.transform, DefaultHeight, DefaultWidth, DefaultStepCount);

        Selection.activeGameObject = root;
        EditorUtility.SetDirty(root);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(root.scene);
        Undo.CollapseUndoOperations(undoGroup);
        Debug.Log("Built proper 3D rope ladder visual. Original collider object kept; old flat image face removed.");
    }

    [MenuItem(ToolMenuPath)]
    private static void OpenTool()
    {
        RopeLadderToolWindow.ShowWindow();
    }

    public static GameObject CreateCustomLadder(string rootName, Vector3 position, Quaternion rotation, float height, float width, int stepCount)
    {
        var root = CreateLadderRoot(rootName, position, rotation);
        BuildInto(root.transform, height, width, stepCount);
        Selection.activeGameObject = root;
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(root.scene);
        return root;
    }

    public static void RebuildSelectedLadder(GameObject root, float height, float width, int stepCount)
    {
        if (root == null)
        {
            return;
        }

        Undo.RegisterFullObjectHierarchyUndo(root, "Rebuild rope ladder");
        ClearChildren(root.transform);
        BuildInto(root.transform, height, width, stepCount);
        EditorUtility.SetDirty(root);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(root.scene);
    }

    private static GameObject CreateLadderRoot(string rootName, Vector3 position, Quaternion rotation)
    {
        var root = new GameObject(rootName);
        Undo.RegisterCreatedObjectUndo(root, "Create rope ladder visual");
        root.transform.SetPositionAndRotation(position, rotation);
        root.transform.localScale = Vector3.one;
        return root;
    }

    private static void BuildInto(Transform root, float height, float width, int stepCount)
    {
        var wood = LoadOrCreateMaterial(WoodMaterialPath, "DK3D_RopeLadder_GameWood", new Color(0.72f, 0.56f, 0.38f), 0.22f, WoodTexturePath);
        var rope = LoadOrCreateMaterial(RopeMaterialPath, "DK3D_RopeLadder_GameRope", new Color(0.66f, 0.58f, 0.46f), 0.34f, RopeTexturePath);

        stepCount = Mathf.Max(2, stepCount);
        height = Mathf.Max(0.75f, height);
        width = Mathf.Max(0.25f, width);

        var railX = width * 0.5f;
        var startY = -height * 0.5f;
        var spacing = height / (stepCount - 1);

        CreateCylinder(root, "Left Long Rope Rail", new Vector3(-railX, 0f, -0.015f), Quaternion.identity, DefaultRopeDiameter, height + 0.9f, rope);
        CreateCylinder(root, "Right Long Rope Rail", new Vector3(railX, 0f, -0.015f), Quaternion.identity, DefaultRopeDiameter, height + 0.9f, rope);

        for (var i = 0; i < stepCount; i++)
        {
            var y = startY + spacing * i;
            var wobble = Mathf.Sin(i * 1.73f) * 0.025f;
            var rungLength = width + 0.32f + Mathf.Sin(i * 0.9f) * 0.04f;
            var angle = Mathf.Sin(i * 0.65f) * 2.5f;

            CreateBox(root, $"Squared Wooden Step {i + 1:00}", new Vector3(wobble, y, -0.05f), Quaternion.Euler(0f, 0f, angle), new Vector3(rungLength, DefaultPlankThickness, DefaultPlankDepth), wood);
        }
    }

    private static void ClearChildren(Transform root)
    {
        for (var i = root.childCount - 1; i >= 0; i--)
        {
            Undo.DestroyObjectImmediate(root.GetChild(i).gameObject);
        }
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

    private sealed class RopeLadderToolWindow : EditorWindow
    {
        private string ladderName = "Rope Ladder";
        private float height = DefaultHeight;
        private float width = DefaultWidth;
        private int stepCount = DefaultStepCount;
        private Vector3 position = Vector3.zero;
        private Vector3 rotation = Vector3.zero;

        public static void ShowWindow()
        {
            var window = GetWindow<RopeLadderToolWindow>("Rope Ladder Tool");
            window.minSize = new Vector2(280f, 250f);
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Create Rope Ladder", EditorStyles.boldLabel);
            ladderName = EditorGUILayout.TextField("Name", ladderName);
            height = EditorGUILayout.Slider("Height", height, 1f, 30f);
            width = EditorGUILayout.Slider("Width", width, 0.5f, 4f);
            stepCount = EditorGUILayout.IntSlider("Steps", stepCount, 2, 40);
            position = EditorGUILayout.Vector3Field("Position", position);
            rotation = EditorGUILayout.Vector3Field("Rotation", rotation);

            if (GUILayout.Button("Create New Ladder"))
            {
                CreateCustomLadder(ladderName, position, Quaternion.Euler(rotation), height, width, stepCount);
            }

            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField("Selected Ladder", EditorStyles.boldLabel);

            using (new EditorGUI.DisabledScope(Selection.activeGameObject == null))
            {
                if (GUILayout.Button("Use Selected Transform"))
                {
                    position = Selection.activeGameObject.transform.position;
                    rotation = Selection.activeGameObject.transform.eulerAngles;
                }

                if (GUILayout.Button("Rebuild Selected Ladder"))
                {
                    if (!IsLikelyGeneratedLadder(Selection.activeGameObject))
                    {
                        EditorUtility.DisplayDialog("Rope Ladder Tool", "Select a generated rope ladder root before rebuilding.", "OK");
                        return;
                    }

                    RebuildSelectedLadder(Selection.activeGameObject, height, width, stepCount);
                }
            }
        }

        private static bool IsLikelyGeneratedLadder(GameObject selected)
        {
            if (selected == null)
            {
                return false;
            }

            return selected.name.Contains("Rope Ladder")
                || selected.transform.Find("Left Long Rope Rail") != null
                || selected.transform.Find("Squared Wooden Step 01") != null;
        }
    }
}
