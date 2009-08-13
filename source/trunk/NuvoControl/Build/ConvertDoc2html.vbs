'Wscript.Echo "Conversion Marco"

wdFormatHTML    = 8
wordFilePath = "E:\NuvoControl_Documentation\"
htmlFilePath = "E:\Documentation\"

Set WSHShell = WScript.CreateObject("WScript.Shell") 
set args = WScript.Arguments
num = args.Count

If (num < 2) Or (num>3) Then
   WScript.Echo "Usage: CScript Wsscript.vbs <sourcePath> <wordFileName.doc> <htmlFileName.htm>"
   WScript.Quit 1
end if

sourcePath = args.Item(0)
wordFileName = args.Item(1)

If (num = 2) Then
	 htmlFileName = wordFileName & ".html"
Else
	 htmlFileName = args.Item(2)
End If

WScript.Echo "Convert " & wordFileName & " to HTML file " & htmlFileName & " !"

Set appWord = Wscript.CreateObject("Word.Application") 

wordFullPath = (wordFilePath & sourcePath & "\" & wordFileName)
htmlFullPath = (htmlFilePath & htmlFileName)
WScript.Echo "Input: " & wordFullPath & " !"
WScript.Echo "Ouptut: " & htmlFullPath & " !"

'Opening File
appWord.Visible = FALSE   
appWord.Documents.Open wordFullPath

appWord.ActiveDocument.SaveAs htmlFullPath,wdFormatHTML,,,FALSE

appWord.Quit

Set appWord = Nothing


