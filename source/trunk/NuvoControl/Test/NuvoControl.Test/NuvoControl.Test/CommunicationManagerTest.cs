using NuvoControl.Test.COMListener;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace NuvoControl.Test
{
    
    
    /// <summary>
    ///This is a test class for CommunicationManagerTest and is intended
    ///to contain all CommunicationManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CommunicationManagerTest
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
        ///A test for ByteToHex
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Test.COMListener.exe")]
        public void ByteToHexTest()
        {
            CommunicationManager_Accessor target = new CommunicationManager_Accessor(); // TODO: Initialize to an appropriate value
            byte[] comByte = { 42, 86, 69, 82, 13 };
            string expected = "2A 56 45 52 0D ";
            string actual;
            actual = target.ByteToHex(comByte);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for HexToByte
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Test.COMListener.exe")]
        public void HexToByteTest()
        {
            CommunicationManager_Accessor target = new CommunicationManager_Accessor(); // TODO: Initialize to an appropriate value
            string msg = "2A 56 45 52 0D";  // = "*VER<CR>"
            byte[] expected = { 42, 86, 69, 82, 13 };
            byte[] actual;
            actual = target.HexToByte(msg);
            int i = 0;
            foreach ( byte b in actual )
            {
                Assert.AreEqual(expected[i], actual[i]);
                Assert.AreEqual(expected[i], b);
                i++;
            }
        }
    }
}
