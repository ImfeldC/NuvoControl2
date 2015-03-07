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


using NuvoControl.Server.ProtocolDriver.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuvoControl.Server.ProtocolDriver;
using System;
using Common.Logging;
using NuvoControl.Common;

namespace NuvoControl.Server.ProtocolDriver.Test
{
    
    /// <summary>
    /// Test class for NuvoEssentiaCommandTest
    ///
    /// It is intended to contain all NuvoEssentiaCommandTest Unit Tests
    /// </summary>
    [TestClass()]
    public class NuvoEssentiaSingleCommandTest
    {
        #region Common Logger
        /// <summary>
        /// Common logger object. Requires the using directive <c>Common.Logging</c>. See 
        /// <see cref="LogManager"/> for more information.
        /// </summary>
        private ILog _log = LogManager.GetCurrentClassLogger();
        #endregion

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
        /// A test for NuvoEssentiaCommand Constructor, ensure that a spontaneous answer of the
        /// format "ZxxPWRppp,SRCs,GRPq,VOL-yy" is treated as 'Read Status Connect'.
        /// Note: The same answer is also returned for other commands.
        /// </summary>
        [TestMethod()]
        public void NuvoEssentiaCommandConstructor1Test()
        {
            string incomingCommand = "Z02PWROFF,SRC4,GRP0,VOL-50";
            NuvoEssentiaSingleCommand_Accessor target = new NuvoEssentiaSingleCommand_Accessor(incomingCommand);
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, target._command);
        }

        /// <summary>
        /// A test for NuvoEssentiaCommand Constructor, ensure that a spontaneous answer of the
        /// format "ZxxORp,BASSuuu,TREBttt,GRPq,VRSTr" is treated as 'Read Status Zone'.
        /// Note: The same answer is also returned for other commands.
        /// </summary>
        [TestMethod()]
        public void NuvoEssentiaCommandConstructor2Test()
        {
            string incomingCommand = "Z04OR0,BASS+10,TREB-10,GRP0,VRST0";
            NuvoEssentiaSingleCommand_Accessor target = new NuvoEssentiaSingleCommand_Accessor(incomingCommand);
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusZONE, target._command);
        }

        /// <summary>
        /// A test for NuvoEssentiaCommand Constructor, ensure that a spontaneous answer of the
        /// format "IRSET:aa,bb,cc,dd,ee,ff" is treated as 'Read Status SOURCE IR'.
        /// Note: The same answer is also returned for other commands.
        /// </summary>
        [TestMethod()]
        public void NuvoEssentiaCommandConstructor3Test()
        {
            string incomingCommand = "IRSET:38,56,38,38,56,56";
            NuvoEssentiaSingleCommand_Accessor target = new NuvoEssentiaSingleCommand_Accessor(incomingCommand);
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusSOURCEIR, target._command);
        }

        /// <summary>
        /// A test for NuvoEssentiaCommand Constructor, ensure that a spontaneous answer of the
        /// format "MPU_E6Dvz.zz" is treated as 'Read Version'.
        /// Note: The same answer is also returned for other commands.
        /// </summary>
        [TestMethod()]
        public void NuvoEssentiaCommandConstructor4Test()
        {
            string incomingCommand = "MPU_E6Dv1.23";
            NuvoEssentiaSingleCommand_Accessor target = new NuvoEssentiaSingleCommand_Accessor(incomingCommand);
            Assert.AreEqual(ENuvoEssentiaCommands.ReadVersion, target._command);
        }

        /// <summary>
        /// A test for NuvoEssentiaCommand Constructor, ensure that a spontaneous answer of the
        /// format "MPU_E6Dvz.zz" is treated as 'Read Version'.
        /// Note: The same answer is also returned for other commands.
        /// </summary>
        [TestMethod()]
        public void NuvoEssentiaCommandConstructor5Test()
        {
            try
            {
                NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.SetZoneStatus, ENuvoEssentiaZones.Zone4, ENuvoEssentiaSources.Source4);
            }
            catch (ProtocolDriverException ex)
            {
                _log.DebugFormat("Correct, a ProtocolDriverException has to be thrown! {0}", ex.ToString() );
                return;
            }
            Assert.Fail("Wrong, expected exception is not thrown!");
        }

        /// <summary>
        /// A test for CompareTo.
        /// In this unit tests the compare method of the Nuvo Essentia command is tested.
        /// </summary>
        [TestMethod()]
        public void CompareToTest()
        {
            NuvoEssentiaSingleCommand commandold = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadVersion);
            NuvoEssentiaSingleCommand command    = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadVersion);
            NuvoEssentiaSingleCommand commandnew = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadVersion);

            Assert.AreEqual(0, command.CompareTo(command));
            Assert.IsTrue(command.CompareTo(commandold) != 0);
            Assert.IsTrue(command.CompareTo(commandnew) != 0 );

        }

        /// <summary>
        /// A test for IncomingCommand.
        /// Test the configured Incoming command string for a command.
        /// </summary>
        [TestMethod()]
        public void IncomingCommandTest()
        {
            //Properties.Settings.Default.NuvoEssentiaProfileFile = "NuvoEssentiaProfile.xml";
            NuvoEssentiaSingleCommand target = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.RampVolumeUP,ENuvoEssentiaZones.Zone3);
            string actual= target.IncomingCommandTemplate;
            Assert.IsTrue(actual.CompareTo("ZxxPWRppp,SRCs,GRPq,VOL-yy") == 0);
        }

        /// <summary>
        /// A test for OutgoingCommand
        /// Test the configured Outgoing command string for a command.
        /// </summary>
        [TestMethod()]
        public void OutgoingCommandTest()
        {
            NuvoEssentiaSingleCommand target = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.RampVolumeUP,ENuvoEssentiaZones.Zone9);
            Assert.AreEqual("ZxxVOL+", target.OutgoingCommandTemplate);
            Assert.AreEqual("Z09VOL+", target.OutgoingCommand);
        }

        /// <summary>
        ///A test for replacePlaceholderForIRFrequency
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void replacePlaceholderForIRFrequencyTest()
        {
            string actual = "";

            actual = NuvoEssentiaSingleCommand_Accessor.replacePlaceholderForIRFrequency("IRSET:aa,bb,cc,dd,ee,ff", EIRCarrierFrequency.IRUnknown, "bb");
            Assert.AreEqual("IRSET:aa,bb,cc,dd,ee,ff", actual);

            actual = NuvoEssentiaSingleCommand_Accessor.replacePlaceholderForIRFrequency("IRSET:aa,bb,cc,dd,ee,ff", EIRCarrierFrequency.IR38kHz, "bb");
            Assert.AreEqual("IRSET:aa,38,cc,dd,ee,ff", actual);

            actual = NuvoEssentiaSingleCommand_Accessor.replacePlaceholderForIRFrequency("IRSET:aa,bb,cc,dd,ee,ff", EIRCarrierFrequency.IR56kHz, "bb");
            Assert.AreEqual("IRSET:aa,56,cc,dd,ee,ff", actual);
        }

        /// <summary>
        ///A test for replacePlaceholderForSource
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void replacePlaceholderForSourceTest()
        {
            string actual = "";

            actual = NuvoEssentiaSingleCommand_Accessor.replacePlaceholderForSource("XXXsXXXXsXXXX", ENuvoEssentiaSources.Source2, "s");
            Assert.AreEqual("XXX2XXXX2XXXX", actual);

            actual = NuvoEssentiaSingleCommand_Accessor.replacePlaceholderForSource("XXXssXXXXssXXXX", ENuvoEssentiaSources.Source4, "ss");
            Assert.AreEqual("XXX04XXXX04XXXX", actual);

        }

        /// <summary>
        ///A test for replacePlaceholderForZone
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void replacePlaceholderForZoneTest()
        {
            string actual = "";

            actual = NuvoEssentiaSingleCommand_Accessor.replacePlaceholderForZone("ZxxORp,BASSyy,TREByy,GRPq,VRSTr", ENuvoEssentiaZones.Zone1, "xx");
            Assert.AreEqual("Z01ORp,BASSyy,TREByy,GRPq,VRSTr", actual);

            actual = NuvoEssentiaSingleCommand_Accessor.replacePlaceholderForZone("ZxxORp,BASSyy,TREByy,GRPq,VRSTr", ENuvoEssentiaZones.Zone11, "xx");
            Assert.AreEqual("Z11ORp,BASSyy,TREByy,GRPq,VRSTr", actual);
        }

        /// <summary>
        ///A test for replacePlaceholderForPowerStatus
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void replacePlaceholderForPowerStatusTest()
        {
            string actual = "";

            actual = NuvoEssentiaSingleCommand_Accessor.replacePlaceholderForPowerStatus("ZxxPWRppp,SRCs,GRPq,VOL-yy", EZonePowerStatus.ZoneStatusON, "ppp");
            Assert.AreEqual("ZxxPWRON,SRCs,GRPq,VOL-yy", actual);

            actual = NuvoEssentiaSingleCommand_Accessor.replacePlaceholderForPowerStatus("ZxxPWRppp,SRCs,GRPq,VOL-yy", EZonePowerStatus.ZoneStatusOFF, "ppp");
            Assert.AreEqual("ZxxPWROFF,SRCs,GRPq,VOL-yy", actual);
        }

        /// <summary>
        ///A test for replacePlaceholderWithVolumeLevel
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void replacePlaceholderWithVolumeLevelTest()
        {
            string actual = "";

            actual = NuvoEssentiaSingleCommand_Accessor.replacePlaceholderWithVolumeLevel("ZxxPWRppp,SRCs,GRPq,VOL-yy", -60, "yy");
            Assert.AreEqual("ZxxPWRppp,SRCs,GRPq,VOL-60", actual);

            actual = NuvoEssentiaSingleCommand_Accessor.replacePlaceholderWithVolumeLevel("ZxxPWRppp,SRCs,GRPq,VOL-yy", -5, "yy");
            Assert.AreEqual("ZxxPWRppp,SRCs,GRPq,VOL-05", actual);

            actual = NuvoEssentiaSingleCommand_Accessor.replacePlaceholderWithVolumeLevel("ZxxVOLyy", -5, "yy");
            Assert.AreEqual("ZxxVOL05", actual);

            actual = NuvoEssentiaSingleCommand_Accessor.replacePlaceholderWithVolumeLevel("ZxxVOLyy", ZoneState.VALUE_UNDEFINED, "yy");
            Assert.AreEqual("ZxxVOLyy", actual);
        }

        /// <summary>
        ///A test for replacePlaceholderWithBassTrebleLevel
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void replacePlaceholderWithBassTrebleLevelTest()
        {
            string actual = "";

            actual = NuvoEssentiaSingleCommand_Accessor.replacePlaceholderWithBassTrebleLevel("ZxxTREBttt", -10, "ttt");
            Assert.AreEqual("ZxxTREB-10", actual);

            actual = NuvoEssentiaSingleCommand_Accessor.replacePlaceholderWithBassTrebleLevel("ZxxTREBttt", -12, "ttt");
            Assert.AreEqual("ZxxTREB-12", actual);

            actual = NuvoEssentiaSingleCommand_Accessor.replacePlaceholderWithBassTrebleLevel("ZxxTREBttt", 5, "ttt");
            Assert.AreEqual("ZxxTREB+05", actual);

            actual = NuvoEssentiaSingleCommand_Accessor.replacePlaceholderWithBassTrebleLevel("ZxxTREBttt", 13, "ttt");
            Assert.AreEqual("ZxxTREBttt", actual);
        }

        /// <summary>
        ///A test for replacePlaceholderWithNumberConsideringPlusMinus
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void replacePlaceholderWithNumberConsideringPlusMinusTest()
        {
            string actual = "";

            actual = NuvoEssentiaSingleCommand_Accessor.replacePlaceholderWithNumberConsideringPlusMinus("XXXtttXXX", 10, "ttt");
            Assert.AreEqual("XXX+10XXX", actual);

            actual = NuvoEssentiaSingleCommand_Accessor.replacePlaceholderWithNumberConsideringPlusMinus("XXXtttXXX", -10, "ttt");
            Assert.AreEqual("XXX-10XXX", actual);

            actual = NuvoEssentiaSingleCommand_Accessor.replacePlaceholderWithNumberConsideringPlusMinus("XXXtttXXX", 5, "ttt");
            Assert.AreEqual("XXX+05XXX", actual);

            actual = NuvoEssentiaSingleCommand_Accessor.replacePlaceholderWithNumberConsideringPlusMinus("XXXtttXXX", -5, "ttt");
            Assert.AreEqual("XXX-05XXX", actual);
        }

        /// <summary>
        ///A test for replacePlaceholders
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void replacePlaceholdersTest()
        {
            string actual = "";
            EIRCarrierFrequency[] ircf = { EIRCarrierFrequency.IRUnknown, EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR56kHz };
            actual = NuvoEssentiaSingleCommand_Accessor.replacePlaceholders("IRSET:aa,bb,cc,dd,ee,ff", ENuvoEssentiaZones.NoZone, ENuvoEssentiaSources.NoSource, 0, 0, 0, EZonePowerStatus.ZoneStatusUnknown, ircf);
            Assert.AreEqual("IRSET:aa,38,56,dd,ee,ff", actual);
        }

        /// <summary>
        ///A test for buildOutgoingCommand: Read Version
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void buildOutgoingCommand1Test()
        {
            NuvoEssentiaSingleCommand_Accessor target = new NuvoEssentiaSingleCommand_Accessor(ENuvoEssentiaCommands.ReadVersion);
            string actual = target.buildOutgoingCommand();
            Assert.AreEqual("VER", actual);
        }

        /// <summary>
        ///A test for buildOutgoingCommand: Read Status Connect for Zone 5
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void buildOutgoingCommand2Test()
        {
            NuvoEssentiaSingleCommand_Accessor target = new NuvoEssentiaSingleCommand_Accessor
                (ENuvoEssentiaCommands.ReadStatusCONNECT,ENuvoEssentiaZones.Zone5);
            string actual = target.buildOutgoingCommand();
            Assert.AreEqual("Z05CONSR", actual);
        }

        /// <summary>
        ///A test for buildOutgoingCommand: Set Source for Zone 10 and Source 2
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void buildOutgoingCommand3Test()
        {
            NuvoEssentiaSingleCommand_Accessor target = new NuvoEssentiaSingleCommand_Accessor
                (ENuvoEssentiaCommands.SetSource, ENuvoEssentiaZones.Zone10, ENuvoEssentiaSources.Source2);
            string actual = target.buildOutgoingCommand();
            Assert.AreEqual("Z10SRC2", actual);
        }

        /// <summary>
        ///A test for buildOutgoingCommand: Set Volume for Zone 12 and Volume Level -60
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void buildOutgoingCommand4Test()
        {
            NuvoEssentiaSingleCommand_Accessor target = new NuvoEssentiaSingleCommand_Accessor
                (ENuvoEssentiaCommands.SetVolume, ENuvoEssentiaZones.Zone12,-60);
            string actual = target.buildOutgoingCommand();
            Assert.AreEqual("Z12VOL60", actual);
        }

        /// <summary>
        ///A test for buildOutgoingCommand: Set Bass and Treble Level
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void buildOutgoingCommand5Test()
        {
            {
                NuvoEssentiaSingleCommand_Accessor target = new NuvoEssentiaSingleCommand_Accessor
                    (ENuvoEssentiaCommands.SetBassLevel, ENuvoEssentiaZones.Zone12, -10, 0);    // ignore treble level
                string actual = target.buildOutgoingCommand();
                Assert.AreEqual("Z12BASS-10", actual);
            }

            {
                NuvoEssentiaSingleCommand_Accessor target = new NuvoEssentiaSingleCommand_Accessor
                    (ENuvoEssentiaCommands.SetBassLevel, ENuvoEssentiaZones.Zone12, 5, 0);      // ignore treble level
                string actual = target.buildOutgoingCommand();
                Assert.AreEqual("Z12BASS+05", actual);
            }

            {
                NuvoEssentiaSingleCommand_Accessor target = new NuvoEssentiaSingleCommand_Accessor
                    (ENuvoEssentiaCommands.SetTrebleLevel, ENuvoEssentiaZones.Zone8, 5, 12);   // ignore bass level
                string actual = target.buildOutgoingCommand();
                Assert.AreEqual("Z08TREB+12", actual);
            }

            {
                NuvoEssentiaSingleCommand_Accessor target = new NuvoEssentiaSingleCommand_Accessor
                    (ENuvoEssentiaCommands.SetTrebleLevel, ENuvoEssentiaZones.Zone9, 5, -12);   // ignore bass level
                string actual = target.buildOutgoingCommand();
                Assert.AreEqual("Z09TREB-12", actual);
            }
        }

        /// <summary>
        ///A test for parseCommand
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void parseCommandTest()
        {
            string actual;

            actual = NuvoEssentiaSingleCommand_Accessor.parseCommand("Z02OR0,BASS-08,TREB+10,GRP0,VRST0", "ZxxORp,BASSuuu,TREBttt,GRPq,VRSTr", "xx");
            Assert.AreEqual("02", actual);

            actual = NuvoEssentiaSingleCommand_Accessor.parseCommand("Z02OR0,BASS-08,TREB+10,GRP0,VRST0", "ZxxORp,BASSuuu,TREBttt,GRPq,VRSTr", "uuu");
            Assert.AreEqual("-08", actual);

            actual = NuvoEssentiaSingleCommand_Accessor.parseCommand("Z02OR0,BASS-08,TREB+10,GRP0,VRST0", "ZxxORp,BASSuuu,TREBttt,GRPq,VRSTr", "ttt");
            Assert.AreEqual("+10", actual);

            actual = NuvoEssentiaSingleCommand_Accessor.parseCommand("Z02OR0,BASS-08,TREB+10,GRP0,VRST0", "ZxxORp,BASSuuu,TREBttt,GRPq,VRSTr", "q");
            Assert.AreEqual("0", actual);

            actual = NuvoEssentiaSingleCommand_Accessor.parseCommand("Z02OR0,BASS-08,TREB+10,GRP0,VRST5", "ZxxORp,BASSuuu,TREBttt,GRPq,VRSTr", "r");
            Assert.AreEqual("5", actual);
        }

        /// <summary>
        ///A test for parseCommandForSource
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void parseCommandForSourceTest()
        {
            {
                string incomingCommand = "Z02PWROFF,SRC4,GRP0,VOL-50";
                NuvoEssentiaSingleCommand_Accessor target = new NuvoEssentiaSingleCommand_Accessor(incomingCommand);
                ENuvoEssentiaSources actual = NuvoEssentiaSingleCommand_Accessor.parseCommandForSource(incomingCommand, target._incomingCommandTemplate);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, target._command);
                Assert.AreEqual(ENuvoEssentiaSources.Source4, actual);
            }
            {
                string incomingCommand = "Z02PWRON,SRC2,GRP0,VOL-50";
                NuvoEssentiaSingleCommand_Accessor target = new NuvoEssentiaSingleCommand_Accessor(incomingCommand);
                ENuvoEssentiaSources actual = NuvoEssentiaSingleCommand_Accessor.parseCommandForSource(incomingCommand, target._incomingCommandTemplate);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, target._command);
                Assert.AreEqual(ENuvoEssentiaSources.Source2, actual);
            }
        }

        /// <summary>
        ///A test for parseCommandForZone
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void parseCommandForZoneTest()
        {
            string incomingCommand = "Z04PWRON,SRC2,GRP0,VOL-50";
            NuvoEssentiaSingleCommand_Accessor target = new NuvoEssentiaSingleCommand_Accessor(incomingCommand);
            ENuvoEssentiaZones actual = NuvoEssentiaSingleCommand_Accessor.parseCommandForZone(incomingCommand,target._incomingCommandTemplate);
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, target._command);
            Assert.AreEqual(ENuvoEssentiaZones.Zone4, actual);
        }


        /// <summary>
        ///A test for parseCommandForPowerStatus
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void parseCommandForPowerStatusTest()
        {
            {
                string incomingCommand = "Z02PWROFF,SRC4,GRP0,VOL-50";
                NuvoEssentiaSingleCommand_Accessor target = new NuvoEssentiaSingleCommand_Accessor(incomingCommand);
                EZonePowerStatus actual = NuvoEssentiaSingleCommand_Accessor.parseCommandForPowerStatus(incomingCommand,target._incomingCommandTemplate);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, target._command);
                Assert.AreEqual(EZonePowerStatus.ZoneStatusOFF, actual);
            }
            {
                string incomingCommand = "Z02PWRON,SRC2,GRP0,VOL-50";
                NuvoEssentiaSingleCommand_Accessor target = new NuvoEssentiaSingleCommand_Accessor(incomingCommand);
                EZonePowerStatus actual = NuvoEssentiaSingleCommand_Accessor.parseCommandForPowerStatus(incomingCommand,target._incomingCommandTemplate);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, target._command);
                Assert.AreEqual(EZonePowerStatus.ZoneStatusON, actual);
            }
            {
                string incomingCommand = "Z02PWRxxx,SRC2,GRP0,VOL-50";
                NuvoEssentiaSingleCommand_Accessor target = new NuvoEssentiaSingleCommand_Accessor(incomingCommand);
                EZonePowerStatus actual = NuvoEssentiaSingleCommand_Accessor.parseCommandForPowerStatus(incomingCommand,target._incomingCommandTemplate);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, target._command);
                Assert.AreEqual(EZonePowerStatus.ZoneStatusUnknown, actual);
            }
        }

        /// <summary>
        ///A test for parseCommandForBassLevel
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void parseCommandForBassAndTrebleLevelTest()
        {
            {
                string incomingCommand = "Z03OR0,BASS-10,TREB+10,GRP0,VRST0";
                NuvoEssentiaSingleCommand_Accessor target = new NuvoEssentiaSingleCommand_Accessor(incomingCommand);
                int actualBassLevel = NuvoEssentiaSingleCommand_Accessor.parseCommandForBassLevel(incomingCommand, target._incomingCommandTemplate);
                int actualTrebleLevel = NuvoEssentiaSingleCommand_Accessor.parseCommandForTrebleLevel(incomingCommand, target._incomingCommandTemplate);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusZONE, target._command);
                Assert.AreEqual(-10, actualBassLevel);
                Assert.AreEqual(10, actualTrebleLevel);
            }
            {
                string incomingCommand = "Z03OR0,BASS+05,TREB-04,GRP0,VRST0";
                NuvoEssentiaSingleCommand_Accessor target = new NuvoEssentiaSingleCommand_Accessor(incomingCommand);
                int actualBassLevel = NuvoEssentiaSingleCommand_Accessor.parseCommandForBassLevel(incomingCommand,target._incomingCommandTemplate);
                int actualTrebleLevel = NuvoEssentiaSingleCommand_Accessor.parseCommandForTrebleLevel(incomingCommand,target._incomingCommandTemplate);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusZONE, target._command);
                Assert.AreEqual(5, actualBassLevel);
                Assert.AreEqual(-4, actualTrebleLevel);
            }
        }

        /// <summary>
        ///A test for parseCommandForVolumeLevel
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void parseCommandForVolumeLevelTest()
        {
            {
                string incomingCommand = "Z02PWROFF,SRC4,GRP0,VOL-50";
                NuvoEssentiaSingleCommand_Accessor target = new NuvoEssentiaSingleCommand_Accessor(incomingCommand);
                int actualVolumeLevel = NuvoEssentiaSingleCommand_Accessor.parseCommandForVolumeLevel(incomingCommand,target._incomingCommandTemplate);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, target._command);
                Assert.AreEqual(-50, actualVolumeLevel);
            }
            {
                string incomingCommand = "Z02PWROFF,SRC4,GRP0,VOL+50";  // invalid command
                NuvoEssentiaSingleCommand_Accessor target = new NuvoEssentiaSingleCommand_Accessor(incomingCommand);
                int actualVolumeLevel = NuvoEssentiaSingleCommand_Accessor.parseCommandForVolumeLevel(incomingCommand,target._incomingCommandTemplate);
                Assert.AreEqual(ENuvoEssentiaCommands.NoCommand, target._command);
                Assert.AreEqual(ZoneState.VALUE_UNDEFINED, actualVolumeLevel);
                Assert.AreEqual(-999, actualVolumeLevel);
            }
        }


        /// <summary>
        /// A test for checkOutgoingCommand.
        /// We expect that the checkOutgoingCommandTest() method returns false, if the
        /// string passed to the method contains a 'lowercase' character.
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void checkOutgoingCommandTest()
        {
            string emptyCommand = "Z02PWROFF,SRC4,GRP0,VOL-50";
            NuvoEssentiaSingleCommand_Accessor target = new NuvoEssentiaSingleCommand_Accessor(emptyCommand);
            Assert.AreEqual(true, target.checkOutgoingCommand("EEEE"));
            Assert.AreEqual(false, target.checkOutgoingCommand("XXXwWWW"));
            Assert.AreEqual(true, target.checkOutgoingCommand("EEEE11111"));
            Assert.AreEqual(false, target.checkOutgoingCommand("XXXwWWW1111"));
            Assert.AreEqual(true, target.checkOutgoingCommand("EEEE11111_+()"));
            Assert.AreEqual(false, target.checkOutgoingCommand("XXXwWWW1111_+()"));
        }

        /// <summary>
        ///A test for NuvoEssentiaSingleCommand Constructor
        /// </summary>
        [TestMethod()]
        public void NuvoEssentiaSingleCommandConstructorTest()
        {
            NuvoEssentiaSingleCommand target = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.NoCommand);
            Assert.AreEqual(ENuvoEssentiaCommands.NoCommand, target.Command);
            //Assert.IsNull(target.OutgoingCommand);
            Assert.IsNull(target.OutgoingCommandTemplate);
            Assert.IsNull(target.IncomingCommand);
            Assert.IsNull(target.IncomingCommandTemplate);
        }
    }
}
