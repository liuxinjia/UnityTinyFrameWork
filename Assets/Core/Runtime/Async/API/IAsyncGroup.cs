namespace Cr7Sund.Async
{
    /// <summary>
    /// 包含一组异步任务
    /// 子异步全部完成时，Group完成
    /// </summary>
    public interface IAsyncGroup : IAsync
    {
        void AddAsync(IAsync ele);
        void RemoveAsync(IAsync ele);
    }
}