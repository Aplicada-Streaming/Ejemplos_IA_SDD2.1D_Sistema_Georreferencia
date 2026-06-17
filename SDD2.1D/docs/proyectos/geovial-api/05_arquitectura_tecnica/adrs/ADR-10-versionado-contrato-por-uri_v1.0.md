# ADR-10 — Versionado del contrato público por URI

**Proyecto:** geovial-api
**Documento:** ADR-10-versionado-contrato-por-uri_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer
**Categoría:** Comunicación

## 1. Contexto

El contrato REST del backend es consumido por dos clientes —el front web y la app móvil— que evolucionan con cadencias propias (intake §14). Un cambio incompatible del contrato no debe romperlos de forma silenciosa: la versión previa debe permanecer disponible mientras los clientes migran (CU-22). El intake fija el versionado por URI con prefijo de versión mayor en la ruta y una política de breaking changes que mantiene la versión previa al menos un MINOR antes de removerla (§17.P.3). La app móvil, al ser offline-first, puede tardar en actualizarse, lo que refuerza la necesidad de convivencia de versiones. Cubre CU-22.

## 2. Decisión

Se adopta versionado del contrato público por URI: cada recurso se expone bajo un prefijo de versión mayor en la ruta. Un cambio compatible (agregar un campo opcional, un recurso o un valor adicional) se incorpora dentro de la misma versión mayor sin romper a los clientes. Un cambio incompatible (quitar un campo, volver obligatorio uno opcional, cambiar la semántica, quitar o renombrar un código de error) se publica como una versión mayor nueva, conservando la anterior durante un período de convivencia. El backend atiende ambas versiones mayores durante la convivencia y comunica el plan de retiro. Una versión retirada o inexistente se rechaza con VERSION_NO_SOPORTADA; un recurso ausente en la versión indicada, con RECURSO_NO_EN_VERSION.

## 3. Estado

Aceptado el 2026-06-15. Decisión pre-tomada en el intake (§17.P.3, §17.P.11) y derivada de CU-22.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Versionado por URI con prefijo de versión mayor (elegido) | Explícito y legible en la ruta; convivencia de versiones simple de enrutar; fijado por el intake | La ruta cambia entre versiones mayores; el cliente debe migrar de ruta |
| Versionado por cabecera de contenido | La ruta no cambia | Menos visible y más fácil de omitir por el cliente; descartado frente al requisito explícito de versión en la ruta (§17.P.3) |
| Sin versionado (un único contrato evolutivo) | Sin convivencia que mantener | Cualquier cambio incompatible rompe a los clientes; inviable con una app móvil offline-first que tarda en actualizarse; descartado por CU-22 |
| Versionado por parámetro de consulta | Fácil de agregar | Mezcla versión con filtros; ambiguo para el enrutado y el cacheo; menos estándar que el prefijo de ruta |

## 5. Consecuencias positivas

1. Ninguna evolución incompatible del contrato rompe a un cliente que permanece en su versión durante el período de convivencia (CU-22, garantía).
2. Los cambios compatibles se incorporan sin afectar a los clientes existentes (CU-22, CA-01).
3. La versión en la ruta es explícita y verificable, y simplifica el enrutado de la convivencia.
4. Protege la dependencia de `geovial-web` y `geovial-mobile` sobre el contrato del backend (intake §14).

## 6. Consecuencias negativas y trade-offs

1. La ruta cambia entre versiones mayores, obligando al cliente a migrar de prefijo; se acepta a cambio de la claridad y la convivencia.
2. Mantener dos versiones mayores durante la convivencia duplica temporalmente la superficie a probar; se acota con la política de retiro tras al menos un MINOR (§17.P.3).
3. La clasificación de cada cambio como compatible o incompatible exige disciplina; un error de clasificación rompería a un cliente (mitigado por los contract tests de CI).

## 7. Implementación

- El servicio de versionado del contrato (capa de API) resuelve la versión mayor del prefijo de la ruta y enruta a la implementación de esa versión.
- La política de compatibilidad y de retiro se declara en `contratos-rest_v1.0.md` §6, alineada con SemVer del proyecto (intake §17.P.7).
- Los errores de versión (VERSION_NO_SOPORTADA, VERSION_REQUERIDA_AUSENTE, RECURSO_NO_EN_VERSION) se devuelven como problem+json (ADR-05).
- Convención impuesta: ningún cambio incompatible se publica dentro de una versión mayor existente; siempre se crea una versión nueva conservando la anterior durante la convivencia.

## 8. Métricas de validación

- Cambio compatible que no rompe al cliente existente; cambio incompatible que publica una versión nueva conservando la anterior (CU-22, verificado en 08).
- Versión retirada rechazada con VERSION_NO_SOPORTADA informando las versiones vigentes; recurso ausente en versión rechazado con RECURSO_NO_EN_VERSION.
- 100 % de los endpoints públicos cubiertos por un contract test por versión vigente (intake §17.P.6).

## 9. Referencias

- NB-01 a NB-05; CU-22; protege la dependencia de los clientes (intake §14).
- Intake §17.P.3 (versionado por URI), §17.P.7 (SemVer), §17.P.11.
- ADRs relacionadas: ADR-05 (errores de versión), ADR-04 (paginación dentro de la versión).
- `contratos-rest_v1.0.md` §6; `arquitectura-solucion_v1.0.md` §7.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión de comunicación: versionado del contrato público por URI con prefijo de versión mayor, convivencia de la versión previa y política de retiro tras al menos un MINOR. Aceptada (pre-tomada en intake §17.P.3, §17.P.11). |
