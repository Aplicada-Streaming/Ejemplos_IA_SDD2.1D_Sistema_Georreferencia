# ADR-04 — Transparencia del proveedor e integridad del contenido

**Proyecto:** geovial-storage
**Documento:** ADR-04-transparencia-limites-proveedor_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer
**Categoría:** Estilo

## 1. Contexto

La transparencia es el invariante central de la librería (RN-01) y el criterio de éxito de NB-07 (cero cambios de comportamiento percibidos al cambiar el destino). A ella se suma la integridad del contenido (RN-02): lo recuperado debe ser idénticamente igual, byte a byte, a lo guardado, porque las fotografías son la evidencia del relevamiento. El estilo ya está decidido (ADR-01) y la superficie pública también (ADR-02); falta decidir el mecanismo que hace observable y verificable la transparencia y la integridad a través de proveedores con capacidades dispares (un destino local frente a un servicio de objetos remoto), y cómo se tratan los límites comunes —en particular el tamaño máximo de archivo y la semántica que se garantiza— para que sean idénticos en todos los proveedores. Los NFR numéricos provienen del intake §17.P.10: latencia p95 ≤ 1 s para archivos de hasta 5 MB con el proveedor local, tamaño máximo configurable con valor por defecto 25 MB, y sin degradación apreciable al cambiar de proveedor.

## 2. Decisión

Se decide que el núcleo de enrutado normalice toda diferencia de los proveedores a un comportamiento observable único: el mismo conjunto de resultados y de códigos de error para las mismas entradas, cualquiera sea el proveedor activo. La integridad byte a byte (RN-02) se preserva no transformando ni recodificando el contenido en ningún punto del enrutado ni en los adaptadores. El tamaño máximo de archivo es un parámetro de configuración común a todos los proveedores, con valor por defecto 25 MB, validado por el núcleo antes de delegar (error TAMANIO_EXCEDIDO sin contactar al proveedor). El contrato fija qué se garantiza (cardinalidad y pertenencia del listado, idempotencia del borrado de inexistentes, igualdad binaria) y qué no se garantiza (orden del listado), de modo que la equivalencia entre proveedores sea verificable. La conformidad se valida con una batería de pruebas de contrato única ejecutada contra cada proveedor soportado.

## 3. Estado

Aceptado el 2026-06-15. La transparencia y los NFR numéricos están fijados en el intake (§17.P.10, §17.P.11) y en RN-01/RN-02.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Normalización en el núcleo + batería de contrato única + tamaño máximo común validado en el núcleo (elegida) | Hace observable y automática la verificación de RN-01; el límite de tamaño es idéntico entre proveedores; integridad por no transformación | Exige mantener la batería de contrato y mapear cada error de adaptador a un código uniforme |
| Dejar que cada adaptador exponga su comportamiento y documentar las diferencias | Menos código de normalización | Viola RN-01; el consumidor necesitaría ramas por proveedor; el límite de tamaño variaría por proveedor |
| Validar el tamaño en cada adaptador en vez del núcleo | El adaptador conoce el límite real de su destino | El límite percibido dependería del proveedor (rompe RN-01); el contenido viajaría al adaptador antes de rechazarse |
| Garantizar también el orden del listado | Contrato más fuerte | Algunos destinos no garantizan orden sin costo; encarecería la operación; la especificación (CU-05) ya declara el orden no garantizado |

## 5. Consecuencias positivas

1. El consumidor observa un comportamiento único; no escribe ramas por proveedor (RN-01).
2. El límite de tamaño se aplica de forma idéntica y temprana, rechazando antes de transferir contenido al proveedor.
3. La integridad byte a byte queda protegida por la ausencia de transformación, verificable con ida y vuelta.
4. La batería de contrato única convierte la transparencia en una propiedad probada en CI, no en una promesa.

## 6. Consecuencias negativas y trade-offs

1. La normalización de errores agrega trabajo de mapeo en cada adaptador nuevo; se acepta porque es la condición de la transparencia.
2. El objetivo de latencia p95 numérico se fija solo para el proveedor local; para el remoto se mide y se acepta la dependencia de la red (no se promete un número que el destino remoto no controla).
3. No garantizar el orden del listado obliga al consumidor a no depender del orden; trade-off ya asumido en CU-05.
4. El tamaño máximo por defecto (25 MB) es un valor configurable propuesto y ratificable (intake §17.P.10): un cambio del valor no es un cambio de contrato, pero debe coordinarse operativamente.

## 7. Implementación

- El núcleo valida el tamaño contra el máximo configurado y rechaza con TAMANIO_EXCEDIDO antes de delegar (CU-01).
- Ningún componente transforma el binario; el tipo de contenido se registra como metadato sin recodificar (RN-02; CU-01 nota).
- El núcleo mapea los fallos de cada adaptador a los códigos uniformes (PROVEEDOR_NO_DISPONIBLE, etc.) sin filtrar detalles del proveedor.
- La recuperación por rango y el listado por paginación se implementan de forma uniforme para no materializar contenidos ni listados completos, sosteniendo el objetivo de latencia.
- La batería de contrato se ejecuta contra cada proveedor soportado en CI y manualmente antes de cada liberación (coherente con la verificación previa a publicar del quick-start, 03).

## 8. Métricas de validación

- Latencia de guardar/recuperar p95 ≤ 1 s para archivos de hasta 5 MB con el proveedor local (prueba de rendimiento en 08).
- 0 diferencias de comportamiento observable y 0 ramas por proveedor en el consumidor (batería de contrato en 08).
- 100 % de igualdad binaria guardar-recuperar, incluida la verificación del segmento en la recuperación por rango (RN-02, 08).
- TAMANIO_EXCEDIDO se dispara por encima del máximo configurado y un contenido en el límite se persiste; el valor por defecto verificable es 25 MB.

## 9. Referencias

- NB-07 (criterio de éxito: cero cambios percibidos); RN-01 (transparencia), RN-02 (integridad).
- CU-01 (tamaño, integridad), CU-02 (recuperación por rango, igualdad binaria), CU-05 (orden no garantizado).
- Intake §17.P.10 (latencia p95, tamaño máximo, transparencia), §17.P.11.
- Catálogo de errores: `dx-error-messages_v1.0.md` (03), `contratos-abstractions_v1.0.md` §5.
- ADRs relacionadas: ADR-01 (estilo), ADR-02 (superficie pública).

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión de transparencia e integridad: normalización en el núcleo, batería de contrato única por proveedor, tamaño máximo común (por defecto 25 MB) validado tempranamente y garantías de contrato explícitas. Incorpora los NFR numéricos del intake §17.P.10. Aceptada. |
