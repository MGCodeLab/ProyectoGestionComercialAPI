# 📋 PLAN DETALLADO - Iteración 3: Completar Módulo Cliente

**Fecha:** 2026-04-25  
**Dependencias:** ✅ Iteración 1 + Iteración 2 completadas  
**Tiempo estimado:** 4-5 horas  
**Historia de cambios:** `History Changed/20260425_T15XX_CompleteClienteModule/`

---

## 🎯 Objetivo

Implementar **CRUD completo para Cliente** siguiendo arquitectura Clean + CQRS pragmático ya establecido.

Requisito: Cliente debe ser **tan funcional como Producto**, pero aprovechando los patrones mejorados de Iteraciones 1 y 2.

---

## 📂 Estructura de Archivos a Crear/Modificar

### A. APPLICATION LAYER

#### 1. Application/Dtos/Cliente/ (DTOs con validaciones)

**Crear:**
```
Application/Dtos/Cliente/
├── CrearClienteDto.cs
├── ActualizarClienteDto.cs
└── ClienteDto.cs (para responses)
```

**CrearClienteDto:**
```csharp
public class CrearClienteDto
{
    public int TipoDocumentoId { get; set; }
    public required string NumeroDocumento { get; set; }
    public required string Nombres { get; set; }
    public required string ApellidoPaterno { get; set; }
    public string? ApellidoMaterno { get; set; }
    public string? Correo { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
}
```

**ActualizarClienteDto:**
```csharp
public class ActualizarClienteDto
{
    public int TipoDocumentoId { get; set; }
    public required string NumeroDocumento { get; set; }
    public required string Nombres { get; set; }
    public required string ApellidoPaterno { get; set; }
    public string? ApellidoMaterno { get; set; }
    public string? Correo { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
}
```

**ClienteDto (response):**
```csharp
public class ClienteDto
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }
    public int TipoDocumentoId { get; set; }
    public string NumeroDocumento { get; set; }
    public string Nombres { get; set; }
    public string ApellidoPaterno { get; set; }
    public string? ApellidoMaterno { get; set; }
    public string? Correo { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public string NombreCompleto { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaRegistro { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}
```

---

#### 2. Application/Features/Clientes/ (Commands + Handlers + Validators)

**Estructura:**
```
Application/Features/Clientes/
├── Crear/
│   ├── CrearClienteCommand.cs
│   ├── CrearClienteHandler.cs
│   └── CrearClienteValidator.cs
├── Actualizar/
│   ├── ActualizarClienteCommand.cs
│   ├── ActualizarClienteHandler.cs
│   └── ActualizarClienteValidator.cs
├── ActualizarEstado/
│   ├── ActualizarEstadoClienteCommand.cs
│   └── ActualizarEstadoClienteHandler.cs
└── Eliminar/
    ├── EliminarClienteCommand.cs
    └── EliminarClienteHandler.cs
```

**Commands Pattern (mismo que Producto):**
```csharp
// CrearClienteCommand
public record CrearClienteCommand(
    int TipoDocumentoId,
    string NumeroDocumento,
    string Nombres,
    string ApellidoPaterno,
    string? ApellidoMaterno,
    string? Correo,
    string? Telefono,
    string? Direccion
) : IRequest<int>;

// ActualizarClienteCommand
public record ActualizarClienteCommand(
    int Id,
    int TipoDocumentoId,
    string NumeroDocumento,
    string Nombres,
    string ApellidoPaterno,
    string? ApellidoMaterno,
    string? Correo,
    string? Telefono,
    string? Direccion
) : IRequest<Unit>;

// ActualizarEstadoClienteCommand
public record ActualizarEstadoClienteCommand(int Id, bool Activo) : IRequest<Unit>;

// EliminarClienteCommand
public record EliminarClienteCommand(int Id) : IRequest<Unit>;
```

**Handlers:**
- CrearClienteHandler: Mapear DTO → Cliente, validar, guardar
- ActualizarClienteHandler: Buscar, actualizar campos, guardar
- ActualizarEstadoClienteHandler: Buscar, set Activo, guardar
- EliminarClienteHandler: Buscar, soft delete (Activo=false), guardar

**Validators:**
- CrearClienteValidator: Validar campos requeridos, formato email, etc
- ActualizarClienteValidator: Mismo como Create
- ActualizarEstadoClienteValidator: Validar que Id existe

---

#### 3. Application/Mappings/Clientes/ (AutoMapper profiles)

**Crear:**
```
Application/Mappings/Clientes/
└── ClienteProfile.cs
```

**ClienteProfile:**
```csharp
public class ClienteProfile : Profile
{
    public ClienteProfile()
    {
        CreateMap<CrearClienteDto, CrearClienteCommand>();
        CreateMap<ActualizarClienteDto, ActualizarClienteCommand>();
        CreateMap<CrearClienteCommand, Cliente>();
        CreateMap<Cliente, ClienteDto>();
    }
}
```

---

#### 4. Refactor: IClienteService

**Ubicación:** `Application/Interfaces/IClienteService.cs` (crear si no existe)

```csharp
public interface IClienteService
{
    Task<List<Cliente>> ObtenerTodos(CancellationToken token);
    Task<Cliente?> ObtenerPorId(int id, bool isAsTracking, CancellationToken token);
    Task<int> Crear(Cliente cliente, CancellationToken token);
    Task Actualizar(CancellationToken token);
    Task Eliminar(Cliente cliente, CancellationToken token);
}
```

---

### B. INFRASTRUCTURE LAYER

#### 1. Implementar ClienteService

**Ubicación:** `Infrastructure/Repository/ClienteService.cs` (ya existe, completar)

```csharp
public class ClienteService : IClienteService
{
    private readonly AppDbContext _context;

    public ClienteService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Cliente>> ObtenerTodos(CancellationToken token)
        => await _context.Clientes.ToListAsync(token);

    public async Task<Cliente?> ObtenerPorId(int id, bool isAsTracking, CancellationToken token) 
        => (isAsTracking) ?
            await _context.Clientes.FirstOrDefaultAsync(x => x.Id == id, token) :
            await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, token);

    public async Task<int> Crear(Cliente cliente, CancellationToken token)
    {
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync(token);
        return cliente.Id;
    }

    public async Task Actualizar(CancellationToken token)
        => await _context.SaveChangesAsync(token);
    
    public async Task Eliminar(Cliente cliente, CancellationToken token)
    {
        _context.Clientes.Remove(cliente);
        await _context.SaveChangesAsync(token);
    }
}
```

---

### C. API LAYER

#### 1. GestionComercial/Controllers/ClientesController.cs

**Crear nuevo controller:**

```csharp
[ApiController]
[Route("api/v1/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IClienteService _service;
    private readonly IMediator _mediator;

    public ClientesController(
        IMapper mapper,
        IClienteService clienteService,
        IMediator mediator)
    {
        _mapper = mapper;
        _service = clienteService;
        _mediator = mediator;
    }

    // GET /api/v1/clientes
    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        var clientes = await _service.ObtenerTodos(HttpContext.RequestAborted);
        var result = _mapper.Map<List<ClienteDto>>(clientes);
        return this.OkResponse(result);
    }

    // GET /api/v1/clientes/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var cliente = await _service.ObtenerPorId(id, isAsTracking: false, HttpContext.RequestAborted);

        if (cliente == null)
            return this.NotFoundResponse("Cliente no encontrado");

        var result = _mapper.Map<ClienteDto>(cliente);
        return this.OkResponse(result);
    }

    // POST /api/v1/clientes
    [HttpPost]
    public async Task<IActionResult> CreateCliente(CrearClienteDto dto)
    {
        var command = _mapper.Map<CrearClienteCommand>(dto);
        var id = await _mediator.Send(command);

        var result = new ClienteDto { Id = id, Nombres = dto.Nombres };

        return this.CreatedResponse(
            nameof(GetById),
            new { id },
            result,
            "Cliente creado exitosamente");
    }

    // PUT /api/v1/clientes/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ActualizarClienteDto dto)
    {
        var command = _mapper.Map<ActualizarClienteCommand>(dto);
        command = command with { Id = id };

        await _mediator.Send(command);

        return this.OkResponse(string.Empty, "Cliente actualizado correctamente");
    }

    // DELETE /api/v1/clientes/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new EliminarClienteCommand(id));
        return this.OkResponse(string.Empty, "Cliente eliminado correctamente");
    }

    // PATCH /api/v1/clientes/{id}/inactivar
    [HttpPatch("{id}/inactivar")]
    public async Task<IActionResult> Inactivar(int id)
    {
        await _mediator.Send(new ActualizarEstadoClienteCommand(id, false));
        return this.OkResponse(string.Empty, "Cliente inactivado correctamente");
    }

    // PATCH /api/v1/clientes/{id}/activar
    [HttpPatch("{id}/activar")]
    public async Task<IActionResult> Activar(int id)
    {
        await _mediator.Send(new ActualizarEstadoClienteCommand(id, true));
        return this.OkResponse(string.Empty, "Cliente activado correctamente");
    }
}
```

---

## 🔄 Cambios en Dependencias Existentes

### 1. Actualizar Program.cs (DependencyInjection)

Agregar registro de ClienteService (si no existe):
```csharp
builder.Services.AddScoped<IClienteService, ClienteService>();
```

(Nota: Probablemente ya existe en Program.cs actual)

---

## 📊 Comparación Cliente vs Producto

| Aspecto | Producto | Cliente |
|---------|----------|---------|
| **Entity** | Simple (Nombre, Descripcion, Precio) | Complejo (múltiples campos + FK) |
| **DTOs** | CrearProductoDto, ActualizarProductoDto | CrearClienteDto, ActualizarClienteDto |
| **Commands** | 4 (Crear, Actualizar, ActualizarEstado, Eliminar) | 4 (igual) |
| **Validaciones** | Nombre required, Precio > 0 | NumeroDocumento unique, Email format, etc |
| **Controller** | ProductosController | ClientesController (NUEVA) |
| **Soft Delete** | Via Activo field + global filter ✅ | Via Activo field + global filter ✅ |

---

## ✅ Checklist de Implementación

- [ ] Crear DTOs: CrearClienteDto, ActualizarClienteDto, ClienteDto
- [ ] Crear Commands: CrearClienteCommand, ActualizarClienteCommand, ActualizarEstadoClienteCommand, EliminarClienteCommand
- [ ] Crear Handlers: CrearClienteHandler, ActualizarClienteHandler, ActualizarEstadoClienteHandler, EliminarClienteHandler
- [ ] Crear Validators: CrearClienteValidator, ActualizarClienteValidator, ActualizarEstadoClienteValidator, EliminarClienteValidator
- [ ] Crear AutoMapper Profile: ClienteProfile
- [ ] Implementar ClienteService (CRUD completo)
- [ ] Crear ClientesController con endpoints CRUD
- [ ] Verificar que IClienteService esté registrado en DependencyInjection
- [ ] Build: ✅ Sin errores
- [ ] Documentar cambios en History Changed/

---

## 🧪 Testing (Manual)

Una vez implementado, probar:

1. **GET /api/v1/clientes** → Listar activos (soft delete aplicado)
2. **GET /api/v1/clientes/{id}** → Obtener por ID
3. **POST /api/v1/clientes** → Crear (validaciones)
4. **PUT /api/v1/clientes/{id}** → Actualizar
5. **PATCH /api/v1/clientes/{id}/inactivar** → Soft delete
6. **PATCH /api/v1/clientes/{id}/activar** → Reactivar
7. **DELETE /api/v1/clientes/{id}** → Eliminar (hard delete)

---

## 📝 Historia de Cambios

**Commit Message:**
```
feat(cliente): complete cliente module with full CRUD

- Create CrearClienteCommand, ActualizarClienteCommand, ActualizarEstadoClienteCommand, EliminarClienteCommand
- Implement handlers + validators for all cliente operations
- Create ClienteDto, CrearClienteDto, ActualizarClienteDto with validations
- Implement ClienteService with full CRUD
- Create ClientesController with endpoints (GET, POST, PUT, PATCH, DELETE)
- Add ClienteProfile for AutoMapper
- Integrate with global soft delete filter (Iteración 2)
- Cliente module now feature-complete and production-ready
```

---

## ⚡ Notas Importantes

### 1. Soft Delete ya funciona
Cliente hereda de AuditableEntity → Activo field → global filter automático ✅

### 2. Validaciones importantes
- NumeroDocumento + TipoDocumentoId deben ser únicos (índice en DB)
- Correo debe ser único (si se proporciona)
- Email debe tener formato válido

### 3. No repetir código
- Handlers son similares a ProductoHandlers → OK (DRY respetado en patrones, no en contenido)
- Validators son simples → OK

### 4. Testing
Con soft delete global filter, verificar que:
- `GetList()` solo retorna clientes activos
- `IncludeSoftDeleted()` retorna todos (si se usa en admin)

---

**¿Aprobado este plan?**

Si hay cambios deseados, indícalos aquí. Si está OK, procedo con implementación.
