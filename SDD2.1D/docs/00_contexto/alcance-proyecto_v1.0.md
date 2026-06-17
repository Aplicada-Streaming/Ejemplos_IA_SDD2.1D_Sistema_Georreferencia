# Alcance del Proyecto

**Proyecto:** GeoVial (solución)
**Documento:** alcance-proyecto_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Product Manager + API Product Owner
**Trazabilidad upstream:** SOLUTION-INTAKE §4, §9, §10
**Trazabilidad downstream:** 01_necesidades_negocio, 02_especificacion_funcional, 03_ux_ui_dx, 05_arquitectura_tecnica, 07_plan-sprint, 11_examples

## 1. Propósito

Este documento delimita qué entra y qué no entra en la solución GeoVial, registra los supuestos y las restricciones bajo los que se construye, y fija los criterios con los que se acepta el proyecto. Sirve para que las categorías downstream trabajen sin reabrir la negociación de alcance con el cliente y para evitar que se incorporen capacidades que el negocio dejó deliberadamente fuera de la primera versión.

## 2. Descripción general

GeoVial es una solución para el relevamiento fotográfico georreferenciado de tramos viales —puentes y caminos—, pensada para trabajar en campo sin conectividad. El jefe de área crea un relevamiento sobre un tramo vial y le asigna agentes de campo; la cuadrilla recolecta en terreno observaciones ancladas a marcadores geográficos, compuestas por notas, fotos, comentarios y etiquetas; la captura ocurre incluso sin conexión y se sincroniza cuando se recupera el acceso a la red. Luego el jefe revisa la información sobre un mapa, resuelve al cierre los conflictos de marcadores y cierra el relevamiento para confeccionar sus informes rutinarios.

La operación se estructura alrededor del ciclo de un relevamiento (recolección, revisión y cierre) y de una jerarquía de usuarios de cuatro niveles que define quién puede hacer qué.

## 3. Objetivos del proyecto

- Digitalizar y estandarizar el relevamiento de tramos viales con georreferenciación confiable de cada observación.
- Habilitar la captura en campo sin depender de la conectividad del lugar, con sincronización posterior automática.
- Dejar la información de cada relevamiento ordenada por marcador y ubicación para acelerar y hacer reproducible la confección de los informes de cierre.
- Soportar la convivencia con conflictos de marcadores durante la recolección, difiriendo su resolución al cierre sin bloquear el acceso a la información.

## 4. Alcance incluido

### 4.1 Capacidades incluidas

Las siguientes capacidades, priorizadas como Must Have y Should Have en el intake (§4), forman parte del alcance de la primera versión:

| ID | Capacidad | Prioridad |
|---|---|---|
| F-01 | Jerarquía y administración de usuarios en cuatro niveles (raíz, jefe general, jefe de área, agente), con altas y bajas según jerarquía | Must Have |
| F-02 | Alta y baja de agentes de campo directamente por el jefe de área | Must Have |
| F-03 | Alta, baja y visualización de relevamientos por el jefe de área; un relevamiento abarca un tramo vial de uno o varios puentes y caminos | Must Have |
| F-04 | Asignación de agentes de campo a un relevamiento | Must Have |
| F-05 | Captura en campo de observaciones con foto y resolución de coordenadas geográficas en el momento | Must Have |
| F-06 | Modelo de observación: marcador geográfico con notas, fotos, comentarios por foto y etiquetas; marcador compartible por varias observaciones | Must Have |
| F-07 | Captura sin conexión con sincronización que sube los cambios locales y luego baja las actualizaciones del relevamiento asignado | Must Have |
| F-08 | Inicio de sesión con credenciales, deslogueo completo para cambio de usuario y relogueo en sesión activa mediante la seguridad del propio dispositivo | Must Have |
| F-09 | Carga manual con priorización de los datos de ubicación de la foto y radio de agrupación de fotos en un marcador | Must Have |
| F-10 | Visualización en mapa con puntos, con desplazamiento del punto y centrado por ubicación en campo | Must Have |
| F-11 | Transición de estado de recolección a revisión y cierre del relevamiento por el jefe | Must Have |
| F-12 | Carrusel de fotos por marcador con encadenado al marcador siguiente y anterior; ampliar, comentar, etiquetar y filtrar | Should Have |
| F-13 | Resolución de conflictos de marcadores al cierre del relevamiento | Should Have |
| F-14 | Reasignación de agentes a un relevamiento desde la aplicación de campo, por el jefe | Should Have |
| F-15 | Carga manual completa del relevamiento por el agente desde el entorno web | Should Have |

Las capacidades F-16 (exportar e importar un relevamiento completo en un único archivo comprimido) y F-17 (configuración del destino de almacenamiento de los archivos por el usuario raíz) están priorizadas como Could Have: se incorporan si la cadencia del proyecto lo permite, sin comprometer el camino principal.

### 4.2 Entregables

- Solución integral que cubre el ciclo del relevamiento de punta a punta para los cuatro roles, accesible desde un entorno web y desde una aplicación de campo.
- Capacidad de captura en campo sin conexión con sincronización posterior.
- Vista de revisión sobre mapa con carrusel de fotos por marcador y resolución de conflictos al cierre.

### 4.3 Ambientes

El alcance contempla los entornos necesarios para construir, probar y desplegar la solución de manera incremental por fases. La definición fina de los ambientes (desarrollo, prueba, despliegue) se elabora en las categorías técnicas downstream.

## 5. Alcance excluido

| Funcionalidad excluida | Justificación | Versión futura tentativa |
|---|---|---|
| Auto-registro o flujo de solicitud y aceptación self-service de agentes de campo | Las altas y bajas de agentes las realiza directamente el jefe de área; un flujo de autogestión no aporta valor en la operación definida para la primera versión | Supeditada a pedido del negocio (no planificada) |
| Análisis automático de imágenes y detección de fallas por visión | La evaluación del estado del tramo la hace el jefe de área de forma manual; GeoVial documenta y ordena la evidencia, no diagnostica | No planificada |
| Ruteo y navegación asistida en terreno | El mapa sirve para ubicar y revisar marcadores, no para guiar el desplazamiento del agente en el campo | No planificada |

## 6. Supuestos

- El cliente confirmará la línea de base operativa actual para ajustar los targets de las métricas de éxito.
- La jerarquía de cuatro roles se mantiene estable durante la construcción de la primera versión.
- La operación tolera que los conflictos de marcadores convivan durante la recolección y se resuelvan recién al cierre del relevamiento.
- El dispositivo de campo dispone de mecanismos de ubicación y de seguridad propia que la solución aprovecha para la captura y el relogueo.

## 7. Restricciones

- Sin fecha objetivo: proyecto de investigación educativo, no atado a un hito externo; la cadencia la fija el avance del equipo.
- Sin presupuesto formal asignado, con fines comerciales futuros.
- Sin exigencias regulatorias declaradas.
- Sin integraciones obligatorias con sistemas existentes.
- Equipo de un único desarrollador: obliga a un alcance incremental y verificable por fases.

## 8. Criterios de aceptación del proyecto

- [ ] La jerarquía de cuatro roles funciona de punta a punta, con altas y bajas según el alcance de cada nivel.
- [ ] El jefe de área puede crear un relevamiento sobre un tramo vial y asignarle agentes de campo.
- [ ] El agente de campo puede capturar observaciones georreferenciadas con foto, comentario y etiqueta sin conexión.
- [ ] Las observaciones capturadas sin conexión se sincronizan al recuperar el acceso a la red, subiendo primero los cambios locales y bajando después las actualizaciones.
- [ ] El jefe puede revisar las observaciones sobre el mapa, resolver los conflictos de marcadores al cierre y cerrar el relevamiento.
- [ ] Cada capacidad incluida tiene su criterio de aceptación verificable definido en la categoría funcional downstream.

## 9. Gestión de cambios de alcance

Todo cambio de alcance se registra como una nueva versión de este documento. Las solicitudes de incorporar capacidades excluidas o de promover capacidades Could Have al camino principal se evalúan contra los objetivos del proyecto y la cadencia disponible, y requieren la aprobación del propietario del problema. Las exclusiones declaradas en §5 no se generan en las categorías downstream salvo que este documento se actualice explícitamente.

## 10. Trazabilidad

Upstream: el alcance incluido proviene del cuadro MoSCoW del intake (§4); las exclusiones y su justificación provienen de §9; las restricciones provienen de §10.

Downstream: este alcance alimenta 01_necesidades_negocio (necesidades acotadas al alcance), 02_especificacion_funcional (casos de uso solo de las capacidades incluidas, evitando las exclusiones de §5), 03_ux_ui_dx (experiencias de las capacidades incluidas), 05_arquitectura_tecnica (decisiones acotadas al alcance), 07_plan-sprint (planificación por capacidad y prioridad) y 11_examples (ejemplos circunscriptos al alcance).
