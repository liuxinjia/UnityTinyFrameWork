
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
            var result = log.Format(LogLevel.Trace, LogChannel.Undefine, format, args);

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
            var result = log.Format(LogLevel.Trace, LogChannel.Undefine, obj.ToString());

            UnityEngine.Debug.Log(result);
        }

        /// <summary>
        /// 比Debug更低粒度级别，用于开发过程中的调试使用，仅开发中可见
        /// </summary>
        /// <param name="logChannel">格式化类型</param>
        /// <param name="format">格式化字符串</param>
        /// <param name="args">不定长格式化参数</param>
        [Conditional(MacroDefine.UNITY_EDITOR)]
        public static void Trace(LogChannel logChannel, string format, params object[] args)
        {
            var result = log.Format(LogLevel.Trace, logChannel, format, args);

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
            var result = log.Format(LogLevel.Debug, LogChannel.Undefine, format, args);

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
            var result = log.Format(LogLevel.Debug, LogChannel.Undefine, obj.ToString());

            UnityEngine.Debug.Log(result);
        }

        /// <summary>
        /// 细粒度级别，用于开发过程中的调试使用，开发中与发布Debug版可见
        /// </summary>
        /// <param name="logChannel">格式化类型</param>
        /// <param name="format">格式化字符串</param>
        /// <param name="args">不定长格式化参数</param>
        [Conditional(MacroDefine.UNITY_EDITOR)]
#if !PROFILER
        [Conditional(MacroDefine.DEBUG)]
#endif
        public static void Debug(LogChannel logChannel, string format, params object[] args)
        {
            var result = log.Format(LogLevel.Debug, logChannel, format, args);

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
            var result = log.Format(LogLevel.Info, LogChannel.Undefine, format, args);

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
            var result = log.Format(LogLevel.Info, LogChannel.Undefine, obj.ToString());

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
            var result = log.Format(LogLevel.Warn, LogChannel.Undefine, format, args);

            UnityEngine.Debug.LogWarning(result);
        }

        /// <summary>
        /// 粗粒度级别，用于运行过程中表明会出现潜在错误的情形，开发中与发布Debug版可见
        /// </summary>
        /// <param name="obj">格式化字符串</param>
        [Conditional(MacroDefine.UNITY_EDITOR), Conditional(MacroDefine.DEBUG), Conditional(MacroDefine.PROFILER)]
        public static void Warn(object obj)
        {
            var result = log.Format(LogLevel.Warn, LogChannel.Undefine, obj.ToString());

            UnityEngine.Debug.LogWarning(result);
        }

        /// <summary>
        /// 粗粒度级别，用于运行过程中表明会出现潜在错误的情形，开发中与发布Debug版可见
        /// </summary>
        /// <param name="exception">异常</param>
        [Conditional(MacroDefine.UNITY_EDITOR), Conditional(MacroDefine.DEBUG), Conditional(MacroDefine.PROFILER)]
        public static void Warn(System.Exception exception)
        {
            Warn(null, exception);
        }

        /// <summary>
        /// 粗粒度级别，用于运行过程中表明会出现潜在错误的情形，开发中与发布Debug版可见
        /// </summary>
        /// <param name="prefix">格式化字符串前缀</param>
        /// <param name="exception">异常</param>
        [Conditional(MacroDefine.UNITY_EDITOR), Conditional(MacroDefine.DEBUG), Conditional(MacroDefine.PROFILER)]
        public static void Warn(string prefix, System.Exception exception)
        {
            if (null == exception)
            {
                Log.Warn("{0} Exception is null.", prefix);
                return;
            }

            string exceptionStr = ParseException(exception);
            Log.Warn("{0} {1}", prefix, exceptionStr);
        }

        /// <summary>
        /// 粗粒度级别，用于运行过程中表明会出现潜在错误的情形，开发中与发布Debug版可见
        /// </summary>
        /// <param name="logChannel">格式化类型</param>
        /// <param name="format">格式化字符串</param>
        /// <param name="args">不定长格式化参数</param>
        [Conditional(MacroDefine.UNITY_EDITOR), Conditional(MacroDefine.DEBUG), Conditional(MacroDefine.PROFILER)]
        public static void Warn(LogChannel logChannel, string format, params object[] args)
        {
            var result = log.Format(LogLevel.Warn, logChannel, format, args);

            UnityEngine.Debug.LogWarning(result);
        }


        #endregion

        #region Error
        /// <summary>
        /// 粗粒度级别，用于运行过程中虽然发生错误事件,但仍然不影响系统的继续运行，所有版本可见
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <param name="args">不定长格式化参数</param>
        [Conditional(MacroDefine.UNITY_EDITOR), Conditional(MacroDefine.DEBUG), Conditional(MacroDefine.PROFILER), Conditional(MacroDefine.FINAL_RELEASE)]
        public static void Error(string format, params object[] args)
        {
            var result = log.Format(LogLevel.Error, LogChannel.Undefine, format, args);

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
            var result = log.Format(LogLevel.Error, LogChannel.Undefine, obj.ToString());

            UnityEngine.Debug.LogError(result, context);
        }

        /// <summary>
        /// 粗粒度级别，用于运行过程中指出每个严重的错误事件将会导致应用程序的退出，将上报后端保存，所有版本可见
        /// 级别与Error相当
        /// </summary>
        /// <param name="format">格式化字符串</param>
        /// <param name="args">不定长格式化参数</param>
        [Conditional(MacroDefine.UNITY_EDITOR), Conditional(MacroDefine.DEBUG), Conditional(MacroDefine.PROFILER), Conditional(MacroDefine.FINAL_RELEASE)]
        public static void Error(System.Exception exception)
        {
            Error(null, exception);
        }

        private static void Error(string prefix, System.Exception exception)
        {
            if (null == exception)
            {
                Log.Error("{0} Exception is null.", prefix);
                return;
            }

            string exceptionStr = ParseException(exception);
            Log.Error("{0} {1}", prefix, exceptionStr);
        }

        /// <summary>
        /// 粗粒度级别，用于运行过程中虽然发生错误事件,但仍然不影响系统的继续运行，所有版本可见
        /// </summary>
        /// <param name="logChannel">格式化类型</param>
        /// <param name="format">格式化字符串</param>
        /// <param name="args">不定长格式化参数</param>
        [Conditional(MacroDefine.UNITY_EDITOR), Conditional(MacroDefine.DEBUG), Conditional(MacroDefine.PROFILER), Conditional(MacroDefine.FINAL_RELEASE)]
        public static void Error(LogChannel logChannel, string format, params object[] args)
        {
            var result = log.Format(LogLevel.Error, logChannel, format, args);

            UnityEngine.Debug.LogError(result);
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
            var result = log.Format(LogLevel.Fatal, LogChannel.Undefine, format, args);

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
            var result = log.Format(LogLevel.Fatal, LogChannel.Undefine, obj.ToString());

            UnityEngine.Debug.LogError(result);
        }

        /// <summary>
        /// 粗粒度级别，用于运行过程中指出每个严重的错误事件将会导致应用程序的退出，将上报后端保存，所有版本可见
        /// </summary>
        /// <param name="prefix">格式化字符串</param>
        /// <param name="exception">不定长格式化参数</param>
        [Conditional(MacroDefine.UNITY_EDITOR), Conditional(MacroDefine.DEBUG), Conditional(MacroDefine.PROFILER), Conditional(MacroDefine.FINAL_RELEASE)]
        public static void Fatal(string prefix, Exception exception)
        {
            if (null == exception)
            {
                Log.Fatal("{0} Exception is null.", prefix);
                return;
            }
            string exceptionStr = ParseException(exception);
            UnityEngine.Debug.LogError(exceptionStr);
        }

        /// <summary>
        /// 粗粒度级别，用于运行过程中指出每个严重的错误事件将会导致应用程序的退出，将上报后端保存，所有版本可见
        /// </summary>
        /// <param name="logChannel">格式化类型</param>
        /// <param name="format">格式化字符串</param>
        /// <param name="args">不定长格式化参数</param>
        [Conditional(MacroDefine.UNITY_EDITOR), Conditional(MacroDefine.DEBUG), Conditional(MacroDefine.PROFILER), Conditional(MacroDefine.FINAL_RELEASE)]
        public static void Fatal(LogChannel logChannel, string format, params object[] args)
        {
            var result = log.Format(LogLevel.Fatal, logChannel, format, args);

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
