using System.Diagnostics;

namespace Cr7Sund.Logger
{
    class Debug
    {
        [Conditional(MacroDefine.UNITY_EDITOR), Conditional(MacroDefine.DEBUG)]
        public static void UnityEditorDebug(string format, params object[] args)
        {
            if (args.Length <= 0)
                UnityEngine.Debug.Log(format);
            else
                UnityEngine.Debug.LogFormat(format, args);
        }

        [Conditional(MacroDefine.UNITY_EDITOR), Conditional(MacroDefine.DEBUG)]
        public static void UnityEditorWarning(string format, params object[] args)
        {
            if (args.Length <= 0)
                UnityEngine.Debug.LogWarning(format);
            else
                UnityEngine.Debug.LogWarningFormat(format, args);
        }

        [Conditional(MacroDefine.UNITY_EDITOR), Conditional(MacroDefine.DEBUG)]
        public static void UnityEditorError(string format, params object[] args)
        {
            if (args.Length <= 0)
                UnityEngine.Debug.LogError(format);
            else
                UnityEngine.Debug.LogErrorFormat(format, args);
        }
    }
}
