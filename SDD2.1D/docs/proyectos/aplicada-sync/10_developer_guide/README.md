# Developer guide — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** README.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-15
**Autor:** Technical Writer + SDK Documentation Lead (AG-10)
**Audiencia:** Developer integrador que consume la librería desde su propia aplicación

Índice navegable de la documentación de consumo del motor de sincronización `aplicada-sync`, un paquete distribuible y agnóstico del dominio que un host integra para propagar cambios locales a un backend remoto bajo la política subir-luego-bajar, sin perder ni duplicar datos ante cortes. Esta carpeta es la ventana del proyecto al integrador: siguiéndola, un developer que nunca vio el motor debería poder hacerlo funcionar sin pedir ayuda al equipo que lo construyó.

Este README es una tabla de contenidos viva; no duplica el contenido de los artículos.

## 1. Artefactos de la sección

| Documento | Tipo Diátaxis | Nivel | Para qué sirve |
| --- | --- | --- | --- |
| `conceptos-fundamentales_v1.0.md` | Explanation | Medio | Modelo mental del motor: ciclo, cola, sesión, conflicto; decisiones de diseño y qué NO hace. |
| `guia-onboarding-developer_v1.0.md` | Tutorial | Básico | De cero a integración en una hora: Hello world, primer caso real, integración. |
| `guia-integracion-aplicacion-movil_v1.0.md` | How-to | Medio | Integrar el motor en una aplicación host móvil genérica, paso a paso. |
| `referencia-api_v1.0.md` | Reference | Avanzado | Contrato exacto: operaciones, formas de datos, contratos de extensión, eventos y excepciones. |
| `troubleshooting_v1.0.md` | How-to (diagnóstico) | Medio | Diagnóstico paso a paso de las condiciones frecuentes (ISSUE-01 a ISSUE-07) y reporte de bugs. |
| `glosario-tecnico_v1.0.md` | Reference | Básico | Vocabulario canónico del consumidor con referencia cruzada. Fuente única de términos. |

## 2. Orden de lectura recomendado

1. `conceptos-fundamentales_v1.0.md` — para entender el modelo mental antes de tocar el código.
2. `guia-onboarding-developer_v1.0.md` — para llegar al primer ciclo exitoso en menos de una hora.
3. `guia-integracion-aplicacion-movil_v1.0.md` — cuando la tarea concreta es integrar en una app host móvil.
4. `referencia-api_v1.0.md` — para consultar la firma exacta de una operación, tipo o código de error.
5. `troubleshooting_v1.0.md` — cuando algo falla; cada entrada lleva un código `ISSUE-XX`.

`glosario-tecnico_v1.0.md` se consulta en cualquier momento: el resto de los documentos enlaza a él para los términos.

## 3. Prerequisitos para empezar

- El paquete distribuible incorporado al proyecto host desde el repositorio público de distribución.
- Una `estrategia-de-almacen-local` y una `estrategia-de-transporte` (obligatorias) que el host implementa e inyecta.
- Opcionalmente, un `proveedor-de-credencial` y una `fuente-de-conectividad` para el modo autenticado y el disparo automático.

No hace falta dominar patrones de sincronización offline-first: el orden, la idempotencia y la reanudación son garantías del motor. Detalle en `guia-onboarding-developer_v1.0.md` §1.

## 4. Quick-start

```text
1. Incorporar el paquete distribuible al proyecto host.
2. Implementar e inyectar las estrategias obligatorias: almacén local y transporte.
3. Inicializar la sesión con la configuración (host, almacén, transporte, credencial).
   -> Esperado: identificador de sesión no vacío y estado "listo".
4. Encolar un cambio local con un identificador estable.
   -> Esperado: tamaño de cola = 1; reencolar el mismo no la hace crecer.
5. Ejecutar la sincronización.
   -> Esperado: resumen con los subidos antes de cualquier bajado; cola en cero.
6. Consultar el estado para confirmar: "listo", 0 pendientes, marca de sincronización avanzada.
```

Recorrido completo, con el tramo de reanudación y disparo automático, en `guia-onboarding-developer_v1.0.md`.

## 5. Referencias cruzadas

- 05 `contratos-abstractions_v1.0.md` y `extensibilidad_v1.0.md`: superficie pública y puntos de extensión (paridad con la referencia).
- 02 casos de uso CU-01 a CU-06 y reglas RN-01 a RN-03: origen funcional del contrato.
- 08 `estrategia-testing_v1.0.md`: cómo el integrador ejercita el motor contra dobles.
- 11 samples (`01-basico`, `02-intermedio`, `03-avanzado-demo-maui`): código ejecutable que ilustra esta guía.
- 03 `dx-developer-experience_v1.0.md` y `dx-error-messages_v1.0.md`: marco DX y catálogo de errores previos.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Índice inicial de la categoría 10 de aplicada-sync: artefactos con nivel y tipo Diátaxis, orden de lectura, prerequisitos y quick-start de seis pasos. |
