using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Cr7Sund.Async
{

    public static class AsyncMonitor
    {
        // 定义MonitorElement类
        public class MonitorElement
        {
            // 定义栈展示信息
            public string StackTrace;
            // 定义异步对象
            public Async Async;
            // 定义状态信息
            public string State;
            // 定义MonitorElement构造函数
            public MonitorElement(Async a)
            {
                // 将异步对象赋值给Async
                Async = a;
                // 将栈展示信息赋值给StackTrace
                StackTrace = new StackTrace(true).ToString();
                State = a.State == null ? "null" : a.State.GetType().ToString();
                // 如果异步对象的状态为null，则赋值为“null”，否则赋值为异步对象的状态的类型
                State = a.State == null ? "null" : a.State.GetType().ToString();



            }
        }
        // 定义异步列表
        public static List<MonitorElement> asyncList = new List<MonitorElement>();
        // 定义注册异步函数
        public static void Register(Async async)
        {
            // 将异步对象添加到异步列表中
            asyncList.Add(new MonitorElement(async));
        }

        // 定义取消注册异步函数
        public static void UnRegister(Async async)
        {
            // 查找Async对应的元素
            var element = asyncList.Find(ele => ele.Async == async);
            // 删除元素
            asyncList.Remove(element);
        }
    }


    public abstract class Async : IAsync
    {
        private AsyncListenerOperation operation;
        public object State { get; set; }
        public Exception Exception { get; set; }
        public bool IsCompleted { get; protected set; }

        public virtual float Progress { get; protected set; }

        public Async()
        {
            //if (MacroDefine.IsEditor)
            //    AsyncMonitor.Register(this);
            Progress = 0;
            operation = new AsyncListenerOperation();
        }

        public virtual void Completed()
        {
            if (IsCompleted)
            {
                Log.Warn("Connot Completed Again!!");
                return;
            }

            //if (MacroDefine.IsEditor)
            //    AsyncMonitor.UnRegister(this);
            IsCompleted = true;
            Progress = 1.0f;
            operation.OnCompleted(this);
        }

        public IAsync On(OnAsyncCompleted action)
        {
            if (action != null)
            {
                if (IsCompleted)
                {
                    action.Invoke(this);
                }
                else
                {
                    operation.On(action);
                }
            }
            return this;
        }

        public IAsync On(IAsyncListener listener, int order = 0)
        {
            if (listener == null) return this;
            if (IsCompleted)
            {
                listener.OnAsyncCompleted(this);
                return this;
            }
            operation.On(listener, order);
            return this;
        }

        public IAsync RemoveOn(OnAsyncCompleted action)
        {
            operation.RemoveOn(action);
            return this;
        }

        public IAsync RemoveOn(IAsyncListener listener)
        {
            operation.RemoveOn(listener);
            return this;
        }

        public IAsync RemoveAll()
        {
            OnRemoveAll();
            operation.Clear();
            return this;
        }

        protected virtual void OnRemoveAll()
        {

        }
    }
}