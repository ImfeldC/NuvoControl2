﻿/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
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


using NuvoControl.Server.ProtocolDriver.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace NuvoControl.Server.ProtocolDriver.Test
{
    
    
    /// <summary>
    /// Test class for SerialPortFactoryTest 
    /// 
    /// It is intended to contain all SerialPortFactoryTest Unit Tests
    /// </summary>
    [TestClass()]
    public class SerialPortFactoryTest
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

/*
        /// <summary>
        ///A test for LoadDriver
        /// </summary>
        [TestMethod()]
        public void LoadDriverTest()
        {
            //TODO: Implement Unit test for Serial Port Factory
            //string assemblyName = @"E:\ccnet\NuvoControl\NuvoControl\Server\NuvoControl.Server.ProtocolDriver\bin\Debug\NuvoControl.Server.ProtocolDriver.dll";
            string assemblyName = @"NuvoControl.Server.ProtocolDriver.dll";
            string className = "NuvoControl.Server.ProtocolDriver.SerialPort";
            ISerialPort expected = null;
            ISerialPort actual;
            actual = SerialPortFactory.LoadDriver(assemblyName, className);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
 */ 

    }
}
