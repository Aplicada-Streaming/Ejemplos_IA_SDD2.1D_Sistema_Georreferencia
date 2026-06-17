# Wireframe — Estado de sincronización

**Proyecto:** geovial-mobile
**Documento:** wireframes-estado-sincronizacion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Mobile UX Designer + Accessibility Specialist
**Variante:** UX/UI

## 1. Pantalla y propósito

Superficie que da visibilidad a la cola offline y al ciclo de sincronización. El agente ve cuántos cambios esperan para subir, sigue el progreso del ciclo subir-luego-bajar, conoce el resumen del último ciclo y ve los elementos en conflicto, sin tener que gestionar la mecánica. Hace tangible la promesa de que el trabajo de campo está a salvo aunque no haya red. CU origen: CU-06.

## 2. Layout

Pantalla en portrait. Cabecera con el estado global; cuerpo con la cola de cambios pendientes y, durante un ciclo, el progreso por fases; pie con el resumen del último ciclo.

```
+------------------------------------------+
|  < Sincronizacion                        |
+------------------------------------------+
|  Estado: Sin conexion                    |  <- estado global
|  Hay 3 cambios esperando para subir.     |
|  Tu trabajo esta guardado en el          |
|  dispositivo.                            |
|                                          |
|  [   S I N C R O N I Z A R   A H O R A ]  |  <- forzar (deshabilitado sin red)
+------------------------------------------+
|  Cambios en cola (3)                     |
|  - Marcador nuevo            pendiente   |
|  - Foto + observacion        pendiente   |
|  - Comentario + etiqueta     pendiente   |
+------------------------------------------+
|  Ultimo ciclo                            |
|  Subidos: 5   Bajados: 2                  |
|  En conflicto: 1  (se resuelve al cierre)|
+------------------------------------------+
```

Durante un ciclo, el cuerpo muestra el progreso por fases:

```
+------------------------------------------+
|  Estado: Sincronizando                   |
|                                          |
|  Fase 1 - Subiendo tus cambios           |
|  [#########.........]  3 de 5            |  <- subir primero (RN-02)
|                                          |
|  Fase 2 - Trayendo actualizaciones       |
|  (espera a que termine la subida)        |  <- bajar solo despues
+------------------------------------------+
```

## 3. Componentes principales

| Componente | Propósito | Datos que muestra | Comportamiento |
| --- | --- | --- | --- |
| Estado global | Resumir la situación de sincronización | Sin conexión / cambios en cola / sincronizando / al día | Refleja el indicador persistente; texto que asegura que el trabajo está guardado |
| Acción Sincronizar ahora | Forzar un ciclo cuando hay red | Rótulo de acción | Dispara el ciclo; deshabilitada sin red; sin efecto si ya hay un ciclo en curso (5.C) |
| Lista de cambios en cola | Mostrar qué espera subir, en orden | Tipo de cambio y estado (pendiente o confirmado) | Lectura; el orden refleja la creación; los confirmados se retiran tras la subida |
| Progreso por fases | Hacer visible el orden subir-luego-bajar | Fase de subida con avance N de M; fase de bajada que espera | La bajada no arranca hasta concluir la subida (RN-02) |
| Resumen del último ciclo | Cerrar el ciclo con cuentas claras | Cambios subidos, actualizaciones bajadas, elementos en conflicto | Lectura; los elementos en conflicto se reportan como tales (RN-03) |
| Aviso de conflicto | Informar marcadores en conflicto sin alarmar | Conteo de elementos en conflicto | No bloqueante; aclara que el jefe resuelve al cierre en la web |

## 4. Interacciones

| Acción | Disparador | Resultado esperado | Precondición |
| --- | --- | --- | --- |
| Sincronizar ahora | El agente toca Sincronizar con red | La app inicia el ciclo: sube primero la cola y solo después baja actualizaciones (RN-02) | Hay conexión; no hay un ciclo en curso |
| Ver progreso | Un ciclo está activo | La app muestra la fase de subida con avance y la de bajada en espera | Ciclo en curso |
| Reanudar tras corte | Vuelve la red tras un corte en la subida | La app retoma sin reaplicar lo confirmado, conservando el resto de la cola (5.A) | Hubo un ciclo parcial; vuelve la conexión |
| Reloguear desde aquí | El token fue rechazado (TOKEN_INVALIDO) | La app deriva a la pantalla de inicio de sesión, conservando la cola intacta | Token rechazado en un ciclo |

## 5. Estados

| Estado | Condición que lo produce | Representación esperada |
| --- | --- | --- |
| Vacío | Cola vacía y nada por sincronizar | "No tenés cambios pendientes. Relevamiento al día." y resumen del último ciclo |
| Cargando | Recuento de la cola y verificación de conectividad | Indicador breve mientras se calcula el estado |
| Con datos | Hay cambios en cola | Lista de cambios pendientes y acción de sincronizar |
| Sin conexión | No hay red | "Sin conexión. Hay N cambios esperando para subir."; la acción Sincronizar queda deshabilitada; la cola se conserva |
| Sincronizando | Un ciclo está en curso | Progreso por fases: subida (N de M) y luego bajada; el agente puede seguir trabajando |
| Éxito | El ciclo concluyó | "Relevamiento al día" con el resumen: subidos, bajados y en conflicto |
| Éxito parcial | Corte durante la subida (5.A) | Lo confirmado quedó subido; el resto permanece en la cola; relevamiento reanudable sin duplicar |
| Error | BACKEND_INALCANZABLE, TOKEN_INVALIDO, RELEVAMIENTO_CERRADO | Backend inalcanzable: conserva la cola y reintenta solo; token inválido: pide reloguear conservando la cola; relevamiento cerrado: no sube, conserva la cola e informa |
| En conflicto | La bajada trajo marcadores en conflicto (5.B) | Aviso no bloqueante con el conteo; aclara que el jefe resuelve al cierre (RN-03) |

## 6. Versión móvil o responsive

App de campo en portrait como orientación primaria. Notas de adaptación:

- La cola y el resumen se apilan en una columna; en pantallas más altas se muestran más entradas de la cola sin compactar el progreso.
- La acción Sincronizar permanece al alcance del pulgar en cualquier alto de pantalla.
- En landscape (no primario), el estado global y el progreso se mantienen legibles; no se exige rotar (1.3.4).

## 7. Notas de implementación

- Accesibilidad: el estado global y cada cambio de fase se anuncian por región de estado para lectores de pantalla (4.1.3); el estado no se comunica solo por color: hay texto e ícono (1.4.1); el progreso por fases tiene texto equivalente al avance (1.1.1); la acción Sincronizar es un objetivo grande (2.5.8); contraste alto para legibilidad bajo sol (1.4.3, 1.4.11). El conflicto se redacta sin alarmar, asegurando que el trabajo sigue disponible.
- Performance percibida: el ciclo corre en segundo plano y nunca bloquea la captura; el progreso por fases hace tangible el orden subir-luego-bajar; un ciclo de 100 cambios completa en 30 s o menos en red móvil típica (NFR del proyecto) y reanuda sin pérdida tras un corte.
- Internacionalización: los rótulos de estado, fase y resumen se externalizan y toleran expansión; los conteos se presentan en formato localizable.
- La mecánica de detección de conectividad, idempotencia y reanudación vive en la librería de sincronización; esta pantalla solo refleja su estado, no lo gobierna (RN-02).

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Persona objetivo | Agente de campo (00) |
| CU origen | CU-06 |
| Marco experiencia aplicado | experiencia-de-uso_v1.0.md §3.6, §4 (estados sin conexión y sincronizando), §5 (accesibilidad), §8 (errores) |
| Reglas de negocio relevantes | RN-02, RN-03, RN-05 |
| US a generar | US-11, US-12, US-13 (en 06) |
| Tests previstos | Orden subir-antes-de-bajar visible; corte en subida deja reanudable sin duplicar; convivencia con conflicto en la bajada reportado en el resumen; token rechazado conserva la cola y deriva a reloguear; relevamiento cerrado conserva la cola e informa (en 08) |

## 9. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Wireframe inicial del estado de sincronización: cola offline de cambios pendientes, progreso del ciclo por fases que hace visible el orden subir-luego-bajar, resumen del último ciclo con elementos en conflicto, acción de forzar sincronización, estados (incluido sin conexión, sincronizando, éxito parcial y en conflicto) y trazabilidad a CU-06 con RN-02, RN-03 y RN-05. |
