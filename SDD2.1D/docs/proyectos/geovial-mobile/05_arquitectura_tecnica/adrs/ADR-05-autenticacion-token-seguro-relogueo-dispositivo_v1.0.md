# ADR-05 — Autenticación con almacenamiento seguro del token y relogueo por seguridad del dispositivo

**Proyecto:** geovial-mobile
**Documento:** ADR-05-autenticacion-token-seguro-relogueo-dispositivo_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto Móvil
**Categoría:** Seguridad

## 1. Contexto

El agente inicia sesión la primera vez en línea con credenciales y recibe un token bearer del backend, que es la única fuente del token (CU-03 del backend; la app no lo emite ni lo renueva). El dispositivo puede ser compartido: se necesita un deslogueo completo que libere el dispositivo para otro usuario, y durante una sesión activa, si el dispositivo se bloqueó o la app se reinició, la app debe rehabilitar el acceso pidiendo verificación por la seguridad del propio dispositivo (patrón, huella o equivalente) sin reingreso de credenciales (RN-04, F-08). El token debe custodiarse en el almacenamiento seguro del dispositivo, nunca en texto plano (intake §17.P.5). La app consume el contrato REST presentando el token como bearer y, para la sincronización, reutiliza ese token a través del proveedor de credencial del motor (ADR-03). Cubre CU-01 y, en la reanudación, CU-06.

## 2. Decisión

Se adopta autenticación basada en token bearer obtenido del backend, custodiado en el almacenamiento seguro del dispositivo provisto por la plataforma, con tres modos de sesión:

- Inicio en línea con credenciales: la primera vez (y al cambiar de usuario) la app exige credenciales en línea, obtiene el token del backend y lo guarda en el almacenamiento seguro.
- Relogueo por seguridad del dispositivo: durante una sesión activa, ante reinicio de la app o desbloqueo del dispositivo, la app rehabilita el acceso pidiendo verificación por la seguridad del dispositivo (patrón, huella o equivalente), sin reingresar credenciales. Si el dispositivo no tiene seguridad configurada, la app advierte y exige inicio en línea en cada reanudación.
- Deslogueo completo: borra el token y los datos de sesión del dispositivo, liberándolo para que otro usuario inicie sesión con su propia cuenta.

El token nunca se guarda ni se registra en texto plano; se presenta como bearer en cada solicitud al contrato REST y se entrega al motor de sincronización a través del proveedor de credencial.

## 3. Estado

Aceptado el 2026-06-15. Decisión pre-tomada en el intake (§17.P.5, §17.P.11) y derivada de RN-04.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Token en almacenamiento seguro + relogueo por seguridad del dispositivo (elegida) | Protege el token en dispositivo compartido; reanuda sin reingresar credenciales; deslogueo completo libera el dispositivo (RN-04) | Depende de que el dispositivo tenga seguridad configurada; sin ella, exige inicio en línea en cada reanudación |
| Token en almacenamiento en texto plano | Implementación trivial | Expone el token en un dispositivo compartido o comprometido; contradice intake §17.P.5 |
| Reingreso de credenciales en cada reanudación | No depende de la seguridad del dispositivo | Fricción de campo alta; contradice RN-04 (relogueo sin credenciales) |
| Sesión sin expiración ni relogueo | Sin fricción | El token queda accesible indefinidamente en un dispositivo compartido; riesgo de seguridad alto |

## 5. Consecuencias positivas

1. El token se protege en el almacenamiento seguro del dispositivo, mitigando el riesgo de token comprometido en un dispositivo compartido (riesgo de §9 de la arquitectura).
2. La reanudación por seguridad del dispositivo evita reingresar credenciales, reduciendo la fricción de campo (RN-04).
3. El deslogueo completo borra token y datos de sesión, habilitando el cambio de usuario seguro (RN-04).
4. La sincronización reutiliza el mismo token vía el proveedor de credencial del motor, sin custodiar la credencial dos veces (ADR-03).

## 6. Consecuencias negativas y trade-offs

1. Si el dispositivo no tiene seguridad configurada, la app exige inicio en línea en cada reanudación; se acepta como salvaguarda (DISPOSITIVO_SIN_SEGURIDAD).
2. La app depende del backend como única fuente del token: si el token expira o se rechaza durante un ciclo, la sincronización pide reloguear; se acepta porque el backend gobierna la vigencia.
3. El relogueo por seguridad del dispositivo agrega un paso de verificación en la reanudación; se acepta por la protección en dispositivo compartido.

## 7. Implementación

- El servicio de sesión orquesta los tres modos; el adaptador de almacenamiento seguro custodia el token y nunca lo expone en texto plano ni en el log.
- En el inicio en línea, la app envía credenciales al endpoint de inicio de sesión del contrato REST y guarda el token devuelto; las credenciales no se persisten.
- En la reanudación con sesión activa, la app solicita la verificación por seguridad del dispositivo antes de exponer datos o reanudar la sincronización (RN-04, ciclo de vida del sistema operativo de `arquitectura-solucion_v1.0.md`).
- El deslogueo completo borra el token, el estado de sesión y los datos de sesión del dispositivo.
- El cliente del contrato REST adjunta el token como bearer en toda solicitud salvo el inicio de sesión; el adaptador de la librería de sincronización provee el token como proveedor de credencial (ADR-03).
- Si el backend rechaza el token durante un ciclo, la sincronización se detiene y la app solicita reloguear (CU-06).

## 8. Métricas de validación

- El token nunca aparece en texto plano en el almacenamiento ni en el log local (revisión de seguridad, 08).
- La reanudación con sesión activa exige verificación por seguridad del dispositivo sin reingreso de credenciales (RN-04, 08, CU-01).
- El deslogueo completo deja el dispositivo sin token ni datos de sesión recuperables (CU-01, 08).
- Sin seguridad de dispositivo, la app advierte y exige inicio en línea en la reanudación (DISPOSITIVO_SIN_SEGURIDAD, 08).
- Arranque en frío ≤ 3 s hasta la pantalla de sesión/verificación (NFR de arranque, junto con ADR-01).

## 9. Referencias

- NB-01; CU-01, CU-06; RN-04; F-08.
- Intake §17.P.5, §17.P.10, §17.P.11.
- Endpoints de sesión del backend: `proyectos/geovial-api/05_arquitectura_tecnica/contratos-rest_v1.0.md` (iniciar sesión, cerrar sesión, revalidar).
- ADRs relacionadas: ADR-03 (proveedor de credencial del motor), ADR-04 (permisos), ADR-01 (estilo).
- `arquitectura-solucion_v1.0.md`; `flujo-ejecucion_v1.0.md`.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión de seguridad: token bearer custodiado en almacenamiento seguro del dispositivo, con inicio en línea por credenciales, relogueo por seguridad del dispositivo en sesión activa y deslogueo completo; el token se reutiliza para la sincronización vía proveedor de credencial. Aceptada (pre-tomada en intake §17.P.5, §17.P.11). |
