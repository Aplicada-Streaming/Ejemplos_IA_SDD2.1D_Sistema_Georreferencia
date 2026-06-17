# CU-03 — Ejecutar la sincronización subir-luego-bajar

**Proyecto:** aplicada-sync
**Documento:** CU-03-ejecutar-sincronizacion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Ejecutar un ciclo completo de sincronización del motor: subir primero los cambios locales pendientes del host al backend remoto y, solo después de que la subida concluya, bajar las actualizaciones del backend para aplicarlas al almacén local. Materializa la política central de la librería y la garantía de orden hacia quien la integra.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Aplicación host | Primario | Solicita la ejecución del ciclo de sincronización |
| Backend remoto | Sistema | Recibe los cambios locales y entrega las actualizaciones |
| Almacén local del host | Sistema | Aporta la cola de pendientes y recibe las actualizaciones bajadas |

## 3. Precondiciones

- Existe una sesión de sincronización inicializada y autenticada (CU-01).
- El motor dispone de una credencial vigente provista por el host.
- El backend remoto es alcanzable al momento de la ejecución.
- No hay otra ejecución de sincronización en curso para la misma sesión.

## 4. Flujo principal

1. La aplicación host solicita al motor ejecutar la sincronización sobre la sesión vigente.
2. El motor marca la sesión como sincronizando y toma la cola de cambios pendientes en su orden de creación.
3. Fase de subida: el motor envía los cambios pendientes al backend remoto respetando el orden y, a medida que el backend confirma cada uno, lo retira de la cola de pendientes y registra el avance.
4. El motor verifica que la fase de subida concluyó sin cambios pendientes confirmables restantes.
5. Fase de bajada: solo entonces el motor solicita al backend las actualizaciones posteriores a la última marca de sincronización conocida.
6. El motor aplica las actualizaciones bajadas al almacén local y actualiza la marca de última sincronización.
7. El motor deja la sesión en estado listo y devuelve un resumen del ciclo: cantidad de cambios subidos, cantidad de actualizaciones bajadas y estado final.

## 5. Flujos alternativos

- 5.A Cola de pendientes vacía. Disparador: al iniciar, la cola no tiene cambios locales. El motor omite la fase de subida, registra cero cambios subidos y procede directamente a la fase de bajada. Retorna al paso 5.
- 5.B Backend reporta estados en conflicto durante la bajada. Disparador: una o más actualizaciones bajadas corresponden a entidades marcadas en conflicto por el backend. El motor aplica esas actualizaciones como estado válido en conflicto, no aborta el ciclo y las incluye en el resumen como elementos en conflicto. Retorna al paso 7 (ver RN-03).
- 5.C Sincronización ya en curso. Disparador: el host solicita ejecutar mientras hay un ciclo activo para la misma sesión. El motor no inicia un segundo ciclo y devuelve el estado de la ejecución vigente. Termina sin efectos adicionales.

## 6. Excepciones y errores

| Código | Causa | Respuesta del motor |
| --- | --- | --- |
| BACKEND_INALCANZABLE | El backend remoto no responde al iniciar o durante la fase de subida | Detiene el ciclo conservando la cola de pendientes no confirmados; no inicia la fase de bajada; devuelve el error y deja la sesión recuperable |
| CREDENCIAL_INVALIDA | El backend rechaza la credencial provista por el host | Detiene el ciclo sin subir ni bajar, no altera la cola y solicita al host renovar la credencial |
| SUBIDA_INCOMPLETA | La fase de subida termina con cambios pendientes no confirmados por corte de conexión | No inicia la fase de bajada, conserva los pendientes y deja la sesión en estado reanudable (continúa en CU-06) |

## 7. Postcondiciones

- Éxito: la cola de pendientes quedó vacía de los cambios confirmados, las actualizaciones se aplicaron al almacén local y la marca de última sincronización avanzó. La fase de bajada solo ocurrió tras concluir la subida.
- Fallo en la subida: no se bajó ninguna actualización; los cambios no confirmados permanecen en la cola y la sesión queda reanudable, sin pérdida ni duplicación.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Una sesión autenticada con 2 cambios pendientes y backend alcanzable | La aplicación host ejecuta la sincronización | El motor sube primero los 2 cambios, luego baja las actualizaciones y devuelve un resumen con 2 subidos antes de cualquier bajada |
| CA-02 | Una sesión autenticada con la cola vacía y backend alcanzable | La aplicación host ejecuta la sincronización | El motor omite la subida, reporta 0 subidos y baja las actualizaciones disponibles |
| CA-03 | Una sesión con 3 pendientes donde el backend deja de responder tras confirmar el primero | La aplicación host ejecuta la sincronización | El motor detiene el ciclo con el código BACKEND_INALCANZABLE, conserva 2 pendientes, no baja actualizaciones y deja la sesión reanudable |
| CA-04 | Una sesión cuya bajada incluye una entidad marcada en conflicto por el backend | La aplicación host ejecuta la sincronización | El motor aplica la actualización en conflicto sin abortar y la reporta como elemento en conflicto en el resumen |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-04 |
| Reglas de negocio aplicables | RN-01, RN-02, RN-03 |
| Historias de usuario a generar | US-05, US-06, US-07 (en 06) |
| Componentes esperados | Ejecutor de subida y de bajada; cola de cambios locales pendientes; registro de estado y progreso (referencia tentativa a 05) |
| Tests previstos | Orden subir-antes-de-bajar verificado; cola vacía omite subida; corte en subida no dispara bajada y deja reanudable; convivencia con elementos en conflicto (en 08) |

## 10. Notas y supuestos

- El orden subir-antes-de-bajar es una garantía dura del motor, formalizada como RN-01; no es configurable hacia un orden inverso.
- La idempotencia (RN-02) asegura que un cambio reenviado tras un corte no se aplique dos veces en el backend ni una actualización bajada dos veces en el local.
- El motor no resuelve conflictos de dominio: los aplica como estado válido y los reporta, difiriendo la resolución al backend o a la aplicación host (RN-03).
- La detección automática del momento de ejecutar pertenece a CU-04; este CU describe la ejecución una vez disparada.

## 17. Compatibilidad de versión pública

La garantía de orden subir-antes-de-bajar y el contrato del resumen del ciclo (campos de cantidad subida, cantidad bajada, estado y elementos en conflicto) forman parte de la superficie pública. Invertir el orden, eliminar la garantía o cambiar la semántica del resumen constituye un cambio incompatible y obliga a incrementar la versión mayor.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de ejecución de la sincronización subir-luego-bajar, derivado de NB-04 y del SOLUTION-INTAKE §17 (aplicada-sync). |
