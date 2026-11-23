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
  "$h = $uri.Host;" ^
  "if ($h -ne 'localhost' -and (-not $h.Contains('.') -or $h.StartsWith('.') -or $h.EndsWith('.') -or $h.Contains('..'))) {" ^
  "  Write-Host ('Ten mien khong hop le (thieu duoi .com/.vn...): ' + $clean) -ForegroundColor Red;" ^
  "  Read-Host 'Nhan Enter de thoat';" ^
  "  exit 1" ^
  "};" ^
  "Write-Host 'Dang kiem tra link...' -ForegroundColor Gray;" ^
  "try { $null = [System.Net.Dns]::GetHostAddresses($h) } catch { Write-Host ('Ten mien khong ton tai (DNS loi): ' + $clean) -ForegroundColor Red; Read-Host 'Nhan Enter de thoat'; exit 1 };" ^
  "try {" ^
  "  $req = [System.Net.HttpWebRequest]::Create($uri);" ^
  "  $req.Method = 'HEAD'; $req.Timeout = 4000; $req.ReadWriteTimeout = 4000; $req.AllowAutoRedirect = $true; $req.UserAgent = 'Mozilla/5.0';" ^
  "  $resp = [System.Net.HttpWebResponse]$req.GetResponse();" ^
  "  $code = [int]$resp.StatusCode; $resp.Close();" ^
  "  if ($code -eq 404 -or $code -eq 410) { Write-Host ('Link khong ton tai (HTTP ' + $code + ')') -ForegroundColor Red; Read-Host 'Nhan Enter de thoat'; exit 1 }" ^
  "} catch [System.Net.WebException] {" ^
  "  $r = $_.Exception.Response;" ^
  "  if ($r -ne $null) { $code = [int]([System.Net.HttpWebResponse]$r).StatusCode; if ($code -eq 404 -or $code -eq 410) { Write-Host ('Link khong ton tai (HTTP ' + $code + ')') -ForegroundColor Red; Read-Host 'Nhan Enter de thoat'; exit 1 } }" ^
  "  else { Write-Host ('Trang web khong phan hoi (die/timeout): ' + $clean) -ForegroundColor Red; Read-Host 'Nhan Enter de thoat'; exit 1 }" ^
  "};" ^
  "Write-Host ('Dang mo: ' + $clean) -ForegroundColor Green;" ^
  "Start-Process $clean"
