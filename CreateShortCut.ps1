New-Item -Path "$env:USERPROFILE\desktop\Import Chat text" -ItemType Directory
New-Item -Path "$env:USERPROFILE\desktop\Import Chat text" -Name "ChatTextImport Log.txt"



$TargetFile = "Z:\Share\VS2017\ChatTextImporter\bin\Debug\ChatTextImport.exe"
$ShortcutFile = "$env:USERPROFILE\desktop\Import Chat text\ChatTextImport.lnk"
$WScriptShell = New-Object -ComObject WScript.Shell
$Shortcut = $WScriptShell.CreateShortcut($ShortcutFile)
$Shortcut.TargetPath = $TargetFile
$Shortcut.Save()