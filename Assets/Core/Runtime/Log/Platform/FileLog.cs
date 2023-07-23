
using System;
using System.Collections.Generic;

namespace Cr7Sund.Logger
{
    class FileLog : ILog
        , ILogFlushable
    {
        private Dictionary<LogType, ILogWritable> _writers;
        // 日志保存上限日期
        private const int _timeout = 3 * 24 * 3600;
        /// <summary> 时间戳基准 </summary>
        internal static readonly DateTime StandardTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        #region Input

        private void Write(LogType type, LogLevel level, string msg) 
        {
            var writer = GetLogNode(type);
            writer.Write(level.ToString(), string.Empty, msg);
        }

        private void Write(LogType type, LogEventData data)
        {
            var writer = GetLogNode(type);
            writer.Write(data.type.ToString(), data.id, data);
        }

        public string Format(LogEventData data)
        {
            Write(LogType.Event, data);

#if UNITY_EDITOR
           var sb = new System.Text.StringBuilder();
            sb.Append(string.Format("[{0}] Type : {1}, ID : {2} ", LogLevel.Event, data.type, data.id ));
            sb.Append("Info : {");

            foreach(var current in data.info)
                sb.Append($"{current.Key} : {current.Value}");

            sb.Append("}");
            return sb.ToString();
#else
            return string.Empty;
#endif
        }

        public string Format(LogLevel level,LogChannel logChannel, string format, params object[] args)
        {
            string result = LogFormatUtility.Format(format, args);
            Write(LogType.Code, level, result);
            string logMessage = string.Format("[{0}][{1}]{2}", level, logChannel, result);
            return logMessage;
        }

        #endregion

        private ILogWritable GetLogNode(LogType logType) 
        {
            if (!_writers.TryGetValue(logType, out ILogWritable writer))
            {
                writer = LogWriterFactory.Create(logType);
                try
                {
                    CheckLogFileTimeout(logType);
                }
                catch (Exception e)
                {
                    Debug.UnityEditorError($"CheckLogFileTimeout failed : {e.Message} \n {e.StackTrace}");
                }
                _writers.Add(logType, writer);
            }
            return writer;
        }

        /// <summary>
        /// 检查日志是否超时
        /// </summary>
        /// <param name="logType"></param>
        private void CheckLogFileTimeout(LogType logType)
        {
            var logFiles = LogFileManager.GetFilesInDirector(LogFileUtil.GetFileDirector(logType));
            if (logFiles.Length <= 0) return;

            var curTime = (DateTime.UtcNow - StandardTime).TotalSeconds;
            string fileTime;

            Debug.UnityEditorDebug($"开始检查日志文件是否超时");
            for (int i = 0, len = logFiles.Length; i < len; i++)
            {
                fileTime = System.IO.Path.GetFileNameWithoutExtension(logFiles[i]);
                if (Int64.TryParse(fileTime, out long time))
                {
                    if (time + _timeout < curTime)
                    {
                        Debug.UnityEditorDebug($"Delete : {logFiles[i]}");
                        LogFileManager.DeleteFile(logFiles[i]);
                    }
                }
            }
        }

        public void Initialize()
        {
            LogFileManager.ExistOrCreate(LogFileUtil.LogDirector);
            _writers = new Dictionary<LogType, ILogWritable>();
        }

        /// <summary>
        /// 强制写入
        /// </summary>
        /// <param name="logType"></param>
        public void Flush(LogType logType)
        {
            try
            {
                var writer = GetLogNode(logType);
                writer.Flush();
            }
            catch(Exception e) 
            {
                Debug.UnityEditorError($"Flush failed : {e.Message} \n {e.StackTrace}");
            }
        }

        public void Dispose()
        {
            foreach (var writer in _writers.Values)
                (writer as IDisposable)?.Dispose();
            _writers.Clear();
        }
    }
}
