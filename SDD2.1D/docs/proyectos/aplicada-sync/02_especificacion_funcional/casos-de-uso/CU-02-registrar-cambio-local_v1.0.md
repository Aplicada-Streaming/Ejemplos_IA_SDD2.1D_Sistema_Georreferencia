# CU-02 — Registrar y encolar un cambio local

**Proyecto:** aplicada-sync
**Documento:** CU-02-registrar-cambio-local_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Permitir que la aplicación host registre en el motor un cambio local producido sin conexión, asignándole un identificador estable, para que quede encolado de forma persistente y ordenada a la espera de ser subido en la próxima sincronización. Es la entrada de trabajo al motor.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Aplicación host | Primario | Entrega el cambio local a encolar |
| Almacén local del host | Sistema | Persiste la cola de cambios pendientes y sus identificadores |

## 3. Precondiciones

- Existe una sesión de sincronización inicializada (CU-01), aunque esté en estado no autenticada.
- La aplicación host puede entregar un cambio local con un identificador de cambio estable y una marca de orden de creación.
- El almacén local del host admite escritura.

## 4. Flujo principal

1. La aplicación host construye el cambio local: identificador de cambio estable, tipo u operación, carga útil opaca para el motor y marca de orden de creación.
2. La aplicación host solicita al motor encolar el cambio local sobre la sesión vigente.
3. El motor valida que el identificador de cambio esté presente y que no exista ya un cambio pendiente con el mismo identificador.
4. El motor persiste el cambio en la cola de pendientes del almacén local, conservando el orden relativo de creación.
5. El motor devuelve la confirmación de encolado y el tamaño actualizado de la cola de pendientes.

## 5. Flujos alternativos

- 5.A Reencolado idempotente del mismo cambio. Disparador: la aplicación host vuelve a encolar un cambio con un identificador ya presente en la cola y aún no subido. El motor no duplica la entrada; actualiza la carga útil conservando una sola entrada por identificador y confirma sin incrementar el tamaño de la cola. Retorna al paso 5.
- 5.B Encolado de un cambio que anula a uno previo. Disparador: el host registra un cambio que marca como eliminado un dato cuyo alta aún está pendiente de subir. El motor conserva ambos en la cola respetando el orden de creación, sin colapsarlos, para que el backend reciba la secuencia completa. Retorna al paso 5.

## 6. Excepciones y errores

| Código | Causa | Respuesta del motor |
| --- | --- | --- |
| IDENTIFICADOR_CAMBIO_AUSENTE | El cambio local llega sin identificador de cambio estable | Rechaza el encolado y no modifica la cola |
| SESION_NO_INICIALIZADA | No hay una sesión de sincronización inicializada para encolar | Rechaza el encolado y devuelve el error indicando que debe inicializarse la sesión |
| ALMACEN_LOCAL_SIN_ESPACIO | El almacén local no puede persistir el cambio por falta de espacio | Rechaza el encolado, no deja entrada parcial y devuelve el error |

## 7. Postcondiciones

- Éxito: el cambio queda persistido en la cola de pendientes con su identificador y su orden de creación; el tamaño de la cola refleja exactamente las entradas únicas pendientes.
- Fallo: la cola de pendientes queda inalterada; no se persiste una entrada parcial ni duplicada.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Una sesión inicializada con la cola vacía | La aplicación host encola un cambio con identificador "chg-100" | El motor confirma el encolado y reporta tamaño de cola igual a 1 |
| CA-02 | Una cola que ya contiene el cambio "chg-100" sin subir | La aplicación host vuelve a encolar el cambio "chg-100" | El motor conserva una sola entrada para "chg-100" y reporta tamaño de cola igual a 1 |
| CA-03 | Una sesión inicializada | La aplicación host intenta encolar un cambio sin identificador | El motor rechaza la operación con el código IDENTIFICADOR_CAMBIO_AUSENTE y deja la cola inalterada |
| CA-04 | No existe ninguna sesión inicializada | La aplicación host intenta encolar un cambio "chg-200" | El motor rechaza la operación con el código SESION_NO_INICIALIZADA |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-04 |
| Reglas de negocio aplicables | RN-02 |
| Historias de usuario a generar | US-03, US-04 (en 06) |
| Componentes esperados | Cola de cambios locales pendientes (referencia tentativa a 05) |
| Tests previstos | Encolado nuevo incrementa la cola; reencolado del mismo identificador no duplica; rechazo por identificador ausente; rechazo sin sesión inicializada (en 08) |

## 10. Notas y supuestos

- La carga útil del cambio es opaca para el motor: la librería no interpreta ni valida el contenido de dominio del host.
- El identificador de cambio estable es responsabilidad del host y es la base de la idempotencia (RN-02): el mismo cambio reencolado o reenviado se reconoce por ese identificador.
- El motor no decide el orden de negocio de los cambios; conserva el orden de creación que el host declara.

## 17. Compatibilidad de versión pública

El contrato del cambio local (presencia obligatoria del identificador estable, opacidad de la carga útil y conservación del orden de creación) integra la superficie pública. Cambiar la obligatoriedad del identificador o la semántica de no duplicación por identificador constituye un cambio incompatible y obliga a incrementar la versión mayor.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de registro y encolado de un cambio local, derivado de NB-04 y del SOLUTION-INTAKE §17 (aplicada-sync). |
