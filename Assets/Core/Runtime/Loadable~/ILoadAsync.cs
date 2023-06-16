
using Cr7Sund.Async;

namespace Cr7Sund.Loadable
{
    public interface ILoadAsync
    {
        IAsync LoadAsync();
        IAsync UnloadAsync();
    }
}