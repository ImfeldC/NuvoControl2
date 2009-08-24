@echo off


rem 
rem Z:
rem

echo Remove previous image z:\NuvoControl ...
rmdir /S /Q z:\NuvoControl
echo Create destination directory
mkdir z:\NuvoControl

echo copy PDF files ...
mkdir z:\NuvoControl\pdf\
robocopy  e:\doxygen\pdf\ z:\NuvoControl\pdf\ /E

echo copy HTML files ...
mkdir z:\NuvoControl\html\
robocopy  e:\doxygen\html\ z:\NuvoControl\html\ /E

echo export source files ...
svn export https://svn2.xp-dev.com/svn/ImfeldC-NuvoControl/source/trunk z:\NuvoControl\source\  --username NuvoControlGuest --password NuvoControlGuest

echo export documentation files ...
svn export https://svn2.xp-dev.com/svn/ImfeldC-NuvoControl/documentation z:\NuvoControl\documentation\  --username NuvoControlGuest --password NuvoControlGuest

echo export setup image (release) ...
mkdir z:\NuvoControl\setup\
robocopy E:\ccnet\NuvoControl\NuvoControl\NuvoControlSetup\Release\ z:\NuvoControl\setup\ /E

echo copy read-me file ...
copy E:\ccnet\NuvoControl\NuvoControl\Documentation\README.dox z:\NuvoControl\ /E

echoc NuvoCOntrol CD Image has been build at z:\NuvoControl



rem
rem E:
rem

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

echo copy read-me file ...
copy E:\ccnet\NuvoControl\NuvoControl\Documentation\README.dox e:\NuvoControl\ /E

echoc NuvoCOntrol CD Image has been build at e:\NuvoControl



rem This last command is required beacause robocopy is setting the exit code <> 0
echo finish with success ....
exit /B 0
