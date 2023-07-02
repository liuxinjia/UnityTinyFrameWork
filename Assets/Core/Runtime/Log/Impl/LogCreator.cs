
namespace Cr7Sund.Logger
{
    static class LogCreator
    {
        public static ILog Create()
        {
            return new UnityLog(); //returns ConsoleLogger for default console output.
        }
    }
}
