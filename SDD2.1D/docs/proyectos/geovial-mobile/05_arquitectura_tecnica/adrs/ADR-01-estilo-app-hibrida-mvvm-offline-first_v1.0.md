# ADR-01 — Estilo de app híbrida con patrón de presentación MVVM y diseño offline-first

**Proyecto:** geovial-mobile
**Documento:** ADR-01-estilo-app-hibrida-mvvm-offline-first_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto Móvil
**Categoría:** Estilo

## 1. Contexto

`geovial-mobile` es la app de campo del agente de relevamiento. Su trabajo ocurre donde no se puede asumir conectividad: la captura de observaciones georreferenciadas debe ser posible 100 % sin conexión y sincronizarse después (NB-04, F-07, RN-05). La app debe presentar un mapa con marcadores, capturar fotos con resolución de coordenadas, registrar comentarios y etiquetas, y orquestar un ciclo de sincronización que delega en la librería de sincronización. El intake fija el estilo como decisión pre-tomada: app móvil híbrida con vistas embebidas y un patrón de presentación, sobre una plataforma móvil única (intake §17.P.2, §17.P.11, §17.P.9). Se necesita un estilo que (a) habilite la captura offline-first, (b) separe la lógica de presentación de las vistas para poder probar el modo offline y la sincronización sin interfaz (intake §17.P.6) y (c) consuma el motor de sincronización por contrato sin reimplementarlo (intake §14). Cubre CU-01 a CU-07.

## 2. Decisión

Se adopta una aplicación móvil híbrida de plataforma única, con vistas embebidas hospedadas en un contenedor nativo y un patrón de presentación de tipo MVVM (modelos de vista que median entre las vistas y los servicios de aplicación), bajo un diseño offline-first. La app se estructura en capas locales con dependencias hacia adentro: presentación (vistas y modelos de vista), aplicación (servicios de sesión, captura y orquestación de sincronización), dominio local (entidades replicadas y reglas de cola) e infraestructura (almacén local, adaptadores de plataforma y cliente del contrato REST). El almacén local es la fuente de trabajo durante la captura; el dominio autoritativo de la API prevalece al sincronizar.

## 3. Estado

Aceptado el 2026-06-15. Decisión pre-tomada en el intake (§17.P.2, §17.P.11).

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| App híbrida + MVVM offline-first (elegida) | Habilita captura 100 % offline; separa lógica y vista para probar sin interfaz; reúsa el motor de sincronización por contrato; un solo modelo de desarrollo para vistas embebidas | Suma la complejidad del modelo offline-first y del contenedor de vistas embebidas (trade-off aceptado, intake §17.P.12) |
| App nativa sin patrón de presentación | Acceso directo a la plataforma | Acopla la lógica de captura a las vistas; dificulta probar el modo offline y la sincronización por separado (intake §17.P.6); contradice la decisión pre-tomada |
| Cliente delgado online dependiente de red | Sincronización trivial; sin almacén local | Imposible trabajar sin conectividad; contradice NB-04, F-07 y RN-05 |
| Front embebido servido por red | Reúso del front web | Requiere conectividad para servir las vistas; incompatible con offline-first |

## 5. Consecuencias positivas

1. La captura de campo funciona 100 % sin conexión sobre el almacén local (RN-05, NFR de captura offline).
2. El patrón de presentación aísla la lógica de las vistas, habilitando pruebas de la lógica y del modo offline sin interfaz (intake §17.P.6).
3. La orquestación de sincronización se delega en la librería consumida, sin reimplementar el motor (intake §14).
4. Las capas con dependencias hacia adentro permiten sustituir adaptadores de plataforma (ubicación, cámara, archivos, almacenamiento seguro) sin tocar la lógica.

## 6. Consecuencias negativas y trade-offs

1. El modelo offline-first agrega complejidad de almacén local, cola y convivencia con conflictos; se acepta para habilitar el trabajo sin conectividad (intake §17.P.12).
2. El contenedor de vistas embebidas añade una capa de hospedaje entre la plataforma y la interfaz; se acepta por el modelo de desarrollo unificado.
3. La separación en capas impone disciplina de puertos y adaptadores que un cliente trivial no necesitaría; se acepta por la testabilidad.

## 7. Implementación

- La presentación se compone de vistas embebidas y modelos de vista; cada pantalla crítica (mapa, captura de foto, comentarios y etiquetas, sincronización) tiene su modelo de vista verificable sin interfaz.
- Los servicios de aplicación (sesión, captura, sincronización) definen puertos que la infraestructura implementa: repositorio del almacén local, adaptadores de ubicación, cámara, archivos y almacenamiento seguro, adaptador de la librería de sincronización y cliente del contrato REST.
- El componente de mapa se integra en la presentación de captura y emite eventos de posición y de gesto sobre el pin.
- Convención impuesta: ninguna lógica de captura ni de sincronización vive en las vistas; las vistas solo enlazan a su modelo de vista.
- El detalle de componentes y vistas vive en `arquitectura-solucion_v1.0.md`.

## 8. Métricas de validación

- La captura de una observación con foto se completa 100 % sin conexión (NFR captura offline, verificado en 08 sobre CU-04).
- Arranque en frío ≤ 3 s hasta la pantalla de sesión en el dispositivo de referencia (NFR de arranque, intake §17.P.10).
- La lógica de captura y de sincronización se prueba sin interfaz, con cobertura de la capa de lógica ≥ 75 % (intake §17.P.6).

## 9. Referencias

- NB-03, NB-04; CU-01 a CU-07; RN-05.
- Intake §14, §17.P.2, §17.P.6, §17.P.9, §17.P.10, §17.P.11, §17.P.12.
- ADRs relacionadas: ADR-02 (persistencia local), ADR-03 (sincronización), ADR-04 (permisos), ADR-05 (autenticación).
- `arquitectura-solucion_v1.0.md`; `flujo-ejecucion_v1.0.md`.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión de estilo: app híbrida de plataforma única con patrón de presentación MVVM y diseño offline-first, en capas locales con dependencias hacia adentro. Aceptada (pre-tomada en intake §17.P.2, §17.P.11). |
