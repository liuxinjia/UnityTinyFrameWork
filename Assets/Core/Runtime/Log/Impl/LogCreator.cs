
namespace Cr7Sund.Logger
{
    static class LogCreator
    {
        public static ILog Create()
        {
            return default(ILog); //returns ConsoleLogger for default console output.
        }
    }
}
