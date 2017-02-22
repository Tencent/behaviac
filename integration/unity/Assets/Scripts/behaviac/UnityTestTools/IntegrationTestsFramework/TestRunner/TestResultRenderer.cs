using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestResultRenderer
{
    private static class Styles
    {
        public static GUIStyle succeedLabelStyle;
        public static GUIStyle failedLabelStyle;
        public static GUIStyle failedMessagesStyle;

        static Styles() {
            succeedLabelStyle = new GUIStyle("label");
            succeedLabelStyle.normal.textColor = Color.green;
            succeedLabelStyle.fontSize = 48;

            failedLabelStyle = new GUIStyle("label");
            failedLabelStyle.normal.textColor = Color.red;
            failedLabelStyle.fontSize = 32;

            failedMessagesStyle = new GUIStyle("label");
            failedMessagesStyle.wordWrap = false;
            failedMessagesStyle.richText = true;
        }
    }
    private Dictionary<string, List<ITestResult>> testCollection = new Dictionary<string, List<ITestResult>> ();

    private bool showResults;
    Vector2 scrollPosition;

    public void ShowResults() {
        showResults = true;
        Screen.showCursor = true;
    }

    public void AddResults(string sceneName, ITestResult result) {
        if (!testCollection.ContainsKey(sceneName))
        { testCollection.Add(sceneName, new List<ITestResult> ()); }

        testCollection[sceneName].Add(result);
    }

    public void Draw() {
        if (!showResults) { return; }

        if (testCollection.Count == 0) {
            GUILayout.Label("All test succeeded", Styles.succeedLabelStyle, GUILayout.Width(600));

        } else {
            int count = 0;
            foreach(var testGroup in testCollection) count += testGroup.Value.Count;
            GUILayout.Label(count + " tests failed!", Styles.failedLabelStyle);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandWidth(true));
            var text = "";
            foreach(var testGroup in testCollection) {
                text += "<b><size=18>" + testGroup.Key + "</size></b>\n";
                text += string.Join("\n", testGroup.Value
                                    .Where(result => !result.IsSuccess)
                                    .Select(result => result.Name + " " + result.ResultState + "\n" + result.Message)
                                    .ToArray());
            }
            GUILayout.TextArea(text, Styles.failedMessagesStyle);
            GUILayout.EndScrollView();
        }

        if (GUILayout.Button("Close"))
        { Application.Quit(); }
    }
}
