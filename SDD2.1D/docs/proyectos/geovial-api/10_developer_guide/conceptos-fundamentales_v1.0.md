# Conceptos fundamentales — geovial-api

**Proyecto:** geovial-api
**Documento:** conceptos-fundamentales_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Technical Writer + API Documentation Lead
**Tipo Diátaxis:** Explanation
**Audiencia:** Developer consumidor de la API HTTP (equipos de geovial-web y geovial-mobile, integradores internos)
**Nivel:** Medio
**Tiempo estimado de lectura:** 16 min

Este documento construye el modelo mental con el que un developer consume el contrato REST de geovial-api. No enseña a hacer el primer request (eso es la `guia-onboarding-developer_v1.0.md`) ni lista firmas exactas (eso es la `referencia-api_v1.0.md`): explica por qué la API funciona como funciona, para que el integrador anticipe el comportamiento de cada recurso en lugar de descubrirlo por ensayo y error. Los términos en `codigo-kebab` se definen en `glosario-tecnico_v1.0.md`.

## 1. Concepto central

geovial-api es el backend que recibe, conserva y entrega el relevamiento fotográfico georreferenciado de tramos viales. Recibe credenciales y devuelve un token bearer; recibe relevamientos, marcadores, observaciones y fotos creados desde el front web o capturados en campo por la app móvil; los conserva con su autoría y su ubicación; y los entrega para revisión sobre mapa y para sincronizar a un cliente que estuvo sin conexión. Es la única fuente de verdad de la solución: el front web y la app móvil no comparten estado entre sí, lo comparten a través de este contrato.

Todo lo que el consumidor hace con la API es operar recursos a través de endpoints versionados, presentando en cada request un token bearer que porta su rol. La API decide qué puede ver y qué puede cambiar según ese rol y según el ámbito jerárquico del solicitante.

## 2. Modelo mental

### 2.1 Flujo principal

El ciclo de vida de un relevamiento atraviesa la API en cinco etapas. El conflicto de marcadores convive con la operación durante las tres primeras y solo bloquea la última.

```text
[1] Alta y asignación        [2] Recolección          [3] Sincronización        [4] Revisión             [5] Cierre
    (jefe de área)               (agente de campo)        (agente de campo)         (jefe de área)           (jefe de área)
    crea relevamiento  ──▶       crea marcadores  ──▶     sube cambios locales ──▶  consulta sobre mapa ──▶  resuelve conflictos
    asigna agentes               ancla observaciones      LUEGO baja               lista conflictos         transiciona a revisión
    estado: recolección          adjunta fotos            actualizaciones          (conflictos visibles)    cierra (sin conflictos
                                                                                                            pendientes)
    entrada: tramo vial          entrada: foto+coordenada entrada: lote de cambios  entrada: id relevamiento entrada: resoluciones
    salida: relevamiento         salida: marcador/obs.    salida: novedades+marca   salida: marcadores+fotos salida: relevamiento cerrado
```

Cada etapa se apoya en un grupo de recursos del contrato (`referencia-api_v1.0.md` §3). El orden no es decorativo: la sincronización siempre sube antes de bajar (§3.2), y el cierre siempre exige que no queden conflictos pendientes (§3.3).

| Etapa | Qué hace el consumidor | Recurso principal | Ejemplo |
| --- | --- | --- | --- |
| Alta y asignación | El jefe de área crea el relevamiento con su tramo y asigna agentes | `relevamientos`, `asignaciones` | Crear el relevamiento del tramo norte con dos puentes y un camino, y asignar al agente A |
| Recolección | El agente crea marcadores, ancla observaciones y adjunta fotos | `marcadores`, `observaciones`, `fotos` | Crear un marcador sobre la pila de un puente y anclarle una observación con dos fotos |
| Sincronización | El agente sube el lote capturado sin conexión y luego baja novedades | `sincronizacion/subida`, `sincronizacion/bajada` | Subir 80 cambios acumulados offline y bajar las actualizaciones posteriores a la marca |
| Revisión | El jefe consulta el relevamiento sobre mapa y lista los conflictos | `relevamientos/{id}`, `conflictos` | Recorrer marcadores con sus fotos y ver dos marcadores en conflicto por proximidad |
| Cierre | El jefe resuelve conflictos, transiciona a revisión y cierra | `conflictos/{id}/resolucion`, `transiciones`, `cierre` | Unificar dos marcadores y cerrar el relevamiento para el informe |

### 2.2 Recursos y jerarquía de datos

La API expone una jerarquía de recursos anidados. El `relevamiento` es la raíz operativa: agrupa marcadores, asignaciones, conflictos y marcas de sincronización. El `marcador` agrupa observaciones; la `observacion` agrupa fotos; la `foto` lleva a lo sumo un comentario y varias etiquetas.

```text
relevamiento
├── tramo-vial (composición no vacía de puentes y caminos)
├── asignacion (agente ↔ relevamiento)
├── marcador-geografico (identidad estable)
│   └── observacion (anclada a un marcador, con autor)
│       └── foto (referencia al almacén, ubicación, comentario, etiquetas)
├── conflicto-marcadores (dos o más marcadores en un mismo radio)
└── marca-de-sincronizacion (una por cliente de campo)
```

El consumidor nunca recibe el binario de una foto incrustado en el recurso: la `foto` lleva una referencia lógica al almacén, y el backend delega el alojamiento a una librería de almacenamiento transparente (ADR-09). El modelo conceptual completo, con cardinalidades, vive en `modelo-conceptual_v1.0.md` (02).

### 2.3 Jerarquía de usuarios y autorización por rol

La API conoce cuatro roles en una jerarquía de mayor a menor alcance: `usuario-raiz` → `jefe-general` → `jefe-de-area` → `agente-de-campo`. El token bearer porta el rol del portador, y la API resuelve el rol y el ámbito antes de ejecutar cualquier efecto y antes de paginar cualquier listado (ADR-03).

| Rol | Qué administra | Qué opera sobre relevamientos |
| --- | --- | --- |
| `usuario-raiz` | Da de alta al jefe general; configura el destino de almacenamiento | Importa relevamientos |
| `jefe-general` | Administra jefes de área | — |
| `jefe-de-area` | Da de alta y de baja agentes de su área | Crea, asigna, consulta, transiciona, resuelve conflictos y cierra |
| `agente-de-campo` | — | Crea marcadores y observaciones, adjunta fotos y sincroniza, solo en relevamientos asignados |

Dos reglas gobiernan la autorización y conviene tenerlas presentes para no malinterpretar un rechazo:

- Solo se administra el nivel inmediato inferior. Un jefe general no da de alta agentes directamente; un jefe de área no da de alta otros jefes de área (RN-01, RC-03). Saltar un nivel devuelve `JERARQUIA_NO_PERMITIDA`.
- Solo se operan recursos del propio ámbito. Un jefe de área no ve relevamientos de otro jefe; un agente no sincroniza un relevamiento que no tiene asignado. Operar fuera del ámbito devuelve `FUERA_DE_ALCANCE` o `RELEVAMIENTO_NO_ASIGNADO`.

Una baja inhabilita el acceso del usuario pero conserva su autoría histórica: las observaciones que registró siguen atribuidas a él (RN-02). Un token emitido antes de la baja deja de servir cuando vence o cuando la validación detecta al usuario inhabilitado (`USUARIO_INHABILITADO`).

### 2.4 Sincronización subir-luego-bajar

El agente de campo trabaja sin conexión y sincroniza al recuperar la red. El ciclo tiene dos fases estrictamente ordenadas (ADR-07, RN-06):

1. Subida (`sincronizacion/subida`): el cliente envía el lote de cambios locales. Cada cambio porta un identificador de origen estable, de modo que reenviar un lote tras un corte no duplica nada (idempotencia, §3.4). El backend incorpora el lote y reconoce los reenvíos.
2. Bajada (`sincronizacion/bajada`): solo cuando la subida concluyó, el cliente baja las novedades posteriores a su `marca-de-sincronizacion` y recibe una marca nueva.

Pedir la bajada sin haber concluido la subida del ciclo devuelve `SUBIDA_NO_CONCLUIDA`. El motivo es de seguridad de datos: bajar primero podría sobrescribir, con datos del servidor, cambios locales aún no enviados. La marca es opaca para el cliente y solo avanza; una marca no reconocible devuelve `MARCA_INVALIDA` y obliga a una sincronización completa (RC-06).

### 2.5 Tolerancia a conflictos

Dos o más marcadores dentro de un mismo radio forman un conflicto de marcadores. La decisión de diseño central de GeoVial es que ese conflicto es un estado válido, no un error (ADR-06, RN-03). Crear o mover marcadores, sincronizar y consultar el relevamiento para revisión nunca se bloquean por un conflicto: el conflicto se registra como una entidad de primera clase con estado pendiente o resuelto, y la información queda accesible. La resolución (unificar o separar) se difiere al cierre y la decide el jefe de área, que cuenta entonces con la evidencia completa. El cierre es el único momento en que el conflicto importa: cerrar con conflictos pendientes devuelve `CONFLICTOS_PENDIENTES`.

### 2.6 Errores, paginación, idempotencia y versionado: contratos transversales

Cuatro comportamientos se encuentran de la misma forma en todos los recursos. Entenderlos una vez evita reaprenderlos endpoint por endpoint:

- Errores uniformes. Todo error llega como problem+json con un código estable en mayúsculas, opaco al idioma. El consumidor decide por el código, nunca por el texto del mensaje (ADR-05, CU-19). Catálogo completo en `referencia-api_v1.0.md` §7.
- Paginación. Ningún listado entrega el conjunto completo. Cada listado acepta tamaño y posición de página y devuelve referencias para navegar; el alcance jerárquico se aplica antes de paginar (ADR-04, CU-20).
- Idempotencia. Las operaciones no seguras reintentables aceptan una clave de idempotencia en una cabecera dedicada; reintentar con la misma clave no duplica el efecto (ADR-08, CU-21).
- Versionado. El contrato se versiona por la ruta con un prefijo de versión mayor (`/v1`). Un cambio incompatible publica una versión mayor nueva conservando la anterior durante un período de convivencia (ADR-10, CU-22).

## 3. Decisiones de diseño relevantes para el consumidor

Cada decisión afecta cómo se consume la API, no su implementación interna. La fuente es la ADR citada en 05.

| Decisión | Por qué (frente a la alternativa) | Impacto en el consumidor | ADR |
| --- | --- | --- | --- |
| Token bearer emitido por el propio backend; autorización por rol jerárquico | Frente a un proveedor de identidad externo: sin dependencia externa para un equipo chico; el token porta rol y alcance | Se obtiene el token enviando credenciales y se presenta como bearer en cada request; un cambio de rol exige un token nuevo | ADR-03 |
| Errores problem+json con código estable | Frente a errores ad-hoc por endpoint: contrato de error homogéneo; el código no cambia al traducir | El consumidor ramifica por el código estable, no por el texto, y lo trata igual en todos los recursos | ADR-05 |
| Paginación uniforme con alcance previo | Frente a listados completos: protege la latencia y nunca expone recursos fuera del ámbito | Siempre se recibe una página; se navega siguiendo las referencias; un tamaño excesivo se acota y se informa, no se rechaza | ADR-04 |
| Tolerancia a conflictos resuelta al cierre | Frente a bloquear o unificar automáticamente: no detiene la captura en campo; conserva toda la evidencia para el jefe | Los conflictos no rompen recolección, sync ni revisión; solo el cierre los exige resueltos | ADR-06 |
| Sincronización subir-antes-de-bajar | Frente a bajar primero o hacer merge automático: evita sobrescribir cambios locales no enviados | La bajada antes de concluir la subida se rechaza; la marca de sync es opaca y solo avanza | ADR-07 |
| Idempotencia explícita por clave e identificador de origen | Frente a confiar en que el cliente no reintenta: un corte no duplica datos | Las escrituras reintentables exigen o admiten clave; reusar la clave con otro contenido se rechaza | ADR-08 |
| Versionado por URI con convivencia | Frente a un contrato sin versión: un cambio incompatible no rompe en silencio a los clientes | El consumidor indica la versión mayor en la ruta y migra de prefijo en una versión mayor nueva | ADR-10 |

## 4. Vocabulario

Subconjunto crítico para entender la API. El glosario completo, con referencia cruzada, vive en `glosario-tecnico_v1.0.md`.

| Término | Definición operativa | Ejemplo |
| --- | --- | --- |
| `token-bearer` | Credencial opaca que el backend emite a partir de credenciales y que el cliente presenta en cada request; porta el rol y tiene vigencia limitada | El token de un jefe de área obtenido al iniciar sesión |
| `relevamiento` | Unidad de trabajo que registra observaciones de un tramo vial y recorre recolección, revisión y cierre | El relevamiento de un tramo con dos puentes en estado de recolección |
| `marcador-geografico` | Punto del mapa con identidad estable que agrupa observaciones, fotos y etiquetas | Un marcador sobre la pila central de un puente |
| `observacion` | Registro anclado a un marcador, con autor, nota y fotos | Una observación con una nota sobre una fisura y dos fotos |
| `conflicto-marcadores` | Estado válido en que dos o más marcadores caen en un mismo radio; se resuelve al cierre | Dos marcadores próximos que describen la misma junta |
| `sincronizacion` | Ciclo de dos fases que sube los cambios locales del agente y luego baja las novedades de sus relevamientos asignados | Subir 80 cambios offline y bajar las actualizaciones |
| `marca-de-sincronizacion` | Referencia opaca y monótona del punto de sincronización por relevamiento y cliente | La marca de la última bajada de un agente |
| `clave-de-idempotencia` | Valor estable que el cliente adjunta a una operación no segura para que un reintento no duplique el efecto | La clave que acompaña un alta reenviada tras un corte |
| `problem-json` | Formato uniforme de error con código estable, mensaje, estado y contexto | Un error `TRAMO_INCOMPLETO` con estado de solicitud inválida |

## 5. Qué NO hace geovial-api

Delimita la responsabilidad del consumidor frente a la de la API, para evitar expectativas falsas.

| La API no hace | Lo hace / es responsabilidad de |
| --- | --- |
| No resuelve conflictos de marcadores por su cuenta | El jefe de área decide unificar o separar al cierre (`conflictos/{id}/resolucion`) |
| No gestiona el almacenamiento seguro del token en el dispositivo ni la revalidación por seguridad del dispositivo | El cliente (la app móvil revalida con la seguridad del dispositivo; el backend solo reconoce el token vigente) |
| No dispara la sincronización ni detecta conectividad | El cliente decide cuándo sincronizar; la app móvil se apoya en la librería de sincronización |
| No analiza imágenes ni diagnostica el estado del tramo | El jefe de área evalúa manualmente; GeoVial documenta, no diagnostica |
| No expone el binario de la foto dentro del recurso ni revela credenciales del proveedor de almacenamiento | El backend delega el binario a la librería de almacenamiento; las credenciales del proveedor entran pero no salen |
| No ofrece auto-registro de usuarios | Las altas las hace el rol inmediato superior según la jerarquía |
| No entrega listados completos sin paginar | El consumidor recorre las páginas siguiendo las referencias de navegación |

## 6. Referencias cruzadas

- 05 `contratos-rest_v1.0.md`: contrato público completo del que esta explicación deriva el modelo mental (paridad con `referencia-api_v1.0.md`).
- 05 ADR-03 (autenticación/autorización), ADR-04 (paginación), ADR-05 (errores), ADR-06 (conflictos), ADR-07 (orden de sync), ADR-08 (idempotencia), ADR-10 (versionado): origen de cada decisión de diseño de §3.
- 02 `modelo-conceptual_v1.0.md` y reglas RN-01 a RN-07, RC-01 a RC-06: entidades, relaciones y restricciones del dominio.
- 03 `dx-developer-experience_v1.0.md` §4: marco Diátaxis que ubica esta explicación; `dx-error-messages_v1.0.md`: catálogo de errores previo.
- `guia-onboarding-developer_v1.0.md`, `referencia-api_v1.0.md`, `glosario-tecnico_v1.0.md`: documentos hermanos de esta categoría.

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Modelo mental inicial de geovial-api: flujo principal en cinco etapas, jerarquía de recursos y de usuarios con autorización por rol, sincronización subir-luego-bajar, tolerancia a conflictos y contratos transversales; decisiones de diseño citando ADR-03 a ADR-10, vocabulario crítico y delimitación de lo que la API no hace. |
