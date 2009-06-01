using NuvoControl.Server.ProtocolDriver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuvoControl.Server.ProtocolDriver.Interface;
using NuvoControl.Common.Configuration;
using NuvoControl.Server.ProtocolDriver.Test.Mock;
using Common.Logging;

namespace NuvoControl.Server.ProtocolDriver.Test
{
    
    
    /// <summary>
    ///This is a test class for ProtocolDriverTest and is intended
    ///to contain all ProtocolDriverTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ProtocolDriverTest
    {
        private ILog _log = LogManager.GetCurrentClassLogger();

        private NuvoTelegramMock _nuvoTelegramMock = null;
        private NuvoEssentiaProtocol _essentiaProtocol = null;
        private ProtocolDriver _protDriver = null;
        private int _deviceId = 1;
        private Address _zoneAddress = null;
        private Communication _commConfig = new Communication("COM1", 9600, 8, 1, "None");

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
        [TestInitialize()]
        public void MyTestInitialize()
        {
            _deviceId = 1;
            _zoneAddress = new Address(_deviceId, 1);
            createProtocolDriver(_deviceId, _commConfig);
        }
        
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            destroyProtocolDriver(_deviceId);
        }
        
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
        /// A test for convertAddressZone2EssentiaZone
        /// </summary>
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
        /// A test for convertAddressSource2EssentiaSource
        /// </summary>
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
        /// A test for ReadZoneStatus
        /// </summary>
        [TestMethod()]
        public void ReadZoneStatusTest()
        {
            Address zoneAddress = new Address(_deviceId, 4);    // Zone 4
            _protDriver.ReadZoneStatus(zoneAddress);
            string strMessage = _nuvoTelegramMock.Telegram;
            Assert.AreEqual("Z04CONSR", _nuvoTelegramMock.TelegramList[0]);
            Assert.AreEqual("Z04SETSR", _nuvoTelegramMock.TelegramList[1]);
            Assert.AreEqual(2, _nuvoTelegramMock.TelegramList.Count);
        }

        /// <summary>
        /// A test for CommandSwitchZoneON
        /// </summary>
        [TestMethod()]
        public void CommandSwitchZoneONTest()
        {
            Address zoneAddress = new Address(_deviceId, 7);    // Zone 7
            _protDriver.CommandSwitchZoneON(zoneAddress);
            string strMessage = _nuvoTelegramMock.Telegram;
            Assert.AreEqual("Z07ON", _nuvoTelegramMock.TelegramList[0]);
            Assert.AreEqual(1, _nuvoTelegramMock.TelegramList.Count);
        }

        /// <summary>
        /// A test for CommandSwitchZoneOFF
        /// </summary>
        [TestMethod()]
        public void CommandSwitchZoneOFFTest()
        {
            Address zoneAddress = new Address(_deviceId, 12);    // Zone 12
            _protDriver.CommandSwitchZoneOFF(zoneAddress);
            string strMessage = _nuvoTelegramMock.Telegram;
            Assert.AreEqual("Z12OFF", _nuvoTelegramMock.TelegramList[0]);
            Assert.AreEqual(1, _nuvoTelegramMock.TelegramList.Count);
        }

        /// <summary>
        /// A test for CommandSetVolume
        /// </summary>
        [TestMethod()]
        public void CommandSetVolumeTest()
        {
            Address zoneAddress = new Address(_deviceId, 8);    // Zone 8
            _protDriver.CommandSetVolume(zoneAddress,50);
            string strMessage = _nuvoTelegramMock.Telegram;
            Assert.AreEqual("Z08VOL50", _nuvoTelegramMock.TelegramList[0]);
            Assert.AreEqual(1, _nuvoTelegramMock.TelegramList.Count);
        }

        /// <summary>
        /// A test for CommandSetSource
        /// </summary>
        [TestMethod()]
        public void CommandSetSourceTest()
        {
            Address zoneAddress = new Address(_deviceId, 2);                        // Zone 2
            _protDriver.CommandSetSource(zoneAddress, new Address(_deviceId,5));    // Source 5
            string strMessage = _nuvoTelegramMock.Telegram;
            Assert.AreEqual("Z02SRC5", _nuvoTelegramMock.TelegramList[0]);
            Assert.AreEqual(1, _nuvoTelegramMock.TelegramList.Count);
        }

        /// <summary>
        /// A test for checkZoneDeviceId
        ///
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void checkZoneDeviceIdTest()
        {
            ProtocolDriver_Accessor target = new ProtocolDriver_Accessor();
            target.Open(ENuvoSystem.NuVoEssentia, 1, _commConfig);  // Open device with id=1

            // Test existing device id
            target.checkZoneDeviceId(1);

            // Test non-existing device id
            try
            {
                target.checkZoneDeviceId(4);
                Assert.Fail("Failed! Excpetion was expected!");
            }
            catch( ProtocolDriverException ex )
            {
                _log.DebugFormat("ProtocolDriverException caught! Exception={0}", ex.ToString());
            }
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
        /// A test for ProtocolDriver Constructor.
        /// With this test several other methods are also tested.
        /// The Open() and Close() method are used in the unit test
        /// initialization. If no expection is thrown, this methods
        /// are also ok.
        /// </summary>
        [TestMethod()]
        public void ProtocolDriverConstructorTest()
        {
            _log.DebugFormat("Unit test for Constructor! Test is ok, if this method runs without execption.");
        }

        /// <summary>
        /// Private method to create a protocol driver stack, using the mock class
        /// for the telegram stack.
        /// </summary>
        /// <param name="deviceId">Device Id to setup the stack.</param>
        /// <param name="commConfig">Communication Configuration to setup the stack.</param>
        private void createProtocolDriver(int deviceId, Communication commConfig)
        {
            _nuvoTelegramMock = new NuvoTelegramMock();
            _essentiaProtocol = new NuvoEssentiaProtocol(deviceId, _nuvoTelegramMock);
            _protDriver = new ProtocolDriver();
            _protDriver.Open(ENuvoSystem.NuVoEssentia, deviceId, commConfig, _essentiaProtocol);
        }

        /// <summary>
        /// Private method to destroy the protocol stack.
        /// </summary>
        /// <param name="deviceId">Device Id used to setup the stack.</param>
        private void destroyProtocolDriver(int deviceId)
        {
            if (_protDriver != null)
            {
                _protDriver.Close(deviceId);
            }
            _protDriver = null;
            _essentiaProtocol = null;
            _nuvoTelegramMock = null;
        }
    }
}
