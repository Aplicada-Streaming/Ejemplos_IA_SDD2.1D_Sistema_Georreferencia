# ADR-03 — Autenticación por token bearer y autorización por rol jerárquico

**Proyecto:** geovial-api
**Documento:** ADR-03-autenticacion-token-bearer-rol-jerarquico_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer
**Categoría:** Seguridad

## 1. Contexto

El backend autentica a usuarios de cuatro roles jerárquicos —usuario raíz, jefe general, jefe de área, agente de campo— y autoriza cada operación según el alcance de ese rol (RN-01). El front web y la app móvil obtienen un token presentando credenciales y lo envían en cada solicitud (intake §17.P.3, §17.P.5). El inicio de sesión inicial requiere conectividad; la app móvil revalida en sesión activa con la seguridad del dispositivo, sin volver a pedir credenciales (CU-03). La baja de un usuario revoca el acceso pero conserva la autoría histórica (RN-02). La cadena de administración no salta niveles ni forma ciclos (RC-03). No hay proveedor de identidad externo: el propio backend emite y valida el token (intake §17.P.5). Cubre CU-03 (sesión) y CU-18 (autorización transversal).

## 2. Decisión

Se adopta autenticación por token bearer emitido por el propio backend a partir de credenciales (el backend es su propio emisor y validador; no hay IdP externo). El token transporta la identidad y el rol del portador. La autorización es un control transversal que, antes de ejecutar cualquier efecto y antes de paginar cualquier listado, resuelve el rol del solicitante y acota la operación al nivel jerárquico inmediato inferior y al ámbito del recurso (RN-01, RC-03). La baja inhabilita la autenticación (USUARIO_INHABILITADO) sin borrar ni desatribuir los registros del usuario (RN-02). La clave de firma del token y demás secretos viven en un gestor de secretos del entorno.

## 3. Estado

Aceptado el 2026-06-15. Decisión pre-tomada en el intake (§17.P.5, §17.P.11): token bearer por credenciales, el backend como emisor/validador, roles jerárquicos.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Token bearer emitido por el backend (elegido) | Sin dependencia de un IdP externo; el token porta rol y alcance; revalidación sin re-credencial en móvil | El backend asume la custodia de la clave de firma y la rotación |
| Proveedor de identidad externo | Delega emisión y rotación; federación | Descartado por el intake (§17.P.5): introduce una dependencia externa y operación adicional para un equipo de un dev; sin requisito de federación |
| Sesión con estado en servidor (cookie de sesión) | Revocación inmediata trivial | No encaja con un cliente móvil offline-first ni con un contrato REST sin estado; acopla el backend a una sesión de servidor |
| Autorización por listas de permiso por endpoint, sin jerarquía | Granularidad fina | No modela la jerarquía de cuatro niveles (RN-01); duplica reglas y se desincroniza del dominio |

## 5. Consecuencias positivas

1. El contrato REST permanece sin estado de sesión en servidor: cada solicitud se autoriza por el token, lo que encaja con el cliente móvil offline-first.
2. La autorización transversal centraliza RN-01 y RC-03 en un único punto previo al efecto, evitando escaladas de privilegio por omisión en un endpoint.
3. La conservación de la autoría ante la baja (RN-02) se sostiene inhabilitando el acceso sin borrar registros.
4. Sin IdP externo, el despliegue y la operación se simplifican para el equipo de un desarrollador.

## 6. Consecuencias negativas y trade-offs

1. La revocación de un token emitido antes de la baja depende de su vigencia; se acepta una ventana de validez acotada y la inhabilitación en el almacén como respaldo (USUARIO_INHABILITADO en cada validación contra el estado del usuario).
2. El backend asume la custodia y rotación de la clave de firma; se mitiga con el gestor de secretos del entorno (intake §17.P.5).
3. El token transporta el rol; un cambio de rol exige reemitir el token o revalidar el rol contra el almacén en operaciones sensibles.

## 7. Implementación

- El adaptador de identidad y token (capa de Infraestructura) emite el token validando credenciales y lo firma con la clave del gestor de secretos; valida el token en cada solicitud.
- El middleware de autorización (capa de API) resuelve rol y alcance del portador y delega en el servicio de autorización por rol y alcance (CU-18) antes del caso de uso.
- La revalidación en sesión activa del móvil (CU-03) se apoya en la seguridad del dispositivo del cliente; el backend solo reconoce el token vigente.
- Convención impuesta: ningún endpoint ejecuta efecto ni pagina antes de pasar el control de jerarquía y alcance (RN-01); ningún secreto se incluye en la imagen ni en el control de versiones.

## 8. Métricas de validación

- Acceso a recursos fuera del ámbito del solicitante rechazado en el 100 % de los casos de prueba (CU-18, RN-01).
- Alta de un nivel no inmediato rechazada y de un nivel inmediato aceptada (CU-01, CU-02, RC-03).
- Usuario dado de baja no puede autenticarse (USUARIO_INHABILITADO) y su autoría permanece (RN-02), verificado en 08.
- Cero secretos en la imagen ni en el repositorio, verificado por análisis de la composición y de configuración.

## 9. Referencias

- NB-01; CU-03, CU-18; RN-01, RN-02; RC-03.
- Intake §17.P.3 (comunicación), §17.P.5 (seguridad), §17.P.11.
- ADRs relacionadas: ADR-01 (estilo), ADR-05 (errores), ADR-10 (versionado).
- `arquitectura-solucion_v1.0.md` §7; `contratos-rest_v1.0.md`.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión de seguridad: autenticación por token bearer emitido por el backend a partir de credenciales y autorización transversal por rol jerárquico; conservación de la autoría ante la baja. Aceptada (pre-tomada en intake §17.P.5, §17.P.11). |
