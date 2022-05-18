using System;

public abstract class SingletonBase<T> where T : class,new()
{
    protected static T p_Instance;




    #region methods

    #region Public Methods
    public void Destroy()
    {
        OnDestroy();
        p_Instance = null;
    }
    public virtual void OnDestroy() { }

    #endregion

    #endregion
}