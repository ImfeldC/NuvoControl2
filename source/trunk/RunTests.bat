rem Start Unit Test of Visual Studio 2008 ...
del TestResults.xml
"C:\Program Files\Microsoft Visual Studio 9.0\Common7\IDE\MSTest.exe" /testcontainer:NuvoControl\Server\NuvoControl.Server.ProtocolDriver.UnitTest\bin\Debug\NuvoControl.Server.ProtocolDriver.Test.dll /resultsfile:TestResults.xml