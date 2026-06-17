# CU-05 — Consultar estado del motor y cola de pendientes

**Proyecto:** aplicada-sync
**Documento:** CU-05-consultar-estado-cola_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Permitir que la aplicación host consulte en cualquier momento el estado del motor de sincronización y el contenido de la cola de cambios pendientes, para informar a su persona usuaria, decidir si dispara una sincronización o diagnosticar una sincronización interrumpida. Es la cara de observabilidad del contrato.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Aplicación host | Primario | Consulta el estado y la cola |
| Almacén local del host | Sistema | Fuente de la cola persistida y de la marca de última sincronización |

## 3. Precondiciones

- Existe una sesión de sincronización inicializada (CU-01), aunque esté en estado no autenticada o reanudable.
- El almacén local del host es accesible para lectura.

## 4. Flujo principal

1. La aplicación host solicita al motor el estado actual de la sesión.
2. El motor compone el estado: situación de la sesión (listo, no autenticada, sincronizando, reanudable), cantidad de cambios pendientes, marca de última sincronización y cantidad de elementos en conflicto conocidos.
3. La aplicación host solicita, opcionalmente, el detalle de la cola de pendientes.
4. El motor devuelve la lista de cambios pendientes con su identificador y su orden de creación, sin exponer la carga útil de dominio si el host no la solicita.
5. La aplicación host usa la información devuelta para su propia presentación o decisión.

## 5. Flujos alternativos

- 5.A Consulta durante una sincronización en curso. Disparador: el host consulta mientras hay un ciclo activo. El motor devuelve estado "sincronizando" con el progreso parcial: cuántos cambios ya se subieron y cuántos restan. Retorna al paso 5.
- 5.B Consulta de elementos en conflicto. Disparador: el host pide específicamente los elementos marcados en conflicto. El motor devuelve la lista de identificadores en conflicto conocidos, dejando claro que el motor convive con ellos y no los resuelve (ver RN-03). Retorna al paso 5.

## 6. Excepciones y errores

| Código | Causa | Respuesta del motor |
| --- | --- | --- |
| SESION_NO_INICIALIZADA | No hay una sesión inicializada para consultar | Devuelve el error indicando que debe inicializarse la sesión antes de consultar |
| ALMACEN_LOCAL_INACCESIBLE | El almacén local no puede leerse para componer la cola | Devuelve el estado en memoria disponible y marca la cola como no legible, sin inventar datos |

## 7. Postcondiciones

- Éxito: la aplicación host recibe un estado consistente con la cola persistida en el momento de la consulta; la consulta no altera la cola ni el estado.
- Fallo: la consulta no modifica nada; el error describe la causa sin dejar el motor en un estado distinto al previo.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Una sesión inicializada con 4 cambios pendientes y sin sincronización en curso | La aplicación host consulta el estado | El motor devuelve estado "listo" y cantidad de pendientes igual a 4 |
| CA-02 | Una sesión con un ciclo en curso que ya subió 1 de 3 cambios | La aplicación host consulta el estado | El motor devuelve estado "sincronizando" con 1 subido y 2 restantes |
| CA-03 | Una sesión donde el backend reportó 2 elementos en conflicto en la última bajada | La aplicación host consulta los elementos en conflicto | El motor devuelve 2 identificadores en conflicto y los reporta como convivientes, no resueltos |
| CA-04 | No existe ninguna sesión inicializada | La aplicación host consulta el estado | El motor devuelve el error SESION_NO_INICIALIZADA |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-04 |
| Reglas de negocio aplicables | RN-03 |
| Historias de usuario a generar | US-10, US-11 (en 06) |
| Componentes esperados | Registro de estado y progreso de la sincronización; cola de cambios locales pendientes (referencia tentativa a 05) |
| Tests previstos | Estado con cola pendiente; progreso parcial durante el ciclo; listado de elementos en conflicto; error sin sesión inicializada (en 08) |

## 10. Notas y supuestos

- La consulta es de solo lectura: jamás dispara una sincronización ni modifica la cola.
- El motor reporta los elementos en conflicto, pero no los resuelve; la resolución pertenece al backend o a la aplicación host (RN-03).
- El detalle de la carga útil de dominio se devuelve únicamente si el host lo solicita, manteniendo la opacidad por defecto.

## 17. Compatibilidad de versión pública

El conjunto de estados que el motor reporta (listo, no autenticada, sincronizando, reanudable) y la forma del estado y de la cola devueltos integran la superficie pública. Quitar un estado, renombrar un campo del estado o cambiar la semántica de los elementos en conflicto constituye un cambio incompatible y obliga a incrementar la versión mayor.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de consulta de estado del motor y de la cola de pendientes, derivado de NB-04 y del SOLUTION-INTAKE §17 (aplicada-sync). |
