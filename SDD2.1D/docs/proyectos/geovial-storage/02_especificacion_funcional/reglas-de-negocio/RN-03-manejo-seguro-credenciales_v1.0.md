# RN-03 — Manejo seguro de las credenciales del proveedor

**Proyecto:** geovial-storage
**Documento:** RN-03-manejo-seguro-credenciales_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Enunciado

Las credenciales y parámetros sensibles de un proveedor de almacenamiento nunca se exponen a través de la superficie pública de la librería: no se devuelven en los resultados de las operaciones, no se incluyen en los mensajes de error y no son recuperables una vez configurados.

## 2. Justificación

El proveedor remoto requiere credenciales de acceso cuya filtración comprometería toda la evidencia almacenada. NB-07 deja el control del destino en manos del usuario raíz; ese control implica custodiar las credenciales que habilitan ese destino. Una credencial visible por la superficie pública o filtrada en un mensaje de error sería un riesgo de seguridad inaceptable.

## 3. Ámbito de aplicación

Se evalúa al configurar el proveedor activo (las credenciales entran pero no salen), al recuperar y al listar (los errores de proveedor no disponible no deben revelar configuración) y en cualquier resultado o error que la librería devuelva al consumidor. La regla es atemporal y aplica a todo proveedor que requiera credenciales.

## 4. Consecuencia si se viola

Si una credencial o un parámetro sensible aparece en un resultado, en un mensaje de error o es recuperable por una operación de lectura, se considera una violación de seguridad que debe rechazarse en revisión y corregirse antes de liberar la librería.

## 5. CU afectados

CU-02, CU-05, CU-06.

## 6. Pruebas que la verifican

Pruebas que configuran un proveedor con credenciales y verifican que ninguna operación de lectura ni de listado las devuelve; pruebas que fuerzan el error de proveedor no disponible y verifican que el mensaje no contiene credenciales ni parámetros de conexión; revisión de que no existe una operación pública que devuelva la configuración sensible del proveedor activo (referencia a casos previstos en 08).

## 7. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la regla de manejo seguro de las credenciales del proveedor. |
