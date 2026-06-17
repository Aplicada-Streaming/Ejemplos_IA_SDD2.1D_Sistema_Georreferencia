# Supply chain y seguridad — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** supply-chain-seguridad_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero DevOps Senior (AG-09), variante DevOps + Release Engineer (library)

## 1. SBOM

| Aspecto | Decisión |
| --- | --- |
| Formato | CycloneDX (JSON). SPDX admitido como alternativa equivalente si el feed o un consumidor lo exige |
| Generador | Generador de SBOM del runtime objetivo, ejecutado en el stage SBOM del pipeline (`pipeline-ci-cd_v1.0.md` §3) |
| Alcance | Inventario completo de dependencias directas y transitivas del paquete `Aplicada.Sync` para el target `net8.0-android` |
| Salida | Archivo JSON |
| Publicación | Adjunto a cada release del canal `stable` (y disponible para `preview`), retención permanente (`pipeline-ci-cd_v1.0.md` §5) |
| Firma del SBOM | El propio SBOM se firma junto con el paquete (§2), de modo que el inventario sea verificable e íntegro |

El SBOM se genera automáticamente en el pipeline (anti-patrón 4.8 "falta de SBOM" evitado) y habilita responder ante un CVE de dependencias sin inventario opaco. Como la librería es agnóstica del dominio y su carga útil es opaca (`arquitectura-solucion_v1.0.md` §6), el SBOM cubre únicamente componentes de software, no datos.

## 2. Firma

| Aspecto | Decisión |
| --- | --- |
| Herramienta | Firma de artefacto con sigstore/cosign u homólogo del runtime, en el stage Firma (`pipeline-ci-cd_v1.0.md` §3), tras los gates y antes del publish |
| Qué se firma | El paquete `.nupkg` y el SBOM adjunto |
| Transparency log | La firma se registra en un transparency log público para verificación independiente |
| Verificación por consumidores | El consumidor verifica la firma contra la identidad de firma y el transparency log; el checksum del artefacto se compara al restaurar (`guia-publicacion-paquete-nuget_v1.0.md` §3) |
| Identidad | Identidad de firma de la organización o keyless con OIDC del CI; la clave o identidad vive en el secret manager del CI (`entornos-deploy_v1.0.md` §5) |

La firma se ejecuta en el stage final antes del publish y se verifica en la verificación post-publish (anti-patrones 4.8 "falta de firma del artefacto" evitado). Sin firma válida y registrada, el stage Publish no se ejecuta (gate bloqueante).

## 3. SLSA

Nivel objetivo: SLSA L3 para el canal `stable`, con L2 como piso inmediato.

| Requisito SLSA | Estado v1 | Cómo se cumple / plan de elevación |
| --- | --- | --- |
| Fuente versionada y trazable (L1+) | Cumplido | Repositorio Git en GitHub; trunk-based con main protegida (`estrategia-versionado_v1.0.md` §4) |
| Build con servicio de build gestionado (L2) | Cumplido | El build y la publicación ocurren solo en el pipeline de CI, no en máquinas de desarrollo, para releases |
| Procedencia generada y autenticada (L2) | Cumplido | Procedencia (provenance) emitida por el pipeline y firmada (§2) |
| Build aislado y parametrizado, procedencia no falsificable (L3) | Objetivo | Runners efímeros por corrida, build sin pasos manuales y procedencia firmada con identidad del CI; plan de elevación: fijar el build a runner aislado y verificar la procedencia en la verificación post-publish |
| Build hermético y reproducible bit a bit (L4) | Fuera de alcance v1 | No es objetivo de v1; se evalúa en una versión futura con un ADR |

La procedencia (quién construyó, desde qué fuente, con qué parámetros) se adjunta al release junto con el SBOM y la firma. El plan de elevación a L3 se cierra antes del primer `stable`; L4 queda fuera de alcance para `equipo_n=1` y target único.

## 4. Dependency scanning

| Aspecto | Decisión |
| --- | --- |
| Tooling SCA | Herramienta de software composition analysis del runtime, en el stage SCA (`pipeline-ci-cd_v1.0.md` §3) |
| Frecuencia | En cada PR y push; además, schedule semanal de mantenimiento que re-escanea las dependencias publicadas (`pipeline-ci-cd_v1.0.md` §2) |
| Actualización de dependencias | Bot de actualización (Dependabot, Renovate o equivalente) que abre PR ante nuevas versiones y avisos de seguridad; cada PR pasa los gates G1-G9 |
| Scan de secretos | Scan automático de commits para detectar secretos expuestos; ningún secreto se commitea (`entornos-deploy_v1.0.md` §5; anti-patrón 4.8) |

Política ante vulnerabilidad por severidad (consecuencia en el pipeline):

| Severidad | Consecuencia en CI | Acción |
| --- | --- | --- |
| Crítica | Bloquea el merge y la publicación | Remediar antes de continuar; sin excepción |
| Alta | Bloquea, salvo excepción registrada con justificación y plazo | Remediar o registrar excepción con fecha límite |
| Media | Advierte; no bloquea | Programar remediación en el tramo siguiente |
| Baja | Advierte; no bloquea | Atender en mantenimiento |

El gate de SCA del pipeline exige 0 CVE críticas y 0 altas sin excepción registrada (`pipeline-ci-cd_v1.0.md` §3).

## 5. SAST y DAST

| Análisis | Aplica | Tooling y stage | Criterio de bloqueo |
| --- | --- | --- | --- |
| SAST (análisis estático de seguridad) | Sí | Analizador estático del runtime en el stage Análisis estático (gate G9, `pipeline-ci-cd_v1.0.md` §3); incluye reglas de seguridad además de las de calidad | Sin issues críticos; bloquea el merge (G9) |
| DAST (análisis dinámico) | No aplica | — | La librería no expone una superficie de red ni un endpoint en ejecución que un DAST pueda recorrer: corre embebida en el host y su transporte es una abstracción inyectada (`arquitectura-solucion_v1.0.md` §5/§7). El DAST se traslada al consumidor que despliega un servicio (por ejemplo `geovial-api`), no a esta librería |

La no aplicabilidad del DAST se declara explícitamente para no dejar el gate como omisión silenciosa: el motor no tiene endpoint propio que recorrer dinámicamente. La cobertura dinámica del comportamiento del motor se hace por las pruebas de NFR y de ciclo completo (gate G7 y la suite de integración de 08), no por un DAST de red.

## 6. Política de CVE

| Severidad | SLA de remediación | Comunicación al consumidor |
| --- | --- | --- |
| Crítica | Fix publicado lo antes posible tras la detección; versión vulnerable unlisted de inmediato | Aviso inmediato en release notes y al canal de consumidores; entrada en CHANGELOG |
| Alta | Fix en la ventana acordada (objetivo: días, no semanas) | Aviso en el release notes de la versión corregida |
| Media | Fix en el tramo de release siguiente | Mención en el CHANGELOG |
| Baja | Atendida en mantenimiento | Mención en el CHANGELOG |

Ventana entre detección y publicación de fix: para crítica y alta, el paquete vulnerable se unlista de inmediato (`guia-publicacion-paquete-nuget_v1.0.md` §4) y el fix se publica en cuanto pasa los gates; la comunicación al consumidor acompaña la publicación del fix. Cualquier defecto de integridad de datos detectado en una versión publicada dispara, además, un TC de regresión (08 estrategia-calidad §5; 08_rules §5.4). La comunicación sigue la deprecation/communication policy de `estrategia-versionado_v1.0.md` §6.

## 7. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Stages SBOM, Firma, SCA, Análisis estático | `pipeline-ci-cd_v1.0.md` §3 |
| Gate de análisis estático | G9 (08 estrategia-calidad §3) |
| Secretos y scan de commits | `entornos-deploy_v1.0.md` §5 |
| Rollback por CVE | `guia-publicacion-paquete-nuget_v1.0.md` §4; `pipeline-ci-cd_v1.0.md` §7 |
| Por qué no aplica DAST | 05 §5/§7 (sin endpoint propio; transporte inyectado) |
| Comunicación al consumidor | `estrategia-versionado_v1.0.md` §6 |

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Política inicial de cadena de suministro de aplicada-sync: SBOM CycloneDX JSON generado y firmado en el pipeline y adjunto al release; firma del paquete y del SBOM con transparency log antes del publish; SLSA L3 objetivo para stable con L2 de piso y L4 fuera de alcance v1; dependency scanning SCA en PR/push y schedule semanal con bot de actualización y scan de secretos, y política por severidad alineada al gate de SCA; SAST por el gate G9 y DAST declarado no aplicable con justificación (la librería no expone endpoint propio); política de CVE con SLA por severidad, unlist inmediato de crítica/alta y comunicación al consumidor. Derivada de 05 §5/§7, de los gates de 08 §3 y del intake §17 P.8. |
