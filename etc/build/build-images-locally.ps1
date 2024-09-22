param ($version='latest')

$currentFolder = $PSScriptRoot
$slnFolder = Join-Path $currentFolder "../../"
<#

Write-Host "********* BUILDING DbMigrator *********" -ForegroundColor Green
$dbMigratorFolder = Join-Path $slnFolder "src/IBLTermocasa.DbMigrator"
Set-Location $dbMigratorFolder
dotnet publish -c Release
docker build -f Dockerfile.local -t regisdtry.gitlab.com/ibl6810230/tcp/ibltermocasa-db-migrator:$version .
#>

Write-Host "********* BUILDING Blazor Application *********" -ForegroundColor Green
$blazorFolder = Join-Path $slnFolder "src/IBLTermocasa.Blazor"
Set-Location $blazorFolder
dotnet publish -c Release -p:PublishTrimmed=false
docker build -f Dockerfile.local -t ibldockeruser/blzr8523:$version .


Write-Host "********* BUILDING Api.Host Application *********" -ForegroundColor Green
$hostFolder = Join-Path $slnFolder "src/IBLTermocasa.HttpApi.Host"
Set-Location $hostFolder
dotnet publish -c Release
docker build -f Dockerfile.local -t ibldockeruser/host1235:$version .

$authServerAppFolder = Join-Path $slnFolder "src/IBLTermocasa.AuthServer"
Set-Location $authServerAppFolder
dotnet publish -c Release
docker build -f Dockerfile.local -t ibldockeruser/auth7856:$version .
<#
Write-Host "********* BUILDING Public Web Application *********" -ForegroundColor Green
$webPublicFolder = Join-Path $slnFolder "src/IBLTermocasa.Web.Public"
Set-Location $webPublicFolder
dotnet publish -c Release
docker build -f Dockerfile.local -t registry.gitlab.com/ibl6810230/tcp/ibltermocasa-web-public:$version .
#>



docker push ibldockeruser/blzr8523:$version
docker push ibldockeruser/host1235:$version
docker push ibldockeruser/auth7856:$version



### ALL COMPLETED
Write-Host "COMPLETED" -ForegroundColor Green
Set-Location $currentFolder