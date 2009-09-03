using NuvoControl.Server.FunctionServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuvoControl.Common.Configuration;
using NuvoControl.Server.ZoneServer;
using System;

namespace NuvoControl.Server.FunctionServer.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for ConcreteSleepFunctionTest and is intended
    ///to contain all ConcreteSleepFunctionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ConcreteSleepFunctionTest
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
        /// A test for ConcreteSleepFunction Constructor.
        /// Test is fullfilled if no exception has been raised.
        /// </summary>
        [TestMethod()]
        public void ConcreteSleepFunctionConstructorTest1()
        {
            SleepFunction function = new SleepFunction();
            IZoneServer zoneServer = null; 
            ConcreteSleepFunction target = new ConcreteSleepFunction(function, zoneServer);
        }

        /// <summary>
        /// A test for ConcreteSleepFunction Constructor.
        /// Test is fullfilled if no exception has been raised.
        /// </summary>
        [TestMethod()]
        public void ConcreteSleepFunctionConstructorTest2()
        {
            try
            {
                SleepFunction function = null;
                IZoneServer zoneServer = null;
                ConcreteSleepFunction target = new ConcreteSleepFunction(function, zoneServer);
            }
            catch (FunctionServerException)
            {
                // ok, catch expection
                return;   
            }
            Assert.Fail("Exception 'FunctionServerException' expected!");
        }


        /// <summary>
        /// A test for isFunctionActiveRightNow.
        /// Test the range over midnight. The function is active from 23:00 till 03:00.
        /// </summary>
        [TestMethod()]
        public void isFunctionActiveRightNowTest1()
        {
            SleepFunction function = new SleepFunction(Guid.NewGuid(), new Address(100,1),
                new TimeSpan(1,0,0),new TimeSpan(23,0,0), new TimeSpan(3,0,0));
            IZoneServer zoneServer = null;
            ConcreteSleepFunction target = new ConcreteSleepFunction(function, zoneServer);
            bool actual = false;

            // (a) Function is not active, current time is outside the validFrom/validTo range
            actual = target.isFunctionActiveRightNow( new DateTime(2000, 1, 1, 10, 0, 0) );
            Assert.AreEqual(false, actual);

            // (b) Function is not active, current time is outside the validFrom/validTo range
            actual = target.isFunctionActiveRightNow(new DateTime(2000, 1, 1, 3, 0, 1));
            Assert.AreEqual(false, actual);

            // (c) Function is not active, current time is outside the validFrom/validTo range
            actual = target.isFunctionActiveRightNow(new DateTime(2000, 1, 1, 22, 59, 59));
            Assert.AreEqual(false, actual);

            // (d) Function is active, current time is inside the validFrom/validTo range
            actual = target.isFunctionActiveRightNow(new DateTime(2000, 1, 1, 23, 0, 10));
            Assert.AreEqual(true, actual);

            // (e) Function is active, current time is inside the validFrom/validTo range
            actual = target.isFunctionActiveRightNow(new DateTime(2000, 1, 1, 23, 59, 59));
            Assert.AreEqual(true, actual);

            // (f) Function is active, current time is inside the validFrom/validTo range
            actual = target.isFunctionActiveRightNow(new DateTime(2000, 1, 1, 0, 0, 0));
            Assert.AreEqual(true, actual);

            // (g) Function is active, current time is inside the validFrom/validTo range
            actual = target.isFunctionActiveRightNow(new DateTime(2000, 1, 1, 23, 0, 0));
            Assert.AreEqual(true, actual);

            // (h) Function is active, current time is inside the validFrom/validTo range
            actual = target.isFunctionActiveRightNow(new DateTime(2000, 1, 1, 3, 0, 0));
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        /// A test for isFunctionActiveRightNow.
        /// Test the range over midnight. The function is active from 06:00 till 09:00.
        /// </summary>
        [TestMethod()]
        public void isFunctionActiveRightNowTest2()
        {
            SleepFunction function = new SleepFunction(Guid.NewGuid(), new Address(100, 1),
                new TimeSpan(1, 0, 0), new TimeSpan(6, 0, 0), new TimeSpan(9, 0, 0));
            IZoneServer zoneServer = null;
            ConcreteSleepFunction target = new ConcreteSleepFunction(function, zoneServer);
            bool actual = false;

            // (a) Function is not active, current time is outside the validFrom/validTo range
            actual = target.isFunctionActiveRightNow(new DateTime(2000, 1, 1, 10, 0, 0));
            Assert.AreEqual(false, actual);

            // (b) Function is not active, current time is outside the validFrom/validTo range
            actual = target.isFunctionActiveRightNow(new DateTime(2000, 1, 1, 9, 0, 1));
            Assert.AreEqual(false, actual);

            // (c) Function is not active, current time is outside the validFrom/validTo range
            actual = target.isFunctionActiveRightNow(new DateTime(2000, 1, 1, 5, 59, 59));
            Assert.AreEqual(false, actual);

            // (d) Function is active, current time is inside the validFrom/validTo range
            actual = target.isFunctionActiveRightNow(new DateTime(2000, 1, 1, 6, 0, 10));
            Assert.AreEqual(true, actual);

            // (e) Function is active, current time is inside the validFrom/validTo range
            actual = target.isFunctionActiveRightNow(new DateTime(2000, 1, 1, 8, 59, 59));
            Assert.AreEqual(true, actual);

            // (f) Function is not active, current time is outside the validFrom/validTo range
            actual = target.isFunctionActiveRightNow(new DateTime(2000, 1, 1, 0, 0, 0));
            Assert.AreEqual(false, actual);

            // (g) Function is active, current time is inside the validFrom/validTo range
            actual = target.isFunctionActiveRightNow(new DateTime(2000, 1, 1, 6, 0, 0));
            Assert.AreEqual(true, actual);

            // (h) Function is active, current time is inside the validFrom/validTo range
            actual = target.isFunctionActiveRightNow(new DateTime(2000, 1, 1, 9, 0, 0));
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        /// A test for notifyOnZoneUpdate
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.FunctionServer.dll")]
        public void notifyOnZoneUpdateTest()
        {
            SleepFunction function = new SleepFunction();
            IZoneServer zoneServer = null;
            ConcreteSleepFunction_Accessor target = new ConcreteSleepFunction_Accessor(function,zoneServer);
            
            // notify the first time, the update time is set to the current update time            
            ZoneStateEventArgs e1 = new ZoneStateEventArgs( new NuvoControl.Common.ZoneState( new Address(100,1), true, 20, NuvoControl.Common.ZoneQuality.Online));
            target.notifyOnZoneUpdate(e1);
            Assert.AreEqual(e1.ZoneState.LastUpdate, target._lastZoneChangeToON);

            // notify volume change, the last zone change to On time stays the same
            ZoneStateEventArgs e2 = new ZoneStateEventArgs(new NuvoControl.Common.ZoneState(new Address(100, 1), true, 30, NuvoControl.Common.ZoneQuality.Online));
            target.notifyOnZoneUpdate(e2);
            Assert.AreEqual(e1.ZoneState.LastUpdate, target._lastZoneChangeToON);

            // notify switch off, the last zone change to On time stays the same
            ZoneStateEventArgs e3 = new ZoneStateEventArgs(new NuvoControl.Common.ZoneState(new Address(100, 1), false, 30, NuvoControl.Common.ZoneQuality.Online));
            target.notifyOnZoneUpdate(e3);
            Assert.AreEqual(e1.ZoneState.LastUpdate, target._lastZoneChangeToON);

            // notify switch on, the last zone change to On time updates
            ZoneStateEventArgs e4 = new ZoneStateEventArgs(new NuvoControl.Common.ZoneState(new Address(100, 1), true, 30, NuvoControl.Common.ZoneQuality.Online));
            target.notifyOnZoneUpdate(e4);
            Assert.AreEqual(e4.ZoneState.LastUpdate, target._lastZoneChangeToON);

        }


        /// <summary>
        /// A test for calculateFunction
        /// We expect an exception, because the actual time is higher than the last zone update
        /// </summary>
        [TestMethod()]
        public void calculateFunctionTest1()
        {
            try
            {
                SleepFunction function = new SleepFunction();
                IZoneServer zoneServer = null;
                ConcreteSleepFunction target = new ConcreteSleepFunction(function, zoneServer);
                //target.notifyOnZoneUpdate();
                DateTime aktTime = new DateTime();
                target.calculateFunction(aktTime);
            }
            catch (FunctionServerException)
            {
                // ok, catch expection
                return;
            }
            Assert.Fail("Exception 'FunctionServerException' expected!");
        }

        /// <summary>
        /// A test for calculateFunction
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.FunctionServer.dll")]
        public void calculateFunctionTest2()
        {
            SleepFunction function = new SleepFunction(Guid.NewGuid(), new Address(100,1),
                new TimeSpan(1,0,0),new TimeSpan(0,0,0), new TimeSpan(23,59,59));
            ZoneServerMock zoneServer = new ZoneServerMock();
            ConcreteSleepFunction_Accessor target = new ConcreteSleepFunction_Accessor(function, zoneServer);

            // notify the first time, the update time is set to the current update time            
            ZoneStateEventArgs e1 = new ZoneStateEventArgs(new NuvoControl.Common.ZoneState(new Address(100, 1), true, 20, NuvoControl.Common.ZoneQuality.Online));
            target.notifyOnZoneUpdate(e1);
            Assert.AreEqual(e1.ZoneState.LastUpdate, target._lastZoneChangeToON);

            DateTime aktTime = DateTime.Now;
            target.calculateFunction(aktTime);

            DateTime nextTime = DateTime.Now + new TimeSpan(1, 0, 0);
            target.calculateFunction(nextTime);

            Assert.AreEqual(1, zoneServer._monitoredZones.Count);   // 1 zone monitored
            Assert.AreEqual(1, zoneServer._zoneStates.Count);       // 1 command issued
            Assert.AreEqual(false, zoneServer._zoneStates[new Address(100,1)].PowerStatus);   // switch off
        }

    }
}
