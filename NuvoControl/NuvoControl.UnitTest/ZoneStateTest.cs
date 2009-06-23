using NuvoControl.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace NuvoControl.UnitTest
{
    
    
    /// <summary>
    /// This is a test class for ZoneStateTest and is intended
    /// to contain all ZoneStateTest Unit Tests
    /// </summary>
    [TestClass()]
    public class ZoneStateTest
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
        /// A test for op_Inequality.
        /// Expected result: Return <c>False</c>, because both values are null.
        /// This means both are the same, so they are NOT unequal.
        /// </summary>
        [TestMethod()]
        public void op_Inequality1Test()
        {
            ZoneState left = null;
            ZoneState right = null;
            bool actual;
            actual = (left != right);
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        /// A test for op_Inequality.
        /// Expected result: Return <c>True</c>, because the left value is NOT null.
        /// This means both are not the same, so they are unequal.
        /// </summary>
        [TestMethod()]
        public void op_Inequality2Test()
        {
            ZoneState left = new ZoneState();
            ZoneState right = null;
            bool actual;
            actual = (left != right);
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        /// A test for op_Inequality.
        /// Expected result: Return <c>True</c>, because the right value is NOT null.
        /// This means both are not the same, so they are unequal.
        /// </summary>
        [TestMethod()]
        public void op_Inequality3Test()
        {
            ZoneState left = null;
            ZoneState right = new ZoneState();
            bool actual;
            actual = (left != right);
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        /// A test for op_Inequality.
        /// Expected result: Return <c>False</c>, because the right and left value are the same.
        /// This means both are the same, so they are NOT unequal.
        /// </summary>
        [TestMethod()]
        public void op_Inequality4Test()
        {
            ZoneState left = new ZoneState();
            ZoneState right = new ZoneState();
            bool actual;
            actual = (left != right);
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        ///A test for op_Equality
        /// Expected result: Return <c>True</c>, because the right and left value are null.
        /// This means both are the same, so they are equal.
        ///</summary>
        [TestMethod()]
        public void op_Equality1Test()
        {
            ZoneState left = new ZoneState();
            ZoneState right = new ZoneState();
            bool actual;
            actual = (left == right);
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        ///A test for op_Equality
        /// Expected result: Return <c>False</c>, because the right and left value are null.
        /// The comparison fails, if one or both parameters are null.
        ///</summary>
        [TestMethod()]
        public void op_Equality2Test()
        {
            ZoneState left = null;
            ZoneState right = null;
            bool actual;
            actual = (left == right);
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        ///A test for op_Equality
        /// Expected result: Return <c>False</c>, because the right value is null.
        /// The comparison fails, if one or both parameters are null.
        ///</summary>
        [TestMethod()]
        public void op_Equality3Test()
        {
            ZoneState left = new ZoneState();
            ZoneState right = null;
            bool actual;
            actual = (left == right);
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        ///A test for op_Equality
        /// Expected result: Return <c>False</c>, because the left value is null.
        /// The comparison fails, if one or both parameters are null.
        ///</summary>
        [TestMethod()]
        public void op_Equality4Test()
        {
            ZoneState left = null;
            ZoneState right = new ZoneState();
            bool actual;
            actual = (left == right);
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        ///A test for op_Equality
        /// Expected result: Return <c>True</c>, because the left and the right value are the same.
        ///</summary>
        [TestMethod()]
        public void op_Equality5Test()
        {
            ZoneState left = new ZoneState();
            ZoneState right = new ZoneState();
            bool actual;
            actual = (left == right);
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        ///A test for op_Equality
        /// Expected result: Return <c>True</c>, because the left and the right value are the same.
        ///</summary>
        [TestMethod()]
        public void op_Equality6Test()
        {
            ZoneState left = new ZoneState(new NuvoControl.Common.Configuration.Address(1, 1), true, 50);
            ZoneState right = new ZoneState(new NuvoControl.Common.Configuration.Address(1, 1), true, 50);
            bool actual;
            actual = (left == right);
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        ///A test for op_Equality
        /// Expected result: Return <c>False</c>, because the left and the right value are NOT the same.
        /// A different address is used.
        ///</summary>
        [TestMethod()]
        public void op_Equality7Test()
        {
            ZoneState left = new ZoneState(new NuvoControl.Common.Configuration.Address(1, 1), true, 50);
            ZoneState right = new ZoneState(new NuvoControl.Common.Configuration.Address(1, 2), true, 50);
            bool actual;
            actual = (left == right);
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        ///A test for op_Equality
        /// Expected result: Return <c>False</c>, because the left and the right value are NOT the same.
        /// A different power status is used.
        ///</summary>
        [TestMethod()]
        public void op_Equality8Test()
        {
            ZoneState left = new ZoneState(new NuvoControl.Common.Configuration.Address(1, 1), true, 50);
            ZoneState right = new ZoneState(new NuvoControl.Common.Configuration.Address(1, 1), false, 50);
            bool actual;
            actual = (left == right);
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        ///A test for op_Equality
        /// Expected result: Return <c>False</c>, because the left and the right value are NOT the same.
        /// A different volume level is used.
        ///</summary>
        [TestMethod()]
        public void op_Equality9Test()
        {
            ZoneState left = new ZoneState(new NuvoControl.Common.Configuration.Address(1, 1), true, 50);
            ZoneState right = new ZoneState(new NuvoControl.Common.Configuration.Address(1, 1), true, 51);
            bool actual;
            actual = (left == right);
            Assert.AreEqual(false, actual);
        }
    }
}
