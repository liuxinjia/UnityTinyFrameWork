
namespace Cr7Sund.Logger
{
    static class LogWriterFactory
    {
        public static ILogWritable Create(LogType type)
        {
            LogFileManager.ExistOrCreate(LogFileUtil.GetFileDirector(type));
            LogWriterBuilder builder;

            switch (type)
            {
                case LogType.Code:
                    builder = new CodeLogWriterBuilder();
                    break;
                case LogType.Event:
                    builder = new EventLogWriterBuilder();
                    break;
                default:
                    throw new System.Exception("未支持的日志类型");
            }

            return LogWriterDirector.Construct(builder);
        }
    }
}

