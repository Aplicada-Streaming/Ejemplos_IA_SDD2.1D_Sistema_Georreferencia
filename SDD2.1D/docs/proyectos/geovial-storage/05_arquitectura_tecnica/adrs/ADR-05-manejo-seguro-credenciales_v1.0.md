# ADR-05 — Manejo seguro de las credenciales del proveedor

**Proyecto:** geovial-storage
**Documento:** ADR-05-manejo-seguro-credenciales_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer
**Categoría:** Seguridad

## 1. Contexto

El proveedor de almacenamiento de objetos remoto requiere credenciales de acceso cuya filtración comprometería toda la evidencia almacenada. La regla RN-03 exige que las credenciales y parámetros sensibles nunca se expongan por la superficie pública: no se devuelven en resultados, no aparecen en mensajes de error y no son recuperables una vez configuradas. Las credenciales entran por la configuración del proveedor activo (CU-06), que es atribución exclusiva del usuario raíz, y afectan también a los errores de recuperación (CU-02) y listado (CU-05), que no deben revelar configuración al fallar. El intake (§17.P.5) deja como pendiente el mecanismo concreto de almacenamiento seguro; esta decisión fija el comportamiento arquitectónico que cualquier mecanismo concreto debe cumplir.

## 2. Decisión

Se decide que las credenciales y parámetros sensibles del proveedor se custodien en un componente de resguardo de credenciales que las acepta por la configuración (CU-06) pero no las devuelve por ninguna vía de la superficie pública. No existe operación pública que lea la configuración sensible del proveedor activo. Los adaptadores acceden a las credenciales solo a través del resguardo, en el momento de operar contra su destino. Los mensajes de error se normalizan sin incluir credenciales ni parámetros de conexión, y los registros de diagnóstico nunca los emiten. La autorización para cambiar el proveedor activo se restringe al alcance de usuario raíz (error AUTORIZACION_INSUFICIENTE para cualquier otro). El mecanismo físico de almacenamiento seguro (cómo se cifran o resguardan en reposo) se delega a la categoría 09 a partir del intake §17.P.5, pero debe respetar este comportamiento.

## 3. Estado

Aceptado el 2026-06-15. El invariante está fijado en RN-03 y en CU-06; el mecanismo físico concreto queda pendiente en el intake (§17.P.5) sin alterar esta decisión de comportamiento.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Resguardo que entra pero no sale, sin operación de lectura de secretos (elegida) | Cumple RN-03 por diseño; minimiza la superficie de filtración; verificable con pruebas | Requiere que cada adaptador acceda al secreto solo por el resguardo |
| Exponer una operación de lectura de la configuración del proveedor activo | Facilita diagnóstico y auditoría desde el consumidor | Viola RN-03 (las credenciales serían recuperables); riesgo de seguridad inaceptable |
| Incluir el detalle del fallo del proveedor (con parámetros de conexión) en el mensaje de error | Diagnóstico más rico | Filtra configuración sensible en el error; viola RN-03 |
| Dejar el manejo de credenciales al consumidor (la librería las recibe en cada llamada) | La librería no custodia secretos | Multiplica los puntos donde el secreto circula; complica la transparencia; traslada el riesgo en vez de acotarlo |

## 5. Consecuencias positivas

1. Las credenciales tienen un único punto de entrada (CU-06) y ningún punto de salida por la superficie pública (RN-03).
2. Los errores y registros quedan libres de secretos por diseño, no por revisión manual.
3. La restricción de autorización a usuario raíz se hace cumplir en la propia librería, además de aguas arriba en el consumidor.
4. El comportamiento es independiente del mecanismo físico de resguardo, lo que permite que 09 elija el mecanismo sin romper la decisión.

## 6. Consecuencias negativas y trade-offs

1. La ausencia de una operación de lectura de la configuración sensible reduce las opciones de diagnóstico; se acepta como costo de seguridad (la validación en seco de CU-06, FA-02, cubre la necesidad de comprobar sin exponer).
2. Cada adaptador nuevo debe respetar el acceso a credenciales solo por el resguardo; es una convención a sostener en revisión.
3. El mecanismo físico de almacenamiento seguro queda pendiente (intake §17.P.5); hasta fijarlo en 09, la garantía es de comportamiento, no de cifrado en reposo.

## 7. Implementación

- El resguardo de credenciales custodia los parámetros sensibles recibidos en CU-06; expone acceso interno controlado a los adaptadores y nada hacia la superficie pública.
- CU-06 valida soporte y formato de credenciales, comprueba conectividad y permisos, y confirma el cambio sin revelar las credenciales recibidas; la validación en seco (FA-02) comprueba sin activar ni exponer.
- El núcleo normaliza PROVEEDOR_NO_DISPONIBLE, PROVEEDOR_INACCESIBLE y CREDENCIALES_INVALIDAS sin incluir secretos ni parámetros de conexión (el mensaje no repite las credenciales recibidas).
- La autorización a usuario raíz se verifica antes de aceptar el cambio (AUTORIZACION_INSUFICIENTE en caso contrario).
- El mecanismo de almacenamiento seguro en reposo lo fija la categoría 09 (intake §17.P.5).

## 8. Métricas de validación

- 0 ocurrencias de credenciales o parámetros de conexión en resultados, mensajes de error y registros (prueba de no filtración en 08, RN-03).
- No existe operación pública que devuelva la configuración sensible del proveedor activo (revisión de superficie pública).
- El cambio de proveedor activo se rechaza con AUTORIZACION_INSUFICIENTE para todo actor sin alcance de usuario raíz (CU-06, CA-04).

## 9. Referencias

- RN-03 (manejo seguro de credenciales); NB-07 (control del destino en manos del usuario raíz).
- CU-06 (configuración del proveedor activo, autorización), CU-02 y CU-05 (errores que no deben filtrar configuración).
- Intake §17.P.5 (manejo de credenciales y almacenamiento seguro: mecanismo concreto pendiente).
- Catálogo de errores: `dx-error-messages_v1.0.md` (03).
- ADRs relacionadas: ADR-01 (estilo), ADR-02 (superficie pública).

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión de seguridad: resguardo de credenciales que entran por CU-06 y no salen por la superficie pública; errores y registros sin secretos; autorización a usuario raíz; mecanismo físico delegado a 09. Aceptada. |
