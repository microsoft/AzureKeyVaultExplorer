@echo off
setlocal

msbuild /v:minimal /fl /flp:verbosity=normal /m /p:Configuration=Release /t:rebuild;publish

if %ERRORLEVEL% NEQ 0 (
    echo Error: Build break!
    exit /b 1
)

