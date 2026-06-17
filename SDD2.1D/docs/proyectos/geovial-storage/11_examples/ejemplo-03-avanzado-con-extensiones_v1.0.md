# Ejemplo 03 — Implementar y registrar un proveedor nuevo contra el puerto de proveedor

**Proyecto:** geovial-storage
**Documento:** ejemplo-03-avanzado-con-extensiones_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-15
**Autor:** Developer Advocate / Sample Engineer Senior
**Nivel:** Avanzado
**Ubicación del código:** `/samples/geovial-storage/03-avanzado-integracion-real/`

## 1. Objetivo del sample

Demostrar el punto de extensión de la librería: implementar un proveedor de almacenamiento nuevo como adaptador del puerto de proveedor, registrarlo en el registro de proveedores y validarlo con la suite de conformidad, todo sin modificar el núcleo de enrutado ni la superficie pública. Al finalizar, el desarrollador sabe extender el conjunto de proveedores soportados y comprobar que el proveedor nuevo produce resultados equivalentes a los demás para las mismas entradas, garantizando la transparencia (RN-01).

## 2. Nivel

Avanzado. Asume completados los samples 01 (datos básicos con el proveedor local) y 02 (gestión y transparencia entre proveedores). Cruza la frontera de consumidor a extensor: el desarrollador deja de invocar el contrato para implementarlo del lado del proveedor. Es el sample que ejercita el único punto de extensión declarado en la arquitectura (puerto de proveedor de almacenamiento) y cierra la progresión.

## 3. Prerequisites

| Prerequisito | Versión mínima / cómo obtenerlo |
| --- | --- |
| Runtime del consumidor | El declarado para la librería en intake §17.P.9. |
| Gestor de paquetes del ecosistema | El del runtime anterior, para restaurar las dependencias del sample. |
| El doble en memoria de referencia | Provisto por el proyecto de tests de la librería; es la plantilla del adaptador y el oráculo de comportamiento esperado. |
| La suite de conformidad de proveedor | Provista por la librería; se parametriza con el proveedor bajo prueba (ver 08, `guia-testing-extensibilidad_v1.0.md`). |
| Destino del proveedor nuevo (efímero) | Un destino real efímero o un doble de conformidad que cumpla el puerto; siempre con credenciales sintéticas no productivas (RN-03). |

## 4. Cómo correrlo

1. Clonar el repositorio y entrar a la carpeta del sample: `cd samples/geovial-storage/03-avanzado-integracion-real`.
2. Restaurar las dependencias con el gestor de paquetes del ecosistema.
3. Ejecutar el comando que registra el proveedor nuevo en el registro de proveedores y lo selecciona como activo (reusa CU-06).
4. Ejecutar la suite de conformidad parametrizada con el proveedor nuevo, que la compara contra el doble en memoria.
5. Comparar la salida con el output esperado de la sección 6 y confirmar que el diff se limita al adaptador, su registro y sus fixtures.

## 5. Estructura del código

```
03-avanzado-integracion-real/
├── README.md                         # Resumen del sample y enlace a este markdown
├── src/
│   ├── ProveedorNuevoAdapter.<ext>   # Implementa el puerto: persistir, leer, borrar, presencia, enumerar, validar
│   ├── RegistroProveedorNuevo.<ext>  # Declara parametros/credenciales y registra el adaptador
│   └── Program.<ext>                 # Registra, selecciona y corre el flujo de consumo
└── tests/
    ├── conformidad-proveedor.<ext>   # Suite de conformidad parametrizada con el proveedor nuevo
    └── no-filtracion.<ext>           # Property-based: 0 credenciales en errores y registros
```

## 6. Qué esperar

Salida esperada al registrar el proveedor nuevo y correr la suite de conformidad:

```
Adaptador implementado: puerto de proveedor (6 operaciones)
Registro -> identificador de proveedor: otro  registrado: si
Seleccion -> proveedor activo: otro (CU-06)

Suite de conformidad (oraculo: doble en memoria)
  Ida y vuelta con igualdad binaria .......... OK
  Guardado de contenido vacio rechazado ...... OK
  Identificador duplicado sin sobrescritura .. OK
  Sobrescritura explicita .................... OK
  Recuperacion de inexistente ................ OK
  Recuperacion por rango ..................... OK
  Eliminacion idempotente de inexistente ..... OK
  Eliminacion multiple por prefijo ........... OK
  Coherencia presencia/ausencia .............. OK
  Listado con paginacion por testigo ......... OK
  Proveedor no disponible sin filtrar creds .. OK
  Tamanio maximo configurado ................. OK
  Equivalencia con el oraculo ................ OK

Resultado: el proveedor nuevo pasa la suite; 0 diferencias con el doble en memoria; nucleo y superficie publica sin cambios
```

El criterio de éxito es la última línea: el proveedor nuevo produce resultados equivalentes al doble en memoria para todas las entradas, sin que el núcleo ni la superficie pública se modifiquen. Si el adaptador introdujera un código de error propio o recodificara el contenido, los casos de transparencia o de igualdad binaria fallarían.

## 7. Variaciones sugeridas

| Variación | Qué cambiar | Resultado |
| --- | --- | --- |
| Seleccionar un proveedor no registrado | Intentar activar un identificador de proveedor que no se registró | La librería rechaza con `PROVEEDOR_NO_SOPORTADO` y conserva el proveedor activo anterior (CU-06) |
| Romper la integridad a propósito | Hacer que el adaptador recodifique el contenido | El caso de ida y vuelta con igualdad binaria falla (RN-02, NFR-04), señalando la violación |
| Romper la transparencia a propósito | Hacer que el adaptador emita un código de error propio | El caso de equivalencia con el oráculo falla (RN-01, NFR-03), señalando la violación |
| Filtrar una credencial en el error | Incluir la credencial en el detalle de un fallo del adaptador | El property-based de no filtración falla (RN-03, NFR-05), señalando el secreto expuesto |

## 8. Trazabilidad

| Artefacto upstream | Tipo | Cómo lo ilustra este sample |
| --- | --- | --- |
| Puerto de proveedor de almacenamiento | Punto de extensión (05, `extensibilidad_v1.0.md`) | Implementa las seis operaciones del puerto en un adaptador nuevo y lo registra sin tocar el núcleo |
| CU-06 | Caso de uso | Registra y selecciona el proveedor nuevo como activo, reusando la configuración del proveedor activo |
| RN-01 | Regla de negocio | Verifica que el proveedor nuevo no introduce comportamientos ni códigos propios; transparencia garantizada por la suite |
| RN-02 | Regla de negocio | Verifica que el adaptador no transforma ni recodifica el contenido (igualdad binaria) |
| RN-03 | Regla de negocio | Verifica que el adaptador no emite credenciales en errores ni registros |
| ADR-01 | Decisión arquitectónica | Materializa la extensión por adaptador del puerto y selección por estrategia, sin reescribir el núcleo |
| ADR-04 | Decisión arquitectónica | Materializa la suite de conformidad única parametrizada por proveedor con el doble en memoria como oráculo |
| ADR-05 | Decisión arquitectónica | Materializa el manejo seguro de credenciales verificado por el property-based de no filtración |
| NFR-03 | Requisito no funcional | 0 diferencias de comportamiento observable del proveedor nuevo respecto del oráculo |
| NFR-05 | Requisito no funcional | 0 ocurrencias de credenciales en resultados, errores y registros del adaptador |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Versión inicial del sample avanzado: implementación y registro de un proveedor nuevo contra el puerto de proveedor y validación con la suite de conformidad, sin modificar el núcleo (extensibilidad, CU-06, RN-01, RN-02, RN-03, NFR-03, NFR-05). |
