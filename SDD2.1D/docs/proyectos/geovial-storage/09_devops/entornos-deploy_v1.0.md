# Entornos y despliegue — geovial-storage

**Proyecto:** geovial-storage
**Documento:** entornos-deploy_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero DevOps Senior (variante DevOps + Release Engineer, library)

## 0. Declaración del modelo

`geovial-storage` es una `library` que no se publica como paquete redistribuible (intake §13, §17.P.7; ADR-03) y no es una unidad de despliegue independiente (05 §5). En consecuencia:

- No hay feed propio de la librería. La librería no se publica a un gestor de paquetes ni a un canal externo.
- No hay ambientes propios de la librería (no aplica DEV/QA/STAGING/PROD a una librería; tampoco canales preview/stable sobre un feed). Aplicar ambientes de despliegue a una librería es el anti-patrón "confundir publicación con despliegue" (regla 09 §4.8).
- La librería viaja embebida dentro del artefacto del backend `geovial-api` (la imagen del backend) y se despliega con él (05 §5).

Por tanto, este documento declara explícitamente que el modelo de ambientes de la librería es el del backend, heredado, y referencia el documento del backend en vez de duplicarlo.

## 1. Ambientes y canales (heredados del backend)

La librería se ejecuta dentro del contenedor del backend en cada ambiente donde el backend se despliega. Los ambientes, sus URL, aprobadores y SLA los define el `entornos-deploy_v1.0.md` de `geovial-api` (categoría 09 del proyecto `geovial-api`), cuya Parte C técnica está diferida en el intake (§17 de geovial-api: P.8, P.10 PENDIENTE). Hasta que ese documento se publique, esta tabla referencia el modelo por tipo D8 del backend (`rest-api`: DEV / QA / STAGING / PROD, regla 09 §2.2) sin fijar sus valores numéricos, que pertenecen al backend.

| Ambiente del backend | Rol de la librería | Aprobador / SLA |
| --- | --- | --- |
| DEV | La librería corre embebida; proveedor local sobre el sistema de archivos del contenedor | Definido por `geovial-api` (heredado) |
| QA | Igual; se ejecutan los contract tests por proveedor | Definido por `geovial-api` (heredado) |
| STAGING | Igual; se mide NFR-01 en ambiente equivalente al productivo (GAP-03) | Definido por `geovial-api` (heredado) |
| PROD | Igual; proveedor activo según configuración del usuario raíz (CU-06) | Definido por `geovial-api` (heredado) |

La librería no agrega ambientes propios ni canales: cualquier ambiente intermedio del backend la arrastra sin acción adicional.

## 2. Provisión (IaC)

La librería no provisiona infraestructura propia: no tiene un destino de despliegue separado. La única dependencia de infraestructura que impone sobre el ambiente del backend, derivada de 05 §5 y del riesgo de pérdida de evidencia (05 §9), es:

- Almacenamiento persistente para el proveedor local. El destino del proveedor local debe ser un volumen persistente montado en el contenedor del backend, para que las fotografías sobrevivan al reciclado del contenedor (05 §5, §9; riesgo de pérdida de evidencia). La provisión de ese volumen es parte de la IaC del backend (Terraform, Bicep, Pulumi u homólogo declarativo, según fije `geovial-api`), no de la librería.

La librería no exige más infraestructura: el proveedor de objetos remoto usa un servicio externo cuya provisión y credenciales son configuración del ambiente del backend (ver §4).

## 3. Configuración por ambiente (12-factor)

La configuración de la librería entra exclusivamente por CU-06 (selección del proveedor activo por el usuario raíz). Sigue el principio 12-factor: la configuración vive en variables de entorno o archivos referenciados del backend, nunca en código.

| Variable de configuración | Origen | Por ambiente |
| --- | --- | --- |
| Proveedor activo (local / objetos remoto / otro) | CU-06, fijado por el usuario raíz | Puede diferir por ambiente |
| Ruta o raíz del proveedor local | Configuración del backend | Volumen persistente del contenedor en cada ambiente |
| Tamaño máximo de archivo (NFR-02, por defecto 25 MB) | Configuración del backend | Mismo valor por defecto; configurable por ambiente |
| Parámetros del proveedor de objetos remoto (endpoint, región, bucket lógico) | Configuración del backend; los parámetros sensibles van a §4 | Por ambiente |

La librería no lee configuración de su propia superficie pública fuera de CU-06 (05 §7) y no devuelve la configuración sensible por ningún resultado (RN-03, ADR-05).

## 4. Secretos

Las credenciales del proveedor de objetos remoto son secretos. Su manejo es crítico (ADR-05; RN-03; NFR-05):

- Prohibición explícita de commit. Ninguna credencial ni parámetro de conexión sensible se versiona en el repositorio (anti-patrón "secretos en commit", regla 09 §4.8; ADR-05). El scan de secretos en commits es parte del supply chain (`supply-chain-seguridad_v1.0.md` §5).
- Almacenamiento en vault. Las credenciales viven en un gestor de secretos del ambiente del backend (vault, secret manager o store de secretos del orquestador de contenedores), inyectadas en el contenedor del backend en tiempo de ejecución. El mecanismo concreto lo fija la categoría 09 del backend a partir del intake §17.P.5 (delegado por ADR-05).
- No salen por la superficie pública. La librería custodia las credenciales en su resguardo de credenciales: entran por CU-06 y no salen por ningún resultado, error ni registro (RN-03, ADR-05). Esto se verifica con el gate G-07 (STAGE-07 del pipeline) y la prueba de no filtración (TC-24).
- Rotación. La rotación de las credenciales del proveedor de objetos remoto se ejecuta en el vault del ambiente del backend; la librería no cachea credenciales entre invocaciones (05 §6), por lo que una rotación toma efecto sin redesplegar la librería.
- Scopes. Las credenciales del proveedor remoto se otorgan con el mínimo alcance necesario para las operaciones del contrato (guardar, recuperar, eliminar, verificar, listar bajo el prefijo del backend).

## 5. Promoción

La promoción entre ambientes es la del backend. La librería no tiene una promoción propia: al promover la imagen del backend de un ambiente al siguiente, la librería embebida promueve con ella.

- El aprobador requerido para promover a PROD es el que defina `geovial-api` (regla 09 §2.2 exige aprobador humano para PROD en tipos desplegables).
- El registro de auditoría de la promoción es el del release del backend.
- La librería no introduce gates de promoción adicionales más allá de los gates de merge y release de 08 ya ejecutados en su pipeline (`pipeline-ci-cd_v1.0.md` §4).

## 6. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| No publicable / no desplegable | intake §13, §17.P.7; 05 §5; ADR-03 |
| Almacenamiento persistente del proveedor local | 05 §5, §9 |
| Configuración por CU-06 | 02 CU-06; 05 §7 |
| Secretos y credenciales | ADR-05; RN-03; NFR-05; intake §17.P.5 |
| Ambientes heredados del backend | `entornos-deploy_v1.0.md` de `geovial-api` (referenciado, no duplicado) |
| Gates de no filtración | `estrategia-calidad_v1.0.md` §3 (G-07) |

## 7. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Entornos y despliegue inicial de geovial-storage: declara que no hay feed ni ambientes propios; la librería viaja embebida en la imagen del backend y hereda los ambientes de `geovial-api` (referenciados, no duplicados); única exigencia de infraestructura es el volumen persistente para el proveedor local (05 §5, §9); configuración 12-factor por CU-06; secretos del proveedor remoto en vault del ambiente del backend, nunca en commit, con resguardo de no filtración (ADR-05); promoción heredada del backend con aprobador de PROD definido por geovial-api. |
