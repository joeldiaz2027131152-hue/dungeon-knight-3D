#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using DungeonKnight.Level;
using DungeonKnight.Player;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DungeonKnight.Editor
{
    public static class DungeonKnight3DEditorMenu
    {
        [MenuItem("Tools/Dungeon Knight 3D/Build Editable World")]
        public static void BuildEditableWorld()
        {
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            DungeonKnight3DBootstrap.BuildEditableScene();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("[Dungeon Knight 3D] Clean editable generated world built in the Scene view.");
        }

        [MenuItem("Tools/Dungeon Knight 3D/Clear Editable World")]
        public static void ClearEditableWorld()
        {
            DungeonKnight3DBootstrap.ClearEditableScene();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("[Dungeon Knight 3D] Editable generated world cleared.");
        }

        [MenuItem("Tools/Dungeon Knight 3D/Quiet Editor Effects")]
        public static void QuietEditorEffects()
        {
            DungeonKnight3DBootstrap.ApplyQuietEditorPreviewToScene();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("[Dungeon Knight 3D] Editor-only flame and point light clutter hidden until Play Mode.");
        }

        [MenuItem("Tools/Dungeon Knight 3D/Make Groups Easy To Select")]
        public static void MakeGroupsEasyToSelect()
        {
            DungeonKnight3DBootstrap.ApplySelectionRootsToScene();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("[Dungeon Knight 3D] Generated grouped objects now select from their parent roots.");
        }

        [MenuItem("Tools/Dungeon Knight 3D/Unify Floor Materials")]
        public static void UnifyFloorMaterials()
        {
            GameObject root = GameObject.Find(DungeonKnight3DBootstrap.GeneratedRootName);
            if (!root)
            {
                Debug.LogWarning("[Dungeon Knight 3D] No generated world found.");
                return;
            }

            Material traversalMaterial = FindGeneratedMaterial(root, "DK3D Exterior Stone");
            if (!traversalMaterial)
            {
                Debug.LogWarning("[Dungeon Knight 3D] Could not find DK3D Exterior Stone in the generated world.");
                return;
            }

            traversalMaterial.mainTextureScale = Vector2.one;
            Material floorMaterial = FindGeneratedMaterial(root, "DK3D Floor Stone Large");
            if (!floorMaterial)
            {
                floorMaterial = new Material(traversalMaterial)
                {
                    name = "DK3D Floor Stone Large"
                };
            }

            Texture2D floorTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Resources/Art/Textures/Dungeon/floor_moss_stone_tiles.png");
            if (floorTexture)
            {
                floorTexture.wrapMode = TextureWrapMode.Repeat;
                floorTexture.filterMode = FilterMode.Bilinear;
                floorMaterial.mainTexture = floorTexture;
                floorMaterial.color = Color.white;
            }

            floorMaterial.mainTextureScale = Vector2.one;

            int changed = 0;
            foreach (MeshRenderer renderer in root.GetComponentsInChildren<MeshRenderer>(true))
            {
                if (!IsFloorMaterialTarget(renderer.transform)) continue;
                Material targetMaterial = IsPrimaryFloorTarget(renderer.transform) ? floorMaterial : traversalMaterial;
                if (renderer.sharedMaterial == targetMaterial) continue;

                renderer.sharedMaterial = targetMaterial;
                changed++;
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log($"[Dungeon Knight 3D] Unified {changed} floor/traversal renderer(s). Primary floors use DK3D Floor Stone Large; platforms use DK3D Exterior Stone.");
        }

        [MenuItem("Tools/Dungeon Knight 3D/Unify Wall Materials")]
        public static void UnifyWallMaterials()
        {
            GameObject root = GameObject.Find(DungeonKnight3DBootstrap.GeneratedRootName);
            if (!root)
            {
                Debug.LogWarning("[Dungeon Knight 3D] No generated world found.");
                return;
            }

            Material sourceMaterial =
                FindGeneratedMaterial(root, "DK3D Dark Moss Stone") ??
                FindGeneratedMaterial(root, "DK3D Shadow Stone") ??
                FindGeneratedMaterial(root, "DK3D Exterior Stone");

            if (!sourceMaterial)
            {
                Debug.LogWarning("[Dungeon Knight 3D] Could not find a source wall material in the generated world.");
                return;
            }

            Material wallMaterial = FindGeneratedMaterial(root, "DK3D Damp Moss Wall") ?? new Material(sourceMaterial)
            {
                name = "DK3D Damp Moss Wall"
            };

            Material vineWallMaterial = FindGeneratedMaterial(root, "DK3D Damp Vine Wall") ?? new Material(sourceMaterial)
            {
                name = "DK3D Damp Vine Wall"
            };

            ConfigureTexturedMaterial(wallMaterial, "Assets/Resources/Art/Textures/Dungeon/wall_damp_moss_stone.png", Vector2.one);
            ConfigureTexturedMaterial(vineWallMaterial, "Assets/Resources/Art/Textures/Dungeon/wall_damp_vines_stone.png", Vector2.one);

            int changed = 0;
            foreach (MeshRenderer renderer in root.GetComponentsInChildren<MeshRenderer>(true))
            {
                if (!IsWallMaterialTarget(renderer.transform)) continue;

                Material source = IsVineWallTarget(renderer.transform) ? vineWallMaterial : wallMaterial;
                Material targetMaterial = new Material(source)
                {
                    name = source.name
                };
                targetMaterial.mainTextureScale = CalculateWallTextureScale(renderer.bounds);
                renderer.sharedMaterial = targetMaterial;
                changed++;
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log($"[Dungeon Knight 3D] Unified {changed} wall renderer(s). Most walls use DK3D Damp Moss Wall; accent walls use DK3D Damp Vine Wall.");
        }

        [MenuItem("Tools/Dungeon Knight 3D/Apply Tower Chamber Expansion")]
        public static void ApplyTowerChamberExpansion()
        {
            GameObject root = GameObject.Find(DungeonKnight3DBootstrap.GeneratedRootName);
            if (!root)
            {
                Debug.LogWarning("[Dungeon Knight 3D] No generated world found.");
                return;
            }

            int removed = RemoveObjects(root, transform =>
                transform.name.StartsWith("World 1-2 Spike") ||
                transform.name == "Buried Spike Tip" ||
                transform.name == "World 1-2 Spike Tips" ||
                transform.name == "Tower Warning Tablet" ||
                transform.name.StartsWith("Tower Warning Tablet ") ||
                transform.name == "Mist Warning Tablet" ||
                transform.name.StartsWith("Mist Warning Tablet "));

            SetWorldTransform(root, "Tower Key Chamber Left Wall", new Vector3(-8.25f, 2.45f, 62.75f), new Vector3(0.7f, 5.6f, 13.1f));
            SetWorldTransform(root, "Tower Key Chamber Right Wall", new Vector3(8.25f, 2.45f, 62.75f), new Vector3(0.7f, 5.6f, 13.1f));
            SetWorldTransform(root, "Tower Key Chamber Front Left Return", new Vector3(-5.55f, 2.15f, 56.45f), new Vector3(5.9f, 4.8f, 0.85f));
            SetWorldTransform(root, "Tower Key Chamber Front Right Return", new Vector3(5.55f, 2.15f, 56.45f), new Vector3(5.9f, 4.8f, 0.85f));
            SetWorldTransform(root, "Tower Key Chamber Front Left Fill", new Vector3(-5.55f, 2.15f, 56.45f), new Vector3(5.9f, 4.8f, 0.85f));
            SetWorldTransform(root, "Tower Key Chamber Front Right Fill", new Vector3(5.55f, 2.15f, 56.45f), new Vector3(5.9f, 4.8f, 0.85f));
            SetWorldTransform(root, "Tower Key Chamber Front Header", new Vector3(0f, 4.05f, 56.45f), new Vector3(16.8f, 2.0f, 0.85f));
            SetWorldTransform(root, "Tower Key Chamber Rear Left Fill", new Vector3(-5.55f, 2.15f, 68.85f), new Vector3(5.9f, 4.8f, 0.9f));
            SetWorldTransform(root, "Tower Key Chamber Rear Right Fill", new Vector3(5.55f, 2.15f, 68.85f), new Vector3(5.9f, 4.8f, 0.9f));
            SetWorldTransform(root, "Tower Key Chamber Rear Header", new Vector3(0f, 4.05f, 68.85f), new Vector3(16.9f, 2.05f, 0.9f));
            SetWorldTransform(root, "Tower Key Chamber Unified Floor", new Vector3(0f, -0.255f, 62.8f), new Vector3(16.9f, 0.5f, 13.1f));
            SetWorldTransform(root, "Tower Key Chamber Ceiling", new Vector3(0f, 4.95f, 62.8f), new Vector3(17.2f, 0.7f, 13.4f));
            SetWorldTransform(root, "Tower Gothic Door (1)", new Vector3(0f, 1.65f, 56.45f), Vector3.one);
            SetWorldTransform(root, "Tower Chamber Entry Door", new Vector3(0f, 1.65f, 56.45f), Vector3.one);

            Material wallMaterial = FindGeneratedMaterial(root, "DK3D Damp Moss Wall") ??
                FindGeneratedMaterial(root, "DK3D Dark Moss Stone") ??
                FindGeneratedMaterial(root, "DK3D Shadow Stone");
            CreateOrUpdateSeal(root, "Tower Key Chamber Front Left Gap Seal", new Vector3(-5.55f, 2.15f, 56.32f), new Vector3(5.9f, 4.8f, 0.9f), wallMaterial);
            CreateOrUpdateSeal(root, "Tower Key Chamber Front Right Gap Seal", new Vector3(5.55f, 2.15f, 56.32f), new Vector3(5.9f, 4.8f, 0.9f), wallMaterial);
            CreateOrUpdateSeal(root, "Tower Key Chamber Front Top Gap Seal", new Vector3(0f, 4.05f, 56.3f), new Vector3(16.9f, 2.05f, 0.9f), wallMaterial);
            CreateOrUpdateSeal(root, "Tower Key Chamber Front Left Jamb Seal", new Vector3(-2.9f, 2.15f, 56.28f), new Vector3(0.38f, 4.8f, 0.95f), wallMaterial);
            CreateOrUpdateSeal(root, "Tower Key Chamber Front Right Jamb Seal", new Vector3(2.9f, 2.15f, 56.28f), new Vector3(0.38f, 4.8f, 0.95f), wallMaterial);
            CreateOrUpdateSeal(root, "Tower Key Chamber Rear Left Gap Seal", new Vector3(-5.55f, 2.15f, 69.02f), new Vector3(5.9f, 4.8f, 0.95f), wallMaterial);
            CreateOrUpdateSeal(root, "Tower Key Chamber Rear Right Gap Seal", new Vector3(5.55f, 2.15f, 69.02f), new Vector3(5.9f, 4.8f, 0.95f), wallMaterial);
            CreateOrUpdateSeal(root, "Tower Key Chamber Rear Top Gap Seal", new Vector3(0f, 4.05f, 69.04f), new Vector3(16.9f, 2.05f, 0.95f), wallMaterial);
            CreateOrUpdateSeal(root, "Tower Key Chamber Rear Left Jamb Seal", new Vector3(-2.9f, 2.15f, 69.06f), new Vector3(0.38f, 4.8f, 0.95f), wallMaterial);
            CreateOrUpdateSeal(root, "Tower Key Chamber Rear Right Jamb Seal", new Vector3(2.9f, 2.15f, 69.06f), new Vector3(0.38f, 4.8f, 0.95f), wallMaterial);
            CreateOrUpdateSeal(root, "Tower Key Chamber Left Upper Side Seal", new Vector3(-8.25f, 4.75f, 62.75f), new Vector3(1.0f, 1.1f, 13.4f), wallMaterial);
            CreateOrUpdateSeal(root, "Tower Key Chamber Right Upper Side Seal", new Vector3(8.25f, 4.75f, 62.75f), new Vector3(1.0f, 1.1f, 13.4f), wallMaterial);
            CreateOrUpdateSeal(root, "Tower Key Chamber Left Floor Side Seal", new Vector3(-8.25f, 0.05f, 62.75f), new Vector3(1.0f, 0.8f, 13.4f), wallMaterial);
            CreateOrUpdateSeal(root, "Tower Key Chamber Right Floor Side Seal", new Vector3(8.25f, 0.05f, 62.75f), new Vector3(1.0f, 0.8f, 13.4f), wallMaterial);

            SetNamedObjectsZ(root, "Animated Wall Torch", 60.1f, 69.1f, 62.75f);

            UnifyFloorMaterials();
            UnifyWallMaterials();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log($"[Dungeon Knight 3D] Tower key chamber expanded toward the old spike trap. Removed {removed} spike/tablet object(s).");
        }

        [MenuItem("Tools/Dungeon Knight 3D/Repair Door Interactions")]
        public static void RepairDoorInteractions()
        {
            GameObject root = GameObject.Find(DungeonKnight3DBootstrap.GeneratedRootName);
            if (!root)
            {
                Debug.LogWarning("[Dungeon Knight 3D] No generated world found.");
                return;
            }

            int repaired = 0;
            foreach (GothicDoubleDoor3D door in root.GetComponentsInChildren<GothicDoubleDoor3D>(true))
            {
                bool requiresTowerKey = door.name == "Tower Gothic Door";
                EnsureDoorInteraction(door.transform, requiresTowerKey);
                repaired++;
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log($"[Dungeon Knight 3D] Repaired {repaired} gothic door interaction trigger(s). Tower Gothic Door requires the tower key.");
        }

        private static void EnsureDoorInteraction(Transform door, bool requiresTowerKey)
        {
            Undo.RecordObject(door.gameObject, "Repair door interactions");

            BoxCollider trigger = null;
            foreach (BoxCollider collider in door.GetComponents<BoxCollider>())
            {
                if (!collider.isTrigger) continue;
                trigger = collider;
                break;
            }

            if (!trigger) trigger = door.gameObject.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.center = new Vector3(0f, 0f, -0.18f);
            trigger.size = new Vector3(6.4f, 3.6f, 2.4f);

            DungeonInteractable3D interactable = door.GetComponent<DungeonInteractable3D>();
            if (!interactable) interactable = door.gameObject.AddComponent<DungeonInteractable3D>();

            if (requiresTowerKey) interactable.ConfigureTowerGate(door);
            else interactable.ConfigureUnlockedGate(door);
        }

        private static int RemoveObjects(GameObject root, System.Predicate<Transform> predicate)
        {
            List<GameObject> targets = new List<GameObject>();
            foreach (Transform transform in root.GetComponentsInChildren<Transform>(true))
            {
                if (transform == root.transform || !predicate(transform)) continue;
                targets.Add(transform.gameObject);
            }

            foreach (GameObject target in targets)
            {
                Object.DestroyImmediate(target);
            }

            return targets.Count;
        }

        private static void SetWorldTransform(GameObject root, string objectName, Vector3 position, Vector3 scale)
        {
            foreach (Transform transform in root.GetComponentsInChildren<Transform>(true))
            {
                if (transform.name != objectName) continue;

                Undo.RecordObject(transform, "Apply tower chamber expansion");
                transform.position = position;
                transform.localScale = scale;
            }
        }

        private static void CreateOrUpdateSeal(GameObject root, string objectName, Vector3 position, Vector3 scale, Material material)
        {
            Transform existing = null;
            foreach (Transform transform in root.GetComponentsInChildren<Transform>(true))
            {
                if (transform.name != objectName) continue;
                existing = transform;
                break;
            }

            if (!existing)
            {
                GameObject seal = GameObject.CreatePrimitive(PrimitiveType.Cube);
                seal.name = objectName;
                seal.transform.SetParent(root.transform, true);
                existing = seal.transform;
            }

            Undo.RecordObject(existing, "Seal tower chamber door gaps");
            existing.position = position;
            existing.localScale = scale;

            MeshRenderer renderer = existing.GetComponent<MeshRenderer>();
            if (renderer && material)
            {
                renderer.sharedMaterial = new Material(material)
                {
                    name = material.name
                };
                renderer.sharedMaterial.mainTextureScale = CalculateWallTextureScale(renderer.bounds);
            }
        }

        private static void SetNamedObjectsZ(GameObject root, string objectName, float minZ, float maxZ, float z)
        {
            foreach (Transform transform in root.GetComponentsInChildren<Transform>(true))
            {
                if (transform.name != objectName) continue;
                float currentZ = transform.position.z;
                if (currentZ < minZ || currentZ > maxZ) continue;

                Undo.RecordObject(transform, "Apply tower chamber expansion");
                transform.position = new Vector3(transform.position.x, transform.position.y, z);
            }
        }

        private static Material FindGeneratedMaterial(GameObject root, string materialName)
        {
            foreach (MeshRenderer renderer in root.GetComponentsInChildren<MeshRenderer>(true))
            {
                Material material = renderer.sharedMaterial;
                if (material && material.name == materialName) return material;
            }

            return null;
        }

        private static void ConfigureTexturedMaterial(Material material, string assetPath, Vector2 tiling)
        {
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            if (texture)
            {
                texture.wrapMode = TextureWrapMode.Repeat;
                texture.filterMode = FilterMode.Bilinear;
                material.mainTexture = texture;
            }

            material.color = Color.white;
            material.mainTextureScale = tiling;
        }

        private static Vector2 CalculateWallTextureScale(Bounds bounds)
        {
            float width = Mathf.Max(bounds.size.x, bounds.size.z);
            float height = bounds.size.y;
            return new Vector2(
                Mathf.Max(1f, width / 2.6f),
                Mathf.Max(1f, height / 5.2f)
            );
        }

        private static bool IsFloorMaterialTarget(Transform transform)
        {
            string name = transform.name;
            string path = GetPath(transform);

            if (name.Contains("Floor Joint") ||
                name.Contains("Floor Crack") ||
                name.Contains("Rubble") ||
                name.Contains("Rail") ||
                name.Contains("Guard") ||
                path.Contains("Torch") ||
                path.Contains("Sword") ||
                path.Contains("Skeleton") ||
                path.Contains("Chest"))
            {
                return false;
            }

            return name.Contains("Floor") ||
                name.Contains("Walkway") ||
                name.Contains("Platform") ||
                name.Contains("Landing") ||
                name.Contains("Bridge") ||
                name.Contains("Stair") ||
                name.Contains("Threshold") ||
                name.Contains("Stone Link") ||
                name.Contains("Unified Floor") ||
                name.Contains("Lift Stone");
        }

        private static bool IsPrimaryFloorTarget(Transform transform)
        {
            string name = transform.name;
            if (name.Contains("Floor Joint") || name.Contains("Floor Crack")) return false;
            if (name.Contains("Rail")) return false;

            return name.EndsWith("Floor") ||
                name.Contains("Unified Floor");
        }

        private static bool IsWallMaterialTarget(Transform transform)
        {
            string name = transform.name;
            string path = GetPath(transform);

            if (name.Contains("Floor") ||
                name.Contains("Ceiling") ||
                name.Contains("Rail") ||
                name.Contains("Railing") ||
                name.Contains("Platform") ||
                name.Contains("Walkway") ||
                name.Contains("Landing") ||
                name.Contains("Bridge") ||
                name.Contains("Stair") ||
                name.Contains("Door") ||
                name.Contains("Torch") ||
                name.Contains("Rubble") ||
                name.Contains("Statue") ||
                name.Contains("Shield") ||
                name.Contains("Elevator") ||
                path.Contains("Torch") ||
                path.Contains("Sword") ||
                path.Contains("Skeleton") ||
                path.Contains("Chest"))
            {
                return false;
            }

            return name.Contains("Wall") ||
                name.Contains("Parapet") ||
                name.Contains("Seal") ||
                name.Contains("Arch") ||
                name.Contains("Return") ||
                name.Contains("Fill") ||
                name.Contains("Header");
        }

        private static bool IsVineWallTarget(Transform transform)
        {
            string name = transform.name;
            return name == "Left Wall" ||
                name == "Rear Wall Above Door" ||
                name == "World 1-2 Right Parapet" ||
                name == "World 1-2 Far Arch Top" ||
                name == "World 1-3 Rear Wall" ||
                name == "World 1-3 High Right Wall" ||
                name == "Tower Key Chamber Rear Left Fill" ||
                name == "Tower Key Chamber Rear Right Fill";
        }

        private static string GetPath(Transform transform)
        {
            string path = transform.name;
            while (transform.parent)
            {
                transform = transform.parent;
                path = transform.name + "/" + path;
            }

            return path;
        }

        [MenuItem("Tools/Dungeon Knight 3D/Repair Player Runtime Setup")]
        public static void RepairPlayerRuntimeSetup()
        {
            PlayerController3D player = DungeonKnight3DBootstrap.RepairPlayerRuntimeSetup(true);
            if (!player)
            {
                Debug.LogWarning("[Dungeon Knight 3D] No Knight 3D player found in the active scene.");
                return;
            }

            Selection.activeGameObject = player.gameObject;
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("[Dungeon Knight 3D] Player movement, height, camera target, and HUD setup repaired.");
        }

        [MenuItem("Tools/Dungeon Knight 3D/Save Editable Scene")]
        public static void SaveEditableScene()
        {
            const string scenePath = "Assets/Scenes/WorldEditable.unity";
            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            {
                AssetDatabase.CreateFolder("Assets", "Scenes");
            }

            UnityEngine.SceneManagement.Scene scene = EditorSceneManager.GetActiveScene();
            bool saved = string.IsNullOrEmpty(scene.path)
                ? EditorSceneManager.SaveScene(scene, scenePath)
                : EditorSceneManager.SaveScene(scene);

            if (saved)
            {
                AssetDatabase.Refresh();
                Debug.Log($"[Dungeon Knight 3D] Scene saved: {(string.IsNullOrEmpty(scene.path) ? scenePath : scene.path)}");
            }
            else
            {
                Debug.LogWarning("[Dungeon Knight 3D] Scene save was cancelled or failed.");
            }
        }

        [MenuItem("Tools/Dungeon Knight 3D/Extend Selected Stair To Ground")]
        public static void ExtendSelectedStairToGround()
        {
            Transform stairRoot = Selection.activeTransform;
            if (!stairRoot)
            {
                Debug.LogWarning("[Dungeon Knight 3D] Select a stair group first.");
                return;
            }

            int added = ExtendStairToGround(stairRoot, 0f);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log($"[Dungeon Knight 3D] Added {added} lower stair step(s) to {stairRoot.name}.");
        }

        private static int ExtendStairToGround(Transform stairRoot, float groundY)
        {
            Transform[] children = stairRoot.GetComponentsInChildren<Transform>();
            Transform lowest = null;
            Transform secondLowest = null;
            float lowestY = float.PositiveInfinity;
            float secondLowestY = float.PositiveInfinity;

            foreach (Transform child in children)
            {
                if (child == stairRoot || !child.GetComponent<Renderer>()) continue;

                float y = child.position.y;
                if (y < lowestY)
                {
                    secondLowest = lowest;
                    secondLowestY = lowestY;
                    lowest = child;
                    lowestY = y;
                }
                else if (y < secondLowestY)
                {
                    secondLowest = child;
                    secondLowestY = y;
                }
            }

            if (!lowest || !secondLowest) return 0;

            Vector3 stepDelta = lowest.position - secondLowest.position;
            if (Mathf.Abs(stepDelta.y) < 0.01f) stepDelta.y = -0.25f;
            if (stepDelta.y > 0f) stepDelta = -stepDelta;

            Renderer lowestRenderer = lowest.GetComponent<Renderer>();
            float bottomY = lowestRenderer.bounds.min.y;
            int added = 0;

            while (bottomY > groundY + 0.05f && added < 24)
            {
                GameObject clone = Object.Instantiate(lowest.gameObject, stairRoot);
                clone.name = $"{lowest.name} Ground Extension {added + 1}";
                clone.transform.position = lowest.position + stepDelta;
                clone.transform.rotation = lowest.rotation;
                clone.transform.localScale = lowest.localScale;

                lowest = clone.transform;
                lowestRenderer = clone.GetComponent<Renderer>();
                bottomY = lowestRenderer ? lowestRenderer.bounds.min.y : clone.transform.position.y;
                added++;
            }

            return added;
        }

        [MenuItem("Tools/Dungeon Knight 3D/Group Selected Objects")]
        public static void GroupSelectedObjects()
        {
            Transform[] selected = Selection.transforms;
            if (selected.Length == 0)
            {
                Debug.LogWarning("[Dungeon Knight 3D] Select objects to group.");
                return;
            }

            Bounds bounds = CalculateBounds(selected);
            GameObject group = new GameObject("Editable Group");
            Undo.RegisterCreatedObjectUndo(group, "Group selected objects");
            group.transform.position = bounds.center;
            group.AddComponent<DungeonKnight3DSelectionRoot>();

            foreach (Transform item in selected)
            {
                Undo.SetTransformParent(item, group.transform, "Group selected objects");
            }

            Selection.activeGameObject = group;
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        [MenuItem("Tools/Dungeon Knight 3D/Snap Selected To Ground")]
        public static void SnapSelectedToGround()
        {
            Transform[] selected = Selection.transforms;
            if (selected.Length == 0)
            {
                Debug.LogWarning("[Dungeon Knight 3D] Select an object to snap.");
                return;
            }

            MoveSelectionToBoundsOffset(new Vector3(0f, -CalculateBounds(selected).min.y, 0f), "Snap selected to ground");
        }

        [MenuItem("Tools/Dungeon Knight 3D/Snap Selected To Nearest Wall")]
        public static void SnapSelectedToNearestWall()
        {
            Transform[] selected = Selection.transforms;
            if (selected.Length == 0)
            {
                Debug.LogWarning("[Dungeon Knight 3D] Select an object to snap.");
                return;
            }

            Bounds selectedBounds = CalculateBounds(selected);
            Renderer[] renderers = Object.FindObjectsByType<Renderer>(FindObjectsInactive.Exclude);
            Renderer nearestWall = null;
            float nearestDistance = float.PositiveInfinity;

            foreach (Renderer renderer in renderers)
            {
                if (!renderer || !renderer.name.ToLowerInvariant().Contains("wall")) continue;
                if (IsInSelection(renderer.transform, selected)) continue;

                Bounds wallBounds = renderer.bounds;
                float distance = Mathf.Min(
                    Mathf.Abs(selectedBounds.min.x - wallBounds.max.x),
                    Mathf.Abs(selectedBounds.max.x - wallBounds.min.x),
                    Mathf.Abs(selectedBounds.min.z - wallBounds.max.z),
                    Mathf.Abs(selectedBounds.max.z - wallBounds.min.z));

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestWall = renderer;
                }
            }

            if (!nearestWall)
            {
                Debug.LogWarning("[Dungeon Knight 3D] No wall renderer found nearby.");
                return;
            }

            Bounds wall = nearestWall.bounds;
            Vector3 offset = Vector3.zero;
            float best = float.PositiveInfinity;
            TryWallOffset(ref offset, ref best, new Vector3(wall.max.x - selectedBounds.min.x, 0f, 0f));
            TryWallOffset(ref offset, ref best, new Vector3(wall.min.x - selectedBounds.max.x, 0f, 0f));
            TryWallOffset(ref offset, ref best, new Vector3(0f, 0f, wall.max.z - selectedBounds.min.z));
            TryWallOffset(ref offset, ref best, new Vector3(0f, 0f, wall.min.z - selectedBounds.max.z));
            MoveSelectionToBoundsOffset(offset, "Snap selected to nearest wall");
        }

        [MenuItem("Tools/Dungeon Knight 3D/Join Active Wall To Selected Wall")]
        public static void JoinActiveWallToSelectedWall()
        {
            Transform active = Selection.activeTransform;
            Transform target = FindJoinTarget(active);
            if (!active || !target)
            {
                Debug.LogWarning("[Dungeon Knight 3D] Select a wall first. If only one wall is selected, it will join to the nearest wall.");
                return;
            }

            Bounds activeBounds = CalculateBounds(new[] { active });
            Bounds targetBounds = CalculateBounds(new[] { target });
            Vector3 offset = FindClosestFaceOffset(activeBounds, targetBounds);

            Undo.RecordObject(active, "Join active wall to selected wall");
            active.position += offset;
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log($"[Dungeon Knight 3D] Joined {active.name} to {target.name}.");
        }

        [MenuItem("Tools/Dungeon Knight 3D/Stretch Active Wall To Selected Wall")]
        public static void StretchActiveWallToSelectedWall()
        {
            Transform active = Selection.activeTransform;
            Transform target = FindJoinTarget(active);
            if (!active || !target)
            {
                Debug.LogWarning("[Dungeon Knight 3D] Select a wall first. If only one wall is selected, it will stretch to the nearest wall.");
                return;
            }

            Bounds activeBounds = CalculateBounds(new[] { active });
            Bounds targetBounds = CalculateBounds(new[] { target });
            JoinCandidate candidate = FindClosestStretchCandidate(activeBounds, targetBounds);

            Undo.RecordObject(active, "Stretch active wall to selected wall");
            Vector3 scale = active.localScale;
            Vector3 position = active.position;
            float currentSize = GetAxisValue(activeBounds.size, candidate.Axis);
            float scaleMultiplier = Mathf.Max(0.01f, candidate.Size) / Mathf.Max(0.01f, currentSize);

            SetAxisValue(ref scale, candidate.Axis, GetAxisValue(scale, candidate.Axis) * scaleMultiplier);
            SetAxisValue(ref position, candidate.Axis, candidate.Center);

            active.localScale = scale;
            active.position = position;

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log($"[Dungeon Knight 3D] Stretched {active.name} to touch {target.name}.");
        }

        [MenuItem("Tools/Dungeon Knight 3D/Duplicate Selected Along X")]
        public static void DuplicateSelectedAlongX()
        {
            DuplicateSelectedAlong(Vector3.right);
        }

        [MenuItem("Tools/Dungeon Knight 3D/Duplicate Selected Along Z")]
        public static void DuplicateSelectedAlongZ()
        {
            DuplicateSelectedAlong(Vector3.forward);
        }

        [MenuItem("Tools/Dungeon Knight 3D/Make Stair From Selection")]
        public static void MakeStairFromSelection()
        {
            Transform[] selected = Selection.transforms;
            if (selected.Length == 0)
            {
                Debug.LogWarning("[Dungeon Knight 3D] Select blocks to turn into a stair.");
                return;
            }

            System.Array.Sort(selected, (a, b) => string.CompareOrdinal(a.name, b.name));
            Vector3 start = selected[0].position;
            Vector3 step = new Vector3(0f, 0.25f, 0.65f);
            for (int i = 0; i < selected.Length; i++)
            {
                Undo.RecordObject(selected[i], "Make stair from selection");
                selected[i].position = start + step * i;
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        [MenuItem("Tools/Dungeon Knight 3D/Create Prefab From Selected")]
        public static void CreatePrefabFromSelected()
        {
            GameObject selected = Selection.activeGameObject;
            if (!selected)
            {
                Debug.LogWarning("[Dungeon Knight 3D] Select an object to save as a prefab.");
                return;
            }

            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            {
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            }

            string safeName = string.Join("_", selected.name.Split(Path.GetInvalidFileNameChars()));
            string path = AssetDatabase.GenerateUniqueAssetPath($"Assets/Prefabs/{safeName}.prefab");
            PrefabUtility.SaveAsPrefabAssetAndConnect(selected, path, InteractionMode.UserAction);
            AssetDatabase.Refresh();
            Debug.Log($"[Dungeon Knight 3D] Prefab created: {path}");
        }

        [MenuItem("Tools/Dungeon Knight 3D/Validate Editable World")]
        public static void ValidateEditableWorld()
        {
            PlayerController3D[] players = Object.FindObjectsByType<PlayerController3D>(FindObjectsInactive.Include);
            DungeonEnemy3D[] enemies = Object.FindObjectsByType<DungeonEnemy3D>(FindObjectsInactive.Include);
            int orphanSwords = 0;

            foreach (Transform transform in Object.FindObjectsByType<Transform>(FindObjectsInactive.Include))
            {
                if (transform.name != "Rusty Sword") continue;
                if (transform.GetComponentInParent<RiggedSkeletonEnemyVisual3D>(true)) continue;
                orphanSwords++;
            }

            if (players.Length == 0) Debug.LogWarning("[Dungeon Knight 3D] Validation: no Knight 3D player found.");
            if (enemies.Length == 0) Debug.LogWarning("[Dungeon Knight 3D] Validation: no DungeonEnemy3D enemies found.");
            if (orphanSwords > 0) Debug.LogWarning($"[Dungeon Knight 3D] Validation: {orphanSwords} loose enemy sword(s) found.");
            Debug.Log($"[Dungeon Knight 3D] Validation complete. Players: {players.Length}, enemies: {enemies.Length}, loose swords: {orphanSwords}.");
        }

        [MenuItem("Tools/Dungeon Knight 3D/Repair Tower Mini Boss")]
        public static void RepairTowerMiniBoss()
        {
            PlayerController3D player = Object.FindAnyObjectByType<PlayerController3D>(FindObjectsInactive.Include);
            int repaired = 0;

            foreach (DungeonEnemy3D enemy in Object.FindObjectsByType<DungeonEnemy3D>(FindObjectsInactive.Include))
            {
                if (!enemy.name.Contains("Tower Key Mini Boss")) continue;

                Undo.RegisterFullObjectHierarchyUndo(enemy.gameObject, "Repair Tower Mini Boss");
                enemy.ConfigureTowerKeyMiniBoss(player);
                repaired++;
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log($"[Dungeon Knight 3D] Tower mini boss repair complete. Repaired: {repaired}.");
        }

        private static void DuplicateSelectedAlong(Vector3 axis)
        {
            Transform[] selected = Selection.transforms;
            if (selected.Length == 0)
            {
                Debug.LogWarning("[Dungeon Knight 3D] Select an object to duplicate.");
                return;
            }

            Bounds bounds = CalculateBounds(selected);
            float spacing = Mathf.Max(1f, Vector3.Scale(bounds.size, axis).magnitude + 0.15f);
            GameObject[] duplicates = new GameObject[selected.Length];
            for (int i = 0; i < selected.Length; i++)
            {
                GameObject clone = Object.Instantiate(selected[i].gameObject, selected[i].parent);
                Undo.RegisterCreatedObjectUndo(clone, "Duplicate selected along line");
                clone.name = $"{selected[i].name} Copy";
                clone.transform.position = selected[i].position + axis * spacing;
                duplicates[i] = clone;
            }

            Selection.objects = duplicates;
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        private static void MoveSelectionToBoundsOffset(Vector3 offset, string undoName)
        {
            Transform[] selected = Selection.transforms;
            if (selected.Length == 0)
            {
                Debug.LogWarning("[Dungeon Knight 3D] Select an object first.");
                return;
            }

            foreach (Transform item in selected)
            {
                Undo.RecordObject(item, undoName);
                item.position += offset;
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        private static Bounds CalculateBounds(Transform[] transforms)
        {
            Bounds bounds = new Bounds(transforms.Length > 0 ? transforms[0].position : Vector3.zero, Vector3.zero);
            bool hasBounds = false;
            foreach (Transform transform in transforms)
            {
                Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in renderers)
                {
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
                }
            }

            return bounds;
        }

        private static bool IsInSelection(Transform target, Transform[] selected)
        {
            foreach (Transform item in selected)
            {
                if (target == item || target.IsChildOf(item)) return true;
            }

            return false;
        }

        private static Transform FindOtherSelectedWall(Transform active)
        {
            if (!active) return null;

            foreach (Transform transform in Selection.transforms)
            {
                if (transform != active && transform.GetComponentInChildren<Renderer>()) return transform;
            }

            return null;
        }

        private static Transform FindJoinTarget(Transform active)
        {
            Transform selectedTarget = FindOtherSelectedWall(active);
            if (selectedTarget) return selectedTarget;
            if (!active) return null;

            Bounds activeBounds = CalculateBounds(new[] { active });
            Renderer[] renderers = Object.FindObjectsByType<Renderer>(FindObjectsInactive.Exclude);
            Renderer nearest = null;
            float nearestDistance = float.PositiveInfinity;

            foreach (Renderer renderer in renderers)
            {
                if (!renderer || renderer.transform == active || renderer.transform.IsChildOf(active)) continue;

                Bounds bounds = renderer.bounds;
                float distance = Mathf.Min(
                    Mathf.Abs(activeBounds.min.x - bounds.max.x),
                    Mathf.Abs(activeBounds.max.x - bounds.min.x),
                    Mathf.Abs(activeBounds.min.z - bounds.max.z),
                    Mathf.Abs(activeBounds.max.z - bounds.min.z));

                if (distance >= nearestDistance) continue;
                nearestDistance = distance;
                nearest = renderer;
            }

            return nearest ? nearest.transform : null;
        }

        private static Vector3 FindClosestFaceOffset(Bounds active, Bounds target)
        {
            Vector3 bestOffset = Vector3.zero;
            float bestDistance = float.PositiveInfinity;
            TryWallOffset(ref bestOffset, ref bestDistance, new Vector3(target.max.x - active.min.x, 0f, 0f));
            TryWallOffset(ref bestOffset, ref bestDistance, new Vector3(target.min.x - active.max.x, 0f, 0f));
            TryWallOffset(ref bestOffset, ref bestDistance, new Vector3(0f, 0f, target.max.z - active.min.z));
            TryWallOffset(ref bestOffset, ref bestDistance, new Vector3(0f, 0f, target.min.z - active.max.z));
            return bestOffset;
        }

        private static JoinCandidate FindClosestStretchCandidate(Bounds active, Bounds target)
        {
            JoinCandidate best = new JoinCandidate { Distance = float.PositiveInfinity, Axis = 0, Center = active.center.x, Size = active.size.x };
            TryStretchCandidate(ref best, 0, active.min.x, active.max.x, target.max.x, false);
            TryStretchCandidate(ref best, 0, active.min.x, active.max.x, target.min.x, true);
            TryStretchCandidate(ref best, 2, active.min.z, active.max.z, target.max.z, false);
            TryStretchCandidate(ref best, 2, active.min.z, active.max.z, target.min.z, true);
            return best;
        }

        private static void TryStretchCandidate(ref JoinCandidate best, int axis, float activeMin, float activeMax, float targetFace, bool keepMin)
        {
            float fixedFace = keepMin ? activeMin : activeMax;
            float size = Mathf.Abs(targetFace - fixedFace);
            if (size < 0.05f) return;

            float distance = keepMin ? Mathf.Abs(targetFace - activeMax) : Mathf.Abs(targetFace - activeMin);
            if (distance >= best.Distance) return;

            best.Axis = axis;
            best.Center = (targetFace + fixedFace) * 0.5f;
            best.Size = size;
            best.Distance = distance;
        }

        private static float GetAxisValue(Vector3 value, int axis)
        {
            switch (axis)
            {
                case 0:
                    return value.x;
                case 1:
                    return value.y;
                default:
                    return value.z;
            }
        }

        private static void SetAxisValue(ref Vector3 value, int axis, float axisValue)
        {
            switch (axis)
            {
                case 0:
                    value.x = axisValue;
                    break;
                case 1:
                    value.y = axisValue;
                    break;
                default:
                    value.z = axisValue;
                    break;
            }
        }

        private static void TryWallOffset(ref Vector3 bestOffset, ref float bestDistance, Vector3 candidate)
        {
            float distance = candidate.sqrMagnitude;
            if (distance >= bestDistance) return;
            bestOffset = candidate;
            bestDistance = distance;
        }

        private struct JoinCandidate
        {
            public int Axis;
            public float Center;
            public float Size;
            public float Distance;
        }
    }
}
#endif
