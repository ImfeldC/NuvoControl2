using NuvoControl.Server.ProtocolDriver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuvoControl.Server.ProtocolDriver.Interface;
using NuvoControl.Server.ProtocolDriver.Test.Mock;

namespace NuvoControl.Server.ProtocolDriver.Test
{
    
    
    /// <summary>
    ///This is a test class for NuvoEssentiaProtocolTest and is intended
    ///to contain all NuvoEssentiaProtocolTest Unit Tests
    ///</summary>
    [TestClass()]
    public class NuvoEssentiaProtocolTest
    {
        bool _eventRaised = false;
        int _eventRaisedCount = 0;
        NuvoEssentiaProtocolEventArgs _nuvoProtocolEventArgs = null;

        // callback function for receiving data from telegram layer
        void serialPort_CommandReceived(object sender, NuvoEssentiaProtocolEventArgs e)
        {
            _eventRaised = true;
            _eventRaisedCount++;
            _nuvoProtocolEventArgs = e;
        }


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
            _eventRaised = false;
            _eventRaisedCount = 0;
            _nuvoProtocolEventArgs = null;
        }
        
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
        }
        
        #endregion


        /// <summary>
        /// A test for SendCommand.
        /// Send a the command 'ReadVersion' and test return value.
        /// </summary>
        [TestMethod()]
        public void SendCommand1Test()
        {
            NuvoTelegramMock nuvoTelegram = new NuvoTelegramMock();
            NuvoEssentiaProtocol target = new NuvoEssentiaProtocol(nuvoTelegram);
            target.onCommandReceived += new NuvoEssentiaProtocolEventHandler(serialPort_CommandReceived);

            NuvoEssentiaCommand command = new NuvoEssentiaCommand(ENuvoEssentiaCommands.ReadVersion);
            target.SendCommand(command);
            nuvoTelegram.passDataToTestClass("NUVO_E6D_vx.yy");

            Assert.IsTrue(_eventRaisedCount == 1);                                                       // event has been raised 1 times
            Assert.AreEqual(command.Command,_nuvoProtocolEventArgs.Command.Command);                     // return same command      
            Assert.AreEqual(ENuvoEssentiaCommands.ReadVersion,_nuvoProtocolEventArgs.Command.Command);   // return same command      
        }

        /// <summary>
        /// A test for SendCommand
        /// Send a the command 'ReadVersion' (as string) and test return value.
        /// </summary>
        [TestMethod()]
        public void SendCommand2Test()
        {
            NuvoTelegramMock nuvoTelegram = new NuvoTelegramMock();
            NuvoEssentiaProtocol target = new NuvoEssentiaProtocol(nuvoTelegram);
            target.onCommandReceived += new NuvoEssentiaProtocolEventHandler(serialPort_CommandReceived);

            target.SendCommand("VER");
            nuvoTelegram.passDataToTestClass("NUVO_E6D_vx.yy");

            Assert.IsTrue(_eventRaisedCount == 1);                                                       // event has been raised 1 times
            Assert.AreEqual(ENuvoEssentiaCommands.ReadVersion, _nuvoProtocolEventArgs.Command.Command);   // return same command      
        }

        /// <summary>
        /// A test for SendCommand
        /// Send a the command 'ReadVersion' (as string) and test return value.
        /// </summary>
        [TestMethod()]
        public void SendCommand3Test()
        {
            NuvoTelegramMock nuvoTelegram = new NuvoTelegramMock();
            NuvoEssentiaProtocol target = new NuvoEssentiaProtocol(nuvoTelegram);
            target.onCommandReceived += new NuvoEssentiaProtocolEventHandler(serialPort_CommandReceived);

            target.SendCommand("VER");
            nuvoTelegram.passDataToTestClass("NUVO_E6D_v1.23");

            Assert.IsTrue(_eventRaisedCount == 1);                                                       // event has been raised 1 times
            Assert.AreEqual(ENuvoEssentiaCommands.ReadVersion, _nuvoProtocolEventArgs.Command.Command);   // return same command      
        }

        /// <summary>
        /// A test for SendCommand
        /// Send several commands and pass correct answers.
        /// Test that it returns always the corresponding command. (Don't test the parsing yet)
        /// </summary>
        [TestMethod()]
        public void SendCommand4Test()
        {
            NuvoTelegramMock nuvoTelegram = new NuvoTelegramMock();
            NuvoEssentiaProtocol target = new NuvoEssentiaProtocol(nuvoTelegram);
            target.onCommandReceived += new NuvoEssentiaProtocolEventHandler(serialPort_CommandReceived);

            // Command: ReadVersion
            {
                NuvoEssentiaCommand command = new NuvoEssentiaCommand(ENuvoEssentiaCommands.ReadVersion);
                target.SendCommand(command);
                nuvoTelegram.passDataToTestClass("NUVO_E6D_v1.23");

                Assert.IsTrue(_eventRaisedCount == 1);                                                        // event has been raised 1 times
                Assert.AreEqual(ENuvoEssentiaCommands.ReadVersion, command.Command);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadVersion, _nuvoProtocolEventArgs.Command.Command);   // return same command      
                Assert.AreEqual(command.Guid, _nuvoProtocolEventArgs.Command.Guid);                           // return same instance of command
            }

            // Command: ReadStatusCONNECT
            {
                NuvoEssentiaCommand command2 = new NuvoEssentiaCommand(ENuvoEssentiaCommands.ReadStatusCONNECT);
                target.SendCommand(command2);
                nuvoTelegram.passDataToTestClass("ZxxPWRppp,SRCs,GRPq,VOL-yy");

                Assert.IsTrue(_eventRaisedCount == 2);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, command2.Command);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, _nuvoProtocolEventArgs.Command.Command);   // return same command      
                Assert.AreEqual(command2.Guid, _nuvoProtocolEventArgs.Command.Guid);                                // return same instance of command
            }

            // Command: ReadStatusCONNECT
            {
                NuvoEssentiaCommand command3 = new NuvoEssentiaCommand(ENuvoEssentiaCommands.ReadStatusCONNECT);
                target.SendCommand(command3);
                nuvoTelegram.passDataToTestClass("ZxxORp,BASSuuu,TREBttt,GRPq,VRSTr");    // return value for ReadStatusZONE

                Assert.IsTrue(_eventRaisedCount == 3);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, command3.Command);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusZONE, _nuvoProtocolEventArgs.Command.Command);
                Assert.AreNotEqual(command3.Guid, _nuvoProtocolEventArgs.Command.Guid);  
            }
        }


        /// <summary>
        /// A test for SendCommand
        /// Send several commands and pass correct answers.
        /// Test that it returns always the corresponding command and the correct values
        /// The commands tested in this unit test refer 1:1 between outgoing and incoming command string
        /// </summary>
        [TestMethod()]
        public void SendCommand5Test()
        {
            NuvoTelegramMock nuvoTelegram = new NuvoTelegramMock();
            NuvoEssentiaProtocol target = new NuvoEssentiaProtocol(nuvoTelegram);
            target.onCommandReceived += new NuvoEssentiaProtocolEventHandler(serialPort_CommandReceived);

            // Command: ReadVersion
            {
                NuvoEssentiaCommand command = new NuvoEssentiaCommand(ENuvoEssentiaCommands.ReadVersion);
                target.SendCommand(command);
                nuvoTelegram.passDataToTestClass("NUVO_E6D_v1.23");

                Assert.IsTrue(_eventRaisedCount == 1);                                                        // event has been raised 1 times
                Assert.AreEqual(ENuvoEssentiaCommands.ReadVersion, command.Command);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadVersion, _nuvoProtocolEventArgs.Command.Command);   // return same command      
                Assert.AreEqual(command.Guid, _nuvoProtocolEventArgs.Command.Guid);                           // return same instance of command
                Assert.AreEqual("v1.23", _nuvoProtocolEventArgs.Command.FirmwareVersion);
            }

            // Command: ReadStatusCONNECT
            {
                NuvoEssentiaCommand command = new NuvoEssentiaCommand(ENuvoEssentiaCommands.ReadStatusCONNECT);
                target.SendCommand(command);
                nuvoTelegram.passDataToTestClass("Z04PWROFF,SRC3,GRP0,VOL-34");

                Assert.IsTrue(_eventRaisedCount == 2);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, command.Command);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, _nuvoProtocolEventArgs.Command.Command);   // return same command      
                Assert.AreEqual(command.Guid, _nuvoProtocolEventArgs.Command.Guid);                                // return same instance of command
                Assert.AreEqual(ENuvoEssentiaZones.Zone4, _nuvoProtocolEventArgs.Command.ZoneId);
                Assert.AreEqual(ENuvoEssentiaSources.Source3, _nuvoProtocolEventArgs.Command.SourceId);
                Assert.AreEqual(-34, _nuvoProtocolEventArgs.Command.VolumeLevel);
                Assert.AreEqual(EZonePowerStatus.ZoneStatusOFF, _nuvoProtocolEventArgs.Command.PowerStatus);
            }

            // Command: ReadStatusCONNECT
            {
                NuvoEssentiaCommand command = new NuvoEssentiaCommand(ENuvoEssentiaCommands.ReadStatusCONNECT);
                target.SendCommand(command);
                nuvoTelegram.passDataToTestClass("Z12OR0,BASS-10,TREB+02,GRP1,VRST1");    // return value for ReadStatusZONE

                Assert.IsTrue(_eventRaisedCount == 3);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, command.Command);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusZONE, _nuvoProtocolEventArgs.Command.Command);
                Assert.AreNotEqual(command.Guid, _nuvoProtocolEventArgs.Command.Guid);
                Assert.AreEqual(ENuvoEssentiaZones.Zone12, _nuvoProtocolEventArgs.Command.ZoneId);
                Assert.AreEqual(-10, _nuvoProtocolEventArgs.Command.BassLevel);
                Assert.AreEqual(2, _nuvoProtocolEventArgs.Command.TrebleLevel);
            }
        }

        /// <summary>
        /// A test for SendCommand
        /// Send several commands and pass correct answers.
        /// Test that it returns always the corresponding command and the correct values
        /// The commands tested in this unit test don't refer 1:1. This means the incoming command
        /// string can match also other commands.
        /// </summary>
        [TestMethod()]
        public void SendCommand6Test()
        {
            NuvoTelegramMock nuvoTelegram = new NuvoTelegramMock();
            NuvoEssentiaProtocol target = new NuvoEssentiaProtocol(nuvoTelegram);
            target.onCommandReceived += new NuvoEssentiaProtocolEventHandler(serialPort_CommandReceived);

            // Command: MuteON
            {
                NuvoEssentiaCommand command = new NuvoEssentiaCommand(ENuvoEssentiaCommands.MuteON);
                target.SendCommand(command);
                nuvoTelegram.passDataToTestClass("Z11PWRON,SRC6,GRP0,VOL-34");     // return value for ReadStatusCONNECT

                Assert.IsTrue(_eventRaisedCount == 1);
                Assert.AreEqual(ENuvoEssentiaCommands.MuteON, command.Command);
                Assert.AreEqual(ENuvoEssentiaCommands.MuteON, _nuvoProtocolEventArgs.Command.Command);   // return same command      
                Assert.AreEqual(command.Guid, _nuvoProtocolEventArgs.Command.Guid);                                // return same instance of command
                Assert.AreEqual(ENuvoEssentiaZones.Zone11, _nuvoProtocolEventArgs.Command.ZoneId);
                Assert.AreEqual(ENuvoEssentiaSources.Source6, _nuvoProtocolEventArgs.Command.SourceId);
                Assert.AreEqual(-34, _nuvoProtocolEventArgs.Command.VolumeLevel);
                Assert.AreEqual(EZonePowerStatus.ZoneStatusON, _nuvoProtocolEventArgs.Command.PowerStatus);
            }

            // Command: SetBassLevel
            {
                NuvoEssentiaCommand command = new NuvoEssentiaCommand(ENuvoEssentiaCommands.SetBassLevel);
                target.SendCommand(command);
                nuvoTelegram.passDataToTestClass("Z08OR0,BASS+04,TREB+00,GRP1,VRST1");    // return value for ReadStatusZONE

                Assert.IsTrue(_eventRaisedCount == 2);
                Assert.AreEqual(ENuvoEssentiaCommands.SetBassLevel, command.Command);
                Assert.AreEqual(ENuvoEssentiaCommands.SetBassLevel, _nuvoProtocolEventArgs.Command.Command);
                Assert.AreEqual(command.Guid, _nuvoProtocolEventArgs.Command.Guid);
                Assert.AreEqual(ENuvoEssentiaZones.Zone8, _nuvoProtocolEventArgs.Command.ZoneId);
                Assert.AreEqual(4, _nuvoProtocolEventArgs.Command.BassLevel);
                Assert.AreEqual(0, _nuvoProtocolEventArgs.Command.TrebleLevel);
            }
        }

        /// <summary>
        ///A test for NuvoEssentiaProtocol Constructor
        ///</summary>
        [TestMethod()]
        public void NuvoEssentiaProtocolConstructorTest()
        {
            NuvoTelegramMock nuvoTelegram = new NuvoTelegramMock();
            NuvoEssentiaProtocol target = new NuvoEssentiaProtocol(nuvoTelegram);
            target.Open(new SerialPortConnectInformation("COM1"));
            target.Close();
            target = null;
        }

        /// <summary>
        /// A test for compareCommandString. This methods compares the received command string with the configured command string.
        /// The following tests will be done:
        /// - "VER" to "VER" --> true
        /// - "VER" to "VERX" --> false
        /// - "NUVO_E6D_vx.yy" to "NUVO_E6D_v1.23" --> true
        /// - "NUVO_E6D_vx.yy" to "NUVO_E6D_v1,23" --> false
        /// - "NUVO_E6D_vx.yy" to "NUVO-E6D_v1.23" --> false
        /// - "ZxxPWRppp,SRCs,GRPq,VOL-yy" to "Z02PWRON,SRC2,GRP0,VOL-00" --> true
        /// - "ZxxPWRppp,SRCs,GRPq,VOL-yy" to "Z02PWROFF,SRC4,GRP1,VOL-79" --> true
        /// - "ZxxPWRppp,SRCs,GRPq,VOL-yy" to "Z02PWRON,SRC2.GRP0,VOL-00" --> false
        /// - "ZxxPWRppp,SRCs,GRPq,VOL-yy" to "Z02PWROFF,SRC4.GRP1,VOL-79" --> false
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void compareCommandStringTest()
        {
            Assert.AreEqual(true, NuvoEssentiaProtocol.compareCommandString("VER", "VER"));
            Assert.AreEqual(false, NuvoEssentiaProtocol.compareCommandString("VER", "VERX"));
            Assert.AreEqual(true, NuvoEssentiaProtocol.compareCommandString("NUVO_E6D_vx.yy", "NUVO_E6D_v1.23"));
            Assert.AreEqual(false, NuvoEssentiaProtocol.compareCommandString("NUVO_E6D_vx.yy", "NUVO_E6D_v1,23"));
            Assert.AreEqual(false, NuvoEssentiaProtocol.compareCommandString("NUVO_E6D_vx.yy", "NUVO-E6D_v1.23"));
            Assert.AreEqual(true, NuvoEssentiaProtocol.compareCommandString("ZxxPWRppp,SRCs,GRPq,VOL-yy", "Z02PWRON,SRC2,GRP0,VOL-00"));
            Assert.AreEqual(true, NuvoEssentiaProtocol.compareCommandString("ZxxPWRppp,SRCs,GRPq,VOL-yy", "Z02PWROFF,SRC4,GRP1,VOL-79"));
            Assert.AreEqual(false, NuvoEssentiaProtocol.compareCommandString("ZxxPWRppp,SRCs,GRPq,VOL-yy", "Z02PWRON,SRC2.GRP0,VOL-00"));
            Assert.AreEqual(false, NuvoEssentiaProtocol.compareCommandString("ZxxPWRppp,SRCs,GRPq,VOL-yy", "Z02PWROFF,SRC4.GRP1,VOL-79"));

        }
    }
}
