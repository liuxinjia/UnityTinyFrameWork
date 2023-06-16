namespace Cr7Sund
{
    /// <summary>
    /// 宏定义
    /// </summary>
    public static class MacroDefine
    {
        //Release
        public const string FINAL_RELEASE = "FINAL_RELEASE";

        //Debug
        public const string DEBUG = "DEBUG";

        //Profiler
        public const string PROFILER = "PROFILER";

        //Editor
        public const string UNITY_EDITOR = "UNITY_EDITOR";
        public const string UNITY_EDITOR_WIN = "UNITY_EDITOR_WIN";
        public const string UNITY_EDITOR_OSX = "UNITY_EDITOR_OSX";

        //Standalone
        public const string UNITY_STANDALONE = "UNITY_STANDALONE";
        public const string UNITY_STANDALONE_WIN = "UNITY_STANDALONE_WIN";
        public const string UNITY_STANDALONE_OSX = "UNITY_STANDALONE_OSX";

        //Android
        public const string UNITY_ANDROID = "UNITY_ANDROID";

        //iOS
        public const string UNITY_IOS = "UNITY_IOS";

        //No_Try_Catch
        public const string NO_TRY_CATCH = "NO_TRY_CATCH";

        public const string KAILASH_ASSERT = "KAILASH_ASSERT";
        public const string KAILASH_LOG = "KAILASH_LOG";

        public const string KAILASH_TEST_INCLUDE = "KAILASH_TEST_INCLUDE";



        public static bool IsEditor
        {
            get
            {
#if UNITY_EDITOR
                return true;
#else
                return false;
#endif
            }
        }

        public static bool IsEditorWin
        {
            get
            {
#if UNITY_EDITOR_WIN
                return true;
#else
                return false;
#endif
            }
        }

        public static bool IsEditorOSX
        {
            get
            {
#if UNITY_EDITOR_OSX
                return true;
#else
                return false;
#endif
            }
        }

        public static bool IsStandalone
        {
            get
            {
#if UNITY_STANDALONE
                return true;
#else
                return false;
#endif
            }
        }

        public static bool IsStandaloneWin
        {
            get
            {
#if UNITY_STANDALONE_WIN
                return true;
#else
                return false;
#endif
            }
        }

        public static bool IsStandaloneOSX
        {
            get
            {
#if UNITY_STANDALONE_OSX
                return true;
#else
                return false;
#endif
            }
        }


        public static bool IsAndroid
        {
            get
            {
#if UNITY_ANDROID
                return true;
#else
                return false;
#endif
            }
        }


        public static bool IsiOS
        {
            get
            {
#if UNITY_IOS
                return true;
#else
                return false;
#endif
            }
        }



        public static bool IsRelease
        {
            get
            {
#if FINAL_RELEASE
                return true;
#else
                return false;
#endif
            }
        }

        public static bool IsDebug
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }


        public static bool IsProfiler
        {
            get
            {
#if PROFILER
                return true;
#else
                return false;
#endif
            }
        }
    }

}
