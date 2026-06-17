# Ejemplo 02 — Sync offline: captura sin conexión y sincronización subir-luego-bajar

**Proyecto:** geovial-mobile
**Documento:** ejemplo-02-sync-offline_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Sample Engineer (mobile)
**Nivel:** Intermedio/Avanzado
**Ubicación del código:** `/samples/geovial-mobile/02-sync-offline/`

## 1. Objetivo del sample

Demostrar el escenario offline-first completo de la app de campo: capturar observaciones sin conexión (crear y mover marcadores, capturar fotos con resolución de coordenadas, escribir comentarios y aplicar etiquetas, y cargar fotos manualmente agrupándolas por radio), encolar cada cambio en un almacén local persistente y, al recuperar conexión, sincronizar subiendo primero los cambios locales y bajando después las actualizaciones del relevamiento asignado, conviviendo con los conflictos y reanudando sin pérdida tras un corte. Al terminar, el desarrollador entiende cómo la app trabaja 100 % sin conexión, cómo se ordena la cola local y cómo el ciclo subir-luego-bajar se delega en el componente de sincronización del producto.

## 2. Nivel

Intermedio/Avanzado. Asume que el lector completó el sample 01 (app básica), que ya resuelve la sesión y la selección de relevamiento; parte de un relevamiento en recolección ya abierto. Agrega lo que el sample 01 no demuestra: el almacén local de observaciones, la cola de cambios pendientes, la captura sin conexión con sus degradaciones por permiso y por señal, y el ciclo de sincronización con su compuerta de orden, su reanudación y su convivencia con conflictos. Es el sample distintivo del proyecto, equivalente al nivel avanzado del tipo.

## 3. Prerequisites

- El proyecto de la app móvil incorporado desde el repositorio, en la versión vigente del producto.
- Haber corrido el sample 01 (recomendado) para entender la sesión y la selección de relevamiento; este sample arranca desde un relevamiento ya seleccionado.
- Herramientas de construcción del ecosistema móvil, en la versión mínima declarada en el SOLUTION-INTAKE §17 P.9 del proyecto geovial-mobile (plataforma target Android, versión mínima Android API 26).
- Un emulador o un dispositivo Android conectado por USB en modo desarrollador, con permisos de ubicación, cámara y acceso a galería para poder ejercitar la captura y sus degradaciones.
- Datos mock y dobles de prueba incluidos en el sample: un relevamiento en recolección precargado, un proveedor de ubicación simulado, una cámara simulada que entrega imágenes de prueba, un conjunto de fotos con ubicación incrustada para la carga manual y un backend de sincronización simulado que reconoce el identificador de origen de cada cambio y entrega actualizaciones posteriores a una marca. No requiere backend real.

## 4. Cómo correrlo

1. Posicionarse en la carpeta del sample: `cd samples/geovial-mobile/02-sync-offline`.
2. Restaurar las dependencias del sample con el gestor de paquetes del ecosistema.
3. Desplegar la app en el emulador o en el dispositivo Android con el comando de arranque del sample, que abre directamente el relevamiento mock en recolección con el modo sin conexión activo.
4. Capturar observaciones sin conexión siguiendo el guion del README del sample: crear un marcador por ubicación, tomar una foto, comentarla y etiquetarla, y cargar un conjunto de fotos por radio de agrupación.
5. Activar el modo con conexión para disparar la sincronización y comparar el resumen del ciclo con el output esperado de §6.

## 5. Estructura del código

```
02-sync-offline/
├── README.md                              # Guion del escenario offline-first y cómo correrlo
├── src/
│   ├── App.<ext>                           # Arranque del sample con relevamiento mock en recolección
│   ├── presentacion/
│   │   ├── PantallaMapaCaptura.<ext>       # Mapa con pines: centrar, crear y mover marcador, capturar foto
│   │   ├── PantallaObservacion.<ext>       # Nota, comentario por foto y etiquetas de la observación
│   │   ├── PantallaCargaManual.<ext>       # Selección de fotos y agrupación por radio
│   │   └── PantallaSincronizacion.<ext>    # Estado del ciclo: cola, marca, conflictos y progreso parcial
│   ├── aplicacion/
│   │   ├── ServicioCaptura.<ext>           # Crea entidades locales y encola cambios; verificable sin interfaz
│   │   ├── ServicioCargaManual.<ext>       # Prioriza ubicación incrustada y agrupa por radio
│   │   └── ServicioSincronizacion.<ext>    # Orquesta el ciclo subir-luego-bajar sobre el componente de sync
│   ├── infraestructura/
│   │   ├── AlmacenLocal.<ext>              # Almacén local persistente: entidades, cola y marca de sincronización
│   │   ├── ColaCambios.<ext>              # Cola ordenada por creación con identificador de origen estable
│   │   ├── UbicacionMock.<ext>            # Proveedor de ubicación simulado (con y sin señal de GPS)
│   │   ├── CamaraMock.<ext>               # Cámara simulada que entrega imágenes de prueba
│   │   ├── AdaptadorSincronizacion.<ext>  # Consume el componente de sincronización del producto
│   │   └── BackendSyncMock.<ext>          # Backend de sincronización simulado, idempotente por identificador
│   └── datos/
│       ├── relevamiento-mock.json          # Relevamiento en recolección de demostración
│       └── fotos-carga-manual/             # Fotos con ubicación incrustada para la carga por radio
└── tests/
    ├── captura_offline_test.<ext>          # Crea marcador y foto sin conexión y verifica el encolado
    ├── carga_manual_radio_test.<ext>       # Agrupa por radio y deja sin ubicación las fotos sin coordenada
    ├── orden_subir_antes_de_bajar_test.<ext> # Verifica que ninguna bajada precede al fin de la subida
    ├── reanudacion_test.<ext>              # Corte en la subida: reanuda sin pérdida ni duplicación
    └── convivencia_conflicto_test.<ext>    # Una actualización en conflicto se aplica sin abortar el ciclo
```

## 6. Qué esperar

Fase de captura sin conexión. Al ejecutar el guion de captura con el modo sin conexión activo, la traza esperada es:

```
Modo sin conexion activo. Relevamiento (mock): Puente Km 12, estado recoleccion.
Centrar por GPS: posicion resuelta. Marcador creado con identidad propia. Cambio encolado (orden 1).
Captura de foto: coordenada resuelta en el momento. Observacion anclada al marcador. Cambio encolado (orden 2).
Comentario y etiqueta aplicados a la foto. Cambio encolado (orden 3).
Carga manual: 3 fotos con ubicacion incrustada agrupadas por radio en 1 marcador. 1 foto sin ubicacion: pendiente de ubicacion manual. Cambios encolados (orden 4, 5, 6, 7).
Cola local: 7 cambios pendientes. Captura disponible 100 % sin conexion.
```

Fase de sincronización. Al activar el modo con conexión, la app detecta la conexión y ejecuta el ciclo:

```
Conexion detectada. Iniciando ciclo de sincronizacion sobre el relevamiento asignado.
Fase de subida: 7 cambios subidos en orden de creacion y confirmados. Cola: 0 pendientes.
Compuerta de orden: subida concluida. Habilitando bajada.
Fase de bajada: 2 actualizaciones bajadas y aplicadas a la copia local. Marca de sincronizacion avanzada.
Resumen del ciclo: subidos=7, bajados=2, en conflicto=1 (marcador dentro de un mismo radio, convive y se difiere al cierre).
```

El orden de las dos fases es parte de lo que el sample demuestra: la fase de bajada nunca aparece antes de que la fase de subida informe cero pendientes confirmables restantes.

Fase de reanudación. Al provocar un corte de conexión tras confirmar parte de la cola y luego recuperar la conexión, la traza esperada es:

```
Corte de conexion durante la subida: 3 cambios confirmados, 4 conservados en cola. Bajada no iniciada. Relevamiento reanudable.
Conexion recuperada. Reanudando desde el punto de corte.
Reenvios reconocidos por identificador de origen: 0 duplicados. 4 cambios restantes subidos y confirmados.
Ciclo completado sin perdida ni duplicacion.
```

## 7. Variaciones sugeridas

| Variación | Qué cambiar | Resultado |
| --- | --- | --- |
| Captura sin señal de GPS | Configurar el proveedor de ubicación mock para no entregar posición al capturar | La foto se conserva anclada al marcador y se marca como pendiente de ubicación, sin coordenada inventada |
| Permiso de ubicación denegado | Negar el permiso de ubicación del sistema operativo antes de centrar | La app degrada a fijación manual del pin en el mapa, sin centrar por GPS, y no cae |
| Forzar un conflicto por radio | Crear un segundo marcador dentro del radio de uno existente | La app crea el segundo marcador, lo deja convivir sin bloquear y lo reporta como elemento en conflicto en el resumen |
| Corte durante la subida | Activar el corte de conexión del mock tras confirmar el primer cambio | La app deja confirmados los ya subidos, conserva el resto en la cola, no baja y deja el relevamiento reanudable |
| Token rechazado en el ciclo | Configurar el backend mock para rechazar el token bearer | La app detiene el ciclo, conserva la cola intacta y solicita reloguear (vínculo con el sample 01) |

## 8. Trazabilidad

| Artefacto upstream | Tipo | Cómo lo ilustra este sample |
| --- | --- | --- |
| CU-03 | Caso de uso | Centra por GPS y crea o mueve un marcador en el almacén local, conservando su identidad y encolando el cambio |
| CU-04 | Caso de uso | Captura una foto resolviendo la coordenada en el momento y la ancla al marcador como observación local |
| CU-05 | Caso de uso | Registra nota, comentario por foto y etiquetas reutilizables en el almacén local y encola los cambios |
| CU-06 | Caso de uso | Detecta conexión y ejecuta el ciclo subir-luego-bajar con compuerta de orden, reanudación y convivencia con conflicto |
| CU-07 | Caso de uso | Carga fotos manualmente priorizando la ubicación incrustada y agrupándolas por radio en marcadores locales |
| RN-01 | Regla de negocio | Prioriza la ubicación incrustada y no inventa coordenadas; las fotos sin ubicación quedan pendientes |
| RN-02 | Regla de negocio | Evidencia el orden subir-antes-de-bajar y la idempotencia por identificador de origen en la traza del ciclo |
| RN-03 | Regla de negocio | Convive con los marcadores en conflicto durante la recolección y la sincronización; difiere la resolución al cierre |
| RN-05 | Regla de negocio | La captura funciona 100 % sin conexión y se persiste sin pérdida hasta su confirmación |
| ADR-01 | Decisión arquitectónica | Materializa el diseño offline-first en capas con la lógica de captura y sincronización fuera de las vistas |
| ADR-02 | Decisión arquitectónica | Usa el almacén local persistente y la cola con orden de creación e identificador de origen estable |
| ADR-03 | Decisión arquitectónica | Consume el motor de sincronización por contrato, sin reimplementar el ciclo subir-luego-bajar |
| ADR-04 | Decisión arquitectónica | Demuestra las degradaciones por permiso y por falta de señal o espacio, sin inventar datos |
| NFR de captura offline y ciclo de sync | Atributo de calidad (intake §17 P.10) | La captura es 100 % offline, la cola tolera ≥ 1000 cambios y un lote de 100 cambios completa el ciclo en ≤ 30 s y reanuda sin pérdida |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del sample de sync offline: captura sin conexión (marcadores, fotos, comentarios, etiquetas y carga manual por radio), cola local y ciclo subir-luego-bajar con reanudación y convivencia con conflicto. Ilustra CU-03 a CU-07. |
