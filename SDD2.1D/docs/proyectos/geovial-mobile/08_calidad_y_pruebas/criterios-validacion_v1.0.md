# Criterios de validación — geovial-mobile

Proyecto: geovial-mobile
Documento: criterios-validacion_v1.0.md
Versión: 1.0
Estado: Propuesto
Fecha: 2026-06-15
Autor: Ingeniero QA / SDET (mobile)

## 1. Propósito

Define qué condiciones numéricas y verificables permiten declarar a `geovial-mobile` validado para release. Cada criterio se mide en el ambiente de prueba equivalente al productivo (dispositivo Android de referencia conectado por USB en modo desarrollador, P.8 y P.9; red móvil típica para el ciclo de sincronización). Un criterio no cumplido solo se acepta con ADR explícita y plan de remediación (§6). Este documento es consumido por la `definition-of-done_v1.0.md` en la DoD de release.

## 2. Criterios funcionales

- Los 7 CU (CU-01 a CU-07) están cubiertos por al menos un TC en verde, y cada criterio Given/When/Then de cada CU tiene su TC asociado en la matriz de cobertura §2 (regla 08 §1.3, validación con AG-02). Sin CU huérfano.
- Las 5 RN (RN-01 a RN-05) están verificadas por al menos un TC en verde (matriz §4): priorización de ubicación incrustada sin inventar coordenada (RN-01), orden subir-antes-de-bajar (RN-02), convivencia con conflictos (RN-03), relogueo por seguridad del dispositivo y custodia del token (RN-04) y captura offline con cola persistente sin pérdida (RN-05).
- El journey crítico de campo se demuestra de punta a punta sobre el dispositivo de referencia: ingreso, selección de relevamiento, creación/movimiento de marcador, captura de foto con coordenada, comentario y etiqueta, y un ciclo de sincronización completo.

## 3. Criterios no funcionales

Cada NFR cumple su SLA medido en el ambiente de referencia. Todos los NFR numéricos de P.10 tienen TC ejecutable (matriz §3).

| NFR | Objetivo numérico | Criterio de validación | TC |
| --- | --- | --- | --- |
| Captura offline | 100 % de la captura de una observación con foto funciona sin conexión | La captura de marcador y de foto se completa con el adaptador de conectividad en estado sin conexión, sin ninguna llamada de red bloqueante | TC-08, TC-12 |
| Capacidad de la cola local | ≥ 1000 cambios pendientes sin pérdida | La cola conserva un lote de al menos 1000 cambios en orden de creación, con identificador de origen único, sin pérdida y con la app operativa | TC-24 |
| Tiempo del ciclo de sincronización | Un lote de 100 cambios completa el ciclo ≤ 30 s en red móvil típica | El ciclo subir-luego-bajar de 100 cambios mide ≤ 30 s en el dispositivo de referencia en red móvil típica | TC-25 |
| Reanudación sin pérdida | El ciclo reanuda tras un corte sin pérdida ni duplicación | Tras un corte después de la primera confirmación, la cola conserva los pendientes y el reenvío no duplica (deduplicación por identificador de origen) | TC-19 |
| Arranque en frío | ≤ 3 s hasta la pantalla de sesión/verificación | El arranque en frío del proceso no residente llega a la pantalla de sesión/verificación en ≤ 3 s, incluyendo la aplicación de migraciones | TC-26 |

El proyecto no tiene SLO de disponibilidad ≥ 99,9 % ni objetivo de latencia p99 numérico (`tiene_observabilidad_critica = false`, P.10), por lo que no hay criterio de disponibilidad operativa que validar para el release.

## 4. Criterios de regresión

- La suite de regresión completa acumulada hasta el release está en verde.
- Ningún TC verde de una versión anterior pasó a rojo sin justificación documentada.
- Cada defecto blocker o crítico cerrado durante el desarrollo generó al menos un TC de regresión que lo previene (regla 08 §4.10).

## 5. Criterios de calidad de código

- Gate de cobertura global cumplido: líneas ≥ 80 %, branches ≥ 70 % sobre el proyecto (intake §17 P.6).
- Pisos por capa cumplidos por separado: lógica (Aplicación + Dominio local) ≥ 75 % de líneas, presentación ≥ 60 % de líneas (intake §17 P.6); infraestructura ≥ 70 % de líneas (estrategia-testing §2). El gate global no compensa una capa por debajo de su piso.
- Mutation score no se exige en `mobile-app-maui` (regla 08 §2.2 lo reserva a `library`).
- Análisis estático sin issues críticos nuevos; compilación sin warnings tratados como error.
- El snapshot de las pantallas críticas coincide con su baseline aprobado, sin diferencias no aprobadas (TC-27).

## 6. Excepciones documentadas

- Cualquier criterio funcional, no funcional o de calidad de código no cumplido se acepta solo con ADR explícita y plan de remediación (regla 08 §4.8); no se admite excepción silenciosa.
- Una capacidad Could Have (US-10, US-15) que no entre en el release no bloquea la validación del camino principal; se documenta como exclusión de alcance, no como excepción.
- Si un TC de NFR no puede medirse en dispositivo por demora del ciclo de distribución del paquete (riesgo del plan §4), su validación se difiere con plan de remediación declarado y el release no se declara validado hasta completarla; la cola ≥ 1000 (TC-24) admite verificación previa sin interfaz sobre almacén local efímero, pero el ciclo (TC-25) y el arranque (TC-26) exigen medición en dispositivo de referencia.
- Las ADR-01 a ADR-05 que gobiernan el proyecto deben estar ratificadas (estado Aceptado) para el release; una ADR aún en Propuesto que condicione un criterio se trata como excepción con plan de cierre.

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Criterios de validación iniciales de geovial-mobile: criterios funcionales (7 CU y 5 RN cubiertos y verdes, journey crítico demostrado), no funcionales con los NFR numéricos de P.10 y su TC (captura 100 % offline, cola ≥ 1000, ciclo de 100 cambios ≤ 30 s, reanudación sin pérdida, arranque ≤ 3 s), de regresión, y de calidad de código con gate global ≥ 80 % / ≥ 70 % reconciliado con los pisos por capa (lógica 75 / presentación 60). Excepciones solo con ADR explícita. |
