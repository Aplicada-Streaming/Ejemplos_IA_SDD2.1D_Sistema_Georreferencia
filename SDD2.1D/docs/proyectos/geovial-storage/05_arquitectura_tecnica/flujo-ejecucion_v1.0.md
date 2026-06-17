# Flujo de ejecución — geovial-storage

**Proyecto:** geovial-storage
**Documento:** flujo-ejecucion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer

## 1. Objetivo

Describir el enrutado paso a paso de una operación de la superficie pública hacia el proveedor activo: la cadena validar → resolver proveedor activo → delegar → normalizar resultado/error. Es el mecanismo común a guardar, recuperar y eliminar (y también verificar y listar), y la pieza que materializa la transparencia (RN-01) y la normalización de errores (ADR-04). Se incluye porque clarifica cómo una sola implementación del núcleo sirve a todos los proveedores sin ramas por proveedor en el consumidor.

## 2. Pipeline común de una operación de datos (CU-01 a CU-05)

Pasos del enrutado, con la transformación de datos en cada etapa:

1. Recepción. El consumidor invoca la operación de la superficie pública con sus entradas (contenido y destino para guardar; identificador para recuperar/eliminar/verificar; prefijo para listar).
2. Validación de entrada. El núcleo valida sin contactar al proveedor: contenido no vacío (CONTENIDO_VACIO), formato de destino o identificador (DESTINO_INVALIDO), rango (RANGO_INVALIDO), testigo de continuación (TESTIGO_INVALIDO) y tamaño máximo configurado (TAMANIO_EXCEDIDO). Si falla, se devuelve el error de entrada inválida y el pipeline termina aquí.
3. Resolución del proveedor activo. El núcleo consulta al registro de proveedores el proveedor activo. Si no hay proveedor configurado, devuelve PROVEEDOR_NO_CONFIGURADO. La resolución entrega el adaptador que implementa el puerto.
4. Delegación. El núcleo invoca la operación correspondiente del puerto sobre el adaptador resuelto, pasando la entrada ya validada. El contenido se transfiere sin transformarse (RN-02); la recuperación por rango y el listado paginado transfieren solo el segmento o la página pedida.
5. Normalización del resultado. El adaptador devuelve el resultado del destino físico (identificador y tamaño al guardar; contenido y metadatos al recuperar; presencia al verificar; lista y testigo al listar; confirmación o cantidad al eliminar). El núcleo lo entrega al consumidor en la forma uniforme del contrato.
6. Normalización del error. Si el adaptador falla, el núcleo mapea el fallo a un código uniforme (PROVEEDOR_NO_DISPONIBLE u otro de la categoría correspondiente) sin filtrar credenciales ni parámetros de conexión (RN-03), y lo propaga.

Resultado: el consumidor observa el mismo comportamiento y el mismo conjunto de códigos de error cualquiera sea el proveedor activo (RN-01).

## 3. Enrutado de guardar, recuperar y eliminar

| Etapa | Guardar (CU-01) | Recuperar (CU-02) | Eliminar (CU-03) |
| --- | --- | --- | --- |
| Validación | Contenido no vacío, destino, tamaño máximo | Identificador, rango | Identificador o prefijo |
| Resolución | Proveedor activo desde el registro | Proveedor activo | Proveedor activo |
| Delegación | Persistir contenido sin transformar | Leer contenido (completo, por rango o solo metadatos) | Borrar (uno o bajo prefijo) |
| Resultado | Identificador lógico + tamaño persistido | Contenido idéntico + metadatos | Confirmación o cantidad eliminada |
| Garantía propia | Sin archivo parcial ante fallo; sobrescritura controlada | Igualdad binaria (RN-02) | Idempotencia sobre inexistente; informe de eliminación parcial |
| Errores propios | IDENTIFICADOR_DUPLICADO, TAMANIO_EXCEDIDO | IDENTIFICADOR_INEXISTENTE, RANGO_INVALIDO | ELIMINACION_PARCIAL |

## 4. Flujo de cambio del proveedor activo (CU-06)

El cambio de proveedor es una transición de configuración, no una operación de datos:

1. El usuario raíz indica el proveedor a activar y entrega parámetros y credenciales (a través del consumidor).
2. El registro valida el alcance de usuario raíz (AUTORIZACION_INSUFICIENTE si no lo tiene) y que el proveedor pertenezca al conjunto soportado (PROVEEDOR_NO_SOPORTADO).
3. Valida el formato de credenciales y parámetros (CREDENCIALES_INVALIDAS) sin intentar conectividad.
4. Comprueba conectividad y permisos contra el proveedor (PROVEEDOR_INACCESIBLE si falla). En modo validación en seco (FA-02), reporta el resultado sin activar.
5. Si todo es satisfactorio, fija el proveedor activo y resguarda las credenciales sin exponerlas (RN-03; ADR-05). Si cualquier paso falla, el proveedor activo previo se conserva intacto.
6. A partir de ese momento, las operaciones de datos resuelven el proveedor recién activado en el paso 3 del pipeline común, sin que el consumidor cambie su forma de invocarlas (RN-01).

## 5. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| CU cubiertos | CU-01, CU-02, CU-03 (foco), CU-04, CU-05 (mismo pipeline), CU-06 (transición) |
| RN aplicables | RN-01 (transparencia), RN-02 (integridad), RN-03 (credenciales) |
| ADRs que lo gobiernan | ADR-01 (enrutado por estrategia), ADR-04 (normalización), ADR-05 (credenciales) |
| Tests previstos (en 08) | Pruebas unitarias de validación y enrutado con doble del puerto; rechazo de entrada inválida sin contactar al proveedor; cambio de proveedor con continuidad del contrato |

## 6. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Flujo de ejecución inicial: pipeline común validar → resolver proveedor activo → delegar → normalizar para CU-01 a CU-05, enrutado comparado de guardar/recuperar/eliminar y transición de cambio de proveedor (CU-06). |
