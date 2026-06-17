# Estrategia de testing — geovial-storage

**Proyecto:** geovial-storage
**Documento:** estrategia-testing_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero QA / SDET Senior (variante QA + SDET Library)

## 1. Pirámide de testing deseada

`geovial-storage` es del tipo `library`: pura transformación entrada-salida sobre una superficie pública, sin interfaz de usuario ni ambiente desplegado. Se adopta la pirámide objetivo de la regla 08 §2.2 para `library`: 80 unit / 15 integration / 5 e2e.

| Nivel | Qué cubre | Tooling (rol abstracto) | Porcentaje objetivo |
| --- | --- | --- | --- |
| Unit | Lógica aislada del núcleo de enrutado, validación de entrada, normalización de errores, resolución de proveedor activo y resguardo de credenciales, probada contra el doble de proveedor en memoria (BT-13) | Framework de tests unitarios del runtime objetivo; framework de property-based testing | 80 % |
| Integration | Contract tests por proveedor (la batería única de transparencia, RN-01), igualdad binaria guardar-recuperar contra un proveedor real efímero, paginación y rango sobre streams reales | Corredor de contract tests parametrizado por proveedor; ambiente efímero del proveedor local | 15 % |
| E2E / snapshot | Snapshot del catálogo de códigos de error uniforme y de la forma de la superficie pública (firma estable del contrato, ADR-02/ADR-03), para detectar cambios incompatibles no intencionales | Framework de snapshot testing equivalente | 5 % |

Justificación contra la pirámide invertida: no hay journey end-to-end con interfaz, así que un e2e pesado no aporta valor y solo agregaría fragilidad; el e2e se reduce a snapshots de contrato. Justificación contra la pirámide aplanada: un número global de cobertura escondería que los getters triviales están al 100 % mientras el núcleo de enrutado y la normalización de errores quedan sin cubrir; por eso la cobertura se reporta por capa (§2) y se complementa con mutation testing en dominio.

Los contract tests cuentan como integración (regla 08 §2.2 lo admite explícitamente para los tipos con contrato): la batería única de RN-01 es el corazón de la verificación de transparencia y se ejecuta contra cada proveedor soportado.

## 2. Cobertura mínima por capa

La arquitectura de 05 es hexagonal (puertos y adaptadores). El mapeo a las capas de la regla 08 §4.9 es: la capa de dominio agrupa Abstracciones, Núcleo de enrutado/validación, Registro de proveedores y Resguardo de credenciales (la lógica propia de la librería, sin dependencias de proveedor concreto); la capa de infraestructura agrupa los Adaptadores de proveedor (local, remoto, otro).

| Capa | Componentes | Líneas (%) | Branches (%) | Mutation score (%) | Umbral mínimo |
| --- | --- | --- | --- | --- | --- |
| Dominio | Abstracciones, Núcleo de enrutado/validación, Registro, Resguardo de credenciales | ≥ 85 | ≥ 80 | ≥ 60 | 85 / 80 / 60 |
| Infraestructura | Adaptadores de proveedor (local / remoto / otro) | ≥ 70 | ≥ 60 | — | 70 / 60 / — |

Reconciliación con el gate global de intake §17 P.6 (≥ 80 % líneas / ≥ 70 % branches): los umbrales por capa son piso, no techo (regla 08 §2.2). El umbral de dominio (85/80) supera al global; el de infraestructura (70/60) queda por debajo del global en el plano de cada capa, pero el global se evalúa sobre la base de código completa, donde el dominio concentra la mayor parte de la lógica de la librería (la infraestructura son adaptadores delgados que delegan en SDK de proveedor). Ponderado, el conjunto satisface ≥ 80 % líneas / ≥ 70 % branches. Por eso ambos gates son compatibles y se declaran como tales: G-04 (por capa) es más estricto en dominio y G-03 (global) actúa como piso agregado. La matriz de cobertura (`matriz-cobertura-pruebas_v1.0.md` §5) consolida ambos.

## 3. Tooling

Frameworks por nivel y por tipo de test, nombrados por rol abstracto (sin productos comerciales ni stacks; la materialización concreta vive en 09 y 10).

| Nivel / tipo | Framework (rol abstracto) |
| --- | --- |
| Unit | Framework de tests unitarios del runtime objetivo |
| Property-based | Framework de property-based testing equivalente |
| Contract | Corredor de contract tests parametrizado, ejecutado una vez por proveedor soportado |
| Snapshot | Framework de snapshot testing equivalente |
| Mutation | Framework de mutation testing equivalente |
| Cobertura | Medidor de cobertura con segmentación por capa |
| Desempeño | Banco de medición de latencia p95 |

## 4. BDD si aplica

Cada CU de 02 declara criterios de aceptación en formato Given-When-Then, y la DoR de 06 los exige con al menos dos escenarios para US Must y Should. La estrategia adopta esos Given-When-Then como especificación ejecutable: cada criterio CA-XX se materializa como un caso de prueba TC-XX en `casos-prueba-referenciales_v1.0.md`, con el mismo Given-When-Then en sus pasos. No se introduce un runner `.feature` separado para una librería de este tamaño; el formato Given-When-Then vive en la descripción de cada test y se mantiene trazable a su CA de origen.

## 5. Mocks y fixtures

- Doble de proveedor en memoria (BT-13): implementación del puerto de proveedor de almacenamiento que reside en memoria, usada por el núcleo para las pruebas unitarias. Es además la plantilla de referencia para validar un proveedor real con la suite de conformidad (ver `guia-testing-extensibilidad_v1.0.md`). Vive centralizado en el proyecto de tests, versionado con el código, y se reutiliza entre todos los tests unitarios del núcleo.
- Fixtures de contenido binario: conjuntos de bytes conocidos (archivo de 245 KB de referencia, archivo de 0 bytes, archivo en el límite de 25 MB y por encima) versionados con el código en una carpeta de fixtures, regenerables por un script determinista. Se reutilizan en los tests de igualdad binaria, de tamaño máximo y de rango.
- Política de aislamiento: cada test parte de un estado limpio (doble en memoria recreado por test, ambiente efímero del proveedor local recreado por sesión de integración). Ningún test depende del orden de ejecución ni del estado dejado por otro.
- Política de reuso y duplicación: los dobles y fixtures se centralizan; queda prohibido duplicar el doble de proveedor por test. Las credenciales usadas en tests son sintéticas y no productivas (RN-03; nunca secretos reales).
- Sin mocks de servicios externos en la capa de dominio: la transparencia se verifica contra el doble en memoria y contra el proveedor local efímero; el proveedor remoto se verifica con un doble de conformidad o un contenedor efímero del servicio de objetos, sin atar el documento a un producto concreto.

## 6. Datos de prueba

- Origen: sintéticos, generados de forma determinista. No se usan datos de producción ni fotos reales de relevamientos.
- El contenido binario de los fixtures se genera con semilla fija para reproducibilidad; el property-based testing genera contenidos y prefijos aleatorios con semilla registrada para que un fallo sea reproducible.
- Versionado: los fixtures binarios y sus semillas se versionan junto al código de los tests.
- Regeneración: un script regenera los fixtures de forma determinista; cualquier cambio de fixture o de snapshot requiere PR con justificación y revisión (regla 08 §4.10, anti-patrón de snapshots regenerados sin control).

## 7. Ambiente de testing

- Aislamiento: los tests unitarios corren en proceso contra el doble en memoria, sin IO real. Los tests de integración del proveedor local usan una ubicación temporal efímera creada y destruida por sesión.
- Proveedor remoto: se verifica con un contenedor efímero del servicio de objetos o con un doble de conformidad que cumple el puerto; el ambiente se levanta y se destruye en la sesión de CI, sin estado persistente entre corridas.
- Variables de entorno y secretos: las credenciales de proveedor en tests son sintéticas y no productivas; nunca se commitean secretos reales (RN-03, ADR-05).
- Determinismo: ningún test depende de reloj de pared, de orden de ejecución ni de red externa no controlada; el banco de desempeño (NFR-01) corre en un ambiente equivalente al productivo declarado para medir la latencia p95.

## 8. Property-based testing (invariantes)

Las tres reglas de negocio se expresan como invariantes verificables con property-based testing, además de los TC de ejemplo concreto:

- Invariante de integridad (RN-02, NFR-04): para todo contenido no vacío y todo identificador lógico válido, recuperar después de guardar devuelve un contenido idénticamente igual byte a byte mientras no se sobrescriba ni elimine.
- Invariante de transparencia (RN-01, NFR-03): para toda secuencia de operaciones válidas, el resultado y el código de error son equivalentes cualquiera sea el proveedor activo.
- Invariante de no filtración (RN-03, NFR-05): para todo error producido por cualquier operación, el mensaje y los datos del error no contienen ninguna credencial ni parámetro de conexión configurado.

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| CU | CU-01 a CU-06 (02) |
| RN | RN-01, RN-02, RN-03 (02) |
| NFR | NFR-01 a NFR-06 (05) |
| ADR | ADR-01 a ADR-05 (05) |
| Backlog | BT-10 (batería de contrato y gate), BT-13 (doble en memoria) de 06 |
| Gate global | intake §17 P.6 |
| Downstream | 09 (gates en CI), 10 (cómo correr los tests), 11 (un test ejecutable por ejemplo) |

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Estrategia de testing inicial de geovial-storage: pirámide 80/15/5 para library con justificación, cobertura por capa (dominio 85/80/60, infraestructura 70/60) reconciliada con el gate global de intake, tooling por rol abstracto, contract tests por proveedor, property-based para las tres invariantes, snapshot del catálogo de errores, política de dobles, fixtures, datos y ambiente. |
