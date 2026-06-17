# ADR-01 — Abstracción de almacenamiento con proveedores intercambiables por estrategia

**Proyecto:** geovial-storage
**Documento:** ADR-01-abstraccion-proveedores-intercambiables_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer
**Categoría:** Estilo

## 1. Contexto

El backend consumidor necesita guardar y recuperar las fotografías de los relevamientos sin atarse al destino físico donde quedan alojadas, y el negocio necesita poder cambiar ese destino (proveedor local, proveedor de almacenamiento de objetos remoto u otro) según costo, capacidad y contexto del despliegue, sin que el consumidor ni los demás roles noten diferencia. Esta es la razón de ser de la librería (NB-07) y su invariante central (RN-01, transparencia). Las operaciones del contrato son CU-01 a CU-05 (guardar, recuperar, eliminar, verificar, listar) y la selección del proveedor activo es CU-06. El intake (§17.P.2, §17.P.11) prefija un estilo de abstracción con proveedores intercambiables y descarta tanto acoplar el acceso al destino físico al backend como integrar un único proveedor fijo. La librería debe poder probarse sin infraestructura real y admitir proveedores nuevos sin reescribir el núcleo (intake §17.P.11, extensibilidad).

## 2. Decisión

Se adopta una arquitectura hexagonal (puertos y adaptadores) en la que la capa de Abstracciones define la superficie pública y un puerto de proveedor de almacenamiento, y cada proveedor concreto es un adaptador que implementa ese puerto, seleccionable en tiempo de ejecución mediante el patrón estrategia. El núcleo de enrutado y validación depende solo del puerto, nunca de un proveedor concreto. El proveedor activo se resuelve a partir de la configuración fijada por el usuario raíz (CU-06).

## 3. Estado

Aceptado el 2026-06-15. Decisión pre-tomada en el intake (§17.P.11): proveedores configurables seleccionables por el usuario raíz y transparencia hacia el resto del sistema.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Hexagonal con puerto de proveedor y estrategia (elegida) | Transparencia garantizada por la estructura (RN-01); núcleo testeable con dobles; punto de extensión limpio para proveedores nuevos | Una indirección por puerto; disciplina para no filtrar detalles del proveedor |
| Capas clásicas (Domain, Application, Infrastructure, Presentation) | Patrón conocido; separación de responsabilidades | La dependencia tiende hacia la infraestructura; la transparencia queda sujeta a disciplina y no a la estructura; ceremonia innecesaria para una librería sin UI |
| Acceso directo al destino físico acoplado al backend | Mínima indirección; menos código | Ata al consumidor a un único destino; viola RN-01; obliga a ramas por proveedor; descartada explícitamente por el intake |
| Un único proveedor fijo dentro de la librería | Simplicidad de implementación | Incumple NB-07 (mínimo dos destinos); elimina el punto de extensión |

## 5. Consecuencias positivas

1. La transparencia (RN-01) queda garantizada por la estructura: el consumidor invoca el contrato sin saber qué proveedor está activo.
2. Agregar un proveedor nuevo se reduce a implementar el puerto y registrarlo, sin tocar el núcleo (ver `extensibilidad_v1.0.md`).
3. El núcleo se prueba con un doble del puerto en memoria, sin infraestructura real, habilitando el gate de cobertura del intake §17.P.6.
4. La batería de pruebas de contrato puede ejecutarse igual contra cada proveedor, lo que verifica la transparencia de forma automática.

## 6. Consecuencias negativas y trade-offs

1. Se acepta el costo de una capa de abstracción adicional (una indirección por puerto) a cambio de independizar al sistema del proveedor (trade-off declarado en intake §17.P.12).
2. Mantener la transparencia exige disciplina de revisión: ningún adaptador debe filtrar detalles propios hacia el contrato (mitigado por ADR-02 y por la batería de contrato).
3. El puerto debe ser lo bastante general para todos los proveedores y lo bastante específico para ser útil; un puerto mal dimensionado obligaría a cambios incompatibles (gobernado por ADR-02 y ADR-03).

## 7. Implementación

- La capa de Abstracciones declara la superficie pública (operaciones de CU-01 a CU-06), el puerto de proveedor de almacenamiento y el catálogo de errores uniforme. No tiene dependencias salientes.
- El núcleo de enrutado valida la entrada (contenido no vacío, formato de destino, rango, testigo), resuelve el proveedor activo desde el registro de proveedores y delega en el puerto; normaliza cualquier error del adaptador a un código catalogado.
- Cada proveedor (local, de objetos remoto, otros) implementa el puerto como adaptador independiente.
- El registro de proveedores resuelve la estrategia activa según la configuración de CU-06.
- Convención impuesta: ningún tipo ni código de error de la superficie pública puede referirse a un proveedor concreto (RN-01).

## 8. Métricas de validación

- Transparencia: la batería de contrato única produce resultados equivalentes y el mismo conjunto de códigos de error contra cada proveedor soportado (NFR de transparencia, §8 de la arquitectura).
- Extensibilidad: un proveedor nuevo registrado pasa la batería de contrato sin modificar el núcleo.
- Cobertura: líneas ≥ 80 % y branches ≥ 70 % en el gate de CI (intake §17.P.6).

## 9. Referencias

- NB-07 (almacenamiento de archivos configurable); NB-03 y NB-06 de soporte.
- CU-01 a CU-06; RN-01 (transparencia).
- Intake §17.P.2 (estilo), §17.P.11 (decisión pre-tomada), §17.P.12 (trade-off de la capa de abstracción).
- ADRs relacionadas: ADR-02 (superficie pública estable), ADR-04 (transparencia del proveedor), ADR-05 (manejo seguro de credenciales).
- `arquitectura-solucion_v1.0.md` §2, §3; `extensibilidad_v1.0.md`.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión de estilo: arquitectura hexagonal con abstracción de almacenamiento y proveedores intercambiables por estrategia. Aceptada (pre-tomada en intake §17.P.11). |
