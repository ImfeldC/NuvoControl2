@echo off

rem
rem E:
rem

echo Remove previous image e:\NuvoControl ...
rmdir /S /Q e:\NuvoControl
echo Create destination directory
mkdir e:\NuvoControl

echo copy HTML files ...
mkdir e:\NuvoControl\html\
robocopy  e:\doxygen\html\ e:\NuvoControl\html\ /E

echo export setup image (release) ...
mkdir e:\NuvoControl\setup\
robocopy E:\ccnet\NuvoControl\NuvoControl\NuvoControlSetup\Release\ e:\NuvoControl\setup\ /E

echo copy read-me file ...
copy E:\ccnet\NuvoControl\NuvoControl\Documentation\README.dox e:\NuvoControl\

echoc NuvoCOntrol CD Image has been build at e:\NuvoControl



rem This last command is required beacause robocopy is setting the exit code <> 0
echo finish with success ....
exit /B 0
