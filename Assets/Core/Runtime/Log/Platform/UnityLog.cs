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
                switch (level)
                {
                    case LogLevel.Trace: color = _colors["log trace"]; break;
                    case LogLevel.Debug: color = _colors["log debug"]; break;
                    case LogLevel.Info: color = _colors["log info"]; break;
                    case LogLevel.Warn: color = _colors["log warn"]; break;
                    case LogLevel.Error: color = _colors["log error"]; break;
                    case LogLevel.Fatal: color = _colors["log fatal"]; break;
                    case LogLevel.Event: color = _colors["log event"]; break;
                    default: color = Color.white; break;
                }
            }
            else
            {
                switch (level)
                {
                    case LogLevel.Trace: color = GetLocalColor("log trace", Color.white); break;
                    case LogLevel.Debug: color = GetLocalColor("log debug", LogColorHelp.HexToColor(0xFF8428D9)); break;
                    case LogLevel.Info: color = GetLocalColor("log info", LogColorHelp.HexToColor(0xFF0E42BC)); break;
                    case LogLevel.Warn: color = GetLocalColor("log warn", LogColorHelp.HexToColor(0xFFFFFF00)); break;
                    case LogLevel.Error: color = GetLocalColor("log error", LogColorHelp.HexToColor(0xFFFF0000)); break;
                    case LogLevel.Fatal: color = GetLocalColor("log fatal", LogColorHelp.HexToColor(0xFFF000FF)); break;
                    case LogLevel.Event: color = GetLocalColor("log event", Color.white); break;
                    default: color = Color.white; break;
                }
            }

            return string.Format("<color=#{0}>{1}</color>", LogColorHelp.ColorToHex(color), msg);
        }

        public string Format(LogLevel level, string format, params object[] args)
        {
            string result = LogFormatUtility.Format(format, args);
            result = DecorateColor(level, string.Format("[{0}] {1}", level, result));
            return result;
        }

        public void Initialize()
        {

        }
    }
}
#endif
