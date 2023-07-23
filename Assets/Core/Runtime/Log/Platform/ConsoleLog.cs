
using System.Text;

namespace Cr7Sund.Logger
{
    class ConsoleLog : ILog
    {

        public string Format(LogLevel level, string format, params object[] args)
        {
            string result = LogFormatUtility.Format(format, args);
            result = string.Format("[{0}] {1}", level, result);
            return result;
        }

        public string Format(LogEventData data)
        {
#if UNITY_EDITOR
            StringBuilder sb = new StringBuilder();
            var str = sb.Append(string.Format("[{0}] Type : {1}, ID : {2} ", LogLevel.Event, data.type, data.id));
            sb.Append("Info : {");

            foreach (var current in data.info)
                sb.Append($"{current.Key} : {current.Value}");

            sb.Append("}");
            return sb.ToString();
#else
            return string.Empty;
#endif
        }

        public void Initialize()
        {

        }
        public void Dispose()
        {
        }
    }
}
