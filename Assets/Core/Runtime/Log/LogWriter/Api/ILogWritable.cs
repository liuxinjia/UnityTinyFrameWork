namespace Cr7Sund.Logger
{
    interface ILogWritable
    {
        void Write(string type, string id, object obj);
        void Flush();
    }
}
