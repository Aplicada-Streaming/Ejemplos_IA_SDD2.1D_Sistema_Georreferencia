# ADR-03 — Autenticación por credenciales con token bearer custodiado del lado servidor del circuito

**Proyecto:** geovial-web
**Documento:** ADR-03-autenticacion-token-bearer-lado-servidor_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto Senior
**Categoría:** Seguridad

## 1. Contexto

`geovial-web` es una herramienta de los roles administradores (usuario raíz, jefe general, jefe de área) con la excepción de la carga manual del agente (RN-03). El usuario ingresa con credenciales y el front debe consumir el contrato REST de `geovial-api`, que exige un token bearer en cada operación salvo el inicio de sesión (contratos-rest de geovial-api §2; ADR-03 de geovial-api). El intake fija que el front obtiene el token del backend enviando credenciales y que el token se mantiene del lado servidor del circuito, no se expone al navegador (§17 geovial-web P.5, §17.P.11). La autorización autoritativa la resuelve el backend por rol jerárquico; el front solo refleja el alcance en la visibilidad de pantallas y acciones (RN-01) y restringe el acceso a roles administradores (RN-03). La baja de un usuario conserva la autoría visible (RN-02), invariante que el backend garantiza. Motivan esta decisión NB-01, CU-01 (sesión), CU-02 y CU-09 (acceso por rol) y las RN-01, RN-02 y RN-03.

## 2. Decisión

Se adopta autenticación por credenciales con token bearer: el front recibe del usuario sus credenciales, las envía al recurso de autenticación de `geovial-api` y obtiene un token bearer. El token se custodia del lado servidor, asociado al circuito interactivo de la sesión, y nunca se serializa ni se entrega al navegador. El Cliente de API adjunta ese token a cada llamada al contrato REST. El cierre de sesión descarta el token y el estado del circuito asociado, dejando el dispositivo listo para otro usuario. La autorización es autoritativa en el backend; el front aplica un control de presentación que muestra solo pantallas y acciones del alcance del rol (RN-01), restringe el front a roles administradores salvo la carga manual del agente (RN-03) y nunca trata el ocultamiento de una acción como equivalente a autorizarla: toda operación se valida en el backend y un rechazo se mapea a feedback (ADR-05).

## 3. Estado

Aceptado el 2026-06-15. Decisión pre-tomada en el intake (§17 geovial-web P.5, P.11): el front obtiene el token del backend enviando credenciales y lo mantiene del lado servidor del circuito, sin exponerlo al navegador.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Token bearer custodiado del lado servidor del circuito (elegido) | El token no viaja al navegador; menor superficie de exposición; encaja con el render server-side | El servidor retiene el token por sesión; la revocación depende de la vigencia del token (gobernada por el backend) |
| Token bearer almacenado en el navegador | Menos estado en el servidor | Descartado por el intake (§17.P.5): expone el token al cliente, ampliando la superficie de robo; contrario al estilo server-side (ADR-01) |
| Sesión propia del front con estado en servidor desacoplada del token de la API | Control de sesión local | Descartado: el contrato de la API exige token bearer en cada llamada; introduciría un segundo mecanismo de sesión redundante |
| Proveedor de identidad externo | Federación; delega emisión y rotación | Descartado: la API es su propio emisor/validador (ADR-03 de geovial-api), sin IdP externo ni requisito de federación |

## 5. Consecuencias positivas

1. El token bearer no se expone al navegador: la superficie de exposición se reduce al servidor del front, alineado con el estilo server-side (ADR-01) y con §17.P.5.
2. El cierre de sesión es completo y simple: descartar el token y el estado del circuito libera el dispositivo para otro usuario (CU-01).
3. La autorización autoritativa queda en un único punto (el backend); el front no duplica las reglas de jerarquía, solo refleja el alcance en la presentación (RN-01, RN-03).
4. La conservación de la autoría ante la baja (RN-02) la sostiene el backend; el front solo presenta la autoría visible sin desatribuir.

## 6. Consecuencias negativas y trade-offs

1. El servidor del front retiene un token por sesión activa, sumando estado por circuito. Se acota porque el token es pequeño y vive solo durante la sesión, y se descarta al cerrar o al perder el circuito.
2. La revocación de un token ya emitido depende de su vigencia, gobernada por el backend (ADR-03 de geovial-api); el front no puede invalidarlo más allá de descartarlo localmente. Se acepta y se delega la política de vigencia al backend.
3. La pérdida del circuito implica reautenticar para reconstruir la sesión. Se acepta: el front presenta el ingreso de nuevo y no retiene credenciales.

## 7. Implementación

- El servicio de sesión y token (Aplicación de UI) recibe las credenciales de la vista de ingreso, las envía al recurso de autenticación de la API y retiene el token en el estado del circuito del lado servidor.
- El Cliente de API adjunta el token a cada solicitud del contrato REST; ninguna vista accede al token ni lo serializa al navegador.
- El control de visibilidad por rol (Aplicación de UI) consulta el rol del portador y oculta o deshabilita pantallas y acciones fuera del alcance (RN-01); restringe el front a roles administradores salvo la carga manual del agente (RN-03).
- Convención impuesta: el token nunca se incluye en el marcado enviado al navegador, en almacenamiento del cliente ni en registros; los secretos del front hacia el backend viven en un gestor de secretos del entorno (§17.P.5).

## 8. Métricas de validación

- Cero exposiciones del token bearer al navegador, verificado por inspección de la superficie de presentación y prueba de componente (NFR §8, 08).
- El cierre de sesión descarta el token y el estado del circuito; tras cerrar, ninguna llamada al contrato lleva el token previo, verificado en 08.
- Una acción fuera del alcance del rol queda oculta o deshabilitada en el front y, si se fuerza, es rechazada por el backend y mapeada a feedback, verificado en 08 (RN-01, RN-03).
- Ningún secreto del front en la imagen ni en el control de versiones, verificado por análisis de la composición y de configuración.

## 9. Referencias

- NB-01; CU-01, CU-02, CU-09; RN-01, RN-02, RN-03.
- Intake §17 geovial-web P.5, P.11.
- `geovial-api`: ADR-03 (autenticación por token bearer y autorización por rol jerárquico), `contratos-rest_v1.0.md` §2.
- ADRs relacionadas: ADR-01 (estilo), ADR-05 (manejo de errores).
- `arquitectura-solucion_v1.0.md` §7.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión de seguridad: autenticación por credenciales con token bearer obtenido del backend, custodiado del lado servidor del circuito y no expuesto al navegador; autorización autoritativa en el backend reflejada en la visibilidad del front. Aceptada (pre-tomada en intake §17.P.5, §17.P.11). |
