# CU-02 — Administrar usuarios por jerarquía desde el front web

**Proyecto:** geovial-web
**Documento:** CU-02-administrar-usuarios-jerarquia_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional

## 1. Propósito

Permitir que un usuario administrador (raíz, jefe general o jefe de área) dé de alta, dé de baja y liste, desde el front web, a los usuarios del nivel inmediato inferior que le corresponde administrar, viendo en cada pantalla solo el universo de usuarios que su rol alcanza. Materializa en el front la delegación de la administración del personal hacia abajo.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Usuario administrador (raíz, jefe general o jefe de área) | Primario | Da de alta y de baja y consulta a los usuarios de su nivel inferior |
| Front web | Sistema | Presenta los formularios y listados acotados al alcance del rol y envía las operaciones al backend |
| Backend de dominio | Sistema | Valida la jerarquía, ejecuta el alta o la baja y devuelve el universo de usuarios visible |

## 3. Precondiciones

- El usuario administrador tiene una sesión activa en el front web (CU-01).
- El rol del usuario admite administrar al menos un nivel inferior (no aplica al agente de campo).

## 4. Flujo principal

1. El usuario abre la pantalla de administración de usuarios del front web.
2. El front web solicita al backend el listado de usuarios del nivel inmediato inferior dentro del alcance del solicitante y lo presenta.
3. Para dar de alta, el usuario completa el formulario con el identificador de acceso, los datos del nuevo usuario y su rol, restringido al nivel inmediato inferior.
4. El front web valida el formulario en pantalla y envía el alta al backend.
5. El backend crea el usuario y el front web lo agrega al listado mostrado.
6. Para dar de baja, el usuario selecciona un usuario del listado y confirma la baja.
7. El front web envía la baja al backend; el backend inhabilita el acceso del usuario y conserva su autoría histórica.
8. El front web actualiza el listado reflejando al usuario como dado de baja, sin removerlo del histórico.

## 5. Flujos alternativos

- 5.A Identificador de acceso ya en uso. Disparador: el identificador del nuevo usuario ya existe en el backend. El backend rechaza el alta y el front web informa el conflicto y vuelve al formulario para corregir el identificador. Retorna al paso 3.
- 5.B Baja de un usuario con trabajo en curso. Disparador: el usuario a dar de baja es un agente con relevamientos asignados activos. El front web advierte que la baja inhabilita el acceso pero conserva lo recolectado, y solicita confirmación explícita antes de enviar la baja. Retorna al paso 6.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| JERARQUIA_NO_PERMITIDA | El usuario intenta crear un rol que no es su nivel inmediato inferior | El front web no ofrece ese rol en el formulario y, si se forzara, el backend rechaza y el front lo informa |
| FUERA_DE_ALCANCE | El usuario intenta administrar a alguien fuera de su ámbito | El front web no lista ese usuario; ante el rechazo del backend informa que está fuera de su alcance |
| IDENTIFICADOR_DUPLICADO | El identificador de acceso del nuevo usuario ya existe | El front web informa el duplicado y mantiene el formulario para corregirlo |

## 7. Postcondiciones

- Éxito en alta: existe un usuario nuevo del nivel inmediato inferior, visible en el listado del administrador, habilitado para operar según su rol.
- Éxito en baja: el usuario queda inhabilitado para acceder, conservando su autoría histórica, y se muestra como dado de baja.
- Fallo: el universo de usuarios no cambia y el front informa la causa del rechazo.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un jefe de área con sesión activa en el front web | Da de alta a un agente "agente.lopez" con rol agente de campo | El front lo crea y lo muestra en el listado de agentes del jefe |
| CA-02 | Un jefe de área que ve solo sus propios agentes | Abre la administración de usuarios | El front no muestra agentes de otros jefes de área ni a jefes generales |
| CA-03 | Un jefe general que intenta crear directamente un agente de campo (dos niveles por debajo) | Abre el formulario de alta | El front solo ofrece el rol jefe de área y no permite crear un agente de campo (JERARQUIA_NO_PERMITIDA) |
| CA-04 | Un jefe de área que da de baja a "agente.lopez" con observaciones ya cargadas | Confirma la baja | El front lo muestra como dado de baja y las observaciones cargadas conservan su autoría |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-01 |
| Reglas de negocio aplicables | RN-01 (geovial-web), RN-02 (geovial-web) |
| Historias de usuario a generar | US-03, US-04, US-05 (en 06) |
| Componentes esperados | Pantalla de administración de usuarios; formulario de alta; listado acotado por rol; consumo del recurso de usuarios del backend (referencia tentativa a 05) |
| Tests previstos | Alta de nivel inmediato inferior visible; listado acotado al alcance; rol no inmediato no ofrecido; baja conserva autoría (en 08) |

## 10. Notas y supuestos

- El front no decide la jerarquía: refleja en pantalla las reglas que el backend impone (RN-01 del backend, geovial-api). Si el front y el backend difieren, manda el backend.
- El alta y la baja consumen el contrato del backend; el front no persiste usuarios (intake §17 geovial-web P.4).
- El detalle visual de los formularios y listados (campos, validaciones de presentación finas) pertenece a la categoría 03; aquí se fija el qué del flujo.

## 13. Interacción multiusuario y concurrencia

- Dos administradores del mismo nivel pueden trabajar a la vez; cada uno ve solo su propio ámbito y sus operaciones no se solapan por construcción del alcance.
- Si un usuario ya fue dado de baja por otra sesión, una segunda baja sobre el mismo usuario no produce efecto adicional y el front refleja el estado vigente que devuelve el backend.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de administración de usuarios por jerarquía desde el front web, derivado de NB-01 (F-01, F-02). |
