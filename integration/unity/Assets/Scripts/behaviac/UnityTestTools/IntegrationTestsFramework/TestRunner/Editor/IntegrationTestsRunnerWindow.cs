using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityTest
{
    [Serializable]
    public class IntegrationTestsRunnerWindow : EditorWindow
    {
        #region GUI Contents
        private readonly GUIContent guiOptionsHideLabel = new GUIContent("Hide", Icons.gearImg);
        private readonly GUIContent guiOptionsShowLabel = new GUIContent("Options", Icons.gearImg);
        private readonly GUIContent guiCreateNewTest = new GUIContent(Icons.plusImg, "Create new test");
        private readonly GUIContent guiRunSelectedTests = new GUIContent(Icons.runImg, "Run selected test(s)");
        private readonly GUIContent guiRunAllTests = new GUIContent(Icons.runAllImg, "Run all tests");
        private readonly GUIContent guiAdvancedFilterShow = new GUIContent("Advanced");
        private readonly GUIContent guiAdvancedFilterHide = new GUIContent("Hide");
        private readonly GUIContent guiAddGOUderTest = new GUIContent("Add GOs under test", "Add new GameObject under selected test");
        private readonly GUIContent guiBlockUI = new GUIContent("Block UI when running", "Block UI when running tests");
        private readonly GUIContent guiShowSucceededTests = new GUIContent("Succeeded", Icons.successImg, "Show tests that succeeded");
        private readonly GUIContent guiShowFailedTests = new GUIContent("Failed", Icons.failImg, "Show tests that failed");
        private readonly GUIContent guiShowIgnoredTests = new GUIContent("Ignored", Icons.ignoreImg, "Show tests that are ignored");
        private readonly GUIContent guiShowNotRunTests = new GUIContent("Not Run", Icons.unknownImg, "Show tests that didn't run");
        #endregion

        #region runner steerign vars
        private static IntegrationTestsRunnerWindow Instance = null;
        [SerializeField] private List<TestComponent> testsToRun;
        [SerializeField] private List<string> dynamicTestsToRun;
        [SerializeField] private bool readyToRun;
        //private bool isCompiling;
        private bool isBuilding;
        public static bool selectedInHierarchy;
        private float horizontalSplitBarPosition = 200;
        private Vector2 testInfoScroll, testListScroll;
        private IntegrationTestRendererBase[] testLines;
        private string currectSceneName = null;

        [SerializeField] private GameObject selectedLine;
        [SerializeField] private List<TestResult> resultList = new List<TestResult> ();
        [SerializeField] private List<GameObject> foldMarkers = new List<GameObject> ();

        private bool showOptions;
        private string filterString;
        private bool showAdvancedFilter;

        private bool showSucceededTest = true;
        private bool showFailedTest = true;
        private bool showNotRunnedTest = true;
        private bool showIgnoredTest = true;
        private bool addNewGameObjectUnderSelectedTest;
        private bool blockUIWhenRunning = true;
        #endregion



        static IntegrationTestsRunnerWindow() {
            InitBackgroundRunners();
        }

        private static void InitBackgroundRunners() {
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyWindowItemDraw;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemDraw;
            EditorApplication.hierarchyWindowChanged -= OnHierarchyChangeUpdate;
            EditorApplication.hierarchyWindowChanged += OnHierarchyChangeUpdate;
            EditorApplication.update -= BackgroundSceneChangeWatch;
            EditorApplication.update += BackgroundSceneChangeWatch;
            EditorApplication.playmodeStateChanged -= OnPlaymodeStateChanged;
            EditorApplication.playmodeStateChanged += OnPlaymodeStateChanged;
        }

        private static void OnPlaymodeStateChanged() {
            if (EditorApplication.isPlaying  == EditorApplication.isPlayingOrWillChangePlaymode)
            { Instance.RebuildTestList(); }
        }

        public void OnDestroy() {
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyWindowItemDraw;
            EditorApplication.update -= BackgroundSceneChangeWatch;
            EditorApplication.hierarchyWindowChanged -= OnHierarchyChangeUpdate;
            EditorApplication.playmodeStateChanged -= OnPlaymodeStateChanged;

            TestComponent.DestroyAllDynamicTests();
        }

        private static void BackgroundSceneChangeWatch() {
            if (Instance.currectSceneName != null && Instance.currectSceneName == EditorApplication.currentScene) { return; }

            if (EditorApplication.isPlayingOrWillChangePlaymode) { return; }

            TestComponent.DestroyAllDynamicTests();
            Instance.currectSceneName = EditorApplication.currentScene;
            Instance.RebuildTestList();
        }

        public void OnEnable() {
            title = "Integration Tests Runner";
            Instance = this;

            if (EditorPrefs.HasKey("ITR-addNewGameObjectUnderSelectedTest")) {
                addNewGameObjectUnderSelectedTest = EditorPrefs.GetBool("ITR-addNewGameObjectUnderSelectedTest");
                blockUIWhenRunning = EditorPrefs.GetBool("ITR-blockUIWhenRunning");
                showSucceededTest = EditorPrefs.GetBool("ITR-showSucceededTest");
                showFailedTest = EditorPrefs.GetBool("ITR-showFailedTest");
                showIgnoredTest = EditorPrefs.GetBool("ITR-showIgnoredTest");
                showNotRunnedTest = EditorPrefs.GetBool("ITR-showNotRunnedTest");
            }

            InitBackgroundRunners();

            if (!EditorApplication.isPlayingOrWillChangePlaymode && !readyToRun) { RebuildTestList(); }
        }

        public void OnSelectionChange() {
            if (EditorApplication.isPlayingOrWillChangePlaymode
                || Selection.objects == null
                || Selection.objects.Length == 0) { return; }

            if (Selection.gameObjects.Length == 1) {
                var go = Selection.gameObjects.Single();
                var temp = go.transform;

                while (temp != null) {
                    var tc = temp.GetComponent<TestComponent> ();

                    if (tc != null) { break; }

                    temp = temp.parent;
                }

                if (temp != null) {
                    SelectInHierarchy(temp.gameObject);
                    Selection.activeGameObject = temp.gameObject;
                    selectedLine = temp.gameObject;
                }
            }
        }

        public static void OnHierarchyChangeUpdate() {
            if (Instance.testLines == null || EditorApplication.isPlayingOrWillChangePlaymode) { return; }

            //create a test runner if it doesn't exist
            TestRunner.GetTestRunner();

            if (Instance.addNewGameObjectUnderSelectedTest
                && Instance.selectedLine != null
                && Selection.activeGameObject != null) {
                var go = Selection.activeGameObject;

                if (go.transform.parent == null
                    && go.GetComponent<TestComponent> () == null
                    && go.GetComponent<TestRunner> () == null) {
                    go.transform.parent = Instance.selectedLine.transform;
                }
            }

            //make tests are not places under a go that is not a test itself
            foreach(var test in TestComponent.FindAllTestsOnScene()) {
                if (test.gameObject.transform.parent != null && test.gameObject.transform.parent.gameObject.GetComponent<TestComponent> () == null) {
                    test.gameObject.transform.parent = null;
                    Debug.LogWarning("Tests need to be on top of hierarchy or directly under another test.");
                }
            }

            if (selectedInHierarchy) { selectedInHierarchy = false; }

            else { Instance.RebuildTestList(); }
        }

        public static void OnHierarchyWindowItemDraw(int id, Rect rect) {
            var o = EditorUtility.InstanceIDToObject(id);

            if (o is GameObject) {
                var go = o as GameObject;
                var tc = go.GetComponent<TestComponent> ();

                if (tc != null) {
                    if (!EditorApplication.isPlayingOrWillChangePlaymode
                        && rect.Contains(Event.current.mousePosition)
                        && Event.current.type == EventType.MouseDown
                        && Event.current.button == 1) {
                        IntegrationTestRendererBase.DrawContextMenu(tc);
                    }

                    EditorGUIUtility.SetIconSize(new Vector2(15, 15));
                    var result = Instance.resultList.Find(r => r.GameObject == go);

                    if (result != null) {
                        var icon = result.Executed ? GuiHelper.GetIconForResult(result.resultType) : Icons.unknownImg;
                        EditorGUI.LabelField(new Rect(rect.xMax - 18, rect.yMin - 2, rect.width, rect.height), new GUIContent(icon));
                    }

                    EditorGUIUtility.SetIconSize(Vector2.zero);
                }

                if (Event.current.type == EventType.MouseDown
                    && Event.current.button == 0
                    && rect.Contains(Event.current.mousePosition)) {
                    var temp = go.transform;

                    while (temp != null) {
                        var c = temp.GetComponent<TestComponent> ();

                        if (c != null) { break; }

                        temp = temp.parent;
                    }

                    if (temp != null) { SelectInHierarchy(temp.gameObject); }
                }
            }
        }

        private static void SelectInHierarchy(GameObject gameObject) {
            if (gameObject == Instance.selectedLine) { return; }

            if (!gameObject.activeSelf) {
                selectedInHierarchy = true;
                gameObject.SetActive(true);
            }

            var tests = TestComponent.FindAllTestsOnScene();
            var skipList = gameObject.GetComponentsInChildren(typeof(TestComponent), true);
            tests.RemoveAll(skipList.Contains);
            foreach(var test in tests) {
                var enable = test.GetComponentsInChildren(typeof(TestComponent), true).Any(c => c.gameObject == gameObject);

                if (test.gameObject.activeSelf != enable) { test.gameObject.SetActive(enable); }
            }
        }

        private void RunTests(IList<ITestComponent> tests) {
            if (!tests.Any() || EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
            { return; }

            FocusWindowIfItsOpen(GetType());

            testsToRun = tests.Where(t => t is TestComponent).Cast<TestComponent> ().ToList();
            var temp = testsToRun.Where(t => t.dynamic).ToArray();
            dynamicTestsToRun = temp.Select(c => c.dynamicTypeName).ToList();
            testsToRun.RemoveAll(temp.Contains);

            readyToRun = true;
            TestComponent.DisableAllTests();
            EditorApplication.isPlaying = true;

            if (blockUIWhenRunning)
            { EditorUtility.DisplayProgressBar("Integration Test Runner", "Initializing", 0); }
        }

        public void Update() {
            if (readyToRun && EditorApplication.isPlaying) {
                readyToRun = false;
                var testRunner = TestRunner.GetTestRunner();
                testRunner.TestRunnerCallback.Add(new RunnerCallback(this));
                testRunner.InitRunner(testsToRun.ToList(), dynamicTestsToRun);
            }
        }

        private void RebuildTestList() {
            testLines = null;

            if (!TestComponent.AnyTestsOnScene()) { return; }

            if (!EditorApplication.isPlayingOrWillChangePlaymode) {
                var dynamicTestsOnScene = TestComponent.FindAllDynamicTestsOnScene();
                var dynamicTestTypes = TestComponent.GetTypesWithHelpAttribute(EditorApplication.currentScene);

                foreach(var dynamicTestType in dynamicTestTypes) {
                    var existingTests = dynamicTestsOnScene.Where(component => component.dynamicTypeName == dynamicTestType.AssemblyQualifiedName);

                    if (existingTests.Any()) {
                        dynamicTestsOnScene.Remove(existingTests.Single());
                        continue;
                    }

                    TestComponent.CreateDynamicTest(dynamicTestType);
                }

                foreach(var testComponent in dynamicTestsOnScene)
                DestroyImmediate(testComponent.gameObject);
            }

            var topTestList = TestComponent.FindAllTopTestsOnScene();

            var newResultList = new List<TestResult> ();
            testLines = ParseTestList(topTestList, newResultList);

            var oldDynamicResults = resultList.Where(result => result.dynamicTest);
            foreach(var oldResult in resultList) {
                var result = newResultList.Find(r => r.Id == oldResult.Id);

                if (result == null) { continue; }

                result.Update(oldResult);
            }
            newResultList.AddRange(oldDynamicResults.Where(r => !newResultList.Contains(r)));
            resultList = newResultList;

            IntegrationTestRendererBase.RunTest = RunTests;
            IntegrationTestGroupLine.FoldMarkers = foldMarkers;
            IntegrationTestLine.Results = resultList;

            foldMarkers.RemoveAll(o => o == null);

            selectedInHierarchy = true;
            Repaint();
        }



        private IntegrationTestRendererBase[] ParseTestList(List<TestComponent> testList, List<TestResult> results) {
            var tempList = new List<IntegrationTestRendererBase> ();
            foreach(var testObject in testList) {
                if (!testObject.IsTestGroup()) {
                    var result = new TestResult(testObject);

                    if (results != null)
                    { results.Add(result); }

                    tempList.Add(new IntegrationTestLine(testObject.gameObject, result));
                    continue;
                }

                var group = new IntegrationTestGroupLine(testObject.gameObject);
                var children = testObject.gameObject.GetComponentsInChildren(typeof(TestComponent), true).Cast<TestComponent> ().ToList();
                children = children.Where(c => c.gameObject.transform.parent == testObject.gameObject.transform).ToList();
                group.AddChildren(ParseTestList(children, results));
                tempList.Add(group);
            }
            tempList.Sort();
            return tempList.ToArray();
        }

        public void OnGUI() {

#if !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2

            if (BuildPipeline.isBuildingPlayer) {
                isBuilding = true;

            } else if (isBuilding) {
                isBuilding = false;
                Repaint();
            }

#endif
            PrintHeadPanel();

            EditorGUILayout.BeginVertical(Styles.testList);
            testListScroll = EditorGUILayout.BeginScrollView(testListScroll);
            bool repaint = PrintTestList(testLines);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            RenderDetails();

            if (repaint) { Repaint(); }
        }

        public void PrintHeadPanel() {
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            var layoutOptions = new[] { GUILayout.Height(24), GUILayout.Width(32) };

            if (GUILayout.Button(guiRunAllTests, Styles.buttonLeft, layoutOptions)
                && !EditorApplication.isPlayingOrWillChangePlaymode) {
                RunTests(TestComponent.FindAllTestsOnScene().Cast<ITestComponent> ().ToList());
            }

            if (GUILayout.Button(guiRunSelectedTests, Styles.buttonMid, layoutOptions)
                && !EditorApplication.isPlayingOrWillChangePlaymode) {
                RunTests(Selection.gameObjects.Select(t => t.GetComponent(typeof(TestComponent))).Cast<ITestComponent> ().ToList());
            }

            if (GUILayout.Button(guiCreateNewTest, Styles.buttonRight, layoutOptions)
                && !EditorApplication.isPlayingOrWillChangePlaymode) {
                var test = TestComponent.CreateTest();

                if (Selection.gameObjects.Length == 1
                    && Selection.activeGameObject != null
                    && Selection.activeGameObject.GetComponent<TestComponent> ()) {
                    test.transform.parent = Selection.activeGameObject.transform.parent;
                }

                Selection.activeGameObject = test;
                RebuildTestList();
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(showOptions ? guiOptionsHideLabel : guiOptionsShowLabel, GUILayout.Height(24), GUILayout.Width(80)))
            { showOptions = !showOptions; }

            EditorGUILayout.EndHorizontal();

            if (showOptions)
            { PrintOptions(); }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Filter:", GUILayout.Width(35));
            filterString = EditorGUILayout.TextField(filterString);

            if (GUILayout.Button(showAdvancedFilter ? guiAdvancedFilterHide : guiAdvancedFilterShow, GUILayout.Width(80), GUILayout.Height(16)))
            { showAdvancedFilter = !showAdvancedFilter; }

            EditorGUILayout.EndHorizontal();

            if (showAdvancedFilter)
            { PrintAdvancedFilter(); }
        }

        public void PrintOptions() {
            var style = EditorStyles.toggle;
            EditorGUILayout.BeginVertical();
            EditorGUI.BeginChangeCheck();
            addNewGameObjectUnderSelectedTest = EditorGUILayout.Toggle(guiAddGOUderTest, addNewGameObjectUnderSelectedTest, style);
            blockUIWhenRunning = EditorGUILayout.Toggle(guiBlockUI, blockUIWhenRunning, style);

            if (EditorGUI.EndChangeCheck()) {
                EditorPrefs.SetBool("ITR-addNewGameObjectUnderSelectedTest", addNewGameObjectUnderSelectedTest);
                EditorPrefs.SetBool("ITR-blockUIWhenRunning", blockUIWhenRunning);
            }

            EditorGUILayout.EndVertical();
        }

        private void PrintAdvancedFilter() {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            showSucceededTest = GUILayout.Toggle(showSucceededTest, guiShowSucceededTests, GUI.skin.FindStyle(GUI.skin.button.name + "left"), GUILayout.ExpandWidth(true));
            showFailedTest = GUILayout.Toggle(showFailedTest, guiShowFailedTests, GUI.skin.FindStyle(GUI.skin.button.name + "mid"));
            showIgnoredTest = GUILayout.Toggle(showIgnoredTest, guiShowIgnoredTests, GUI.skin.FindStyle(GUI.skin.button.name + "mid"));
            showNotRunnedTest = GUILayout.Toggle(showNotRunnedTest, guiShowNotRunTests, GUI.skin.FindStyle(GUI.skin.button.name + "right"), GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck()) {
                EditorPrefs.SetBool("ITR-showSucceededTest", showSucceededTest);
                EditorPrefs.SetBool("ITR-showFailedTest", showFailedTest);
                EditorPrefs.SetBool("ITR-showIgnoredTest", showIgnoredTest);
                EditorPrefs.SetBool("ITR-showNotRunnedTest", showNotRunnedTest);
            }
        }

        private bool PrintTestList(IntegrationTestRendererBase[] renderedLines) {
            if (renderedLines == null) { return false; }

            var filter = new RenderingOptions();
            filter.showSucceeded = showSucceededTest;
            filter.showFailed = showFailedTest;
            filter.showNotRunned = showNotRunnedTest;
            filter.showIgnored = showIgnoredTest;
            filter.nameFilter = filterString;

            bool repaint = false;
            foreach(var renderedLine in renderedLines) {
                repaint |= renderedLine.Render(filter);
            }
            return repaint;
        }

        private void RenderDetails() {
            var ctrlId = EditorGUIUtility.GetControlID(FocusType.Passive);

            Rect rect = GUILayoutUtility.GetLastRect();
            rect.y = rect.height + rect.y - 1;
            rect.height = 3;

            EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeVertical);
            var e = Event.current;

            switch (e.type) {
                case EventType.MouseDown:
                    if (EditorGUIUtility.hotControl == 0 && rect.Contains(e.mousePosition))
                    { EditorGUIUtility.hotControl = ctrlId; }

                    break;

                case EventType.MouseDrag:
                    if (EditorGUIUtility.hotControl == ctrlId) {
                        horizontalSplitBarPosition -= e.delta.y;

                        if (horizontalSplitBarPosition < 20) { horizontalSplitBarPosition = 20; }

                        Repaint();
                    }

                    break;

                case EventType.MouseUp:
                    if (EditorGUIUtility.hotControl == ctrlId)
                    { EditorGUIUtility.hotControl = 0; }

                    break;
            }

            testInfoScroll = EditorGUILayout.BeginScrollView(testInfoScroll, GUILayout.MinHeight(horizontalSplitBarPosition));

            var message = "";

            if (selectedLine != null)
            { message = GetResultText(selectedLine); }

            EditorGUILayout.TextArea(message, Styles.info);
            EditorGUILayout.EndScrollView();
        }

        private string GetResultText(GameObject go) {
            var result = resultList.Find(r => r.GameObject == go);

            if (result == null) { return ""; }

            var messages = result.Name;
            messages += "\n\n" + result.messages;

            if (!string.IsNullOrEmpty(result.stacktrace))
            { messages += "\n" + result.stacktrace; }

            return messages.Trim();
        }

        public void OnInspectorUpdate() {
            if (focusedWindow != this) { Repaint(); }
        }

        private void SetCurrentTest(TestComponent tc) {
            foreach(var line in testLines)
            line.SetCurrentTest(tc);
        }

        class RunnerCallback : IntegrationTestRunner.ITestRunnerCallback
        {
            private IntegrationTestsRunnerWindow window;
            private int testNumber = 0;
            private int currentTestNumber = 0;

            private bool consoleErrorOnPauseValue;
            private bool runInBackground;

            public RunnerCallback(IntegrationTestsRunnerWindow window) {
                this.window = window;

                consoleErrorOnPauseValue = GuiHelper.GetConsoleErrorPause();
                GuiHelper.SetConsoleErrorPause(false);
                runInBackground = PlayerSettings.runInBackground;
                PlayerSettings.runInBackground = true;
            }

            public void RunStarted(string platform, List<TestComponent> testsToRun) {
                testNumber = testsToRun.Count;
                foreach(var test in testsToRun) {
                    var result = window.resultList.Find(r => r.TestComponent == test);

                    if (result != null) { result.Reset(); }
                }
            }

            public void RunFinished(List<TestResult> testResults) {
                window.SetCurrentTest(null);
                EditorApplication.isPlaying = false;
                EditorUtility.ClearProgressBar();
                GuiHelper.SetConsoleErrorPause(consoleErrorOnPauseValue);
                PlayerSettings.runInBackground = runInBackground;
            }

            public void TestStarted(TestResult test) {
                window.SetCurrentTest(test.TestComponent);

                if (window.blockUIWhenRunning
                    && EditorUtility.DisplayCancelableProgressBar("Integration Test Runner",
                                                                  "Running " + test.Name,
                                                                  (float) currentTestNumber / testNumber)) {
                    TestRunInterrupted(null);
                }
            }


            public void TestFinished(TestResult test) {
                currentTestNumber++;

                var result = window.resultList.Find(r => r.Id == test.Id);

                if (result != null)
                { result.Update(test); }

                else
                { window.resultList.Add(test); }
            }

            public void TestRunInterrupted(List<ITestComponent> testsNotRun) {
                Debug.Log("Test run interrupted");
                RunFinished(new List<TestResult>());
            }
        }

        [MenuItem("Unity Test Tools/Integration Test Runner %#&t")]
        public static IntegrationTestsRunnerWindow ShowWindow() {
            var w = GetWindow(typeof(IntegrationTestsRunnerWindow));
            w.Show();
            return w as IntegrationTestsRunnerWindow;
        }
    }
}
