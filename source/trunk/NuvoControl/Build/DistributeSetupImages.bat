@echo off
echo Distribute Setup images (DistributeSetupImages.bat)
rmdir /s /q E:\doxygen\html\setup
mkdir E:\doxygen\html\setup
mkdir E:\doxygen\html\setup\debug\
mkdir E:\doxygen\html\setup\release\
mkdir E:\doxygen\html\setup\config\
copy /Y E:\ccnet\NuvoControl\NuvoControl\NuvoControlSetup\debug E:\doxygen\html\setup\debug\
copy /Y E:\ccnet\NuvoControl\NuvoControl\NuvoControlSetup\release E:\doxygen\html\setup\release\
copy /Y E:\ccnet\NuvoControl\NuvoControl\Config\Eichholzweg9E.zip E:\doxygen\html\setup\config\
echo Done ...