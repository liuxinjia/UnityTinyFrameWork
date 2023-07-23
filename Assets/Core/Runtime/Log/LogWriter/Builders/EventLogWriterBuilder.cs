namespace Cr7Sund.Logger
{
    class EventLogWriterBuilder : LogWriterBuilder
    {
        public override void BuildLogWriter()
        {
            _writer = new EventLogWriter(_formatter, _mmFile);
        }

        public override void BuildMMFile()
        {
            _mmFile = new MMFile(LogFileUtil.GetMemoryPath(LogType.Event), LogFileUtil.LogMemorySize);
        }
    }
}
