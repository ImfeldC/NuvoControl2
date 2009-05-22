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
            NuvoTelegramMock nuvoTelegram = new NuvoTelegramMock();
            NuvoEssentiaProtocol_Accessor target = new NuvoEssentiaProtocol_Accessor(nuvoTelegram);

            Assert.AreEqual(true, target.compareCommandString("VER", "VER"));
            Assert.AreEqual(false, target.compareCommandString("VER", "VERX"));
            Assert.AreEqual(true, target.compareCommandString("NUVO_E6D_vx.yy", "NUVO_E6D_v1.23"));
            Assert.AreEqual(false, target.compareCommandString("NUVO_E6D_vx.yy", "NUVO_E6D_v1,23"));
            Assert.AreEqual(false, target.compareCommandString("NUVO_E6D_vx.yy", "NUVO-E6D_v1.23"));
            Assert.AreEqual(true, target.compareCommandString("ZxxPWRppp,SRCs,GRPq,VOL-yy", "Z02PWRON,SRC2,GRP0,VOL-00"));
            Assert.AreEqual(true, target.compareCommandString("ZxxPWRppp,SRCs,GRPq,VOL-yy", "Z02PWROFF,SRC4,GRP1,VOL-79"));
            Assert.AreEqual(false, target.compareCommandString("ZxxPWRppp,SRCs,GRPq,VOL-yy", "Z02PWRON,SRC2.GRP0,VOL-00"));
            Assert.AreEqual(false, target.compareCommandString("ZxxPWRppp,SRCs,GRPq,VOL-yy", "Z02PWROFF,SRC4.GRP1,VOL-79"));

        }
    }
}
