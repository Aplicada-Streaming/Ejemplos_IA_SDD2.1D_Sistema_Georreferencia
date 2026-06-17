# CU-03 — Crear, editar y listar relevamientos

**Proyecto:** geovial-web
**Documento:** CU-03-crear-editar-listar-relevamiento_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional

## 1. Propósito

Permitir que el jefe de área cree desde el front web un relevamiento sobre un tramo vial (uno o varios puentes y caminos), edite sus datos mientras está en recolección, dé de baja relevamientos y consulte el listado de sus relevamientos con su estado del ciclo. Es el punto de partida de la planificación del trabajo de campo.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Jefe de área | Primario | Crea, edita, da de baja y lista sus relevamientos |
| Front web | Sistema | Presenta el formulario y el listado de relevamientos y envía las operaciones al backend |
| Backend de dominio | Sistema | Valida el alcance del tramo y el estado, crea o modifica el relevamiento y devuelve el listado |

## 3. Precondiciones

- El jefe de área tiene una sesión activa en el front web (CU-01).
- El jefe de área administra al menos un ámbito sobre el que crear relevamientos.

## 4. Flujo principal

1. El jefe de área abre la pantalla de relevamientos del front web.
2. El front web solicita al backend el listado de relevamientos del jefe y los presenta con su estado del ciclo (recolección, revisión, cierre).
3. Para crear, el jefe completa el formulario con el nombre del relevamiento y la composición del tramo vial (los puentes y caminos que abarca).
4. El front web valida que la composición del tramo no esté vacía y envía la creación al backend.
5. El backend crea el relevamiento en estado de recolección y lo devuelve; el front lo agrega al listado.
6. Para editar, el jefe selecciona un relevamiento en recolección y modifica su nombre o la composición del tramo; el front envía los cambios al backend y refleja el resultado.
7. Para dar de baja, el jefe selecciona un relevamiento y confirma; el front envía la baja al backend y actualiza el listado.

## 5. Flujos alternativos

- 5.A Edición de un relevamiento que ya no está en recolección. Disparador: el jefe intenta editar un relevamiento en revisión o cerrado. El front web ofrece la pantalla en modo solo lectura y no habilita la edición de la composición del tramo. Retorna al paso 2.
- 5.B Filtro y búsqueda del listado. Disparador: el jefe tiene muchos relevamientos. El front web ofrece filtrar por estado del ciclo y buscar por nombre del tramo, delegando el filtrado y la paginación al backend. Retorna al paso 2.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| TRAMO_VACIO | El jefe intenta crear un relevamiento sin ningún puente ni camino | El front web bloquea el envío e informa que el tramo debe abarcar al menos un puente o camino |
| FUERA_DE_ALCANCE | El jefe intenta editar o dar de baja un relevamiento que no le pertenece | El front no lo lista; ante el rechazo del backend informa que está fuera de su alcance |
| RELEVAMIENTO_NO_EN_RECOLECCION | El jefe intenta editar la composición de un relevamiento que ya avanzó de estado | El front presenta la vista en solo lectura y no envía la edición |

## 7. Postcondiciones

- Éxito en creación: existe un relevamiento nuevo en estado de recolección, creado por el jefe, visible en su listado.
- Éxito en edición: los datos del relevamiento en recolección quedan actualizados.
- Éxito en baja: el relevamiento queda dado de baja y se refleja en el listado.
- Fallo: el conjunto de relevamientos no cambia y el front informa la causa.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un jefe de área con sesión activa | Crea un relevamiento "Tramo Norte" con dos puentes y un camino | El front lo crea en estado de recolección y lo muestra en su listado |
| CA-02 | Un jefe de área en el formulario de creación | Intenta crear un relevamiento sin ningún puente ni camino | El front bloquea el envío con TRAMO_VACIO y mantiene el formulario |
| CA-03 | Un relevamiento "Tramo Norte" ya en estado de revisión | El jefe lo abre para editar la composición del tramo | El front lo presenta en solo lectura (RELEVAMIENTO_NO_EN_RECOLECCION) |
| CA-04 | Un jefe con quince relevamientos en distintos estados | Filtra el listado por estado "recolección" | El front muestra solo los relevamientos en recolección de ese jefe |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-02 |
| Reglas de negocio aplicables | RN-01 (geovial-web), RN-04 (geovial-web) |
| Historias de usuario a generar | US-06, US-07, US-08 (en 06) |
| Componentes esperados | Pantalla de relevamientos; formulario de alta y edición; listado con filtros; consumo del recurso de relevamientos del backend (referencia tentativa a 05) |
| Tests previstos | Creación en recolección visible; tramo vacío bloqueado; edición fuera de recolección en solo lectura; filtro por estado (en 08) |

## 10. Notas y supuestos

- El estado del relevamiento y las transiciones válidas las gobierna el backend (RN-05 de geovial-api); el front habilita u oculta acciones según el estado vigente que el backend reporta (RN-04 de geovial-web).
- El front no persiste relevamientos; cada operación consume el contrato del backend (intake §17 geovial-web P.4).
- La paginación y el filtrado de listados los resuelve el backend; el front solo expresa los criterios elegidos por el usuario.

## 13. Interacción multiusuario y concurrencia

- Dos jefes de área distintos no comparten relevamientos: cada uno lista únicamente los propios por alcance de rol.
- Si un relevamiento cambió de estado en otra sesión mientras el jefe lo tenía abierto, al guardar el front refleja el rechazo del backend y recarga el estado vigente antes de reintentar.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de creación, edición y listado de relevamientos desde el front web, derivado de NB-02 (F-03). |
