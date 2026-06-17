# Ejemplo 02 — Cola de cambios, estado consultable y reanudación con conflicto

**Proyecto:** aplicada-sync
**Documento:** ejemplo-02-intermedio_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Sample Engineer (AG-11)
**Nivel:** Intermedio
**Ubicación del código:** `/samples/aplicada-sync/02-intermedio/`

## 1. Objetivo del sample

Demostrar el ciclo de trabajo realista del integrador: encolar varios cambios locales producidos sin conexión, consultar en cualquier momento el estado del motor y el contenido de la cola, reanudar una sincronización que quedó interrumpida por una subida parcial y convivir con un elemento que el backend reporta en conflicto. Al terminar, el desarrollador sabe operar la cola, leer la observabilidad del motor, recuperarse de un corte sin perder ni duplicar datos y entender que el motor reporta el conflicto pero no lo resuelve.

## 2. Nivel

Intermedio. Asume que el lector completó el sample 01 (inicialización y un ciclo mínimo). Agrega tres capacidades que el básico no demuestra: la cola con varios cambios y la idempotencia por identificador estable, la cara de observabilidad del motor (consulta de estado y de cola) y la reanudación desde el punto de corte conviviendo con un elemento en conflicto. Todavía no integra el motor en una app host ajena ni implementa la totalidad de los puntos de extensión opcionales; eso pertenece al sample avanzado.

## 3. Prerequisites

- El paquete distribuible de la librería incorporado al proyecto del sample desde el repositorio de distribución, en la versión de superficie pública vigente del producto.
- Runtime objetivo del ecosistema, en la versión mínima declarada en el SOLUTION-INTAKE §17 P.9 del proyecto aplicada-sync.
- Una estrategia de almacén local persistente de prueba (obligatoria), provista en el propio sample, que conserva la cola y la marca de progreso a través de reinicios del proceso, para poder demostrar la reanudación.
- Una estrategia de transporte de prueba (obligatoria), provista en el propio sample, que reconoce el identificador de cambio estable (idempotencia), permite simular un corte de conectividad tras confirmar parte de los cambios y puede reportar una entidad en conflicto durante la bajada.
- Un proveedor de credencial de prueba (opcional, incluido para dejar la sesión en estado listo).

## 4. Cómo correrlo

1. Posicionarse en la carpeta del sample: `cd samples/aplicada-sync/02-intermedio`.
2. Restaurar las dependencias del sample con el gestor de paquetes del ecosistema.
3. Ejecutar el comando de arranque con el escenario por defecto, que encola cinco cambios y fuerza un corte tras confirmar dos de ellos.
4. Observar la consulta de estado que reporta la sesión reanudable con tres pendientes, y volver a ejecutar el comando para que el motor reanude.
5. Comparar la salida de la reanudación, incluida la línea de elemento en conflicto, con el output esperado de §6.

## 5. Estructura del código

```
02-intermedio/
├── README.md                        # Qué demuestra el sample y cómo correrlo
├── src/
│   ├── Programa.<ext>               # Orquesta el escenario: encola, corta, consulta, reanuda
│   ├── AlmacenLocalPersistente.<ext># Estrategia de almacen local que sobrevive a reinicios
│   ├── TransporteConCorteYConflicto.<ext> # Transporte que simula corte parcial y reporta conflicto
│   └── CredencialFija.<ext>         # Proveedor de credencial de prueba (opcional)
└── tests/
    ├── cola_idempotencia_test.<ext> # Reencolado del mismo identificador no duplica la cola
    ├── reanudacion_test.<ext>       # Reanuda reenviando solo los faltantes, sin duplicar
    └── conflicto_convive_test.<ext> # El ciclo concluye y reporta el conflicto sin abortar
```

## 6. Qué esperar

Salida esperada en consola en la primera ejecución (encolado y corte parcial):

```
Sesion inicializada. Id de sesion: ses-0002. Estado: listo.
Encolados 5 cambios. Tamano de cola: 5.
Reencolado de chg-103 (duplicado): tamano de cola sin cambios: 5.
Ejecutando ciclo de sincronizacion...
Fase de subida: 2 cambios confirmados antes del corte de conectividad.
Corte detectado. Codigo: SUBIDA_INCOMPLETA. No se inicia la fase de bajada.
Consulta de estado: sesion=reanudable, pendientes=3, ultima marca de sincronizacion=sin avanzar.
```

Salida esperada en la segunda ejecución (reanudación y convivencia con conflicto):

```
Sesion recuperada del almacen local. Estado: reanudable. Pendientes: 3.
Reanudando ciclo de sincronizacion...
Fase de subida reanudada: 3 cambios nuevos confirmados, 0 reaplicados (reconocidos por identificador estable).
Fase de bajada: 2 actualizaciones bajadas y aplicadas.
Elemento en conflicto reportado por el backend: mkr-conflicto-1 (el motor lo aplica y convive, no lo resuelve).
Resumen del ciclo: subidos=3, bajados=2, en conflicto=1, estado final=listo.
```

## 7. Variaciones sugeridas

| Variación | Qué cambiar | Resultado |
| --- | --- | --- |
| Backend aún inalcanzable al reanudar | Mantener el transporte sin responder en la segunda ejecución | La reanudación devuelve `BACKEND_INALCANZABLE`, conserva los tres pendientes y no baja actualizaciones |
| Segundo corte durante la reanudación | Forzar un nuevo corte tras reenviar uno de los tres pendientes | El motor conserva el avance, deja la sesión reanudable con dos pendientes y no inicia la bajada |
| Todos los pendientes ya recibidos | Hacer que el backend ya tenía los cinco cambios pero el motor no lo registró | El backend los reconoce por identificador, no reaplica ninguno y el motor procede directo a la bajada |
| Consulta de elementos en conflicto | Pedir específicamente la lista de elementos en conflicto tras el ciclo | El motor devuelve el identificador en conflicto conocido y lo reporta como conviviente, no resuelto |

Estas variaciones preparan el terreno para el sample avanzado, que integra el motor en una app host ajena al sistema y ejercita la totalidad de los puntos de extensión.

## 8. Trazabilidad

| Artefacto upstream | Tipo | Cómo lo ilustra este sample |
| --- | --- | --- |
| CU-02 | Caso de uso | Encola varios cambios locales y demuestra que el reencolado del mismo identificador no duplica la cola |
| CU-05 | Caso de uso | Consulta el estado del motor y la cola, incluido el progreso parcial y los elementos en conflicto |
| CU-06 | Caso de uso | Reanuda una sincronización interrumpida por subida parcial, reenviando solo los faltantes |
| RN-02 | Regla de negocio | La reanudación se apoya en la idempotencia por identificador de cambio estable |
| RN-03 | Regla de negocio | El motor convive con el elemento en conflicto y lo reporta sin resolverlo |
| ADR-04 | Decisión arquitectónica | Materializa la cola local persistente y ordenada, con una entrada por identificador |
| ADR-06 | Decisión arquitectónica | Materializa la reanudación por marca de progreso ante subida parcial |
| ADR-07 | Decisión arquitectónica | Materializa la idempotencia por identificador de cambio estable |
| ADR-08 | Decisión arquitectónica | Materializa la convivencia con estados en conflicto sin bloqueo |
| NFR Reanudación sin pérdida | Atributo de calidad (arquitectura §8) | La verificación comprueba 0 cambios perdidos y 0 duplicados tras el corte en la fase de subida |
| NFR Continuidad ante conflicto | Atributo de calidad (arquitectura §8) | La verificación comprueba que el ciclo concluye sin abortar ante un estado en conflicto reportado |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del sample intermedio: cola de cambios, consulta de estado, reanudación interrumpida y convivencia con conflicto. Ilustra CU-02, CU-05 y CU-06. |
