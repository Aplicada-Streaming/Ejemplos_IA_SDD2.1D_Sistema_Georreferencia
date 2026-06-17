# CU-01 — Iniciar sesión, deslogueo completo y relogueo por seguridad del dispositivo

**Proyecto:** geovial-mobile
**Documento:** CU-01-iniciar-cerrar-sesion-relogueo-dispositivo_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + Mobile UX Analyst

## 1. Propósito

Permitir que el agente de campo inicie sesión en la app móvil la primera vez con sus credenciales y conexión, mantenga la sesión activa para trabajar en terreno, cierre por completo la sesión para liberar un dispositivo compartido y, cuando la app se reinicia o el dispositivo se bloquea, vuelva a habilitarla mediante la seguridad del propio dispositivo sin reingresar credenciales. Resuelve la identidad del relevador en un equipo que puede ser usado por varias personas.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Agente de campo | Primario | Inicia sesión, cierra sesión completa y se reloguea por la seguridad del dispositivo |
| App móvil | Sistema | Obtiene y conserva el token bearer, gestiona el ciclo de sesión y solicita la verificación del dispositivo |
| Backend de autenticación | Sistema | Valida las credenciales y emite el token bearer en el inicio online |
| Seguridad del dispositivo | Sistema | Verifica la identidad por el mecanismo del sistema operativo (patrón, huella) en el relogueo |

## 3. Precondiciones

- La app está instalada en un dispositivo con la seguridad del sistema operativo configurada (patrón, huella o equivalente).
- Para el inicio inicial, el dispositivo tiene conexión a internet.
- El agente tiene un usuario habilitado en el sistema (alta previa por el jefe de área, fuera de este CU).

## 4. Flujo principal

1. El agente abre la app por primera vez en el dispositivo; la app no tiene sesión guardada y presenta el inicio de sesión.
2. El agente ingresa sus credenciales y confirma; la app las envía al backend de autenticación con conexión.
3. El backend valida las credenciales y devuelve un token bearer; la app lo guarda en el almacenamiento seguro del dispositivo, nunca en texto plano.
4. La app queda con sesión activa y habilita el trabajo de campo del agente.
5. Cuando la app se reinicia o el dispositivo se desbloquea con una sesión activa guardada, la app no pide credenciales: solicita al agente verificarse por la seguridad del dispositivo.
6. El agente se verifica por el mecanismo del sistema operativo; la app rehabilita el acceso reutilizando el token guardado.
7. Cuando el agente termina su jornada en un dispositivo compartido, ejecuta el deslogueo completo; la app borra el token y los datos de sesión del almacenamiento seguro y vuelve al inicio de sesión, dejando el equipo libre para otro usuario.

## 5. Flujos alternativos

- 5.A Token vencido en el relogueo. Disparador: al verificarse por la seguridad del dispositivo, el token guardado ya venció. La app exige un nuevo inicio de sesión online con credenciales y, una vez con conexión, retorna al paso 2.
- 5.B Inicio sin conexión la primera vez. Disparador: el agente intenta el inicio inicial sin red. La app informa que el primer inicio requiere conexión y no crea sesión; cuando hay red, retorna al paso 2.
- 5.C Cambio de usuario en el dispositivo. Disparador: el dispositivo tiene sesión de otro agente. El agente nuevo no puede reloguearse con la seguridad del dispositivo sobre una sesión ajena; primero se ejecuta el deslogueo completo (paso 7) y luego un inicio online (paso 2).

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| CREDENCIALES_INVALIDAS | El backend rechaza las credenciales en el inicio online | La app no crea sesión, no guarda token y solicita reingresar las credenciales |
| SIN_CONEXION_INICIO | No hay red para el inicio online inicial | La app informa que el primer inicio requiere conexión y no habilita el trabajo |
| VERIFICACION_DISPOSITIVO_FALLIDA | La seguridad del dispositivo no verifica al agente en el relogueo | La app mantiene el acceso bloqueado y permite reintentar la verificación o cerrar sesión completa |
| DISPOSITIVO_SIN_SEGURIDAD | El dispositivo no tiene configurada la seguridad del sistema operativo | La app advierte que el relogueo por seguridad del dispositivo no está disponible y exige inicio online en cada reanudación |

## 7. Postcondiciones

- Éxito en inicio: existe una sesión activa con el token bearer guardado en el almacenamiento seguro del dispositivo y el trabajo de campo habilitado.
- Éxito en relogueo: el acceso queda rehabilitado reutilizando el token guardado, sin reingreso de credenciales.
- Éxito en deslogueo: el token y los datos de sesión quedan borrados del dispositivo y la app vuelve al inicio de sesión, sin datos del agente anterior visibles.
- Fallo: no se crea ni se rehabilita la sesión y el trabajo de campo permanece deshabilitado.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Una app recién instalada, con conexión, y un agente con usuario habilitado | El agente ingresa credenciales válidas y confirma | La app obtiene el token bearer, lo guarda en el almacenamiento seguro del dispositivo y habilita el trabajo de campo |
| CA-02 | Una sesión activa guardada y la app reiniciada | El agente abre la app y se verifica por la seguridad del dispositivo (huella) | La app rehabilita el acceso reutilizando el token, sin pedir credenciales |
| CA-03 | Un dispositivo con la sesión activa del agente A | El agente A ejecuta el deslogueo completo | La app borra el token y los datos de sesión y muestra el inicio de sesión sin datos del agente A |
| CA-04 | Una app recién instalada sin conexión | El agente intenta el inicio inicial | La app responde con SIN_CONEXION_INICIO y no crea sesión |
| CA-05 | Un dispositivo con la sesión activa del agente A | El agente B intenta reloguearse por la seguridad del dispositivo | La app no le da acceso sobre la sesión ajena y exige deslogueo completo antes de un nuevo inicio online |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-01 |
| Reglas de negocio aplicables | RN-04 |
| Historias de usuario a generar | US-01, US-02 (en 06) |
| Componentes esperados | Pantalla de inicio de sesión; servicio de sesión y almacenamiento seguro del token; integración con la seguridad del sistema operativo; cliente del contrato de autenticación del backend (referencia tentativa a 05) |
| Tests previstos | Inicio online guarda token en almacén seguro; relogueo por seguridad del dispositivo sin credenciales; deslogueo borra sesión; inicio sin conexión rechazado; cambio de usuario exige deslogueo (en 08) |

## 10. Notas y supuestos

- El token se almacena en el almacenamiento seguro del dispositivo provisto por la plataforma, nunca en texto plano (alineado con intake §17 P.5 de geovial-mobile).
- El backend es la única fuente del token; la app no lo emite ni lo renueva por sí misma. El inicio y el cierre de sesión del lado servidor pertenecen a geovial-api (CU-03 de geovial-api).
- El relogueo por seguridad del dispositivo es una revalidación local de identidad sobre una sesión ya iniciada; no reemplaza la autenticación contra el backend, solo evita reingresar credenciales mientras el token sea válido.
- La duración exacta de validez del token y el comportamiento ante token vencido durante el trabajo de campo se rigen por el contrato del backend; aquí se asume que un token vencido fuerza un nuevo inicio online, a confirmar con el negocio (alineado con geovial-api 02 §9).

## 14. Permisos del sistema operativo

- La app requiere el permiso de uso de la seguridad del dispositivo (autenticación biométrica o patrón) para el relogueo; si el usuario no lo concede o el dispositivo no tiene seguridad configurada, se aplica 5.A o DISPOSITIVO_SIN_SEGURIDAD.
- No requiere ubicación, cámara ni almacenamiento para este CU; esos permisos se solicitan en los CU de captura.

## 12. Performance esperado del CU

- El arranque en frío de la app hasta presentar el inicio de sesión o la verificación del dispositivo se mantiene dentro del objetivo de arranque (menor o igual a 3 s en el dispositivo de referencia, según el NFR del proyecto).

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de inicio, deslogueo completo y relogueo por seguridad del dispositivo, derivado de NB-01 (F-08). |
