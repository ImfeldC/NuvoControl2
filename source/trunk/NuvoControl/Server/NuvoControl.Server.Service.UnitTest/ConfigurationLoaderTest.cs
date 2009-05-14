/**************************************************************************************************
 * 
 *   Copyright (C) Siemens AG 2006 All Rights Reserved. Confidential
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
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;
using System.Xml;

namespace NuvoControl.Server.Service.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for ConfigurationLoaderTest and is intended
    ///to contain all ConfigurationLoaderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ConfigurationLoaderTest
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
        ///A test for ConfigurationLoader Constructor with wellformed XML file
        ///</summary>
        [TestMethod()]
        public void ConfigurationLoaderConstructorTestWellFormedXml()
        {
            string file = @"..\..\..\Config\NuvoControlKonfiguration.xml";
            ConfigurationLoader target = new ConfigurationLoader(file);
            Assert.IsTrue(true, "Testing the successful instntiation of the XML Document.");
        }


        /// <summary>
        ///A test for ConfigurationLoader Constructor with wrong XML file path
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(DirectoryNotFoundException), "The directory exists inappropriately.")]
        public void ConfigurationLoaderConstructorTestWrongXmlFilePath()
        {
            string file = @"..\..\..\Confi\NuvoControlKonfiguration.xml";
            ConfigurationLoader target = new ConfigurationLoader(file);
        }


        /// <summary>
        ///A test for ConfigurationLoader Constructor with wrong XML file name
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(FileNotFoundException), "The XML Configuration file exists inappropriately.")]
        public void ConfigurationLoaderConstructorTestWrongXmlFileName()
        {
            string file = @"..\..\..\Config\NuvoControlKonfigur.xml";
            ConfigurationLoader target = new ConfigurationLoader(file);
        }


        /// <summary>
        ///A test for ConfigurationLoader Constructor with not well formed XML file
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(XmlException), "The file exists inappropriately.")]
        public void ConfigurationLoaderConstructorTestNotWellFormedXml()
        {
            string file = @"..\..\..\Config\NuvoControlKonfigurationNotWellFormed.xml";
            ConfigurationLoader target = new ConfigurationLoader(file);
        }


        /// <summary>
        ///A test for Validate
        ///</summary>
        [TestMethod()]
        public void ValidateTest()
        {
            string file = @"..\..\..\Config\NuvoControlKonfiguration.xml";
            ConfigurationLoader target = new ConfigurationLoader(file);
            bool expected = false;
            bool actual;
            actual = target.Validate();
            Assert.AreEqual(expected, actual);
        }
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/