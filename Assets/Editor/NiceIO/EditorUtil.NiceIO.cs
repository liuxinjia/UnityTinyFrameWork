using UnityEngine;
using UnityEditor;
using System.Reflection;

public sealed partial class EditorUtil
{
    public static string GetAssetAbsolutePath(string path)
    {
        return Application.dataPath +"/"+ path;
    }

    public static string GetProjectAbsolutePath(string path)
    {
        // Application.dataPath return <path to project folder>/Assets
        var projectPath = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);
        return projectPath + path;
    }

    public static string GetSuffix(string path)
    {
        string ext = System.IO.Path.GetExtension(path);
        string[] arg = ext.Split(new char[] { '.' });
        return arg[1];
    }

}