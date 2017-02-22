using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Core;
using UnityEditor;
using UnityEngine;
using Event = UnityEngine.Event;

namespace UnityTest
{
    public abstract class UnitTestRendererLine : IComparable<UnitTestRendererLine>
    {
        public static Action<TestFilter> RunTest;
        public static List<UnitTestRendererLine> SelectedLines;

        protected static bool refresh;

        protected static GUIContent guiRunSelected = new GUIContent("Run Selected");
        protected static GUIContent guiRun = new GUIContent("Run");
        protected static GUIContent guiTimeoutIcon = new GUIContent(Icons.stopwatchImg, "Timeout");

        protected string uniqueId;
        protected internal string fullName;
        protected string renderedName;
        protected internal Test test;

        protected UnitTestRendererLine(Test test) {
            this.fullName = test.TestName.FullName;
            this.renderedName = test.TestName.Name;
            this.uniqueId = test.TestName.UniqueName;

            this.test = test;
        }

        public int CompareTo(UnitTestRendererLine other) {
            return uniqueId.CompareTo(other.uniqueId);
        }

        public bool Render(RenderingOptions options) {
            refresh = false;
            EditorGUIUtility.SetIconSize(new Vector2(15, 15));
            Render(0, options);
            EditorGUIUtility.SetIconSize(Vector2.zero);
            return refresh;
        }

        protected internal virtual void Render(int indend, RenderingOptions options) {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(indend * 10);
            DrawLine(SelectedLines.Contains(this), options);
            EditorGUILayout.EndHorizontal();
        }

        protected void OnSelect() {
            if (!Event.current.control) { SelectedLines.Clear(); }

            if (Event.current.control && SelectedLines.Contains(this))
            { SelectedLines.Remove(this); }

            else
            { SelectedLines.Add(this); }

            refresh = true;
        }

        protected abstract void DrawLine(bool isSelected, RenderingOptions options);
        protected internal abstract TestResultState ? GetResult();
        protected internal abstract bool IsVisible(RenderingOptions options);

        public void RunTests(object[] testObjectsList) {
            RunTest(new TestFilter() { objects = testObjectsList });
        }

        public void RunTests(string[] testList) {
            RunTest(new TestFilter() {names = testList});
        }

        public void RunSelectedTests() {
            RunTest(new TestFilter() { objects = SelectedLines.Select(line => line.test.TestName).ToArray() });
        }

        public virtual string GetResultText() {
            return renderedName;
        }
    }
}
