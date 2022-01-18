using CatLib;
using UnityEngine;
using CatLib.Container;

namespace Demo
{
    public class ProviderDebug : IServiceProvider
    {
        public void Init()
        {
            App.Make<IFileSystem>().SetData(2);

        }

        public void Register()
        {
            App.Singleton<IDemoDebug, DemoDebug>().OnResolving<DemoDebug>((demoDebug) => { });
        }
    }
}