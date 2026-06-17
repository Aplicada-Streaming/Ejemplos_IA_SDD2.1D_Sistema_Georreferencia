# Backlog técnico — geovial-storage

**Proyecto:** geovial-storage
**Documento:** backlog-tecnico_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Scrum Master + Backlog Curator
**Tipo (D8):** library
**Técnica de estimación:** Fibonacci (1, 2, 3, 5, 8, 13)

Vista del backlog desde la lente técnica. Las trece tareas técnicas (BT-01 a BT-13) superan el mínimo de diez exigido para `library` (regla 06 §2.2) y quedan por debajo del umbral de treinta, por lo que se documentan inline (regla 06 §3.3). Cada BT declara su fuente upstream (NB, CU, ADR o contrato) y al menos una US consumidora, o se justifica como infraestructura compartida. La estimación se mantiene en Fibonacci, consistente con el product backlog.

## 1. Épicas técnicas

### EP-04 — Fundaciones del contrato y verificación de transparencia

- Objetivo: levantar la capa de Abstracciones (superficie pública), el núcleo de enrutado y validación, el catálogo de errores uniforme y la batería de contrato única que prueba la transparencia, más el gate de cobertura de CI.
- Alcance: contrato público estable y mecanismo común a todas las operaciones; no incluye la lógica específica de cada proveedor (vive en EP-03).
- Fuente upstream: ADR-01 (estilo hexagonal), ADR-02 (superficie pública estable), ADR-04 (transparencia e integridad), `contratos-abstractions_v1.0.md`, componente Núcleo de enrutado y validación y componente Abstracciones de la arquitectura; intake §17.P.6 (cobertura).
- BT contenidas: BT-04, BT-05, BT-11, BT-13.

### EP-01 — Superficie de almacenamiento (operaciones de datos)

- Objetivo: implementar las cinco operaciones de datos del contrato (guardar, recuperar, eliminar, verificar, listar) sobre el núcleo de enrutado, con su validación de entrada y su normalización de errores.
- Alcance: comportamiento observable de CU-01 a CU-05 a través de la superficie pública; integridad byte a byte y garantías de contrato (cardinalidad, idempotencia, igualdad binaria).
- Fuente upstream: CU-01 a CU-05; RN-01, RN-02; ADR-04; `contratos-abstractions_v1.0.md` §3.
- BT contenidas: BT-01, BT-02, BT-03.

### EP-02 — Configuración del proveedor activo

- Objetivo: implementar la selección, validación, activación y validación-en-seco del proveedor activo por el usuario raíz, con el resguardo de credenciales y la autorización.
- Alcance: CU-06 y sus flujos alternativos; resguardo que entra pero no sale (RN-03) y autorización a usuario raíz.
- Fuente upstream: CU-06; RN-01, RN-03; ADR-05; componentes Registro de proveedores y Resguardo de credenciales.
- BT contenidas: BT-06, BT-07.

### EP-03 — Proveedores intercambiables y punto de extensión

- Objetivo: construir los adaptadores de proveedor (local y de objetos remoto) que implementan el puerto, y el punto de extensión documentado para registrar proveedores nuevos.
- Alcance: dos adaptadores que satisfacen NB-07 (mínimo dos destinos) más el contrato del puerto; no incluye la elección de productos concretos (intake §17).
- Fuente upstream: ADR-01, ADR-04, `extensibilidad_v1.0.md`, componentes Adaptador de proveedor local y Adaptador de proveedor de objetos remoto; NB-07 (criterio ≥ 2 destinos).
- BT contenidas: BT-08, BT-09, BT-10, BT-12.

## 2. BT por épica

| BT | Título | Tipo | Prioridad | Estimación | Fuente upstream | Dependencias | Criterios de aceptación |
| --- | --- | --- | --- | --- | --- | --- | --- |
| BT-11 | Declarar la capa de Abstracciones y el puerto de proveedor | feature | Must | 5 SP | ADR-01, ADR-02, `contratos-abstractions_v1.0.md` §2-§4 | — | La superficie pública declara las dos interfaces (almacenamiento y configuración) y el puerto de proveedor; ningún tipo público nombra un proveedor concreto (RN-01); compila sin dependencias salientes hacia adaptadores. |
| BT-05 | Definir el catálogo de errores uniforme | feature | Must | 3 SP | `contratos-abstractions_v1.0.md` §5, ADR-04, RN-01 | BT-11 | Existen los códigos catalogados (CONTENIDO_VACIO, DESTINO_INVALIDO, RANGO_INVALIDO, TESTIGO_INVALIDO, CREDENCIALES_INVALIDAS, PROVEEDOR_NO_SOPORTADO, TAMANIO_EXCEDIDO, IDENTIFICADOR_INEXISTENTE, PROVEEDOR_NO_CONFIGURADO, IDENTIFICADOR_DUPLICADO, ELIMINACION_PARCIAL, PROVEEDOR_NO_DISPONIBLE, PROVEEDOR_INACCESIBLE, AUTORIZACION_INSUFICIENTE) en mayúsculas, sin tildes e independientes del proveedor; ningún error incluye credenciales (RN-03). |
| BT-04 | Implementar el núcleo de enrutado, validación de entrada y normalización de errores | feature | Must | 8 SP | Componente Núcleo de enrutado; ADR-01, ADR-04; CU-01 a CU-05 | BT-11, BT-05 | El núcleo valida contenido no vacío, formato de destino, rango, testigo y tamaño máximo antes de delegar; resuelve el proveedor activo y enruta por el puerto; mapea cualquier fallo del adaptador a un código catalogado sin filtrar detalles del proveedor. |
| BT-13 | Doble de proveedor en memoria para pruebas del núcleo | feature | Must | 3 SP | `extensibilidad_v1.0.md` §5 (adaptador mínimo de prueba); ADR-01 | BT-11 | Existe un adaptador en memoria que implementa el puerto y permite ejercitar el núcleo sin infraestructura real; sirve de plantilla para un proveedor real y habilita el gate de cobertura. |
| BT-01 | Implementar la operación de guardado con marca de sobrescritura | feature | Must | 5 SP | CU-01, RN-02, ADR-04, `contratos-abstractions_v1.0.md` §3 | BT-04 | La operación devuelve identificador y tamaño persistido; rechaza CONTENIDO_VACIO, TAMANIO_EXCEDIDO e IDENTIFICADOR_DUPLICADO; respeta la sobrescritura; no deja archivo parcial ante fallo. |
| BT-02 | Resolver la generación y colisión del identificador lógico | feature | Should | 3 SP | CU-01 (FA-01, FA-02), ADR-03 (estabilidad del identificador) | BT-01 | El identificador generado es estable y opaco; un identificador explícito existente sin sobrescritura dispara IDENTIFICADOR_DUPLICADO; el identificador conserva su significado a través de versiones menores. |
| BT-03 | Implementar recuperación (rango y solo-metadatos), eliminación, verificación y listado | feature | Must | 8 SP | CU-02, CU-03, CU-04, CU-05; RN-02; ADR-04; `contratos-abstractions_v1.0.md` §3 | BT-04 | Recuperar devuelve contenido idéntico al guardado, soporta rango y solo-metadatos; eliminar es idempotente y soporta prefijo con ELIMINACION_PARCIAL; verificar informa presencia coherente; listar pagina por testigo garantizando cardinalidad y pertenencia (no orden). |
| BT-07 | Implementar el resguardo de credenciales que entra pero no sale | feature | Must | 5 SP | ADR-05, RN-03, componente Resguardo de credenciales | BT-11 | Las credenciales entran por configuración y no se devuelven por ninguna operación ni mensaje de error; no existe operación pública que lea la configuración sensible; los adaptadores acceden solo por el resguardo. |
| BT-06 | Implementar el registro de proveedores, la activación y la validación-en-seco con autorización | feature | Must | 8 SP | CU-06, RN-01, RN-03, ADR-05; componente Registro de proveedores | BT-04, BT-07 | La activación valida soporte, formato de credenciales y conectividad/permisos antes de fijar; conserva el proveedor previo ante fallo; la validación-en-seco no cambia estado; rechaza AUTORIZACION_INSUFICIENTE para actores sin alcance de usuario raíz; PROVEEDOR_NO_SOPORTADO para proveedores no registrados. |
| BT-08 | Implementar el adaptador de proveedor local | feature | Must | 5 SP | Componente Adaptador de proveedor local; ADR-01, ADR-04; NB-07 (≥ 2 destinos) | BT-04, BT-13 | El adaptador persiste, lee, borra, comprueba presencia y enumera contra una ubicación local accesible y escribible; mapea sus fallos a los códigos uniformes; no transforma el binario (RN-02). |
| BT-09 | Implementar el adaptador de proveedor de objetos remoto | feature | Should | 8 SP | Componente Adaptador de proveedor de objetos remoto; ADR-01, ADR-04, ADR-05; NB-07 (≥ 2 destinos) | BT-06, BT-08 | El adaptador implementa el puerto contra un servicio de objetos remoto accediendo a credenciales solo por el resguardo; mapea sus fallos a los códigos uniformes sin filtrar configuración (RN-03); pasa la misma batería de contrato que el local. |
| BT-12 | Implementar transferencia por tramos (rango y paginación) uniforme entre proveedores | feature | Should | 5 SP | ADR-04 (no materializar contenidos/listados completos), CU-02, CU-05; NFR de latencia §8 | BT-03, BT-08 | La recuperación por rango y el listado por testigo no materializan el contenido ni el listado completo en memoria; el comportamiento es idéntico entre proveedor local y remoto. |
| BT-10 | Batería de pruebas de contrato única por proveedor y gate de cobertura | devops | Must | 5 SP | ADR-04, ADR-01; NFR de transparencia y cobertura §8; intake §17.P.6, §17.P.8 | BT-08, BT-09, BT-13 | Una batería única se ejecuta contra cada proveedor soportado y produce resultados equivalentes y el mismo conjunto de códigos para las mismas entradas; el gate de CI exige líneas ≥ 80 % y branches ≥ 70 % y bloquea si no se cumple; un proveedor nuevo registrado debe pasarla sin tocar el núcleo. |

Notas de prioridad y tipo:

- No hay spikes: las decisiones de estilo, superficie, versionado, transparencia y credenciales ya están cerradas en ADR-01 a ADR-05 (estado Aceptado), por lo que no queda investigación abierta que justifique una caja temporal.
- BT-09 (adaptador remoto) y BT-12 (transferencia por tramos) son Should: el MVP de transparencia se demuestra con el proveedor local y el doble en memoria como segundo destino verificable; el remoto eleva la prueba de transparencia a un destino de capacidades dispares pero no bloquea el MVP.
- BT-02 es Should porque la colisión y sobrescritura refinan la operación de guardado (US-02 Should) sin condicionar el guardado básico (US-01 Must).

## 3. Trazabilidad BT↔US↔CU

| BT | Tipo | US consumidoras | CU upstream | Fuente upstream principal |
| --- | --- | --- | --- | --- |
| BT-01 | feature | US-01, US-02 | CU-01 | CU-01, RN-02, ADR-04 |
| BT-02 | feature | US-01, US-02 | CU-01 | CU-01 (FA-01/FA-02), ADR-03 |
| BT-03 | feature | US-03, US-04, US-05, US-06, US-07 | CU-02, CU-03, CU-04, CU-05 | CU-02 a CU-05, RN-02, ADR-04 |
| BT-04 | feature | US-01, US-03, US-05, US-06 | CU-01 a CU-05 | Núcleo de enrutado, ADR-01, ADR-04 |
| BT-05 | feature | US-01, US-03 | CU-01 a CU-06 (catálogo común) | `contratos-abstractions` §5, RN-01 |
| BT-06 | feature | US-08, US-09 | CU-06 | CU-06, Registro de proveedores |
| BT-07 | feature | US-03, US-08 | CU-06 (soporte CU-02, CU-05) | ADR-05, RN-03 |
| BT-08 | feature | US-01, US-03, US-05, US-06, US-07, US-08 | CU-01 a CU-05 | Adaptador local, NB-07 |
| BT-09 | feature | US-03, US-08 | CU-01 a CU-05 | Adaptador remoto, ADR-05 |
| BT-10 | devops | US-01, US-03, US-08 (infraestructura compartida de verificación) | CU-01 a CU-06 | ADR-04, intake §17.P.6/§17.P.8 |
| BT-11 | feature | US-01, US-03, US-08 (infraestructura compartida del contrato) | CU-01 a CU-06 | ADR-01, ADR-02 |
| BT-12 | feature | US-04, US-07 | CU-02, CU-05 | ADR-04, NFR latencia |
| BT-13 | feature | US-01, US-03 (infraestructura compartida de prueba) | CU-01 a CU-05 | `extensibilidad` §5, ADR-01 |

Justificación de las BT de infraestructura compartida (regla 06 §4.5, anti-patrón "BT sin US consumidora"):

- BT-10 (batería de contrato y gate de cobertura) da soporte a todas las US Must que requieren transparencia verificada; su fuente es ADR-04 y el intake §17.P.6/§17.P.8. Se asocia explícitamente a US-01, US-03 y US-08 como consumidoras representativas.
- BT-11 (capa de Abstracciones y puerto) es la fundación del contrato que toda US consume; se ancla en ADR-01 y ADR-02 y se asocia a US-01, US-03 y US-08.
- BT-13 (doble en memoria) habilita la prueba del núcleo sin infraestructura real y el gate de cobertura; se ancla en `extensibilidad_v1.0.md` §5 y ADR-01.

## 4. Documentos relacionados

- Vista de producto: [product-backlog_v1.0.md](product-backlog_v1.0.md) — épicas, US-01 a US-09, MoSCoW y métricas.
- Filtro de entrada: [definition-of-ready_v1.0.md](definition-of-ready_v1.0.md) — DoR para US y BT.
- Índice de la sección: [README.md](README.md).

## 5. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Backlog técnico inicial de geovial-storage: cuatro épicas técnicas (fundaciones del contrato, superficie de almacenamiento, configuración del proveedor, proveedores intercambiables), trece BT inline (BT-01 a BT-13, por encima del mínimo de diez para library) con tipo, prioridad, estimación Fibonacci, fuente upstream, dependencias y criterios, y matriz cruzada BT↔US↔CU. |
