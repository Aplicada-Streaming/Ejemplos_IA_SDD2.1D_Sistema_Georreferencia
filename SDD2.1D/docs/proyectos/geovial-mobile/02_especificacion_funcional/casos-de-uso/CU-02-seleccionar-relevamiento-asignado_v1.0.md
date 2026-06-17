# CU-02 — Seleccionar un relevamiento asignado

**Proyecto:** geovial-mobile
**Documento:** CU-02-seleccionar-relevamiento-asignado_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + Mobile UX Analyst

## 1. Propósito

Permitir que el agente de campo, ya con sesión activa, vea la lista de los relevamientos que tiene asignados y elija uno para trabajar en terreno, dejándolo como contexto activo de captura, tanto con conexión como a partir de la copia local cuando no hay red. Es el punto de entrada de toda la recolección de observaciones.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Agente de campo | Primario | Consulta sus relevamientos asignados y selecciona uno como contexto de trabajo |
| App móvil | Sistema | Lista los relevamientos asignados desde el almacén local del dispositivo y fija el contexto activo |
| Backend de relevamientos | Sistema | Provee la lista de asignaciones del agente cuando hay conexión |

## 3. Precondiciones

- El agente tiene una sesión activa en la app (CU-01).
- El agente tiene al menos un relevamiento asignado (la asignación la realiza el jefe de área, fuera de este CU).
- El almacén local del dispositivo conserva una copia de los relevamientos asignados sincronizados previamente.

## 4. Flujo principal

1. El agente abre la sección de relevamientos asignados.
2. La app muestra la lista de relevamientos asignados al agente desde el almacén local del dispositivo, con su nombre de tramo y su estado.
3. El agente selecciona un relevamiento.
4. La app fija ese relevamiento como contexto activo de captura y abre la vista de mapa con los marcadores y observaciones de la copia local.
5. El agente queda habilitado para crear marcadores, capturar fotos y registrar observaciones sobre ese relevamiento (CU-03, CU-04, CU-05).

## 5. Flujos alternativos

- 5.A Lista vacía en el primer uso. Disparador: el agente no tiene relevamientos sincronizados aún en el dispositivo. La app, si hay conexión, baja la lista de asignaciones del agente y la almacena localmente; si no hay conexión, informa que no hay relevamientos disponibles sin conexión y retorna al paso 2 cuando haya datos.
- 5.B Refresco de la lista con conexión. Disparador: el agente solicita actualizar la lista y hay red. La app sincroniza para reflejar nuevas asignaciones o cambios y actualiza la copia local (CU-06). Retorna al paso 2.
- 5.C Relevamiento cerrado por el jefe. Disparador: el relevamiento seleccionado figura como cerrado en la copia local. La app lo abre solo en modo lectura para revisar lo recolectado y no habilita nuevas capturas. Termina en vista de solo lectura.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| SIN_RELEVAMIENTOS_LOCALES | El dispositivo no tiene relevamientos asignados sincronizados y no hay conexión para bajarlos | La app informa que no hay relevamientos disponibles sin conexión y no fija contexto activo |
| RELEVAMIENTO_NO_ASIGNADO | El relevamiento elegido ya no figura asignado al agente tras un refresco | La app lo retira de la lista y no lo fija como contexto activo |
| RELEVAMIENTO_CERRADO | El relevamiento seleccionado está cerrado | La app lo abre en modo lectura y no habilita capturas |

## 7. Postcondiciones

- Éxito: un relevamiento queda fijado como contexto activo de captura y su mapa con marcadores y observaciones locales queda abierto.
- Éxito en relevamiento cerrado: el relevamiento queda abierto en modo lectura, sin habilitar capturas.
- Fallo: no se fija contexto activo y la app permanece en la lista de relevamientos.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un agente con sesión activa y 3 relevamientos asignados en la copia local | El agente abre la lista y selecciona el segundo | La app fija ese relevamiento como contexto activo y abre su mapa con los marcadores locales |
| CA-02 | Un agente sin relevamientos sincronizados y sin conexión | El agente abre la lista de relevamientos | La app responde con SIN_RELEVAMIENTOS_LOCALES y no fija contexto activo |
| CA-03 | Un agente con conexión que solicita refrescar y tiene una asignación nueva en el backend | El agente actualiza la lista | La app sincroniza, agrega el relevamiento nuevo a la copia local y lo muestra en la lista |
| CA-04 | Un relevamiento cerrado por el jefe presente en la copia local | El agente lo selecciona | La app lo abre en modo lectura y no habilita nuevas capturas |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-04, NB-03 |
| Reglas de negocio aplicables | RN-05, RN-02 |
| Historias de usuario a generar | US-03, US-04 (en 06) |
| Componentes esperados | Pantalla de lista de relevamientos asignados; servicio de contexto activo; lectura del almacén local; integración con la sincronización (referencia tentativa a 05) |
| Tests previstos | Selección fija contexto y abre mapa local; lista vacía sin conexión rechazada; refresco con conexión agrega asignación; relevamiento cerrado abre en solo lectura (en 08) |

## 10. Notas y supuestos

- La app es offline-first: la lista y el detalle se sirven del almacén local del dispositivo; la conexión solo se usa para refrescar y sincronizar (CU-06).
- La asignación y reasignación de agentes a relevamientos es una operación del jefe de área en el backend; la app solo consume el resultado.
- El conjunto exacto de campos del relevamiento mostrados en la lista pertenece a la categoría 03 (UX/UI); aquí solo se fija que la lista identifica el tramo y el estado.

## 14. Permisos del sistema operativo

- Este CU no requiere permisos de ubicación, cámara ni almacenamiento del sistema operativo; opera sobre el almacén local de la app y, si hay red, sobre la conexión de datos.

## 12. Performance esperado del CU

- La apertura de la lista y la fijación del contexto activo se resuelven contra el almacén local sin depender de la red; la copia local debe responder de forma fluida con relevamientos de tamaño habitual.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de selección de relevamiento asignado, derivado de NB-04 y NB-03 (F-04, F-07). |
