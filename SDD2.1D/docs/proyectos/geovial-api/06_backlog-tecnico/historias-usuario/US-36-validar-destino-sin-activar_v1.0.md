# US-36 — Validar un destino de almacenamiento sin activarlo

**Proyecto:** geovial-api
**Documento:** US-36-validar-destino-sin-activar_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + API Product Owner
**Épica:** EP-07 Configuración de almacenamiento
**Prioridad MoSCoW:** Could
**Estimación:** 3 SP (Fibonacci)

## 1. Historia

Como usuario raíz, quiero validar un proveedor de almacenamiento sin activarlo, para comprobar que el destino funciona antes de cambiar el activo.

## 2. Contexto

CU-17, en su flujo alternativo FA-02, prevé validar un destino de almacenamiento sin cambiar el activo. NB-07 enmarca la configuración de almacenamiento como capacidad Could Have de la API. ADR-09 resuelve delegar el acceso al destino en la abstracción de almacenamiento, lo que permite probar la conectividad sin comprometer la configuración vigente.

## 3. Criterios de aceptación

- Given un proveedor de almacenamiento accesible, When el usuario raíz solicita su validación, Then el sistema confirma que el destino funciona sin cambiar el activo.
- Given un proveedor que no responde, When el usuario raíz solicita su validación, Then el sistema responde con el código PROVEEDOR_NO_DISPONIBLE y no altera el activo.
- Given credenciales de proveedor inválidas, When el usuario raíz solicita la validación, Then el sistema responde con el código CREDENCIALES_PROVEEDOR_INVALIDAS y no altera el activo.

## 4. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| NB upstream | NB-07 |
| CU cubiertos | CU-17 |
| BT derivadas | BT-15 |
| Tests previstos | acceptance/AT-36-validar-almacenamiento; contract test de POST de validación de almacenamiento |

## 5. Prioridad y estimación

Could porque la validación previa acompaña a la configuración de almacenamiento de NB-07, que es Could Have; reduce el riesgo de activar un destino roto, pero no es imprescindible para el ciclo central. 3 SP por Planning Poker (Fibonacci): es una operación acotada de prueba de conectividad que no muta el estado activo, con dos códigos de rechazo estables bien definidos.

## 6. DoR check

- [x] Historia con valor explícito para el rol
- [x] CU relacionado identificado (CU-17)
- [x] Criterios de aceptación en Given/When/Then con happy path y edge case
- [x] Estimada en SP (Fibonacci)
- [x] Prioridad MoSCoW con justificación
- [x] Sin dependencias bloqueantes sin planificar (BT-15)
- [x] Insumos de prueba identificados

## 7. Notas y supuestos

La validación es repetible y segura: no modifica el destino activo ni deja efectos persistentes. La activación efectiva del destino, junto con su consulta sin revelar credenciales, se detalla en US-35.
