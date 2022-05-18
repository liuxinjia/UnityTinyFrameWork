using System;
using UnityEngine;

[SingletonBootMode(SingletonBootModeType.GAMESTART)]
public class ASingletonSystem : Singleton<ASingletonSystem>
{
    public string Name;
    public void Test()
    {
        Debug.Log(nameof(ASingletonSystem));
    }


}