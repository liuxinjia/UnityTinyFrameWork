using System;
using NUnit.Framework;
using UnityCodeGen.Builder;

namespace UnityCodeGen.Test.Builder
{

    public class ParameterBuilderTest
    {
        [Test]
        public void TestWithName()
        {
            var builder = new ParameterBuilder();
            builder.WithName("FooBar");

            var result = builder.Build();

            Assert.AreEqual("FooBar", result.Name);
        }

        [Test]
        public void TestWIthType()
        {
            var builder = new ParameterBuilder();
            builder.WithType("FooBar");

            var result = builder.Build();

            Assert.AreEqual("FooBar", result.Type);
        }
    }
}
