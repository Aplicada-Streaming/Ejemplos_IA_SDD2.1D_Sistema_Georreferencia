# RN-04 — Relogueo por seguridad del dispositivo en sesión activa

**Proyecto:** geovial-mobile
**Documento:** RN-04-relogueo-por-seguridad-del-dispositivo_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + Mobile UX Analyst

## 1. Enunciado de la regla

Durante una sesión activa, cuando la app se reinicia o el dispositivo se desbloquea, la app rehabilita el acceso pidiendo verificación por la seguridad del propio dispositivo (patrón, huella o equivalente) sin reingreso de credenciales; el inicio inicial y el cambio de usuario sí exigen credenciales online, y el deslogueo completo borra el token y los datos de sesión del dispositivo.

## 2. Justificación

La operación de campo ocurre en dispositivos que pueden ser compartidos, donde hace falta proteger la sesión sin obligar al agente a reingresar credenciales en cada reanudación y, a la vez, poder liberar el equipo para otro usuario sin filtrar datos (NB-01, intake §1, §4 F-08, §17 P.5 de geovial-mobile). El token vive en el almacenamiento seguro del dispositivo, nunca en texto plano.

## 3. Ámbito de aplicación

Se evalúa en cada reanudación de la app con una sesión activa guardada (relogueo), en el inicio inicial sin sesión (credenciales online), en el cambio de usuario y en el deslogueo completo. Aplica a la gestión del ciclo de sesión y del token en el cliente móvil.

## 4. Consecuencia si se viola

Si la verificación por la seguridad del dispositivo falla, el acceso permanece bloqueado y se permite reintentar o cerrar sesión completa. Si el token guardado venció, el relogueo no alcanza y se exige un nuevo inicio online con credenciales. Un agente no puede reloguearse por la seguridad del dispositivo sobre la sesión de otro: primero debe ejecutarse el deslogueo completo. Conservar el token en texto plano o dejar datos del usuario anterior tras el deslogueo viola la regla.

## 5. CU afectados

CU-01 (iniciar sesión, deslogueo completo y relogueo por seguridad del dispositivo). De forma indirecta, CU-06, que ante un token rechazado solicita reloguear.

## 6. Pruebas que la verifican

- Reanudar con sesión activa exige verificación del dispositivo y no pide credenciales (08, sobre CU-01).
- El deslogueo completo borra el token y los datos de sesión y deja el equipo libre (08, sobre CU-01).
- El relogueo con token vencido exige nuevo inicio online (08, sobre CU-01).
- Un agente no puede reloguearse sobre la sesión de otro sin deslogueo previo (08, sobre CU-01).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la regla de relogueo por seguridad del dispositivo en sesión activa, derivada de NB-01 (F-08) y del intake §17 P.5 de geovial-mobile. |
