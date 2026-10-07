# Auditoría de Homogeneidad - Hallazgos Técnicos Pendientes

**Fecha:** 2026-06-27  
**Auditoría:** Completa - Nexus Backend Architect  
**Total Issues:** 15 (4 Críticos, 5 Altos, 5 Medios, 1 Bajo)  
**Reporte Detallado:** `.claude/execution-status/code-homogeneity-review-2026-06-27.md`

---

## TAREAS PENDIENTES CRÍTICAS

### TAREA-1: FechaActualizacion en 9 Actualizar Handlers

**Prioridad:** P0 - CRÍTICO  
**Módulos afectados:** CategoriaProducto, CondicionPago, ListaPrecio, MarcaProducto, TipoDocumento, Proveedor, Almacen, Empresa, Sucursal

**Acción:** Agregar `entity.FechaActualizacion = DateTime.UtcNow;` ANTES de `await _service.Actualizar()`

```csharp
// Template para todos los Actualizar Handlers
_mapper.Map(request, entity);
entity.FechaActualizacion = DateTime.UtcNow;  // ← AGREGAR ESTA LÍNEA
await _service.Actualizar(cancellationToken);
```

**Archivos:**
- [ ] ActualizarCategoriaProductoHandler.cs
- [ ] ActualizarCondicionPagoHandler.cs
- [ ] ActualizarListaPrecioHandler.cs
- [ ] ActualizarMarcaProductoHandler.cs
- [ ] ActualizarTipoDocumentoHandler.cs
- [ ] ActualizarProveedorHandler.cs
- [ ] ActualizarAlmacenHandler.cs
- [ ] ActualizarEmpresaHandler.cs
- [ ] ActualizarSucursalHandler.cs

---

### TAREA-2: Mapeo manual → AutoMapper en 5 Handlers

**Prioridad:** P0 - CRÍTICO  
**Módulos afectados:** CategoriaProducto, MarcaProducto, TipoDocumento

**Acción:** Reemplazar `new Entity {}` y asignaciones manuales por `_mapper.Map()`

#### Crear Handlers (2):
- [ ] `CrearCategoriaProductoHandler.cs:30` → `var categoria = _mapper.Map<Domain.Catalogo.CategoriaProducto>(command);`
- [ ] `CrearMarcaProductoHandler.cs:18` → `var marca = _mapper.Map<Domain.Catalogo.MarcaProducto>(command);`

#### Actualizar Handlers (3):
- [ ] `ActualizarCategoriaProductoHandler.cs:34` → `_mapper.Map(request, entity);`
- [ ] `ActualizarMarcaProductoHandler.cs:25` → `_mapper.Map(request, entity);`
- [ ] `ActualizarTipoDocumentoHandler.cs:25` → `_mapper.Map(request, entity);`

---

### TAREA-3: Agregar ILogger en 9 Handlers

**Prioridad:** P0 - CRÍTICO  
**Módulos afectados:** CategoriaProducto (4), MarcaProducto (4), SerieDocumento (1)

**Acción:** Inyectar `ILogger<T>` en constructor y agregar logging INFO

```csharp
// Template de inyección
public class Crear{Entity}Handler : IRequestHandler<Crear{Entity}Command, int>
{
    private readonly ILogger<Crear{Entity}Handler> _logger;  // ← AGREGAR

    public Crear{Entity}Handler(
        I{Entity}Service service,
        IMapper mapper,
        ILogger<Crear{Entity}Handler> logger)  // ← AGREGAR PARÁMETRO
    {
        _logger = logger;  // ← GUARDAR
    }

    public async Task<int> Handle(Crear{Entity}Command request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Crear{Entity}: {@request}", request);  // ← AGREGAR
        
        // ... lógica ...
        
        _logger.LogInformation("Crear{Entity}: ID {id}", id);  // ← AGREGAR
        return id;
    }
}
```

**Handlers a corregir:**
- [ ] CrearCategoriaProductoHandler.cs
- [ ] ActualizarCategoriaProductoHandler.cs
- [ ] ActualizarEstadoCategoriaProductoHandler.cs
- [ ] EliminarCategoriaProductoHandler.cs
- [ ] CrearMarcaProductoHandler.cs
- [ ] ActualizarMarcaProductoHandler.cs
- [ ] ActualizarEstadoMarcaProductoHandler.cs
- [ ] EliminarMarcaProductoHandler.cs
- [ ] ObtenerProximoNumeroHandler.cs (SerieDocumento)

---

### TAREA-4: Completar AutoMapper Profiles (3 archivos)

**Prioridad:** P0 - CRÍTICO  
**Módulos afectados:** ParametroSistema, TipoDocumento, UnidadMedida

#### ParametroSistemaProfile.cs
```csharp
// AGREGAR en constructor:
CreateMap<ActualizarParametroSistemaDto, ActualizarParametroSistemaCommand>();
CreateMap<ActualizarParametroSistemaCommand, ParametroSistema>().ReverseMap();
```

#### TipoDocumentoProfile.cs
```csharp
// AGREGAR en constructor:
CreateMap<ActualizarTipoDocumentoCommand, TipoDocumento>().ReverseMap();
```

#### UnidadMedidaProfile.cs
```csharp
// CAMBIAR línea 16 de:
CreateMap<ActualizarUnidadMedidaCommand, UnidadMedida>();

// A:
CreateMap<ActualizarUnidadMedidaCommand, UnidadMedida>().ReverseMap();
```

---

## TAREAS PENDIENTES ALTAS

### TAREA-5: Corregir ObtenerPorId en 4 Services

**Prioridad:** P1 - ALTO  
**Módulos afectados:** Moneda, ModuloSistema, ParametroSistema

**Acción:** Aplicar patrón ternario para respetar parámetro `isAsTracking`

```csharp
// PATRÓN CORRECTO:
public async Task<{Entity}?> ObtenerPorId(int id, bool isAsTracking, CancellationToken token)
    => isAsTracking
        ? await _context.{Entities}.FirstOrDefaultAsync(x => x.Id == id, token)
        : await _context.{Entities}.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, token);
```

**Archivos:**
- [ ] MonedaService.cs (línea 14)
- [ ] ModuloSistemaService.cs (línea 16)
- [ ] ParametroSistemaService.cs (línea 7)
- [ ] Referencia correcta: TipoDocumentoService.cs (línea 17-20)

---

### TAREA-6: Agregar .AsNoTracking() en ObtenerTodos

**Prioridad:** P1 - ALTO  
**Módulos afectados:** CondicionPago

**Acción:** Agregar `.AsNoTracking()` en CondicionPagoService.ObtenerTodos()

```csharp
// CAMBIAR de:
public async Task<List<CondicionPago>> ObtenerTodos(CancellationToken token) 
    => await _context.CondicionesPago.ToListAsync(token);

// A:
public async Task<List<CondicionPago>> ObtenerTodos(CancellationToken token) 
    => await _context.CondicionesPago.AsNoTracking().ToListAsync(token);
```

---

## TAREAS PENDIENTES MEDIAS

### TAREA-7: Refactorizar MonedasController.Create()

**Prioridad:** P2 - MEDIO  
**Módulo:** Moneda

**Acción:** Usar mapper en lugar de construir DTO manualmente

**Archivo:** `GestionComercial/Controllers/MonedasController.cs` línea 59

```csharp
// CAMBIAR construcción manual por mapper:
var moneda = await _service.ObtenerPorId(id, isAsTracking: false, HttpContext.RequestAborted);
var result = _mapper.Map<MonedaDto>(moneda);
return this.CreatedResponse(nameof(GetById), new { id }, result, "Moneda creada exitosamente");
```

---

### TAREA-8: Reformatear ParametroSistemaService

**Prioridad:** P2 - MEDIO  
**Módulo:** ParametroSistema

**Acción:** Desminificar clase (está en una línea)

**Archivo:** `Infrastructure/Repository/ParametroSistemaService.cs`

Separar en múltiples líneas con indentación estándar para legibilidad.

---

## CHECKLIST DE IMPLEMENTACIÓN

### Fase 1: CRÍTICO (2-3 horas)
- [ ] Agregar FechaActualizacion en 9 handlers (TAREA-1)
- [ ] Reemplazar mapeo manual en 5 handlers (TAREA-2)
- [ ] Completar 3 AutoMapper Profiles (TAREA-4)
- [ ] Agregar ILogger en 9 handlers (TAREA-3)
- [ ] Compilar y verificar sin errores
- [ ] Unit tests: FechaActualizacion, Logger calls, Mapper usage

### Fase 2: ALTO (1-2 horas)
- [ ] Corregir ObtenerPorId en 4 Services (TAREA-5)
- [ ] Agregar AsNoTracking en CondicionPago (TAREA-6)
- [ ] Compilar y unit tests

### Fase 3: MEDIO (1 hora)
- [ ] Refactorizar MonedasController (TAREA-7)
- [ ] Reformatear ParametroSistemaService (TAREA-8)
- [ ] Compilar y unit tests

### Fase 4: Verificación Final
- [ ] Re-ejecutar auditoría automática
- [ ] Verificar que todos los issues están resueltos
- [ ] Integration tests end-to-end
- [ ] Commit y PR

---

## NOTAS DE IMPLEMENTACIÓN

### Patrón Crítico: FechaActualizacion
OBLIGATORIO en TODOS los Actualizar Handlers:
```csharp
entity.FechaActualizacion = DateTime.UtcNow;
await _service.Actualizar(cancellationToken);
```

**Por qué:** Trazabilidad de auditoría. Cada actualización debe registrarse con timestamp.

### Patrón Crítico: Mapper en Handlers
```csharp
// Crear:
var entity = _mapper.Map<Entity>(command);

// Actualizar:
_mapper.Map(request, entity);
```

**Por qué:** Separación de responsabilidades. El mapper centraliza transformación DTO→Entity.

### Patrón Crítico: Logger en Handlers
```csharp
_logger.LogInformation("Crear{Entity}: {@request}", request);
_logger.LogInformation("Crear{Entity}: ID {id}", id);
```

**Por qué:** Trazabilidad de ejecución. Essential para auditoría y debugging.

### Patrón Crítico: AsNoTracking
```csharp
// Para queries de lectura (no modifican):
_context.Entity.AsNoTracking().FirstOrDefaultAsync(...)

// Para queries que sí modifican:
_context.Entity.FirstOrDefaultAsync(...)
```

**Por qué:** Performance. EF Core no necesita trackear cambios en queries de lectura.

---

## ESTIMACIÓN DE ESFUERZO

| Fase | Tareas | Duración | Complejidad |
|------|--------|----------|-------------|
| **1 (Crítico)** | 4 tareas | 2-3h | Media |
| **2 (Alto)** | 2 tareas | 1-2h | Baja |
| **3 (Medio)** | 2 tareas | 1h | Baja |
| **Verificación** | Tests + PR | 1h | Baja |
| **TOTAL** | 8 tareas | **5-7h** | **Media** |

---

## REFERENCIAS TÉCNICAS

- **IMPLEMENTATION_PATTERNS.md:** Líneas 257, 268-272, 333-336, 376-379, 385-390
- **ARCHITECTURE_DECISIONS.md:** ADR-003 (Soft Delete), ADR-005 (AuditableEntity)
- **VALIDATOR_SERVICE_PATTERN.md:** Pattern de validación async sin violar Clean Architecture

---

**Creado por:** Nexus-Backend-Architect  
**Fecha de creación:** 2026-06-27  
**Próxima revisión:** Después de implementar Fase 1
