using System.Collections.Generic;

namespace Cr7Sund.Logger
{
    /// <summary>
    /// 埋点数据
    /// </summary>
    struct LogEventData
    {
        /// <summary>
        /// 埋点类型
        /// </summary>
        public string type;

        /// <summary>
        /// 埋点ID
        /// </summary>
        public string id;

        /// <summary>
        /// 埋点信息
        /// </summary>

        public Dictionary<string, string> info;

        public LogEventData(string type, string id)
        {
            this.type = type;
            this.id = id;
            this.info = new Dictionary<string, string>();
        }

        public LogEventData Add(string title, string content)
        {
            this.info.Add(title, content);
            return this;
        }
    }
}
