using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security;
using System.Text;
using UnityEngine;

namespace UnityTest
{
    public class XmlResultWriter
    {
        private StringBuilder resultWriter = new StringBuilder();
        private int indend = 0;
        private string suiteName;
        private ITestResult[] results;

        public XmlResultWriter(string suiteName, ITestResult[] results) {
            this.suiteName = suiteName;
            this.results = results;
        }

        private const string nUnitVersion = "2.6.2-Unity";

        public string GetTestResult() {
            InitializeXmlFile(suiteName, new ResultSummarizer(results));
            foreach(var result in results) {
                WriteResultElement(result);
            }
            TerminateXmlFile();
            return resultWriter.ToString();
        }

        private void InitializeXmlFile(string resultsName, ResultSummarizer summaryResults) {
            WriteHeader();

            DateTime now = DateTime.Now;
            var attributes = new Dictionary<string, string> {
                {"name", "Unity Tests"},
                {"total", summaryResults.TestsRun.ToString()},
                {"errors", summaryResults.Errors.ToString()},
                {"failures", summaryResults.Failures.ToString()},
                {"not-run", summaryResults.TestsNotRun.ToString()},
                {"inconclusive", summaryResults.Inconclusive.ToString()},
                {"ignored", summaryResults.Ignored.ToString()},
                {"skipped", summaryResults.Skipped.ToString()},
                {"invalid", summaryResults.NotRunnable.ToString()},
                {"date", now.ToString("yyyy-MM-dd")},
                {"time", now.ToString("HH:mm:ss")}
            };

            WriteOpeningElement("test-results", attributes);

            WriteEnvironment();
            WriteCultureInfo();
            WriteTestSuite(resultsName, summaryResults);
            WriteOpeningElement("results");
        }

        private void WriteOpeningElement(string elementName) {
            WriteOpeningElement(elementName, new Dictionary<string, string> ());
        }

        private void WriteOpeningElement(string elementName, Dictionary<string, string> attributes) {
            WriteOpeningElement(elementName, attributes, false);
        }


        private void WriteOpeningElement(string elementName, Dictionary<string, string> attributes, bool closeImmediatelly) {
            WriteIndend();
            indend++;
            resultWriter.Append("<");
            resultWriter.Append(elementName);
            foreach(var attribute in attributes) {
                resultWriter.AppendFormat(" {0}=\"{1}\"", attribute.Key, SecurityElement.Escape(attribute.Value));
            }

            if (closeImmediatelly) {
                resultWriter.Append(" /");
                indend--;
            }

            resultWriter.AppendLine(">");
        }

        private void WriteIndend() {
            for (int i = 0; i < indend; i++) {
                resultWriter.Append("  ");
            }
        }

        private void WriteClosingElement(string elementName) {
            indend--;
            WriteIndend();
            resultWriter.AppendLine("</" + elementName + ">");
        }

        private void WriteHeader() {
            resultWriter.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            resultWriter.AppendLine("<!--This file represents the results of running a test suite-->");
        }

        static string GetEnvironmentUserName() {
#if !UNITY_WP8 && !UNITY_METRO
            return Environment.UserName;
#else
            return "";
#endif
        }

        static string GetEnvironmentMachineName() {
#if !UNITY_WP8 && !UNITY_METRO
            return Environment.MachineName;
#else
            return "";
#endif
        }

        static string GetEnvironmentUserDomainName() {
#if !UNITY_WP8 && !UNITY_METRO
            return Environment.UserDomainName;
#else
            return "";
#endif
        }

        static string GetEnvironmentVersion() {
#if !UNITY_METRO
            return Environment.Version.ToString();
#else
            return "";
#endif
        }

        static string GetEnvironmentOSVersion() {
#if !UNITY_METRO
            return Environment.OSVersion.ToString();
#else
            return "";
#endif
        }

        static string GetEnvironmentOSVersionPlatform() {
#if !UNITY_METRO
            return Environment.OSVersion.Platform.ToString();
#else
            return "";
#endif
        }

        static string EnvironmentGetCurrentDirectory() {
#if !UNITY_METRO
            return Environment.CurrentDirectory;
#else
            return "";
#endif
        }

        private void WriteEnvironment() {
            var attributes = new Dictionary<string, string> {
                {"nunit-version", nUnitVersion},
                {"clr-version", GetEnvironmentVersion()},
                {"os-version", GetEnvironmentOSVersion()},
                {"platform", GetEnvironmentOSVersionPlatform()},
                {"cwd", EnvironmentGetCurrentDirectory()},
                {"machine-name", GetEnvironmentMachineName()},
                {"user", GetEnvironmentUserName()},
                {"user-domain", GetEnvironmentUserDomainName()}
            };
            WriteOpeningElement("environment", attributes, true);
        }

        private void WriteCultureInfo() {
            var attributes = new Dictionary<string, string> {
                {"current-culture", CultureInfo.CurrentCulture.ToString()},
                {"current-uiculture", CultureInfo.CurrentUICulture.ToString()}
            };
            WriteOpeningElement("culture-info", attributes, true);
        }

        private void WriteTestSuite(string resultsName, ResultSummarizer summaryResults) {
            var attributes = new Dictionary<string, string> {
                {"name", resultsName},
                {"type", "Assembly"},
                {"executed", "True"},
                {"result", summaryResults.Success ? "Success" : "Failure"},
                {"success", summaryResults.Success ? "True" : "False"},
                {"time", summaryResults.Duration.ToString("#####0.000", NumberFormatInfo.InvariantInfo)}
            };
            WriteOpeningElement("test-suite", attributes);
        }

        private void WriteResultElement(ITestResult result) {
            StartTestElement(result);

            switch (result.ResultState) {
                case TestResultState.Ignored:
                case TestResultState.NotRunnable:
                case TestResultState.Skipped:
                    WriteReasonElement(result);
                    break;

                case TestResultState.Failure:
                case TestResultState.Error:
                case TestResultState.Cancelled:
                    WriteFailureElement(result);
                    break;

                case TestResultState.Success:
                case TestResultState.Inconclusive:
                    if (result.Message != null)
                    { WriteReasonElement(result); }

                    break;
            }

            WriteClosingElement("test-case");
        }

        private void TerminateXmlFile() {
            WriteClosingElement("results");
            WriteClosingElement("test-suite");
            WriteClosingElement("test-results");
        }

        #region Element Creation Helpers

        private void StartTestElement(ITestResult result) {
            var attributes = new Dictionary<string, string> {
                {"name", result.FullName},
                {"executed", result.Executed.ToString()}
            };
            var resultString = "";

            switch (result.ResultState) {
                case TestResultState.Cancelled:
                    resultString = TestResultState.Failure.ToString();
                    break;

                default:
                    resultString = result.ResultState.ToString();
                    break;
            }

            attributes.Add("result", resultString);

            if (result.Executed) {
                attributes.Add("success", result.IsSuccess.ToString());
                attributes.Add("time", result.Duration.ToString("#####0.000", NumberFormatInfo.InvariantInfo));
            }

            WriteOpeningElement("test-case", attributes);
        }

        private void WriteReasonElement(ITestResult result) {
            WriteOpeningElement("reason");
            WriteOpeningElement("message");
            WriteCData(result.Message);
            WriteClosingElement("message");
            WriteClosingElement("reason");
        }

        private void WriteFailureElement(ITestResult result) {
            WriteOpeningElement("failure");
            WriteOpeningElement("message");
            WriteCData(result.Message);
            WriteClosingElement("message");
            WriteOpeningElement("stack-trace");

            if (result.StackTrace != null)
            { WriteCData(StackTraceFilter.Filter(result.StackTrace)); }

            WriteClosingElement("stack-trace");
            WriteClosingElement("failure");
        }

        #endregion

        private void WriteCData(string text) {
            if (text.Length == 0)
            { return; }

            resultWriter.AppendFormat("<![CDATA[{0}]]>", text);
            resultWriter.AppendLine();
        }

#if !UNITY_METRO
        public void WriteToFile(string resultDestiantion, string resultFileName) {
            try {
                var path = System.IO.Path.Combine(resultDestiantion, resultFileName);
                Debug.Log("Saving results in " + path);
                using(var fs = System.IO.File.OpenWrite(path))
                using(var sw = new System.IO.StreamWriter(fs, Encoding.UTF8)) {
                    sw.Write(GetTestResult());
                }

            } catch (Exception e) {
                Debug.LogError("Error while opening file");
                Debug.LogException(e);
            }
        }
#endif
    }
}
