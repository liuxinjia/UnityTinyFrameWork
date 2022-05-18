using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityCodeGen.Ast;
using UnityCodeGen.Builder;

namespace UnityCodeGen.Test.Builder
{

    public class PropertyBuilderTest
    {
        [Test]
        public void ItBuildsWithCorrectName()
        {
            var builder = new PropertyBuilder();
            builder.WithName("FooBar");

            var result = builder.Build();

            Assert.AreEqual("FooBar", result.Name);
        }

        [Test]
        public void ItBuildsWithCorrectGetAccess()
        {
            var builder = new PropertyBuilder();
            builder.WithVisibility(AccessType.Public);

            var result = builder.Build();

            Assert.AreEqual(AccessType.Public, result.Visibility);
        }

        [Test]
        public void ItBuildsWithCorrectSetAccess()
        {
            var builder = new PropertyBuilder();
            builder.WithSetVisibility(AccessType.Public);

            var result = builder.Build();

            Assert.AreEqual(AccessType.Public, result.SetVisibility);
        }

        [Test]
        public void ItBuildsWithCorrectType()
        {
            var builder = new PropertyBuilder();
            builder.WithType("FooBar");

            var result = builder.Build();

            Assert.AreEqual("FooBar", result.Type);
        }
    }
}
