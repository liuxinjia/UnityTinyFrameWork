using System;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace Cr7Sund.Logger
{
    struct MMFileHeader
    {
        public int position;
        public int length;

        public readonly static int PositionIndex = 0;
        public readonly static int LengthIndex = PositionIndex + sizeof(int);
        public readonly static int FileIndex = LengthIndex + sizeof(int);

        public readonly static int ContextIndex = FileIndex;
    }

    class MMFile : IDisposable
    {
        private readonly MemoryMappedFile _mmf;
        private readonly MemoryMappedViewAccessor _accessor;

        public MMFile(string path, int capacity = 2048)
        {
            try
            {
                if (!File.Exists(path))
                {
                    using var fileStream = File.Create(path);
                    fileStream.SetLength(capacity);
                    _mmf = MemoryMappedFile.CreateFromFile(fileStream, null, capacity, MemoryMappedFileAccess.ReadWrite, HandleInheritability.None, true);
                }
                else
                {
                    using var fileStream = File.Open(path, FileMode.Open, FileAccess.ReadWrite);
                    fileStream.SetLength(capacity);
                    _mmf = MemoryMappedFile.CreateFromFile(fileStream, null, capacity, MemoryMappedFileAccess.ReadWrite, HandleInheritability.None, true);
                }
                _accessor = _mmf.CreateViewAccessor(0, capacity, MemoryMappedFileAccess.ReadWrite);
                var header = ReadHeader();
                header.length = capacity;
                WriteHeader(header);
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.Log(e);
            }
        }

        private MMFileHeader ReadHeader()
        {
            if (_accessor == null)
                throw new Exception("The accessor has not been initialized!!!");

            var header = new MMFileHeader
            {
                position = _accessor.ReadInt32(0),
                length = _accessor.ReadInt32(4)
            };
            return header;
        }

        private void WriteHeader(MMFileHeader header)
        {
            if (_accessor == null)
                throw new Exception("The accessor has not been initialized!!!");

            _accessor.Write(MMFileHeader.PositionIndex, header.position);
            _accessor.Write(MMFileHeader.LengthIndex, header.length);
        }

        public bool IsWritable()
        {
            var header = ReadHeader();
            return header.position <= 0;
        }
        
        public byte[] ReadAll()
        {
            if (_accessor == null)
                throw new Exception("The accessor has not been initialized!!!");

            var header = ReadHeader();
            var result = new byte[header.position];
            _accessor.ReadArray(MMFileHeader.ContextIndex, result, 0, header.position);
            return result;
        }

        public void Write(byte[] sources, int offset, int length)
        {
            if (_accessor == null)
                throw new System.Exception("The accessor has not been initialized");

            if (!_accessor.CanWrite)
                throw new System.Exception("The accessor is write-protected");

            var header = ReadHeader();
            var absolutePosition = header.position + MMFileHeader.ContextIndex;

            if (absolutePosition + length > header.length)
            {
                var overflow = absolutePosition + length - header.length;
                throw new MMFileOverflowException(overflow, "Write overflow!");
            }

            _accessor.WriteArray(absolutePosition, sources, offset, length);
            header.position += length;
            WriteHeader(header);
        }

        public void Reset()
        {
            var header = ReadHeader();
            header.position = 0;
            WriteHeader(header);
        }

        public void Dispose()
        {
            _accessor?.Dispose();
            _mmf?.Dispose();
        }
    }
}




