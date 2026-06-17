# Extensibilidad — geovial-storage

**Proyecto:** geovial-storage
**Documento:** extensibilidad_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer

## 1. Objetivo

Describir el punto de extensión de `geovial-storage` —cómo se agrega un proveedor de almacenamiento nuevo— y el contrato que ese proveedor debe cumplir para integrarse sin tocar el núcleo de la librería. La extensibilidad es una capacidad pre-decidida del proyecto (intake §17.P.11: proveedores configurables local / remoto / otro) y se justifica en ADR-01 (abstracción con proveedores intercambiables por estrategia).

## 2. Punto de extensión: el proveedor de almacenamiento

El único punto de extensión de la librería es el puerto de proveedor de almacenamiento declarado en la capa de Abstracciones. Cualquier destino nuevo (otro servicio de objetos remoto, un almacenamiento de archivos alternativo, un doble de prueba) se incorpora como un adaptador que implementa ese puerto y se registra en el registro de proveedores. El núcleo de enrutado y la superficie pública no cambian: el proveedor nuevo queda disponible para CU-01 a CU-05 y seleccionable por CU-06 sin modificar al consumidor (RN-01).

| Atributo del punto de extensión | Valor |
| --- | --- |
| Qué se extiende | El conjunto de proveedores de almacenamiento soportados |
| Contrato a implementar | El puerto de proveedor de almacenamiento (operaciones de persistir, leer, borrar, comprobar presencia y enumerar bajo prefijo) |
| Cómo se activa | Registro del adaptador en el registro de proveedores y selección por el usuario raíz (CU-06) |
| Qué no se toca | La superficie pública (ADR-02), el núcleo de enrutado y el catálogo de errores |
| ADR que lo justifica | ADR-01 |

## 3. Contrato del proveedor (puerto)

Un proveedor nuevo debe implementar las operaciones del puerto de forma que el núcleo pueda enrutar hacia él de manera transparente. Cada operación del puerto corresponde a una operación de la superficie pública:

| Operación del puerto | Da soporte a | Obligación del adaptador |
| --- | --- | --- |
| Persistir contenido | CU-01 | Guardar el binario sin transformarlo (RN-02); no dejar archivo parcial ante fallo; respetar la marca de sobrescritura |
| Leer contenido | CU-02 | Devolver el contenido idéntico al persistido; soportar lectura por rango y modo solo-metadatos |
| Borrar | CU-03 | Quitar el contenido; soportar borrado bajo prefijo; informar el conjunto no eliminado en caso parcial |
| Comprobar presencia | CU-04 | Informar presencia coherente con el estado real, sin transferir contenido |
| Enumerar bajo prefijo | CU-05 | Devolver los identificadores bajo el prefijo, con paginación por testigo; garantizar cardinalidad y pertenencia (no el orden) |
| Validar configuración | CU-06 | Comprobar conectividad y permisos; aceptar credenciales sin exponerlas (RN-03) |

Obligaciones transversales que el adaptador debe cumplir (verificadas por la batería de contrato, ADR-04):

- Transparencia (RN-01): mapear sus fallos a los códigos de error uniformes del catálogo, sin introducir códigos ni comportamientos propios del proveedor.
- Integridad (RN-02): no transformar ni recodificar el contenido.
- Manejo seguro de credenciales (RN-03): acceder a las credenciales solo a través del resguardo de credenciales y no emitirlas en errores ni registros.
- Límite de tamaño: respetar el tamaño máximo configurado, validado por el núcleo antes de delegar (ADR-04).

## 4. Registro de un proveedor nuevo

Pasos para incorporar un proveedor, descritos como mecanismo (el detalle de stack vive en 11 y en intake §17):

1. Implementar un adaptador que cumpla el puerto de proveedor de almacenamiento (§3) y sus obligaciones transversales.
2. Declarar los parámetros y credenciales que el proveedor requiere y su validación de formato (CU-06, CREDENCIALES_INVALIDAS).
3. Registrar el adaptador en el registro de proveedores con un identificador de proveedor, de modo que el usuario raíz pueda seleccionarlo (CU-06). Si el proveedor no está registrado, la selección se rechaza con PROVEEDOR_NO_SOPORTADO.
4. Ejecutar la batería de pruebas de contrato única contra el proveedor nuevo (ADR-04): debe producir resultados equivalentes a los demás proveedores para las mismas entradas.
5. Verificar la no filtración de credenciales en operaciones, errores y registros (RN-03; ADR-05).

Una vez registrado y validado, el proveedor queda disponible sin que el consumidor `geovial-api` cambie su forma de invocar las operaciones (RN-01).

## 5. Ejemplo de extensión (referencia a 11)

El intake (§16.1) prevé para `geovial-storage` samples de "consumidores progresivos del proveedor de almacenamiento". El ejemplo de extensión —cómo construir y registrar un proveedor nuevo y correr la batería de contrato contra él— se materializa en la categoría 11 dentro de `samples/geovial-storage/` (detalle y cantidad exactos: pendientes en intake §16.1). El sample debe ser autocontenido y reproducible, y demostrar que el proveedor nuevo pasa la batería de contrato sin tocar el núcleo. El doble de prueba en memoria, usado por el núcleo para sus pruebas unitarias, es el ejemplo mínimo de adaptador y sirve de plantilla para un proveedor real.

## 6. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| CU cubiertos por el punto de extensión | CU-01, CU-02, CU-03, CU-04, CU-05 (operación) y CU-06 (registro y selección) |
| RN aplicables | RN-01 (transparencia), RN-02 (integridad), RN-03 (manejo seguro de credenciales) |
| ADR que lo justifica | ADR-01 (abstracción con proveedores intercambiables); ADR-04 (batería de contrato); ADR-05 (credenciales) |
| Ejemplo en 11 | `samples/geovial-storage/` consumidores progresivos del proveedor (intake §16.1); detalle pendiente |
| Tests previstos (en 08) | Prueba de que un proveedor nuevo registrado pasa la batería de contrato; prueba de rechazo por proveedor no soportado |

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Punto de extensión inicial de geovial-storage: el puerto de proveedor de almacenamiento, su contrato y obligaciones transversales, los pasos de registro de un proveedor nuevo, la referencia al ejemplo en 11 y la trazabilidad a ADR-01/ADR-04/ADR-05. |
