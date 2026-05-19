# ValidatorService Pattern — Guía de Implementación

**Última actualización:** 2026-05-10  
**Relevancia:** Crítica para mantener Clean Architecture en validaciones async  
**Aplicable a:** Todas las entidades con campos únicos o validaciones contra BD

---

## El Problema

En Clean Architecture, **Application layer NO DEBE importar Infrastructure.Persistence** (incluido EF Core).

Pero las validaciones async contra base de datos requieren EF Core:

```csharp
// ❌ PROBLEMA: Validator con AppDbContext directo
public class CrearPaisValidator : AbstractValidator<CrearPaisCommand>
{
    private readonly AppDbContext _context;  // ← VIOLATION: Application importa Infrastructure
    
    public CrearPaisValidator(AppDbContext context)
    {
        RuleFor(x => x.Codigo)
            .MustAsync(async (codigo, ct) =>
                !await _context.Paises.AnyAsync(p => p.Codigo == codigo, ct)
            );
    }
}
```

**Problemas con este enfoque:**
1. Application importa Infrastructure.Persistence (violación Clean Architecture)
2. Difícil de testear (necesitas mock de AppDbContext)
3. Duplicación si múltiples validators validan lo mismo
4. Tight coupling a EF Core

---

## La Solución: ValidatorService Pattern

Delegar la lógica de persistencia a un **Service en Infrastructure**, que se inyecta via **interface en Application**.

### Arquitectura

```
Application Layer (NO EF Core)
├── Validator
│   ├── inyecta: IXxxValidatorService (interface)
│   └── usa: await _validator.IsCodigoUnique(codigo)
│
Infrastructure Layer (CON EF Core)
└── ValidatorService
    ├── implementa: IXxxValidatorService
    ├── inyecta: AppDbContext (privado)
    └── método: public async Task<bool> IsCodigoUnique(string codigo)
        → return !await _context.Paises.AnyAsync(...);
```

---

## Implementación Paso a Paso

### 1. Crear Interface en Application

**Ubicación:** `Application/Interfaces/IXxxValidatorService.cs`

```csharp
namespace Application.Interfaces;

public interface IMonedaValidatorService
{
    /// <summary>Verifica que el código ISO sea único en toda la tabla</summary>
    Task<bool> IsCodigoISOneUnique(string codigoISO, CancellationToken cancellationToken);
    
    /// <summary>Verifica que el código ISO sea único excepto para el ID especificado</summary>
    Task<bool> IsCodigoISOneUniqueExcept(int monedaId, string codigoISO, CancellationToken cancellationToken);
}
```

**Convención:**
- Métodos nombrados: `Is{Campo}Unique()` para CREATE
- Métodos nombrados: `Is{Campo}UniqueExcept()` para UPDATE
- Siempre retornan `Task<bool>` (true = válido, false = duplicado)

---

### 2. Crear Implementación en Infrastructure

**Ubicación:** `Infrastructure/Repository/XxxValidatorService.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using Infrastructure.Persistence;

namespace Infrastructure.Repository;

public class MonedaValidatorService : IMonedaValidatorService
{
    private readonly AppDbContext _context;

    public MonedaValidatorService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsCodigoISOneUnique(string codigoISO, CancellationToken cancellationToken)
    {
        return !await _context.Monedas
            .AnyAsync(m => m.CodigoISO == codigoISO.ToUpper(), cancellationToken);
    }

    public async Task<bool> IsCodigoISOneUniqueExcept(int monedaId, string codigoISO, CancellationToken cancellationToken)
    {
        return !await _context.Monedas
            .AnyAsync(m => m.CodigoISO == codigoISO.ToUpper() && m.Id != monedaId, cancellationToken);
    }
}
```

**Patrón:**
- `AnyAsync()` con `.ToUpper()` para case-insensitive
- Excepción por ID para ediciones
- Return `!Any()` (true = no existe = válido)

---

### 3. Usar en Validator

**En Application/Features/Xxx/Crear/CrearXxxValidator.cs:**

```csharp
using FluentValidation;
using Application.Interfaces;

namespace Application.Features.Catalogo.Moneda.Crear;

public class CrearMonedaValidator : AbstractValidator<CrearMonedaCommand>
{
    private readonly IMonedaValidatorService _validatorService;  // ← Interface, NO impl

    public CrearMonedaValidator(IMonedaValidatorService validatorService)
    {
        _validatorService = validatorService;

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("Nombre requerido")
            .MaximumLength(100);

        RuleFor(x => x.CodigoISO)
            .NotEmpty()
            .MustAsync(BeUniqueCode)  // ← Delegado a validator service
            .WithMessage("Código ISO ya existe");
    }

    private async Task<bool> BeUniqueCode(string codigoISO, CancellationToken cancellationToken)
    {
        return await _validatorService.IsCodigoISOneUnique(codigoISO, cancellationToken);
    }
}
```

**En Application/Features/Xxx/Actualizar/ActualizarXxxValidator.cs:**

```csharp
public class ActualizarMonedaValidator : AbstractValidator<ActualizarMonedaCommand>
{
    private readonly IMonedaValidatorService _validatorService;

    public ActualizarMonedaValidator(IMonedaValidatorService validatorService)
    {
        _validatorService = validatorService;

        RuleFor(x => x.CodigoISO)
            .NotEmpty()
            .MustAsync((command, codigoISO, ct) => 
                BeUniqueCodeExcept(command, codigoISO, ct))  // ← Pasa command para extraer ID
            .WithMessage("Código ISO ya existe");
    }

    private async Task<bool> BeUniqueCodeExcept(
        ActualizarMonedaCommand command, 
        string codigoISO, 
        CancellationToken cancellationToken)
    {
        return await _validatorService.IsCodigoISOneUniqueExcept(command.Id, codigoISO, cancellationToken);
    }
}
```

---

### 4. Registrar en Program.cs

**Ubicación:** `GestionComercial/Program.cs` línea ~40

```csharp
// Registrar ValidatorServices (Infrastructure)
builder.Services.AddScoped<IMonedaValidatorService, MonedaValidatorService>();
builder.Services.AddScoped<IUnidadMedidaValidatorService, UnidadMedidaValidatorService>();
builder.Services.AddScoped<IModuloSistemaValidatorService, ModuloSistemaValidatorService>();
builder.Services.AddScoped<IParametroSistemaValidatorService, ParametroSistemaValidatorService>();
builder.Services.AddScoped<IPaisValidatorService, PaisValidatorService>();
```

---

## Ventajas

| Ventaja | Impacto |
|---------|---------|
| **Clean Architecture** | Application NO importa Infrastructure |
| **Testabilidad** | Mock fácil de IXxxValidatorService en tests |
| **Reutilización** | Lógica de validación centralizada |
| **Mantenibilidad** | Un solo lugar para cambiar reglas de unicidad |
| **Flexibilidad** | Cambiar a otro ORM = solo implementación Service |
| **Escalabilidad** | Agregar validators sin duplicar EF Core |

---

## Checklist para Nueva Entidad

Cuando implementes ValidatorService para una entidad nueva:

- [ ] Crear `IXxxValidatorService` en Application/Interfaces
- [ ] Implementar `XxxValidatorService` en Infrastructure/Repository
- [ ] Inyectar `IXxxValidatorService` en CrearXxxValidator
- [ ] Inyectar `IXxxValidatorService` en ActualizarXxxValidator
- [ ] Registrar en Program.cs: `AddScoped<IXxxValidatorService, XxxValidatorService>()`
- [ ] Compilar: `dotnet build` → 0 errores
- [ ] Test: Validator rechaza duplicados, acepta únicos

---

## Casos de Uso

### Caso 1: Campo Código Único (la mayoría)
```csharp
// Interface
Task<bool> IsCodigoUnique(string codigo, CancellationToken ct);
Task<bool> IsCodigoUniqueExcept(int id, string codigo, CancellationToken ct);

// Implementación
public async Task<bool> IsCodigoUnique(string codigo, CancellationToken ct)
    => !await _context.Tablas.AnyAsync(x => x.Codigo == codigo.ToUpper(), ct);
```

### Caso 2: Email Único con NULL Permitido
```csharp
// Usar filtered index en SQL, no constraint
// Verificar en validator solo si email no es null
RuleFor(x => x.Email)
    .EmailAddress()
    .When(x => !string.IsNullOrEmpty(x.Email))  // Solo valida si tiene valor
    .MustAsync(BeUniqueEmail)
    .WithMessage("Email ya existe");

// Implementación
public async Task<bool> IsEmailUnique(string email, CancellationToken ct)
{
    // Email null pasa automáticamente (no entra al MustAsync)
    return !await _context.Tablas
        .AnyAsync(x => x.Email == email, ct);
}
```

### Caso 3: Combinación de Campos Única
```csharp
// Interface
Task<bool> IsCodigoSerieLaUnique(int tipoDocId, int sucursalId, string serie, CancellationToken ct);

// Implementación
public async Task<bool> IsCodigoSerieLaUnique(...)
{
    return !await _context.SeriesDocumento
        .AnyAsync(x => x.TipoComprobanteId == tipoDocId 
                    && x.SucursalId == sucursalId 
                    && x.Serie == serie, ct);
}
```

---

## Errores Comunes a Evitar

### ❌ Error 1: ValidatorService en Application
```csharp
// ❌ NUNCA hacer esto
namespace Application.Services;  // ← INCORRECTO
public class MonedaValidatorService { }
```

**Por qué:** Application NO debe tener lógica de persistencia. El servicio DEBE estar en Infrastructure.

### ❌ Error 2: No Registrar en DI
```csharp
// ❌ Si falta en Program.cs
builder.Services.AddScoped<IMonedaValidatorService, MonedaValidatorService>();
```

**Resultado:** Runtime error "No service for type IMonedaValidatorService"

### ❌ Error 3: ToUpper() Inconsistente
```csharp
// ❌ INCORRECTO (diferentes en interface e implementación)
// Interface: IsCodigoUnique(string codigo)
// Implementation: codigo.ToUpper() en algunos, sin ToUpper() en otros
```

**Resultado:** Búsquedas case-sensitive rompen validación

### ❌ Error 4: Olvidar CancellationToken
```csharp
// ❌ SIN cancellationToken
public async Task<bool> IsCodigoUnique(string codigo)
    => !await _context.Paises.AnyAsync(p => p.Codigo == codigo);

// ✅ CON cancellationToken
public async Task<bool> IsCodigoUnique(string codigo, CancellationToken ct)
    => !await _context.Paises.AnyAsync(p => p.Codigo == codigo, ct);
```

---

## Referencias

- **Pattern origin:** Domain-Driven Design + Clean Architecture
- **Validación async:** FluentValidation `MustAsync()`
- **Inyección:** Dependency Injection en Program.cs
- **Aplicado en:** Pais, Moneda, UnidadMedida, ModuloSistema, ParametroSistema

---

**Última revisión:** 2026-05-10  
**Próxima revisión:** Después de implementar Sprint 2 (si hay nuevos patrones)
