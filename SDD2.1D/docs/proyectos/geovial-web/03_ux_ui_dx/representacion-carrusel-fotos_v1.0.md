# Representación — Carrusel de fotos encadenado

**Proyecto:** geovial-web
**Documento:** representacion-carrusel-fotos_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** UX/UI Designer + Frontend Lead
**Variante:** UX/UI

## 1. Concepto representado y propósito

El carrusel de fotos encadenado es el componente con el que el jefe de área recorre las fotos de un marcador y, al llegar a un extremo, continúa con las fotos del marcador contiguo sin cerrar el recorrido. Cada foto se muestra con su comentario, su etiqueta y la autoría de quien la registró. El propósito es ordenar la evidencia por marcador y por ubicación para confeccionar el informe de cierre, manteniendo el contexto geográfico (qué marcador se está mirando) mientras se navega la evidencia.

Es un concepto único reutilizado en más de una superficie, por eso se centraliza aquí: lo invocan el wireframe de revisión sobre mapa (CU-06) y el de resolución de conflictos (CU-07, en el comparador de marcadores). Centralizarlo evita describir su comportamiento dos veces y mantiene una sola fuente de verdad para sus estados y su accesibilidad.

## 2. Apariencia esquemática

```text
+----------------------------------------------------------------------+
|  Marcador M-03  ·  foto 3 de 6                                  [ X ] |
+----------------------------------------------------------------------+
|                                                                      |
|   [ < ]               [    FOTO AMPLIADA    ]              [ > ]      |
|                                                                      |
|   ( placeholder mientras llega el binario / marcador "foto no        |
|     disponible" si no se puede recuperar )                           |
|                                                                      |
+----------------------------------------------------------------------+
|  Comentario: "fisura longitudinal en junta de dilatacion"            |
|  Etiqueta: fisura      Autor: <agente>  (autoria conservada)         |
|                                                                      |
|  Tira de miniaturas: [m][m][M][m][m][m]  ->  | siguiente marcador    |
|                              ^ actual          (encadenado)          |
+----------------------------------------------------------------------+

Encadenamiento:
  [marcador A: f1 f2 f3] --(siguiente en el extremo)--> [marcador B: f1 f2 ...]
  [marcador B: f1] <--(anterior en el extremo)-- [marcador A: ... f3]
```

- Controles anterior y siguiente amplios y de ubicación fija (Ley de Fitts), operables por puntero, teclado y, en móvil, por gesto de deslizamiento.
- Encabezado con el marcador actual y la posición de la foto (foto N de M) para no exigir memoria de trabajo (Ley de Miller).
- Al pasar de un marcador a otro, el encabezado cambia y se anuncia el nuevo marcador; el recorrido no se interrumpe.

## 3. Variantes

| Variante | Condición de uso | Diferencias esperadas |
| --- | --- | --- |
| Carrusel de revisión | Revisión sobre mapa (CU-06) | Recorre todos los marcadores del relevamiento encadenados por contigüidad; respeta el filtro por etiqueta activo |
| Carrusel del comparador de conflictos | Resolución de conflictos (CU-07) | Acotado a las fotos de los marcadores de un conflicto; sirve para comparar antes de decidir unificar o separar |
| Marcador sin fotos | El marcador seleccionado no tiene fotos | No abre el recorrido de imágenes; muestra el aviso de marcador sin fotos y ofrece pasar al contiguo (CU-06 5.A) |
| Foto no disponible | El binario de una foto no se recupera del almacén | Muestra un marcador de foto no disponible en lugar de la imagen y continúa el recorrido con el resto (FOTO_NO_DISPONIBLE) |
| Cargando una foto | El binario aún no llegó | Placeholder en la posición de la imagen; las fotos contiguas se precargan; nunca bloquea la navegación |

## 4. Datos que consume

- Secuencia ordenada de marcadores del relevamiento y, por cada marcador, su secuencia de fotos (define el orden de encadenamiento).
- Por cada foto: el binario o su indisponibilidad, el comentario, la etiqueta y la autoría de quien la registró (la autoría se muestra aun si el autor fue dado de baja, RN-02).
- El filtro por etiqueta activo, cuando aplica (variante de revisión), para acotar el conjunto recorrible.
- La posición actual (marcador y foto) para el encabezado y el anuncio de cambios.

El carrusel no persiste ni transforma estos datos: los consume del contrato del backend a través de la superficie que lo invoca; no inventa coordenadas, autoría ni etiquetas.

## 5. Restricciones de accesibilidad

- Cada foto expone su comentario y etiqueta como alternativa textual accesible (WCAG 1.1.1); la foto no disponible se anuncia como tal, no como imagen vacía.
- El avance, el retroceso y el cambio de marcador se anuncian por región de estado para lectores de pantalla (4.1.3), incluyendo la posición (foto N de M) y el marcador actual.
- Toda la operación es por teclado (2.1.1) y el carrusel no atrapa el foco: al cerrarse lo devuelve al elemento que lo abrió (2.1.2); el foco permanece visible y no oscurecido por el overlay (2.4.7, 2.4.11).
- Los controles anterior, siguiente y ampliar cumplen el tamaño mínimo de objetivo (2.5.8) por su uso repetitivo.
- El estado de cada foto (disponible, cargando, no disponible) y el marcador actual no se comunican solo por color (1.4.1); se acompañan de texto e ícono.
- Se respeta la preferencia de movimiento reducido: la transición entre fotos se simplifica o se suprime si el usuario la activó.

## 6. Reutilización

| Artefacto que la invoca | Uso |
| --- | --- |
| wireframes-revision-mapa-carrusel_v1.0.md | Carrusel de revisión: recorre los marcadores encadenados del relevamiento (CU-06) |
| wireframes-resolucion-conflictos-cierre_v1.0.md | Carrusel del comparador: revisa la evidencia de cada marcador en conflicto antes de decidir (CU-07) |
| experiencia-de-uso_v1.0.md | Concepto transversal declarado en §9 (trazabilidad) y en los flujos 3.6 y 3.7 |

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Representación inicial del carrusel de fotos encadenado: concepto y propósito, apariencia esquemática con encadenamiento entre marcadores, variantes (revisión, comparador, sin fotos, foto no disponible, cargando), datos que consume, restricciones de accesibilidad WCAG 2.2 AA y reutilización por los wireframes de revisión y de resolución de conflictos. |
