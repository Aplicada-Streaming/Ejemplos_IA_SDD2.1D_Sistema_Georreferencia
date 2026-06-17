# 03 UX / UI / DX — geovial-storage

**Proyecto:** geovial-storage
**Tipo (D8):** library
**Variante:** DX
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** DX Lead

Punto de entrada navegable de la sección 03 de `geovial-storage`, la librería que provee al backend de GeoVial una abstracción de alojamiento de archivos transparente con proveedores intercambiables (local / remoto / otro) seleccionables por el usuario raíz. Por ser un proyecto `library`, la sección se desarrolla en variante DX (reglas 03 §1.2, §2.2): la superficie pública es código, contratos, mensajes de error y documentación, no pantallas. No se producen wireframes (mínimo de wireframes para `library`: 0).

La audiencia es el developer backend integrador de `geovial-api`, con el usuario raíz como lector indirecto de la configuración del proveedor (00 §2).

## Artefactos vigentes

| Artefacto | Variante | Propósito | Estado |
| --- | --- | --- | --- |
| [dx-developer-experience_v1.0.md](dx-developer-experience_v1.0.md) | DX | Marco DX: audiencia, onboarding por tramos 5/30/60, quick-start, plan Diátaxis, mensajes de error, métricas DX, feedback loop y trazabilidad | Propuesto |
| [guia-onboarding-developer_v1.0.md](guia-onboarding-developer_v1.0.md) | DX | Recorrido de la primera hora del integrador: acceso con proveedor local, primer ejemplo guardar-recuperar idéntico, diagnóstico y próximos pasos | Propuesto |
| [dx-error-messages_v1.0.md](dx-error-messages_v1.0.md) | DX | Catálogo de errores de la abstracción de almacenamiento con código, categoría, causa y acción | Propuesto |

Estos tres son los DX docs obligatorios para `library` según las reglas 03 §2.2.

## Superficie pública documentada

Contrato único de seis operaciones, una por caso de uso de 02:

| Operación | CU |
| --- | --- |
| Guardar un archivo | CU-01 |
| Recuperar un archivo | CU-02 |
| Eliminar un archivo | CU-03 |
| Verificar la existencia de un archivo | CU-04 |
| Listar archivos bajo un prefijo | CU-05 |
| Configurar el proveedor de almacenamiento activo | CU-06 |

## Trazabilidad

- Upstream: audiencia de 00 (developer backend integrador, usuario raíz); CU-01 a CU-06 y RN-01, RN-02, RN-03 de 02; necesidad de negocio raíz NB-07 (con NB-03 y NB-06 de soporte).
- Downstream: US-01 a US-09 en 06; tests de quick-start, de contrato por proveedor (RN-01), de igualdad binaria (RN-02) y de no filtración de credenciales (RN-03) en 08. El contrato y los tipos concretos, en 05; los ejemplos ejecutables, en 11.

## Omisiones declaradas

- `dx-portal-developers_v1.0.md`: omitido. La librería no es redistribuible (`redistribuible = false`, intake §17) y no tiene portal público hospedado; su único consumidor es `geovial-api` dentro de la misma solución. Las reglas 03 §2.2 no lo exigen para `library` (solo lo recomiendan para `library con portal hospedado`, que no es el caso). Se deja constancia de la omisión aquí, conforme al encargo.
- Wireframes y `experiencia-de-uso`: no aplican; son artefactos de la variante UX/UI para tipos con UI final.
- `glosario-ux`: no se genera. La sección reutiliza los términos del glosario de dominio de 00 §9 y de la especificación de 02 (identificador lógico, prefijo, proveedor activo, credenciales) sin introducir vocabulario nuevo propio de 03, por lo que no hay términos que absorber (reglas 03 §3.3, anti-patrón de glosario duplicado).

## Glosario de la sección

No aplica un glosario propio. Los términos usados (identificador lógico, prefijo, proveedor activo, credenciales, transparencia, integridad) ya están definidos en 00 §9 y en 02 con la misma semántica; se referencian y no se duplican.

## Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | README inicial de la sección 03 DX de geovial-storage: índice de los tres DX docs obligatorios, superficie pública, trazabilidad y omisión declarada de dx-portal-developers. |
