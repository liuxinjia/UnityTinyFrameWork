namespace Cr7Sund.Async
{
    /// <summary>
    /// 可被取消的异步操作
    /// </summary>
    public interface IAsyncCancelable
    {
        void CancelAsync();
    }
}