using UnityEngine;

namespace Demo
{
    public class DemoDebug : IDemoDebug
    {
        public void Log(string logInfo)
        {
           Debug.Log(logInfo);
        }

        public void LogError(string logInfo)
        {
            Debug.LogError($"<color=#800000ff>{logInfo}</color>");
        }
    }
}