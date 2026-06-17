# ADR-03 — Estrategia de versionado del contrato público

**Proyecto:** geovial-storage
**Documento:** ADR-03-estrategia-versionado-contrato_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer
**Categoría:** Estilo

## 1. Contexto

`geovial-storage` expone una superficie pública (ADR-02) consumida por `geovial-api` dentro de la misma solución. La librería no se distribuye como paquete redistribuible (intake §13, §17.P.7): se integra al backend y se construye junto a él. Aun así, su contrato debe versionarse para que el consumidor sepa qué cambios son seguros y cuáles requieren coordinación. La especificación funcional (02 §6) ya fija qué cambios son compatibles (versión menor) y cuáles incompatibles (versión mayor); el intake (§17.P.7) adopta SemVer 2.0.0 y Conventional Commits y propone derivar la versión del tag, alineada al ciclo del backend. Falta consolidar esa estrategia como decisión arquitectónica con su mecanismo de compatibilidad hacia atrás.

## 2. Decisión

Se adopta versionado semántico (SemVer 2.0.0) del contrato público, con la siguiente clasificación, alineada a 02 §6: agregar un proveedor nuevo, una operación nueva o un parámetro opcional con valor por defecto es un cambio menor (compatible, no rompe al consumidor); cambiar la semántica de una operación, quitar una operación, quitar o renombrar un código de error, o volver obligatorio un parámetro antes opcional es un cambio mayor (incompatible) y obliga a coordinar con `geovial-api`. El identificador lógico emitido por CU-01 conserva su significado a través de versiones menores. La versión se deriva del tag del repositorio y el ciclo de versión se alinea al del backend que la integra; los cambios se rotulan con Conventional Commits.

## 3. Estado

Aceptado el 2026-06-15. SemVer 2.0.0 y Conventional Commits están fijados en el intake (§17.P.7); la clasificación de compatibilidad está fijada en la especificación funcional (02 §6).

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| SemVer derivado del tag, alineado al ciclo del backend (elegida) | Estándar conocido; clasificación clara mayor/menor; coherente con 02 §6 y con que la librería se construye junto al backend | El consumidor debe leer la clasificación para saber si un cambio lo afecta |
| Versionado independiente con publicación como paquete redistribuible | Reutilización fuera de la solución | Contradice el intake (§13: la librería no se publica como paquete redistribuible); agrega ceremonia de feed innecesaria para un consumidor único interno |
| Sin versionado explícito (seguir el del backend sin clasificar cambios) | Mínima ceremonia | Cambios incompatibles silenciosos romperían al consumidor; anti-patrón de contrato sin versionado (regla 05 §4.7) |
| Versionado del contrato vía número embebido en cada operación | Permite convivencia de versiones | Sobreingeniería para un consumidor único; complica la superficie y contradice la transparencia |

## 5. Consecuencias positivas

1. El consumidor sabe, por la versión, si un cambio es seguro de adoptar sin tocar su código.
2. La clasificación mayor/menor es objetiva y alineada con 02 §6, lo que hace revisable cada cambio.
3. Al derivar la versión del tag y alinearla al backend, no se introduce infraestructura de publicación que el intake descarta.
4. La estabilidad del identificador lógico a través de menores protege las referencias que el backend ya guardó.

## 6. Consecuencias negativas y trade-offs

1. Al alinear el ciclo al backend, una versión mayor del backend puede arrastrar a la librería aunque su contrato no cambie; se acepta a cambio de simplicidad operativa (un único artefacto).
2. No publicar como paquete impide la reutilización externa; trade-off aceptado por el intake (la reutilización transversal vive en otro proyecto redistribuible de la solución).
3. La disciplina de Conventional Commits debe sostenerse para que el cálculo de versión sea fiable.

## 7. Implementación

- Cada cambio del contrato se clasifica como mayor o menor según §2 antes de mergear; los commits siguen Conventional Commits.
- La versión se calcula a partir del tag del repositorio (herramienta concreta en intake §17.P.7).
- La compatibilidad hacia atrás se garantiza no quitando ni cambiando la semántica de operaciones ni de códigos de error existentes dentro de una línea mayor.
- La deprecación de una operación o un código se anuncia en una versión menor (marcándolo como obsoleto sin quitarlo) antes de removerlo en la siguiente mayor.
- El detalle de versionado del contrato vive en `contratos-abstractions_v1.0.md` §6.

## 8. Métricas de validación

- 0 cambios incompatibles liberados sin incremento de versión mayor.
- 100 % de los cambios del contrato clasificados y rotulados con Conventional Commits.
- El consumidor `geovial-api` puede adoptar cualquier versión menor sin cambios de código (prueba de compatibilidad en 08).

## 9. Referencias

- Especificación funcional 02 §6 (compatibilidad de versión pública).
- Intake §13 (no redistribuible), §17.P.7 (SemVer 2.0.0, Conventional Commits, versión derivada del tag alineada al backend).
- CU-01 (identificador lógico estable); RN-01 (transparencia).
- ADRs relacionadas: ADR-02 (superficie pública estable).
- `contratos-abstractions_v1.0.md` §6.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión de versionado: SemVer 2.0.0 derivado del tag y alineado al ciclo del backend, con clasificación mayor/menor según 02 §6 y compatibilidad hacia atrás. Aceptada. |
