# Developer guide — geovial-storage

**Proyecto:** geovial-storage
**Documento:** README.md
**Versión:** 1.0
**Estado:** Borrador
**Fecha:** 2026-06-15
**Autor:** Technical Writer + SDK Documentation Lead
**Tipo Diátaxis:** Índice (no aplica a un cuadrante)
**Audiencia:** Developer integrador del backend que consume la abstracción de almacenamiento
**Nivel:** Básico
**Tiempo estimado de lectura:** 4 min

Esta carpeta es la ventana que `geovial-storage` le abre al developer que integra su abstracción de almacenamiento en un servicio backend. Un developer nuevo debería poder hacer funcionar la librería siguiendo solo esta carpeta. Es un proyecto tipo `library`: la categoría 10 es obligatoria.

## 1. Artefactos vigentes

| Documento | Tipo Diátaxis | Complejidad | Para qué sirve |
| --- | --- | --- | --- |
| `conceptos-fundamentales_v1.0.md` | Explanation | Básico | Modelo mental de la abstracción: operaciones, proveedor activo, transparencia, integridad, qué NO hace. |
| `guia-onboarding-developer_v1.0.md` | Tutorial | Básico | De cero al cambio de proveedor sin tocar el código, en menos de una hora. |
| `guia-integracion-servicio-backend_v1.0.md` | How-to | Medio | Integrar la abstracción en un servicio backend y seleccionar el proveedor activo. |
| `referencia-api_v1.0.md` | Reference | Medio | Las seis operaciones con parámetros, retorno y excepciones; paridad con 05. |
| `troubleshooting_v1.0.md` | How-to (diagnóstico) | Medio | Seis ISSUE-XX con diagnóstico paso a paso y plantilla de reporte. |
| `glosario-tecnico_v1.0.md` | Reference | Básico | Vocabulario canónico del consumidor con referencia cruzada por término. |

## 2. Orden de lectura recomendado

1. `conceptos-fundamentales_v1.0.md` — entender qué es y por qué es transparente al proveedor.
2. `guia-onboarding-developer_v1.0.md` — primer resultado exitoso y primera integración.
3. `guia-integracion-servicio-backend_v1.0.md` — integrar en un servicio propio y cambiar de proveedor.
4. `referencia-api_v1.0.md` — el dato exacto de cada operación, parámetro o código de error.
5. `troubleshooting_v1.0.md` — cuando algo falla.
6. `glosario-tecnico_v1.0.md` — consulta terminológica transversal.

## 3. Prerequisitos para empezar

- La librería disponible para el backend (incorporación según 05/11).
- Capacidad de editar la configuración del backend y ejecutar sus pruebas.
- Una ubicación local accesible y escribible para el proveedor local.
- Credenciales de un proveedor remoto solo si se va a cambiar el proveedor activo (provistas por el usuario raíz).

## 4. Quick-start

```
1. configurar(proveedor="local", parametros-y-credenciales={ ubicacion: "<carpeta-local>" })  -> proveedor local activo
2. guardar(contenido="hola-geovial", destino="pruebas/quick-start/", tipo="text/plain")        -> identificador no vacío + tamaño
3. verificar(identificador=<el del paso 2>)                                                    -> presencia=verdadero
4. recuperar(identificador=<el del paso 2>)                                                    -> contenido idéntico al guardado
# Si el contenido del paso 4 es idéntico al del paso 2, lograste el primer resultado exitoso.
# Recorrido completo (incluido el cambio de proveedor sin tocar el código): guia-onboarding-developer_v1.0.md
```

## 5. Referencias cruzadas

- 05 `contratos-abstractions_v1.0.md`: contrato fuente del que `referencia-api_v1.0.md` mantiene paridad.
- 05 `adrs/`: ADR-01 a ADR-05 que gobiernan las decisiones citadas en los conceptos.
- 02 `casos-de-uso/`: CU-01 a CU-06, origen funcional de las operaciones.
- 03 `dx-developer-experience_v1.0.md` y `dx-error-messages_v1.0.md`: marco DX y catálogo de errores upstream.
- 11 `samples/geovial-storage/`: ejemplos ejecutables que materializan estos documentos.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Índice inicial de la categoría 10: artefactos vigentes con tipo Diátaxis y complejidad, orden de lectura, prerequisitos, quick-start de proveedor local y referencias cruzadas. |
