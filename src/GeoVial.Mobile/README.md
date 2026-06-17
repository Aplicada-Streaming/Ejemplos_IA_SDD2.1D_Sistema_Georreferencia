# GeoVial.Mobile (geovial-mobile)

App de campo del agente: **.NET MAUI Blazor Hybrid + MudBlazor**, compatible **solo con Android**
(SOLUTION-MANIFEST, flag 5). Es el cliente de captura sin conexión y de sincronización.

## Estado actual

- Proyecto scaffoldeado y acotado a `net8.0-android`, con identidad de app GeoVial
  (`ar.gob.vialidad.geovial`).
- MudBlazor cableado: paquete, `AddMudServices()`, proveedores en `MainLayout` y estáticos en
  `wwwroot/index.html`.
- Referencia a **Aplicada.Sync** (motor de sincronización offline subir-luego-bajar): el host
  móvil implementará sus puertos `IAlmacenLocal` (SQLite) e `IBackendSincronizacion` (HTTP contra
  geovial-api `…/sincronizacion/subida` y `/bajada`).

## Por qué no está en `GeoVial.sln` todavía

El build de `net8.0-android` exige el manifiesto del workload `android` para el SDK pineado en
`global.json` (8.0.406). En el entorno de desarrollo actual ese workload está instalado por Visual
Studio en otra banda y `dotnet workload install/repair` requiere **elevación (UAC)**, que no está
disponible de forma desatendida. Para no romper el build verde de la solución, este proyecto queda
**fuera de `GeoVial.sln`** hasta reparar el workload.

## Cómo habilitar el build (una vez, con permisos de administrador)

```powershell
dotnet workload install android        # o: dotnet workload repair
$env:ANDROID_HOME = "C:\Program Files (x86)\Android\android-sdk"
dotnet build src/GeoVial.Mobile/GeoVial.Mobile.csproj -f net8.0-android
```

Luego, agregarlo a la solución:

```powershell
dotnet sln GeoVial.sln add src/GeoVial.Mobile/GeoVial.Mobile.csproj
```

## Próximos pasos (cuando el build esté habilitado)

1. Pantalla de login y sesión, reusando el contrato REST de geovial-api.
2. Lista de relevamientos asignados y captura de observaciones/fotos sin conexión.
3. Implementar `IAlmacenLocal` con SQLite y `IBackendSincronizacion` con `HttpClient`, y disparar
   `MotorSincronizacion.SincronizarAsync()` al recuperar conectividad (CU-04).
