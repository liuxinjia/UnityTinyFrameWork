
namespace Cr7Sund.Logger
{
    /// <summary>
    /// 日志文件格式化
    /// zstd 压缩
    /// </summary>
    class LogFileFormatter : ILogFileFormatting
    {
        public byte[] Formatting(byte[] buffer)
        {
            buffer = LoggerNative.ZstdCompress(buffer);
            return buffer;
        }

        public byte[] UnFormatting(byte[] buffer)
        {
            buffer = LoggerNative.ZstdDecompress(buffer);
            return buffer;
        }
    }
}
