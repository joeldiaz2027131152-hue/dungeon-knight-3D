#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DungeonKnight.Editor
{
    [InitializeOnLoad]
    public static class DungeonKnight3DWallCornerHandles
    {
        private const string EnabledKey = "DungeonKnight3D.WallCornerHandles.Enabled";
        private const float MinSize = 0.05f;

        private static bool enabled;

        static DungeonKnight3DWallCornerHandles()
        {
            enabled = EditorPrefs.GetBool(EnabledKey, false);
            SceneView.duringSceneGui += DrawSceneHandles;
        }

        [MenuItem("Tools/Dungeon Knight 3D/Wall Corner Handles")]
        public static void Toggle()
        {
            enabled = !enabled;
            EditorPrefs.SetBool(EnabledKey, enabled);
            SceneView.RepaintAll();
        }

        [MenuItem("Tools/Dungeon Knight 3D/Wall Corner Handles", true)]
        public static bool ToggleValidate()
        {
            Menu.SetChecked("Tools/Dungeon Knight 3D/Wall Corner Handles", enabled);
            return true;
        }

        private static void DrawSceneHandles(SceneView sceneView)
        {
            if (!enabled) return;

            Transform selected = Selection.activeTransform;
            if (!selected || !selected.GetComponent<Renderer>()) return;

            Handles.color = new Color(1f, 0.48f, 0.05f, 0.95f);
            DrawCornerHandle(selected, new Vector3(-1f, -1f, -1f));
            DrawCornerHandle(selected, new Vector3(-1f, -1f, 1f));
            DrawCornerHandle(selected, new Vector3(-1f, 1f, -1f));
            DrawCornerHandle(selected, new Vector3(-1f, 1f, 1f));
            DrawCornerHandle(selected, new Vector3(1f, -1f, -1f));
            DrawCornerHandle(selected, new Vector3(1f, -1f, 1f));
            DrawCornerHandle(selected, new Vector3(1f, 1f, -1f));
            DrawCornerHandle(selected, new Vector3(1f, 1f, 1f));
        }

        private static void DrawCornerHandle(Transform target, Vector3 sign)
        {
            Vector3 halfSize = target.localScale * 0.5f;
            Vector3 corner = target.position + Vector3.Scale(halfSize, sign);
            Vector3 fixedCorner = target.position - Vector3.Scale(halfSize, sign);
            float handleSize = HandleUtility.GetHandleSize(corner) * 0.09f;

            EditorGUI.BeginChangeCheck();
            Vector3 movedCorner = Handles.FreeMoveHandle(corner, handleSize, Vector3.one * 0.1f, Handles.CubeHandleCap);
            if (!EditorGUI.EndChangeCheck()) return;

            Undo.RecordObject(target, "Stretch wall corner");

            Vector3 newScale = new Vector3(
                Mathf.Max(MinSize, Mathf.Abs(movedCorner.x - fixedCorner.x)),
                Mathf.Max(MinSize, Mathf.Abs(movedCorner.y - fixedCorner.y)),
                Mathf.Max(MinSize, Mathf.Abs(movedCorner.z - fixedCorner.z)));

            target.position = (movedCorner + fixedCorner) * 0.5f;
            target.localScale = newScale;

            EditorUtility.SetDirty(target);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }

    public sealed class DungeonKnight3DWallStretchTool : EditorWindow
    {
        private enum StretchAxis
        {
            X,
            Y,
            Z
        }

        private enum StretchAnchor
        {
            Center,
            NegativeSide,
            PositiveSide
        }

        private StretchAxis axis = StretchAxis.X;
        private StretchAnchor anchor = StretchAnchor.Center;
        private float targetSize = 1f;
        private bool initializedFromSelection;

        [MenuItem("Tools/Dungeon Knight 3D/Wall Stretch Tool")]
        public static void Open()
        {
            GetWindow<DungeonKnight3DWallStretchTool>("Wall Stretch");
        }

        private void OnSelectionChange()
        {
            initializedFromSelection = false;
            Repaint();
        }

        private void OnGUI()
        {
            Transform selected = Selection.activeTransform;
            if (!selected)
            {
                EditorGUILayout.HelpBox("Select a wall or cube in the scene.", MessageType.Info);
                return;
            }

            if (!initializedFromSelection)
            {
                targetSize = GetAxisValue(selected.localScale, axis);
                initializedFromSelection = true;
            }

            EditorGUILayout.LabelField("Selected", selected.name, EditorStyles.boldLabel);
            EditorGUILayout.Vector3Field("Position", selected.position);
            EditorGUILayout.Vector3Field("Scale", selected.localScale);

            EditorGUI.BeginChangeCheck();
            axis = (StretchAxis)EditorGUILayout.EnumPopup("Axis", axis);
            if (EditorGUI.EndChangeCheck())
            {
                targetSize = GetAxisValue(selected.localScale, axis);
            }

            anchor = (StretchAnchor)EditorGUILayout.EnumPopup("Keep Fixed", anchor);
            targetSize = Mathf.Max(0.01f, EditorGUILayout.FloatField("New Size", targetSize));

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("- 0.5")) targetSize = Mathf.Max(0.01f, targetSize - 0.5f);
            if (GUILayout.Button("+ 0.5")) targetSize += 0.5f;
            if (GUILayout.Button("+ 1")) targetSize += 1f;
            EditorGUILayout.EndHorizontal();

            using (new EditorGUI.DisabledScope(Mathf.Approximately(GetAxisValue(selected.localScale, axis), targetSize)))
            {
                if (GUILayout.Button("Apply To Selected Wall"))
                {
                    Apply(selected);
                }
            }
        }

        private void Apply(Transform selected)
        {
            Undo.RecordObject(selected, "Stretch wall");

            Vector3 scale = selected.localScale;
            Vector3 position = selected.position;
            float oldSize = GetAxisValue(scale, axis);
            float delta = targetSize - oldSize;

            SetAxisValue(ref scale, axis, targetSize);
            SetAxisValue(ref position, axis, GetAxisValue(position, axis) + GetAnchorOffset(delta, anchor));

            selected.localScale = scale;
            selected.position = position;

            EditorUtility.SetDirty(selected);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log($"[Dungeon Knight 3D] Stretched {selected.name} on {axis} from {oldSize:0.###} to {targetSize:0.###}.");
        }

        private static float GetAnchorOffset(float delta, StretchAnchor stretchAnchor)
        {
            switch (stretchAnchor)
            {
                case StretchAnchor.NegativeSide:
                    return delta * 0.5f;
                case StretchAnchor.PositiveSide:
                    return -delta * 0.5f;
                default:
                    return 0f;
            }
        }

        private static float GetAxisValue(Vector3 value, StretchAxis stretchAxis)
        {
            switch (stretchAxis)
            {
                case StretchAxis.X:
                    return value.x;
                case StretchAxis.Y:
                    return value.y;
                default:
                    return value.z;
            }
        }

        private static void SetAxisValue(ref Vector3 value, StretchAxis stretchAxis, float axisValue)
        {
            switch (stretchAxis)
            {
                case StretchAxis.X:
                    value.x = axisValue;
                    break;
                case StretchAxis.Y:
                    value.y = axisValue;
                    break;
                default:
                    value.z = axisValue;
                    break;
            }
        }
    }
}
#endif
