// ****************************************************************
// Based on nUnit 2.6.2 (http://www.nunit.org/)
// ****************************************************************

using System;

namespace UnityTest
{
    /// <summary>
    /// Summary description for ResultSummarizer.
    /// </summary>
    public class ResultSummarizer
    {
        private int errorCount = 0;
        private int failureCount = 0;
        private int ignoreCount = 0;
        private int inconclusiveCount = 0;
        private int notRunnable = 0;
        private int resultCount = 0;
        private int skipCount = 0;
        private int successCount = 0;
        private int testsRun = 0;

        private TimeSpan duration = new TimeSpan();

        public ResultSummarizer(ITestResult[] results) {
            foreach(var result in results)
            Summarize(result);
        }

        public bool Success {
            get { return failureCount == 0; }
        }

        /// <summary>
        /// Returns the number of test cases for which results
        /// have been summarized. Any tests excluded by use of
        /// Category or Explicit attributes are not counted.
        /// </summary>
        public int ResultCount {
            get { return resultCount; }
        }

        /// <summary>
        /// Returns the number of test cases actually run, which
        /// is the same as ResultCount, less any Skipped, Ignored
        /// or NonRunnable tests.
        /// </summary>
        public int TestsRun {
            get { return testsRun; }
        }

        /// <summary>
        /// Returns the number of tests that passed
        /// </summary>
        public int Passed {
            get { return successCount; }
        }

        /// <summary>
        /// Returns the number of test cases that had an error.
        /// </summary>
        public int Errors {
            get { return errorCount; }
        }

        /// <summary>
        /// Returns the number of test cases that failed.
        /// </summary>
        public int Failures {
            get { return failureCount; }
        }

        /// <summary>
        /// Returns the number of test cases that failed.
        /// </summary>
        public int Inconclusive {
            get { return inconclusiveCount; }
        }

        /// <summary>
        /// Returns the number of test cases that were not runnable
        /// due to errors in the signature of the class or method.
        /// Such tests are also counted as Errors.
        /// </summary>
        public int NotRunnable {
            get { return notRunnable; }
        }

        /// <summary>
        /// Returns the number of test cases that were skipped.
        /// </summary>
        public int Skipped {
            get { return skipCount; }
        }

        public int Ignored {
            get { return ignoreCount; }
        }

        public double Duration {
            get { return duration.TotalSeconds; }
        }

        public int TestsNotRun {
            get { return skipCount + ignoreCount + notRunnable; }
        }

        public void Summarize(ITestResult result) {
            duration += TimeSpan.FromSeconds(result.Duration);
            resultCount++;

            switch (result.ResultState) {
                case TestResultState.Success:
                    successCount++;
                    testsRun++;
                    break;

                case TestResultState.Failure:
                    failureCount++;
                    testsRun++;
                    break;

                case TestResultState.Error:
                case TestResultState.Cancelled:
                    errorCount++;
                    testsRun++;
                    break;

                case TestResultState.Inconclusive:
                    inconclusiveCount++;
                    testsRun++;
                    break;

                case TestResultState.NotRunnable:
                    notRunnable++;
                    //errorCount++;
                    break;

                case TestResultState.Ignored:
                    ignoreCount++;
                    break;

                case TestResultState.Skipped:
                default:
                    skipCount++;
                    break;
            }
        }
    }
}
