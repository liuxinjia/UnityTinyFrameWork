
using Cr7Sund.Async;
using Cr7Sund.Logger;
using UnityEngine.Assertions;

namespace Cr7Sund.Loadable
{
    /// <summary>
    /// 异步加载器
    /// </summary>
    public class AsyncLoadable : ILoadAsync
    {
        public LoadState State { get; protected set; }
        private AsyncGroup loadGroup;
        private AsyncGroup unloadGroup;

        public IAsync LoadStatus => loadGroup;

        public IAsync UnloadStatus => unloadGroup;

        public IAsync LoadAsync()
        {
            Assert.IsTrue(State == LoadState.None || State == LoadState.Unloaded, $"Connot LoadAsync On State {State} Loadable: {this} ");

            State = LoadState.Loading;

            loadGroup = new AsyncGroup();

            try
            {
                OnLoadAsync(loadGroup);
            }
            catch (System.Exception e)
            {
                Log.Fatal($"AsyncLoadable.OnLoadAsync Error: \n" +
                    $"Message: {e.Message},\n" +
                    $"StackTrace: {e.StackTrace}");
            }

            if (loadGroup != null)
            {
                loadGroup.On(_Loaded);
                loadGroup.End();
            }

            return loadGroup;
        }

        public IAsync UnloadAsync()
        {
            Assert.IsFalse(State != LoadState.Loading && State != LoadState.Loaded, $"Connot UnloadAsync On State {State}  Loadable: {this} ");

            if (State == LoadState.Loading)
            {
                loadGroup?.CancelAsync();
            }
            State = LoadState.Unloading;

            unloadGroup = new AsyncGroup();

            try
            {
                OnUnloadAsync(unloadGroup);
            }
            catch (System.Exception e)
            {
                Log.Fatal($"AsyncLoadable.OnUnloadAsync Error: \n" +
                        $"Message: {e.Message},\n" +
                        $"StackTrace: {e.StackTrace}");
            }

            if (unloadGroup != null)
            {
                unloadGroup.On(_Unloaded);
                unloadGroup.End();
            }

            return unloadGroup;
        }

        private void _Loaded(IAsync async = null)
        {
            State = LoadState.Loaded;
            try
            {
                OnLoaded();
            }
            catch (System.Exception e)
            {
                Log.Fatal($"AsyncLoadable.OnLoaded Error: \n" +
                  $"Message: {e.Message},\n" +
                  $"StackTrace: {e.StackTrace}");
            }

        }

        private void _Unloaded(IAsync async = null)
        {
            State = LoadState.Unloaded;
            try
            {
                OnUnloaded();
            }
            catch (System.Exception e)
            {
                Log.Fatal($"AsyncLoadable.OnUnloaded Error: \n" +
                   $"Message: {e.Message},\n" +
                   $"StackTrace: {e.StackTrace}");
            }

        }

        protected virtual void OnLoaded() { }
        protected virtual void OnUnloaded() { }
        protected virtual void OnLoadAsync(IAsyncGroup group) { }
        protected virtual void OnUnloadAsync(IAsyncGroup group) { }
    }
}
