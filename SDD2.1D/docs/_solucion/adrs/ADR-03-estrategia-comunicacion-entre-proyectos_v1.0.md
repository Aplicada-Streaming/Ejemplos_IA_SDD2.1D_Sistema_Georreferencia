# ADR-03 — Estrategia de comunicación entre proyectos

**Proyecto:** GeoVial (solución)
**Documento:** ADR-03-estrategia-comunicacion-entre-proyectos_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Soluciones Senior
**Categoría:** Comunicación

## 1. Contexto

Las cuatro aristas del grafo del manifiesto materializan dos clases de comunicación distintas. Dos aristas cruzan la red (cliente a backend): `geovial-web → geovial-api` y `geovial-mobile → geovial-api`; ambas exigen autenticación y un contrato versionado, porque los clientes son procesos separados que consumen el backend por red (intake §14). Las otras dos aristas son integraciones en proceso: `geovial-api → geovial-storage` (el backend invoca la librería de almacenamiento en su mismo proceso) y `geovial-mobile → aplicada-sync` (la app integra el motor de sincronización como paquete y lo invoca en proceso); ninguna de las dos tiene contrato de red propio.

El intake §14 fija que los clientes consumen el contrato REST autenticado con token bearer del backend, y que la app móvil sincroniza a través del contrato de la librería de sincronización contra los endpoints de sincronización del backend. La estrategia de comunicación de nivel solución debe declarar qué mecanismo gobierna cada clase de arista, sin reescribir el detalle de cada contrato (que vive en el `contratos-<area>` del productor).

## 2. Decisión

La comunicación de la solución usa dos mecanismos según la clase de frontera. Las fronteras cliente-backend se comunican por contrato REST sobre HTTP con payloads JSON, autenticado con token bearer presentado en la cabecera de autorización en toda operación salvo el inicio de sesión, y versionado por prefijo de versión mayor en la ruta. Las fronteras de integración en proceso (backend con librería de almacenamiento, app con motor de sincronización) se comunican por invocación directa de una superficie pública de abstracciones inyectada por inversión de dependencia, sin transporte de red propio. La sincronización del cliente móvil es un caso compuesto: el motor de sincronización (contrato de sincronización, en proceso) orquesta el ciclo subir-luego-bajar y, a través de un puerto de transporte que el host implementa, consume los endpoints REST de sincronización del backend (contrato REST, por red).

## 3. Estado

Aceptado el 2026-06-15. Gobierna las cuatro aristas del grafo del manifiesto; se referencia desde `vista-solucion_v1.0.md` §4 y `contratos-inter-proyecto_v1.0.md`.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| REST autenticado por red + abstracciones en proceso (elegida) | Coincide con la naturaleza de cada frontera (clientes separados vs. librerías integradas); contrato versionado para los clientes; sin overhead de red para las librerías | Dos disciplinas de contrato que sostener; el contrato de sincronización compone ambos mecanismos |
| Todo por red (también las librerías expuestas como servicios) | Aislamiento de fallas de cada librería | Introduce latencia y operación de red donde no se necesita; contradice que las librerías son in-process (intake §13); el redistribuible perdería su valor de integración directa |
| Comunicación asíncrona por eventos entre proyectos | Desacopla productor y consumidor en el tiempo | Sobredimensionado para un dominio de consistencia inmediata operado por un equipo de una persona; no hay requisito de mensajería |
| Sesión por cookie en vez de token bearer | Familiar en clientes web | No sirve al cliente móvil offline-first ni a la reutilización del token por la librería de sincronización; el intake fija token bearer (§14) |

## 5. Consecuencias positivas

1. Cada frontera usa el mecanismo acorde a su naturaleza: red autenticada para los clientes, invocación en proceso para las librerías, sin pagar costos que no corresponden.
2. El token bearer único habilita tanto las llamadas directas de los clientes como la reutilización del token por el motor de sincronización dentro de la app móvil.
3. El contrato REST versionado protege a los dos clientes de cambios incompatibles; las abstracciones en proceso aíslan al backend del proveedor de almacenamiento y a la app del transporte concreto de sincronización.
4. La estrategia es coherente con la composición de ADR-01 y con la política de versionado de ADR-02.

## 6. Consecuencias negativas y trade-offs

1. La app móvil compone dos contratos para sincronizar (el de sincronización en proceso y el REST por red); un cambio en cualquiera de los dos puede afectar el ciclo. Se acepta porque cada contrato versiona su compatibilidad por separado.
2. La autenticación por token bearer obliga a custodiar el token de forma segura en cada cliente (del lado servidor del circuito en el front, en el almacén seguro del dispositivo en el móvil). Se acepta como requisito de seguridad (intake §17.P.5 de cada cliente).
3. Las librerías en proceso no aíslan fallas como lo haría un servicio de red: un defecto se manifiesta dentro del host. Se acepta a cambio de menor latencia y complejidad (heredado de ADR-01).

## 7. Implementación

- Contrato REST cliente-backend: definido por el productor en `geovial-api` `contratos-rest_v1.0.md` (recursos, operaciones, esquemas, errores problem+json, versionado por URI). Los consumidores `geovial-web` y `geovial-mobile` lo consumen a través de su Cliente de API; el token bearer se presenta en la cabecera de autorización.
- Contrato de almacenamiento en proceso: definido por el productor en `geovial-storage` `contratos-abstractions_v1.0.md` (dos interfaces, operaciones asincrónicas, taxonomía de errores uniforme). El backend lo consume tras su puerto de almacenamiento por inversión de dependencia.
- Contrato de sincronización en proceso: definido por el productor en `aplicada-sync` `contratos-abstractions_v1.0.md` (operaciones del ciclo de vida, formas de datos, garantías de orden e idempotencia). La app móvil lo consume y le inyecta los puertos de almacén local, transporte hacia el backend y proveedor de credencial.
- Composición de la sincronización: el motor consume el contrato REST de sincronización del backend (subida y bajada) a través del puerto de transporte que la app implementa; el orden subir-luego-bajar es garantía del motor y del backend (ambos lo declaran).

## 8. Métricas de validación

- 100 % de las operaciones REST (salvo inicio de sesión) rechazadas sin token bearer válido (pruebas de autorización en 08 del backend).
- El ciclo de sincronización de la app reutiliza el token de la sesión sin volver a pedir credenciales en sesión activa (pruebas de sincronización en 08 del móvil).
- Las librerías se invocan en proceso sin abrir un canal de red propio (verificable por revisión de dependencias: no hay endpoint de red en `geovial-storage` ni en `aplicada-sync`).

## 9. Referencias

- Manifiesto: `SOLUTION-MANIFEST-geovial_v1.0.md` §3 (aristas del grafo).
- Intake: `SOLUTION-INTAKE-geovial_v1.0.md` §14 (contratos inter-proyecto), §17.P.3 y §17.P.5 de cada proyecto.
- Contratos de los productores: `proyectos/geovial-api/.../contratos-rest_v1.0.md`; `proyectos/geovial-storage/.../contratos-abstractions_v1.0.md`; `proyectos/aplicada-sync/.../contratos-abstractions_v1.0.md`.
- ADRs de nivel solución relacionados: ADR-01 (estilo de composición), ADR-02 (versionado inter-proyecto).
- Vista de solución: `vista-solucion_v1.0.md` §4; detalle en `contratos-inter-proyecto_v1.0.md`.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Estrategia inicial de comunicación entre proyectos: REST autenticado por token bearer y versionado por URI para las fronteras cliente-backend; abstracciones invocadas en proceso por inversión de dependencia para las librerías; sincronización como composición de ambos mecanismos. Para ADR aceptadas, la única edición permitida es el cambio de estado a Superado por ADR-YY. |
