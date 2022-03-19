using UnityEngine;
using UnityEditor;
using System.Reflection;

public sealed partial class EditorUtil
{
    private MethodInfo clearConsoleMethodInfo;

    private void InitConsole()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        clearConsoleMethodInfo = type.GetMethod("Clear");
    }

    public void ClearConsoleLog()
    {
        if(clearConsoleMethodInfo == null) InitConsole();
        clearConsoleMethodInfo.Invoke(null, null);
    }
}