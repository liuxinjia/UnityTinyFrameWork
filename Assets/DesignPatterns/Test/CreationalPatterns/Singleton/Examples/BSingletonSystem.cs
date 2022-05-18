using UnityEngine;

[SingletonBootMode(SingletonBootModeType.SCENESCOPE)]
public class BSingletonSystem : SingletonBase<BSingletonSystem>
{
    public void Test()
    {
        Debug.Log("Test");
    }

    public static BSingletonSystem Instance
    {
        get
        {
            if (p_Instance == null)
            {
                p_Instance = new BSingletonSystem();
            }
            return p_Instance;
        }
    }
    
}