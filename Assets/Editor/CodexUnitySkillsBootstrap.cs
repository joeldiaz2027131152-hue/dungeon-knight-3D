#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DungeonKnight.Editor
{
    public static class CodexUnitySkillsBootstrap
    {
        [MenuItem("Tools/Codex/Start UnitySkills Server")]
        public static void StartUnitySkillsServer()
        {
            Type serverType = Type.GetType("UnitySkills.SkillsHttpServer, UnitySkills.Editor");
            if (serverType == null)
            {
                Debug.LogWarning("[Codex] UnitySkills package is not loaded.");
                return;
            }

            PropertyInfo isRunningProperty = serverType.GetProperty("IsRunning", BindingFlags.Public | BindingFlags.Static);
            if (isRunningProperty?.GetValue(null) is true)
            {
                return;
            }

            MethodInfo startMethod = serverType.GetMethod(
                "Start",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(int), typeof(bool) },
                null);

            if (startMethod == null)
            {
                Debug.LogWarning("[Codex] UnitySkills Start method was not found.");
                return;
            }

            startMethod.Invoke(null, new object[] { 0, true });
            Debug.Log("[Codex] UnitySkills server start requested.");
        }
    }
}
#endif
