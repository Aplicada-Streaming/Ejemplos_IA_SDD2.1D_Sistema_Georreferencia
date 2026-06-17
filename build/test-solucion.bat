@echo off
rem Ejecuta todas las pruebas de la solución. Uso: test-solucion.bat [Debug^|Release]
setlocal
set CONFIG=%1
if "%CONFIG%"=="" set CONFIG=Release
cd /d "%~dp0.."
echo Ejecutando pruebas de GeoVial.sln (%CONFIG%)...
dotnet test GeoVial.sln -c %CONFIG% --nologo
exit /b %ERRORLEVEL%
