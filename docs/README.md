# Documentación de GeoVial

Esta carpeta reúne documentación de **ingeniería** del proyecto (técnicas, decisiones y guías
prácticas). Es complementaria a la documentación de la **metodología SDD 2.1**, que vive en
[SDD2.1D/docs/](../SDD2.1D/docs/) (visión, necesidades de negocio, casos de uso, reglas de negocio,
arquitectura por proyecto, etc.).

## Notas técnicas

Notas autocontenidas sobre las técnicas que usa la solución, ancladas al código real del repositorio.

| Nota | De qué trata |
|---|---|
| [Prueba de adaptadores con dobles/mocks](tecnicas/pruebas-de-adaptadores-con-mocks.md) | Aislar un componente de su colaborador externo con dobles de prueba; mock vs. fake; ejemplos con el adaptador S3 y NSubstitute. |
| [Puertos y adaptadores (arquitectura hexagonal)](tecnicas/puertos-y-adaptadores.md) | Separar el núcleo de las tecnologías externas detrás de contratos; cómo lo aplica GeoVial (almacenamiento, sincronización, persistencia). |
| [Idempotencia con clave (Idempotency-Key)](tecnicas/idempotencia-con-clave.md) | Reintentos seguros de operaciones no seguras; el middleware de idempotencia y la idempotencia por identificador de origen en la sincronización. |
| [Sincronización subir-luego-bajar](tecnicas/sincronizacion-subir-luego-bajar.md) | Protocolo de sincronización offline del agente de campo: orden, idempotencia, marca monótona y convivencia con conflictos. |

## Cómo se relacionan con el código

- Las técnicas se materializan en `src/` (`GeoVial.WebApi`, `GeoVial.Storage`, `GeoVial.Web`,
  `Aplicada.Sync`) y se verifican en `tests/`.
- Cada nota enlaza los archivos concretos que ilustran la técnica, de modo que se pueda ir del
  concepto al código y a sus pruebas.
