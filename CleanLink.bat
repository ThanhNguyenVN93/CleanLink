@echo off
title Clean Link - Open
chcp 65001 >nul
powershell -NoProfile -ExecutionPolicy Bypass -Command ^
  "$raw = Read-Host 'Dan link can xoa khoang trang roi nhan Enter';" ^
  "$clean = ($raw -replace '\s+', '');" ^
  "if ([string]::IsNullOrEmpty($clean)) { Write-Host 'Link trong!' -ForegroundColor Yellow; exit 1 };" ^
  "$uri = $null;" ^
  "if (-not [Uri]::TryCreate($clean, [UriKind]::Absolute, [ref]$uri) -or ($uri.Scheme -ne 'http' -and $uri.Scheme -ne 'https')) {" ^
  "  Write-Host ('Link khong hop le: ' + $clean) -ForegroundColor Red;" ^
  "  Read-Host 'Nhan Enter de thoat';" ^
  "  exit 1" ^
  "};" ^
  "Write-Host ('Dang mo: ' + $clean) -ForegroundColor Green;" ^
  "Start-Process $clean"
