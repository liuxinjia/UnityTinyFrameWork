
namespace Cr7Sund.Async
{
    /// <summary>
    /// 异步结果接口
    /// </summary>
    public interface IResultAsync<TResult> : IAsync, IAsyncResultable<TResult>
    {
        /// <summary>
        /// 获取结果
        /// </summary>
        TResult Result { get; }
    }
}