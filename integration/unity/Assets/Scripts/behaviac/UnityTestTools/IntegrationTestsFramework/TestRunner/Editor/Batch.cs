using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityTest.IntegrationTests;

namespace UnityTest
{
    public static partial class Batch
    {
        private const string testScenesParam = "-testscenes=";
        private static string targetPlatformParam = "-targetPlatform=";
        private static string resultFileDirParam = "-resultsFileDirectory=";

        public static void RunIntegrationTests() {
            var targetPlatform = GetTargetPlatform();
            var sceneList = GetTestScenesList();

            if (targetPlatform.HasValue)
            { BuildAndRun(targetPlatform.Value, sceneList); }

            else
            { RunInEditor(sceneList); }
        }

        private static void BuildAndRun(BuildTarget target, List<string> sceneList) {
            var resultFilePath = GetParameterArgument(resultFileDirParam);
            PlatformRunner.BuildAndRunInPlayer(target, sceneList.ToArray(), "IntegrationTests", resultFilePath);
            EditorApplication.Exit(0);
        }

        private static void RunInEditor(List<string> sceneList) {
            if (sceneList == null || sceneList.Count == 0) {
                Debug.Log("No scenes on the list");
                EditorApplication.Exit(0);
                return;
            }

            EditorBuildSettings.scenes = sceneList.Select(s => new EditorBuildSettingsScene(s, true)).ToArray();
            EditorApplication.OpenScene(sceneList.First());
            GuiHelper.SetConsoleErrorPause(false);
            EditorApplication.isPlaying = true;
        }

        private static BuildTarget ? GetTargetPlatform() {
            string platformString = null;
            BuildTarget buildTarget;
            foreach(var arg in Environment.GetCommandLineArgs()) {
                if (arg.ToLower().StartsWith(targetPlatformParam.ToLower())) {
                    platformString = arg.Substring(resultFilePathParam.Length);
                    break;
                }
            }

            try {
                buildTarget = (BuildTarget) Enum.Parse(typeof(BuildTarget), platformString);

            } catch {
                return null;
            }

            return buildTarget;
        }

        private static List<string> GetTestScenesList() {
            var sceneList = new List<string> ();
            foreach(var arg in Environment.GetCommandLineArgs()) {
                if (arg.ToLower().StartsWith(testScenesParam)) {
                    var scenesFromParam = arg.Substring(testScenesParam.Length).Split(',');
                    foreach(var scene in scenesFromParam) {
                        var sceneName = scene;

                        if (!sceneName.EndsWith(".unity"))
                        { sceneName += ".unity"; }

                        var foundScenes = Directory.GetFiles(Directory.GetCurrentDirectory(), sceneName, SearchOption.AllDirectories);

                        if (foundScenes.Length == 1)
                        { sceneList.Add(foundScenes[0].Substring(Directory.GetCurrentDirectory().Length + 1)); }

                        else
                        { Debug.Log(sceneName + " not found or multiple entries found"); }
                    }
                }
            }
            return sceneList.Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();
        }
    }
}
