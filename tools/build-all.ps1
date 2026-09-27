# בונה את כל פרויקטי ה-.NET בקורס (Windows / PowerShell)
$ErrorActionPreference = "Continue"
Set-Location (Join-Path $PSScriptRoot "..")
$env:DOTNET_CLI_TELEMETRY_OPTOUT = 1
$fail = 0; $ok = 0
Get-ChildItem -Recurse -Filter *.csproj | Where-Object { $_.FullName -notmatch '\\(bin|obj|node_modules)\\' } | ForEach-Object {
  dotnet build $_.FullName -nologo -v q -clp:NoSummary | Out-Null
  if ($LASTEXITCODE -eq 0) { $ok++; Write-Host "OK    $($_.FullName)" } else { $fail++; Write-Host "FAIL  $($_.FullName)" -ForegroundColor Red }
}
Write-Host "---- built: $ok  failed: $fail"
exit $fail
