@echo off
rem ===============================================================================================
rem Script to build and publish new VaultExplorer ClickOnce package
rem
rem Author: Eli Zeitlin (elize) 01/16/2013
rem ===============================================================================================

setlocal

set ClickOnceInstallUpdateUrl=\\ELIZE-HP8540\C$\Temp\VaultExplorer\

msbuild /v:minimal /fl /flp:verbosity=normal /m /p:Configuration=Release;ClickOnceInstallUpdateUrl=%ClickOnceInstallUpdateUrl% /t:rebuild;publish

if %ERRORLEVEL% NEQ 0 (
    echo Error: Build break!
    exit /b 1
)

del .\bin\Release\app.publish\VaultExplorer.exe
if %ERRORLEVEL% NEQ 0 (
    echo Error: Failed to delete unnecessary file. This is not expected!
    exit /b 1
)

xcopy .\bin\Release\app.publish %ClickOnceInstallUpdateUrl% /S /Y /Q
if %ERRORLEVEL% NEQ 0 (
    echo Error: Failed to copy from .\bin\Release\app.publish to %ClickOnceInstallUpdateUrl%
    exit /b 1
)
      
start %windir%\explorer.exe "%ClickOnceInstallUpdateUrl%Application Files\"