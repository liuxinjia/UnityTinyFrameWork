namespace Cr7Sund.Logger
{
    static class LogWriterDirector
    {
        public static ILogWritable Construct(LogWriterBuilder builder)
        {
            builder.BuildMMFile();
            builder.BuildFormatter();
            builder.BuildLogWriter();
            return builder.GetProduct();
        }
    }
}
