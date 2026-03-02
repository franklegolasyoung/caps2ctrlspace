# Build single-file release
param(
    [switch]$SelfContained,
    [string]$Output = "publish"
)

$args = @(
    "publish"
    "-c", "Release"
    "-r", "win-x64"
    "-p:PublishSingleFile=true"
    "-o", $Output
)

if ($SelfContained) {
    $args += "--self-contained", "true"
    $args += "-p:IncludeNativeLibrariesForSelfExtract=true"
    Write-Host "Building self-contained single file..." -ForegroundColor Cyan
} else {
    $args += "--self-contained", "false"
    Write-Host "Building framework-dependent single file..." -ForegroundColor Cyan
}

dotnet @args

if ($LASTEXITCODE -eq 0) {
    Write-Host "`nBuild succeeded! Output: $Output/" -ForegroundColor Green
} else {
    Write-Host "`nBuild failed!" -ForegroundColor Red
}
