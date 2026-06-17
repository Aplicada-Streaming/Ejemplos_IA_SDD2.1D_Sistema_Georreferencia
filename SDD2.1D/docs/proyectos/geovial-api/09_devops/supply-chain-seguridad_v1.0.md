# Supply chain y seguridad — geovial-api

**Proyecto:** geovial-api
**Documento:** supply-chain-seguridad_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero DevOps + Platform Engineer

## 1. SBOM

| Aspecto | Decisión |
| --- | --- |
| Formato | CycloneDX (JSON). SPDX admitido como alternativa equivalente si un consumidor o el registro lo exige |
| Generador | Generador de SBOM del runtime objetivo, ejecutado en el stage SBOM del pipeline (`pipeline-ci-cd_v1.0.md` §3) |
| Alcance | Inventario completo de dependencias directas y transitivas de la imagen de contenedor del backend, incluida la librería de almacenamiento embebida (`geovial-storage`, ADR-09) y las capas de la imagen base |
| Salida | Archivo JSON |
| Publicación | Adjunto a cada release y al artefacto de imagen publicado; retención permanente (`pipeline-ci-cd_v1.0.md` §5) |
| Firma del SBOM | El propio SBOM se firma junto con la imagen (§2), de modo que el inventario sea verificable e íntegro |

El SBOM se genera automáticamente en el pipeline (anti-patrón §4.8 "falta de SBOM" evitado) y habilita responder ante un CVE de dependencias sin inventario opaco. Cubre componentes de software de la imagen; no incluye los datos de dominio ni los binarios de fotos, que residen fuera de la imagen (`arquitectura-solucion_v1.0.md` §6).

## 2. Firma

| Aspecto | Decisión |
| --- | --- |
| Herramienta | Firma de artefacto con sigstore/cosign u homólogo del runtime, en el stage Firma (`pipeline-ci-cd_v1.0.md` §3), tras los gates y antes del publish |
| Qué se firma | La imagen de contenedor (por digest) y el SBOM adjunto; la procedencia (provenance) del build (§3) |
| Transparency log | La firma se registra en un transparency log público para verificación independiente |
| Verificación | La firma de la imagen se verifica contra la identidad de firma y el transparency log antes de cada despliegue (`guia-publicacion-image-docker_v1.0.md` §3) |
| Identidad | Identidad de firma de la organización o keyless con OIDC del CI; la clave o identidad vive en el vault del CI (`entornos-deploy_v1.0.md` §4) |

La firma se ejecuta en el stage final antes del publish y se verifica antes de habilitar tráfico de cualquier despliegue (anti-patrón §4.8 "falta de firma del artefacto" evitado). Sin firma válida y registrada, el stage Publish imagen no se ejecuta (gate bloqueante) y ningún ambiente despliega la imagen.

## 3. SLSA

Nivel objetivo: SLSA L3 para los artefactos que se promueven a PROD, con L2 como piso inmediato.

| Requisito SLSA | Estado v1 | Cómo se cumple / plan de elevación |
| --- | --- | --- |
| Fuente versionada y trazable (L1+) | Cumplido | Repositorio Git con `main` protegida; trunk-based (`estrategia-versionado_v1.0.md` §4) |
| Build con servicio de build gestionado (L2) | Cumplido | El build y la publicación de releases ocurren solo en el pipeline de CI, no en máquinas de desarrollo |
| Procedencia generada y autenticada (L2) | Cumplido | Procedencia emitida por el pipeline y firmada (§2) |
| Build aislado y parametrizado, procedencia no falsificable (L3) | Objetivo | Runners efímeros por corrida, build sin pasos manuales y procedencia firmada con identidad del CI; plan de elevación: fijar el build a runner aislado y verificar la procedencia antes del deploy a STAGING/PROD |
| Build hermético y reproducible bit a bit (L4) | Fuera de alcance v1 | No es objetivo de v1; se evalúa en una versión futura con un ADR |

La procedencia (quién construyó, desde qué fuente, con qué parámetros) se adjunta al release junto con el SBOM y la firma. El plan de elevación a L3 se cierra antes del primer despliegue a PROD; L4 queda fuera de alcance para `equipo_n=1` y artefacto único.

## 4. Dependency scanning

| Aspecto | Decisión |
| --- | --- |
| Tooling SCA | Herramienta de software composition analysis del runtime, en el stage SCA (`pipeline-ci-cd_v1.0.md` §3); cubre dependencias del backend y de la imagen base |
| Frecuencia | En cada PR y push; además, schedule semanal de mantenimiento que re-escanea las dependencias y la imagen publicada (`pipeline-ci-cd_v1.0.md` §2) |
| Actualización de dependencias | Bot de actualización (Dependabot, Renovate o equivalente) que abre PR ante nuevas versiones y avisos de seguridad; cada PR pasa los gates G1-G6 |
| Scan de secretos | Scan automático de commits para detectar secretos expuestos; ningún secreto se commitea (`entornos-deploy_v1.0.md` §4; anti-patrón §4.8) |

Política ante vulnerabilidad por severidad (consecuencia en el pipeline):

| Severidad | Consecuencia en CI | Acción |
| --- | --- | --- |
| Crítica | Bloquea el merge y el despliegue | Remediar antes de continuar; sin excepción |
| Alta | Bloquea, salvo excepción registrada con justificación y plazo | Remediar o registrar excepción con fecha límite |
| Media | Advierte; no bloquea | Programar remediación en el tramo siguiente |
| Baja | Advierte; no bloquea | Atender en mantenimiento |

El gate de SCA del pipeline exige 0 CVE críticas y 0 altas sin excepción registrada (`pipeline-ci-cd_v1.0.md` §3).

## 5. SAST y DAST

| Análisis | Aplica | Tooling y stage | Criterio de bloqueo |
| --- | --- | --- | --- |
| SAST (análisis estático de seguridad) | Sí | Analizador estático del runtime en el stage Análisis estático (gate G6, `pipeline-ci-cd_v1.0.md` §3); incluye reglas de seguridad además de las de calidad | Sin issues críticos nuevos; bloquea el merge (G6) |
| DAST (análisis dinámico) | Sí | Recorre la superficie REST en ejecución de un ambiente desplegado (DEV o QA) sobre los 35 endpoints; ejecutado en el schedule de mantenimiento y antes de promover a STAGING (`pipeline-ci-cd_v1.0.md` §2) | Hallazgo crítico bloquea la promoción a STAGING |

A diferencia de una librería sin endpoint propio, `geovial-api` expone una superficie REST autenticada por token bearer (`arquitectura-solucion_v1.0.md` §5/§7): el DAST sí aplica y recorre dinámicamente los endpoints en un ambiente desplegado, verificando entre otros el control de acceso jerárquico (CU-18, RN-01), el manejo uniforme de errores problem+json (CU-19) sin filtración de detalles internos, y que las credenciales del proveedor de almacenamiento ni la clave de firma del token se filtren por ninguna respuesta (ADR-09; RN-03 de storage; 05 §9). El DAST se ejecuta contra DEV o QA, nunca contra PROD con datos reales.

## 6. Política de CVE

| Severidad | SLA de remediación | Comunicación al consumidor |
| --- | --- | --- |
| Crítica | Fix desplegado lo antes posible tras la detección; despliegue rollback por desvío de tráfico si la versión en PROD es vulnerable | Aviso inmediato a `geovial-web` y `geovial-mobile` y entrada en CHANGELOG |
| Alta | Fix en la ventana acordada (objetivo: días, no semanas) | Aviso en el release notes de la versión corregida |
| Media | Fix en el tramo de release siguiente | Mención en el CHANGELOG |
| Baja | Atendida en mantenimiento | Mención en el CHANGELOG |

Ventana entre detección y publicación de fix: para crítica y alta, si la versión vulnerable está en PROD se ejecuta el rollback por desvío de tráfico al digest previo seguro (`guia-publicacion-image-docker_v1.0.md` §4) y el fix se despliega en cuanto pasa los gates; la comunicación a los clientes acompaña la publicación del fix. Cualquier defecto de integridad de datos o de control de acceso detectado en una versión desplegada dispara, además, un TC de regresión (08 estrategia-calidad §5; criterios-validacion §4). La comunicación sigue la deprecation/communication policy de `estrategia-versionado_v1.0.md` §6 y respeta la convivencia de versiones del contrato (ADR-10).

## 7. Secretos del backend

Los secretos del backend viven en el vault del entorno, nunca en commit ni en la imagen (intake §17 P.5; 05 §7/§9). El detalle por ambiente y la rotación viven en `entornos-deploy_v1.0.md` §4; acá se consolida su perfil de cadena de suministro:

| Secreto | ADR pertinente | Riesgo si se filtra |
| --- | --- | --- |
| Clave de firma del token bearer | ADR-03 (autenticación por token bearer y rol jerárquico) | Suplantación de sesión y escalada de privilegios (05 §9, riesgo de filtración) |
| Credenciales del proveedor de almacenamiento de fotos | ADR-09 (integración de la abstracción de almacenamiento) | Acceso indebido a la evidencia de campo; el backend nunca las expone en una respuesta ni error (RN-03 de storage) |
| Cadena de conexión al almacén relacional | ADR-02 (persistencia en almacén relacional con migraciones) | Acceso indebido a los datos de dominio |
| Credencial de publicación de imagen y de contrato; identidad de firma | — | Publicación o firma no autorizada de artefactos |

El scan de secretos en commits y el bot de actualización (§4) son la primera línea; el vault del entorno y del CI con scope mínimo y rotación es la segunda (`entornos-deploy_v1.0.md` §4).

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Stages SBOM, Firma, SCA, Análisis estático | `pipeline-ci-cd_v1.0.md` §3 |
| Gate de análisis estático (SAST) | G6 (08 estrategia-calidad §3) |
| DAST sobre la superficie REST | `pipeline-ci-cd_v1.0.md` §2; 05 §5/§7; CU-18, CU-19 |
| Secretos y scan de commits | `entornos-deploy_v1.0.md` §4; ADR-03, ADR-09, ADR-02 |
| Rollback por CVE | `guia-publicacion-image-docker_v1.0.md` §4; `pipeline-ci-cd_v1.0.md` §7 |
| Comunicación al consumidor | `estrategia-versionado_v1.0.md` §6; ADR-10 |

## 9. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Política inicial de cadena de suministro de geovial-api: SBOM CycloneDX JSON generado y firmado en el pipeline y adjunto a la imagen, incluida la librería de almacenamiento embebida; firma de la imagen, del SBOM y de la procedencia con transparency log y verificación antes de cada despliegue; SLSA L3 objetivo para los artefactos a PROD con L2 de piso y L4 fuera de alcance v1; dependency scanning SCA en PR/push y schedule con bot de actualización y scan de secretos, y política por severidad alineada al gate de SCA; SAST por el gate G6 y DAST aplicable que recorre los 35 endpoints en un ambiente desplegado verificando control de acceso, errores sin filtración y no filtración de secretos; política de CVE con SLA por severidad, rollback por desvío de tráfico ante vulnerable en PROD y comunicación a los clientes; consolidación de los secretos del backend (clave de firma del token ADR-03, credenciales del proveedor de almacenamiento ADR-09, cadena de conexión ADR-02) en vault. Derivada de 05 §5/§7/§9, de los gates de 08 §3, de ADR-02/ADR-03/ADR-09/ADR-10 y del intake §17 P.5/P.8. |
