@echo off
rem Compila la solución completa de GeoVial. Uso: build-solucion.bat [Debug^|Release]
setlocal
set CONFIG=%1
if "%CONFIG%"=="" set CONFIG=Release
cd /d "%~dp0.."
echo Compilando GeoVial.sln (%CONFIG%)...
dotnet build GeoVial.sln -c %CONFIG% --nologo
exit /b %ERRORLEVEL%
