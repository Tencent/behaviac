using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityTest.IntegrationTests
{
    public class PlatformRunnerSettingsWindow : EditorWindow
    {
        private BuildTarget buildTarget;
        private List<string> sceneList;
        private Vector2 scrollPosition;
        private string resultsPath;
        private List<string> selectedScenes = new List<string> ();

        GUIContent label = new GUIContent("Results target directory", "Directory where the results will be saved. If no value is specified, the results will be generated in project's data folder.");

        public PlatformRunnerSettingsWindow() {
            title = "Run on platform";
            buildTarget = PlatformRunner.defaultBuildTarget;
            position.Set(position.xMin, position.yMin, 200, position.height);
            sceneList = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.unity", SearchOption.AllDirectories).ToList();
            sceneList.Sort();
            var currentScene = (Directory.GetCurrentDirectory() + EditorApplication.currentScene).Replace("\\", "").Replace("/", "");
            var currentScenePath = sceneList.Where(s => s.Replace("\\", "").Replace("/", "") == currentScene);
            selectedScenes.AddRange(currentScenePath);

            resultsPath = EditorPrefs.GetString("PR-resultsPath");
        }

        private void OnGUI() {
            EditorGUILayout.BeginVertical();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            EditorGUILayout.LabelField("List of scenes to build:", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            foreach(var scenePath in sceneList) {
                var path = Path.GetFileNameWithoutExtension(scenePath);
                var guiContent = new GUIContent(path, scenePath);
                var rect = GUILayoutUtility.GetRect(guiContent, EditorStyles.label);

                if (rect.Contains(Event.current.mousePosition)) {
                    if (Event.current.type == EventType.mouseDown && Event.current.button == 0) {
                        if (!Event.current.control)
                        { selectedScenes.Clear(); }

                        if (!selectedScenes.Contains(scenePath))
                        { selectedScenes.Add(scenePath); }

                        else
                        { selectedScenes.Remove(scenePath); }

                        Event.current.Use();
                    }
                }

                var style = new GUIStyle(EditorStyles.label);

                if (selectedScenes.Contains(scenePath))
                { style.normal.textColor = new Color(0.3f, 0.5f, 0.85f); }

                EditorGUI.LabelField(rect, guiContent, style);
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndScrollView();

            GUILayout.Box("", new[] {GUILayout.ExpandWidth(true), GUILayout.Height(1)});

            buildTarget = (BuildTarget) EditorGUILayout.EnumPopup("Build tests for", buildTarget);

            if (PlatformRunner.defaultBuildTarget != buildTarget) {
                if (GUILayout.Button("Make default target platform")) {
                    PlatformRunner.defaultBuildTarget = buildTarget;
                }
            }

            DrawSetting();
            var build = GUILayout.Button("Build and run tests");
            EditorGUILayout.EndVertical();

            if (!build) { return; }

            PlatformRunner.BuildAndRunInPlayer(buildTarget, selectedScenes.ToArray(), "IntegrationTests", resultsPath);
            Close();
        }

        private void DrawSetting() {
            EditorGUI.BeginChangeCheck();
            resultsPath = EditorGUILayout.TextField(label, resultsPath);

            if (EditorGUI.EndChangeCheck()) {
                EditorPrefs.SetString("PR-resultsPath", resultsPath);
            }

            if (!string.IsNullOrEmpty(resultsPath)) {
                Uri uri;

                if (!Uri.TryCreate(resultsPath, UriKind.Absolute, out uri) || !uri.IsFile || uri.IsWellFormedOriginalString()) {
                    EditorGUILayout.HelpBox("Invalid URI path", MessageType.Warning);
                }
            }
        }
    }
}
