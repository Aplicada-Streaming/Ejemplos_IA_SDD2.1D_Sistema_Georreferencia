# Extensibilidad — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** extensibilidad_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer

## 1. Objetivo

Describir los puntos de extensión del motor de sincronización: las abstracciones que el host implementa e inyecta para adaptar el motor a su almacén local, su backend remoto, su fuente de conectividad y su credencial, sin tocar el núcleo. La extensibilidad es la consecuencia directa de la inversión de dependencias (ADR-02) sobre la capa Abstractions (ADR-01) y es lo que hace al paquete reutilizable fuera de la solución.

## 2. Modelo de extensión

El motor define contratos de extensión (estrategias) y recibe sus implementaciones por inyección al inicializar la sesión (CU-01) o al habilitar una capacidad (CU-04). El núcleo nunca instancia un adaptador concreto: programa contra la abstracción. Cada punto de extensión es una estrategia sustituible con una responsabilidad acotada y un contrato estable versionado junto con la superficie pública (ADR-03).

## 3. Puntos de extensión

| Punto de extensión | Responsabilidad | Obligatorio | Inyección | CU que lo usa | Error si falta |
| --- | --- | --- | --- | --- | --- |
| Estrategia de almacén local | Persistir y leer la cola y los metadatos de sincronización en el almacén local del host | Sí | Configuración de sesión | CU-01, CU-02, CU-05, CU-06 | ALMACEN_LOCAL_INACCESIBLE / CONFIGURACION_INCOMPLETA |
| Estrategia de transporte | Enviar un cambio al backend remoto por identificador estable y obtener actualizaciones posteriores a una marca | Sí | Configuración de sesión | CU-03, CU-06 | CONFIGURACION_INCOMPLETA; en ejecución, BACKEND_INALCANZABLE |
| Proveedor de credencial | Entregar la credencial vigente del host al motor | No (su ausencia produce estado no autenticada) | Configuración de sesión | CU-01, CU-03 | SESION_NO_AUTENTICADA / CREDENCIAL_INVALIDA |
| Fuente de eventos de conectividad | Notificar transiciones de red para el disparo automático | No (solo para el modo automático) | Al habilitar el disparo automático | CU-04 | FUENTE_CONECTIVIDAD_AUSENTE |

Notas de contrato de extensión:

- La estrategia de transporte debe reconocer el identificador de cambio estable para garantizar la idempotencia efectiva en el backend (RN-02; ADR-07).
- La estrategia de transporte y el backend remoto deben poder reportar un estado en conflicto como condición no bloqueante para que el motor lo aplique y lo conviva (RN-03; ADR-08).
- Ningún punto de extensión expone ni interpreta la carga útil de dominio: el motor la trata como opaca.

## 4. Contrato de extensión

Cada punto de extensión cumple un contrato estable con estas garantías mutuas:

- Hacia el motor: la estrategia respeta la forma de datos del contrato (`contratos-abstractions_v1.0.md` §4), reporta sus condiciones con los códigos estables del catálogo y no asume conocimiento del dominio del host.
- Hacia el host: el motor invoca cada estrategia respetando el orden del pipeline (RN-01), no la llama de forma concurrente para una misma sesión (exclusión mutua del ciclo) y no almacena la credencial más allá de la fase que la requiere.
- Estabilidad y versionado: los contratos de extensión forman parte de la superficie pública; cambiarlos sigue la política de versionado semántico (ADR-03). Agregar un punto de extensión opcional es compatible; quitar o cambiar la firma de uno existente es un cambio mayor.

## 5. Registro y resolución de las extensiones

- El host registra las estrategias obligatorias (almacén local, transporte) al armar la configuración de sesión; el motor valida su presencia y coherencia en CU-01 y rechaza con código estable si falta una obligatoria.
- Las estrategias opcionales se registran al habilitar la capacidad que las usa: el proveedor de credencial al inicializar o más tarde (habilita pasar de no autenticada a listo); la fuente de conectividad al habilitar el disparo automático (CU-04).
- No hay descubrimiento automático ni selección implícita de estrategias: el registro es siempre explícito por el host, para que el comportamiento sea razonable y versionable (ADR-02 alternativa descartada).

## 6. Ejemplo de extensión (referencia a 11)

El ejemplo de referencia es el sample de demostración ajeno a la solución GeoVial descrito en el intake §18 y §16.1, ubicado en `/samples/aplicada-sync/03-avanzado-demo-maui/` (categoría 11). Ese sample implementa las estrategias obligatorias (almacén local y transporte) y las opcionales (proveedor de credencial y fuente de conectividad) contra un host de demostración que no pertenece al dominio de GeoVial, demostrando el punto de extensión principal —el motor de sincronización reutilizable— y validando que el motor opera sin modificación con adaptadores propios del integrador. Los samples `01-basico` y `02-intermedio` ejercitan progresivamente los puntos de extensión obligatorios. El código ejecutable vive en 11; este documento define el contrato, no la implementación.

## 7. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| CU cubiertos | CU-01, CU-02, CU-03, CU-04, CU-05, CU-06 |
| RN aplicables | RN-01, RN-02, RN-03 |
| ADRs que lo justifican | ADR-01 (capa Abstractions), ADR-02 (inversión de dependencias), ADR-03 (versionado de contratos de extensión), ADR-07 (transporte reconoce identificador), ADR-08 (transporte reporta conflicto) |
| Ejemplo de extensión | Sample `03-avanzado-demo-maui` y samples `01-basico`/`02-intermedio` (categoría 11; intake §18, §16.1) |
| Tests previstos (08) | Doble de cada estrategia; rechazo por estrategia obligatoria ausente; operación del motor con adaptadores del integrador |

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Puntos de extensión iniciales del motor aplicada-sync: estrategias de almacén local, transporte, credencial y conectividad; contrato de extensión, registro explícito y referencia al ejemplo de 11. Derivado de los ADR-01/02/03/07/08 y del intake §18/§16.1. |
