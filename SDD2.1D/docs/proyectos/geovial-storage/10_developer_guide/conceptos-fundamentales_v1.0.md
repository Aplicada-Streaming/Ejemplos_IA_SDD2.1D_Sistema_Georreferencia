# Conceptos fundamentales — geovial-storage

**Proyecto:** geovial-storage
**Documento:** conceptos-fundamentales_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Technical Writer + SDK Documentation Lead
**Tipo Diátaxis:** Explanation
**Audiencia:** Developer integrador del backend que consume la abstracción de almacenamiento
**Nivel:** Básico
**Tiempo estimado de lectura:** 12 min

## 1. Concepto central

`geovial-storage` es una abstracción de almacenamiento de archivos: recibe el contenido de un archivo (típicamente una fotografía de un relevamiento) y entrega a cambio un identificador lógico estable con el que recuperarlo, verificarlo o eliminarlo después. El consumidor invoca siempre el mismo contrato de operaciones sin saber dónde queda físicamente el archivo. El destino físico —local, remoto u otro— se llama proveedor activo, es intercambiable y lo selecciona el usuario raíz, no el consumidor.

La razón de ser de la librería es desacoplar al backend del lugar donde se guardan los archivos: el negocio puede cambiar de destino según costo, capacidad o contexto de despliegue, y el código de integración no cambia. Esa propiedad se llama transparencia y es el invariante central del sistema (RN-01).

## 2. Modelo mental

El flujo principal tiene cuatro etapas. El consumidor solo ve la primera y la última; las dos del medio son internas y explican por qué el comportamiento es uniforme entre proveedores.

```
[Consumidor]                [Núcleo de enrutado]          [Registro de         [Proveedor activo]
   guardar/recuperar/   -->    valida la entrada      -->  proveedores]     -->  persiste / lee /
   eliminar/verificar/        y normaliza errores         resuelve el            borra / enumera
   listar                                                 proveedor activo
   (identificador) <------------------------------------------------------------ (confirmación)
```

| Etapa | Qué es | Ejemplo |
| --- | --- | --- |
| Invocación del contrato | El consumidor llama a una de las operaciones públicas con entradas lógicas (contenido, identificador, prefijo). | Guardar una foto bajo el destino `relevamientos/2026/r-001/`. |
| Validación y normalización | El núcleo valida la entrada (contenido no vacío, formato de destino, rango, tamaño máximo) y traduce cualquier fallo del proveedor a un código de error uniforme. | Rechazar un contenido de 0 bytes con `CONTENIDO_VACIO` sin contactar al proveedor. |
| Resolución del proveedor | El registro de proveedores resuelve cuál es el proveedor activo según la configuración fijada por el usuario raíz (CU-06). | El proveedor activo es el local; mañana puede ser el remoto sin que el consumidor lo note. |
| Delegación al proveedor | El proveedor activo persiste, lee, borra o enumera el contenido y confirma la operación. | El proveedor local escribe el binario en una ubicación accesible y escribible. |

Las seis operaciones del contrato, una por caso de uso de la especificación funcional (02), son:

| Operación | CU | Qué hace |
| --- | --- | --- |
| Guardar | CU-01 | Persiste un contenido y devuelve un identificador lógico estable. |
| Recuperar | CU-02 | Devuelve el contenido idénticamente igual al guardado, por identificador. |
| Eliminar | CU-03 | Quita el contenido asociado a un identificador; eliminar un inexistente es éxito (idempotencia). |
| Verificar existencia | CU-04 | Informa si un identificador corresponde a un archivo presente, sin transferir el binario. |
| Listar bajo prefijo | CU-05 | Enumera los identificadores presentes bajo un prefijo, con paginación por testigo. |
| Configurar proveedor activo | CU-06 | Selecciona y valida el proveedor activo y sus credenciales (atribución del usuario raíz). |

El contrato se reparte en dos interfaces alineadas con su audiencia: una de almacenamiento (las cinco operaciones de datos, dirigidas al consumidor) y una de configuración del proveedor activo (CU-06, dirigida al usuario raíz). La separación es una decisión de diseño, no un detalle de implementación; se explica en §3.

## 3. Decisiones de diseño relevantes para el consumidor

Solo se documentan aquí las decisiones que cambian cómo se usa el sistema. Cada una cita su ADR de origen en la categoría 05.

| Identificador | Decisión | Por qué importa al consumidor | ADR |
| --- | --- | --- | --- |
| `concepto-transparencia` | El núcleo normaliza toda diferencia de los proveedores a un comportamiento observable único: mismos resultados y mismos códigos de error para las mismas entradas. | El consumidor escribe su código una vez y no necesita ramas por proveedor; cambiar de destino no cambia su integración (RN-01). | ADR-01, ADR-04 |
| `concepto-abstraccion` | Se expone una única interfaz de almacenamiento más una interfaz separada de configuración; los adaptadores de proveedor y el núcleo son internos. | El consumidor depende de una superficie estable; los cambios internos no lo afectan y nada en el contrato nombra un proveedor concreto. | ADR-02 |
| `concepto-integridad` | El binario no se transforma ni recodifica en ningún punto del enrutado ni en los adaptadores. | Lo recuperado es idénticamente igual a lo guardado, byte a byte, mientras no se sobrescriba ni elimine (RN-02). La evidencia del relevamiento no se altera. | ADR-04 |
| `concepto-credenciales` | Las credenciales del proveedor entran por la configuración (CU-06) y no salen por ninguna vía de la superficie pública; no aparecen en resultados, errores ni registros. | El consumidor nunca descubre una credencial filtrada en un resultado o en un log; tampoco existe una operación que lea la configuración sensible (RN-03). | ADR-05 |
| `concepto-versionado` | El contrato se versiona con SemVer: agregar un proveedor, una operación o un parámetro opcional es versión menor (compatible); cambiar semántica o quitar una operación o un código es versión mayor (incompatible). | El consumidor sabe, por la versión, si un cambio es seguro de adoptar sin tocar su código. El identificador lógico conserva su significado a través de versiones menores. | ADR-03 |
| `concepto-limite-tamano` | El tamaño máximo de archivo es un parámetro común a todos los proveedores (valor por defecto 25 MB), validado por el núcleo antes de delegar. | El límite percibido es idéntico cualquiera sea el proveedor; un contenido excedido se rechaza con `TAMANIO_EXCEDIDO` sin transferirse al proveedor. | ADR-04 |

## 4. Vocabulario

Subconjunto crítico para entender el sistema. El vocabulario completo, con referencia cruzada por término, vive en `glosario-tecnico_v1.0.md`.

| Término | Definición operativa | Ejemplo |
| --- | --- | --- |
| identificador-logico | Cadena opaca para el consumidor en cuanto a la ubicación física, con la que recupera, verifica o elimina un archivo. | `relevamientos/2026/r-001/foto-01.jpg` |
| destino-logico | Prefijo o ruta lógica con un formato admitido que agrupa los archivos de un relevamiento. | `relevamientos/2026/r-001/` |
| proveedor-activo | Destino físico que aloja el contenido en un momento dado, seleccionado por el usuario raíz. | Proveedor local hoy; proveedor remoto mañana, sin cambio en el consumidor. |
| testigo-de-continuacion | Marca opaca que devuelve el listado cuando la enumeración no se completó. | Se reenvía para obtener la página siguiente de un relevamiento con muchas fotos. |
| puerto-de-proveedor | Contrato interno que un proveedor nuevo implementa para integrarse sin tocar el núcleo. | Un adaptador para un destino alternativo o un doble de prueba en memoria. |

## 5. Qué NO hace el sistema

Delimita la responsabilidad del sistema frente a la del consumidor, para evitar expectativas falsas.

| El sistema NO | Responsabilidad de | Detalle |
| --- | --- | --- |
| Decide políticas de retención ni ciclo de vida del archivo | Consumidor | La librería persiste cuando se la invoca; no caduca ni archiva por su cuenta. |
| Transforma, comprime ni recodifica el contenido | Consumidor | El tipo de contenido se registra como metadato; el binario viaja intacto (RN-02). |
| Migra los archivos al cambiar de proveedor activo | Operación externa | Cambiar el proveedor no mueve lo ya guardado en el anterior; la migración, si se requiere, es otra operación. |
| Cachea contenidos | Consumidor / categoría 05 | Cada recuperación consulta al proveedor activo, salvo decisión de 05. |
| Garantiza el orden del listado | Consumidor | Se garantiza cardinalidad y pertenencia bajo el prefijo, no el orden (CU-05). |
| Expone capacidades específicas de un proveedor | — | La superficie es lo común entre proveedores; nada del contrato nombra un destino concreto (RN-01; ADR-02). |
| Implementa papelera ni recuperación posterior | Consumidor | Eliminar es definitivo desde la perspectiva del contrato. |
| Lee ni devuelve la configuración sensible del proveedor | — | No hay operación pública que recupere credenciales (RN-03; ADR-05). |

## 6. Referencias cruzadas

- 05 `decisiones-arquitectura_v1.0.md` y `adrs/ADR-01-abstraccion-proveedores-intercambiables_v1.0.md`: estilo de la abstracción con proveedores intercambiables (`concepto-transparencia`, `concepto-abstraccion`).
- 05 `adrs/ADR-04-transparencia-limites-proveedor_v1.0.md`: normalización en el núcleo, integridad binaria y tamaño máximo (`concepto-integridad`, `concepto-limite-tamano`).
- 05 `adrs/ADR-05-manejo-seguro-credenciales_v1.0.md`: resguardo de credenciales (`concepto-credenciales`).
- 05 `contratos-abstractions_v1.0.md` §6 y `adrs/ADR-03-estrategia-versionado-contrato_v1.0.md`: versionado del contrato (`concepto-versionado`).
- 02 `casos-de-uso/`: CU-01 a CU-06, origen funcional de las seis operaciones.
- 10 `glosario-tecnico_v1.0.md`: vocabulario canónico completo.
- 10 `referencia-api_v1.0.md`: firmas, parámetros y errores de cada operación.

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Conceptos fundamentales iniciales: concepto central de la abstracción de almacenamiento, modelo mental de cuatro etapas, seis operaciones, decisiones de diseño con cita a ADR-01 a ADR-05, vocabulario crítico y delimitación de responsabilidades. |
