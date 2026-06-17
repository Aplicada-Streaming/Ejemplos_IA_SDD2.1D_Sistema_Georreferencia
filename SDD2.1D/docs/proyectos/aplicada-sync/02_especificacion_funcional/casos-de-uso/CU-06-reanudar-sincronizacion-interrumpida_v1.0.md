# CU-06 — Reanudar una sincronización interrumpida

**Proyecto:** aplicada-sync
**Documento:** CU-06-reanudar-sincronizacion-interrumpida_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Permitir que el motor retome una sincronización que quedó interrumpida durante la fase de subida (subida parcial), continuando desde el punto donde se cortó sin reenviar de manera efectiva los cambios ya confirmados y sin haber bajado actualizaciones antes de terminar la subida. Garantiza que un corte en campo no produzca pérdida ni duplicación de datos.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Aplicación host | Primario | Solicita reanudar o relanza el ciclo tras un corte |
| Backend remoto | Sistema | Recibe los cambios restantes y entrega las actualizaciones |
| Almacén local del host | Sistema | Conserva la cola con los pendientes no confirmados y la marca de progreso |

## 3. Precondiciones

- Existe una sesión cuyo último ciclo quedó en estado reanudable por una subida parcial (excepción SUBIDA_INCOMPLETA de CU-03).
- La cola de pendientes conserva los cambios no confirmados con su identificador estable y su orden de creación.
- El backend remoto vuelve a ser alcanzable y la credencial está vigente.

## 4. Flujo principal

1. La aplicación host solicita reanudar la sincronización de la sesión en estado reanudable (o vuelve a ejecutar el ciclo, que el motor reconoce como reanudación).
2. El motor identifica los cambios pendientes no confirmados a partir de la cola y de la marca de progreso registrada.
3. Fase de subida reanudada: el motor reenvía únicamente los cambios no confirmados, en su orden de creación; el backend reconoce por identificador estable los que ya había recibido y los confirma sin volver a aplicarlos (idempotencia, RN-02).
4. El motor verifica que la subida concluyó sin pendientes confirmables restantes.
5. Fase de bajada: solo entonces el motor baja las actualizaciones del backend y las aplica al almacén local.
6. El motor deja la sesión en estado listo y devuelve un resumen: cambios efectivamente nuevos confirmados, cambios reconocidos como ya recibidos, actualizaciones bajadas y estado final.

## 5. Flujos alternativos

- 5.A Todos los pendientes ya habían sido recibidos. Disparador: el corte ocurrió después de que el backend recibió todos los cambios pero antes de que el motor registrara la confirmación. Al reanudar, el backend reconoce todos por identificador, no aplica ninguno de nuevo y el motor procede a la bajada. Retorna al paso 6.
- 5.B Nuevo corte durante la reanudación. Disparador: la conectividad vuelve a caer mientras se reenvían los pendientes. El motor conserva el avance de la reanudación, deja la sesión nuevamente reanudable y no inicia la bajada. El próximo intento continúa desde el nuevo punto. Termina dejando la sesión reanudable.

## 6. Excepciones y errores

| Código | Causa | Respuesta del motor |
| --- | --- | --- |
| SESION_NO_REANUDABLE | Se solicita reanudar una sesión que no quedó en estado reanudable | El motor trata la solicitud como un ciclo normal (CU-03) o la rechaza si la sesión no está autenticada, sin inventar progreso |
| BACKEND_INALCANZABLE | El backend sigue sin responder al intentar reanudar | El motor mantiene la sesión reanudable, no baja actualizaciones y devuelve el error |
| PROGRESO_INCONSISTENTE | La marca de progreso y la cola no concuerdan al reanudar | El motor adopta la cola persistida como fuente de verdad, reenvía los pendientes apoyándose en la idempotencia del backend y registra la inconsistencia para diagnóstico |

## 7. Postcondiciones

- Éxito: la subida concluyó incorporando solo los cambios faltantes sin aplicar dos veces los ya recibidos; la bajada ocurrió después; la cola quedó vacía de confirmados y la marca de última sincronización avanzó.
- Fallo: la sesión permanece reanudable con los pendientes intactos; no se bajó ninguna actualización; no hubo pérdida ni duplicación de datos.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Una sesión reanudable con 5 cambios, de los cuales 2 ya fueron confirmados antes del corte | La aplicación host reanuda la sincronización | El motor reenvía solo los 3 restantes, recién entonces baja actualizaciones y reporta 3 nuevos confirmados |
| CA-02 | Una sesión reanudable donde el backend ya había recibido los 5 cambios pero el motor no lo registró | La aplicación host reanuda la sincronización | El backend reconoce los 5 por identificador, no aplica ninguno de nuevo y el motor procede a la bajada sin duplicar datos |
| CA-03 | Una sesión reanudable y backend aún inalcanzable | La aplicación host reanuda la sincronización | El motor devuelve BACKEND_INALCANZABLE, conserva los pendientes y no baja actualizaciones |
| CA-04 | Una sesión reanudable donde la conectividad cae otra vez tras reenviar 1 de 3 pendientes | La aplicación host reanuda la sincronización | El motor conserva el avance, deja la sesión reanudable con 2 pendientes y no inicia la bajada |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-04 |
| Reglas de negocio aplicables | RN-01, RN-02 |
| Historias de usuario a generar | US-12, US-13 (en 06) |
| Componentes esperados | Ejecutor de subida y de bajada; cola de cambios locales pendientes; registro de estado y progreso (referencia tentativa a 05) |
| Tests previstos | Reanudación reenvía solo faltantes; reconocimiento por identificador sin duplicar; backend aún inalcanzable mantiene reanudable; corte durante la reanudación (en 08) |

## 10. Notas y supuestos

- La reanudación se apoya en la idempotencia del backend por identificador de cambio estable (RN-02): el motor puede reenviar sin temor a duplicar.
- La garantía de orden subir-antes-de-bajar (RN-01) se mantiene en la reanudación: nunca se baja antes de concluir la subida pendiente.
- El motor trata la cola persistida como fuente de verdad ante una inconsistencia de progreso, priorizando no perder cambios.
- Este CU cubre el caso límite del intake §7 sobre pérdida de conexión en medio de una sincronización con subida parcial, en términos generales del contrato de la librería.

## 17. Compatibilidad de versión pública

El estado reanudable, su semántica de continuación desde el punto de corte y el contrato del resumen de reanudación integran la superficie pública. Eliminar el estado reanudable, cambiar la forma de continuación o relajar la garantía de no duplicación constituye un cambio incompatible y obliga a incrementar la versión mayor.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de reanudación de una sincronización interrumpida por subida parcial, derivado de NB-04 y del SOLUTION-INTAKE §17 (aplicada-sync) y §7. |
