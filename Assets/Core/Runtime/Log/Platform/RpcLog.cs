// #if UNITY_STANDALONE
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Cr7Sud;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cr7Sund.Logger
{
    internal class RpcLog : ILog
    {
        private LogServer server;

        public string Format(LogLevel level, LogChannel logChannel, string format, params object[] args)
        {
            string result = LogFormatUtility.Format(format, args);
            string logMessage = string.Format("[{0}][{1}]{2}", level, logChannel, result);

            var st = new StackTrace();
            WriteToDevice(logMessage, st.ToString(), level);

            return logMessage;
        }

        public string Format(LogEventData data)
        {
            return string.Empty;
        }

        public async void Initialize()
        {
            server = new LogServer();
            await server.StartServer();
        }

        private void WriteToDevice(string logString, string stackTrace, LogLevel type)
        {
            var logInfo = new LogInfo
            {
                TimeStamp = TimeUtils.ConvertToTimestamp(System.DateTime.Now),
                Info = logString,
                StackTrace = stackTrace
            };
            string logMsg = JsonUtility.ToJson(logInfo);
            server?.SendAsync(logMsg);
        }

        public void Dispose()
        {
            server?.DisConnect();
        }

    }

    public class LogInfo
    {
        public long TimeStamp;
        public string Info;
        public string StackTrace;
    }
}
// #endif
