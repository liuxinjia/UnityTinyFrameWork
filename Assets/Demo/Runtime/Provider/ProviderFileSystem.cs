using CatLib;

namespace Demo
{
    public class ProviderFileSystem : IServiceProvider
    {
        public void Init()
        {
        }

        public void Register()
        {
            App.Bind<IFileSystem, FileSystem>().OnResolving((fileSystem, t) => { });
        }
    }
}