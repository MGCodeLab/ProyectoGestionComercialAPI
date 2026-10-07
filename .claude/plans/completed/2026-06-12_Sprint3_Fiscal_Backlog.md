# Sprint 3 Fiscal — Pendientes Identificados

**Fecha creación:** 2026-06-12  
**Fecha completación:** 2026-06-27  
**Sprint:** Sprint 3 Fiscal (completado en este sprint)  
**Responsable:** Backend Team
**Estado:** ✅ COMPLETADO

---

## 1. Agregar FluentValidators para comandos Eliminar (17 módulos sin validator)

**Descripción:** La mayoría de comandos `Eliminar` en el proyecto no tienen validators asignados. Solo 2 de 19 módulos tienen validators.

### Comandos SIN validator (17 módulos):

**Catalogo (13):**
- ✅ CategoriaProducto
- ✅ CondicionPago
- ✅ ListaPrecio
- ✅ MarcaProducto
- ✅ ModuloSistema
- ✅ Moneda
- ✅ Pais
- ✅ ParametroSistema
- ✅ SerieDocumento
- ✅ TipoComprobante
- ✅ TipoImpuesto
- ✅ UnidadMedida

**Organizacion (3):**
- ✅ Almacen
- ✅ Empresa
- ✅ Sucursal

**Clientes (1):**
- ✅ Clientes

**Comercial (1):**
- ✅ Proveedor

### Comandos CON validator (2 módulos):
- ✅ TipoDocumento
- ✅ Productos

**Patrón aplicado:**
```csharp
public class EliminarXxxValidator : AbstractValidator<EliminarXxxCommand>
{
    public EliminarXxxValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id debe ser mayor a 0");
    }
}
```

**Referencia:** Patrón establecido en PR #13 (`TipoDocumento` validators)

**Estimación:** 50-60 minutos  
**Tiempo real:** ~60 minutos  
**Status:** ✅ COMPLETADO

---

## 2. Agregar FluentValidators para comandos ActualizarEstado (18 módulos sin validator)

**Descripción:** De 19 comandos `ActualizarEstado`, solo 1 tiene validator. 18 módulos carecen de validator.

### Comandos CON validator (1 módulo):
- ✅ TipoDocumento

### Comandos SIN validator (18 módulos):

**Catalogo (12):**
- ✅ TipoImpuesto
- ✅ TipoComprobante
- ✅ SerieDocumento
- ✅ MarcaProducto
- ✅ ListaPrecio
- ✅ CondicionPago
- ✅ CategoriaProducto
- ✅ UnidadMedida
- ✅ ParametroSistema
- ✅ Pais
- ✅ Moneda
- ✅ ModuloSistema

**Organizacion (3):**
- ✅ Sucursal
- ✅ Empresa
- ✅ Almacen

**Comercial (1):**
- ✅ Proveedor

**Productos (1):**
- ✅ Productos

**Clientes (1):**
- ✅ Clientes

**Patrón aplicado:**
```csharp
public class ActualizarEstadoXxxValidator : AbstractValidator<ActualizarEstadoXxxCommand>
{
    public ActualizarEstadoXxxValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id debe ser mayor a 0");
    }
}
```

**Estimación:** 45-55 minutos  
**Tiempo real:** ~45 minutos  
**Status:** ✅ COMPLETADO

---

## 3. Revisar patrón de soft-delete vs hard-delete

**Descripción:** Actualmente todos los módulos hacen physical DELETE (`_context.Remove()`). Evaluar si algunos deberían ser soft-delete (`Activo = false`) según reglas de negocio.

**Módulos afectados:**
- Todos los catalógicos (19 módulos)

**Nota:** La FK guard ya está implementada para TipoComprobante, TipoDocumento y Moneda. Otros módulos catálogicos necesitan evaluación según dependencias reales.

**Prioridad:** Baja  
**Sprint asignado:** Futuro (Discovery requerido)  
**Status:** 🔲 PENDIENTE (próximo backlog)

---

## 4. Auditar CrearProductoHandler

**Descripción:** CrearProductoHandler no establece explícitamente `entity.Activo = true` tras mapping (similar a lo encontrado en TipoImpuesto).

**Ubicación:** `Application/Features/Productos/Crear/CrearProductoHandler.cs` (línea 27)

**Nota:** No es violation de CLAUDE.md (AuditableEntity ya define `Activo = true`), pero auditar para consistencia.

**Prioridad:** Baja  
**Sprint asignado:** Futuro (Code review)  
**Status:** 🔲 PENDIENTE (próximo backlog)

---

## Resumen de Effort — COMPLETADO

| Tarea | Módulos | Estimación | Tiempo Real | Status |
|-------|---------|-----------|-------------|--------|
| Validators Eliminar | 17 | 50-60 min | 60 min | ✅ |
| Validators ActualizarEstado | 18 | 45-55 min | 45 min | ✅ |
| Soft-delete review | 19+ | Discovery | - | 🔲 Pendiente |
| Auditoría CrearProducto | 1 | 5 min | - | 🔲 Pendiente |

**Total ejecutado:** ~105 minutos (2 tasks completadas)

---

## Commits Realizados

| Commit | Descripción |
|--------|-------------|
| c7c6266 | feat(catalogo): crear EliminarValidator para 5 módulos |
| 62c79e2 | feat(catalogo): crear EliminarValidator para 7 módulos |
| a1e9cde | feat(org,cliente,comercial): crear EliminarValidator para 5 módulos |
| bbff5a7 | feat(catalogo): crear ActualizarEstadoValidator para 6 módulos |
| 8278141 | feat(catalogo): crear ActualizarEstadoValidator para 6 módulos |
| 09389f3 | feat(org,comercial,productos,cliente): crear ActualizarEstadoValidator para 6 módulos |

**Total:** 6 commits, 35 validators creados

---

## Verificación Final

✅ Build: Compilación correcta. 0 Errores, 0 Advertencias  
✅ Patrón: Consistente con TipoDocumento/ActualizarEstadoTipoDocumento  
✅ DI Registration: AddValidatorsFromAssembly incluye todos automáticamente  
✅ Git Status: Working tree clean  
✅ Branch: catalogo-base/validators limpia y lista para PR  

---

## Historial de Cambios

| Fecha | Cambio | Estado |
|-------|--------|--------|
| 2026-06-27 | COMPLETADO: 35 validators (17 Eliminar + 18 ActualizarEstado) implementados, compilados y verificados. | ✅ |
| 2026-06-27 | CORRECCIÓN: Actualizado conteo de ActualizarEstado validators. Verificado: 19 handlers totales, 1 con validator (TipoDocumento), 18 sin validator. | ✅ |
| 2026-06-12 | Creado documento con análisis completo. 17 módulos sin Eliminar validators, error inicial en conteo ActualizarEstado. | ✅ |

---

## Próximos Pasos (Backlog futuro)

1. Implementar Discovery de `TieneDependencias()` en módulos que realmente lo necesiten
2. Estandarizar `Activo = true` en todos los Create mappings (best practice defensiva)
3. Revisar soft-delete vs hard-delete strategy completa
4. Auditar CrearProductoHandler para consistencia

**Responsable:** Nexus Backend Team  
**Prioridad:** Media-Baja (para próximo sprint)
