using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class BoostrapService : MonoBehaviour
{
    public static GameObject SingletonRootGO
    {
        get
        {
            if (p_SingletonRootGO == null)
            {
                var curScene = SceneManager.GetActiveScene();
                p_SingletonRootGO = new GameObject();
                m_BoostrapService = p_SingletonRootGO.AddComponent<BoostrapService>();
                // Make instance persistent.
                DontDestroyOnLoad(p_SingletonRootGO);

            }
            return p_SingletonRootGO;
        }
    }
    public static GameObject p_SingletonRootGO;
    public static BoostrapService m_BoostrapService;
    public static int Order = 1;
    // public static double Cost = 1d;
    // public static double PrevCost = -1d;

    private void Awake()
    {
        GetAllBootAttributeTypes();

    }

    public void GetAllBootAttributeTypes()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies(); // Replace Assembly configs
        var configAssembly = new List<Assembly>();

        var singletonBootTypes = new List<Type>();
        foreach (var assembly in assemblies)
        {
            var classTypes = assembly.GetTypes().Where(t => t.IsPublic &&
            typeof(SingletonBehaviourBase).IsAssignableFrom(t)
            && t.IsDefined(typeof(SingletonBootModeAttribute)));

            var collection = classTypes.ToArray();
            if (collection.Length > 0)
            {
                configAssembly.Add(assembly);
                singletonBootTypes.AddRange(collection);
            }
        }


    }


    private void OnEnable()
    {
    }

    private void Start()
    {
    }

    private void Update()
    {
        // PrevCost = Cost;
        // Cost = 1d;
    }

    private void OnDisable()
    {
    }

    private void Destroy()
    {
    }
}