# Casos de prueba referenciales — geovial-mobile

Proyecto: geovial-mobile
Documento: casos-prueba-referenciales_v1.0.md
Versión: 1.0
Estado: Propuesto
Fecha: 2026-06-15
Autor: Ingeniero QA / SDET (mobile)

## 1. Propósito y convenciones

Catálogo de casos de prueba referenciales (TC-XX) de `geovial-mobile`. Cada TC referencia al menos un CU, una RN o un NFR (regla 08 §4.10, antipatrón de tests sin trazabilidad). Hay al menos un TC por cada uno de los 7 CU y por cada una de las 5 RN. Se incluyen TC del modo offline (cola, reanudación tras corte, convivencia con conflicto), TC de captura georreferenciada y TC de los NFR numéricos (cola ≥ 1000, ciclo de 100 cambios ≤ 30 s, arranque ≤ 3 s).

El estado de todos los TC es Pendiente porque la app aún no se implementó (todos los tramos del mini-plan de 07 están abiertos); el campo Actual queda como pendiente y se completa al ejecutar la suite, actualizando la matriz al cierre de cada tramo. Los pasos conservan la forma Given/When/Then para preservar el vínculo con el CU de origen. Los identificadores son contiguos de dos dígitos. Los tipos posibles son: unitario, integración, interfaz móvil (e2e), snapshot, sincronización (modo offline/sincronización) y ciclo de vida.

## 2. Catálogo de casos de prueba

### TC-01-inicio-online-token-seguro
- Tipo: integración
- Cubre: CU-01 (CA-01), US-01, RN-04, ADR-05
- Setup: app recién instalada, con conexión, doble del backend que emite token bearer válido, agente con usuario habilitado, doble del almacén seguro del dispositivo vacío.
- Pasos: Given app recién instalada con conexión y agente habilitado; When ingresa credenciales válidas y confirma; Then obtiene el token bearer, lo guarda en el almacén seguro (nunca en texto plano) y habilita el trabajo de campo.
- Expected: sesión abierta; el token queda en el doble del almacén seguro y no aparece en texto plano en ningún log ni almacén; el trabajo de campo queda habilitado.
- Actual: pendiente.
- Status: Pendiente.

### TC-02-relogueo-seguridad-dispositivo
- Tipo: ciclo de vida
- Cubre: CU-01 (CA-02), US-02, RN-04, ADR-05
- Setup: sesión activa guardada con token vigente; doble de verificación por seguridad del dispositivo configurado para aceptar; app reiniciada.
- Pasos: Given sesión activa guardada y app reiniciada; When abre la app y se verifica por seguridad del dispositivo (huella o patrón); Then rehabilita el acceso reutilizando el token, sin pedir credenciales.
- Expected: acceso rehabilitado reutilizando el token; no se solicitan credenciales; no hay nueva llamada de inicio online.
- Actual: pendiente.
- Status: Pendiente.

### TC-03-deslogueo-completo-libera-dispositivo
- Tipo: integración
- Cubre: CU-01 (CA-03, CA-05), US-02, RN-04
- Setup: dispositivo con sesión activa del agente A; token y datos de sesión presentes en el doble del almacén seguro.
- Pasos: Given dispositivo con sesión activa del agente A; When el agente A ejecuta deslogueo completo y luego el agente B intenta reloguearse por seguridad del dispositivo; Then borra token y datos de sesión, muestra el inicio de sesión sin datos del agente A y no da acceso sobre sesión ajena, exigiendo deslogueo completo y nuevo inicio online.
- Expected: el almacén seguro queda sin token ni datos del agente A; la pantalla de inicio no muestra datos del agente A; el agente B no accede a la sesión del agente A y debe iniciar online.
- Actual: pendiente.
- Status: Pendiente.

### TC-04-inicio-sin-conexion-rechazado
- Tipo: integración
- Cubre: CU-01 (CA-04), US-01, RN-05
- Setup: app recién instalada sin conexión (doble de conectividad en estado sin conexión); sin sesión previa.
- Pasos: Given app recién instalada sin conexión; When intenta el inicio inicial; Then responde con SIN_CONEXION_INICIO y no crea sesión.
- Expected: error SIN_CONEXION_INICIO; no se crea sesión; no se escribe token en el almacén seguro.
- Actual: pendiente.
- Status: Pendiente.

### TC-05-seleccionar-relevamiento-contexto-activo
- Tipo: interfaz móvil (e2e)
- Cubre: CU-02 (CA-01), US-03, RN-05
- Setup: agente con sesión activa y 3 relevamientos asignados en copia local (almacén local sembrado); sin dependencia de red.
- Pasos: Given agente con sesión activa y 3 relevamientos asignados en copia local; When abre la lista y selecciona el segundo; Then fija ese relevamiento como contexto activo y abre su mapa con los marcadores locales.
- Expected: el segundo relevamiento queda como contexto activo; el mapa abre con los marcadores locales; la operación se resuelve contra el almacén local sin llamada de red.
- Actual: pendiente.
- Status: Pendiente.

### TC-06-lista-sin-relevamientos-locales
- Tipo: integración
- Cubre: CU-02 (CA-02), US-03, RN-05
- Setup: agente sin relevamientos sincronizados y sin conexión.
- Pasos: Given agente sin relevamientos sincronizados y sin conexión; When abre la lista; Then responde con SIN_RELEVAMIENTOS_LOCALES y no fija contexto activo.
- Expected: error SIN_RELEVAMIENTOS_LOCALES; no se fija contexto activo; sin llamada de red.
- Actual: pendiente.
- Status: Pendiente.

### TC-07-relevamiento-cerrado-modo-lectura
- Tipo: integración
- Cubre: CU-02 (CA-04), US-03, RN-03
- Setup: relevamiento cerrado por el jefe presente en copia local (estado cierre).
- Pasos: Given relevamiento cerrado presente en copia local; When lo selecciona; Then lo abre en modo lectura y no habilita nuevas capturas.
- Expected: el relevamiento abre en modo lectura; las acciones de captura quedan inhabilitadas; un intento de captura responde RELEVAMIENTO_CERRADO.
- Actual: pendiente.
- Status: Pendiente.

### TC-08-centrar-gps-crear-marcador-offline
- Tipo: sincronización (modo offline)
- Cubre: CU-03 (CA-01), US-05, RN-05, NFR captura offline
- Setup: agente con relevamiento en recolección, permiso de ubicación concedido, doble de ubicación con señal de GPS, doble de conectividad sin conexión.
- Pasos: Given relevamiento en recolección y permiso de ubicación concedido, sin conexión; When centra por GPS y crea un marcador en la posición; Then crea el marcador en el almacén local con identidad propia y lo encola como cambio pendiente.
- Expected: marcador creado localmente con identidad estable; un cambio encolado con identificador de origen estable y orden de creación; toda la operación sin conexión.
- Actual: pendiente.
- Status: Pendiente.

### TC-09-conflicto-radio-convive-sin-bloquear
- Tipo: unitario
- Cubre: CU-03 (CA-02), US-05, RN-03
- Setup: marcador existente con su radio de agrupación en un relevamiento en recolección.
- Pasos: Given marcador existente y su radio de agrupación; When crea otro marcador dentro de ese radio; Then crea el segundo marcador y lo deja convivir sin bloquear, marcando el conflicto y difiriéndolo al cierre.
- Expected: ambos marcadores existen y quedan accesibles; el segundo queda con el flag de conflicto; la app no resuelve el conflicto (la resolución se difiere al cierre desde la web).
- Actual: pendiente.
- Status: Pendiente.

### TC-10-sin-senal-gps-degrada-a-manual
- Tipo: unitario
- Cubre: CU-03 (CA-03, CA-04), US-05, RN-01, ADR-04
- Setup: doble de ubicación sin señal de GPS y, en una variante, permiso de ubicación denegado.
- Pasos: Given dispositivo sin señal de GPS (o permiso denegado); When toca centrar por GPS; Then responde con SIN_SENAL_GPS (o PERMISO_UBICACION_DENEGADO) y ofrece fijar el marcador manualmente en el mapa, sin inventar coordenada.
- Expected: error SIN_SENAL_GPS o PERMISO_UBICACION_DENEGADO; se ofrece fijación manual; no se persiste ninguna coordenada inventada.
- Actual: pendiente.
- Status: Pendiente.

### TC-11-mover-marcador-conserva-identidad
- Tipo: unitario
- Cubre: CU-03 (CA-05), US-06, RN-05
- Setup: marcador existente en el mapa con identidad conocida y observaciones asociadas.
- Pasos: Given marcador existente en el mapa; When lo arrastra a una nueva coordenada; Then actualiza la coordenada conservando la identidad del marcador y encola el cambio.
- Expected: la coordenada cambia; el identificador del marcador no cambia; las observaciones siguen ancladas; se encola un cambio de movimiento.
- Actual: pendiente.
- Status: Pendiente.

### TC-12-capturar-foto-resuelve-coordenada-offline
- Tipo: sincronización (modo offline)
- Cubre: CU-04 (CA-01), US-07, RN-05, NFR captura offline
- Setup: agente sobre marcador activo con permisos de cámara y ubicación concedidos, doble de ubicación con señal, doble de conectividad sin conexión.
- Pasos: Given agente sobre marcador activo con permisos y señal de GPS, sin conexión; When toma una foto; Then resuelve la coordenada en el momento (del dispositivo, no de la red), ancla la foto a una observación del marcador y la encola.
- Expected: foto anclada a una observación del marcador con coordenada del momento; cambio encolado; binario referenciado fuera de la fila de datos; todo sin conexión.
- Actual: pendiente.
- Status: Pendiente.

### TC-13-foto-sin-gps-pendiente-de-ubicacion
- Tipo: unitario
- Cubre: CU-04 (CA-02, CA-03), US-08, RN-01, ADR-04
- Setup: doble de ubicación sin señal; en una variante, permiso de cámara denegado.
- Pasos: Given agente que toma una foto sin señal de GPS (o con permiso de cámara denegado); When confirma la captura; Then conserva la foto anclada al marcador y la marca como pendiente de ubicación sin inventar coordenada (o responde PERMISO_CAMARA_DENEGADO y no abre la cámara).
- Expected: foto conservada y marcada pendiente de ubicación, sin coordenada inventada; en la variante de permiso, error PERMISO_CAMARA_DENEGADO y cámara no abierta.
- Actual: pendiente.
- Status: Pendiente.

### TC-14-marcador-compartido-varias-observaciones
- Tipo: integración
- Cubre: CU-04 (CA-04), US-07, RN-05
- Setup: marcador existente con una observación previa y una foto.
- Pasos: Given marcador existente con una observación previa; When captura otra foto sobre ese marcador; Then agrega la foto al mismo marcador, que queda compartido por varias observaciones.
- Expected: el marcador queda con más de una observación; la referencia observación→marcador se mantiene (restricción RC-02); ambos cambios quedan encolados.
- Actual: pendiente.
- Status: Pendiente.

### TC-15-comentario-y-etiqueta-offline
- Tipo: integración
- Cubre: CU-05 (CA-01), US-09, RN-05
- Setup: una foto de una observación en un relevamiento en recolección; doble de conectividad sin conexión.
- Pasos: Given una foto de una observación en recolección; When escribe el comentario "fisura longitudinal de 30 cm" y aplica la etiqueta fisura; Then registra comentario y etiqueta en el almacén local y encola los cambios.
- Expected: comentario y etiqueta persistidos localmente; una foto con a lo sumo un comentario; cambios encolados; sin conexión.
- Actual: pendiente.
- Status: Pendiente.

### TC-16-etiqueta-reutilizada-sin-duplicar
- Tipo: unitario
- Cubre: CU-05 (CA-02, CA-04), US-10, RN-05
- Setup: etiqueta fisura ya usada en el relevamiento; en una variante, intento de etiqueta sin nombre.
- Pasos: Given etiqueta fisura ya usada en el relevamiento; When la aplica a otra foto (y en la variante, confirma una etiqueta vacía); Then aplica la misma etiqueta sin duplicarla y la deja compartida entre las dos fotos (y en la variante responde ETIQUETA_VACIA y no crea la etiqueta).
- Expected: la etiqueta es única por nombre en el relevamiento y queda compartida (N:N); la variante de etiqueta vacía produce ETIQUETA_VACIA y no crea etiqueta.
- Actual: pendiente.
- Status: Pendiente.

### TC-17-comentar-foto-sin-ubicacion-resuelta
- Tipo: unitario
- Cubre: CU-05 (CA-03), US-09, RN-01
- Setup: foto que quedó pendiente de ubicación precisa, en una observación en recolección.
- Pasos: Given foto pendiente de ubicación precisa; When le agrega comentario y etiqueta; Then registra comentario y etiqueta sin requerir coordenada de la foto.
- Expected: comentario y etiqueta registrados; la foto sigue pendiente de ubicación; no se exige ni se inventa coordenada.
- Actual: pendiente.
- Status: Pendiente.

### TC-18-sync-sube-antes-de-bajar
- Tipo: sincronización (contrato)
- Cubre: CU-06 (CA-01), US-11, RN-02, ADR-03
- Setup: agente con 5 cambios locales encolados; doble de conectividad que pasa a conexión recuperada; doble del backend que acepta subida y tiene actualizaciones para bajar.
- Pasos: Given 5 cambios locales encolados y conexión recuperada; When la app detecta conexión y sincroniza; Then sube primero los 5 cambios y solo después baja las actualizaciones, mostrando 5 subidos antes de cualquier bajada.
- Expected: orden estricto subir→bajar; los 5 cambios suben y pasan a confirmado antes de la primera bajada; un intento de bajar antes de concluir la subida es rechazado (SUBIDA_NO_CONCLUIDA).
- Actual: pendiente.
- Status: Pendiente.

### TC-19-reanudacion-tras-corte-sin-duplicar
- Tipo: sincronización (modo offline)
- Cubre: CU-06 (CA-02), US-12, RN-02, RN-05, NFR reanudación sin pérdida
- Setup: ciclo con 3 cambios pendientes; doble de conectividad que corta tras confirmar el primero; identificador de origen estable por cambio.
- Pasos: Given ciclo con 3 pendientes y conexión que se corta tras confirmar el primero; When sincroniza y luego se recupera la conexión; Then deja 1 confirmado, conserva 2 en la cola, no baja actualizaciones tras el corte y al reanudar reenvía sin duplicar.
- Expected: 1 confirmado y 2 en cola tras el corte; no hubo bajada; al reanudar, el backend reconoce los reenvíos por identificador de origen y no se duplica ningún cambio; sin pérdida.
- Actual: pendiente.
- Status: Pendiente.

### TC-20-bajada-aplica-conflicto-sin-abortar
- Tipo: sincronización (contrato)
- Cubre: CU-06 (CA-03), US-13, RN-03
- Setup: bajada que incluye un marcador en conflicto por radio; cola local sin pendientes.
- Pasos: Given bajada que incluye un marcador en conflicto por radio; When sincroniza; Then aplica la actualización en conflicto a la copia local sin abortar y la reporta como elemento en conflicto en el resumen del ciclo.
- Expected: la actualización en conflicto se aplica localmente con su flag de conflicto; el ciclo no aborta; el resumen reporta el elemento en conflicto; la app no resuelve el conflicto.
- Actual: pendiente.
- Status: Pendiente.

### TC-21-token-rechazado-conserva-cola
- Tipo: sincronización (modo offline)
- Cubre: CU-06 (CA-04), US-11, RN-04
- Setup: agente con cambios encolados cuyo token fue rechazado por el doble del backend (TOKEN_INVALIDO).
- Pasos: Given agente con cambios encolados cuyo token fue rechazado; When sincroniza; Then responde con TOKEN_INVALIDO, conserva la cola intacta y solicita reloguear.
- Expected: error TOKEN_INVALIDO; la cola conserva todos sus cambios; no se descarta ni se duplica nada; se solicita relogueo (nuevo inicio online por token vencido, RN-04).
- Actual: pendiente.
- Status: Pendiente.

### TC-22-carga-manual-agrupa-por-radio
- Tipo: unitario
- Cubre: CU-07 (CA-01, CA-02), US-14, RN-01
- Setup: radio de agrupación definido; tres fotos con coordenada incrustada dentro del radio; una cuarta foto con coordenada lejana a todo marcador.
- Pasos: Given radio de agrupación definido y fotos con coordenada incrustada; When carga las fotos; Then agrupa las tres dentro del radio en un único marcador (cero marcadores nuevos adicionales) y crea un marcador nuevo en la coordenada de la foto lejana, agrupándola en él.
- Expected: las tres fotos dentro del radio quedan en un único marcador; la foto lejana crea un marcador nuevo en su coordenada incrustada; la priorización usa la ubicación incrustada de la imagen.
- Actual: pendiente.
- Status: Pendiente.

### TC-23-carga-manual-sin-exif-y-sin-radio
- Tipo: unitario
- Cubre: CU-07 (CA-03, CA-04), US-15, RN-01
- Setup: conjunto con una foto sin datos de ubicación incrustados; en una variante, carga sin radio de agrupación aplicable.
- Pasos: Given conjunto con una foto sin ubicación incrustada (y en la variante, sin radio aplicable); When carga el conjunto; Then registra esa foto como pendiente de ubicación manual sin coordenada inventada y la incluye como sin ubicación resuelta (y en la variante responde RADIO_NO_DEFINIDO y no procesa el conjunto).
- Expected: la foto sin ubicación queda pendiente de ubicación manual, sin coordenada inventada, y se reporta como sin ubicación resuelta; la variante sin radio produce RADIO_NO_DEFINIDO y no procesa.
- Actual: pendiente.
- Status: Pendiente.

### TC-24-cola-tolera-1000-cambios
- Tipo: sincronización (modo offline)
- Cubre: NFR cola ≥ 1000, CU-06, RN-05, ADR-02
- Setup: factoría determinista que encola un lote de al menos 1000 cambios pendientes en el almacén local sin conexión.
- Pasos: Given un relevamiento en recolección sin conexión; When se encolan 1000 cambios o más; Then la cola los conserva todos en orden de creación, sin pérdida, y la app sigue operativa.
- Expected: la cola contiene ≥ 1000 cambios con orden de creación monótono e identificador de origen único; no hay pérdida; la app no se degrada de forma que impida seguir capturando.
- Actual: pendiente.
- Status: Pendiente.

### TC-25-ciclo-100-cambios-bajo-30s
- Tipo: sincronización (rendimiento)
- Cubre: NFR ciclo 100 cambios ≤ 30 s, CU-06, RN-02
- Setup: 100 cambios encolados; dispositivo de referencia; red móvil típica; doble del backend o backend de referencia que responde el contrato subir-luego-bajar.
- Pasos: Given 100 cambios encolados en el dispositivo de referencia en red móvil típica; When dispara el ciclo de sincronización; Then completa el ciclo subir-luego-bajar en 30 s o menos.
- Expected: el ciclo completo (subida de los 100 más la bajada de actualizaciones) mide ≤ 30 s en el ambiente de referencia; medición registrada.
- Actual: pendiente.
- Status: Pendiente.

### TC-26-arranque-en-frio-bajo-3s
- Tipo: ciclo de vida (rendimiento)
- Cubre: NFR arranque ≤ 3 s, CU-01, ADR-01, ADR-05
- Setup: app instalada con almacén local migrado; dispositivo de referencia; arranque en frío (proceso no residente).
- Pasos: Given la app instalada en el dispositivo de referencia con el proceso no residente; When se inicia en frío; Then llega a la pantalla de sesión/verificación en 3 s o menos.
- Expected: tiempo de arranque en frío hasta la pantalla de sesión/verificación ≤ 3 s, incluyendo la aplicación de migraciones en el arranque; medición registrada.
- Actual: pendiente.
- Status: Pendiente.

### TC-27-snapshot-pantallas-criticas
- Tipo: snapshot
- Cubre: CU-01, CU-02, CU-03, CU-04, CU-05, CU-06 (render de pantallas críticas)
- Setup: baseline aprobado de las cinco pantallas críticas (login/relogueo, lista de relevamientos asignados, mapa de captura, detalle de observación, estado de sincronización); dataset sintético sembrado.
- Pasos: Given el baseline aprobado de las pantallas críticas; When se renderizan con el dataset sintético; Then el render coincide con el baseline sin diferencias no aprobadas.
- Expected: cada pantalla crítica coincide con su snapshot baseline; cualquier diferencia exige cambio con justificación y revisión (no se regenera el snapshot para que pase).
- Actual: pendiente.
- Status: Pendiente.

### TC-28-migracion-inicial-en-arranque
- Tipo: integración
- Cubre: CU-02, ADR-02, RN-05
- Setup: almacén local efímero vacío; migración inicial `0001` disponible.
- Pasos: Given un almacén local vacío; When la app arranca; Then aplica la migración inicial y reconstruye el esquema (8 entidades y asociaciones) con sus índices y restricciones de réplica.
- Expected: el esquema queda creado desde la migración base; los índices clave (idempotencia de la cola, drenado en orden, marcador en conflicto, foto pendiente) existen; el arranque no pierde datos preexistentes en migraciones posteriores.
- Actual: pendiente.
- Status: Pendiente.

## 3. Resumen de cobertura del catálogo

- TC por CU: CU-01 → TC-01..TC-04, TC-26, TC-27; CU-02 → TC-05..TC-07, TC-27, TC-28; CU-03 → TC-08..TC-11, TC-27; CU-04 → TC-12..TC-14, TC-27; CU-05 → TC-15..TC-17, TC-27; CU-06 → TC-18..TC-21, TC-24, TC-25, TC-27, TC-28; CU-07 → TC-22, TC-23. Cada CU tiene al menos un TC.
- TC por RN: RN-01 → TC-10, TC-13, TC-17, TC-22, TC-23; RN-02 → TC-18, TC-19, TC-25; RN-03 → TC-07, TC-09, TC-20; RN-04 → TC-01, TC-02, TC-03, TC-21; RN-05 → TC-04, TC-05, TC-06, TC-08, TC-11, TC-12, TC-14, TC-15, TC-16, TC-19, TC-24, TC-28. Cada RN tiene al menos un TC.
- TC de modo offline: cola (TC-24), reanudación tras corte (TC-19), convivencia con conflicto (TC-09, TC-20). TC de captura georreferenciada: TC-08 (GPS al crear marcador), TC-12 (foto con coordenada del momento), TC-22 (carga manual por ubicación incrustada y radio).
- TC de NFR numéricos: cola ≥ 1000 (TC-24), ciclo de 100 cambios ≤ 30 s (TC-25), arranque ≤ 3 s (TC-26), más captura 100 % offline (TC-08, TC-12) y reanudación sin pérdida (TC-19).

## 4. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Catálogo inicial de 28 casos de prueba referenciales de geovial-mobile, con al menos un TC por cada uno de los 7 CU y por cada una de las 5 RN; incluye TC del modo offline (cola TC-24, reanudación tras corte TC-19, convivencia con conflicto TC-09/TC-20), TC de captura georreferenciada (TC-08, TC-12, TC-22) y TC de los NFR numéricos (cola ≥ 1000 TC-24, ciclo de 100 cambios ≤ 30 s TC-25, arranque ≤ 3 s TC-26), más snapshot de pantallas críticas (TC-27) y migración inicial (TC-28). Estados Pendiente por app no implementada. |
