namespace Cr7Sund.Logger
{
    abstract class LogWriterBuilder
    {
        protected ILogWritable _writer;
        protected MMFile _mmFile;

        protected ILogFileFormatting _formatter;

        public void BuildFormatter()
        {
            _formatter = new LogFileFormatter();
        }

        public abstract void BuildMMFile();
        public abstract void BuildLogWriter();

        public ILogWritable GetProduct()
        {
            return _writer;
        }
    }
}
