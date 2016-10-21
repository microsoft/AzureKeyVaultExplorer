@echo off

REM Copyright (c) Microsoft Corporation. All rights reserved. 
REM Licensed under the MIT License. See License.txt in the project root for license information. 

powershell -ExecutionPolicy Unrestricted -Command "iex \"^& '%~dp0\updatenuget.ps1' '%~dp0\nuget.exe'\""
