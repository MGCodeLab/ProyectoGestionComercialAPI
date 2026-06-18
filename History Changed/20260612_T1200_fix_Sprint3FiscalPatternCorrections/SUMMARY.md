# Fix: Sprint 3 Fiscal — Corrección de Patrones y Refactoring

**Fecha:** 2026-06-12  
**Tipo:** fix + refactor  
**Módulos afectados:** TipoComprobante, SerieDocumento  
**Estado final:** ✅ Compilación exitosa — 0 errores

---

## Resumen Ejecutivo

Auditoría y corrección completa de los módulos TipoComprobante y SerieDocumento del Sprint 3 Fiscal.
Ambos módulos fueron construidos sin seguir los patrones establecidos en `IMPLEMENTATION_PATTERNS.md`.
Se corrigieron todas las desviaciones y se agregó funcionalidad nueva (endpoint GetCombo para TipoComprobante).

---

## Cambios Realizados

### 1. TipoComprobante — GetCombo Endpoint (nuevo)

**Motivación:** El frontend necesita poblar selects/dropdowns con los tipos de comprobante activos.

**Archivos modificados:**
- `Application/Interfaces/ITipoComprobanteService.cs` — Agregado `ObtenerCombo(CancellationToken)`
- `Infrastructure/Repository/TipoComprobanteService.cs` — Implementación con `.Where(x => x.Activo).Select(ComboDto)`
- `GestionComercial/Controllers/TiposComprobanteController.cs` — Endpoint `GET /combo/list`

---

### 2. SerieDocumento — Refactoring Completo del Módulo

**Motivación:** El módulo completo tenía múltiples desviaciones del patrón estándar (ver Issues 13-17 en `COMMON_ISSUES_AND_FIXES.md`).

**Archivos modificados:**

| Archivo | Cambio |
|---------|--------|
| `Application/Interfaces/ISerieDocumentoService.cs` | Reescrito: 5 métodos estándar + `ObtenerProximoNumeroAsync` preservado |
| `Infrastructure/Repository/SerieDocumentoService.cs` | Reescrito: `ObtenerTodos` sin filtro Activo, `Eliminar` hace DELETE físico, todos con CancellationToken |
| `Application/Features/.../ActualizarSerieDocumentoCommand.cs` | `IRequest<int>` → `IRequest<Unit>` |
| `Application/Features/.../ActualizarEstadoSerieDocumentoCommand.cs` | `IRequest<int>` → `IRequest<Unit>` |
| `Application/Features/.../EliminarSerieDocumentoCommand.cs` | `IRequest<int>` → `IRequest<Unit>` |
| `Application/Features/.../CrearSerieDocumentoHandler.cs` | Agregados IMapper + ILogger; `_mapper.Map<SerieDocumento>(request)` |
| `Application/Features/.../ActualizarSerieDocumentoHandler.cs` | `IRequest<Unit>`, IMapper+ILogger, `_mapper.Map(request, entity)`, `FechaActualizacion` |
| `Application/Features/.../ActualizarEstadoSerieDocumentoHandler.cs` | `IRequest<Unit>`, ILogger, `FechaActualizacion` |
| `Application/Features/.../EliminarSerieDocumentoHandler.cs` | `IRequest<Unit>`, ILogger, `_service.Eliminar(entity, token)` |
| `Application/Mappings/Catalogo/SerieDocumentoProfile.cs` | Reescrito: `ReverseMap()` + todos los command mappings |
| `GestionComercial/Controllers/SeriesDocumentoController.cs` | Reescrito: `_mapper.Map<Command>(dto)`, `command with { Id = id }`, `OkResponse(string.Empty,...)`, `HttpContext.RequestAborted` |

---

### 3. Fix de Build Error — NotFoundException Constructor

**Error:** Los 3 handlers de SerieDocumento (Actualizar, ActualizarEstado, Eliminar) llamaban `NotFoundException` con 2 argumentos.

**Fix:** `new NotFoundException($"SerieDocumento con ID {request.Id} no encontrado")`

**Archivos corregidos:** Los 3 handlers de SerieDocumento listados arriba.

---

## Errores Encontrados y Corregidos

| # | Error | Causa | Fix |
|---|-------|-------|-----|
| 1 | `CS1503` en handlers | `NotFoundException(nameof(T), int)` — sobrecarga inexistente | Usar `NotFoundException($"... {id} ...")` |
| 2 | Eliminar hacía soft-delete | `_context.Remove()` no llamado, en cambio `Activo = false` | Cambiar a `_context.Remove(entity)` |
| 3 | `ObtenerTodos` filtraba activos | `.Where(x => x.Activo)` en service | Eliminar filtro — `ObtenerTodos` retorna todos |
| 4 | Commands retornaban `IRequest<int>` | Copiar incorrecto del patrón Crear | Actualizar/ActualizarEstado/Eliminar → `IRequest<Unit>` |
| 5 | Handlers sin IMapper/ILogger | Creados solo con IService | Agregar IMapper (Crear/Actualizar) e ILogger (todos) |
| 6 | Profile sin mappings commands | Solo DTO↔Entity | Agregar `CreateMap<Command, Entity>()` y `ReverseMap()` |

---

## Lecciones Aprendidas

1. **Una desviación en el módulo → auditar TODO el módulo.** Siempre hay múltiples desviaciones correlacionadas.
2. **`ObtenerCombo` es el único método que filtra por `Activo`.** `ObtenerTodos` nunca filtra.
3. **`NotFoundException` en este proyecto: un solo constructor `string`.** Leer la clase antes de usarla.
4. **Antes de asumir la API de una clase → verificar la implementación real o copiar de un handler existente que compile.**
5. **Método extra del negocio (`ObtenerProximoNumeroAsync`) debe preservarse al refactorizar hacia el patrón estándar.**

---

## Documentación Actualizada

- `IA_Docs/COMMON_ISSUES_AND_FIXES.md` — Issues 19, 20, 21 agregados
- `IA_Docs/IMPLEMENTATION_PATTERNS.md` — Sección 11 (Patrón Combo) + reglas NotFoundException en Handlers + checklist actualizado
