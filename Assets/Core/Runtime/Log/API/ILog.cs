
using System;

namespace Cr7Sund.Logger
{
    interface ILog:IDisposable
    {
        void Initialize();
        string Format(LogLevel level, string format, params object[] args);
    }
}