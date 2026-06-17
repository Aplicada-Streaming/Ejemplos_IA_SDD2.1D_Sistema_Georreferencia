# Ejemplo 01 — Datos seed para arrancar la app de demostración

**Proyecto:** geovial-web
**Documento:** ejemplo-01-datos-seed_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Sample Engineer (web-monolith)
**Nivel:** Básico
**Ubicación del código:** `/samples/geovial-web/01-datos-seed/`

## 1. Objetivo del sample

Demostrar cómo dejar la app de demostración del front web con un estado inicial recorrible mediante un conjunto de datos seed y un bootstrap de un comando. El seed siembra los usuarios de la jerarquía administradora, un relevamiento con su composición de tramo, sus agentes asignados, sus marcadores iniciales con etiquetas, una muestra de evidencia para revisar sobre el mapa y el carrusel, un conflicto de marcadores pendiente y un relevamiento en cada estado del ciclo. Al terminar, el desarrollador puede ingresar con cualquier rol administrador y recorrer los flujos administrativos del front sin tener que poblar el dominio a mano.

## 2. Nivel

Básico. Es el punto de entrada del proyecto: no requiere conocer el detalle interno del front ni el contrato del backend; basta con levantar la app de demostración con su backend de demostración ya sembrado y abrir el navegador. Sienta el estado inicial sobre el que se ejercitan los flujos de las variaciones sugeridas (carga manual, portabilidad, configuración de almacenamiento). No hay un sample previo: este es el primero y único del piso `web-monolith`.

## 3. Prerequisites

- Un motor de contenedores capaz de levantar la composición de demostración (el front web de demostración y su backend de demostración ya sembrado), versión moderna con soporte de composición multiservicio.
- Un navegador de escritorio evergreen (últimas dos versiones mayores de los navegadores de uso corriente), exigido por el estilo de render server-side con circuito interactivo.
- El repositorio de la solución clonado, para acceder a la carpeta del sample.
- Las credenciales de prueba que el seed deja documentadas (un usuario por rol administrador), provistas en la plantilla de entorno del sample.

No requiere base de datos local ni almacén durable del lado del front: el dato de dominio es autoritativo del backend de demostración y el front lo consume por contrato (ADR-02).

## 4. Cómo correrlo

1. Entrar a la carpeta del sample: `cd samples/geovial-web/01-datos-seed`.
2. Copiar la plantilla de entorno a su archivo efectivo: `cp .env.example .env` (trae la dirección del backend de demostración y las credenciales de prueba por rol).
3. Levantar la app de demostración con el seed aplicado: `./bootstrap.sh` (levanta la composición y siembra el conjunto seed contra el backend de demostración).
4. Abrir el front web de demostración en el navegador en la dirección que imprime el bootstrap e ingresar con las credenciales del rol que se quiera recorrer.
5. Comparar el estado inicial visible con el output esperado de la sección 6.

El bootstrap es idempotente: volver a ejecutarlo deja el mismo conjunto seed sin duplicar usuarios ni relevamientos.

## 5. Estructura del código

```
01-datos-seed/
├── README.md                     # Resumen del sample y enlace a este markdown
├── .env.example                  # Plantilla de la dirección del backend de demostración y credenciales por rol
├── bootstrap.sh                  # Levanta la composición de demostración y aplica el seed
├── compose.demo.yaml             # Composición del front web de demostración y su backend sembrado
├── seed/
│   ├── 01-usuarios.seed          # Usuarios de la jerarquía: raíz, jefe general, jefes de área, agentes
│   ├── 02-relevamientos.seed     # Relevamientos: uno por estado del ciclo, con su composición de tramo
│   ├── 03-asignaciones.seed      # Agentes asignados a cada relevamiento
│   ├── 04-marcadores.seed        # Marcadores iniciales con etiquetas, incluido un par en conflicto
│   └── 05-evidencia.seed         # Observaciones, fotos y comentarios para revisar sobre el mapa
└── tests/
    └── estado-inicial.test.sh    # Verifica que el seed dejó el estado inicial esperado
```

El conjunto seed se aplica contra el backend de demostración a través del contrato; el front no escribe el seed ni persiste dominio (ADR-02, ADR-04).

## 6. Qué esperar

Tras ejecutar el bootstrap, la app de demostración queda con un estado inicial visible. Lo que el desarrollador observa al ingresar:

- Pantalla de ingreso. Ingresando con el usuario raíz, el jefe general o un jefe de área, el front abre la sesión con el rol correspondiente y habilita solo las pantallas y acciones de ese rol; ingresar con un usuario dado de baja informa el acceso revocado y no abre sesión (CU-01, RN-01, RN-03).
- Administración de usuarios. Cada administrador ve únicamente a los usuarios de su nivel inmediato inferior dentro de su alcance: el jefe de área ve sus agentes, uno de ellos dado de baja pero con su autoría conservada (CU-02, RN-02).
- Listado de relevamientos. El jefe de área ve sus relevamientos seed con su estado del ciclo: uno en recolección, uno en revisión y uno cerrado, con las acciones habilitadas según el estado (CU-03, RN-04).
- Sección de agentes. El relevamiento en recolección muestra sus agentes asignados, listos para reasignar (CU-04).
- Componente de mapa. El relevamiento muestra sus marcadores iniciales fijados con sus etiquetas; un par de marcadores quedó sembrado dentro de un mismo radio para que conviva como conflicto pendiente (CU-05, CU-07).
- Revisión sobre mapa y carrusel. El relevamiento en revisión deja recorrer los marcadores y, al seleccionar uno, abrir el carrusel de fotos con sus comentarios, encadenando con el marcador contiguo; el filtro por etiqueta acota la evidencia mostrada (CU-06).
- Resolución de conflictos y cierre. El relevamiento en revisión presenta el conflicto pendiente; el front no ofrece el cierre hasta resolverlo, y una vez resuelto habilita la transición de cierre (CU-07, CU-08, RN-05).

Resumen del estado inicial que imprime el bootstrap al terminar:

```
Seed aplicado contra el backend de demostracion.
Usuarios: 1 raiz, 1 jefe general, 2 jefes de area, 3 agentes (1 dado de baja).
Relevamientos: 3 (1 en recoleccion, 1 en revision, 1 cerrado).
Marcadores: 7 (incluye 1 par en conflicto pendiente).
Evidencia: observaciones y fotos con comentarios y etiquetas para revisar.
Front web de demostracion disponible en: <direccion-impresa>
```

## 7. Variaciones sugeridas

| Variación | Qué cambiar | Resultado |
| --- | --- | --- |
| Ejercitar la carga manual del agente | Ingresar con un agente seed asignado y abrir la carga manual de su relevamiento en recolección | El front deja subir fotos, agruparlas por radio en marcadores y completar comentarios y etiquetas (CU-09) |
| Ejercitar la portabilidad | Exportar el relevamiento cerrado del seed y volver a importar la unidad transferible | El front entrega una unidad descargable y reconstruye el relevamiento con su evidencia (CU-10) |
| Ejercitar la configuración de almacenamiento | Ingresar con el usuario raíz y cambiar el destino de almacenamiento vigente | El front muestra el nuevo destino vigente, transparente para los demás roles (CU-11) |
| Sembrar más conflictos | Agregar marcadores dentro del radio de otros en `04-marcadores.seed` y re-aplicar el bootstrap | El relevamiento en revisión presenta más conflictos pendientes que bloquean el cierre hasta resolverlos (CU-07, RN-05) |

## 8. Trazabilidad

| Artefacto upstream | Tipo | Cómo lo ilustra este sample |
| --- | --- | --- |
| [CU-01](../02_especificacion_funcional/casos-de-uso/CU-01-iniciar-cerrar-sesion-web_v1.0.md) | Caso de uso | El seed deja usuarios por rol para ingresar y cerrar sesión con cada nivel administrador |
| [CU-02](../02_especificacion_funcional/casos-de-uso/CU-02-administrar-usuarios-jerarquia_v1.0.md) | Caso de uso | El seed deja la jerarquía de usuarios, incluido uno dado de baja con autoría conservada, para recorrer la administración por alcance |
| [CU-03](../02_especificacion_funcional/casos-de-uso/CU-03-crear-editar-listar-relevamiento_v1.0.md) | Caso de uso | El seed deja relevamientos con su composición de tramo y estado del ciclo en el listado |
| [CU-04](../02_especificacion_funcional/casos-de-uso/CU-04-asignar-reasignar-agentes_v1.0.md) | Caso de uso | El seed deja agentes asignados a un relevamiento para recorrer la asignación y reasignación |
| [CU-05](../02_especificacion_funcional/casos-de-uso/CU-05-crear-marcadores-iniciales_v1.0.md) | Caso de uso | El seed deja marcadores iniciales con etiquetas fijados sobre el mapa |
| [CU-06](../02_especificacion_funcional/casos-de-uso/CU-06-revisar-relevamiento-mapa-carrusel_v1.0.md) | Caso de uso | El seed deja evidencia para recorrer marcadores y carrusel de fotos y filtrar por etiqueta |
| [CU-07](../02_especificacion_funcional/casos-de-uso/CU-07-resolver-conflictos-cierre_v1.0.md) | Caso de uso | El seed deja un conflicto de marcadores pendiente para ejercitar su resolución antes del cierre |
| [CU-08](../02_especificacion_funcional/casos-de-uso/CU-08-transicionar-estado-cerrar_v1.0.md) | Caso de uso | El seed deja un relevamiento por estado del ciclo para recorrer transiciones y el cierre condicionado |
| [RN-01](../02_especificacion_funcional/reglas-de-negocio/RN-01-visibilidad-acciones-por-rol_v1.0.md) | Regla de negocio | El front presenta solo las pantallas y acciones del alcance del rol con el que se ingresó |
| [RN-04](../02_especificacion_funcional/reglas-de-negocio/RN-04-estados-visibles-habilitacion-acciones_v1.0.md) | Regla de negocio | El front habilita solo las acciones válidas para el estado vigente de cada relevamiento seed |
| [RN-05](../02_especificacion_funcional/reglas-de-negocio/RN-05-conflictos-precondicion-cierre_v1.0.md) | Regla de negocio | El front no ofrece el cierre con el conflicto seed pendiente hasta resolverlo |
| [ADR-01](../05_arquitectura_tecnica/adrs/ADR-01-estilo-render-server-side-circuito-interactivo_v1.0.md) | Decisión arquitectónica | La app de demostración se sirve con render server-side y circuito interactivo, recorrida en un navegador evergreen |
| [ADR-02](../05_arquitectura_tecnica/adrs/ADR-02-sin-persistencia-dominio-estado-efimero_v1.0.md) | Decisión arquitectónica | El dato seed es autoritativo del backend de demostración; el front lo consume y no persiste dominio |
| [ADR-03](../05_arquitectura_tecnica/adrs/ADR-03-autenticacion-token-bearer-lado-servidor_v1.0.md) | Decisión arquitectónica | El ingreso con las credenciales seed obtiene un token bearer custodiado del lado servidor del circuito |
| [ADR-04](../05_arquitectura_tecnica/adrs/ADR-04-separacion-capas-presentacion-aplicacion-cliente-api_v1.0.md) | Decisión arquitectónica | El seed se aplica por el contrato del backend; el front consume el estado a través del Cliente de API |
| NFR (intake §17 geovial-web P.10) | Requisito no funcional | El estado inicial recorrido sostiene la latencia de interacción p95 ≤ 200 ms del circuito sobre red estable |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Versión inicial del sample de datos seed: bootstrap de un comando que deja la app de demostración con usuarios de la jerarquía, relevamientos por estado del ciclo, asignaciones, marcadores con etiquetas, evidencia para revisar y un conflicto pendiente, ilustrando CU-01 a CU-08 y las RN-01, RN-04 y RN-05. |
