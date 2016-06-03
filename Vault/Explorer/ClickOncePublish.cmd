@echo off
rem ===============================================================================================
rem Script to build and publish new VaultExplorer ClickOnce package
rem
rem Author: Eli Zeitlin (elize) 06/03/2016
rem ===============================================================================================

setlocal

set ClickOnceInstallUpdateUrl=\\avtest.redmond.corp.microsoft.com\scratch\elize\VaultExplorer\
set ManifestCertificateThumbprint=F0DD019529A68E0257DD9E412ED61219776EB546

msbuild /v:minimal /fl /flp:verbosity=normal /m /t:rebuild;publish /p:Configuration=Release;ClickOnceInstallUpdateUrl=%ClickOnceInstallUpdateUrl%;ManifestCertificateThumbprint=%ManifestCertificateThumbprint%

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

set LatestVersionLink=%ClickOnceInstallUpdateUrl%LatestVersion.lnk
if not exist %LatestVersionLink% (
    PowerShell "$s=(New-Object -COM WScript.Shell).CreateShortcut('%LatestVersionLink%');$s.TargetPath='%ClickOnceInstallUpdateUrl%VaultExplorer.application';$s.Save()"
)
      
start %windir%\explorer.exe "%ClickOnceInstallUpdateUrl%Application Files\"