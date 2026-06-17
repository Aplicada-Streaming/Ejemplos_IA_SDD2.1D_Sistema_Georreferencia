# DX — Guía de onboarding del developer de geovial-storage

**Proyecto:** geovial-storage
**Documento:** guia-onboarding-developer_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** DX Lead
**Variante:** DX

## 0. Superficie pública y propósito de la guía

Esta guía recorre la primera hora del developer backend que integra la abstracción de almacenamiento de `geovial-storage` dentro de `geovial-api`. La superficie pública es el contrato único de seis operaciones —guardar, recuperar, eliminar, verificar existencia, listar bajo prefijo y configurar proveedor activo— descrito en la especificación funcional (02, CU-01 a CU-06). El recorrido se expresa en pasos y comportamiento observable, sin código de stack concreto: el código vive en 11 y el stack en 05. Es el modo tutorial del plan Diátaxis declarado en `dx-developer-experience_v1.0.md` §4.

## 1. Audiencia y prerrequisitos

Audiencia: developer backend integrador, de nivel intermedio, que ya maneja manejo de archivos y manejo de errores y que necesita guardar y recuperar las fotografías de los relevamientos sin atarse a un destino físico (ver `dx-developer-experience_v1.0.md` §1).

Prerrequisitos para iniciar:

- La librería `geovial-storage` está disponible para el backend `geovial-api` (el detalle de incorporación pertenece a 05/11).
- El developer puede editar la configuración del backend y ejecutar sus pruebas.
- No se requieren credenciales remotas para empezar: el proveedor local siempre está disponible como mínimo (CU-06, precondición). El cambio a un proveedor de almacenamiento de objetos remoto se ve al final de la primera hora.

## 2. Instalación o acceso

Pasos mínimos verificables para dejar la librería operativa con el proveedor local. El detalle de cómo se incorpora la librería al backend es de la categoría 05/11; aquí se fija el comportamiento que el developer debe observar.

1. Incorporar la abstracción de almacenamiento al backend según la categoría 05/11.
2. Configurar el proveedor activo como proveedor local, indicando una ubicación local accesible y escribible (CU-06, FA-01).
3. Verificación: la librería confirma que el proveedor local quedó activo. Si la ubicación no es accesible o escribible, la activación se rechaza con el error de proveedor inaccesible y la configuración previa sigue vigente (CU-06, error PROVEEDOR_INACCESIBLE; ver `dx-error-messages_v1.0.md`).

Hito verificable del acceso: existe un proveedor local activo y la librería lo confirma. A partir de aquí el contrato está listo para invocarse.

## 3. Primer ejemplo ejecutable

Recorrido que produce un resultado visible: guardar una fotografía de un relevamiento y recuperarla idéntica. Es el hito del tramo de 5 minutos. El snippet concreto se materializa en 11; aquí se describen los pasos y el resultado esperado, reproducibles contra el proveedor local sin credenciales remotas.

1. Invocar la operación de guardado entregando el contenido de una fotografía no vacía, el prefijo de destino `relevamientos/2026/r-001/` y el tipo de contenido `image/jpeg`. Resultado esperado: la librería devuelve un identificador lógico no vacío y el tamaño persistido, igual al tamaño entregado (CU-01, CA-01).
2. Invocar la operación de verificación de existencia con el identificador devuelto. Resultado esperado: presencia verdadera y el tamaño del archivo (CU-04, CA-01).
3. Invocar la operación de recuperación con ese identificador. Resultado esperado: un contenido idénticamente igual, byte a byte, al guardado, y el tipo `image/jpeg` (CU-02, CA-01; RN-02).
4. Invocar la operación de listado bajo el prefijo `relevamientos/2026/r-001/`. Resultado esperado: la lista contiene el identificador recién guardado (CU-05, CA-01).

Criterio de éxito: el contenido recuperado en el paso 3 es idéntico al guardado en el paso 1. Este es el primer resultado exitoso y confirma la integridad del contrato (RN-02).

## 4. Diagnóstico de problemas frecuentes en la primera hora

| Síntoma observado | Causa probable | Acción sugerida |
| --- | --- | --- |
| La activación del proveedor local se rechaza | La ubicación local indicada no existe, no es accesible o no es escribible | Corregir la ubicación y reintentar; ver error PROVEEDOR_INACCESIBLE en `dx-error-messages_v1.0.md` |
| El guardado se rechaza sin contactar al proveedor | El contenido entregado tiene tamaño cero, o el prefijo no cumple el formato admitido | Verificar que el contenido no esté vacío y que el destino lógico sea válido; ver CONTENIDO_VACIO y DESTINO_INVALIDO |
| La recuperación devuelve identificador inexistente | El identificador no fue guardado, fue eliminado o está mal escrito | Listar bajo el prefijo para confirmar el identificador exacto; ver IDENTIFICADOR_INEXISTENTE |
| Un guardado con identificador explícito se rechaza | El identificador ya existe y no se pidió sobrescritura | Activar la marca de sobrescritura si la intención es reemplazar, o elegir otro identificador; ver IDENTIFICADOR_DUPLICADO |
| El listado paginado parece incompleto | La cantidad de archivos excede el tamaño de página y hay un testigo de continuación pendiente | Reinvocar el listado con el testigo de continuación hasta agotarlo; ver TESTIGO_INVALIDO si el testigo venció |
| Al cambiar a un proveedor remoto, la activación se rechaza | Credenciales con formato inválido, o el proveedor remoto rechaza la conexión | Validar primero en seco (CU-06, FA-02); revisar el formato de credenciales y la conectividad; ver CREDENCIALES_INVALIDAS y PROVEEDOR_INACCESIBLE |
| Una operación falla y se sospecha del proveedor | El proveedor activo no responde | El error PROVEEDOR_NO_DISPONIBLE se propaga uniforme y nunca expone credenciales (RN-03); reintentar y, si persiste, revisar el estado del proveedor con el usuario raíz |

## 5. Próximos pasos

Enlaces explícitos a los modos Diátaxis (ver `dx-developer-experience_v1.0.md` §4):

- Tutorial completo: este documento; al terminarlo, el developer cubrió los tramos de 5 y 30 minutos.
- How-to (tarea): recetas por operación materializadas en 11 — guardar una foto, recuperar por rango, listar con paginación, eliminar bajo prefijo, validar un proveedor remoto en seco antes de activarlo (CU-06, FA-02).
- Reference (información): el contrato y sus tipos en la categoría 05, y el catálogo de errores en `dx-error-messages_v1.0.md`.
- Explanation (comprensión): por qué el contrato es idéntico en todo proveedor (RN-01), por qué la recuperación es byte a byte (RN-02) y por qué las credenciales nunca salen por la superficie pública (RN-03).
- Cierre de la primera hora (tramo de 60 minutos): asistir al usuario raíz a validar en seco y luego activar un proveedor de almacenamiento de objetos remoto, y confirmar que las operaciones ya escritas siguen funcionando sin cambios de código (CU-06, CA-01; RN-01).

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Guía de onboarding inicial: prerrequisitos, acceso con proveedor local, primer ejemplo guardar-recuperar idéntico, diagnóstico de la primera hora y próximos pasos enlazados a los cuatro modos Diátaxis. |
