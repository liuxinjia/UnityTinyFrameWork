using System;

namespace Cr7Sund.Async
{
    /// <summary>
    /// 异步完成回调接口
    /// 一般地接口比 回调函数更高效！但不易于使用
    /// </summary>
    public interface IAsyncListener
    {
        void OnAsyncCompleted(IAsync aAsync);
    }

    /// <summary>
    /// 异步完成回调方法
    /// </summary>
    /// <param name="aAsync"></param>
    public delegate void OnAsyncCompleted(IAsync aAsync);

    /// <summary>
    /// 异步核心接口
    /// </summary>
    public interface IAsync
    {
        /// <summary>
        /// 是否完成
        /// </summary>
        bool IsCompleted { get; }
        /// <summary>
        /// 进度
        /// </summary>
        float Progress { get; }
        /// <summary>
        /// 用户自定义对象
        /// </summary>
        object State { get; set; }
        /// <summary>
        /// 异常（如果发生）
        /// </summary>
        Exception Exception { get; }

        /// <summary>
        /// 异步完成的监听,
        /// 无论是否发生异常都会回调回来,
        /// 此版本回调会无序执行
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IAsync On(OnAsyncCompleted action);

        /// <summary>
        /// 异步完成的监听,
        /// 无论是否发生异常都会回调回来，
        /// listener会按order排序,数值越小越先执行, 小于0会先于回调函数版本执行
        /// </summary>
        /// <param name="listener"></param>
        /// <para name ="order"></para>
        /// <returns></returns>
        IAsync On(IAsyncListener listener, int order = 0);

        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        IAsync RemoveOn(OnAsyncCompleted action);

        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="listener"></param>
        /// <returns></returns>
        IAsync RemoveOn(IAsyncListener listener);
        /// <summary>
        /// 移除所有监听
        /// </summary>
        /// <returns></returns>
        IAsync RemoveAll();
    }
}