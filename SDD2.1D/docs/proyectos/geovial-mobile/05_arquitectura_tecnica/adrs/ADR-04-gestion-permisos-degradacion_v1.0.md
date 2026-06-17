# ADR-04 — Gestión de permisos del sistema operativo con degradación

**Proyecto:** geovial-mobile
**Documento:** ADR-04-gestion-permisos-degradacion_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto Móvil
**Categoría:** Seguridad

## 1. Contexto

La captura de campo necesita recursos del dispositivo gobernados por permisos del sistema operativo: ubicación/GPS para centrar el mapa y resolver coordenadas (CU-03, CU-04), cámara para capturar fotos (CU-04) y acceso a archivos o galería para la carga manual (CU-07). El sistema operativo puede no conceder un permiso o revocarlo en cualquier momento, y el dispositivo puede no tener señal de GPS o no contar con espacio de almacenamiento. La app debe seguir siendo usable y nunca inventar datos: la regla RN-01 prohíbe inventar coordenadas y RN-05 exige que la captura offline no se pierda. Los CU definen las degradaciones esperadas: fijación manual del pin sin permiso de ubicación, bloqueo explicado de cámara y galería sin sus permisos, foto pendiente de ubicación sin señal de GPS y aviso sin pérdida cuando no hay espacio. Cubre CU-03, CU-04, CU-07 y, en el arranque, CU-01.

## 2. Decisión

Se adopta una política única de gestión de permisos centralizada en los adaptadores de plataforma: cada permiso (ubicación/GPS, cámara, acceso a archivos) se solicita en el momento del primer uso, se chequea antes de cada operación que lo requiere y, ante denegación o revocación, la app degrada de forma explícita sin caer, conforme a las degradaciones definidas en los CU:

- Ubicación/GPS denegada o revocada: la app degrada a fijación manual del pin en el mapa, sin centrar por GPS (CU-03).
- Sin señal de GPS al capturar: la foto se conserva y se marca como pendiente de ubicación, sin coordenada inventada (CU-04, RN-01); el marcador del entorno se fija manualmente.
- Cámara denegada: la app no abre la cámara y explica que el permiso es necesario (CU-04).
- Acceso a archivos o galería denegado: la app no accede a las fotos y explica que el permiso es necesario (CU-07).
- Espacio de almacenamiento insuficiente: la app no guarda el binario de la foto y avisa al agente que libere espacio, sin perder lo ya encolado (CU-04, ADR-02).

## 3. Estado

Aceptado el 2026-06-15. Derivado de los CU-03, CU-04 y CU-07 y de las reglas RN-01 y RN-05; el tipo de proyecto exige una decisión de gestión de permisos (regla 05 §2.2).

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Permisos en el primer uso con degradación explícita (elegida) | El agente concede el permiso en contexto; la app sigue usable sin él; no inventa datos (RN-01) | Requiere mantener un camino degradado por cada permiso y probarlos por separado |
| Solicitar todos los permisos al inicio, bloqueante | Un solo punto de solicitud | Pide permisos fuera de contexto; si se niegan, bloquea funciones sin necesidad; mala experiencia de campo |
| Asumir permisos concedidos y fallar si faltan | Implementación mínima | La app cae o queda en estado inconsistente ante denegación o revocación; viola la usabilidad de campo |
| Inventar coordenada aproximada sin señal de GPS | La foto siempre tiene coordenada | Contradice RN-01 (no inventar) y degrada la calidad de georreferenciación (riesgo de negocio) |

## 5. Consecuencias positivas

1. La app permanece usable aunque se nieguen o revoquen permisos: cada función degrada de forma definida sin caer.
2. Nunca se inventan coordenadas: las fotos sin ubicación quedan pendientes y ubicables luego (RN-01).
3. La captura offline no se pierde ante falta de espacio: se avisa y se conserva lo encolado (RN-05, ADR-02).
4. Las degradaciones se prueban por separado, una por permiso (intake §17.P.6).

## 6. Consecuencias negativas y trade-offs

1. Cada permiso suma un camino degradado a mantener y probar; se acepta a cambio de la usabilidad de campo.
2. La fijación manual del pin exige una interacción adicional del agente cuando no hay GPS; se acepta para no inventar datos.
3. La solicitud en el primer uso puede interrumpir un gesto de captura; se acepta por solicitar en contexto y no fuera de él.

## 7. Implementación

- Los adaptadores de plataforma (ubicación, cámara, archivos) encapsulan la solicitud y el chequeo de permiso y exponen el estado a los servicios de captura; ninguna vista solicita permisos directamente.
- El servicio de captura consulta el estado del permiso antes de cada operación y enruta a la degradación correspondiente según el catálogo de §2.
- La foto pendiente de ubicación se persiste como `FotoLocal` sin coordenada y se puede ubicar luego en el mapa (CU-04, RN-01).
- La detección de espacio insuficiente impide persistir el binario y emite el aviso; la entidad y su cambio encolado no se crean si el binario no se pudo guardar (atomicidad local, ADR-02).
- Las condiciones de degradación se registran en el log local sin volcar datos sensibles (cross-cutting de `arquitectura-solucion_v1.0.md`).

## 8. Métricas de validación

- Con permiso de ubicación denegado, la creación de marcador degrada a fijación manual sin caída (CU-03, 08).
- Con cámara denegada, la app no abre la cámara y muestra el mensaje accionable (CU-04, 08).
- Con galería denegada, la carga manual no accede a las fotos y explica el permiso (CU-07, 08).
- Sin señal de GPS, la foto queda pendiente de ubicación sin coordenada inventada (CU-04, RN-01, 08).
- Sin espacio, no se guarda el binario y se conserva lo encolado (CU-04, RN-05, 08).

## 9. Referencias

- NB-03; CU-03, CU-04, CU-07, CU-01; RN-01, RN-05.
- Intake §11 (riesgo de georreferenciación imprecisa), §17.P.6; regla 05 §2.2 (permisos obligatorios para el tipo).
- ADRs relacionadas: ADR-02 (espacio y atomicidad), ADR-01 (adaptadores de plataforma).
- `arquitectura-solucion_v1.0.md`; `flujo-ejecucion_v1.0.md`.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión de gestión de permisos: solicitud en el primer uso, chequeo previo a cada operación y degradación explícita por permiso (ubicación/GPS, cámara, archivos) y por falta de señal o espacio, sin inventar datos. Aceptada (derivada de CU-03/04/07 y RN-01/05). |
