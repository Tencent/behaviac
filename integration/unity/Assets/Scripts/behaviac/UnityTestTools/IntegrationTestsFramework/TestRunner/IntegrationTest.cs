using System;
using System.IO;
using System.Linq;
using UnityEngine;

public static class IntegrationTest
{
    public const string passMessage = "IntegrationTest Pass";
    public const string failMessage = "IntegrationTest Fail";

    public static void Pass(GameObject go) {
        go = FindTopGameObject(go);
        LogResult(go, passMessage);
    }

    public static void Fail(GameObject go, string reason) {
        Fail(go);

        if (!string.IsNullOrEmpty(reason)) { Debug.Log(reason); }
    }

    public static void Fail(GameObject go) {
        go = FindTopGameObject(go);
        LogResult(go, failMessage);
    }

    public static void Assert(GameObject go, bool condition) {
        Assert(go, condition, "");
    }

    public static void Assert(GameObject go, bool condition, string message) {
        if (condition) { Pass(go); }

        else { Fail(go, message); }
    }

    private static void LogResult(GameObject go, string message) {
        Debug.Log(message + " (" + FindTopGameObject(go).name + ")",
                  go);
    }

    private static GameObject FindTopGameObject(GameObject go) {
        while (go.transform.parent != null)
        { go = go.transform.parent.gameObject; }

        return go;
    }

    #region Dynamic test attributes

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExcludePlatformAttribute : Attribute
    {
        public string[] platformsToExclude;

        public ExcludePlatformAttribute(params RuntimePlatform[] platformsToExclude) {
            this.platformsToExclude = platformsToExclude.Select(platform => platform.ToString()).ToArray();
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExpectExceptions : Attribute
    {
        public string[] exceptionTypeNames;
        public bool succeedOnException;

        public ExpectExceptions() : this(false) {
        }

        public ExpectExceptions(bool succeedOnException) : this(succeedOnException, new string[0]) {
        }

        public ExpectExceptions(bool succeedOnException, params string[] exceptionTypeNames) {
            this.succeedOnException = succeedOnException;
            this.exceptionTypeNames = exceptionTypeNames;
        }

        public ExpectExceptions(bool succeedOnException, params Type[] exceptionTypes)
            : this(succeedOnException, exceptionTypes.Select(type => type.FullName).ToArray()) {
        }

        public ExpectExceptions(params string[] exceptionTypeNames) : this(false, exceptionTypeNames) {
        }

        public ExpectExceptions(params Type[] exceptionTypes) : this(false, exceptionTypes) {
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class IgnoreAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DynamicTestAttribute : Attribute
    {
        private string sceneName;

        public DynamicTestAttribute(string sceneName) {
            if (sceneName.EndsWith(".unity"))
            { sceneName = sceneName.Substring(0, sceneName.Length - ".unity".Length); }

            this.sceneName = sceneName;
        }

        public bool IncludeOnScene(string sceneName) {
            var fileName = Path.GetFileNameWithoutExtension(sceneName);
            return fileName == this.sceneName;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SucceedWithAssertions : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TimeoutAttribute : Attribute
    {
        public float timeout;

        public TimeoutAttribute(float seconds) {
            this.timeout = seconds;
        }
    }

    #endregion
}
