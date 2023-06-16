namespace Cr7Sund.Async
{
    /// <summary>
    /// 带有返回结束的异步，可通过GetResult获取结果
    /// 实现此接口的Awaiter 可通过await 获取值
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IAsyncResultable<TResult>
    {
        TResult GetResult();
    }

    /// <summary>
    /// 带有返回结束的异步，可通过GetResult获取结果
    /// 实现此接口的Awaiter 可通过await 获取值
    /// </summary>
    public interface IAsyncResultable
    {
        TResult GetResult<TResult>();
    }
}