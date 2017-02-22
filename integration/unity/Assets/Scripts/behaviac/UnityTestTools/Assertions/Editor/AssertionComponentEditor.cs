using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityTest
{
    [CustomEditor(typeof(AssertionComponent))]
    public class AssertionComponentEditor : Editor
    {
        private readonly DropDownControl<Type> comparerDropDown = new DropDownControl<Type>();

        private readonly PropertyPathSelector thisPathSelector = new PropertyPathSelector("Compare");
        private readonly PropertyPathSelector otherPathSelector = new PropertyPathSelector("Compare to");

        //private bool focusBackToEdit;

        #region GUI Contents
        private readonly GUIContent guiCheckAfterTimeGuiContent = new GUIContent("Check after (seconds)", "After how many seconds the assertion should be checked");
        private readonly GUIContent guiRepeatCheckTimeGuiContent = new GUIContent("Repeat check", "Should the check be repeated.");
        private readonly GUIContent guiRepeatEveryTimeGuiContent = new GUIContent("Frequency of repetitions", "How often should the check be done");
        private readonly GUIContent guiCheckAfterFramesGuiContent = new GUIContent("Check after (frames)", "After how many frames the assertion should be checked");
        private readonly GUIContent guiRepeatCheckFrameGuiContent = new GUIContent("Repeat check", "Should the check be repeated.");
        #endregion

        public AssertionComponentEditor() {
            comparerDropDown.convertForButtonLabel = type => type.Name;
            comparerDropDown.convertForGUIContent = type => type.Name;
            comparerDropDown.ignoreConvertForGUIContent = types => false;
            comparerDropDown.tooltip = "Comparer that will be used to compare values and determine the result of assertion.";
        }

        public override void OnInspectorGUI() {
            var script = (AssertionComponent) target;
            EditorGUILayout.BeginHorizontal();
            var obj = DrawComparerSelection(script);
            script.checkMethods = (CheckMethod)EditorGUILayout.EnumMaskField(script.checkMethods,
                                                                             EditorStyles.popup,
                                                                             GUILayout.ExpandWidth(false));
            EditorGUILayout.EndHorizontal();

            if (script.IsCheckMethodSelected(CheckMethod.AfterPeriodOfTime)) {
                DrawOptionsForAfterPeriodOfTime(script);
            }

            if (script.IsCheckMethodSelected(CheckMethod.Update)) {
                DrawOptionsForOnUpdate(script);
            }

            if (obj) {
                EditorGUILayout.Space();

                thisPathSelector.Draw(script.Action.go, script.Action,
                                      script.Action.thisPropertyPath, script.Action.GetAccepatbleTypesForA(),
                go => {
                    script.Action.go = go;
                    AssertionExplorerWindow.Reload();
                },
                s => {
                    script.Action.thisPropertyPath = s;
                    AssertionExplorerWindow.Reload();
                });

                EditorGUILayout.Space();

                DrawCustomFields(script);

                EditorGUILayout.Space();

                if (script.Action is ComparerBase) {
                    DrawCompareToType(script.Action as ComparerBase);
                }
            }
        }

        private void DrawOptionsForAfterPeriodOfTime(AssertionComponent script) {
            EditorGUILayout.Space();
            script.checkAfterTime = EditorGUILayout.FloatField(guiCheckAfterTimeGuiContent,
                                                               script.checkAfterTime);

            if (script.checkAfterTime < 0)
            { script.checkAfterTime = 0; }

            script.repeatCheckTime = EditorGUILayout.Toggle(guiRepeatCheckTimeGuiContent,
                                                            script.repeatCheckTime);

            if (script.repeatCheckTime) {
                script.repeatEveryTime = EditorGUILayout.FloatField(guiRepeatEveryTimeGuiContent,
                                                                    script.repeatEveryTime);

                if (script.repeatEveryTime < 0)
                { script.repeatEveryTime = 0; }
            }
        }

        private void DrawOptionsForOnUpdate(AssertionComponent script) {
            EditorGUILayout.Space();
            script.checkAfterFrames = EditorGUILayout.IntField(guiCheckAfterFramesGuiContent,
                                                               script.checkAfterFrames);

            if (script.checkAfterFrames < 1)
            { script.checkAfterFrames = 1; }

            script.repeatCheckFrame = EditorGUILayout.Toggle(guiRepeatCheckFrameGuiContent,
                                                             script.repeatCheckFrame);

            if (script.repeatCheckFrame) {
                script.repeatEveryFrame = EditorGUILayout.IntField(guiRepeatEveryTimeGuiContent,
                                                                   script.repeatEveryFrame);

                if (script.repeatEveryFrame < 1)
                { script.repeatEveryFrame = 1; }
            }
        }

        private void DrawCompareToType(ComparerBase comparer) {
            comparer.compareToType = (ComparerBase.CompareToType) EditorGUILayout.EnumPopup("Compare to type",
                                                                                            comparer.compareToType,
                                                                                            EditorStyles.popup);

            if (comparer.compareToType == ComparerBase.CompareToType.CompareToConstantValue) {
                try {
                    DrawConstCompareField(comparer);

                } catch (NotImplementedException) {
                    Debug.LogWarning("This comparer can't compare to static value");
                    comparer.compareToType = ComparerBase.CompareToType.CompareToObject;
                }

            } else if (comparer.compareToType == ComparerBase.CompareToType.CompareToObject) {
                DrawObjectCompareField(comparer);
            }
        }

        private void DrawObjectCompareField(ComparerBase comparer) {
            otherPathSelector.Draw(comparer.other, comparer,
                                   comparer.otherPropertyPath, comparer.GetAccepatbleTypesForB(),
            go => {
                comparer.other = go;
                AssertionExplorerWindow.Reload();
            },
            s => {
                comparer.otherPropertyPath = s;
                AssertionExplorerWindow.Reload();
            }
                                  );
        }

        private void DrawConstCompareField(ComparerBase comparer) {
            if (comparer.ConstValue == null) {
                comparer.ConstValue = comparer.GetDefaultConstValue();
            }

            var so = new SerializedObject(comparer);
            var sp = so.FindProperty("constantValueGeneric");

            if (sp != null) {
                EditorGUILayout.PropertyField(sp, new GUIContent("Constant"), true);
                so.ApplyModifiedProperties();
            }
        }

        private bool DrawComparerSelection(AssertionComponent script) {
            var types = typeof(ActionBase).Assembly.GetTypes();
            var allComparers = types.Where(type => type.IsSubclassOf(typeof(ActionBase)) && !type.IsAbstract).ToArray();

            if (script.Action == null)
            { script.Action = (ActionBase)CreateInstance(allComparers.First()); }

            comparerDropDown.Draw(script.Action.GetType(), allComparers,
            type => {
                if (script.Action == null || script.Action.GetType().Name != type.Name) {
                    script.Action = (ActionBase)CreateInstance(type);
                    AssertionExplorerWindow.Reload();
                }
            });

            return script.Action != null;
        }

        private void DrawCustomFields(AssertionComponent script) {
            foreach(var prop in script.Action.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)) {
                var type = prop.FieldType;

                if (!type.IsSerializable)
                { continue; }

                var so = new SerializedObject(script.Action);
                var sp = so.FindProperty(prop.Name);

                if (sp != null) {
                    EditorGUILayout.PropertyField(sp);
                    so.ApplyModifiedProperties();
                }
            }
        }
    }
}
