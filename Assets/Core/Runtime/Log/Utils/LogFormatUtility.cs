namespace Cr7Sund.Logger
{
    static class LogFormatUtility
    {
        public static string Format(string format, params object[] args) 
        {
            if (args == null || args.Length <= 0)
                return format;
            else
                return string.Format(format, args);
        }
    }
}
