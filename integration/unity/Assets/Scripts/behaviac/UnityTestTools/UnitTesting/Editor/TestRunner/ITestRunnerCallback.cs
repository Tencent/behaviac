using System;

namespace UnityTest.UnitTestRunner
{
    public interface ITestRunnerCallback {
        void TestStarted(string fullName);
        void TestFinished(ITestResult fullName);
        void RunStarted(string suiteName, int testCount);
        void RunFinished();
        void RunFinishedException(Exception exception);
    }
}
