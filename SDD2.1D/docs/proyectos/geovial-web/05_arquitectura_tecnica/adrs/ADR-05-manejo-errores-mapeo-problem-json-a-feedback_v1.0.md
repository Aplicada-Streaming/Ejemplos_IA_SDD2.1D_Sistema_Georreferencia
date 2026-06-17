# ADR-05 — Manejo de errores: mapeo de problem+json de la API a feedback de UI

**Proyecto:** geovial-web
**Documento:** ADR-05-manejo-errores-mapeo-problem-json-a-feedback_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Arquitecto Senior
**Categoría:** Comunicación

## 1. Contexto

`geovial-web` no genera errores de dominio propios: todo error proviene del contrato REST de `geovial-api`, que los devuelve como problem+json RFC 7807 con un código estable en mayúsculas sin tildes, opaco al idioma (contratos-rest de geovial-api §5; ADR-05 de geovial-api). El front debe traducir ese error a un feedback comprensible para el usuario administrador, alineado con el catálogo de mensajes de la sección 03 (dx/mensajes de UX), sin filtrar detalles internos del backend. Hay errores que el front debe presentar de forma específica: credenciales inválidas (CU-01), conflictos pendientes que bloquean el cierre (RN-05, CU-07, CU-08), acción fuera del alcance del rol (RN-01, RN-03), recurso no encontrado o no disponible en la versión (acoplamiento al contrato), proveedor de almacenamiento no disponible (CU-11) y unidad transferible malformada (CU-10). El intake deja el manejo de sesión y el detalle de presentación como ratificables (§17 geovial-web P.3, P.11), por lo que esta decisión es del arquitecto y se declara `Propuesto`. Motivan esta decisión las once CU del front y las RN-01, RN-03, RN-04 y RN-05.

## 2. Decisión

Se adopta un mapeador de errores a feedback de UI, centralizado en la Aplicación de UI, que traduce cada código problem+json de `geovial-api` a un estado de pantalla y un mensaje comprensibles, alineados con el catálogo de mensajes de 03. El mapeo se gobierna por el código estable del error (no por el texto), de modo que sea independiente del idioma. El mapeador distingue al menos: errores de validación de entrada (se resaltan los campos implicados sin perder lo ya ingresado), errores de autorización y alcance (se informa que la acción no está disponible para el rol, RN-01/RN-03), errores de estado y de ciclo (no se ofrece el cierre con conflictos pendientes, RN-05; las acciones inválidas para el estado quedan deshabilitadas, RN-04), errores de versión del contrato (se informa una incompatibilidad y se evita reintentar a ciegas) y errores genéricos no contemplados (se presenta un fallo genérico sin filtrar detalles del backend). El Cliente de API normaliza el problem+json a una forma interna que el mapeador consume; ninguna vista interpreta el error crudo.

## 3. Estado

Propuesto el 2026-06-15. El manejo de sesión y el detalle de presentación quedaron ratificables en el intake (§17 geovial-web P.3, P.11); este mapeo de errores es la propuesta del arquitecto, a ratificar en Sprint 0 junto con el catálogo de mensajes de 03. Si se ratifica, transiciona a Aceptado; si evoluciona, se crea una ADR nueva y esta pasa a `Superado por ADR-YY`.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Mapeador centralizado por código estable (elegido) | Un único punto de traducción; consistente; independiente del idioma; no filtra detalles del backend | Requiere mantener el mapa de códigos alineado con el catálogo de la API y de 03 |
| Manejo de error por vista | Mensajes a medida de cada pantalla | Duplica e inconsistencia; cada vista interpreta el problem+json crudo; difícil de mantener con once CU |
| Mostrar el mensaje del backend tal cual | Sin trabajo de mapeo | El mensaje del backend puede no estar en el idioma del usuario, puede filtrar detalles internos y no se alinea con la voz de UX de 03 |
| Mapear por estado HTTP en vez de por código estable | Simple | Pierde la granularidad: distintos códigos comparten estado HTTP (varios 400, varios 409) y exigen feedback distinto (validación vs. conflictos pendientes) |

## 5. Consecuencias positivas

1. El feedback es consistente en todo el front y alineado con el catálogo de mensajes de 03, con una sola fuente de traducción.
2. El mapeo por código estable es independiente del idioma y robusto ante traducciones de mensaje del backend, que son cambios compatibles del contrato (versionado de geovial-api).
3. Los errores específicos del dominio del front se presentan de forma adecuada: conflictos pendientes que bloquean el cierre (RN-05), acción fuera de alcance (RN-01, RN-03), validación con campos resaltados, sin filtrar detalles internos.
4. Las vistas quedan libres de interpretar el error crudo: reciben un feedback ya resuelto.

## 6. Consecuencias negativas y trade-offs

1. El mapa de códigos debe mantenerse alineado con el catálogo de errores de `geovial-api` (contratos-rest §5) y con los mensajes de 03. Se acepta y se gestiona como dependencia de versión del contrato; un código nuevo sin mapeo cae en el feedback genérico hasta agregarlo.
2. El feedback genérico para errores no contemplados puede ser poco específico. Se acepta para no filtrar detalles del backend; los códigos frecuentes se mapean explícitamente.
3. La normalización del problem+json en el Cliente de API agrega un paso. Se acepta porque confina el conocimiento del formato de error a un único punto (ADR-04).

## 7. Implementación

- El Cliente de API normaliza toda respuesta de error problem+json a una forma interna (código estable, estado, campos implicados) que el mapeador consume.
- El mapeador de errores (Aplicación de UI) traduce el código a un estado de pantalla y un mensaje del catálogo de 03; las vistas solo reciben el feedback resuelto.
- El control de habilitación por estado (ADR-04) evita de antemano ofrecer acciones inválidas (cierre con conflictos pendientes, RN-05); el mapeador cubre el caso en que el backend rechaza igualmente.
- Convención impuesta: ninguna vista muestra el mensaje crudo del backend ni el detalle interno de un error 500; el mapeo es por código estable, no por texto ni solo por estado HTTP.

## 8. Métricas de validación

- Cada código estable del catálogo de `geovial-api` relevante para el front tiene un mapeo a feedback definido, o cae explícitamente en el feedback genérico; verificado por prueba de componente en 08.
- Un error de validación con varios campos resalta los campos sin perder lo ingresado, verificado en 08 (alineado con CU-19 FA-01 de la API).
- El intento de cierre con conflictos pendientes presenta el feedback de bloqueo (RN-05) y no ejecuta el cierre, verificado en 08.
- Un error interno del backend se presenta como fallo genérico sin filtrar detalles, verificado en 08.

## 9. Referencias

- CU-01, CU-07, CU-08, CU-10, CU-11; CU-19 de la API; RN-01, RN-03, RN-04, RN-05.
- Intake §17 geovial-web P.3, P.11.
- `geovial-api`: ADR-05 (manejo de errores con problem+json RFC 7807), `contratos-rest_v1.0.md` §5; 03 catálogo de mensajes de UX.
- ADRs relacionadas: ADR-01 (estilo), ADR-03 (autenticación), ADR-04 (separación de capas).
- `arquitectura-solucion_v1.0.md` §7.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión de manejo de errores: mapeador centralizado que traduce el problem+json de geovial-api a feedback de UI por código estable, alineado con el catálogo de 03, sin filtrar detalles del backend. Propuesta (detalle de presentación ratificable en intake §17.P.3). |
