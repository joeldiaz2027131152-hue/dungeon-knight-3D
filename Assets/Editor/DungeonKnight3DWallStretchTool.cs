#if UNITY_EDITOR
using System.Collections.Generic;
using DungeonKnight.Level;
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

    public sealed class DungeonKnight3DMapAttachTool : EditorWindow
    {
        private enum EditAxis
        {
            X,
            Y,
            Z
        }

        private enum ResizeAnchor
        {
            Center,
            NegativeSide,
            PositiveSide
        }

        private readonly List<GameObject> pickedObjects = new List<GameObject>();

        private bool clickPickEnabled = true;
        private GameObject snapTarget;
        private EditAxis resizeAxis = EditAxis.Y;
        private ResizeAnchor resizeAnchor = ResizeAnchor.NegativeSide;
        private float resizeValue = 4.8f;
        private Vector3 moveOffset = Vector3.zero;
        private Vector2 scroll;
        private bool groupCornerHandles = true;

        [MenuItem("Tools/Dungeon Knight 3D/Map Attach Tool")]
        public static void Open()
        {
            GetWindow<DungeonKnight3DMapAttachTool>("Map Attach");
        }

        private void OnEnable()
        {
            SceneView.duringSceneGui += DrawScenePicker;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= DrawScenePicker;
        }

        private void OnSelectionChange()
        {
            Repaint();
        }

        private void OnGUI()
        {
            CleanupPickedObjects();
            scroll = EditorGUILayout.BeginScrollView(scroll);

            EditorGUILayout.LabelField("Click Pick", EditorStyles.boldLabel);
            clickPickEnabled = EditorGUILayout.Toggle("Enabled", clickPickEnabled);
            EditorGUILayout.HelpBox("When enabled, left-click map pieces in the Scene view to add them. Shift-click removes one.", MessageType.Info);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Use Unity Selection"))
            {
                LoadFromUnitySelection();
            }

            if (GUILayout.Button("Select Picked"))
            {
                Selection.objects = pickedObjects.ToArray();
            }

            if (GUILayout.Button("Clear"))
            {
                pickedObjects.Clear();
                RepaintScene();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField($"Picked Pieces: {pickedObjects.Count}", EditorStyles.boldLabel);
            using (new EditorGUI.DisabledScope(pickedObjects.Count == 0))
            {
                for (int i = 0; i < pickedObjects.Count; i++)
                {
                    EditorGUILayout.ObjectField(pickedObjects[i], typeof(GameObject), true);
                }
            }

            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("Attach", EditorStyles.boldLabel);
            using (new EditorGUI.DisabledScope(pickedObjects.Count == 0))
            {
                if (GUILayout.Button("Make Attached Group"))
                {
                    MakeAttachedGroup();
                }
            }

            snapTarget = (GameObject)EditorGUILayout.ObjectField("Snap Target", snapTarget, typeof(GameObject), true);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Target From Active"))
            {
                snapTarget = Selection.activeGameObject;
            }

            using (new EditorGUI.DisabledScope(pickedObjects.Count == 0 || !snapTarget))
            {
                if (GUILayout.Button("Stick Picked To Target"))
                {
                    StickPickedToTarget();
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("Move Together", EditorStyles.boldLabel);
            groupCornerHandles = EditorGUILayout.Toggle("Corner Handles", groupCornerHandles);
            moveOffset = EditorGUILayout.Vector3Field("Offset", moveOffset);
            EditorGUILayout.BeginHorizontal();
            using (new EditorGUI.DisabledScope(pickedObjects.Count == 0 || moveOffset == Vector3.zero))
            {
                if (GUILayout.Button("Move Picked"))
                {
                    MovePicked(moveOffset);
                }
            }

            if (GUILayout.Button("Reset Offset"))
            {
                moveOffset = Vector3.zero;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField("Set Same Size", EditorStyles.boldLabel);
            resizeAxis = (EditAxis)EditorGUILayout.EnumPopup("Axis", resizeAxis);
            resizeAnchor = (ResizeAnchor)EditorGUILayout.EnumPopup("Keep Fixed", resizeAnchor);
            resizeValue = Mathf.Max(0.01f, EditorGUILayout.FloatField("New Size", resizeValue));

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("- 0.5")) resizeValue = Mathf.Max(0.01f, resizeValue - 0.5f);
            if (GUILayout.Button("+ 0.5")) resizeValue += 0.5f;
            if (GUILayout.Button("+ 1")) resizeValue += 1f;
            EditorGUILayout.EndHorizontal();

            using (new EditorGUI.DisabledScope(pickedObjects.Count == 0))
            {
                if (GUILayout.Button("Apply Size To Picked"))
                {
                    ResizePicked();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawScenePicker(SceneView sceneView)
        {
            if (!clickPickEnabled) return;

            Event current = Event.current;
            int controlId = GUIUtility.GetControlID(FocusType.Passive);
            if (current.type == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(controlId);
            }

            DrawPickedBounds();

            if (current.type != EventType.MouseDown || current.button != 0 || current.alt) return;

            GameObject picked = HandleUtility.PickGameObject(current.mousePosition, false);
            if (!picked) return;

            picked = FindEditableRoot(picked).gameObject;
            if (current.shift)
            {
                pickedObjects.Remove(picked);
            }
            else if (!pickedObjects.Contains(picked))
            {
                pickedObjects.Add(picked);
            }

            Selection.objects = pickedObjects.ToArray();
            current.Use();
            Repaint();
            RepaintScene();
        }

        private void DrawPickedBounds()
        {
            CleanupPickedObjects();
            if (pickedObjects.Count == 0) return;

            Handles.color = new Color(0.1f, 0.75f, 1f, 0.95f);
            foreach (GameObject picked in pickedObjects)
            {
                if (!TryCalculateBounds(new[] { picked.transform }, out Bounds bounds)) continue;
                Handles.DrawWireCube(bounds.center, bounds.size);
            }

            if (TryCalculateBounds(GetPickedTransforms(), out Bounds groupBounds))
            {
                Handles.color = new Color(1f, 0.72f, 0.12f, 0.95f);
                Handles.DrawWireCube(groupBounds.center, groupBounds.size);
                DrawGroupCornerHandles(groupBounds);
            }
        }

        private void DrawGroupCornerHandles(Bounds groupBounds)
        {
            if (!groupCornerHandles) return;

            Handles.color = new Color(1f, 0.48f, 0.05f, 0.95f);
            DrawGroupMoveHandle(new Vector3(groupBounds.min.x, groupBounds.max.y, groupBounds.min.z));
            DrawGroupMoveHandle(new Vector3(groupBounds.min.x, groupBounds.max.y, groupBounds.max.z));
            DrawGroupMoveHandle(new Vector3(groupBounds.max.x, groupBounds.max.y, groupBounds.min.z));
            DrawGroupMoveHandle(new Vector3(groupBounds.max.x, groupBounds.max.y, groupBounds.max.z));
        }

        private void DrawGroupMoveHandle(Vector3 corner)
        {
            float handleSize = HandleUtility.GetHandleSize(corner) * 0.1f;

            EditorGUI.BeginChangeCheck();
            Vector3 movedCorner = Handles.FreeMoveHandle(corner, handleSize, Vector3.one * 0.1f, Handles.SphereHandleCap);
            if (!EditorGUI.EndChangeCheck()) return;

            MovePicked(movedCorner - corner);
        }

        private void LoadFromUnitySelection()
        {
            pickedObjects.Clear();
            foreach (Transform transform in Selection.transforms)
            {
                if (!transform) continue;

                GameObject root = FindEditableRoot(transform.gameObject).gameObject;
                if (!pickedObjects.Contains(root))
                {
                    pickedObjects.Add(root);
                }
            }

            RepaintScene();
        }

        private void MakeAttachedGroup()
        {
            if (!TryCalculateBounds(GetPickedTransforms(), out Bounds bounds)) return;

            GameObject group = new GameObject("Attached Map Pieces");
            Undo.RegisterCreatedObjectUndo(group, "Make attached map group");
            group.transform.position = bounds.center;
            group.AddComponent<DungeonKnight3DSelectionRoot>();

            foreach (GameObject picked in pickedObjects)
            {
                if (!picked || picked.transform == group.transform) continue;
                Undo.SetTransformParent(picked.transform, group.transform, "Make attached map group");
            }

            pickedObjects.Clear();
            pickedObjects.Add(group);
            Selection.activeGameObject = group;
            MarkDirty();
        }

        private void StickPickedToTarget()
        {
            Transform[] picked = GetPickedTransforms();
            if (!snapTarget || picked.Length == 0) return;
            if (!TryCalculateBounds(picked, out Bounds pickedBounds)) return;
            if (!TryCalculateBounds(new[] { snapTarget.transform }, out Bounds targetBounds)) return;

            Vector3 offset = FindClosestFaceOffset(pickedBounds, targetBounds);
            MovePicked(offset);
        }

        private void MovePicked(Vector3 offset)
        {
            foreach (GameObject picked in pickedObjects)
            {
                if (!picked) continue;

                Undo.RecordObject(picked.transform, "Move attached map pieces");
                picked.transform.position += offset;
                EditorUtility.SetDirty(picked.transform);
            }

            MarkDirty();
        }

        private void ResizePicked()
        {
            foreach (GameObject picked in pickedObjects)
            {
                if (!picked) continue;

                Transform target = picked.transform;
                Vector3 scale = target.localScale;
                Vector3 position = target.position;
                float oldSize = GetAxisValue(scale, resizeAxis);
                float delta = resizeValue - oldSize;

                Undo.RecordObject(target, "Resize attached map pieces");
                SetAxisValue(ref scale, resizeAxis, resizeValue);
                SetAxisValue(ref position, resizeAxis, GetAxisValue(position, resizeAxis) + GetAnchorOffset(delta, resizeAnchor));
                target.localScale = scale;
                target.position = position;
                EditorUtility.SetDirty(target);
            }

            MarkDirty();
        }

        private Transform[] GetPickedTransforms()
        {
            CleanupPickedObjects();
            Transform[] transforms = new Transform[pickedObjects.Count];
            for (int i = 0; i < pickedObjects.Count; i++)
            {
                transforms[i] = pickedObjects[i].transform;
            }

            return transforms;
        }

        private void CleanupPickedObjects()
        {
            for (int i = pickedObjects.Count - 1; i >= 0; i--)
            {
                if (!pickedObjects[i])
                {
                    pickedObjects.RemoveAt(i);
                }
            }
        }

        private static Transform FindEditableRoot(GameObject picked)
        {
            Transform current = picked.transform;
            while (current.parent)
            {
                if (current.GetComponent<DungeonKnight3DSelectionRoot>()) return current;
                current = current.parent;
            }

            return picked.transform;
        }

        private static bool TryCalculateBounds(Transform[] transforms, out Bounds bounds)
        {
            bounds = new Bounds(transforms.Length > 0 ? transforms[0].position : Vector3.zero, Vector3.zero);
            bool hasBounds = false;

            foreach (Transform transform in transforms)
            {
                if (!transform) continue;

                Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                {
                    if (!renderer) continue;

                    if (!hasBounds)
                    {
                        bounds = renderer.bounds;
                        hasBounds = true;
                    }
                    else
                    {
                        bounds.Encapsulate(renderer.bounds);
                    }
                }

                if (!hasBounds)
                {
                    bounds.Encapsulate(transform.position);
                    hasBounds = true;
                }
            }

            return hasBounds;
        }

        private static Vector3 FindClosestFaceOffset(Bounds moving, Bounds target)
        {
            Vector3 best = new Vector3(target.max.x - moving.min.x, 0f, 0f);
            float bestDistance = Mathf.Abs(best.x);

            TryFaceOffset(ref best, ref bestDistance, new Vector3(target.min.x - moving.max.x, 0f, 0f));
            TryFaceOffset(ref best, ref bestDistance, new Vector3(0f, target.max.y - moving.min.y, 0f));
            TryFaceOffset(ref best, ref bestDistance, new Vector3(0f, target.min.y - moving.max.y, 0f));
            TryFaceOffset(ref best, ref bestDistance, new Vector3(0f, 0f, target.max.z - moving.min.z));
            TryFaceOffset(ref best, ref bestDistance, new Vector3(0f, 0f, target.min.z - moving.max.z));

            return best;
        }

        private static void TryFaceOffset(ref Vector3 best, ref float bestDistance, Vector3 candidate)
        {
            float distance = candidate.sqrMagnitude;
            if (distance >= bestDistance * bestDistance) return;

            best = candidate;
            bestDistance = Mathf.Sqrt(distance);
        }

        private static float GetAnchorOffset(float delta, ResizeAnchor stretchAnchor)
        {
            switch (stretchAnchor)
            {
                case ResizeAnchor.NegativeSide:
                    return delta * 0.5f;
                case ResizeAnchor.PositiveSide:
                    return -delta * 0.5f;
                default:
                    return 0f;
            }
        }

        private static float GetAxisValue(Vector3 value, EditAxis stretchAxis)
        {
            switch (stretchAxis)
            {
                case EditAxis.X:
                    return value.x;
                case EditAxis.Y:
                    return value.y;
                default:
                    return value.z;
            }
        }

        private static void SetAxisValue(ref Vector3 value, EditAxis stretchAxis, float axisValue)
        {
            switch (stretchAxis)
            {
                case EditAxis.X:
                    value.x = axisValue;
                    break;
                case EditAxis.Y:
                    value.y = axisValue;
                    break;
                default:
                    value.z = axisValue;
                    break;
            }
        }

        private static void MarkDirty()
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            RepaintScene();
        }

        private static void RepaintScene()
        {
            SceneView.RepaintAll();
        }
    }
}
#endif
