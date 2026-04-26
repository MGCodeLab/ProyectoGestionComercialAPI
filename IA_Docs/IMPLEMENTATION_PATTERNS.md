# Patrones de Implementación - Referencia Rápida

Cuando agregues un nuevo módulo, sigue estos patrones exactamente.

---

## 📋 Estructura de Carpetas

```
Domain/
  └── {Contexto}/{Entity}.cs
      └── Hereda AuditableEntity

Application/
  ├── Dtos/{Contexto}/
  │   ├── Crear{Entity}Dto.cs
  │   ├── Actualizar{Entity}Dto.cs
  │   └── {Entity}Dto.cs (con audit fields)
  ├── Features/{Contexto}/
  │   ├── Crear/
  │   │   ├── Crear{Entity}Command.cs
  │   │   ├── Crear{Entity}Handler.cs
  │   │   └── Crear{Entity}Validator.cs
  │   ├── Actualizar/...
  │   ├── ActualizarEstado/...
  │   └── Eliminar/...
  ├── Interfaces/
  │   └── I{Entity}Service.cs
  └── Mappings/{Contexto}/
      └── {Entity}Profile.cs

Infrastructure/
  ├── Repository/
  │   └── {Entity}Service.cs
  └── Persistence/Configurations/
      └── {Entity}Configuration.cs

API/
  └── Controllers/
      └── {Entity}sController.cs
```

---

## 🔧 Snippets de Código

### 1. Entity (Domain)

```csharp
namespace Domain.{Contexto}
{
    public class {Entity} : AuditableEntity
    {
        public string Campo1 { get; set; } = string.Empty;
        public decimal Campo2 { get; set; }
        public int ForeignKeyId { get; set; }
        
        // Navigation properties (si hay)
        public virtual OtherEntity OtherNavigation { get; set; }
    }
}
```

### 2. Configuration (Infrastructure)

```csharp
namespace Infrastructure.Persistence.Configurations
{
    public class {Entity}Configuration : AuditableEntityConfiguration<{Entity}>
    {
        public override void Configure(EntityTypeBuilder<{Entity}> builder)
        {
            base.Configure(builder);
            
            // Domain-specific configuration
            builder.Property(e => e.Campo1).HasMaxLength(150).IsRequired();
            builder.Property(e => e.Campo2).HasPrecision(18, 2);
            
            // Foreign keys
            builder.HasOne<OtherEntity>()
                .WithMany()
                .HasForeignKey(e => e.ForeignKeyId);
            
            // Unique constraints
            builder.HasIndex(e => e.Campo1).IsUnique();
        }
    }
}
```

### 3. DTOs (Application)

```csharp
// CrearDto
namespace Application.Dtos.{Contexto}
{
    public class Crear{Entity}Dto
    {
        public string Campo1 { get; set; }
        public decimal Campo2 { get; set; }
    }
}

// ActualizarDto (mismo que Crear)
public class Actualizar{Entity}Dto : Crear{Entity}Dto { }

// Response DTO (incluye audit fields)
public class {Entity}Dto
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }
    public string Campo1 { get; set; }
    public decimal Campo2 { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaRegistro { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}
```

### 4. Command (Application)

```csharp
using MediatR;

namespace Application.Features.{Contexto}.Crear
{
    public record Crear{Entity}Command(
        string Campo1,
        decimal Campo2
    ) : IRequest<int>;  // Retorna ID del nuevo registro
}
```

### 5. Handler (Application)

```csharp
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.{Contexto}.Crear
{
    public class Crear{Entity}Handler : IRequestHandler<Crear{Entity}Command, int>
    {
        private readonly I{Entity}Service _service;
        private readonly IMapper _mapper;
        private readonly ILogger<Crear{Entity}Handler> _logger;

        public Crear{Entity}Handler(
            I{Entity}Service service,
            IMapper mapper,
            ILogger<Crear{Entity}Handler> logger)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> Handle(Crear{Entity}Command request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Crear{Entity}: {@request}", request);
            
            var entity = _mapper.Map<{Entity}>(request);
            var id = await _service.Crear(entity, cancellationToken);
            
            _logger.LogInformation("Crear{Entity}: ID {id}", id);
            return id;
        }
    }
}
```

### 6. Validator (Application)

```csharp
using FluentValidation;

namespace Application.Features.{Contexto}.Crear
{
    public class Crear{Entity}Validator : AbstractValidator<Crear{Entity}Command>
    {
        public Crear{Entity}Validator()
        {
            RuleFor(x => x.Campo1)
                .NotEmpty().WithMessage("Campo1 es requerido")
                .MaximumLength(150).WithMessage("Máximo 150 caracteres");
            
            RuleFor(x => x.Campo2)
                .GreaterThan(0).WithMessage("Campo2 debe ser mayor a 0");
        }
    }
}
```

### 7. Service Interface (Application)

```csharp
namespace Application.Interfaces
{
    public interface I{Entity}Service
    {
        Task<List<{Entity}>> ObtenerTodos(CancellationToken token);
        Task<{Entity}?> ObtenerPorId(int id, bool isAsTracking, CancellationToken token);
        Task<int> Crear({Entity} entity, CancellationToken token);
        Task Actualizar(CancellationToken token);
        Task Eliminar({Entity} entity, CancellationToken token);
    }
}
```

### 8. Service Implementation (Infrastructure)

```csharp
namespace Infrastructure.Repository
{
    public class {Entity}Service : I{Entity}Service
    {
        private readonly AppDbContext _context;

        public {Entity}Service(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<{Entity}>> ObtenerTodos(CancellationToken token)
            => await _context.{Entities}.ToListAsync(token);

        public async Task<{Entity}?> ObtenerPorId(int id, bool isAsTracking, CancellationToken token)
            => (isAsTracking) ?
                await _context.{Entities}.FirstOrDefaultAsync(x => x.Id == id, token) :
                await _context.{Entities}.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, token);

        public async Task<int> Crear({Entity} entity, CancellationToken token)
        {
            _context.{Entities}.Add(entity);
            await _context.SaveChangesAsync(token);
            return entity.Id;
        }

        public async Task Actualizar(CancellationToken token)
            => await _context.SaveChangesAsync(token);

        public async Task Eliminar({Entity} entity, CancellationToken token)
        {
            _context.{Entities}.Remove(entity);
            await _context.SaveChangesAsync(token);
        }
    }
}
```

### 9. AutoMapper Profile (Application)

```csharp
using AutoMapper;

namespace Application.Mappings.{Contexto}
{
    public class {Entity}Profile : Profile
    {
        public {Entity}Profile()
        {
            CreateMap<Crear{Entity}Dto, Crear{Entity}Command>();
            CreateMap<Actualizar{Entity}Dto, Actualizar{Entity}Command>();
            CreateMap<Crear{Entity}Command, {Entity}>();
            CreateMap<{Entity}, {Entity}Dto>();
        }
    }
}
```

### 10. Controller (API)

```csharp
namespace API.GestionComercial.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class {Entities}Controller : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly I{Entity}Service _service;
        private readonly IMediator _mediator;

        public {Entities}Controller(
            IMapper mapper,
            I{Entity}Service service,
            IMediator mediator)
        {
            _mapper = mapper;
            _service = service;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var items = await _service.ObtenerTodos(HttpContext.RequestAborted);
            var result = _mapper.Map<List<{Entity}Dto>>(items);
            return this.OkResponse(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.ObtenerPorId(id, isAsTracking: false, HttpContext.RequestAborted);
            if (item == null)
                return this.NotFoundResponse("{Entity} no encontrado");
            
            var result = _mapper.Map<{Entity}Dto>(item);
            return this.OkResponse(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Crear{Entity}Dto dto)
        {
            var command = _mapper.Map<Crear{Entity}Command>(dto);
            var id = await _mediator.Send(command);
            
            return this.CreatedResponse(
                nameof(GetById),
                new { id },
                new { id, campo1 = dto.Campo1 },
                "{Entity} creado exitosamente");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Actualizar{Entity}Dto dto)
        {
            var command = _mapper.Map<Actualizar{Entity}Command>(dto);
            command = command with { Id = id };
            
            await _mediator.Send(command);
            return this.OkResponse(string.Empty, "{Entity} actualizado correctamente");
        }

        [HttpPatch("{id}/inactivar")]
        public async Task<IActionResult> Inactivar(int id)
        {
            await _mediator.Send(new ActualizarEstado{Entity}Command(id, false));
            return this.OkResponse(string.Empty, "{Entity} inactivado correctamente");
        }

        [HttpPatch("{id}/activar")]
        public async Task<IActionResult> Activar(int id)
        {
            await _mediator.Send(new ActualizarEstado{Entity}Command(id, true));
            return this.OkResponse(string.Empty, "{Entity} activado correctamente");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new Eliminar{Entity}Command(id));
            return this.OkResponse(string.Empty, "{Entity} eliminado correctamente");
        }
    }
}
```

### 11. Program.cs - DI Registration

```csharp
// En Program.cs, agregar:
builder.Services.AddScoped<I{Entity}Service, {Entity}Service>();

// Ya debe existir:
builder.Services.AddApplication();  // Registra MediatR y Validators
builder.Services.AddAutoMapper();   // Registra AutoMapper Profiles
```

### 12. DbContext - Add DbSet

```csharp
// En AppDbContext.cs, agregar:
public DbSet<{Entity}> {Entities} { get; set; }

// En OnModelCreating:
modelBuilder.ApplyConfiguration(new {Entity}Configuration());
```

---

## ✅ Checklist de Implementación

```
[ ] Entity creada en Domain
    [ ] Hereda AuditableEntity
    [ ] Propiedades definidas
    [ ] Navigation properties (si aplica)

[ ] Configuration creada en Infrastructure
    [ ] Hereda AuditableEntityConfiguration<T>
    [ ] Propiedades configuradas
    [ ] Foreign keys definidas
    [ ] Unique constraints definidas

[ ] DTOs creadas en Application/Dtos
    [ ] CrearDto
    [ ] ActualizarDto
    [ ] ResponseDto (con audit fields)

[ ] Commands creadas en Application/Features
    [ ] CrearCommand
    [ ] ActualizarCommand
    [ ] ActualizarEstadoCommand
    [ ] EliminarCommand

[ ] Handlers creados en Application/Features
    [ ] CrearHandler con logging
    [ ] ActualizarHandler
    [ ] ActualizarEstadoHandler
    [ ] EliminarHandler

[ ] Validators creados en Application/Features
    [ ] CrearValidator
    [ ] ActualizarValidator

[ ] Service Interface creada en Application/Interfaces
    [ ] ObtenerTodos
    [ ] ObtenerPorId
    [ ] Crear (retorna int)
    [ ] Actualizar
    [ ] Eliminar

[ ] Service Implementation creada en Infrastructure/Repository
    [ ] Implementa Interface
    [ ] Todos los métodos implementados
    [ ] SaveChangesAsync usado correctamente

[ ] AutoMapper Profile creado en Application/Mappings
    [ ] Dto → Command
    [ ] Command → Entity
    [ ] Entity → ResponseDto

[ ] Controller creado en API/Controllers
    [ ] 6 endpoints implementados (GET list, GET id, POST, PUT, PATCH x2, DELETE)
    [ ] Mapper usado correctamente
    [ ] Service/Mediator usado correctamente
    [ ] Response wrapper utilizado

[ ] DI Registration en Program.cs
    [ ] builder.Services.AddScoped<IService, Service>()

[ ] DbSet en AppDbContext
    [ ] public DbSet<Entity> Entities

[ ] Configuration en AppDbContext.OnModelCreating
    [ ] modelBuilder.ApplyConfiguration(new EntityConfiguration())

[ ] Test data seeded en database
    [ ] INSERT statements ejecutados
    [ ] Datos verificables

[ ] Endpoints testeados
    [ ] GET /api/v1/entities
    [ ] GET /api/v1/entities/{id}
    [ ] POST /api/v1/entities
    [ ] PUT /api/v1/entities/{id}
    [ ] PATCH /api/v1/entities/{id}/inactivar
    [ ] PATCH /api/v1/entities/{id}/activar
    [ ] DELETE /api/v1/entities/{id}

[ ] Commit realizado
    [ ] feat({modulo}): descripcion
    [ ] Incluye Co-Authored-By
```

---

## 🎯 Pasos Rápidos para Nuevo Módulo

1. **Crear Entity en Domain** → Hereda AuditableEntity
2. **Crear Configuration** → Hereda AuditableEntityConfiguration<T>
3. **Crear DTOs** → Crear, Actualizar, Response
4. **Crear Commands** → 4 commands (Crear, Actualizar, ActualizarEstado, Eliminar)
5. **Crear Handlers** → 4 handlers con logging
6. **Crear Validators** → Al menos 2 (CrearValidator, ActualizarValidator)
7. **Crear Service Interface** → 5 métodos
8. **Crear Service Implementation** → Implementa Interface
9. **Crear AutoMapper Profile** → 4 maps mínimo
10. **Crear Controller** → 6 endpoints CRUD + soft delete
11. **DI Registration** → Program.cs + AppDbContext
12. **Test Data** → SQL INSERT
13. **Testing Manual** → CURL todos los endpoints
14. **Commit** → feat(modulo): descripcion

---

## 🚀 Tiempo Estimado

- **Por módulo simple:** 2-3 horas
- **Por módulo con FKs:** 3-4 horas
- **Por módulo con lógica compleja:** 4-6 horas

---

**Última Actualización:** 2026-04-25  
**Versión:** v3.0.0
