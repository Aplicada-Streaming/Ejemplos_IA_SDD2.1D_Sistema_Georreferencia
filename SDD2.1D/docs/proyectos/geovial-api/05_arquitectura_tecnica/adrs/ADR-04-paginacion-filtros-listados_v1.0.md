# ADR-04 — Estrategia de paginación y filtros de listados

**Proyecto:** geovial-api
**Documento:** ADR-04-paginacion-filtros-listados_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer
**Categoría:** Comunicación

## 1. Contexto

Los listados de recursos del backend —relevamientos, marcadores, observaciones, usuarios— pueden crecer en volumen y no deben entregarse completos sin paginar (CU-20). El alcance jerárquico debe aplicarse siempre antes de paginar: nunca se paginan recursos fuera del ámbito del solicitante (RN-01). Cada recurso declara qué filtros y campos de orden admite. La entrega de una página debe mantenerse dentro del objetivo de lectura del proyecto (p95 ≤ 300 ms) con independencia del tamaño total del conjunto (intake §17.P.10). Cubre CU-20 y aplica a los CU funcionales que devuelven colecciones (CU-04, CU-12).

## 2. Decisión

Se adopta una estrategia de paginación uniforme para toda la superficie REST: cada listado acepta tamaño de página y posición de página, aplica los filtros declarados por el recurso en conjunción, ordena por el campo solicitado o por un orden por defecto estable, y devuelve la página con su tamaño efectivo y las referencias para navegar a la página siguiente y a la anterior. El tamaño de página tiene un máximo; un pedido por encima del máximo se acota y se informa, sin rechazar. El alcance jerárquico (RN-01) se aplica antes de filtrar y paginar. Los filtros y campos de orden no admitidos se rechazan con un problem+json (FILTRO_NO_SOPORTADO, ORDEN_NO_SOPORTADO, POSICION_INVALIDA).

## 3. Estado

Aceptado el 2026-06-15. Derivado del CU transversal CU-20 y de la naturaleza `rest-api` (regla 05 §1.2, 02 §2.2).

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Paginación uniforme por página con filtros declarados (elegida) | Contrato homogéneo entre recursos; navegación previsible; alcance antes de paginar (RN-01) | Una página profunda puede ser costosa si no hay índice de orden |
| Sin paginación (listado completo) | Implementación trivial | Sobrecarga la red y la respuesta; rompe el objetivo de latencia p95; descartada por CU-20 |
| Paginación por cursor opaco únicamente | Estable ante inserciones; eficiente en profundidad | Mayor complejidad de contrato; menos intuitiva para listados acotados por alcance; se reserva como evolución compatible si el volumen lo exige |
| Filtros libres no declarados | Flexibilidad para el cliente | Imposible garantizar índice ni previsibilidad; abre la puerta a consultas costosas; rompe la uniformidad |

## 5. Consecuencias positivas

1. Ningún listado entrega el conjunto completo sin paginar; el contrato de paginación es uniforme entre recursos (CU-20, postcondición).
2. El alcance jerárquico se aplica antes de paginar, de modo que nunca se exponen recursos fuera del ámbito (RN-01).
3. El máximo de página acota el costo por respuesta y sostiene el objetivo de latencia p95 de lecturas.
4. Los filtros y campos de orden declarados por recurso se respaldan con índices del modelo lógico, manteniendo la consulta previsible.

## 6. Consecuencias negativas y trade-offs

1. La paginación por posición puede ser costosa en páginas profundas; se acepta para los volúmenes esperados y se reserva la paginación por cursor como evolución compatible.
2. Declarar filtros y orden por recurso agrega un descriptor por recurso; se acepta a cambio de previsibilidad y de poder indexar.
3. Acotar el tamaño en vez de rechazar puede sorprender al cliente que pidió más; se mitiga informando el tamaño efectivo en la respuesta.

## 7. Implementación

- El servicio de paginación y filtros (capa de Aplicación) recibe los parámetros de listado, aplica el alcance (RN-01), los filtros en conjunción, el orden y la paginación, y arma la página con referencias de navegación.
- Cada recurso declara su descriptor de filtros y campos de orden admitidos; el contrato exacto vive en `contratos-rest_v1.0.md`.
- Los campos de orden y filtro frecuentes se respaldan con índices en `modelo-datos-logico_v1.0.md`.
- Convención impuesta: todo endpoint de listado pagina; ninguno devuelve la colección completa.

## 8. Métricas de validación

- Latencia p95 de listados ≤ 300 ms con tamaños de página razonables, con independencia del tamaño total del conjunto (intake §17.P.10).
- Página con referencia a la página siguiente; filtro combinado en conjunción; tamaño acotado al máximo informado; filtro no soportado rechazado (CU-20, verificado en 08).
- Listados acotados al alcance del solicitante antes de paginar (RN-01).

## 9. Referencias

- NB-02, NB-03, NB-05; CU-20 (y CU-04, CU-12 que lo consumen); RN-01.
- Intake §17.P.10 (NFR de latencia).
- ADRs relacionadas: ADR-03 (autorización/alcance), ADR-05 (errores), ADR-10 (versionado).
- `contratos-rest_v1.0.md`; `arquitectura-solucion_v1.0.md` §3, §7.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión de comunicación: paginación uniforme por página con filtros y orden declarados por recurso, alcance jerárquico previo a la paginación y máximo de página acotado. Aceptada (derivada de CU-20). |
