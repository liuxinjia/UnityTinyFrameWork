using UnityEngine.Assertions;

namespace Cr7Sund
{
    public class Node : AsyncLoadable
    {
        public IAsync Load(Node parentNode)
        {
            Assert.IsTrue(State == LoadState.None);
            return LoadAsync();
        }
    }
}