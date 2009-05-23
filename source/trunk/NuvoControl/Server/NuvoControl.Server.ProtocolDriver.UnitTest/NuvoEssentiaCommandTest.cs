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
            string actual= target.IncomingCommand;
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
            string actual = target.OutgoingCommand;
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
    }
}
