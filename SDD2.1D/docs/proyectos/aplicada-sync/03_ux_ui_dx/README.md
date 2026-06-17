# UX / UI / DX — aplicada-sync (guía de la sección)

Esta sección reúne los artefactos de la categoría 03 del proyecto `aplicada-sync`, el motor de sincronización redistribuible para aplicaciones móviles. El proyecto es de tipo `library`, por lo que se aplica la variante DX (reglas de la categoría 03, §1.2 y §2.2): la superficie pública es código, contrato, mensajes de error y documentación, y el consumidor es un developer integrador. No hay variante UX/UI ni wireframes, porque la librería no tiene interfaz visible a una persona usuaria final.

## Variante aplicada

Variante DX. Justificación: `aplicada-sync` es de tipo `library` y se consume por código; el foco está en la superficie pública (CU-01 a CU-06 de la categoría 02), su documentación y sus ejemplos. La experiencia de la persona usuaria final pertenece a la aplicación host que integra la librería, no a la librería.

## Artefactos vigentes

| Artefacto | Variante | Propósito | Estado |
| --- | --- | --- | --- |
| [dx-developer-experience_v1.0.md](dx-developer-experience_v1.0.md) | DX | Marco DX del motor: audiencia integrador, onboarding por tramos 5/30/60 verificables, quick-start, plan Diátaxis y métricas TTFS/TTFV. | Propuesto |
| [guia-onboarding-developer_v1.0.md](guia-onboarding-developer_v1.0.md) | DX | Recorrido de la primera hora del integrador: prerrequisitos, acceso, primer ejemplo, diagnóstico y próximos pasos por modo Diátaxis. | Propuesto |
| [dx-error-messages_v1.0.md](dx-error-messages_v1.0.md) | DX | Catálogo de errores del motor (conectividad, sincronización interrumpida o parcial, conflicto tolerado, cola pendiente) con código, causa probable y acción sugerida. | Propuesto |
| [dx-portal-developers_v1.0.md](dx-portal-developers_v1.0.md) | DX | Especificación del portal de documentación del paquete público: estructura Diátaxis, páginas obligatorias, accesibilidad WCAG 2.2 AA y métricas de uso. | Propuesto |

## DX docs obligatorios para el tipo library

Las reglas de la categoría 03 (§2.2) exigen para `library` tres DX docs obligatorios; los tres están presentes:

- `dx-developer-experience` — presente.
- `guia-onboarding-developer` — presente.
- `dx-error-messages` — presente.

## DX doc recomendado incluido

- `dx-portal-developers` — incluido. La tabla maestra (§2.1) lo recomienda para `library con portal hospedado`. Se genera porque el proyecto es redistribuible con repositorio público y vocación de reutilización fuera de la solución (intake §13, §17.P.7, §17.P.11): el portal aloja los cuatro modos Diátaxis del paquete. La especificación adapta las páginas y los ejemplos al carácter de una librería offline-first (sin sandbox hospedado y con una página Status de compatibilidad, no de salud de servicio). No se registra omisión: el documento se incluye.

## Documentos no aplicables (omisiones justificadas)

- `experiencia-de-uso`, `wireframes-<superficie>`, `representacion-<concepto>`, `glosario-ux`: no aplican. Son artefactos de la variante UX/UI para tipos con interfaz visible a la persona usuaria final; `aplicada-sync` es una librería sin UI propia. El vocabulario técnico del motor (sesión de sincronización, cambio local, cola de pendientes, estado reanudable, elemento en conflicto) ya está definido en la especificación funcional de la categoría 02 y se reutiliza sin duplicar, por lo que no se crea un `glosario-ux` propio.
- `dx-operability`: no aplica. Es el artefacto del operador para `worker-service`; `aplicada-sync` es una librería integrada en la aplicación host, no un servicio operado.

## Orden de lectura sugerido

1. `dx-developer-experience_v1.0.md` — el marco: audiencia, onboarding, quick-start, Diátaxis y métricas.
2. `guia-onboarding-developer_v1.0.md` — el recorrido de la primera hora (modo tutorial).
3. `dx-error-messages_v1.0.md` — el catálogo de errores para diagnosticar (modo reference de errores).
4. `dx-portal-developers_v1.0.md` — cómo se aloja todo lo anterior en el portal público.

## Trazabilidad

- Upstream: la persona objetivo es el developer integrador derivado de la visión de producto (00, audiencia del implementador y de la aplicación de campo que integra la librería). La superficie pública documentada son los CU-01 a CU-06 y las RN-01 a RN-03 de la categoría 02 del proyecto `aplicada-sync`.
- Downstream: cada DX doc alimenta 06 con las US a generar (US-01 a US-13 según la matriz NB→CU→RN→US de 02) y 08 con los tests de orden, idempotencia, convivencia con conflicto, reanudación y verificación reproducible del quick-start. El código ejecutable de los quick-starts y ejemplos vive en 11; el stack y el transporte, en 05.

## Convenciones

- Idioma rioplatense técnico con tildes y eñes en el cuerpo; nombres de archivo en ASCII. Codificación UTF-8 con fin de línea LF. Fechas en formato YYYY-MM-DD.
- Nomenclatura `<nombre>_v1.0.md` con guion bajo antes de `v` (nunca `.v`) y slug en minúsculas kebab-case.
- Vocabulario neutral y agnóstico del dominio de la solución que consume la librería: paquete distribuible, almacén local del host, backend remoto, sesión de sincronización, cambio local. No se mencionan stacks, productos comerciales ni protocolos concretos, que viven en la categoría 05.

## Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | README inicial de la sección 03 de aplicada-sync (variante DX): cuatro DX docs (tres obligatorios más el portal recomendado), omisiones justificadas de los artefactos UX/UI y de operability, orden de lectura y trazabilidad upstream/downstream. |
