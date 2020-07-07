using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xarial.Docify.Base.Context;
using Xarial.Docify.Core.Compiler.Context;
using Xarial.Docify.Core.Helpers;

namespace Core.Tests
{
    public class ContextMetadataTests
    {
        public enum Test_e 
        {
            x,
            y,
            z
        }

        [Test]
        public void TestGetOrDefault() 
        {
            var md = CreateMetadata("val1: 1");
            
            var res1 = md.GetOrDefault<int>("val1");
            var res2 = md.GetOrDefault<string>("val2");

            Assert.AreEqual(1, res1);
            Assert.AreEqual(null, res2);
        }

        [Test]
        public void TestGetOrDefaultArray()
        {
            var md = CreateMetadata("val1:\r\n  - 1\r\n  - 2");

            var res1 = md.GetOrDefault<string[]>("val1");

            Assert.IsTrue(res1.SequenceEqual(new string[] { "1", "2" }));
        }

        [Test]
        public void TestGetOrDefaultDictionary()
        {
            var md = CreateMetadata("val1:\r\n  a: X\r\n  b: Y");

            var res1 = md.GetOrDefault<Dictionary<string, object>>("val1");

            Assert.AreEqual(2, res1.Count);
            Assert.AreEqual("X", res1["a"]);
            Assert.AreEqual("Y", res1["b"]);
        }

        [Test]
        public void TestGetOrDefaultEnum()
        {
            var md = CreateMetadata("val1: y");

            var res1 = md.GetOrDefault<Test_e>("val1");

            Assert.AreEqual(Test_e.y, res1);
        }

        public class ObjectMock 
        {
            public class SubClass 
            {
                public bool BoolVal { get; set; }
                public IEnumerable<string> Enumerable { get; set; }
                public Test_e Enum { get; set; }
            }

            public int IntVal { get; set; }
            public string StrVal { get; set; }
            public Dictionary<string, int> DictVals { get; set; }
            public SubClass Nested { get; set; }
        }

        [Test]
        public void TestToObject()
        {
            var md = CreateMetadata("int-val: 1\r\nstr-val: ABC\r\nDictVals:\r\n  a: 10\r\n  b: 20\r\nNested:\r\n  bool-val: true\r\n  enumerable:\r\n    - K\r\n    - L\r\n  enum: z");

            var res1 = md.ToObject<ObjectMock>();

            Assert.AreEqual(1, res1.IntVal);
            Assert.AreEqual("ABC", res1.StrVal);
            Assert.AreEqual(2, res1.DictVals.Count);
            Assert.AreEqual(10, res1.DictVals["A"]);
            Assert.AreEqual(20, res1.DictVals["B"]);
            Assert.IsNotNull(res1.Nested);
            Assert.AreEqual(true, res1.Nested.BoolVal);
            Assert.IsTrue(res1.Nested.Enumerable.SequenceEqual(new string[] { "K", "L" }));
            Assert.AreEqual(Test_e.z, res1.Nested.Enum);
        }

        private IContextMetadata CreateMetadata(string paramStr)
        {
            var yamlDeserializer = new MetadataSerializer();

            var data = yamlDeserializer.Deserialize<Dictionary<string, object>>(paramStr);

            return new ContextMetadata(data);
        }
    }
}
