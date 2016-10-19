@echo off

REM 
REM Setup minimal command line environment, based on the root location
REM
set RepoRoot=%~dp0

REM 
REM Aliases
REM
doskey root=pushd "%RepoRoot%"

set NUGET_EXE_PATH="%~dp0.init\nuget\nuget.exe"
doskey nuget=%NUGET_EXE_PATH% $*

set MSBUILD_BASE_ARGS=/v:minimal /fl /flp:verbosity=normal /m
doskey bz=msbuild %MSBUILD_BASE_ARGS% $*
doskey br=msbuild %MSBUILD_BASE_ARGS% /p:Configuration=Release $*
doskey bzr=msbuild %MSBUILD_BASE_ARGS% /p:BuildRelease=true $*
doskey bc=msbuild %MSBUILD_BASE_ARGS% /p:BuildRelease=true /t:Clean $*

REM NuGet package build
doskey bp=%~dp0.init\nuget\bp.cmd $*

REM 
REM Git configuration
REM
git config include.path "%~dp0.gitconfig"

REM 
REM NuGet push macro
REM
doskey nugetpush=%~dp0.init\nuget\nugetpush.cmd

set NUGET_CREDENTIALPROVIDERS_PATH=%~dp0.packages

REM 
REM Invoke the main initialization script
REM
powershell -ExecutionPolicy Unrestricted -Command "iex \"^& '%~dp0\.init\init.ps1'\""

REM 
REM Run custom environment init script for the repository (if any)
REM
if EXIST "%RepoRoot%\init.custom.cmd" (
    call "%RepoRoot%\init.custom.cmd"
) else (
    echo .
)


