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
using NuvoControl.Server.ProtocolDriver.Test.Mock;

namespace NuvoControl.Server.ProtocolDriver.Test
{
    
    
    /// <summary>
    /// Test class for NuvoEssentiaProtocolTest 
    /// 
    /// It is intended to contain all NuvoEssentiaProtocolTest Unit Tests
    /// </summary>
    [TestClass()]
    public class NuvoEssentiaProtocolTest
    {
        int _eventRaisedCount = 0;
        ConreteProtocolEventArgs _nuvoProtocolEventArgs = null;

        // callback function for receiving data from telegram layer
        void serialPort_CommandReceived(object sender, ConreteProtocolEventArgs e)
        {
            _eventRaisedCount++;
            _nuvoProtocolEventArgs = e;
        }


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
            _eventRaisedCount = 0;
            _nuvoProtocolEventArgs = null;
        }

        /// <summary>
        /// Use TestCleanup to run code after each test has run
        /// </summary>
        [TestCleanup()]
        public void MyTestCleanup()
        {
        }
        
        #endregion


        /// <summary>
        /// A test for receive command. 
        /// </summary>
        [TestMethod()]
        public void ReceiveCommand1Test()
        {
            NuvoTelegramMock nuvoTelegram = new NuvoTelegramMock();
            NuvoEssentiaProtocol target = new NuvoEssentiaProtocol(1,nuvoTelegram);
            target.onCommandReceived += new ConcreteProtocolEventHandler(serialPort_CommandReceived);

            // Return command for ReadStatusCONNECT
            nuvoTelegram.passDataToTestClass("Z02PWRON,SRC3,GRP0,VOL-50");
            ConreteProtocolEventArgs event1 = _nuvoProtocolEventArgs;  // save return argument
            // Return command for ReadStatusCONNECT
            nuvoTelegram.passDataToTestClass("Z03PWRON,SRC3,GRP0,VOL-50");
            ConreteProtocolEventArgs event2 = _nuvoProtocolEventArgs;  // save return argument

            Assert.IsTrue(_eventRaisedCount == 2);
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, event1.Command.Command);
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, event2.Command.Command);
        }

        /// <summary>
        /// A test for receive command. 
        /// The returned values in case of a 'ALLOFF' command are tested.
        /// </summary>
        [TestMethod()]
        public void ReceiveCommand2Test()
        {
            NuvoTelegramMock nuvoTelegram = new NuvoTelegramMock();
            NuvoEssentiaProtocol target = new NuvoEssentiaProtocol(1, nuvoTelegram);
            target.onCommandReceived += new ConcreteProtocolEventHandler(serialPort_CommandReceived);

            // Return command for ReadStatusCONNECT
            nuvoTelegram.passDataToTestClass("ALLOFF");
            ConreteProtocolEventArgs event1 = _nuvoProtocolEventArgs;  // save return argument

            Assert.IsTrue(_eventRaisedCount == 1);
            Assert.AreEqual(ENuvoEssentiaCommands.TurnALLZoneOFF, event1.Command.Command);
        }

        /// <summary>
        /// A test for SendCommand.
        /// Send a the command 'ReadVersion' and test return value.
        /// </summary>
        [TestMethod()]
        public void SendCommand1Test()
        {
            NuvoTelegramMock nuvoTelegram = new NuvoTelegramMock();
            NuvoEssentiaProtocol target = new NuvoEssentiaProtocol(1, nuvoTelegram);
            target.onCommandReceived += new ConcreteProtocolEventHandler(serialPort_CommandReceived);

            NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadVersion);
            target.SendCommand(command);
            nuvoTelegram.passDataToTestClass("MPU_E6Dvx.yy");

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
            NuvoEssentiaProtocol target = new NuvoEssentiaProtocol(1, nuvoTelegram);
            target.onCommandReceived += new ConcreteProtocolEventHandler(serialPort_CommandReceived);

            target.SendCommand("VER");
            nuvoTelegram.passDataToTestClass("MPU_E6Dvx.yy");

            Assert.IsTrue(_eventRaisedCount == 1);                                                       // event has been raised 1 times
            Assert.AreEqual(ENuvoEssentiaCommands.ReadVersion, _nuvoProtocolEventArgs.Command.Command);   // return same command      
            Assert.AreEqual("VER", nuvoTelegram.Telegram);
        }

        /// <summary>
        /// A test for SendCommand
        /// Send a the command 'ReadVersion' (as string) and test return value.
        /// </summary>
        [TestMethod()]
        public void SendCommand3Test()
        {
            NuvoTelegramMock nuvoTelegram = new NuvoTelegramMock();
            NuvoEssentiaProtocol target = new NuvoEssentiaProtocol(1, nuvoTelegram);
            target.onCommandReceived += new ConcreteProtocolEventHandler(serialPort_CommandReceived);

            target.SendCommand("VER");
            nuvoTelegram.passDataToTestClass("MPU_E6Dv1.23");

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
            NuvoEssentiaProtocol target = new NuvoEssentiaProtocol(1, nuvoTelegram);
            target.onCommandReceived += new ConcreteProtocolEventHandler(serialPort_CommandReceived);

            // Command: ReadVersion
            {
                NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadVersion);
                target.SendCommand(command);
                nuvoTelegram.passDataToTestClass("MPU_E6Dv1.23");

                Assert.IsTrue(_eventRaisedCount == 1);                                                        // event has been raised 1 times
                Assert.AreEqual(ENuvoEssentiaCommands.ReadVersion, command.Command);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadVersion, _nuvoProtocolEventArgs.Command.Command);   // return same command      
                Assert.AreEqual(command.Guid, _nuvoProtocolEventArgs.Command.Guid);                           // return same instance of command
            }

            // Command: ReadStatusCONNECT
            {
                NuvoEssentiaSingleCommand command2 = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadStatusCONNECT, ENuvoEssentiaZones.Zone2);
                target.SendCommand(command2);
                nuvoTelegram.passDataToTestClass("ZxxPWRppp,SRCs,GRPq,VOL-yy");

                Assert.IsTrue(_eventRaisedCount == 2);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, command2.Command);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, _nuvoProtocolEventArgs.Command.Command);   // return same command      
                Assert.AreEqual(command2.Guid, _nuvoProtocolEventArgs.Command.Guid);                                // return same instance of command
                Assert.AreEqual("Z02CONSR", nuvoTelegram.Telegram);
            }

            // Command: ReadStatusCONNECT
            {
                NuvoEssentiaSingleCommand command3 = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadStatusCONNECT,ENuvoEssentiaZones.Zone6);
                target.SendCommand(command3);
                nuvoTelegram.passDataToTestClass("Z06ORp,BASSuuu,TREBttt,GRPq,VRSTr");    // return value for ReadStatusZONE

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
            NuvoEssentiaProtocol target = new NuvoEssentiaProtocol(1, nuvoTelegram);
            target.onCommandReceived += new ConcreteProtocolEventHandler(serialPort_CommandReceived);

            // Command: ReadVersion
            {
                NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadVersion);
                target.SendCommand(command);
                nuvoTelegram.passDataToTestClass("MPU_E6Dv1.23");

                Assert.IsTrue(_eventRaisedCount == 1);                                                        // event has been raised 1 times
                Assert.AreEqual(ENuvoEssentiaCommands.ReadVersion, command.Command);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadVersion, _nuvoProtocolEventArgs.Command.Command);   // return same command      
                Assert.AreEqual(command.Guid, _nuvoProtocolEventArgs.Command.Guid);                           // return same instance of command
                Assert.AreEqual("v1.23", _nuvoProtocolEventArgs.Command.FirmwareVersion);
            }

            // Command: ReadStatusSOURCEIR
            {
                NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadStatusSOURCEIR);
                target.SendCommand(command);
                nuvoTelegram.passDataToTestClass("IRSET:38,56,56,38,38,56");

                Assert.IsTrue(_eventRaisedCount == 2);                                                        // event has been raised 1 times
                Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusSOURCEIR, command.Command);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusSOURCEIR, _nuvoProtocolEventArgs.Command.Command);   // return same command      
                Assert.AreEqual(command.Guid, _nuvoProtocolEventArgs.Command.Guid);                           // return same instance of command
                Assert.AreEqual(EIRCarrierFrequency.IR38kHz, _nuvoProtocolEventArgs.Command.IrCarrierFrequencySource(ENuvoEssentiaSources.Source1));
                Assert.AreEqual(EIRCarrierFrequency.IR38kHz, _nuvoProtocolEventArgs.Command.IrCarrierFrequencySource(ENuvoEssentiaSources.Source4));
                Assert.AreEqual(EIRCarrierFrequency.IR38kHz, _nuvoProtocolEventArgs.Command.IrCarrierFrequencySource(ENuvoEssentiaSources.Source5));
                Assert.AreEqual(EIRCarrierFrequency.IR56kHz, _nuvoProtocolEventArgs.Command.IrCarrierFrequencySource(ENuvoEssentiaSources.Source2));
                Assert.AreEqual(EIRCarrierFrequency.IR56kHz, _nuvoProtocolEventArgs.Command.IrCarrierFrequencySource(ENuvoEssentiaSources.Source3));
                Assert.AreEqual(EIRCarrierFrequency.IR56kHz, _nuvoProtocolEventArgs.Command.IrCarrierFrequencySource(ENuvoEssentiaSources.Source6));
            }

            // Command: ReadStatusCONNECT
            {
                NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadStatusCONNECT,ENuvoEssentiaZones.Zone7);
                target.SendCommand(command);
                nuvoTelegram.passDataToTestClass("Z07PWROFF,SRC3,GRP1,VOL-34");

                Assert.IsTrue(_eventRaisedCount == 3);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, command.Command);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, _nuvoProtocolEventArgs.Command.Command);   // return same command      
                Assert.AreEqual(command.Guid, _nuvoProtocolEventArgs.Command.Guid);                                // return same instance of command
                Assert.AreEqual(ENuvoEssentiaZones.Zone7, _nuvoProtocolEventArgs.Command.ZoneId);
                Assert.AreEqual(ENuvoEssentiaSources.Source3, _nuvoProtocolEventArgs.Command.SourceId);
                Assert.AreEqual(-34, _nuvoProtocolEventArgs.Command.VolumeLevel);
                Assert.AreEqual(EZonePowerStatus.ZoneStatusOFF, _nuvoProtocolEventArgs.Command.PowerStatus);
                Assert.AreEqual(ESourceGroupStatus.SourceGroupON, _nuvoProtocolEventArgs.Command.SourceGrupStatus);
            }

            // Command: ReadStatusCONNECT
            {
                NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadStatusCONNECT, ENuvoEssentiaZones.Zone12);
                target.SendCommand(command);
                nuvoTelegram.passDataToTestClass("Z12OR1,BASS-10,TREB+02,GRP1,VRST0");    // return value for ReadStatusZONE

                Assert.IsTrue(_eventRaisedCount == 4);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, command.Command);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusZONE, _nuvoProtocolEventArgs.Command.Command);
                Assert.AreNotEqual(command.Guid, _nuvoProtocolEventArgs.Command.Guid);
                Assert.AreEqual(ENuvoEssentiaZones.Zone12, _nuvoProtocolEventArgs.Command.ZoneId);
                Assert.AreEqual(-10, _nuvoProtocolEventArgs.Command.BassLevel);
                Assert.AreEqual(2, _nuvoProtocolEventArgs.Command.TrebleLevel);
                Assert.AreEqual(EVolumeResetStatus.VolumeResetOFF, _nuvoProtocolEventArgs.Command.VolumeResetStatus);
                Assert.AreEqual(EDIPSwitchOverrideStatus.DIPSwitchOverrideON, _nuvoProtocolEventArgs.Command.DIPSwitchOverrideStatus);
                Assert.AreEqual(ESourceGroupStatus.SourceGroupON, _nuvoProtocolEventArgs.Command.SourceGrupStatus);
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
            NuvoEssentiaProtocol target = new NuvoEssentiaProtocol(1, nuvoTelegram);
            target.onCommandReceived += new ConcreteProtocolEventHandler(serialPort_CommandReceived);

            // Command: SetSOURCEIR56
            {
                NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.SetSOURCEIR56,ENuvoEssentiaSources.Source3);
                target.SendCommand(command);
                nuvoTelegram.passDataToTestClass("IRSET:56,38,56,38,56,88");

                Assert.IsTrue(_eventRaisedCount == 1);                                                        // event has been raised 1 times
                Assert.AreEqual(ENuvoEssentiaCommands.SetSOURCEIR56, command.Command);
                Assert.AreEqual(ENuvoEssentiaCommands.SetSOURCEIR56, _nuvoProtocolEventArgs.Command.Command);   // return same command      
                Assert.AreEqual(command.Guid, _nuvoProtocolEventArgs.Command.Guid);                           // return same instance of command
                Assert.AreEqual(EIRCarrierFrequency.IR56kHz, _nuvoProtocolEventArgs.Command.IrCarrierFrequencySource(ENuvoEssentiaSources.Source1));
                Assert.AreEqual(EIRCarrierFrequency.IR38kHz, _nuvoProtocolEventArgs.Command.IrCarrierFrequencySource(ENuvoEssentiaSources.Source2));
                Assert.AreEqual(EIRCarrierFrequency.IR56kHz, _nuvoProtocolEventArgs.Command.IrCarrierFrequencySource(ENuvoEssentiaSources.Source3));
                Assert.AreEqual(EIRCarrierFrequency.IR38kHz, _nuvoProtocolEventArgs.Command.IrCarrierFrequencySource(ENuvoEssentiaSources.Source4));
                Assert.AreEqual(EIRCarrierFrequency.IR56kHz, _nuvoProtocolEventArgs.Command.IrCarrierFrequencySource(ENuvoEssentiaSources.Source5));
                Assert.AreEqual(EIRCarrierFrequency.IRUnknown, _nuvoProtocolEventArgs.Command.IrCarrierFrequencySource(ENuvoEssentiaSources.Source6));
            }

            // Command: MuteON
            {
                NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.MuteON, ENuvoEssentiaZones.Zone11);
                target.SendCommand(command);
                nuvoTelegram.passDataToTestClass("Z11PWRON,SRC6,GRP0,VOL-34");     // return value for ReadStatusCONNECT

                Assert.IsTrue(_eventRaisedCount == 2);
                Assert.AreEqual(ENuvoEssentiaCommands.MuteON, command.Command);
                Assert.AreEqual(ENuvoEssentiaCommands.MuteON, _nuvoProtocolEventArgs.Command.Command);   // return same command      
                Assert.AreEqual(command.Guid, _nuvoProtocolEventArgs.Command.Guid);                                // return same instance of command
                Assert.AreEqual(ENuvoEssentiaZones.Zone11, _nuvoProtocolEventArgs.Command.ZoneId);
                Assert.AreEqual(ENuvoEssentiaSources.Source6, _nuvoProtocolEventArgs.Command.SourceId);
                Assert.AreEqual(-34, _nuvoProtocolEventArgs.Command.VolumeLevel);
                Assert.AreEqual(EZonePowerStatus.ZoneStatusON, _nuvoProtocolEventArgs.Command.PowerStatus);
                Assert.AreEqual(ESourceGroupStatus.SourceGroupOFF, _nuvoProtocolEventArgs.Command.SourceGrupStatus);
            }

            // Command: SetBassLevel
            {
                NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.SetBassLevel, ENuvoEssentiaZones.Zone8, 4, 0);
                target.SendCommand(command);
                nuvoTelegram.passDataToTestClass("Z08OR0,BASS+04,TREB+00,GRP0,VRST1");    // return value for ReadStatusZONE

                Assert.IsTrue(_eventRaisedCount == 3);
                Assert.AreEqual(ENuvoEssentiaCommands.SetBassLevel, command.Command);
                Assert.AreEqual(ENuvoEssentiaCommands.SetBassLevel, _nuvoProtocolEventArgs.Command.Command);
                Assert.AreEqual(command.Guid, _nuvoProtocolEventArgs.Command.Guid);
                Assert.AreEqual(ENuvoEssentiaZones.Zone8, _nuvoProtocolEventArgs.Command.ZoneId);
                Assert.AreEqual(4, _nuvoProtocolEventArgs.Command.BassLevel);
                Assert.AreEqual(0, _nuvoProtocolEventArgs.Command.TrebleLevel);
                Assert.AreEqual(EVolumeResetStatus.VolumeResetON, _nuvoProtocolEventArgs.Command.VolumeResetStatus);
                Assert.AreEqual(EDIPSwitchOverrideStatus.DIPSwitchOverrideOFF, _nuvoProtocolEventArgs.Command.DIPSwitchOverrideStatus);
                Assert.AreEqual(ESourceGroupStatus.SourceGroupOFF, _nuvoProtocolEventArgs.Command.SourceGrupStatus);
            }
        }

        /// <summary>
        /// A test for SendCommand
        /// Send a the non-nuvo command 'SetInitialZoneStatus' and test return value.
        /// </summary>
        [TestMethod()]
        public void SendCommand7Test()
        {
            NuvoTelegramMock nuvoTelegram = new NuvoTelegramMock();
            NuvoEssentiaProtocol target = new NuvoEssentiaProtocol(1, nuvoTelegram);
            target.onCommandReceived += new ConcreteProtocolEventHandler(serialPort_CommandReceived);

            NuvoEssentiaCommand command = new NuvoEssentiaCommand(ENuvoEssentiaCommands.SetZoneStatus,ENuvoEssentiaZones.Zone4,ENuvoEssentiaSources.Source4,-50);
            target.SendCommand(command);
            // Three commands => TurnZoneON, SetVolume and SetSource.
            nuvoTelegram.passDataToTestClass("Z04PWRON,SRC1,GRP0,VOL-60");
            Assert.IsTrue(_eventRaisedCount == 1);
            Assert.AreEqual(ENuvoEssentiaCommands.TurnZoneON, _nuvoProtocolEventArgs.Command.Command);
            Assert.AreEqual("Z04ON", _nuvoProtocolEventArgs.Command.OutgoingCommand);

            nuvoTelegram.passDataToTestClass("Z04PWRON,SRC1,GRP0,VOL-50");
            Assert.IsTrue(_eventRaisedCount == 2);
            Assert.AreEqual(ENuvoEssentiaCommands.SetVolume, _nuvoProtocolEventArgs.Command.Command);
            Assert.AreEqual("Z04VOL50", _nuvoProtocolEventArgs.Command.OutgoingCommand);
            
            nuvoTelegram.passDataToTestClass("Z04PWRON,SRC4,GRP0,VOL-50");
            Assert.IsTrue(_eventRaisedCount == 3);
            Assert.AreEqual(ENuvoEssentiaCommands.SetSource, _nuvoProtocolEventArgs.Command.Command);
            Assert.AreEqual("Z04SRC4", _nuvoProtocolEventArgs.Command.OutgoingCommand);

        }


        /// <summary>
        /// A test for SendCommand
        /// A NoComamnd command is sent to send() method. This command is not send to the next layer
        /// and it isn't stored in the running command list.
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void SendCommand8Test()
        {
            NuvoTelegramMock nuvoTelegram = new NuvoTelegramMock();
            NuvoEssentiaProtocol_Accessor target = new NuvoEssentiaProtocol_Accessor(1, nuvoTelegram);
            INuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.NoCommand);

            target.SendCommand(command);

            Assert.AreEqual(0, target._runningCommands.Count);
        }

        /// <summary>
        /// A test for SendCommand. Multiple commands are send to the protcol layer
        /// before getting the answer from Nuvo Essentia. 
        /// It's expected that the commands are queued and processed in the correct order.
        /// </summary>
        [TestMethod()]
        public void SendMultipleCommand1Test()
        {
            NuvoTelegramMock nuvoTelegram = new NuvoTelegramMock();
            NuvoEssentiaProtocol target = new NuvoEssentiaProtocol(1, nuvoTelegram);
            target.onCommandReceived += new ConcreteProtocolEventHandler(serialPort_CommandReceived);

            // Send ReadStatusCONNECT
            NuvoEssentiaSingleCommand command1 = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadStatusCONNECT, ENuvoEssentiaZones.Zone2);
            target.SendCommand(command1);
            // Send ReadStatusZONE
            NuvoEssentiaSingleCommand command2 = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadStatusZONE, ENuvoEssentiaZones.Zone2);
            target.SendCommand(command2);
            // Return command for ReadStatusCONNECT
            nuvoTelegram.passDataToTestClass("Z02PWRON,SRC3,GRP0,VOL-50");
            ConreteProtocolEventArgs event1 = _nuvoProtocolEventArgs;  // save return argument
            // Return command for ReadStatusZONE
            nuvoTelegram.passDataToTestClass("Z02OR0,BASS-00,TREB+00,GRP0,VRST0");
            ConreteProtocolEventArgs event2 = _nuvoProtocolEventArgs;  // save return argument

            Assert.IsTrue(_eventRaisedCount == 2);
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, command1.Command);
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, event1.Command.Command);
            Assert.AreEqual(command1.Guid, event1.Command.Guid); // return same instance of command
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusZONE, command2.Command);
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusZONE, event2.Command.Command);
            Assert.AreEqual(command2.Guid, event2.Command.Guid); // return same instance of command
        }

        /// <summary>
        /// A test for SendCommand. Multiple commands are send to the protcol layer
        /// before getting the answer from Nuvo Essentia. 
        /// It's expected that the commands are queued and processed in the correct order.
        /// </summary>
        [TestMethod()]
        public void SendMultipleCommand2Test()
        {
            NuvoTelegramMock nuvoTelegram = new NuvoTelegramMock();
            NuvoEssentiaProtocol target = new NuvoEssentiaProtocol(1, nuvoTelegram);
            target.onCommandReceived += new ConcreteProtocolEventHandler(serialPort_CommandReceived);

            // Send ReadStatusCONNECT
            NuvoEssentiaSingleCommand command1 = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadStatusCONNECT, ENuvoEssentiaZones.Zone2);
            target.SendCommand(command1);
            // Send ReadStatusZONE
            NuvoEssentiaSingleCommand command2 = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadStatusZONE, ENuvoEssentiaZones.Zone2);
            target.SendCommand(command2);
            // Send ReadStatusCONNECT
            NuvoEssentiaSingleCommand command3 = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadStatusCONNECT, ENuvoEssentiaZones.Zone3);
            target.SendCommand(command3);
            // Send ReadStatusZONE
            NuvoEssentiaSingleCommand command4 = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadStatusZONE, ENuvoEssentiaZones.Zone3);
            target.SendCommand(command4);

            // Return command for ReadStatusCONNECT
            nuvoTelegram.passDataToTestClass("Z02PWRON,SRC3,GRP0,VOL-50");
            ConreteProtocolEventArgs event1 = _nuvoProtocolEventArgs;  // save return argument
            // Return command for ReadStatusZONE
            nuvoTelegram.passDataToTestClass("Z02OR0,BASS-00,TREB+00,GRP0,VRST0");
            ConreteProtocolEventArgs event2 = _nuvoProtocolEventArgs;  // save return argument
            // Return command for ReadStatusCONNECT
            nuvoTelegram.passDataToTestClass("Z03PWRON,SRC3,GRP0,VOL-50");
            ConreteProtocolEventArgs event3 = _nuvoProtocolEventArgs;  // save return argument
            // Return command for ReadStatusZONE
            nuvoTelegram.passDataToTestClass("Z03OR0,BASS-00,TREB+00,GRP0,VRST0");
            ConreteProtocolEventArgs event4 = _nuvoProtocolEventArgs;  // save return argument

            Assert.IsTrue(_eventRaisedCount == 4);
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, command1.Command);
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, event1.Command.Command);
            Assert.AreEqual(command1.Guid, event1.Command.Guid); // return same instance of command
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusZONE, command2.Command);
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusZONE, event2.Command.Command);
            Assert.AreEqual(command2.Guid, event2.Command.Guid); // return same instance of command
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, command3.Command);
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, event3.Command.Command);
            Assert.AreEqual(command3.Guid, event3.Command.Guid); // return same instance of command
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusZONE, command4.Command);
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusZONE, event4.Command.Command);
            Assert.AreEqual(command4.Guid, event4.Command.Guid); // return same instance of command
        }

        /// <summary>
        /// A test for SendCommand. Multiple commands are send to the protcol layer
        /// before getting the answer from Nuvo Essentia. 
        /// One command fails with an error.
        /// </summary>
        [TestMethod()]
        public void SendMultipleCommandWithSomeErrorTest()
        {
            NuvoTelegramMock nuvoTelegram = new NuvoTelegramMock();
            NuvoEssentiaProtocol target = new NuvoEssentiaProtocol(1, nuvoTelegram);
            target.onCommandReceived += new ConcreteProtocolEventHandler(serialPort_CommandReceived);

            // Send ReadStatusCONNECT
            NuvoEssentiaSingleCommand command1 = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadStatusCONNECT, ENuvoEssentiaZones.Zone2);
            target.SendCommand(command1);
            // Send ReadStatusZONE
            NuvoEssentiaSingleCommand command2 = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadStatusZONE, ENuvoEssentiaZones.Zone2);
            target.SendCommand(command2);
            // Send ReadStatusCONNECT
            NuvoEssentiaSingleCommand command3 = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadStatusCONNECT, ENuvoEssentiaZones.Zone3);
            target.SendCommand(command3);
            // Send ReadStatusZONE
            NuvoEssentiaSingleCommand command4 = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadStatusZONE, ENuvoEssentiaZones.Zone3);
            target.SendCommand(command4);

            // Return command for ReadStatusCONNECT
            nuvoTelegram.passDataToTestClass("Z02PWRON,SRC3,GRP0,VOL-50");
            ConreteProtocolEventArgs event1 = _nuvoProtocolEventArgs;  // save return argument
            // Return command for ReadStatusZONE
            nuvoTelegram.passDataToTestClass("?");  // ERROR
            ConreteProtocolEventArgs event2 = _nuvoProtocolEventArgs;  // save return argument
            // Return command for ReadStatusCONNECT
            nuvoTelegram.passDataToTestClass("Z03PWRON,SRC3,GRP0,VOL-50");
            ConreteProtocolEventArgs event3 = _nuvoProtocolEventArgs;  // save return argument
            // Return command for ReadStatusZONE
            nuvoTelegram.passDataToTestClass("Z03OR0,BASS-00,TREB+00,GRP0,VRST0");
            ConreteProtocolEventArgs event4 = _nuvoProtocolEventArgs;  // save return argument

            Assert.IsTrue(_eventRaisedCount == 4);
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, command1.Command);
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, event1.Command.Command);
            Assert.AreEqual(command1.Guid, event1.Command.Guid); // return same instance of command
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusZONE, command2.Command);
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusZONE, event2.Command.Command);
            Assert.AreEqual(command2.Guid, event2.Command.Guid); // return same instance of command
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, command3.Command);
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, event3.Command.Command);
            Assert.AreEqual(command3.Guid, event3.Command.Guid); // return same instance of command
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusZONE, command4.Command);
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusZONE, event4.Command.Command);
            Assert.AreEqual(command4.Guid, event4.Command.Guid); // return same instance of command
        }

        /// <summary>
        /// A test for SendCommand. Multiple commands are send to the protcol layer
        /// before getting the answer from Nuvo Essentia. 
        /// In addition a spontaneous command ir received from Nuvo Essentia.
        /// </summary>
        [TestMethod()]
        public void SendMultipleCommandWithSpontaneousAdditionalAnswers1Test()
        {
            NuvoTelegramMock nuvoTelegram = new NuvoTelegramMock();
            NuvoEssentiaProtocol target = new NuvoEssentiaProtocol(1, nuvoTelegram);
            target.onCommandReceived += new ConcreteProtocolEventHandler(serialPort_CommandReceived);

            // Send ReadStatusCONNECT
            NuvoEssentiaSingleCommand command1 = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadStatusCONNECT, ENuvoEssentiaZones.Zone2);
            target.SendCommand(command1);
            // Send ReadStatusZONE
            NuvoEssentiaSingleCommand command2 = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadStatusZONE, ENuvoEssentiaZones.Zone2);
            target.SendCommand(command2);
            // Send ReadStatusCONNECT
            NuvoEssentiaSingleCommand command3 = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadStatusCONNECT, ENuvoEssentiaZones.Zone3);
            target.SendCommand(command3);
            // Send ReadStatusZONE
            NuvoEssentiaSingleCommand command4 = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadStatusZONE, ENuvoEssentiaZones.Zone3);
            target.SendCommand(command4);

            // Return command for ReadStatusCONNECT
            nuvoTelegram.passDataToTestClass("Z02PWRON,SRC3,GRP0,VOL-50");
            ConreteProtocolEventArgs event1 = _nuvoProtocolEventArgs;  // save return argument
            // Return command for ReadStatusZONE
            nuvoTelegram.passDataToTestClass("Z02OR0,BASS-00,TREB+00,GRP0,VRST0");
            ConreteProtocolEventArgs event2 = _nuvoProtocolEventArgs;  // save return argument
            // Return command for ReadStatusCONNECT
            nuvoTelegram.passDataToTestClass("Z03PWRON,SRC3,GRP0,VOL-50");
            ConreteProtocolEventArgs event3 = _nuvoProtocolEventArgs;  // save return argument
            // Additional command 
            nuvoTelegram.passDataToTestClass("Z06PWRON,SRC6,GRP0,VOL-50");
            ConreteProtocolEventArgs eventx = _nuvoProtocolEventArgs;  // save return argument
            // Return command for ReadStatusZONE
            nuvoTelegram.passDataToTestClass("Z03OR0,BASS-00,TREB+00,GRP0,VRST0");
            ConreteProtocolEventArgs event4 = _nuvoProtocolEventArgs;  // save return argument

            Assert.IsTrue(_eventRaisedCount == 5);
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, command1.Command);
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, event1.Command.Command);
            Assert.AreEqual(command1.Guid, event1.Command.Guid); // return same instance of command
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusZONE, command2.Command);
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusZONE, event2.Command.Command);
            Assert.AreEqual(command2.Guid, event2.Command.Guid); // return same instance of command
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, command3.Command);
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, event3.Command.Command);
            Assert.AreEqual(command3.Guid, event3.Command.Guid); // return same instance of command
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusZONE, command4.Command);
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusZONE, event4.Command.Command);
            Assert.AreEqual(command4.Guid, event4.Command.Guid); // return same instance of command

            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, eventx.Command.Command);
        }

        /// <summary>
        /// A test for SendCommand. Multiple commands are send to the protcol layer
        /// before getting the answer from Nuvo Essentia. 
        /// In addition a spontaneous command is received from Nuvo Essentia, which 
        /// could be the answer to a sent command except the zone differs.
        /// </summary>
        [TestMethod()]
        public void SendMultipleCommandWithSpontaneousAdditionalAnswers2Test()
        {
            NuvoTelegramMock nuvoTelegram = new NuvoTelegramMock();
            NuvoEssentiaProtocol target = new NuvoEssentiaProtocol(1, nuvoTelegram);
            target.onCommandReceived += new ConcreteProtocolEventHandler(serialPort_CommandReceived);

            // Send ReadStatusCONNECT
            NuvoEssentiaSingleCommand command1 = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadStatusCONNECT, ENuvoEssentiaZones.Zone2);
            target.SendCommand(command1);
            // Send ReadStatusCONNECT
            NuvoEssentiaSingleCommand command2 = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadStatusCONNECT, ENuvoEssentiaZones.Zone3);
            target.SendCommand(command2);

            // Return command for ReadStatusCONNECT
            nuvoTelegram.passDataToTestClass("Z02PWRON,SRC3,GRP0,VOL-50");
            ConreteProtocolEventArgs event1 = _nuvoProtocolEventArgs;  // save return argument
            // Additional command 
            nuvoTelegram.passDataToTestClass("Z06PWRON,SRC6,GRP0,VOL-50");
            ConreteProtocolEventArgs eventx = _nuvoProtocolEventArgs;  // save return argument
            // Return command for ReadStatusCONNECT
            nuvoTelegram.passDataToTestClass("Z03PWRON,SRC3,GRP0,VOL-50");
            ConreteProtocolEventArgs event2 = _nuvoProtocolEventArgs;  // save return argument

            Assert.IsTrue(_eventRaisedCount == 3);
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, command1.Command);
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, event1.Command.Command);
            Assert.AreEqual(command1.Guid, event1.Command.Guid); // return same instance of command
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, command2.Command);
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, event2.Command.Command);
            Assert.AreEqual(command2.Guid, event2.Command.Guid); // return same instance of command

            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, eventx.Command.Command);
        }


        /// <summary>
        ///A test for NuvoEssentiaProtocol Constructor
        /// </summary>
        [TestMethod()]
        public void NuvoEssentiaProtocolConstructorTest()
        {
            NuvoTelegramMock nuvoTelegram = new NuvoTelegramMock();
            NuvoEssentiaProtocol target = new NuvoEssentiaProtocol(1, nuvoTelegram);
            target.Open(new SerialPortConnectInformation("COM1"));
            target.Close();
            target = null;
        }

        /// <summary>
        /// A test for compareCommandString. This methods compares the received command string with the configured command string.
        /// The following tests will be done:
        /// - "VER" to "VER" --> true
        /// - "VER" to "VERX" --> false
        /// - "MPU_E6Dvx.yy" to "MPU_E6Dv1.23" --> true
        /// - "MPU_E6Dvx.yy" to "MPU_E6Dv1,23" --> false
        /// - "MPU_E6Dvx.yy" to "NUVO-E6D_v1.23" --> false
        /// - "ZxxPWRppp,SRCs,GRPq,VOL-yy" to "Z02PWRON,SRC2,GRP0,VOL-00" --> true
        /// - "ZxxPWRppp,SRCs,GRPq,VOL-yy" to "Z02PWROFF,SRC4,GRP1,VOL-79" --> true
        /// - "ZxxPWRppp,SRCs,GRPq,VOL-yy" to "Z02PWRON,SRC2.GRP0,VOL-00" --> false
        /// - "ZxxPWRppp,SRCs,GRPq,VOL-yy" to "Z02PWROFF,SRC4.GRP1,VOL-79" --> false
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void compareCommandStringTest()
        {
            Assert.AreEqual(true, NuvoEssentiaSingleCommand.compareCommandString("VER", "VER"));
            Assert.AreEqual(false, NuvoEssentiaSingleCommand.compareCommandString("VER", "VERX"));
            Assert.AreEqual(true, NuvoEssentiaSingleCommand.compareCommandString("MPU_E6Dvx.yy", "MPU_E6Dv1.23"));
            Assert.AreEqual(false, NuvoEssentiaSingleCommand.compareCommandString("MPU_E6Dvx.yy", "MPU_E6Dv1,23"));
            Assert.AreEqual(false, NuvoEssentiaSingleCommand.compareCommandString("MPU_E6Dvx.yy", "NUVO-E6D_v1.23"));
            Assert.AreEqual(true, NuvoEssentiaSingleCommand.compareCommandString("ZxxPWRppp,SRCs,GRPq,VOL-yy", "Z02PWRON,SRC2,GRP0,VOL-00"));
            Assert.AreEqual(true, NuvoEssentiaSingleCommand.compareCommandString("ZxxPWRppp,SRCs,GRPq,VOL-yy", "Z02PWROFF,SRC4,GRP1,VOL-79"));
            Assert.AreEqual(false, NuvoEssentiaSingleCommand.compareCommandString("ZxxPWRppp,SRCs,GRPq,VOL-yy", "Z02PWRON,SRC2.GRP0,VOL-00"));
            Assert.AreEqual(false, NuvoEssentiaSingleCommand.compareCommandString("ZxxPWRppp,SRCs,GRPq,VOL-yy", "Z02PWROFF,SRC4.GRP1,VOL-79"));

        }

    }
}
