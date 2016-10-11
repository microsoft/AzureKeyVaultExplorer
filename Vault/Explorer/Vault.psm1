# Copyright (c) Microsoft Corporation. All rights reserved. 
# Licensed under the MIT License. See License.txt in the project root for license information. 

<#
.SYNOPSIS
    Obtain Azure vault object reference from specified Vaults.json
    configuration file with single or dual name of the vault(s)

.DESCRIPTION
    Obtain Microsoft.Vault.Library.Vault object reference from specified
    Vaults.json configuration file with single or dual name of the vault(s)
    This function will also update $Global:CurrentAzureVault variable

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
    $accessType = [Microsoft.Vault.Library.VaultAccessTypeEnum]::ReadWrite

    # return the vault object (reference)
    $Global:CurrentAzureVault = New-Object Microsoft.Vault.Library.Vault @($vaultsConfigFile, $accessType, $firstVaultName, $secondVaultName)

    $title = "Vaults: $firstVaultName $secondVaultName"
    $host.ui.RawUI.WindowTitle = $title
    Write-Host $title -ForegroundColor Green

    return $Global:CurrentAzureVault 
}

<#
.SYNOPSIS
    Write Azure vault secret

.DESCRIPTION
    Write a single secret in Azure key vault with 
    associated metadata (properties/tags) to it

.NOTES
    Author: Eli Zeitlin
    Date:   October 10, 2016
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

function Get-VaultOrCurrent(
    [Microsoft.Vault.Library.Vault]$vault = $null
    )
{
    if ($vault -eq $null) {
        $vault = $Global:CurrentAzureVault
    }
    if ($vault -eq $null) {
        Write-Error 'Provided $vault object and $Global:CurrentAzureVault are both NULLs'
    }
    return $vault
}

<#
.SYNOPSIS
    Obtain Azure vault secrets 

.DESCRIPTION
    Obtain Azure vault secrets for the specified environment 
    (Geo, Environment)

.NOTES
    Author: Eli Zeitlin
    Date:   October 10, 2016
#>
function Get-AzureVaultSecrets(
    [Microsoft.Vault.Library.Vault]$vault = $null
    )
{
    $vault = Get-VaultOrCurrent($vault)

    # pull the list of secrets
    $secrets = $vault.ListSecretsAsync() | select -exp Result

    # compose and return the list
    $secrets | % { 

        New-Object psobject -prop ([ordered]@{ 

            # name of the secret
            Name = $_.Identifier.Name;

            # attributes of the secret
            Attributes = $_.Attributes;

            # content type
            ContentType = $_.ContentType;

            # secret tags
            Tags = $_.Tags;
        }) 
    }
}

# Register the assembly resolver
function Register-AssemblyResolver
{
    $path = Get-ChildItem -r -Path $PSScriptRoot 'Microsoft.Vault.Core.dll' | select -first 1
    
    if (-not $path)
    {
        throw "Assembly '$_' not found."
    }

    Add-type -Path $path.FullName
    
    $assemblyMap = New-Object "System.Collections.Generic.Dictionary[string,string]"
    $assemblyMap.Add("Newtonsoft.Json", "Newtonsoft.Json.dll")
    $assemblyMap.Add("Microsoft.Vault.Library", "Microsoft.Vault.Library.dll")
    
    [Microsoft.Vault.Core.AssemblyResolver]::Register($PSScriptRoot, $assemblyMap)
}

Register-AssemblyResolver

# Load dependent assemblies
$assemblies = @(
    'Newtonsoft.Json.dll',
    'Microsoft.Vault.Library.dll'
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