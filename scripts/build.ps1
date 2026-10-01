param([switch]$Installer)
$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path -Parent $PSScriptRoot
$appOutput = Join-Path $projectRoot 'artifacts\app'
& dotnet publish (Join-Path $projectRoot 'LiveDesktop.csproj') -c Release -r win-x64 --self-contained true -o $appOutput
if ($LASTEXITCODE -ne 0) { throw 'dotnet publish failed.' }
if ($Installer) {
    $compiler = Get-Command ISCC.exe -ErrorAction SilentlyContinue
    $compilerPath = if ($compiler) { $compiler.Source } else { Join-Path ${env:ProgramFiles(x86)} 'Inno Setup 6\ISCC.exe' }
    if (!(Test-Path -LiteralPath $compilerPath)) { throw 'Install Inno Setup 6 or add ISCC.exe to PATH.' }
    & $compilerPath (Join-Path $projectRoot 'installer\LiveDesktop.iss')
    if ($LASTEXITCODE -ne 0) { throw 'Installer compilation failed.' }
}
