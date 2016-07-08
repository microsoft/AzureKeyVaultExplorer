@echo off
rem ===============================================================================================
rem Script to build and publish new VaultExplorer ClickOnce package
rem
rem Author: Eli Zeitlin (elize) 06/03/2016
rem ===============================================================================================

setlocal

rem set ClickOnceInstallUpdateUrl=\\elizedev\Temp\VaultExplorer\
set ClickOnceInstallUpdateUrl=\\avtest.redmond.corp.microsoft.com\scratch\elize\VaultExplorer\
set ManifestCertificateThumbprint=F0DD019529A68E0257DD9E412ED61219776EB546
set Configuration=Release

msbuild /v:minimal /fl /flp:verbosity=normal /m /t:rebuild;publish /p:Configuration=%Configuration%;ClickOnceInstallUpdateUrl=%ClickOnceInstallUpdateUrl%;ManifestCertificateThumbprint=%ManifestCertificateThumbprint%

if %ERRORLEVEL% NEQ 0 (
    echo Error: Build break!
    exit /b 1
)

set AppId=afe75cd6-6516-4fd8-938a-a3c8516966e5
set ApiKey=i3ff4xf3j7f1zq2mddrlls6zig88et1uqw7e602m
set ReleaseFilePath=.\bin\%Configuration%\app.publish\VaultExplorer.exe
powershell -ExecutionPolicy Unrestricted -Command "iex \"^& '%~dp0\CreateReleaseAnnotation.ps1' -applicationId '%AppId%' -apiKey '%ApiKey%' -releaseFilePath '%ReleaseFilePath%'\""

if %ERRORLEVEL% NEQ 0 (
    echo Error: Failed to create release annotation 
    exit /b 1
)

del %ReleaseFilePath%
if %ERRORLEVEL% NEQ 0 (
    echo Error: Failed to delete unnecessary file. This is not expected!
    exit /b 1
)

echo Copying files from .\bin\%Configuration%\app.publish to %ClickOnceInstallUpdateUrl%
xcopy .\bin\%Configuration%\app.publish %ClickOnceInstallUpdateUrl% /S /Y /Q
if %ERRORLEVEL% NEQ 0 (
    echo Error: Failed to copy from .\bin\%Configuration%\app.publish to %ClickOnceInstallUpdateUrl%
    exit /b 1
)

set LatestVersionLink=%ClickOnceInstallUpdateUrl%LatestVersion.lnk
if not exist %LatestVersionLink% (
    PowerShell "$s=(New-Object -COM WScript.Shell).CreateShortcut('%LatestVersionLink%');$s.TargetPath='%ClickOnceInstallUpdateUrl%VaultExplorer.application';$s.Save()"
)
      
start %windir%\explorer.exe "%ClickOnceInstallUpdateUrl%Application Files\"