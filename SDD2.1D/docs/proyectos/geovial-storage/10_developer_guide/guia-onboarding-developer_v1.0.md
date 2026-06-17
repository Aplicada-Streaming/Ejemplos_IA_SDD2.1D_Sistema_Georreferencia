# Guía de onboarding del developer — geovial-storage

**Proyecto:** geovial-storage
**Documento:** guia-onboarding-developer_v1.0.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Technical Writer + SDK Documentation Lead
**Tipo Diátaxis:** Tutorial
**Audiencia:** Developer integrador del backend que consume la abstracción de almacenamiento
**Nivel:** Básico
**Tiempo estimado de lectura:** 15 min

Esta guía lleva a un developer nuevo desde cero hasta el cambio de proveedor activo sin tocar su código, en menos de una hora. Cada paso declara su efecto esperado, observable, para confirmar que el hito se alcanzó. El recorrido se describe en operaciones y resultados; el código ejecutable equivalente vive en 11 y el detalle de incorporación al backend, en 05/11.

## 1. Prerequisites

| Prerequisito | Cómo obtenerlo |
| --- | --- |
| La librería disponible para el backend | Incorporar la abstracción de almacenamiento según la categoría 05/11. |
| Capacidad de editar la configuración del backend y ejecutar sus pruebas | Acceso al repositorio del backend que integra la librería. |
| Una ubicación local accesible y escribible | Cualquier carpeta del entorno con permisos de lectura y escritura; se usa para el proveedor local. |
| Credenciales de un proveedor remoto (solo para el paso 4) | Provistas por el usuario raíz; no se necesitan para los pasos 2 y 3. |

No se requieren credenciales remotas para empezar: el proveedor local siempre está disponible como mínimo (CU-06, precondición). El cambio a un proveedor remoto recién aparece en el paso de integración.

## 2. Hello world (5 min)

Objetivo: obtener el primer resultado exitoso con el proveedor local. El hito es duro: el contenido recuperado debe ser idéntico al guardado.

Paso 1. Configurar el proveedor activo como proveedor local, indicando la ubicación local accesible y escribible.

```
configurar(proveedor="local", parametros-y-credenciales={ ubicacion: "<carpeta-local>" })
```

Efecto esperado: la librería confirma que el proveedor local quedó activo (CU-06, FA-01). Si la ubicación no es accesible o escribible, la activación se rechaza con `PROVEEDOR_INACCESIBLE` y la configuración previa sigue vigente.

Paso 2. Guardar un contenido de prueba no vacío bajo un prefijo válido.

```
guardar(contenido="hola-geovial", destino="pruebas/quick-start/", tipo="text/plain")
```

Efecto esperado: la librería devuelve un identificador lógico no vacío y el tamaño persistido, igual al tamaño entregado (CU-01, paso 5).

Paso 3. Recuperar ese identificador.

```
recuperar(identificador=<el devuelto en el paso 2>)
```

Efecto esperado: la librería devuelve un contenido idénticamente igual, byte a byte, al guardado (CU-02, CA-01; RN-02).

Criterio de éxito del Hello world: el contenido del paso 3 es idéntico al del paso 2. Si se cumple, el primer resultado exitoso está logrado.

## 3. Primer caso real (30 min)

Objetivo: integrar el contrato en un flujo propio con datos representativos y manejo de al menos un error catalogado, sin que el flujo se caiga. Caso: gestionar las fotografías de un relevamiento.

Paso 1. Guardar una fotografía de relevamiento bajo un prefijo propio.

```
guardar(contenido=<bytes de la foto>, destino="relevamientos/2026/r-001/", tipo="image/jpeg")
```

Efecto esperado: identificador lógico no vacío (por ejemplo `relevamientos/2026/r-001/foto-01.jpg`) y tamaño persistido (por ejemplo 245 KB) (CU-01, CA-01).

Paso 2. Verificar la existencia del identificador devuelto.

```
verificar(identificador=<el devuelto en el paso 1>)
```

Efecto esperado: presencia verdadera y el tamaño del archivo (CU-04, CA-01).

Paso 3. Recuperar por rango los primeros 1024 bytes.

```
recuperar(identificador=<el devuelto>, rango=[0, 1023])
```

Efecto esperado: exactamente 1024 bytes correspondientes al inicio del archivo, con integridad del segmento (CU-02, CA-03).

Paso 4. Listar bajo el prefijo del relevamiento.

```
listar(prefijo="relevamientos/2026/r-001/")
```

Efecto esperado: la lista contiene el identificador recién guardado (CU-05, CA-01).

Paso 5. Provocar y manejar un error catalogado: recuperar un identificador inexistente.

```
recuperar(identificador="relevamientos/2026/r-001/inexistente.jpg")
```

Efecto esperado: la librería devuelve `IDENTIFICADOR_INEXISTENTE` sin contenido (CU-02, CA-02). El flujo debe capturar el código y continuar; no debe caerse. Programar contra el código, no contra el texto del mensaje.

Criterio de éxito del primer caso real: los pasos 1 a 4 producen sus efectos esperados y el paso 5 se maneja sin interrumpir el flujo.

## 4. Integración con un servicio backend (1 hora)

Objetivo: cambiar el proveedor activo sin tocar el código de integración escrito en §3. Este paso materializa la promesa de transparencia (RN-01): el éxito se mide en que el código del developer no cambia al cambiar el proveedor. Es el time-to-first-value de la librería. El detalle completo está en `guia-integracion-servicio-backend_v1.0.md`.

Paso 1. Inyectar la abstracción de almacenamiento en un servicio del backend a través de su interfaz, no de un proveedor concreto. El servicio depende del contrato, nunca de un destino físico.

Efecto esperado: el servicio compila y opera contra el proveedor local ya configurado, reutilizando el flujo de §3.

Paso 2. El usuario raíz valida en seco una configuración de proveedor remoto, sin activarla.

```
configurar(proveedor="remoto", parametros-y-credenciales=cfg, validacion-en-seco=verdadero)
```

Efecto esperado: la librería reporta el resultado de la validación (soporte, formato, conectividad, permisos) y el proveedor activo NO cambia (CU-06, FA-02). Si las credenciales tienen formato inválido, devuelve `CREDENCIALES_INVALIDAS`; si el remoto rechaza la conexión, `PROVEEDOR_INACCESIBLE`. Ninguno expone las credenciales (RN-03).

Paso 3. El usuario raíz activa el proveedor remoto.

```
configurar(proveedor="remoto", parametros-y-credenciales=cfg)
```

Efecto esperado: la librería confirma el cambio sin revelar las credenciales (CU-06, CA-01).

Paso 4. Reejecutar el flujo de §3 sin modificar una sola línea de código del servicio.

Efecto esperado: guardar, verificar, recuperar, listar y el manejo de error producen los mismos resultados observables que con el proveedor local. Un listado bajo prefijo enumera los archivos del relevamiento. No hay ramas de código por proveedor (RN-01).

Criterio de éxito de la integración: el flujo de §3 funciona idéntico contra el proveedor remoto sin cambios de código. Si se cumple, el developer cerró la primera hora.

## 5. Siguientes pasos

- Profundizar en el porqué: `conceptos-fundamentales_v1.0.md` (transparencia, integridad, credenciales, versionado).
- Detalle de la integración en un servicio backend: `guia-integracion-servicio-backend_v1.0.md`.
- Dato exacto de cada operación, parámetro o código de error: `referencia-api_v1.0.md`.
- Resolver un error frecuente: `troubleshooting_v1.0.md`.
- Ejemplos ejecutables: categoría 11 (`samples/geovial-storage/`).

## 6. Referencias cruzadas

- 05 `adrs/ADR-01-abstraccion-proveedores-intercambiables_v1.0.md`: por qué cambiar de proveedor no cambia el código del consumidor.
- 05 `adrs/ADR-04-transparencia-limites-proveedor_v1.0.md`: garantía de igualdad binaria que sostiene el criterio de éxito del Hello world.
- 02 `casos-de-uso/CU-01-guardar-archivo_v1.0.md` y `CU-06-configurar-proveedor-activo_v1.0.md`: flujos y criterios de aceptación citados en cada paso.
- 03 `guia-onboarding-developer_v1.0.md` y `dx-developer-experience_v1.0.md` §2: tramos 5/30/60 y métricas TTFS/TTFV de origen.
- 10 `referencia-api_v1.0.md`, `guia-integracion-servicio-backend_v1.0.md`, `troubleshooting_v1.0.md`: continuación del recorrido.

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Tutorial de onboarding inicial: prerequisites, Hello world con proveedor local (< 5 min), primer caso real con relevamiento y manejo de error (< 30 min), integración con cambio de proveedor sin tocar el código (< 1 hora) y siguientes pasos enlazados a los demás documentos. |
