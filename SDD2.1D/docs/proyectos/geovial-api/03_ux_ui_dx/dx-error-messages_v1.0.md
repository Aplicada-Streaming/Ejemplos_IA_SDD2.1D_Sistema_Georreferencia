# DX — Catálogo de errores de geovial-api

**Proyecto:** geovial-api
**Documento:** dx-error-messages_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** API DX Designer + Developer Advocate
**Variante:** DX

## 0. Superficie pública que cubre este catálogo

Este catálogo alinea los códigos de error declarados por los casos de uso de 02 (CU-01 a CU-22) en un único contrato de error accionable para el integrador. Es el modo reference de errores del plan Diátaxis (`dx-developer-experience_v1.0.md` §4). Todos los errores se devuelven con el formato de problema uniforme problem+json (CU-19). Se describe en pasos y comportamiento, con vocabulario REST genérico; los estados de respuesta exactos y la representación física del problema pertenecen a 05, y la referencia formal exhaustiva a 10.

## 1. Principios de redacción de errores

- Cada error dice qué pasó, por qué pasó y qué hacer al respecto. El código indica qué pasó, la causa indica por qué, y la acción sugerida indica qué hacer.
- Lenguaje plano y sin culpar al integrador ni al usuario. Se describe la condición, no se acusa.
- El código estable es la clave de decisión, opaco al idioma. El integrador decide su tratamiento por el código, nunca por el texto del mensaje (CU-19).
- Un único problema por respuesta. Ante varios campos inválidos a la vez, el problema enumera cada campo con su motivo, en vez de emitir varias respuestas (CU-19, flujo 5.A).
- El error interno no filtra detalles internos sensibles: devuelve un código genérico sin exponer el origen (CU-19, flujo 5.B).
- La acción sugerida es concreta y ejecutable por el integrador, no un consejo genérico.

## 2. Taxonomía

| Categoría | Naturaleza | Estado de respuesta típico | Cómo la trata el integrador |
| --- | --- | --- | --- |
| Autenticación | El solicitante no está autenticado o su sesión no es válida | No autorizado | Reautenticar y reintentar (CU-03, CU-18) |
| Autorización por rol y alcance | Autenticado, pero el rol o el ámbito no habilitan la acción | Prohibido | No reintentar igual; corregir rol o ámbito (CU-18, RN-01) |
| Validación de entrada | La solicitud no respeta la estructura, el rango o las opciones admitidas | Solicitud inválida | Corregir la entrada y reintentar (CU-19, CU-20) |
| Recurso inexistente | El recurso solicitado no existe o no existe en la versión pedida | No encontrado | Verificar el identificador o la versión del contrato (CU-19, CU-22) |
| Conflicto de estado | El recurso está en un estado que no admite la operación | Conflicto | Resolver el estado previo antes de reintentar (RN-05) |
| Relevamiento cerrado | El relevamiento cerrado ya no admite cambios | Conflicto | Dejar de escribir sobre ese relevamiento (RN-05) |
| Idempotencia | Conflicto o ausencia relacionada con la clave de idempotencia | Conflicto o solicitud inválida | Ajustar la clave de idempotencia (CU-21, RN-07) |
| Error interno | Fallo no previsto en el procesamiento | Error interno | Reintentar más tarde; escalar si persiste (CU-19) |

## 3. Catálogo

### 3.1 Autenticación

| Código | Categoría | Causa probable | Acción sugerida | CU |
| --- | --- | --- | --- | --- |
| CREDENCIALES_INVALIDAS | Autenticación | El identificador o la credencial no coinciden | Verificar el par de credenciales y volver a solicitar el token | CU-03 |
| USUARIO_INHABILITADO | Autenticación | El usuario fue dado de baja; conserva traza pero no acceso | Usar un usuario habilitado; el acceso del usuario dado de baja está revocado | CU-03 |
| TOKEN_REVOCADO | Autenticación | El token fue invalidado por un cierre de sesión completo | Reautenticarse para obtener un token nuevo | CU-03 |
| NO_AUTENTICADO | Autenticación | La solicitud no porta token o el token no es legítimo | Adjuntar el token bearer válido en el encabezado de autorización | CU-18 |

### 3.2 Autorización por rol y alcance

| Código | Categoría | Causa probable | Acción sugerida | CU |
| --- | --- | --- | --- | --- |
| ACCION_NO_PERMITIDA | Autorización | El rol del solicitante no habilita la acción pedida | Realizar la acción con un usuario cuyo rol la permita; no reintentar con el mismo rol | CU-18 |
| FUERA_DE_ALCANCE | Autorización | El recurso destino pertenece a otro ámbito jerárquico | Operar solo recursos dentro del propio ámbito del solicitante (RN-01) | CU-18 |
| ROL_NO_AUTORIZADO | Autorización | El solicitante no tiene el rol requerido por el recurso (no es jefe de área, dueño del relevamiento, usuario raíz o habilitado para importar, según el caso) | Usar el rol correspondiente al recurso (CU-02, CU-04, CU-06, CU-13, CU-14, CU-16, CU-17) | CU-02, CU-04, CU-06, CU-13, CU-14, CU-16, CU-17 |
| JERARQUIA_NO_PERMITIDA | Autorización | Se intenta administrar un nivel que no es el inmediato inferior al del solicitante | Administrar solo el nivel inmediato inferior según la jerarquía (RN-01) | CU-01 |
| USUARIO_FUERA_DE_AMBITO | Autorización | La baja apunta a un usuario fuera del ámbito del solicitante | Operar solo usuarios del propio ámbito | CU-01 |
| AGENTE_FUERA_DE_AREA | Autorización | El agente no pertenece al área del jefe | Operar o asignar solo agentes del área propia | CU-02, CU-05 |
| RELEVAMIENTO_FUERA_DE_AMBITO | Autorización | El relevamiento pertenece a otro jefe | Operar solo relevamientos del propio ámbito | CU-04, CU-12, CU-15 |
| RELEVAMIENTO_NO_ASIGNADO | Autorización | El relevamiento no está asignado al agente | Sincronizar solo relevamientos asignados al agente | CU-10, CU-11 |

### 3.3 Validación de entrada

| Código | Categoría | Causa probable | Acción sugerida | CU |
| --- | --- | --- | --- | --- |
| FORMATO_SOLICITUD_INVALIDO | Validación | La solicitud no respeta la estructura esperada del recurso | Corregir la estructura del cuerpo según el defecto señalado en el problema | CU-19 |
| TRAMO_INCOMPLETO | Validación | El tramo vial no define ningún puente ni camino | Incluir al menos un puente o camino en el tramo antes de crear el relevamiento | CU-04 |
| COORDENADA_INVALIDA | Validación | La coordenada está fuera del rango geográfico admitido | Enviar una coordenada dentro del rango admitido | CU-07 |
| AGENTE_INHABILITADO | Validación | El agente a asignar fue dado de baja | Asignar un agente habilitado | CU-05 |
| RADIO_NO_DEFINIDO | Validación | No hay un radio de agrupación aplicable a la carga | Definir el radio de agrupación antes de la carga manual de fotos (RN-04) | CU-09 |
| FORMATO_FOTO_NO_SOPORTADO | Validación | Una foto del lote tiene un formato que el sistema no procesa | Reenviar esa foto en un formato soportado; el resto del lote se procesó y la foto omitida se señala en la respuesta | CU-09 |
| MARCADOR_INEXISTENTE | Validación | La observación referencia un marcador que no existe en el relevamiento | Crear u referenciar un marcador existente antes de la observación (RC-02) | CU-08 |
| UNIDAD_INVALIDA | Validación | La unidad de importación no es reconocible o está corrupta | Reexportar la unidad desde el origen y reintentar la importación | CU-16 |
| UNIDAD_INCOMPLETA | Validación | A la unidad le faltan piezas para reconstruir el relevamiento | Completar las piezas faltantes que indica el problema y reimportar | CU-16 |
| LOTE_MALFORMADO | Validación | Un cambio del lote no porta identificador de origen o viola la estructura | Corregir el cambio señalado; el lote no se aplicó (CU-10) | CU-10 |
| MARCA_INVALIDA | Validación | La marca de última sincronización aportada no es reconocible | Solicitar una sincronización completa del relevamiento (RN-06) | CU-11 |
| FILTRO_NO_SOPORTADO | Validación | El recurso no admite el filtro indicado | Usar uno de los filtros válidos que informa la respuesta | CU-20 |
| ORDEN_NO_SOPORTADO | Validación | El campo de orden no está admitido | Usar uno de los campos de orden válidos que informa la respuesta | CU-20 |
| POSICION_INVALIDA | Validación | La posición de página solicitada no es válida | Empezar por la primera página y seguir las referencias de página | CU-20 |
| PROVEEDOR_NO_DISPONIBLE | Validación | El proveedor de destino de almacenamiento no es alcanzable | Verificar la disponibilidad del proveedor antes de activarlo | CU-17 |
| CREDENCIALES_PROVEEDOR_INVALIDAS | Validación | Las credenciales del proveedor no permiten alojar ni recuperar archivos | Corregir las credenciales del proveedor; se conserva el destino anterior | CU-17 |
| VERSION_NO_SOPORTADA | Validación | Se solicita una versión del contrato retirada o inexistente | Migrar a una de las versiones vigentes que informa la respuesta (CU-22) | CU-22 |
| VERSION_REQUERIDA_AUSENTE | Validación | La política exige versión explícita y no se indicó | Indicar la versión mayor del contrato según informa la respuesta | CU-22 |

### 3.4 Recurso inexistente

| Código | Categoría | Causa probable | Acción sugerida | CU |
| --- | --- | --- | --- | --- |
| RECURSO_NO_ENCONTRADO | Recurso inexistente | El recurso solicitado no existe | Verificar el identificador del recurso | CU-19 |
| RELEVAMIENTO_INEXISTENTE | Recurso inexistente | El relevamiento solicitado no existe | Verificar el identificador del relevamiento | CU-12, CU-15 |
| RECURSO_NO_EN_VERSION | Recurso inexistente | El recurso no existe en la versión indicada del contrato | Pedir el recurso en una versión que lo exponga (CU-22) | CU-22 |
| CONFLICTO_INEXISTENTE | Recurso inexistente / Conflicto de estado | El conflicto a resolver no existe o ya fue resuelto y cerrado | Releer los conflictos pendientes antes de resolver | CU-13 |

### 3.5 Conflicto de estado

| Código | Categoría | Causa probable | Acción sugerida | CU |
| --- | --- | --- | --- | --- |
| TRANSICION_NO_PERMITIDA | Conflicto de estado | El estado origen no admite la transición pedida | Avanzar el estado respetando el ciclo recolección, revisión, cierre (RN-05) | CU-06 |
| RELEVAMIENTO_NO_EN_REVISION | Conflicto de estado | El relevamiento no está en revisión al resolver conflictos o cerrar | Llevar el relevamiento a revisión antes de resolver conflictos o cerrar (RN-05) | CU-13, CU-14 |
| CONFLICTOS_PENDIENTES | Conflicto de estado | Quedan conflictos de marcadores sin resolver al intentar cerrar | Resolver todos los conflictos pendientes antes de cerrar (RN-03, RN-05) | CU-14 |
| MARCADOR_CON_OBSERVACIONES | Conflicto de estado | La baja del marcador dejaría observaciones sin ancla | No dar de baja un marcador con observaciones; reasignar o quitar las observaciones primero (RC-02) | CU-07 |
| IDENTIFICADOR_DUPLICADO | Conflicto de estado | El identificador de acceso del nuevo usuario ya existe | Elegir un identificador de acceso distinto | CU-01, CU-02 |
| SUBIDA_NO_CONCLUIDA | Conflicto de estado | Se solicita la bajada sin haber concluido la subida del ciclo | Completar primero la subida del ciclo de sincronización (RN-06) | CU-11 |

### 3.6 Relevamiento cerrado

| Código | Categoría | Causa probable | Acción sugerida | CU |
| --- | --- | --- | --- | --- |
| RELEVAMIENTO_CERRADO | Relevamiento cerrado | El relevamiento está cerrado y no admite cambios (marcadores, observaciones, fotos, asignaciones, transiciones ni subida) | Dejar de escribir sobre ese relevamiento; el cierre bloquea nuevas escrituras. En la sincronización, el cliente deja de reintentar la subida de ese relevamiento | CU-05, CU-06, CU-07, CU-08, CU-09, CU-10 |

### 3.7 Idempotencia

| Código | Categoría | Causa probable | Acción sugerida | CU |
| --- | --- | --- | --- | --- |
| CLAVE_REQUERIDA_AUSENTE | Idempotencia | La operación no segura exige clave de idempotencia y no se proveyó | Adjuntar una clave de idempotencia estable para la operación | CU-21 |
| CLAVE_REUTILIZADA_INCONSISTENTE | Idempotencia | Una clave ya procesada se reutiliza con un contenido distinto | Usar una clave nueva para una operación de contenido distinto; reusar la clave solo para el reintento exacto de la misma operación | CU-21 |
| OPERACION_NO_IDEMPOTENTE | Idempotencia | Se envía clave a una operación que no admite idempotencia | No enviar clave de idempotencia a operaciones seguras o no reintentables | CU-21 |

### 3.8 Recursos de almacenamiento parcial (degradación informada, no rechazo duro)

Estos casos no rechazan toda la operación: la completan parcialmente e informan el faltante en el problema, para que el integrador decida.

| Código | Categoría | Causa probable | Acción sugerida | CU |
| --- | --- | --- | --- | --- |
| FOTO_NO_ALMACENABLE | Validación / Error transitorio | El almacén no puede alojar el binario de la foto | Reintentar la carga de esa foto; la observación se conservó sin esa foto | CU-08 |
| FOTO_NO_RECUPERABLE | Error interno / transitorio | Una foto del relevamiento no puede recuperarse del almacén al exportar | La exportación se detuvo para no entregar una unidad incompleta; verificar el almacén y reexportar | CU-15 |

### 3.9 Error interno

| Código | Categoría | Causa probable | Acción sugerida | CU |
| --- | --- | --- | --- | --- |
| ERROR_INTERNO | Error interno | Fallo no previsto en el procesamiento | Reintentar más tarde; si persiste, escalar al equipo de geovial-api con el momento del fallo. El problema no expone detalles internos | CU-19 |

### 3.10 Notas de degradación informada

- ETIQUETA_DESCONOCIDA (CU-12) no es un error duro: el filtro por una etiqueta inexistente devuelve un conjunto vacío e informa las etiquetas válidas. El integrador no debe tratarlo como rechazo, sino releer las etiquetas válidas.

## 4. Tono y voz

- Tono neutro, descriptivo y orientado a la acción. Coherente con el contrato funcional de error de CU-19 y con la guía de estilo del proyecto que se consolida en 10.
- Sin culpa ni juicio sobre el integrador o el usuario. Se describe la condición y el siguiente paso.
- Mensajes legibles cortos para mostrar al usuario final de la app cliente; el detalle accionable vive en el código estable y en este catálogo, no en el texto del mensaje.
- Consistencia de vocabulario entre recursos: el mismo concepto usa el mismo código en toda la superficie (por ejemplo, RELEVAMIENTO_CERRADO es el mismo código en todos los recursos que escriben sobre un relevamiento).

## 5. Localización

- Los códigos estables son opacos al idioma y no se traducen: son la clave de decisión del cliente (CU-19). El integrador nunca ramifica por el texto.
- El mensaje legible es el único elemento traducible. La política de traducción del mensaje para el usuario final reside en las apps cliente (geovial-web, geovial-mobile), que deciden el idioma de presentación.
- La causa probable y la acción sugerida de este catálogo son material para developers; su idioma sigue la guía de estilo del proyecto y no condiciona el idioma del mensaje mostrado al usuario final.
- Al agregar un código nuevo, se agrega aquí su causa y su acción accionable antes de exponerlo en el contrato, para no publicar un código sin diagnóstico.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Catálogo inicial de errores de geovial-api alineado a los códigos de los CU-01 a CU-22, agrupado por taxonomía (autenticación, autorización por rol y alcance, validación, recurso inexistente, conflicto de estado, relevamiento cerrado, idempotencia, error interno), con causa y acción accionable por código, sobre formato problem+json. |
