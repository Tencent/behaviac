using System.Collections.Generic;
using System.Linq;
using NUnit.Core;
using UnityEditor;
using UnityEngine;
using Event = UnityEngine.Event;

namespace UnityTest
{
    public class GroupLine : UnitTestRendererLine
    {
        public static List<string> FoldMarkers;

        protected static GUIContent guiExpandAll = new GUIContent("Expand all");
        protected static GUIContent guiCollapseAll = new GUIContent("Collapse all");
        private List<UnitTestRendererLine> children = new List<UnitTestRendererLine> ();

        public GroupLine(TestSuite suite)
            : base(suite) {
            if (suite is NamespaceSuite) { renderedName = fullName; }
        }

        private bool Folded {
            get { return FoldMarkers.Contains(fullName); }

            set
            {
                if (value)
                { FoldMarkers.Add(fullName); }

                else
                { FoldMarkers.RemoveAll(s => s == fullName); }
            }
        }

        public void AddChildren(UnitTestRendererLine[] children) {
            this.children.AddRange(children);
        }

        protected internal override void Render(int indend, RenderingOptions options) {
            if (!AnyVisibleChildren(options)) { return; }

            base.Render(indend, options);

            if (!Folded)
                foreach(var child in children)
                child.Render(indend + 1, options);
        }

        private bool AnyVisibleChildren(RenderingOptions options) {
            return children.Any(l => l.IsVisible(options) == true);
        }

        protected internal override bool IsVisible(RenderingOptions options) {
            return AnyVisibleChildren(options);
        }

        protected override void DrawLine(bool isSelected, RenderingOptions options) {
            var resultIcon = GetResult().HasValue ? GuiHelper.GetIconForResult(GetResult().Value) : Icons.unknownImg;

            var guiContent = new GUIContent(renderedName, resultIcon, fullName);

            var rect = GUILayoutUtility.GetRect(guiContent, Styles.foldout, GUILayout.MaxHeight(16));

            OnLeftMouseButtonClick(rect);
            OnContextClick(rect);

            EditorGUI.BeginChangeCheck();
            var expanded = !EditorGUI.Foldout(rect, !Folded, guiContent, false, isSelected ? Styles.selectedFoldout : Styles.foldout);

            if (EditorGUI.EndChangeCheck()) { Folded = expanded; }
        }

        protected internal override TestResultState ? GetResult() {
            TestResultState ? tempResult = null;

            foreach(var child in children) {
                var childResultState = child.GetResult();

                if (childResultState == TestResultState.Failure || childResultState == TestResultState.Error) {
                    tempResult = TestResultState.Failure;
                    break;
                }

                if (childResultState == TestResultState.Success)
                { tempResult = TestResultState.Success; }

                else if (childResultState == TestResultState.Ignored)
                { tempResult = TestResultState.Ignored; }
            }

            if (tempResult.HasValue) { return tempResult.Value; }

            return null;
        }

        private void OnLeftMouseButtonClick(Rect rect) {
            if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.mouseDown && Event.current.button == 0) {
                OnSelect();
            }
        }

        private void OnContextClick(Rect rect) {
            if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.ContextClick) {
                PrintGroupContextMenu();
            }
        }

        private void PrintGroupContextMenu() {
            var multilineSelection = SelectedLines.Count() > 1;
            var m = new GenericMenu();

            if (multilineSelection) {
                m.AddItem(guiRunSelected,
                          false,
                          data => RunTests(SelectedLines.Select(line => line.test.TestName).ToArray()),
                          "");
            }

            if (!string.IsNullOrEmpty(fullName)) {
                m.AddItem(guiRun,
                          false,
                          data => RunTests(new[] { test.TestName }),
                          "");
            }

            if (!multilineSelection) {
                m.AddSeparator("");

                m.AddItem(Folded ? guiExpandAll : guiCollapseAll,
                          false,
                          data => ExpandOrCollapseAll(Folded),
                          "");
            }

            m.ShowAsContext();
        }

        private void ExpandOrCollapseAll(bool expand) {
            Folded = !expand;
            foreach(var child in children) {
                if (child is GroupLine) { (child as GroupLine).ExpandOrCollapseAll(expand); }
            }
        }
    }
}
