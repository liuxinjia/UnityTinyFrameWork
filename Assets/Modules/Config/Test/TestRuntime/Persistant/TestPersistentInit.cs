using UnityEngine.TestTools;

public class TestPersistentInit : IPrebuildSetup
{
    public static string PersistantName = "PersistentTest";
    public static int Length => intLength + floatLength + stringLength;

    public static int shortLength = 3; //16bit - 2 byte
    public static int intLength = 3; // 32-bit - 4 byte
    public static int floatLength = 3;//    4-byte
    public static int doubleLength = 3;//    8-byte
    public static int stringLength = 3;
    public static int boolLength = 3;
    public static int charLength = 3; // 16bit - 2bytes

    public void Setup()
    {
       
    }
}