rem Start Unit Test of Visual Studio 2008 ...
del TestResults1.xml
"C:\Program Files\Microsoft Visual Studio 9.0\Common7\IDE\MSTest.exe" /testcontainer:NuvoControl\Server\NuvoControl.Server.ProtocolDriver.UnitTest\bin\Debug\NuvoControl.Server.ProtocolDriver.Test.dll /resultsfile:TestResults1.xml
del TestResults2.xml
"C:\Program Files\Microsoft Visual Studio 9.0\Common7\IDE\MSTest.exe" /testcontainer:NuvoControl\Server\NuvoControl.Server.Service.UnitTest\bin\Debug\NuvoControl.Server.Service.UnitTest.dll /resultsfile:TestResults2.xml
del TestResults3.xml
"C:\Program Files\Microsoft Visual Studio 9.0\Common7\IDE\MSTest.exe" /testcontainer:NuvoControl\Server\NuvoControl.Server.Dal.UnitTest\bin\Debug\NuvoControl.Server.Dal.UnitTest.dll /resultsfile:TestResults3.xml
rem Finish Unit Test of Visual Studio 2008 ...