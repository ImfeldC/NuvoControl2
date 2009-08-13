@echo off
echo Cleanup ..
e:
cd E:\doxygen\
rmdir /s /q Documentation
echo Create directory ...
cd E:\doxygen\
mkdir Documentation
echo Start converting word file ....
cscript E:\NuvoControl_Trunk\Build\ConvertDoc2html.vbs UserDocumentation NuvoControl_8100_ProjektKurzbeschreibung.doc 
cscript E:\NuvoControl_Trunk\Build\ConvertDoc2html.vbs UserDocumentation NuvoControl_8600_SetupGuideNuvoControl.docx
cscript E:\NuvoControl_Trunk\Build\ConvertDoc2html.vbs Vision   NuvoControl_0110_Projektantrag.doc
cscript E:\NuvoControl_Trunk\Build\ConvertDoc2html.vbs Analyse  NuvoControl_1202_Anforderungsspezifikation.doc
cscript E:\NuvoControl_Trunk\Build\ConvertDoc2html.vbs Design   NuvoControl_1300_Software_Architektur_und_Design.doc
cscript E:\NuvoControl_Trunk\Build\ConvertDoc2html.vbs ProjektManagement NuvoControl_0010_StundenNachweis.doc
cscript E:\NuvoControl_Trunk\Build\ConvertDoc2html.vbs ProjektManagement NuvoControl_0040_FeatureList_TaskList.doc
cscript E:\NuvoControl_Trunk\Build\ConvertDoc2html.vbs ProjektManagement NuvoControl_0100_ProjektHandbuch.doc
cscript E:\NuvoControl_Trunk\Build\ConvertDoc2html.vbs ProjektManagement NuvoControl_0103_Glossary.doc
cscript E:\NuvoControl_Trunk\Build\ConvertDoc2html.vbs ProjektManagement NuvoControl_0105_DocumentStatusList.doc
cscript E:\NuvoControl_Trunk\Build\ConvertDoc2html.vbs ProjektManagement NuvoControl_0001_Pendenzenliste.doc
