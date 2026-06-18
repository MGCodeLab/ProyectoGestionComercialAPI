# Sprint 3 Fiscal — Pendientes Identificados

**Fecha:** 2026-06-12  
**Sprint:** Sprint 3 Fiscal (completado en este sprint)  
**Responsable:** Backend Team

---

## 1. Agregar FluentValidators para comandos Eliminar (17 módulos sin validator)

**Descripción:** La mayoría de comandos `Eliminar` en el proyecto no tienen validators asignados. Solo 2 de 19 módulos tienen validators.

### Comandos SIN validator (17 módulos):

**Catalogo (13):**
- ❌ CategoriaProducto
- ❌ CondicionPago
- ❌ ListaPrecio
- ❌ MarcaProducto
- ❌ ModuloSistema
- ❌ Moneda
- ❌ Pais
- ❌ ParametroSistema
- ❌ SerieDocumento
- ❌ TipoComprobante
- ❌ TipoImpuesto
- ❌ UnidadMedida

**Organizacion (3):**
- ❌ Almacen
- ❌ Empresa
- ❌ Sucursal

**Clientes (1):**
- ❌ Clientes

**Comercial (1):**
- ❌ Proveedor

### Comandos CON validator (2 módulos):
- ✅ TipoDocumento
- ✅ Productos

**Patrón a crear para cada uno:**
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

**Por qué:** Valida que el ID sea válido (>0) antes de pasar al handler. Previene requests malformados llegando a la BD.

**Referencia:** Patrón establecido en PR #13 (`TipoDocumento` validators)

**Prioridad:** Media  
**Estimación:** ~50-60 minutos (17 validators × ~3-4 minutos c/u)  
**Sprint asignado:** Futuro (TBD)

---

## 2. Agregar FluentValidators para comandos ActualizarEstado (0 existen)

**Descripción:** NINGÚN comando `ActualizarEstado` tiene validator en el proyecto.

**Patrón a crear:**
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

**Por qué:** Misma razón que Eliminar — validación de ID antes de llegar al handler.

**Prioridad:** Media  
**Estimación:** Depende de cuántos módulos tengan ActualizarEstado  
**Sprint asignado:** Futuro (TBD)

---

## 3. Revisar patrón de soft-delete vs hard-delete

**Descripción:** Actualmente todos los módulos hacen physical DELETE (`_context.Remove()`). Evaluar si algunos deberían ser soft-delete (`Activo = false`) según reglas de negocio.

**Módulos afectados:**
- Todos los catalógicos (19 módulos)

**Nota:** La FK guard ya está implementada para TipoComprobante. Verificar si otros módulos también necesitan `TieneDependencias()`.

**Prioridad:** Baja  
**Sprint asignado:** Futuro (Discovery requerido)

---

## 4. Auditar CrearProductoHandler

**Descripción:** CrearProductoHandler no establece explícitamente `entity.Activo = true` tras mapping (similar a lo encontrado en TipoImpuesto).

**Ubicación:** `Application/Features/Productos/Crear/CrearProductoHandler.cs` (línea 27)

**Investigar:** ¿Es intencional o debe agregarse?

**Nota:** No es violation de CLAUDE.md (AuditableEntity ya define `Activo = true`), pero auditar para consistencia.

**Prioridad:** Baja  
**Sprint asignado:** Futuro (Code review)

---

## Resumen de Effort

| Tarea | Módulos | Estimación | Prioridad |
|-------|---------|-----------|-----------|
| Validators Eliminar | 17 | 50-60 min | Media |
| Validators ActualizarEstado | TBD | TBD | Media |
| Soft-delete review | 19+ | Discovery | Baja |
| Auditoría CrearProducto | 1 | 5 min | Baja |

---

## Historial de Cambios

| Fecha | Cambio | Estado |
|-------|--------|--------|
| 2026-06-12 | Creado documento con análisis completo. 17 módulos sin Eliminar validators, 0 ActualizarEstado validators. | ✅ |

