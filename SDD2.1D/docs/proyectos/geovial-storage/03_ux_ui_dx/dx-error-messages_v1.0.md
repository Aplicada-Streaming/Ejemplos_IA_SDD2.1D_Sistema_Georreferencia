# DX — Catálogo de mensajes de error de geovial-storage

**Proyecto:** geovial-storage
**Documento:** dx-error-messages_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** DX Lead
**Variante:** DX

## 0. Superficie pública cubierta

Este catálogo cubre los errores que la abstracción de almacenamiento de `geovial-storage` devuelve por su superficie pública al developer backend integrador y, en la configuración del proveedor (CU-06), al usuario raíz. Los códigos provienen de las secciones de excepciones de los seis casos de uso (02, CU-01 a CU-06). Es el componente reference de los errores dentro del plan Diátaxis declarado en `dx-developer-experience_v1.0.md` §4 y §5.

Dos invariantes de dominio gobiernan todo el catálogo:

- Transparencia (RN-01): el conjunto de códigos de error es el mismo cualquiera sea el proveedor activo. Ningún código ni mensaje depende del proveedor; el developer no escribe ramas por proveedor para interpretarlos.
- Manejo seguro de credenciales (RN-03): ningún mensaje de error incluye credenciales ni parámetros de conexión del proveedor.

## 1. Principios de redacción de errores

- Lenguaje plano: el mensaje se entiende sin conocer el detalle interno del proveedor.
- Estructura de tres partes: qué pasó, por qué pasó y qué hacer al respecto.
- Sin culpar al developer ni al usuario: el mensaje describe la condición, no a la persona.
- Sin filtrar secretos: el mensaje nunca incluye credenciales, claves ni parámetros de conexión del proveedor (RN-03).
- Uniforme entre proveedores: el mismo código y el mismo texto para la misma condición, sea el proveedor local, el remoto u otro (RN-01).
- Accionable: cada mensaje termina en un paso concreto que el developer o el usuario raíz puede ejecutar.

## 2. Taxonomía

| Categoría | Definición | Códigos de esta categoría |
| --- | --- | --- |
| Entrada inválida | El llamado no cumple el formato o las precondiciones del contrato; se rechaza antes de delegar en el proveedor | CONTENIDO_VACIO, DESTINO_INVALIDO, RANGO_INVALIDO, TESTIGO_INVALIDO, CREDENCIALES_INVALIDAS, PROVEEDOR_NO_SOPORTADO, TAMANIO_EXCEDIDO |
| Recurso ausente | El identificador o el proveedor pedido no está presente o no está configurado | IDENTIFICADOR_INEXISTENTE, PROVEEDOR_NO_CONFIGURADO |
| Conflicto de estado | La operación choca con el estado actual del almacenamiento | IDENTIFICADOR_DUPLICADO, ELIMINACION_PARCIAL |
| Error transitorio | Una condición externa o de conectividad impide completar la operación; puede reintentarse | PROVEEDOR_NO_DISPONIBLE, PROVEEDOR_INACCESIBLE |
| Permiso insuficiente | Quien invoca no tiene el alcance requerido | AUTORIZACION_INSUFICIENTE |

## 3. Catálogo

Cada fila declara qué pasó (mensaje), por qué pasó (causa probable) y qué hacer al respecto (acción sugerida). Los cinco errores que el encargo pide cubrir de forma explícita —archivo inexistente, proveedor no configurado, credenciales inválidas, fallo del proveedor remoto y tamaño excedido— están marcados en la columna de notas.

| Código | Categoría | Mensaje (qué pasó) | Causa probable (por qué pasó) | Acción sugerida (qué hacer) | CU | Notas |
| --- | --- | --- | --- | --- | --- | --- |
| CONTENIDO_VACIO | Entrada inválida | El contenido a guardar está vacío; no se creó ningún archivo | Se invocó guardar con un contenido de tamaño cero | Verificar que el contenido tenga al menos un byte antes de guardar | CU-01 | — |
| DESTINO_INVALIDO | Entrada inválida | El identificador o prefijo lógico no cumple el formato admitido | El destino contiene caracteres o estructura no admitidos por el contrato | Ajustar el identificador o prefijo al formato admitido (definido en 05) y reintentar | CU-01, CU-02, CU-03, CU-04, CU-05 | — |
| IDENTIFICADOR_DUPLICADO | Conflicto de estado | Ya existe un archivo con ese identificador y no se pidió sobrescritura | Se guardó con un identificador explícito que ya estaba en uso | Activar la marca de sobrescritura para reemplazar, o elegir otro identificador | CU-01 | — |
| TAMANIO_EXCEDIDO | Entrada inválida | El contenido supera el tamaño máximo admitido por el proveedor activo | La fotografía u archivo excede el límite de tamaño configurado | Reducir el tamaño del contenido o revisar con el usuario raíz el límite del proveedor activo | CU-01 | Tamaño excedido (encargo). El límite numérico exacto es un NFR pendiente en intake §17.P.10; ver §0 de notas |
| IDENTIFICADOR_INEXISTENTE | Recurso ausente | El identificador no corresponde a ningún archivo del proveedor activo; no se devuelve contenido | El archivo nunca se guardó, fue eliminado o el identificador está mal escrito | Listar bajo el prefijo para confirmar el identificador exacto, o guardar el archivo antes de recuperarlo | CU-02 | Archivo inexistente (encargo) |
| RANGO_INVALIDO | Entrada inválida | El rango de bytes solicitado excede el tamaño del archivo o está mal formado | Se pidió un rango fuera de los límites del archivo | Ajustar el rango al tamaño del archivo (verificable con la operación de existencia) y reintentar | CU-02 | — |
| ELIMINACION_PARCIAL | Conflicto de estado | Parte del conjunto bajo el prefijo no pudo eliminarse | En una eliminación múltiple, algunos identificadores no se eliminaron | Reintentar la eliminación sobre los identificadores informados como no eliminados | CU-03 | — |
| TESTIGO_INVALIDO | Entrada inválida | El testigo de continuación del listado está vencido o mal formado | Se reinvocó el listado con un testigo caducado o alterado | Reiniciar el listado desde la primera página, sin testigo | CU-05 | — |
| PROVEEDOR_NO_SOPORTADO | Entrada inválida | El proveedor indicado no pertenece al conjunto soportado | Se pidió activar un proveedor que la librería no conoce | Elegir un proveedor del conjunto soportado (local / remoto / otro, según 05) | CU-06 | El proveedor activo anterior se conserva |
| CREDENCIALES_INVALIDAS | Entrada inválida | Las credenciales o parámetros no tienen el formato requerido por el proveedor | El formato de las credenciales entregadas no es el esperado | Corregir el formato de las credenciales según el proveedor elegido (detalle en 05) y reintentar | CU-06 | Credenciales inválidas (encargo). El mensaje no repite ni refleja las credenciales recibidas (RN-03) |
| PROVEEDOR_INACCESIBLE | Error transitorio | La comprobación de conectividad o de permisos contra el proveedor falló; la activación se rechazó | El proveedor remoto rechazó la conexión, o la ubicación local no es accesible/escribible | Validar en seco (CU-06, FA-02), revisar conectividad y permisos, y reintentar; el proveedor activo anterior sigue vigente | CU-06 | Fallo del proveedor remoto en activación (encargo). El mensaje no expone parámetros de conexión (RN-03) |
| PROVEEDOR_NO_DISPONIBLE | Error transitorio | El proveedor activo no respondió o rechazó la operación; no quedó un archivo a medias | Caída temporal o indisponibilidad del proveedor activo durante guardar, recuperar, eliminar, verificar o listar | Reintentar la operación; si persiste, revisar con el usuario raíz el estado del proveedor activo | CU-01, CU-02, CU-03, CU-04, CU-05 | Fallo del proveedor remoto en operación (encargo). Se propaga uniforme y nunca expone credenciales (RN-01, RN-03) |
| PROVEEDOR_NO_CONFIGURADO | Recurso ausente | No hay un proveedor activo configurado para operar | Se invocó una operación de almacenamiento antes de configurar el proveedor activo | Configurar el proveedor activo con CU-06 (el proveedor local siempre está disponible como mínimo) y reintentar | CU-01, CU-02, CU-03, CU-04, CU-05 | Proveedor no configurado (encargo). Deriva de la precondición común de los CU; ver §0 de notas |
| AUTORIZACION_INSUFICIENTE | Permiso insuficiente | Quien invoca no tiene el alcance de usuario raíz para cambiar el proveedor activo | Un rol distinto del usuario raíz intentó configurar el proveedor | Ejecutar la configuración con el alcance de usuario raíz; ningún otro rol puede cambiar el proveedor activo | CU-06 | El proveedor activo no se modifica |

Notas del catálogo:

- PROVEEDOR_NO_CONFIGURADO y TAMANIO_EXCEDIDO no figuran con esos nombres en las tablas de excepciones de 02, pero el encargo de esta sección pide cubrir "proveedor no configurado" y "tamaño excedido". Se incorporan como entradas del catálogo derivadas de las precondiciones comunes de los CU (existe un proveedor activo configurado y validado) y de la postcondición de guardado, respectivamente. El nombre y el detalle definitivos del código se confirman al concretar el contrato en 05; aquí se fija el comportamiento esperado, coherente con RN-01.
- El límite numérico de tamaño que dispara TAMANIO_EXCEDIDO es un requerimiento no funcional pendiente en el intake (§17.P.10, tamaño máximo de archivo: PENDIENTE). No es bloqueante para este catálogo: el mensaje y la acción no dependen del valor exacto. Se documenta la dependencia para que 05 fije el umbral.

## 4. Tono y voz

- Voz neutra y directa, en español rioplatense técnico, coherente con el tono del resto de la documentación de la solución.
- Mensajes en presente y orientados a la acción: "ajustá el rango", "reintentá la operación".
- Sin jerga del proveedor concreto: se habla de "proveedor activo", "proveedor remoto", "proveedor local", nunca de un producto comercial.
- Sin signos de exclamación, sin emojis, sin culpa: el mensaje describe la condición y propone el siguiente paso.
- Consistencia léxica con el glosario de 02: "identificador lógico", "prefijo", "proveedor activo", "credenciales".

## 5. Localización

- Idioma base de los mensajes: español rioplatense, alineado con la audiencia interna del implementador (00 §2).
- Los códigos de error (en mayúsculas, sin tildes) son estables e independientes del idioma; sirven como clave de localización y de diagnóstico. El developer programa contra el código, no contra el texto.
- El texto descriptivo (qué pasó / por qué / qué hacer) es traducible sin cambiar el código ni la semántica. Agregar una traducción es un cambio compatible (versión menor), coherente con la nota de estabilidad del contrato (02 §6).
- Política para esta versión: solo español. No hay multilenguaje comprometido; si el negocio lo solicita, se incorpora como tabla de traducción indexada por código.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Catálogo inicial de errores de la abstracción de almacenamiento: principios de redacción, taxonomía de cinco categorías, catálogo de catorce códigos derivados de CU-01 a CU-06 (incluidos archivo inexistente, proveedor no configurado, credenciales inválidas, fallo del proveedor remoto y tamaño excedido), tono y voz, y política de localización en español. |
