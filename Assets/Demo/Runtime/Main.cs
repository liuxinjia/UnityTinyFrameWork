/*
 * This file is part of the CatLib package.
 *
 * (c) CatLib <support@catlib.io>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 *
 * Document: http://catlib.io/
 */

using CatLib;
using CatLib.Util;
using UnityEngine;

namespace Demo
{
    /// <summary>
    /// Main project entrance.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Main : Framework
    {
        /// <inheritdoc />
        protected override void OnStartCompleted(IApplication application, StartCompletedEventArgs args)
        {
            // Application entry, Your code starts writing here
            // called this function after, use App.Make function to get service
            // ex: App.Make<IYourService>().Debug("hello world");

            App.OnNewApplication += (app) => { Debug.Log("new Application"); };

            Debug.Log("Hello CatLib, Debug Level: " + App.Make<DebugLevel>());
            App.Watch<DebugLevel>(newLevel =>
            {
                Debug.Log("Change debug level: " + newLevel);
            });

            App.OnResolving<IFileSystem>((fileSystem) => {Debug.Log("OnResolving");});
            App.OnResolving((instance) => { Debug.Log("OnResolving Gloablly " + instance.GetType()); });

            App.DebugLevel = DebugLevel.Staging;
            var fileSystem1 = App.Make<IFileSystem>();
            fileSystem1.SetData(1);

            // App.Unbind<IFileSystem>();
            // App.Release<IFileSystem>();
            fileSystem1.PrintData();
            // App.Bind<IFileSystem, FileSystem>();
            var fileSystem2 = App.Make<IFileSystem>();
            fileSystem2.SetData(2);
            fileSystem2.PrintData();

            App.Make<IDemoDebug>().Log("Hello");

        }

        /// <inheritdoc />
        protected override IBootstrap[] GetBootstraps()
        {
            return Arr.Merge(base.GetBootstraps(), Bootstraps.GetBoostraps(this));
        }
    }
}
