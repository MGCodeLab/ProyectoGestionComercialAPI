# PENDIENTE: Homogeneidad ActualizarEstado + Servicios con FechaActualizacion Duplicada

**Fecha:** 2026-06-28  
**Prioridad:** Media (post-testing del sprint actual)  
**Responsable:** Nexus-Fast-Builder  
**Aprobado por:** Miguel González Cuevas  

---

## 📋 CONTEXTO

Tras refactorizar 29 handlers `Actualizar` a `AuditableUpdateHandlerBase`, se detectaron dos problemas residuales de homogeneidad:

1. **9 handlers `ActualizarEstado`** aún usan `IRequestHandler` directamente
2. **4+ services** asignan `FechaActualizacion = DateTime.UtcNow` en su método `Actualizar()` — lo cual ahora es **redundante** porque el base handler ya lo hace antes de llamar al service

---

## 🔴 PROBLEMA 1: 9 Handlers ActualizarEstado sin refactorizar

### Patrón A — Handler gestiona entidad, llama `_service.Actualizar(entity)` (7 handlers)

Estos SÍ pueden usar `AuditableUpdateHandlerBase`. Siguen el mismo patrón que los ya refactorizados.

| Handler | Ruta |
|---------|------|
| ActualizarEstadoCategoriaProductoHandler | `Application/Features/Catalogo/CategoriaProducto/ActualizarEstado/` |
| ActualizarEstadoMarcaProductoHandler | `Application/Features/Catalogo/MarcaProducto/ActualizarEstado/` |
| ActualizarEstadoListaPrecioHandler | `Application/Features/Catalogo/ListaPrecio/ActualizarEstado/` |
| ActualizarEstadoTipoDocumentoHandler | `Application/Features/Catalogo/TipoDocumento/ActualizarEstado/` |
| ActualizarEstadoAlmacenHandler | `Application/Features/Organizacion/Almacen/ActualizarEstado/` |
| ActualizarEstadoEmpresaHandler | `Application/Features/Organizacion/Empresa/ActualizarEstado/` |
| ActualizarEstadoSucursalHandler | `Application/Features/Organizacion/Sucursal/ActualizarEstado/` |

**Patrón actual (antes):**
```csharp
public class ActualizarEstadoCategoriaProductoHandler : IRequestHandler<..., int>
{
    public async Task<int> Handle(command, CancellationToken ct)
    {
        var categoria = await _service.ObtenerPorIdAsync(command.Id, tracking: true, ct);
        categoria.Activo = command.Activo;
        await _service.Actualizar(categoria, ct);   // ← service aún tiene FechaActualizacion interna
        return categoria.Id;
    }
}
```

**Patrón objetivo (después):**
```csharp
public class ActualizarEstadoCategoriaProductoHandler
    : AuditableUpdateHandlerBase<ActualizarEstadoCategoriaProductoCommand, int>
{
    public ActualizarEstadoCategoriaProductoHandler(
        ICategoriaProductoService service,
        ILogger<ActualizarEstadoCategoriaProductoHandler> logger)
        : base(null, logger)   // ← null porque no se necesita IMapper en ActualizarEstado
    {
        _service = service;
    }

    public override async Task<int> Handle(ActualizarEstadoCategoriaProductoCommand command, CancellationToken ct)
    {
        var categoria = await _service.ObtenerPorIdAsync(command.Id, tracking: true, ct);
        if (categoria == null)
            throw new KeyNotFoundException($"CategoriaProducto con ID {command.Id} no encontrada");

        categoria.Activo = command.Activo;

        return await UpdateAuditableEntity(
            categoria,
            async () => await _service.Actualizar(categoria, ct),
            () => categoria.Id,
            ct
        );
    }
}
```

**Nota clave:** Se pasa `null` para `IMapper` porque estos handlers no hacen mapeo de DTOs. Es seguro mientras el handler no llame `Mapper.Map()`. Ver Problema 3 abajo para el riesgo.

---

### Patrón B — Handler delega a `_service.ActualizarEstado(id, activo)` (2 handlers)

Estos handlers **NO** pueden usar fácilmente `AuditableUpdateHandlerBase` porque no gestionan la entidad directamente — el service la carga y actualiza internamente.

| Handler | Ruta |
|---------|------|
| ActualizarEstadoProveedorHandler | `Application/Features/Comercial/Proveedor/ActualizarEstado/` |
| ActualizarEstadoCondicionPagoHandler | `Application/Features/Catalogo/CondicionPago/ActualizarEstado/` |

**Decisión arquitectónica para Patrón B:**  
Estos services (ProveedorService.ActualizarEstado, CondicionPagoService.ActualizarEstado) DEBEN conservar `FechaActualizacion = DateTime.UtcNow` dentro del método de service, porque el handler no tiene acceso a la entidad.

**Para estos 2 handlers NO aplicar AuditableUpdateHandlerBase.** Dejarlos como están es correcto para este patrón.

---

## 🔴 PROBLEMA 2: Services con FechaActualizacion Redundante en Actualizar()

Tras la refactorización del sprint, el base handler (`UpdateAuditableEntity`) setea `FechaActualizacion = DateTime.UtcNow` ANTES de llamar al service. Los siguientes services también lo setean en `Actualizar()`:

| Service | Línea | Método |
|---------|-------|--------|
| `Infrastructure/Repository/AlmacenService.cs` | 104 | `Actualizar()` |
| `Infrastructure/Repository/EmpresaService.cs` | 50 | `Actualizar()` |
| `Infrastructure/Repository/ListaPrecioService.cs` | 44 | `Actualizar()` |
| `Infrastructure/Repository/ProveedorService.cs` | 53 | `Actualizar()` |

**El resultado actual (problema):**
```
1. Handler: base.UpdateAuditableEntity() → entity.FechaActualizacion = DateTime.UtcNow   ← asignación 1
2. Service.Actualizar() → entity.FechaActualizacion = DateTime.UtcNow                     ← asignación 2 (redundante)
3. _context.SaveChangesAsync()
```

La segunda asignación es redundante. No es un bug crítico (el valor es prácticamente idéntico), pero viola la regla de responsabilidad única: la auditoría pertenece al handler, no al service.

**Excepción — NO remover:**
- `CondicionPagoService.ActualizarEstado()` (línea 41) — este método gestiona la entidad internamente (Patrón B). SÍ debe conservar `FechaActualizacion`.

### Solución para Problema 2

Eliminar `FechaActualizacion = DateTime.UtcNow` de estos 4 services:

```csharp
// ❌ ANTES — AlmacenService.Actualizar()
public async Task Actualizar(Almacen almacen, CancellationToken cancellationToken)
{
    almacen.FechaActualizacion = DateTime.UtcNow;   // ← ELIMINAR
    _context.Almacenes.Update(almacen);
    await _context.SaveChangesAsync(cancellationToken);
}

// ✅ DESPUÉS
public async Task Actualizar(Almacen almacen, CancellationToken cancellationToken)
{
    _context.Almacenes.Update(almacen);
    await _context.SaveChangesAsync(cancellationToken);
}
```

---

## 🟡 PROBLEMA 3: null Mapper en handlers ActualizarEstado (Riesgo leve)

Los handlers ActualizarEstado refactorizados pasan `null` para `IMapper`:
```csharp
: base(null, logger)  // ← IMapper es null
```

**Riesgo:** Si en el futuro un handler llama `Mapper.Map()` y se olvidó cambiar `base(null, logger)` → `NullReferenceException` en runtime.

**Sugerencia:** Registrar este patrón como conocido. Si en el futuro se necesita mapeo en un ActualizarEstado handler, pasar el `IMapper` real.

---

## 🛠️ PLAN DE EJECUCIÓN

### Paso 1: Refactorizar 7 handlers Patrón A

Para cada uno:
1. Cambiar herencia a `AuditableUpdateHandlerBase<TCommand, TResponse>`
2. Actualizar usings: `using Application.Handlers;`
3. Constructor: `base(null, logger)` (sin IMapper)
4. Handle: `public override` + usar `UpdateAuditableEntity`
5. NO usar `Mapper.Map()` en estos handlers

**Referencia completada:** `Application/Features/Catalogo/Pais/ActualizarEstado/ActualizarEstadoPaisHandler.cs`

### Paso 2: Limpiar FechaActualizacion de 4 services

Eliminar la línea `FechaActualizacion = DateTime.UtcNow` de:
- `AlmacenService.Actualizar()`
- `EmpresaService.Actualizar()`
- `ListaPrecioService.Actualizar()`
- `ProveedorService.Actualizar()`

**NO tocar:**
- `CondicionPagoService.ActualizarEstado()` — es Patrón B, debe conservarlo

### Paso 3: Validar compilación y testing

```bash
dotnet build → 0 errores
```

Probar endpoints:
- `PATCH /api/v1/almacenes/{id}/inactivar` → FechaActualizacion cambia
- `PATCH /api/v1/empresas/{id}/activar` → FechaActualizacion cambia
- `PATCH /api/v1/categoriaproductos/{id}/inactivar` → FechaActualizacion cambia

---

## 📊 ESTADO DESPUÉS DE EJECUTAR ESTE PENDIENTE

| Componente | Antes | Después |
|-----------|-------|---------|
| ActualizarEstado Patrón A | 7 sin base handler | 7 con base handler |
| ActualizarEstado Patrón B | 2 sin base handler | 2 sin base handler (correcto) |
| Services con FechaActualizacion redundante | 4 redundantes | 0 redundantes |
| Services con FechaActualizacion necesaria | CondicionPago, Proveedor (ActualizarEstado) | CondicionPago (ActualizarEstado) solo |

---

## 🔗 REFERENCIAS

- **IA_Docs/AUDITABLE_ENTITY_UPDATE_HANDLERS_PATTERN.md** — Patrón base
- **IA_Docs/ARCHITECTURE_DECISIONS.md** — ADR-012
- **Handler de referencia (Patrón A):** `Application/Features/Catalogo/Pais/ActualizarEstado/ActualizarEstadoPaisHandler.cs`
- **Handler de referencia (Patrón B — NO refactorizar):** `Application/Features/Comercial/Proveedor/ActualizarEstado/ActualizarEstadoProveedorHandler.cs`

---

**Estado:** Listo para ejecutar cuando Miguel apruebe  
**Prerequisito:** Testing del sprint actual debe pasar primero  
**Impacto:** Bajo riesgo — eliminar redundancias, sin cambio de lógica de negocio
