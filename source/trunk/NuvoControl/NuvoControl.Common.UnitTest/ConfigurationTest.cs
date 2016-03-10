using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuvoControl.Common.Configuration;

namespace NuvoControl.Common.UnitTest
{
    [TestClass]
    public class ConfigurationTest
    {
        [TestMethod]
        public void TestAddressEmpty()
        {
            Address address = new Address();
            Assert.AreEqual(true, address.isEmpty());
        }

        [TestMethod]
        public void TestAddressNotEmpty()
        {
            Address address = new Address(100,1);
            Assert.AreEqual(false, address.isEmpty());
        }

        [TestMethod]
        public void TestAddress()
        {
            Address address = new Address(100, 1);
            Assert.AreEqual(100,address.DeviceId);
            Assert.AreEqual(1, address.ObjectId);
            Assert.AreEqual(false, address.isEmpty());
        }

        [TestMethod]
        public void TestAddressString()
        {
            Address address = new Address("100.1");
            Assert.AreEqual(100, address.DeviceId);
            Assert.AreEqual(1, address.ObjectId);
            Assert.AreEqual(false, address.isEmpty());
        }
    }
}
