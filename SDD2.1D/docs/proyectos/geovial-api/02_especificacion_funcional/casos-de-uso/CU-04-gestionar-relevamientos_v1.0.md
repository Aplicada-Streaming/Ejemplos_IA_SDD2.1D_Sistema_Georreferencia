# CU-04 — Crear, dar de baja y visualizar relevamientos de un tramo vial

**Proyecto:** geovial-api
**Documento:** CU-04-gestionar-relevamientos_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Permitir que el jefe de área cree un relevamiento delimitando el tramo vial a relevar (uno o varios puentes y caminos), lo dé de baja cuando corresponda y consulte sus relevamientos y el detalle de cada uno. Da al jefe la unidad de trabajo concreta sobre la que se organiza la recolección.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Jefe de área | Primario | Crea, da de baja y visualiza sus relevamientos |
| Backend de relevamientos | Sistema | Valida el alcance, persiste el relevamiento y controla la visibilidad por rol |
| Almacén relacional | Sistema | Persiste el relevamiento, su tramo vial y su estado |

## 3. Precondiciones

- El solicitante está autenticado y su rol es jefe de área (CU-03).
- Para la baja y la visualización del detalle, el relevamiento existe y pertenece al ámbito del jefe solicitante.

## 4. Flujo principal

1. El jefe de área solicita crear un relevamiento indicando el tramo vial (puentes y caminos que abarca) y una descripción.
2. El backend valida que el solicitante es jefe de área y que el alcance del tramo está completo.
3. El backend crea el relevamiento en estado de recolección, lo vincula al jefe que lo creó y registra su tramo vial.
4. El backend responde con la representación del relevamiento y la ubicación del recurso creado.
5. Para visualizar, el jefe solicita la lista de sus relevamientos o el detalle de uno; el backend responde aplicando paginación y filtros (ver CU-20) y respetando su alcance.
6. Para la baja, el jefe solicita dar de baja un relevamiento de su ámbito; el backend lo marca como dado de baja conservando su evidencia asociada.

## 5. Flujos alternativos

- 5.A Visualización por el agente asignado. Disparador: un agente de campo solicita el detalle de un relevamiento que tiene asignado. El backend responde con el relevamiento y sus marcadores, sin permitir su baja ni su edición de alcance. Retorna al paso 5.
- 5.B Baja de un relevamiento ya cerrado. Disparador: el jefe solicita dar de baja un relevamiento en estado de cierre. El backend conserva el cierre como hito y registra la baja sin destruir la evidencia, manteniendo la trazabilidad del informe. Retorna al paso 6.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| ROL_NO_AUTORIZADO | El solicitante no es jefe de área al crear o dar de baja | Rechaza con estado de prohibido y no modifica relevamientos |
| TRAMO_INCOMPLETO | El alcance del tramo vial no define ningún puente ni camino | Rechaza con estado de solicitud inválida y no crea el relevamiento |
| RELEVAMIENTO_FUERA_DE_AMBITO | La operación apunta a un relevamiento de otro jefe | Rechaza con estado de prohibido y no expone ni modifica el relevamiento |

## 7. Postcondiciones

- Éxito en creación: existe un relevamiento en estado de recolección, con su tramo vial y vinculado al jefe que lo creó.
- Éxito en baja: el relevamiento queda dado de baja con su evidencia conservada.
- Éxito en visualización: el solicitante recibe solo los relevamientos de su alcance.
- Fallo: el estado no cambia y se devuelve un problema con el código correspondiente.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un jefe de área autenticado | Crea un relevamiento de un tramo con dos puentes y un camino | El backend crea el relevamiento en estado de recolección y responde con la ubicación del recurso |
| CA-02 | Un jefe de área autenticado | Crea un relevamiento sin indicar ningún puente ni camino | El backend rechaza con el código TRAMO_INCOMPLETO |
| CA-03 | Un jefe de área con 30 relevamientos | Solicita la lista filtrando por estado de recolección con tamaño de página 10 | El backend devuelve 10 relevamientos en recolección y un indicador de la página siguiente |
| CA-04 | Un jefe de área autenticado | Solicita el detalle de un relevamiento creado por otro jefe | El backend rechaza con el código RELEVAMIENTO_FUERA_DE_AMBITO |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-02 |
| Reglas de negocio aplicables | RN-01, RN-05 |
| Historias de usuario a generar | US-07, US-08, US-09 (en 06) |
| Componentes esperados | Recurso de relevamientos; servicio de alcance del tramo; repositorio de relevamientos (referencia tentativa a 05) |
| Tests previstos | Creación con tramo válido; tramo incompleto rechazado; lista paginada y filtrada por estado; acceso fuera de ámbito rechazado (en 08) |

## 10. Notas y supuestos

- La asignación de agentes y la transición de estado se especifican en CU-05 y CU-06; este CU cubre el ciclo de vida del recurso relevamiento en sí.
- El relevamiento nace en estado de recolección; los estados válidos y sus transiciones los fija RN-05.

## 12. Performance esperado del CU

- La lectura del listado y del detalle debe mantenerse dentro del objetivo de lectura del proyecto (p95 menor o igual a 300 ms); la creación y la baja, dentro del objetivo de escritura (p95 menor o igual a 500 ms).

## 15. Idempotencia y reintento

- La creación admite clave de idempotencia para evitar relevamientos duplicados ante reintentos (CU-21).
- La baja repetida de un relevamiento ya dado de baja deja el estado sin cambios y responde con éxito.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de creación, baja y visualización de relevamientos, derivado de NB-02 (F-03). |
