# Sprint 4: Producto Enriquecido (CategoriaProducto, MarcaProducto, ALTER Productos) — ✅ IMPLEMENTADO

**Versión:** 1.1  
**Fecha Especificación:** 2026-05-17  
**Fecha Implementación:** 2026-05-18  
**Duración Real:** ~3.5 horas (mejor que estimado 5-6h)  
**Estado:** ✅ **IMPLEMENTADO — TESTING COMPLETADO**  
**Arquitecto:** Nexus Backend Architect  
**Implementador:** Nexus Fast Builder + Miguel Gonzalez (SQL execution + testing)  
**Rama:** `catalogo-base/sprint_4`

**📋 OBJETIVO:**
Enriquecer catálogo Productos con categorización jerárquica (self-referencia) y marcas, mediante 2 entidades nuevas + alteración segura de tabla Productos (FKs nullable).

---

## 📍 ALCANCE DE IMPLEMENTACIÓN

### Entidades Nuevas (2)
- **CategoriaProducto** → `catalogo.CategoriasProducto` (self-referencia, máx 3 niveles)
- **MarcaProducto** → `catalogo.MarcasProducto` (catálogo simple)

### Migración de Datos (1)
- **ALTER TABLE Productos** → Agregar 3 ForeignKeys nullable (UnidadMedidaId, CategoriaProductoId, MarcaProductoId)

### Artefactos
- **CQRS:** 8 Commands + 8 Handlers + 4 Validators
- **DTOs:** 6 nuevos (3 × 2 entidades)
- **Servicios:** 4 nuevos (2 Services + 2 ValidatorServices)
- **Controllers:** 2 nuevos (14 endpoints totales)
- **Mappings:** 2 nuevos (AutoMapper profiles)
- **Configurations:** 2 nuevas (Entity Configurations)
- **SQL Scripts:** 3 nuevos (2 tablas + 1 FIX migration)
- **Archivos nuevos:** ~18 totales

---

## 🎯 ENTIDADES & ESPECIFICACIÓN TÉCNICA

### 1. CategoriaProducto → `catalogo.CategoriasProducto`

#### Domain Entity
```csharp
namespace Domain.Catalogo
{
    public class CategoriaProducto : AuditableEntity
    {
        public string Nombre { get; set; }                     // 150, obligatorio
        public string? Descripcion { get; set; }               // 500, opcional
        public int? CategoriaPadreId { get; set; }             // FK self-ref (opcional)
        
        // Navigation property para árbol jerárquico
        public virtual CategoriaProducto? CategoriaPadre { get; set; }
        public virtual ICollection<CategoriaProducto> Subcategorias { get; set; } = new List<CategoriaProducto>();
    }
}
```

#### Configuration
```
Schema: catalogo
Table: CategoriasProducto
Constraints:
- PK: Id (INT)
- FK: CategoriaPadreId → CategoriasProducto.Id (RESTRICT, nullable)
- Indices: CategoriaPadreId, Activo, Nombre
- Default: Activo = 1
```

#### DTOs
- `CrearCategoriaProductoDto`: Nombre, Descripcion?, CategoriaPadreId?
- `ActualizarCategoriaProductoDto`: Nombre, Descripcion?, CategoriaPadreId?
- `CategoriaProductoDto`: Full response con PublicId, Subcategorias (árbol)

#### Validaciones Críticas
- **Nombre:** required, max 150 chars
- **CategoriaPadreId:** optional, debe existir si presente
- **Profundidad:** MÁXIMO 3 niveles (Application rule en Handler)
  ```
  Nivel 1: Electrónica
  Nivel 2: Computadoras (padre=1)
  Nivel 3: Laptops (padre=2)
  ❌ Nivel 4: NO PERMITIDO
  ```
- **Ciclos:** No permitir `CategoriaPadre.CategoriaPadreId != self` (prevención de ciclos en actualización)

#### ValidatorService
```csharp
public class CategoriaProductoValidatorService
{
    public async Task<bool> NombreUnicoAsync(string nombre, int? excludeId = null)
    {
        return !await _context.CategoriasProducto
            .Where(c => c.Nombre == nombre && (excludeId == null || c.Id != excludeId))
            .AnyAsync();
    }

    public async Task<int> CalcularProfundidadAsync(int categoriaId)
    {
        var categoria = await _context.CategoriasProducto.FindAsync(categoriaId);
        if (categoria?.CategoriaPadreId == null) return 1;
        
        var profundidad = 1;
        var padreId = categoria.CategoriaPadreId;
        
        while (padreId.HasValue)
        {
            var padre = await _context.CategoriasProducto.FindAsync(padreId);
            if (padre == null) break;
            
            profundidad++;
            padreId = padre.CategoriaPadreId;
        }
        
        return profundidad;
    }

    public async Task<bool> EsDescendienteDeAsync(int ancestorId, int descendantId)
    {
        var actual = await _context.CategoriasProducto.FindAsync(descendantId);
        
        while (actual?.CategoriaPadreId.HasValue == true)
        {
            if (actual.CategoriaPadreId == ancestorId) return true;
            actual = await _context.CategoriasProducto.FindAsync(actual.CategoriaPadreId);
        }
        
        return false;
    }
}
```

#### Seed Data
```sql
INSERT INTO catalogo.CategoriasProducto (Nombre, Descripcion, CategoriaPadreId, Activo)
VALUES
('Electrónica', 'Productos electrónicos en general', NULL, 1),
('Computadoras', 'Equipos de cómputo', 1, 1),
('Laptops', 'Computadoras portátiles', 2, 1),
('Escritorios', 'Computadoras de escritorio', 2, 1),
('Accesorios', 'Accesorios tecnológicos', NULL, 1),
('Periféricos', 'Periféricos de computadora', 5, 1);
```

#### Endpoints (7 estándar + 1 especial)
```
GET    /api/v1/categorias-producto              → Listar (con árbol jerárquico)
GET    /api/v1/categorias-producto/{id}          → Obtener por ID
POST   /api/v1/categorias-producto               → Crear (valida profundidad)
PUT    /api/v1/categorias-producto/{id}          → Actualizar (valida ciclos)
PATCH  /api/v1/categorias-producto/{id}/activar  → Activar
PATCH  /api/v1/categorias-producto/{id}/inactivar → Inactivar
DELETE /api/v1/categorias-producto/{id}          → Eliminar (soft)
GET    /api/v1/categorias-producto/raices        → Obtener solo raíces (nivel 1)
```

---

### 2. MarcaProducto → `catalogo.MarcasProducto`

#### Domain Entity
```csharp
namespace Domain.Catalogo
{
    public class MarcaProducto : AuditableEntity
    {
        public string Nombre { get; set; }                     // 150, obligatorio
        public string? Descripcion { get; set; }               // 500, opcional
        public string? LogoUrl { get; set; }                   // 500, opcional
    }
}
```

#### Configuration
```
Schema: catalogo
Table: MarcasProducto
Constraints:
- PK: Id (INT)
- Indices: Nombre, Activo
- Default: Activo = 1
- No foreign keys (catálogo puro)
```

#### DTOs
- `CrearMarcaProductoDto`: Nombre, Descripcion?, LogoUrl?
- `ActualizarMarcaProductoDto`: Nombre, Descripcion?, LogoUrl?
- `MarcaProductoDto`: Full response con PublicId

#### Validaciones
- **Nombre:** required, max 150 chars, unique
- **Descripcion:** optional, max 500 chars
- **LogoUrl:** optional, max 500 chars (URL format opcional)

#### ValidatorService
```csharp
public class MarcaProductoValidatorService
{
    public async Task<bool> NombreUnicoAsync(string nombre, int? excludeId = null)
    {
        return !await _context.MarcasProducto
            .Where(m => m.Nombre == nombre && (excludeId == null || m.Id != excludeId))
            .AnyAsync();
    }
}
```

#### Seed Data
```sql
INSERT INTO catalogo.MarcasProducto (Nombre, Descripcion, LogoUrl, Activo)
VALUES
('Dell', 'Dell Inc. - Computadoras y periféricos', NULL, 1),
('HP', 'Hewlett-Packard - Computadoras y accesorios', NULL, 1),
('Lenovo', 'Lenovo Group - Equipos de cómputo', NULL, 1),
('Apple', 'Apple Inc. - Computadoras premium', NULL, 1),
('Asus', 'Asus - Motherboards y componentes', NULL, 1),
('Intel', 'Intel - Procesadores', NULL, 1);
```

#### Endpoints (7 estándar)
```
GET    /api/v1/marcas-producto              → Listar
GET    /api/v1/marcas-producto/{id}          → Obtener por ID
POST   /api/v1/marcas-producto               → Crear
PUT    /api/v1/marcas-producto/{id}          → Actualizar
PATCH  /api/v1/marcas-producto/{id}/activar  → Activar
PATCH  /api/v1/marcas-producto/{id}/inactivar → Inactivar
DELETE /api/v1/marcas-producto/{id}          → Eliminar (soft)
```

---

### 3. ALTER TABLE Productos (Migración Segura)

#### Problema Crítico: Datos Existentes
```
Tabla: catalogo.Productos
Registros existentes: N > 0
Problema: Si FKs son NOT NULL → ALTER falla
Solución: Hacer FKs NULLABLE
```

#### SQL Migration Script

**Archivo:** `Database/02_Tablas/15_AddProductoFKs.sql`

```sql
-- Safe idempotent migration: Agregar FKs a Productos
-- Ejecutar después de tablas CategoriasProducto y MarcasProducto creadas

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Productos' 
    AND TABLE_SCHEMA = 'catalogo'
    AND COLUMN_NAME = 'UnidadMedidaId'
)
BEGIN
    -- Agregar columnas
    ALTER TABLE catalogo.Productos ADD
        UnidadMedidaId      INT NULL,
        CategoriaProductoId INT NULL,
        MarcaProductoId     INT NULL;
    
    -- Agregar constraints
    ALTER TABLE catalogo.Productos ADD
        CONSTRAINT FK_Productos_UnidadMedida 
            FOREIGN KEY (UnidadMedidaId) 
            REFERENCES catalogo.UnidadesMedida(Id) ON DELETE RESTRICT;
    
    ALTER TABLE catalogo.Productos ADD
        CONSTRAINT FK_Productos_CategoriaProducto 
            FOREIGN KEY (CategoriaProductoId) 
            REFERENCES catalogo.CategoriasProducto(Id) ON DELETE RESTRICT;
    
    ALTER TABLE catalogo.Productos ADD
        CONSTRAINT FK_Productos_MarcaProducto 
            FOREIGN KEY (MarcaProductoId) 
            REFERENCES catalogo.MarcasProducto(Id) ON DELETE RESTRICT;
    
    -- Crear índices para lookups
    CREATE INDEX IX_Productos_UnidadMedidaId ON catalogo.Productos(UnidadMedidaId);
    CREATE INDEX IX_Productos_CategoriaProductoId ON catalogo.Productos(CategoriaProductoId);
    CREATE INDEX IX_Productos_MarcaProductoId ON catalogo.Productos(MarcaProductoId);
    
    PRINT 'Successfully added UnidadMedidaId, CategoriaProductoId, MarcaProductoId to Productos';
END
ELSE
BEGIN
    PRINT 'Columns already exist in Productos - skipping migration';
END
GO
```

#### Cambios en Entity Producto
```csharp
public class Producto : AuditableEntity
{
    // Campos existentes...
    public string Nombre { get; set; }
    public decimal Precio { get; set; }
    // ...
    
    // Nuevas FKs (NULLABLE)
    public int? UnidadMedidaId { get; set; }
    public int? CategoriaProductoId { get; set; }
    public int? MarcaProductoId { get; set; }
    
    // Navigation properties
    public virtual UnidadMedida? UnidadMedida { get; set; }
    public virtual CategoriaProducto? CategoriaProducto { get; set; }
    public virtual MarcaProducto? MarcaProducto { get; set; }
}
```

#### Cambios en ProductoConfiguration
```csharp
protected override void Configure(EntityTypeBuilder<Producto> builder)
{
    // Configuraciones existentes...
    
    // Nuevas FKs
    builder.HasOne(p => p.UnidadMedida)
        .WithMany()
        .HasForeignKey(p => p.UnidadMedidaId)
        .OnDelete(DeleteBehavior.Restrict)
        .IsRequired(false);
    
    builder.HasOne(p => p.CategoriaProducto)
        .WithMany()
        .HasForeignKey(p => p.CategoriaProductoId)
        .OnDelete(DeleteBehavior.Restrict)
        .IsRequired(false);
    
    builder.HasOne(p => p.MarcaProducto)
        .WithMany()
        .HasForeignKey(p => p.MarcaProductoId)
        .OnDelete(DeleteBehavior.Restrict)
        .IsRequired(false);
}
```

#### Cambios en DTOs de Producto

**CrearProductoDto / ActualizarProductoDto**
```csharp
public class CrearProductoDto
{
    // Campos existentes
    public string Nombre { get; set; }
    public decimal Precio { get; set; }
    // ...
    
    // Nuevos campos OPCIONALES
    public int? UnidadMedidaId { get; set; }
    public int? CategoriaProductoId { get; set; }
    public int? MarcaProductoId { get; set; }
}
```

**ProductoDto (Response)**
```csharp
public class ProductoDto
{
    public int Id { get; set; }
    public string PublicId { get; set; }
    public string Nombre { get; set; }
    public decimal Precio { get; set; }
    // ...
    
    // Nuevas propiedades (nested objects)
    public UnidadMedidaDto? UnidadMedida { get; set; }
    public CategoriaProductoDto? CategoriaProducto { get; set; }
    public MarcaProductoDto? MarcaProducto { get; set; }
}
```

---

## 📂 ESTRUCTURA DE ARCHIVOS A CREAR

```
Domain/Catalogo/
├── CategoriaProducto.cs (NEW)
└── MarcaProducto.cs (NEW)

Application/Features/Catalogo/
├── CategoriaProducto/
│   ├── Crear/
│   │   ├── CrearCategoriaProductoCommand.cs
│   │   ├── CrearCategoriaProductoHandler.cs
│   │   └── CrearCategoriaProductoValidator.cs
│   ├── Actualizar/
│   │   ├── ActualizarCategoriaProductoCommand.cs
│   │   ├── ActualizarCategoriaProductoHandler.cs
│   │   └── ActualizarCategoriaProductoValidator.cs
│   ├── ActualizarEstado/
│   │   ├── ActualizarEstadoCategoriaProductoCommand.cs
│   │   └── ActualizarEstadoCategoriaProductoHandler.cs
│   ├── Eliminar/
│   │   ├── EliminarCategoriaProductoCommand.cs
│   │   └── EliminarCategoriaProductoHandler.cs
│   └── Queries/ (opcional - usar Services)
└── MarcaProducto/ (estructura idéntica × 4 handlers)

Application/Dtos/Catalogo/
├── CrearCategoriaProductoDto.cs
├── ActualizarCategoriaProductoDto.cs
├── CategoriaProductoDto.cs
├── CrearMarcaProductoDto.cs
├── ActualizarMarcaProductoDto.cs
└── MarcaProductoDto.cs

Application/Interfaces/
├── ICategoriaProductoService.cs
├── ICategoriaProductoValidatorService.cs
├── IMarcaProductoService.cs
└── IMarcaProductoValidatorService.cs

Application/Mappings/Catalogo/
├── CategoriaProductoProfile.cs
└── MarcaProductoProfile.cs

Infrastructure/Persistence/Configurations/
├── CategoriaProductoConfiguration.cs
└── MarcaProductoConfiguration.cs

Infrastructure/Repository/
├── CategoriaProductoService.cs
├── CategoriaProductoValidatorService.cs
├── MarcaProductoService.cs
└── MarcaProductoValidatorService.cs

GestionComercial/Controllers/
├── CategoriasProductoController.cs
└── MarcasProductoController.cs

Database/02_Tablas/
├── 13_CategoriasProducto.sql (NEW)
├── 14_MarcasProducto.sql (NEW)
└── 15_AddProductoFKs.sql (MIGRATION - ejecutar al final)

Database/03_Seeds/
└── 12_InitCategoriasProductoMarcasProducto.sql (NEW)
```

---

## 🔄 PATRONES EXACTOS A SEGUIR

### Handler: Crear con Validación de Profundidad

```csharp
public class CrearCategoriaProductoHandler : IRequestHandler<CrearCategoriaProductoCommand, Result<int>>
{
    private readonly ICategoriaProductoService _service;
    private readonly ICategoriaProductoValidatorService _validator;
    private readonly IMapper _mapper;
    private readonly ILogger<CrearCategoriaProductoHandler> _logger;

    public async Task<Result<int>> Handle(CrearCategoriaProductoCommand request, CancellationToken ct)
    {
        // Validar profundidad si tiene padre
        if (request.CategoriaPadreId.HasValue)
        {
            var profundidad = await _validator.CalcularProfundidadAsync(request.CategoriaPadreId.Value);
            if (profundidad >= 3)
                throw new InvalidOperationException("Máximo 3 niveles de profundidad permitidos");
        }

        var categoria = _mapper.Map<CategoriaProducto>(request);
        var resultado = await _service.Crear(categoria);
        
        _logger.LogInformation($"Categoría producto creada: {categoria.Id}");
        
        return Result<int>.Success(resultado);
    }
}
```

### Handler: Actualizar con Prevención de Ciclos

```csharp
public class ActualizarCategoriaProductoHandler : IRequestHandler<ActualizarCategoriaProductoCommand, Result<int>>
{
    private readonly ICategoriaProductoService _service;
    private readonly ICategoriaProductoValidatorService _validator;
    private readonly IMapper _mapper;

    public async Task<Result<int>> Handle(ActualizarCategoriaProductoCommand request, CancellationToken ct)
    {
        var categoria = await _service.ObtenerPorId(request.Id);
        if (categoria == null)
            throw new NotFoundException($"Categoría {request.Id} no encontrada");

        // Prevenir ciclos
        if (request.CategoriaPadreId.HasValue && request.CategoriaPadreId != categoria.CategoriaPadreId)
        {
            var esDescendiente = await _validator.EsDescendienteDeAsync(request.CategoriaPadreId.Value, request.Id);
            if (esDescendiente)
                throw new InvalidOperationException("No se puede crear ciclo: padre no puede ser descendiente");
        }

        _mapper.Map(request, categoria);
        await _service.Actualizar(categoria);
        
        return Result<int>.Success(request.Id);
    }
}
```

### Validator: Crear con Validación Async

```csharp
public class CrearCategoriaProductoValidator : AbstractValidator<CrearCategoriaProductoCommand>
{
    private readonly ICategoriaProductoValidatorService _validator;

    public CrearCategoriaProductoValidator(ICategoriaProductoValidatorService validator)
    {
        _validator = validator;

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("Nombre requerido")
            .MaximumLength(150).WithMessage("Nombre máximo 150 caracteres");

        RuleFor(x => x.CategoriaPadreId)
            .MustAsync(async (parentId, ct) =>
            {
                if (parentId == null) return true;
                
                var existe = await _validator.ObtenerPorIdAsync(parentId.Value) != null;
                return existe;
            })
            .WithMessage("Categoría padre no existe")
            .When(x => x.CategoriaPadreId.HasValue);
    }
}
```

### Service Interface

```csharp
public interface ICategoriaProductoService
{
    Task<CategoriaProducto?> ObtenerPorId(int id, bool tracking = false);
    Task<List<CategoriaProducto>> ObtenerTodos();
    Task<List<CategoriaProducto>> ObtenerRaices(); // Solo nivel 1
    Task<int> Crear(CategoriaProducto categoria);
    Task Actualizar(CategoriaProducto categoria);
    Task Eliminar(int id);
}
```

### AutoMapper Profile

```csharp
public class CategoriaProductoProfile : Profile
{
    public CategoriaProductoProfile()
    {
        CreateMap<CrearCategoriaProductoCommand, CategoriaProducto>();
        CreateMap<ActualizarCategoriaProductoCommand, CategoriaProducto>()
            .ForMember(d => d.Id, opt => opt.Ignore());
        CreateMap<CategoriaProducto, CategoriaProductoDto>()
            .ForMember(d => d.Subcategorias, opt => opt.MapFrom(s => s.Subcategorias));
    }
}
```

---

## 📊 RESUMEN DE ARTEFACTOS

| Item | Cantidad | Descripción |
|------|----------|-------------|
| Entidades nuevas | 2 | CategoriaProducto, MarcaProducto |
| Commands | 8 | 4 × CategoriaProducto, 4 × MarcaProducto |
| Handlers | 8 | Crear, Actualizar, ActualizarEstado, Eliminar × 2 |
| Validators | 4 | Crear + Actualizar × 2 |
| ValidatorServices | 2 | Cada entidad con métodos específicos |
| Services | 2 | Interfaz + Implementación |
| DTOs | 6 | 3 × 2 entidades |
| Mappings | 2 | AutoMapper profiles |
| Configurations | 2 | Entity Framework configurations |
| Controllers | 2 | CategoriasProductoController, MarcasProductoController |
| Endpoints | 14 | 7 × 2 entidades (estándar CRUD) |
| SQL Scripts | 3 | 2 tablas nuevas + 1 FIX migration |
| Archivos nuevos | ~18 | Total |

---

## ⚠️ RIESGOS CRÍTICOS & MITIGACIÓN

### R-01: ALTER TABLE rompe datos existentes
**Probabilidad:** Alta | **Impacto:** Alto | **Estado:** MITIGADO
- **Solución:** FKs NULLABLE + script idempotente
- **Validación:** Backup antes de ejecutar, comprobar sin errores

### R-02: Ciclos en CategoriaProducto
**Probabilidad:** Media | **Impacto:** Medio | **Estado:** MITIGADO
- **Solución:** Validación en `ActualizarCategoriaProductoHandler` + `EsDescendienteDeAsync()`
- **Validación:** Tests de actualización con ciclo deben fallar

### R-03: Profundidad ilimitada
**Probabilidad:** Baja | **Impacto:** Bajo | **Estado:** MITIGADO
- **Solución:** Validación en `CrearCategoriaProductoHandler` + límite 3 niveles
- **Validación:** Test creando categoría nivel 4 debe fallar

### R-04: Performance en árboles grandes
**Probabilidad:** Baja | **Impacto:** Medio | **Estado:** MITIGADO
- **Solución:** Índices en CategoriaPadreId, paginación en listados
- **Validación:** Seed con 6+ categorías, verificar índices creados

---

## 🚨 CRITICAL RULES

1. **Self-Reference en CategoriaProducto:** Implementar correctamente con DeleteBehavior.Restrict
2. **Profundidad Máxima:** Aplicar en Handler, NO en BD constraint (application rule)
3. **Ciclos en Actualización:** Validar `EsDescendienteDeAsync()` antes de permitir cambio de padre
4. **FKs NULLABLE en Productos:** Absolutamente crítico para migración segura
5. **Script Migration Idempotente:** Usar `IF NOT EXISTS` para permitir múltiples ejecuciones
6. **Soft Delete:** Patrón `Activo = false`, no hard delete
7. **No Cascada:** Usar DeleteBehavior.Restrict en todos los FK
8. **Índices Estratégicos:** Crear después de tablas (performance en lookups)

---

## ✅ CHECKLIST PRE-IMPLEMENTACIÓN

- [ ] Leer plan activo: `plans/active/2026-05-16_catalogo-sprint4-producto.md`
- [ ] Revisar SPRINT_3_READY.md para patrones más recientes
- [ ] Compilar proyecto baseline (verify 0 errores)
- [ ] Verificar Domain/Catalogo existe
- [ ] Verificar Application/Features/Catalogo existe
- [ ] Verificar Infrastructure/Repository existe
- [ ] Verificar Database/02_Tablas, 03_Seeds existen
- [ ] Crear backup de base de datos (antes ALTER TABLE)
- [ ] Script 15_AddProductoFKs.sql listo para ejecutar

---

## 📋 CHECKLIST DE DESARROLLO

**Fase 1: Entidades de Dominio**
- [ ] Crear CategoriaProducto.cs (con navegación Subcategorias)
- [ ] Crear MarcaProducto.cs

**Fase 2: CQRS Commands**
- [ ] CrearCategoriaProductoCommand.cs
- [ ] ActualizarCategoriaProductoCommand.cs
- [ ] ActualizarEstadoCategoriaProductoCommand.cs
- [ ] EliminarCategoriaProductoCommand.cs
- [ ] Ídem para MarcaProducto (4 files)

**Fase 3: CQRS Handlers**
- [ ] CrearCategoriaProductoHandler.cs (con validación profundidad)
- [ ] ActualizarCategoriaProductoHandler.cs (con prevención ciclos)
- [ ] ActualizarEstadoCategoriaProductoHandler.cs
- [ ] EliminarCategoriaProductoHandler.cs
- [ ] Ídem para MarcaProducto (4 files)

**Fase 4: CQRS Validators**
- [ ] CrearCategoriaProductoValidator.cs
- [ ] ActualizarCategoriaProductoValidator.cs
- [ ] CrearMarcaProductoValidator.cs
- [ ] ActualizarMarcaProductoValidator.cs

**Fase 5: DTOs & Mappings**
- [ ] CrearCategoriaProductoDto.cs + ActualizarCategoriaProductoDto.cs + CategoriaProductoDto.cs
- [ ] CrearMarcaProductoDto.cs + ActualizarMarcaProductoDto.cs + MarcaProductoDto.cs
- [ ] CategoriaProductoProfile.cs (AutoMapper)
- [ ] MarcaProductoProfile.cs (AutoMapper)

**Fase 6: Services & Validaciones**
- [ ] ICategoriaProductoService.cs + CategoriaProductoService.cs
- [ ] ICategoriaProductoValidatorService.cs + CategoriaProductoValidatorService.cs
- [ ] IMarcaProductoService.cs + MarcaProductoService.cs
- [ ] IMarcaProductoValidatorService.cs + MarcaProductoValidatorService.cs

**Fase 7: Database**
- [ ] CategoriaProductoConfiguration.cs (con self-ref setup)
- [ ] MarcaProductoConfiguration.cs
- [ ] Database/02_Tablas/13_CategoriasProducto.sql
- [ ] Database/02_Tablas/14_MarcasProducto.sql
- [ ] Database/02_Tablas/15_AddProductoFKs.sql (migration script)
- [ ] Database/03_Seeds/12_InitCategoriasProductoMarcasProducto.sql

**Fase 8: API & Controllers**
- [ ] CategoriasProductoController.cs (7 endpoints + 1 especial)
- [ ] MarcasProductoController.cs (7 endpoints)
- [ ] Registrar rutas en Program.cs

**Fase 9: Integración**
- [ ] Actualizar Producto.cs (agregar 3 FK + navigations)
- [ ] Actualizar ProductoConfiguration.cs (configurar FKs)
- [ ] Actualizar ProductoDto (agregar nested dtos)
- [ ] Actualizar ProductoProfile (mapear nuevos campos)
- [ ] Actualizar Program.cs (agregar 4 DI registrations)

**Fase 10: Testing & Smoke**
- [ ] Build project (0 errores, 0 advertencias)
- [ ] Ejecutar scripts SQL en orden
- [ ] Verificar tablas creadas correctamente
- [ ] GET /api/v1/categorias-producto (lista con seed)
- [ ] GET /api/v1/marcas-producto (lista con seed)
- [ ] POST creación categoría nivel 4 (debe fallar)
- [ ] PUT actualización con ciclo (debe fallar)
- [ ] Verificar Productos NO se quebraron (migration safe)

---

## 📊 SUCCESS CRITERIA

- [ ] Compilación: 0 errores, 0 advertencias, 0 CS warnings
- [ ] Endpoints: 14 totales (7 × 2 entidades)
- [ ] Commands: 8 nuevos funcionando
- [ ] Handlers: 8 nuevos con lógica completa
- [ ] Validators: 4 nuevos con validaciones async
- [ ] Services: 2 nuevos + 2 ValidatorServices
- [ ] DTOs: 6 nuevos
- [ ] Controllers: 2 nuevos
- [ ] Configurations: 2 nuevas
- [ ] SQL: 2 tablas + 1 FIX script idempotente
- [ ] Program.cs: +4 DI registrations (2 Service + 2 ValidatorService)
- [ ] Productos.cs: +3 FK + navigations sin errores
- [ ] Seed ejecutado: categorías raíces + subcategorías + marcas
- [ ] GET /api/v1/categorias-producto: árbol jerárquico correcto
- [ ] POST nivel 4: rechazado con error
- [ ] PUT ciclo: rechazado con error
- [ ] ALTER TABLE: ejecutado sin romper productos existentes
- [ ] Soft delete: Activo = false funciona en ambas entidades

---

## 🔗 REFERENCIAS CRÍTICAS

```
plans/active/2026-05-16_catalogo-sprint4-producto.md
  └─ Plan detallado con scope, riesgos y decisiones

SPRINT_2_READY.md
SPRINT_3_READY.md
  └─ Patrones recientes (handlers, validators, controllers)

IA_Docs/VALIDATOR_SERVICE_PATTERN.md
  └─ Patrón ValidatorService (OBLIGATORIO)

IA_Docs/ARCHITECTURE_DECISIONS.md
  └─ Decisiones arquitectónicas (ADR-001 a ADR-010)

execution-status/catalogo-base-status.md
  └─ Actualizar progreso diariamente

pending/2026-05-15_technical-backlog.md
  └─ Estado actual de deuda técnica
```

---

## 📝 POST-BUILD ACTIONS

1. [ ] Ejecutar SQL scripts en orden:
   - `Database/02_Tablas/13_CategoriasProducto.sql`
   - `Database/02_Tablas/14_MarcasProducto.sql`
   - `Database/02_Tablas/15_AddProductoFKs.sql` (CRÍTICO - último)
   - `Database/03_Seeds/12_InitCategoriasProductoMarcasProducto.sql`

2. [ ] Update `execution-status/catalogo-base-status.md`
   - Sprint 4: 0% → 100%
   - Modules: 2 completed (CategoriaProducto, MarcaProducto)
   - Migración: Productos enriquecida ✅

3. [ ] Create History Changed entry
   - `20260517_THHMM_feat_Sprint4ProductoEnriquecido`
   - SUMMARY.md con:
     * Entidades creadas + validaciones
     * Riesgos mitigados (ALTER TABLE, ciclos, profundidad)
     * Migración de Productos segura
     * Patrones especiales (self-ref, profundidad, ciclos)
     * Cambios en Productos.cs

4. [ ] Commit
   - Message: `feat(catalogo): Sprint 4 — Producto Enriquecido (CategoriaProducto, MarcaProducto, ALTER Productos)`

5. [ ] Merge a rama develop
   - PR: `catalogo-base/sprint_4` → `develop`
   - Revisor: Arquitecto Backend

6. [ ] Move plan
   - `plans/active/` → `plans/completed/`
   - Crear SUMMARY.md en History Changed

---

## 🎯 PRÓXIMO PASO

**Sprint 5 (Fase 4 — Comercial):**
- CondicionPago → `catalogo.CondicionesPago`
- ListaPrecio → `catalogo.ListasPrecios`
- Proveedor → `comercial.Proveedores` (clone de Cliente)

**Bloqueador actual:** Ninguno
**Dependencia completada:** Sprint 3 (Fiscal) ✅

---

**Status:** ⏳ SPRINT 4 LISTO PARA EJECUTAR — Especificación ejecutable completa  
**Documento:** SPRINT_4_READY.md (Especificación Ejecutable)  
**Patrón:** Self-referencia + Validación profundidad + Prevención ciclos + Migración segura  
**Siguiente:** Iniciar implementación cuando equipo esté disponible
