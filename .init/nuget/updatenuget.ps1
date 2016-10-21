# Copyright (c) Microsoft Corporation. All rights reserved. 
# Licensed under the MIT License. See License.txt in the project root for license information. 

Invoke-WebRequest "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe" -OutFile $args[0]
