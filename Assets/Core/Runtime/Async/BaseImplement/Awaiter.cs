using System;

namespace Cr7Sund.Async
{
    public class Awaiter : IAwaiter
    {
        // 定义一个 Action 类型的变量，用于存储 await 后面的代码
        protected Action _continuation;

        // 定义一个 IsCompleted 属性，用于指示 await 开始时任务已经执行完成
        public bool IsCompleted
        {
            get; protected set;
        }

        // 定义一个 OnCompleted 方法，用于接收 await 后面的代码
        public void OnCompleted(Action continuation)
        {
            if (IsCompleted)
            {
                // 如果 await 开始时任务已经执行完成，则直接执行 await 后面的代码。
                // 注意，即便 _continuation 有值，也无需关心，因为报告结束的时候就会将其执行。
                continuation?.Invoke();
            }
            else
            {
                // 当使用多个 await 关键字等待此同一个 awaitable 实例时，此 OnCompleted 方法会被多次执行。
                // 当任务真正结束后，需要将这些所有的 await 后面的代码都执行。
                _continuation += continuation;
            }
        }
        // 定义一个 Completed 方法，用于接收 awaitable 实例
        public virtual void Completed(IAsync async)
        {
            // 如果 await 开始时任务已经执行完成，则把 await 后面的代码都执行
            if (IsCompleted) return;
            IsCompleted = true;
            _continuation?.Invoke();
        }

        // 定义一个 GetResult 方法，用于获取 awaitable 实例的结果
        public void GetResult() { }
    }
}