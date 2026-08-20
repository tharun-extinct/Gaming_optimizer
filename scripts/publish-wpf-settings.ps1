param(
    [string]$Configuration = "Release",
    [string]$OutputDirectory = "$PSScriptRoot\..\target\release"
)

$project = Join-Path $PSScriptRoot "..\apps\EdgeOptimizer.Settings.Wpf\EdgeOptimizer.Settings.Wpf.csproj"

dotnet publish $project --configuration $Configuration --output $OutputDirectory
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

$settingsExecutable = Join-Path $OutputDirectory "EdgeOptimizer.Settings.Wpf.exe"
if (-not (Test-Path -LiteralPath $settingsExecutable)) {
    throw "WPF Settings publish did not produce $settingsExecutable."
}
