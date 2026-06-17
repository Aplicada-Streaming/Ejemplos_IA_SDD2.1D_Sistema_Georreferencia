# ADR-03 — Sincronización por consumo del motor subir-luego-bajar con convivencia de conflictos y reanudación

**Proyecto:** geovial-mobile
**Documento:** ADR-03-sincronizacion-motor-subir-luego-bajar_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto Móvil
**Categoría:** Comunicación

## 1. Contexto

El agente captura sin conexión y sincroniza al recuperar la red. La sincronización debe (a) subir primero los cambios locales y solo después bajar las actualizaciones del relevamiento asignado (RN-02), (b) convivir con los marcadores en conflicto como estado válido, sin bloquear la recolección, difiriendo la resolución al cierre desde la web (RN-03), (c) reanudar sin pérdida ni duplicación tras un corte, conservando la cola (RN-02, RN-05) y (d) detectar la conectividad para disparar el ciclo automáticamente, con disparo manual también disponible. El intake fija que la sincronización se delega en la librería de sincronización (intake §14, §17.P.3, §17.P.11): la detección de conectividad, el orden subir-antes-de-bajar, la idempotencia y la reanudación son responsabilidad de esa librería; la app especifica su consumo, no su mecánica interna (02 §2, CU-06). Cubre CU-06 y, en la actualización del contexto, CU-02.

## 2. Decisión

Se adopta la sincronización por consumo del motor de la librería de sincronización, no por implementación propia. La app integra la librería implementando los puertos que esta requiere (referencia al almacén local, referencia al backend remoto y proveedor de credencial) e invoca sus operaciones del ciclo de vida: inicializar la sesión, encolar cada cambio local con su identificador de origen estable, ejecutar el ciclo subir-luego-bajar, habilitar el disparo automático ante recuperación de conectividad, consultar el estado y la cola de pendientes, y reanudar una subida parcial desde el punto de corte. El orden subir-antes-de-bajar (RN-02), la no duplicación por identificador (idempotencia) y el reporte no bloqueante de los elementos en conflicto (RN-03) son garantías del contrato del motor, no opciones configurables; la app las consume tal cual. La app fija la versión mayor del contrato de sincronización y del contrato REST que consume.

## 3. Estado

Aceptado el 2026-06-15. Decisión pre-tomada en el intake (§17.P.3, §17.P.11) y derivada de RN-02, RN-03 y RN-05.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Consumir el motor de la librería de sincronización (elegida) | Reúsa el motor por contrato; orden, idempotencia, reanudación y convivencia son garantías del motor; alineado con el reúso pedido (intake §14) | Acopla la app a la superficie pública del contrato; un cambio mayor del contrato obliga a actualizar |
| Reimplementar la sincronización en la app | Sin dependencia del paquete | Duplica un motor complejo; contradice el reúso del intake; reintroduce el riesgo de pérdida/duplicación que el motor ya mitiga |
| Sincronización bidireccional simultánea con merge automático | Un solo paso | El merge automático decide conflictos sin criterio del jefe; contradice RN-03 (convivencia y resolución al cierre) |
| Bajar antes de subir | El cliente recibe novedades primero | El servidor sobrescribe cambios locales no enviados; contradice RN-02 y materializa el riesgo de pérdida |

## 5. Consecuencias positivas

1. El orden subir-antes-de-bajar y la no duplicación por identificador son garantías del motor, no código de la app (RN-02, idempotencia).
2. La app convive con los marcadores en conflicto sin abortar el ciclo: se reportan en el resumen y en la consulta de estado, y se resuelven al cierre desde la web (RN-03).
3. La reanudación tras un corte conserva la cola y no pierde ni duplica cambios; la marca solo avanza al confirmar (RN-02, RN-05).
4. El disparo automático ante recuperación de conectividad reduce la latencia entre el fin de la recolección y la disponibilidad de los datos (métrica de negocio de §8 del intake).

## 6. Consecuencias negativas y trade-offs

1. La app queda acoplada a la superficie pública del contrato de sincronización; un cambio mayor del contrato obliga a actualizar la integración. Se mitiga fijando la versión mayor y por la política de compatibilidad hacia atrás del contrato.
2. La mecánica interna (detección de conectividad, esquema de metadatos, transporte) vive en la librería; la app no la controla. Se acepta a cambio del reúso y de la verificación post-publicación del paquete.
3. La app debe proveer correctamente los puertos (almacén local, backend remoto, credencial); un defecto de integración se manifiesta como error de configuración del motor. Se mitiga con pruebas de contrato del consumo.

## 7. Implementación

- El adaptador de la librería de sincronización implementa los puertos requeridos: referencia al almacén local (ADR-02), referencia al backend remoto sobre el cliente del contrato REST y proveedor de credencial sobre el almacenamiento seguro (ADR-05).
- En el arranque de la sesión, la app inicializa el motor con la configuración de sesión (identificador de host, almacén local, backend remoto y proveedor de credencial).
- Cada captura encola su cambio con identificador de origen estable y orden de creación (ADR-02); el servicio de sincronización ejecuta el ciclo ante la señal de conectividad o por acción manual.
- La consulta de estado alimenta la presentación de sincronización: situación del motor, tamaño de cola, marca, conflictos conocidos y progreso parcial.
- La reanudación continúa la subida parcial desde el punto de corte; los reenvíos se reconocen como ya recibidos por el identificador de origen.
- El pipeline paso a paso vive en `flujo-ejecucion_v1.0.md`. El contrato consumido vive en `proyectos/aplicada-sync/05_arquitectura_tecnica/contratos-abstractions_v1.0.md`; los endpoints de subida y bajada del backend, en `proyectos/geovial-api/05_arquitectura_tecnica/contratos-rest_v1.0.md`.

## 8. Métricas de validación

- Un lote de 100 cambios completa el ciclo subir-luego-bajar en ≤ 30 s en red móvil típica (NFR de tiempo de ciclo, 08).
- La cola tolera ≥ 1000 cambios pendientes (NFR de capacidad, junto con ADR-02).
- Un corte durante la subida reanuda sin pérdida ni duplicación: los reenvíos se reconocen por identificador de origen (NFR de reanudación, 08, sobre CU-06).
- La bajada no se atiende antes de concluir la subida del mismo ciclo (RN-02, verificado en 08).
- Un marcador en conflicto se sincroniza y se reporta sin abortar el ciclo (RN-03, 08).

## 9. Referencias

- NB-04; CU-06, CU-02; RN-02, RN-03, RN-05.
- Intake §14, §17.P.3, §17.P.10, §17.P.11.
- Contrato consumido: `proyectos/aplicada-sync/05_arquitectura_tecnica/contratos-abstractions_v1.0.md` (operaciones inicializar sesión, encolar cambio, ejecutar sincronización, habilitar disparo automático, consultar estado, reanudar).
- Endpoints del backend: `proyectos/geovial-api/05_arquitectura_tecnica/contratos-rest_v1.0.md` (subida y bajada de sincronización).
- ADRs relacionadas: ADR-01 (estilo), ADR-02 (cola persistente), ADR-05 (credencial del motor).
- `flujo-ejecucion_v1.0.md`.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión de comunicación: sincronización por consumo del motor de la librería de sincronización con ciclo subir-luego-bajar, idempotencia por identificador de origen, convivencia con conflictos y reanudación; la app fija la versión mayor de los contratos consumidos. Aceptada (pre-tomada en intake §14, §17.P.3, §17.P.11). |
