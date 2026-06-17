# Mini-plan de construcción — geovial-api

**Proyecto:** geovial-api
**Documento:** mini-plan_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API PM (equipo_n=1)
**Modo:** Mini-plan (proyecto de un solo desarrollador, regla 07 §2.1/§2.2)

## 1. Información general

- Modo de planificación: mini-plan condensado para un único desarrollador. Sustituye a `plan-iteracion-sprint-XX`, las plantillas de review/retrospectiva y `velocidad-equipo` (regla 07 §2.2, escenario equipo de 1 dev).
- Unidad de estimación: story points en escala Fibonacci (1, 2, 3, 5, 8, 13), coherente con el product backlog y el backlog técnico de 06.
- Organización del trabajo: por tramos de construcción alineados a las fases del roadmap de producto (00, `roadmap-producto_v1.0.md`, F0 a F3) y a los releases por incremento. Cada tramo se cierra por criterios de transición verificables, no por calendario: el intake no fija fecha objetivo y la cadencia la marca el avance de un único desarrollador.
- Cadencia interna sugerida: ciclos cortos de una semana dentro de cada tramo durante el walking skeleton (regla 07 §3.2, sprint corto justificado en fase exploratoria), revisables a dos semanas una vez establecida una línea de base de avance.
- Total comprometido en el backlog: 44 US (211 SP) y 21 BT. El alcance Must (NB-01 a NB-05 más capacidades transversales Must) compone el MVP; las US Could (EP-06, EP-07 y US-30) se difieren al último tramo según cadencia.

Capacidad declarada: al no existir histórico de velocidad (primer ciclo del proyecto), no se fija un tope de compromiso por promedio móvil. La capacidad efectiva se calibra al cierre del Tramo 1 con el avance real del desarrollador y se registra en la bitácora de la §7.

## 2. Objetivo del mini-plan

Llevar geovial-api a un contrato público REST que recorra el ciclo completo del relevamiento de punta a punta —autenticar y administrar la jerarquía de usuarios, gestionar y asignar relevamientos, capturar observaciones georreferenciadas, sincronizar el trabajo sin conexión y revisar sobre mapa con cierre y resolución de conflictos— sostenido por las capacidades transversales del contrato.

## 3. Ítems comprometidos por tramo

Los ítems se ordenan respetando las dependencias del backlog técnico (BT) y de las historias (US), y la secuencia topológica del roadmap. El primer tramo materializa el walking skeleton de autenticación y jerarquía de usuarios de punta a punta (F0). Prioridad y estimación se toman tal cual del backlog de 06; no se re-estima en el plan.

### 3.1 Tramo 1 — Esqueleto, autenticación y jerarquía de usuarios (Fase F0, Incremento 1)

Walking skeleton end-to-end: fundaciones de capas, persistencia, emisión y validación de token, autorización por rol y alcance, errores uniformes y versionado del contrato, más la administración de la jerarquía de usuarios y la sesión.

| ID | Tipo | Descripción | Prioridad | SP | Estado |
| --- | --- | --- | --- | --- | --- |
| BT-01 | BT | Esqueleto de las cuatro capas con dependencia unidireccional | Must | 5 | Pendiente |
| BT-02 | BT | Modelo de dominio puro con invariantes | Must | 8 | Pendiente |
| BT-03 | BT | Puertos y orquestación de casos de uso | Must | 5 | Pendiente |
| BT-04 | BT | Esquema relacional y migración inicial M0001 | Must | 8 | Pendiente |
| BT-05 | BT | Adaptadores de repositorio con restricciones e índices | Must | 8 | Pendiente |
| BT-06 | BT | Frontera transaccional atómica por comando | Must | 5 | Pendiente |
| BT-07 | BT | Adaptador de emisión y validación de token bearer | Must | 8 | Pendiente |
| BT-08 | BT | Middleware de autorización por rol y alcance | Must | 8 | Pendiente |
| BT-09 | BT | Manejador de errores problem+json y catálogo de códigos | Must | 5 | Pendiente |
| BT-17 | BT | Versionado por URI y política de compatibilidad | Must | 5 | Pendiente |
| US-05 | US | Iniciar sesión y obtener un token de acceso | Must | 5 | Pendiente |
| US-06 | US | Cerrar sesión y revalidar la sesión activa | Should | 3 | Pendiente |
| US-01 | US | Administrar la jerarquía de usuarios del nivel inmediato inferior | Must | 8 | Pendiente |
| US-02 | US | Dar de baja un usuario conservando su autoría histórica | Must | 5 | Pendiente |
| US-03 | US | Dar de alta agentes de campo por el jefe de área | Must | 5 | Pendiente |
| US-04 | US | Dar de baja un agente de campo sin perder su trabajo registrado | Must | 3 | Pendiente |
| US-37 | US | Autorizar cada operación según el rol y el alcance del solicitante | Must | 8 | Pendiente |
| US-38 | US | Acotar cada listado al alcance jerárquico antes de paginar | Must | 5 | Pendiente |
| US-39 | US | Devolver todos los errores con un formato de problema uniforme | Must | 5 | Pendiente |
| US-44 | US | Versionar el contrato público y preservar la compatibilidad | Must | 5 | Pendiente |

Subtotal Tramo 1: 117 SP (10 BT por 65 SP + 10 US por 52 SP).

### 3.2 Tramo 2 — Relevamientos y marcadores (Fase F1, Incremento 2)

Alta, baja, visualización y asignación de relevamientos sobre su tramo, transición a revisión y retorno, marcadores geográficos con identidad estable, observaciones con notas, fotos, comentarios y etiquetas, más paginación, filtros y la tolerancia a conflictos como estado válido.

| ID | Tipo | Descripción | Prioridad | SP | Estado |
| --- | --- | --- | --- | --- | --- |
| BT-10 | BT | Servicio de paginación, filtros y orden de listados | Must | 5 | Pendiente |
| BT-14 | BT | Tolerancia a conflictos de marcadores como estado válido | Must | 5 | Pendiente |
| BT-15 | BT | Puerto de almacenamiento y adaptador a la abstracción | Must | 8 | Pendiente |
| US-07 | US | Crear un relevamiento con su tramo vial no vacío | Must | 5 | Pendiente |
| US-08 | US | Visualizar y consultar los relevamientos del alcance | Must | 3 | Pendiente |
| US-09 | US | Dar de baja un relevamiento del alcance | Should | 3 | Pendiente |
| US-10 | US | Asignar un agente de campo a un relevamiento | Must | 5 | Pendiente |
| US-11 | US | Reasignar y revocar agentes de un relevamiento | Should | 3 | Pendiente |
| US-12 | US | Avanzar el relevamiento de recolección a revisión | Must | 5 | Pendiente |
| US-13 | US | Retornar el relevamiento de revisión a recolección | Should | 3 | Pendiente |
| US-14 | US | Crear y mover marcadores geográficos con identidad estable | Must | 5 | Pendiente |
| US-15 | US | Dar de baja un marcador solo si no tiene observaciones ancladas | Should | 3 | Pendiente |
| US-16 | US | Crear una observación anclada a un marcador existente | Must | 5 | Pendiente |
| US-17 | US | Adjuntar fotos a una observación delegando el binario al almacén | Must | 5 | Pendiente |
| US-18 | US | Comentar y etiquetar fotos y marcadores | Should | 3 | Pendiente |
| US-40 | US | Paginar los listados de recursos con referencias de navegación | Must | 5 | Pendiente |
| US-41 | US | Filtrar y ordenar los listados por criterios soportados | Should | 3 | Pendiente |

Subtotal Tramo 2: 74 SP (3 BT por 18 SP + 14 US por 56 SP).

### 3.3 Tramo 3 — Captura en campo y sincronización (Fase F2, Incremento 3)

Carga manual con priorización de ubicación y agrupación por radio, subida del lote de cambios locales con reanudación sin duplicar, bajada incremental por marca con el orden subir-antes-de-bajar, y la idempotencia de las operaciones no seguras.

| ID | Tipo | Descripción | Prioridad | SP | Estado |
| --- | --- | --- | --- | --- | --- |
| BT-11 | BT | Servicio de idempotencia con almacén de claves | Must | 8 | Pendiente |
| BT-16 | BT | Carga manual con priorización de ubicación y agrupación por radio | Should | 5 | Pendiente |
| BT-12 | BT | Pipeline de subida del lote de cambios locales | Must | 13 | Pendiente |
| BT-13 | BT | Bajada incremental de actualizaciones por marca | Must | 8 | Pendiente |
| US-19 | US | Cargar fotos manualmente priorizando la ubicación incrustada | Must | 5 | Pendiente |
| US-20 | US | Agrupar por radio las fotos sin ubicación o cercanas | Should | 5 | Pendiente |
| US-21 | US | Subir el lote de cambios locales del agente | Must | 8 | Pendiente |
| US-22 | US | Reanudar una subida interrumpida sin duplicar cambios | Must | 5 | Pendiente |
| US-23 | US | Bajar las actualizaciones del relevamiento posteriores a la marca | Must | 8 | Pendiente |
| US-24 | US | Rechazar la bajada hasta concluir la subida del ciclo | Must | 3 | Pendiente |
| US-42 | US | Aceptar una clave de idempotencia en las operaciones no seguras | Must | 8 | Pendiente |
| US-43 | US | Rechazar una clave de idempotencia reutilizada de forma inconsistente | Should | 3 | Pendiente |

Subtotal Tramo 3: 79 SP (4 BT por 34 SP + 8 US por 45 SP).

### 3.4 Tramo 4 — Revisión, conflictos, cierre y cierre de alcance (Fase F3, Incremento 4)

Consulta del relevamiento para revisión sobre mapa con carrusel por marcador, listado y resolución de conflictos, cierre exigiendo conflictos resueltos y reapertura, materialización del contrato OpenAPI y contract tests del 100 % de endpoints, gate de cobertura y observabilidad, más las capacidades Could de portabilidad y almacenamiento configurable si la cadencia lo permite.

| ID | Tipo | Descripción | Prioridad | SP | Estado |
| --- | --- | --- | --- | --- | --- |
| BT-18 | BT | Materialización del contrato como especificación OpenAPI versionada | Should | 5 | Pendiente |
| BT-19 | BT | Contract tests del 100 % de endpoints públicos por versión | Must | 8 | Pendiente |
| BT-20 | BT | Gate de cobertura del pipeline de integración continua | Must | 5 | Pendiente |
| BT-21 | BT | Registros estructurados con correlación y puntos de medición | Should | 5 | Pendiente |
| US-25 | US | Consultar el relevamiento completo para la revisión sobre mapa | Must | 5 | Pendiente |
| US-26 | US | Recuperar las fotos por marcador para el carrusel de revisión | Should | 3 | Pendiente |
| US-27 | US | Listar los conflictos de marcadores del relevamiento | Must | 3 | Pendiente |
| US-28 | US | Resolver un conflicto unificando o separando marcadores | Must | 5 | Pendiente |
| US-29 | US | Cerrar el relevamiento exigiendo los conflictos resueltos | Must | 5 | Pendiente |
| US-30 | US | Reabrir un relevamiento cerrado a revisión | Could | 3 | Pendiente |
| US-31 | US | Exportar un relevamiento completo en una unidad transferible | Could | 8 | Pendiente |
| US-32 | US | Incluir comentarios, etiquetas y fotos en la exportación | Could | 5 | Pendiente |
| US-33 | US | Importar un relevamiento reconstruyendo su estructura | Could | 8 | Pendiente |
| US-34 | US | Importar de forma idempotente sin duplicar un relevamiento | Could | 5 | Pendiente |
| US-35 | US | Configurar el destino de almacenamiento activo por el usuario raíz | Could | 5 | Pendiente |
| US-36 | US | Validar un destino de almacenamiento sin activarlo | Could | 3 | Pendiente |

Subtotal Tramo 4: 81 SP (4 BT por 23 SP + 12 US por 58 SP).

Total general comprometido: 351 SP, suma de los cuatro subtotales de tramo (117 + 74 + 79 + 81). Se descompone en 21 BT por 140 SP y 44 US por 211 SP. El backlog de producto declara 211 SP de US (coherente con 06 §4); las 21 BT aportan los 140 SP del esfuerzo de construcción interno. Cada BT-01 a BT-21 aparece exactamente una vez entre los cuatro tramos.

## 4. Alcance técnico y dependencias

El orden de construcción respeta las dependencias declaradas en el backlog técnico de 06 (§2) y la cadena topológica del roadmap (00, §4):

- Tramo 1 abre con las fundaciones: BT-01 sin dependencias; BT-02 depende de BT-01; BT-03 de BT-01 y BT-02. Sobre ellas, BT-04 (depende de BT-02), BT-05 (de BT-03 y BT-04) y BT-06 (de BT-05) materializan la persistencia. BT-07 (de BT-03) y BT-08 (de BT-07) habilitan token y autorización; BT-09 (de BT-01) y BT-17 (de BT-01) cierran las transversales mínimas del esqueleto. Las US-01 a US-06, US-37, US-38, US-39 y US-44 consumen estas BT para entregar la jerarquía y la sesión de punta a punta.
- Tramo 2 añade BT-10 (depende de BT-05 y BT-08), BT-14 (de BT-05) y BT-15 (de BT-03), que sostienen los listados acotados, la convivencia con conflictos y la delegación del binario al almacén; las US-07 a US-18, US-40 y US-41 construyen relevamientos y marcadores sobre esa base.
- Tramo 3 incorpora BT-11 (depende de BT-05), BT-12 (de BT-11), BT-13 (de BT-12) y BT-16 (de BT-15), que materializan idempotencia, subida, bajada y carga manual por radio; las US-19 a US-24, US-42 y US-43 cubren captura y sincronización.
- Tramo 4 cierra con BT-18 (depende de BT-17), BT-19 (de BT-18), BT-20 (de BT-19) y BT-21 (de BT-09), que publican el contrato, verifican el 100 % de endpoints, bloquean por cobertura y aportan observabilidad; las US-25 a US-36 cubren revisión, conflictos, cierre y las capacidades Could de portabilidad y almacenamiento.

Esta sección no redefine arquitectura: la gobiernan las ADR-01 a ADR-10 de 05 (`arquitectura-solucion_v1.0.md`, `contratos-rest_v1.0.md`, `modelo-datos-logico_v1.0.md`). El versionado del contrato (US-44, BT-17, ADR-10) arbitra que ningún tramo introduzca un cambio incompatible sin pasar por la política de compatibilidad; las ventanas de breaking change se concentran al inicio de cada versión mayor y no a mitad de un tramo.

## 5. Definition of Done aplicada

La Definition of Done canónica del proyecto reside en la categoría 08 (`08_calidad_y_pruebas`), pendiente de generación. Este mini-plan referenciará por enlace esa DoD canónica cuando exista; hasta entonces, cada ítem se considera terminado cuando cumple los criterios de aceptación declarados en su ficha de 06 (US individual en `historias-usuario/` o BT en `backlog-tecnico_v1.0.md` §2), pasa sus pruebas automáticas y, para todo endpoint público, dispone de su contract test (BT-19) y respeta el gate de cobertura (BT-20). Criterios específicos de este mini-plan, adicionales a la DoD canónica: ningún tramo se da por cerrado sin satisfacer los criterios de transición de fase del roadmap (00, §5) correspondientes a su incremento.

## 6. Trazabilidad por tramo

CU avanzados (CU-01 a CU-22) y NB avanzadas (NB-01 a NB-07) al cierre de cada tramo, con las ADR que gobiernan:

| Tramo | NB que avanzan | CU que avanzan | ADR que gobiernan |
| --- | --- | --- | --- |
| Tramo 1 (F0) | NB-01 | CU-01, CU-02, CU-03, CU-18, CU-19, CU-22 | ADR-01, ADR-02, ADR-03, ADR-05, ADR-10 |
| Tramo 2 (F1) | NB-02, NB-03 (parcial) | CU-04, CU-05, CU-06, CU-07, CU-08, CU-20 | ADR-02, ADR-04, ADR-06, ADR-09 |
| Tramo 3 (F2) | NB-03 (cierre), NB-04 | CU-09, CU-10, CU-11, CU-21 | ADR-07, ADR-08, ADR-09 |
| Tramo 4 (F3) | NB-05, NB-06, NB-07 | CU-12, CU-13, CU-14, CU-15, CU-16, CU-17 | ADR-06, ADR-09, ADR-10 |

Al cierre del Tramo 4 quedan avanzados los 22 CU y las 7 NB. Las NB-06 y NB-07 (Could Have) avanzan solo si la cadencia del Tramo 4 lo permite; en caso contrario se difieren como deuda planificable sin afectar el MVP (NB-01 a NB-05).

## 7. Riesgos y mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
| --- | --- | --- | --- |
| Sin línea de base de velocidad, el compromiso por tramo puede sobreestimar la capacidad de un único desarrollador y arrastrar carry-over sistemático | Alta | Medio | Calibrar la capacidad al cierre del Tramo 1 con el avance real registrado en la bitácora §7; usar ciclos cortos de una semana en F0; si más del 20 % del esfuerzo de un tramo se traslada, replantear el alcance del siguiente antes de comprometerlo |
| El pipeline de subida (BT-12, 13 SP) concentra el mayor riesgo técnico: orden subir-antes-de-bajar, deduplicación por identificador de origen, reanudación sin duplicar y lote de al menos 1000 cambios sin pérdida | Media | Alto | Spike técnico al inicio del Tramo 3 sobre BT-11 (idempotencia) antes de BT-12; aislar la frontera transaccional (BT-06) y los contract tests de sincronización (BT-19) como red de seguridad; fallback a procesamiento por sublotes si el lote completo no cierra en una transacción |
| Un cambio incompatible introducido en un tramo posterior rompe clientes ya integrados contra una versión publicada | Media | Alto | Aplicar la política de versionado por URI (US-44, BT-17, ADR-10) desde el Tramo 1; concentrar breaking changes al inicio de una versión mayor nueva conservando la anterior durante la convivencia; verificar compatibilidad con los contract tests de BT-19 antes de cada cierre de tramo |
| Las US Could de portabilidad y almacenamiento (EP-06, EP-07, US-30) compiten por la capacidad del Tramo 4 con el cierre del MVP Must | Media | Bajo | Tratar las US Could como deuda planificable explícita; priorizar el cierre Must del Tramo 4 (revisión, conflictos, cierre) y diferir las Could a un ciclo posterior si la cadencia no alcanza, sin bloquear el incremento |

## 8. Criterios de hecho por tramo

Un tramo se considera completo cuando todas sus US Must comprometidas están en estado terminado según la DoD de §5, sus contract tests pasan y se satisfacen los criterios de transición de fase del roadmap (00, §5) del incremento correspondiente. Las US Should y Could de un tramo no bloquean su cierre si su diferimiento queda registrado en la bitácora con motivo. El cierre del Tramo 1 habilita el Tramo 2 solo tras verificar el walking skeleton de autenticación y jerarquía de punta a punta cubierto por pruebas automáticas.

## 9. Bitácora de avance

Tabla a completar por tramo a medida que avanza la construcción. Registra el avance real, el carry-over y la calibración de capacidad para el tramo siguiente.

| Tramo | Semana | SP comprometidos | SP completados | Carry-over | Notas |
| --- | --- | --- | --- | --- | --- |
| Tramo 1 (F0) | — | 117 | — | — | Por iniciar; calibrar capacidad al cierre |
| Tramo 2 (F1) | — | 74 | — | — | Por iniciar |
| Tramo 3 (F2) | — | 79 | — | — | Por iniciar; spike BT-11/BT-12 al arranque |
| Tramo 4 (F3) | — | 81 | — | — | Por iniciar; US Could sujetas a cadencia |

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Mini-plan inicial de geovial-api en modo equipo_n=1. Condensa objetivo único de valor, 44 US y 21 BT organizadas en cuatro tramos alineados a las fases F0 a F3 del roadmap de 00, con prioridad y estimación Fibonacci tomadas de 06, trazabilidad a CU-01 a CU-22 y NB-01 a NB-07 por tramo, ADR gobernantes, cuatro riesgos con mitigación, referencia pendiente a la DoD canónica de 08 y bitácora de avance a completar. |
