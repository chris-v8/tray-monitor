@echo off
rem Builds all tray icons (Release), the installer and a portable zip.
rem Usage:  setup\build_all.bat [version]      e.g.  setup\build_all.bat 2.6.0.1
rem Without a version, the default from setup\config.iss is used.
rem Output: bin\Release\TrayMonitor-Setup-[version].exe
rem         bin\Release\TrayMonitor-Portable-[version].zip
setlocal
cd /d "%~dp0\.."

rem Command output goes through a temp file instead of for /f ('command'):
rem for /f runs the command in a child cmd, which also runs the user's cmd
rem AutoRun (e.g. "cls") and mixes its output into the result.
set "TMPF=%TEMP%\traymonitor_build_%RANDOM%.txt"

rem --- version ---
set "VER=%~1"
if "%VER%"=="" (
  findstr /c:"#define MyAppVersion" setup\config.iss > "%TMPF%"
  for /f "usebackq tokens=3" %%v in ("%TMPF%") do set "VER=%%~v"
)
echo === TrayMonitor %VER% ===

rem --- find MSBuild (works in a normal command prompt, no Developer Prompt needed) ---
set "MSBUILD="
where msbuild > "%TMPF%" 2>nul && set /p MSBUILD=<"%TMPF%"
set "VSWHERE=%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"
if not defined MSBUILD if exist "%VSWHERE%" "%VSWHERE%" -latest -prerelease -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe > "%TMPF%" && set /p MSBUILD=<"%TMPF%"
if not defined MSBUILD (
  echo ERROR: MSBuild not found. Install Visual Studio or Build Tools with ".NET desktop development".
  goto :fail
)

rem --- find Inno Setup compiler ---
set "ISCC="
if exist "%ProgramFiles(x86)%\Inno Setup 6\ISCC.exe" set "ISCC=%ProgramFiles(x86)%\Inno Setup 6\ISCC.exe"
if not defined ISCC if exist "%ProgramFiles%\Inno Setup 6\ISCC.exe" set "ISCC=%ProgramFiles%\Inno Setup 6\ISCC.exe"
if not defined ISCC if exist "%LOCALAPPDATA%\Programs\Inno Setup 6\ISCC.exe" set "ISCC=%LOCALAPPDATA%\Programs\Inno Setup 6\ISCC.exe"
if not defined ISCC where iscc > "%TMPF%" 2>nul && set /p ISCC=<"%TMPF%"
if exist "%TMPF%" del "%TMPF%"
if not defined ISCC (
  echo ERROR: Inno Setup 6 not found. Install it with:  choco install innosetup -y
  goto :fail
)

rem --- build the icons ---
for %%P in (BatteryIcon CpuIcon GpuIcon RamIcon DiskIcon NetIcon) do (
  echo.
  echo --- %%P ---
  "%MSBUILD%" src\%%P\%%P.csproj /p:Configuration=Release /v:minimal /nologo || goto :fail
)

rem --- installer ---
echo.
echo --- Installer ---
pushd setup
"%ISCC%" /Q "/DMyAppVersion=%VER%" config.iss || (popd & goto :fail)
popd

rem --- portable zip ---
echo.
echo --- Portable zip ---
set "ZIP=bin\Release\TrayMonitor-Portable-%VER%.zip"
if exist "%ZIP%" del "%ZIP%"
powershell -NoProfile -Command "Compress-Archive -Path 'bin\Release\BatteryIcon.exe','bin\Release\CpuIcon.exe','bin\Release\GpuIcon.exe','bin\Release\RamIcon.exe','bin\Release\DiskIcon.exe','bin\Release\NetIcon.exe' -DestinationPath '%ZIP%'" || goto :fail

echo.
echo === Done ===
echo   bin\Release\TrayMonitor-Setup-%VER%.exe
echo   %ZIP%
if not defined CI pause
exit /b 0

:fail
if exist "%TMPF%" del "%TMPF%"
echo.
echo === BUILD FAILED ===
if not defined CI pause
exit /b 1
