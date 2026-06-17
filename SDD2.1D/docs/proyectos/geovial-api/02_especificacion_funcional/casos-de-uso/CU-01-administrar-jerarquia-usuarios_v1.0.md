# CU-01 — Administrar la jerarquía de usuarios en cuatro niveles

**Proyecto:** geovial-api
**Documento:** CU-01-administrar-jerarquia-usuarios_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Permitir que cada nivel de la jerarquía dé de alta y de baja a los usuarios del nivel inmediato inferior a través del recurso de usuarios del backend: el usuario raíz administra al jefe general, el jefe general administra a los jefes de área y el jefe de área administra a los agentes de campo. Resuelve la delegación controlada de la administración del personal sin un administrador central único.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Administrador del nivel superior | Primario | Solicita el alta o la baja de un usuario del nivel inmediato inferior |
| Backend de usuarios | Sistema | Valida la jerarquía, registra el usuario y controla el alcance |
| Almacén relacional | Sistema | Persiste el usuario, su rol y su relación de pertenencia |

## 3. Precondiciones

- El solicitante posee un token de autenticación vigente emitido por el backend (ver CU-03).
- El rol del solicitante está habilitado para administrar el nivel inmediato inferior según RN-01.
- Para una baja, el usuario destino existe y pertenece al ámbito administrable por el solicitante.

## 4. Flujo principal

1. El administrador del nivel superior solicita el alta de un usuario indicando rol destino y datos de identificación.
2. El backend verifica que el rol del solicitante puede administrar exactamente el nivel inmediato inferior al suyo (RN-01).
3. El backend valida la unicidad del identificador de acceso del nuevo usuario.
4. El backend crea el usuario, le asigna el rol destino y lo vincula como dependiente del administrador solicitante.
5. El backend responde con la representación del recurso creado y la ubicación del nuevo recurso.
6. Para una baja, el administrador solicita desactivar a un usuario de su ámbito; el backend marca el usuario como inhabilitado, conserva su traza y revoca su acceso futuro (RN-02).

## 5. Flujos alternativos

- 5.A Baja de un usuario con trabajo asociado. Disparador: el usuario a dar de baja tiene observaciones o relevamientos vinculados. El backend inhabilita el acceso pero conserva la autoría histórica de los registros, sin borrarlos. Retorna al paso 6.
- 5.B Alta idempotente reintentada. Disparador: el solicitante reintenta un alta con la misma clave de idempotencia tras una respuesta no recibida. El backend devuelve el mismo recurso ya creado sin duplicarlo (ver CU-21). Retorna al paso 5.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| JERARQUIA_NO_PERMITIDA | El solicitante intenta administrar un nivel que no es el inmediato inferior al suyo | Rechaza con estado de prohibido y un problema descriptivo; no crea ni modifica usuario (RN-01) |
| IDENTIFICADOR_DUPLICADO | El identificador de acceso del nuevo usuario ya existe | Rechaza con estado de conflicto y no crea el usuario |
| USUARIO_FUERA_DE_AMBITO | La baja apunta a un usuario que no pertenece al ámbito del solicitante | Rechaza con estado de prohibido y no modifica al usuario |

## 7. Postcondiciones

- Éxito en alta: existe un usuario nuevo con su rol, vinculado al administrador que lo creó, con acceso habilitado.
- Éxito en baja: el usuario queda inhabilitado, sin poder autenticarse, y su autoría histórica permanece intacta.
- Fallo: el estado de usuarios no cambia y se devuelve un problema con el código correspondiente.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un jefe de área autenticado y un identificador de agente libre | Solicita el alta de un agente de campo en su área | El backend crea el agente con rol agente, lo vincula al jefe y responde con la ubicación del recurso |
| CA-02 | Un jefe de área autenticado | Solicita el alta de un jefe general (dos niveles por encima del agente) | El backend rechaza con el código JERARQUIA_NO_PERMITIDA y no crea el usuario |
| CA-03 | Un jefe de área autenticado con un agente activo que ya cargó observaciones | Solicita la baja de ese agente | El backend inhabilita el acceso del agente y conserva sus observaciones con su autoría |
| CA-04 | Un usuario raíz autenticado | Solicita el alta de un jefe general con un identificador ya usado | El backend rechaza con el código IDENTIFICADOR_DUPLICADO |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-01 |
| Reglas de negocio aplicables | RN-01, RN-02 |
| Historias de usuario a generar | US-01, US-02 (en 06) |
| Componentes esperados | Recurso de usuarios; servicio de jerarquía y alcance; repositorio de usuarios sobre almacén relacional (referencia tentativa a 05) |
| Tests previstos | Alta del nivel inmediato inferior aceptada; alta de nivel no inmediato rechazada; baja conserva autoría; identificador duplicado rechazado (en 08) |

## 10. Notas y supuestos

- El usuario raíz es preexistente a la operación del sistema y no se da de alta por este CU; se asume provisto en la configuración inicial.
- Este CU cubre alta y baja según jerarquía; el inicio de sesión y el cierre de sesión se especifican en CU-03.
- La regla de que cada nivel administra solo el inmediato inferior es la invariante RN-01.

## 12. Performance esperado del CU

- El alta o la baja de un usuario debe resolverse dentro del objetivo de escritura del proyecto (p95 menor o igual a 500 ms en ambiente equivalente al productivo).

## 15. Idempotencia y reintento

- El alta de usuario es una operación no segura; admite una clave de idempotencia provista por el solicitante para que un reintento devuelva el mismo recurso sin crear duplicados (ver CU-21).
- La baja es idempotente por naturaleza: repetir la baja de un usuario ya inhabilitado deja el estado sin cambios y responde con éxito.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de administración de la jerarquía de usuarios, derivado de NB-01 (F-01, F-02). |
