using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityTest
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TestComponent))]
    public class TestComponentEditor : Editor
    {
        private SerializedProperty expectException;
        private SerializedProperty expectedExceptionList;
        private SerializedProperty ignored;
        private SerializedProperty succeedAssertions;
        private SerializedProperty succeedWhenExceptionIsThrown;
        private SerializedProperty timeout;

        #region GUI Contens

        private readonly GUIContent guiExpectException = new GUIContent("Expect exception", "Should the test expect an exception");
        private readonly GUIContent guiExpectExceptionList = new GUIContent("Expected exception list", "A comma separated list of exception types which will not fail the test when thrown");
        private readonly GUIContent guiIgnore = new GUIContent("Ignore", "Ignore the tests in runs");
        private readonly GUIContent guiIncludePlatforms = new GUIContent("Included platforms", "Platform on which the test should run");
        private readonly GUIContent guiSuccedOnAssertions = new GUIContent("Succeed on assertions", "Succeed after all assertions are executed");
        private readonly GUIContent guiSucceedWhenExceptionIsThrown = new GUIContent("Succeed when exception is thrown", "Should the test succeed when an expected exception is thrown");
        private readonly GUIContent guiTestName = new GUIContent("Test name", "Name of the test (is equal to the GameObject name)");
        private readonly GUIContent guiTimeout = new GUIContent("Timeout", "Number of seconds after which the test will timeout");

        #endregion

        public void OnEnable() {
            timeout = serializedObject.FindProperty("timeout");
            ignored = serializedObject.FindProperty("ignored");
            succeedAssertions = serializedObject.FindProperty("succeedAfterAllAssertionsAreExecuted");
            expectException = serializedObject.FindProperty("expectException");
            expectedExceptionList = serializedObject.FindProperty("expectedExceptionList");
            succeedWhenExceptionIsThrown = serializedObject.FindProperty("succeedWhenExceptionIsThrown");
        }

        public override void OnInspectorGUI() {
            var component = (TestComponent) target;

            if (component.dynamic && GUILayout.Button("Reload dynamic tests")) {
                TestComponent.DestroyAllDynamicTests();
                Selection.objects = new UnityEngine.Object[0];
                IntegrationTestsRunnerWindow.selectedInHierarchy = false;
                return;
            }

            if (component.IsTestGroup()) {
                EditorGUI.BeginChangeCheck();
                var newGroupName = EditorGUILayout.TextField(guiTestName, component.name);

                if (EditorGUI.EndChangeCheck()) { component.name = newGroupName; }

                serializedObject.ApplyModifiedProperties();
                return;
            }

            serializedObject.Update();

            EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);

            EditorGUI.BeginChangeCheck();
            var newName = EditorGUILayout.TextField(guiTestName, component.name);

            if (EditorGUI.EndChangeCheck()) { component.name = newName; }

            if (component.platformsToIgnore == null) {
                component.platformsToIgnore = GetListOfIgnoredPlatforms(Enum.GetNames(typeof(TestComponent.IncludedPlatforms)), (int)component.includedPlatforms);
            }

            var enumList = Enum.GetNames(typeof(RuntimePlatform));
            var flags = GetFlagList(enumList, component.platformsToIgnore);
            flags = EditorGUILayout.MaskField(guiIncludePlatforms, flags, enumList, EditorStyles.popup);
            var newList = GetListOfIgnoredPlatforms(enumList, flags);

            if (!component.dynamic)
            { component.platformsToIgnore = newList; }

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.PropertyField(timeout, guiTimeout);
            EditorGUILayout.PropertyField(ignored, guiIgnore);
            EditorGUILayout.PropertyField(succeedAssertions, guiSuccedOnAssertions);
            EditorGUILayout.PropertyField(expectException, guiExpectException);

            EditorGUI.BeginDisabledGroup(!expectException.boolValue);
            EditorGUILayout.PropertyField(expectedExceptionList, guiExpectExceptionList);
            EditorGUILayout.PropertyField(succeedWhenExceptionIsThrown, guiSucceedWhenExceptionIsThrown);
            EditorGUI.EndDisabledGroup();

            if (!component.dynamic) { serializedObject.ApplyModifiedProperties(); }
        }

        private string[] GetListOfIgnoredPlatforms(string[] enumList, int flags) {
            var notSelectedPlatforms = new List<string> ();

            for (int i = 0; i < enumList.Length; i++) {
                var sel = (flags & (1 << i)) != 0;

                if (!sel) { notSelectedPlatforms.Add(enumList[i]); }
            }

            return notSelectedPlatforms.ToArray();
        }

        private int GetFlagList(string[] enumList, string[] platformsToIgnore) {
            int flags = ~0;

            for (int i = 0; i < enumList.Length; i++)
                if (platformsToIgnore != null && platformsToIgnore.Any(s => s == enumList[i]))
                { flags &= ~(1 << i); }

            return flags;
        }
    }
}
