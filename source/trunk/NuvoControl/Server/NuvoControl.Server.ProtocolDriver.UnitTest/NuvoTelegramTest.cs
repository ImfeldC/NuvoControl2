using NuvoControl.Server.ProtocolDriver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuvoControl.Server.ProtocolDriver.Interface;
using NuvoControl.Server.ProtocolDriver.Test.Mock;

namespace NuvoControl.Server.ProtocolDriver.Test
{
    
    
    /// <summary>
    ///This is a test class for NuvoTelegramTest and is intended
    ///to contain all NuvoTelegramTest Unit Tests
    ///</summary>
    [TestClass()]
    public class NuvoTelegramTest
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
        ///A test for serialPort_DataReceived
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void serialPort_DataReceivedTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            NuvoTelegram_Accessor target = new NuvoTelegram_Accessor(param0); // TODO: Initialize to an appropriate value
            object sender = null; // TODO: Initialize to an appropriate value
            SerialPortEventArgs e = null; // TODO: Initialize to an appropriate value
            target.serialPort_DataReceived(sender, e);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for SendTelegram
        ///</summary>
        [TestMethod()]
        public void SendTelegramTest()
        {
            bool eventRaised = false;
            SerialPortMock serialPort = new SerialPortMock();
            NuvoTelegram target = new NuvoTelegram(serialPort);
            target.onTelegramReceived += delegate(object sender, NuvoTelegramEventArgs e)
            {
                eventRaised = true;
            };

            string telegram = string.Empty; // TODO: Initialize to an appropriate value
            //target.SendTelegram(telegram);
            serialPort.passDataToTestClass("#RET\r");

            Assert.IsTrue(eventRaised);
        }

        /// <summary>
        ///A test for Open
        ///</summary>
        [TestMethod()]
        public void OpenTest()
        {
            ISerialPort serialPort = null; // TODO: Initialize to an appropriate value
            NuvoTelegram target = new NuvoTelegram(serialPort); // TODO: Initialize to an appropriate value
            SerialPortConnectInformation serialPortConnectInformation = null; // TODO: Initialize to an appropriate value
            target.Open(serialPortConnectInformation);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for cutLeadingCharacters
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void cutLeadingCharactersTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            NuvoTelegram_Accessor target = new NuvoTelegram_Accessor(param0); // TODO: Initialize to an appropriate value
            target.cutLeadingCharacters();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for cutAndSendTelegram
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void cutAndSendTelegramTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            NuvoTelegram_Accessor target = new NuvoTelegram_Accessor(param0); // TODO: Initialize to an appropriate value
            target.cutAndSendTelegram();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Close
        ///</summary>
        [TestMethod()]
        public void CloseTest()
        {
            ISerialPort serialPort = null; // TODO: Initialize to an appropriate value
            NuvoTelegram target = new NuvoTelegram(serialPort); // TODO: Initialize to an appropriate value
            target.Close();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for NuvoTelegram Constructor
        ///</summary>
        [TestMethod()]
        public void NuvoTelegramConstructorTest()
        {
            ISerialPort serialPort = null; // TODO: Initialize to an appropriate value
            NuvoTelegram target = new NuvoTelegram(serialPort);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
