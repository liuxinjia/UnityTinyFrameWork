using UnityEngine;
using UnityEngine.Windows;

namespace Cr7Sund.Editor
{
    public sealed partial class EditorUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public static string GetAssetFolderAbsPath(string path)
        {
            string folderName = string.Empty;
            CreateDirectory_Recursive(Application.dataPath, path);

            path = $"{Application.dataPath}/{path}";
            return path;
        }

        public static string GetProjectFolderAbsPath(string path)
        {

            // Application.dataPath return <path to project folder>/Assets
            var projectPath = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);
            CreateDirectory_Recursive(projectPath, path);

            path = $"{projectPath}/{path}";
            return path;
        }

        private static void CreateDirectory_Recursive(string path, string folderName)
        {
            string[] args = folderName.Split(new[] { '/' });

            for (int i = 0; i < args.Length - 1; i++) // exclude the last file name
            {
                path = $"{path}/{args[i]}";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
        }

        public static string GetSuffix(string path)
        {
            string ext = System.IO.Path.GetExtension(path);
            string[] args = ext.Split(new char[] { '.' });
            return args[args.Length - 1];
        }



    }
}