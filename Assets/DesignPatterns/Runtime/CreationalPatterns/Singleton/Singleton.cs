using System;
public class Singleton<T> : SingletonBase<T> where T : class, new()
{
    public static T Instance
    {
        get
        {
            if (p_Instance == null)
            {
                p_Instance = Activator.CreateInstance<T>();
            }
            return p_Instance;
        }
    }

    public static T NewInstance
    {
        get
        {
            if (p_Instance == null)
            {
                p_Instance = new T();
            }
            return p_Instance;
        }
    }

}