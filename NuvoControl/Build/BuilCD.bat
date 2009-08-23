@echo off
echo Remove previous image e:\NuvoControl ...
rmdir /S /Q e:\NuvoControl
echo Create destination directory
mkdir e:\NuvoControl

echo copy PDF files ...
mkdir e:\NuvoControl\pdf\
robocopy  e:\doxygen\pdf\ e:\NuvoControl\pdf\ /E

echo copy HTML files ...
mkdir e:\NuvoControl\html\
robocopy  e:\doxygen\html\ e:\NuvoControl\html\ /E

echo export source files ...
svn export https://svn2.xp-dev.com/svn/ImfeldC-NuvoControl/source/trunk e:\NuvoControl\source\  --username NuvoControlGuest --password NuvoControlGuest

echo export documentation files ...
svn export https://svn2.xp-dev.com/svn/ImfeldC-NuvoControl/documentation e:\NuvoControl\documentation\  --username NuvoControlGuest --password NuvoControlGuest

echo export setup image (release) ...
mkdir e:\NuvoControl\setup\
robocopy E:\ccnet\NuvoControl\NuvoControl\NuvoControlSetup\Release\ e:\NuvoControl\setup\ /E

echoc NuvoCOntrol CD Image has been build at e:\NuvoControl