using NuvoControl.Server.ProtocolDriver.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuvoControl.Server.ProtocolDriver;

namespace NuvoControl.Server.ProtocolDriver.Test
{
    
    
    /// <summary>
    ///This is a test class for NuvoEssentiaCommandTest and is intended
    ///to contain all NuvoEssentiaCommandTest Unit Tests
    ///</summary>
    [TestClass()]
    public class NuvoEssentiaCommandTest
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
        /// A test for NuvoEssentiaCommand Constructor, ensure that a spontaneous answer of the
        /// format "ZxxPWRppp,SRCs,GRPq,VOL-yy" is treated as 'Read Status Connect'.
        /// Note: The same answer is also returned for other commands.
        /// </summary>
        [TestMethod()]
        public void NuvoEssentiaCommandConstructor1Test()
        {
            string incomingCommand = "Z02PWROFF,SRC4,GRP0,VOL-50";
            NuvoEssentiaCommand_Accessor target = new NuvoEssentiaCommand_Accessor(incomingCommand);
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
            NuvoEssentiaCommand_Accessor target = new NuvoEssentiaCommand_Accessor(incomingCommand);
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
            string incomingCommand = "IRSET:38,55,38,38,55,55";
            NuvoEssentiaCommand_Accessor target = new NuvoEssentiaCommand_Accessor(incomingCommand);
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusSOURCEIR, target._command);
        }

        /// <summary>
        /// A test for NuvoEssentiaCommand Constructor, ensure that a spontaneous answer of the
        /// format "NUVO_E6D_vz.zz" is treated as 'Read Version'.
        /// Note: The same answer is also returned for other commands.
        /// </summary>
        [TestMethod()]
        public void NuvoEssentiaCommandConstructor4Test()
        {
            string incomingCommand = "NUVO_E6D_v1.23";
            NuvoEssentiaCommand_Accessor target = new NuvoEssentiaCommand_Accessor(incomingCommand);
            Assert.AreEqual(ENuvoEssentiaCommands.ReadVersion, target._command);
        }

        /// <summary>
        /// A test for CompareTo.
        /// In this unit tests the compare method of the Nuvo Essentia command is tested.
        /// </summary>
        [TestMethod()]
        public void CompareToTest()
        {
            NuvoEssentiaCommand commandold = new NuvoEssentiaCommand(ENuvoEssentiaCommands.ReadVersion);
            NuvoEssentiaCommand command    = new NuvoEssentiaCommand(ENuvoEssentiaCommands.ReadVersion);
            NuvoEssentiaCommand commandnew = new NuvoEssentiaCommand(ENuvoEssentiaCommands.ReadVersion);

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
            NuvoEssentiaCommand target = new NuvoEssentiaCommand(ENuvoEssentiaCommands.RampVolumeUP);
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
            NuvoEssentiaCommand target = new NuvoEssentiaCommand(ENuvoEssentiaCommands.RampVolumeUP);
            string actual = target.OutgoingCommandTemplate;
            Assert.IsTrue(actual.CompareTo("ZxxVOL+") == 0);
        }

        /// <summary>
        ///A test for replacePlaceholderForIRFrequency
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void replacePlaceholderForIRFrequencyTest()
        {
            string actual = "";
            NuvoEssentiaCommand_Accessor target = new NuvoEssentiaCommand_Accessor(ENuvoEssentiaCommands.NoCommand);

            actual = target.replacePlaceholderForIRFrequency("IRSET:aa,bb,cc,dd,ee,ff", EIRCarrierFrequency.IRUnknown, "bb");
            Assert.AreEqual("IRSET:aa,bb,cc,dd,ee,ff", actual);

            actual = target.replacePlaceholderForIRFrequency("IRSET:aa,bb,cc,dd,ee,ff", EIRCarrierFrequency.IR38kHz, "bb");
            Assert.AreEqual("IRSET:aa,38,cc,dd,ee,ff", actual);

            actual = target.replacePlaceholderForIRFrequency("IRSET:aa,bb,cc,dd,ee,ff", EIRCarrierFrequency.IR55kHz, "bb");
            Assert.AreEqual("IRSET:aa,55,cc,dd,ee,ff", actual);
        }

        /// <summary>
        ///A test for replacePlaceholderForSource
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void replacePlaceholderForSourceTest()
        {
            string actual = "";
            NuvoEssentiaCommand_Accessor target = new NuvoEssentiaCommand_Accessor(ENuvoEssentiaCommands.NoCommand);

            actual = target.replacePlaceholderForSource("XXXsXXXXsXXXX", ENuvoEssentiaSources.Source2, "s");
            Assert.AreEqual("XXX2XXXX2XXXX", actual);

            actual = target.replacePlaceholderForSource("XXXssXXXXssXXXX", ENuvoEssentiaSources.Source4, "ss");
            Assert.AreEqual("XXX04XXXX04XXXX", actual);

        }

        /// <summary>
        ///A test for replacePlaceholderForZone
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void replacePlaceholderForZoneTest()
        {
            string actual = "";
            NuvoEssentiaCommand_Accessor target = new NuvoEssentiaCommand_Accessor(ENuvoEssentiaCommands.NoCommand);

            actual = target.replacePlaceholderForZone("ZxxORp,BASSyy,TREByy,GRPq,VRSTr", ENuvoEssentiaZones.Zone1, "xx");
            Assert.AreEqual("Z01ORp,BASSyy,TREByy,GRPq,VRSTr", actual);

            actual = target.replacePlaceholderForZone("ZxxORp,BASSyy,TREByy,GRPq,VRSTr", ENuvoEssentiaZones.Zone11, "xx");
            Assert.AreEqual("Z11ORp,BASSyy,TREByy,GRPq,VRSTr", actual);
        }

        /// <summary>
        ///A test for replacePlaceholderForPowerStatus
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void replacePlaceholderForPowerStatusTest()
        {
            string actual = "";
            NuvoEssentiaCommand_Accessor target = new NuvoEssentiaCommand_Accessor(ENuvoEssentiaCommands.NoCommand);

            actual = target.replacePlaceholderForPowerStatus("ZxxPWRppp,SRCs,GRPq,VOL-yy", EZonePowerStatus.ZoneStatusON, "ppp");
            Assert.AreEqual("ZxxPWRON,SRCs,GRPq,VOL-yy", actual);

            actual = target.replacePlaceholderForPowerStatus("ZxxPWRppp,SRCs,GRPq,VOL-yy", EZonePowerStatus.ZoneStatusOFF, "ppp");
            Assert.AreEqual("ZxxPWROFF,SRCs,GRPq,VOL-yy", actual);
        }

        /// <summary>
        ///A test for replacePlaceholderWithVolumeLevel
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void replacePlaceholderWithVolumeLevelTest()
        {
            string actual = "";
            NuvoEssentiaCommand_Accessor target = new NuvoEssentiaCommand_Accessor(ENuvoEssentiaCommands.NoCommand);

            actual = target.replacePlaceholderWithVolumeLevel("ZxxPWRppp,SRCs,GRPq,VOL-yy", -60, "yy");
            Assert.AreEqual("ZxxPWRppp,SRCs,GRPq,VOL-60", actual);

            actual = target.replacePlaceholderWithVolumeLevel("ZxxPWRppp,SRCs,GRPq,VOL-yy", -5, "yy");
            Assert.AreEqual("ZxxPWRppp,SRCs,GRPq,VOL-05", actual);

            actual = target.replacePlaceholderWithVolumeLevel("ZxxVOLyy", -5, "yy");
            Assert.AreEqual("ZxxVOL05", actual);

            actual = target.replacePlaceholderWithVolumeLevel("ZxxVOLyy", -999, "yy");
            Assert.AreEqual("ZxxVOLyy", actual);
        }

        /// <summary>
        ///A test for replacePlaceholderWithBassTrebleLevel
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void replacePlaceholderWithBassTrebleLevelTest()
        {
            string actual = "";
            NuvoEssentiaCommand_Accessor target = new NuvoEssentiaCommand_Accessor(ENuvoEssentiaCommands.NoCommand);

            actual = target.replacePlaceholderWithBassTrebleLevel("ZxxTREBttt", -10, "ttt");
            Assert.AreEqual("ZxxTREB-10", actual);

            actual = target.replacePlaceholderWithBassTrebleLevel("ZxxTREBttt", -12, "ttt");
            Assert.AreEqual("ZxxTREB-12", actual);

            actual = target.replacePlaceholderWithBassTrebleLevel("ZxxTREBttt", 5, "ttt");
            Assert.AreEqual("ZxxTREB+05", actual);

            actual = target.replacePlaceholderWithBassTrebleLevel("ZxxTREBttt", 13, "ttt");
            Assert.AreEqual("ZxxTREBttt", actual);
        }

        /// <summary>
        ///A test for replacePlaceholderWithNumberConsideringPlusMinus
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void replacePlaceholderWithNumberConsideringPlusMinusTest()
        {
            string actual = "";
            NuvoEssentiaCommand_Accessor target = new NuvoEssentiaCommand_Accessor(ENuvoEssentiaCommands.NoCommand);

            actual = target.replacePlaceholderWithNumberConsideringPlusMinus("XXXtttXXX", 10, "ttt");
            Assert.AreEqual("XXX+10XXX", actual);

            actual = target.replacePlaceholderWithNumberConsideringPlusMinus("XXXtttXXX", -10, "ttt");
            Assert.AreEqual("XXX-10XXX", actual);

            actual = target.replacePlaceholderWithNumberConsideringPlusMinus("XXXtttXXX", 5, "ttt");
            Assert.AreEqual("XXX+05XXX", actual);

            actual = target.replacePlaceholderWithNumberConsideringPlusMinus("XXXtttXXX", -5, "ttt");
            Assert.AreEqual("XXX-05XXX", actual);
        }

        /// <summary>
        ///A test for replacePlaceholders
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void replacePlaceholdersTest()
        {
            string actual = "";
            NuvoEssentiaCommand_Accessor target = new NuvoEssentiaCommand_Accessor(ENuvoEssentiaCommands.NoCommand);

            actual = target.replacePlaceholders("IRSET:aa,bb,cc,dd,ee,ff");
            Assert.AreEqual("IRSET:aa,bb,cc,dd,ee,ff", actual);
        }

        /// <summary>
        ///A test for buildOutgoingCommand: Read Version
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void buildOutgoingCommand1Test()
        {
            NuvoEssentiaCommand_Accessor target = new NuvoEssentiaCommand_Accessor(ENuvoEssentiaCommands.ReadVersion);
            string actual = target.buildOutgoingCommand();
            Assert.AreEqual("VER", actual);
        }

        /// <summary>
        ///A test for buildOutgoingCommand: Read Status Connect for Zone 5
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void buildOutgoingCommand2Test()
        {
            NuvoEssentiaCommand_Accessor target = new NuvoEssentiaCommand_Accessor
                (ENuvoEssentiaCommands.ReadStatusCONNECT,ENuvoEssentiaZones.Zone5);
            string actual = target.buildOutgoingCommand();
            Assert.AreEqual("Z05CONSR", actual);
        }

        /// <summary>
        ///A test for buildOutgoingCommand: Set Source for Zone 10 and Source 2
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void buildOutgoingCommand3Test()
        {
            NuvoEssentiaCommand_Accessor target = new NuvoEssentiaCommand_Accessor
                (ENuvoEssentiaCommands.SetSource, ENuvoEssentiaZones.Zone10, ENuvoEssentiaSources.Source2);
            string actual = target.buildOutgoingCommand();
            Assert.AreEqual("Z10SRC2", actual);
        }

        /// <summary>
        ///A test for buildOutgoingCommand: Set Volume for Zone 12 and Volume Level -60
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void buildOutgoingCommand4Test()
        {
            NuvoEssentiaCommand_Accessor target = new NuvoEssentiaCommand_Accessor
                (ENuvoEssentiaCommands.SetVolume, ENuvoEssentiaZones.Zone12,-60);
            string actual = target.buildOutgoingCommand();
            Assert.AreEqual("Z12VOL60", actual);
        }

        /// <summary>
        ///A test for buildOutgoingCommand: Set Bass and Treble Level
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void buildOutgoingCommand5Test()
        {
            {
                NuvoEssentiaCommand_Accessor target = new NuvoEssentiaCommand_Accessor
                    (ENuvoEssentiaCommands.SetBassLevel, ENuvoEssentiaZones.Zone12, -10, 0);    // ignore treble level
                string actual = target.buildOutgoingCommand();
                Assert.AreEqual("Z12BASS-10", actual);
            }

            {
                NuvoEssentiaCommand_Accessor target = new NuvoEssentiaCommand_Accessor
                    (ENuvoEssentiaCommands.SetBassLevel, ENuvoEssentiaZones.Zone12, 5, 0);      // ignore treble level
                string actual = target.buildOutgoingCommand();
                Assert.AreEqual("Z12BASS+05", actual);
            }

            {
                NuvoEssentiaCommand_Accessor target = new NuvoEssentiaCommand_Accessor
                    (ENuvoEssentiaCommands.SetTrebleLevel, ENuvoEssentiaZones.Zone8, 5, 12);   // ignore bass level
                string actual = target.buildOutgoingCommand();
                Assert.AreEqual("Z08TREB+12", actual);
            }

            {
                NuvoEssentiaCommand_Accessor target = new NuvoEssentiaCommand_Accessor
                    (ENuvoEssentiaCommands.SetTrebleLevel, ENuvoEssentiaZones.Zone9, 5, -12);   // ignore bass level
                string actual = target.buildOutgoingCommand();
                Assert.AreEqual("Z09TREB-12", actual);
            }
        }

        /// <summary>
        ///A test for parseCommand
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void parseCommandTest()
        {
            string actual;

            actual = NuvoEssentiaCommand_Accessor.parseCommand("Z02OR0,BASS-08,TREB+10,GRP0,VRST0", "ZxxORp,BASSuuu,TREBttt,GRPq,VRSTr", "xx");
            Assert.AreEqual("02", actual);

            actual = NuvoEssentiaCommand_Accessor.parseCommand("Z02OR0,BASS-08,TREB+10,GRP0,VRST0", "ZxxORp,BASSuuu,TREBttt,GRPq,VRSTr", "uuu");
            Assert.AreEqual("-08", actual);

            actual = NuvoEssentiaCommand_Accessor.parseCommand("Z02OR0,BASS-08,TREB+10,GRP0,VRST0", "ZxxORp,BASSuuu,TREBttt,GRPq,VRSTr", "ttt");
            Assert.AreEqual("+10", actual);

            actual = NuvoEssentiaCommand_Accessor.parseCommand("Z02OR0,BASS-08,TREB+10,GRP0,VRST0", "ZxxORp,BASSuuu,TREBttt,GRPq,VRSTr", "q");
            Assert.AreEqual("0", actual);

            actual = NuvoEssentiaCommand_Accessor.parseCommand("Z02OR0,BASS-08,TREB+10,GRP0,VRST5", "ZxxORp,BASSuuu,TREBttt,GRPq,VRSTr", "r");
            Assert.AreEqual("5", actual);
        }

        /// <summary>
        ///A test for parseCommandForSource
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void parseCommandForSourceTest()
        {
            {
                string incomingCommand = "Z02PWROFF,SRC4,GRP0,VOL-50";
                NuvoEssentiaCommand_Accessor target = new NuvoEssentiaCommand_Accessor(incomingCommand);
                ENuvoEssentiaSources actual = target.parseCommandForSource(incomingCommand);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, target._command);
                Assert.AreEqual(ENuvoEssentiaSources.Source4, actual);
            }
            {
                string incomingCommand = "Z02PWRON,SRC2,GRP0,VOL-50";
                NuvoEssentiaCommand_Accessor target = new NuvoEssentiaCommand_Accessor(incomingCommand);
                ENuvoEssentiaSources actual = target.parseCommandForSource(incomingCommand);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, target._command);
                Assert.AreEqual(ENuvoEssentiaSources.Source2, actual);
            }
        }

        /// <summary>
        ///A test for parseCommandForZone
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void parseCommandForZoneTest()
        {
            string incomingCommand = "Z04PWRON,SRC2,GRP0,VOL-50";
            NuvoEssentiaCommand_Accessor target = new NuvoEssentiaCommand_Accessor(incomingCommand);
            ENuvoEssentiaZones actual = target.parseCommandForZone(incomingCommand);
            Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, target._command);
            Assert.AreEqual(ENuvoEssentiaZones.Zone4, actual);
        }


        /// <summary>
        ///A test for parseCommandForPowerStatus
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void parseCommandForPowerStatusTest()
        {
            {
                string incomingCommand = "Z02PWROFF,SRC4,GRP0,VOL-50";
                NuvoEssentiaCommand_Accessor target = new NuvoEssentiaCommand_Accessor(incomingCommand);
                EZonePowerStatus actual = target.parseCommandForPowerStatus(incomingCommand);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, target._command);
                Assert.AreEqual(EZonePowerStatus.ZoneStatusOFF, actual);
            }
            {
                string incomingCommand = "Z02PWRON,SRC2,GRP0,VOL-50";
                NuvoEssentiaCommand_Accessor target = new NuvoEssentiaCommand_Accessor(incomingCommand);
                EZonePowerStatus actual = target.parseCommandForPowerStatus(incomingCommand);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, target._command);
                Assert.AreEqual(EZonePowerStatus.ZoneStatusON, actual);
            }
            {
                string incomingCommand = "Z02PWRxxx,SRC2,GRP0,VOL-50";
                NuvoEssentiaCommand_Accessor target = new NuvoEssentiaCommand_Accessor(incomingCommand);
                EZonePowerStatus actual = target.parseCommandForPowerStatus(incomingCommand);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, target._command);
                Assert.AreEqual(EZonePowerStatus.ZoneStatusUnknown, actual);
            }
        }

        /// <summary>
        ///A test for parseCommandForBassLevel
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void parseCommandForBassAndTrebleLevelTest()
        {
            {
                string incomingCommand = "Z03OR0,BASS-10,TREB+10,GRP0,VRST0";
                NuvoEssentiaCommand_Accessor target = new NuvoEssentiaCommand_Accessor(incomingCommand);
                int actualBassLevel = target.parseCommandForBassLevel(incomingCommand);
                int actualTrebleLevel = target.parseCommandForTrebleLevel(incomingCommand);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusZONE, target._command);
                Assert.AreEqual(-10, actualBassLevel);
                Assert.AreEqual(10, actualTrebleLevel);
            }
            {
                string incomingCommand = "Z03OR0,BASS+05,TREB-04,GRP0,VRST0";
                NuvoEssentiaCommand_Accessor target = new NuvoEssentiaCommand_Accessor(incomingCommand);
                int actualBassLevel = target.parseCommandForBassLevel(incomingCommand);
                int actualTrebleLevel = target.parseCommandForTrebleLevel(incomingCommand);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusZONE, target._command);
                Assert.AreEqual(5, actualBassLevel);
                Assert.AreEqual(-4, actualTrebleLevel);
            }
        }

        /// <summary>
        ///A test for parseCommandForVolumeLevel
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void parseCommandForVolumeLevelTest()
        {
            {
                string incomingCommand = "Z02PWROFF,SRC4,GRP0,VOL-50";
                NuvoEssentiaCommand_Accessor target = new NuvoEssentiaCommand_Accessor(incomingCommand);
                int actualVolumeLevel = target.parseCommandForVolumeLevel(incomingCommand);
                Assert.AreEqual(ENuvoEssentiaCommands.ReadStatusCONNECT, target._command);
                Assert.AreEqual(-50, actualVolumeLevel);
            }
            {
                string incomingCommand = "Z02PWROFF,SRC4,GRP0,VOL+50";  // invalid command
                NuvoEssentiaCommand_Accessor target = new NuvoEssentiaCommand_Accessor(incomingCommand);
                int actualVolumeLevel = target.parseCommandForVolumeLevel(incomingCommand);
                Assert.AreEqual(ENuvoEssentiaCommands.NoCommand, target._command);
                Assert.AreEqual(-999, actualVolumeLevel);
            }
        }

    }
}
