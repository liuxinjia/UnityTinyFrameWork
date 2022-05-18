using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCodeGen.Builder;

namespace UnityCodeGen.Test.Builder
{

    public class NamespaceBuilderTest
    {
        [Test]
        public void ItBuildsWithCorrectName()
        {
            var builder = new NamespaceBuilder();
            builder.WithName("FooBar");

            var result = builder.Build();

            Assert.AreEqual("FooBar", result.Name);
        }

        [Test]
        public void ItBuildsWithAClass()
        {
            var builder = new NamespaceBuilder();
            builder.WithClass();

            var result = builder.Build();

            Assert.AreEqual(1, result.Classes.Length);
        }
    }
}
