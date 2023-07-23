using System;

namespace Cr7Sund.Logger
{
    class CodeLogWriter : LogWriter<string>
    {
        public CodeLogWriter(ILogFileFormatting formatter, MMFile mmFile) : base(formatter, mmFile)
        {
        }

        protected override LogType LogType => LogType.Code;

        protected override string Formatting(string level, string id, string msg)
        {
            jsonData["log_time"] = DateTime.UtcNow.AddHours(8).ToString("yyyy-MM-ddTHH:mm:ss.fffzzzz");
            jsonData["log_level"] = level;
            jsonData["log_info"] = msg;
            return jsonData.ToJson();
        }
    }
}
