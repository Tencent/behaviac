using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityTest
{
    class IntegrationTestGroupLine : IntegrationTestRendererBase
    {
        public static List<GameObject> FoldMarkers;
        private IntegrationTestRendererBase[] children;

        public IntegrationTestGroupLine(GameObject gameObject) : base(gameObject) {
        }

        protected internal override void DrawLine(Rect rect, GUIContent label, bool isSelected, RenderingOptions options) {
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            var isClassFolded = !EditorGUI.Foldout(rect, !Folded, label , isSelected ? Styles.selectedFoldout : Styles.foldout);

            if (EditorGUI.EndChangeCheck()) { Folded = isClassFolded; }

            EditorGUILayout.EndHorizontal();
        }

        private bool Folded {
            get { return FoldMarkers.Contains(gameObject); }

            set
            {
                if (value) { FoldMarkers.Add(gameObject); }

                else { FoldMarkers.RemoveAll(s => s == gameObject); }
            }
        }

        protected internal override void Render(int indend, RenderingOptions options) {
            base.Render(indend, options);

            if (!Folded)
                foreach(var child in children)
                child.Render(indend + 1, options);
        }

        protected internal override TestResult.ResultType GetResult() {
            bool ignored = false;
            bool success = false;
            foreach(var child in children) {
                var result = child.GetResult();

                if (result == TestResult.ResultType.Failed || result == TestResult.ResultType.FailedException || result == TestResult.ResultType.Timeout)
                { return TestResult.ResultType.Failed; }

                if (result == TestResult.ResultType.Success)
                { success = true; }

                else if (result == TestResult.ResultType.Ignored)
                { ignored = true; }

                else
                { ignored = false; }
            }

            if (success) { return TestResult.ResultType.Success; }

            if (ignored) { return TestResult.ResultType.Ignored; }

            return TestResult.ResultType.NotRun;
        }

        protected internal override bool IsVisible(RenderingOptions options) {
            return children.Any(c => c.IsVisible(options));
        }

        public override bool SetCurrentTest(TestComponent tc) {
            IsRunning = false;
            foreach(var child in children)
            IsRunning |= child.SetCurrentTest(tc);
            return IsRunning;
        }

        public void AddChildren(IntegrationTestRendererBase[] parseTestList) {
            children = parseTestList;
        }
    }
}
