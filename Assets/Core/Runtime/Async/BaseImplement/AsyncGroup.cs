using System;
using System.Collections.Generic;

namespace Cr7Sund.Async
{
    public class AsyncGroup<TAwaiter> : AsyncAwaitable<TAwaiter>, IAsyncGroup, IAsyncCancelable, IDisposable, IAsyncListener where TAwaiter : IAwaiter, new()
    {
        protected HashSet<IAsync> elements;
        /// <summary>
        /// 是否不包含任务子异步，一般地如果为空则异步组就没什么价值
        /// </summary>
        public bool Empty { get; private set; } = true;
        private bool bEnded = false;
        /// <summary>
        /// 执行中的任务子异步数量，用于计算Progress
        /// </summary>
        private int asyncCount = 0;
        public override float Progress
        {
            get
            {
                if (IsCompleted || elements == null || asyncCount <= 0)
                    return base.Progress;
                float progress = 0;
                int count = elements.Count;
                foreach (var it in elements)
                {
                    progress += it.Progress;
                }
                if (asyncCount >= count)
                {
                    return (progress + asyncCount - count) / asyncCount;
                }
                return progress / count;
            }
            protected set => base.Progress = value;
        }

        public void AddAsync(IAsync ele)
        {
            if (ele.IsCompleted)
            {
                return;
            }
            Empty = false;
            if (elements == null)
            {
                elements = new HashSet<IAsync>();
            }
            if (elements.Add(ele))
            {
                ele.On(this, 100);
                asyncCount++;
            }
        }

        public void RemoveAsync(IAsync ele)
        {
            if (elements == null) return;
            if (elements.Remove(ele))
            {
                ele.RemoveOn(this);
                asyncCount--;
            }
        }

        public void OnAsyncCompleted(IAsync aAsync)
        {
            if (aAsync.Exception != null)
            {
                if (Exception == null)
                {
                    Exception = new AsyncGroupException();
                }
                (Exception as AsyncGroupException).AddException(aAsync.Exception);
            }
            elements.Remove(aAsync);
            Check();
        }

        /// <summary>
        /// 避免在添加过程中出现异步已经完成的情况！
        /// 需要确定一个时机表示AsyncGroup已经添加完子元素
        /// </summary>
        public void End()
        {
            if (bEnded) return;
            bEnded = true;
            Check();
        }

        private void Check()
        {
            if (!bEnded) return;
            if (elements == null || elements.Count == 0)
            {
                asyncCount = 0;
                Completed();
            }
        }

        public void CancelAsync()
        {
            if (elements != null)
            {
                foreach (var element in elements)
                {
                    if (element != null && element is IAsyncCancelable cancelAsync)
                    {
                        cancelAsync.CancelAsync();
                    }
                    else
                    {
                        element?.RemoveAll();
                    }
                }
                elements.Clear();
            }
            RemoveAll();
        }

        protected override void OnRemoveAll()
        {
            if (elements != null)
            {
                foreach (var element in elements)
                {
                    element?.RemoveAll();
                }
                elements.Clear();
            }
        }

        public void Dispose()
        {
            if (Empty && !IsCompleted)
            {
                Completed();
            }
        }
    }

    public class AsyncGroup : AsyncGroup<Awaiter> { }
}