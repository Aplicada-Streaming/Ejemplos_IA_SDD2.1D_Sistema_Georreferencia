# Wireframe — Panel de relevamientos

**Proyecto:** geovial-web
**Documento:** wireframes-panel-relevamientos_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** UX/UI Designer + Frontend Lead
**Variante:** UX/UI

## 1. Pantalla y propósito

Panel de relevamientos: la superficie inicial (home) del jefe de área una vez ingresado. Lista sus relevamientos con el estado vigente del ciclo, permite crear uno nuevo indicando nombre y composición del tramo, editar la composición mientras está en recolección, dar de baja con confirmación y filtrar o buscar para acotar listas largas. Es el punto de partida desde el que se abre la revisión, la resolución de conflictos y el cierre. CU origen: CU-03. Marco aplicado: `experiencia-de-uso_v1.0.md` (flujo 3.3, estados §4.2, errores §8).

## 2. Layout

Layout de aplicación administrativa: barra superior con identidad y rol y cierre de sesión; navegación lateral con las superficies del rol vigente; área principal con encabezado de acciones, fila de filtros y una tabla de datos. La creación y la edición ocurren en un formulario lateral o modal; la baja, en un modal de confirmación.

```text
+----------------------------------------------------------------------+
| GeoVial        Jefe de área: <identidad>            [ Cerrar sesion ] |
+-----------+----------------------------------------------------------+
| Navegacion|  Relevamientos                      [ + Nuevo relevamiento ]
|           |                                                          |
| > Relev.  |  Estado: [ Todos v ]   Buscar tramo: [______________] (X)|
|   Agentes |                                                          |
|   ...     |  +----------------------------------------------------+  |
|           |  | Nombre del tramo | Estado     | Composicion | Acc. |  |
| (solo las |  +----------------------------------------------------+  |
|  del rol) |  | Tramo Norte      | Recoleccion| 2 P / 1 C   | ...  |  |
|           |  | Ruta del Bajo    | Revision   | 1 P / 3 C   | ...  |  |
|           |  | Acceso Sur       | Cerrado    | 0 P / 2 C   | ...  |  |
|           |  | ...                                                |  |
|           |  +----------------------------------------------------+  |
|           |  [ < anterior ]   pagina 1 de N   [ siguiente > ]        |
+-----------+----------------------------------------------------------+

Acciones por fila (segun estado y alcance, RN-01 / RN-04):
  Recoleccion -> Abrir | Editar | Asignar agentes | Pasar a revision | Dar de baja
  Revision    -> Abrir (revisar) | Resolver conflictos | Cerrar | Devolver a recoleccion
  Cerrado     -> Abrir (solo lectura)
```

Formulario de alta y edición (lateral o modal):

```text
+--------------------------------------------+
|  Nuevo / Editar relevamiento          [ X ]|
|--------------------------------------------|
|  Nombre del relevamiento                   |
|  [______________________________________]  |
|                                            |
|  Composicion del tramo                     |
|  [ + Agregar puente ]  [ + Agregar camino ]|
|  - Puente: <nombre>                  [del] |
|  - Camino: <nombre>                  [del] |
|  ( aviso inline: el tramo no puede quedar  |
|    vacio )                                 |
|                                            |
|        [ Cancelar ]    [ Guardar ]         |
+--------------------------------------------+
```

## 3. Componentes principales

| Componente | Propósito | Datos que muestra | Comportamiento |
| --- | --- | --- | --- |
| Barra superior | Mostrar identidad, rol y salida | Identidad y rol del usuario, acción de cierre de sesión | Cierre de sesión vuelve a la pantalla de ingreso (CU-01) |
| Navegación lateral | Acceder a las superficies del rol | Solo las superficies del rol vigente (RN-01) | Oculta lo fuera de alcance; no lista superficies de otros roles |
| Acción Nuevo relevamiento | Iniciar la creación | Rótulo | Abre el formulario de alta; destino amplio |
| Fila de filtros | Acotar el listado | Filtro por estado del ciclo y búsqueda por nombre de tramo | Delegan filtrado y paginación al backend (CU-03 5.B); el filtro de estado no se comunica solo por color |
| Tabla de datos de relevamientos | Listar los relevamientos del alcance | Nombre, estado del ciclo, resumen de composición, acciones | Encabezados semánticos; estado siempre visible (RN-04); acciones por fila habilitadas según estado y alcance |
| Indicador de estado del ciclo | Comunicar la etapa | Etiqueta de texto e ícono (recolección, revisión, cierre) | No depende solo del color (1.4.1) |
| Formulario de alta y edición | Capturar nombre y composición del tramo | Campos y lista de puentes y caminos | Valida que el tramo no quede vacío antes de enviar (CU-03 paso 4); en edición fuera de recolección, solo lectura |
| Modal de confirmación de baja | Prevenir bajas accidentales | Mensaje de confirmación que aclara que la baja conserva la autoría (RN-02) | Confirmable y cancelable; devuelve el foco al cerrarse |
| Paginación | Recorrer listas largas | Página actual y total | Delegada al backend |

## 4. Interacciones

| Acción | Disparador | Resultado esperado | Precondición |
| --- | --- | --- | --- |
| Crear relevamiento | Guardar en el formulario de alta con tramo no vacío | El backend lo crea en recolección y aparece en el listado (CU-03 paso 5) | Tramo con al menos un puente o camino |
| Bloquear creación con tramo vacío | Guardar sin puentes ni caminos | Aviso inline TRAMO_VACIO; el formulario se conserva (CU-03 CA-02) | — |
| Editar composición | Editar un relevamiento en recolección | El backend aplica los cambios y el listado se refresca (CU-03 paso 6) | Relevamiento en recolección (RN-04) |
| Intentar editar fuera de recolección | Abrir para editar un relevamiento en revisión o cerrado | Vista en solo lectura (CU-03 5.A, RELEVAMIENTO_NO_EN_RECOLECCION) | Relevamiento fuera de recolección |
| Dar de baja | Confirmar en el modal de baja | El backend da de baja y el listado se actualiza; la autoría se conserva (RN-02) | Relevamiento del alcance |
| Filtrar por estado o buscar por tramo | Cambio de filtro o término de búsqueda | El backend devuelve el subconjunto y la tabla se refresca (CU-03 5.B) | — |
| Abrir una acción de ciclo | Acción por fila (revisar, resolver, cerrar, transicionar) | Navega a la superficie correspondiente (CU-06, CU-07, CU-08) | Acción válida para el estado y el alcance |

## 5. Estados

| Estado | Condición que lo produce | Representación esperada |
| --- | --- | --- |
| Vacío | El jefe no tiene relevamientos todavía | Ilustración o ícono neutro y CTA "crear el primer relevamiento" con texto orientativo |
| Cargando | Listado en curso contra el backend | Skeleton de filas de la tabla; spinner solo si supera el umbral percibido |
| Con datos | El backend devolvió relevamientos | Tabla con estado del ciclo, filtros activos y acciones por fila habilitadas según estado |
| Vacío por filtro | El filtro o la búsqueda no arrojan coincidencias | Mensaje "no hay relevamientos que coincidan" y acción para limpiar el filtro |
| Error recuperable | Falla al listar, crear, editar o dar de baja | Banner con reintento (listado) o aviso inline (formulario); el formulario conserva lo ingresado |
| Sin conexión al circuito | El backend no responde o la sesión expiró | Banner persistente de servicio no disponible o aviso de sesión expirada con reingreso |

## 6. Versión móvil o responsive

En anchos reducidos la navegación lateral colapsa a un menú desplegable y la tabla de datos refluye a tarjetas apiladas, una por relevamiento, con el nombre del tramo, el estado siempre visible y las acciones agrupadas en un menú por tarjeta. Los filtros se apilan sobre la lista. El formulario de alta pasa a ocupar la pantalla completa. Se conserva el tamaño mínimo de objetivo en todas las acciones.

## 7. Notas de implementación

- Accesibilidad: tabla con encabezados semánticos y nombres de columna (1.3.1); estado del ciclo comunicado por texto e ícono, no solo por color (1.4.1); foco visible en filas y acciones (2.4.7); el modal de baja devuelve el foco al disparador al cerrarse y no atrapa el teclado (2.1.2); campos del formulario con etiquetas asociadas (3.3.2).
- Performance percibida: skeleton de filas mientras llega el listado; paginación y filtrado delegados al backend para no bloquear; estado de envío en el guardado y en la baja.
- Internacionalización: los nombres de tramo y las etiquetas son contenido del usuario, de longitud variable; se truncan con indicación visual y texto completo accesible. Los encabezados de columna toleran expansión sin romper el layout.
- Prevención de errores: las acciones inválidas para el estado vigente o el alcance del rol se ocultan o deshabilitan (RN-01, RN-04); la baja siempre pasa por confirmación.

## 8. Trazabilidad

| Dimensión | Referencia |
| --- | --- |
| Persona objetivo | Jefe de área (00) |
| CU origen | CU-03 (crear, editar y listar relevamientos) |
| Reglas de negocio relevantes | RN-01 (visibilidad por rol), RN-02 (conservación de autoría en la baja), RN-04 (estados visibles y habilitación de acciones) |
| Marco de experiencia aplicado | experiencia-de-uso_v1.0.md (flujo 3.3, estados §4.2, errores §8) |
| US a generar | US-06, US-07, US-08 (06) |
| Tests previstos | Creación en recolección visible; tramo vacío bloqueado; edición fuera de recolección en solo lectura; filtro por estado; baja conserva autoría (08) |

## 9. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Wireframe inicial del panel de relevamientos (home del jefe de área), anclado a CU-03 y al marco de experiencia. Layout de listado con filtros, formulario de alta y edición, modal de baja, estados (vacío, cargando, con datos, vacío por filtro, error y sin conexión al circuito), reflujo a tarjetas en móvil y notas de accesibilidad WCAG 2.2 AA. |
