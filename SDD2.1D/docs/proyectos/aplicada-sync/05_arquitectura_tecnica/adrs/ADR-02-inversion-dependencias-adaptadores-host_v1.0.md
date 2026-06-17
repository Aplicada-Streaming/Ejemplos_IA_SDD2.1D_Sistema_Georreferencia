# ADR-02 — Inversión de dependencias hacia adaptadores provistos por el host

**Proyecto:** aplicada-sync
**Documento:** ADR-02-inversion-dependencias-adaptadores-host_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer
**Categoría:** Extensibilidad

## 1. Contexto

El motor necesita un almacén local para la cola y los metadatos, un transporte hacia el backend remoto, una fuente de eventos de conectividad y una credencial vigente. Ninguno de esos recursos pertenece a la librería: son del host o de su plataforma. Para mantener el motor agnóstico (ADR-01) y reutilizable, esos recursos no pueden estar cableados dentro del motor. Lo motivan CU-01 (la configuración de sesión entrega almacén, backend y proveedor de credencial), CU-04 (suscripción a una fuente de conectividad) y el requisito de reutilización del intake §17 P.2.

## 2. Decisión

El motor define contratos de extensión en su capa Abstractions y recibe sus implementaciones por inyección desde el host al inicializar la sesión. El host provee adaptadores para: almacén local, transporte hacia el backend remoto, proveedor de credencial y fuente de eventos de conectividad. El motor nunca instancia un adaptador concreto por sí mismo.

## 3. Estado

Aceptado el 2026-06-15, como consecuencia directa del estilo de ADR-01 y de la forma de la configuración de sesión definida en CU-01.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Inyección de adaptadores por el host (elegida) | Motor agnóstico y reutilizable; sustituibilidad total; testeo con dobles | El host debe implementar y registrar los adaptadores correctamente |
| Adaptadores concretos por defecto incluidos en el motor | Arranque más rápido para un caso típico | Arrastra dependencias concretas a la librería; rompe la neutralidad y agranda la superficie |
| Descubrimiento automático de adaptadores en tiempo de ejecución | Menos configuración explícita del host | Comportamiento implícito difícil de razonar y de versionar; riesgo de elegir un adaptador no deseado |

## 5. Consecuencias positivas

- El mismo motor sirve a distintos hosts cambiando solo los adaptadores (cumple la reutilización fuera de GeoVial).
- Los adaptadores se sustituyen por dobles en pruebas, habilitando los tests de orden, idempotencia y reanudación sin infraestructura real.
- La superficie pública no crece con dependencias de infraestructura concretas.

## 6. Consecuencias negativas y trade-offs

- Se acepta trasladar al host la responsabilidad de proveer adaptadores válidos; un adaptador incorrecto degrada al motor.
- Se acepta más configuración explícita en la inicialización a cambio de neutralidad y sustituibilidad.
- El motor debe validar la presencia de los adaptadores obligatorios y reportar los faltantes con códigos estables (por ejemplo, recurso de conectividad ausente).

## 7. Implementación

Los contratos de extensión y su forma se definen en `contratos-abstractions_v1.0.md` y `extensibilidad_v1.0.md`. Convención: los adaptadores obligatorios (almacén local, transporte) se exigen en la configuración de sesión; los opcionales (fuente de conectividad para el modo automático) se exigen solo al habilitar la capacidad que los usa. El motor valida coherencia en CU-01 y al habilitar el disparo automático en CU-04.

## 8. Métricas de validación

- 100 % de los contratos de extensión tienen al menos un doble de prueba en la suite de 08.
- El motor rechaza con código estable toda inicialización a la que le falte un adaptador obligatorio (verificable contra CU-01 CA-02 y el catálogo de errores).
- La demostración de integración ajena al sistema (sample de 11) corre con adaptadores propios sin modificar el motor.

## 9. Referencias

- CU-01 (configuración e inicialización), CU-04 (suscripción a conectividad).
- ADR-01 (estilo y capa Abstractions).
- SOLUTION-INTAKE §17 P.2 (aplicada-sync).
- `contratos-abstractions_v1.0.md`, `extensibilidad_v1.0.md`.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión inicial de inversión de dependencias hacia adaptadores provistos por el host. |
