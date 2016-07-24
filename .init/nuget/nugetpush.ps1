# Stop on error
$errorActionPreference = 'Stop'

# Obtain the path to nuget.exe
$nuGetExe = (Join-Path $PSScriptRoot .\nuget.exe)

# Used to compose Debug and Release feeds
$nuGetPushTemplateFeed = "https://microsoft.pkgs.visualstudio.com/DefaultCollection/_packaging/WD.Services.App{0}/nuget/v3/index.json"

# Obtain current git branch
function Get-CurrentGitBranch
{
    (iex 'git name-rev --name-only HEAD') 2>&1
}

$branch = Get-CurrentGitBranch

# Check if fast-forward is possible between 
# commits $from and $to ($from -> $to)
function Get-IsGitFastForwardPossible(
    [string]$from, [string]$to)
{
    # prepare the command
    $cmd = "git merge-base --is-ancestor '$from' '$to'"

    # invoke the command
    (iex $cmd) 2>&1

    # exit code - (0) 'possible'; (1) 'not-possible'
    ($LastExitCode -eq 0)
}

# Check if git repo is clean - no untracked 
# and/or uncommitted changes present
function Get-IsGitRepoClean
{
    $c = (iex 'git status --porcelain') 2>&1

    # return the (bool) result
    -not $c
}

# Obtain parsed ('resolved') git (short) commit
function Get-GitCommitShortSha([string]$commit)
{
    (iex "git rev-parse --short $commit") 2>&1
}


# Check NuGet push pre-requisites
function Check-NuGetPushPrerequisites
{
    if (-not (Get-IsGitRepoClean))
    {
        $msg = 
@"
`n
    ***********************   ERROR   ****************************
    *                                                            *
    * This repository contains uncommitted or untracked changes. 
    * Use 'git status' command to list these changes and then    
    *                                                            
    * 'git commit', 'git checkout', 'git reset' etc. to commit   
    * (or revert) all changes (clean your repository)            
    *                                                            
    * Make sure your repository is clean and retry 'nugetpush'  
    *                                                            *
    **************************************************************
`n
"@
        Write-Error $msg
    }

    if (-not (Get-IsGitFastForwardPossible "HEAD" "origin/$branch"))
    {
        $head = Get-GitCommitShortSha 'HEAD'
        
        $remote = Get-GitCommitShortSha "origin/$branch"

        $msg = 
@"
`n
    ************************   ERROR   ***************************
    *                                                            *
    * The current (HEAD) commit ( $head ) of this repository    
    * cannot be 'fast-forwarded' to 'origin/$branch' ( $remote )   
    *                                                           
    * Use 'git pull' and 'git push' commands to sync with remote
    * and retry 'nugetpush'     
    *                                                            *
    **************************************************************
`n
"@
        Write-Error $msg
    }
}

# Build nuget packages for VSO feed
function Build-NuGetPackages
{
    iex "msbuild /v:minimal /fl /flp:verbosity=normal /m /p:BuildRelease=true"

    if ($LastExitCode -ne 0)
    {
        Write-Error "Build failed. "
    }
}

# Push nuget packages to VSO feed
function Push-NuGetPackages
{
    # Obtain the NuGet specs
    $nuspecs = Get-ChildItem .\bin\Release\*.nuspec

    if (-not $nuspecs)
    {
        Write-Warning "'nuspec' file(s) not found in '$project'; pushing will be skipped ..."
        return
    }

    $nuspecs | % {
        $nuspecXml = New-Object XML
        $nuspecXml.Load($_.FullName)
        
        # Obtain the package id and version
        $id = $nuspecXml.package.metadata.id
        $ver = $nuspecXml.package.metadata.version

        # jobs for parallel nuget push
        $jobs = @()
        ("Debug", "Release") | % {
            # Compose the feed
            $nugetPushFeed = [string]::Format($nuGetPushTemplateFeed, "")

            # Compose .nupkg filename
            $nupkgFileName = "$id.$ver.nupkg"

            if (-not (Test-Path .\.nupkg\$_\$nupkgFileName -PathType Leaf))
            {
                # Try "configuration specific" package path
                $nupkgFileName = "$id-$_.$ver.nupkg"
                $nugetPushFeed = [string]::Format($nuGetPushTemplateFeed, "-$_")
            }
            
            $nupkgFullPath = (Resolve-Path -Path .\.nupkg\$_\$nupkgFileName).Path

            # Compose the nuget.exe command
            $nuGetCmd = "& `"$nuGetExe`" push `"$nupkgFullPath`" -Source $nugetPushFeed -ApiKey WDApp"
            Write-Host " > '$nupkgFullPath' ($_) -> '$nugetPushFeed'"
            
            # Execute nuget push as a background job
            $jobs += Start-Job -ArgumentList $nuGetCmd -ScriptBlock `
            { 
                param($nuGetCmd)

                iex $nuGetCmd

                # Bail on error
                if ($LastExitCode -ne 0)
                {
                    Write-Error "Command '$nuGetCmd' failed. "
                }
            }
        }

        $r = Receive-Job $jobs -Wait -AutoRemoveJob
    }
}

# Check pre-requisites for NuGet push
if ($env:NUGET_PUSH_FORCE -ne 1)
{
    Check-NuGetPushPrerequisites
}

# Obtain the project name
$project = Split-Path . -Leaf

Write-Host
Write-Host "BUILDING - $project ..." -fore Yellow
Build-NuGetPackages

Write-Host
Write-Host "PUSHING - $project ..."  -fore Yellow
Push-NuGetPackages

