using UnityEngine;


[System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class SingletonBootModeAttribute : System.Attribute
{
    // See the attribute guidelines at
    //  http://go.microsoft.com/fwlink/?LinkId=85236
    readonly SingletonBootModeType positionalString;

    // This is a positional argument
    public SingletonBootModeAttribute(SingletonBootModeType bootType)
    {
        this.positionalString = bootType;
        Debug.Log(bootType);
    }

    public SingletonBootModeType PositionalString
    {
        get { return positionalString; }
    }


}