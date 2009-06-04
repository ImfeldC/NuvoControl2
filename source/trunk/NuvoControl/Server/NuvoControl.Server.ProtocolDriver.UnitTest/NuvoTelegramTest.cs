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
        bool _eventRaised = false;
        int _eventRaisedCount = 0;
        NuvoTelegramEventArgs _nuvoTelegramEventArgs = null;
        string _eventMessageString = "";

        // callback function for receiving data from telegram layer
        void serialPort_TelegramReceived(object sender, NuvoTelegramEventArgs e)
        {
            _eventRaised = true;
            _eventRaisedCount++;
            _nuvoTelegramEventArgs = e;
            _eventMessageString += e.Message;
        }


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

        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
            _eventRaised = false;
            _eventRaisedCount = 0;
            _nuvoTelegramEventArgs = null;
            _eventMessageString = "";
        }
        
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
        }

        #endregion


        #region onTelegramReceived Unittests

        /// <summary>
        /// Unittest to test the event onTelegramReceived of the telegram layer.
        /// Test: Regular telegram (command is unknown, but not relevant for this test)
        /// Expected: Return 'RET'
        /// </summary>
        [TestMethod()]
        public void onTelegramReceived1Test()
        {
            SerialPortMock serialPort = new SerialPortMock();
            NuvoTelegram target = new NuvoTelegram(serialPort);
            target.Open(new SerialPortConnectInformation("COM1"));
            target.onTelegramReceived += new NuvoTelegramEventHandler(serialPort_TelegramReceived);

            serialPort.passDataToTestClass("#RET\r");
            Assert.IsTrue(_eventRaised);                                        // event has been raised
            Assert.IsTrue(_eventRaisedCount==1);                                // event has been raised just once
            Assert.IsTrue(_nuvoTelegramEventArgs.Message.Length == 3);          // telegram length is 3 (= RET)
            Assert.IsTrue(_nuvoTelegramEventArgs.Message.CompareTo("RET")==0);  // telegram is equal "RET"
        }

        /// <summary>
        /// Unittest to test the event onTelegramReceived of the telegram layer.
        /// Test: Leading characters
        /// Expected: Return 'COMAND'
        /// </summary>
        [TestMethod()]
        public void onTelegramReceived2Test()
        {
            SerialPortMock serialPort = new SerialPortMock();
            NuvoTelegram target = new NuvoTelegram(serialPort);
            target.Open(new SerialPortConnectInformation("COM1"));
            target.onTelegramReceived += new NuvoTelegramEventHandler(serialPort_TelegramReceived);

            serialPort.passDataToTestClass("...#COMAND\r");
            Assert.IsTrue(_eventRaised);                                                // event has been raised
            Assert.IsTrue(_eventRaisedCount == 1);                                      // event has been raised just once
            Assert.IsTrue(_nuvoTelegramEventArgs.Message.Length == 6);                  // telegram length is 7 (= COMAND)
            Assert.IsTrue(_nuvoTelegramEventArgs.Message.CompareTo("COMAND") == 0);     // telegram is equal "COMAND"
        }

        /// <summary>
        /// Unittest to test the event onTelegramReceived of the telegram layer.
        /// Test: Multiple commands received in one package
        /// Expected: Return 'COMANDAAAA' and 'COMANDBB'
        /// </summary>
        [TestMethod()]
        public void onTelegramReceived3Test()
        {
            SerialPortMock serialPort = new SerialPortMock();
            NuvoTelegram target = new NuvoTelegram(serialPort);
            target.Open(new SerialPortConnectInformation("COM1"));
            target.onTelegramReceived += new NuvoTelegramEventHandler(serialPort_TelegramReceived);

            serialPort.passDataToTestClass("...#COMANDAAAA\r...#COMANDBB\r");
            Assert.IsTrue(_eventRaised);                                                // event has been raised
            Assert.IsTrue(_eventRaisedCount == 2);                                      // event has been raised twice
            Assert.IsTrue(_nuvoTelegramEventArgs.Message.CompareTo("COMANDBB") == 0);   // telegram is equal "COMANDBB"
        }

        /// <summary>
        /// Unittest to test the event onTelegramReceived of the telegram layer.
        /// Test: Multiple commands received in several package
        /// Expected: Return 'COMAND1' till 'COMAND6'
        /// </summary>
        [TestMethod()]
        public void onTelegramReceived4Test()
        {
            SerialPortMock serialPort = new SerialPortMock();
            NuvoTelegram target = new NuvoTelegram(serialPort);
            target.Open(new SerialPortConnectInformation("COM1"));
            target.onTelegramReceived += new NuvoTelegramEventHandler(serialPort_TelegramReceived);

            serialPort.passDataToTestClass("...#COMAND1\r#COM");
            Assert.IsTrue(_eventRaisedCount == 1);                                      // event has been raised 1 times
            serialPort.passDataToTestClass("AND2\r#COMAND3\r#COMAN");
            Assert.IsTrue(_eventRaisedCount == 3);                                      // event has been raised 3 times
            serialPort.passDataToTestClass("D4\r....#COMAND5\r...#");
            Assert.IsTrue(_eventRaisedCount == 5);                                      // event has been raised 5 times
            serialPort.passDataToTestClass("COMAND6\r...");
            Assert.IsTrue(_eventRaisedCount == 6);                                      // event has been raised 6 times
            Assert.IsTrue(_nuvoTelegramEventArgs.Message.CompareTo("COMAND6") == 0);    // telegram is equal "COMAND6"
            Assert.IsTrue(_eventMessageString.CompareTo("COMAND1COMAND2COMAND3COMAND4COMAND5COMAND6") == 0);
        }


        /// <summary>
        /// Unittest to test the event onTelegramReceived of the telegram layer.
        /// Test: Receive 'empty' telegram
        /// Expected: Return ''
        /// </summary>
        [TestMethod()]
        public void onTelegramReceived5Test()
        {
            SerialPortMock serialPort = new SerialPortMock();
            NuvoTelegram target = new NuvoTelegram(serialPort);
            target.Open(new SerialPortConnectInformation("COM1"));
            target.onTelegramReceived += new NuvoTelegramEventHandler(serialPort_TelegramReceived);

            serialPort.passDataToTestClass("...#\r...");
            Assert.IsTrue(_eventRaisedCount == 1);                      // event has been raised 1 times
            Assert.IsTrue(_nuvoTelegramEventArgs.Message.Length==0);    // telegram is equal ""
        }


        /// <summary>
        /// Unittest to test the event onTelegramReceived of the telegram layer.
        /// Test: Receive 'empty' telegram
        /// Expected: Return '' and telegram buffer contains 3 charachters
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void onTelegramReceived6Test()
        {
            SerialPortMock serialPort = new SerialPortMock();
            NuvoTelegram_Accessor target = new NuvoTelegram_Accessor(serialPort);
            target.Open(new SerialPortConnectInformation("COM1"));

            serialPort.passDataToTestClass("...#\r...");
            Assert.IsTrue(target._currentTelegramBuffer.Length == 3);   // telegram buffer contains 3 charachters
        }


        /// <summary>
        /// Unittest to test the event onTelegramReceived of the telegram layer.
        /// Test: Receive 'incomplete' telegram (start with '\r')
        /// Expected: Return 'COMAND'
        /// </summary>
        [TestMethod()]
        public void onTelegramReceived7Test()
        {
            SerialPortMock serialPort = new SerialPortMock();
            NuvoTelegram target = new NuvoTelegram(serialPort);
            target.Open(new SerialPortConnectInformation("COM1"));
            target.onTelegramReceived += new NuvoTelegramEventHandler(serialPort_TelegramReceived);

            serialPort.passDataToTestClass("...\r..#COMAND\r.");
            Assert.IsTrue(_eventRaisedCount == 1);                                   // event has been raised 1 times
            Assert.IsTrue(_nuvoTelegramEventArgs.Message.CompareTo("COMAND") == 0);  // telegram is equal "COMAND"
        }


        /// <summary>
        /// Unittest to test the event onTelegramReceived of the telegram layer.
        /// Test: Receive buggy telegram (start with '\r' and several '#' signs)
        /// Expected: Return 'COMAND'
        /// </summary>
        [TestMethod()]
        public void onTelegramReceived8Test()
        {
            SerialPortMock serialPort = new SerialPortMock();
            NuvoTelegram target = new NuvoTelegram(serialPort);
            target.Open(new SerialPortConnectInformation("COM1"));
            target.onTelegramReceived += new NuvoTelegramEventHandler(serialPort_TelegramReceived);

            serialPort.passDataToTestClass("...\r...#....#COMAND\r.");
            Assert.IsTrue(_eventRaisedCount == 1);                                   // event has been raised 1 times
            Assert.IsTrue(_nuvoTelegramEventArgs.Message.CompareTo("COMAND") == 0);  // telegram is equal "COMAND"
        }


        /// <summary>
        /// Unittest to test the event onTelegramReceived of the telegram layer.
        /// Test: Receive 'incomplete' telegram (no start sign '#' has been received)
        /// Expected: Return nothing, internal telegram buffer is empty
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void onTelegramReceived9Test()
        {
            SerialPortMock serialPort = new SerialPortMock();
            NuvoTelegram_Accessor target = new NuvoTelegram_Accessor(serialPort);

            serialPort.passDataToTestClass("...\r.......COMAND\r.");
            serialPort.passDataToTestClass("...\r.......COMAND\r.");
            Assert.IsTrue(target._currentTelegramBuffer.Length == 0);
        }

        #endregion



        /// <summary>
        /// Unittest to test the write method of the telegram layer.
        /// Test: Send regular telegram
        /// Expected: Telegram is send, adding leading #-sign and ending '\r'-sign
        /// </summary>
        [TestMethod()]
        public void Write1Test()
        {
            SerialPortMock serialPort = new SerialPortMock();
            NuvoTelegram target = new NuvoTelegram(serialPort);
            target.Open(new SerialPortConnectInformation("COM1"));

            target.SendTelegram("SendMessage");
            Assert.AreEqual('*', serialPort.WriteText[0]);
            Assert.AreEqual('\r', serialPort.WriteText[serialPort.WriteText.Length - 1]);
            Assert.AreEqual("*SendMessage\r", serialPort.WriteText);
        }


    }
}
