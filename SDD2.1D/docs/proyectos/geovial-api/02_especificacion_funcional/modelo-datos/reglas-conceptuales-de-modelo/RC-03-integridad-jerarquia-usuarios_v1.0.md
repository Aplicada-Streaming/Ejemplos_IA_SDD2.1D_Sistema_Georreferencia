# RC-03 — Integridad de la jerarquía de usuarios

**Proyecto:** geovial-api
**Documento:** RC-03-integridad-jerarquia-usuarios_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Analista Funcional + API Designer

## 1. Enunciado

Todo usuario distinto del usuario raíz referencia a un usuario administrador del nivel inmediato superior que lo dio de alta; el usuario raíz no tiene administrador, y la cadena de administración no forma ciclos ni salta niveles.

## 2. Entidades involucradas

Usuario, Rol.

## 3. Tipo de restricción

Referencial y de cardinalidad sobre una relación autorreferente.

## 4. Mecanismo de verificación conceptual

Al dar de alta un usuario se comprueba que su administrador existe y que su rol es exactamente el nivel inmediato superior al del usuario creado; el rol raíz se admite sin administrador. Se verifica que recorrer la cadena de administradores desde cualquier usuario llega al raíz sin repetir usuarios.

## 5. RN o CU que la justifican

RN-01; CU-01, CU-02, CU-18.

## 6. Control de cambios

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Redacción inicial de la regla conceptual de integridad de la jerarquía de usuarios. |
