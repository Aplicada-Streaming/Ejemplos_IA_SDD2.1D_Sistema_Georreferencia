# Wireframe — Resolución de conflictos y cierre

**Proyecto:** geovial-web
**Documento:** wireframes-resolucion-conflictos-cierre_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** UX/UI Designer + Frontend Lead
**Variante:** UX/UI

## 1. Pantalla y propósito

Resolución de conflictos de marcadores y cierre del relevamiento. Con el relevamiento en revisión, el jefe de área recorre los conflictos pendientes —dos o más marcadores dentro de un mismo radio—, compara sobre el mapa los marcadores involucrados con su evidencia y decide por cada uno unificar o mantener separados, pudiendo reabrir una decisión antes del cierre. El cierre solo se ofrece cuando no quedan conflictos pendientes; si el jefe intenta cerrar con conflictos, el front bloquea y deriva aquí. CU origen: CU-07 (resolución) y CU-08 (transición y cierre). Marco aplicado: `experiencia-de-uso_v1.0.md` (flujos 3.7 y 3.8, estados §4.2, errores §8). El comparador reutiliza el carrusel de `representacion-carrusel-fotos_v1.0.md`. Esta superficie cubre además el estado de error de avance de ciclo (cierre bloqueado por conflictos).

## 2. Layout

Lista de conflictos pendientes a la izquierda; a la derecha, el comparador de los marcadores del conflicto seleccionado sobre el mapa, con su evidencia y los controles de decisión. La barra de contexto muestra el estado del relevamiento y el control de cierre, habilitado o bloqueado según queden conflictos.

```text
+----------------------------------------------------------------------+
| < Relevamientos   Tramo Norte   Estado: Revision                     |
|                        Conflictos pendientes: 2   [ Cerrar (bloq.) ]  |
+--------------------------------+-------------------------------------+
|  Conflictos pendientes         |  Comparar conflicto C-01            |
|  ----------------------------- |  --------------------------------- |
|  > C-01  M-03 / M-07   (radio) |     Mapa: (M-03*) ... (M-07)        |
|    C-02  M-11 / M-12 / M-14    |                                     |
|                                |  M-03  etiquetas: fisura            |
|  (lista vacia => "sin          |        fotos: 6  [ ver carrusel ]   |
|   conflictos, cierre           |  M-07  etiquetas: junta             |
|   habilitado")                 |        fotos: 4  [ ver carrusel ]   |
|                                |                                     |
|                                |  Decision:                          |
|                                |   ( ) Unificar en un marcador       |
|                                |   ( ) Mantener separados            |
|                                |  ( aviso: al unificar, el resultante|
|                                |    conserva la union de etiquetas ) |
|                                |                                     |
|                                |  [ Reabrir decision ] [ Confirmar ] |
+--------------------------------+-------------------------------------+

Modal de cierre (cuando no quedan conflictos):
+----------------------------------------------+
|  Cerrar relevamiento                    [ X ]|
|  Vas a cerrar "Tramo Norte". El cierre        |
|  habilita el informe y deja firmes las        |
|  resoluciones. Esta accion se puede revertir  |
|  solo si el sistema habilita la reapertura.   |
|        [ Cancelar ]      [ Cerrar ]           |
+----------------------------------------------+
```

## 3. Componentes principales

| Componente | Propósito | Datos que muestra | Comportamiento |
| --- | --- | --- | --- |
| Barra de contexto y estado | Situar el relevamiento y su avance de ciclo | Estado vigente, conteo de conflictos pendientes, control de cierre | El cierre se ofrece solo si no quedan conflictos (RN-05); muestra solo transiciones válidas (CU-08, RN-04) |
| Lista de conflictos pendientes | Recorrer los conflictos a resolver | Identificador del conflicto y marcadores involucrados | Selección de un conflicto; se vacía a medida que se resuelven; al quedar vacía habilita el cierre |
| Comparador de marcadores | Comparar los marcadores del conflicto | Posición sobre el mapa, etiquetas, conteo de fotos y autoría de cada marcador | Distingue cada marcador por forma e ícono; abre el carrusel de cada uno |
| Carrusel de evidencia | Revisar las fotos de cada marcador en conflicto | Foto, comentario, etiqueta, autoría | Reutiliza `representacion-carrusel-fotos_v1.0.md` |
| Control de decisión | Elegir unificar o separar | Opciones de decisión y aviso de unión de etiquetas | Al unificar marcadores con etiquetas distintas, advierte que el resultante conserva la unión (CU-07 5.B, CA-02) |
| Acción Reabrir decisión | Cambiar una resolución antes del cierre | Estado de la resolución | Vuelve el conflicto a pendiente y permite decidir de nuevo (CU-07 5.A, CA-04) |
| Control de cierre | Cerrar el relevamiento | Estado del cierre (habilitado o bloqueado) | Bloqueado con conflictos pendientes; al activarse, abre el modal de cierre (CU-08) |
| Modal de cierre | Confirmar el hito de cierre | Resumen del cierre y su reversibilidad | Confirmable y cancelable; bloquea el doble disparo del cierre |

## 4. Interacciones

| Acción | Disparador | Resultado esperado | Precondición |
| --- | --- | --- | --- |
| Seleccionar un conflicto | Clic o teclado sobre la lista | El comparador muestra los marcadores y su evidencia (CU-07 paso 3) | Relevamiento en revisión con conflictos |
| Unificar marcadores | Confirmar decisión Unificar | El backend reasigna la evidencia al marcador resultante y retira el conflicto de la lista (CU-07 paso 6, CA-01) | Relevamiento en revisión |
| Mantener separados | Confirmar decisión Separados | El backend marca el conflicto resuelto sin alterar la evidencia y lo retira (CU-07 paso 7) | Relevamiento en revisión |
| Advertir unión de etiquetas | Elegir Unificar con etiquetas distintas | Aviso de que el resultante conserva la unión de etiquetas (CU-07 5.B, CA-02) | Marcadores con etiquetas diferentes |
| Reabrir una decisión | Acción Reabrir decisión | El conflicto vuelve a pendiente y se puede decidir de nuevo (CU-07 5.A, CA-04) | Relevamiento aún no cerrado |
| Pasar de recolección a revisión | Acción de transición válida | El backend aplica y el front muestra el nuevo estado (CU-08 paso 3, CA-01) | Relevamiento en recolección |
| Devolver de revisión a recolección | Acción de transición válida | El backend aplica el retorno controlado (CU-08 5.A) | Relevamiento en revisión |
| Intentar cerrar con conflictos | Acción de cierre con pendientes | Cierre bloqueado, aviso CONFLICTOS_PENDIENTES y derivación a esta lista (CU-08 5.B, CA-02) | Quedan conflictos pendientes |
| Cerrar sin conflictos | Confirmar en el modal de cierre | El backend cierra y el front lo muestra cerrado, habilitado para el informe (CU-08 paso 7, CA-03) | Sin conflictos pendientes |

## 5. Estados

| Estado | Condición que lo produce | Representación esperada |
| --- | --- | --- |
| Vacío | No quedan conflictos pendientes | Mensaje de estado resuelto y control de cierre habilitado (CU-07 postcondición, RN-05) |
| Cargando | Carga de la lista de conflictos o del comparador | Skeleton de la lista y del comparador; estado de envío al confirmar una decisión |
| Con datos | El backend devolvió conflictos pendientes | Lista de conflictos, comparador sobre el mapa y controles de decisión operables |
| Error: cierre bloqueado por conflictos | Intento de cierre con pendientes | Aviso CONFLICTOS_PENDIENTES no destructivo y derivación a la lista de conflictos (CU-08 5.B) |
| Error: conflicto inexistente | El conflicto ya fue resuelto o no existe | Se retira de la lista y se refresca el listado de pendientes (CONFLICTO_INEXISTENTE) |
| Solo lectura: fuera de revisión | El relevamiento no está en revisión | Pantalla en solo lectura con aviso de que la resolución se hace en revisión (RELEVAMIENTO_NO_EN_REVISION, RN-04) |
| Error: transición no permitida | Transición inválida desde el estado actual | La transición no se ofrece; ante rechazo del backend, aviso y recarga del estado vigente (TRANSICION_NO_PERMITIDA) |
| Sin conexión al circuito | El backend no responde o la sesión expiró | Banner persistente de servicio no disponible o aviso de sesión expirada |
| Fuera de alcance | El relevamiento no pertenece al jefe | No se abre; ante rechazo del backend, mensaje y retorno al listado (FUERA_DE_ALCANCE) |

## 6. Versión móvil o responsive

En anchos reducidos la lista de conflictos y el comparador se apilan: la lista arriba como selector compacto y el comparador debajo. El mapa del comparador alterna con el detalle de cada marcador mediante un control. El carrusel de evidencia y el modal de cierre ocupan la pantalla completa. El control de cierre y su estado (habilitado o bloqueado) permanecen visibles de forma consistente en la barra de contexto. Se conserva el tamaño mínimo de objetivo en los controles de decisión y de cierre.

## 7. Notas de implementación

- Accesibilidad: el resultado de cada resolución, el bloqueo del cierre y la confirmación del cierre se anuncian por región de estado (4.1.3); los controles de decisión son operables por teclado y tienen etiquetas asociadas (2.1.1, 1.3.1); el modal de cierre devuelve el foco al cerrarse y no atrapa el teclado (2.1.2); los marcadores en conflicto se distinguen por forma e ícono además del color (1.4.1); el control de cierre comunica su estado bloqueado por texto e ícono, no solo por color.
- Performance percibida: estado de envío en la confirmación de cada decisión y en el cierre; bloqueo del doble disparo del cierre; actualización de la lista de pendientes al confirmar.
- Internacionalización: el aviso de unión de etiquetas y los mensajes de bloqueo toleran expansión de texto; las etiquetas del dominio se truncan con texto completo accesible.
- Prevención de errores: el cierre nunca se ofrece con conflictos pendientes (RN-05); las transiciones se limitan a las válidas desde el estado vigente (RN-04); toda decisión es reversible antes del cierre (CU-07 5.A).

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Persona objetivo | Jefe de área (00) |
| CU origen | CU-07 (resolver conflictos al cierre) y CU-08 (transicionar el estado y cerrar) |
| Reglas de negocio relevantes | RN-01 (visibilidad por rol), RN-04 (estados visibles y habilitación), RN-05 (conflictos como precondición visible del cierre) |
| Marco de experiencia aplicado | experiencia-de-uso_v1.0.md (flujos 3.7 y 3.8, estados §4.2, errores §8) |
| Representación reutilizada | representacion-carrusel-fotos_v1.0.md |
| US a generar | US-16, US-17 (CU-07); US-18, US-19 (CU-08) |
| Tests previstos | Unificación reasigna evidencia; unión de etiquetas; reapertura antes del cierre; fuera de revisión en solo lectura; transición recolección a revisión; cierre bloqueado con conflictos y derivación; cierre sin conflictos; transición no permitida (08) |

## 9. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Wireframe inicial de resolución de conflictos y cierre, anclado a CU-07 y CU-08, al marco de experiencia y a la representación del carrusel. Cubre el estado de error de avance de ciclo (cierre bloqueado por conflictos). Layout de lista de conflictos más comparador, modal de cierre, estados (vacío, cargando, con datos, cierre bloqueado, conflicto inexistente, solo lectura fuera de revisión, transición no permitida, sin conexión y fuera de alcance), reflujo apilado en móvil y notas de accesibilidad WCAG 2.2 AA. |
