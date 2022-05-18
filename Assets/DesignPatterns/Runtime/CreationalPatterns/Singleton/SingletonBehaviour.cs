using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonBehaviour<T> : SingletonBehaviourBase where T : SingletonBehaviourBase
{
    protected static T p_Instance;

    public static T Instance
    {
        get
        {
            if (p_Instance == null)
            {
                // gameobject.
                p_Instance = BoostrapService.SingletonRootGO.AddComponent<T>();
                p_Instance.Order = BoostrapService.Order++;
            }
            return p_Instance;
        }
    }


}
