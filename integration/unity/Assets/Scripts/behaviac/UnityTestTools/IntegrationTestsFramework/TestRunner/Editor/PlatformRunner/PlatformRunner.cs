using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityTest.IntegrationTests
{
    public class PlatformRunner
    {
        static string resourcesPath = Path.Combine("Assets", "Resources");

        public static BuildTarget defaultBuildTarget {
            get
            {
                var target = EditorPrefs.GetString("ITR-platformRunnerBuildTarget");
                BuildTarget buildTarget;

                try
                {
                    buildTarget = (BuildTarget) Enum.Parse(typeof(BuildTarget), target);
                } catch
                {
                    return GetDefaultBuildTarget();
                }
                return buildTarget;
            }
            set { EditorPrefs.SetString("ITR-platformRunnerBuildTarget", value.ToString()); }
        }

        [MenuItem("Unity Test Tools/Platform Runner/Run current scene %#&r")]
        public static void BuildAndRunCurrentScene() {
            Debug.Log("Building and running current test for " + defaultBuildTarget);
            BuildAndRunInPlayer(defaultBuildTarget, new string[0], null, null);
        }

        [MenuItem("Unity Test Tools/Platform Runner/Run on platform %#r")]
        public static void RunInPlayer() {
            var w = EditorWindow.GetWindow(typeof(PlatformRunnerSettingsWindow));
            w.Show();
        }

        public static void BuildAndRunInPlayer(BuildTarget buildTarget, string[] scenes, string name, string resultFilePath) {
            var folderExisted = AddConfigurationFile(resultFilePath);

            var tempDisplayResolutionDialog = PlayerSettings.displayResolutionDialog;
            PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;
            var tempRunInBackground = PlayerSettings.runInBackground;
            PlayerSettings.runInBackground = true;
            var tempFullScreen = PlayerSettings.defaultIsFullScreen;
            PlayerSettings.defaultIsFullScreen = false;
            PlayerSettings.resizableWindow = true;

            BuildPipeline.BuildPlayer(scenes,
                                      GetTempPath(buildTarget, name ?? Application.loadedLevelName),
                                      buildTarget,
                                      BuildOptions.AutoRunPlayer | BuildOptions.Development);


            PlayerSettings.defaultIsFullScreen = tempFullScreen;
            PlayerSettings.runInBackground = tempRunInBackground;
            PlayerSettings.displayResolutionDialog = tempDisplayResolutionDialog;

            RemoveConfigurationFile(folderExisted);
        }

        private static void RemoveConfigurationFile(bool directoryExisted) {
            var batchRunFileMarkerPath = Path.Combine(resourcesPath, TestRunner.batchRunFileMarker);
            AssetDatabase.DeleteAsset(batchRunFileMarkerPath);
            var configFilePath = Path.Combine(resourcesPath, TestRunner.integrationTestsConfigFileName);
            AssetDatabase.DeleteAsset(configFilePath);

            if (!directoryExisted)
            { AssetDatabase.DeleteAsset(resourcesPath); }
        }

        private static bool AddConfigurationFile(string resultFilePath) {
            var resDirExisted = Directory.Exists(resourcesPath);

            if (!resDirExisted)
            { AssetDatabase.CreateFolder("Assets", "Resources"); }

            if (UnityEditorInternal.InternalEditorUtility.inBatchMode) {
                var batchRunFileMarkerPath = Path.Combine(resourcesPath, TestRunner.batchRunFileMarker);
                File.WriteAllText(batchRunFileMarkerPath, "");
            }

            if (!string.IsNullOrEmpty(resultFilePath)) {
                if (!Directory.Exists(resultFilePath))
                { Directory.CreateDirectory(resultFilePath); }

                var configFilePath = Path.Combine(resourcesPath, TestRunner.integrationTestsConfigFileName);
                File.WriteAllText(configFilePath, resultFilePath);
            }

            AssetDatabase.Refresh();
            return resDirExisted;
        }

        private static string GetTempPath(BuildTarget buildTarget, string name) {
            if (string.IsNullOrEmpty(name))
            { name = Path.GetTempFileName(); }

            var path = Path.Combine("Temp", name);

            switch (buildTarget) {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return path + ".exe";

                case BuildTarget.StandaloneOSXIntel:
                    return path + ".app";

                case BuildTarget.Android:
                    return path + ".apk";

                default:
                    return path;
            }
        }

        private static BuildTarget GetDefaultBuildTarget() {
            switch (EditorUserBuildSettings.selectedBuildTargetGroup) {
                case BuildTargetGroup.Android:
                    return BuildTarget.Android;

                case BuildTargetGroup.WebPlayer:
                    return BuildTarget.WebPlayer;

                case BuildTargetGroup.Standalone:
                default: {
                    switch (Application.platform) {
                        case RuntimePlatform.WindowsPlayer:
                            return BuildTarget.StandaloneWindows;

                        case RuntimePlatform.OSXPlayer:
                            return BuildTarget.StandaloneOSXIntel;

                        case RuntimePlatform.LinuxPlayer:
                            return BuildTarget.StandaloneLinux;
                    }

                    return BuildTarget.WebPlayer;
                }
            }
        }
    }
}
