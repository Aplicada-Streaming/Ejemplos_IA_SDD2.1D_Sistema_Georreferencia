# ADR-02 — Superficie pública estable: una interfaz de almacenamiento única

**Proyecto:** geovial-storage
**Documento:** ADR-02-superficie-publica-estable_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer
**Categoría:** Estilo

## 1. Contexto

Por ser una `library` (regla 05 §2.2), `geovial-storage` debe exponer una superficie pública versionada y estable. El consumidor `geovial-api` programa contra esa superficie y no debe romperse cuando la librería evoluciona internamente ni cuando cambia el proveedor activo. La especificación funcional (02 §6) fija las reglas de estabilidad del contrato: agregar un proveedor o un parámetro opcional es compatible; cambiar la semántica, quitar una operación o volver obligatorio un parámetro opcional es incompatible. La transparencia (RN-01) exige además que la superficie no exponga nada dependiente del proveedor. Hace falta decidir qué forma tiene esa superficie pública y dónde se traza la frontera entre lo público (estable) y lo interno (libre de cambiar).

## 2. Decisión

Se expone una única interfaz pública de almacenamiento con las cinco operaciones de datos (guardar, recuperar, eliminar, verificar, listar) y una interfaz separada de configuración del proveedor activo, ambas declaradas en la capa de Abstracciones. Todo lo demás —adaptadores de proveedor, núcleo de enrutado, registro y resguardo de credenciales— es interno y no forma parte del contrato. Los códigos de error catalogados forman parte de la superficie pública estable; los detalles de cada proveedor, no. La superficie pública no expone ningún tipo, parámetro ni código que dependa del proveedor (RN-01).

## 3. Estado

Aceptado el 2026-06-15. Coherente con la nota de compatibilidad de versión pública de la especificación funcional (02 §6) y con la decisión de estilo de ADR-01.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Interfaz pública única de almacenamiento + interfaz de configuración separada (elegida) | Frontera nítida entre público e interno; la configuración (rol usuario raíz) no contamina el contrato de operación (rol consumidor) | Dos puntos de entrada que mantener coherentes |
| Una sola interfaz que mezcle operaciones de datos y configuración | Un único punto de entrada | Mezcla dos audiencias (consumidor y usuario raíz) y dos ritmos de cambio; CU-06 tiene reglas de autorización propias que no aplican a CU-01 a CU-05 |
| Exponer los adaptadores de proveedor como parte de la superficie pública | Acceso directo a capacidades específicas del proveedor | Rompe RN-01 (filtra el proveedor); obliga a ramas por proveedor en el consumidor; bloquea la sustitución transparente |
| Superficie por proveedor (una interfaz por destino) | Aprovecha capacidades específicas | Viola frontalmente la transparencia; multiplica el contrato; descartada |

## 5. Consecuencias positivas

1. El consumidor depende de una única interfaz estable; los cambios internos no lo afectan.
2. Separar la configuración del proveedor (CU-06, usuario raíz) de las operaciones de datos (CU-01 a CU-05, consumidor) alinea cada interfaz con su audiencia y su política de autorización.
3. La frontera pública/interna explícita permite que ADR-03 (versionado) clasifique con precisión qué cambios son compatibles.
4. Habilita la revisión de superficie pública como gate de cada cambio, evitando la erosión silenciosa del contrato.

## 6. Consecuencias negativas y trade-offs

1. Mantener dos interfaces coherentes exige cuidado para que el catálogo de errores y los tipos compartidos no diverjan.
2. Restringir la superficie a lo común entre proveedores implica no exponer capacidades específicas de un proveedor (por ejemplo, particularidades del servicio remoto), trade-off aceptado en favor de la transparencia (RN-01).
3. Cualquier necesidad futura de exponer una capacidad específica obligará a evaluarla contra RN-01 antes de incorporarla al contrato.

## 7. Implementación

- La capa de Abstracciones declara la interfaz pública de almacenamiento (CU-01 a CU-05) y la interfaz de configuración del proveedor activo (CU-06).
- El catálogo de errores uniforme (ver `dx-error-messages_v1.0.md` de 03 y `contratos-abstractions_v1.0.md`) es parte del contrato; los códigos son estables e independientes del idioma y del proveedor.
- Los componentes internos (adaptadores, núcleo, registro, resguardo) quedan fuera del ensamblado de la superficie pública o marcados como no públicos, según fije la categoría 09/10.
- Convención impuesta: ningún miembro de la superficie pública nombra ni revela un proveedor concreto.

## 8. Métricas de validación

- Estabilidad: 0 cambios incompatibles no anunciados; toda evolución se clasifica según ADR-03.
- Transparencia: la revisión de superficie pública confirma que ningún tipo ni código depende del proveedor (RN-01).
- El consumidor `geovial-api` integra la librería sin ramas de código por proveedor (NFR de transparencia, §8 de la arquitectura).

## 9. Referencias

- Regla 05 §2.2 (library expone contratos de Abstractions); especificación funcional 02 §6 (compatibilidad de versión pública).
- CU-01 a CU-06; RN-01 (transparencia).
- ADRs relacionadas: ADR-01 (estilo), ADR-03 (versionado), ADR-05 (credenciales).
- `contratos-abstractions_v1.0.md`; `arquitectura-solucion_v1.0.md` §3, §7.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión de superficie pública: interfaz única de almacenamiento más interfaz de configuración separada, con frontera pública/interna explícita y catálogo de errores como parte estable del contrato. Aceptada. |
