using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityTest
{
    [Serializable]
    public class AssertionExplorerWindow : EditorWindow
    {
        private List<AssertionComponent> allAssertions = new List<AssertionComponent> ();
        [SerializeField]
        private string filterText = "";
        [SerializeField]
        private FilterType filterType;
        [SerializeField]
        private readonly List<string> foldMarkers = new List<string> ();
        [SerializeField]
        private GroupByType groupBy;
        [SerializeField]
        private Vector2 scrollPosition = Vector2.zero;
        private DateTime nextReload = DateTime.Now;
        [SerializeField]
        private static bool shouldReload;
        [SerializeField]
        private ShowType showType;

        public AssertionExplorerWindow() {
            title = "Assertion Explorer";
        }

        public void OnDidOpenScene() {
            ReloadAssertionList();
        }

        public void OnFocus() {
            ReloadAssertionList();
        }

        private void ReloadAssertionList() {
            nextReload = DateTime.Now.AddSeconds(1);
            shouldReload = true;
        }

        public void OnHierarchyChange() {
            ReloadAssertionList();
        }

        public void OnInspectorUpdate() {
            if (shouldReload && nextReload < DateTime.Now) {
                shouldReload = false;
                allAssertions = new List<AssertionComponent> (Resources.FindObjectsOfTypeAll(typeof(AssertionComponent)) as AssertionComponent[]);
                Repaint();
            }
        }

        public void OnGUI() {
            DrawMenuPanel();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            if (allAssertions != null)
            { GetResultRendere().Render(FilterResults(allAssertions, filterText.ToLower()), foldMarkers); }

            EditorGUILayout.EndScrollView();
        }

        private IEnumerable<AssertionComponent> FilterResults(List<AssertionComponent> assertionComponents, string text) {
            if (showType == ShowType.ShowDisabled)
            { assertionComponents = assertionComponents.Where(c => !c.enabled).ToList(); }

            else if (showType == ShowType.ShowEnabled)
            { assertionComponents = assertionComponents.Where(c => c.enabled).ToList(); }

            if (string.IsNullOrEmpty(text))
            { return assertionComponents; }

            switch (filterType) {
                case FilterType.ComparerName:
                    return assertionComponents.Where(c => c.Action.GetType().Name.ToLower().Contains(text));

                case FilterType.AttachedGameObject:
                    return assertionComponents.Where(c => c.gameObject.name.ToLower().Contains(text));

                case FilterType.FirstComparedGameObjectPath:
                    return assertionComponents.Where(c => c.Action.thisPropertyPath.ToLower().Contains(text));

                case FilterType.FirstComparedGameObject:
                    return assertionComponents.Where(c => c.Action.go != null
                                                     && c.Action.go.name.ToLower().Contains(text));

                case FilterType.SecondComparedGameObjectPath:
                    return assertionComponents.Where(c =>
                                                     c.Action is ComparerBase
                                                     && (c.Action as ComparerBase).otherPropertyPath.ToLower().Contains(text));

                case FilterType.SecondComparedGameObject:
                    return assertionComponents.Where(c =>
                                                     c.Action is ComparerBase
                                                     && (c.Action as ComparerBase).other != null
                                                     && (c.Action as ComparerBase).other.name.ToLower().Contains(text));

                default:
                    return assertionComponents;
            }
        }

        private readonly IListRenderer groupByComparerRenderer = new GroupByComparerRenderer();
        private readonly IListRenderer groupByExecutionMethodRenderer = new GroupByExecutionMethodRenderer();
        private readonly IListRenderer groupByGORenderer = new GroupByGORenderer();
        private readonly IListRenderer groupByTestsRenderer = new GroupByTestsRenderer();
        private readonly IListRenderer groupByNothingRenderer = new GroupByNothingRenderer();

        private IListRenderer GetResultRendere() {
            switch (groupBy) {
                case GroupByType.Comparer:
                    return groupByComparerRenderer;

                case GroupByType.ExecutionMethod:
                    return groupByExecutionMethodRenderer;

                case GroupByType.GameObjects:
                    return groupByGORenderer;

                case GroupByType.Tests:
                    return groupByTestsRenderer;

                case GroupByType.Nothing:
                default:
                    return groupByNothingRenderer;
            }
        }

        private void DrawMenuPanel() {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Group by", GUILayout.MaxWidth(60));
            groupBy = (GroupByType) EditorGUILayout.EnumPopup(groupBy, GUILayout.MaxWidth(150));

            GUILayout.FlexibleSpace();

            showType = (ShowType) EditorGUILayout.EnumPopup(showType, GUILayout.MaxWidth(100));

            EditorGUILayout.LabelField("Filter by", GUILayout.MaxWidth(50));
            filterType = (FilterType) EditorGUILayout.EnumPopup(filterType, GUILayout.MaxWidth(100));
            filterText = EditorGUILayout.TextField(filterText, GUILayout.MaxWidth(100));

            if (GUILayout.Button("clear", GUILayout.ExpandWidth(false)))
            { filterText = ""; }

            EditorGUILayout.EndHorizontal();
        }

        [MenuItem("Unity Test Tools/Assertion Explorer")]
        public static AssertionExplorerWindow ShowWindow() {
            var w = GetWindow(typeof(AssertionExplorerWindow));
            w.Show();
            return w as AssertionExplorerWindow;
        }

        private enum FilterType {
            ComparerName,
            FirstComparedGameObject,
            FirstComparedGameObjectPath,
            SecondComparedGameObject,
            SecondComparedGameObjectPath,
            AttachedGameObject
        }

        private enum ShowType {
            ShowAll,
            ShowEnabled,
            ShowDisabled
        }

        private enum GroupByType {
            Nothing,
            Comparer,
            GameObjects,
            ExecutionMethod,
            Tests
        }

        public static void Reload() {
            shouldReload = true;
        }
    }
}
