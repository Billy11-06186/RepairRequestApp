@echo off
echo Восстановление пакетов NuGet...
cd /d "%~dp0"

if not exist "nuget.exe" (
    echo Скачивание nuget.exe...
    powershell -Command "Invoke-WebRequest https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -OutFile nuget.exe"
)

nuget restore RepairRequestApp.sln

echo Готово!
pause