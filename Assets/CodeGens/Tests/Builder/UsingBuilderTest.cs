using System;
using NUnit.Framework;
using UnityCodeGen.Builder;

namespace UnityCodeGen.Test.Builder
{

    public class UsingBuilderTest
    {
        [Test]
        public void ItBuildsWithCorrectNamespaceName()
        {
            var builder = new UsingBuilder();
            builder.WithNamespaceName("FooBar");

            var result = builder.Build();

            Assert.AreEqual("FooBar", result.NamespaceName);
        }
    }
}
