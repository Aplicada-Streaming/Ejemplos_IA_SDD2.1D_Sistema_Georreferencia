# Wireframe — Pantalla de inicio de sesión y relogueo

**Proyecto:** geovial-mobile
**Documento:** wireframes-pantalla-login-relogueo_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Mobile UX Designer + Accessibility Specialist
**Variante:** UX/UI

## 1. Pantalla y propósito

Superficie de entrada de la app de campo. El agente inicia sesión por primera vez con credenciales y conexión, rehabilita una sesión activa mediante la seguridad del dispositivo sin reingresar credenciales, y cierra sesión por completo para liberar un equipo compartido. Resuelve la identidad del relevador antes de habilitar cualquier trabajo de campo. CU origen: CU-01.

## 2. Layout

Pantalla en portrait, una sola columna, con la acción primaria en la zona alcanzable con el pulgar. Tiene dos modos según haya o no una sesión activa guardada.

Modo A — Primer inicio (sin sesión guardada):

```
+------------------------------------------+
|                [ marca app ]             |
|                                          |
|        Iniciar sesion en GeoVial         |
|                                          |
|  Usuario                                 |
|  [____________________________________]  |
|                                          |
|  Credencial                              |
|  [______________________________] [ojo]  |
|                                          |
|  ( ! ) <area de mensaje inline>          |
|                                          |
|                                          |
|        [   I N I C I A R   S E S I O N ]  |  <- accion primaria, grande
|                                          |
|  El primer inicio necesita conexion      |
|  por unica vez.                          |
+------------------------------------------+
```

Modo B — Relogueo (sesion activa guardada):

```
+------------------------------------------+
|                [ marca app ]             |
|                                          |
|        Hola de nuevo, <usuario>          |
|                                          |
|        [   icono huella / patron   ]     |
|                                          |
|     Verificate con la seguridad de       |
|     tu dispositivo para continuar.       |
|                                          |
|        [  V E R I F I C A R M E       ]   |  <- accion primaria, grande
|                                          |
|  ( ! ) <area de mensaje inline>          |
|                                          |
|        [ Cerrar sesion completa ]        |  <- accion secundaria
+------------------------------------------+
```

El deslogueo completo se dispara desde el menú de la app durante la sesión y, como salida de bloqueo, desde el Modo B (acción secundaria). No es la acción primaria de esta pantalla.

## 3. Componentes principales

| Componente | Propósito | Datos que muestra | Comportamiento |
| --- | --- | --- | --- |
| Campo Usuario | Identificar al agente en el primer inicio | Texto ingresado | Etiqueta asociada; admite pegar; se conserva tras un fallo de servicio |
| Campo Credencial | Recibir la credencial en el primer inicio | Texto enmascarado | Control de mostrar/ocultar la credencial; etiqueta asociada |
| Acción primaria Iniciar sesión | Enviar credenciales al backend de autenticación | Rótulo de acción | Objetivo grande; se deshabilita mientras valida para evitar doble envío |
| Bloque de verificación del dispositivo | Disparar la verificación por la seguridad del dispositivo en el relogueo | Ícono y nombre del usuario de la sesión activa | Invoca la verificación del sistema operativo (huella, patrón); reutiliza el token guardado al verificar |
| Acción Cerrar sesión completa | Liberar el equipo borrando token y datos de sesión | Rótulo de acción | Confirmable; deja la app en Modo A sin datos del agente anterior |
| Área de mensaje inline | Comunicar errores y advertencias de sesión | Texto del mensaje (§8 del marco) | Aparece bajo el control afectado; nombre accesible anunciado por región de estado |

## 4. Interacciones

| Acción | Disparador | Resultado esperado | Precondición |
| --- | --- | --- | --- |
| Iniciar sesión | El agente toca Iniciar sesión con usuario y credencial | La app valida contra el backend, guarda el token en el almacenamiento seguro y habilita el trabajo de campo | App en Modo A, con conexión |
| Verificarse por el dispositivo | El agente toca Verificarme | La app invoca la seguridad del dispositivo y, al verificar, rehabilita el acceso reutilizando el token sin pedir credenciales | App en Modo B, con sesión activa guardada |
| Cerrar sesión completa | El agente confirma el deslogueo | La app borra token y datos de sesión y vuelve al Modo A sin datos del agente anterior | Existe una sesión activa |
| Mostrar u ocultar credencial | El agente toca el control de ojo | Alterna entre texto enmascarado y visible | App en Modo A |
| Reintentar verificación | El agente toca Verificarme tras un fallo | La app reintenta la verificación del dispositivo | Verificación previa fallida |

## 5. Estados

| Estado | Condición que lo produce | Representación esperada |
| --- | --- | --- |
| Vacío (Modo A) | App recién instalada sin sesión guardada | Formulario de credenciales limpio con la acción primaria |
| Vacío (Modo B) | Sesión activa guardada y app reabierta | Bloque de verificación del dispositivo con el nombre del usuario |
| Cargando | Validación de credenciales o verificación en curso | Indicador en la acción primaria, controles deshabilitados |
| Con datos | Sesión abierta o rehabilitada | Transición a la lista de relevamientos asignados |
| Sin conexión | Primer inicio sin red (SIN_CONEXION_INICIO) | Mensaje inline "El primer inicio necesita conexión por única vez"; no se crea sesión. El relogueo del Modo B opera sin red |
| Sincronizando | No aplica en esta superficie | La sesión no sincroniza datos del relevamiento; el estado de sincronización vive en sus pantallas |
| Error | CREDENCIALES_INVALIDAS, VERIFICACION_DISPOSITIVO_FALLIDA, DISPOSITIVO_SIN_SEGURIDAD | Mensaje inline accionable; en token vencido se exige nuevo inicio online (5.A); en sesión ajena se exige deslogueo previo (5.C) |

## 6. Versión móvil o responsive

Esta es una app de campo en portrait como orientación primaria; no hay versión de escritorio. Notas de adaptación:

- La pantalla se diseña para una sola mano: la acción primaria queda en la mitad inferior, al alcance del pulgar.
- En pantallas más altas, el espacio extra se reparte como aire por encima de la marca y por debajo de la acción; los controles no se estiran.
- En landscape (no primario), el formulario se mantiene en una columna centrada y el teclado del sistema no debe tapar el campo en foco; la pantalla hace scroll si es necesario. No se exige rotar para completar el inicio (1.3.4).

## 7. Notas de implementación

- Accesibilidad: campos con etiqueta asociada y nombre accesible (1.3.1, 4.1.2); foco visible y no oscurecido por el teclado del sistema (2.4.7, 2.4.11); autenticación accesible apoyada en la seguridad del dispositivo, sin pruebas cognitivas, con opción de mostrar la credencial y de pegarla (3.3.8); mensajes inline anunciados por región de estado (4.1.3); objetivos táctiles grandes para uso con guantes (2.5.8).
- Performance percibida: arranque en frío hasta esta pantalla en pocos segundos (objetivo ≤ 3 s del proyecto); la acción primaria se deshabilita al enviar para evitar doble disparo.
- Internacionalización: rótulos y mensajes externalizados; el nombre del usuario y los rótulos toleran expansión sin truncar; los diálogos de la seguridad del dispositivo los presenta el sistema operativo en el idioma del dispositivo.
- Seguridad: el token se guarda en el almacenamiento seguro del dispositivo, nunca en texto plano; el deslogueo borra token y datos de sesión.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Persona objetivo | Agente de campo (00) |
| CU origen | CU-01 |
| Marco experiencia aplicado | experiencia-de-uso_v1.0.md §3.1, §4 (estados), §5 (accesibilidad), §8 (errores) |
| Reglas de negocio relevantes | RN-04 |
| US a generar | US-01, US-02 (en 06) |
| Tests previstos | Inicio online guarda token en almacén seguro; relogueo por seguridad del dispositivo sin credenciales; deslogueo borra sesión; inicio sin conexión rechazado (SIN_CONEXION_INICIO); cambio de usuario exige deslogueo (en 08) |

## 9. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Wireframe inicial de la pantalla de inicio de sesión y relogueo por seguridad del dispositivo, con modos primer inicio y relogueo, estados (incluido sin conexión y sin aplicación de sincronización), interacciones y trazabilidad a CU-01 y RN-04. |
