# Copyright (c) Microsoft Corporation. All rights reserved. 
# Licensed under the MIT License. See License.txt in the project root for license information. 

<#
.SYNOPSIS
    Obtain Azure vault object reference (low level)

.DESCRIPTION
    Obtain Azure vault (namespace) object reference
    for the specified environment (Geo, Environment)

.NOTES
    Author: Eli Zeitlin
    Date:   October 10, 2016
#>
function Get-AzureVaultObject(
    [Parameter(Mandatory=$true)]
    [string]$vaultsConfigFile,

    [Parameter(Mandatory=$true)]
    [string]$firstVaultName,

    [Parameter(Mandatory=$false)]
    [string]$secondVaultName = ""
    )
{
    # return the vault object (reference)
    $accessType = [VaultLibrary.VaultAccessTypeEnum]::ReadWrite

    New-Object VaultLibrary.Vault @($vaultsConfigFile, $accessType, $firstVaultName, $secondVaultName)
}

<#
.SYNOPSIS
    Write Azure vault secret

.DESCRIPTION
    Write a single secret in Azure key vault with 
    associated metadata (properties/tags) to it

.NOTES
    Author: Kalin Toshev
    Date:   May 12, 2016
#>
function Write-AzureVaultSecret(
    # vault object reference, obtained via Get-AzureVaultObject
    [Parameter(Mandatory=$true)]
    $vaultObject, 

    # secret name
    [Parameter(Mandatory=$true)]
    [string]$secretName,

    # secret value 
    [Parameter(Mandatory=$true)]
    [string]$secretValue, 

    # content type
    [string]$contentType
    )
{
    $tags = New-Object "System.Collections.Generic.Dictionary[string,string]"

    $tags.Add("ChangedBy", "$($env:UserDomain)\$($env:UserName)");
    $tags.Add("ChangedVia", "vault.psm1");

    # write the secret (synchronously)
    $vaultObject.SetSecretAsync($secretName, $secretValue, $tags, $contentType, $null, `
        ([Threading.CancellationToken]::None)) | select -exp Result
}

<#
.SYNOPSIS
    Obtain Azure vault secrets 

.DESCRIPTION
    Obtain Azure vault secrets for the specified environment 
    (Geo, Environment)

.NOTES
    Author: Kalin Toshev
    Date:   May 06, 2016
#>
function Get-AzureVaultSecrets(
    [ValidateSet('devus', 'ppeus', 'produs', 'prodau', 'prodeu', 'prodas', 'prodbr')]
    [string]$env = 'devus'
    )
{
    $vault = Get-AzureVaultObject -env $env

    # pull the list of secrets
    $secrets = $vault.ListSecretsAsync() | select -exp Result

    # compose and return the list
    $secrets | select -exp Identifier | select -exp Name | % { 

        $secret = $vault.GetSecretAsync($_).Result

        New-Object psobject -prop ([ordered]@{ 

            # name of the secret
            Name = $_; 

            # secret value
            Value = $secret.Value;

            # secret tags
            Tags = $secret.Tags;

            # content type
            ContentType = $secret.ContentType;
        }) 
    }
}

# Register the assembly resolver
function Register-AssemblyResolver
{
    $path = Get-ChildItem -r -Path $PSScriptRoot 'VaultLibrary.dll' | select -first 1
    
    if (-not $path)
    {
        throw "Assembly '$_' not found."
    }

    Add-type -Path $path.FullName
    
    $assemblyMap = New-Object "System.Collections.Generic.Dictionary[string,string]"
    $assemblyMap.Add("Newtonsoft.Json", "Newtonsoft.Json.dll")
    $assemblyMap.Add("VaultLibrary", "VaultLibrary.dll")
    
    [Microsoft.PS.Common.Powershell.AssemblyResolver]::Register($PSScriptRoot, $assemblyMap)
}

#Register-AssemblyResolver

# Load dependent assemblies
$assemblies = @(
    'Newtonsoft.Json.dll',
    'VaultLibrary.dll'
)

$assemblies | % {
    $path = Get-ChildItem -r -Path $PSScriptRoot $_ | select -first 1

    if (-not $path)
    {
        throw "Assembly '$_' not found."
    }

    Add-type -Path $path.FullName
}

Export-ModuleMember Get-AzureVaultObject,Write-AzureVaultSecret,Get-AzureVaultSecrets