using UnityEngine;
using UnityEditor;
using System.Reflection;

public sealed partial class EditorUtil
{
    private static volatile EditorUtil instance;
    private static Object syncRootObject = new Object();

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

                    }
                }
            }
            return instance;
        }
    }
}