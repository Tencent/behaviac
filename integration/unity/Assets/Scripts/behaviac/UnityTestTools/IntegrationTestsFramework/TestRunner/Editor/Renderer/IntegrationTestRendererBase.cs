using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Event = UnityEngine.Event;
using Object = UnityEngine.Object;

namespace UnityTest
{
    public abstract class IntegrationTestRendererBase : IComparable<IntegrationTestRendererBase>
    {
        public static Action<IList<ITestComponent>> RunTest;

        protected static bool refresh;

        private static GUIContent guiRunSelected = new GUIContent("Run Selected");
        private static GUIContent guiRun = new GUIContent("Run");
        private static GUIContent guiDelete = new GUIContent("Delete");
        private static GUIContent guiDeleteSelected = new GUIContent("Delete selected");

        protected static GUIContent guiTimeoutIcon = new GUIContent(Icons.stopwatchImg, "Timeout");

        protected GameObject gameObject;
        public TestComponent test;
        private string name;

        protected IntegrationTestRendererBase(GameObject gameObject) {
            this.test = gameObject.GetComponent(typeof(TestComponent)) as TestComponent;

            if (test == null) { throw new ArgumentException("Provided GameObject is not a test object"); }

            this.gameObject = gameObject;
            this.name = test.Name;
        }

        public int CompareTo(IntegrationTestRendererBase other) {
            return test.CompareTo(other.test);
        }

        public bool Render(RenderingOptions options) {
            refresh = false;
            EditorGUIUtility.SetIconSize(new Vector2(15, 15));
            Render(0, options);
            EditorGUIUtility.SetIconSize(Vector2.zero);
            return refresh;
        }

        protected internal virtual void Render(int indend, RenderingOptions options) {
            if (!IsVisible(options)) { return; }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(indend * 10);

            var tempColor = GUI.color;

            if (IsRunning) {
                var frame = Mathf.Abs(Mathf.Cos(Time.realtimeSinceStartup * 4)) * 0.6f + 0.4f;
                GUI.color = new Color(1, 1, 1, frame);
            }

            var isSelected = Selection.gameObjects.Contains(gameObject);

            var value = GetResult();
            var icon = GuiHelper.GetIconForResult(value);

            var label = new GUIContent(name, icon);
            var labelRect = GUILayoutUtility.GetRect(label, EditorStyles.label, GUILayout.ExpandWidth(true), GUILayout.Height(18));

            OnLeftMouseButtonClick(labelRect);
            OnContextClick(labelRect);
            DrawLine(labelRect, label, isSelected, options);

            if (IsRunning) { GUI.color = tempColor; }

            EditorGUILayout.EndHorizontal();
        }

        protected void OnSelect() {
            if (!Event.current.control) { Selection.objects = new UnityEngine.Object[0]; }

            if (Event.current.control && Selection.gameObjects.Contains(test.gameObject))
            { Selection.objects = Selection.gameObjects.Where(o => o != test.gameObject).ToArray(); }

            else
                Selection.objects = Selection.gameObjects.Concat(new[] { test.gameObject }).ToArray();
        }

        protected void OnLeftMouseButtonClick(Rect rect) {
            if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.mouseDown && Event.current.button == 0) {
                rect.width = 20;

                if (rect.Contains(Event.current.mousePosition)) { return; }

                Event.current.Use();
                OnSelect();
            }
        }

        protected void OnContextClick(Rect rect) {
            if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.ContextClick) {
                DrawContextMenu(test);
            }
        }

        public static void DrawContextMenu(TestComponent testComponent) {
            if (EditorApplication.isPlayingOrWillChangePlaymode) { return; }

            var selectedTests = Selection.gameObjects.Where(go => go.GetComponent(typeof(TestComponent)));
            var manySelected = selectedTests.Count() > 1;

            var m = new GenericMenu();

            if (manySelected) {
                //var testsToRun
                m.AddItem(guiRunSelected, false, data => RunTest(selectedTests.Select(o => o.GetComponent(typeof(TestComponent))).Cast<ITestComponent> ().ToList()), null);
            }

            m.AddItem(guiRun, false, data => RunTest(new[] { testComponent }), null);
            m.AddSeparator("");
            m.AddItem(manySelected ? guiDeleteSelected : guiDelete, false, data => RemoveTests(selectedTests.ToArray()), null);
            m.ShowAsContext();
        }

        private static void RemoveTests(GameObject[] testsToDelete) {
            foreach(var t in testsToDelete) {
#if UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
                Undo.RegisterSceneUndo("Destroy Tests");
                GameObject.DestroyImmediate(t);
#else
                Undo.DestroyObjectImmediate(t);
#endif
            }
        }

        protected internal bool IsRunning;
        protected internal abstract void DrawLine(Rect rect, GUIContent label, bool isSelected, RenderingOptions options);
        protected internal abstract TestResult.ResultType GetResult();
        protected internal abstract bool IsVisible(RenderingOptions options);
        public abstract bool SetCurrentTest(TestComponent tc);
    }
}
