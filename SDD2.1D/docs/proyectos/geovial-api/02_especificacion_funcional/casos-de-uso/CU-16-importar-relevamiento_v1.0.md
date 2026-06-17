# CU-16 — Importar un relevamiento completo reconstruyendo su estructura

**Proyecto:** geovial-api
**Documento:** CU-16-importar-relevamiento_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Propósito

Permitir reincorporar al sistema un relevamiento previamente exportado como unidad transferible única, reconstruyendo sus marcadores, observaciones, fotos, comentarios y etiquetas con su estructura intacta. Habilita mover un relevamiento entre entornos y restaurarlo sin perder correspondencia entre sus piezas.

## 2. Actores

| Actor | Tipo | Rol |
| --- | --- | --- |
| Jefe de área o usuario raíz | Primario | Solicita la importación de una unidad transferible |
| Backend de portabilidad | Sistema | Valida y reconstruye el relevamiento desde la unidad |
| Almacén relacional y de archivos | Sistema | Persiste el relevamiento reconstruido y aloja sus fotos |

## 3. Precondiciones

- El solicitante está autenticado y su rol está habilitado para importar relevamientos (jefe de área o usuario raíz).
- La unidad transferible está completa y corresponde a un relevamiento exportado por el sistema.

## 4. Flujo principal

1. El solicitante envía la unidad transferible a importar.
2. El backend valida la integridad y la estructura de la unidad.
3. El backend reconstruye los marcadores, observaciones, comentarios y etiquetas, y aloja las fotos a través de la librería de almacenamiento.
4. El backend restablece las referencias entre fotos, comentarios, etiquetas, observaciones y marcadores tal como estaban en el origen.
5. El backend crea el relevamiento importado en el ámbito del solicitante y responde con la ubicación del recurso reconstruido.

## 5. Flujos alternativos

- 5.A Importación de un relevamiento ya presente. Disparador: el relevamiento de la unidad ya existe en el entorno. El backend importa como un relevamiento distinto, sin sobrescribir el existente, o informa el duplicado según la decisión del solicitante. Retorna al paso 5.
- 5.B Unidad con fotos parcialmente alojables. Disparador: el almacén no puede alojar algunas fotos. El backend reconstruye el relevamiento con las fotos alojables y reporta las no alojadas, sin descartar el resto de la estructura. Retorna al paso 5.

## 6. Excepciones y errores

| Código | Causa | Respuesta del sistema |
| --- | --- | --- |
| UNIDAD_INVALIDA | La unidad no corresponde a una exportación reconocible o está corrupta | Rechaza con estado de solicitud inválida y no importa nada |
| UNIDAD_INCOMPLETA | A la unidad le faltan piezas necesarias para reconstruir el relevamiento | Rechaza con estado de solicitud inválida e indica las piezas faltantes |
| ROL_NO_AUTORIZADO | El solicitante no está habilitado para importar | Rechaza con estado de prohibido y no importa nada |

## 7. Postcondiciones

- Éxito: existe un relevamiento reconstruido con su estructura sin pérdidas, en el ámbito del solicitante.
- Fallo: no se crea ningún relevamiento y se devuelve un problema con el código correspondiente.

## 8. Criterios de aceptación Given/When/Then

| ID | Given | When | Then |
| --- | --- | --- | --- |
| CA-01 | Una unidad transferible válida exportada por el sistema | El jefe la importa | El backend reconstruye el relevamiento con el 100 por ciento de su estructura y responde con la ubicación del recurso |
| CA-02 | Una unidad corrupta o ajena al sistema | El jefe intenta importarla | El backend rechaza con el código UNIDAD_INVALIDA y no importa nada |
| CA-03 | Una unidad cuyas fotos solo se alojan parcialmente | El jefe la importa | El backend reconstruye el relevamiento con las fotos alojables y reporta las no alojadas |
| CA-04 | Un usuario con rol agente de campo | Intenta importar una unidad | El backend rechaza con el código ROL_NO_AUTORIZADO |

## 9. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Necesidad de negocio | NB-06 |
| Reglas de negocio aplicables | RN-01 |
| Historias de usuario a generar | US-33, US-34 (en 06) |
| Componentes esperados | Recurso de importación; reconstructor de relevamiento; validador de integridad de la unidad; integración con la librería de almacenamiento (referencia tentativa a 05) |
| Tests previstos | Importación fiel de la estructura; unidad inválida rechazada; fotos parcialmente alojables reportadas; importación por rol no autorizado rechazada (en 08) |

## 10. Notas y supuestos

- La capacidad de portabilidad es Could Have (NB-06); se incorpora si la cadencia lo permite.
- La fidelidad de la importación (criterio de NB-06) exige reconstruir la estructura sin pérdidas; un faltante de piezas necesarias se rechaza en vez de importar una estructura incompleta.
- El formato de la unidad transferible es el mismo que produce CU-15; su detalle pertenece a la categoría 05.

## 12. Performance esperado del CU

- La importación debe escalar con el tamaño del relevamiento sin degradar el resto del sistema; el alojamiento de fotos depende del proveedor configurado.

## 15. Idempotencia y reintento

- La importación admite clave de idempotencia para que un reintento de la misma unidad no genere relevamientos duplicados (CU-21).
- Un reintento de una importación parcialmente fallida reanuda la reconstrucción sin duplicar las piezas ya creadas.

## 11. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial del CU de importación del relevamiento completo, derivado de NB-06 (F-16). |
