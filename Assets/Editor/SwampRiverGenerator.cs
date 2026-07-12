using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class SwampRiverGenerator
{
    private const string ParentPath = "Linear Swamp Approach";
    private const string OldAtmospherePassPath = "Linear Swamp Approach/Swamp Ground Atmosphere Pass";
    private const string RiverObjectPath = "Linear Swamp Approach/Swamp Encircling River";
    private const string MeshFolder = "Assets/Art/Models/SwampApproach/River";
    private const string MeshPath = MeshFolder + "/swamp_encircling_river.asset";
    private const string WaterMaterialPath = "Assets/Art/Materials/SwampAtmosphere/SwampShallowWater.mat";
    private const string MudMaterialPath = "Assets/Art/Materials/SwampAtmosphere/SwampWetMudWash.mat";

    [MenuItem("Dungeon Knight 3D/Swamp/Generate Encircling River")]
    public static void Generate()
    {
        var parent = GameObject.Find(ParentPath);
        if (parent == null)
        {
            Debug.LogError($"Could not find {ParentPath}");
            return;
        }

        DeleteIfFound(OldAtmospherePassPath);
        DeleteIfFound(RiverObjectPath);

        EnsureFolder("Assets/Art/Models/SwampApproach", "River");

        var mesh = BuildRiverMesh();
        AssetDatabase.DeleteAsset(MeshPath);
        AssetDatabase.CreateAsset(mesh, MeshPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        var river = new GameObject("Swamp Encircling River");
        Undo.RegisterCreatedObjectUndo(river, "Create swamp encircling river");
        river.transform.SetParent(parent.transform, false);
        river.transform.localPosition = Vector3.zero;
        river.transform.localRotation = Quaternion.identity;
        river.transform.localScale = Vector3.one;

        var filter = river.AddComponent<MeshFilter>();
        filter.sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(MeshPath);

        var renderer = river.AddComponent<MeshRenderer>();
        renderer.sharedMaterials = new[]
        {
            AssetDatabase.LoadAssetAtPath<Material>(MudMaterialPath),
            AssetDatabase.LoadAssetAtPath<Material>(WaterMaterialPath)
        };

        EditorUtility.SetDirty(river);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(river.scene);
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(river.scene);
        Debug.Log("Generated one continuous swamp river with a full loop around the center tree.");
    }

    private static Mesh BuildRiverMesh()
    {
        var vertices = new List<Vector3>();
        var uvs = new List<Vector2>();
        var mudTriangles = new List<int>();
        var waterTriangles = new List<int>();

        var treeCenter = new Vector2(-13.13f, 219.0f);
        const float waterY = 82.735f;
        const float mudY = 82.695f;
        const float ringCenterRadius = 22.0f;
        const float waterHalfWidth = 3.7f;
        const float mudHalfWidth = 5.6f;

        AddRibbon(
            BuildBezier(new Vector2(5.0f, 108.0f), new Vector2(10.0f, 142.0f), new Vector2(-21.0f, 170.0f), treeCenter + new Vector2(0.0f, -ringCenterRadius), 22),
            mudHalfWidth, mudY, vertices, uvs, mudTriangles);
        AddRibbon(
            BuildBezier(treeCenter + new Vector2(0.0f, ringCenterRadius), new Vector2(-20.0f, 266.0f), new Vector2(8.0f, 344.0f), new Vector2(3.0f, 430.0f), 34),
            mudHalfWidth, mudY, vertices, uvs, mudTriangles);
        AddRing(treeCenter, ringCenterRadius, mudHalfWidth, mudY, 96, vertices, uvs, mudTriangles);

        AddRibbon(
            BuildBezier(new Vector2(5.0f, 108.0f), new Vector2(10.0f, 142.0f), new Vector2(-21.0f, 170.0f), treeCenter + new Vector2(0.0f, -ringCenterRadius), 22),
            waterHalfWidth, waterY, vertices, uvs, waterTriangles);
        AddRibbon(
            BuildBezier(treeCenter + new Vector2(0.0f, ringCenterRadius), new Vector2(-20.0f, 266.0f), new Vector2(8.0f, 344.0f), new Vector2(3.0f, 430.0f), 34),
            waterHalfWidth, waterY, vertices, uvs, waterTriangles);
        AddRing(treeCenter, ringCenterRadius, waterHalfWidth, waterY, 96, vertices, uvs, waterTriangles);

        var mesh = new Mesh
        {
            name = "swamp_encircling_river"
        };
        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uvs);
        mesh.subMeshCount = 2;
        mesh.SetTriangles(mudTriangles, 0);
        mesh.SetTriangles(waterTriangles, 1);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    private static List<Vector2> BuildBezier(Vector2 a, Vector2 b, Vector2 c, Vector2 d, int steps)
    {
        var points = new List<Vector2>(steps + 1);
        for (var i = 0; i <= steps; i++)
        {
            var t = i / (float)steps;
            var omt = 1.0f - t;
            points.Add(
                omt * omt * omt * a +
                3.0f * omt * omt * t * b +
                3.0f * omt * t * t * c +
                t * t * t * d);
        }
        return points;
    }

    private static void AddRibbon(List<Vector2> points, float halfWidth, float y, List<Vector3> vertices, List<Vector2> uvs, List<int> triangles)
    {
        var start = vertices.Count;
        var distance = 0.0f;
        for (var i = 0; i < points.Count; i++)
        {
            if (i > 0)
            {
                distance += Vector2.Distance(points[i - 1], points[i]);
            }

            var tangent = GetTangent(points, i);
            var normal = new Vector2(-tangent.y, tangent.x).normalized;
            var left = points[i] + normal * halfWidth;
            var right = points[i] - normal * halfWidth;
            vertices.Add(new Vector3(left.x, y, left.y));
            vertices.Add(new Vector3(right.x, y, right.y));
            uvs.Add(new Vector2(0.0f, distance * 0.08f));
            uvs.Add(new Vector2(1.0f, distance * 0.08f));
        }

        for (var i = 0; i < points.Count - 1; i++)
        {
            var a = start + i * 2;
            var b = a + 1;
            var c = a + 2;
            var d = a + 3;
            triangles.Add(a);
            triangles.Add(c);
            triangles.Add(b);
            triangles.Add(c);
            triangles.Add(d);
            triangles.Add(b);
        }
    }

    private static void AddRing(Vector2 center, float radius, float halfWidth, float y, int segments, List<Vector3> vertices, List<Vector2> uvs, List<int> triangles)
    {
        var start = vertices.Count;
        var inner = radius - halfWidth;
        var outer = radius + halfWidth;
        for (var i = 0; i <= segments; i++)
        {
            var angle = Mathf.PI * 2.0f * i / segments;
            var dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            var outerPoint = center + dir * outer;
            var innerPoint = center + dir * inner;
            vertices.Add(new Vector3(outerPoint.x, y, outerPoint.y));
            vertices.Add(new Vector3(innerPoint.x, y, innerPoint.y));
            uvs.Add(new Vector2(0.0f, i / 8.0f));
            uvs.Add(new Vector2(1.0f, i / 8.0f));
        }

        for (var i = 0; i < segments; i++)
        {
            var a = start + i * 2;
            var b = a + 1;
            var c = a + 2;
            var d = a + 3;
            triangles.Add(a);
            triangles.Add(c);
            triangles.Add(b);
            triangles.Add(c);
            triangles.Add(d);
            triangles.Add(b);
        }
    }

    private static Vector2 GetTangent(List<Vector2> points, int index)
    {
        if (index == 0)
        {
            return (points[1] - points[0]).normalized;
        }

        if (index == points.Count - 1)
        {
            return (points[index] - points[index - 1]).normalized;
        }

        return (points[index + 1] - points[index - 1]).normalized;
    }

    private static void DeleteIfFound(string path)
    {
        var existing = GameObject.Find(path);
        if (existing == null)
        {
            return;
        }

        Undo.DestroyObjectImmediate(existing);
    }

    private static void EnsureFolder(string parentFolder, string childName)
    {
        var fullPath = parentFolder + "/" + childName;
        if (AssetDatabase.IsValidFolder(fullPath))
        {
            return;
        }

        AssetDatabase.CreateFolder(parentFolder, childName);
    }
}
