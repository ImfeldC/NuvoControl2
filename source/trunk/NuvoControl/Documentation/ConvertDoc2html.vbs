'Wscript.Echo "Conversion Marco"

wdFormatHTML    = 8


Set WSHShell = WScript.CreateObject("WScript.Shell") 
set args = WScript.Arguments
num = args.Count

if num <> 2 Then
   WScript.Echo "Usage: CScript Wsscript.vbs <wordFileName.doc> <htmlFileName.htm>"
   WScript.Quit 1
end if


Set appWord = Wscript.CreateObject("Word.Application") 

wordFileName = args.Item(0)
htmlFileName = args.Item(1)


'Opening File
appWord.Visible = FALSE   
appWord.Documents.Open wordFileName   

appWord.ActiveDocument.SaveAs htmlFileName,wdFormatHTML,,,FALSE

appWord.Quit

Set appWord = Nothing


