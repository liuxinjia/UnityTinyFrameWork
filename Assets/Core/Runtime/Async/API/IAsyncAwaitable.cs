namespace Cr7Sund.Async
{
    /// <summary>
    /// 可被等待异步，实现此接口可使用C# async await 语法实现异步编程
    /// </summary>
    /// <typeparam name="TAwaitable"></typeparam>
    public interface IAsyncAwaitable<TAwaiter> where TAwaiter : IAwaiter
    {
        TAwaiter GetAwaiter();
    }
}