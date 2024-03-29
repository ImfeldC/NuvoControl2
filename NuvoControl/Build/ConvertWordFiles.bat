@echo off
rem 
rem ****************************************
rem If you add a new file here, please add them also to the index page
rem "WordDocumentation.dox"
rem ****************************************
rem
echo Start converting word file (project documentation) ....
cscript E:\ccnet\NuvoControl\NuvoControl\Build\ConvertDoc2html.vbs ProjektManagement NuvoControl_0001_Pendenzenliste.doc
cscript E:\ccnet\NuvoControl\NuvoControl\Build\ConvertDoc2html.vbs ProjektManagement NuvoControl_0040_FeatureList_TaskList.doc
cscript E:\ccnet\NuvoControl\NuvoControl\Build\ConvertDoc2html.vbs ProjektManagement NuvoControl_0100_ProjektHandbuch.doc
cscript E:\ccnet\NuvoControl\NuvoControl\Build\ConvertDoc2html.vbs ProjektManagement NuvoControl_0104_DocumentTemplate.doc
cscript E:\ccnet\NuvoControl\NuvoControl\Build\ConvertDoc2html.vbs ProjektManagement NuvoControl_0105_DocumentStatusList.doc
cscript E:\ccnet\NuvoControl\NuvoControl\Build\ConvertDoc2html.vbs Analyse  NuvoControl_1202_Anforderungsspezifikation.doc
cscript E:\ccnet\NuvoControl\NuvoControl\Build\ConvertDoc2html.vbs Design   NuvoControl_1300_Software_Architektur_und_Design.doc
cscript E:\ccnet\NuvoControl\NuvoControl\Build\ConvertDoc2html.vbs Test     NuvoControl_1400_Testplan.doc
cscript E:\ccnet\NuvoControl\NuvoControl\Build\ConvertDoc2html.vbs UserDocumentation NuvoControl_8100_ProjektKurzbeschreibung.doc 
cscript E:\ccnet\NuvoControl\NuvoControl\Build\ConvertDoc2html.vbs UserDocumentation NuvoControl_8600_SetupGuideNuvoControl.docx
cscript E:\ccnet\NuvoControl\NuvoControl\Build\ConvertDoc2html.vbs UserDocumentation NuvoControl_8610_ConfigurationGuideNuvoControl.doc

echo Start copying EA files (project documentation) ...
rem -- not needed with Robocopy -- mkdir E:\doxygen\html\NuvoControl_1203_SystemDesign.eap_htmlexport\
robocopy E:\NuvoControl_Documentation\Design\NuvoControl_1203_SystemDesign.eap_htmlexport\ E:\doxygen\html\NuvoControl_1203_SystemDesign.eap_htmlexport\ /E

echo Start copying PowerPoint files (project documentation) ...
copy E:\NuvoControl_Documentation\UserDocumentation\NuvoContro_7100_ProjektPraesentation.pdf E:\doxygen\html\
copy E:\NuvoControl_Documentation\UserDocumentation\NuvoContro_7101_ProjektPraesentation_Demo.pdf E:\doxygen\html\

echo Start converting word file ....
cscript E:\ccnet\NuvoControl\NuvoControl\Build\ConvertDoc2html.vbs Design   HowToExportEnterpriseArchtitectFiles.doc


rem This last command is required beacause robocopy is setting the exit code <> 0
echo finish with success ....
exit /B 0
