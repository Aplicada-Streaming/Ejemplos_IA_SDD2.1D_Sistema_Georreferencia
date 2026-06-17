# RC-06 — Monotonía de la marca de sincronización

**Proyecto:** geovial-api
**Documento:** RC-06-monotonia-marca-sincronizacion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Enunciado

La marca de sincronización de un relevamiento para un cliente de campo solo avanza: una bajada de actualizaciones adopta una marca posterior o igual a la anterior, nunca anterior, de modo que el cliente no vuelva a un punto de sincronización ya superado.

## 2. Entidades involucradas

MarcaSincronizacion, Relevamiento, Usuario (cliente de campo).

## 3. Tipo de restricción

Derivación y valor permitido (orden monótono).

## 4. Mecanismo de verificación conceptual

Al entregar una nueva marca en una bajada se comprueba que es posterior o igual a la marca previa del mismo relevamiento y cliente; la marca solo se adopta cuando el cliente confirma haber aplicado las novedades, evitando retroceder ante un corte. Una marca aportada que no es reconocible se rechaza y obliga a una sincronización completa.

## 5. RN o CU que la justifican

RN-06, RN-07; CU-10, CU-11, CU-21.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la regla conceptual de monotonía de la marca de sincronización. |
