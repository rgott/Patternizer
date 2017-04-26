using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PatternizerTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string blah = "as";
            Assert.AreEqual("as", blah);
        }
    }
}
