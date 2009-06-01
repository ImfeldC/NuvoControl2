using NuvoControl.Server.ProtocolDriver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuvoControl.Server.ProtocolDriver.Interface;
using NuvoControl.Common.Configuration;

namespace NuvoControl.Server.ProtocolDriver.Test
{
    
    
    /// <summary>
    ///This is a test class for ProtocolDriverTest and is intended
    ///to contain all ProtocolDriverTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ProtocolDriverTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for SendCommand
        ///</summary>
        [TestMethod()]
        public void SendCommandTest1()
        {
            ProtocolDriver target = new ProtocolDriver(); // TODO: Initialize to an appropriate value
            Address zoneAddress = null; // TODO: Initialize to an appropriate value
            INuvoEssentiaCommand command = null; // TODO: Initialize to an appropriate value
            target.SendCommand(zoneAddress, command);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for SendCommand
        ///</summary>
        [TestMethod()]
        public void SendCommandTest()
        {
            ProtocolDriver target = new ProtocolDriver(); // TODO: Initialize to an appropriate value
            Address zoneAddress = null; // TODO: Initialize to an appropriate value
            INuvoEssentiaSingleCommand command = null; // TODO: Initialize to an appropriate value
            target.SendCommand(zoneAddress, command);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for ReadZoneStatus
        ///</summary>
        [TestMethod()]
        public void ReadZoneStatusTest()
        {
            ProtocolDriver target = new ProtocolDriver(); // TODO: Initialize to an appropriate value
            Address zoneAddress = null; // TODO: Initialize to an appropriate value
            target.ReadZoneStatus(zoneAddress);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Open
        ///</summary>
        [TestMethod()]
        public void OpenTest1()
        {
            ProtocolDriver target = new ProtocolDriver(); // TODO: Initialize to an appropriate value
            ENuvoSystem system = new ENuvoSystem(); // TODO: Initialize to an appropriate value
            int deviceId = 0; // TODO: Initialize to an appropriate value
            Communication communicationConfiguration = null; // TODO: Initialize to an appropriate value
            INuvoEssentiaProtocol essentiaProtocol = null; // TODO: Initialize to an appropriate value
            target.Open(system, deviceId, communicationConfiguration, essentiaProtocol);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Open
        ///</summary>
        [TestMethod()]
        public void OpenTest()
        {
            ProtocolDriver target = new ProtocolDriver(); // TODO: Initialize to an appropriate value
            ENuvoSystem system = new ENuvoSystem(); // TODO: Initialize to an appropriate value
            int deviceId = 0; // TODO: Initialize to an appropriate value
            Communication communicationConfiguration = null; // TODO: Initialize to an appropriate value
            target.Open(system, deviceId, communicationConfiguration);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for convertAddressZone2EssentiaZone
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void convertAddressZone2EssentiaZoneTest()
        {
            ProtocolDriver_Accessor target = new ProtocolDriver_Accessor(); // TODO: Initialize to an appropriate value
            Address zoneAddress = null; // TODO: Initialize to an appropriate value
            ENuvoEssentiaZones expected = new ENuvoEssentiaZones(); // TODO: Initialize to an appropriate value
            ENuvoEssentiaZones actual;
            actual = target.convertAddressZone2EssentiaZone(zoneAddress);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for convertAddressSource2EssentiaSource
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void convertAddressSource2EssentiaSourceTest()
        {
            ProtocolDriver_Accessor target = new ProtocolDriver_Accessor(); // TODO: Initialize to an appropriate value
            Address sourceAddress = null; // TODO: Initialize to an appropriate value
            ENuvoEssentiaSources expected = new ENuvoEssentiaSources(); // TODO: Initialize to an appropriate value
            ENuvoEssentiaSources actual;
            actual = target.convertAddressSource2EssentiaSource(sourceAddress);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CommandSwitchZoneON
        ///</summary>
        [TestMethod()]
        public void CommandSwitchZoneONTest()
        {
            ProtocolDriver target = new ProtocolDriver(); // TODO: Initialize to an appropriate value
            Address zoneAddress = null; // TODO: Initialize to an appropriate value
            target.CommandSwitchZoneON(zoneAddress);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for CommandSwitchZoneOFF
        ///</summary>
        [TestMethod()]
        public void CommandSwitchZoneOFFTest()
        {
            ProtocolDriver target = new ProtocolDriver(); // TODO: Initialize to an appropriate value
            Address zoneAddress = null; // TODO: Initialize to an appropriate value
            target.CommandSwitchZoneOFF(zoneAddress);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for CommandSetVolume
        ///</summary>
        [TestMethod()]
        public void CommandSetVolumeTest()
        {
            ProtocolDriver target = new ProtocolDriver(); // TODO: Initialize to an appropriate value
            Address zoneAddress = null; // TODO: Initialize to an appropriate value
            int volumeLevel = 0; // TODO: Initialize to an appropriate value
            target.CommandSetVolume(zoneAddress, volumeLevel);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for CommandSetSource
        ///</summary>
        [TestMethod()]
        public void CommandSetSourceTest()
        {
            ProtocolDriver target = new ProtocolDriver(); // TODO: Initialize to an appropriate value
            Address zoneAddress = null; // TODO: Initialize to an appropriate value
            Address sourceAddress = null; // TODO: Initialize to an appropriate value
            target.CommandSetSource(zoneAddress, sourceAddress);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Close
        ///</summary>
        [TestMethod()]
        public void CloseTest()
        {
            ProtocolDriver target = new ProtocolDriver(); // TODO: Initialize to an appropriate value
            int deviceId = 0; // TODO: Initialize to an appropriate value
            target.Close(deviceId);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for checkZoneDeviceId
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void checkZoneDeviceIdTest()
        {
            ProtocolDriver_Accessor target = new ProtocolDriver_Accessor(); // TODO: Initialize to an appropriate value
            int deviceId = 0; // TODO: Initialize to an appropriate value
            target.checkZoneDeviceId(deviceId);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for _essentiaProtocol_onCommandReceived
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void _essentiaProtocol_onCommandReceivedTest()
        {
            ProtocolDriver_Accessor target = new ProtocolDriver_Accessor(); // TODO: Initialize to an appropriate value
            object sender = null; // TODO: Initialize to an appropriate value
            NuvoEssentiaProtocolEventArgs e = null; // TODO: Initialize to an appropriate value
            target._essentiaProtocol_onCommandReceived(sender, e);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for ProtocolDriver Constructor
        ///</summary>
        [TestMethod()]
        public void ProtocolDriverConstructorTest()
        {
            ProtocolDriver target = new ProtocolDriver();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
