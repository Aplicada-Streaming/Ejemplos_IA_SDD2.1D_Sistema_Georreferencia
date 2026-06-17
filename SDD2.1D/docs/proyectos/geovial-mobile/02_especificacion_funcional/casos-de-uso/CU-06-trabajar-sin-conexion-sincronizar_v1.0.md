# CU-06 — Trabajar sin conexión y sincronizar subiendo antes de bajar

**Proyecto:** geovial-mobile
**Documento:** CU-06-trabajar-sin-conexion-sincronizar_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + Mobile UX Analyst

## 1. Propósito

Permitir que el agente de campo recolecte sin conexión acumulando los cambios en una cola local y que la app, al detectar conexión, sincronice con el backend subiendo primero los cambios locales y bajando después las actualizaciones del relevamiento asignado, sin pérdidas ni duplicaciones y conviviendo con los conflictos. Es la capacidad que hace viable el trabajo de campo donde no hay red.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Agente de campo | Primario | Recolecta sin conexión y dispara o deja que se dispare la sincronización |
| App móvil | Sistema | Acumula los cambios en la cola local, detecta conexión y orquesta la sincronización a través de la librería de sincronización |
| Librería de sincronización | Sistema | Ejecuta el ciclo subir-luego-bajar, mantiene la marca de sincronización y la idempotencia |
| Backend de sincronización | Sistema | Recibe los cambios locales y entrega las actualizaciones del relevamiento |

## 3. Precondiciones

- El agente tiene una sesión activa con token bearer vigente (CU-01).
- Existe al menos un relevamiento asignado con copia local (CU-02).
- La cola local conserva, en orden de creación, los cambios capturados sin conexión (CU-03, CU-04, CU-05).

## 4. Flujo principal

1. El agente trabaja sin conexión; cada marcador, observación, foto, comentario y etiqueta se registra en el almacén local y se encola como cambio local pendiente.
2. La app detecta automáticamente que hay conexión disponible.
3. La app inicia la sincronización a través de la librería de sincronización sobre el relevamiento asignado.
4. Fase de subida: la librería envía la cola de cambios locales en su orden de creación; a medida que el backend confirma cada cambio, se retira de la cola y se registra el avance, reconociendo reenvíos por su identificador de origen sin duplicar.
5. La app verifica que la subida concluyó sin cambios pendientes confirmables restantes.
6. Fase de bajada: solo entonces la librería solicita al backend las actualizaciones posteriores a la última marca de sincronización del relevamiento y las aplica a la copia local, avanzando la marca.
7. La app deja el relevamiento sincronizado y muestra al agente el resumen del ciclo: cambios subidos, actualizaciones bajadas y elementos en conflicto.

## 5. Flujos alternativos

- 5.A Corte de conexión durante la subida (sincronización parcial). Disparador: la conexión se pierde tras confirmar parte de la cola. La librería deja confirmados los cambios ya subidos, conserva el resto en la cola, no inicia la bajada y deja la sesión reanudable; en la siguiente conexión retoma sin reaplicar lo confirmado. Retorna al paso 2 cuando vuelve la red.
- 5.B Actualizaciones en conflicto durante la bajada. Disparador: una o más actualizaciones bajadas corresponden a marcadores marcados en conflicto por el backend. La app las aplica a la copia local como estado válido en conflicto, no aborta el ciclo y las incluye en el resumen; la resolución se difiere al cierre desde la web (RN-03). Retorna al paso 7.
- 5.C Sincronización ya en curso. Disparador: el agente fuerza una sincronización mientras hay un ciclo activo para ese relevamiento. La app no inicia un segundo ciclo y muestra el estado del ciclo vigente. Termina sin efectos adicionales.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| BACKEND_INALCANZABLE | El backend no responde al iniciar o durante la subida | La app detiene el ciclo, conserva la cola de pendientes no confirmados, no inicia la bajada y deja el relevamiento reanudable |
| TOKEN_INVALIDO | El backend rechaza el token bearer del agente | La app detiene el ciclo sin subir ni bajar, no altera la cola y solicita reloguear (CU-01) |
| RELEVAMIENTO_CERRADO | El relevamiento fue cerrado por el jefe y no admite nuevas subidas | La app no sube los cambios locales pendientes, los conserva en la cola y avisa al agente que el relevamiento ya no admite cambios (ver Notas) |

## 7. Postcondiciones

- Éxito: la cola quedó vacía de los cambios confirmados, las actualizaciones se aplicaron a la copia local y la marca de sincronización avanzó; la bajada solo ocurrió tras concluir la subida.
- Éxito parcial: los cambios confirmados quedaron subidos, el resto permanece en la cola y el relevamiento queda reanudable, sin pérdida ni duplicación.
- Fallo: no se sube ni se baja nada, la cola se conserva intacta y el agente recibe la indicación correspondiente.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un agente con 5 cambios locales encolados y conexión recuperada | La app detecta conexión y sincroniza | La app sube primero los 5 cambios y solo después baja las actualizaciones, mostrando 5 subidos antes de cualquier bajada |
| CA-02 | Un ciclo con 3 pendientes donde la conexión se corta tras confirmar el primero | La app sincroniza | La app deja 1 confirmado, conserva 2 en la cola, no baja actualizaciones y deja el relevamiento reanudable sin duplicar |
| CA-03 | Una bajada que incluye un marcador en conflicto por radio | La app sincroniza | La app aplica la actualización en conflicto a la copia local sin abortar y la reporta como elemento en conflicto en el resumen |
| CA-04 | Un agente con cambios encolados cuyo token fue rechazado | La app sincroniza | La app responde con TOKEN_INVALIDO, conserva la cola intacta y solicita reloguear |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-04 |
| Reglas de negocio aplicables | RN-02, RN-03, RN-05 |
| Historias de usuario a generar | US-11, US-12, US-13 (en 06) |
| Componentes esperados | Cola local de cambios persistente y ordenada; detector de conectividad; integración con la librería de sincronización; aplicador de actualizaciones sobre la copia local (referencia tentativa a 05) |
| Tests previstos | Orden subir-antes-de-bajar verificado; corte en subida deja reanudable sin duplicar; convivencia con conflicto en la bajada; token rechazado conserva la cola (en 08) |

## 10. Notas y supuestos

- El orden subir-antes-de-bajar es una garantía dura aportada por la librería de sincronización y se refleja como RN-02; la bajada no se atiende hasta concluir la subida del ciclo.
- La idempotencia por identificador de origen asegura que un reenvío tras un corte no aplique un cambio dos veces; la app delega esta garantía a la librería de sincronización y al backend.
- La detección automática de conexión dispara el ciclo; el agente también puede forzarlo. La detección de conectividad y la ejecución del ciclo viven en la librería de sincronización (CU-04 y CU-03 de aplicada-sync).
- Los casos límite del intake (§7) figuran como pendientes de respuesta del cliente y se asumen explícitamente, a confirmar con el negocio (alineado con geovial-api 02 §9): la sincronización parcial por corte se asume reanudable e idempotente (5.A); el cierre del relevamiento mientras el agente tiene cambios sin sincronizar se asume como bloqueo de nuevas subidas con respuesta RELEVAMIENTO_CERRADO; los conflictos entre cambios de dos agentes se asumen con la misma política de convivencia y resolución al cierre que los conflictos por radio.

## 14. Permisos del sistema operativo

- Requiere acceso a la red de datos del sistema operativo para sincronizar; sin red, el trabajo continúa sin conexión y la sincronización se difiere.
- No requiere ubicación ni cámara; opera sobre la cola local y la conexión de datos.

## 12. Performance esperado del CU

- La captura permanece 100 % disponible sin conexión y la cola local tolera al menos 1000 cambios pendientes; un ciclo de sincronización de 100 cambios completa en 30 s o menos en red móvil típica y reanuda sin pérdida tras un corte, según los NFR del proyecto.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de trabajo sin conexión y sincronización subir-luego-bajar, derivado de NB-04 (F-07). |
