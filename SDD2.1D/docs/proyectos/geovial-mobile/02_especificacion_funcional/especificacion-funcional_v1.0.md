# Especificación funcional — geovial-mobile

**Proyecto:** geovial-mobile
**Documento:** especificacion-funcional_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + Mobile UX Analyst

## 1. Propósito

Este documento es el índice maestro de la especificación funcional de `geovial-mobile`, la app de captura en terreno del agente de campo dentro de la solución GeoVial. El proyecto es del tipo `mobile-app-maui`: cada caso de uso (CU) describe un flujo táctil de campo, con sus permisos del sistema operativo, su comportamiento offline-first y su sincronización diferida, en formato Cockburn con criterios de aceptación BDD (Given/When/Then). La especificación define el qué; el cómo (stack, almacén local concreto, interfaces) vive en la categoría 05.

`geovial-mobile` cubre el lado de campo de tres necesidades de negocio de la solución: el login del agente (NB-01), la captura georreferenciada y estructurada de observaciones (NB-03) y el trabajo sin conexión con sincronización (NB-04). Consume el contrato REST de `geovial-api` y delega la sincronización en la librería de sincronización (`aplicada-sync`). La revisión y el cierre del relevamiento, con la resolución de conflictos (NB-05), ocurren del lado de la web (`geovial-web`) y no originan CU en este proyecto; la app solo convive con los conflictos y los sincroniza.

## 2. Alcance funcional cubierto

El alcance de esta especificación es la experiencia de campo del agente: iniciar y cerrar sesión y reloguear por la seguridad del dispositivo, seleccionar un relevamiento asignado, centrar por GPS y crear o mover marcadores, capturar fotos con resolución de coordenadas en el momento, agregar comentarios y etiquetas, trabajar sin conexión y sincronizar subiendo antes de bajar, y cargar fotos manualmente priorizando la ubicación incrustada con un radio de agrupación.

Por tratarse de un proyecto con almacenamiento offline (regla 02 §2.2), se produce el modelo conceptual del almacén local del dispositivo. Como ese modelo local no supera las diez entidades, no se acompañan reglas conceptuales de modelo (RC). Se incorporan, en los CU donde aplica, las secciones opcionales de permisos del sistema operativo (02 §4.3 §14) y de performance del CU (02 §4.3 §12), propias del tipo `mobile-app-maui`.

Quedan fuera de esta especificación, por la composición de la solución y las exclusiones de alcance (intake §9, §13): la administración de usuarios y relevamientos (operación del jefe de área en la web y el backend), la revisión sobre mapa y el cierre con resolución de conflictos (NB-05, `geovial-web`), la exportación e importación de relevamientos (`geovial-api`/`geovial-web`) y el auto-registro de agentes (Won't Have v1). El motor de sincronización (detección de conectividad, orden subir-luego-bajar, idempotencia y reanudación) es responsabilidad de `aplicada-sync`; aquí se especifica su consumo desde la app, no su mecánica interna.

## 3. Catálogo de casos de uso

| CU | Nombre | Flujo de campo | Actor primario | NB | Estado |
| --- | --- | --- | --- | --- | --- |
| CU-01 | Iniciar sesión, deslogueo completo y relogueo por seguridad del dispositivo | Sesión y seguridad | Agente de campo | NB-01 | Propuesto |
| CU-02 | Seleccionar un relevamiento asignado | Contexto de trabajo | Agente de campo | NB-04, NB-03 | Propuesto |
| CU-03 | Centrar por GPS y crear o mover un marcador en el mapa | Georreferenciación | Agente de campo | NB-03 | Propuesto |
| CU-04 | Capturar una foto con resolución de coordenadas en el momento | Captura georreferenciada | Agente de campo | NB-03 | Propuesto |
| CU-05 | Agregar comentarios y etiquetas a la observación | Enriquecimiento de evidencia | Agente de campo | NB-03 | Propuesto |
| CU-06 | Trabajar sin conexión y sincronizar subiendo antes de bajar | Offline-first y sincronización | Agente de campo | NB-04 | Propuesto |
| CU-07 | Cargar fotos manualmente priorizando ubicación con radio de agrupación | Carga manual | Agente de campo | NB-03 | Propuesto |

Total: 7 CU. El mínimo del tipo `mobile-app-maui` (6 CU, 02 §2.2) se cumple y se supera por la cobertura completa del lado de campo de NB-01, NB-03 y NB-04. Actor primario único en cada CU (siempre el agente de campo).

## 4. Catálogo de reglas de negocio

| RN | Nombre | Invariante (resumen) | CU afectados |
| --- | --- | --- | --- |
| RN-01 | Prioridad de la ubicación incrustada y radio de agrupación | La carga manual prioriza la ubicación de la foto y agrupa por radio | CU-07 |
| RN-02 | Orden de sincronización subir antes de bajar | La bajada no se atiende hasta concluir la subida del ciclo | CU-06, CU-02 |
| RN-03 | Convivencia con conflictos de marcadores en el cliente | El conflicto convive sin bloquear y se resuelve al cierre en la web | CU-03, CU-06, CU-07 |
| RN-04 | Relogueo por seguridad del dispositivo en sesión activa | La reanudación revalida por el dispositivo sin reingreso de credenciales | CU-01, CU-06 |
| RN-05 | Captura sin conexión con cola local persistente | Toda la captura es offline y se conserva en la cola hasta confirmarse | CU-02, CU-03, CU-04, CU-05, CU-07 |

Las reglas del lado móvil replican y se alinean con las invariantes equivalentes del backend y la librería de sincronización: RN-01 con geovial-api RN-04; RN-02 con aplicada-sync RN-01 y geovial-api RN-06; RN-03 con geovial-api RN-03 y aplicada-sync RN-03.

## 5. Modelo conceptual del almacén local

El modelo conceptual (`modelo-datos/modelo-conceptual_v1.0.md`) describe el almacén local del dispositivo con 8 entidades: RelevamientoLocal, MarcadorLocal, ObservacionLocal, FotoLocal, ComentarioLocal, EtiquetaLocal, CambioEncolado y MarcaSincronizacionLocal. El dominio autoritativo es el de geovial-api; el modelo local es una réplica parcial para el trabajo offline. Por no superar las diez entidades (02 §2.2), no se acompañan reglas conceptuales de modelo (RC); las invariantes de integridad fina (identidad estable del marcador, referencia observación-marcador, monotonía de la marca de sincronización) las gobierna el backend y el cliente las respeta como réplica.

## 6. Matriz de trazabilidad NB → CU → RN → US

| NB upstream | CU | RN aplicables | US a generar (en 06) |
| --- | --- | --- | --- |
| NB-01 | CU-01 Iniciar sesión, deslogueo y relogueo por seguridad del dispositivo | RN-04 | US-01, US-02 |
| NB-04, NB-03 | CU-02 Seleccionar un relevamiento asignado | RN-05, RN-02 | US-03, US-04 |
| NB-03 | CU-03 Centrar por GPS y crear o mover un marcador | RN-03, RN-05 | US-05, US-06 |
| NB-03 | CU-04 Capturar una foto con resolución de coordenadas | RN-01, RN-05 | US-07, US-08 |
| NB-03 | CU-05 Agregar comentarios y etiquetas a la observación | RN-05, RN-03 | US-09, US-10 |
| NB-04 | CU-06 Trabajar sin conexión y sincronizar | RN-02, RN-03, RN-05 | US-11, US-12, US-13 |
| NB-03 | CU-07 Cargar fotos manualmente con radio de agrupación | RN-01, RN-03 | US-14, US-15 |

Cobertura bidireccional sobre el alcance de campo: cada CU declara al menos una NB (NB-01, NB-03 o NB-04) y cada una de esas tres NB tiene al menos un CU en este proyecto. No hay CU huérfano. NB-02 (gestión y asignación de relevamientos) y NB-05 (revisión y cierre) se cubren en `geovial-api`/`geovial-web`; la app solo consume sus resultados (relevamiento asignado, relevamiento cerrado en modo lectura) sin originar CU propios.

## 7. Correspondencia con la numeración de CU

Los CU de geovial-mobile usan numeración propia del proyecto (CU-01 a CU-07), independiente de la de geovial-api. La numeración es contigua y sin huecos. Donde la app consume un recurso del backend, los CU lo referencian por su nombre (por ejemplo, CU-03 de geovial-api para el inicio y cierre de sesión del lado servidor) sin reutilizar su número.

## 8. Decisiones de recorte (02 §5.2)

- El inicio de sesión, el deslogueo completo y el relogueo por seguridad del dispositivo se especifican en un único CU (CU-01) por compartir actor primario y el ciclo de sesión; sus tres modos se distinguen como flujos y excepciones, no como CU separados.
- La captura se reparte en tres CU por gesto y permiso del sistema operativo distintos: marcador por GPS (CU-03, permiso de ubicación), foto en el momento (CU-04, permisos de cámara y ubicación) y comentarios y etiquetas (CU-05, sin permisos), para que cada permiso y cada degradación queden verificables por separado.
- La sincronización se especifica como un solo CU (CU-06) que orquesta el ciclo subir-luego-bajar delegado a la librería de sincronización; su mecánica interna (detección de conectividad, idempotencia, reanudación) vive en los CU de `aplicada-sync` y no se duplica aquí.
- La revisión, el cierre y la resolución de conflictos (NB-05) no se especifican como CU móviles porque son operación de la web; la app solo refleja el estado cerrado en modo lectura (CU-02) y convive con los conflictos (RN-03).

## 9. Ambigüedades y supuestos abiertos (master-prompt §9)

El intake declara como PENDIENTE de respuesta del cliente varios casos límite de §7 que tocan la captura y la sincronización del lado móvil. Se especificaron con un supuesto explícito, marcado en los CU correspondientes y alineado con los supuestos del índice de geovial-api (02 §9), a confirmar con el negocio:

- Foto cargada manualmente sin metadatos de ubicación (intake §7): se asume que queda pendiente de ubicación manual, sin inventarle coordenada, ubicable luego en el mapa. Reflejado en CU-07 (5.A) y RN-01.
- Captura sin señal de GPS en el momento (intake §7): se asume que el marcador se fija manualmente en el mapa y la foto queda anclada al marcador del entorno pendiente de ubicación precisa, sin coordenada inventada. Reflejado en CU-03 (5.C, SIN_SENAL_GPS) y CU-04 (5.A).
- Pérdida de conexión durante una sincronización con subida parcial (intake §7): se asume reanudación idempotente sin pérdida ni duplicación, conservando la cola. Reflejado en CU-06 (5.A) y RN-02.
- Cierre del relevamiento mientras el agente tiene cambios locales sin sincronizar (intake §7): se asume que el cierre bloquea nuevas subidas, el backend responde RELEVAMIENTO_CERRADO y la app conserva la cola e informa al agente. Reflejado en CU-06.
- Conflictos entre cambios de dos agentes sobre el mismo relevamiento o marcador (intake §7): se asume la misma política de convivencia y resolución al cierre que para los conflictos por radio. Reflejado en CU-06 y RN-03.

Ninguno de estos supuestos bloquea la especificación; cada uno se resolverá al confirmar el negocio, sin alterar la estructura de los CU.

## 10. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Índice maestro inicial de la especificación funcional de geovial-mobile: 7 CU de flujos de campo, 5 RN del lado móvil, modelo conceptual del almacén local de 8 entidades (sin RC) y matriz de trazabilidad NB→CU→RN→US sobre el lado de campo de NB-01, NB-03 y NB-04. |
