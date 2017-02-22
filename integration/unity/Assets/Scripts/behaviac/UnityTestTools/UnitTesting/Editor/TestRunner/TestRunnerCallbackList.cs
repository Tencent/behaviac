using System;
using System.Collections.Generic;

namespace UnityTest.UnitTestRunner
{
    public class TestRunnerCallbackList : ITestRunnerCallback
    {
        private List<ITestRunnerCallback> callbackList = new List<ITestRunnerCallback> ();

        public void TestStarted(string fullName) {
            foreach(var unitTestRunnerCallback in callbackList) {
                unitTestRunnerCallback.TestStarted(fullName);
            }
        }

        public void TestFinished(ITestResult fullName) {
            foreach(var unitTestRunnerCallback in callbackList) {
                unitTestRunnerCallback.TestFinished(fullName);
            }
        }

        public void RunStarted(string suiteName, int testCount) {
            foreach(var unitTestRunnerCallback in callbackList) {
                unitTestRunnerCallback.RunStarted(suiteName,
                                                  testCount);
            }
        }

        public void RunFinished() {
            foreach(var unitTestRunnerCallback in callbackList) {
                unitTestRunnerCallback.RunFinished();
            }
        }

        public void RunFinishedException(Exception exception) {
            foreach(var unitTestRunnerCallback in callbackList) {
                unitTestRunnerCallback.RunFinishedException(exception);
            }
        }

        public void Add(ITestRunnerCallback callback) {
            callbackList.Add(callback);
        }

        public void Remove(ITestRunnerCallback callback) {
            callbackList.Remove(callback);
        }
    }
}
