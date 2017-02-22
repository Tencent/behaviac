using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityTest
{
    public class PropertyPathSelector
    {
        private readonly DropDownControl<string> thisDropDown = new DropDownControl<string>();
        private readonly Func<string, string> replaceDotWithSlashAndAddGOGroup = s => s.Replace('.', '/');

        private readonly string name;
        private bool focusBackToEdit;
        private SelectedPathError error;

        public PropertyPathSelector(string name) {
            this.name = name;
            thisDropDown.convertForGUIContent = replaceDotWithSlashAndAddGOGroup;
            thisDropDown.tooltip = "Select path the value you want to be used for comparisment.";
        }

        public void Draw(GameObject go, ActionBase comparer, string propertPath, Type[] accepatbleTypes, Action<GameObject> onSelectedGO, Action<string> onSelectedPath) {
            var newGO = (GameObject)EditorGUILayout.ObjectField(name, go, typeof(GameObject), true);

            if (newGO != go)
            { onSelectedGO(newGO); }

            if (go != null) {
                var newPath =  DrawListOfMethods(go, comparer, propertPath, accepatbleTypes, thisDropDown);

                if (newPath != propertPath)
                { onSelectedPath(newPath); }
            }
        }

        private string DrawListOfMethods(GameObject go, ActionBase comparer, string propertPath, Type[] accepatbleTypes, DropDownControl<string> dropDown) {
            string result = propertPath;

            if (accepatbleTypes == null) {
                result = DrawManualPropertyEditField(go, propertPath, accepatbleTypes, dropDown);

            } else {
                bool isPropertyOrFieldFound = true;

                if (string.IsNullOrEmpty(result)) {
                    var options = GetFieldsAndProperties(go, comparer, result, accepatbleTypes);
                    isPropertyOrFieldFound = options.Any();

                    if (isPropertyOrFieldFound) {
                        result = options.First();
                    }
                }

                if (isPropertyOrFieldFound) {
                    dropDown.Draw(go.name + '.', result,
                    () => {
                        try
                        {
                            var options = GetFieldsAndProperties(go, comparer, result, accepatbleTypes);
                            return options.ToArray();
                        } catch (Exception) {
                            Debug.LogWarning("An exception was thrown while resolving property list. Reseting property path.");
                            result = "";
                            return new string[0];
                        }
                    }, s => result = s);

                } else {
                    result = DrawManualPropertyEditField(go, propertPath, accepatbleTypes, dropDown);
                }
            }

            return result;
        }

        private static List<string> GetFieldsAndProperties(GameObject go, ActionBase comparer, string extendPath, Type[] accepatbleTypes) {
            var propertyResolver = new PropertyResolver {AllowedTypes = accepatbleTypes, ExcludedFieldNames = comparer.GetExcludedFieldNames()};
            var options = propertyResolver.GetFieldsAndPropertiesFromGameObject(go, comparer.GetDepthOfSearch(), extendPath).ToList();
            options.Sort((x, y) => {
                if (char.IsLower(x[0]))
                    return -1;
                if (char.IsLower(y[0]))
                    return 1;
                return x.CompareTo(y);
            });
            return options;
        }

        private string DrawManualPropertyEditField(GameObject go, string propertPath, Type[] acceptableTypes, DropDownControl<string> dropDown) {
            var propertyResolver = new PropertyResolver { AllowedTypes = acceptableTypes };
            IList<string> list;

            var loadProps = new Func<string[]> (() => {
                try
                {
                    list = propertyResolver.GetFieldsAndPropertiesUnderPath(go, propertPath);
                } catch (ArgumentException) {
                    list = propertyResolver.GetFieldsAndPropertiesUnderPath(go, "");
                }
                return list.ToArray();
            });

            EditorGUILayout.BeginHorizontal();

            var labelSize = EditorStyles.label.CalcSize(new GUIContent(go.name + '.'));
            GUILayout.Label(go.name + (propertPath.Length > 0 ? "." : ""), EditorStyles.label, GUILayout.Width(labelSize.x));

            string btnName = "hintBtn";

            if (GUI.GetNameOfFocusedControl() == btnName
                && Event.current.type == EventType.KeyDown
                && Event.current.keyCode == KeyCode.DownArrow) {
                Event.current.Use();
                dropDown.PrintMenu(loadProps());
                GUI.FocusControl("");
                focusBackToEdit = true;
            }

            EditorGUI.BeginChangeCheck();
            GUI.SetNextControlName(btnName);
            var result = GUILayout.TextField(propertPath, EditorStyles.textField);

            if (EditorGUI.EndChangeCheck()) {
                error = DoesPropertyExist(go, result);
            }

            if (focusBackToEdit) {
                focusBackToEdit = false;
                GUI.FocusControl(btnName);
            }

            if (GUILayout.Button("clear", EditorStyles.miniButton, GUILayout.Width(38))) {
                result = "";
                GUI.FocusControl(null);
                focusBackToEdit = true;
                error = DoesPropertyExist(go, result);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(labelSize.x));

            dropDown.Draw("", result ?? "", loadProps, s => {
                result = s;
                GUI.FocusControl(null);
                focusBackToEdit = true;
                error = DoesPropertyExist(go, result);
            });
            EditorGUILayout.EndHorizontal();

            switch (error) {
                case SelectedPathError.InvalidPath:
                    EditorGUILayout.HelpBox("This property does not exist", MessageType.Error);
                    break;

                case SelectedPathError.MissingComponent:
                    EditorGUILayout.HelpBox("This property or field is not attached or set. It will fail unless it will be attached before the check is perfomed.", MessageType.Warning);
                    break;
            }

            return result;
        }

        private SelectedPathError DoesPropertyExist(GameObject go, string propertPath) {
            try {
                object obj;

                if (MemberResolver.TryGetValue(go, propertPath, out obj))
                { return SelectedPathError.None; }

                else
                { return SelectedPathError.InvalidPath; }

            } catch (TargetInvocationException  e) {
                if (e.InnerException is MissingComponentException)
                { return SelectedPathError.MissingComponent; }

                else
                { throw; }
            }
        }

        private enum SelectedPathError {
            None,
            MissingComponent,
            InvalidPath
        }
    }

}
