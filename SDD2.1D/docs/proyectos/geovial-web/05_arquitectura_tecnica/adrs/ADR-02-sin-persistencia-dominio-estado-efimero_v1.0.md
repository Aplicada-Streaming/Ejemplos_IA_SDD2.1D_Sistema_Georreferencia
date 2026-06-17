# ADR-02 — Sin persistencia de dominio en el front; estado de UI/sesión efímero

**Proyecto:** geovial-web
**Documento:** ADR-02-sin-persistencia-dominio-estado-efimero_v1.0.md
**Versión:** 1.0
**Estado:** Aceptado
**Fecha:** 2026-06-15
**Autor:** Arquitecto Senior
**Categoría:** Persistencia

## 1. Contexto

`geovial-web` presenta y manipula entidades de dominio (Usuario, Rol, Relevamiento, TramoVial, Asignacion, MarcadorGeografico, ConflictoMarcadores, Observacion, Foto, Comentario, Etiqueta y la configuración de almacenamiento), pero no es el dueño de ese dominio: el modelo conceptual del front es una vista de consumo del modelo autoritativo de `geovial-api` (02 §5), que es la fuente de verdad y garantiza la integridad mediante sus reglas conceptuales. El intake fija que el front no tiene base de datos propia y que su estado es solo efímero del circuito y caché de sesión en memoria (§17 geovial-web P.4, `tiene_persistencia=false`). Esta decisión motiva la omisión del artefacto `modelo-datos-logico_v1.0.md`, que la regla 05 §2.2 marca como "Sí" por defecto para `web-monolith` pero que aquí no aplica por ausencia de persistencia. Motivan esta decisión las once CU del front (que leen y escriben dominio siempre contra la API) y las cinco RN de presentación.

## 2. Decisión

Se decide que `geovial-web` no persiste estado de dominio: no posee base de datos ni almacenamiento durable propio. Todo dato autoritativo se lee y se escribe contra el contrato REST de `geovial-api`. El front mantiene únicamente estado de UI y de sesión efímero en memoria del circuito interactivo: pantalla actual, estado del componente de mapa, estado del carrusel, filtros, resultados de consulta cacheados para la sesión y el token bearer. Ese estado es volátil y reconstruible desde la API. En consecuencia, no se produce `modelo-datos-logico_v1.0.md` para este proyecto: el modelo lógico autoritativo vive en `geovial-api` (`modelo-datos-logico_v1.0.md` de ese proyecto), y esta omisión queda registrada como decisión aquí.

## 3. Estado

Aceptado el 2026-06-15. Decisión fijada por el intake (§17 geovial-web P.4, `tiene_persistencia=false`) y por la naturaleza del front como vista de consumo del dominio de `geovial-api` (02 §5).

## 4. Alternativas consideradas

| Alternativa | Pros | Contras |
| --- | --- | --- |
| Sin persistencia de dominio; estado de UI/sesión efímero (elegido) | Una sola fuente de verdad (la API); sin riesgo de divergencia ni de doble integridad; despliegue del front sin almacenamiento durable | El estado de UI se pierde al perder el circuito (mitigado: reconstruible) |
| Caché persistente de dominio en el front | Menos llamadas a la API; lecturas más rápidas tras reconexión | Descartado: introduce una segunda copia del dominio con riesgo de divergencia e invariantes duplicadas; contra §17.P.4; complejidad de invalidación para un equipo de un dev |
| Base de datos local del front | Trabajo parcial sin la API | Descartado: el front no es offline-first (eso es de la app de campo, fuera de alcance, 02 §2); duplica el rol de persistencia de la API |

## 5. Consecuencias positivas

1. Hay una única fuente de verdad: la integridad del dominio la garantiza `geovial-api` (sus reglas conceptuales), sin invariantes duplicadas en el front (02 §5).
2. El contenedor de front no requiere almacenamiento persistente: el reciclado no pierde dato de dominio, solo circuitos activos reconstruibles (arquitectura §5).
3. Se elimina el riesgo de divergencia entre una copia local y la API y el costo de invalidación de caché durable, adecuado para un equipo de un desarrollador.
4. El alcance del front queda nítido: presentación y consumo, sin responsabilidad de persistencia (no se produce `modelo-datos-logico`).

## 6. Consecuencias negativas y trade-offs

1. La pérdida del circuito pierde el estado de UI en curso (pantalla, filtros, posición del mapa). Se acepta: el estado es reconstruible consultando a la API al reconectar y ninguna acción de dominio depende de retener estado no confirmado.
2. El front depende de la disponibilidad y latencia de la API para toda lectura: no hay modo degradado con datos locales. Se acepta porque el front es una herramienta de oficina con conectividad asumida (la captura offline es de la app de campo, fuera de alcance).
3. Cada reconexión puede implicar reconsultar a la API. Se acota con la caché de sesión en memoria del circuito, de vida igual a la del circuito.

## 7. Implementación

- El Cliente de API es el único punto de lectura y escritura del dominio; no existe repositorio ni almacén durable en el front.
- La Aplicación de UI cachea resultados de consulta solo en memoria del circuito, con invalidación al cambiar de pantalla o ante un error de versión o de estado del backend.
- No se genera ni mantiene ningún esquema de datos ni migración en este proyecto; el modelo lógico autoritativo y su migración inicial viven en `geovial-api`.
- Convención impuesta: ningún componente del front escribe a disco ni a una base; los archivos (fotos, unidad transferible) se delegan al contrato REST de la API.

## 8. Métricas de validación

- Cero artefactos de persistencia durable en el front, verificado por inspección de la composición y del despliegue (09).
- El reciclado del contenedor de front no pierde dato de dominio: tras reiniciar, toda la información se recupera consultando a la API, verificado en 08.
- La caché de sesión se invalida correctamente ante cambio de pantalla y ante error de versión/estado, verificado por prueba de componente en 08.

## 9. Referencias

- NB-01, NB-02, NB-05, NB-06, NB-07; CU-01 a CU-11; RN-01 a RN-05.
- Intake §17 geovial-web P.4 (`tiene_persistencia=false`).
- 02 §5 (modelo conceptual como vista de consumo, sin RC propias); `geovial-api` `modelo-datos-logico_v1.0.md` (modelo lógico autoritativo).
- ADRs relacionadas: ADR-01 (estilo), ADR-03 (autenticación).
- `arquitectura-solucion_v1.0.md` §6.

## 10. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Decisión de persistencia: el front no persiste dominio; estado de UI/sesión efímero en el circuito; el dato autoritativo es de geovial-api. Registra la omisión de `modelo-datos-logico_v1.0.md` (el modelo lógico autoritativo vive en geovial-api). Aceptada (fijada en intake §17.P.4). |
