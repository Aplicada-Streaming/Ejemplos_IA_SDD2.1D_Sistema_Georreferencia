# Criterios de validación — aplicada-sync

**Proyecto:** aplicada-sync
**Documento:** criterios-validacion_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Ingeniero QA / SDET Senior (AG-08), variante QA + SDET Library

## 1. Propósito

Define qué significa "el sistema validado para release" en `aplicada-sync`. Como la librería es redistribuible y se publica como paquete consumible, "validado para release" equivale a "publicable": la superficie pública cumple sus invariantes, sus NFR numéricos se midieron en condiciones equivalentes a las de operación y la compatibilidad del contrato está verificada. Estos criterios son la puerta que el release debe cruzar; complementan la Definition of Done por capa de `definition-of-done_v1.0.md` y se materializan como los quality gates de `estrategia-calidad_v1.0.md` §3. El criterio de hecho del release del mini-plan de 07 §7 referencia estos criterios.

## 2. Criterios funcionales

Cada CU crítico debe estar cubierto y verde antes del release. Los CU críticos son los que materializan las garantías de NB-04.

- [ ] CU-01 inicializar sesión: TC-01, TC-02, TC-03 verdes; recuperación de sesión persistida verde (CU-01 CA-03).
- [ ] CU-02 encolar cambio local: TC-04, TC-05, TC-06 verdes; no duplicación por identificador verificada.
- [ ] CU-03 ejecutar ciclo subir-luego-bajar: TC-07, TC-08, TC-09, TC-16 verdes; orden y convivencia con conflicto cubiertos.
- [ ] CU-04 detectar conectividad y disparar: TC-15, TC-18 verdes; disparo y no reentrada cubiertos.
- [ ] CU-05 consultar estado y cola: TC-19 verde en sus tres variantes.
- [ ] CU-06 reanudar sincronización interrumpida: TC-10, TC-11 verdes; reanudación sin pérdida ni duplicación cubierta.
- [ ] Cada criterio Given/When/Then de la tabla de aceptación de cada CU (02) tiene su TC verde en la matriz de cobertura.

## 3. Criterios no funcionales

Cada NFR cumple su SLA numérico medido en el ambiente de pruebas equivalente al de operación (red móvil simulada, almacén persistente efímero). Origen: intake §17 P.10 y arquitectura 05 §8.

- [ ] Tiempo de sincronización de lote: lote de 100 cambios sincronizado en <= 30 s en red móvil típica (TC-20).
- [ ] Capacidad de cola local: cola de >= 1000 cambios pendientes sin degradación funcional; tamaño reportado coincide con las entradas únicas (TC-14).
- [ ] Reanudación sin pérdida: 0 cambios perdidos y 0 duplicados tras un corte en la fase de subida (TC-09, TC-10).
- [ ] Idempotencia ante reintento: 100 % de los cambios reenviados o reaplicados con efecto neto único (TC-11, TC-12).
- [ ] Orden subir-antes-de-bajar: 0 actualizaciones descendentes aplicadas mientras quedan pendientes confirmables (TC-07, TC-13).
- [ ] Continuidad ante conflicto: 0 ciclos abortados por un estado en conflicto reportado por el backend (TC-16).

## 4. Criterios de regresión

- [ ] La suite de regresión completa se ejecuta y queda verde antes del release.
- [ ] Ningún TC verde de la versión anterior pasó a rojo sin una justificación documentada (08_rules §4.7 y §5.4).
- [ ] Todo defecto de integridad de datos cerrado durante el desarrollo generó al menos un TC de regresión que lo previene (08_rules §4.10, falta de prueba de regresión); toda semilla property-based que provocó un contraejemplo quedó fijada como TC dedicado.

## 5. Criterios de calidad de código

- [ ] Cobertura por capa cumplida: dominio >= 85 % líneas / >= 80 % branches; infraestructura >= 70 % / >= 60 %; global >= 80 % / >= 70 % (gate G4; intake §17 P.6).
- [ ] Mutation score del dominio >= 60 % (gate G5).
- [ ] Análisis estático sin issues críticos y sin warnings nuevos respecto de la versión anterior (gate G9).
- [ ] Ningún test sin assert; ningún catch silencioso que enmascare un fallo (gate G2; 08_rules §5.4).
- [ ] Compilación sin advertencias tratadas como error (gate G1).

## 6. Criterios de compatibilidad de superficie pública

Específicos de una librería redistribuible (ADR-03; contrato §6); condición de release adicional declarada en el mini-plan de 07 §5.

- [ ] Ningún cambio incompatible de la superficie pública (operaciones, formas de datos, conjunto de estados, garantía de orden, códigos de error) se publica sin un incremento de versión mayor.
- [ ] El snapshot del contrato (resumen del ciclo, resumen de reanudación, estado consultable, conjunto de códigos de error) coincide con la baseline aprobada o el cambio está justificado por un incremento de versión correspondiente (TC-21).
- [ ] Verificación post-publicación: el paquete publicado se restaura en un proyecto limpio y el quick-start reproduce el contrato (TC-21, BT-14, intake §17 P.8); un quick-start que no reproduzca el comportamiento bloquea la publicación.

## 7. Excepciones documentadas

Cualquier criterio no cumplido se acepta solo con una ADR explícita y un plan de remediación con BT en el backlog (08_rules §4.7; backlog de 06).

- Un umbral de cobertura solo se baja con un ADR que lo justifique, registrado en el control de cambios de `estrategia-calidad_v1.0.md` (08_rules §2.2: los porcentajes son piso, no techo).
- Un NFR cuyo módulo no entra en el tramo en curso no bloquea ese tramo, pero sí el release global: TC-14 y TC-20 deben estar verdes antes de declarar el paquete publicable (R3 del mini-plan de 07).
- Una deuda técnica que impida cumplir un criterio se admite solo si queda registrada como BT explícita en el backlog de 06 con su plan de remediación.

## 8. Control de cambios

| Versión | Fecha | Descripción |
| --- | --- | --- |
| 1.0 | 2026-06-15 | Criterios de validación iniciales de aplicada-sync: funcionales por CU con TC verde por criterio Given/When/Then, no funcionales por los seis NFR numéricos del intake §17 P.10 con su TC de medición, regresión con TC obligatorio por defecto de integridad y por contraejemplo property-based, calidad de código (cobertura por capa, mutation >= 60 %, análisis estático), compatibilidad de superficie pública (snapshot del contrato y verificación post-publicación) y excepciones solo con ADR y BT de remediación. Derivados de 02, del intake §17 P.6/P.10, de ADR-03 y de las reglas 08 §4.7. |
