using System;
using System.IO;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class IntegrationTestAttribute : Attribute
{
    private string path;

    public IntegrationTestAttribute(string path) {
        if (path.EndsWith(".unity"))
        { path = path.Substring(0, path.Length - ".unity".Length); }

        this.path = path;
    }

    public bool IncludeOnScene(string scenePath) {
        if (scenePath == path) { return true; }

        var fileName = Path.GetFileNameWithoutExtension(scenePath);
        return fileName == path;
    }
}
