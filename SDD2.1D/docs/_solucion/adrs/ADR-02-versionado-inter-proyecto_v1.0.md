# ADR-02 — Política de versionado inter-proyecto

**Proyecto:** GeoVial (solución)
**Documento:** ADR-02-versionado-inter-proyecto_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Soluciones Senior
**Categoría:** Despliegue

## 1. Contexto

Los cinco proyectos de GeoVial se versionan de forma independiente (SemVer 2.0.0 y Conventional Commits, intake §17.P.7 de cada proyecto), pero están encadenados por cuatro contratos que cruzan fronteras de proyecto. Un cambio en un proyecto productor puede romper a su consumidor si no se coordina el orden de publicación y la compatibilidad. El caso más sensible es la librería redistribuible `aplicada-sync` (`redistribuible: true`), que se publica como paquete desde un repositorio externo y que la app móvil consume por referencia de versión: si la app referencia una versión del paquete que aún no está publicada, la construcción del consumidor falla.

El manifiesto fija un orden topológico de construcción en tres niveles (nivel 0: `aplicada-sync`, `geovial-storage`; nivel 1: `geovial-api`; nivel 2: `geovial-web`, `geovial-mobile`). La política de versionado de nivel solución debe garantizar que ese orden se respete también en la publicación, no solo en la compilación, y que cada productor mantenga compatibilidad hacia atrás dentro de su versión mayor para no obligar a desplegar a todos sus consumidores a la vez.

## 2. Decisión

Cada proyecto productor publica antes que sus consumidores y mantiene compatibilidad hacia atrás dentro de la misma versión mayor de su contrato. En particular: el redistribuible `aplicada-sync` se publica al feed de paquetes antes de que `geovial-mobile` referencie su versión; `geovial-storage` se construye e integra al backend antes de publicar la imagen de `geovial-api`; y `geovial-api` publica una versión nueva de su contrato REST conservando la versión mayor previa durante un período de convivencia de al menos un MINOR antes de retirarla, de modo que `geovial-web` y `geovial-mobile` puedan migrar de forma escalonada.

## 3. Estado

Aceptado el 2026-06-15. Gobierna la gestión de versiones de los paquetes compartidos y del redistribuible; se referencia desde `vista-solucion_v1.0.md` §5 y §6.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Productor publica antes que el consumidor + compatibilidad hacia atrás por versión mayor (elegida) | Respeta el orden topológico en la publicación; evita roturas por referencia a versión inexistente; permite migración escalonada de los consumidores | Exige disciplina de orden de publicación y un período de convivencia de versiones del contrato |
| Versionado conjunto de toda la solución (una versión única para los cinco proyectos) | Trazabilidad simple de qué versiones componen un release | Obliga a re-publicar y re-desplegar proyectos sin cambios; rompe el versionado independiente del manifiesto y la reutilización del redistribuible fuera de la solución |
| Sin política de compatibilidad (cada productor cambia libremente) | Cero overhead de coordinación | Cualquier cambio del productor rompe silenciosamente a sus consumidores; inviable con un redistribuible externo |
| Fijar versiones por rango flexible en los consumidores | Menos actualizaciones manuales de referencias | Un cambio menor del productor podría alterar el comportamiento del consumidor sin control; reduce la reproducibilidad de la construcción |

## 5. Consecuencias positivas

1. La construcción del consumidor nunca falla por referenciar una versión del productor que aún no existe: el orden topológico se honra también en la publicación.
2. Los consumidores migran de versión mayor del contrato de forma escalonada gracias al período de convivencia, sin un despliegue coordinado de toda la solución.
3. El redistribuible `aplicada-sync` mantiene su valor de reutilización fuera de la solución: su versionado no queda atado al calendario de release de GeoVial.
4. La trazabilidad de qué versión de cada contrato consume cada proyecto queda explícita en la vista de solución.

## 6. Consecuencias negativas y trade-offs

1. Exige disciplina de pipeline: el orden de publicación (librerías → backend → clientes) debe estar codificado en CI/CD, no librado al operador. Se acepta a cambio de evitar roturas en cadena.
2. Mantener una versión mayor previa de un contrato durante el período de convivencia duplica temporalmente la superficie a sostener y probar. Se acepta para permitir la migración escalonada.
3. La verificación post-publicación del redistribuible (restaurar el paquete en un proyecto limpio, intake §17.P.8 de `aplicada-sync`) agrega un paso al pipeline. Se acepta como red de seguridad contra publicaciones rotas.

## 7. Implementación

- Orden de publicación en CI/CD: primero el redistribuible `aplicada-sync` (publicación al feed y verificación post-publish en proyecto limpio), luego `geovial-storage` integrada y la imagen de `geovial-api`, y por último las imágenes y el paquete de `geovial-web` y `geovial-mobile`.
- Compatibilidad del contrato REST: gobernada por la política de versionado por URI del productor (`geovial-api` `contratos-rest_v1.0.md` §6 y ADR-10 del proyecto); convivencia de la versión mayor previa de al menos un MINOR.
- Compatibilidad del contrato de la librería de almacenamiento: cambios menores (agregar proveedor, operación u opción) no rompen al backend; cambios incompatibles coordinan con el consumidor (`geovial-storage` `contratos-abstractions_v1.0.md` §6).
- Compatibilidad del contrato de sincronización: versionado semántico de la superficie pública; ningún elemento se remueve sin período de deprecación y versión mayor (`aplicada-sync` `contratos-abstractions_v1.0.md` §6); el consumidor fija la versión mayor que integra.
- Rollback inter-proyecto: cada productor revierte a su versión previa (redepliegue de imagen o unlist del paquete) sin obligar a revertir a sus consumidores mientras la versión mayor del contrato no cambie.

## 8. Métricas de validación

- Cero fallos de construcción del consumidor por versión del productor ausente en el feed, verificable en el historial del pipeline.
- 100 % de los cambios menores del contrato de un productor sin requerir cambios en el código del consumidor (contract tests del consumidor en verde tras un cambio menor del productor, 08).
- El paquete redistribuible publicado se restaura y ejercita en un proyecto limpio antes de habilitar su consumo (verificación post-publish, intake §17.P.8 de `aplicada-sync`).

## 9. Referencias

- Manifiesto: `SOLUTION-MANIFEST-geovial_v1.0.md` §2 (`redistribuible`), §3 (orden topológico).
- Intake: `SOLUTION-INTAKE-geovial_v1.0.md` §15 (orden de construcción), §17.P.7 y §17.P.8 de cada proyecto (SemVer, feed, verificación post-publish).
- Contratos de los productores: `proyectos/geovial-api/.../contratos-rest_v1.0.md` §6; `proyectos/geovial-storage/.../contratos-abstractions_v1.0.md` §6; `proyectos/aplicada-sync/.../contratos-abstractions_v1.0.md` §6.
- ADRs de nivel solución relacionados: ADR-01 (estilo de composición), ADR-03 (comunicación entre proyectos).
- Vista de solución: `vista-solucion_v1.0.md` §5, §6, §7.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Política inicial de versionado inter-proyecto: el productor publica antes que el consumidor; compatibilidad hacia atrás por versión mayor con período de convivencia; el redistribuible se publica y verifica antes de su consumo. Para ADR aceptadas, la única edición permitida es el cambio de estado a Superado por ADR-YY. |
