namespace Cr7Sund.Logger
{
    enum LogLevel
    {
        Trace,
        Debug,
        Info,
        Warn,
        Error,
        Fatal,
        Event,
    }

    enum LogType : byte
    { 
        /// <summary> 代码日志 </summary>
        Code = 1,

        /// <summary> 埋点日志 </summary>
        Event = 1 << 1,
    }
}
