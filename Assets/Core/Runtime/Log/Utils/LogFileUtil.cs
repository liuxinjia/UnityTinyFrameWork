using System;
using System.IO;
using UnityEngine;

namespace Cr7Sund.Logger
{
    static class LogFileUtil
    {

        private static readonly string _dataPath = Application.dataPath;

#if UNITY_EDITOR
        private static readonly string _streamingAssetsPath = Environment.CurrentDirectory.Replace("\\", "/") + "/ExtAssets/StreamAssets";
#else
        private static readonly string _streamingAssetsPath = Application.streamingAssetsPath;
#endif

#if UNITY_EDITOR
        private static readonly string _persistentDataPath = Environment.CurrentDirectory.Replace("\\", "/") + "/ExtAssets";
#elif UNITY_STANDALONE
        private static readonly string _persistentDataPath = Application.dataPath;
#else
        private static readonly string _persistentDataPath = Application.persistentDataPath;
#endif

#if UNITY_EDITOR
        public static string LogDirector = System.Environment.CurrentDirectory.Replace("\\", "/") + "/ExtAssets/Logs";
#else
        public static string LogDirector = _persistentDataPath + "/Logs";
#endif

        private static readonly string _configPath = "Configs/Log.bytes";

        #region 代码日志
        private static readonly string _codeMMF = "codelogmemory.bytes";
        private static readonly string _codeDirector = Path.Combine(LogDirector, "Codes");
        private static readonly string _codeMMFPath = Path.Combine(LogDirector, _codeMMF);
        #endregion

        #region 事件日志
        private static readonly string _eventMMF = "eventlogmemory.bytes";
        private static readonly string _eventDirector = Path.Combine(LogDirector, "Events");
        private static readonly string _eventMMFPath = Path.Combine(LogDirector, _eventMMF);
        #endregion

        /// <summary> 日志映射的内存上限 </summary>
        public static readonly int LogMemorySize = 1024 * 1024;

        /// <summary>
        /// 获取对应类型日志的MMF名称
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetMMFileMapName(LogType type)
        {
            switch (type)
            {
                case LogType.Code:
                    return "code";
                case LogType.Event:
                    return "event";
                default:
                    throw new System.Exception("未支持的日志类型");
            }
        }

        /// <summary>
        /// 获取对应日志文件所在的文件夹
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetFileDirector(LogType type)
        {
            switch (type)
            {
                case LogType.Code:
                    return LogFileUtil._codeDirector;
                case LogType.Event:
                    return LogFileUtil._eventDirector;
                default:
                    throw new System.Exception("未支持的日志类型");
            }
        }

        /// <summary>
        /// 拼接对应日志本地缓存的路径
        /// </summary>
        /// <param name="type"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string GetCopyFilePath(LogType type, string file)
        {
            switch (type)
            {
                case LogType.Code:
                    return Path.Combine(_codeDirector, $"{file}.bytes");
                case LogType.Event:
                    return Path.Combine(_eventDirector, $"{file}.bytes");
                default:
                    throw new System.Exception("未支持的日志类型");
            }
        }

        /// <summary>
        /// 获取对应日志MMF所在路径
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetMemoryPath(LogType type)
        {
            switch (type)
            {
                case LogType.Code:
                    return LogFileUtil._codeMMFPath;
                case LogType.Event:
                    return LogFileUtil._eventMMFPath;
                default:
                    throw new System.Exception("未支持的日志类型");

            }
        }

        /// <summary>
        /// 获取游戏包里的文件路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="includeUpdate"></param>
        /// <returns></returns>
        public static string GetLogConfigPath()
        {
            //找不到再从内置资源里取
            if (Application.platform == RuntimePlatform.Android)
                return $"{_dataPath}!assets/{LogFileUtil._configPath}";
            else
                return Path.Combine(_streamingAssetsPath, LogFileUtil._configPath);
        }
    }
}