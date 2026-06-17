# Flujo de ejecución — captura offline y sincronización de geovial-mobile

**Proyecto:** geovial-mobile
**Documento:** flujo-ejecucion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Arquitecto Móvil

## 1. Objetivo

Este documento describe el pipeline de captura offline y de sincronización de la app de campo, paso a paso, con sus transformaciones de datos sobre el almacén local y su apoyo en el contrato de la librería de sincronización. Cubre la detección de conexión, el ciclo subir-luego-bajar, la reanudación tras un corte y la convivencia con conflictos. Está dirigido a quien implementa (06), prueba (08) y revisa la integración. La mecánica interna del motor (detección de conectividad fina, transporte, esquema de metadatos) vive en la librería consumida; aquí se describe su consumo desde la app (ADR-03).

## 2. Actores y componentes del flujo

- Agente de campo: actor primario de todos los CU.
- Servicio de sesión y adaptador de almacenamiento seguro: custodian el token y gobiernan el relogueo por seguridad del dispositivo (ADR-05).
- Servicio de captura, repositorio del almacén local y adaptadores de plataforma (ubicación, cámara, archivos): materializan la captura offline (ADR-01, ADR-02, ADR-04).
- Servicio de sincronización y adaptador de la librería de sincronización: orquestan el ciclo subir-luego-bajar consumiendo el motor (ADR-03).
- Cliente del contrato REST: transporta hacia el dominio autoritativo (consume `proyectos/geovial-api/05_arquitectura_tecnica/contratos-rest_v1.0.md`).
- Librería de sincronización: provee las operaciones del ciclo de vida (consume `proyectos/aplicada-sync/05_arquitectura_tecnica/contratos-abstractions_v1.0.md`).

## 3. Pipeline general

```text
[Arranque y sesión]
  -> arranque en frio (<= 3 s)
  -> sesion activa? -> relogueo por seguridad del dispositivo (sin credenciales)
                    -> sin sesion -> inicio en linea con credenciales -> token al almacen seguro
  -> inicializar motor de sincronizacion (configuracion de sesion)

[Captura offline]  (sin red, RN-05)
  seleccionar relevamiento asignado (almacen local)
  -> crear/mover marcador (ubicacion o fijacion manual)
  -> capturar foto (resolver coordenada o pendiente de ubicacion)
  -> comentar / etiquetar
  -> cada accion: persistir entidad local + encolar CambioEncolado (transaccion local)

[Sincronizacion]  (al detectar conexion o por accion manual)
  detectar conexion -> habilitar disparo automatico
  -> ejecutar ciclo: SUBIDA (cambios locales) -> al concluir -> BAJADA (actualizaciones)
  -> aplicar bajada al almacen local -> avanzar marca de sincronizacion
  -> conflictos: reportados, conviven, no abortan (RN-03)

[Reanudacion]  (tras corte durante la subida)
  reanudar desde el punto de corte -> reenvios reconocidos por identificador de origen
  -> sin perdida ni duplicacion -> continuar o completar el ciclo
```

## 4. Etapa 1 — Arranque y sesión

1. Arranque en frío hasta la pantalla de sesión o verificación en ≤ 3 s (NFR de arranque, ADR-01, ADR-05).
2. Si hay una sesión activa (reinicio de app o desbloqueo de dispositivo), la app solicita la verificación por seguridad del dispositivo (patrón, huella o equivalente) sin reingresar credenciales (RN-04, CU-01). Si el dispositivo no tiene seguridad configurada, la app advierte y exige inicio en línea.
3. Si no hay sesión, el agente inicia sesión en línea con credenciales; la app recibe el token bearer del backend (operación de inicio de sesión del contrato REST) y lo guarda en el almacenamiento seguro (ADR-05).
4. La app inicializa el motor de sincronización con la configuración de sesión: identificador de host remoto, referencia al almacén local, referencia al backend remoto y proveedor de credencial (operación inicializar sesión del contrato de la librería). Transformación: estado de sesión en memoria + estado inicial del motor (situación, cola pendiente, marca, conflictos conocidos).

## 5. Etapa 2 — Captura offline

Toda la captura es offline-first y se persiste en el almacén local, encolando un cambio por acción (RN-05). Cada acción es una transacción local: entidad y `CambioEncolado` se escriben juntos o no se escribe ninguno (ADR-02).

1. Seleccionar relevamiento asignado (CU-02): lectura desde `relevamiento_local`; carga marcadores y observaciones locales en el mapa, sin dependencia de red. Si hay conexión, puede dispararse antes un ciclo para refrescar asignaciones (CU-06).
2. Crear o mover marcador (CU-03): el adaptador de ubicación centra por GPS si hay permiso y señal; si el permiso está denegado o revocado, degrada a fijación manual del pin (ADR-04). Transformación: alta o actualización de `marcador_local` (coordenada o pendiente de ubicación) + alta de `cambio_encolado` (tipo crear/mover, identificador de origen estable, orden de creación).
3. Capturar foto y resolver coordenadas (CU-04): el adaptador de cámara captura la imagen (requiere permiso de cámara, ADR-04); el adaptador de ubicación resuelve la coordenada en el momento. Sin señal de GPS, la foto se conserva como pendiente de ubicación sin coordenada inventada (RN-01). El binario se aloja en el dispositivo; sin espacio, no se guarda y se avisa (ADR-04). Transformación: alta o reúso de `observacion_local`, alta de `foto_local` (referencia al binario, origen de ubicación) + `cambio_encolado`.
4. Comentar y etiquetar (CU-05): sin permisos del sistema operativo. Transformación: alta de `comentario_local` (a lo sumo uno por foto), alta o reúso de `etiqueta_local` y de sus asociaciones + `cambio_encolado`.
5. Carga manual con radio de agrupación (CU-07): el adaptador de archivos accede a la galería (requiere permiso, ADR-04); la app prioriza la ubicación incrustada de cada foto y agrupa por el radio en un marcador existente o crea uno nuevo; las fotos sin ubicación incrustada quedan pendientes de ubicación (RN-01). Una carga sin radio aplicable se rechaza. Transformación: altas o reúsos de `marcador_local`, `observacion_local`, `foto_local` + `cambio_encolado`.

Invariante de la etapa: los marcadores en conflicto (dos o más dentro de un mismo radio) se crean, conviven y quedan accesibles, sin bloquear la recolección; su resolución se difiere al cierre desde la web (RN-03).

## 6. Etapa 3 — Sincronización (ciclo subir-luego-bajar)

El ciclo se delega en el motor de la librería de sincronización (ADR-03). La app no atiende la bajada hasta que la subida del ciclo concluye (RN-02).

1. Detección de conexión: la app habilita el disparo automático del ciclo ante recuperación de conectividad (operación habilitar disparo automático del contrato de la librería); el agente también puede forzarlo. La detección fina de conectividad vive en la librería.
2. Fase de subida: el motor toma la cola de `cambio_encolado` en orden de creación y la sube al backend (operación ejecutar sincronización; endpoint de subida del contrato REST). El backend deduplica por identificador de origen (idempotencia, RN-02). Transformación: los cambios confirmados pasan a estado `confirmado` y se retiran de la cola; los conflictos detectados se reportan en el resumen del ciclo sin abortar (RN-03).
3. Compuerta de orden: solo cuando la subida del ciclo concluye se atiende la bajada (RN-02). Si se intentara bajar sin concluir la subida, el backend rechaza (SUBIDA_NO_CONCLUIDA del contrato REST).
4. Fase de bajada: el motor solicita las novedades posteriores a la marca de sincronización del relevamiento (endpoint de bajada del contrato REST; la bajada porta la marca opaca del cliente). Transformación: aplica las actualizaciones del backend sobre `relevamiento_local`, `marcador_local`, `observacion_local`, `foto_local`, `comentario_local` y `etiqueta_local`, que prevalecen como dominio autoritativo.
5. Avance de la marca: tras confirmar la aplicación de las novedades, la app avanza `marca_sincronizacion_local` con la marca nueva (monótona, RC-06 replicada). Transformación: actualización de `marca_sincronizacion_local`.
6. Estado: la consulta de estado del motor alimenta la presentación de sincronización (situación, tamaño de cola, marca, conflictos conocidos, progreso parcial).

Métrica de la etapa: un lote de 100 cambios completa el ciclo en ≤ 30 s en red móvil típica (NFR de tiempo de ciclo).

## 7. Etapa 4 — Reanudación tras un corte

Si la conexión se pierde durante la subida (subida parcial), el ciclo no avanza a la bajada (RN-02) y la cola se conserva consistente.

1. Al recuperar conexión, la app reanuda la sincronización desde el punto de corte (operación reanudar sincronización del contrato de la librería).
2. El motor reenvía los cambios pendientes; el backend reconoce los reenvíos por identificador de origen y no los duplica (idempotencia, RN-02). Transformación: los cambios efectivamente nuevos se confirman; los ya recibidos se reconocen sin duplicar.
3. Si el progreso parcial es inconsistente o la marca no es reconocible, el motor reporta la condición; una marca no reconocible obliga a una sincronización completa (alineado con MARCA_INVALIDA del contrato REST).
4. Resultado: el ciclo continúa o se completa sin pérdida ni duplicación (NFR de reanudación).

## 8. Casos límite y supuestos

Alineados con los supuestos abiertos de 02 §9 (a confirmar con el negocio):

- Foto sin ubicación incrustada en carga manual: queda pendiente de ubicación manual, sin coordenada inventada (RN-01, CU-07).
- Sin señal de GPS al capturar: el marcador se fija manualmente y la foto queda pendiente de ubicación (CU-03, CU-04).
- Corte durante la subida: reanudación idempotente sin pérdida ni duplicación, conservando la cola (Etapa 4, RN-02).
- Cierre del relevamiento con cambios locales sin sincronizar: el backend responde RELEVAMIENTO_CERRADO; la app conserva la cola e informa al agente (CU-06).
- Conflictos entre cambios de dos agentes: misma política de convivencia y resolución al cierre que los conflictos por radio (RN-03).

## 9. Trazabilidad

| Etapa | CU | RN | ADRs | Contrato apoyado |
| --- | --- | --- | --- | --- |
| Arranque y sesión | CU-01 | RN-04 | ADR-05, ADR-01 | REST (sesión) |
| Captura offline | CU-02, CU-03, CU-04, CU-05, CU-07 | RN-01, RN-05, RN-03 | ADR-02, ADR-04 | — (local) |
| Sincronización | CU-06, CU-02 | RN-02, RN-03 | ADR-03 | Librería de sincronización; REST (subida/bajada) |
| Reanudación | CU-06 | RN-02, RN-05 | ADR-03 | Librería de sincronización (reanudar) |

Downstream: 06 (US del pipeline), 08 (pruebas de modo offline, ciclo subir-luego-bajar, reanudación y convivencia con conflictos, intake §17.P.6).

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Pipeline inicial de captura offline y sincronización: arranque y sesión con relogueo por seguridad del dispositivo, captura offline con transacción local y encolado, ciclo subir-luego-bajar con compuerta de orden y avance de marca, reanudación idempotente y convivencia con conflictos. Apoyado en el contrato de la librería de sincronización y en el contrato REST. |
