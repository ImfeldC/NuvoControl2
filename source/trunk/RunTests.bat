@echo off
rem
rem Start Unit Test of Visual Studio 2008 ...
rem
if EXIST TestResults1.xml del TestResults1.xml
"C:\Program Files\Microsoft Visual Studio 9.0\Common7\IDE\MSTest.exe" /runconfig:NuvoControl\LocalTestRun.testrunconfig /testcontainer:NuvoControl\Server\NuvoControl.Server.ProtocolDriver.UnitTest\bin\Debug\NuvoControl.Server.ProtocolDriver.Test.dll /resultsfile:TestResults1.xml
if EXIST TestResults2.xml del TestResults2.xml
"C:\Program Files\Microsoft Visual Studio 9.0\Common7\IDE\MSTest.exe" /runconfig:NuvoControl\LocalTestRun.testrunconfig /testcontainer:NuvoControl\Server\NuvoControl.Server.Service.UnitTest\bin\Debug\NuvoControl.Server.Service.UnitTest.dll /resultsfile:TestResults2.xml
if EXIST TestResults3.xml del TestResults3.xml
"C:\Program Files\Microsoft Visual Studio 9.0\Common7\IDE\MSTest.exe" /runconfig:NuvoControl\LocalTestRun.testrunconfig /testcontainer:NuvoControl\Server\NuvoControl.Server.Dal.UnitTest\bin\Debug\NuvoControl.Server.Dal.UnitTest.dll /testcontainer:NuvoControl\Test\NuvoControl.Test\NuvoControl.Test\bin\Debug\NuvoControl.Test.dll /resultsfile:TestResults3.xml
rem
rem Finish Unit Test of Visual Studio 2008 ...
rem
