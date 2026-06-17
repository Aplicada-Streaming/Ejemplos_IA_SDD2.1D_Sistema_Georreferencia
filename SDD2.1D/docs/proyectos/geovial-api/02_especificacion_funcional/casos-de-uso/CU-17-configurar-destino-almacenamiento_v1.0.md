# CU-17 — Configurar el destino de almacenamiento de archivos

**Proyecto:** geovial-api
**Documento:** CU-17-configurar-destino-almacenamiento_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Permitir que el usuario raíz seleccione y cambie el destino donde se alojan las fotografías de los relevamientos (almacenamiento dentro de la propia infraestructura, en un servicio externo de objetos u otro proveedor), de forma transparente para los demás roles. Da al negocio control sobre el almacenamiento según costo, capacidad y contexto del despliegue.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Usuario raíz | Primario | Selecciona y cambia el destino de almacenamiento |
| Backend de configuración | Sistema | Valida y aplica el destino activo a través de la librería de almacenamiento |
| Librería de almacenamiento | Sistema | Materializa el proveedor activo seleccionado |

## 3. Precondiciones

- El solicitante está autenticado y su rol es usuario raíz (CU-03).
- El proveedor de destino a configurar está disponible y sus credenciales son válidas.

## 4. Flujo principal

1. El usuario raíz solicita establecer el destino de almacenamiento, indicando el proveedor y sus parámetros de acceso.
2. El backend valida que el solicitante es usuario raíz (RN-01: solo el nivel raíz configura el sistema).
3. El backend valida que el proveedor es alcanzable y que sus credenciales permiten alojar y recuperar archivos.
4. El backend establece el proveedor como destino activo a través de la librería de almacenamiento, de forma transparente para los demás roles (RN sobre transparencia, en geovial-storage).
5. El backend responde con el destino activo configurado, sin exponer las credenciales del proveedor.

## 5. Flujos alternativos

- 5.A Cambio de destino con fotos existentes. Disparador: ya hay fotos alojadas en el destino anterior. El backend establece el nuevo destino para las fotos futuras y mantiene accesibles las anteriores a través de la librería, sin que agentes ni jefes perciban diferencia. Retorna al paso 5.
- 5.B Validación previa sin aplicar. Disparador: el usuario raíz solicita validar un proveedor sin activarlo aún. El backend comprueba alcance y credenciales y reporta el resultado sin cambiar el destino activo. Termina con el resultado de validación.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| ROL_NO_AUTORIZADO | El solicitante no es usuario raíz | Rechaza con estado de prohibido y no cambia el destino |
| PROVEEDOR_NO_DISPONIBLE | El proveedor de destino no es alcanzable | Rechaza con estado de solicitud inválida y no activa el destino |
| CREDENCIALES_PROVEEDOR_INVALIDAS | Las credenciales del proveedor no permiten alojar ni recuperar archivos | Rechaza con estado de solicitud inválida y conserva el destino anterior |

## 7. Postcondiciones

- Éxito: el destino activo queda configurado para las fotos futuras, las fotos existentes permanecen accesibles y el cambio es transparente para los demás roles.
- Fallo: el destino activo no cambia y se devuelve un problema con el código correspondiente, sin exponer credenciales.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un usuario raíz autenticado y un proveedor de almacenamiento válido | Establece ese proveedor como destino activo | El backend activa el destino y responde sin exponer las credenciales del proveedor |
| CA-02 | Un usuario con rol jefe de área | Intenta cambiar el destino de almacenamiento | El backend rechaza con el código ROL_NO_AUTORIZADO |
| CA-03 | Un destino con fotos ya alojadas y un nuevo proveedor configurado | El usuario raíz cambia el destino | El backend aloja las fotos futuras en el nuevo destino y mantiene accesibles las anteriores, sin cambio percibido por agentes ni jefes |
| CA-04 | Un proveedor con credenciales inválidas | El usuario raíz intenta activarlo | El backend rechaza con el código CREDENCIALES_PROVEEDOR_INVALIDAS y conserva el destino anterior |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-07 |
| Reglas de negocio aplicables | RN-01 |
| Historias de usuario a generar | US-35, US-36 (en 06) |
| Componentes esperados | Recurso de configuración de almacenamiento; servicio de validación de proveedor; integración con la librería de almacenamiento (referencia tentativa a 05) |
| Tests previstos | Activación de destino por usuario raíz; cambio por rol no autorizado rechazado; transparencia del cambio para otros roles; credenciales inválidas rechazadas (en 08) |

## 10. Notas y supuestos

- La capacidad es Could Have (NB-07); se incorpora si la cadencia lo permite.
- La selección de proveedor y el detalle de cada destino los implementa la librería de almacenamiento; este CU especifica el contrato de configuración del backend que la gobierna.
- La transparencia del cambio para agentes y jefes es un criterio de éxito de NB-07: ningún otro rol percibe el destino activo.

## 12. Performance esperado del CU

- El cambio de destino debe completarse con una interrupción del servicio acotada (en el orden de a lo sumo una hora, según el criterio de NB-07), sin afectar la operación de campo en curso.

## 15. Idempotencia y reintento

- Establecer un destino ya activo deja la configuración sin cambios y responde con éxito.
- La validación previa de un proveedor es una operación segura y repetible sin efectos sobre el destino activo.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de configuración del destino de almacenamiento, derivado de NB-07 (F-17). |
