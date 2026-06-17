# Criterios de validación — geovial-storage

**Proyecto:** geovial-storage
**Documento:** criterios-validacion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero QA / SDET Senior (variante QA + SDET Library)

## 1. Propósito

Define qué significa "sistema validado para release" en `geovial-storage`. La librería se valida cuando su contrato público se comporta de forma idéntica entre proveedores (RN-01), garantiza integridad binaria (RN-02), no filtra credenciales (RN-03), cumple sus NFR numéricas y alcanza la cobertura por capa y global comprometida. Los criterios son numéricos y verificables mecánicamente; un release no se declara válido si algún criterio no se cumple sin una excepción documentada (§6).

## 2. Criterios funcionales

| ID | Criterio | Verificación | Umbral |
| --- | --- | --- | --- |
| CV-F1 | Cada CU crítico (CU-01, CU-02, CU-03, CU-04, CU-06) tiene todos sus criterios Given-When-Then cubiertos por un TC en verde | Reporte de la suite y matriz de cobertura | 100 % de los CA críticos en verde |
| CV-F2 | CU-05 (listar) cubierto en sus criterios principales y de paginación | TC-13, TC-14, TC-15 en verde | 100 % de los CA en verde |
| CV-F3 | Los flujos alternativos verificables (sobrescritura, idempotencia, rango, solo-metadatos, eliminación múltiple, validación-en-seco) tienen TC en verde | TC-04, TC-07, TC-10, TC-11, TC-20 | Todos en verde |

CU-05 se considera no bloqueante por sí solo del MVP (sus US asociadas US-07 es Should), pero su batería se exige verde para el release porque es parte de la superficie pública estable.

## 3. Criterios no funcionales

Cada NFR se mide en un ambiente de pruebas equivalente al productivo (para NFR-01) o por gate de CI (resto).

| ID | NFR | Criterio numérico | Test |
| --- | --- | --- | --- |
| CV-N1 | NFR-01 latencia | p95 ≤ 1 s para archivos ≤ 5 MB con proveedor local | TC-25 |
| CV-N2 | NFR-02 tamaño máximo | El límite por defecto de 25 MB se respeta: el contenido en el límite se persiste y el que excede se rechaza con TAMANIO_EXCEDIDO antes de delegar | TC-26 |
| CV-N3 | NFR-03 transparencia | 0 diferencias de comportamiento observable y 0 ramas por proveedor en el consumidor; la batería única pasa contra cada proveedor | TC-21 |
| CV-N4 | NFR-04 integridad | 100 % de igualdad binaria byte a byte en la propiedad y en los TC de ida y vuelta | TC-05, TC-23 |
| CV-N5 | NFR-05 no filtración | 0 ocurrencias de credenciales o parámetros de conexión en resultados, errores y registros | TC-08, TC-15, TC-24 |
| CV-N6 | NFR-06 cobertura | Líneas ≥ 80 % y branches ≥ 70 % sobre la suite completa | Gate G-03 |

## 4. Criterios de regresión

| ID | Criterio | Verificación |
| --- | --- | --- |
| CV-R1 | La suite de regresión completa se ejecuta y está en verde | Reporte de CI de la suite completa |
| CV-R2 | Ningún test verde de la versión anterior pasó a rojo sin justificación documentada | Comparación de reportes entre versiones |
| CV-R3 | Cada bug cerrado generó al menos un TC nuevo o extendió uno existente | Vínculo bug↔TC en el seguimiento |
| CV-R4 | El snapshot del contrato y del catálogo de errores no cambió sin PR aprobado | TC-22; revisión de PR |

## 5. Criterios de calidad de código

| ID | Criterio | Verificación | Umbral |
| --- | --- | --- | --- |
| CV-C1 | Cobertura por capa cumplida | Medidor de cobertura segmentado | Dominio ≥ 85 % líneas / 80 % branches; infraestructura ≥ 70 % líneas / 60 % branches |
| CV-C2 | Mutation score de dominio cumplido | Framework de mutation testing | ≥ 60 % en dominio |
| CV-C3 | Cobertura global cumplida (gate de intake §17 P.6) | Medidor de cobertura agregado | ≥ 80 % líneas / ≥ 70 % branches |
| CV-C4 | Análisis estático sin issues críticos ni warnings nuevos | Analizador estático | 0 issues críticos |
| CV-C5 | Compilación sin warnings tratados como error | Compilador | Sin warnings-as-errors |

Los criterios CV-C1 (por capa) y CV-C3 (global) son compatibles y se evalúan en conjunto: el global es el piso agregado y el por capa es más estricto en dominio (ver `matriz-cobertura-pruebas_v1.0.md` §5).

## 6. Excepciones documentadas

Cualquier criterio no cumplido se acepta solo con una ADR explícita y un plan de remediación con BT en el backlog (regla 08 §4.7). Excepciones admitidas conocidas en v1.0:

- El contract test contra el proveedor remoto (parte de TC-21) puede diferirse del MVP al Tramo 5 porque el adaptador remoto es Should (BT-09); la transparencia se valida con el proveedor local y el doble en memoria hasta entonces (GAP-02).
- La medición de NFR-01 en ambiente equivalente al productivo se ratifica antes del release; hasta tener ese ambiente (definido por 09) se mide en CI como aproximación (GAP-03).
- El almacenamiento físico seguro de credenciales en reposo está delegado a 09 (ADR-05); en 08 se valida solo la no filtración por la superficie pública (GAP-04).

Ninguna excepción puede afectar a las invariantes RN-01, RN-02 ni RN-03 sobre el proveedor local y el doble en memoria: esas deben estar en verde para declarar el MVP validado.

## 7. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| CU | CU-01 a CU-06 (02) |
| RN | RN-01, RN-02, RN-03 (02) |
| NFR | NFR-01 a NFR-06 (05) |
| Gate global | intake §17 P.6 |
| Tests | `casos-prueba-referenciales_v1.0.md`; gates en `estrategia-calidad_v1.0.md` §3 |

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Criterios de validación iniciales de geovial-storage: criterios funcionales por CU, no funcionales por NFR numérica, de regresión, de calidad de código (cobertura por capa, mutation, global, análisis estático) y excepciones documentadas con plan de remediación. |
