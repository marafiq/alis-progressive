#!/usr/bin/env pwsh
# Update all Jamidon namespaces to Alis.Progressive

$ErrorActionPreference = "Stop"

Write-Host "Updating namespaces..." -ForegroundColor Cyan

# Update Controllers
Get-ChildItem -Path "src/Alis.Progressive.SandboxApp/Controllers" -Filter "*.cs" -Recurse | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    $content = $content -replace 'using Jamidon\.Models;', 'using Alis.Progressive.SandboxApp.Models;'
    $content = $content -replace 'namespace Jamidon\.Controllers', 'namespace Alis.Progressive.SandboxApp.Controllers'
    Set-Content $_.FullName -Value $content -NoNewline
    Write-Host "  Updated: $($_.Name)" -ForegroundColor Gray
}

# Update Views
Get-ChildItem -Path "src/Alis.Progressive.SandboxApp/Views" -Filter "*.cshtml" -Recurse | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    $content = $content -replace '@model Jamidon\.Models\.', '@model Alis.Progressive.SandboxApp.Models.'
    $content = $content -replace '@using Jamidon\.Models', '@using Alis.Progressive.SandboxApp.Models'
    $content = $content -replace '@addTagHelper \*, Jamidon', '@addTagHelper *, Alis.Progressive.TagHelpers'
    Set-Content $_.FullName -Value $content -NoNewline
    Write-Host "  Updated: $($_.Name)" -ForegroundColor Gray
}

Write-Host "✓ Namespaces updated" -ForegroundColor Green

