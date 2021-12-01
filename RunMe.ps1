New-Item -Path "C:\Temp\" -ItemType Directory
New-Item -Path "C:\Temp\Import Chat text" -ItemType Directory

Invoke-WebRequest -Uri "https://download.visualstudio.microsoft.com/download/pr/a865ccae-2219-4184-bcd6-0178dc580589/ba452d37e8396b7a49a9adc0e1a07e87/windowsdesktop-runtime-6.0.0-win-x64.exe" -OutFile "C:\Temp\windowsdesktop-runtime-6.0.0-win-x64.exe"

Invoke-WebRequest -Uri "https://github.com/davasorus/ChatTextImporter/releases/download/1.0/ChatTextImport.exe" -OutFile "C:\Temp\Import Chat text\ChatTextImport.exe"


Start-Process -Wait -FilePath ‘C:\Temp\windowsdesktop-runtime-6.0.0-win-x64.exe’ -ArgumentList ‘/install /quiet’

Start-Process -Wait -FilePath ‘StreamDBSQLDeploy.exe’


New-Item -Path "$env:USERPROFILE\desktop\Import Chat text" -ItemType Directory
New-Item -Path "$env:USERPROFILE\desktop\Import Chat text" -Name "ChatTextImport Log.txt"

$TargetFile = "C:\Temp\Import Chat text\ChatTextImport.exe"
$ShortcutFile = "$env:USERPROFILE\desktop\Import Chat text\ChatTextImport.lnk"
$WScriptShell = New-Object -ComObject WScript.Shell
$Shortcut = $WScriptShell.CreateShortcut($ShortcutFile)
$Shortcut.TargetPath = $TargetFile
$Shortcut.Save()


$wsh = New-Object -ComObject Wscript.Shell

$wsh.Popup("Computer Must Restart to finish. Once this is done your Database is deployed and ready for use")

      
Start-Sleep -Seconds 5

 Restart-Computer