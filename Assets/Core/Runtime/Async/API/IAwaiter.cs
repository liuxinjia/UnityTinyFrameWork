using System.Runtime.CompilerServices;

namespace Cr7Sund.Async
{
    /// <summary>
    /// 供C# await语法
    /// awaiter状态应该与所属的IAsync保持一致
    /// </summary>
    public interface IAwaiter : INotifyCompletion
    {
        bool IsCompleted { get; }
        void Completed(IAsync result);
    }
}