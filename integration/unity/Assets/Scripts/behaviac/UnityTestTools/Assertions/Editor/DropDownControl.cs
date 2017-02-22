using System;
using UnityEditor;
using UnityEngine;

namespace UnityTest
{
    [Serializable]
    internal class DropDownControl<T>
    {
        private GUILayoutOption[] buttonLayoutOptions = new [] { GUILayout.ExpandWidth(true) };
        public Func<T, string> convertForButtonLabel = s => s.ToString();
        public Func<T, string> convertForGUIContent = s => s.ToString();
        public Func<T[], bool> ignoreConvertForGUIContent = t => t.Length <= 40;
        public Action<T> printContextMenu = null;
        public string tooltip = "";

        private object selectedValue;


        public void Draw(T selected, T[] options, Action<T> onValueSelected) {
            Draw(null,
                 selected,
                 options,
                 onValueSelected);
        }

        public void Draw(string label, T selected, T[] options, Action<T> onValueSelected) {
            Draw(label, selected, () => options, onValueSelected);
        }

        public void Draw(string label, T selected, Func<T[]> loadOptions, Action<T> onValueSelected) {
            if (!string.IsNullOrEmpty(label))
            { EditorGUILayout.BeginHorizontal(); }

            var guiContent = new GUIContent(label);
            var labelSize = EditorStyles.label.CalcSize(guiContent);

            if (!string.IsNullOrEmpty(label))
            { GUILayout.Label(label, EditorStyles.label, GUILayout.Width(labelSize.x)); }

            if (GUILayout.Button(new GUIContent(convertForButtonLabel(selected), tooltip),
                                 EditorStyles.popup, buttonLayoutOptions)) {
                if (Event.current.button == 0) {
                    PrintMenu(loadOptions());

                } else if (printContextMenu != null && Event.current.button == 1)
                { printContextMenu(selected); }
            }

            if (selectedValue != null) {
                onValueSelected((T) selectedValue);
                selectedValue = null;
            }

            if (!string.IsNullOrEmpty(label))
            { EditorGUILayout.EndHorizontal(); }
        }

        public void PrintMenu(T[] options) {
            var menu = new GenericMenu();
            foreach(var s in options) {
                var localS = s;
                menu.AddItem(new GUIContent((ignoreConvertForGUIContent(options) ? localS.ToString() : convertForGUIContent(localS))),
                             false,
                             () => { selectedValue = localS;}
                            );
            }
            menu.ShowAsContext();
        }
    }
}
