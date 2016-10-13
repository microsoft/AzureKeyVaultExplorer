# Copyright (c) Microsoft Corporation. All rights reserved. 
# Licensed under the MIT License. See License.txt in the project root for license information. 

param(
    [Parameter(Mandatory=$true)]
    [string]$vaultsJsonFile,
    [Parameter(Mandatory=$true)]
    [string]$firstVaultName,
    [Parameter(Mandatory=$false)]
    [string]$secondVaultName = ""
    )

$ErrorActionPreference = 'Stop'; 

$null = Test-Path -PathType Leaf -Path $vaultsJsonFile

Import-Module $PSScriptRoot\VaultModule.psm1

$null = Get-AzureVaultObject $vaultsJsonFile $firstVaultName $secondVaultName

Write-Host 
Write-Host ' -- Azure Key Vault Explorer commands -- '
Write-Host 
Get-Command -Name *-AzureVault*

# Set current directory to user's home path
Set-Location -Path $env:HOMEPATH