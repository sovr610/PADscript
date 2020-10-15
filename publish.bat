@echo off
dotnet restore
dotnet build --configuration Release
dotnet publish -r win-x64 -f netcoreapp3.0 -c Release /p:PublishSingleFile=true
xcopy bin\Release\netcoreapp3.0\win-x64\publish\PADscript.exe C:\Windows\System32\PADscript.exe /s /e
pause