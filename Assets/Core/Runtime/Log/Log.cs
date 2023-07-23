
using System;
using System.Diagnostics;
using Cr7Sund.Logger;

namespace Cr7Sund
{
    public static class Log
    {
        private static ILog _log;
        private static ILog log => _log ??= LogCreator.Create();

        #region utility

        private static string ParseException(System.Exception exception)
        {
            return string.Format("{0}: {1}\nStackTrace: {2}", exception.GetType().FullName, exception.Message, exception.StackTrace);
        }

        #endregion

        #region Trace

        /// <summary>
        /// 比Debug更低粒度级别，用于开发过程中的调试使用，仅开发中可见
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <param name="args">不定长格式化参数</param>
        [Conditional(MacroDefine.UNITY_EDITOR)]
        public static void Trace(string format, params object[] args)
        {
            var result = log.Format(LogLevel.Trace, format, args);

            UnityEngine.Debug.Log(result);
        }

        /// <summary>
        /// 比Debug更低粒度级别，用于开发过程中的调试使用，仅开发中可见
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <param name="args">不定长格式化参数</param>
        [Conditional(MacroDefine.UNITY_EDITOR)]
        public static void Trace(object obj)
        {
            var result = log.Format(LogLevel.Trace, obj.ToString());

            UnityEngine.Debug.Log(result);
        }

        #endregion

        #region Debug

        /// <summary>
        /// 细粒度级别，用于开发过程中的调试使用，开发中与发布Debug版可见
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <param name="args">不定长格式化参数</param>
        [Conditional(MacroDefine.UNITY_EDITOR)]
#if !PROFILER
        [Conditional(MacroDefine.DEBUG)]
#endif
        public static void Debug(string format, params object[] args)
        {
            var result = log.Format(LogLevel.Debug, format, args);

            UnityEngine.Debug.Log(result);
        }

        /// <summary>
        /// 细粒度级别，用于开发过程中的调试使用，开发中与发布Debug版可见
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <param name="args">不定长格式化参数</param>
        [Conditional(MacroDefine.UNITY_EDITOR)]
#if !PROFILER
        [Conditional(MacroDefine.DEBUG)]
#endif
        public static void Debug(object obj)
        {
            var result = log.Format(LogLevel.Debug, obj.ToString());

            UnityEngine.Debug.Log(result);
        }

        #endregion

        #region Info
        /// <summary>
        /// 粗粒度级别，用于运行过程中突出强调应用程序的运行过程(eg:LogSystem init success/fail)，所有版本可见
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <param name="args">不定长格式化参数</param>
        [Conditional(MacroDefine.UNITY_EDITOR), Conditional(MacroDefine.DEBUG), Conditional(MacroDefine.PROFILER), Conditional(MacroDefine.FINAL_RELEASE)]
        public static void Info(string format, params object[] args)
        {
            var result = log.Format(LogLevel.Info, format, args);

            UnityEngine.Debug.Log(result);
        }

        /// <summary>
        /// 粗粒度级别，用于运行过程中突出强调应用程序的运行过程(eg:LogSystem init success/fail)，所有版本可见
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <param name="args">不定长格式化参数</param>
        [Conditional(MacroDefine.UNITY_EDITOR), Conditional(MacroDefine.DEBUG), Conditional(MacroDefine.PROFILER), Conditional(MacroDefine.FINAL_RELEASE)]
        public static void Info(object obj)
        {
            var result = log.Format(LogLevel.Info, obj.ToString());

            UnityEngine.Debug.Log(result);
        }
        #endregion

        #region Warn
        /// <summary>
        /// 粗粒度级别，用于运行过程中表明会出现潜在错误的情形，开发中与发布Debug版可见
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <param name="args">不定长格式化参数</param>
        [Conditional(MacroDefine.UNITY_EDITOR), Conditional(MacroDefine.DEBUG), Conditional(MacroDefine.PROFILER)]
        public static void Warn(string format, params object[] args)
        {
            var result = log.Format(LogLevel.Warn, format, args);

            UnityEngine.Debug.LogWarning(result);
        }

        /// <summary>
        /// 粗粒度级别，用于运行过程中表明会出现潜在错误的情形，开发中与发布Debug版可见
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <param name="args">不定长格式化参数</param>
        [Conditional(MacroDefine.UNITY_EDITOR), Conditional(MacroDefine.DEBUG), Conditional(MacroDefine.PROFILER)]
        public static void Warn(object obj)
        {
            var result = log.Format(LogLevel.Warn, obj.ToString());

            UnityEngine.Debug.LogWarning(result);
        }

        /// <summary>
        /// 粗粒度级别，用于运行过程中表明会出现潜在错误的情形，开发中与发布Debug版可见
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <param name="args">不定长格式化参数</param>
        [Conditional(MacroDefine.UNITY_EDITOR), Conditional(MacroDefine.DEBUG), Conditional(MacroDefine.PROFILER)]
        public static void WarnException(System.Exception exception)
        {
            WarnException(null, exception);
        }

        /// <summary>
        /// 粗粒度级别，用于运行过程中表明会出现潜在错误的情形，开发中与发布Debug版可见
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <param name="args">不定长格式化参数</param>
        [Conditional(MacroDefine.UNITY_EDITOR), Conditional(MacroDefine.DEBUG), Conditional(MacroDefine.PROFILER)]
        public static void WarnException(string prefix, System.Exception exception)
        {
            if (null == exception)
            {
                Log.Warn("{0} Exception is null.", prefix);
                return;
            }

            string exceptionStr = ParseException(exception);
            Log.Warn("{0} {1}", prefix, exceptionStr);
        }

        #endregion

        #region Error
        /// <summary>
        /// 粗粒度级别，用于运行过程中虽然发生错误事件,但仍然不影响系统的继续运行，所有版本可见
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <param name="args">不定长格式化参数</param>
        [Conditional(MacroDefine.UNITY_EDITOR), Conditional(MacroDefine.DEBUG), Conditional(MacroDefine.PROFILER), Conditional(MacroDefine.FINAL_RELEASE)]
        public static void ErrorFormat(string format, params object[] args)
        {
            var result = log.Format(LogLevel.Error, format, args);

            UnityEngine.Debug.LogError(result);
        }

        /// <summary>
        /// 粗粒度级别，用于运行过程中虽然发生错误事件,但仍然不影响系统的继续运行，所有版本可见
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <param name="args">不定长格式化参数</param>
        /// <param name="context">Object to which the message applies.</param>
        [Conditional(MacroDefine.UNITY_EDITOR), Conditional(MacroDefine.DEBUG), Conditional(MacroDefine.PROFILER), Conditional(MacroDefine.FINAL_RELEASE)]
        public static void Error(object obj, UnityEngine.Object context = null)
        {
            var result = log.Format(LogLevel.Error, obj.ToString());

            UnityEngine.Debug.LogError(result, context);
        }

        /// <summary>
        /// 粗粒度级别，用于运行过程中指出每个严重的错误事件将会导致应用程序的退出，将上报后端保存，所有版本可见
        /// 级别与Error相当
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <param name="args">不定长格式化参数</param>
        [Conditional(MacroDefine.UNITY_EDITOR), Conditional(MacroDefine.DEBUG), Conditional(MacroDefine.PROFILER), Conditional(MacroDefine.FINAL_RELEASE)]
        public static void Exception(System.Exception exception)
        {
            ExceptionFormat(null, exception);
        }

        /// <summary>
        /// 粗粒度级别，用于运行过程中指出每个严重的错误事件将会导致应用程序的退出，将上报后端保存，所有版本可见
        /// 级别与Error相当
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <param name="args">不定长格式化参数</param>
        [Conditional(MacroDefine.UNITY_EDITOR), Conditional(MacroDefine.DEBUG), Conditional(MacroDefine.PROFILER), Conditional(MacroDefine.FINAL_RELEASE)]
        public static void ExceptionFormat(string prefix, System.Exception exception)
        {
            if (null == exception)
            {
                Log.ErrorFormat("{0} Exception is null.", prefix);
                return;
            }

            string exceptionStr = ParseException(exception);
            Log.ErrorFormat("{0} {1}", prefix, exceptionStr);
        }

        #endregion

        #region Fatal
        /// <summary>
        /// 粗粒度级别，用于运行过程中指出每个严重的错误事件将会导致应用程序的退出，将上报后端保存，所有版本可见
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <param name="args">不定长格式化参数</param>
        [Conditional(MacroDefine.UNITY_EDITOR), Conditional(MacroDefine.DEBUG), Conditional(MacroDefine.PROFILER), Conditional(MacroDefine.FINAL_RELEASE)]
        public static void Fatal(string format, params object[] args)
        {
            var result = log.Format(LogLevel.Fatal, format, args);

            UnityEngine.Debug.LogError(result);
        }

        /// <summary>
        /// 粗粒度级别，用于运行过程中指出每个严重的错误事件将会导致应用程序的退出，将上报后端保存，所有版本可见
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <param name="args">不定长格式化参数</param>
        [Conditional(MacroDefine.UNITY_EDITOR), Conditional(MacroDefine.DEBUG), Conditional(MacroDefine.PROFILER), Conditional(MacroDefine.FINAL_RELEASE)]
        public static void Fatal(object obj)
        {
            var result = log.Format(LogLevel.Fatal, obj.ToString());

            UnityEngine.Debug.LogError(result);
        }

        /// <summary>
        /// 粗粒度级别，用于运行过程中指出每个严重的错误事件将会导致应用程序的退出，将上报后端保存，所有版本可见
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <param name="args">不定长格式化参数</param>
        [Conditional(MacroDefine.UNITY_EDITOR), Conditional(MacroDefine.DEBUG), Conditional(MacroDefine.PROFILER), Conditional(MacroDefine.FINAL_RELEASE)]
        public static void Fatal(string format, Exception e)
        {
            var result = ($"{format} \n" +
                        $"Message: {e.Message},\n" +
                        $"StackTrace: {e.StackTrace}");
            result = log.Format(LogLevel.Fatal, result);

            UnityEngine.Debug.LogError(result);
        }

        #endregion


        #region 其他


        internal static void Initialize()
        {
            log.Initialize();
        }

        internal static void Dispose()
        {
            (_log as IDisposable)?.Dispose();
            _log = null;
        }


        #endregion
    }
}
