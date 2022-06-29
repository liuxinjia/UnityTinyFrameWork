using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cr7Sund.Core
{
    public static class NiceIO
    {
        /// <summary>
        /// windows :  %userprofile%\AppData\LocalLow\<companyname>\<productname>
        /// andropid : /storage/emulated/0/Android/data/<packagename>/files
        /// ios : /var/mobile/Containers/Data/Application/<guid>/Documents
        /// MAC : ~/Library/Application Support/company name/product name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetPersistentDataPath(string name)
        {
            // dont use create directory if support mobilie
            return System.IO.Path.Combine(Application.persistentDataPath, name);
        }
    }
}