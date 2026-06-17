# CU-04 — Capturar una foto con resolución de coordenadas en el momento

**Proyecto:** geovial-mobile
**Documento:** CU-04-capturar-foto-resolver-coordenadas_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + Mobile UX Analyst

## 1. Propósito

Permitir que el agente de campo tome una fotografía en terreno y que la app resuelva la coordenada geográfica en el momento de la captura, anclándola al marcador del entorno como parte de una observación, todo en el almacén local y sin necesidad de conexión. Es la captura georreferenciada que ancla la evidencia al punto del tramo donde fue tomada.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Agente de campo | Primario | Toma la foto en el lugar de la observación |
| App móvil | Sistema | Resuelve la coordenada en el momento, ancla la foto al marcador, crea la observación local y la encola |
| Cámara del dispositivo | Sistema | Captura la imagen |
| Proveedor de ubicación del dispositivo | Sistema | Entrega la coordenada del momento de la captura |

## 3. Precondiciones

- El agente tiene una sesión activa (CU-01) y un relevamiento abierto en recolección (CU-02).
- Existe un marcador del entorno al cual anclar la foto, o el agente acaba de crear uno por GPS (CU-03).
- La app cuenta con los permisos de cámara y de ubicación concedidos por el sistema operativo.

## 4. Flujo principal

1. El agente toca la acción de capturar foto sobre el marcador activo.
2. La app abre la cámara del dispositivo y el agente toma la fotografía.
3. En el momento de la captura, la app resuelve la coordenada geográfica desde el proveedor de ubicación del dispositivo.
4. La app crea o reutiliza la observación anclada al marcador del entorno, asocia la foto a esa observación con su coordenada resuelta y aloja la imagen en el almacén local del dispositivo.
5. La app encola la observación y la foto como cambios locales pendientes de sincronizar (CU-06).
6. La foto queda disponible para agregarle comentario y etiqueta (CU-05).

## 5. Flujos alternativos

- 5.A Captura sin señal de GPS en el momento. Disparador: al tomar la foto, el dispositivo no obtiene coordenada del GPS. La app conserva la foto anclada al marcador del entorno y la registra como pendiente de ubicación precisa, sin inventar coordenada; el agente puede ajustarla luego en el mapa. Retorna al paso 5.
- 5.B Foto asociada a un marcador ya existente. Disparador: el agente captura sobre un marcador previo en vez de uno recién creado. La app agrega la foto a una observación del mismo marcador, que queda compartido por varias observaciones. Retorna al paso 5.
- 5.C Captura sin un marcador del entorno. Disparador: no hay marcador cercano al agente. La app crea un marcador en la coordenada del momento (CU-03) y ancla la foto a él. Retorna al paso 4.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| PERMISO_CAMARA_DENEGADO | El usuario no concedió el permiso de cámara del sistema operativo | La app no abre la cámara y explica que el permiso es necesario para capturar |
| SIN_SENAL_GPS | El dispositivo no obtiene coordenada al momento de la captura | La app conserva la foto sin coordenada precisa y la marca como pendiente de ubicación, sin inventar coordenada |
| ALMACEN_LOCAL_SIN_ESPACIO | El almacén local del dispositivo no tiene espacio para alojar la imagen | La app no guarda la foto y avisa al agente que libere espacio |
| RELEVAMIENTO_CERRADO | El relevamiento activo está cerrado | La app no permite capturar y lo deja en modo lectura |

## 7. Postcondiciones

- Éxito: existe una observación anclada a un marcador con una foto que tiene su coordenada resuelta, alojada en el almacén local y encolada como cambio pendiente.
- Éxito sin GPS: la foto queda anclada al marcador del entorno y marcada como pendiente de ubicación precisa, sin coordenada inventada.
- Fallo: no se crea la observación ni se guarda la foto y el agente recibe la indicación correspondiente.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un agente sobre un marcador activo con permisos de cámara y ubicación concedidos y señal de GPS | El agente toma una foto | La app resuelve la coordenada en el momento, ancla la foto a una observación del marcador y la encola |
| CA-02 | Un agente que toma una foto sin señal de GPS | El agente confirma la captura | La app conserva la foto anclada al marcador y la marca como pendiente de ubicación, sin inventar coordenada |
| CA-03 | Un agente que negó el permiso de cámara | El agente toca capturar foto | La app responde con PERMISO_CAMARA_DENEGADO y no abre la cámara |
| CA-04 | Un marcador existente con una observación previa | El agente captura otra foto sobre ese marcador | La app agrega la foto al mismo marcador, que queda compartido por varias observaciones |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-03 |
| Reglas de negocio aplicables | RN-01, RN-05 |
| Historias de usuario a generar | US-07, US-08 (en 06) |
| Componentes esperados | Integración con la cámara; servicio de ubicación del dispositivo; repositorio local de observaciones y fotos; almacenamiento local de binarios; cola local de cambios (referencia tentativa a 05) |
| Tests previstos | Captura resuelve coordenada y ancla foto; captura sin GPS deja pendiente de ubicación; permiso de cámara denegado bloquea captura; marcador compartido por varias observaciones (en 08) |

## 10. Notas y supuestos

- La captura de foto con resolución de coordenadas en el momento funciona 100 % sin conexión; la coordenada se toma del dispositivo, no de la red (NFR de captura offline del proyecto).
- El caso de captura sin señal de GPS figura como pendiente de respuesta del cliente en el intake (§7); aquí se asume que la foto queda anclada al marcador y pendiente de ubicación precisa, sin coordenada inventada, a confirmar con el negocio (alineado con geovial-api 02 §9).
- El binario de la foto se aloja localmente y se transfiere al almacén de archivos del backend durante la sincronización (CU-06); la organización física del binario pertenece a la categoría 05.
- El dominio autoritativo de observación, foto y marcador es el de geovial-api; la app trabaja sobre una réplica local.

## 14. Permisos del sistema operativo

- Requiere el permiso de cámara para capturar la imagen; si se deniega, la captura no procede (PERMISO_CAMARA_DENEGADO).
- Requiere el permiso de ubicación para resolver la coordenada en el momento; si no hay señal o el permiso se revoca, aplica 5.A o SIN_SENAL_GPS.
- Puede requerir acceso al almacenamiento del dispositivo para alojar la imagen, según la plataforma; la falta de espacio se trata como ALMACEN_LOCAL_SIN_ESPACIO.

## 12. Performance esperado del CU

- La captura y el anclaje con resolución de coordenadas se completan en terreno sin conexión; la operación se mantiene fluida con la cámara del dispositivo de referencia.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de captura de foto con resolución de coordenadas en el momento, derivado de NB-03 (F-05, F-06). |
