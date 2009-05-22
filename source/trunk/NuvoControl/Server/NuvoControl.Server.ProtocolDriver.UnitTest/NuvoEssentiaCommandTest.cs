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
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            NuvoEssentiaCommand_Accessor target = new NuvoEssentiaCommand_Accessor(param0); // TODO: Initialize to an appropriate value
            string command = string.Empty; // TODO: Initialize to an appropriate value
            EIRCarrierFrequency ircf = new EIRCarrierFrequency(); // TODO: Initialize to an appropriate value
            string placeholder = string.Empty; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
 //           actual = target.replacePlaceholderForIRFrequency(command, ircf, placeholder);
 //           Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for replacePlaceholderForSource
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void replacePlaceholderForSourceTest()
        {
            NuvoEssentiaCommand_Accessor target = new NuvoEssentiaCommand_Accessor(ENuvoEssentiaCommands.NoCommand);
            string actual = target.replacePlaceholderForSource("XXXsXXXXsXXXX", ENuvoEssentiaSources.Source2, "s");
            Assert.AreEqual("XXX2XXXX2XXXX", actual);
        }
    }
}
