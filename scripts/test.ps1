param([switch]$Desktop)
$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
$exe = Join-Path $projectRoot 'artifacts\app\LiveDesktop.exe'
if (!(Test-Path -LiteralPath $exe)) { throw 'Run scripts/build.ps1 first.' }
$argument = if ($Desktop) { '--self-test' } else { '--render-test' }
$process = Start-Process -FilePath $exe -ArgumentList $argument -WindowStyle Hidden -Wait -PassThru
Get-Content (Join-Path $projectRoot 'artifacts\app\test-results\results.txt')
if ($process.ExitCode -ne 0) { throw "Tests failed (exit $($process.ExitCode))." }
