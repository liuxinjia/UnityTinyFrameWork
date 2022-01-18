using CatLib.Container;
using UnityEngine;

namespace Demo
{
    public class FileSystem : IFileSystem
    {
        [Inject]public IDemoDebug demoDebug{get;set;}
        
        public FileSystem(IDemoDebug _demoDebug)
        {
            _demoDebug.Log("_construct FileSystem");
            // error:    demoDebug.LogError("Construct FileSystem");
        }
        public int a;
        public void SetData(int _a)
        {
            a = _a;
        }

        public void PrintData()
        {
            Debug.Log("Test: " + this.a);
        }
    }
}