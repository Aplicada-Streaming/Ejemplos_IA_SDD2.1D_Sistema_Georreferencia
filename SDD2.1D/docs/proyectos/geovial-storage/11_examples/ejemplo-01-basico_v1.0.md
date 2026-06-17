# Ejemplo 01 — Guardar y recuperar un archivo con el proveedor local

**Proyecto:** geovial-storage
**Documento:** ejemplo-01-basico_v1.0.md
**Versión:** 1.0
**Estado:** Vigente
**Fecha:** 2026-06-15
**Autor:** Developer Advocate / Sample Engineer Senior
**Nivel:** Básico
**Ubicación del código:** `/samples/geovial-storage/01-basico-consola/`

## 1. Objetivo del sample

Demostrar el camino feliz mínimo de la abstracción de almacenamiento: configurar el proveedor local, guardar un contenido y recuperarlo, comprobando que lo recuperado es idénticamente igual a lo guardado. Al finalizar, el desarrollador sabe obtener su primer resultado exitoso con la librería y entiende que el contrato devuelve un identificador lógico estable con el que recuperar el archivo después, sin conocer su ubicación física.

## 2. Nivel

Básico. Es el punto de entrada absoluto: solo el proveedor local, dos operaciones de datos (guardar y recuperar) y una configuración inicial sin credenciales remotas. No asume ningún sample previo. Los samples siguientes parten de acá para agregar la gestión completa (sample 02) y la extensión del puerto de proveedor (sample 03).

## 3. Prerequisites

| Prerequisito | Versión mínima / cómo obtenerlo |
| --- | --- |
| Runtime del consumidor | El declarado para la librería en intake §17.P.9 (proyecto base de la solución). |
| Gestor de paquetes del ecosistema | El del runtime anterior, para restaurar las dependencias del sample. |
| Editor con soporte del lenguaje del proyecto | Cualquiera con resaltado y ejecución del runtime objetivo. |
| Una ubicación local accesible y escribible | Cualquier carpeta del entorno con permisos de lectura y escritura; la usa el proveedor local. No se requieren credenciales remotas. |

## 4. Cómo correrlo

1. Clonar el repositorio de la solución.
2. Entrar a la carpeta del sample: `cd samples/geovial-storage/01-basico-consola`.
3. Restaurar las dependencias con el gestor de paquetes del ecosistema.
4. Ejecutar el comando de arranque del sample, que crea una ubicación local temporal, guarda un contenido de prueba y lo recupera.
5. Comparar la salida en consola con el output esperado de la sección 6.

## 5. Estructura del código

```
01-basico-consola/
├── README.md                     # Resumen del sample y enlace a este markdown
├── src/
│   ├── Program.<ext>             # Punto de entrada: configura local, guarda y recupera
│   └── data/foto-demo.<ext>      # Contenido de prueba no vacío que se guarda
└── tests/
    └── ida-vuelta-test.<ext>     # Verifica la igualdad binaria guardar-recuperar
```

## 6. Qué esperar

Salida esperada en consola:

```
Proveedor activo: local (ubicacion temporal)
Guardado OK   -> identificador: pruebas/quick-start/foto-demo  tamanio: 245 KB
Recuperado OK -> tamanio: 245 KB  igualdad binaria: si
Resultado: el contenido recuperado es identicamente igual al guardado
```

El criterio de éxito es la última línea: la igualdad binaria entre lo guardado en el paso de guardado y lo recuperado confirma la integridad (RN-02). Si la ubicación local no es accesible o escribible, la configuración se rechaza con `PROVEEDOR_INACCESIBLE` y no se intenta guardar.

## 7. Variaciones sugeridas

| Variación | Qué cambiar | Resultado |
| --- | --- | --- |
| Guardar contenido vacío | Entregar un contenido de 0 bytes a la operación de guardado | La librería rechaza con `CONTENIDO_VACIO` y no crea ningún archivo (CU-01, CA-02) |
| Recuperar un identificador inexistente | Pedir recuperar un identificador que nunca se guardó | La librería devuelve `IDENTIFICADOR_INEXISTENTE` sin contenido (CU-02, CA-02) |
| Recuperar por rango | Pedir solo los primeros 1024 bytes del archivo guardado | La librería devuelve exactamente 1024 bytes del inicio, con integridad del segmento (CU-02, CA-03); puente hacia el sample 02 |

## 8. Trazabilidad

| Artefacto upstream | Tipo | Cómo lo ilustra este sample |
| --- | --- | --- |
| CU-01 | Caso de uso | Guarda un contenido no vacío con el proveedor local y obtiene un identificador lógico y el tamaño persistido (flujo principal) |
| CU-02 | Caso de uso | Recupera el contenido por su identificador y verifica que es idénticamente igual al guardado (flujo principal y CA-01) |
| RN-02 | Regla de negocio | Verifica la igualdad binaria guardar-recuperar; la librería no transforma ni recodifica el contenido |
| ADR-04 | Decisión arquitectónica | Materializa la garantía de integridad byte a byte sostenida por la ausencia de transformación |
| NFR-04 | Requisito no funcional | El test del sample comprueba 100 % de igualdad binaria byte a byte |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Versión inicial del sample básico: configuración del proveedor local, guardado y recuperación con verificación de igualdad binaria (CU-01, CU-02, RN-02, NFR-04). |
