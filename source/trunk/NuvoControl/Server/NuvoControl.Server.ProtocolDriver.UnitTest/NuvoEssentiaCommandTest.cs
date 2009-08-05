using NuvoControl.Server.ProtocolDriver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace NuvoControl.Server.ProtocolDriver.Test
{
    
    
    /// <summary>
    /// Test class for NuvoEssentiaCommandTest.
    ///
    /// It is intended to contain all NuvoEssentiaCommandTest Unit Tests
    /// </summary>
    [TestClass()]
    public class NuvoEssentiaCommandTest
    {


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
        /// A test for calcVolume2NuvoControl
        /// L  = Laustärke NuvoControl
        /// LE = Laustärke Essentia
        /// L  = ((LE + 79) * 100) / 79
        /// LE = ((L * 79) / 100) - 79
        /// </summary>
        [TestMethod()]
        public void calcVolume2NuvoControlTest()
        {
            Assert.AreEqual(0, NuvoEssentiaCommand.calcVolume2NuvoControl(-79));
            Assert.AreEqual(100, NuvoEssentiaCommand.calcVolume2NuvoControl(0));
            Assert.AreEqual(50, NuvoEssentiaCommand.calcVolume2NuvoControl(-39));
        }

        /// <summary>
        ///A test for calcVolume2NuvoEssentia
        /// </summary>
        [TestMethod()]
        public void calcVolume2NuvoEssentiaTest()
        {
            Assert.AreEqual(-79, NuvoEssentiaCommand.calcVolume2NuvoEssentia(0));
            Assert.AreEqual(0, NuvoEssentiaCommand.calcVolume2NuvoEssentia(100));
            Assert.AreEqual(-39, NuvoEssentiaCommand.calcVolume2NuvoEssentia(50));
        }
    }
}
