using System;
using System.Collections.Generic;

namespace UnityTest.IntegrationTestRunner
{
    public class TestRunnerCallbackList : ITestRunnerCallback
    {
        private List<ITestRunnerCallback> callbackList = new List<ITestRunnerCallback> ();

        public void Add(ITestRunnerCallback callback) {
            callbackList.Add(callback);
        }

        public void Remove(ITestRunnerCallback callback) {
            callbackList.Remove(callback);
        }

        public void RunStarted(string platform, List<TestComponent> testsToRun) {
            foreach(var unitTestRunnerCallback in callbackList) {
                unitTestRunnerCallback.RunStarted(platform, testsToRun);
            }
        }

        public void RunFinished(List<TestResult> testResults) {
            foreach(var unitTestRunnerCallback in callbackList) {
                unitTestRunnerCallback.RunFinished(testResults);
            }
        }

        public void TestStarted(TestResult test) {
            foreach(var unitTestRunnerCallback in callbackList) {
                unitTestRunnerCallback.TestStarted(test);
            }
        }

        public void TestFinished(TestResult test) {
            foreach(var unitTestRunnerCallback in callbackList) {
                unitTestRunnerCallback.TestFinished(test);
            }
        }

        public void TestRunInterrupted(List<ITestComponent> testsNotRun) {
            foreach(var unitTestRunnerCallback in callbackList) {
                unitTestRunnerCallback.TestRunInterrupted(testsNotRun);
            }
        }
    }
}
