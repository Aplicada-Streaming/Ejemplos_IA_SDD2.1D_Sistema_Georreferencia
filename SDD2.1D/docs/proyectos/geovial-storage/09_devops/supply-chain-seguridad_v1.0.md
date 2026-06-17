# Supply chain y seguridad — geovial-storage

**Proyecto:** geovial-storage
**Documento:** supply-chain-seguridad_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero DevOps Senior (variante DevOps + Release Engineer, library)

## 0. Alcance

`geovial-storage` es una `library` que no se publica externamente: se integra al backend `geovial-api` y se distribuye dentro de su artefacto (intake §13, §17.P.7; ADR-03). Por eso la cadena de suministro de la librería se materializa en dos planos: el inventario y la verificación de sus propias dependencias (este documento), y la firma y el SBOM del artefacto final, que se producen sobre la imagen del backend con la librería ya embebida. Atención específica a las credenciales de proveedores: ADR-05 prohíbe exponerlas y delega su almacenamiento físico a esta categoría; nunca van en commit, viven en vault (ver `entornos-deploy_v1.0.md` §4).

## 1. SBOM

| Aspecto | Decisión |
| --- | --- |
| Formato | Inventario de software en formato estándar interoperable (CycloneDX o SPDX, salida JSON), el mismo que adopte la solución |
| Generador | Generador de SBOM del runtime objetivo, ejecutado en STAGE-10 del pipeline (`pipeline-ci-cd_v1.0.md` §1) |
| Contenido | Todas las dependencias directas y transitivas de la librería con su versión y licencia |
| Propagación | El SBOM de la librería se incorpora al SBOM del artefacto del backend; el SBOM consolidado se adjunta al release del backend |
| Firma del SBOM | El SBOM consolidado del backend se firma junto con la imagen (ver §2) |

La librería no publica un SBOM propio a un feed (no tiene feed); su inventario alimenta el SBOM del backend para que ante una CVE de una dependencia transitiva se pueda responder con el inventario completo del artefacto desplegado.

## 2. Firma

| Aspecto | Decisión |
| --- | --- |
| Qué se firma | La imagen del backend que integra la librería (la librería no produce un artefacto firmable independiente) |
| Herramienta | Firmador de artefactos con registro de transparencia (sigstore/cosign u homólogo), ejecutado en STAGE-12 del pipeline |
| Registro de transparencia | La firma se registra en un transparency log verificable |
| Verificación por el consumidor | El despliegue del backend verifica la firma de la imagen antes de promover (gate de la promoción del backend) |
| Política | Ninguna imagen del backend sin firma válida y registrada se promueve a un ambiente (regla 09 §4.8, "falta de firma del artefacto") |

## 3. SLSA

| Aspecto | Decisión |
| --- | --- |
| Nivel objetivo | SLSA L2 para el artefacto del backend que integra la librería |
| Criterios cumplidos en L2 | Build en servicio de CI hospedado (no en máquina del desarrollador), procedencia (provenance) generada y firmada del artefacto, fuente versionada con historial |
| Plan de elevación a L3 | Build con aislamiento reforzado y procedencia no falsificable; se evalúa cuando el backend lo adopte, dado que la librería hereda el nivel del artefacto del backend |
| Contribución de la librería | El build de la librería ocurre en el mismo servicio de CI hospedado y su procedencia queda incluida en la del backend |

El nivel SLSA es del artefacto final (la imagen del backend); la librería no tiene un nivel SLSA independiente porque no se publica por separado.

## 4. Dependency scanning (SCA)

| Aspecto | Decisión |
| --- | --- |
| Tooling | Analizador de composición de software del runtime objetivo, ejecutado en STAGE-09 del pipeline |
| Frecuencia | En cada PR y en cada build de `main`; además, escaneo programado periódico sobre `main` para detectar CVE nuevas en dependencias sin cambios |
| Automatización de actualizaciones | Bot de actualización de dependencias (Dependabot, Renovate u homólogo) que abre PR ante versiones con parche de seguridad |
| Política por severidad | Ver §6 (política de CVE) |

La política ante vulnerabilidad de dependencia: crítica y alta bloquean el merge (gate de SCA, STAGE-09) salvo excepción documentada con ADR y plan de remediación (alineado con DoD-release y `criterios-validacion_v1.0.md` §6); media y baja generan ticket sin bloquear.

## 5. SAST y DAST

| Análisis | Aplicabilidad | Stage / criterio de bloqueo |
| --- | --- | --- |
| SAST (análisis estático) | Aplica. La librería tiene código propio que manipula archivos, rutas y credenciales | STAGE-01 (lint) y el analizador estático del gate G-09 (sin issues críticos) bloquean el merge; el property-based test de no filtración de credenciales (G-07, STAGE-07) refuerza el análisis de la superficie sensible |
| Escaneo de secretos en commits | Aplica. ADR-05 y RN-03 prohíben exponer credenciales | Escaneo de secretos sobre el historial y los PR; un secreto detectado bloquea el merge y dispara rotación (anti-patrón "secretos en commit") |
| DAST (análisis dinámico) | No aplica directamente a la librería | La librería no expone una interfaz de red ni un endpoint propio (05 §3): no tiene superficie dinámica que escanear. El DAST se ejecuta sobre el backend desplegado (`geovial-api`), donde la librería queda ejercida indirectamente a través de los endpoints del backend |

La no aplicabilidad de DAST a la librería se registra explícitamente: la librería corre dentro del proceso del backend sin contrato de red propio (05 §3, §4), por lo que su análisis dinámico es responsabilidad del pipeline del backend.

## 6. Política de CVE

SLA de remediación por severidad, desde la detección hasta la disponibilidad del fix en el artefacto del backend que integra la librería:

| Severidad | SLA de remediación | Consecuencia en el pipeline | Comunicación |
| --- | --- | --- | --- |
| Crítica | 48 horas | Bloquea merge y release (STAGE-09); detiene la promoción del backend | Notificación inmediata al equipo y al consumidor `geovial-api` |
| Alta | 7 días | Bloquea merge salvo excepción documentada con ADR y plan de remediación | Notificación al equipo; registro en el seguimiento |
| Media | 30 días | No bloquea; ticket con BT en el backlog de 06 | Registro en el seguimiento |
| Baja | 90 días o próxima ventana de mantenimiento | No bloquea; ticket | Registro en el seguimiento |

- La comunicación al consumidor `geovial-api` es directa porque es el único consumidor (no hay consumidores externos): un fix de la librería se entrega reconstruyendo y re-desplegando la imagen del backend (`pipeline-ci-cd_v1.0.md` §5, rollback/fix).
- La ventana entre detección y publicación del fix se acota por el SLA por severidad; el fix sigue SemVer (PATCH si es retrocompatible; MAJOR + coordinación si toca el contrato, ADR-03).
- Toda excepción a un bloqueo requiere ADR explícita y plan de remediación con BT en 06 (regla 08 §4.7; `criterios-validacion_v1.0.md` §6).

## 7. Credenciales de proveedores (ADR-05)

Tratamiento específico exigido por el insumo, derivado de ADR-05 y RN-03:

- Las credenciales del proveedor de objetos remoto no van nunca en commit; viven en el vault del ambiente del backend (`entornos-deploy_v1.0.md` §4).
- La librería las custodia en su resguardo de credenciales: entran por CU-06 y no salen por resultado, error ni registro (RN-03; NFR-05).
- La no filtración se verifica de forma mecánica en el pipeline: gate G-07 (STAGE-07) con property-based test y analizador estático, y prueba de no filtración TC-24.
- El escaneo de secretos en commits (§5) cierra el frente del repositorio.
- El mecanismo de cifrado en reposo del secreto en el vault lo fija esta categoría a partir del intake §17.P.5, delegado por ADR-05 §7; hasta fijarlo, la garantía es de no filtración por la superficie pública (GAP-04 de `criterios-validacion_v1.0.md` §6).

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| SBOM y firma del artefacto | STAGE-10, STAGE-12 de `pipeline-ci-cd_v1.0.md` |
| SCA y SAST | STAGE-09, STAGE-01, G-07, G-09 |
| Credenciales | ADR-05; RN-03; NFR-05; intake §17.P.5; `entornos-deploy_v1.0.md` §4 |
| No filtración | G-07; TC-24; `criterios-validacion_v1.0.md` CV-N5 |
| Excepciones y SLA | DoD-release de 08; `criterios-validacion_v1.0.md` §6 |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Supply chain y seguridad inicial de geovial-storage: SBOM de la librería propagado al SBOM del backend; firma de la imagen del backend que la integra con registro de transparencia; SLSA L2 objetivo heredado del artefacto del backend; dependency scanning con SLA por severidad; SAST y escaneo de secretos aplicables, DAST no aplicable a la librería (registrado); política de CVE con SLA crítica 48 h / alta 7 d / media 30 d / baja 90 d; y tratamiento de credenciales de proveedores según ADR-05 (vault, no commit, no filtración verificada por G-07). |
