'Wscript.Echo "Conversion Marco"

	wdFormatHTML    = 8
	wdExportFormatPDF = 17
	wdExportOptimizeForOnScreen = 1
	wdExportAllDocument = 0
	wdExportDocumentWithMarkup = 7 			' or wdExportDocumentContent = 0
	wdExportCreateWordBookmarks = 2			' or wdExportCreateNoBookmarks = 0
	wordFilePath = "E:\NuvoControl_Documentation\"
	htmlFilePath = "E:\doxygen\html\"
	pdfFilePath  = "E:\doxygen\pdf\"
	
	Set WSHShell = WScript.CreateObject("WScript.Shell") 
	set args = WScript.Arguments
	num = args.Count
	
	If (num < 2) Or (num>3) Then
	   WScript.Echo "Usage: CScript Wsscript.vbs <sourcePath> <wordFileName.doc> <htmlFileName.htm>"
	   WScript.Quit 1
	end if
	
	sourcePath = args.Item(0)
	wordFileName = args.Item(1)
	
	' Create Word Filename
	If (num = 2) Then
		 htmlFileName = wordFileName & ".html"
	Else
		 htmlFileName = args.Item(2)
	End If
	
	' Create PDF Filename
	pdfFileName = wordFileName & ".pdf"
	
	WScript.Echo "Convert " & wordFileName & " to HTML file " & htmlFileName & " and PDf file " & pdfFileName & " !"
	
On Error Resume Next
	Set appWord = Wscript.CreateObject("Word.Application") 
	
	wordFullPath = (wordFilePath & sourcePath & "\" & wordFileName)
	WScript.Echo "Input: " & wordFullPath & " !"
	htmlFullPath = (htmlFilePath & htmlFileName)
	pdfFullPath = (pdfFilePath & pdfFileName)
	
	'Opening File
	appWord.Visible = FALSE   
	appWord.Documents.Open wordFullPath
	
	' Export as PDF
	WScript.Echo "PDF Ouptut: " & pdfFullPath & " !"
	appWord.ActiveDocument.ExportAsFixedFormat pdfFullPath, wdExportFormatPDF, False, _
        wdExportOptimizeForOnScreen, wdExportAllDocument, _
        0, 0, wdExportDocumentWithMarkup, true, true, _
        wdExportCreateWordBookmarks, true, true, False

	' Save as HTML
	WScript.Echo "HTML Ouptut: " & htmlFullPath & " !"
	appWord.ActiveDocument.SaveAs htmlFullPath,wdFormatHTML,,,FALSE

Handler:
	appWord.Quit
	Set appWord = Nothing


