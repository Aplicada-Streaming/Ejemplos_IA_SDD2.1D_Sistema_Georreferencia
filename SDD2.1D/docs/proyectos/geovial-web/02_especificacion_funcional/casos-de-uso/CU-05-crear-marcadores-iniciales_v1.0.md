# CU-05 — Crear marcadores geográficos iniciales sobre el mapa

**Proyecto:** geovial-web
**Documento:** CU-05-crear-marcadores-iniciales_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional

## 1. Propósito

Permitir que el jefe de área, desde el componente de mapa del front web, cree y ubique marcadores geográficos iniciales sobre un relevamiento en recolección, para previsualizar la experiencia de campo y orientar a la cuadrilla sobre los puntos de referencia del tramo. Deja puntos de partida sobre el mapa antes de salir a terreno.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Jefe de área | Primario | Crea, ubica y mueve marcadores iniciales sobre el mapa |
| Front web | Sistema | Presenta el componente de mapa, captura la ubicación del marcador y envía la operación al backend |
| Backend de dominio | Sistema | Crea el marcador en el relevamiento, registra su coordenada y devuelve su identidad estable |

## 3. Precondiciones

- El jefe de área tiene una sesión activa en el front web (CU-01).
- Existe un relevamiento del jefe en estado de recolección.

## 4. Flujo principal

1. El jefe de área abre el relevamiento sobre el componente de mapa del front web.
2. El front web presenta el mapa centrado en la zona del tramo, con los marcadores ya existentes si los hubiera.
3. El jefe crea un marcador indicando un punto del mapa; el front captura la coordenada de ese punto.
4. El jefe opcionalmente ajusta la posición moviendo el marcador y le agrega una etiqueta de referencia.
5. El front web envía la creación del marcador con su coordenada al backend.
6. El backend crea el marcador con identidad estable dentro del relevamiento y lo devuelve; el front lo muestra fijado en el mapa.

## 5. Flujos alternativos

- 5.A Marcador dentro del radio de otro. Disparador: el jefe ubica un marcador inicial dentro del radio de uno ya existente. El front web lo crea igualmente; el backend registra el conflicto de marcadores como estado válido y la información queda accesible, difiriendo la resolución al cierre. Retorna al paso 6.
- 5.B Mover un marcador ya creado. Disparador: el jefe arrastra un marcador existente a otra posición. El front captura la nueva coordenada y envía la actualización al backend, que conserva la identidad del marcador. Retorna al paso 6.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| RELEVAMIENTO_NO_EN_RECOLECCION | El jefe intenta crear marcadores en un relevamiento que no está en recolección | El front presenta el mapa en solo lectura y no envía la creación |
| COORDENADA_FUERA_DE_RANGO | El punto indicado cae fuera del rango geográfico admitido | El front no fija el marcador e informa que la ubicación no es válida |
| FUERA_DE_ALCANCE | El jefe intenta crear marcadores en un relevamiento ajeno | El front no abre el relevamiento; ante el rechazo del backend informa que está fuera de su alcance |

## 7. Postcondiciones

- Éxito: existen uno o más marcadores iniciales con coordenada y, opcionalmente, etiquetas, visibles sobre el mapa del relevamiento.
- Éxito con conflicto: el marcador se crea aunque caiga dentro del radio de otro; el conflicto convive sin bloquear y la información queda accesible.
- Fallo: el mapa no incorpora el marcador y el front informa la causa.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Un relevamiento "Tramo Norte" en recolección abierto en el mapa | El jefe crea un marcador en un punto del puente y le pone la etiqueta "acceso" | El front lo fija en el mapa con esa etiqueta y el backend le da identidad estable |
| CA-02 | Un marcador ya creado sobre el mapa | El jefe lo arrastra a una posición cercana | El front actualiza su coordenada conservando su identidad |
| CA-03 | Un marcador inicial ubicado dentro del radio de otro existente | El jefe lo crea igualmente | El front lo muestra y la información queda accesible aunque exista conflicto de marcadores |
| CA-04 | Un relevamiento ya en revisión | El jefe intenta crear un marcador inicial | El front presenta el mapa en solo lectura (RELEVAMIENTO_NO_EN_RECOLECCION) |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-02 |
| Reglas de negocio aplicables | RN-01 (geovial-web), RN-02 (geovial-web), RN-04 (geovial-web) |
| Historias de usuario a generar | US-11, US-12 (en 06) |
| Componentes esperados | Componente de mapa con puntos; captura y edición de coordenada del marcador; consumo del recurso de marcadores del backend (referencia tentativa a 05) |
| Tests previstos | Creación de marcador con etiqueta; movimiento conserva identidad; creación dentro de radio convive con conflicto; mapa en solo lectura fuera de recolección (en 08) |

## 10. Notas y supuestos

- El detalle de interacción del mapa (arrastre, zoom, pines) pertenece a la categoría 03; aquí se fija el qué del flujo de creación y ubicación.
- La identidad estable del marcador y la convivencia con conflictos son del backend (RC-01 y RN-03 de geovial-api); el front no resuelve conflictos en este flujo, los difiere al cierre (CU-07).
- El front no persiste marcadores; consume el contrato del backend (intake §17 geovial-web P.4).

## 13. Interacción multiusuario y concurrencia

- Si un agente crea marcadores en campo sobre el mismo relevamiento, el jefe verá esos marcadores al recargar el mapa; ambos conjuntos conviven sin bloquearse.
- Dos marcadores próximos creados por personas distintas conviven como conflicto hasta la resolución al cierre, sin impedir la operación.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de creación de marcadores iniciales sobre el mapa desde el front web, derivado de NB-02 (F-10) y del flujo 1 del intake (§6). |
