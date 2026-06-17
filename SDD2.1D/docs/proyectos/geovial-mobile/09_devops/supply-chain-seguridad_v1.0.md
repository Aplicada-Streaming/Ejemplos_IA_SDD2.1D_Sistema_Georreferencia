# Supply chain y seguridad — geovial-mobile

**Proyecto:** geovial-mobile
**Documento:** supply-chain-seguridad_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero DevOps + Mobile Release Engineer

## 1. SBOM

- Formato: CycloneDX (admite SPDX como alternativa equivalente), generado en salida JSON.
- Generador: generador de SBOM del runtime objetivo, ejecutado en el stage SBOM del pipeline (`pipeline-ci-cd_v1.0.md` §3) sobre el grafo de dependencias del paquete de aplicación Android empaquetado.
- Publicación: el SBOM se adjunta al release del canal `production` y se conserva de forma permanente (retención permanente, `pipeline-ci-cd_v1.0.md` §5).
- Firma del propio SBOM: el SBOM se firma junto al paquete en el stage Firma del paquete, de modo que el inventario de dependencias sea verificable y no repudiable.

El SBOM responde a la trazabilidad de dependencias ante una CVE (anti-patrón 09 §4.8 "falta de SBOM" evitado): permite identificar en minutos qué versión distribuida incluye una dependencia vulnerable.

## 2. Firma

- Qué se firma: el paquete de aplicación Android y su SBOM, en el stage Firma del paquete del pipeline.
- Con qué: la credencial de firma resguardada (almacén de claves de firma) que vive en el almacén seguro del pipeline (gestor de secretos), nunca en el repositorio ni en logs (anti-patrón 09 §4.8 "secretos en commit" evitado; 08 `estrategia-testing_v1.0.md` §7).
- Política de la credencial: acceso restringido al solo stage Firma del paquete; rotación planificada y rotación inmediata ante sospecha de compromiso; cada uso queda registrado y ligado al run del pipeline (`entornos-deploy_v1.0.md` §4).
- Verificación: la verificación post-distribución comprueba que la firma del paquete instalado corresponde a la credencial resguardada antes de aceptar la versión (`guia-publicacion-store-mobile_v1.0.md` §3). Un paquete con firma ausente o distinta no se distribuye ni se acepta (anti-patrón 09 §4.8 "falta de firma del artefacto" evitado).

El gate Firma del paquete de 08 §3 bloquea la distribución al canal interno si la firma no es válida.

## 3. SLSA

- Nivel objetivo: SLSA L2 para v1.
- Criterios cumplidos hacia L2: build en un servicio de CI hospedado y versionado (no build local manual para el artefacto distribuido), procedencia generada y adjunta al release (la corrida del pipeline registra el tag, el árbol de fuentes y la versión calculada), y fuente y build separados de la credencial de firma resguardada.
- Plan de elevación a L3: aislar el stage Firma del paquete en un entorno de build endurecido con procedencia no falsificable y parámetros de build no inyectables por el solicitante; se considera al estabilizar el canal `production`.

## 4. Dependency scanning

- Tooling: herramienta de software composition analysis (SCA) del runtime, ejecutada en el stage SCA del pipeline y en el schedule semanal (`pipeline-ci-cd_v1.0.md` §2/§3).
- Frecuencia: en cada push a `main`, en cada tag de distribución y semanalmente por cron para captar CVE publicadas entre releases.
- Actualización de dependencias: bot de actualización de dependencias (tipo Dependabot/Renovate u homólogo) que abre PR de bump; cada PR pasa la suite completa y los gates antes de mergear.
- Política ante vulnerabilidad por severidad:

| Severidad | Acción | Bloqueo |
| --- | --- | --- |
| Crítica | Remediar antes de cualquier distribución; no se distribuye una versión con CVE crítica abierta | Bloquea PR y distribución |
| Alta | Remediar antes de distribuir a `production`; excepción solo registrada y con plan | Bloquea distribución a `production` salvo excepción registrada |
| Media | Planificar remediación en el sprint siguiente | No bloquea |
| Baja | Registrar y atender por oportunidad | No bloquea |

## 5. SAST y DAST

- SAST: el análisis estático del runtime corre en el stage Análisis estático del pipeline (`pipeline-ci-cd_v1.0.md` §3), bloqueante de PR ante issues críticos nuevos (gate Análisis estático de 08 §3). Cubre, entre otros, el manejo de la credencial y del token (que el token nunca se registre en logs ni se persista en texto plano, 05 §7, ADR-05).
- DAST: no aplica como análisis dinámico de un servicio expuesto, porque `geovial-mobile` no expone una superficie de red propia; es una app cliente que consume el contrato REST y la librería de sincronización (05 §5). El análisis dinámico de la superficie del backend corresponde a `geovial-api`. La omisión queda registrada acá por la convención de la sección.

## 6. Política de CVE

- SLA de remediación por severidad: crítica en ≤ 7 días desde la detección; alta en ≤ 30 días; media en el sprint siguiente; baja por oportunidad.
- Comunicación al consumidor: una CVE que afecte una versión distribuida a `production` se comunica a los agentes de campo en las notas de versión del fix y, si es crítica, con aviso directo de actualizar; el rollback por redistribución de la versión previa (`guia-publicacion-store-mobile_v1.0.md` §4) está disponible si el fix no es inmediato.
- Ventana entre detección y publicación del fix: dentro del SLA por severidad; la trazabilidad de qué versión incluye la dependencia vulnerable se resuelve con el SBOM (§1).
- Secretos: la credencial de firma y todo secreto viven en el almacén seguro, nunca en commit; el escaneo de secretos en commits es parte del dependency scanning del schedule (§4).

## 7. Trazabilidad

- El SBOM, la firma y el SCA son los stages SBOM, Firma del paquete y SCA de `pipeline-ci-cd_v1.0.md` §3; el SAST es el stage Análisis estático (gate Análisis estático de 08 §3).
- La credencial de firma resguardada y su resguardo se coordinan con `entornos-deploy_v1.0.md` §4 (secretos) y `guia-publicacion-store-mobile_v1.0.md` §2 (pre-requisitos).
- La verificación de firma post-distribución cierra el lazo con `guia-publicacion-store-mobile_v1.0.md` §3.

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Política de supply chain inicial de geovial-mobile: SBOM CycloneDX JSON firmado y adjunto al release, firma del paquete de aplicación Android y de su SBOM con la credencial de firma resguardada en almacén seguro (nunca en commit), SLSA L2 objetivo con plan a L3, dependency scanning por SCA en push/tag y schedule semanal con política por severidad, SAST por análisis estático (DAST no aplica por ser app cliente sin superficie de red propia) y política de CVE con SLA por severidad y comunicación a los agentes de campo. Derivado de la regla 09 §4.6, de 05 §7 y del intake §17 P.5/P.8. |
