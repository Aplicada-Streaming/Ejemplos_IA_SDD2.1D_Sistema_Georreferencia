# Guía de testing de extensibilidad — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** guia-testing-extensibilidad_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero QA / SDET Senior (AG-08), variante QA + SDET Library

## 1. Por qué existe esta guía

`aplicada-sync` tiene puntos de extensión (`tiene_extensibilidad=true`, intake §17 P.2 y `extensibilidad_v1.0.md`): el motor define cuatro contratos de extensión (estrategias) que el host implementa e inyecta sin tocar el núcleo. El tipo D8 `library` con extensiones obliga a producir esta guía (08_rules §2.2 y §6). El objetivo es describir cómo testear esos puntos de extensión de forma que el núcleo permanezca intacto y que tanto el equipo de la librería como el integrador externo puedan verificar que el motor opera con adaptadores propios.

Los cuatro puntos de extensión (de `extensibilidad_v1.0.md` §3) son: estrategia de almacén local (obligatoria), estrategia de transporte (obligatoria), proveedor de credencial (opcional) y fuente de eventos de conectividad (opcional). El motor programa contra la abstracción y nunca instancia un adaptador concreto (ADR-01, ADR-02).

## 2. Principio: testear el punto de extensión sin tocar el núcleo

El núcleo del motor se prueba contra dobles de las estrategias; las estrategias concretas se prueban contra el contrato que el motor espera. La extensibilidad se testea en dos direcciones complementarias, sin modificar el código de producción del núcleo:

- Contract tests por interfaz (hacia el motor): verifican que cualquier implementación de una estrategia respeta la forma de datos y los códigos de error del contrato (`contratos-abstractions_v1.0.md` §4 y §5). Son la red que detecta una deriva entre el contrato y un adaptador.
- Tests de operación del motor con adaptadores del integrador (hacia el host): verifican que el motor ejecuta un ciclo completo sin modificación usando estrategias provistas por un integrador, sustituyendo los dobles de la librería por adaptadores ajenos al dominio de GeoVial (el sample de demostración del intake §18).

## 3. Cómo testear cada punto de extensión

Cada punto de extensión se prueba con un doble controlable y un conjunto de contract tests que cubren el camino feliz, el rechazo por contrato y la condición transitoria. Los dobles viven en el módulo de soporte de tests centralizado (`estrategia-testing_v1.0.md` §5).

| Punto de extensión | Doble usado | Qué verifica el contract test | Error si falta / si viola el contrato | TC relacionado |
| --- | --- | --- | --- | --- |
| Estrategia de almacén local (obligatoria) | Doble en memoria y doble persistente efímero | Persiste y lee la cola y la marca de progreso conservando el orden de creación y la unicidad por identificador; reanuda desde la marca persistida | ALMACEN_LOCAL_INACCESIBLE; CONFIGURACION_INCOMPLETA si falta al inicializar | TC-04, TC-05, TC-10, TC-14 |
| Estrategia de transporte (obligatoria) | Doble parametrizable (confirma / falla / corta / reporta conflicto) | Reconoce el identificador de cambio estable para idempotencia efectiva (RN-02; ADR-07); reporta el conflicto como condición no bloqueante (RN-03; ADR-08); entrega actualizaciones posteriores a una marca | CONFIGURACION_INCOMPLETA; en ejecución BACKEND_INALCANZABLE | TC-07, TC-09, TC-11, TC-16 |
| Proveedor de credencial (opcional) | Doble que entrega credencial vigente o vencida | Su ausencia produce estado no autenticada; su presencia habilita ejecutar; nunca se persiste más allá de la fase que la requiere | SESION_NO_AUTENTICADA / CREDENCIAL_INVALIDA | TC-03, TC-15 |
| Fuente de eventos de conectividad (opcional) | Doble que emite transiciones programadas (disponible / pérdida / rebote) | Dispara a lo sumo un ciclo ante recuperación; ignora eventos redundantes durante un ciclo; su ausencia impide el modo automático | FUENTE_CONECTIVIDAD_AUSENTE | TC-15, TC-18 |

## 4. Verificar el rechazo por estrategia obligatoria ausente

Un caso central de la extensibilidad es que el motor rechace con código estable cuando falta una estrategia obligatoria, sin instanciar nada por su cuenta (no hay descubrimiento automático; el registro es explícito, ADR-02 / `extensibilidad_v1.0.md` §5). Se testea:

- Inicializar sin estrategia de almacén local: el motor responde CONFIGURACION_INCOMPLETA y no crea sesión (cubierto por TC-02 extendido a la estrategia faltante).
- Inicializar sin estrategia de transporte: el motor responde CONFIGURACION_INCOMPLETA.
- Habilitar el disparo automático sin fuente de conectividad suscripta: el motor responde FUENTE_CONECTIVIDAD_AUSENTE.

Estos tests confirman la regla de registro explícito: ninguna estrategia se resuelve de forma implícita.

## 5. Verificar que el motor opera con adaptadores del integrador

El test de mayor nivel de la extensibilidad sustituye los dobles de la librería por adaptadores ajenos al dominio de GeoVial, replicando lo que hace el sample de demostración MAUI del intake §18 y §16.1 (categoría 11, `/samples/aplicada-sync/03-avanzado-demo-maui/`). Se verifica:

- Un ciclo completo subir-luego-bajar ejecuta sin modificar el núcleo, usando un almacén local y un transporte implementados por el integrador.
- El orden (RN-01), la idempotencia (RN-02) y la convivencia con conflicto (RN-03) se mantienen con adaptadores ajenos, no solo con los dobles de la librería.
- El motor no asume conocimiento del dominio del integrador: la carga útil sigue siendo opaca.

Este test es la evidencia de que el punto de extensión principal (el motor reutilizable) funciona fuera de la solución GeoVial. Su ejecución se ancla en la categoría 11 (al menos un test ejecutable por sample, 08_rules §3.3 downstream); esta guía define qué se verifica, no la implementación del sample.

## 6. Estabilidad y versionado de los contratos de extensión

Los contratos de extensión forman parte de la superficie pública (`extensibilidad_v1.0.md` §4; ADR-03). El testing de extensibilidad protege esa estabilidad:

- Agregar un punto de extensión opcional es compatible: se testea que las estrategias existentes siguen funcionando sin cambios (no regresión del contrato).
- Quitar o cambiar la firma de un punto de extensión existente es un cambio mayor: el snapshot del contrato (TC-21) lo detecta y debe corresponder a un incremento de versión mayor.
- Cada cambio en un contrato de extensión obliga a actualizar el doble correspondiente en el mismo PR, de modo que el contract test detecte la deriva (`estrategia-testing_v1.0.md` §5).

## 7. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Puntos de extensión | Estrategia de almacén local, estrategia de transporte, proveedor de credencial, fuente de conectividad (`extensibilidad_v1.0.md` §3) |
| CU cubiertos | CU-01, CU-02, CU-03, CU-04, CU-05, CU-06 |
| RN verificadas | RN-01, RN-02, RN-03 |
| ADRs | ADR-01 (capa Abstractions), ADR-02 (inversión de dependencias), ADR-03 (versionado de contratos de extensión), ADR-07 (transporte reconoce identificador), ADR-08 (transporte reporta conflicto) |
| TC relacionados | TC-02 (rechazo por obligatoria ausente), TC-03, TC-04, TC-05, TC-07, TC-09, TC-10, TC-11, TC-14, TC-15, TC-16, TC-18, TC-21 |
| Downstream | Categoría 11: sample `03-avanzado-demo-maui` y samples `01-basico`/`02-intermedio` con al menos un test ejecutable cada uno |

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Guía inicial de testing de extensibilidad de aplicada-sync: principio de testear el punto de extensión sin tocar el núcleo (contract tests por interfaz más operación del motor con adaptadores del integrador), cómo testear cada una de las cuatro estrategias con su doble y su TC, verificación del rechazo por estrategia obligatoria ausente con registro explícito, validación del motor con adaptadores ajenos a GeoVial replicando el sample del intake §18, y protección de la estabilidad y el versionado de los contratos de extensión. Derivada de `extensibilidad_v1.0.md`, de los ADR-01/02/03/07/08 y de las reglas 08 §2.2/§6. |
