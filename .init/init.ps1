# Stop on error
$errorActionPreference = 'Stop'

# Common NuGet package feeds (scanned in this order)
$pkgFeeds = @(
    # NuGet official feed
    "nuget.org",

    # WD Services (External)
    "WD.Services.External",

    # WD Services (App)
    "WD.Services.App",

    # WD Services (App-Debug)
    "WD.Services.App-Debug"
)

# Obtain the path to nuget.exe
$nuGetExe = (Join-Path $PSScriptRoot nuget\nuget.exe)

# Pick %AppData%\NuGet\NuGet.config when running from build agent, it has <packageSourceCredentials> to access all our VSO NuGet feeds,
# otherwise pick the .\NuGet\NuGetPS.config and rely on VisualStudio.Services.NuGet.CredentialProvider to authenticate to our VSO NuGet feeds
if ($env:USERNAME -eq 'buildagent')
{
    $nuGetCfg = (Join-Path $env:APPDATA nuget\NuGet.Config)
}
else
{
    $nuGetCfg = (Join-Path $PSScriptRoot nuget\NuGetPS.Config)
}

# NuGet (restored) packages root
$nuGetPackageRoot = Join-Path $PSScriptRoot '..\.packages'

if (-not (Test-Path $nuGetPackageRoot -PathType Container))
{
    [void] (mkdir "$nuGetPackageRoot")
}

$nuGetPackageRoot = Resolve-Path $nuGetPackageRoot

# NuGet (restored) packages root
$nuGetPackagePropertyFile = (Join-Path $nuGetPackageRoot 'Packages.props')


#
# BEGIN FUNCTIONS
#

function Install-NuGetVsoAuthProvider
{
    $nuGetCmd = "& `"$nuGetExe`" install Microsoft.VisualStudio.Services.NuGet.CredentialProvider -Source https://api.nuget.org/v3/index.json -OutputDirectory `"$nuGetPackageRoot`""
    iex $nuGetCmd

    # bail on error
    if ($LASTEXITCODE -ne 0)
    {
        Write-Error "Command '$nuGetCmd' failed. "
    }
}

# Restore NuGet packages, listed in packages.config
function Restore-NuGetPackages(
    [string]$packageConfig = "$PSScriptRoot\packages.config"
    )
{
    # obtain package sources (for nuget.exe)
    $pkgSources += ($pkgFeeds | % { "-Source `"$_`"" } ) -join ' '

    # pre-install the VSO credential provider
    Install-NuGetVsoAuthProvider

    # obtain packages.config (currently a single item)
    $pkgConfigFiles = @($packageConfig)

    $pkgConfigFiles | % {

        $pkgConfigFile = $_

        $pkgDir = [IO.Path]::GetDirectoryName($pkgConfigFile); Set-Location $pkgDir

        # compose the nuget.exe command and execute it
        $nuGetCmd = "& `"$nuGetExe`" restore -MSBuildVersion 14.0 -ConfigFile `"$nuGetCfg`" -Verbosity detailed $pkgSources -PackagesDirectory `"$nuGetPackageRoot`""
        iex $nuGetCmd

        # bail on error
        if ($LASTEXITCODE -ne 0)
        {
            Write-Error "Command '$nuGetCmd' failed. "
        }
    }
}

# Obtains package config collection, based on NuGet 'packages.config'
function Get-NuGetPackageConfig(
    [string]$packageConfig = "$PSScriptRoot\packages.config"
    )
{
    [xml]$root = (Get-Content -Raw $packageConfig)

    # return all packages as an array of package objects
    $root.packages.package
}

# Generate package MSBuild property file ('Packages.props')
function New-NuGetPackagePropertyFile(
    [Parameter(ValueFromPipeline=$true, Mandatory=$true)]
    $packageGroup
    )
{
    begin
    {
        # regex (for replacing special chars)
        $regex = New-Object System.Text.RegularExpressions.Regex "[.-]"

        # xml writer (output)
        $xws = New-Object System.Xml.XmlWriterSettings

        $xws.Encoding = [Text.Encoding]::UTF8
        $xws.OmitXmlDeclaration = $true
        $xws.Indent = $true
        
        $xw = [Xml.XmlWriter]::Create($nuGetPackagePropertyFile, $xws);

        # msbuild xml namespace
        $ns = "http://schemas.microsoft.com/developer/msbuild/2003";

        # MSBuild Project, PropertyGroup elements
        $xw.WriteStartElement("Project", $ns);

        $xw.WriteComment(" Auto-generated based on packages.config; do not modify by hand! ");
        $xw.WriteStartElement("PropertyGroup", $ns);
        
        $xw.WriteComment(" Packages root ");
        $xw.WriteElementString("PackagesRoot", $ns, '$(RepositoryRoot)\.packages');
        $xw.WriteComment(" Packages sorted by name and version ");
    }

    process 
    {
        # define element render worker
        $renderWorker = {
            if (-not $cfg)
            {
                $xw.WriteElementString($elementName, $ns, $elementValue);
            }
            else
            {
                $xw.WriteStartElement($elementName, $ns);
                $xw.WriteAttributeString("Condition", "'`$(Configuration)' == '$cfg'");
                $xw.WriteString($elementValue);
                $xw.WriteEndElement();
            }
        }

        $packageGroup.Group | % {

            # capture the id and version
            $id = $_.id
            $ver = $_.version
            $cfg = $_.configuration

            # transform the id and ver
            $eid = $id
            if ($cfg)
            {
                # strip the configuration flavor from the package id
                $eid = $eid.Substring(0, $eid.Length - ".$cfg".Length);
            }

            $rid = $regex.Replace($eid, "_")
            $rver = $regex.Replace($ver, "_")
         
            $elementName = "Pkg$($rid)_$($rver)"
            $elementValue = '$(PackagesRoot)\' + "$id.$ver"

            & $renderWorker
        }

        # write the ver independent element for each group
        $elementName = "Pkg$($rid)"

        & $renderWorker
    }

    end
    {
        # close the PropertyGroup and Project elements
        $xw.WriteEndElement();

        $xw.WriteEndElement();

        # close the xml writer
        $xw.Close();
    }
}

# Obtains fingerprint of packages.config, if changed
function Get-PackagesConfigFingerPrintIfChanged
{
    # obtain the fingerprint of the packages.config
    $fpPackagesConfig = (Get-FileHash "$PSScriptRoot\packages.config" -Algorithm MD5).Hash
    # return the fingerprint in case packages.props doesn't exist
    if (-not (Test-Path $nuGetPackagePropertyFile -PathType Leaf))
    {
        return $fpPackagesConfig
    }

    if (Test-Path $fpFileName -PathType Leaf)
    {
        # obtain the current (stored) fingerprint
        $fp = (Get-Content -Raw $fpFileName)

        if ($fp -eq $fpPackagesConfig)
        {
            # no change
            return $null
        }
    }

    # return the fingerprint of the (current) packages.config
    $fpPackagesConfig
}

#
# END FUNCTIONS
#

# Grab the init mutex
$initMutex = New-Object System.Threading.Mutex($false, 'init-09b304a4-57ae-4046-bfad-69d056be8a78')
[Void] $initMutex.WaitOne()

try
{
    # File name of the finderprint file
    $fpFileName = (Join-Path $nuGetPackageRoot .fingerprint)

    $fp = Get-PackagesConfigFingerPrintIfChanged
       
    if ($fp)
    {
        # Restore the NuGet packages
        Write-Host
        Write-Host "init: Restoring NuGet packages to '$nuGetPackageRoot' ..." -ForegroundColor Yellow
        Restore-NuGetPackages

        # Obtain package groups
        $packageGroups = @(Get-NuGetPackageConfig | sort id, version | group id)

        # Generate MSBuild property file
        Write-Host
        Write-Host "init: Generating '$nuGetPackagePropertyFile' ..." -ForegroundColor Yellow
        $packageGroups.GetEnumerator() | New-NuGetPackagePropertyFile

        # Write the new fingerprint
        [IO.File]::WriteAllText($fpFileName, $fp);

        Write-Host
        Write-Host "init: Done. " -ForegroundColor Yellow
    }
    else
    {
        Write-Host
        Write-Host "init: No changes detected. " -ForegroundColor Yellow
    }
}
finally
{
    # Note: we do this to prevent abandoned mutex exception in another thread
    $initMutex.ReleaseMutex()
}

