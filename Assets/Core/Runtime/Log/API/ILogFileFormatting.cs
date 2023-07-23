namespace Cr7Sund.Logger
{
    interface ILogFileFormatting
    {
        byte[] Formatting(byte[] buffer);
        byte[] UnFormatting(byte[] buffer);
    }
}
