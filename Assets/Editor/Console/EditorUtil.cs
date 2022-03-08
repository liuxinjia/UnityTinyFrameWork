using UnityEngine;
using UnityEditor;
using System.Reflection;

public sealed partial class EditorUtil
{
    private static volatile EditorUtil instance;
    private static Object syncRootObject = new Object();

    private MethodInfo clearConsoleMethodInfo;
    public static EditorUtil Instance
    {
        get
        {
            if (instance == null)
            {
                lock (syncRootObject)
                {
                    if (instance == null)
                    {
                        instance = new EditorUtil();
                        instance.Init();
                    }
                }
            }
            return instance;
        }
    }

    private void Init()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        clearConsoleMethodInfo = type.GetMethod("Clear");

    }

    public void ClearConsoleLog()
    {
        clearConsoleMethodInfo.Invoke(null, null);
    }
}