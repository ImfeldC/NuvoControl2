/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.Service
 *   Author:         Bernhard Limacher
 *   Creation Date:  14.05.2009
 *   File Name:      ConfigurationLoader.cs
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 14.05.2009, Bernhard Limacher: Implementation of the interface.
 * 
 **************************************************************************************************/


using NuvoControl.Server.Service.Configuration;
using NuvoControl.Common.Configuration;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.IO;
using System.Xml;
using System.Drawing;

namespace NuvoControl.Server.Service.UnitTest.Configuration
{
    
    
    /// <summary>
    ///This is a test class for SystemConfigurationTest and is intended
    ///to contain all SystemConfigurationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SystemConfigurationTest
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
        ///A test for Validate
        ///</summary>
        [TestMethod()]
        public void ValidateTest()
        {
            string file = @"NuvoControlKonfiguration.xml";
            NuvoControlConfiguration target = new NuvoControlConfiguration(file);
            Assert.AreEqual(true, target.Validate());
        }


        /// <summary>
        ///A test for Validate
        ///</summary>
        [TestMethod()]
        public void ValidateTestNotDistinctZones()
        {
            string file = @"NuvoControlKonfigurationNotDistinctZones.xml";
            NuvoControlConfiguration target = new NuvoControlConfiguration(file);
            Assert.AreEqual(false, target.Validate());
        }


        /// <summary>
        ///A test for Validate
        ///</summary>
        [TestMethod()]
        public void ValidateTestNotDistinctSources()
        {
            string file = @"NuvoControlKonfigurationNotDistinctSources.xml";
            NuvoControlConfiguration target = new NuvoControlConfiguration(file);
            Assert.AreEqual(false, target.Validate());
        }


        /// <summary>
        ///A test for Validate
        ///</summary>
        [TestMethod()]
        public void ValidateTestInvalidZoneIdInGraphic()
        {
            string file = @"NuvoControlKonfigurationInvalidZoneIdInGraphic.xml";
            NuvoControlConfiguration target = new NuvoControlConfiguration(file);
            Assert.AreEqual(false, target.Validate());
        }


        /// <summary>
        ///A test for Validate
        ///</summary>
        [TestMethod()]
        public void ValidateTestInvalidSourceIdInGraphic()
        {
            string file = @"NuvoControlKonfigurationInvalidSourceIdInGraphic.xml";
            NuvoControlConfiguration target = new NuvoControlConfiguration(file);
            Assert.AreEqual(false, target.Validate());
        }


        /// <summary>
        ///A test for Validate
        ///</summary>
        [TestMethod()]
        public void ValidateTestInvalidZoneIdInSleepFunction()
        {
            string file = @"NuvoControlKonfigurationInvalidZoneIdInSleepFunction.xml";
            NuvoControlConfiguration target = new NuvoControlConfiguration(file);
            Assert.AreEqual(false, target.Validate());
        }


        /// <summary>
        ///A test for Validate
        ///</summary>
        [TestMethod()]
        public void ValidateTestInvalidSourceIdInAlarmFunction()
        {
            string file = @"NuvoControlKonfigurationInvalidSourceIdInAlarmFunction.xml";
            NuvoControlConfiguration target = new NuvoControlConfiguration(file);
            Assert.AreEqual(false, target.Validate());
        }

    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/