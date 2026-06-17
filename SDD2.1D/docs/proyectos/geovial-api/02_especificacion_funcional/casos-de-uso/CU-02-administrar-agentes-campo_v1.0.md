# CU-02 — Dar de alta y de baja agentes de campo por el jefe de área

**Proyecto:** geovial-api
**Documento:** CU-02-administrar-agentes-campo_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Permitir que el jefe de área dé de alta y de baja directamente a los agentes de campo de su área, sin escalar a un administrador central, dejando registrada la pertenencia del agente al jefe que lo creó. Es la instancia operativa más frecuente de la administración jerárquica y la que habilita la recolección en terreno.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Jefe de área | Primario | Da de alta y de baja a los agentes de su área |
| Backend de usuarios | Sistema | Valida que el solicitante sea jefe de área y aplica el control de alcance |
| Almacén relacional | Sistema | Persiste el agente, su rol y su vínculo de pertenencia al jefe |

## 3. Precondiciones

- El solicitante está autenticado y su rol es jefe de área (CU-03).
- Para una baja, el agente destino existe y pertenece al área del jefe solicitante.

## 4. Flujo principal

1. El jefe de área solicita el alta de un agente indicando sus datos de identificación y acceso.
2. El backend valida que el solicitante es jefe de área y que el rol destino es agente de campo (el nivel inmediato inferior, RN-01).
3. El backend valida la unicidad del identificador de acceso del agente.
4. El backend crea el agente con rol agente de campo y lo vincula al jefe de área solicitante como su administrador.
5. El backend responde con la representación del agente y la ubicación del recurso creado.
6. Para una baja, el jefe solicita inhabilitar a un agente de su área; el backend revoca su acceso, conserva la autoría de sus observaciones y lo deja inhabilitado (RN-02).

## 5. Flujos alternativos

- 5.A Baja de un agente con relevamientos asignados. Disparador: el agente a dar de baja tiene relevamientos asignados en recolección. El backend inhabilita su acceso y deja los relevamientos sin ese agente asignado para que el jefe los reasigne (ver CU-05), sin perder lo ya recolectado. Retorna al paso 6.
- 5.B Alta reintentada con clave de idempotencia. Disparador: el jefe reintenta un alta tras una respuesta no recibida. El backend devuelve el agente ya creado sin duplicarlo (ver CU-21). Retorna al paso 5.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| ROL_NO_AUTORIZADO | El solicitante no es jefe de área | Rechaza con estado de prohibido; no crea ni modifica al agente |
| AGENTE_FUERA_DE_AREA | La baja apunta a un agente que no pertenece al área del jefe | Rechaza con estado de prohibido y no modifica al agente |
| IDENTIFICADOR_DUPLICADO | El identificador de acceso del agente ya existe | Rechaza con estado de conflicto y no crea el agente |

## 7. Postcondiciones

- Éxito en alta: existe un agente de campo habilitado, vinculado al jefe de área que lo creó.
- Éxito en baja: el agente queda inhabilitado, sus relevamientos quedan disponibles para reasignación y su autoría histórica se conserva.
- Fallo: el estado de usuarios no cambia y se devuelve un problema con el código correspondiente.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un jefe de área autenticado y un identificador de agente libre | Da de alta a un agente de campo | El backend crea el agente vinculado al jefe y responde con la ubicación del recurso en menos de 10 minutos de gestión total |
| CA-02 | Un agente de campo recién creado por otro jefe de área | El jefe actual solicita la baja de ese agente | El backend rechaza con el código AGENTE_FUERA_DE_AREA |
| CA-03 | Un agente con dos relevamientos asignados en recolección | El jefe da de baja al agente | El backend inhabilita el acceso del agente y deja los dos relevamientos disponibles para reasignación |
| CA-04 | Un usuario con rol agente de campo autenticado | Intenta dar de alta a otro agente | El backend rechaza con el código ROL_NO_AUTORIZADO |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-01 |
| Reglas de negocio aplicables | RN-01, RN-02 |
| Historias de usuario a generar | US-03, US-04 (en 06) |
| Componentes esperados | Recurso de usuarios filtrado por área; servicio de pertenencia jefe-agente; repositorio de usuarios (referencia tentativa a 05) |
| Tests previstos | Alta de agente por jefe aceptada; baja fuera de área rechazada; baja libera relevamientos asignados; alta por agente rechazada (en 08) |

## 10. Notas y supuestos

- Este CU es la materialización del tramo jefe de área → agente de campo de la jerarquía general (CU-01), con sus reglas de área propias; no se fusiona con CU-01 porque tiene un actor primario distinto y consecuencias específicas sobre relevamientos.
- El auto-registro o la solicitud self-service de agentes está excluido del alcance (F-18, Won't Have v1); el alta la hace siempre el jefe.

## 12. Performance esperado del CU

- El alta o la baja de un agente debe resolverse dentro del objetivo de escritura (p95 menor o igual a 500 ms), de modo que el tiempo total de habilitación de un agente nuevo se mantenga en el orden de los minutos.

## 15. Idempotencia y reintento

- El alta admite clave de idempotencia para reintentos seguros (CU-21).
- La baja repetida de un agente ya inhabilitado deja el estado sin cambios y responde con éxito.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de alta y baja de agentes de campo por el jefe de área, derivado de NB-01 (F-02). |
