@echo off
rem Construye la imagen de contenedor del backend. Uso: build-imagen-webapi.bat [tag]
rem Requiere Docker instalado y en ejecución.
setlocal
set TAG=%1
if "%TAG%"=="" set TAG=geovial-webapi:dev
cd /d "%~dp0.."
echo Construyendo imagen %TAG% desde deploy/Dockerfile.webapi...
docker build -f deploy/Dockerfile.webapi -t %TAG% .
exit /b %ERRORLEVEL%
