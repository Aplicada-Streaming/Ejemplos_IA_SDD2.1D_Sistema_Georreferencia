# Troubleshooting — geovial-storage

**Proyecto:** geovial-storage
**Documento:** troubleshooting_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Technical Writer + SDK Documentation Lead
**Tipo Diátaxis:** How-to (orientado a diagnóstico)
**Audiencia:** Developer integrador del backend que consume la abstracción de almacenamiento
**Nivel:** Medio
**Tiempo estimado de lectura:** 13 min

Cada entrada se identifica con un código `ISSUE-XX` referenciable desde el código de error, los logs o un ticket. El developer programa contra el código de error estable, no contra el texto del mensaje (RN-01). Ningún diagnóstico ni log expone credenciales ni parámetros de conexión (RN-03). El catálogo completo de mensajes vive en `dx-error-messages_v1.0.md` (03); las firmas, en `referencia-api_v1.0.md`.

## 1. Errores comunes

| Issue | Síntoma | Código | Causa probable | Solución |
| --- | --- | --- | --- | --- |
| ISSUE-01 | Recuperar o verificar devuelve que el archivo no existe | `IDENTIFICADOR_INEXISTENTE` | El archivo nunca se guardó, fue eliminado o el identificador está mal escrito | Listar bajo el prefijo para confirmar el identificador exacto, o guardar antes de recuperar |
| ISSUE-02 | Cualquier operación de datos falla antes de operar contra un destino | `PROVEEDOR_NO_CONFIGURADO` | Se invocó una operación sin un proveedor activo configurado | Configurar el proveedor activo con CU-06 (el local siempre está disponible) y reintentar |
| ISSUE-03 | La activación de un proveedor remoto se rechaza por formato | `CREDENCIALES_INVALIDAS` | Las credenciales o parámetros no tienen el formato requerido por el proveedor | Corregir el formato según el proveedor elegido (detalle en 05) y reintentar; el proveedor anterior se conserva |
| ISSUE-04 | Una operación o una activación falla por el proveedor (no responde, no accesible) | `PROVEEDOR_NO_DISPONIBLE`, `PROVEEDOR_INACCESIBLE` | El proveedor remoto rechazó la conexión, o la ubicación local no es accesible/escribible | Validar en seco (CU-06, FA-02), revisar conectividad y permisos, y reintentar; el proveedor anterior sigue vigente |
| ISSUE-05 | El guardado se rechaza por tamaño antes de contactar al proveedor | `TAMANIO_EXCEDIDO` | El contenido supera el tamaño máximo configurado (default 25 MB) | Reducir el contenido o revisar con el usuario raíz el límite configurado |
| ISSUE-06 | El cambio de proveedor activo se rechaza por permisos | `AUTORIZACION_INSUFICIENTE` | Un actor sin alcance de usuario raíz intentó cambiar el proveedor activo | Ejecutar la configuración con el alcance de usuario raíz; ningún otro rol puede cambiarlo |

## 2. Diagnóstico paso a paso

Antes de aplicar la solución, confirmar la causa con la secuencia de checks correspondiente.

### ISSUE-01 — Archivo inexistente

1. Confirmar que el código devuelto es exactamente `IDENTIFICADOR_INEXISTENTE` (no `DESTINO_INVALIDO`, que indica formato del identificador mal formado).
2. Listar bajo el prefijo del archivo: `listar(prefijo="<prefijo>")`. Comprobar si el identificador exacto aparece en el resultado.
3. Si no aparece: verificar que no fue eliminado por un flujo previo (`verificar` devuelve presencia falsa tras eliminar, CU-04, CA-03).
4. Si aparece con otra grafía: corregir el identificador en el código del consumidor; es opaco pero exacto.
5. Solución: guardar el archivo antes de recuperarlo, o usar el identificador exacto observado en el paso 2.

### ISSUE-02 — Proveedor no configurado

1. Confirmar que el código es `PROVEEDOR_NO_CONFIGURADO` y no `PROVEEDOR_NO_DISPONIBLE` (este último implica que sí hay proveedor pero no responde).
2. Verificar el orden de arranque del backend: la configuración del proveedor activo (CU-06) debe ejecutarse antes de la primera operación de datos.
3. Comprobar que la configuración inicial no falló silenciosamente: revisar el log de arranque buscando la confirmación de activación del proveedor.
4. Solución: configurar el proveedor activo (el local no requiere credenciales remotas) y reintentar la operación.

### ISSUE-03 — Credenciales inválidas

1. Confirmar que el código es `CREDENCIALES_INVALIDAS` (entrada inválida, se rechaza sin intentar conectividad) y no `PROVEEDOR_INACCESIBLE` (la conectividad sí se intentó y falló).
2. Revisar el formato de las credenciales contra el detalle del proveedor en 05; el error indica formato, no permisos.
3. Ejecutar una validación en seco (CU-06, FA-02) con las credenciales corregidas: `configurar(proveedor="remoto", parametros-y-credenciales=cfg, validacion-en-seco=verdadero)`.
4. Comprobar que el mensaje de error no contiene fragmentos de la credencial (RN-03); si los contuviera, es un defecto a reportar.
5. Solución: corregir el formato y reactivar; el proveedor anterior se conservó intacto.

### ISSUE-04 — Fallo del proveedor (no disponible / inaccesible)

1. Distinguir el código: `PROVEEDOR_INACCESIBLE` aparece al configurar (conectividad/permisos en la activación); `PROVEEDOR_NO_DISPONIBLE` aparece en una operación de datos con un proveedor ya activo.
2. Ejecutar una validación en seco del proveedor (CU-06, FA-02) para aislar si el problema es de conectividad o de permisos, sin cambiar el proveedor activo.
3. Para el proveedor local: comprobar que la ubicación existe, es accesible y escribible.
4. Para el proveedor remoto: confirmar conectividad de red y vigencia de permisos con el usuario raíz, sin volcar credenciales en los checks.
5. Reintentar la operación: ambos códigos son de error transitorio y reintentables.
6. Solución: si persiste, escalar al usuario raíz para revisar el estado del proveedor activo; el contenido no quedó a medias (CU-01, postcondición).

### ISSUE-05 — Tamaño excedido

1. Confirmar que el código es `TAMANIO_EXCEDIDO`; el rechazo ocurre en el núcleo antes de contactar al proveedor (ADR-04).
2. Medir el tamaño del contenido entregado y compararlo con el máximo configurado (valor por defecto 25 MB).
3. Comprobar que el límite es idéntico para todos los proveedores: el valor lo valida el núcleo, no el proveedor, así que no depende del destino (RN-01).
4. Solución: reducir el tamaño del contenido, o coordinar con el usuario raíz un ajuste del límite configurado (cambiar el valor no es un cambio de contrato, pero se coordina operativamente, ADR-04).

### ISSUE-06 — Autorización insuficiente

1. Confirmar que el código es `AUTORIZACION_INSUFICIENTE`; aplica solo a la operación de configurar el proveedor activo (CU-06).
2. Verificar que quien invoca el cambio tiene el alcance de usuario raíz; las operaciones de datos (CU-01 a CU-05) no requieren ese alcance.
3. Comprobar que el proveedor activo no cambió: la postcondición conserva la configuración previa (CU-06, CA-04).
4. Solución: ejecutar la configuración con el alcance de usuario raíz; ningún otro rol puede cambiar el proveedor activo.

## 3. Logs útiles

La librería expone puntos de medición de latencia por operación y conteo de errores por código (05 §7), sin imponer un sistema de métricas. Qué revisar:

| Qué buscar | Dónde | Patrón |
| --- | --- | --- |
| Confirmación de activación del proveedor | Log de arranque del backend | Evento de activación del proveedor activo (sin credenciales, RN-03) |
| Código de error de una operación fallida | Log de la operación afectada | El código estable en mayúsculas (por ejemplo `PROVEEDOR_NO_DISPONIBLE`) |
| Conteo de errores por código | Punto de medición de errores | Pico de un código concreto que indique una causa común |
| Latencia anómala de guardar/recuperar | Punto de medición de latencia | Percentil 95 por encima del objetivo (≤ 1 s para ≤ 5 MB con proveedor local, ADR-04) |

Regla de seguridad: ningún log de la librería emite credenciales ni parámetros de conexión. Si se observa un secreto en un log, es un defecto de severidad alta a reportar de inmediato (RN-03; ADR-05).

## 4. Cómo reportar un bug

Los defectos se reportan como issues etiquetados `dx` en el repositorio de la solución, separando "error de integración" de "documentación poco clara" (03 §7). Para preguntas de integración que no son defectos, usar la sección de discusiones del repositorio.

Plantilla de reporte:

```
Título: [geovial-storage] <síntoma en una línea>

Versión de la librería: <versión derivada del tag>
Operación afectada: guardar | recuperar | eliminar | verificar | listar | configurar
Código de error observado: <CODIGO_EN_MAYUSCULAS>  (o "ninguno")
Issue de troubleshooting relacionado: ISSUE-XX  (si aplica)
Proveedor activo: local | remoto | otro
Tamaño del contenido (si aplica): <bytes/KB/MB>

Pasos para reproducir:
1.
2.
3.

Resultado esperado:
Resultado obtenido:

Diagnóstico ya ejecutado (de §2):
Logs relevantes (SIN credenciales ni parámetros de conexión, RN-03):
```

Datos mínimos a adjuntar: versión, operación, código de error, proveedor activo y pasos de reproducción. Severidad sugerida: alta si hay filtración de credenciales en errores o logs (RN-03), pérdida de integridad del contenido (RN-02) o un cambio incompatible no anunciado (ADR-03); media para el resto. El tiempo de respuesta esperado lo fija el ciclo del equipo; dado el implementador de un único desarrollador (00 §2), la documentación prioriza el autodiagnóstico autónomo con las secciones §1 a §3 antes de abrir un issue.

## 5. Referencias cruzadas

- 03 `dx-error-messages_v1.0.md` §3: catálogo completo de los catorce códigos con mensaje, causa y acción.
- 05 `adrs/ADR-04-transparencia-limites-proveedor_v1.0.md`: tamaño máximo validado en el núcleo (ISSUE-05) y normalización de errores.
- 05 `adrs/ADR-05-manejo-seguro-credenciales_v1.0.md`: por qué ningún error ni log filtra secretos (ISSUE-03, ISSUE-04).
- 02 `casos-de-uso/CU-06-configurar-proveedor-activo_v1.0.md`: autorización (ISSUE-06) y validación en seco usada en el diagnóstico.
- 08 `casos-prueba-referenciales_v1.0.md` y `estrategia-testing_v1.0.md`: pruebas que cubren estos errores y la no filtración de credenciales.
- 10 `referencia-api_v1.0.md` §4 y `guia-integracion-servicio-backend_v1.0.md` §5: códigos y troubleshooting específico de la integración.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Troubleshooting inicial: seis entradas ISSUE-01 a ISSUE-06 (archivo inexistente, proveedor no configurado, credenciales inválidas, fallo del proveedor remoto/local, tamaño excedido y autorización insuficiente) con diagnóstico paso a paso, tabla de logs útiles y plantilla de reporte de bug. |
