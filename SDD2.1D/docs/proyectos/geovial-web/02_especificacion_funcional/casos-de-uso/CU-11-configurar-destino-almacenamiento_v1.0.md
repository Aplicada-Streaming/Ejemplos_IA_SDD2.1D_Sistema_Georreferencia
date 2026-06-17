# CU-11 — Configurar el destino de almacenamiento de archivos

**Proyecto:** geovial-web
**Documento:** CU-11-configurar-destino-almacenamiento_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional

## 1. Propósito

Permitir que el usuario raíz consulte y cambie, desde el front web, el destino donde el sistema aloja las fotografías de los relevamientos, eligiendo entre los destinos disponibles, de forma transparente para los demás roles. Da al negocio control sobre dónde se guarda la evidencia según costo, capacidad y contexto.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Usuario raíz | Primario | Consulta y cambia el destino de almacenamiento de archivos |
| Front web | Sistema | Presenta los destinos disponibles y la configuración vigente y envía el cambio al backend |
| Backend de dominio | Sistema | Valida que el solicitante es el usuario raíz, aplica el destino y confirma la configuración |

## 3. Precondiciones

- El usuario raíz tiene una sesión activa en el front web (CU-01).
- El backend expone al menos dos destinos de almacenamiento configurables.

## 4. Flujo principal

1. El usuario raíz abre la pantalla de configuración de almacenamiento del front web.
2. El front web consume del backend el destino vigente y los destinos disponibles, y los presenta.
3. El usuario raíz selecciona un destino distinto y completa los datos de configuración que ese destino requiere.
4. El front web valida en pantalla que los datos requeridos estén completos y envía el cambio al backend.
5. El backend aplica el destino seleccionado y confirma la nueva configuración.
6. El front web muestra el destino vigente actualizado, sin que cambie nada visible para los agentes ni los jefes.

## 5. Flujos alternativos

- 5.A Datos de destino incompletos. Disparador: el destino elegido requiere datos de conexión que el usuario raíz no completó. El front web no envía el cambio e indica qué datos faltan. Retorna al paso 3.
- 5.B Verificación del destino antes de aplicar. Disparador: el usuario raíz solicita verificar el destino antes de confirmarlo. El front pide al backend una comprobación del destino y muestra su resultado; si es correcto, habilita aplicar el cambio. Retorna al paso 4.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| ROL_NO_AUTORIZADO | Un usuario que no es el raíz intenta cambiar el destino | El front no ofrece esta pantalla a otros roles; ante el rechazo del backend informa que no está autorizado |
| DESTINO_NO_DISPONIBLE | El destino elegido no está entre los configurables del backend | El front no lo ofrece y, si se forzara, informa que el destino no está disponible |
| CONFIGURACION_INVALIDA | Los datos del destino no permiten alcanzarlo | El front informa que la configuración no es válida y no aplica el cambio |

## 7. Postcondiciones

- Éxito: el destino de almacenamiento vigente queda actualizado y las nuevas fotografías se alojan allí, de forma transparente para los demás roles.
- Fallo: el destino vigente no cambia y el front informa la causa.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un usuario raíz con el destino vigente "infraestructura propia" | Cambia el destino a un servicio externo y completa sus datos | El front muestra el nuevo destino vigente y confirma el cambio |
| CA-02 | Un jefe de área con sesión activa | Intenta acceder a la configuración de almacenamiento | El front no le ofrece la pantalla (ROL_NO_AUTORIZADO) |
| CA-03 | Un usuario raíz que elige un destino externo sin completar sus datos de conexión | Intenta aplicar el cambio | El front no lo envía e indica los datos faltantes (CONFIGURACION_INVALIDA) |
| CA-04 | Un cambio de destino aplicado por el usuario raíz | Un agente sube fotos en su carga | El agente no percibe diferencia y las nuevas fotos se alojan en el destino vigente |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-07 |
| Reglas de negocio aplicables | RN-01 (geovial-web) |
| Historias de usuario a generar | US-24, US-25 (en 06) |
| Componentes esperados | Pantalla de configuración de almacenamiento; selector de destino; consumo del recurso de configuración del backend (referencia tentativa a 05) |
| Tests previstos | Cambio de destino por el raíz; pantalla no disponible para otros roles; datos incompletos no aplican; transparencia para el agente (en 08) |

## 10. Notas y supuestos

- La configurabilidad del almacenamiento es Could Have (NB-07, intake §4 F-17): se incorpora si la cadencia lo permite y no integra el camino principal del relevamiento.
- El conjunto de destinos disponibles y el efecto del cambio los define el backend a través de la librería de almacenamiento (CU-17 de geovial-api); el front solo presenta y envía la selección del usuario raíz.
- El manejo seguro de las credenciales del destino (por ejemplo, claves de un servicio externo) es competencia del backend; el front no las persiste (intake §17 geovial-web P.4, P.5).

## 13. Interacción multiusuario y concurrencia

- La configuración del destino es exclusiva del usuario raíz; ningún otro rol la modifica en paralelo.
- El cambio de destino es transparente para los agentes y jefes que estén operando: no interrumpe sus flujos de carga ni de revisión en curso.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de configuración del destino de almacenamiento desde el front web, derivado de NB-07 (F-17). |
