# CU-01 — Inicializar y configurar la sesión de sincronización

**Proyecto:** aplicada-sync
**Documento:** CU-01-inicializar-sesion-sincronizacion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Permitir que una aplicación host configure e inicialice una sesión de sincronización del motor, indicando el origen del almacén local, el destino del backend remoto y la credencial vigente, de modo que el motor quede listo para encolar cambios y ejecutar la sincronización. Es el punto de entrada del contrato de la librería.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Aplicación host | Primario | Provee la configuración e invoca la inicialización del motor |
| Almacén local del host | Sistema | Conserva la cola de cambios y los metadatos de sincronización |
| Backend remoto | Sistema | Destino de la sincronización; se valida su alcanzabilidad de forma diferida |

## 3. Precondiciones

- La aplicación host dispone de un proveedor de credencial de autenticación vigente que puede entregar al motor.
- La aplicación host puede indicar un almacén local accesible para conservar metadatos y cola.
- La aplicación host conoce el punto de acceso del backend remoto al que sincronizar.
- No existe una sesión de sincronización ya inicializada con el mismo identificador lógico de host.

## 4. Flujo principal

1. La aplicación host arma la configuración de la sesión: identificador del host, referencia al almacén local, referencia al backend remoto y proveedor de credencial.
2. La aplicación host solicita al motor inicializar la sesión con esa configuración.
3. El motor valida que la configuración esté completa y sea internamente coherente.
4. El motor prepara o verifica las estructuras de metadatos de sincronización en el almacén local del host (cola de pendientes, marca de progreso, identificador de sesión).
5. El motor deja la sesión en estado listo y devuelve a la aplicación host un identificador de sesión y el estado inicial.
6. La aplicación host conserva la referencia a la sesión para las operaciones posteriores (encolar cambios, ejecutar, consultar estado).

## 5. Flujos alternativos

- 5.A Reinicialización sobre sesión previa existente. Disparador: el almacén local ya contiene una sesión previa del mismo host con cola y progreso. El motor reutiliza el estado persistido en lugar de crearlo, conserva los cambios pendientes y devuelve el estado recuperado. Retorna al paso 5 del flujo principal.
- 5.B Configuración sin proveedor de credencial inmediato. Disparador: el host inicializa de forma anticipada sin credencial vigente. El motor deja la sesión inicializada pero marcada como no autenticada; admite encolar cambios locales pero no ejecutar la sincronización hasta que se provea la credencial. Retorna al paso 5.

## 6. Excepciones y errores

| Código | Causa | Respuesta del motor |
| --- | --- | --- |
| CONFIGURACION_INCOMPLETA | Falta un campo obligatorio de la configuración (almacén local, backend remoto o identificador de host) | Rechaza la inicialización, no crea sesión y devuelve el detalle del campo faltante |
| ALMACEN_LOCAL_INACCESIBLE | El almacén local indicado no puede abrirse o no admite escritura de metadatos | Rechaza la inicialización y devuelve el error sin dejar estado parcial |
| SESION_YA_INICIALIZADA | Ya existe una sesión activa en memoria para el mismo identificador de host | Rechaza la segunda inicialización y devuelve la referencia a la sesión vigente |

## 7. Postcondiciones

- Éxito: existe una sesión de sincronización en estado listo (o no autenticada, en el flujo 5.B), con estructuras de metadatos disponibles en el almacén local del host.
- Fallo: no queda ninguna sesión a medias inicializar; el almacén local no conserva estructuras parciales atribuibles a esta operación.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Una configuración completa con identificador de host "host-01", almacén local accesible, backend remoto y credencial vigente | La aplicación host solicita inicializar la sesión | El motor devuelve un identificador de sesión no vacío y estado "listo" |
| CA-02 | Una configuración a la que le falta la referencia al backend remoto | La aplicación host solicita inicializar la sesión | El motor rechaza la operación con el código CONFIGURACION_INCOMPLETA y no crea sesión |
| CA-03 | Un almacén local que ya contiene una sesión previa del host "host-01" con 3 cambios pendientes | La aplicación host vuelve a inicializar con el mismo identificador de host | El motor recupera la sesión persistida y reporta 3 cambios pendientes sin perderlos |
| CA-04 | Una configuración completa pero sin proveedor de credencial vigente | La aplicación host solicita inicializar la sesión | El motor devuelve estado "no autenticada" y admite encolar cambios pero no ejecutar la sincronización |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-04 |
| Reglas de negocio aplicables | RN-02 |
| Historias de usuario a generar | US-01, US-02 (en 06) |
| Componentes esperados | Coordinador de sesión de sincronización; cola de cambios locales pendientes (referencia tentativa a 05) |
| Tests previstos | Inicialización con configuración completa; rechazo por configuración incompleta; recuperación de sesión persistida; sesión no autenticada (en 08) |

## 10. Notas y supuestos

- El motor no emite ni renueva credenciales: las recibe del host mediante un proveedor. La autenticación pertenece a la aplicación host.
- El motor es agnóstico del dominio del host: no interpreta el contenido de los cambios locales que más tarde se encolen.
- La validación de alcanzabilidad efectiva del backend remoto se difiere al momento de ejecutar la sincronización (CU-03); la inicialización no exige conectividad.

## 17. Compatibilidad de versión pública

La forma de la configuración de sesión y el contrato del estado devuelto (incluido el valor "no autenticada") forman parte de la superficie pública. Quitar un campo de configuración, cambiar su obligatoriedad o alterar el conjunto de estados devueltos constituye un cambio incompatible y obliga a incrementar la versión mayor del paquete.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de inicialización y configuración de la sesión de sincronización, derivado de NB-04 y del SOLUTION-INTAKE §17 (aplicada-sync). |
