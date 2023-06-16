using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Cr7Sund.Async;

public class TestCr7SundAsync
{
    TestAsync async1;
    TestAsync async2;
    AsyncGroup asyncGroup;

    // A UnityTest allows `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator TestAsync()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.

        async1 = new TestAsync();
        async1.On(OnAsyncCompleted1);

        TestTaskAsync();
        yield return null;
        Debug.Log("End Test");

    }

    [UnityTest]
    public IEnumerator TestAsyncGroup()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        asyncGroup = new AsyncGroup();
        async1 = new TestAsync();
        async2 = new TestAsync();
        async1.On(OnAsyncCompleted1);
        async2.On(OnAsyncCompleted2);
        asyncGroup.On(OnAsyncGroupCompleted);

        asyncGroup.AddAsync(async1);
        asyncGroup.End();

        asyncGroup.Then((a) =>
        {
            Debug.Log("Then AsyncGroup");
        });

        TestTaskAsync();
        yield return null;
        Debug.Log("End Test");

    }

    private void OnAsyncCompleted1(IAsync async)
    {
        Debug.Log("OnAsyncCompleted1");
    }

    private void OnAsyncCompleted2(IAsync async)
    {
        Debug.Log("OnAsyncCompleted2");
    }


    private void OnAsyncGroupCompleted(IAsync async)
    {
        Debug.Log("OnAsyncGroupCompleted");
    }

    async Task TestTaskAsync()
    {
        Debug.Log("Start Task");
        await Task.Delay(1000);
        async1.Completed();
        asyncGroup.AddAsync(async2);

        await Task.Delay(1000);
        async2.Completed();

        // asyncGroup.Completed();

        Debug.Log("End Task");
    }
}
