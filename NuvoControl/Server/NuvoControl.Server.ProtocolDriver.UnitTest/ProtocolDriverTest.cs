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
using System.Threading;

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
            Thread.Sleep(2000); // give control to worker thread

            Assert.IsTrue(_nuvoTelegramMock.TelegramList.Count >= 1);
            Assert.AreEqual(true, _nuvoTelegramMock.TelegramList.Contains("ALLMOFF"));
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
            Thread.Sleep(2000); // give control to worker thread

            Assert.IsTrue(_nuvoTelegramMock.TelegramList.Count >= 3);
            Assert.AreEqual(true, _nuvoTelegramMock.TelegramList.Contains("Z04ON"));
            Assert.AreEqual(true, _nuvoTelegramMock.TelegramList.Contains("Z04VOL22"));
            Assert.AreEqual(true, _nuvoTelegramMock.TelegramList.Contains("Z04SRC5"));
        }

        /// <summary>
        /// A test for SendCommand
        /// This test is intended to test one 'multiple' command. This command holds more than 
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
            _protDriver.onCommandReceived += new ProtocolCommandReceivedEventHandler(_protDriver_onCommandReceived);
            _protDriver.onDeviceStatusUpdate += new ProtocolDeviceUpdatedEventHandler(_protDriver_onDeviceStatusUpdate);
            _protDriver.onZoneStatusUpdate += new ProtocolZoneUpdatedEventHandler(_protDriver_onZoneStatusUpdate);
            _protDriver.SendCommand(_zoneAddress, command);
            Thread.Sleep(2000); // give control to worker thread

            _nuvoTelegramMock.passDataToTestClass("Z05PWRON,SRC3,GRP0,VOL-50"); // as answer to "Z05ON" (TurnZoneOn)
            _nuvoTelegramMock.passDataToTestClass("Z05PWRON,SRC3,GRP0,VOL-30"); // as answer to "Z05VOL30" (SetVolume)
            _nuvoTelegramMock.passDataToTestClass("Z05PWRON,SRC4,GRP0,VOL-50"); // as answer to "Z05SRC4" (SetSource)

            Assert.AreEqual(0, _protDeviceUpdatedEventArgs.Count);      // get ZERO device state update events
            Assert.AreEqual(3, _protCommandReceivedEventArgs.Count);    // get THREE command events
            Assert.AreEqual(1, _protZoneUpdatedEventArgs.Count);        // get ONE zone state update event
        }

        /// <summary>
        /// A test for SendCommand
        /// This test is intended to test more than one 'multiple' command. This command holds more than 
        /// one single command and executes them. To the client it should look like, that the driver
        /// is executing just one command.
        /// 
        /// Pass in a combined command to set the zone state (source, volume and power state).
        /// This command, combines three single commands.
        /// </summary>
        [TestMethod()]
        public void SendCommandTest3()
        {
            INuvoEssentiaCommand command1 = new NuvoEssentiaCommand(ENuvoEssentiaCommands.SetZoneStatus, ENuvoEssentiaZones.Zone5, ENuvoEssentiaSources.Source4, -30);
            INuvoEssentiaCommand command2 = new NuvoEssentiaCommand(ENuvoEssentiaCommands.SetZoneStatus, ENuvoEssentiaZones.Zone7, ENuvoEssentiaSources.Source2, -40);
            _protDriver.onCommandReceived += new ProtocolCommandReceivedEventHandler(_protDriver_onCommandReceived);
            _protDriver.onDeviceStatusUpdate += new ProtocolDeviceUpdatedEventHandler(_protDriver_onDeviceStatusUpdate);
            _protDriver.onZoneStatusUpdate += new ProtocolZoneUpdatedEventHandler(_protDriver_onZoneStatusUpdate);
            _protDriver.SendCommand(_zoneAddress, command1);
            _protDriver.SendCommand(_zoneAddress, command2);
            Thread.Sleep(2000); // give control to worker thread

            // return the answer of the first command
            _nuvoTelegramMock.passDataToTestClass("Z05PWRON,SRC3,GRP0,VOL-50");
            _nuvoTelegramMock.passDataToTestClass("Z05PWRON,SRC3,GRP0,VOL-30"); 
            _nuvoTelegramMock.passDataToTestClass("Z05PWRON,SRC4,GRP0,VOL-30");

            Assert.AreEqual(0, _protDeviceUpdatedEventArgs.Count);      // get ZERO device state update events
            Assert.AreEqual(3, _protCommandReceivedEventArgs.Count);    // get THREE command events
            Assert.AreEqual(1, _protZoneUpdatedEventArgs.Count);        // get ONE zone state update event

            // .. and now return the answer of the second command
            _nuvoTelegramMock.passDataToTestClass("Z07PWRON,SRC3,GRP0,VOL-50");
            _nuvoTelegramMock.passDataToTestClass("Z07PWRON,SRC3,GRP0,VOL-40");
            _nuvoTelegramMock.passDataToTestClass("Z07PWRON,SRC2,GRP0,VOL-40");

            Assert.AreEqual(0, _protDeviceUpdatedEventArgs.Count);      // get ZERO device state update events
            Assert.AreEqual(6, _protCommandReceivedEventArgs.Count);    // get SIX command events
            Assert.AreEqual(2, _protZoneUpdatedEventArgs.Count);        // get TWO zone state update event
        }

        /// <summary>
        /// A test for SendCommand
        /// This test is used to test the asnwer returned by the protocol driver
        /// in case a 'ALLOFF' has been received.
        /// In this case each zone needs to updated, because the new zone state (OFF)
        /// is not automatically send for each zone.
        /// </summary>
        [TestMethod()]
        public void SendCommandTest4()
        {
            INuvoEssentiaCommand command = new NuvoEssentiaCommand(ENuvoEssentiaCommands.SetZoneStatus, ENuvoEssentiaZones.Zone5, ENuvoEssentiaSources.Source4, -30);
            _protDriver.onCommandReceived += new ProtocolCommandReceivedEventHandler(_protDriver_onCommandReceived);
            _protDriver.onDeviceStatusUpdate += new ProtocolDeviceUpdatedEventHandler(_protDriver_onDeviceStatusUpdate);
            _protDriver.onZoneStatusUpdate += new ProtocolZoneUpdatedEventHandler(_protDriver_onZoneStatusUpdate);

            _nuvoTelegramMock.passDataToTestClass("ALLOFF");    // receive spontaneous command

            Assert.AreEqual(0, _protDeviceUpdatedEventArgs.Count);      // get ZERO device state update events
            Assert.AreEqual(1, _protCommandReceivedEventArgs.Count);    // get ONE command events
            Assert.AreEqual(12, _protZoneUpdatedEventArgs.Count);       // get TWELVE zone state update event
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
            Thread.Sleep(2000); // give control to worker thread

            Assert.IsTrue(_nuvoTelegramMock.TelegramList.Count >= 2);
            Assert.AreEqual(true, _nuvoTelegramMock.TelegramList.Contains("Z04CONSR"));
            Assert.AreEqual(true, _nuvoTelegramMock.TelegramList.Contains("Z04SETSR"));
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
            Thread.Sleep(2000); // give control to worker thread

            Assert.IsTrue(_nuvoTelegramMock.TelegramList.Count >= 3);
            Assert.AreEqual(true, _nuvoTelegramMock.TelegramList.Contains("Z02ON"));
            Assert.AreEqual(true, _nuvoTelegramMock.TelegramList.Contains("Z02VOL55"));
            Assert.AreEqual(true, _nuvoTelegramMock.TelegramList.Contains("Z02SRC5"));
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
            Thread.Sleep(2000); // give control to worker thread

            Assert.IsTrue(_nuvoTelegramMock.TelegramList.Count >= 1);
            Assert.AreEqual(true, _nuvoTelegramMock.TelegramList.Contains("Z04OFF"));
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
            Thread.Sleep(2000); // give control to worker thread

            Assert.IsTrue(_nuvoTelegramMock.TelegramList.Count >= 3);
            Assert.AreEqual(true, _nuvoTelegramMock.TelegramList.Contains("Z12ON"));
            Assert.AreEqual(true, _nuvoTelegramMock.TelegramList.Contains("Z12VOL63"));
            Assert.AreEqual(true, _nuvoTelegramMock.TelegramList.Contains("Z12SRC6"));
        }

        /// <summary>
        /// A test for CommandSwitchZoneON
        /// </summary>
        [TestMethod()]
        public void CommandSwitchZoneONTest()
        {
            Address zoneAddress = new Address(_deviceId, 7);    // Zone 7
            _protDriver.CommandSwitchZoneON(zoneAddress);
            Thread.Sleep(2000); // give control to worker thread

            Assert.IsTrue(_nuvoTelegramMock.TelegramList.Count >= 1);
            Assert.AreEqual(true, _nuvoTelegramMock.TelegramList.Contains("Z07ON"));
        }

        /// <summary>
        /// A test for CommandSwitchZoneOFF
        /// </summary>
        [TestMethod()]
        public void CommandSwitchZoneOFFTest()
        {
            Address zoneAddress = new Address(_deviceId, 12);    // Zone 12
            _protDriver.CommandSwitchZoneOFF(zoneAddress);
            Thread.Sleep(2000); // give control to worker thread

            Assert.IsTrue(_nuvoTelegramMock.TelegramList.Count >= 1);
            Assert.AreEqual(true, _nuvoTelegramMock.TelegramList.Contains("Z12OFF"));
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
            Thread.Sleep(2000); // give control to worker thread

            Assert.IsTrue(_nuvoTelegramMock.TelegramList.Count >= 1);
            Assert.AreEqual(true, _nuvoTelegramMock.TelegramList.Contains("Z08VOL39"));
        }

        /// <summary>
        /// A test for CommandSetSource
        /// </summary>
        [TestMethod()]
        public void CommandSetSourceTest()
        {
            Address zoneAddress = new Address(_deviceId, 2);                        // Zone 2
            _protDriver.CommandSetSource(zoneAddress, new Address(_deviceId,5));    // Source 5
            Thread.Sleep(2000); // give control to worker thread

            Assert.IsTrue( _nuvoTelegramMock.TelegramList.Count >= 1 );
            Assert.AreEqual(true, _nuvoTelegramMock.TelegramList.Contains("Z02SRC5"));
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



        /// <summary>
        /// Test method for the private method removeReadVersionCommandFromList
        /// </summary>
        [TestMethod()]
        public void testRemoveReadVersionCommandFromList()
        {
            List<string> _testList = new List<string>();

            _testList.Add("String1");
            _testList.Add("String2");
            _testList.Add("VER");
            _testList.Add("VER");
            _testList.Add("String3");
            _testList.Add("VER");
            _testList.Add("VER");
            _testList.Add("String4");

            removeReadVersionCommandFromList(_testList, "VER");

            Assert.AreEqual(4, _testList.Count);
            Assert.AreEqual("String1", _testList[0]);
            Assert.AreEqual("String2", _testList[1]);
            Assert.AreEqual("String3", _testList[2]);
            Assert.AreEqual("String4", _testList[3]);

        }

        /// <summary>
        /// Private method to remove several items form a string list.
        /// </summary>
        /// <param name="cmdList">List, where to remove all occurences of the string strToRemove</param>
        /// <param name="strToRemove">String to remove from the list cmdList</param>
        private void removeReadVersionCommandFromList(List<string> cmdList, string strToRemove)
        {
            while (cmdList.Remove(strToRemove))
            {
            }
        }
    }
}
