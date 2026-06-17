# NB-01 — Administración jerárquica de usuarios y control de acceso

| Campo | Valor |
| --- | --- |
| Proyecto | geovial-api |
| Documento | NB-01-administracion-jerarquica-usuarios_v1.0.md |
| Versión | 1.0 |
| Estado | Propuesto |
| Fecha | 2026-06-15 |
| Autor | Analista de Negocio + API Product Analyst |
| Trazabilidad upstream | SOLUTION-INTAKE §1, §2, §4; vision-producto_v1.0.md; alcance-proyecto_v1.0.md |
| Trazabilidad downstream | CU-01, CU-02, CU-03 (previstas en 02_especificacion_funcional) |

## 1. Descripción de la necesidad

La organización necesita que el acceso al relevamiento de tramos viales esté gobernado por una jerarquía clara de responsabilidades, porque hoy el trabajo se reparte de manera informal y no queda registrado quién está habilitado para crear relevamientos, quién para administrar al personal de campo y quién solo puede cargar observaciones. Sin una estructura de cuatro niveles —usuario raíz, jefe general, jefe de área y agente de campo—, la organización no puede delegar la administración del personal hacia abajo ni acotar lo que cada persona ve y hace según su rol.

El dolor concreto es la falta de un control de alcance: cuando cualquier persona puede tocar cualquier relevamiento, se pierde la responsabilidad sobre la información, se mezclan tramos de distintas áreas y no hay forma de auditar quién recolectó o modificó qué. A esto se suma que la operación de campo se hace desde dispositivos compartidos, donde hace falta poder liberar el equipo para que otro agente lo use con su propia identidad, sin filtrar datos entre personas.

La necesidad importa porque es la base sobre la que se apoya todo el ciclo del relevamiento: si la organización no puede confiar en quién hizo cada cosa, los informes de cierre del jefe de área dejan de ser trazables y reproducibles, que es justamente el problema que el negocio quiere resolver.

## 2. Ejemplo de uso desde la perspectiva del negocio

Un jefe de área recibe a un nuevo relevador en su cuadrilla y necesita habilitarlo ese mismo día para que salga a campo, sin depender de un administrador central. Lo da de alta él mismo y queda registrado que ese agente pertenece a su área. Semanas después, cuando ese agente termina su contrato, el jefe lo da de baja y el sistema deja de permitirle el acceso, pero conserva la traza de las observaciones que ya había cargado. En paralelo, en una jornada en la que dos relevadores comparten el mismo equipo de campo, el primero cierra por completo su sesión al terminar y entrega el dispositivo libre para que el segundo ingrese con su propia cuenta.

## 3. Impacto

- Habilita la delegación de la administración del personal de campo al jefe de área, sin pasar por un administrador central.
- Acota qué relevamientos y qué acciones ve cada rol, reduciendo el riesgo de cruces entre áreas.
- Da sustento a la trazabilidad de toda observación: cada registro queda asociado a un responsable identificado.
- Si queda sin resolver, persiste la operación informal actual, los informes pierden trazabilidad y no hay accountability sobre la evidencia recolectada.
- Condiciona la seguridad de la operación en dispositivos compartidos en terreno.

## 4. Problema específico que resuelve

- No existe una jerarquía de roles que defina quién puede administrar a quién ni quién accede a cada relevamiento.
- El alta y la baja de agentes de campo dependen hoy de gestiones manuales y no quedan registradas.
- En dispositivos compartidos no hay forma de liberar el equipo para otra persona sin arrastrar la sesión anterior.
- No hay un punto de control único que garantice que cada acción quede atada a un usuario identificado.

## 5. Criterios de éxito

| Criterio | Métrica | Target | Plazo |
| --- | --- | --- | --- |
| Cobertura de la jerarquía de roles | Niveles de la jerarquía operativos de punta a punta | 4 niveles | release 1.0 |
| Autonomía del jefe de área para administrar agentes | Altas y bajas de agentes que el jefe resuelve sin escalar a un administrador central | 100 % | 3 meses post-despliegue |
| Trazabilidad de la autoría | Porcentaje de observaciones asociadas a un usuario identificado | 100 % | continuo |
| Aislamiento entre sesiones en dispositivo compartido | Incidentes de datos visibles entre usuarios tras cambio de cuenta | 0 incidentes por mes | continuo |
| Tiempo de habilitación de un agente nuevo | Minutos desde la decisión del jefe hasta que el agente puede operar | ≤ 10 min | 3 meses post-despliegue |

## 6. Stakeholders involucrados

| Rol | Nivel | Qué pide o aporta |
| --- | --- | --- |
| Vialidad provincial | Propietario | Aprueba el modelo de jerarquía y el alcance de cada rol |
| Departamento de desarrollo de software (1 desarrollador) | Implementador | Construye y mantiene la administración de usuarios y el control de acceso |
| Jefe de área | Beneficiario | Da de alta y de baja a sus agentes de campo y valida que el control de alcance refleje su operación |
| Usuario raíz | Beneficiario | Configura el sistema y da de alta al jefe general en la cúspide de la jerarquía |

## 7. Trazabilidad a CU

| NB | CU prevista | Estado |
| --- | --- | --- |
| NB-01 | CU-01 administrar la jerarquía de usuarios en cuatro niveles | a generar |
| NB-01 | CU-02 dar de alta y de baja agentes de campo por el jefe de área | a generar |
| NB-01 | CU-03 iniciar sesión, deslogueo completo y relogueo en sesión activa | a generar |

## 8. Dependencias con otras NB

Sin dependencias. Es la NB fundacional sobre la que se apoyan las demás.

## 9. Prioridad MoSCoW

Must Have. Las capacidades F-01, F-02 y F-08 del intake (§4) son Must Have y sin control de acceso jerárquico ningún otro flujo del relevamiento tiene responsable identificable ni alcance acotado.

## 10. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la necesidad a partir de SOLUTION-INTAKE §1, §2, §4 (F-01, F-02, F-08) y de la visión y el alcance de la categoría 00. |
