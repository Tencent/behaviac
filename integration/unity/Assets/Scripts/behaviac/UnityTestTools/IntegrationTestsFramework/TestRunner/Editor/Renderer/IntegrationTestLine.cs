using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityTest
{
    class IntegrationTestLine : IntegrationTestRendererBase
    {
        public static List<TestResult> Results;
        protected TestResult result;

        public IntegrationTestLine(GameObject gameObject, TestResult testResult) : base(gameObject) {
            this.result = testResult;
        }

        protected internal override void DrawLine(Rect rect, GUIContent label, bool isSelected, RenderingOptions options) {
            EditorGUILayout.BeginHorizontal();
            rect.x += 10;

            EditorGUI.LabelField(rect, label, isSelected ? Styles.selectedLabel : Styles.label);

            if (result.IsTimeout) {
                var timeoutRect = new Rect(rect);
                timeoutRect.x = timeoutRect.x + timeoutRect.width;
                timeoutRect.width = 24;
                EditorGUI.LabelField(timeoutRect, guiTimeoutIcon);
                GUILayout.FlexibleSpace();
            }

            EditorGUILayout.EndHorizontal();
        }

        protected internal override TestResult.ResultType GetResult() {
            if (!result.Executed && test.ignored) { return TestResult.ResultType.Ignored; }

            return result.resultType;
        }

        protected internal override bool IsVisible(RenderingOptions options) {
            if (!string.IsNullOrEmpty(options.nameFilter) && !gameObject.name.ToLower().Contains(options.nameFilter.ToLower())) { return false; }

            if (!options.showSucceeded && result.IsSuccess) { return false; }

            if (!options.showFailed && result.IsFailure) { return false; }

            if (!options.showNotRunned && !result.Executed) { return false; }

            if (!options.showIgnored && test.ignored) { return false; }

            return true;
        }

        public override bool SetCurrentTest(TestComponent tc) {
            IsRunning = test == tc;
            return IsRunning;
        }
    }
}
