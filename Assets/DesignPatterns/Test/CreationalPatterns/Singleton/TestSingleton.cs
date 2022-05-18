using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.PerformanceTesting;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;

public class TestSingleton
{

    #region Create
    // A Test behaves as an ordinary method
    // Also support constructor with params
    [Test]
    public void TestSingleton_Null()
    {
        // Use the Assert class to test conditions
        Assert.IsNotNull(ASingletonSystem.Instance);
        Assert.IsNotNull(BSingletonSystem.Instance);

    }
    /// <summary>
    /// Test create instance with reflection - create instance
    /// </summary>
    [Test, Performance]
    public void TestSingletonCreateMethods_Reflection()
    {
        Measure.Method(() =>
        {
            Assert.IsNotNull(ASingletonSystem.Instance);
        })
        .WarmupCount(20)
        .MeasurementCount(20)
        .IterationsPerMeasurement(50)
        .SampleGroup("CreateSingleton")
        .CleanUp(() => ASingletonSystem.Instance.Destroy())
        .GC()
        .Run();
    }
    /// <summary>
    /// Test create instance with new generic constraint - New T()
    /// </summary>

    [Test, Performance]
    public void TestSingletonCreateMethods_NewT()
    {
        Measure.Method(() =>
        {
            Assert.IsNotNull(ASingletonSystem.NewInstance);
        })
        .WarmupCount(20)
        .MeasurementCount(20)
        .IterationsPerMeasurement(50)
        .CleanUp(() => ASingletonSystem.Instance.Destroy())
        .GC()
        .Run();
    }

    /// <summary>
    /// Test create instance with explictly create specific classs
    /// </summary>
    [Test, Performance]
    public void TestSingletonCreateMethods_DeclareSpecificClass()
    {
        Measure.Method(() =>
        {
            Assert.IsNotNull(BSingletonSystem.Instance);
        })
        .WarmupCount(20)
        .MeasurementCount(20)
        .IterationsPerMeasurement(50)
        .CleanUp(() => ASingletonSystem.Instance.Destroy())
        .GC()
        .Run();
    }


    #endregion




    #region Singleton Two qualifications
    [Test]
    public void Test_Unique()
    {
        string firstName = "WestBrook";
        string secondName = "Durant";
        ASingletonSystem.Instance.Name = firstName;
        ASingletonSystem.Instance.Name = secondName;

        Assert.AreEqual(secondName, ASingletonSystem.Instance.Name);
        Assert.AreNotSame(ASingletonSystem.Instance, BSingletonSystem.Instance);
    }
    #endregion
}
