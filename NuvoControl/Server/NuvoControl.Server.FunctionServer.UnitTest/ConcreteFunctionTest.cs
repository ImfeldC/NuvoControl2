using NuvoControl.Server.FunctionServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

using Common.Logging;

using NuvoControl.Common;
using NuvoControl.Common.Configuration;
using NuvoControl.Server.ZoneServer;
using NuvoControl.Server.ProtocolDriver.Interface;

namespace NuvoControl.Server.FunctionServer.UnitTest
{

    /// <summary>
    /// Test class (mock) for funcion
    /// </summary>
    class FunctionTestClass : Function
    {
    }


    /// <summary>
    /// Test class (mock) for concrete function
    /// </summary>
    class ConcreteFunctionTestClass : ConcreteFunction
    {
        private Function _function;

        /// <summary>
        /// The new states of the zone.
        /// Received in notification method and processed in calculate method.
        /// </summary>
        private List<ZoneState> _newZoneState = new List<ZoneState>();


        public List<ZoneState> NewZoneState
        {
            get { return _newZoneState; }
        }

        /// <summary>
        /// Public method to test onFunctionError()
        /// </summary>
        public new void onFunctionError()
        {
            LogHelper.Log(LogLevel.Debug, String.Format(">>> onFunctionError: {0} [Function={1}]", this.ToString(), (_function != null ? _function.ToString() : "<null>")));
            base.onFunctionError();
        }

        /// <summary>
        /// Public method to test onFunctionStart()
        /// </summary>
        public new void onFunctionStart()
        {
            LogHelper.Log(LogLevel.Debug, String.Format(">>> onFunctionStart: {0}", _function.ToString()));
            base.onFunctionStart();
        }

        /// <summary>
        /// Public method to test onFunctionEnd()
        /// </summary>
        public new void onFunctionEnd()
        {
            LogHelper.Log(LogLevel.Debug, String.Format(">>> onFunctionEnd: {0}", _function.ToString()));
            base.onFunctionEnd();
        }

        /// <summary>
        /// Public method to test onValidityStart()
        /// </summary>
        public new void onValidityStart()
        {
            LogHelper.Log(LogLevel.Debug, String.Format(">>> onValidityStart: {0}", _function.ToString()));
            base.onValidityStart();
        }

        /// <summary>
        /// Public method to test onValidityEnd()
        /// </summary>
        public new void onValidityEnd()
        {
            LogHelper.Log(LogLevel.Debug, String.Format(">>> onValidityEnd: {0}", _function.ToString()));
            base.onValidityEnd();
        }


        #region ConcreteFunction methods

        public ConcreteFunctionTestClass(Function function, IZoneServer zoneServer, Dictionary<int, IAudioDriver> audioDrivers) :
            base(zoneServer, function, audioDrivers)
        {
            _function = function;
            subscribeZone(_function.ZoneId);
        }

        protected override void notifyOnZoneUpdate(ZoneStateEventArgs e)
        {
            // Store zone change, will be processed in calculate method
            _newZoneState.Add(e.ZoneState);
        }

        public override Function Function
        {
            get { return _function; }
        }

        public override void calculateFunction(DateTime aktTime)
        {
            throw new NotImplementedException();
        }

        #endregion

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
            ConcreteFunction target = new ConcreteFunctionTestClass(function, zoneServer, audioDrivers);
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


        /// <summary>
        ///A test for concrete function
        ///</summary>
        [TestMethod()]
        public void ConcreteFunctionTest1()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new SendNuvoCommand(new Guid(), false, false, false, false, false, true, true, new Address("100.1"), "OFF", "100.6", 10));
            ZoneChangeFunction function = new ZoneChangeFunction(new Guid(), new Address("100.1"), new Address(), 0, true, true, true, true, null, new TimeSpan(), new TimeSpan(), commands);
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = new Dictionary<int, IAudioDriver>();
            audioDrivers.Add(2, new AudioDriverMock());
            ConcreteFunctionTestClass target = new ConcreteFunctionTestClass(function, zoneServer, audioDrivers);

            // no validity defined -> per default valid
            Assert.AreEqual(true, target.Active);                   
            Assert.AreEqual(true, target.isActiveAt(DateTime.Now));
            Assert.AreEqual(true, target.isFunctionActiveToday(DateTime.Now));
            Assert.AreEqual(true, target.isFunctionActiveToday(DateTime.Now));

            // Check configuration and mock objects
            Assert.AreEqual(1, target.Function.Commands.Count);
            Assert.AreEqual(0, zoneServer.ZoneStateList.Count);
        }

        /// <summary>
        ///A test for concrete function
        ///</summary>
        [TestMethod()]
        public void ConcreteFunctionTest2()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new SendNuvoCommand(new Guid(), false, false, false, false, false, true, true, new Address("100.1"), "OFF", "100.6", 10));

            List<DayOfWeek> dayOfWeek = new List<DayOfWeek>();
            dayOfWeek.Add(DayOfWeek.Monday);
            dayOfWeek.Add(DayOfWeek.Tuesday);
            dayOfWeek.Add(DayOfWeek.Wednesday);
            dayOfWeek.Add(DayOfWeek.Thursday);
            dayOfWeek.Add(DayOfWeek.Friday);
            dayOfWeek.Add(DayOfWeek.Saturday);
            dayOfWeek.Add(DayOfWeek.Sunday);
            TimeSpan validFrom = new TimeSpan(0, 0, 1);     // NOTE: (0,0,0) is considered as "not set" (not used)
            TimeSpan validTo = new TimeSpan(23, 59, 59);

            ZoneChangeFunction function = new ZoneChangeFunction(new Guid(), new Address("100.1"), new Address(), 0, true, true, true, true, dayOfWeek, validFrom, validTo, commands);
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = new Dictionary<int, IAudioDriver>();
            audioDrivers.Add(2, new AudioDriverMock());
            ConcreteFunctionTestClass target = new ConcreteFunctionTestClass(function, zoneServer, audioDrivers);

            // no validity defined -> per default valid
            Assert.AreEqual(true, target.Active);
            Assert.AreEqual(true, target.isActiveAt(DateTime.Now));
            Assert.AreEqual(true, target.isFunctionActiveToday(DateTime.Now));
            Assert.AreEqual(true, target.isFunctionActiveToday(DateTime.Now));

            // Check configuration and mock objects
            Assert.AreEqual(1, target.Function.Commands.Count);
            Assert.AreEqual(0, zoneServer.ZoneStateList.Count);
        }

        /// <summary>
        ///A test for concrete function
        ///</summary>
        [TestMethod()]
        public void ConcreteFunctionTest3()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new SendNuvoCommand(new Guid(), false, false, false, false, false, true, true, new Address("100.1"), "OFF", "100.6", 10));

            List<DayOfWeek> dayOfWeek = new List<DayOfWeek>();
            //dayOfWeek.Add(DayOfWeek.Monday); -> not valid on Monday
            dayOfWeek.Add(DayOfWeek.Tuesday);
            dayOfWeek.Add(DayOfWeek.Wednesday);
            dayOfWeek.Add(DayOfWeek.Thursday);
            dayOfWeek.Add(DayOfWeek.Friday);
            dayOfWeek.Add(DayOfWeek.Saturday);
            dayOfWeek.Add(DayOfWeek.Sunday);
            TimeSpan validFrom = new TimeSpan(0, 0, 1);     // NOTE: (0,0,0) is considered as "not set" (not used)
            TimeSpan validTo = new TimeSpan(23, 59, 59);

            ZoneChangeFunction function = new ZoneChangeFunction(new Guid(), new Address("100.1"), new Address(), 0, true, true, true, true, dayOfWeek, validFrom, validTo, commands);
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = new Dictionary<int, IAudioDriver>();
            audioDrivers.Add(2, new AudioDriverMock());
            ConcreteFunctionTestClass target = new ConcreteFunctionTestClass(function, zoneServer, audioDrivers);

            // 22.02.2016 is Monday
            //Assert.AreEqual(true, target.Active); --> works only with current date+time
            Assert.AreEqual(true, target.isActiveAt(new DateTime(2016,2,21,23,59,59)));
            Assert.AreEqual(true, target.isFunctionActiveToday(new DateTime(2016, 2, 21, 23, 59, 59)));
            Assert.AreEqual(true, target.isFunctionActiveRightNow(new DateTime(2016, 2, 21, 23, 59, 59)));

            Assert.AreEqual(false, target.isActiveAt(new DateTime(2016, 2, 22, 0, 0, 0)));                  // considers TIME + DATE
            Assert.AreEqual(false, target.isFunctionActiveToday(new DateTime(2016, 2, 22, 0, 0, 0)));       // considers only DATE
            Assert.AreEqual(false, target.isFunctionActiveRightNow(new DateTime(2016, 2, 22, 0, 0, 0)));    // considers only TIME
            Assert.AreEqual(false, target.isActiveAt(new DateTime(2016, 2, 22, 23, 59, 59)));
            Assert.AreEqual(false, target.isFunctionActiveToday(new DateTime(2016, 2, 22, 23, 59, 59)));
            Assert.AreEqual(true, target.isFunctionActiveRightNow(new DateTime(2016, 2, 22, 23, 59, 58)));  // considers only TIME
            Assert.AreEqual(true, target.isFunctionActiveRightNow(new DateTime(2016, 2, 22, 23, 59, 59)));  // considers only TIME
            
            Assert.AreEqual(false, target.isActiveAt(new DateTime(2016, 2, 23, 0, 0, 0)));                  // considers TIME + DATE
            Assert.AreEqual(true, target.isActiveAt(new DateTime(2016, 2, 23, 0, 0, 1)));                   // considers TIME + DATE
            Assert.AreEqual(true, target.isFunctionActiveToday(new DateTime(2016, 2, 23, 0, 0, 0)));        // considers only DATE
            Assert.AreEqual(true, target.isFunctionActiveToday(new DateTime(2016, 2, 23, 0, 0, 1)));        // considers only DATE
            Assert.AreEqual(false, target.isFunctionActiveRightNow(new DateTime(2016, 2, 23, 0, 0, 0)));    // considers only TIME
            Assert.AreEqual(true, target.isFunctionActiveRightNow(new DateTime(2016, 2, 23, 0, 0, 1)));     // considers only TIME

            // Check configuration and mock objects
            Assert.AreEqual(1, target.Function.Commands.Count);
            Assert.AreEqual(0, zoneServer.ZoneStateList.Count);
        }

        /// <summary>
        ///A test for concrete function
        ///</summary>
        [TestMethod()]
        public void ConcreteFunctionTest4()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new SendNuvoCommand(new Guid(), false, false, false, false, false, true, true, new Address("100.1"), "OFF", "100.6", 10));

            List<DayOfWeek> dayOfWeek = new List<DayOfWeek>();
            dayOfWeek.Add(DayOfWeek.Monday);
            dayOfWeek.Add(DayOfWeek.Tuesday);
            dayOfWeek.Add(DayOfWeek.Wednesday);
            dayOfWeek.Add(DayOfWeek.Thursday);
            dayOfWeek.Add(DayOfWeek.Friday);
            dayOfWeek.Add(DayOfWeek.Saturday);
            dayOfWeek.Add(DayOfWeek.Sunday);
            TimeSpan validFrom = new TimeSpan(0, 0, 0);     // NOTE: (0,0,0) is considered as "not set" (not used)
            TimeSpan validTo = new TimeSpan(0,0,0);

            ZoneChangeFunction function = new ZoneChangeFunction(new Guid(), new Address("100.1"), new Address(), 0, true, true, true, true, dayOfWeek, validFrom, validTo, commands);
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = new Dictionary<int, IAudioDriver>();
            audioDrivers.Add(2, new AudioDriverMock());
            ConcreteFunctionTestClass target = new ConcreteFunctionTestClass(function, zoneServer, audioDrivers);

            // no validity defined -> per default valid
            Assert.AreEqual(true, target.Active);

            // execute events ...
            target.onFunctionError();
            target.onValidityStart();
            target.onFunctionStart();
            target.onFunctionEnd();
            target.onValidityEnd();

            // Check configuration and mock objects
            Assert.AreEqual(1, target.Function.Commands.Count);
            Assert.AreEqual(0, zoneServer.ZoneStateList.Count);
        }

        /// <summary>
        ///A test for concrete function
        ///</summary>
        [TestMethod()]
        public void ConcreteFunctionTest5()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new SendNuvoCommand(new Guid(), true, true, true, true, true, true, true, new Address("100.1"), "OFF", "100.6", 10));

            List<DayOfWeek> dayOfWeek = new List<DayOfWeek>();
            dayOfWeek.Add(DayOfWeek.Monday);
            dayOfWeek.Add(DayOfWeek.Tuesday);
            dayOfWeek.Add(DayOfWeek.Wednesday);
            dayOfWeek.Add(DayOfWeek.Thursday);
            dayOfWeek.Add(DayOfWeek.Friday);
            dayOfWeek.Add(DayOfWeek.Saturday);
            dayOfWeek.Add(DayOfWeek.Sunday);
            TimeSpan validFrom = new TimeSpan(0, 0, 0);     // NOTE: (0,0,0) is considered as "not set" (not used)
            TimeSpan validTo = new TimeSpan(0, 0, 0);

            ZoneChangeFunction function = new ZoneChangeFunction(new Guid(), new Address("100.1"), new Address(), 0, true, true, true, true, dayOfWeek, validFrom, validTo, commands);
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = new Dictionary<int, IAudioDriver>();
            audioDrivers.Add(2, new AudioDriverMock());
            ConcreteFunctionTestClass target = new ConcreteFunctionTestClass(function, zoneServer, audioDrivers);

            // no validity defined -> per default valid
            Assert.AreEqual(true, target.Active);

            // execute events ...
            target.onFunctionError();
            target.onValidityStart();
            target.onFunctionStart();
            target.onFunctionEnd();
            target.onValidityEnd();

            // Check configuration and mock objects
            Assert.AreEqual(1, target.Function.Commands.Count);
            Assert.AreEqual(5, zoneServer.ZoneStateList.Count);
            Assert.AreEqual(false, zoneServer.ZoneStateList[0].PowerStatus );
        }

        /// <summary>
        ///A test for concrete function
        ///</summary>
        [TestMethod()]
        public void ConcreteFunctionTest6a()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new SendNuvoCommand(new Guid(), false, true, false, false, false, true, true, new Address("100.1"), "ON", "100.6", 10));

            List<DayOfWeek> dayOfWeek = null;
            TimeSpan validFrom = new TimeSpan(0, 0, 0);     // NOTE: (0,0,0) is considered as "not set" (not used)
            TimeSpan validTo = new TimeSpan(0, 0, 0);

            ZoneChangeFunction function = new ZoneChangeFunction(new Guid(), new Address("100.1"), new Address(), 0, true, true, true, true, dayOfWeek, validFrom, validTo, commands);
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = new Dictionary<int, IAudioDriver>();
            audioDrivers.Add(2, new AudioDriverMock());
            ConcreteFunctionTestClass target = new ConcreteFunctionTestClass(function, zoneServer, audioDrivers);

            // no validity defined -> per default valid
            Assert.AreEqual(true, target.Active);

            // execute events ...
            target.onFunctionStart();

            // Check configuration and mock objects
            Assert.AreEqual(1, target.Function.Commands.Count);
            Assert.AreEqual(1, zoneServer.ZoneStateList.Count);
            Assert.AreEqual(true, zoneServer.ZoneStateList[0].PowerStatus);
        }

        /// <summary>
        ///A test for concrete function
        ///</summary>
        [TestMethod()]
        public void ConcreteFunctionTest7a()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new SendNuvoCommand(new Guid(), false, false, true, false, false, true, true, new Address("100.1"), "ON", "100.6", 10));

            List<DayOfWeek> dayOfWeek = null;
            TimeSpan validFrom = new TimeSpan(0, 0, 0);     // NOTE: (0,0,0) is considered as "not set" (not used)
            TimeSpan validTo = new TimeSpan(0, 0, 0);

            ZoneChangeFunction function = new ZoneChangeFunction(new Guid(), new Address("100.1"), new Address(), 0, true, true, true, true, dayOfWeek, validFrom, validTo, commands);
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = new Dictionary<int, IAudioDriver>();
            audioDrivers.Add(2, new AudioDriverMock());
            ConcreteFunctionTestClass target = new ConcreteFunctionTestClass(function, zoneServer, audioDrivers);

            // no validity defined -> per default valid
            Assert.AreEqual(true, target.Active);

            // execute events ...
            target.onFunctionEnd();

            // Check configuration and mock objects
            Assert.AreEqual(1, target.Function.Commands.Count);
            Assert.AreEqual(1, zoneServer.ZoneStateList.Count);
            Assert.AreEqual(true, zoneServer.ZoneStateList[0].PowerStatus);
        }

        /// <summary>
        ///A test for concrete function
        ///</summary>
        [TestMethod()]
        public void ConcreteFunctionTest8a()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new SendNuvoCommand(new Guid(), false, false, false, true, false, true, true, new Address("100.1"), "ON", "100.6", 10));

            List<DayOfWeek> dayOfWeek = null;
            TimeSpan validFrom = new TimeSpan(0, 0, 0);     // NOTE: (0,0,0) is considered as "not set" (not used)
            TimeSpan validTo = new TimeSpan(0, 0, 0);

            ZoneChangeFunction function = new ZoneChangeFunction(new Guid(), new Address("100.1"), new Address(), 0, true, true, true, true, dayOfWeek, validFrom, validTo, commands);
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = new Dictionary<int, IAudioDriver>();
            audioDrivers.Add(2, new AudioDriverMock());
            ConcreteFunctionTestClass target = new ConcreteFunctionTestClass(function, zoneServer, audioDrivers);

            // no validity defined -> per default valid
            Assert.AreEqual(true, target.Active);

            // execute events ...
            target.onValidityStart();

            // Check configuration and mock objects
            Assert.AreEqual(1, target.Function.Commands.Count);
            Assert.AreEqual(1, zoneServer.ZoneStateList.Count);
            Assert.AreEqual(true, zoneServer.ZoneStateList[0].PowerStatus);
        }

        /// <summary>
        ///A test for concrete function
        ///</summary>
        [TestMethod()]
        public void ConcreteFunctionTest9a()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new SendNuvoCommand(new Guid(), false, false, false, false, true, true, true, new Address("100.1"), "ON", "100.6", 10));

            List<DayOfWeek> dayOfWeek = null;
            TimeSpan validFrom = new TimeSpan(0, 0, 0);     // NOTE: (0,0,0) is considered as "not set" (not used)
            TimeSpan validTo = new TimeSpan(0, 0, 0);

            ZoneChangeFunction function = new ZoneChangeFunction(new Guid(), new Address("100.1"), new Address(), 0, true, true, true, true, dayOfWeek, validFrom, validTo, commands);
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = new Dictionary<int, IAudioDriver>();
            audioDrivers.Add(2, new AudioDriverMock());
            ConcreteFunctionTestClass target = new ConcreteFunctionTestClass(function, zoneServer, audioDrivers);

            // no validity defined -> per default valid
            Assert.AreEqual(true, target.Active);

            // execute events ...
            target.onValidityEnd();

            // Check configuration and mock objects
            Assert.AreEqual(1, target.Function.Commands.Count);
            Assert.AreEqual(1, zoneServer.ZoneStateList.Count);
            Assert.AreEqual(true, zoneServer.ZoneStateList[0].PowerStatus);
        }

        /// <summary>
        ///A test for concrete function
        ///</summary>
        [TestMethod()]
        public void ConcreteFunctionTest10a()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new SendNuvoCommand(new Guid(), true, false, false, false, false, true, true, new Address("100.1"), "ON", "100.6", 10));

            List<DayOfWeek> dayOfWeek = null;
            TimeSpan validFrom = new TimeSpan(0, 0, 0);     // NOTE: (0,0,0) is considered as "not set" (not used)
            TimeSpan validTo = new TimeSpan(0, 0, 0);

            ZoneChangeFunction function = new ZoneChangeFunction(new Guid(), new Address("100.1"), new Address(), 0, true, true, true, true, dayOfWeek, validFrom, validTo, commands);
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = new Dictionary<int, IAudioDriver>();
            audioDrivers.Add(2, new AudioDriverMock());
            ConcreteFunctionTestClass target = new ConcreteFunctionTestClass(function, zoneServer, audioDrivers);

            // no validity defined -> per default valid
            Assert.AreEqual(true, target.Active);

            // execute events ...
            target.onFunctionError();

            // Check configuration and mock objects
            Assert.AreEqual(1, target.Function.Commands.Count);
            Assert.AreEqual(1, zoneServer.ZoneStateList.Count);
            Assert.AreEqual(true, zoneServer.ZoneStateList[0].PowerStatus);
        }

        /// <summary>
        ///A test for concrete function
        ///</summary>
        [TestMethod()]
        public void ConcreteFunctionTest6b()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new SendNuvoCommand(new Guid(), false, true, false, false, false, true, true, new Address("100.1"), "ON", "100.6", 10));

            List<DayOfWeek> dayOfWeek = null;
            TimeSpan validFrom = new TimeSpan(0, 0, 0);     // NOTE: (0,0,0) is considered as "not set" (not used)
            TimeSpan validTo = new TimeSpan(0, 0, 0);

            ZoneChangeFunction function = new ZoneChangeFunction(new Guid(), new Address("100.1"), new Address(), 0, true, true, true, true, dayOfWeek, validFrom, validTo, commands);
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = new Dictionary<int, IAudioDriver>();
            audioDrivers.Add(2, new AudioDriverMock());
            ConcreteFunctionTestClass target = new ConcreteFunctionTestClass(function, zoneServer, audioDrivers);

            // no validity defined -> per default valid
            Assert.AreEqual(true, target.Active);

            // execute events ...
            target.onFunctionError();
            Assert.AreEqual(0, zoneServer.ZoneStateList.Count);
            target.onValidityStart();
            Assert.AreEqual(0, zoneServer.ZoneStateList.Count);
            target.onFunctionStart();
            Assert.AreEqual(1, zoneServer.ZoneStateList.Count);
            target.onFunctionEnd();
            Assert.AreEqual(1, zoneServer.ZoneStateList.Count);
            target.onValidityEnd();
            Assert.AreEqual(1, zoneServer.ZoneStateList.Count);

            // Check configuration and mock objects
            Assert.AreEqual(1, target.Function.Commands.Count);
            Assert.AreEqual(1, zoneServer.ZoneStateList.Count);
            Assert.AreEqual(true, zoneServer.ZoneStateList[0].PowerStatus);
        }

        /// <summary>
        ///A test for concrete function
        ///</summary>
        [TestMethod()]
        public void ConcreteFunctionTest7b()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new SendNuvoCommand(new Guid(), false, false, true, false, false, true, true, new Address("100.1"), "ON", "100.6", 10));

            List<DayOfWeek> dayOfWeek = null;
            TimeSpan validFrom = new TimeSpan(0, 0, 0);     // NOTE: (0,0,0) is considered as "not set" (not used)
            TimeSpan validTo = new TimeSpan(0, 0, 0);

            ZoneChangeFunction function = new ZoneChangeFunction(new Guid(), new Address("100.1"), new Address(), 0, true, true, true, true, dayOfWeek, validFrom, validTo, commands);
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = new Dictionary<int, IAudioDriver>();
            audioDrivers.Add(2, new AudioDriverMock());
            ConcreteFunctionTestClass target = new ConcreteFunctionTestClass(function, zoneServer, audioDrivers);

            // no validity defined -> per default valid
            Assert.AreEqual(true, target.Active);

            // execute events ...
            target.onFunctionError();
            Assert.AreEqual(0, zoneServer.ZoneStateList.Count);
            target.onValidityStart();
            Assert.AreEqual(0, zoneServer.ZoneStateList.Count);
            target.onFunctionStart();
            Assert.AreEqual(0, zoneServer.ZoneStateList.Count);
            target.onFunctionEnd();
            Assert.AreEqual(1, zoneServer.ZoneStateList.Count);
            target.onValidityEnd();
            Assert.AreEqual(1, zoneServer.ZoneStateList.Count);

            // Check configuration and mock objects
            Assert.AreEqual(1, target.Function.Commands.Count);
            Assert.AreEqual(1, zoneServer.ZoneStateList.Count);
            Assert.AreEqual(true, zoneServer.ZoneStateList[0].PowerStatus);
        }

        /// <summary>
        ///A test for concrete function
        ///</summary>
        [TestMethod()]
        public void ConcreteFunctionTest8b()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new SendNuvoCommand(new Guid(), false, false, false, true, false, true, true, new Address("100.1"), "ON", "100.6", 10));

            List<DayOfWeek> dayOfWeek = null;
            TimeSpan validFrom = new TimeSpan(0, 0, 0);     // NOTE: (0,0,0) is considered as "not set" (not used)
            TimeSpan validTo = new TimeSpan(0, 0, 0);

            ZoneChangeFunction function = new ZoneChangeFunction(new Guid(), new Address("100.1"), new Address(), 0, true, true, true, true, dayOfWeek, validFrom, validTo, commands);
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = new Dictionary<int, IAudioDriver>();
            audioDrivers.Add(2, new AudioDriverMock());
            ConcreteFunctionTestClass target = new ConcreteFunctionTestClass(function, zoneServer, audioDrivers);

            // no validity defined -> per default valid
            Assert.AreEqual(true, target.Active);

            // execute events ...
            target.onFunctionError();
            Assert.AreEqual(0, zoneServer.ZoneStateList.Count);
            target.onValidityStart();
            Assert.AreEqual(1, zoneServer.ZoneStateList.Count);
            target.onFunctionStart();
            Assert.AreEqual(1, zoneServer.ZoneStateList.Count);
            target.onFunctionEnd();
            Assert.AreEqual(1, zoneServer.ZoneStateList.Count);
            target.onValidityEnd();
            Assert.AreEqual(1, zoneServer.ZoneStateList.Count);

            // Check configuration and mock objects
            Assert.AreEqual(1, target.Function.Commands.Count);
            Assert.AreEqual(1, zoneServer.ZoneStateList.Count);
            Assert.AreEqual(true, zoneServer.ZoneStateList[0].PowerStatus);
        }

        /// <summary>
        ///A test for concrete function
        ///</summary>
        [TestMethod()]
        public void ConcreteFunctionTest9b()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new SendNuvoCommand(new Guid(), false, false, false, false, true, true, true, new Address("100.1"), "ON", "100.6", 10));

            List<DayOfWeek> dayOfWeek = null;
            TimeSpan validFrom = new TimeSpan(0, 0, 0);     // NOTE: (0,0,0) is considered as "not set" (not used)
            TimeSpan validTo = new TimeSpan(0, 0, 0);

            ZoneChangeFunction function = new ZoneChangeFunction(new Guid(), new Address("100.1"), new Address(), 0, true, true, true, true, dayOfWeek, validFrom, validTo, commands);
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = new Dictionary<int, IAudioDriver>();
            audioDrivers.Add(2, new AudioDriverMock());
            ConcreteFunctionTestClass target = new ConcreteFunctionTestClass(function, zoneServer, audioDrivers);

            // no validity defined -> per default valid
            Assert.AreEqual(true, target.Active);

            // execute events ...
            target.onFunctionError();
            Assert.AreEqual(0, zoneServer.ZoneStateList.Count);
            target.onValidityStart();
            Assert.AreEqual(0, zoneServer.ZoneStateList.Count);
            target.onFunctionStart();
            Assert.AreEqual(0, zoneServer.ZoneStateList.Count);
            target.onFunctionEnd();
            Assert.AreEqual(0, zoneServer.ZoneStateList.Count);
            target.onValidityEnd();
            Assert.AreEqual(1, zoneServer.ZoneStateList.Count);

            // Check configuration and mock objects
            Assert.AreEqual(1, target.Function.Commands.Count);
            Assert.AreEqual(1, zoneServer.ZoneStateList.Count);
            Assert.AreEqual(true, zoneServer.ZoneStateList[0].PowerStatus);
        }

        /// <summary>
        ///A test for concrete function
        ///</summary>
        [TestMethod()]
        public void ConcreteFunctionTest10b()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new SendNuvoCommand(new Guid(), true, false, false, false, false, true, true, new Address("100.1"), "ON", "100.6", 10));

            List<DayOfWeek> dayOfWeek = null;
            TimeSpan validFrom = new TimeSpan(0, 0, 0);     // NOTE: (0,0,0) is considered as "not set" (not used)
            TimeSpan validTo = new TimeSpan(0, 0, 0);

            ZoneChangeFunction function = new ZoneChangeFunction(new Guid(), new Address("100.1"), new Address(), 0, true, true, true, true, dayOfWeek, validFrom, validTo, commands);
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = new Dictionary<int, IAudioDriver>();
            audioDrivers.Add(2, new AudioDriverMock());
            ConcreteFunctionTestClass target = new ConcreteFunctionTestClass(function, zoneServer, audioDrivers);

            // no validity defined -> per default valid
            Assert.AreEqual(true, target.Active);

            // execute events ...
            target.onFunctionError();
            Assert.AreEqual(1, zoneServer.ZoneStateList.Count);
            target.onValidityStart();
            Assert.AreEqual(1, zoneServer.ZoneStateList.Count);
            target.onFunctionStart();
            Assert.AreEqual(1, zoneServer.ZoneStateList.Count);
            target.onFunctionEnd();
            Assert.AreEqual(1, zoneServer.ZoneStateList.Count);
            target.onValidityEnd();
            Assert.AreEqual(1, zoneServer.ZoneStateList.Count);

            // Check configuration and mock objects
            Assert.AreEqual(1, target.Function.Commands.Count);
            Assert.AreEqual(1, zoneServer.ZoneStateList.Count);
            Assert.AreEqual(true, zoneServer.ZoneStateList[0].PowerStatus);
        }

        /// <summary>
        ///A test for concrete function
        ///</summary>
        [TestMethod()]
        public void ConcreteFunctionTest11()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new SendNuvoCommand(new Guid(), true, false, false, false, false, true, true, new Address("100.1"), "ON", "100.6", 10));
            commands.Add(new SendNuvoCommand(new Guid(), true, false, false, false, false, true, true, new Address("100.2"), "OFF", "100.5", 20));

            List<DayOfWeek> dayOfWeek = null;
            TimeSpan validFrom = new TimeSpan(0, 0, 0);     // NOTE: (0,0,0) is considered as "not set" (not used)
            TimeSpan validTo = new TimeSpan(0, 0, 0);

            ZoneChangeFunction function = new ZoneChangeFunction(new Guid(), new Address("100.1"), new Address(), 0, true, true, true, true, dayOfWeek, validFrom, validTo, commands);
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = new Dictionary<int, IAudioDriver>();
            audioDrivers.Add(2, new AudioDriverMock());
            ConcreteFunctionTestClass target = new ConcreteFunctionTestClass(function, zoneServer, audioDrivers);

            // no validity defined -> per default valid
            Assert.AreEqual(true, target.Active);

            // execute events ...
            target.onFunctionError();
            Assert.AreEqual(2, zoneServer.ZoneStateList.Count);
            target.onValidityStart();
            Assert.AreEqual(2, zoneServer.ZoneStateList.Count);
            target.onFunctionStart();
            Assert.AreEqual(2, zoneServer.ZoneStateList.Count);
            target.onFunctionEnd();
            Assert.AreEqual(2, zoneServer.ZoneStateList.Count);
            target.onValidityEnd();
            Assert.AreEqual(2, zoneServer.ZoneStateList.Count);

            // Check configuration and mock objects
            Assert.AreEqual(2, target.Function.Commands.Count);
            Assert.AreEqual(2, zoneServer.ZoneStateList.Count);
            Assert.AreEqual(true, zoneServer.ZoneStateList[0].PowerStatus);
            Assert.AreEqual(false, zoneServer.ZoneStateList[1].PowerStatus);
            Assert.AreEqual(6, zoneServer.ZoneStateList[0].Source.ObjectId);
            Assert.AreEqual(5, zoneServer.ZoneStateList[1].Source.ObjectId);
            Assert.AreEqual(2, zoneServer.ZoneStates.Count);
            Assert.IsNotNull(zoneServer.ZoneStates[new Address("100.1")]);
            Assert.IsNotNull(zoneServer.ZoneStates[new Address("100.2")]);
        }


        #region Disabled code, generated by unit test wizard

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

        #endregion

    }
}
