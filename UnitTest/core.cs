using System;
using Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class core
    {
        [TestMethod]
        public void GetDriveRootTest()
        {
            Assert.IsNotNull(Drive.GetDriveRoot());
        }
    }
}
