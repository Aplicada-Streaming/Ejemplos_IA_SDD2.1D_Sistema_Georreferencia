# ADR-04 — Separación de capas en el cliente: Presentación / Aplicación de UI / Cliente de API

**Proyecto:** geovial-web
**Documento:** ADR-04-separacion-capas-presentacion-aplicacion-cliente-api_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Arquitecto Senior
**Categoría:** Estilo

## 1. Contexto

El estilo del front (ADR-01) es render server-side con circuito interactivo persistente. Dentro de ese estilo hay que decidir cómo se estructura internamente el cliente para que la lógica de interacción, el consumo del contrato REST de `geovial-api` y la presentación no se mezclen, de modo que se pueda probar la lógica sin levantar la red y acotar el acoplamiento a la API a un único punto. El front cubre once CU con vistas heterogéneas (mapa, carrusel, formularios, listados, modales de resolución y cierre) y servicios transversales (sesión y token, visibilidad por rol, habilitación por estado, mapeo de errores) que toda vista invoca. El intake deja el detalle interno como propuesto y ratificable (§17 geovial-web P.2: "Detalle interno: PENDIENTE"), por lo que esta decisión es del arquitecto y se declara `Propuesto`. La regla 05 §1.2 indica para `web-monolith` separación de capas con patrón de presentación. Motivan esta decisión los once CU del front y las cinco RN de presentación, y el atributo de testabilidad de §17.P.6.

## 2. Decisión

Se decide estructurar el cliente en tres capas de dependencia unidireccional hacia el núcleo de la aplicación de UI:

- Presentación: vistas y componentes de interfaz, incluido el componente de mapa y el carrusel. No contiene lógica de dominio ni acceso a la red; delega en la Aplicación de UI.
- Aplicación de UI: orquestadores de interacción de cada CU, servicio de sesión y token, control de visibilidad por rol, control de habilitación por estado del relevamiento y mapeador de errores. Define un puerto de acceso al dominio que la capa inferior implementa.
- Cliente de API: adaptador que implementa el puerto de acceso al dominio traduciendo intenciones en llamadas al contrato REST de `geovial-api` y normalizando respuestas y errores. Es el único punto que conoce el contrato y la red.

La Presentación depende de la Aplicación de UI; la Aplicación de UI depende del puerto que el Cliente de API implementa; ninguna vista accede a la red directamente. El consumo del contrato de `geovial-api` queda confinado al Cliente de API.

## 3. Estado

Propuesto el 2026-06-15. El detalle interno del front quedó como ratificable en el intake (§17 geovial-web P.2); esta separación de capas es la propuesta del arquitecto, a ratificar en Sprint 0. Si se ratifica, transiciona a Aceptado; si la decisión evoluciona, se crea una ADR nueva y esta pasa a `Superado por ADR-YY`.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Tres capas: Presentación / Aplicación de UI / Cliente de API (elegido) | Aísla el consumo del contrato en un punto; permite probar la lógica sin red; ordena los servicios transversales | Más componentes que un front plano; requiere disciplina de dependencia |
| Front plano (lógica y acceso a la API en las vistas) | Menos piezas; rápido al inicio | Impide testear sin red; dispersa el manejo del token y del contrato; difícil de mantener con once CU |
| Dos capas (Presentación + acceso a datos) sin capa de Aplicación de UI | Más simple que tres | Los servicios transversales (sesión, visibilidad, habilitación, errores) quedarían en las vistas o en el acceso a datos, mezclando responsabilidades |
| Patrón de presentación por vista sin puerto de dominio compartido | Encapsula cada pantalla | Duplica el consumo del contrato por vista; no aísla el acoplamiento a la API en un único punto |

## 5. Consecuencias positivas

1. El acoplamiento al contrato REST de `geovial-api` queda en un único componente (Cliente de API), lo que acota el impacto de un cambio del contrato y materializa la mitigación del riesgo de acoplamiento (arquitectura §9).
2. La Aplicación de UI y el Cliente de API se prueban con dobles del puerto sin levantar la red, habilitando el gate de cobertura de §17.P.6 (líneas ≥ 80 %, branches ≥ 70 %, presentación ≥ 60 %).
3. Los servicios transversales (sesión y token, visibilidad por rol, habilitación por estado, mapeo de errores) viven en un único lugar y se invocan de forma consistente desde toda la superficie.
4. La presentación queda libre de lógica de dominio, facilitando los wireframes y la evolución de la UI sin tocar el consumo del contrato.

## 6. Consecuencias negativas y trade-offs

1. La separación introduce más componentes y un puerto de indirección. Se acepta por la testabilidad y el aislamiento del acoplamiento; el costo es bajo para once CU.
2. Exige disciplina para que ninguna vista llame a la red directamente. Se mitiga con la convención impuesta y con la revisión en 06/08.
3. El puerto de acceso al dominio debe evolucionar con el contrato de la API; un cambio del contrato puede requerir ajustar el puerto. Se acepta porque el ajuste queda confinado al Cliente de API.

## 7. Implementación

- Cada CU tiene un orquestador de interacción en la Aplicación de UI que valida la entrada de pantalla y delega en el puerto de acceso al dominio.
- El Cliente de API implementa el puerto contra el contrato REST de `geovial-api`, aplicando la paginación y los filtros que el contrato expone (CU-20 de la API) para los listados (CU-02, CU-03, CU-06).
- Los servicios transversales se inyectan en los orquestadores; ninguna vista los reimplementa.
- Convención impuesta: la dependencia apunta siempre hacia adentro (Presentación → Aplicación de UI → puerto ← Cliente de API); ninguna vista importa el Cliente de API ni conoce la dirección de red.

## 8. Métricas de validación

- El consumo del contrato REST está confinado al Cliente de API: ninguna vista ni orquestador llama a la red directamente, verificado por inspección de la composición y prueba de arquitectura en 08.
- Cobertura de pruebas: líneas ≥ 80 %, branches ≥ 70 %, presentación ≥ 60 %, alcanzada probando la Aplicación de UI y el Cliente de API con dobles del puerto (intake §17.P.6, 08).
- La latencia de interacción p95 ≤ 200 ms se sostiene con la indirección de capas, verificada en 08 (NFR §8).

## 9. Referencias

- CU-01 a CU-11; RN-01 a RN-05.
- Intake §17 geovial-web P.2 (detalle interno ratificable), P.6 (cobertura).
- Regla 05 §1.2 (`web-monolith`: separación de capas con patrón de presentación).
- ADRs relacionadas: ADR-01 (estilo), ADR-03 (autenticación), ADR-05 (manejo de errores).
- `arquitectura-solucion_v1.0.md` §3, §9.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión de separación de capas en el cliente: Presentación / Aplicación de UI / Cliente de API, con dependencia unidireccional hacia el núcleo y el consumo del contrato confinado al Cliente de API. Propuesta (detalle interno ratificable en intake §17.P.2). |
