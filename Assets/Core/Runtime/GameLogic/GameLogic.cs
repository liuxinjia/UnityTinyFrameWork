namespace Cr7Sund.Logger
{
    using UnityEngine;

    public class GameLogic : MonoBehaviour
    {
        private void Awake()
        {
            Log.Initialize();
        }

        private  void OnApplicationQuit()
        {
            Log.Dispose();
        }
    }
}