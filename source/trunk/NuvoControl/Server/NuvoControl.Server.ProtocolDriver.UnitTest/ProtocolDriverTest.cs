/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.ProtocolDriver.UnitTest
 *   Author:         Ch.Imfeld
 *   Creation Date:  6/12/2009 11:02:29 PM
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 6/12/2009 11:02:29 PM, Ch.Imfeld: Initial implementation.
 * 
 **************************************************************************************************/


using NuvoControl.Server.ProtocolDriver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuvoControl.Server.ProtocolDriver.Interface;
using NuvoControl.Common.Configuration;
using NuvoControl.Server.ProtocolDriver.Test.Mock;
using Common.Logging;
using NuvoControl.Common;
using System.Collections.Generic;

namespace NuvoControl.Server.ProtocolDriver.Test
{
    
    
    /// <summary>
    /// Test class for ProtocolDriverTest
    /// 
    /// It is intended to contain all ProtocolDriverTest Unit Tests
    /// </summary>
    [TestClass()]
    public class ProtocolDriverTest
    {
        #region Common Logger
        /// <summary>
        /// Common logger object. Requires the using directive <c>Common.Logging</c>. See 
        /// <see cref="LogManager"/> for more information.
        /// </summary>
        private ILog _log = LogManager.GetCurrentClassLogger();
        #endregion

        private NuvoTelegramMock _nuvoTelegramMock = null;
        private NuvoEssentiaProtocol _essentiaProtocol = null;
        private NuvoEssentiaProtocolDriver _protDriver = null;
        private int _deviceId = 1;
        private Address _zoneAddress = null;
        private Communication _commConfig = new Communication("COM1", 9600, 8, 1, "None");

        private List<ProtocolCommandReceivedEventArgs> _protCommandReceivedEventArgs = new List<ProtocolCommandReceivedEventArgs>();
        private List<ProtocolZoneUpdatedEventArgs> _protZoneUpdatedEventArgs = new List<ProtocolZoneUpdatedEventArgs>();
        private List<ProtocolDeviceUpdatedEventArgs> _protDeviceUpdatedEventArgs = new List<ProtocolDeviceUpdatedEventArgs>();

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        /// </summary>
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

        /// <summary>
        /// Use TestInitialize to run code before running each test
        /// </summary>
        [TestInitialize()]
        public void MyTestInitialize()
        {
            _deviceId = 1;
            _zoneAddress = new Address(_deviceId, 1);
            createProtocolDriver(_deviceId, _commConfig);
        }
        
        /// <summary>
        /// Use TestCleanup to run code after each test has run
        /// </summary>
        [TestCleanup()]
        public void MyTestCleanup()
        {
            destroyProtocolDriver(_deviceId);
        }
        
        #endregion


        /// <summary>
        ///A test for SendCommand. Using the Single Command.
        /// </summary>
        [TestMethod()]
        public void SendCommandTest()
        {
            INuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.MuteALLZoneOFF);
            Address zoneAddress = new Address(_deviceId, 0);    // Zone x -> not used
            _protDriver.SendCommand(zoneAddress, command);
            string strMessage = _nuvoTelegramMock.Telegram;
            Assert.AreEqual("ALLMOFF", _nuvoTelegramMock.TelegramList[0]);
            Assert.AreEqual(1, _nuvoTelegramMock.TelegramList.Count);
        }

        /// <summary>
        ///A test for SendCommand. Using the combined (non-single) command.
        /// </summary>
        [TestMethod()]
        public void SendCommandTest1()
        {
            INuvoEssentiaCommand command = new NuvoEssentiaCommand(ENuvoEssentiaCommands.SetZoneStatus, ENuvoEssentiaZones.Zone4, ENuvoEssentiaSources.Source5, -22);
            Address zoneAddress = new Address(_deviceId, 4);    // Zone 4
            _protDriver.SendCommand(zoneAddress, command);
            string strMessage = _nuvoTelegramMock.Telegram;
            Assert.AreEqual("Z04ON", _nuvoTelegramMock.TelegramList[0]);
            Assert.AreEqual("Z04VOL22", _nuvoTelegramMock.TelegramList[1]);
            Assert.AreEqual("Z04SRC5", _nuvoTelegramMock.TelegramList[2]);
            Assert.AreEqual(3, _nuvoTelegramMock.TelegramList.Count);
        }

        /// <summary>
        /// A test for SendCommand
        /// This test is intended to test the 'multiple' command. This command holds more than 
        /// one single command and executes them. To the client it should look like, that the driver
        /// is executing just one command.
        /// 
        /// Pass in a combined command to set the zone state (source, volume and power state).
        /// This command, combines three single commands.
        /// </summary>
        [TestMethod()]
        public void SendCommandTest2()
        {
            INuvoEssentiaCommand command = new NuvoEssentiaCommand(ENuvoEssentiaCommands.SetZoneStatus, ENuvoEssentiaZones.Zone5, ENuvoEssentiaSources.Source4, -30);
            _protDriver.SendCommand(_zoneAddress, command);
            _protDriver.onCommandReceived += new ProtocolCommandReceivedEventHandler(_protDriver_onCommandReceived);
            _protDriver.onDeviceStatusUpdate += new ProtocolDeviceUpdatedEventHandler(_protDriver_onDeviceStatusUpdate);
            _protDriver.onZoneStatusUpdate += new ProtocolZoneUpdatedEventHandler(_protDriver_onZoneStatusUpdate);

            List<string> strMessageLsit = _nuvoTelegramMock.TelegramList;

            _nuvoTelegramMock.passDataToTestClass("Z05PWRON,SRC3,GRP0,VOL-50"); // as answer to "Z05ON" (TurnZoneOn)
            _nuvoTelegramMock.passDataToTestClass("Z05PWRON,SRC3,GRP0,VOL-30"); // as answer to "Z05VOL30" (SetVolume)
            _nuvoTelegramMock.passDataToTestClass("Z05PWRON,SRC4,GRP0,VOL-50"); // as answer to "Z05SRC4" (SetSource)

            Assert.AreEqual(0, _protDeviceUpdatedEventArgs.Count);      // get ZERO device state update events
            Assert.AreEqual(3, _protCommandReceivedEventArgs.Count);    // get THREE command events
            Assert.AreEqual(1, _protZoneUpdatedEventArgs.Count);        // get ONE zone state update event
        }

        /// <summary>
        /// Event method used in SendCommandTest2
        /// </summary>
        /// <param name="sender">This pointer to the sender of the event.</param>
        /// <param name="e">Event arguments, returned by the sender.</param>
        void _protDriver_onZoneStatusUpdate(object sender, ProtocolZoneUpdatedEventArgs e)
        {
            _protZoneUpdatedEventArgs.Add(e);
        }

        /// <summary>
        /// Event method used in SendCommandTest2
        /// </summary>
        /// <param name="sender">This pointer to the sender of the event.</param>
        /// <param name="e">Event arguments, returned by the sender.</param>
        void _protDriver_onDeviceStatusUpdate(object sender, ProtocolDeviceUpdatedEventArgs e)
        {
            _protDeviceUpdatedEventArgs.Add(e);
        }

        /// <summary>
        /// Event method used in SendCommandTest2
        /// </summary>
        /// <param name="sender">This pointer to the sender of the event.</param>
        /// <param name="e">Event arguments, returned by the sender.</param>
        void _protDriver_onCommandReceived(object sender, ProtocolCommandReceivedEventArgs e)
        {
            _protCommandReceivedEventArgs.Add(e);
        }


        /// <summary>
        /// A test for ReadZoneState
        /// </summary>
        [TestMethod()]
        public void ReadZoneStateTest()
        {
            Address zoneAddress = new Address(_deviceId, 4);    // Zone 4
            _protDriver.ReadZoneState(zoneAddress);
            string strMessage = _nuvoTelegramMock.Telegram;
            Assert.AreEqual(2, _nuvoTelegramMock.TelegramList.Count);
            Assert.AreEqual("Z04CONSR", _nuvoTelegramMock.TelegramList[0]);
            Assert.AreEqual("Z04SETSR", _nuvoTelegramMock.TelegramList[1]);
        }

        /// <summary>
        /// A test for SetZoneState.
        /// Set zone state: Zone 2, Source 5, Power On, Volume=30
        /// The volume is re-calculated from 30 (for NuvoControl) to -55 (for Nuvo Essentia)
        /// </summary>
        [TestMethod()]
        public void SetZoneState1Test()
        {
            Address zoneAddress = new Address(_deviceId, 2);                     // Zone 2
            ZoneState zoneState = new ZoneState(new Address(_deviceId, 5), true, 30, ZoneQuality.Online);    // Source 5, Power On, Volume=30
            _protDriver.SetZoneState(zoneAddress, zoneState);
            Assert.AreEqual(3, _nuvoTelegramMock.TelegramList.Count);
            Assert.AreEqual("Z02ON", _nuvoTelegramMock.TelegramList[0]);
            Assert.AreEqual("Z02VOL55", _nuvoTelegramMock.TelegramList[1]);
            Assert.AreEqual("Z02SRC5", _nuvoTelegramMock.TelegramList[2]);
        }

        /// <summary>
        /// A test for SetZoneState.
        /// Set zone state: Zone 4, Source 3, Power Off, Volume=30
        /// </summary>
        [TestMethod()]
        public void SetZoneState2Test()
        {
            Address zoneAddress = new Address(_deviceId, 4);
            ZoneState zoneState = new ZoneState(new Address(_deviceId, 3), false, 30, ZoneQuality.Online);
            _protDriver.SetZoneState(zoneAddress, zoneState);
            Assert.AreEqual(1, _nuvoTelegramMock.TelegramList.Count);
            Assert.AreEqual("Z04OFF", _nuvoTelegramMock.TelegramList[0]);
        }

        /// <summary>
        /// A test for SetZoneState.
        /// Set zone state: Zone 12, Source 6, Power On, Volume=20
        /// The volume 20 (for NuvoControl) is re-calculated to -63 (for Nuvo Essentia)
        /// </summary>
        [TestMethod()]
        public void SetZoneState3Test()
        {
            Address zoneAddress = new Address(_deviceId, 12);
            ZoneState zoneState = new ZoneState(new Address(_deviceId, 6), true, 20, ZoneQuality.Online);
            _protDriver.SetZoneState(zoneAddress, zoneState);
            Assert.AreEqual(3, _nuvoTelegramMock.TelegramList.Count);
            Assert.AreEqual("Z12ON", _nuvoTelegramMock.TelegramList[0]);
            Assert.AreEqual("Z12VOL63", _nuvoTelegramMock.TelegramList[1]);
            Assert.AreEqual("Z12SRC6", _nuvoTelegramMock.TelegramList[2]);
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
            Assert.AreEqual(1, _nuvoTelegramMock.TelegramList.Count);
            Assert.AreEqual("Z07ON", _nuvoTelegramMock.TelegramList[0]);
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
            Assert.AreEqual(1, _nuvoTelegramMock.TelegramList.Count);
            Assert.AreEqual("Z12OFF", _nuvoTelegramMock.TelegramList[0]);
        }

        /// <summary>
        /// A test for CommandSetVolume.
        /// The Volume level is re-calculated: The value 50 (of NuvoControl) is -39 (of Nuvo Essentia)
        /// </summary>
        [TestMethod()]
        public void CommandSetVolumeTest()
        {
            Address zoneAddress = new Address(_deviceId, 8);    // Zone 8
            _protDriver.CommandSetVolume(zoneAddress,50);
            string strMessage = _nuvoTelegramMock.Telegram;
            Assert.AreEqual(1, _nuvoTelegramMock.TelegramList.Count);
            Assert.AreEqual("Z08VOL39", _nuvoTelegramMock.TelegramList[0]);
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
            Assert.AreEqual(1, _nuvoTelegramMock.TelegramList.Count);
            Assert.AreEqual("Z02SRC5", _nuvoTelegramMock.TelegramList[0]);
        }

        /// <summary>
        /// A test for checkZoneDeviceId
        ///
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void checkZoneDeviceIdTest()
        {
            NuvoEssentiaProtocolDriver_Accessor target = new NuvoEssentiaProtocolDriver_Accessor();
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
            _protDriver = new NuvoEssentiaProtocolDriver();
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

        /// <summary>
        /// A test for Open. Create an instance of the serial port simulator.
        /// This is done via the COM port name "SIM"
        /// </summary>
        [TestMethod()]
        public void OpenSimulatorTest()
        {
            NuvoEssentiaProtocolDriver target = new NuvoEssentiaProtocolDriver();
            Communication communicationConfiguration = new Communication("SIM", 9600, 8, 1, "None"); // Create Simulator
            target.Open(ENuvoSystem.NuVoEssentia, _deviceId, communicationConfiguration, null);
        }

        /// <summary>
        /// A test for CommandSetSource
        /// </summary>
        [TestMethod()]
        public void SimulatorCommandSetSourceTest()
        {
            NuvoEssentiaProtocolDriver target = new NuvoEssentiaProtocolDriver();
            Communication communicationConfiguration = new Communication("SIM", 9600, 8, 1, "None"); // Create Simulator
            target.Open(ENuvoSystem.NuVoEssentia, _deviceId, communicationConfiguration, null);
            target.onCommandReceived += new ProtocolCommandReceivedEventHandler(target_onCommandReceived);

            Address zoneAddress = new Address(_deviceId, 2);                        // Zone 2
            target.CommandSetSource(zoneAddress, new Address(_deviceId, 5));    // Source 5


        }

        /// <summary>
        /// Event method used in the test SimulatorCommandSetSourceTest
        /// </summary>
        /// <param name="sender">This pointer to the sender of the event.</param>
        /// <param name="e">Event arguments, returned by the sender.</param>
        void target_onCommandReceived(object sender, ProtocolCommandReceivedEventArgs e)
        {
            _protCommandReceivedEventArgs.Add(e);
        }

    }
}
