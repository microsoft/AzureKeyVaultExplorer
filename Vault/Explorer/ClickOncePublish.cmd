@echo off
rem ===============================================================================================
rem Script to build and publish new VaultExplorer ClickOnce package
rem
rem Author: Eli Zeitlin (elize) 06/03/2016
rem ===============================================================================================

setlocal

if '%COMPUTERNAME%' EQU 'ELIZE-HP8540' (
    set ManifestCertificateThumbprint=14B804B917DE9AF55006443639A49BD5C0A42505
    set ClickOnceInstallUpdateUrl=\\ELIZE-HP8540\Temp\VaultExplorer\
    set Configuration=Debug
) else (
    set ManifestCertificateThumbprint=F0DD019529A68E0257DD9E412ED61219776EB546
    set ClickOnceInstallUpdateUrl=https://elize.blob.core.windows.net/vaultexplorer/
    rem set ClickOnceInstallUpdateUrl=\\avtest.redmond.corp.microsoft.com\scratch\elize\VaultExplorer\
    set Configuration=Debug
)

msbuild /v:minimal /fl /flp:verbosity=normal /m /t:rebuild;publish /p:Configuration=%Configuration%;ClickOnceInstallUpdateUrl=%ClickOnceInstallUpdateUrl%;ManifestCertificateThumbprint=%ManifestCertificateThumbprint%

if %ERRORLEVEL% NEQ 0 (
    echo Error: Build break!
    exit /b 1
)

set AppId=afe75cd6-6516-4fd8-938a-a3c8516966e5
set ApiKey=i3ff4xf3j7f1zq2mddrlls6zig88et1uqw7e602m
set ReleaseFilePath=.\bin\%Configuration%\app.publish\VaultExplorer.exe
rem powershell -ExecutionPolicy Unrestricted -Command "iex \"^& '%~dp0\CreateReleaseAnnotation.ps1' -applicationId '%AppId%' -apiKey '%ApiKey%' -releaseFilePath '%ReleaseFilePath%'\""

if %ERRORLEVEL% NEQ 0 (
    echo Error: Failed to create release annotation 
    exit /b 1
)

del %ReleaseFilePath%
if %ERRORLEVEL% NEQ 0 (
    echo Error: Failed to delete unnecessary file. This is not expected!
    exit /b 1
)

set AzCopy="%ProgramFiles(x86)%\Microsoft SDKs\Azure\AzCopy\AzCopy.exe"
if not exist %AzCopy% (
    echo Error: %AzCopy% is not found. Please install it from here http://aka.ms/azcopy
    exit /b 1
)

echo Uploading files from .\bin\%Configuration%\app.publish to %ClickOnceInstallUpdateUrl%
%AzCopy% /Source:.\bin\%Configuration%\app.publish /Dest:%ClickOnceInstallUpdateUrl% /DestKey:e44/rtNSwcbUoPMV73vA0ELecGk+N54lix+mTv4zEROfqkOWQmTyeVKyIAxkNkDUFa+f8vULMU722zDZ8mzMiQ== /S /Y
if %ERRORLEVEL% NEQ 0 (
    echo Error: Failed to upload from .\bin\%Configuration%\app.publish to %ClickOnceInstallUpdateUrl%
    exit /b 1
)

exit /b 1

set LatestVersionLink=%ClickOnceInstallUpdateUrl%LatestVersion.lnk
if not exist %LatestVersionLink% (
    PowerShell "$s=(New-Object -COM WScript.Shell).CreateShortcut('%LatestVersionLink%');$s.TargetPath='%ClickOnceInstallUpdateUrl%VaultExplorer.application';$s.Save()"
)
      
start %windir%\explorer.exe "%ClickOnceInstallUpdateUrl%Application Files\"