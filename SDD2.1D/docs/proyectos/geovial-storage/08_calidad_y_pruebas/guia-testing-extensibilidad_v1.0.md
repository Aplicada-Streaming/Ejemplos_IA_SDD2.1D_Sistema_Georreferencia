# Guía de testing de extensibilidad — geovial-storage

**Proyecto:** geovial-storage
**Documento:** guia-testing-extensibilidad_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero QA / SDET Senior (variante QA + SDET Library)

## 1. Propósito

`geovial-storage` tiene extensibilidad (`tiene_extensibilidad=true`): su único punto de extensión es el puerto de proveedor de almacenamiento declarado en la capa de Abstracciones (05, `extensibilidad_v1.0.md`). Un destino nuevo (proveedor "otro") se incorpora como un adaptador que implementa ese puerto y se registra en el registro de proveedores, sin modificar el núcleo de enrutado ni la superficie pública. Esta guía describe cómo testear un proveedor nuevo contra el contrato del puerto mediante una suite de conformidad de proveedor, de modo que la transparencia (RN-01) quede garantizada sin tocar el núcleo.

La regla 08 §2.2 exige esta guía para `library` que expone plugins o extensiones; este proyecto califica.

## 2. El contrato del puerto a verificar

El puerto de proveedor de almacenamiento agrupa seis operaciones que el adaptador debe implementar. La suite de conformidad verifica cada una contra las garantías textuales del contrato (05, `contratos-abstractions_v1.0.md` §3 y `extensibilidad_v1.0.md` §3):

| Operación del puerto | Garantía que la suite verifica |
| --- | --- |
| Persistir contenido | Guarda el binario sin transformarlo (RN-02); no deja archivo parcial ante fallo; respeta la marca de sobrescritura |
| Leer contenido | Devuelve el contenido idéntico al persistido; soporta lectura por rango y modo solo-metadatos |
| Borrar | Quita el contenido; soporta borrado bajo prefijo; informa el conjunto no eliminado en caso parcial; trata como éxito el borrado idempotente de un inexistente |
| Comprobar presencia | Informa presencia coherente con el estado real, sin transferir contenido |
| Enumerar bajo prefijo | Devuelve los identificadores bajo el prefijo con paginación por testigo; garantiza cardinalidad y pertenencia (no el orden) |
| Validar configuración | Comprueba conectividad y permisos; acepta credenciales sin exponerlas (RN-03) |

### Obligaciones transversales (verificadas por la batería de conformidad)

1. Transparencia (RN-01): el adaptador mapea sus fallos a los códigos de error uniformes del catálogo, sin introducir códigos ni comportamientos propios del proveedor.
2. Integridad (RN-02): el adaptador no transforma ni recodifica el contenido.
3. Manejo seguro de credenciales (RN-03): el adaptador accede a las credenciales solo a través del resguardo de credenciales y no las emite en errores ni registros.
4. Límite de tamaño: el adaptador respeta el tamaño máximo configurado, validado por el núcleo antes de delegar (ADR-04); el adaptador no necesita reimplementar esa validación.

## 3. La suite de conformidad de proveedor

La suite de conformidad es la materialización de la batería de contrato única (RN-01, ADR-04) aplicada a un proveedor cualquiera. Se diseña como un conjunto de tests parametrizados por el proveedor bajo prueba: las mismas entradas y los mismos expected se ejecutan contra el doble en memoria (BT-13), el adaptador local, el adaptador remoto y cualquier adaptador nuevo. El doble en memoria es la plantilla de referencia y el oráculo de comportamiento esperado.

Casos de conformidad obligatorios (derivados de los TC del catálogo, reusados con el proveedor como parámetro):

| Caso de conformidad | Verifica | TC de referencia |
| --- | --- | --- |
| Ida y vuelta con igualdad binaria | RN-02, NFR-04 | TC-05, TC-23 |
| Guardado de contenido vacío rechazado | CONTENIDO_VACIO uniforme | TC-02 |
| Identificador duplicado sin sobrescritura | IDENTIFICADOR_DUPLICADO uniforme | TC-03 |
| Sobrescritura explícita | reemplazo conservando identificador | TC-04 |
| Recuperación de inexistente | IDENTIFICADOR_INEXISTENTE uniforme | TC-06 |
| Recuperación por rango | segmento exacto | TC-07 |
| Eliminación idempotente de inexistente | éxito por idempotencia | TC-10 |
| Eliminación múltiple por prefijo | cardinalidad eliminada | TC-11 |
| Coherencia presencia/ausencia | CU-04 coherente con el estado real | TC-12 |
| Listado con paginación por testigo | cardinalidad, pertenencia, testigo | TC-14 |
| Proveedor no disponible sin filtrar credenciales | PROVEEDOR_NO_DISPONIBLE uniforme; 0 credenciales | TC-08, TC-15, TC-24 |
| Tamaño máximo configurado | TAMANIO_EXCEDIDO antes de delegar | TC-26 |
| Equivalencia con el oráculo | 0 diferencias de comportamiento observable respecto del doble en memoria | TC-21, TC-27 |

Criterio de aprobación: el proveedor nuevo pasa la suite cuando produce resultados equivalentes al doble en memoria para todas las entradas, sin que el núcleo ni la superficie pública se modifiquen.

## 4. Procedimiento para testear un proveedor nuevo

1. Implementar un adaptador que cumpla el puerto de proveedor de almacenamiento y sus obligaciones transversales (§2). Usar el doble en memoria como plantilla de referencia.
2. Declarar los parámetros y credenciales que el proveedor requiere y su validación de formato (CU-06; el formato inválido produce CREDENCIALES_INVALIDAS).
3. Registrar el adaptador en el registro de proveedores con un identificador de proveedor, para que el usuario raíz pueda seleccionarlo por CU-06. Si no está registrado, su activación se rechaza con PROVEEDOR_NO_SOPORTADO (TC-28).
4. Ejecutar la suite de conformidad (§3) parametrizada con el proveedor nuevo (TC-27). Debe producir resultados equivalentes a los demás proveedores para las mismas entradas.
5. Verificar la no filtración de credenciales en operaciones, errores y registros con el property-based de no filtración (TC-24), parametrizado con el proveedor nuevo (RN-03, ADR-05).
6. Confirmar que el núcleo, las Abstracciones y los demás adaptadores no fueron modificados: el diff del PR debe limitarse al adaptador nuevo, su registro y sus fixtures de conformidad.

## 5. Ambiente y dobles

- Doble en memoria (BT-13): oráculo de comportamiento esperado; reside en el proyecto de tests, versionado y centralizado. Toda discrepancia de un proveedor real frente al doble es un fallo de conformidad.
- Proveedor local: ambiente efímero (ubicación temporal creada y destruida por sesión).
- Proveedor remoto u otro: contenedor efímero del servicio de objetos o un doble de conformidad que cumple el puerto; nunca un servicio productivo, y siempre con credenciales sintéticas no productivas (RN-03).
- Determinismo: la suite se ejecuta sin depender del orden, del reloj de pared ni de red externa no controlada; los contenidos se generan con semilla registrada para reproducir contraejemplos.

## 6. Anti-patrones a evitar en la extensión

| Anti-patrón | Problema | Solución |
| --- | --- | --- |
| El adaptador introduce un código de error propio | Rompe la transparencia (RN-01); el consumidor debería ramificar por proveedor | Mapear todo fallo al catálogo de errores uniforme |
| El adaptador recodifica o normaliza el contenido | Rompe la integridad binaria (RN-02) | No transformar el binario; verificar con la propiedad de igualdad binaria |
| El adaptador registra la credencial al fallar | Filtra secretos (RN-03) | Acceder a credenciales solo por el resguardo; verificar con el property-based de no filtración |
| El proveedor nuevo modifica el núcleo para encajar | Viola el punto de extensión; acopla el núcleo al proveedor | El diff debe limitarse al adaptador y su registro; la suite de conformidad lo demuestra |
| Probar el proveedor nuevo con tests ad-hoc distintos de la batería única | No garantiza equivalencia con los demás proveedores | Reusar la suite de conformidad parametrizada, con el doble en memoria como oráculo |

## 7. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Punto de extensión | Puerto de proveedor de almacenamiento (05, `extensibilidad_v1.0.md`) |
| RN | RN-01 transparencia, RN-02 integridad, RN-03 credenciales (02) |
| ADR | ADR-01 estilo, ADR-04 transparencia/límites, ADR-05 credenciales (05) |
| CU | CU-06 (registro y selección del proveedor activo) |
| Tests | TC-21, TC-22, TC-23, TC-24, TC-26, TC-27, TC-28 de `casos-prueba-referenciales_v1.0.md` |
| Backlog | BT-13 (doble en memoria), BT-10 (batería de contrato y gate) de 06 |
| Downstream | 11 (samples de consumidores progresivos del proveedor en `samples/geovial-storage/`) |

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Guía inicial de testing de extensibilidad de geovial-storage: contrato del puerto a verificar, suite de conformidad de proveedor parametrizada con el doble en memoria como oráculo, procedimiento de seis pasos para testear un proveedor nuevo sin tocar el núcleo, ambiente/dobles y anti-patrones de extensión. |
