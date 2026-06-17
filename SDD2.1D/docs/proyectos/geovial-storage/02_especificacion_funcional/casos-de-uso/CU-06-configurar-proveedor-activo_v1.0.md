# CU-06 — Configurar el proveedor de almacenamiento activo

**Proyecto:** geovial-storage
**Documento:** CU-06-configurar-proveedor-activo_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Permitir que el usuario raíz seleccione cuál es el proveedor de almacenamiento activo (proveedor local, proveedor de almacenamiento de objetos remoto u otro proveedor) y entregue las credenciales y parámetros necesarios, de modo que las operaciones de guardado, recuperación, eliminación, verificación y listado pasen a usar ese proveedor de forma transparente para el consumidor.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Usuario raíz | Primario | Selecciona el proveedor activo y entrega sus credenciales y parámetros |
| Proveedor de almacenamiento seleccionado | Sistema | Valida que las credenciales y parámetros permiten operar |

## 3. Precondiciones

- El usuario raíz está autenticado con el alcance de configuración del sistema.
- Existe al menos un proveedor disponible para seleccionar (el proveedor local siempre está disponible como mínimo).

## 4. Flujo principal

1. El usuario raíz indica qué proveedor desea activar y entrega sus parámetros y credenciales.
2. La librería valida que el proveedor indicado pertenezca al conjunto de proveedores soportados.
3. La librería valida que los parámetros y credenciales tengan el formato requerido por ese proveedor.
4. La librería realiza una comprobación de conectividad y de permisos contra el proveedor seleccionado.
5. Si la comprobación es satisfactoria, la librería fija ese proveedor como activo y confirma el cambio, sin revelar las credenciales recibidas (RN-03).
6. A partir de ese momento, las operaciones CU-01 a CU-05 usan el proveedor recién activado, sin que el consumidor cambie su forma de invocarlas (RN-01).

## 5. Flujos alternativos

- FA-01 Activación del proveedor local sin credenciales remotas. Disparador: el usuario raíz selecciona el proveedor local. La librería omite la validación de credenciales remotas y comprueba que la ubicación local sea accesible y escribible. Punto de retorno: paso 5 del flujo principal.
- FA-02 Validación sin activación (prueba en seco). Disparador: el usuario raíz solicita validar una configuración sin fijarla como activa. La librería ejecuta los pasos 2 a 4 y reporta el resultado sin cambiar el proveedor activo. Punto de retorno: fin del CU.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| PROVEEDOR_NO_SOPORTADO | El proveedor indicado no pertenece al conjunto soportado | La librería rechaza la operación y conserva el proveedor activo anterior |
| CREDENCIALES_INVALIDAS | Las credenciales o parámetros no tienen el formato requerido | La librería rechaza la operación sin intentar conectividad y conserva el proveedor activo anterior |
| PROVEEDOR_INACCESIBLE | La comprobación de conectividad o de permisos contra el proveedor falla | La librería rechaza la activación y conserva el proveedor activo anterior; no deja un estado de configuración a medias |
| AUTORIZACION_INSUFICIENTE | Quien invoca no tiene el alcance de usuario raíz | La librería rechaza la operación; ningún rol distinto del usuario raíz puede cambiar el proveedor activo |

## 7. Postcondiciones

- En caso de éxito: el proveedor activo queda fijado en el nuevo proveedor; las operaciones siguientes operan contra él sin cambios en el contrato público; las credenciales quedan resguardadas y no son legibles por la superficie pública (RN-03).
- En caso de fallo: el proveedor activo no cambia; la configuración previa sigue vigente y operativa.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | El proveedor activo es el local y el usuario raíz entrega parámetros válidos de un proveedor de objetos remoto con credenciales correctas | El usuario raíz activa el proveedor remoto | La librería confirma el cambio y las operaciones siguientes usan el proveedor remoto, sin cambiar la forma de invocarlas |
| CA-02 | El usuario raíz entrega credenciales con formato inválido para un proveedor remoto | El usuario raíz intenta activar ese proveedor | La librería rechaza con CREDENCIALES_INVALIDAS y mantiene el proveedor local como activo |
| CA-03 | El usuario raíz entrega credenciales con formato válido pero el proveedor remoto rechaza la conexión | El usuario raíz intenta activar ese proveedor | La librería rechaza con PROVEEDOR_INACCESIBLE y mantiene el proveedor activo anterior |
| CA-04 | Un actor sin alcance de usuario raíz | Ese actor intenta cambiar el proveedor activo | La librería rechaza con AUTORIZACION_INSUFICIENTE y no modifica el proveedor activo |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-07 (principal) |
| Reglas de negocio aplicables | RN-01, RN-03 |
| Historias de usuario a generar | US-08, US-09 (en 06) |
| Componentes esperados | Operación de selección y validación del proveedor activo, registro de proveedores soportados y resguardo de credenciales (en 05) |
| Tests previstos | Prueba de cambio efectivo de proveedor con continuidad del contrato; prueba de rechazo por credenciales inválidas; prueba de rechazo por proveedor inaccesible; prueba de rechazo por autorización insuficiente (en 08) |

## 10. Notas y supuestos

- Solo el usuario raíz cambia el proveedor activo; la jerarquía de roles proviene del intake (§2) y se hace cumplir aguas arriba por el consumidor, pero la librería no acepta el cambio sin el alcance correspondiente.
- El cambio de proveedor activo no migra los archivos ya guardados en el proveedor anterior; la migración de contenido, si se requiere, es una operación distinta que esta versión no contempla.
- El conjunto exacto de proveedores soportados y el formato de sus credenciales se definen en la categoría 05; esta especificación describe la selección de forma abstracta (local / remoto / otro).

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de configuración del proveedor activo del contrato de almacenamiento. |
