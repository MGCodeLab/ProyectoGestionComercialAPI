# SPRINT 2 — READY TO BUILD 🚀

**Fecha de aprobación:** 2026-05-16  
**Estado:** ✅ LISTO PARA EJECUTAR  
**Destinatario:** Nexus-Fast-Builder  
**Acción:** Crear Empresa, Sucursal, Almacén (3 entidades)  
**Duración estimada:** 6-8 horas  
**Compilación esperada:** 0 errores, 0 advertencias

---

## ✅ DECISIONES APROBADAS

### 1️⃣ SingleTenant en Empresa
- **Opción aprobada:** A — Application-Level Guard
- **Implementación:** En `CrearEmpresaHandler`
- **Código exacto:**
```csharp
var empresaExistente = await _empresaService.ObtenerPrimera();
if (empresaExistente != null)
    throw new InvalidOperationException("Solo 1 empresa permitida en sistema");
```
- **Ventaja:** Flexible — fácil cambiar a multi-tenant después

### 2️⃣ IDs TipoDocumento Verificados
```
CE = 3
DNI = 4
RUC = 5
PASSPORT = 6
```

### 3️⃣ Patrón a Seguir
Idéntico a Sprint 1:
- Handlers: inyectan `IService`, NO `AppDbContext`
- Validators: inyectan `IValidatorService` (interface)
- Controllers: delgados, solo orquestación
- DTOs: separados por caso de uso (Crear, Actualizar, Response)
- AutoMapper: bidireccional
- ValidatorService: obligatorio para campos únicos

---

## 📋 ENTIDADES A CREAR (3)

### 1. Empresa → `organizacion.Empresas`

**Campos:**
```csharp
public class Empresa : AuditableEntity
{
    public string RazonSocial { get; set; }              // 200, obligatorio
    public string? NombreComercial { get; set; }         // 200, opcional
    public string NumeroDocumento { get; set; }          // 20, UNIQUE (RUC)
    public int TipoDocumentoId { get; set; }             // FK TipoDocumentos (RESTRICT)
    public int PaisId { get; set; }                      // FK Paises (RESTRICT)
    public int MonedaBaseId { get; set; }                // FK Monedas (RESTRICT)
    public string? DireccionFiscal { get; set; }         // 300
    public string? Telefono { get; set; }
    public string? Correo { get; set; }
    public string? LogoUrl { get; set; }
}
```

**Validador:** CrearEmpresaValidator
- RazonSocial: required, max 200
- NumeroDocumento: required, unique (ValidatorService)
- TipoDocumentoId, PaisId, MonedaBaseId: required

**Handler:** CrearEmpresaHandler
- **GUARD CRÍTICO:** Verificar SingleTenant (solo 1 empresa)
- Map command → entity
- Call service.Crear()

**DTOs:**
- CrearEmpresaDto
- ActualizarEmpresaDto
- EmpresaDto (response)

---

### 2. Sucursal → `organizacion.Sucursales`

**Campos:**
```csharp
public class Sucursal : AuditableEntity
{
    public string Nombre { get; set; }                   // 150, obligatorio
    public string Codigo { get; set; }                   // 10, UNIQUE
    public int EmpresaId { get; set; }                   // FK Empresas (RESTRICT)
    public int PaisId { get; set; }                      // FK Paises (RESTRICT)
    public string? Direccion { get; set; }               // 300
    public string? Telefono { get; set; }
    public bool EsPrincipal { get; set; }                // Solo 1 true por empresa
}
```

**Validador:** CrearSucursalValidator
- Nombre: required, max 150
- Codigo: required, unique
- EmpresaId, PaisId: required

**Handler:** CrearSucursalHandler
- Check EsPrincipal rule: solo 1 true por empresa (APPLICATION RULE, NO BD)
- Map → create

**DTOs:**
- CrearSucursalDto
- ActualizarSucursalDto
- SucursalDto

---

### 3. Almacén → `organizacion.Almacenes`

**Campos:**
```csharp
public class Almacen : AuditableEntity
{
    public string Nombre { get; set; }                   // 150, obligatorio
    public string Codigo { get; set; }                   // 10, UNIQUE
    public int SucursalId { get; set; }                  // FK Sucursales (RESTRICT)
    public string? Descripcion { get; set; }             // 500
    public bool EsPrincipal { get; set; }
}
```

**Validador:** CrearAlmacenValidator
- Nombre: required, max 150
- Codigo: required, unique
- SucursalId: required

**DTOs:**
- CrearAlmacenDto
- ActualizarAlmacenDto
- AlmacenDto

---

## 📂 ESTRUCTURA DE ARCHIVOS A CREAR

### Domain/
```
Domain/Catalogo/
├── Empresa.cs
├── Sucursal.cs
└── Almacen.cs
```

### Application/
```
Application/
├── Features/Catalogo/
│   ├── Empresa/
│   │   ├── Crear/
│   │   │   ├── CrearEmpresaCommand.cs
│   │   │   ├── CrearEmpresaHandler.cs
│   │   │   └── CrearEmpresaValidator.cs
│   │   ├── Actualizar/
│   │   │   ├── ActualizarEmpresaCommand.cs
│   │   │   ├── ActualizarEmpresaHandler.cs
│   │   │   └── ActualizarEmpresaValidator.cs
│   │   ├── ActualizarEstado/
│   │   │   ├── ActualizarEstadoEmpresaCommand.cs
│   │   │   └── ActualizarEstadoEmpresaHandler.cs
│   │   └── Eliminar/
│   │       ├── EliminarEmpresaCommand.cs
│   │       └── EliminarEmpresaHandler.cs
│   ├── Sucursal/ (estructura idéntica)
│   └── Almacen/ (estructura idéntica)
├── Dtos/Catalogo/
│   ├── CrearEmpresaDto.cs
│   ├── ActualizarEmpresaDto.cs
│   ├── EmpresaDto.cs
│   └── (repetir para Sucursal y Almacén)
├── Interfaces/
│   ├── IEmpresaValidatorService.cs
│   ├── ISucursalValidatorService.cs
│   └── IAlmacenValidatorService.cs
└── Mappings/Catalogo/
    ├── EmpresaProfile.cs
    ├── SucursalProfile.cs
    └── AlmacenProfile.cs
```

### Infrastructure/
```
Infrastructure/
├── Persistence/Configurations/
│   ├── EmpresaConfiguration.cs
│   ├── SucursalConfiguration.cs
│   └── AlmacenConfiguration.cs
├── Repository/
│   ├── EmpresaService.cs
│   ├── EmpresaValidatorService.cs
│   ├── SucursalService.cs
│   ├── SucursalValidatorService.cs
│   ├── AlmacenService.cs
│   └── AlmacenValidatorService.cs
└── AppDbContext.cs (+ 3 DbSets)
```

### Database/
```
Database/
├── 01_Schemas/
│   └── 02_Schemas.sql (CREATE SCHEMA organizacion — si no existe)
├── 02_Tablas/
│   ├── 07_Empresas.sql
│   ├── 08_Sucursales.sql
│   └── 09_Almacenes.sql
└── 03_Seeds/
    └── 08_InitEmpresaSucursalAlmacen.sql
```

### Controllers/
```
GestionComercial/Controllers/
├── EmpresasController.cs (7 endpoints)
├── SucursalesController.cs (7 endpoints)
└── AlmacenesController.cs (7 endpoints)
```

### Program.cs
```csharp
// Agregar DI registrations:
builder.Services.AddScoped<IEmpresaService, EmpresaService>();
builder.Services.AddScoped<IEmpresaValidatorService, EmpresaValidatorService>();
builder.Services.AddScoped<ISucursalService, SucursalService>();
builder.Services.AddScoped<ISucursalValidatorService, SucursalValidatorService>();
builder.Services.AddScoped<IAlmacenService, AlmacenService>();
builder.Services.AddScoped<IAlmacenValidatorService, AlmacenValidatorService>();
```

---

## 🎯 ENDPOINTS (21 total)

### Empresas
```
GET    /api/v1/empresas
GET    /api/v1/empresas/{id}
POST   /api/v1/empresas
PUT    /api/v1/empresas/{id}
PATCH  /api/v1/empresas/{id}/activar
PATCH  /api/v1/empresas/{id}/inactivar
DELETE /api/v1/empresas/{id}
```

### Sucursales
```
GET    /api/v1/sucursales
GET    /api/v1/sucursales/{id}
POST   /api/v1/sucursales
PUT    /api/v1/sucursales/{id}
PATCH  /api/v1/sucursales/{id}/activar
PATCH  /api/v1/sucursales/{id}/inactivar
DELETE /api/v1/sucursales/{id}
```

### Almacenes
```
GET    /api/v1/almacenes
GET    /api/v1/almacenes/{id}
POST   /api/v1/almacenes
PUT    /api/v1/almacenes/{id}
PATCH  /api/v1/almacenes/{id}/activar
PATCH  /api/v1/almacenes/{id}/inactivar
DELETE /api/v1/almacenes/{id}
```

---

## ✅ CHECKLIST PRE-BUILD

- [ ] Leer `plans/active/2026-05-10_catalogo-roadmap-sprints2-5.md` (contexto general)
- [ ] Leer `IA_Docs/VALIDATOR_SERVICE_PATTERN.md` (patrón obligatorio)
- [ ] Compilar proyecto baseline (verify 0 errores)
- [ ] Verificar Domain/Catalogo/ existe (para colocar entidades nuevas)
- [ ] Verificar Application/Features/Catalogo/ existe
- [ ] Verificar Infrastructure/Repository/ existe
- [ ] Verificar Database/ estructura (01_Schemas, 02_Tablas, 03_Seeds)

---

## 🔄 PATRÓN EXACTO A SEGUIR

### Handler Pattern (Copy-Paste)
```csharp
public class CrearEmpresaHandler : IRequestHandler<CrearEmpresaCommand, Result<int>>
{
    private readonly IEmpresaService _service;
    private readonly IMapper _mapper;
    private readonly ILogger<CrearEmpresaHandler> _logger;

    public async Task<Result<int>> Handle(CrearEmpresaCommand request, CancellationToken ct)
    {
        // GUARD: SingleTenant
        var empresaExistente = await _service.ObtenerPrimera();
        if (empresaExistente != null)
            throw new InvalidOperationException("Solo 1 empresa permitida");

        // Map
        var empresa = _mapper.Map<Empresa>(request);
        
        // Persist
        var resultado = await _service.Crear(empresa);
        
        // Log
        _logger.LogInformation($"Empresa creada: {empresa.Id}");
        
        return Result<int>.Success(resultado);
    }
}
```

### Validator Pattern (Copy-Paste)
```csharp
public class CrearEmpresaValidator : AbstractValidator<CrearEmpresaCommand>
{
    private readonly IEmpresaValidatorService _validator;

    public CrearEmpresaValidator(IEmpresaValidatorService validator)
    {
        _validator = validator;

        RuleFor(x => x.RazonSocial)
            .NotEmpty().WithMessage("Razón social requerida")
            .MaximumLength(200);

        RuleFor(x => x.NumeroDocumento)
            .NotEmpty()
            .MustAsync(BeUniqueDocumento)
            .WithMessage("Número documento ya existe");
    }

    private async Task<bool> BeUniqueDocumento(string numDoc, CancellationToken ct)
        => await _validator.IsNumeroDocumentoUnique(numDoc, ct);
}
```

### Service Pattern (Copy-Paste)
```csharp
public interface IEmpresaService
{
    Task<Empresa?> ObtenerPorId(int id, bool tracking = false);
    Task<Empresa?> ObtenerPrimera();
    Task<List<Empresa>> ObtenerTodos();
    Task<int> Crear(Empresa empresa);
    Task Actualizar(Empresa empresa);
    Task Eliminar(int id);
}
```

---

## 🚨 CRITICAL RULES

1. **SingleTenant Guard:** Implementar en CrearEmpresaHandler (Opción A aprobada)
2. **EsPrincipal Rule:** Solo 1 true por empresa/sucursal — APPLICATION RULE, no constraint BD
3. **FK Strategy:** RESTRICT en todos (no cascadas)
4. **Soft Delete:** Patrón Activo = auditoría (GET retorna todos)
5. **ValidatorService:** Obligatorio para NumeroDocumento (Empresa), Codigo (Sucursal, Almacén)
6. **No Hardcoding:** Usar IDs correctos: 3, 4, 5, 6 para TipoDocumento
7. **AuditableEntity:** Todas las entidades heredan de AuditableEntity

---

## 📊 SUCCESS CRITERIA

- [ ] Compilación: 0 errores, 0 advertencias
- [ ] Endpoints: 21 totales (7 × 3 entidades)
- [ ] Handlers: 12 nuevos (4 × 3)
- [ ] Validators: 6 nuevos (2 × 3)
- [ ] ValidatorServices: 3 nuevos
- [ ] DTOs: 9 nuevos (3 × 3)
- [ ] Services: 3 nuevos
- [ ] Controllers: 3 nuevos
- [ ] Configuration: 3 nuevas
- [ ] SQL: schema organizacion + 3 tablas + seed
- [ ] Program.cs: +6 DI registrations
- [ ] Endpoints tested: GET, POST, PUT, PATCH, DELETE
- [ ] Validations work: duplicates rejected, 404s return
- [ ] SingleTenant guard: POST segunda empresa rechazada

---

## 🔗 REFERENCIAS CRÍTICAS

```
plans/active/2026-05-10_catalogo-roadmap-sprints2-5.md
  └─ Roadmap completo, dependencias, timeline

IA_Docs/VALIDATOR_SERVICE_PATTERN.md
  └─ Patrón ValidatorService (OBLIGATORIO)

IA_Docs/ARCHITECTURE_DECISIONS.md
  └─ Decisiones arquitectónicas (ADR-001 a ADR-010)

execution-status/catalogo-base-status.md
  └─ Actualizar progreso diariamente

pending/2026-05-15_technical-backlog.md
  └─ PD-02.5 (SingleTenant) aprobado Opción A
```

---

## 📝 POST-BUILD ACTIONS

1. [ ] Update `execution-status/catalogo-base-status.md`
   - Sprint 2: 0% → 100%
   - Modules: 3 completed (Empresa, Sucursal, Almacén)

2. [ ] Create History Changed entry
   - `20260516_THHMM_feat_Sprint2Organizacion`
   - SUMMARY.md con detalles

3. [ ] Commit
   - Message: `feat(catalogo): Sprint 2 — Organización (Empresa, Sucursal, Almacén)`

4. [ ] Move plan
   - `plans/active/` → actualizar progreso
   - O crear nuevo plan para Sprint 3

---

**Status:** ✅ LISTO PARA NEXUS-FAST-BUILDER  
**Documento:** SPRINT_2_READY.md  
**Para usar:** POST-COMPACT  
**Siguiente:** /compact → Nexus-Fast-Builder inicia ejecución

