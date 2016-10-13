# Copyright (c) Microsoft Corporation. All rights reserved. 
# Licensed under the MIT License. See License.txt in the project root for license information. 

# Internal helper function
function GetCurrentVaultObject(
    [Microsoft.Vault.Library.Vault]$vaultObject = $null
    )
{
    if ($vaultObject -eq $null) {
        $vaultObject = $Global:CurrentAzureVaultObject
    }
    if ($vaultObject -eq $null) {
        Write-Error 'Provided $vault object and $Global:CurrentAzureVaultObject are both NULLs. Did you forgot to call Get-AzureVaultObject()?'
    }
    return $vaultObject
}

# Convert tags hashtable to Dictionary<string, string>
function HashtableToDictionary(
    [Parameter(Mandatory=$true)]
    [hashtable] $h
    )
{
    $result = New-Object "System.Collections.Generic.Dictionary[string,string]"

    $h.Keys | % { $result.Add($_.ToString(), $h.Item($_).ToString()) } 

    $result
}

<#
.SYNOPSIS
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

    # update global object for this session and return the vault object (reference)
    $Global:CurrentAzureVaultObject = New-Object Microsoft.Vault.Library.Vault @($vaultsConfigFile, $accessType, $firstVaultName, $secondVaultName)

    $title = "Vaults: $firstVaultName $secondVaultName"
    $host.ui.RawUI.WindowTitle = $title

    return $Global:CurrentAzureVaultObject 
}

<#
.SYNOPSIS
    List all Azure vault secrets

.EXAMPLE
    Get-AzureVaultSecrets

.NOTES
    Author: Eli Zeitlin
    Date:   October 10, 2016
#>
function Get-AzureVaultSecrets(
    # vault object reference, obtained via Get-AzureVaultObject
    [Microsoft.Vault.Library.Vault]$vaultObject = $null
    )
{
    $vaultObject = GetCurrentVaultObject($vaultObject)

    # pull the list of secrets
    $secrets = $vaultObject.ListSecretsAsync() | select -exp Result

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

<#
.SYNOPSIS
    Get Azure vault secret value

.EXAMPLE
    Get-AzureVaultSecretValue secretName

.NOTES
    Author: Eli Zeitlin
    Date:   October 10, 2016
#>
function Get-AzureVaultSecretValue(
    # secret name
    [Parameter(Mandatory=$true)]
    [string]$secretName,

    # version of the secret (optional)
    [string]$secretVersion = "",

    # vault object reference, obtained via Get-AzureVaultObject
    [Microsoft.Vault.Library.Vault]$vaultObject = $null
    )
{
    $vaultObject = GetCurrentVaultObject($vaultObject)

    # pull the list of secrets
    $secret = $vaultObject.GetSecretAsync($secretName, $secretVersion) | select -exp Result

    # compose and return new object
    New-Object psobject -prop ([ordered]@{ 

        # name of the secret
        Name = $secret.SecretIdentifier.Name;

        # value of the secret
        Value = $secret.Value;

        # attributes of the secret
        Attributes = $secret.Attributes;

        # content type
        ContentType = $secret.ContentType;

        # secret tags
        Tags = $secret.Tags;
    }) 
}

<#
.SYNOPSIS
    Set Azure vault secret with value, tags, content type and other attributes

.EXAMPLE
    Set-AzureVaultSecret secretName secretValue @{'key1'='value1';'key2'='value2'} text/plain

.NOTES
    Author: Eli Zeitlin
    Date:   October 10, 2016
#>
function Set-AzureVaultSecret(
    # secret name
    [Parameter(Mandatory=$true)]
    [string]$secretName,

    # secret value 
    [Parameter(Mandatory=$true)]
    [string]$secretValue,

    # tags
    [hashtable]$tags,

    # content type
    [string]$contentType,

    # attributes for the secret
    [Microsoft.Azure.KeyVault.Models.SecretAttributes] $secretAttributes,

    # vault object reference, obtained via Get-AzureVaultObject
    [Microsoft.Vault.Library.Vault]$vaultObject = $null
    )
{
    $vaultObject = GetCurrentVaultObject($vaultObject)

    $tagsAsDictionary = HashtableToDictionary $tags

    # write the secret (synchronously)
    $vaultObject.SetSecretAsync($secretName, $secretValue, $tagsAsDictionary, $contentType, $null, `
        ([Threading.CancellationToken]::None)) | select -exp Result
}

<#
.SYNOPSIS
    Updates the attributes associated with the specified secret in Azure key vault

.EXAMPLE
    Update-AzureVaultSecret secretName @{'key1'='value1';'key2'='value2'} text/plain

.NOTES
    Author: Eli Zeitlin
    Date:   October 10, 2016
#>
function Update-AzureVaultSecret(
    # secret name
    [Parameter(Mandatory=$true)]
    [string]$secretName,

    # tags
    [hashtable]$tags,

    # content type
    [string]$contentType,

    # attributes for the secret
    [Microsoft.Azure.KeyVault.Models.SecretAttributes] $secretAttributes,

    # secret version (optional)
    [string]$secretVersion = "",

    # vault object reference, obtained via Get-AzureVaultObject
    [Microsoft.Vault.Library.Vault]$vaultObject = $null
    )
{
    $vaultObject = GetCurrentVaultObject($vaultObject)

    $tagsAsDictionary = HashtableToDictionary $tags

    # write the secret (synchronously)
    $vaultObject.UpdateSecretAsync($secretName, $secretVersion, $tagsAsDictionary, $contentType, $secretAttributes, `
        ([Threading.CancellationToken]::None)) | select -exp Result
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
    # For backward compatibility reasons, to be able to deserialize old Vaults.json we resolve the assembly and point to our new Microsoft.Vault.Library.dll
    $assemblyMap.Add("Microsoft.PS.Common.Vault", "Microsoft.Vault.Library.dll")
    
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

Export-ModuleMember Get-AzureVaultObject,Get-AzureVaultSecrets,Get-AzureVaultSecretValue,Set-AzureVaultSecret,Update-AzureVaultSecret

