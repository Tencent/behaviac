using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityTest
{
    public static class Icons
    {
        private const string iconsAssetsPathPattern = "Common/Editor/icons/";
        private static string iconsAssetsPath = "";

        public static readonly Texture2D failImg;
        public static readonly Texture2D ignoreImg;
        public static readonly Texture2D runImg;
        public static readonly Texture2D runFailedImg;
        public static readonly Texture2D runAllImg;
        public static readonly Texture2D successImg;
        public static readonly Texture2D unknownImg;
        public static readonly Texture2D inconclusiveImg;
        public static readonly Texture2D stopwatchImg;
        public static readonly Texture2D plusImg;
        public static readonly Texture2D gearImg;

        public static readonly GUIContent guiUnknownImg;
        public static readonly GUIContent guiInconclusiveImg;
        public static readonly GUIContent guiIgnoreImg;
        public static readonly GUIContent guiSuccessImg;
        public static readonly GUIContent guiFailImg;

        static Icons() {
            var dirs = Directory.GetDirectories("Assets", "UnityTestTools", SearchOption.AllDirectories);

            if (dirs.Length > 0)
            { iconsAssetsPath = Path.Combine(dirs[0], iconsAssetsPathPattern); }

            else
            { Debug.LogWarning("The UnityTestTools asset folder path is incorrect. If you relocated the tools please change the path accordingly (Icons.cs)."); }

            failImg = LoadTexture("failed.png");
            ignoreImg = LoadTexture("ignored.png");
            successImg = LoadTexture("passed.png");
            unknownImg = LoadTexture("normal.png");
            inconclusiveImg = LoadTexture("inconclusive.png");
            stopwatchImg = LoadTexture("stopwatch.png");

            if (EditorGUIUtility.isProSkin) {
                runAllImg = LoadTexture("play-darktheme.png");
                runImg = LoadTexture("play_selected-darktheme.png");
                runFailedImg = LoadTexture("rerun-darktheme.png");
                plusImg = LoadTexture("create-darktheme.png");
                gearImg = LoadTexture("options-darktheme.png");

            } else {
                runAllImg = LoadTexture("play-lighttheme.png");
                runImg = LoadTexture("play_selected-lighttheme.png");
                runFailedImg = LoadTexture("rerun-lighttheme.png");
                plusImg = LoadTexture("create-lighttheme.png");
                gearImg = LoadTexture("options-lighttheme.png");
            }

            guiUnknownImg = new GUIContent(unknownImg);
            guiInconclusiveImg = new GUIContent(inconclusiveImg);
            guiIgnoreImg = new GUIContent(ignoreImg);
            guiSuccessImg = new GUIContent(successImg);
            guiFailImg = new GUIContent(failImg);
        }

        private static Texture2D LoadTexture(string fileName) {
            return (Texture2D)Resources.LoadAssetAtPath(iconsAssetsPath + fileName, typeof(Texture2D));
        }
    }
}
