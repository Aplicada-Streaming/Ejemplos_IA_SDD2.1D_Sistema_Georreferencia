# ADR-09 — Integración con la abstracción de almacenamiento de archivos

**Proyecto:** geovial-api
**Documento:** ADR-09-integracion-abstraccion-almacenamiento_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto de Software + API Designer
**Categoría:** Persistencia

## 1. Contexto

El backend debe alojar los binarios de las fotografías de los relevamientos sin atarse al destino físico, y el usuario raíz debe poder cambiar ese destino (proveedor local, de objetos remoto u otro) de forma transparente para los demás roles (NB-07, CU-17). La solución provee una librería de almacenamiento (`geovial-storage`, nivel 0, ya documentada) que expone una abstracción transparente con dos interfaces —operaciones de archivo y configuración del proveedor activo— y un catálogo de error uniforme, idéntico cualquiera sea el proveedor (contrato de `geovial-storage`, dependencia del manifiesto §13). El backend la consume en proceso, no por red. La carga manual prioriza la ubicación incrustada y agrupa por radio (RN-04). Cubre CU-08, CU-09, CU-15, CU-16, CU-17.

## 2. Decisión

Se integra la abstracción de almacenamiento de `geovial-storage` a través de un puerto de almacenamiento declarado por la capa de Aplicación e implementado por un adaptador de la capa de Infraestructura que delega en la interfaz de almacenamiento de la librería. El backend persiste en su almacén relacional únicamente la referencia lógica (identificador opaco) que devuelve la librería al guardar una foto; el binario reside en el proveedor activo. La configuración del destino (CU-17) se delega a la interfaz de configuración del proveedor activo de la librería y solo la ejerce el usuario raíz. Los códigos de error de la librería se normalizan al cruzar la frontera hacia el contrato REST del backend (ADR-05). El backend no migra los binarios existentes al cambiar de proveedor.

## 3. Estado

Aceptado el 2026-06-15. Decisión pre-tomada en el intake (§14, §17.P.1, §17.P.11): integración de la librería de almacenamiento transparente.

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Consumir la abstracción de `geovial-storage` por un puerto (elegido) | Transparencia del proveedor heredada (RN-01 de storage); el backend persiste solo la referencia lógica; configuración por raíz delegada | Acopla el backend a la superficie pública de la librería; depende de su política de versionado |
| Acceder al destino físico directamente desde el backend | Sin dependencia de la librería | Ata el backend a un único destino; viola la transparencia; obliga a ramas por proveedor; descartado por el intake §14 |
| Guardar el binario en el almacén relacional | Una sola fuente de datos | Infla la base con binarios; degrada el rendimiento de las consultas; impide el destino remoto configurable |
| Integrar un único proveedor de almacenamiento fijo en el backend | Simplicidad | Incumple NB-07 (destino configurable por el raíz); elimina la transparencia y el punto de configuración |

## 5. Consecuencias positivas

1. El backend guarda y recupera fotos sin conocer el destino físico: la transparencia la garantiza la librería (RN-01 de storage; CU-17, transparencia para otros roles).
2. La tabla de fotos guarda solo la referencia lógica, manteniendo el almacén relacional liviano y las consultas de revisión rápidas.
3. El cambio de destino por el usuario raíz (CU-17) es una reconfiguración, no un redespliegue; las fotos previas permanecen accesibles.
4. La carga manual (CU-09) asocia la referencia del binario al marcador resultante de la priorización por ubicación y radio (RN-04) sin acoplar el almacenamiento a esa lógica.

## 6. Consecuencias negativas y trade-offs

1. El backend depende de la superficie pública y de la política de versionado de `geovial-storage`; un cambio incompatible de la librería obligaría a coordinar (mitigado por el versionado del contrato de storage).
2. El backend acepta el costo de una capa de abstracción para independizarse del proveedor (trade-off heredado de storage §17.P.12).
3. Al cambiar de proveedor no se migran los binarios existentes; el backend debe poder recuperar fotos previas desde el destino anterior mientras existan (decisión funcional de CU-17 y CU-06 de storage).

## 7. Implementación

- La capa de Aplicación declara un puerto de almacenamiento (guardar, recuperar, eliminar, verificar, listar) alineado con la interfaz de almacenamiento de `geovial-storage`.
- El adaptador de almacenamiento (Infraestructura) implementa el puerto delegando en la librería embebida en proceso.
- La tabla de fotos persiste la referencia lógica devuelta al guardar; nunca el binario (modelo lógico).
- La configuración del destino (CU-17) usa la interfaz de configuración del proveedor activo; solo el usuario raíz la ejerce (RN-01); las credenciales del proveedor no salen por ninguna respuesta (RN-03 de storage).
- Los códigos de error de la librería se normalizan a problem+json del backend (ADR-05).
- Convención impuesta: el backend nunca accede al destino físico salvo a través del puerto de almacenamiento.

## 8. Métricas de validación

- Referencia de foto persistida y binario recuperable de forma idéntica al guardado (RN-02 de storage; verificado en 08 sobre CU-08, CU-09).
- Cambio de destino por el usuario raíz transparente para agentes y jefes; cambio por rol no autorizado rechazado con ROL_NO_AUTORIZADO (CU-17).
- Credenciales del proveedor no expuestas en ninguna respuesta ni error del backend (RN-03 de storage).
- La exportación e importación de un relevamiento incluye sus fotos íntegras (CU-15, CU-16).

## 9. Referencias

- NB-07, NB-03, NB-06; CU-08, CU-09, CU-15, CU-16, CU-17; RN-04.
- Intake §14 (composición), §17.P.1 (stack), §17.P.11.
- Contrato consumido: `geovial-storage` — `contratos-abstractions_v1.0.md` (arista del manifiesto §13; se indexa en la vista de solución de `_solucion/`).
- ADRs relacionadas: ADR-01 (puertos), ADR-02 (referencia en el almacén), ADR-05 (normalización de errores).

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión de persistencia: integración de la abstracción de almacenamiento de geovial-storage por un puerto; el almacén relacional guarda solo la referencia lógica; configuración del destino por el usuario raíz. Aceptada (pre-tomada en intake §14, §17.P.11). |
