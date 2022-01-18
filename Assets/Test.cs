using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        TestClass();
        TestStruct();
    }

    private void TestStruct()
    {
        Profiler.BeginSample("TestStruct");
        for (int i = 0; i < 10000; i++) { var a = new MyStruct(); }
        Profiler.EndSample();
    }

    private void TestClass()
    {
        Profiler.BeginSample("TestClass");
        for (int i = 0; i < 10000; i++)
        { var a = new MyClass(); }
        Profiler.EndSample();
    }

    public class MyClass
    {
        int a;
    }

    public struct MyStruct
    {
        int a;
    }
}
