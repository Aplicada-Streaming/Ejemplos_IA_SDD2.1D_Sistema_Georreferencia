# CU-20 — Paginar y filtrar los listados de recursos

**Proyecto:** geovial-api
**Documento:** CU-20-paginar-filtrar-listados_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Garantizar de forma transversal que todo listado de recursos del backend —relevamientos, marcadores, observaciones, usuarios— se entregue paginado y admita filtros y un orden previsibles, para que los clientes consuman volúmenes grandes sin sobrecargar la red ni la respuesta. Unifica el contrato de listado de la superficie REST.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Cliente consumidor | Primario | Solicita un listado con paginación, filtros y orden |
| Backend de la API | Sistema | Aplica el alcance, los filtros, el orden y la paginación al listado |

## 3. Precondiciones

- El solicitante está autorizado para listar el recurso dentro de su alcance (CU-18).
- El recurso a listar declara los filtros y los campos de orden admitidos.

## 4. Flujo principal

1. El cliente solicita un listado indicando, opcionalmente, filtros, campo de orden, tamaño de página y posición de la página.
2. El backend acota el conjunto al alcance del solicitante (RN-01) y aplica los filtros indicados.
3. El backend ordena el conjunto por el campo solicitado o por un orden por defecto estable.
4. El backend devuelve la página solicitada con sus elementos, el tamaño de página efectivo y la referencia para obtener la página siguiente y la anterior.
5. El cliente recorre las páginas hasta agotar el conjunto.

## 5. Flujos alternativos

- 5.A Tamaño de página fuera de rango. Disparador: el cliente pide un tamaño de página mayor al máximo admitido. El backend acota el tamaño al máximo permitido y lo informa en la respuesta, sin rechazar la solicitud. Retorna al paso 4.
- 5.B Filtro combinado. Disparador: el cliente combina varios filtros (por ejemplo, estado y etiqueta). El backend aplica la conjunción de todos los filtros y devuelve solo los elementos que los cumplen todos. Retorna al paso 4.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| FILTRO_NO_SOPORTADO | El cliente indica un filtro que el recurso no admite | Rechaza con estado de solicitud inválida e informa los filtros válidos |
| ORDEN_NO_SOPORTADO | El campo de orden solicitado no está admitido | Rechaza con estado de solicitud inválida e informa los campos de orden válidos |
| POSICION_INVALIDA | La posición de página solicitada no es válida | Rechaza con estado de solicitud inválida |

## 7. Postcondiciones

- Éxito: el cliente recibe una página acotada a su alcance, con los filtros y el orden aplicados, y las referencias a las páginas contiguas.
- Garantía: ningún listado entrega el conjunto completo sin paginar; el contrato de paginación es uniforme entre recursos.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un jefe con 30 relevamientos | Solicita la primera página con tamaño 10 | El backend devuelve 10 relevamientos y la referencia a la página siguiente |
| CA-02 | Un listado de relevamientos con estados mixtos | El cliente filtra por estado de revisión y etiqueta fisura | El backend devuelve solo los relevamientos en revisión que portan la etiqueta fisura |
| CA-03 | Un cliente que pide un tamaño de página por encima del máximo | El cliente envía la solicitud | El backend acota el tamaño al máximo permitido y lo informa en la respuesta |
| CA-04 | Un cliente que indica un filtro inexistente para el recurso | El cliente envía la solicitud | El backend rechaza con el código FILTRO_NO_SOPORTADO e informa los filtros válidos |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-02, NB-03, NB-05 |
| Reglas de negocio aplicables | RN-01 |
| Historias de usuario a generar | US-40, US-41 (en 06) |
| Componentes esperados | Servicio transversal de paginación y filtro; descriptor de filtros y orden por recurso (referencia tentativa a 05) |
| Tests previstos | Paginación con referencia a página siguiente; filtro combinado; tamaño acotado al máximo; filtro no soportado rechazado (en 08) |

## 10. Notas y supuestos

- Este CU transversal aplica a los listados de los CU funcionales que devuelven colecciones (por ejemplo CU-04, CU-12) y por eso esos CU lo referencian en vez de repetir la mecánica.
- Los filtros y campos de orden admitidos por cada recurso se declaran en el contrato; su detalle exacto pertenece a la categoría 05.
- El alcance jerárquico se aplica siempre antes de la paginación: nunca se paginan recursos fuera del ámbito del solicitante (RN-01).

## 12. Performance esperado del CU

- La entrega de una página debe mantenerse dentro del objetivo de lectura del proyecto (p95 menor o igual a 300 ms) con tamaños de página razonables, con independencia del tamaño total del conjunto.

## 15. Idempotencia y reintento

- El listado es una operación segura y repetible: solicitar la misma página con los mismos filtros y orden devuelve el mismo resultado, sin efectos colaterales.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU transversal de paginación y filtros, derivado de la naturaleza rest-api del proyecto (02 §2.2). |
