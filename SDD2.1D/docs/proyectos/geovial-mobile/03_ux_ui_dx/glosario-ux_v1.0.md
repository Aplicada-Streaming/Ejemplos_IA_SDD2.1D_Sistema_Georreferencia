# Glosario UX — geovial-mobile

**Proyecto:** geovial-mobile
**Documento:** glosario-ux_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Mobile UX Designer + Accessibility Specialist
**Variante:** UX/UI

## 0. Propósito y alcance

Vocabulario canónico de la experiencia de usuario de la app de campo `geovial-mobile`. Reúne los términos de interacción, estado y accesibilidad que aparecen en más de un artefacto de esta sección, para que el marco de experiencia, los cinco wireframes, 06 y 08 los usen con el mismo significado.

No se redefine el vocabulario del dominio, que ya vive en el glosario de la solución (visión §9) y en el glosario del modelo conceptual de 02. Términos como relevamiento, tramo vial, observación, marcador geográfico, conflicto de marcadores, radio de agrupación, sincronización, marca de sincronización, etiqueta, comentario, agente de campo, almacén local, copia local, cola local y cambio encolado se reutilizan de allí y solo se referencian cuando hace falta. Este glosario agrega únicamente los términos propios de la capa de experiencia móvil.

## 1. Términos de la experiencia móvil

| Término | Definición (en el contexto UX de esta app) | Origen / nota |
| --- | --- | --- |
| Contexto activo de captura | Relevamiento que el agente eligió y sobre el cual la app habilita crear marcadores, capturar fotos y registrar observaciones | Materializa la selección de CU-02 en la experiencia |
| Mapa de captura | Superficie central donde el agente se ubica, crea o mueve marcadores y dispara la captura de fotos | Wireframe wireframes-mapa-captura |
| Pin | Representación visual de un marcador geográfico sobre el componente de mapa | UX de CU-03; el concepto de dominio es "marcador" (02) |
| Hoja de acciones del marcador | Panel inferior que aparece al tocar un pin y ofrece capturar, ver observaciones, mover o etiquetar | Patrón de interacción del mapa de captura |
| Acción primaria de captura | Control grande y al alcance del pulgar que dispara la tarea principal de una pantalla (centrar por GPS, crear marcador, capturar foto, sincronizar) | Aplica la ley de Fitts del marco §2 |
| Centrar por GPS | Gesto que sitúa el mapa sobre la posición actual del agente tomada del proveedor de ubicación del dispositivo | UX de CU-03 |
| Fijación manual en el mapa | Alternativa de ubicar un marcador tocando un punto del mapa cuando no hay señal de GPS o se denegó el permiso de ubicación, sin inventar coordenada | Degradación de CU-03; nunca produce coordenada inventada |
| Pendiente de ubicación | Estado de una foto que quedó sin coordenada (sin señal de GPS o sin metadatos) y que el agente puede ubicar luego en el mapa | Estado del marco §4; CU-04 5.A y CU-07 5.A |
| Indicador de conectividad y sincronización | Elemento persistente que comunica si la app está sin conexión, con cambios en cola, sincronizando o al día | Patrón transversal del marco §9 |
| Estado sin conexión | Estado esperado y normal en el que la app opera sin red; la captura sigue disponible y los cambios se encolan | Modo normal offline-first; marco §4 |
| Estado sincronizando | Estado en el que un ciclo de sincronización corre en segundo plano, con progreso de la cola, sin bloquear la captura | Marco §4; CU-06 |
| Estado al día | Estado en el que la cola está vacía y la copia local refleja el último ciclo confirmado | Resumen de CU-06 |
| Cola visible | Presentación al agente de los cambios pendientes de subir, en orden, en la pantalla de estado de sincronización | UX de CU-06; el concepto de dominio es "cola local" (02) |
| Progreso por fases | Visualización del ciclo de sincronización que muestra primero la subida y luego la bajada, haciendo tangible el orden subir-luego-bajar | UX de RN-02; CU-06 |
| Resumen del ciclo | Cierre de un ciclo de sincronización con las cuentas de cambios subidos, actualizaciones bajadas y elementos en conflicto | UX de CU-06 |
| Marca de conflicto | Señal no bloqueante sobre un pin o en el resumen que indica que un marcador está en conflicto por radio | UX de RN-03; no habilita a resolver en la app |
| Modo lectura | Presentación de un relevamiento cerrado en la que se puede revisar pero no capturar ni editar | UX de RELEVAMIENTO_CERRADO en CU-02, CU-03, CU-04, CU-05, CU-07 |
| Relogueo por seguridad del dispositivo | Rehabilitación del acceso a una sesión activa mediante la verificación del sistema operativo (patrón, huella), sin reingresar credenciales | UX de CU-01; concepto de RN-04 |
| Deslogueo completo | Cierre de sesión que borra el token y los datos de sesión del dispositivo para liberar un equipo compartido | UX de CU-01; concepto de RN-04 |
| Microcopy de permiso | Texto breve propio de la app que justifica para qué necesita un permiso del sistema operativo antes o al momento de solicitarlo | Aplica a ubicación, cámara y almacenamiento (CU-03, CU-04, CU-07) |
| Guardado local optimista | Confirmación inmediata de que una captura quedó guardada en el dispositivo, mientras el encolado para sincronizar ocurre detrás | Técnica de performance percibida del marco §7 |
| Objetivo táctil | Área tocable de un control, dimensionada y separada para uso con guantes y bajo sol | Accesibilidad del marco §5 (WCAG 2.5.8) |

## 2. Términos de accesibilidad usados en la sección

| Término | Definición | Nota |
| --- | --- | --- |
| Región de estado | Zona que anuncia cambios dinámicos (guardado, conectividad, progreso de sincronización) a lectores de pantalla | WCAG 2.2 AA, criterio 4.1.3 |
| Nombre accesible | Texto que identifica un control, un pin o una foto para tecnologías de asistencia | WCAG 2.2 AA, criterios 1.3.1, 4.1.2 |
| Texto alternativo | Descripción accesible de una foto que expone su comentario, su etiqueta y su estado de ubicación | WCAG 2.2 AA, criterio 1.1.1 |
| Alternativa a gesto | Acción de un solo toque equivalente a un gesto (por ejemplo, Mover marcador como alternativa a arrastrar el pin) | WCAG 2.2 AA, criterio 2.5.1 |
| Foco no oscurecido | Garantía de que el elemento enfocado no queda tapado por el teclado del sistema o por una hoja inferior | WCAG 2.2 AA, criterio 2.4.11 (novedad 2.2) |
| Autenticación accesible | Verificación que no exige pruebas cognitivas ni transcripción; aquí se apoya en la seguridad del dispositivo | WCAG 2.2 AA, criterio 3.3.8 (novedad 2.2) |

## 3. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Glosario UX inicial de geovial-mobile: términos propios de la experiencia móvil (contexto activo de captura, mapa de captura, pin, hoja de acciones, fijación manual, pendiente de ubicación, indicador y estados de sincronización, modo lectura, relogueo y deslogueo, microcopy de permiso, guardado optimista, objetivo táctil) y términos de accesibilidad WCAG 2.2 AA usados en la sección. No duplica el vocabulario de dominio del glosario de 02 ni de la visión §9; lo referencia. |
