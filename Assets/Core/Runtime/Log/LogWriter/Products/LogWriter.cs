using System;
using System.Threading;
using LitJson;

namespace Cr7Sund.Logger
{
    abstract class LogWriter<TMsg> : ILogWritable
        , IDisposable
    {
        protected ILogFileFormatting _formatter;
        protected MMFile _mmFile;
        protected abstract LogType LogType { get; }
        protected JsonData jsonData = new JsonData();

        public object _syncRoot;
        private object SyncRoot
        {
            get
            {
                if (this._syncRoot == null)
                {
                    //如果_syncRoot和null相等，将new object赋值给 _syncRoot
                    //Interlocked.CompareExchange方法保证多个线程在使用 syncRoot时是线程安全的
                    Interlocked.CompareExchange(ref this._syncRoot, new object(), null);
                }
                return this._syncRoot;
            }
        }

        public LogWriter(ILogFileFormatting formatter, MMFile mmFile)
        {
            _formatter = formatter;
            _mmFile = mmFile;
        }

        protected abstract string Formatting(string type, string id, TMsg obj);

        private void WriteToFile()
        {
            if (_mmFile.IsWritable()) return;
            var buffer = _mmFile.ReadAll();
            buffer = _formatter.Formatting(buffer);
            LogFileManager.Append(LogFileUtil.GetCopyFilePath(LogType, ((long)(DateTime.UtcNow - FileLog.StandardTime).TotalSeconds).ToString()), buffer, 0, buffer.Length);
        }

        private void Write(byte[] buffer, int offset, int length)
        {
            try
            {
                _mmFile.Write(buffer, offset, length);
            }
            catch (MMFileOverflowException e)
            {
                WriteToFile();
                _mmFile.Reset();
                Write(buffer, offset, length);
            }
            catch (System.Exception e)
            {
                throw new Exception($"{e.Message}");
            }
        }

        public void Write(string type, string id, object obj)
        {
            lock (this.SyncRoot)
            {
                var msg = Formatting(type, id, (TMsg)obj);
                var buffer = System.Text.Encoding.UTF8.GetBytes(msg);
                Write(buffer, 0, buffer.Length);
                buffer = System.Text.Encoding.UTF8.GetBytes("\n");
                Write(buffer, 0, buffer.Length);
            }
        }

        public void Flush()
        {
            lock (this.SyncRoot)
            {
                WriteToFile();
                _mmFile.Reset();
            }
        }

        public void Dispose()
        {
            _mmFile.Dispose();
            _mmFile = null;
        }
    }
}
