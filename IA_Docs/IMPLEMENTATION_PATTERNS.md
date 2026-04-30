# Patrones de Implementación - Estándar Obligatorio v3.0.0

**IMPORTANTE:** Este documento define el estándar EXACTO para todos los módulos (Producto, Cliente, Ventas, Compras, etc.).
NO inventar patrones nuevos a menos que se establezca un nuevo estándar aquí.
Todos los módulos DEBEN seguir estos patrones idénticos para mantener código prolijo, homogéneo y mantenible.

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

### 3. DTOs (Application) - CON VALIDACIÓN Y DOCUMENTACIÓN

```csharp
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.{Contexto}
{
    /// <summary>
    /// DTO para crear un nuevo {Entity}.
    /// Contiene solo los campos requeridos para la creación.
    /// </summary>
    public class Crear{Entity}Dto
    {
        /// <summary>Descripción del campo 1.</summary>
        [Required(ErrorMessage = "Campo1 es obligatorio")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Campo1 debe tener entre 3 y 150 caracteres")]
        public required string Campo1 { get; set; }

        /// <summary>Descripción del campo 2 (decimal).</summary>
        [Range(typeof(decimal), "0.01", "999999.99", ErrorMessage = "Campo2 debe estar entre 0.01 y 999999.99")]
        public decimal Campo2 { get; set; }
    }

    /// <summary>
    /// DTO para actualizar un {Entity} existente.
    /// Contiene los mismos campos que Crear (sin audit info).
    /// </summary>
    public class Actualizar{Entity}Dto
    {
        /// <summary>Nuevo valor de Campo1.</summary>
        [Required(ErrorMessage = "Campo1 es obligatorio")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Campo1 debe tener entre 3 y 150 caracteres")]
        public required string Campo1 { get; set; }

        /// <summary>Nuevo valor de Campo2.</summary>
        [Range(typeof(decimal), "0.01", "999999.99", ErrorMessage = "Campo2 debe estar entre 0.01 y 999999.99")]
        public decimal Campo2 { get; set; }
    }

    /// <summary>
    /// DTO de respuesta para {Entity}.
    /// Incluye todos los campos incluyendo auditoría (Id, PublicId, Activo, fechas).
    /// </summary>
    public class {Entity}Dto
    {
        /// <summary>Identificador único interno.</summary>
        public int Id { get; set; }

        /// <summary>Identificador público único (GUID).</summary>
        public Guid PublicId { get; set; }

        /// <summary>Campo 1 del {Entity}.</summary>
        public string Campo1 { get; set; } = string.Empty;

        /// <summary>Campo 2 del {Entity}.</summary>
        public decimal Campo2 { get; set; }

        /// <summary>Indica si el {Entity} está activo.</summary>
        public bool Activo { get; set; }

        /// <summary>Fecha de creación del registro (UTC).</summary>
        public DateTime FechaRegistro { get; set; }

        /// <summary>Fecha de última actualización (UTC, nullable).</summary>
        public DateTime? FechaActualizacion { get; set; }
    }
}
```

**REGLAS OBLIGATORIAS PARA DTOs:**
1. ✅ XMLdoc comments (`///`) en clase y cada propiedad
2. ✅ Validación con `[Required]`, `[StringLength]`, `[Range]`, `[EmailAddress]`, etc.
3. ✅ `required` modifier en propiedades obligatorias de Crear/Actualizar
4. ✅ Response DTO SIEMPRE incluye Id, PublicId, Activo, FechaRegistro, FechaActualizacion
5. ✅ Mensajes de validación en ESPAÑOL

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

### 5. Handlers (Application) - ESTÁNDAR CON MAPPER

**CrearHandler:**
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

**ActualizarHandler:**
```csharp
using Application.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.{Contexto}.Actualizar
{
    public class Actualizar{Entity}Handler : IRequestHandler<Actualizar{Entity}Command, Unit>
    {
        private readonly I{Entity}Service _service;
        private readonly IMapper _mapper;
        private readonly ILogger<Actualizar{Entity}Handler> _logger;

        public Actualizar{Entity}Handler(
            I{Entity}Service service,
            IMapper mapper,
            ILogger<Actualizar{Entity}Handler> _logger)
        {
            _service = service;
            _mapper = mapper;
            _logger = _logger;
        }

        public async Task<Unit> Handle(Actualizar{Entity}Command request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Actualizando {Entity} {Id}", nameof({Entity}), request.Id);

            var entity = await _service.ObtenerPorId(request.Id, isAsTracking: true, cancellationToken);

            if (entity == null)
                throw new NotFoundException($"{Entity} con id {request.Id} no encontrado");

            // IMPORTANTE: Usar mapper.Map(source, destination) para aplicar cambios
            _mapper.Map(request, entity);
            entity.FechaActualizacion = DateTime.UtcNow;  // Siempre actualizar fecha

            await _service.Actualizar(cancellationToken);

            _logger.LogInformation("{Entity} {Id} actualizado correctamente", nameof({Entity}), request.Id);
            return Unit.Value;
        }
    }
}
```

**REGLAS OBLIGATORIAS PARA HANDLERS:**
1. ✅ CrearHandler: `_mapper.Map<{Entity}>(request)` → Crea entity desde Command
2. ✅ ActualizarHandler: `_mapper.Map(request, entity)` → Aplica cambios al entity existente
3. ✅ ActualizarHandler: SIEMPRE `entity.FechaActualizacion = DateTime.UtcNow;`
4. ✅ Logging en INFO level con información relevante
5. ✅ Validación y excepciones antes de procesar

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

### 9. AutoMapper Profile (Application) - ESTÁNDAR OBLIGATORIO

```csharp
using AutoMapper;

namespace Application.Mappings.{Contexto}
{
    public class {Entity}Profile : Profile
    {
        public {Entity}Profile()
        {
            // Response DTO ↔ Entity (bidireccional)
            CreateMap<{Entity}, {Entity}Dto>().ReverseMap();

            // Crear (DTO → Command → Entity)
            CreateMap<Crear{Entity}Dto, Crear{Entity}Command>();
            CreateMap<Crear{Entity}Dto, {Entity}>();
            CreateMap<Crear{Entity}Command, {Entity}>();

            // Actualizar (DTO → Command ↔ Entity)
            CreateMap<Actualizar{Entity}Dto, {Entity}>();
            CreateMap<Actualizar{Entity}Dto, Actualizar{Entity}Command>();
            CreateMap<Actualizar{Entity}Command, {Entity}>().ReverseMap();
        }
    }
}
```

**REGLAS OBLIGATORIAS PARA AutoMapper:**
1. ✅ `CreateMap<{Entity}, {Entity}Dto>().ReverseMap();` - SIEMPRE bidireccional
2. ✅ `CreateMap<CrearCommand, {Entity}>();` - Command → Entity
3. ✅ `CreateMap<ActualizarCommand, {Entity}>().ReverseMap();` - SIEMPRE bidireccional en actualizar
4. ✅ `CreateMap<CrearDto, {Entity}>();` - Para CreateHandler mapper
5. ✅ `CreateMap<ActualizarDto, {Entity}>();` - Para ActualizarHandler mapper

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

---

## 🔐 REGLAS DE ORO - NO NEGOCIABLES

Estas reglas garantizan consistencia, mantenibilidad y código prolijo:

### Dominio (Domain)
1. ✅ **TODA** entidad hereda `AuditableEntity` (sin excepciones)
2. ✅ Propiedades con valores por defecto: `public string Nombre { get; set; } = string.Empty;`
3. ✅ Navigation properties como `public virtual Entity Relation { get; set; } = null!;`

### Configuración (Infrastructure.Persistence.Configurations)
1. ✅ **TODA** configuración hereda `AuditableEntityConfiguration<T>`
2. ✅ Llamar `base.Configure(builder);` como primera línea
3. ✅ Foreign keys con `OnDelete(DeleteBehavior.Restrict)` (no borrar entidades relacionadas sin validar)
4. ✅ Unique constraints explícitas donde corresponda

### DTOs (Application.Dtos)
1. ✅ XMLdoc comments en clase y **todas** las propiedades
2. ✅ Validación con `[Required]`, `[StringLength]`, `[Range]`, etc.
3. ✅ `required` keyword en campos obligatorios
4. ✅ Response DTO **siempre** incluye: Id, PublicId, Activo, FechaRegistro, FechaActualizacion
5. ✅ Mensajes de error en **ESPAÑOL**

### Commands (Application.Features)
1. ✅ `public record {Operation}{Entity}Command(...) : IRequest<T>;`
2. ✅ Crear devuelve `IRequest<int>` (ID del nuevo registro)
3. ✅ Actualizar devuelve `IRequest<Unit>` (sin retorno)
4. ✅ ActualizarEstado devuelve `IRequest<Unit>`
5. ✅ Eliminar devuelve `IRequest<Unit>`

### Handlers (Application.Features)
1. ✅ **CREAR:** `_mapper.Map<{Entity}>(request)` → Crea entity desde Command
2. ✅ **ACTUALIZAR:** `_mapper.Map(request, entity)` → Aplica cambios, luego `FechaActualizacion = DateTime.UtcNow`
3. ✅ Logging en INFO level con información relevante
4. ✅ Validaciones y excepciones **antes** de modificar estado
5. ✅ Inyectar: IService, IMapper, ILogger (en ese orden)

### AutoMapper (Application.Mappings)
1. ✅ `CreateMap<{Entity}, {Entity}Dto>().ReverseMap();` - BIDIRECCIONAL
2. ✅ `CreateMap<Crear{Entity}Command, {Entity}>();`
3. ✅ `CreateMap<Actualizar{Entity}Command, {Entity}>().ReverseMap();` - BIDIRECCIONAL
4. ✅ `CreateMap<Crear{Entity}Dto, {Entity}>();` - Para que CrearHandler pueda usar mapper
5. ✅ `CreateMap<Actualizar{Entity}Dto, {Entity}>();` - Para que ActualizarHandler pueda usar mapper

### Validators (Application.Features)
1. ✅ Heredar `AbstractValidator<{Operation}{Entity}Command>`
2. ✅ Mensajes en **ESPAÑOL**
3. ✅ Validar todos los campos en RuleFor()
4. ✅ Validators se registran automáticamente via `builder.Services.AddApplication()`

### Service Interface (Application.Interfaces)
1. ✅ `Task<List<{Entity}>> ObtenerTodos(CancellationToken token);`
2. ✅ `Task<{Entity}?> ObtenerPorId(int id, bool isAsTracking, CancellationToken token);`
3. ✅ `Task<int> Crear({Entity} entity, CancellationToken token);` → Retorna ID
4. ✅ `Task Actualizar(CancellationToken token);` → Usa SaveChangesAsync del DbContext
5. ✅ `Task Eliminar({Entity} entity, CancellationToken token);`

### Service Implementation (Infrastructure.Repository)
1. ✅ Inyectar solo `AppDbContext`
2. ✅ `ObtenerPorId` usa `isAsTracking` para AsNoTracking() o no
3. ✅ `Crear` retorna `entity.Id` después de SaveChangesAsync
4. ✅ `Actualizar` solo hace SaveChangesAsync (cambios ya aplicados via mapper en Handler)
5. ✅ Usar async/await correctamente

### Controller (API.Controllers)
1. ✅ 6 endpoints CRUD: GET list, GET id, POST, PUT, PATCH inactivar, PATCH activar, DELETE
2. ✅ Usar `this.OkResponse()`, `this.NotFoundResponse()`, etc. (ControllerExtensions)
3. ✅ Mediator para Commands (Crear, Actualizar, ActualizarEstado, Eliminar)
4. ✅ Service directo para Queries (ObtenerTodos, ObtenerPorId)
5. ✅ Mapper para convertir Command → Entity, Entity → Dto
6. ✅ Logging no obligatorio en Controller (ya está en Handler)

### DI Registration (Program.cs)
1. ✅ `builder.Services.AddScoped<I{Entity}Service, {Entity}Service>();`
2. ✅ Ya existe: `builder.Services.AddApplication();` (registra MediatR, Validators, AutoMapper)

### DbContext (Infrastructure.Persistence)
1. ✅ `public DbSet<{Entity}> {Entities} { get; set; }`
2. ✅ `modelBuilder.ApplyConfiguration(new {Entity}Configuration());`

### Database (Database/02_Tablas)
1. ✅ Crear tabla con **todas** las columnas de auditoría: PublicId, FechaRegistro, FechaActualizacion, Activo
2. ✅ PublicId: `UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID()`
3. ✅ FechaRegistro: `DATETIME2 NOT NULL DEFAULT GETUTCDATE()`
4. ✅ Activo: `BIT NOT NULL DEFAULT 1`
5. ✅ FechaActualizacion: `DATETIME2 NULL`

---

## ✅ Verificación de Consistencia

Antes de hacer commit, verificar:

```
CÓDIGO PROLIJO:
[ ] Entity: Hereda AuditableEntity con propiedades bien documentadas
[ ] Configuration: Hereda AuditableEntityConfiguration<T>
[ ] DTOs: Todos con XMLdoc y validación [Required], [StringLength], etc.
[ ] Commands: Sintaxis record, tipos de retorno correctos
[ ] Handlers: Usando mapper.Map correctamente, logging en INFO
[ ] Validators: Mensajes en español
[ ] Service: Interface clara, implementation simple
[ ] AutoMapper: Mappings bidireccionales donde corresponda
[ ] Controller: 6 endpoints CRUD, response wrappers usados
[ ] DI: Registrado en Program.cs
[ ] DB: Script con todas las columnas de auditoría

CONSISTENCIA CON OTROS MÓDULOS:
[ ] Structure de carpetas igual a Producto/Cliente
[ ] Patrones de mapeo idénticos a ProductoProfile
[ ] Handlers siguen mismo patrón que ProductoHandler/ClienteHandler
[ ] DTOs tienen misma estructura de validación
[ ] Service interface tiene mismos 5 métodos
[ ] Controller tiene mismos 6 endpoints

HOMOGENEIDAD:
[ ] Ningún patrón inventado "por esta vez"
[ ] Nombres de clases consistentes con convención
[ ] Logging con mismo formato en todas partes
[ ] Mensajes de validación en español
[ ] Excepciones NotFoundException usadas consistentemente
```

---

**Última Actualización:** 2026-04-27  
**Versión:** v3.0.0  
**Estado:** ESTÁNDAR OBLIGATORIO - Todos los módulos futuros deben seguir exactamente estos patrones
