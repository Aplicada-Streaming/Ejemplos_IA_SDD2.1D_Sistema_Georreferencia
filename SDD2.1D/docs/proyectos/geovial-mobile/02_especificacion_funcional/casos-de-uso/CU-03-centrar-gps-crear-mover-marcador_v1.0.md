# CU-03 — Centrar por GPS y crear o mover un marcador en el mapa

**Proyecto:** geovial-mobile
**Documento:** CU-03-centrar-gps-crear-mover-marcador_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + Mobile UX Analyst

## 1. Propósito

Permitir que el agente de campo centre el mapa sobre su posición tomada del GPS del dispositivo y, sobre esa posición, cree un marcador geográfico nuevo o mueva uno existente, dejándolo registrado en el almacén local para anclar sus observaciones. Es el gesto base de georreferenciación en terreno, ejecutable sin conexión.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Agente de campo | Primario | Centra por GPS y crea o mueve marcadores sobre el mapa |
| App móvil | Sistema | Resuelve la posición del GPS, registra el marcador en el almacén local y lo encola para sincronizar |
| Proveedor de ubicación del dispositivo | Sistema | Entrega la posición geográfica actual (GPS) cuando hay señal |
| Componente de mapa | Sistema | Renderiza el mapa, los pines y la posición del agente |

## 3. Precondiciones

- El agente tiene una sesión activa (CU-01) y un relevamiento abierto como contexto activo en estado de recolección (CU-02).
- La app cuenta con el permiso de ubicación concedido por el sistema operativo.

## 4. Flujo principal

1. El agente toca la acción de centrar por GPS.
2. La app solicita la posición actual al proveedor de ubicación del dispositivo y centra el mapa sobre esa posición.
3. El agente toca el pin general y crea un marcador en la posición tomada del GPS.
4. La app crea el marcador en el almacén local con una identidad propia y estable, su coordenada y, opcionalmente, etiquetas, y lo encola como cambio local pendiente de sincronizar.
5. El agente puede mover un marcador existente a una nueva coordenada (la actual del GPS o una elegida en el mapa); la app actualiza la coordenada conservando la identidad del marcador y encola el cambio.
6. El marcador queda disponible como ancla para capturar fotos y registrar observaciones (CU-04, CU-05).

## 5. Flujos alternativos

- 5.A Marcador creado dentro del radio de otro. Disparador: la coordenada del nuevo marcador cae dentro del radio de agrupación de un marcador existente. La app crea el marcador igualmente y lo deja convivir como posible conflicto, sin bloquear; la resolución se difiere al cierre desde la web (RN-03). Retorna al paso 6.
- 5.B Ubicación elegida manualmente en el mapa. Disparador: el agente prefiere fijar el marcador tocando un punto del mapa en lugar de usar el GPS. La app crea o mueve el marcador en la coordenada tocada y lo encola igual. Retorna al paso 6.
- 5.C Posición de baja precisión. Disparador: el GPS entrega una posición con precisión pobre. La app igualmente centra y permite crear el marcador, señalando que la precisión es baja para que el agente la ajuste si corresponde. Retorna al paso 3.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| PERMISO_UBICACION_DENEGADO | El usuario no concedió el permiso de ubicación del sistema operativo | La app no centra por GPS, explica que el permiso es necesario y ofrece fijar el marcador manualmente en el mapa |
| SIN_SENAL_GPS | El dispositivo no obtiene posición del GPS al momento de centrar | La app informa que no hay señal y permite fijar el marcador manualmente en el mapa, sin inventar coordenada |
| RELEVAMIENTO_CERRADO | El relevamiento activo está cerrado | La app no permite crear ni mover marcadores y lo deja en modo lectura |

## 7. Postcondiciones

- Éxito en creación: existe un marcador en el almacén local con identidad propia, coordenada y etiquetas, encolado como cambio local pendiente.
- Éxito en movimiento: el marcador conserva su identidad y refleja la nueva coordenada en el almacén local, con el cambio encolado.
- Fallo: el estado de marcadores en el almacén local no cambia y el agente recibe la indicación correspondiente.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un agente con un relevamiento en recolección y permiso de ubicación concedido | El agente centra por GPS y crea un marcador en la posición | La app crea el marcador en el almacén local con identidad propia y lo encola como cambio pendiente |
| CA-02 | Un marcador existente y su radio de agrupación | El agente crea otro marcador dentro de ese radio | La app crea el segundo marcador y lo deja convivir sin bloquear, difiriendo el conflicto al cierre |
| CA-03 | Un dispositivo sin señal de GPS | El agente toca centrar por GPS | La app responde con SIN_SENAL_GPS y ofrece fijar el marcador manualmente en el mapa |
| CA-04 | Un agente que niega el permiso de ubicación | El agente intenta centrar por GPS | La app responde con PERMISO_UBICACION_DENEGADO y ofrece fijación manual en el mapa |
| CA-05 | Un marcador existente en el mapa | El agente lo arrastra a una nueva coordenada | La app actualiza la coordenada conservando la identidad del marcador y encola el cambio |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-03 |
| Reglas de negocio aplicables | RN-03, RN-05 |
| Historias de usuario a generar | US-05, US-06 (en 06) |
| Componentes esperados | Componente de mapa con pines; servicio de ubicación del dispositivo; repositorio local de marcadores; cola local de cambios (referencia tentativa a 05) |
| Tests previstos | Centrar por GPS y crear marcador encolado; convivencia con conflicto por radio; sin señal de GPS ofrece fijación manual; permiso denegado ofrece fijación manual; mover marcador conserva identidad (en 08) |

## 10. Notas y supuestos

- La identidad del marcador es estable: moverlo o etiquetarlo no genera uno nuevo, alineado con el dominio autoritativo del backend (geovial-api, marcador con identidad propia y estable).
- El conflicto de marcadores por radio convive durante la recolección y se resuelve al cierre desde la web; la app solo lo registra, no lo resuelve (RN-03).
- El caso de captura sin señal de GPS figura como pendiente de respuesta del cliente en el intake (§7); aquí se asume que el marcador puede fijarse manualmente en el mapa sin inventar coordenada, a confirmar con el negocio (alineado con geovial-api 02 §9).
- El render del mapa, el estilo de los pines y los gestos táctiles concretos pertenecen a la categoría 03 (UX/UI); aquí solo se fija el qué.

## 14. Permisos del sistema operativo

- Requiere el permiso de ubicación (GPS) del sistema operativo para centrar y tomar coordenadas; si se deniega, la app degrada a fijación manual en el mapa (PERMISO_UBICACION_DENEGADO).
- El permiso se solicita en el momento del primer uso de la función de ubicación; el agente puede revocarlo desde el sistema operativo en cualquier momento, lo que reactiva la degradación a fijación manual.

## 12. Performance esperado del CU

- La obtención de posición y el centrado del mapa se resuelven en terreno sin depender de la red; la creación y el movimiento del marcador funcionan 100 % sin conexión, alineado con el NFR de captura offline del proyecto.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de centrado por GPS y creación o movimiento de marcador en el mapa, derivado de NB-03 (F-05, F-10). |
