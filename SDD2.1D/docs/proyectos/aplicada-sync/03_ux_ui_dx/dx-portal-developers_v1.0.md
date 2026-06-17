# DX — Portal de developers de aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** dx-portal-developers_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** DX Lead
**Variante:** DX

## 0. Superficie pública que documenta y motivo de inclusión

`aplicada-sync` es un paquete distribuible (`redistribuible: true`) con repositorio público y vocación de reutilización fuera de la solución (intake §13, §17.P.7, §17.P.11). Por ese carácter público, este documento especifica el portal de documentación del paquete que aloja, en un sitio navegable, los cuatro modos Diátaxis definidos en `dx-developer-experience_v1.0.md` §4. El portal documenta la misma superficie pública del motor: el contrato del ciclo de vida (CU-01 a CU-06) y las invariantes RN-01, RN-02 y RN-03 de la categoría 02. El stack del portal y su materialización viven en la categoría 05; este documento define el qué y el cómo de la experiencia, no la tecnología.

## 1. Audiencia y objetivos del portal

Audiencia: developer integrador (audiencia primaria, ver `dx-developer-experience_v1.0.md` §1) y, en segundo plano, el contributor que evalúa el motor para reutilizarlo en otro proyecto.

Objetivos del portal:

- Que el integrador alcance el primer éxito (inicializar la sesión, CU-01) en cinco minutos desde la landing, sin salir del portal (alineado con el TTFS de `dx-developer-experience_v1.0.md` §6).
- Que el integrador encuentre la respuesta a una tarea concreta (how-to) o el contrato de una operación (reference) en pocos clics, sin tener que leer el motor por dentro.
- Que el contributor comprenda las invariantes del motor (RN-01, RN-02, RN-03) y la política de compatibilidad de la versión pública (02 §8) antes de adoptar o extender la librería.
- Que cualquier código de error devuelto por el motor sea buscable y resoluble desde el portal (catálogo `dx-error-messages_v1.0.md`).

## 2. Estructura de información según Diátaxis

El portal organiza su contenido en los cuatro modos, sin mezclarlos en una misma página, replicando el plan de `dx-developer-experience_v1.0.md` §4:

| Sección del portal | Modo Diátaxis | Contenido | Origen |
| --- | --- | --- | --- |
| Aprender | Tutorial | Recorrido de primera hora del integrador. | `guia-onboarding-developer_v1.0.md`; sample `01-basico` (11). |
| Cómo hacer | How-to | Tareas concretas: habilitar disparo automático, reanudar tras un corte, consultar cola y conflictos, operar el modo no autenticado. | CU-04, CU-06, CU-05, CU-01 flujo 5.B. |
| Referencia | Reference | Contrato de cada operación de la superficie pública con sus flujos alternativos y sus códigos de error. | CU-01 a CU-06; catálogo de errores. |
| Explicación | Explanation | Por qué el orden no es configurable, por qué la idempotencia descansa en el identificador estable, por qué el motor convive con el conflicto. | RN-01, RN-02, RN-03. |

Cada página declara su modo de forma visible para que el integrador sepa si está aprendiendo, resolviendo una tarea, consultando o comprendiendo.

## 3. Navegación principal y búsqueda

- Navegación principal persistente con las cuatro entradas Diátaxis (Aprender, Cómo hacer, Referencia, Explicación) más Inicio (landing), Changelog y Estado.
- Búsqueda global indexada sobre todo el contenido del portal, con resultados que indican el modo Diátaxis de cada acierto. Los códigos de error son términos buscables de primera clase: buscar un código (por ejemplo, SUBIDA_INCOMPLETA) lleva a su entrada en el catálogo y a la operación que lo produce.
- Navegación contextual entre modos: cada página de tutorial enlaza al how-to y al reference relacionados; cada reference enlaza al catálogo de errores y a la explanation de la invariante que lo gobierna (coherente con los enlaces entre modos de `dx-developer-experience_v1.0.md` §4).
- Migas de pan que ubican la página dentro de su modo y de la jerarquía de la superficie pública.

## 4. Páginas obligatorias

| Página | Propósito | Contenido mínimo |
| --- | --- | --- |
| Landing | Presentar el motor y el primer paso. | Qué resuelve el motor (sincronización offline-first sin pérdida ni duplicación), enlace directo al quick-start y a la sección Aprender. |
| Quick-start | Llevar al primer éxito en cinco pasos o menos. | Los pasos del quick-start de `dx-developer-experience_v1.0.md` §3, descritos en comportamiento, con enlace al sample `01-basico` (11). |
| Reference | Documentar el contrato completo. | Las seis operaciones (CU-01 a CU-06) con entradas, contrato de retorno, flujos alternativos y códigos de error enlazados al catálogo. |
| Changelog | Comunicar la evolución de la versión pública. | Historial de versiones del paquete según SemVer; marca explícita de los cambios incompatibles que incrementan la versión mayor (02 §8). |
| Status | Informar el estado de la documentación y la compatibilidad. | Versión vigente del paquete, versión de la documentación, matriz de compatibilidad de la superficie pública y enlace a la política de versionado. |

El portal documenta una librería distribuible, no un servicio en línea: la página Status informa el estado de la documentación y de la compatibilidad de la versión pública, no la disponibilidad de un endpoint. No se promete un panel de salud de un servicio que la librería no opera.

## 5. Ejemplos ejecutables y sandbox

- Los ejemplos ejecutables del portal enlazan a los samples de la categoría 11 (`01-basico`, `02-intermedio`, `03-avanzado` con la demostración ajena al sistema del intake §18), no duplican código en el portal. El portal muestra el recorrido en pasos y comportamiento, y enlaza al código real reproducible.
- No se ofrece un sandbox interactivo en línea para un paquete que se integra en el dispositivo del host y trabaja sin conexión: el "sandbox" del integrador es el sample `01-basico`, ejecutable en su propio entorno y reproducible en cinco pasos o menos (intake §18). El portal lo declara explícitamente para no prometer un entorno hospedado que no aplica a una librería offline-first.
- Cada ejemplo declara su tramo de onboarding (5/30/60 minutos) y su resultado verificable, en línea con `dx-developer-experience_v1.0.md` §2.

## 6. Accesibilidad del portal (WCAG 2.2 AA)

El portal toma WCAG 2.2 nivel AA como piso mínimo de accesibilidad. Criterios prioritarios para un sitio de documentación técnica:

- Contraste de texto de al menos 4.5:1 en cuerpo y de 3:1 en texto grande y componentes de interfaz.
- Foco visible en todos los controles de navegación y en los enlaces, con orden de foco lógico.
- Navegación completa por teclado, incluida la búsqueda global y la navegación entre modos.
- Estructura semántica con encabezados jerárquicos correctos, puntos de referencia (landmarks) y un enlace para saltar al contenido principal.
- Alternativas textuales para todo diagrama de flujo del motor; los diagramas no son el único medio para comprender el ciclo subir-luego-bajar.
- Bloques de código y tablas legibles por lectores de pantalla, con encabezados de columna asociados.
- Texto redimensionable hasta el 200 % sin pérdida de contenido ni de funcionalidad.

## 7. Métricas de uso del portal

| Métrica | Definición | Objetivo | Cómo se mide |
| --- | --- | --- | --- |
| Tiempo a quick-start | Tiempo desde la landing hasta completar la página de quick-start. | <= 5 minutos | Telemetría opt-in del portal, alineada con el TTFS (`dx-developer-experience_v1.0.md` §6). |
| Tasa de búsqueda exitosa | Porcentaje de búsquedas que terminan en una página visitada (no en una nueva búsqueda ni en abandono). | >= 80 % | Analítica de búsqueda del portal. |
| Resolución de errores por código | Porcentaje de visitas a una entrada del catálogo de errores que no derivan en una consulta de soporte. | >= 80 % | Correlación entre visitas al catálogo y consultas abiertas en el canal de feedback. |
| Cobertura de la superficie pública | Porcentaje de operaciones (CU-01 a CU-06) con tutorial, how-to o reference publicado. | 100 % | Auditoría del portal contra el catálogo de CU de la categoría 02. |

La telemetría del portal es opt-in y con consentimiento, coherente con el feedback loop del marco DX (`dx-developer-experience_v1.0.md` §7); nunca recoge la carga útil de dominio del host, que es opaca para el motor.

## 8. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Especificación inicial del portal de developers de aplicada-sync: objetivos, estructura de información Diátaxis, navegación y búsqueda con códigos de error buscables, páginas obligatorias (landing, quick-start, reference, changelog, status) adaptadas a una librería offline-first, ejemplos enlazados a 11 sin sandbox hospedado, accesibilidad WCAG 2.2 AA y métricas de uso. Derivada del marco DX, de la superficie pública de 02 y del SOLUTION-INTAKE §13, §17 y §18. |
