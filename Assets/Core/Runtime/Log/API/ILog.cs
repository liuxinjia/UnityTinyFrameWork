
namespace Cr7Sund.Logger
{
    interface ILog
    {
        void Initialize();
        string Format(LogLevel level, string format, params object[] args);
    }
}