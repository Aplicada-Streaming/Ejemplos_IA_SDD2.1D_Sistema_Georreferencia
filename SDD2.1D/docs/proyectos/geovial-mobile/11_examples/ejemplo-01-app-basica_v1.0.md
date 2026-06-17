# Ejemplo 01 — App básica: arranque, sesión y selección de relevamiento

**Proyecto:** geovial-mobile
**Documento:** ejemplo-01-app-basica_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Sample Engineer (mobile)
**Nivel:** Básico
**Ubicación del código:** `/samples/geovial-mobile/01-app-basica/`

## 1. Objetivo del sample

Demostrar el camino feliz mínimo de la app de campo: arrancar la app, iniciar sesión como agente con credenciales, navegar a la lista de relevamientos asignados y abrir uno como contexto de trabajo, todo sobre datos mock y sin un backend real. Al terminar, el desarrollador sabe cómo la app resuelve la identidad del agente, custodia el token en el almacenamiento seguro del dispositivo, ofrece el relogueo por la seguridad del dispositivo cuando la app se reinicia y fija un relevamiento asignado como contexto activo de captura. Es la base sobre la que el sample 02 agrega la captura offline y la sincronización.

## 2. Nivel

Básico. Es el punto de entrada absoluto del proyecto: no asume haber leído ningún sample previo. Ejercita el ciclo de sesión y la navegación inicial contra datos mock, sin captura, sin almacén local de observaciones y sin sincronización. Deja para el sample 02 (sync offline) la captura de marcadores y fotos, la cola local de cambios y el ciclo subir-luego-bajar.

## 3. Prerequisites

- El proyecto de la app móvil incorporado desde el repositorio, en la versión vigente del producto.
- Herramientas de construcción del ecosistema móvil, en la versión mínima declarada en el SOLUTION-INTAKE §17 P.9 del proyecto geovial-mobile (plataforma target Android, versión mínima Android API 26).
- Un emulador o un dispositivo Android conectado por USB en modo desarrollador, con la seguridad del sistema operativo (patrón o equivalente) configurada para poder probar el relogueo.
- Datos mock incluidos en el propio sample: un agente de prueba con credenciales fijas y un conjunto de relevamientos asignados precargados; no requiere backend real ni conexión.

## 4. Cómo correrlo

1. Posicionarse en la carpeta del sample: `cd samples/geovial-mobile/01-app-basica`.
2. Restaurar las dependencias del sample con el gestor de paquetes del ecosistema.
3. Desplegar la app en el emulador o en el dispositivo Android conectado con el comando de arranque del sample.
4. En la pantalla de inicio de sesión, ingresar las credenciales del agente de prueba que indica el README del sample y confirmar.
5. Abrir la lista de relevamientos asignados, seleccionar uno y comparar la navegación resultante con el output esperado de §6.

## 5. Estructura del código

```
01-app-basica/
├── README.md                          # Qué demuestra el sample y cómo correrlo
├── src/
│   ├── App.<ext>                       # Arranque de la app y arranque en frío hasta la pantalla de sesión
│   ├── presentacion/
│   │   ├── PantallaSesion.<ext>        # Inicio de sesión, deslogueo y relogueo por seguridad del dispositivo
│   │   └── PantallaRelevamientos.<ext> # Lista de relevamientos asignados y selección de contexto activo
│   ├── aplicacion/
│   │   ├── ServicioSesion.<ext>        # Orquesta inicio, relogueo y deslogueo; verificable sin interfaz
│   │   └── ContextoActivo.<ext>        # Fija el relevamiento seleccionado como contexto de captura
│   ├── infraestructura/
│   │   ├── AlmacenSeguroMock.<ext>     # Almacén seguro de prueba para el token del dispositivo
│   │   ├── AutenticacionMock.<ext>     # Backend de autenticación simulado con credenciales fijas
│   │   └── RelevamientosMock.<ext>     # Datos mock de relevamientos asignados precargados
│   └── datos/
│       └── relevamientos-mock.json     # Conjunto de relevamientos asignados de demostración
└── tests/
    ├── sesion_test.<ext>               # Inicio guarda token; relogueo sin credenciales; deslogueo borra sesión
    └── seleccion_relevamiento_test.<ext> # Selección fija contexto activo y abre la vista del relevamiento
```

## 6. Qué esperar

Tras el inicio de sesión y la selección de un relevamiento, la traza de la app en consola se ve así, en este orden:

```
Arranque en frio completado en 1.8 s. Sin sesion guardada: mostrando inicio de sesion.
Inicio de sesion en linea: credenciales aceptadas. Token guardado en almacen seguro del dispositivo.
Sesion activa. Agente: agente-demo. Trabajo de campo habilitado.
Relevamientos asignados (mock): 3 cargados desde el almacen local.
  [1] Tramo Norte        estado: recoleccion
  [2] Puente Km 12       estado: recoleccion
  [3] Camino del Bajo    estado: cerrado
Seleccion: [2] Puente Km 12. Contexto activo fijado. Abriendo vista del relevamiento.
```

Al cerrar y reabrir la app sin desloguear, con una sesión activa guardada, la traza esperada es:

```
Arranque en frio completado en 1.6 s. Sesion activa guardada: solicitando verificacion por seguridad del dispositivo.
Verificacion por seguridad del dispositivo: confirmada. Acceso rehabilitado sin reingreso de credenciales.
```

Al ejecutar el deslogueo completo, la traza esperada es:

```
Deslogueo completo: token y datos de sesion borrados del dispositivo. Volviendo al inicio de sesion.
```

## 7. Variaciones sugeridas

| Variación | Qué cambiar | Resultado |
| --- | --- | --- |
| Inicio sin conexión la primera vez | Arrancar el sample con el modo sin conexión activo antes del primer inicio | La app responde con `SIN_CONEXION_INICIO`, no crea sesión y explica que el primer inicio requiere conexión |
| Cambio de usuario en el dispositivo | Tras una sesión activa, intentar reloguear como un segundo agente sin deslogueo previo | La app no da acceso sobre la sesión ajena y exige deslogueo completo antes de un nuevo inicio en línea |
| Abrir un relevamiento cerrado | Seleccionar el relevamiento mock en estado cerrado | La app lo abre en modo lectura y no habilita capturas; sirve de puente al sample 02, que captura sobre un relevamiento en recolección |
| Dispositivo sin seguridad configurada | Probar el relogueo en un dispositivo sin patrón ni equivalente | La app advierte `DISPOSITIVO_SIN_SEGURIDAD` y exige inicio en línea en cada reanudación |

Estas variaciones encadenan con el sample 02 (sync offline), que parte de un relevamiento en recolección ya seleccionado y agrega la captura de observaciones sin conexión y el ciclo de sincronización.

## 8. Trazabilidad

| Artefacto upstream | Tipo | Cómo lo ilustra este sample |
| --- | --- | --- |
| CU-01 | Caso de uso | Ejecuta el inicio de sesión en línea, el deslogueo completo y el relogueo por seguridad del dispositivo sobre datos mock |
| CU-02 | Caso de uso | Lista los relevamientos asignados desde el almacén local mock y fija uno como contexto activo de captura |
| RN-04 | Regla de negocio | Evidencia el relogueo por seguridad del dispositivo en sesión activa, sin reingreso de credenciales |
| ADR-05 | Decisión arquitectónica | Materializa el token bearer custodiado en el almacenamiento seguro, con inicio en línea, relogueo y deslogueo completo |
| ADR-01 | Decisión arquitectónica | Materializa el estilo en capas con patrón de presentación: la lógica de sesión vive fuera de las vistas y se prueba sin interfaz |
| NFR de arranque en frío (≤ 3 s) | Atributo de calidad (intake §17 P.10) | La traza muestra el arranque en frío hasta la pantalla de sesión dentro del objetivo |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del sample de app básica: arranque, sesión con relogueo por seguridad del dispositivo y selección de relevamiento asignado sobre datos mock. Ilustra CU-01 y CU-02. |
