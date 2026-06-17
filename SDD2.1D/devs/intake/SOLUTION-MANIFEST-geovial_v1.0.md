# SOLUTION-MANIFEST-geovial

Manifiesto canónico de la solución GeoVial. Artefacto derivado por el orquestador a partir de `SOLUTION-INTAKE-geovial_v1.0.md` §13, durante la Fase de validación de intake (`master-prompt.md` §3), siguiendo las reglas de derivación de `rules/_intake_rules.md` §4. Confirmado por el usuario el 2026-06-15.

---

## §1 Bloque de solución

| Campo | Valor |
|---|---|
| Nombre de solución | GeoVial |
| `nombre-solucion-kebab` | `geovial` |
| `NombreSolucionCodigo` | `GeoVial` |
| Proyecto principal | `geovial-api` |
| Intake (origen) | `SOLUTION-INTAKE-geovial_v1.0.md` (de su §13 se deriva este manifiesto) |
| Documento | `SOLUTION-MANIFEST-geovial_v1.0.md` |
| Versión | 1.0 |
| Fecha | 2026-06-15 |
| Estado | Aprobado |

### §1.1 Perfil de convención de nombres

| Parámetro | Valor | Notas |
|---|---|---|
| Forma del nombre de solución en código | PascalCase | `NombreSolucionCodigo` = `GeoVial` |
| Separador de segmentos | `.` | Separa la raíz de la solución del sufijo de rol |
| Prefijo de paquetes redistribuibles | `Aplicada` | Reemplaza la raíz cuando `redistribuible: true` |

---

## §2 Tabla de proyectos

| `nombre-proyecto-kebab` | `nombre-proyecto-codigo` | `project_type` (D8) | Rol en la solución | `redistribuible` | Dependencias | Path `/src` |
|---|---|---|---|---|---|---|
| `geovial-api` | `GeoVial.WebApi` | `rest-api` | Backend monolítico que expone la API REST consumida por el front web y la app móvil; concentra la lógica, la persistencia (SQL Server) y la seguridad (principal) | false | `geovial-storage` | `src/GeoVial.WebApi/` |
| `geovial-web` | `GeoVial.Web` | `web-monolith` | Front web de creación, recolección y revisión de relevamientos sobre mapa | false | `geovial-api` | `src/GeoVial.Web/` |
| `geovial-mobile` | `GeoVial.Mobile` | `mobile-app-maui` | App de captura de observaciones en terreno, offline-first, con sincronización | false | `geovial-api`, `aplicada-sync` | `src/GeoVial.Mobile/` |
| `geovial-storage` | `GeoVial.Storage` | `library` | Soporte de alojamiento de archivos transparente al sistema, con backend configurable (local / S3 / otro) por el usuario raíz; se integra al backend, no se publica como NuGet | false | — | `src/GeoVial.Storage/` |
| `aplicada-sync` | `Aplicada.Sync` | `library` | Soporte de sincronización para apps móviles, integrable a .NET MAUI vía NuGet (repo en GitHub), reutilizable fuera de la solución | true | — | `src/Aplicada.Sync/` |

Regla de nombres aplicada: cada proyecto se nombra `GeoVial.<Sufijo>` salvo los redistribuibles, que arrancan con el prefijo de organización `Aplicada`. `aplicada-sync` es el único `redistribuible: true` y por eso resuelve a `Aplicada.Sync`.

---

## §3 Grafo de dependencias

```text
aplicada-sync   ─┐
geovial-storage ─┼─> geovial-api ─┬─> geovial-web
                 │                └─> geovial-mobile
aplicada-sync ───────────────────────> geovial-mobile
```

Aristas declaradas en §13 del intake: `geovial-api → geovial-storage`, `geovial-web → geovial-api`, `geovial-mobile → geovial-api`, `geovial-mobile → aplicada-sync`. Grafo acíclico (DAG).

Orden topológico de generación y construcción:

```text
nivel 0: aplicada-sync, geovial-storage
nivel 1: geovial-api
nivel 2: geovial-web, geovial-mobile   (paralelizables)
```

---

## §4 Validaciones bloqueantes (resultado)

| Validación | Resultado |
|---|---|
| Cada `project_type` ∈ D8 | OK (`rest-api`, `web-monolith`, `mobile-app-maui`, `library`, `library`) |
| Exactamente un proyecto principal | OK (`geovial-api`) |
| Sin colisión de `nombre-proyecto-kebab` ni de `nombre-proyecto-codigo` | OK |
| Cada dependencia referencia un proyecto existente | OK |
| Grafo de dependencias acíclico | OK |
| §13 recorrible (filas reemplazadas, perfil presente, campos completos) | OK |

---

## §5 Checklist de validación del manifiesto derivado

- [x] El bloque de solución tiene nombre, `nombre-solucion-kebab`, `NombreSolucionCodigo`, proyecto principal y referencias de intake completos.
- [x] El perfil de convención de nombres está declarado (forma PascalCase, separador, prefijo de redistribuibles).
- [x] La tabla de proyectos tiene al menos una fila y todos los campos obligatorios completos.
- [x] Cada `project_type` pertenece al conjunto cerrado D8 de 8 valores.
- [x] Hay exactamente un proyecto principal.
- [x] No hay colisiones de `nombre-proyecto-kebab` ni de `nombre-proyecto-codigo`.
- [x] Cada dependencia referencia un proyecto existente en la tabla.
- [x] El grafo de dependencias es acíclico.
- [x] Cada proyecto `redistribuible: true` arranca su nombre de código con el prefijo de organización (`aplicada-sync` → `Aplicada.Sync`).
- [x] El control de cambios refleja la versión y fecha del documento.

---

## Control de cambios

| Versión | Fecha | Cambios | Autor |
|---|---|---|---|
| 1.0 | 2026-06-15 | Manifiesto inicial derivado de `SOLUTION-INTAKE-geovial_v1.0.md` §13 y confirmado por el usuario. 5 proyectos (`geovial-api` principal, `geovial-web`, `geovial-mobile`, `geovial-storage`, `aplicada-sync`), grafo acíclico de 2 niveles más principal. | Orquestador SDD 2.1 |
