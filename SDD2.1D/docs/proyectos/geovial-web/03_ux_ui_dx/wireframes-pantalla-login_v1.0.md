# Wireframe — Pantalla de ingreso

**Proyecto:** geovial-web
**Documento:** wireframes-pantalla-login_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** UX/UI Designer + Frontend Lead
**Variante:** UX/UI

## 1. Pantalla y propósito

Pantalla de ingreso del front web. Es la puerta de entrada de los roles administradores (raíz, jefe general, jefe de área) y, por excepción, del agente asignado para la carga manual. El usuario entrega su identificador y su credencial y obtiene una sesión con las superficies de su rol; si las credenciales no validan, el rol no tiene acceso web o el servicio no responde, queda en esta pantalla con un motivo claro. CU origen: CU-01. Marco aplicado: `experiencia-de-uso_v1.0.md` (flujo 3.1, estados §4.2, errores §8).

## 2. Layout

Disposición centrada en columna única, sin navegación de aplicación (todavía no hay sesión). El bloque de marca queda arriba, el formulario al centro y el área de mensajes inmediatamente sobre o bajo el formulario según el alcance del error.

```text
+--------------------------------------------------------------+
|                                                              |
|                      [ Marca GeoVial ]                       |
|                  Herramienta de administración               |
|                                                              |
|     +--------------------------------------------------+     |
|     |  [ banner de servicio / sesión expirada ]        |     |  <- solo en error de circuito
|     +--------------------------------------------------+     |
|                                                              |
|     +--------------------------------------------------+     |
|     |  Identificador de acceso                         |     |
|     |  [____________________________________________]  |     |
|     |  ( mensaje inline de error de campo )            |     |
|     |                                                  |     |
|     |  Credencial                                      |     |
|     |  [____________________________________] [mostrar]|     |
|     |  ( mensaje inline de error de campo )            |     |
|     |                                                  |     |
|     |  ( aviso inline: credenciales / acceso revocado )|     |
|     |                                                  |     |
|     |             [      Ingresar      ]               |     |  <- acción primaria, destino amplio
|     +--------------------------------------------------+     |
|                                                              |
|                 Ayuda de acceso / contacto                   |
|                                                              |
+--------------------------------------------------------------+
```

Tras una sesión activa, el cierre de sesión devuelve siempre a este mismo layout, sin precargar el identificador anterior ni rastros de la identidad previa.

## 3. Componentes principales

| Componente | Propósito | Datos que muestra | Comportamiento |
| --- | --- | --- | --- |
| Bloque de marca | Ubicar al usuario en la herramienta | Nombre del producto y descriptor de la herramienta | Estático |
| Campo identificador de acceso | Capturar el identificador del usuario | Texto ingresado | Etiqueta asociada; foco inicial; conserva su valor ante fallo de servicio (no ante credenciales inválidas si la política lo exige) |
| Campo credencial | Capturar la credencial | Texto enmascarado | Etiqueta asociada; control de mostrar y ocultar; nunca se persiste ni se remuestra |
| Acción primaria Ingresar | Disparar el ingreso | Rótulo y estado de envío | Se deshabilita mientras valida para evitar reenvíos; destino amplio (Ley de Fitts) |
| Banner de circuito | Comunicar servicio no disponible o sesión expirada | Mensaje de §8 | Persistente y no intrusivo; ofrece reintento o reingreso |
| Aviso inline de acceso | Comunicar credenciales inválidas, usuario inhabilitado o rol sin acceso web | Mensaje de §8 | Aparece dentro del formulario; se anuncia a lector de pantalla |
| Enlace de ayuda y contacto | Orientar ante acceso revocado (handoff humano) | Texto de ayuda | Estático; visible siempre de forma consistente |

## 4. Interacciones

| Acción | Disparador | Resultado esperado | Precondición |
| --- | --- | --- | --- |
| Ingresar con credenciales válidas | Envío del formulario | Se abre la sesión y se redirige a la superficie inicial del rol | Usuario habilitado y backend alcanzable (CU-01 paso 3) |
| Mostrar u ocultar la credencial | Control de mostrar | Alterna el enmascarado del campo credencial | — |
| Reintentar tras fallo de servicio | Acción en el banner de circuito | Reenvía las credenciales conservando el identificador | Backend recupera alcance (CU-01 5.B) |
| Reingresar tras sesión expirada | Llegada redirigida desde otra superficie | Muestra esta pantalla con aviso de sesión expirada | Token vencido durante el uso (CU-01 5.A) |
| Volver con el botón de retroceso del navegador tras cerrar sesión | Navegación del navegador | No se recupera la sesión anterior; exige ingresar de nuevo (CU-01 CA-03) | Sesión cerrada |

## 5. Estados

| Estado | Condición que lo produce | Representación esperada |
| --- | --- | --- |
| Vacío | Primera carga o tras cierre de sesión | Formulario limpio, foco en el identificador, sin mensajes |
| Cargando | Validación de credenciales en curso | Acción primaria en estado de envío; campos y acción deshabilitados; indicación de validación |
| Con datos (éxito) | El backend valida y devuelve el rol | Transición a la superficie inicial del rol; confirmación sutil de bienvenida opcional |
| Error de validación de acceso | CREDENCIALES_INVALIDAS, USUARIO_INHABILITADO, ROL_SIN_ACCESO_WEB | Aviso inline en el formulario con el mensaje de §8; no se abre sesión; en rol sin acceso, cierre de la sesión recién abierta y retorno al ingreso |
| Error / sin conexión al circuito | El backend no responde (5.B) o la sesión expiró (5.A) | Banner persistente de servicio no disponible o sesión expirada; identificador conservado para reintentar |

## 6. Versión móvil o responsive

No es la superficie de uso primario (el contexto es escritorio de oficina), pero degrada con dignidad: la columna única se mantiene, el formulario ocupa el ancho disponible con márgenes, los campos y la acción primaria conservan el tamaño mínimo de objetivo y el banner de circuito se ancla arriba sin tapar los campos. No hay elementos que se oculten: la pantalla ya es mínima.

## 7. Notas de implementación

- Accesibilidad: etiquetas asociadas a cada campo (1.3.1, 3.3.2); foco inicial en el identificador; foco visible y no oscurecido por el banner (2.4.7, 2.4.11); los avisos de acceso y de circuito se anuncian por región de estado (4.1.3); la acción de mostrar credencial tiene nombre accesible.
- Performance percibida: respuesta inmediata al enviar mediante el estado de envío de la acción primaria; no se simula instantaneidad si el backend tarda.
- Internacionalización: rótulos y mensajes externalizados; el botón Ingresar y los avisos toleran expansión de texto sin truncar.
- Entrada redundante (3.3.7): tras un fallo de servicio no se pide reingresar el identificador.
- Seguridad de experiencia: la credencial nunca se remuestra ni se conserva; el cierre de sesión no deja rastros recuperables por navegación del navegador.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Persona objetivo | Jefe de área, jefe general, usuario raíz (00); agente solo en la excepción de carga manual |
| CU origen | CU-01 (iniciar y cerrar sesión en el front web) |
| Reglas de negocio relevantes | RN-01 (visibilidad por rol), RN-03 (acceso restringido a roles administradores) |
| Marco de experiencia aplicado | experiencia-de-uso_v1.0.md (flujo 3.1, estados §4.2, errores §8) |
| US a generar | US-01, US-02 (06) |
| Tests previstos | Ingreso válido habilita el rol; credenciales inválidas sin sesión; cierre no recupera la sesión; rol sin acceso web rechazado; fallo de servicio conserva el identificador (08) |

## 9. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Wireframe inicial de la pantalla de ingreso del front web, anclado a CU-01 y al marco de experiencia. Layout en columna única, componentes, interacciones, estados (vacío, cargando, con datos, error de acceso y sin conexión al circuito), nota responsive y notas de accesibilidad WCAG 2.2 AA. |
