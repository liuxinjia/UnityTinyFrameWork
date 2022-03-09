using UnityEngine;
using UnityEditor;
using System.Reflection;

public sealed partial class EditorUtil
{
    public string GetAssetAbsolutePath(string path)
    {
        return Application.dataPath + path;
    }

    public string GetProjectAbsolutePath(string path)
    {
        // Application.dataPath return <path to project folder>/Assets
        var projectPath = Application.dataPath.Substring(0, Application.dataPath.Length - "/Assets".Length);
        return projectPath + path;
    }
}