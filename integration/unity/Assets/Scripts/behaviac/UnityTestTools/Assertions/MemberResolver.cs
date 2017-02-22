using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace UnityTest
{
    public class MemberResolver
    {
        private object callingObjectRef;
        private MemberInfo[] callstack;
        private GameObject gameObject;
        private string path;

        public MemberResolver(GameObject gameObject, string path) {
            path = path.Trim();
            ValidatePath(path);

            this.gameObject = gameObject;
            this.path = path.Trim();
        }

        public object GetValue(bool useCache) {
            if (useCache && callingObjectRef != null) {
                object val = callingObjectRef;

                for (int i = 0; i < callstack.Length; i++)
                { val = GetValueFromMember(val, callstack[i]); }

                return val;
            }

            object result = GetBaseObject();
            var fullCallStack = GetCallstack();

            callingObjectRef = result;
            var tempCallstack = new List<MemberInfo> ();

            for (int i = 0; i < fullCallStack.Length; i++) {
                var member = fullCallStack[i];
                result = GetValueFromMember(result, member);
                tempCallstack.Add(member);

                if (result == null) { return null; }

                if (!IsValueType(result.GetType())) {
                    tempCallstack.Clear();
                    callingObjectRef = result;
                }
            }

            callstack = tempCallstack.ToArray();
            return result;
        }

        public Type GetMemberType() {
            var callstack = GetCallstack();

            if (callstack.Length == 0) { return GetBaseObject().GetType(); }

            var member = callstack[callstack.Length - 1];

            if (member is FieldInfo)
            { return (member as FieldInfo).FieldType; }

            if (member is MethodInfo)
            { return (member as MethodInfo).ReturnType; }

            return null;
        }

        #region Static wrappers
        public static bool TryGetMemberType(GameObject gameObject, string path, out Type value) {
            try {
                var mr = new MemberResolver(gameObject, path);
                value = mr.GetMemberType();
                return true;

            } catch (InvalidPathException) {
                value = null;
                return false;
            }
        }

        public static bool TryGetValue(GameObject gameObject, string path, out object value) {
            try {
                var mr = new MemberResolver(gameObject, path);
                value = mr.GetValue(false);
                return true;

            } catch (InvalidPathException) {
                value = null;
                return false;
            }
        }
        #endregion

        private object GetValueFromMember(object obj, MemberInfo memberInfo) {
            if (memberInfo is FieldInfo)
            { return (memberInfo as FieldInfo).GetValue(obj); }

            if (memberInfo is MethodInfo)
            { return (memberInfo as MethodInfo).Invoke(obj, null); }

            throw new InvalidPathException(memberInfo.Name);
        }

        private object GetBaseObject() {
            if (string.IsNullOrEmpty(path)) { return gameObject; }

            var firstElement = path.Split('.')[0];
            var comp = gameObject.GetComponent(firstElement);

            if (comp != null)
            { return comp; }

            else
            { return gameObject; }
        }

        private MemberInfo[] GetCallstack() {
            if (path == "") { return new MemberInfo[0]; }

            var propsQueue = new Queue<string> (path.Split('.'));

            Type type = GetBaseObject().GetType();

            if (type != typeof(GameObject))
            { propsQueue.Dequeue(); }

            PropertyInfo propertyTemp = null;
            FieldInfo fieldTemp = null;
            var list = new List<MemberInfo> ();

            while (propsQueue.Count != 0) {
                var nameToFind = propsQueue.Dequeue();
                fieldTemp = GetField(type, nameToFind);

                if (fieldTemp != null) {
                    type = fieldTemp.FieldType;
                    list.Add(fieldTemp);
                    continue;
                }

                propertyTemp = GetProperty(type, nameToFind);

                if (propertyTemp != null) {
                    type = propertyTemp.PropertyType;
                    var getMethod = GetGetMethod(propertyTemp);
                    list.Add(getMethod);
                    continue;
                }

                throw new InvalidPathException(nameToFind);
            }

            return list.ToArray();
        }

        private Type GetTypeFromMember(MemberInfo memberInfo) {
            if (memberInfo is FieldInfo)
            { return (memberInfo as FieldInfo).FieldType; }

            if (memberInfo is MethodInfo)
            { return (memberInfo as MethodInfo).ReturnType; }

            throw new InvalidPathException(memberInfo.Name);
        }

        private void ValidatePath(string path) {
            bool invalid = false;

            if (path.StartsWith(".") || path.EndsWith("."))
            { invalid = true; }

            if (path.IndexOf("..") >= 0)
            { invalid = true; }

            if (Regex.IsMatch(path, @"\s"))
            { invalid = true; }

            if (invalid)
            { throw new InvalidPathException(path); }
        }

        private static bool IsValueType(Type type) {
#if !UNITY_METRO
            return type.IsValueType;
#else
            return false;
#endif
        }

        private static FieldInfo GetField(Type type, string fieldName) {
#if !UNITY_METRO
            return type.GetField(fieldName);
#else
            return null;
#endif
        }

        private static PropertyInfo GetProperty(Type type, string propertyName) {
#if !UNITY_METRO
            return type.GetProperty(propertyName);
#else
            return null;
#endif
        }

        private static MethodInfo GetGetMethod(PropertyInfo propertyInfo) {
#if !UNITY_METRO
            return propertyInfo.GetGetMethod();
#else
            return null;
#endif
        }
    }

}
