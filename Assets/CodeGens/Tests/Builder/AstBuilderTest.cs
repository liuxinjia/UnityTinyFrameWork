using System;
using NUnit.Framework;
using UnityCodeGen.Builder;

namespace UnityCodeGen.Test.Builder
{

    public class AstBuilderTest
    {
        [Test]
        public void ItBuildsWithAUsing()
        {
            var builder = new AstBuilder();
            builder.WithUsing();

            var result = builder.Build();

            Assert.AreEqual(1, result.Usings.Length);
        }

        [Test]
        public void ItBuildsWithAClass()
        {
            var builder = new AstBuilder();
            builder.WithClass();

            var result = builder.Build();

            Assert.AreEqual(1, result.Classes.Length);
        }

        [Test]
        public void ItBuildsWithANamespace()
        {
            var builder = new AstBuilder();
            builder.WithNamespace();

            var result = builder.Build();

            Assert.AreEqual(1, result.Namespaces.Length);
        }
    }
}
