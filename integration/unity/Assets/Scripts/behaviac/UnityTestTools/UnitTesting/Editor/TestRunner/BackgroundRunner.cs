using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UnityTest
{
    [InitializeOnLoad]
    public partial class UnitTestView
    {
        static UnitTestView() {
            if (Instance != null && Instance.runOnRecompilation)
            { EnableBackgroundRunner(true); }
        }

        #region Background runner

        private static float nextCheck;
        private static string uttRecompile = "UTT-recompile";

        public static void EnableBackgroundRunner(bool enable) {
            EditorApplication.update -= BackgroudRunner;

            if (enable) {
                EditorApplication.update += BackgroudRunner;
                nextCheck = 0;
            }
        }

        private static void BackgroudRunner() {
            if (EditorApplication.isPlayingOrWillChangePlaymode) { return; }

            if (!Instance.runOnRecompilation) { EnableBackgroundRunner(false); }

            if (EditorApplication.isCompiling) {
                EditorPrefs.SetString(uttRecompile, Application.dataPath);
                EditorApplication.update -= BackgroudRunner;
                return;
            }

            var t = Time.realtimeSinceStartup;

            if (t < nextCheck) { return; }

            nextCheck = t + 0.5f;

            if (EditorPrefs.HasKey(uttRecompile)) {
                var recompile = EditorPrefs.GetString(uttRecompile);

                if (recompile == Application.dataPath) {
                    Instance.RunTests();
                    Instance.Repaint();
                }

                EditorPrefs.DeleteKey(uttRecompile);
                nextCheck = 0;
            }
        }
        #endregion
    }
}
