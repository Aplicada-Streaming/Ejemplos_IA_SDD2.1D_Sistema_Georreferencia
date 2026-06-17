# Ejemplo 03 — Integración del motor en una app host ajena y punto de extensión principal

**Proyecto:** aplicada-sync
**Documento:** ejemplo-03-avanzado-integracion-real_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Sample Engineer (AG-11)
**Nivel:** Avanzado
**Ubicación del código:** `/samples/aplicada-sync/03-avanzado-demo-maui/`

## 1. Objetivo del sample

Demostrar la integración completa del motor en una aplicación host ajena al sistema, construida solo para evaluar la librería con vistas a reutilizarla en otros proyectos. El sample implementa las cuatro estrategias del motor con adaptadores propios del integrador y ejercita el punto de extensión principal —el motor de sincronización reutilizable— habilitando además el disparo automático ante recuperación de conectividad. Al terminar, el desarrollador comprueba que el motor opera sin modificación contra adaptadores que no pertenecen al dominio del sistema, validando su reutilización.

## 2. Nivel

Avanzado. Asume haber completado los samples 01 y 02. Es la demostración de integración: a diferencia de los anteriores, que usan dobles de prueba mínimos, este implementa las dos estrategias obligatorias (almacén local y transporte) y las dos opcionales (proveedor de credencial y fuente de eventos de conectividad) en una app host autónoma. Cubre el modo de sincronización automática disparado por conectividad, que los samples previos no ejercitan, y materializa el punto de extensión principal declarado en la arquitectura.

## 3. Prerequisites

- El paquete distribuible de la librería incorporado a la app host de demostración desde el repositorio de distribución, en la versión de superficie pública vigente del producto.
- Runtime objetivo del ecosistema y herramientas de construcción de la app host, en las versiones mínimas declaradas en el SOLUTION-INTAKE §17 P.9 del proyecto aplicada-sync.
- Las cuatro estrategias del motor implementadas en la propia app de demostración: estrategia de almacén local y estrategia de transporte (obligatorias) y proveedor de credencial y fuente de eventos de conectividad (opcionales).
- Un backend remoto de prueba alcanzable por la estrategia de transporte, que reconozca el identificador de cambio estable y pueda reportar un estado en conflicto.
- Un dispositivo o entorno de ejecución de la app host acorde a la plataforma target declarada en el SOLUTION-INTAKE §17 P.9.

## 4. Cómo correrlo

1. Posicionarse en la carpeta del sample: `cd samples/aplicada-sync/03-avanzado-demo-maui`.
2. Restaurar las dependencias del sample con el gestor de paquetes del ecosistema.
3. Configurar el punto de acceso del backend remoto de prueba en el archivo de configuración del sample.
4. Construir y lanzar la app host de demostración con el comando de arranque del sample.
5. Operar el escenario guiado de la app (capturar cambios sin conexión, restablecer la red y observar el disparo automático) y comparar la notificación de resultado con el output esperado de §6.

## 5. Estructura del código

```
03-avanzado-demo-maui/
├── README.md                             # Qué demuestra el sample y cómo correrlo
├── src/
│   ├── HostApp/                          # App host de demostración ajena al sistema
│   │   ├── PuntoDeEntrada.<ext>          # Compone la sesión, registra las cuatro estrategias
│   │   └── PantallaEscenario.<ext>       # Escenario guiado: capturar offline, recuperar red
│   └── Estrategias/
│       ├── AlmacenLocalDelIntegrador.<ext>   # Estrategia de almacen local (obligatoria)
│       ├── TransporteDelIntegrador.<ext>     # Estrategia de transporte (obligatoria)
│       ├── ProveedorDeCredencial.<ext>       # Proveedor de credencial (opcional)
│       └── FuenteDeConectividad.<ext>        # Fuente de eventos de conectividad (opcional)
└── tests/
    ├── disparo_automatico_test.<ext>     # Un único ciclo por recuperación; sin ciclos paralelos
    └── integracion_estrategias_test.<ext># El motor opera con las cuatro estrategias del integrador
```

## 6. Qué esperar

Salida esperada, observada en la notificación de resultado de la app host tras recuperar la conectividad:

```
Sesion inicializada con cuatro estrategias del integrador. Estado: listo.
Capturados 3 cambios sin conexion. Tamano de cola: 3.
Red recuperada. Disparo automatico habilitado: iniciando un unico ciclo.
Fase de subida: 3 cambios subidos y confirmados.
Fase de bajada: 4 actualizaciones bajadas y aplicadas.
Elemento en conflicto reportado: ent-conflicto-1 (aplicado y conviviente, no resuelto).
Notificacion al host: subidos=3, bajados=4, en conflicto=1, estado final=listo.
```

Si llegan varios eventos de recuperación de red en una ventana breve mientras el ciclo está en curso, la app host informa que el motor ignoró los eventos redundantes y mantuvo un único ciclo activo, sin iniciar ciclos paralelos.

## 7. Variaciones sugeridas

| Variación | Qué cambiar | Resultado |
| --- | --- | --- |
| Disparo automático deshabilitado | No habilitar el disparo automático al inicializar | Al recuperar la red, el motor no dispara ningún ciclo y registra el evento como ignorado |
| Sesión no autenticada con red disponible | Quitar el proveedor de credencial de la composición | Al recuperar la red, el motor no dispara el ciclo y notifica al host que se requiere credencial |
| Rebote de conectividad | Emitir varias recuperaciones de red en menos de un segundo | El motor descarta los eventos redundantes y mantiene un único ciclo activo |
| Sustituir el adaptador de transporte | Reemplazar la estrategia de transporte por otra implementación del integrador | El motor opera sin cambios contra el nuevo adaptador, evidenciando la reutilización del paquete |

## 8. Trazabilidad

| Artefacto upstream | Tipo | Cómo lo ilustra este sample |
| --- | --- | --- |
| CU-04 | Caso de uso | Detecta la recuperación de conectividad y dispara automáticamente un único ciclo de sincronización |
| CU-01 | Caso de uso | Inicializa la sesión registrando las cuatro estrategias del integrador |
| CU-03 | Caso de uso | El ciclo disparado respeta el orden subir-luego-bajar y devuelve el resumen al host |
| Extensibilidad (05) | Punto de extensión principal | Implementa las cuatro estrategias del motor en una app host ajena y demuestra el motor reutilizable (extensibilidad §3, §6) |
| RN-01 | Regla de negocio | El ciclo disparado mantiene el orden subir-antes-de-bajar igual que el disparado manualmente |
| RN-03 | Regla de negocio | El motor convive con el estado en conflicto reportado y lo notifica sin resolverlo |
| ADR-01 | Decisión arquitectónica | El sample integra contra la capa Abstractions estable sin tocar el núcleo |
| ADR-02 | Decisión arquitectónica | Inyecta todas las implementaciones concretas desde el host; el motor no instancia adaptadores |
| ADR-08 | Decisión arquitectónica | Materializa la convivencia con estados en conflicto sin bloqueo en la integración real |
| NFR Tiempo de sincronización de lote | Atributo de calidad (arquitectura §8) | El escenario permite medir el tiempo del ciclo subir-luego-bajar de un lote contra el backend de prueba |
| NFR Capacidad de cola local | Atributo de calidad (arquitectura §8) | El adaptador de almacén local del integrador sostiene la cola de pendientes objetivo sin degradación |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del sample avanzado de integración: el motor integrado en una app host ajena al sistema, con las cuatro estrategias del integrador y disparo automático por conectividad. Ilustra CU-04 y el punto de extensión principal de la arquitectura. |
