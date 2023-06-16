using System;
using System.Collections.Generic;

namespace Cr7Sund.Async
{
    /// <summary>
    /// 异步的回调监听操作
    /// </summary>
    public struct AsyncListenerOperation
    {
        private struct ListenerKV : IComparable<ListenerKV>
        {
            public IAsyncListener Listener;
            public int Order;

            public ListenerKV(IAsyncListener l, int o)
            {
                Listener = l;
                Order = o;
            }

            public int CompareTo(ListenerKV other)
            {
                return Order - other.Order;
            }
        }
        private OnAsyncCompleted completedAction;
        private List<ListenerKV> completedList;
        private bool bNeedSort;
        /// <summary>
        /// 是否有监听事件
        /// </summary>
        public bool HaveListener => completedAction != null || (completedList != null && completedList.Count > 0);
        public void On(OnAsyncCompleted action)
        {
            if (action != null)
            {
                completedAction += action;
            }
        }

        public void On(IAsyncListener listener, int order = 0)
        {
            if (listener == null) return;
            if (completedList == null)
            {
                completedList = new List<ListenerKV>(4);
            }
            completedList.Add(new ListenerKV(listener, order));
            if (order != 0)
            {
                bNeedSort = true;
            }
        }

        public void RemoveOn(OnAsyncCompleted action)
        {
            if (action != null) completedAction -= action;
        }

        public void RemoveOn(IAsyncListener listener)
        {
            for (int i = 0; i < completedList.Count; i++)
            {
                var cur = completedList[i];
                if (cur.Listener == listener)
                {
                    completedList.RemoveAt(i);
                    break;
                }
            }
        }
        public void OnCompleted(IAsync async)
        {
            int excutedIdx = -1;
            if (completedList != null && completedList.Count > 0)
            {
                if (bNeedSort && completedList.Count > 1)
                {
                    completedList.Sort();
                }
                //先执行completedList中order小于0的
                for (excutedIdx = 0; excutedIdx < completedList.Count;)
                {
                    var cur = completedList[excutedIdx];
                    if (cur.Order >= 0) break;
                    cur.Listener.OnAsyncCompleted(async);
                    excutedIdx++;
                }
            }
            //执行回调函数
            completedAction?.Invoke(async);
            completedAction = null;
            //再执行completedList中大于0的部分
            if (excutedIdx >= 0)
            {
                for (; excutedIdx < completedList.Count; excutedIdx++)
                {
                    completedList[excutedIdx].Listener.OnAsyncCompleted(async);
                }
                completedList.Clear();
            }
        }
        public void Clear()
        {
            completedAction = null;
            completedList?.Clear();
        }

    }
}
