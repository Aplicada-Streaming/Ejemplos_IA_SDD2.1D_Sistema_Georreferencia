# CU-04 — Detectar conectividad y disparar la sincronización

**Proyecto:** aplicada-sync
**Documento:** CU-04-detectar-conectividad-disparar-sync_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Permitir que el motor reaccione de forma automática a la recuperación de conectividad: cuando el detector de conectividad informa que hay red disponible, el motor dispara por sí solo un ciclo de sincronización subir-luego-bajar, sin que la aplicación host tenga que orquestar el momento. Habilita el comportamiento de sincronización automática.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Detector de conectividad | Primario | Emite eventos de cambio de conectividad que el motor observa |
| Aplicación host | Secundario | Habilita el disparo automático y recibe notificación del resultado |
| Backend remoto | Sistema | Destino del ciclo de sincronización disparado |

## 3. Precondiciones

- Existe una sesión de sincronización inicializada y autenticada (CU-01).
- La aplicación host habilitó el disparo automático ante recuperación de conectividad.
- El motor está suscripto a una fuente de eventos de conectividad provista por el host o por su plataforma.

## 4. Flujo principal

1. El detector de conectividad emite un evento de transición a estado con red disponible.
2. El motor recibe el evento y verifica que el disparo automático esté habilitado y que la sesión esté autenticada.
3. El motor comprueba que no haya un ciclo de sincronización ya en curso para la sesión.
4. El motor dispara internamente el ciclo de sincronización subir-luego-bajar (delegado en el flujo de CU-03).
5. Al concluir, el motor notifica a la aplicación host el resultado del ciclo: cambios subidos, actualizaciones bajadas y estado final.

## 5. Flujos alternativos

- 5.A Evento de pérdida de conectividad. Disparador: el detector emite una transición a estado sin red. El motor no dispara ciclo alguno; si hay un ciclo en curso, lo deja que finalice o se detenga por sus propias excepciones (CU-03). Termina sin disparar.
- 5.B Cola vacía al disparar. Disparador: hay red recuperada pero la cola de pendientes está vacía. El motor igualmente dispara el ciclo, que omite la subida y baja actualizaciones (flujo 5.A de CU-03). Retorna al paso 5.
- 5.C Rebote de conectividad. Disparador: llegan varios eventos de recuperación en una ventana breve mientras un ciclo ya está en curso. El motor ignora los eventos redundantes y no inicia ciclos paralelos. Termina sin disparar un segundo ciclo.

## 6. Excepciones y errores

| Código | Causa | Respuesta del motor |
| --- | --- | --- |
| DISPARO_AUTOMATICO_DESHABILITADO | Llega un evento de red disponible pero el host no habilitó el disparo automático | El motor no dispara el ciclo y registra el evento como ignorado |
| SESION_NO_AUTENTICADA | Hay red disponible pero la sesión no tiene credencial vigente | El motor no dispara el ciclo y notifica al host que se requiere credencial |
| FUENTE_CONECTIVIDAD_AUSENTE | El motor no tiene una fuente de eventos de conectividad suscripta | El motor no puede operar en modo automático y devuelve el error al habilitarlo |

## 7. Postcondiciones

- Éxito: ante una recuperación de conectividad válida, se ejecutó a lo sumo un ciclo de sincronización y el host fue notificado del resultado.
- Fallo o condición no apta: no se disparó ningún ciclo; el estado de la sesión y la cola quedaron inalterados por este CU.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Una sesión autenticada con disparo automático habilitado y 1 cambio pendiente | El detector emite un evento de red disponible | El motor dispara un ciclo de sincronización y notifica al host el resultado con 1 subido |
| CA-02 | Una sesión autenticada con disparo automático deshabilitado | El detector emite un evento de red disponible | El motor no dispara ningún ciclo y registra el evento como ignorado |
| CA-03 | Una sesión con disparo automático habilitado y un ciclo ya en curso | El detector emite dos eventos de red disponible en 1 segundo | El motor no inicia ciclos paralelos y mantiene un único ciclo activo |
| CA-04 | Una sesión no autenticada con disparo automático habilitado | El detector emite un evento de red disponible | El motor no dispara el ciclo y notifica al host que se requiere credencial con el código SESION_NO_AUTENTICADA |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-04 |
| Reglas de negocio aplicables | RN-01 |
| Historias de usuario a generar | US-08, US-09 (en 06) |
| Componentes esperados | Observador de conectividad; coordinador de sesión de sincronización (referencia tentativa a 05) |
| Tests previstos | Disparo ante red recuperada; no disparo con automático deshabilitado; no reentrada ante rebote de conectividad; no disparo sin credencial (en 08) |

## 10. Notas y supuestos

- El motor consume eventos de conectividad de una fuente que el host o la plataforma le proveen; no implementa la detección de red de bajo nivel.
- El ciclo disparado respeta la garantía de orden subir-antes-de-bajar (RN-01), igual que el disparado manualmente en CU-03.
- La librería evita disparos concurrentes: un único ciclo activo por sesión, sin importar cuántos eventos de conectividad lleguen.

## 17. Compatibilidad de versión pública

El contrato de habilitación del disparo automático y la notificación de resultado al host forman parte de la superficie pública. Cambiar el modelo de suscripción a eventos o la semántica de no concurrencia constituye un cambio incompatible y obliga a incrementar la versión mayor.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de detección de conectividad y disparo automático de la sincronización, derivado de NB-04 y del SOLUTION-INTAKE §17 (aplicada-sync). |
