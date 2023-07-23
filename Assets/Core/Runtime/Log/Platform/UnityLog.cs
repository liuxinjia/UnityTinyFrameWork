#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace Cr7Sund.Logger
{
    internal class UnityLog : ILog
    {
        private Color32 GetLocalColor(string key, Color32 defaultColor)
        {
            var str = UnityEditor.EditorPrefs.GetString(key, string.Empty);
            if (!string.IsNullOrEmpty(str))
                return JsonUtility.FromJson<Color32>(str);
            else
                return defaultColor;
        }

        Dictionary<string, Color32> _colors = new Dictionary<string, Color32>();

        public UnityLog()
        {
            Debug.UnityEditorDebug("构建UnityLog");

            var color = GetLocalColor("log trace", Color.white);
            _colors["log trace"] = color;

            color = GetLocalColor("log debug", LogColorHelp.HexToColor(0xFF8428D9));
            _colors["log debug"] = color;

            color = GetLocalColor("log info", LogColorHelp.HexToColor(0xFF0E42BC));
            _colors["log info"] = color;

            color = GetLocalColor("log warn", LogColorHelp.HexToColor(0xFFFFFF00));
            _colors["log warn"] = color;

            color = GetLocalColor("log error", LogColorHelp.HexToColor(0xFFFF0000));
            _colors["log error"] = color;

            color = GetLocalColor("log fatal", LogColorHelp.HexToColor(0xFFF000FF));
            _colors["log fatal"] = color;

            color = GetLocalColor("log event", Color.white);
            _colors["log event"] = color;
        }

        private string DecorateColor(LogLevel level, string msg)
        {
            Color32 color;
            //unity主线程默认为1
            if (System.Threading.Thread.CurrentThread.ManagedThreadId != 1)
            {
                color = level switch
                {
                    LogLevel.Trace => _colors["log trace"],
                    LogLevel.Debug => _colors["log debug"],
                    LogLevel.Info => _colors["log info"],
                    LogLevel.Warn => _colors["log warn"],
                    LogLevel.Error => _colors["log error"],
                    LogLevel.Fatal => _colors["log fatal"],
                    LogLevel.Event => _colors["log event"],
                    _ => (Color32)Color.white,
                };
            }
            else
            {
                color = level switch
                {
                    LogLevel.Trace => GetLocalColor("log trace", Color.white),
                    LogLevel.Debug => GetLocalColor("log debug", LogColorHelp.HexToColor(0xFF8428D9)),
                    LogLevel.Info => GetLocalColor("log info", LogColorHelp.HexToColor(0xFF0E42BC)),
                    LogLevel.Warn => GetLocalColor("log warn", LogColorHelp.HexToColor(0xFFFFFF00)),
                    LogLevel.Error => GetLocalColor("log error", LogColorHelp.HexToColor(0xFFFF0000)),
                    LogLevel.Fatal => GetLocalColor("log fatal", LogColorHelp.HexToColor(0xFFF000FF)),
                    LogLevel.Event => GetLocalColor("log event", Color.white),
                    _ => (Color32)Color.white,
                };
            }

            return string.Format("<color=#{0}>{1}</color>", LogColorHelp.ColorToHex(color), msg);
        }

        public string Format(LogLevel level, LogChannel logChannel, string format, params object[] args)
        {
            string result = LogFormatUtility.Format(format, args);
            string logMessage = string.Format("[{0}][{1}]{2}", level, logChannel, result);

            result = DecorateColor(level, logMessage);
            return result;
        }

        public void Initialize()
        {

        }

        public void Dispose()
        {
        }
    }
}
#endif
