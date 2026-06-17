@echo off
rem Publica el backend GeoVial.WebApi a artifacts/webapi. Uso: publish-webapi.bat [Release^|Debug]
setlocal
set CONFIG=%1
if "%CONFIG%"=="" set CONFIG=Release
cd /d "%~dp0.."
echo Publicando GeoVial.WebApi (%CONFIG%)...
dotnet publish src/GeoVial.WebApi/GeoVial.WebApi.csproj -c %CONFIG% -o artifacts/webapi --nologo
exit /b %ERRORLEVEL%
