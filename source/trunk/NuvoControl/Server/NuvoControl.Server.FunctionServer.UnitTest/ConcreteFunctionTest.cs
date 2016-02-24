using NuvoControl.Server.FunctionServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using NuvoControl.Common.Configuration;
using NuvoControl.Common;
using NuvoControl.Server.ZoneServer;
using System.Collections.Generic;
using NuvoControl.Server.ProtocolDriver.Interface;

namespace NuvoControl.Server.FunctionServer.UnitTest
{

    class FunctionTestClass : Function
    {
    }

    class ConcreteFunctionTestClass : ConcreteFunction
    {
        private Function _function;

        public ConcreteFunctionTestClass(IZoneServer zoneServer, Function function, Dictionary<int, IAudioDriver> audioDrivers):
            base(zoneServer, function, audioDrivers)
        {
            _function = function;
        }

        protected override void notifyOnZoneUpdate(ZoneStateEventArgs e)
        {
            throw new NotImplementedException();
        }

        public override Function Function
        {
            get { return _function; }
        }

        public override void calculateFunction(DateTime aktTime)
        {
            throw new NotImplementedException();
        }
    }

    
    /// <summary>
    ///This is a test class for ConcreteFunctionTest and is intended
    ///to contain all ConcreteFunctionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ConcreteFunctionTest
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


        internal virtual ConcreteFunction CreateConcreteFunction()
        {
            IZoneServer zoneServer = null;
            Function function = new FunctionTestClass();
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteFunction target = new ConcreteFunctionTestClass(zoneServer, function, audioDrivers);
            return target;
        }

        /// <summary>
        ///A test for Function
        ///</summary>
        [TestMethod()]
        public void FunctionTest()
        {
            ConcreteFunction target = CreateConcreteFunction();
            Function actual;
            actual = target.Function;
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for Active
        ///</summary>
        [TestMethod()]
        public void ActiveTest()
        {
            ConcreteFunction target = CreateConcreteFunction();
            bool actual;
            actual = target.Active;
            Assert.AreEqual(true, actual);
        }


/*
        internal virtual ConcreteFunction_Accessor CreateConcreteFunction_Accessor()
        {
            // TODO: Instantiate an appropriate concrete class.
            ConcreteFunction_Accessor target = null;
            return target;
        }

        /// <summary>
        ///A test for unsubscribeZone
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.FunctionServer.dll")]
        public void unsubscribeZoneTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ConcreteFunction_Accessor target = new ConcreteFunction_Accessor(param0); // TODO: Initialize to an appropriate value
            Address zoneId = null; // TODO: Initialize to an appropriate value
            target.unsubscribeZone(zoneId);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for subscribeZone
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.FunctionServer.dll")]
        public void subscribeZoneTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ConcreteFunction_Accessor target = new ConcreteFunction_Accessor(param0); // TODO: Initialize to an appropriate value
            Address zoneId = null; // TODO: Initialize to an appropriate value
            target.subscribeZone(zoneId);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for onValidityStart
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.FunctionServer.dll")]
        public void onValidityStartTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ConcreteFunction_Accessor target = new ConcreteFunction_Accessor(param0); // TODO: Initialize to an appropriate value
            target.onValidityStart();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for onValidityEnd
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.FunctionServer.dll")]
        public void onValidityEndTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ConcreteFunction_Accessor target = new ConcreteFunction_Accessor(param0); // TODO: Initialize to an appropriate value
            target.onValidityEnd();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for onFunctionStart
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.FunctionServer.dll")]
        public void onFunctionStartTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ConcreteFunction_Accessor target = new ConcreteFunction_Accessor(param0); // TODO: Initialize to an appropriate value
            target.onFunctionStart();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for onFunctionEvent
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.FunctionServer.dll")]
        public void onFunctionEventTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ConcreteFunction_Accessor target = new ConcreteFunction_Accessor(param0); // TODO: Initialize to an appropriate value
            eCommandType commandType = new eCommandType(); // TODO: Initialize to an appropriate value
            target.onFunctionEvent(commandType);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for onFunctionError
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.FunctionServer.dll")]
        public void onFunctionErrorTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ConcreteFunction_Accessor target = new ConcreteFunction_Accessor(param0); // TODO: Initialize to an appropriate value
            target.onFunctionError();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for onFunctionEnd
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.FunctionServer.dll")]
        public void onFunctionEndTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ConcreteFunction_Accessor target = new ConcreteFunction_Accessor(param0); // TODO: Initialize to an appropriate value
            target.onFunctionEnd();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for notifyOnZoneUpdate
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.FunctionServer.dll")]
        public void notifyOnZoneUpdateTest()
        {
            // Private Accessor for notifyOnZoneUpdate is not found. Please rebuild the containing project or run the Publicize.exe manually.
            Assert.Inconclusive("Private Accessor for notifyOnZoneUpdate is not found. Please rebuild the containi" +
                    "ng project or run the Publicize.exe manually.");
        }

        /// <summary>
        ///A test for isFunctionActiveToday
        ///</summary>
        [TestMethod()]
        public void isFunctionActiveTodayTest()
        {
            ConcreteFunction target = CreateConcreteFunction(); // TODO: Initialize to an appropriate value
            DateTime aktTime = new DateTime(); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.isFunctionActiveToday(aktTime);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for isFunctionActiveRightNow
        ///</summary>
        [TestMethod()]
        public void isFunctionActiveRightNowTest()
        {
            ConcreteFunction target = CreateConcreteFunction(); // TODO: Initialize to an appropriate value
            DateTime aktTime = new DateTime(); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.isFunctionActiveRightNow(aktTime);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for instantiateConcreteCommands
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.FunctionServer.dll")]
        public void instantiateConcreteCommandsTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ConcreteFunction_Accessor target = new ConcreteFunction_Accessor(param0); // TODO: Initialize to an appropriate value
            target.instantiateConcreteCommands();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for calculateZoneChangeToON
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.FunctionServer.dll")]
        public void calculateZoneChangeToONTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ConcreteFunction_Accessor target = new ConcreteFunction_Accessor(param0); // TODO: Initialize to an appropriate value
            DateTime lastZoneChangeToOn = new DateTime(); // TODO: Initialize to an appropriate value
            ZoneState oldState = null; // TODO: Initialize to an appropriate value
            ZoneState newState = null; // TODO: Initialize to an appropriate value
            DateTime expected = new DateTime(); // TODO: Initialize to an appropriate value
            DateTime actual;
            actual = target.calculateZoneChangeToON(lastZoneChangeToOn, oldState, newState);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for calculateFunction
        ///</summary>
        [TestMethod()]
        public void calculateFunctionTest()
        {
            ConcreteFunction target = CreateConcreteFunction(); // TODO: Initialize to an appropriate value
            DateTime aktTime = new DateTime(); // TODO: Initialize to an appropriate value
            target.calculateFunction(aktTime);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for OnZoneNotification
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.FunctionServer.dll")]
        public void OnZoneNotificationTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            ConcreteFunction_Accessor target = new ConcreteFunction_Accessor(param0); // TODO: Initialize to an appropriate value
            object sender = null; // TODO: Initialize to an appropriate value
            ZoneStateEventArgs e = null; // TODO: Initialize to an appropriate value
            target.OnZoneNotification(sender, e);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Dispose
        ///</summary>
        [TestMethod()]
        public void DisposeTest()
        {
            ConcreteFunction target = CreateConcreteFunction(); // TODO: Initialize to an appropriate value
            target.Dispose();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
 */
 
    }
}
