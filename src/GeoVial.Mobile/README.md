# GeoVial.Mobile (geovial-mobile)

App de campo del agente: **.NET MAUI Blazor Hybrid + MudBlazor**, compatible **solo con Android**
(SOLUTION-MANIFEST, flag 5). Es el cliente de captura sin conexión y de sincronización.

## Estado actual

- Proyecto scaffoldeado y acotado a `net8.0-android`, con identidad de app GeoVial
  (`ar.gob.vialidad.geovial`).
- MudBlazor cableado: paquete, `AddMudServices()`, proveedores en `MainLayout` y estáticos en
  `wwwroot/index.html`.
- **Flujo de la app**: `Login` (sesión contra geovial-api) y `Relevamientos` (lista de los
  relevamientos del agente), con `EstadoSesion` y un `ClienteApi` tipado (`Servicios/`). `Home`
  redirige según haya sesión.
- Referencia a **Aplicada.Sync** (motor de sincronización offline subir-luego-bajar): el host
  móvil implementará sus puertos `IAlmacenLocal` (SQLite) e `IBackendSincronizacion` (HTTP contra
  geovial-api `…/sincronizacion/subida` y `/bajada`) en el incremento de captura.
- `ApiBaseUrl` por defecto `http://10.0.2.2:5080/` (loopback del host desde el emulador Android).

> ⚠️ **Verificación**: el código de la app espeja los patrones ya verificados de geovial-web,
> pero **no pudo compilarse por línea de comandos en este entorno** (ver abajo). Compilar/ejecutar
> desde Visual Studio o tras reparar el workload.

## Por qué no está en `GeoVial.sln` todavía

El build de `net8.0-android` con `UseMaui` exige el workload **`maui-android`** para el SDK pineado
en `global.json` (8.0.406). En esta máquina los workloads están instalados por Visual Studio en la
banda 8.0.100, y el SDK CLI 8.0.406 (banda 8.0.400) no los resuelve:

- `dotnet build -f net8.0-android` → `NETSDK1147: deben estar instaladas: maui-android`.
- `dotnet workload install maui-android` → falla con `Object reference not set…` y revierte los
  MSI (estado de workloads inconsistente entre bandas VS/CLI).

Por eso el proyecto queda **fuera de `GeoVial.sln`** (para no romper el build verde de la solución).

## Cómo habilitar el build

La vía más confiable en esta máquina es **compilar desde Visual Studio 2022** (usa sus propios
workloads). Por línea de comandos, primero hay que dejar consistente el estado de workloads:

```powershell
# Con permisos de administrador:
dotnet workload update                  # alinea manifiestos a la banda del SDK 8.0.406
dotnet workload install maui-android    # o: dotnet workload install maui
$env:ANDROID_HOME = "C:\Program Files (x86)\Android\android-sdk"
dotnet build src/GeoVial.Mobile/GeoVial.Mobile.csproj -f net8.0-android
```

Una vez que compile, agregarlo a la solución:

```powershell
dotnet sln GeoVial.sln add src/GeoVial.Mobile/GeoVial.Mobile.csproj
```

## Próximos pasos (cuando el build esté habilitado)

1. ✅ Pantalla de login y sesión, reusando el contrato REST de geovial-api.
2. ✅ Lista de relevamientos asignados del agente.
3. Captura de observaciones/fotos sin conexión (encolar como `CambioLocal` en `Aplicada.Sync`).
4. Implementar `IAlmacenLocal` con SQLite y `IBackendSincronizacion` con `HttpClient`, y disparar
   `MotorSincronizacion.SincronizarAsync()` al recuperar conectividad (CU-04).
