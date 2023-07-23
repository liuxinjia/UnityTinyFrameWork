
namespace Cr7Sund.Logger
{
    static class LogCreator
    {
        public static ILog Create()
        {
            return new RpcLog();

#if UNITY_EDITOR
            return new UnityLog(); //returns ConsoleLogger for default console output.
#elif UNITY_STANDALONE
            return new RpcLog();
#elif FINAL_RELEASE || PROFILER
            return new FileLog();
#else
            return new ConsoleLog();
#endif
        }
    }
}
