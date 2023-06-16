using System;

namespace Cr7Sund.Async
{
    /// <summary>
    /// 异步扩展方法类！
    /// </summary>
    public static class AsyncEx
    {
        /// <summary>
        /// 将Async加入到AsyncGroup 方便写代码！放心使用不用担心空指针
        /// </summary>
        /// <param name="async"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public static T Join<T>(this T async, IAsyncGroup group) where T : IAsync
        {
            if (async != null && group != null)
            {
                group.AddAsync(async);
            }
            return async;
        }

        /// <summary>
        /// 执行成功且没有发生任何异常
        /// </summary>
        /// <typeparam name="TAsync"></typeparam>
        /// <param name="async"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IAsync Then(this IAsync async, Action<IAsync> action)
        {
            if (action == null) return async;
            if (async == null || async.IsCompleted)
            {
                action.Invoke(async);
                return async;
            }
            async.On(a =>
            {
                if (a.Exception == null)
                {
                    action.Invoke(async);
                }
            });
            return async;
        }

        /// <summary>
        /// 异步过程中产生了所关注的异常
        /// 如果关注所有异常TException 设置为 Exception
        /// </summary>
        /// <typeparam name="TAsync"></typeparam>
        /// <typeparam name="TException"></typeparam>
        /// <param name="async"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static TAsync Catch<TAsync, TException>(this TAsync async, Action<TException> action) where TAsync : IAsync where TException : Exception
        {
            if (async == null || action == null) return async;
            async.On(a =>
            {

                TException exc = a.Exception as TException;
                if (exc != null)
                {
                    action.Invoke(exc);
                }
            });
            return async;
        }

        //public static TAsync Result<TAsync, TResult>(this TAsync async, Action<TResult> setter) where TAsync : IAsync, IAsyncResultable<TResult>
        //{
        //    if (async == null || setter == null) return async;
        //    async.On(a =>
        //    {
        //        if (a.Exception == null)
        //        {
        //            setter.Invoke(async.GetResult());
        //        }
        //    });
        //    return async;
        //}
    }
}