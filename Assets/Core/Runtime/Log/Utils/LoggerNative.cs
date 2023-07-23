using System;
using System.Runtime.InteropServices;
using System.Text;
using ZstdNet;

namespace Cr7Sund.Logger
{
    /// <summary>
    /// zstd
    /// </summary>
    partial class LoggerNative
    {

        #region zstd


        public static byte[] ZstdCompress(byte[] source)
        {
            if (source.Length <= 0) return source;

            using var compressor = new Compressor();
            try
            {
                byte[] compressData = compressor.Wrap(source);
                return compressData;
            }
            catch (Exception e)
            {
                // can not replace with error message since it will be running at release  version
                throw new Exception($"Could not compress the log file:{e}");
            }
        }

        public static byte[] ZstdDecompress(byte[] source)
        {
            if (source.Length <= 0) return source;

            using var compressor = new Decompressor();
            try
            {
                byte[] decompressedData = compressor.Unwrap(source);
                return decompressedData;
            }
            catch (Exception e)
            {
                // can not replace with error message since it will be running at release  version
                throw new Exception($"Could not compress the log file:{e}");
            }
        }

        #endregion

    }
}
