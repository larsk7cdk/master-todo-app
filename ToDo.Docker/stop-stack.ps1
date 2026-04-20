# ToDo Docker Compose Stop Script

param(
    [switch]$RemoveVolumes,
    [switch]$RemoveImages
)

Write-Host "🛑 Stopping ToDo Stack..." -ForegroundColor Cyan
Write-Host ""

# Navigate to Infrastructure directory
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptPath

if ($RemoveVolumes -and $RemoveImages) {
    Write-Host "⚠️  This will remove all containers, volumes, and images!" -ForegroundColor Yellow
    Write-Host "   All data will be lost!" -ForegroundColor Red
    $confirmation = Read-Host "Are you sure? (yes/no)"
    if ($confirmation -ne "yes") {
        Write-Host "❌ Cancelled" -ForegroundColor Yellow
        exit 0
    }
    docker compose down -v --rmi all
} elseif ($RemoveVolumes) {
    Write-Host "⚠️  This will remove all containers and volumes!" -ForegroundColor Yellow
    Write-Host "   All data will be lost!" -ForegroundColor Red
    $confirmation = Read-Host "Are you sure? (yes/no)"
    if ($confirmation -ne "yes") {
        Write-Host "❌ Cancelled" -ForegroundColor Yellow
        exit 0
    }
    docker compose down -v
} else {
    docker compose down
}

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "✅ All services stopped successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "🚀 Start services again:" -ForegroundColor Cyan
    Write-Host "   .\start-stack.ps1" -ForegroundColor Gray
    Write-Host ""
} else {
    Write-Host ""
    Write-Host "❌ Failed to stop services. Check the logs above for errors." -ForegroundColor Red
    exit 1
}

