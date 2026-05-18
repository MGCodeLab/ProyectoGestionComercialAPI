# Sprint 1: Catálogos Base (Pais, Moneda, UnidadMedida, ModuloSistema, ParametroSistema) — IMPLEMENTADO ✅

**Versión:** 1.0  
**Fecha Especificación:** 2026-05-10  
**Fecha Implementación:** 2026-05-10 (Sprint 1)  
**Estado:** ✅ **IMPLEMENTACIÓN COMPLETADA**  
**Arquitecto:** Nexus Backend Architect  
**Implementador:** Nexus Fast Builder  
**Rama:** `catalogo-base/sprint_1`

**📍 IMPLEMENTACIÓN COMPLETADA:**
- ✅ Todas las entidades creadas (Pais, Moneda, UnidadMedida, ModuloSistema, ParametroSistema)
- ✅ CQRS completo: 20 Commands + 20 Handlers + 10 Validators
- ✅ 47+ archivos nuevos — 0 errores de compilación
- ✅ ValidatorService pattern implementado (Clean Architecture)
- ✅ 3 problemas encontrados y documentados
- ✅ Documentación: IA_Docs, History Changed, Planes
- ✅ SQL scripts ejecutados + smoke testing completado

---

## ✅ DECISIONES APROBADAS

### 1️⃣ Fundación Multi-País
- **Decisión:** No hardcodear Perú — parametrizar por `Pais`
- **Beneficio:** Multi-país desde el inicio sin refactoring futuro
- **Implementación:** `catalogo.Paises` con ISO 3166-1 alpha-2 codes

### 2️⃣ Moneda Funcional
- **Decisión:** Moneda funcional única (PEN hoy), pero arquitectura lista para multi-moneda
- **Conversión de cambio:** Deferred a v3.2+
- **Implementación:** `catalogo.Monedas` con ISO 4217 codes

### 3️⃣ Feature Flags desde Etapa Temprana
- **Decisión:** `ModuloSistema` como catálogo para control comercial
- **Beneficio:** Módulos activables/desactivables sin código
- **Patrón:** Si catálogo vacío → fail-open (todos activos por defecto)

### 4️⃣ Patrón ValidatorService
- **Decisión:** ValidatorServices en Infrastructure para validaciones de BD
- **Beneficio:** Clean Architecture garantizada (Application sin imports Infrastructure concretos)
- **Implementación:** IXxxValidatorService (Application) → XxxValidatorService (Infrastructure)

### 5️⃣ CQRS Pragmático
- **Commands:** Crear, Actualizar, ActualizarEstado, Eliminar (4 por entidad)
- **Queries:** GetAll, GetById via Service (no Commands)
- **Patrón:** Commands via MediatR, Queries via Services directos

---

## 📋 ENTIDADES A CREAR (5)

### 1. Pais → `catalogo.Paises`

**Campos:**
```csharp
public class Pais : AuditableEntity
{
    public string Nombre { get; set; }                 // 100, obligatorio
    public string Codigo { get; set; }                 // 2, UNIQUE, ISO 3166-1 (PE, CL, AR)
    public string CodigoMoneda { get; set; }           // 3, ISO 4217 (PEN, CLP, ARS)
}
```

**Validador:** CrearPaisValidator
- Nombre: required, max 100
- Codigo: required, unique, length 2
- CodigoMoneda: required, length 3

**DTOs:**
- CrearPaisDto
- ActualizarPaisDto
- PaisDto (response)

**Controller:** 7 endpoints (GET, GET/{id}, POST, PUT, PATCH activar/inactivar, DELETE)

---

### 2. Moneda → `catalogo.Monedas`

**Campos:**
```csharp
public class Moneda : AuditableEntity
{
    public string Nombre { get; set; }                 // 100, obligatorio
    public string Simbolo { get; set; }                // 5, obligatorio (S/, $, €)
    public string CodigoISO { get; set; }              // 3, UNIQUE, ISO 4217
    public bool EsMonedaBase { get; set; }             // Solo 1 puede ser true
}
```

**Validador:** CrearMonedaValidator
- Nombre: required, max 100
- Simbolo: required, max 5
- CodigoISO: required, unique, length 3
- EsMonedaBase: logical validation (solo 1 true)

**DTOs:** CrearMonedaDto, ActualizarMonedaDto, MonedaDto

**Controller:** 7 endpoints

**Seed:** PEN como moneda base, USD como secundaria

---

### 3. UnidadMedida → `catalogo.UnidadesMedida`

**Campos:**
```csharp
public class UnidadMedida : AuditableEntity
{
    public string Nombre { get; set; }                 // 100, obligatorio
    public string Simbolo { get; set; }                // 10, obligatorio
    public string Codigo { get; set; }                 // 10, UNIQUE, SUNAT standard
}
```

**Validador:** CrearUnidadMedidaValidator
- Nombre: required, max 100
- Simbolo: required, max 10
- Codigo: required, unique, max 10

**DTOs:** CrearUnidadMedidaDto, ActualizarUnidadMedidaDto, UnidadMedidaDto

**Controller:** 7 endpoints

**Seed:** UND (unidad), KGM (kilogramo), LTR (litro), MTR (metro), CAJ (caja), PAQ (paquete), DOZ (docena)

**Nota especial:** Códigos SUNAT estándar — verificar unicidad en seed

---

### 4. ModuloSistema → `configuracion.ModulosSistema`

**Campos:**
```csharp
public class ModuloSistema : AuditableEntity
{
    public string Nombre { get; set; }                 // 100, obligatorio
    public string Codigo { get; set; }                 // 50, UNIQUE (VENTAS, COMPRAS, INVENTARIO)
    public string? Descripcion { get; set; }           // 500, opcional
}
```

**Validador:** CrearModuloSistemaValidator
- Nombre: required, max 100
- Codigo: required, unique, max 50
- Descripcion: optional, max 500

**DTOs:** CrearModuloSistemaDto, ActualizarModuloSistemaDto, ModuloSistemaDto

**Controller:** 7 endpoints

**Seed:** VENTAS=true, COMPRAS=false, INVENTARIO=false

**Patrón:** Si catálogo vacío en BD → fail-open, todos activos

---

### 5. ParametroSistema → `configuracion.ParametrosSistema`

**Campos:**
```csharp
public class ParametroSistema : AuditableEntity
{
    public string Clave { get; set; }                  // 100, UNIQUE (MONEDA_BASE, IGV_PORCENTAJE)
    public string Valor { get; set; }                  // 500, obligatorio
    public string TipoDato { get; set; }               // 20, default 'STRING' (STRING, INT, DECIMAL, BOOL)
    public string? Descripcion { get; set; }           // 500, opcional
}
```

**Validador:** CrearParametroSistemaValidator
- Clave: required, unique, max 100
- Valor: required, max 500
- TipoDato: required, max 20
- Descripcion: optional, max 500

**DTOs:** CrearParametroSistemaDto, ActualizarParametroSistemaDto, ParametroSistemaDto

**Controller:** 7 endpoints

**Seed:** MONEDA_BASE=PEN, IGV_PORCENTAJE=18, EMPRESA_RUC=20000000001

**Nota especial:** Campo único es `Clave` (no `Codigo`)

---

## 📂 ESTRUCTURA DE ARCHIVOS A CREAR

### Domain/
```
Domain/Catalogo/
├── Pais.cs
├── Moneda.cs
├── UnidadMedida.cs
└── (ModuloSistema y ParametroSistema van en Domain/Configuracion/)

Domain/Configuracion/
├── ModuloSistema.cs
└── ParametroSistema.cs
```

### Application/
```
Application/
├── Features/Catalogo/
│   ├── Pais/
│   │   ├── Crear/
│   │   │   ├── CrearPaisCommand.cs
│   │   │   ├── CrearPaisHandler.cs
│   │   │   └── CrearPaisValidator.cs
│   │   ├── Actualizar/
│   │   │   ├── ActualizarPaisCommand.cs
│   │   │   ├── ActualizarPaisHandler.cs
│   │   │   └── ActualizarPaisValidator.cs
│   │   ├── ActualizarEstado/
│   │   │   ├── ActualizarEstadoPaisCommand.cs
│   │   │   └── ActualizarEstadoPaisHandler.cs
│   │   └── Eliminar/
│   │       ├── EliminarPaisCommand.cs
│   │       └── EliminarPaisHandler.cs
│   ├── Moneda/ (estructura idéntica)
│   └── UnidadMedida/ (estructura idéntica)
├── Features/Configuracion/
│   ├── ModuloSistema/ (estructura idéntica)
│   └── ParametroSistema/ (estructura idéntica)
├── Dtos/Catalogo/
│   ├── CrearPaisDto.cs
│   ├── ActualizarPaisDto.cs
│   ├── PaisDto.cs
│   └── (repetir para Moneda y UnidadMedida)
├── Dtos/Configuracion/
│   ├── CrearModuloSistemaDto.cs
│   ├── ActualizarModuloSistemaDto.cs
│   ├── ModuloSistemaDto.cs
│   └── (repetir para ParametroSistema)
├── Interfaces/
│   ├── IPaisValidatorService.cs
│   ├── IMonedaValidatorService.cs
│   ├── IUnidadMedidaValidatorService.cs
│   ├── IModuloSistemaValidatorService.cs
│   └── IParametroSistemaValidatorService.cs
└── Mappings/
    ├── Catalogo/
    │   ├── PaisProfile.cs
    │   ├── MonedaProfile.cs
    │   └── UnidadMedidaProfile.cs
    └── Configuracion/
        ├── ModuloSistemaProfile.cs
        └── ParametroSistemaProfile.cs
```

### Infrastructure/
```
Infrastructure/
├── Persistence/Configurations/
│   ├── PaisConfiguration.cs
│   ├── MonedaConfiguration.cs
│   ├── UnidadMedidaConfiguration.cs
│   ├── ModuloSistemaConfiguration.cs
│   └── ParametroSistemaConfiguration.cs
├── Repository/
│   ├── PaisService.cs
│   ├── PaisValidatorService.cs
│   ├── MonedaService.cs
│   ├── MonedaValidatorService.cs
│   ├── UnidadMedidaService.cs
│   ├── UnidadMedidaValidatorService.cs
│   ├── ModuloSistemaService.cs
│   ├── ModuloSistemaValidatorService.cs
│   ├── ParametroSistemaService.cs
│   └── ParametroSistemaValidatorService.cs
└── AppDbContext.cs (+ 5 DbSets)
```

### Database/
```
Database/
├── 01_Schemas/
│   └── 01_Schemas.sql (CREATE SCHEMA catalogo, configuracion)
├── 02_Tablas/
│   ├── 01_Paises.sql
│   ├── 02_Monedas.sql
│   ├── 03_UnidadesMedida.sql
│   ├── 04_ModulosSistema.sql
│   └── 05_ParametrosSistema.sql
└── 03_Seeds/
    └── 01_InitPaisMonedaUnidadModuloParametro.sql
```

### Controllers/
```
GestionComercial/Controllers/
├── PaisesController.cs (7 endpoints)
├── MonedasController.cs (7 endpoints)
├── UnidadesMedidaController.cs (7 endpoints)
├── ModulosSistemaController.cs (7 endpoints)
└── ParametrosSistemaController.cs (7 endpoints)
```

### Program.cs
```csharp
// Agregar DI registrations (10 líneas):
builder.Services.AddScoped<IPaisService, PaisService>();
builder.Services.AddScoped<IPaisValidatorService, PaisValidatorService>();
builder.Services.AddScoped<IMonedaService, MonedaService>();
builder.Services.AddScoped<IMonedaValidatorService, MonedaValidatorService>();
builder.Services.AddScoped<IUnidadMedidaService, UnidadMedidaService>();
builder.Services.AddScoped<IUnidadMedidaValidatorService, UnidadMedidaValidatorService>();
builder.Services.AddScoped<IModuloSistemaService, ModuloSistemaService>();
builder.Services.AddScoped<IModuloSistemaValidatorService, ModuloSistemaValidatorService>();
builder.Services.AddScoped<IParametroSistemaService, ParametroSistemaService>();
builder.Services.AddScoped<IParametroSistemaValidatorService, ParametroSistemaValidatorService>();
```

---

## 🎯 ENDPOINTS (35 total)

### Paises
```
GET    /api/v1/paises
GET    /api/v1/paises/{id}
POST   /api/v1/paises
PUT    /api/v1/paises/{id}
PATCH  /api/v1/paises/{id}/activar
PATCH  /api/v1/paises/{id}/inactivar
DELETE /api/v1/paises/{id}
```

### Monedas
```
GET    /api/v1/monedas
GET    /api/v1/monedas/{id}
POST   /api/v1/monedas
PUT    /api/v1/monedas/{id}
PATCH  /api/v1/monedas/{id}/activar
PATCH  /api/v1/monedas/{id}/inactivar
DELETE /api/v1/monedas/{id}
```

### UnidadesMedida
```
GET    /api/v1/unidades-medida
GET    /api/v1/unidades-medida/{id}
POST   /api/v1/unidades-medida
PUT    /api/v1/unidades-medida/{id}
PATCH  /api/v1/unidades-medida/{id}/activar
PATCH  /api/v1/unidades-medida/{id}/inactivar
DELETE /api/v1/unidades-medida/{id}
```

### ModulosSistema
```
GET    /api/v1/modulos-sistema
GET    /api/v1/modulos-sistema/{id}
POST   /api/v1/modulos-sistema
PUT    /api/v1/modulos-sistema/{id}
PATCH  /api/v1/modulos-sistema/{id}/activar
PATCH  /api/v1/modulos-sistema/{id}/inactivar
DELETE /api/v1/modulos-sistema/{id}
```

### ParametrosSistema
```
GET    /api/v1/parametros-sistema
GET    /api/v1/parametros-sistema/{id}
POST   /api/v1/parametros-sistema
PUT    /api/v1/parametros-sistema/{id}
PATCH  /api/v1/parametros-sistema/{id}/activar
PATCH  /api/v1/parametros-sistema/{id}/inactivar
DELETE /api/v1/parametros-sistema/{id}
```

---

## ✅ CHECKLIST PRE-BUILD

- [ ] Leer `plans/active/2026-05-10_catalogo-roadmap-sprints2-5.md` (contexto general)
- [ ] Leer `IA_Docs/VALIDATOR_SERVICE_PATTERN.md` (patrón obligatorio)
- [ ] Compilar proyecto baseline (verify 0 errores)
- [ ] Verificar Domain/Catalogo/ existe (para colocar entidades nuevas)
- [ ] Verificar Domain/Configuracion/ existe
- [ ] Verificar Application/Features/Catalogo/ existe
- [ ] Verificar Application/Features/Configuracion/ existe
- [ ] Verificar Infrastructure/Repository/ existe
- [ ] Verificar Database/ estructura (01_Schemas, 02_Tablas, 03_Seeds)

---

## 🔄 PATRÓN EXACTO A SEGUIR

### Handler Pattern (Copy-Paste)
```csharp
public class CrearPaisHandler : IRequestHandler<CrearPaisCommand, int>
{
    private readonly IPaisService _service;
    private readonly IMapper _mapper;

    public async Task<int> Handle(CrearPaisCommand request, CancellationToken ct)
    {
        // Map
        var pais = _mapper.Map<Pais>(request);
        
        // Persist
        var resultado = await _service.Crear(pais);
        
        return resultado;
    }
}
```

### Validator Pattern (Copy-Paste)
```csharp
public class CrearPaisValidator : AbstractValidator<CrearPaisCommand>
{
    private readonly IPaisValidatorService _validator;

    public CrearPaisValidator(IPaisValidatorService validator)
    {
        _validator = validator;

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("Nombre requerido")
            .MaximumLength(100);

        RuleFor(x => x.Codigo)
            .NotEmpty()
            .MustAsync(BeUniqueCodigo)
            .WithMessage("Código ya existe");
    }

    private async Task<bool> BeUniqueCodigo(string codigo, CancellationToken ct)
        => await _validator.IsCodigoUnique(codigo, ct);
}
```

### Service Pattern (Copy-Paste)
```csharp
public interface IPaisService
{
    Task<Pais?> ObtenerPorId(int id, bool tracking = false);
    Task<List<Pais>> ObtenerTodos();
    Task<int> Crear(Pais pais);
    Task Actualizar(Pais pais);
    Task Eliminar(int id);
}

public interface IPaisValidatorService
{
    Task<bool> IsCodigoUnique(string codigo, CancellationToken ct);
    Task<bool> IsCodigoUniqueExcept(string codigo, int excludeId, CancellationToken ct);
}
```

### AutoMapper Profile Pattern
```csharp
public class PaisProfile : Profile
{
    public PaisProfile()
    {
        CreateMap<CrearPaisCommand, Pais>();
        CreateMap<ActualizarPaisCommand, Pais>();
        CreateMap<Pais, PaisDto>().ReverseMap();
    }
}
```

---

## 🚨 CRITICAL RULES

1. **No hardcodear ubicaciones:** Usar catálogos Pais y Moneda para localización
2. **ValidatorService obligatorio:** Para cualquier campo único (Codigo, Clave, etc.)
3. **Clean Architecture strict:** Application layer NUNCA importa Infrastructure.Persistence concretos
4. **Handlers inyectan IService, NO AppDbContext** — Esto es innegociable
5. **Validators inyectan IValidatorService interface** — No EF Core directo
6. **Soft Delete:** Patrón Activo = auditoría (GET retorna todos, no filtra)
7. **AuditableEntity:** Todas las entidades heredan de AuditableEntity
8. **Seeds obligatorios:** Todo nuevo catálogo debe tener seed con datos útiles
9. **DTOs completos:** Crear + Actualizar + Response (NO omitir DTO Actualizar)
10. **Unique constraints:** Verificar en seed (sin constraint violations)

---

## 📊 SUCCESS CRITERIA

- [ ] Compilación: 0 errores, 0 advertencias
- [ ] Endpoints: 35 totales (7 × 5 entidades)
- [ ] Handlers: 20 nuevos (4 × 5)
- [ ] Validators: 10 nuevos (2 × 5)
- [ ] ValidatorServices: 5 nuevos
- [ ] DTOs: 15 nuevos (3 × 5)
- [ ] Services: 5 nuevos
- [ ] Controllers: 5 nuevos
- [ ] Configurations: 5 nuevas
- [ ] SQL: schemas catalogo + configuracion + 5 tablas + seed
- [ ] Program.cs: +10 DI registrations
- [ ] Endpoints tested: GET, POST, PUT, PATCH, DELETE all working
- [ ] Validations work: duplicates rejected, 404s return
- [ ] Seeds loaded: Data visible in GET endpoints

---

## 🔗 REFERENCIAS CRÍTICAS

```
plans/active/2026-05-10_catalogo-roadmap-sprints2-5.md
  └─ Roadmap completo, dependencias, timeline

IA_Docs/VALIDATOR_SERVICE_PATTERN.md
  └─ Patrón ValidatorService (OBLIGATORIO)

IA_Docs/ARCHITECTURE_DECISIONS.md
  └─ Decisiones arquitectónicas vigentes

execution-status/catalogo-base-status.md
  └─ Actualizar progreso diariamente

pending/2026-05-15_technical-backlog.md
  └─ Decisiones pendientes y riesgos
```

---

## 📝 POST-BUILD ACTIONS

1. [ ] Update `execution-status/catalogo-base-status.md`
   - Sprint 1: 0% → 100%
   - Modules: 5 completed (Pais, Moneda, UnidadMedida, ModuloSistema, ParametroSistema)

2. [ ] Create History Changed entry
   - `20260510_THHMM_feat_CompletarCRUDCatalogosBase_Sprint1`
   - SUMMARY.md con detalles

3. [ ] Execute SQL scripts
   - Create schemas
   - Create 5 tables
   - Load seeds
   - Verify indices

4. [ ] Smoke test endpoints
   - GET /api/v1/paises → 200 OK + datos
   - GET /api/v1/monedas → 200 OK + datos
   - POST /api/v1/unidades-medida → 201 Created
   - PUT endpoints → 200 OK
   - DELETE → 204 No Content

5. [ ] Commit
   - Message: `feat(catalogo): Sprint 1 CQRS completo — 5 catálogos base (Pais, Moneda, UnidadMedida, ModuloSistema, ParametroSistema)`

6. [ ] Move plan
   - `plans/active/` → `plans/completed/`
   - Create new plan for Sprint 2

---

## 🎯 BLOQUEANTES PARA SIGUIENTE SPRINT

**Ninguno.** Sprint 1 no tiene dependencias externas. Sprint 2 (Empresa, Sucursal, Almacén) depende de Sprint 1 completado.

---

**Status:** ✅ SPRINT 1 COMPLETADO — Ejecutado sin bloqueantes  
**Documento:** SPRINT_1_READY.md (Especificación Ejecutable Retroactiva)  
**Para referencia:** Patrón base para todos los catálogos posteriores  
**Siguiente:** Sprint 2 — Organización (Empresa, Sucursal, Almacén)
