param(
    [Parameter(Mandatory=$true)]
    [string[]]$pkgs
    )

$errorActionPreference = 'Stop'

if (-not $env:RepoRoot)
{
    Write-Error "'%RepoRoot%' variable not defined. Run 'init.cmd' and try again. "
}

$pkgs | % {
    $pkg = $_

    cd $pkg

    iex "$($env:RepoRoot)\.init\nuget\nugetpush.cmd"
    if ($LASTEXITCODE -ne 0)
    {
        Write-Error "'$_' failed. "
    }

    cd ..
}

Write-Host
Write-Host "Done. " -fore Green

