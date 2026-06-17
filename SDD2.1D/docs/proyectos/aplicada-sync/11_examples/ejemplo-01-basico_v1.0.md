# Ejemplo 01 — Primer ciclo de sincronización subir-luego-bajar

**Proyecto:** aplicada-sync
**Documento:** ejemplo-01-basico_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Sample Engineer (AG-11)
**Nivel:** Básico
**Ubicación del código:** `/samples/aplicada-sync/01-basico/`

## 1. Objetivo del sample

Demostrar el camino feliz mínimo de la librería: inicializar una sesión de sincronización con la configuración obligatoria y ejecutar un único ciclo subir-luego-bajar. Al terminar, el desarrollador sabe armar la configuración de sesión, obtener un identificador de sesión en estado listo y disparar un ciclo que sube los cambios pendientes y recién después baja las actualizaciones, leyendo el resumen del ciclo para confirmar el resultado.

## 2. Nivel

Básico. Es el punto de entrada absoluto de la librería: no asume haber leído ningún sample previo. Solo ejercita los dos puntos de extensión obligatorios contra dobles de prueba y la operación de ejecución en su forma más simple. Deja para el sample intermedio la cola con varios cambios, la consulta de estado, la reanudación y la convivencia con conflicto.

## 3. Prerequisites

- El paquete distribuible de la librería incorporado al proyecto del sample desde el repositorio de distribución, en la versión de superficie pública vigente del producto.
- Runtime objetivo del ecosistema, en la versión mínima declarada en el SOLUTION-INTAKE §17 P.9 del proyecto aplicada-sync.
- Una estrategia de almacén local de prueba (obligatoria), provista en el propio sample, que persiste la cola y los metadatos de sincronización en memoria o en un almacén local efímero.
- Una estrategia de transporte de prueba (obligatoria), provista en el propio sample, que simula un backend remoto que reconoce el identificador de cambio estable y entrega actualizaciones posteriores a una marca.
- Un proveedor de credencial de prueba (opcional para este sample, incluido para dejar la sesión en estado listo y no en estado no autenticada).

## 4. Cómo correrlo

1. Posicionarse en la carpeta del sample: `cd samples/aplicada-sync/01-basico`.
2. Restaurar las dependencias del sample con el gestor de paquetes del ecosistema.
3. Ejecutar el comando de arranque del sample.
4. Observar en consola el identificador de sesión y el estado inicial listo.
5. Observar el resumen del ciclo impreso al final y compararlo con el output esperado de §6.

## 5. Estructura del código

```
01-basico/
├── README.md                       # Qué demuestra el sample y cómo correrlo
├── src/
│   ├── Programa.<ext>              # Punto de entrada: arma la configuración, inicializa y ejecuta el ciclo
│   ├── AlmacenLocalEnMemoria.<ext> # Estrategia de almacén local de prueba (obligatoria)
│   ├── TransporteSimulado.<ext>    # Estrategia de transporte de prueba (obligatoria)
│   └── CredencialFija.<ext>        # Proveedor de credencial de prueba (opcional)
└── tests/
    └── ciclo_basico_test.<ext>     # Verifica el orden subir-antes-de-bajar y el resumen del ciclo
```

## 6. Qué esperar

Salida esperada en consola, en este orden:

```
Sesion inicializada. Id de sesion: ses-0001. Estado: listo.
Cambios pendientes en cola: 1.
Ejecutando ciclo de sincronizacion...
Fase de subida: 1 cambio subido y confirmado.
Fase de bajada: 2 actualizaciones bajadas y aplicadas.
Resumen del ciclo: subidos=1, bajados=2, en conflicto=0, estado final=listo.
```

El orden de las dos líneas de fase es parte de lo que el sample demuestra: la fase de bajada nunca aparece antes de que la fase de subida informe cero pendientes confirmables restantes.

## 7. Variaciones sugeridas

| Variación | Qué cambiar | Resultado |
| --- | --- | --- |
| Cola vacía al iniciar | No encolar ningún cambio antes de ejecutar | El ciclo omite la fase de subida, reporta `subidos=0` y baja las actualizaciones disponibles |
| Sesión sin credencial | Inicializar sin el proveedor de credencial | La sesión queda en estado no autenticada; admite encolar pero el ciclo no se ejecuta hasta proveer la credencial |
| Configuración incompleta | Omitir la referencia al backend remoto en la configuración | La inicialización se rechaza con el código `CONFIGURACION_INCOMPLETA` y no se crea sesión |

Estas variaciones encadenan con el sample intermedio, que parte de una cola con varios cambios y agrega consulta de estado y reanudación.

## 8. Trazabilidad

| Artefacto upstream | Tipo | Cómo lo ilustra este sample |
| --- | --- | --- |
| CU-01 | Caso de uso | Inicializa y configura la sesión con la configuración obligatoria y obtiene un identificador de sesión en estado listo |
| CU-03 | Caso de uso | Ejecuta un ciclo subir-luego-bajar mínimo y muestra el resumen del ciclo |
| RN-01 | Regla de negocio | El sample evidencia el orden subir-antes-de-bajar en la salida de consola |
| ADR-05 | Decisión arquitectónica | Materializa el pipeline de orden estricto subir-antes-de-bajar |
| ADR-02 | Decisión arquitectónica | Inyecta las estrategias obligatorias desde el host; el motor no instancia adaptadores concretos |
| NFR Orden subir-antes-de-bajar | Atributo de calidad (arquitectura §8) | La verificación del sample comprueba que ninguna bajada precede a la última confirmación de subida |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del sample básico: inicialización de sesión y un ciclo subir-luego-bajar mínimo. Ilustra CU-01 y CU-03. |
