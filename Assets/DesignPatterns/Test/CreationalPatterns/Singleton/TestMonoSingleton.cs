using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.PerformanceTesting;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;

public class TestMonoSingleton
{

    #region NonMono-Create

    /// <summary>
    /// Test craeate mono instance
    /// </summary>
    [Test]
    public void TestCreateMonoSingleton()
    {
        Assert.IsNotNull(AMonoSingleton.Instance);
    }

    /// <summary>
    /// Test craeate mono instance
    /// </summary>
    [Test]
    public void TestCreateSingletonUnqieu()
    {
        Assert.AreNotEqual(AMonoSingleton.Instance.GetType(), BMonoSingleton.Instance.GetType());
        Assert.AreNotSame(AMonoSingleton.Instance, BMonoSingleton.Instance);
    }

    [SetUp]
    public void SetUp()
    {
        Assert.IsNotNull(BoostrapService.SingletonRootGO);
        BoostrapService.m_BoostrapService.GetAllBootAttributeTypes();
    }

    /// <summary>
    /// Test craeate mono instance
    /// </summary>
    [Test, Performance]
    public void TestMonoExecuteOrder()
    {
        Measure.Method(() =>
        {
            BoostrapService.m_BoostrapService.GetAllBootAttributeTypes();
        })
        .WarmupCount(20)
        .MeasurementCount(20)
        .IterationsPerMeasurement(50)
        .CleanUp(() => ASingletonSystem.Instance.Destroy())
        .GC()
        .Run();
    }

    #endregion

}
