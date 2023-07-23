using System;
using System.IO;
using System.Text;

namespace Cr7Sund.Logger
{
    static class LogFileManager
    {
        public static void Append(string path, byte[] buffer, int offset, int length)
        {
            Create(path);
            try
            {
                using (var steam = File.Open(path, FileMode.Append, FileAccess.Write))
                {
                    steam.Write(buffer, 0, buffer.Length);
                    steam.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception("写入文件失败" + e.Message);
            }
        }

        public static void Create(string path)
        {
            try
            {
                if (!File.Exists(path))
                    File.Create(path).Close();
            }
            catch (Exception e)
            {
                throw new Exception("创建新文件夹失败" + e.Message);
            }
        }

        public static void DeleteFile(string file)
        {
            try
            {
                if (System.IO.File.Exists(file)) System.IO.File.Delete(file);
            }
            catch (Exception e)
            {
                throw new Exception("移除文件失败" + e.Message);
            }
        }

        public static string[] GetFilesInDirector(string director)
        {
            if (!Directory.Exists(director))
                throw new Exception("不存在此类型的文件夹");

            return Directory.GetFiles(director);
        }

        public static void ExistOrCreate(string director)
        {
            if (!System.IO.Directory.Exists(director))
                System.IO.Directory.CreateDirectory(director);
        }
    }
}
