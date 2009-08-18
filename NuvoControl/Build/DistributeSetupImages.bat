@echo off
rmdir /s /q E:\doxygen\html\setup
mkdir E:\doxygen\html\setup
mkdir E:\doxygen\html\setup\debug\
mkdir E:\doxygen\html\setup\release\
copy E:\ccnet\NuvoControl\NuvoControl\NuvoControlSetup\debug .\debug\
copy E:\ccnet\NuvoControl\NuvoControl\NuvoControlSetup\release .\release\
echo Done ...