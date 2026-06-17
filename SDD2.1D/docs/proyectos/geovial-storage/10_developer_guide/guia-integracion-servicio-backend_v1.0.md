# Guía de integración en un servicio backend — geovial-storage

**Proyecto:** geovial-storage
**Documento:** guia-integracion-servicio-backend_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Technical Writer + SDK Documentation Lead
**Tipo Diátaxis:** How-to
**Audiencia:** Developer integrador del backend que consume la abstracción de almacenamiento
**Nivel:** Medio
**Tiempo estimado de lectura:** 12 min

## 1. Objetivo

Integrar la abstracción de almacenamiento en un servicio backend genérico y seleccionar el proveedor activo, de modo que el servicio guarde y recupere archivos sin atarse a un destino físico y que cambiar de proveedor no requiera tocar el código del servicio. El sistema objetivo es un servicio backend cualquiera que necesite persistir archivos a través del contrato; el detalle de stack vive en 05/11. El porqué de cada decisión vive en `conceptos-fundamentales_v1.0.md`; esta guía resuelve la tarea sin desviaciones explicativas.

## 2. Prerequisites

| Prerequisito | Estado inicial mínimo |
| --- | --- |
| La librería incorporada al backend | Disponible según 05/11. |
| Un proveedor local listo | Una ubicación local accesible y escribible para la configuración inicial. |
| Capacidad de inyección de dependencias en el servicio | El servicio puede recibir una dependencia por su interfaz. |
| Configuración del proveedor remoto (opcional) | Parámetros y credenciales provistos por el usuario raíz, solo si se va a cambiar de proveedor. |

## 3. Pasos

### Paso 1. Depender del contrato, no del proveedor

Hacer que el servicio reciba la interfaz de almacenamiento (las cinco operaciones de datos) por inyección de dependencias. El servicio no debe referenciar ningún proveedor concreto ni ramificar por proveedor (RN-01; ADR-02).

Efecto esperado: el servicio compila dependiendo solo de la superficie pública de la librería.

### Paso 2. Configurar el proveedor activo inicial

El usuario raíz, a través de la interfaz de configuración, fija el proveedor local como activo indicando la ubicación local.

```
configurar(proveedor="local", parametros-y-credenciales={ ubicacion: "<carpeta-local>" })
```

Efecto esperado: la librería confirma que el proveedor local quedó activo (CU-06, FA-01).

### Paso 3. Guardar un archivo desde el servicio

Invocar la operación de guardado entregando el contenido, un destino lógico que agrupe los archivos del caso y el tipo de contenido.

```
guardar(contenido=<bytes>, destino="relevamientos/2026/r-001/", tipo="image/jpeg")
```

Efecto esperado: identificador lógico no vacío y tamaño persistido (CU-01, CA-01). Persistir ese identificador en el modelo del servicio para recuperarlo después.

### Paso 4. Recuperar y verificar desde el servicio

Usar el identificador guardado para recuperar el contenido y para verificar presencia sin transferir el binario.

```
verificar(identificador=<guardado>)            -> presencia=verdadero, tamaño
recuperar(identificador=<guardado>)            -> contenido idéntico al guardado (RN-02)
```

Efecto esperado: el contenido recuperado es idéntico al guardado; la verificación es coherente con el estado real (CU-02, CU-04).

### Paso 5. Manejar los errores por código

Capturar las excepciones por su código estable, no por el texto. Como mínimo, el servicio debe tratar `IDENTIFICADOR_INEXISTENTE` (recuperar/verificar), `CONTENIDO_VACIO` y `DESTINO_INVALIDO` (guardar) y `PROVEEDOR_NO_DISPONIBLE` (cualquier operación, reintentable). Los códigos son idénticos cualquiera sea el proveedor (RN-01).

Efecto esperado: ante un error catalogado, el servicio responde de forma controlada y no se cae. Para la lista completa, ver `referencia-api_v1.0.md` §4.

### Paso 6. Seleccionar otro proveedor activo sin tocar el servicio

El usuario raíz valida en seco y luego activa el proveedor remoto. El código del servicio no cambia.

```
configurar(proveedor="remoto", parametros-y-credenciales=cfg, validacion-en-seco=verdadero)  -> resultado; no activa
configurar(proveedor="remoto", parametros-y-credenciales=cfg)                                 -> confirmación (sin revelar credenciales)
```

Efecto esperado: a partir de la activación, las operaciones del servicio operan contra el proveedor remoto sin cambios de código (CU-06, CA-01; RN-01). Las credenciales no se devuelven ni aparecen en logs (RN-03).

## 4. Verificación

Confirmar que la integración funciona con esta lista de checks:

- El servicio guarda un archivo y obtiene un identificador no vacío.
- Recuperar ese identificador devuelve un contenido idéntico al guardado (igualdad binaria).
- Verificar el identificador devuelve presencia verdadera; verificar uno eliminado, presencia falsa.
- Listar bajo el prefijo del caso enumera el identificador guardado.
- Tras cambiar el proveedor activo de local a remoto, los cuatro checks anteriores siguen pasando sin modificar el código del servicio.
- Ningún resultado, error ni log contiene credenciales ni parámetros de conexión.

Criterio de éxito: el último check (transparencia al cambiar de proveedor) se cumple. Si se cumple, la integración está completa.

## 5. Troubleshooting específico

Subset de problemas frecuentes de esta integración; el diagnóstico paso a paso vive en `troubleshooting_v1.0.md`.

| Síntoma en la integración | Código | Diagnóstico |
| --- | --- | --- |
| Las operaciones fallan antes de operar contra ningún destino | `PROVEEDOR_NO_CONFIGURADO` | El servicio invocó datos antes de que el usuario raíz configurara el proveedor activo. Ver ISSUE-02. |
| La activación del proveedor local se rechaza | `PROVEEDOR_INACCESIBLE` | La ubicación local no existe, no es accesible o no es escribible. Ver ISSUE-04. |
| La activación del proveedor remoto se rechaza por formato | `CREDENCIALES_INVALIDAS` | El formato de las credenciales no es el esperado por el proveedor elegido. Ver ISSUE-03. |
| El cambio de proveedor lo rechaza por permisos | `AUTORIZACION_INSUFICIENTE` | Un actor sin alcance de usuario raíz intentó cambiar el proveedor activo. Ver ISSUE-06. |
| Una operación de datos falla de forma intermitente | `PROVEEDOR_NO_DISPONIBLE` | El proveedor activo no responde; reintentar. Ver ISSUE-04. |
| El guardado se rechaza por tamaño | `TAMANIO_EXCEDIDO` | El contenido supera el máximo configurado (default 25 MB). Ver ISSUE-05. |

## 6. Referencias cruzadas

- 05 `extensibilidad_v1.0.md` §2 y §3: el punto de extensión que permite que el servicio sea agnóstico al proveedor.
- 05 `contratos-abstractions_v1.0.md` §2: separación de la interfaz de almacenamiento y la de configuración que esta guía respeta.
- 05 `adrs/ADR-05-manejo-seguro-credenciales_v1.0.md`: por qué las credenciales no aparecen en resultados ni logs del servicio.
- 02 `casos-de-uso/CU-06-configurar-proveedor-activo_v1.0.md`: validación en seco y autorización del usuario raíz.
- 10 `referencia-api_v1.0.md` y `troubleshooting_v1.0.md`: firmas exactas y diagnóstico de los errores citados.

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | How-to de integración en un servicio backend genérico: dependencia del contrato por inyección, configuración del proveedor activo, flujo guardar/verificar/recuperar, manejo de errores por código, cambio de proveedor sin tocar el servicio, verificación y troubleshooting específico enlazado a los ISSUE-XX globales. |
