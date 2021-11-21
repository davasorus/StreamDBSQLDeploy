Invoke-WebRequest -Uri "https://download.visualstudio.microsoft.com/download/pr/a865ccae-2219-4184-bcd6-0178dc580589/ba452d37e8396b7a49a9adc0e1a07e87/windowsdesktop-runtime-6.0.0-win-x64.exe" -OutFile "C:\Temp\windowsdesktop-runtime-6.0.0-win-x64.exe"

 Start-Process -Wait -FilePath ‘C:\Temp\windowsdesktop-runtime-6.0.0-win-x64.exe’ -ArgumentList ‘/install /quiet’

 Start-Process -Wait -FilePath ‘StreamDBSQLDeploy.exe’