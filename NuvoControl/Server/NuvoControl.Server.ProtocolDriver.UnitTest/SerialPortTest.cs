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


using NuvoControl.Server.ProtocolDriver;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuvoControl.Server.ProtocolDriver.Interface;

namespace NuvoControl.Server.ProtocolDriver.UnitTest
{
    
    
    /// <summary>
    /// Test class for SerialPortTest 
    /// 
    /// It is intended to contain all SerialPortTest Unit Tests
    /// </summary>
    [TestClass()]
    public class SerialPortTest
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
        ///A test for Open
        /// </summary>
        [TestMethod()]
        public void OpenTest()
        {
            SerialPort target = new SerialPort();
            SerialPortConnectInformation serialPortConnectInformation = null; 
            target.Open(serialPortConnectInformation);
            bool actual = target.IsOpen;
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        ///A test for IsOpen
        /// </summary>
        [TestMethod()]
        public void IsOpenTest()
        {
            SerialPort target = new SerialPort();
            bool actual = target.IsOpen;
            Assert.AreEqual(false,actual);
        }

        /// <summary>
        ///A test for Write
        /// </summary>
        [TestMethod()]
        public void WriteTest()
        {
            SerialPort target = new SerialPort();
            string text = string.Empty;
            target.Write(text);
            bool actual = target.IsOpen;
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        ///A test for OpenPort.
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.ProtocolDriver.dll")]
        public void OpenPortTest()
        {
            SerialPort_Accessor target = new SerialPort_Accessor();
            bool actual;
            actual = target.OpenPort();
            Assert.AreEqual(false, actual);
        }

    }
}
