$dotnet = "C:\Program Files\dotnet\dotnet.exe"
$baseDir = Get-Location

Write-Host "Iniciando SexShop.API..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$baseDir\SexShop.API'; & '$dotnet' run --launch-profile https"

Write-Host "Iniciando SexShop.Web..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$baseDir\SexShop.Web'; & '$dotnet' run --launch-profile https"

Write-Host "Se han intentado abrir dos nuevas ventanas." -ForegroundColor Green
Write-Host "Si las ventanas se cierran inmediatamente, por favor dime qué error aparece en ellas." -ForegroundColor Yellow
Write-Host "URL Tienda: https://localhost:7137"
Write-Host "URL API: https://localhost:7005/swagger"
