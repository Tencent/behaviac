using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityTest.UnitTestRunner;
using Event = UnityEngine.Event;

namespace UnityTest
{
    public partial class UnitTestView
    {
        private int failedTestCount = 0;
        private void UpdateTestInfo(ITestResult result) {
            FindTestResult(result.Id).Update(result, false);

            if (!result.IsSuccess)
            { ++failedTestCount; }
        }

        private UnitTestResult FindTestResult(string resultId) {
            var idx = resultList.FindIndex(testResult => testResult.Id == resultId);

            if (idx == -1) {
                Debug.LogWarning("Id not found for test: " + resultId);
                return null;
            }

            return resultList.ElementAt(idx);
        }

        private void RunTests() {
            var filter = new TestFilter();
            var categories = GetSelectedCategories();

            if (categories != null && categories.Length > 0)
            { filter.categories = categories; }

            RunTests(filter);
        }

        private void RunTests(TestFilter filter) {
            string currentScene = null;
            int undoGroup = -1;

            for (int i = 0; i < 3; ++i) {
                //if (runTestOnANewScene)
                //{
                //if (autoSaveSceneBeforeRun)
                //	EditorApplication.SaveScene ();

                //if (!EditorApplication.SaveCurrentSceneIfUserWantsTo ())
                //	return;
                //}
                failedTestCount = 0;

                if (runTestOnANewScene)
                { currentScene = OpenNewScene(); }

                else
                { undoGroup = RegisterUndo(); }

                string strInfo = "";

                if (testXmlFormat && i == 0) {
                    strInfo = "Xml";

                } else if (testBsonFormat && i == 1) {
                    strInfo = "Bson";

                } else if (testCSharpFormat && i == 2) {
                    strInfo = "CSharp";

                } else {
                    continue;
                }

                UnitTestView.Format = strInfo;
                StartTestRun(filter, new TestRunnerEventListener(UpdateTestInfo));

                if (failedTestCount > 0) {
                    Debug.LogError(strInfo + " Format test failed!");
                    break;
                }
            }

            if (runTestOnANewScene)
            { LoadPreviousScene(currentScene); }

            else
            { PerformUndo(undoGroup); }
        }

        private string OpenNewScene() {
            var currentScene = EditorApplication.currentScene;

            if (runTestOnANewScene)
            { EditorApplication.NewScene(); }

            return currentScene;
        }

        private void LoadPreviousScene(string currentScene) {
            if (!string.IsNullOrEmpty(currentScene))
            { EditorApplication.OpenScene(currentScene); }

            else
            { EditorApplication.NewScene(); }

            if (Event.current != null)
            { GUIUtility.ExitGUI(); }
        }

        public void StartTestRun(TestFilter filter, ITestRunnerCallback eventListener) {
            var callbackList = new TestRunnerCallbackList();

            if (eventListener != null) { callbackList.Add(eventListener); }

            testEngine.RunTests(filter, callbackList);
        }

        private static int RegisterUndo() {
#if UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
            Undo.RegisterSceneUndo("UnitTestRunSceneSave");
            return -1;
#else
            return Undo.GetCurrentGroup();
#endif
        }

        private static void PerformUndo(int undoGroup) {
            EditorUtility.DisplayProgressBar("Undo", "Reverting changes to the scene", 0);
            var undoStartTime = DateTime.Now;
#if UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
            Undo.PerformUndo();
#else
            Undo.RevertAllDownToGroup(undoGroup);
#endif

            if ((DateTime.Now - undoStartTime).Seconds > 1)
            { Debug.LogWarning("Undo after unit test run took " + (DateTime.Now - undoStartTime).Seconds + " seconds. Consider running unit tests on a new scene for better performance."); }

            EditorUtility.ClearProgressBar();
        }

        public class TestRunnerEventListener : ITestRunnerCallback
        {
            private Action<ITestResult> updateCallback;

            public TestRunnerEventListener(Action<ITestResult> updateCallback) {
                this.updateCallback = updateCallback;
            }

            public void TestStarted(string fullName) {
                EditorUtility.DisplayProgressBar("Unit Tests Runner", fullName, 1);
            }

            public void TestFinished(ITestResult result) {
                updateCallback(result);
            }

            public void RunStarted(string suiteName, int testCount) {
            }

            public void RunFinished() {
                EditorUtility.ClearProgressBar();
            }

            public void RunFinishedException(Exception exception) {
                RunFinished();
            }
        }

        [MenuItem("Unity Test Tools/Unit Test Runner %#&u")]
        public static void ShowWindow() {
            GetWindow(typeof(UnitTestView)).Show();
        }
    }

    public class TestFilter
    {
        public string[] names;
        public string[] categories;
        public object[] objects;
        public static TestFilter Empty = new TestFilter();
    }
}
