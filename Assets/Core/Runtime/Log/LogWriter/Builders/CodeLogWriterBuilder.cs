namespace Cr7Sund.Logger
{
    class CodeLogWriterBuilder : LogWriterBuilder
    {
        public override void BuildLogWriter()
        {
            _writer = new CodeLogWriter(_formatter, _mmFile);
        }
        
        public override void BuildMMFile()
        {
            _mmFile = new MMFile(LogFileUtil.GetMemoryPath(LogType.Code), LogFileUtil.LogMemorySize);
        }
    }
}
