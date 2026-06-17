# Compatibilidad de Plataformas

**Proyecto:** GeoVial (solución)
**Documento:** compatibilidad-plataformas_v1.0.md
**Versión:** 1.0
**Estado:** Propuesto
**Fecha:** 2026-06-15
**Autor:** Product Manager + API Product Owner
**Trazabilidad upstream:** SOLUTION-INTAKE §17 P.9 (geovial-api, geovial-web, geovial-mobile, geovial-storage, aplicada-sync)
**Trazabilidad downstream:** 05_arquitectura_tecnica, 09_devops

## 1. Resumen ejecutivo

GeoVial corre sobre tres clases de plataforma target, acotadas a lo que declara el intake:

- El backend y el front web se ejecutan en contenedores con runtime .NET sobre contenedor base Linux: un contenedor para el backend y otro para el front, según la infraestructura pedida en el intake (§16).
- El front web se consume desde navegadores modernos de actualización continua (evergreen).
- La aplicación de campo se distribuye únicamente para Android. No se soportan iOS ni Windows en la primera versión, por decisión confirmada del cliente (intake §17 P.9).

Las versiones mínimas exactas (nivel de API de Android, versión de runtime .NET y versiones de navegador) todavía no están fijadas. En este documento se proponen valores por defecto razonables para un stack .NET actual y se marcan explícitamente como propuestos, a confirmar al cerrar §17 P.9 del intake.

## 2. Matriz de compatibilidad

| Componente | Contenedor (runtime .NET sobre base Linux) | Navegador web (evergreen) | Android | Notas |
|---|---|---|---|---|
| Backend (geovial-api) | Soportado | No aplica | No aplica | Se ejecuta en el contenedor de backend dedicado |
| Front web (geovial-web) | Soportado (alojado en el contenedor de front) | Soportado | No aplica | Se sirve desde el contenedor de front y se consume en navegador |
| Aplicación de campo (geovial-mobile) | No aplica | No aplica | Soportado | Único target Android; iOS y Windows fuera de v1 |
| Soporte de almacenamiento (geovial-storage) | Soportado | No aplica | No aplica | Se integra al backend; comparte su runtime y contenedor |
| Soporte de sincronización (aplicada-sync) | No aplica | No aplica | Soportado | Acompaña a la app de campo; único target Android en v1 |

## 3. Restricciones de plataforma justificadas

| Plataforma | Versión mínima | Estado del dato | Motivo |
|---|---|---|---|
| Contenedor base Linux con runtime .NET (backend y front) | .NET 8 (LTS) | Propuesto, a confirmar al cerrar §17 P.9 | Valor por defecto razonable para un stack .NET actual con soporte de largo plazo; el intake aún no fija la versión |
| Navegadores modernos evergreen (front web) | Últimas dos versiones estables de cada navegador de actualización continua | Propuesto, a confirmar al cerrar §17 P.9 | Cubre el parque de navegadores actualizados sin comprometer a versiones legacy; el intake aún no fija las versiones |
| Android (app de campo y sincronización) | Nivel de API 26 (Android 8.0) | Propuesto, a confirmar al cerrar §17 P.9 | Valor por defecto razonable para una app de campo .NET actual; el intake difiere la versión mínima a la Parte C de geovial-mobile |

Las versiones declaradas como propuestas no constituyen un compromiso: se confirman cuando se cierre §17 P.9 del intake para cada proyecto afectado, y recién entonces este documento pasa de Propuesto a un estado posterior.

## 4. Alternativas para plataformas no soportadas

| Plataforma no soportada en v1 | Justificación | Alternativa para el usuario |
|---|---|---|
| iOS (app de campo) | El cliente confirmó Android como único target móvil de v1; sumar iOS no aporta valor en la operación definida y multiplica el costo de construcción y distribución para un equipo de un desarrollador | Usar un dispositivo Android para la captura en campo; la incorporación de iOS queda supeditada a un pedido futuro del negocio |
| Windows (app de campo) | Mismo criterio: el único target móvil confirmado es Android; el trabajo de campo se apoya en dispositivos Android | Usar un dispositivo Android para la captura en campo; el front web cubre las tareas que no requieren estar en terreno |
| Navegadores legacy sin actualización continua | El front web apunta a navegadores de actualización continua; sostener navegadores legacy agrega costo sin demanda declarada | Usar un navegador moderno actualizado para acceder al front web |

## 5. Estado de implementación por plataforma

| Componente | Plataforma | Estado |
|---|---|---|
| Backend (geovial-api) | Contenedor con runtime .NET sobre base Linux | Planificado |
| Front web (geovial-web) | Contenedor de front + navegadores evergreen | Planificado |
| Aplicación de campo (geovial-mobile) | Android | Planificado |
| Soporte de almacenamiento (geovial-storage) | Contenedor del backend | Planificado |
| Soporte de sincronización (aplicada-sync) | Android | Planificado |

El estado es Planificado para todos los componentes: la construcción se realiza por fases según el roadmap, y las versiones mínimas se confirman al cerrar §17 P.9.

## 6. Trazabilidad downstream

Upstream: la matriz se acota a las plataformas que declara el intake en §17 P.9 de cada proyecto (contenedor con runtime .NET para backend y front, navegadores evergreen para el front web, y Android como único target móvil para geovial-mobile y aplicada-sync).

Downstream: este documento alimenta 05_arquitectura_tecnica (decisiones de runtime y empaquetado en contenedor) y 09_devops (matriz de sistema operativo, runtime y entornos de integración continua, build de la app de campo para Android y construcción de imágenes de contenedor). Las versiones mínimas propuestas se confirman al cerrar §17 P.9 del intake y se reflejan luego en 09_devops.
