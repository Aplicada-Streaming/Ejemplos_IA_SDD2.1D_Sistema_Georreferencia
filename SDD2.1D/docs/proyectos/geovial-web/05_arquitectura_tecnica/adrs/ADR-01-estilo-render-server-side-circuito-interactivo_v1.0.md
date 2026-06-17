# ADR-01 — Estilo: render server-side con circuito interactivo persistente y separación de capas en el cliente

**Proyecto:** geovial-web
**Documento:** ADR-01-estilo-render-server-side-circuito-interactivo_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto Senior
**Categoría:** Estilo

## 1. Contexto

`geovial-web` es el front web de los roles administradores de la solución GeoVial: crea y administra relevamientos, asigna agentes, crea marcadores iniciales sobre un mapa, revisa la evidencia sobre mapa con un carrusel encadenado de fotos y resuelve conflictos al cierre (CU-01 a CU-11). La experiencia exige interacción continua y rica —un componente de mapa con pines que se mueven, un carrusel que encadena marcadores contiguos— que el usuario percibe sin recargas (CU-05, CU-06; 03 experiencia-de-uso). El front no es dueño del dominio: consume por contrato la API REST de `geovial-api` (intake §14, §17.P.3) y no posee persistencia propia (§17.P.4). El equipo es de un desarrollador (intake §2), lo que premia un modelo de desarrollo unificado. El intake fija el estilo como decisión pre-tomada: render server-side con circuito interactivo persistente y una biblioteca de componentes de UI (§17.P.2, §17.P.11). Motiva esta decisión NB-01, NB-02 y NB-05, los once CU del front y las cinco RN de presentación.

## 2. Decisión

Se adopta un front de render server-side con circuito interactivo persistente: la interfaz se renderiza en el servidor y mantiene, por sesión de usuario, un circuito de larga vida que sincroniza el estado de la interfaz con el navegador sobre una conexión persistente. Sobre ese estilo se impone una separación de capas en el cliente de dependencia unidireccional hacia el núcleo de la aplicación de UI: Presentación (vistas y componentes, incluido el componente de mapa), Aplicación de UI (orquestación de la interacción, estado de sesión y de pantalla, mapeo de errores) y Cliente de API (adaptador hacia el contrato REST de `geovial-api`). Ninguna vista accede a la red directamente; toda llamada al dominio pasa por el Cliente de API. El front mantiene únicamente estado de UI y de sesión efímero en el circuito; el dato autoritativo vive en la API.

## 3. Estado

Aceptado el 2026-06-15. Decisión pre-tomada en el intake (§17.P.2, §17.P.11): render server-side con circuito interactivo persistente y biblioteca de componentes de UI fijados por requisito. La separación de capas en el cliente es la materialización que el arquitecto impone sobre ese estilo.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Render server-side con circuito interactivo persistente y separación de capas (elegido) | Modelo de desarrollo unificado; el token se custodia del lado servidor; empuje de estado al navegador para mapa y carrusel; un solo artefacto | Exige conexión persistente con el servidor; requiere afinidad de sesión al escalar |
| Cliente enriquecido que ejecuta en el navegador | Tolera cortes de red del cliente; descarga trabajo del servidor | Descartado por el intake (§17.P.2); obligaría a custodiar el token en el navegador (contra §17.P.5) y a duplicar lógica de dominio |
| Front de páginas sin estado interactivo persistente | Simplicidad; sin conexión persistente | No sostiene la interacción continua de mapa y carrusel (CU-05, CU-06); degrada la experiencia |
| Front sin separación de capas (lógica y acceso a la API en las vistas) | Menos componentes | Impide probar la presentación y el mapeo de errores sin red; dispersa el manejo del token y del contrato |

## 5. Consecuencias positivas

1. El token bearer se custodia del lado servidor del circuito y no se expone al navegador (habilita ADR-03), reduciendo la superficie de exposición.
2. La interacción de mapa y carrusel (CU-05, CU-06) se sostiene por empuje de estado del servidor al navegador sin recargas, alineada con la experiencia de 03.
3. La separación de capas permite probar la Aplicación de UI y el Cliente de API sin levantar la red, y aísla el consumo del contrato en un único componente, acotando el acoplamiento a `geovial-api`.
4. Un único modelo de desarrollo y un solo artefacto desplegable se ajustan a un equipo de un desarrollador (intake §2).

## 6. Consecuencias negativas y trade-offs

1. El estilo exige una conexión persistente con el servidor: un corte de red pierde el circuito y su estado de UI en curso. Se acepta porque el estado es efímero y reconstruible desde la API (fuente de verdad) y ninguna acción de dominio depende de retener estado de UI no confirmado.
2. Escalar a más de una réplica del contenedor de front exige afinidad de sesión (las solicitudes de un circuito vuelven a la misma réplica). Se acepta y se traslada a 09.
3. El estado por circuito consume memoria del servidor por usuario activo; se acota manteniéndolo de tamaño pequeño y reconstruible, y se valida contra el objetivo de circuitos concurrentes (NFR §8).

## 7. Implementación

- La capa de Presentación expone las once vistas de CU y el componente de carrusel; el componente de mapa se integra mediante un adaptador (vista de mapa y marcadores).
- La capa de Aplicación de UI aloja los orquestadores de interacción de cada CU, el servicio de sesión y token, el control de visibilidad por rol, el control de habilitación por estado y el mapeador de errores.
- La capa de Cliente de API implementa el puerto de acceso al dominio traduciendo intenciones en llamadas al contrato REST de `geovial-api` y normalizando respuestas y errores.
- Convención impuesta: ninguna vista llama a la red; todo acceso al dominio pasa por el Cliente de API. El estado de dominio nunca se persiste en el front; se consulta a la API y se cachea solo en memoria del circuito.

## 8. Métricas de validación

- Latencia de interacción p95 ≤ 200 ms sobre el circuito en red estable, medida sobre las pantallas clave excluyendo la latencia del backend (NFR §8, 08).
- Al menos 50 circuitos interactivos concurrentes sostenidos sin pérdida de estado de sesión ni degradación de la latencia p95 (NFR §8, 08).
- El reciclado o la pérdida del circuito reconstruye la pantalla desde la API sin pérdida de dato de dominio, verificado en 08.
- Cobertura de pruebas: líneas ≥ 80 %, branches ≥ 70 %, presentación ≥ 60 % (intake §17.P.6).

## 9. Referencias

- NB-01, NB-02, NB-05; CU-01 a CU-11; RN-01 a RN-05.
- Intake §14, §17 geovial-web P.2, P.11, P.12.
- ADRs relacionadas: ADR-02 (persistencia), ADR-03 (autenticación), ADR-04 (separación de capas), ADR-05 (manejo de errores).
- `arquitectura-solucion_v1.0.md` §2, §3, §4; 03 `experiencia-de-uso_v1.0.md`.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión de estilo: render server-side con circuito interactivo persistente y separación de capas en el cliente (Presentación / Aplicación de UI / Cliente de API). Aceptada (pre-tomada en intake §17.P.2, §17.P.11). |
