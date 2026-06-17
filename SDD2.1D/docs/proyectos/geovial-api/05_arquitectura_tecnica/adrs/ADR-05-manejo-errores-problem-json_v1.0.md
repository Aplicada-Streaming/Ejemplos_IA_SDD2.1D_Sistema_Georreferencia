# ADR-05 — Manejo de errores con problem+json RFC 7807

**Proyecto:** geovial-api
**Documento:** ADR-05-manejo-errores-problem-json_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer
**Categoría:** Comunicación

## 1. Contexto

Todo error del backend debe expresarse con una estructura uniforme para que el front web y la app móvil traten los fallos de manera consistente (CU-19). Los CU funcionales declaran códigos de error estables (por ejemplo TRAMO_INCOMPLETO, CONFLICTOS_PENDIENTES, SUBIDA_NO_CONCLUIDA, RELEVAMIENTO_CERRADO, JERARQUIA_NO_PERMITIDA), y el contrato de error no debe variar entre recursos. Los códigos son opacos al idioma: el cliente decide por el código, no por el texto. Un error interno no previsto no debe filtrar detalles sensibles. La librería de almacenamiento ya expone una taxonomía de error uniforme que el backend integra (contrato de `geovial-storage`). Cubre CU-19 y uniformiza las RN de todos los CU.

## 2. Decisión

Se adopta problem+json RFC 7807 como formato único de error de toda la superficie REST. Cada respuesta de error transporta un código estable (en mayúsculas, sin tildes, independiente del idioma), un mensaje legible, el estado HTTP acorde a la naturaleza del fallo (solicitud inválida, no autorizado, prohibido, no encontrado, conflicto o error interno) y, cuando aporta, el campo o recurso implicado; los errores de validación con varios campos se devuelven en un único problema que enumera cada campo. Un fallo no contemplado devuelve un código genérico de error interno sin exponer detalles. El catálogo de códigos estables se centraliza y se alinea con `dx-error-messages_v1.0.md` (03).

## 3. Estado

Aceptado el 2026-06-15. Derivado del CU transversal CU-19 y de la naturaleza `rest-api` (regla 05 §1.2, 02 §2.2).

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| problem+json RFC 7807 con códigos estables (elegido) | Formato estándar para REST; contrato de error homogéneo; código opaco al idioma; un problema enumera múltiples campos | Requiere disciplina para mantener el catálogo de códigos estable |
| Errores ad-hoc por endpoint (forma libre) | Sin ceremonia de catálogo | Cada recurso devolvería una forma distinta; los clientes tratarían los fallos de manera inconsistente; descartado por CU-19 |
| Solo código de estado HTTP sin cuerpo estructurado | Mínimo | Insuficiente para distinguir causas dentro de un mismo estado (varias causas comparten 400/409); no transporta el código estable de dominio |
| Códigos numéricos propietarios | Compactos | Menos legibles; no estandarizados; obligarían a una tabla de traducción fuera del contrato |

## 5. Consecuencias positivas

1. El formato de error no varía entre recursos; los clientes lo tratan de manera homogénea (CU-19, garantía).
2. El código estable permite al cliente decidir su tratamiento sin depender del texto del mensaje (opacidad al idioma).
3. Un único problema enumera todos los campos inválidos de una solicitud, evitando múltiples respuestas (CU-19, FA-01).
4. El error interno no previsto se devuelve con un código genérico sin filtrar detalles sensibles (CU-19, FA-02).

## 6. Consecuencias negativas y trade-offs

1. Mantener el catálogo de códigos estable y sin colisiones exige disciplina y revisión en cada cambio del contrato.
2. El mapeo de cada fallo de dominio a un estado HTTP y a un código debe ser consistente; una divergencia sería un defecto (mitigado por contract tests).
3. La traducción del mensaje legible se gestiona aparte del código; el código nunca cambia al traducir.

## 7. Implementación

- El manejador de errores transversal (middleware de la capa de API) intercepta todo fallo y construye la representación problem+json con código, mensaje, estado y contexto.
- Los códigos estables de los CU funcionales (CU-01 a CU-17) y de los transversales se consolidan en el catálogo de `contratos-rest_v1.0.md`, alineado con `dx-error-messages_v1.0.md` (03).
- Los códigos de la librería de almacenamiento se normalizan al cruzar la frontera hacia el contrato REST del backend, manteniendo la uniformidad.
- Convención impuesta: ningún endpoint devuelve un error fuera del formato problem+json; ningún error interno expone detalles sensibles.

## 8. Métricas de validación

- 100 % de los endpoints públicos cubiertos por un contract test que verifica el formato de error (intake §17.P.6).
- Error de validación uniforme; múltiples campos en un solo problema; recurso inexistente con RECURSO_NO_ENCONTRADO; error interno con ERROR_INTERNO sin filtración (CU-19, verificado en 08).
- Cero respuestas de error fuera del formato problem+json en la batería de contrato.

## 9. Referencias

- NB-01 a NB-05; CU-19 (y los códigos de CU-01 a CU-17, CU-18, CU-20, CU-21, CU-22); uniformiza RN-01 a RN-07.
- Intake §17.P.3 (comunicación); 02 §2.2 (CU transversal de errores).
- ADRs relacionadas: ADR-03 (autorización), ADR-04 (paginación), ADR-08 (idempotencia), ADR-10 (versionado).
- `contratos-rest_v1.0.md` §5; `dx-error-messages_v1.0.md` (03); `arquitectura-solucion_v1.0.md` §7.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión de comunicación: problem+json RFC 7807 como formato único de error con códigos estables opacos al idioma. Aceptada (derivada de CU-19). |
