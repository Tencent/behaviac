using System;
using UnityEngine;

namespace UnityTest
{
    [Serializable]
    public class TestResult : ITestResult, IComparable<TestResult>
    {
        private GameObject go;
        //private TestComponent testComponent;
        private string name;
        public ResultType resultType = ResultType.NotRun;
        public double duration;
        public string messages;
        public string stacktrace;
        public string id;
        public bool dynamicTest;

        public TestComponent TestComponent;

        public GameObject GameObject {
            get { return go; }
        }

        public TestResult(TestComponent testComponent) {
            this.TestComponent = testComponent;
            this.go = testComponent.gameObject;
            this.id = testComponent.gameObject.GetInstanceID().ToString();
            this.dynamicTest = testComponent.dynamic;

            if (go != null) { name = go.name; }

            if (dynamicTest)
            { id = testComponent.dynamicTypeName; }
        }

        public void Update(TestResult oldResult) {
            resultType = oldResult.resultType;
            duration = oldResult.duration;
            messages = oldResult.messages;
            stacktrace = oldResult.stacktrace;
        }

        public enum ResultType {
            Success,
            Failed,
            Timeout,
            NotRun,
            FailedException,
            Ignored
        }

        public void Reset() {
            resultType = ResultType.NotRun;
            duration = 0f;
            messages = "";
            stacktrace = "";
        }

        #region ITestResult implementation
        public TestResultState ResultState { get
            {
                switch (resultType) {
                    case ResultType.Success:
                        return TestResultState.Success;

                    case ResultType.Failed:
                        return TestResultState.Failure;

                    case ResultType.FailedException:
                        return TestResultState.Error;

                    case ResultType.Ignored:
                        return TestResultState.Ignored;

                    case ResultType.NotRun:
                        return TestResultState.Skipped;

                    case ResultType.Timeout:
                        return TestResultState.Cancelled;

                    default:
                        throw new Exception();
                }
            }
        }
        public string Message { get { return messages; } }
        public bool Executed { get { return resultType != ResultType.NotRun; } }
        public string Name { get { if (go != null) { name = go.name; } return name; } }
    public string Id { get { return id; } }
    public bool IsSuccess { get { return resultType == ResultType.Success; } }
    public bool IsTimeout { get { return resultType == ResultType.Timeout; } }
    public double Duration { get { return duration; } }
    public string StackTrace { get { return stacktrace; } }
    public string FullName {
        get
        {
            var fullName = Name;

            if (go != null) {
                    var tempGO = go.transform.parent;

                    while (tempGO != null) {
                        fullName = tempGO.name + "." + fullName;
                        tempGO = tempGO.transform.parent;
                    }
                }

                return fullName;
            }
        }

        public bool IsIgnored { get { return resultType == ResultType.Ignored; } }
        public bool IsFailure {
            get
            {
                return resultType == ResultType.Failed
                || resultType == ResultType.FailedException
                || resultType == ResultType.Timeout;
            }
        }
        #endregion


        #region IComparable, GetHashCode and Equals implementation
        public override int GetHashCode() {
            return id.GetHashCode();
        }

        public int CompareTo(TestResult other) {
            var result = Name.CompareTo(other.Name);

            if (result == 0)
            { result = go.GetInstanceID().CompareTo(other.go.GetInstanceID()); }

            return result;
        }

        public override bool Equals(object obj) {
            if (obj is TestResult)
            { return GetHashCode() == obj.GetHashCode(); }

            return base.Equals(obj);
        }
        #endregion
    }
}
