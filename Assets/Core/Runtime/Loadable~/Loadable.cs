using UnityEngine.Assertions;

namespace Cr7Sund.Loadable
{
    public class Loadable : AsyncLoadable, ILoadable
    {
        public void LoadSync()
        {
            Assert.IsFalse(State != LoadState.None, $"Connot LoadSync On State {State}  Loadable: {this} ");

            OnLoadSync();
            State = LoadState.Loaded;
            OnLoaded();
        }

        public void UnloadSync()
        {
            Assert.IsFalse(State != LoadState.Loaded, $"Connot UnloadSync On State {State}  Loadable: {this} ");

            OnUnloadSync();
            State = LoadState.Unloaded;
            OnUnloaded();
        }

        protected virtual void OnLoadSync() { }
        protected virtual void OnUnloadSync() { }
    }
}