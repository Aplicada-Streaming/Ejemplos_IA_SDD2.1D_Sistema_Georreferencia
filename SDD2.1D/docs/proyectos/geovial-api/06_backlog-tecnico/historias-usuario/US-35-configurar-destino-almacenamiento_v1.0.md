# US-35 — Configurar el destino de almacenamiento activo por el usuario raíz

**Proyecto:** geovial-api
**Documento:** US-35-configurar-destino-almacenamiento_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-07 Configuración de almacenamiento
**Prioridad MoSCoW:** Could
**Estimación:** 5 SP (Fibonacci)

## 1. Historia

Como usuario raíz, quiero configurar y consultar el destino de almacenamiento de archivos activo, para elegir dónde se guardan las fotos sin tocar el resto del sistema.

## 2. Contexto

CU-17 describe la configuración del destino de almacenamiento de archivos, reservada al usuario raíz. NB-07 enmarca la configuración de almacenamiento como capacidad Could Have de la API. ADR-09 resuelve delegar el guardado de archivos en la abstracción de almacenamiento, de modo que cambiar el destino activo no afecte al resto del sistema.

## 3. Criterios de aceptación

- Given un usuario raíz, When activa un proveedor de almacenamiento y luego consulta la configuración, Then el sistema registra el activo y la consulta lo muestra sin revelar las credenciales.
- Given un rol que no es usuario raíz, When intenta configurar el destino de almacenamiento, Then el sistema responde con el código ROL_NO_AUTORIZADO y no cambia el activo.
- Given credenciales de proveedor inválidas, When el usuario raíz intenta activar el destino, Then el sistema responde con el código CREDENCIALES_PROVEEDOR_INVALIDAS y conserva el destino vigente.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-07 |
| CU cubiertos | CU-17 |
| BT derivadas | BT-15 |
| Tests previstos | acceptance/AT-35-configurar-almacenamiento; contract test de PUT y GET de configuración de almacenamiento |

## 5. Prioridad y estimación

Could porque la elección del destino de almacenamiento es una capacidad de configuración de NB-07 que aporta flexibilidad operativa, pero el sistema puede funcionar con un destino por defecto; no bloquea el ciclo central. 5 SP por Planning Poker (Fibonacci): combina alta y consulta de configuración, restricción de rol al usuario raíz, validación de credenciales y la regla de que las credenciales entran pero no salen.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-17)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-15)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

Las credenciales del proveedor entran en la configuración pero nunca salen en la consulta. La validación previa de un destino sin activarlo se detalla en US-36; el guardado real de archivos queda delegado en la abstracción de almacenamiento según ADR-09.
