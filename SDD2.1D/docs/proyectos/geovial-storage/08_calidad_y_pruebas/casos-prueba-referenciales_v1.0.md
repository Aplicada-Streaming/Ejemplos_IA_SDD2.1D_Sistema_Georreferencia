# Casos de prueba referenciales — geovial-storage

**Proyecto:** geovial-storage
**Documento:** casos-prueba-referenciales_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero QA / SDET Senior (variante QA + SDET Library)

## 1. Propósito y convenciones

Catálogo de casos de prueba referenciales (TC-XX). Cada TC referencia al menos un CU, RN o NFR (regla 08 §4.10), declara su tipo, su setup, sus pasos en formato Given-When-Then, su expected output y su status. La numeración es contigua de dos dígitos. El status inicial es Pendiente: los tests aún no están implementados (el mini-plan de 07 arranca el 2026-06-15); el campo "Actual" se completa en la última ejecución y la matriz de cobertura se actualiza al cierre de cada tramo.

Hay al menos un TC por CU crítico y por RN. Los contract tests (TC-21, TC-22, TC-27) validan que la superficie pública no cambia entre proveedores (transparencia, RN-01). El proveedor se nombra de forma neutral: local / remoto / otro, y "doble en memoria" para el doble de conformidad.

## 2. Catálogo de casos de prueba

### TC-01 — guardar-feliz

- Tipo: unit
- Cubre: CU-01 (CA-01), RN-02
- Setup: doble de proveedor en memoria activo; contenido sintético de 245 KB; destino `relevamientos/2026/r-001/`; tipo `image/jpeg`.
- Pasos: Given un proveedor activo y un contenido de 245 KB con destino válido y tipo `image/jpeg`, When el consumidor invoca guardar, Then la librería devuelve un identificador lógico no vacío y un tamaño persistido de 245 KB.
- Expected output: identificador no vacío; tamaño persistido = 245 KB.
- Actual output: pendiente.
- Status: Pendiente.

### TC-02 — guardar-contenido-vacio

- Tipo: unit
- Cubre: CU-01 (CA-02)
- Setup: doble en memoria activo; contenido de 0 bytes; destino válido.
- Pasos: Given un contenido de 0 bytes con destino válido, When el consumidor invoca guardar, Then la librería rechaza con CONTENIDO_VACIO y no crea ningún archivo.
- Expected output: error CONTENIDO_VACIO; no se contacta al proveedor; sin archivo creado.
- Actual output: pendiente.
- Status: Pendiente.

### TC-03 — guardar-identificador-duplicado

- Tipo: unit
- Cubre: CU-01 (CA-03), RN-02
- Setup: doble en memoria con `relevamientos/2026/r-001/foto-01.jpg` ya presente; sin marca de sobrescritura.
- Pasos: Given un identificador que ya existe sin marca de sobrescritura, When el consumidor invoca guardar con ese identificador, Then la librería rechaza con IDENTIFICADOR_DUPLICADO y conserva el archivo preexistente.
- Expected output: error IDENTIFICADOR_DUPLICADO; archivo preexistente intacto.
- Actual output: pendiente.
- Status: Pendiente.

### TC-04 — guardar-sobrescritura-explicita

- Tipo: unit
- Cubre: CU-01 (CA-04), RN-02
- Setup: doble en memoria con `relevamientos/2026/r-001/foto-01.jpg` ya presente; marca de sobrescritura activada; contenido nuevo distinto.
- Pasos: Given un identificador existente con marca de sobrescritura activada, When el consumidor invoca guardar con ese identificador, Then la librería reemplaza el contenido y devuelve el mismo identificador.
- Expected output: mismo identificador; contenido recuperado posterior = contenido nuevo (igualdad binaria).
- Actual output: pendiente.
- Status: Pendiente.

### TC-05 — recuperar-feliz-igualdad-binaria

- Tipo: unit
- Cubre: CU-02 (CA-01), RN-02, NFR-04
- Setup: doble en memoria con `relevamientos/2026/r-001/foto-01.jpg` de 245 KB tipo `image/jpeg` previamente guardado.
- Pasos: Given un archivo previamente guardado de 245 KB y tipo `image/jpeg`, When el consumidor invoca recuperar con ese identificador, Then la librería devuelve un contenido de 245 KB idénticamente igual byte a byte al guardado y el tipo `image/jpeg`.
- Expected output: contenido idéntico byte a byte; tipo `image/jpeg`.
- Actual output: pendiente.
- Status: Pendiente.

### TC-06 — recuperar-identificador-inexistente

- Tipo: unit
- Cubre: CU-02 (CA-02)
- Setup: doble en memoria sin el identificador `relevamientos/2026/r-001/inexistente.jpg`.
- Pasos: Given un identificador que nunca fue guardado, When el consumidor invoca recuperar con ese identificador, Then la librería devuelve IDENTIFICADOR_INEXISTENTE sin contenido.
- Expected output: error IDENTIFICADOR_INEXISTENTE; sin contenido.
- Actual output: pendiente.
- Status: Pendiente.

### TC-07 — recuperar-por-rango

- Tipo: unit
- Cubre: CU-02 (CA-03), RN-02
- Setup: doble en memoria con un archivo de 245 KB guardado; solicitud de rango de bytes 0 a 1023.
- Pasos: Given un archivo guardado de 245 KB y una solicitud del rango 0 a 1023, When el consumidor invoca recuperar por rango, Then la librería devuelve exactamente 1024 bytes correspondientes al inicio del archivo.
- Expected output: exactamente 1024 bytes; segmento igual al tramo inicial del original.
- Actual output: pendiente.
- Status: Pendiente.

### TC-08 — recuperar-proveedor-no-disponible-sin-credenciales

- Tipo: unit
- Cubre: CU-02 (CA-04), RN-03, NFR-05
- Setup: doble en memoria configurado para simular un proveedor que no responde, con credenciales sintéticas configuradas.
- Pasos: Given un proveedor activo que no responde, When el consumidor invoca recuperar un identificador válido, Then la librería devuelve PROVEEDOR_NO_DISPONIBLE sin exponer las credenciales del proveedor.
- Expected output: error PROVEEDOR_NO_DISPONIBLE; el error no contiene ninguna credencial ni parámetro de conexión.
- Actual output: pendiente.
- Status: Pendiente.

### TC-09 — eliminar-feliz-y-coherencia

- Tipo: unit
- Cubre: CU-03 (CA-01), RN-02
- Setup: doble en memoria con `relevamientos/2026/r-001/foto-01.jpg` guardado.
- Pasos: Given un archivo guardado, When el consumidor invoca eliminar con ese identificador, Then la librería confirma la eliminación y una verificación posterior (CU-04) reporta el identificador como inexistente.
- Expected output: eliminación confirmada; verificación posterior = ausente.
- Actual output: pendiente.
- Status: Pendiente.

### TC-10 — eliminar-idempotente-inexistente

- Tipo: unit
- Cubre: CU-03 (CA-02)
- Setup: doble en memoria sin el identificador `relevamientos/2026/r-001/inexistente.jpg`.
- Pasos: Given un identificador que no existe, When el consumidor invoca eliminar con ese identificador, Then la librería informa éxito por idempotencia, sin error.
- Expected output: éxito por idempotencia; sin error.
- Actual output: pendiente.
- Status: Pendiente.

### TC-11 — eliminar-multiple-bajo-prefijo

- Tipo: unit
- Cubre: CU-03 (CA-03)
- Setup: doble en memoria con tres archivos bajo `relevamientos/2026/r-001/`.
- Pasos: Given tres archivos bajo el prefijo `relevamientos/2026/r-001/`, When el consumidor invoca eliminar bajo ese prefijo, Then la librería devuelve una cantidad eliminada de 3.
- Expected output: cantidad eliminada = 3.
- Actual output: pendiente.
- Status: Pendiente.

### TC-12 — verificar-presencia-y-ausencia

- Tipo: unit
- Cubre: CU-04 (CA-01, CA-02, CA-03), RN-02
- Setup: doble en memoria con `relevamientos/2026/r-001/foto-01.jpg` de 245 KB; un identificador inexistente; un identificador previamente eliminado por CU-03.
- Pasos: Given un archivo guardado de 245 KB, un identificador inexistente y uno eliminado, When el consumidor invoca verificar sobre cada uno, Then la librería devuelve presencia verdadera y tamaño 245 KB para el primero, y presencia falsa para los otros dos.
- Expected output: presente=true, tamaño=245 KB para el guardado; presente=false para inexistente y para eliminado.
- Actual output: pendiente.
- Status: Pendiente.

### TC-13 — listar-bajo-prefijo-y-vacio

- Tipo: unit
- Cubre: CU-05 (CA-01, CA-03)
- Setup: doble en memoria con tres archivos bajo `relevamientos/2026/r-001/` y ninguno bajo `relevamientos/2026/r-999/`.
- Pasos: Given tres archivos bajo un prefijo y ninguno bajo otro, When el consumidor invoca listar con cada prefijo, Then la librería devuelve los tres identificadores sin testigo de continuación para el primero y una lista vacía sin error para el segundo.
- Expected output: tres identificadores y sin testigo; lista vacía sin error.
- Actual output: pendiente.
- Status: Pendiente.

### TC-14 — listar-paginacion-por-testigo

- Tipo: unit
- Cubre: CU-05 (CA-02), CU-05 (FA-01)
- Setup: doble en memoria con diez archivos bajo `relevamientos/2026/r-002/`; tamaño de página = 4.
- Pasos: Given diez archivos bajo el prefijo y un tamaño de página de 4, When el consumidor invoca listar con ese prefijo y ese tamaño, Then la librería devuelve 4 identificadores y un testigo de continuación no vacío.
- Expected output: 4 identificadores; testigo de continuación no vacío; las páginas siguientes completan los 10 sin repetir.
- Actual output: pendiente.
- Status: Pendiente.

### TC-15 — listar-proveedor-no-disponible-sin-credenciales

- Tipo: unit
- Cubre: CU-05 (CA-04), RN-03, NFR-05
- Setup: doble en memoria que simula proveedor que no responde, con credenciales sintéticas configuradas.
- Pasos: Given un proveedor activo que no responde, When el consumidor invoca listar bajo un prefijo válido, Then la librería devuelve PROVEEDOR_NO_DISPONIBLE sin exponer las credenciales del proveedor.
- Expected output: error PROVEEDOR_NO_DISPONIBLE; el error no contiene credenciales.
- Actual output: pendiente.
- Status: Pendiente.

### TC-16 — configurar-activar-proveedor-remoto

- Tipo: integration
- Cubre: CU-06 (CA-01), RN-01
- Setup: proveedor activo = local; parámetros y credenciales sintéticas válidas de un proveedor remoto; doble de conformidad del proveedor remoto disponible.
- Pasos: Given el proveedor activo local y parámetros válidos de un proveedor remoto con credenciales correctas, When el usuario raíz activa el proveedor remoto, Then la librería confirma el cambio y las operaciones siguientes usan el proveedor remoto sin cambiar la forma de invocarlas.
- Expected output: cambio confirmado; CU-01 a CU-05 siguen funcionando con la misma firma.
- Actual output: pendiente.
- Status: Pendiente.

### TC-17 — configurar-credenciales-invalidas

- Tipo: unit
- Cubre: CU-06 (CA-02), RN-03
- Setup: proveedor activo = local; credenciales con formato inválido para un proveedor remoto.
- Pasos: Given credenciales con formato inválido para un proveedor remoto, When el usuario raíz intenta activar ese proveedor, Then la librería rechaza con CREDENCIALES_INVALIDAS y mantiene el proveedor local como activo.
- Expected output: error CREDENCIALES_INVALIDAS; proveedor activo sigue siendo local; sin intento de conectividad.
- Actual output: pendiente.
- Status: Pendiente.

### TC-18 — configurar-proveedor-inaccesible

- Tipo: integration
- Cubre: CU-06 (CA-03)
- Setup: proveedor activo = local; credenciales con formato válido pero proveedor remoto que rechaza la conexión.
- Pasos: Given credenciales con formato válido pero un proveedor remoto que rechaza la conexión, When el usuario raíz intenta activar ese proveedor, Then la librería rechaza con PROVEEDOR_INACCESIBLE y mantiene el proveedor activo anterior.
- Expected output: error PROVEEDOR_INACCESIBLE; proveedor activo anterior conservado; sin estado de configuración a medias.
- Actual output: pendiente.
- Status: Pendiente.

### TC-19 — configurar-autorizacion-insuficiente

- Tipo: unit
- Cubre: CU-06 (CA-04), RN-03
- Setup: actor sin alcance de usuario raíz.
- Pasos: Given un actor sin alcance de usuario raíz, When ese actor intenta cambiar el proveedor activo, Then la librería rechaza con AUTORIZACION_INSUFICIENTE y no modifica el proveedor activo.
- Expected output: error AUTORIZACION_INSUFICIENTE; proveedor activo sin cambios.
- Actual output: pendiente.
- Status: Pendiente.

### TC-20 — validacion-en-seco-sin-activar

- Tipo: unit
- Cubre: CU-06 (FA-02), US-09
- Setup: proveedor activo = local; configuración candidata de un proveedor remoto; modo validación-en-seco.
- Pasos: Given una configuración candidata y el modo validación-en-seco, When el usuario raíz solicita validar sin activar, Then la librería reporta el resultado de la validación sin cambiar el proveedor activo.
- Expected output: resultado de validación; proveedor activo sigue siendo local.
- Actual output: pendiente.
- Status: Pendiente.

### TC-21 — contrato-transparencia-bateria-unica

- Tipo: contract
- Cubre: RN-01, NFR-03, CU-01 a CU-05
- Setup: la misma batería de pruebas de contrato parametrizada se ejecuta una vez por cada proveedor soportado (local, remoto, doble en memoria) con las mismas entradas.
- Pasos: Given una misma batería de operaciones válidas y de error, When se ejecuta contra cada proveedor soportado, Then los resultados y los códigos de error son equivalentes para las mismas entradas y el consumidor no requiere ninguna rama por proveedor.
- Expected output: 0 diferencias de comportamiento observable entre proveedores; mismo conjunto de códigos de error.
- Actual output: pendiente.
- Status: Pendiente.

### TC-22 — contrato-superficie-estable-snapshot

- Tipo: snapshot
- Cubre: RN-01, ADR-02, ADR-03
- Setup: snapshot de la forma de la superficie pública (operaciones, parámetros) y del catálogo de códigos de error uniforme.
- Pasos: Given el snapshot vigente del contrato y del catálogo de errores, When se ejecuta la suite, Then la forma de la superficie pública y los códigos de error coinciden con el snapshot, salvo cambio aprobado por PR con justificación.
- Expected output: snapshot coincide; un cambio incompatible no aprobado falla el test (detecta breaking change no intencional).
- Actual output: pendiente.
- Status: Pendiente.

### TC-23 — propiedad-igualdad-binaria

- Tipo: property-based
- Cubre: RN-02, NFR-04, CU-01, CU-02
- Setup: generador de contenidos binarios no vacíos con semilla registrada; doble en memoria.
- Pasos: Given todo contenido no vacío y todo identificador lógico válido generados, When se guarda y luego se recupera sin sobrescribir ni eliminar, Then el contenido recuperado es idénticamente igual byte a byte al guardado.
- Expected output: la propiedad se sostiene para todos los casos generados; un contraejemplo se reporta con su semilla.
- Actual output: pendiente.
- Status: Pendiente.

### TC-24 — propiedad-no-filtracion-credenciales

- Tipo: property-based
- Cubre: RN-03, NFR-05
- Setup: generador de operaciones que producen error; credenciales sintéticas configuradas y registradas.
- Pasos: Given todo error producido por cualquier operación con credenciales configuradas, When se inspeccionan el mensaje y los datos del error, Then ninguno contiene credenciales ni parámetros de conexión.
- Expected output: 0 ocurrencias de credenciales en cualquier error; un contraejemplo se reporta con su semilla.
- Actual output: pendiente.
- Status: Pendiente.

### TC-25 — nfr-latencia-p95-local

- Tipo: integration (desempeño)
- Cubre: NFR-01
- Setup: proveedor local activo en ambiente equivalente al productivo; lote de archivos de hasta 5 MB; banco de medición de latencia p95.
- Pasos: Given el proveedor local activo y archivos de hasta 5 MB, When se ejecuta un lote de operaciones guardar y recuperar, Then la latencia p95 medida desde la superficie pública hasta el resultado es ≤ 1 s.
- Expected output: latencia p95 ≤ 1 s para archivos ≤ 5 MB.
- Actual output: pendiente.
- Status: Pendiente.

### TC-26 — nfr-tamano-maximo-configurable

- Tipo: unit
- Cubre: NFR-02, CU-01
- Setup: tamaño máximo configurado por defecto en 25 MB; un contenido en el límite (25 MB) y otro por encima.
- Pasos: Given el tamaño máximo configurado en 25 MB, When el consumidor invoca guardar con un contenido en el límite y con uno por encima, Then el del límite se persiste y el que excede se rechaza con TAMANIO_EXCEDIDO antes de contactar al proveedor.
- Expected output: contenido de 25 MB persistido; contenido > 25 MB rechazado con TAMANIO_EXCEDIDO sin delegar en el proveedor.
- Actual output: pendiente.
- Status: Pendiente.

### TC-27 — extensibilidad-proveedor-nuevo-conformidad

- Tipo: contract
- Cubre: RN-01, ADR-01, ADR-04, extensibilidad
- Setup: un proveedor nuevo registrado (un adaptador de conformidad de ejemplo) que implementa el puerto de proveedor de almacenamiento.
- Pasos: Given un proveedor nuevo registrado que implementa el puerto, When se ejecuta la suite de conformidad de proveedor, Then produce resultados equivalentes a los demás proveedores para las mismas entradas, sin tocar el núcleo.
- Expected output: el proveedor nuevo pasa la batería de conformidad completa; el núcleo no se modifica.
- Actual output: pendiente.
- Status: Pendiente.

### TC-28 — extensibilidad-proveedor-no-soportado

- Tipo: unit
- Cubre: RN-01, CU-06, extensibilidad
- Setup: una solicitud de activación de un proveedor que no está registrado.
- Pasos: Given un proveedor que no está en el registro, When el usuario raíz intenta activarlo, Then la librería rechaza con PROVEEDOR_NO_SOPORTADO y conserva el proveedor activo anterior.
- Expected output: error PROVEEDOR_NO_SOPORTADO; proveedor activo conservado.
- Actual output: pendiente.
- Status: Pendiente.

## 3. Resumen de cobertura del catálogo

| CU / RN / NFR | TC asociados |
| --- | --- |
| CU-01 | TC-01, TC-02, TC-03, TC-04, TC-23, TC-26 |
| CU-02 | TC-05, TC-06, TC-07, TC-08, TC-23 |
| CU-03 | TC-09, TC-10, TC-11 |
| CU-04 | TC-12 |
| CU-05 | TC-13, TC-14, TC-15 |
| CU-06 | TC-16, TC-17, TC-18, TC-19, TC-20, TC-28 |
| RN-01 | TC-16, TC-21, TC-22, TC-27, TC-28 |
| RN-02 | TC-01, TC-03, TC-04, TC-05, TC-07, TC-09, TC-12, TC-23 |
| RN-03 | TC-08, TC-15, TC-17, TC-19, TC-24 |
| NFR-01 | TC-25 |
| NFR-02 | TC-26 |
| NFR-03 | TC-21 |
| NFR-04 | TC-05, TC-23 |
| NFR-05 | TC-08, TC-15, TC-24 |

## 4. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Catálogo inicial de 28 casos de prueba referenciales de geovial-storage: al menos un TC por CU y por RN, contract tests de transparencia (TC-21, TC-22, TC-27), property-based para integridad y no filtración (TC-23, TC-24) y TC numéricos para NFR-01 y NFR-02. Status inicial Pendiente (tests no implementados; el mini-plan arranca 2026-06-15). |
| 1.0 | 2026-06-15 | Corrección de consistencia interna (sin cambio de alcance): en §1 se corrige la mención equivocada a TC-18/TC-19 como batería de transparencia; los contract tests de transparencia (RN-01) son TC-21, TC-22 y TC-27, conforme a la matriz de cobertura. No se altera la numeración ni el mapeo TC↔CU/RN/NFR del catálogo. |
