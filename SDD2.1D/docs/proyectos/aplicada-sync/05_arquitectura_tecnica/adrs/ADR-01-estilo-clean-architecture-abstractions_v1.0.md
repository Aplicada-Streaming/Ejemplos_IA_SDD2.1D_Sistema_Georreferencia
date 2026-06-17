# ADR-01 — Estilo Clean Architecture con capa Abstractions estable

**Proyecto:** aplicada-sync
**Documento:** ADR-01-estilo-clean-architecture-abstractions_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer
**Categoría:** Estilo

## 1. Contexto

`aplicada-sync` es un paquete distribuible y reutilizable fuera de la solución (`redistribuible: true`). Debe sincronizar cambios locales de una aplicación host contra un backend remoto sin conocer la naturaleza de esos cambios, y sin atar a quien la integra a un almacén local, un transporte ni un dominio concretos. El núcleo del motor debe poder probarse sin infraestructura y la superficie pública debe permanecer estable a través de versiones. Motivan esta decisión NB-04 (trabajo sin conexión con sincronización confiable), los seis CU (CU-01 a CU-06) que describen el contrato del ciclo de vida y el carácter redistribuible declarado en el intake §17 P.2 y P.11 del proyecto aplicada-sync.

## 2. Decisión

Se adopta Clean Architecture interna con una capa Abstractions estable en el centro. El núcleo del motor (coordinador de sesión, cola de cambios, orquestador del ciclo, ejecutores de fase, registro de estado, observador de conectividad) depende únicamente de la capa Abstractions; toda dependencia concreta (almacén local del host, contrato de transporte hacia el backend, fuente de eventos de conectividad, proveedor de credencial) se invierte y se inyecta desde el host.

## 3. Estado

Aceptado el 2026-06-15. La decisión está pre-tomada por el intake §17 P.2 y P.11 del proyecto aplicada-sync, que fija un motor de sincronización reutilizable y desacoplado del dominio de GeoVial.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Clean Architecture + capa Abstractions (elegida) | Superficie pública estable y mínima; núcleo testeable sin infraestructura; transporte y almacén sustituibles por inversión de dependencia | Costo de diseñar y mantener las abstracciones; algo más de indirección inicial |
| Capas planas con acceso directo a almacén y transporte concretos | Menos indirección; implementación más directa | Acopla el motor a un almacén y un transporte fijos; impide testear el núcleo aislado; cualquier cambio de infraestructura altera la superficie pública |
| Sincronización ad-hoc embebida en la app host (sin librería) | Cero overhead de empaquetado | Contradice el requisito de reutilización; reimplementa el orden, la idempotencia y la reanudación en cada app, multiplicando defectos |

## 5. Consecuencias positivas

- La superficie pública queda aislada de los detalles de infraestructura, habilitando la política de compatibilidad del paquete (ADR-03).
- El núcleo del motor se prueba sin almacén, transporte ni red reales, mediante dobles de las abstracciones (soporta los tests previstos en 08).
- El host puede sustituir almacén local, transporte y fuente de conectividad sin tocar el motor (habilita ADR-02 y la extensibilidad).
- Cumple el carácter agnóstico del dominio exigido por la reutilización fuera de GeoVial.

## 6. Consecuencias negativas y trade-offs

- Se acepta el costo de definir y versionar una capa Abstractions explícita en lugar de programar contra implementaciones concretas.
- Se acepta una indirección adicional en cada llamada a infraestructura a cambio de la sustituibilidad y la testeabilidad.
- El host asume la responsabilidad de proveer implementaciones correctas de las abstracciones; un adaptador defectuoso del host puede degradar al motor sin que este sea el culpable.

## 7. Implementación

Se materializa con la capa Abstractions descrita en `contratos-abstractions_v1.0.md` y con los componentes de la vista lógica de `arquitectura-solucion_v1.0.md` §3. Convención impuesta: ningún componente del núcleo referencia un adaptador concreto; toda dependencia externa entra como abstracción inyectada en la configuración de la sesión (CU-01). Los puntos de inyección se detallan en `extensibilidad_v1.0.md`.

## 8. Métricas de validación

- 100 % de los componentes del núcleo se prueban con dobles de las abstracciones, sin infraestructura real (revisión de la suite de 08).
- Cero referencias del núcleo a tipos de adaptadores concretos (verificable por revisión de dependencias de capa).
- La sustitución del adaptador de transporte o de almacén no cambia ninguna firma de la superficie pública (verificable contra la matriz de compatibilidad).

## 9. Referencias

- NB-04 (trabajo sin conexión con sincronización confiable).
- CU-01 a CU-06 (especificación funcional de 02).
- ADR-02 (inyección de dependencias del host), ADR-03 (versionado de la superficie pública).
- SOLUTION-INTAKE §17 P.2 y P.11 (aplicada-sync).
- `contratos-abstractions_v1.0.md`, `extensibilidad_v1.0.md`.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión inicial de estilo: Clean Architecture con capa Abstractions estable y pipeline subir-luego-bajar. |
