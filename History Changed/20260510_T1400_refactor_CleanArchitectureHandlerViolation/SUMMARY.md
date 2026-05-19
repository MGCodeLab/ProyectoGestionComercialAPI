# Refactor: Clean Architecture Handler Violation — P-03 Resolved

**Fecha:** 2026-05-10 (Sesión 2, continuación)  
**Rama:** `catalogo-base/sprint_1` (sin commit)  
**Severidad:** Crítica (bloqueante para compilación)  
**Estado:** ✅ RESUELTO

---

## PROBLEMA ORIGINAL (P-03)

### Síntoma
Handlers de Pais importaban directamente `Infrastructure.Persistence.AppDbContext` desde Application layer, violando Clean Architecture:

```csharp
// ❌ INCORRECTO - Application/Features/Catalogo/Pais/Crear/CrearPaisHandler.cs
using Infrastructure.Persistence;  // VIOLACIÓN: Application no debe referenciar Infrastructure concreto
private readonly AppDbContext _context;
```

### Impacto
- 20+ errores de compilación CS0246 (tipos no encontrados)
- Violación de Clean Architecture (dependencia circular posible)
- Imposibilidad de cambiar implementación de persistencia sin tocar Application

### Causa Raíz
Generación inicial de Handlers con patrón incorrecto, sin seguir cliente patrón (ClienteHandler) que usaba servicios.

---

## SOLUCIÓN APLICADA

### 1. Refactor de Handlers (4 archivos)
Cambio de inyectar `AppDbContext` a inyectar `IPaisService`:

**Antes:**
```csharp
private readonly AppDbContext _context;

public async Task<int> Handle(CrearPaisCommand request, CancellationToken cancellationToken)
{
    var pais = _mapper.Map<Domain.Catalogo.Pais>(request);
    pais.FechaRegistro = DateTime.UtcNow;
    _context.Paises.Add(pais);
    await _context.SaveChangesAsync(cancellationToken);
    return pais.Id;
}
```

**Después:**
```csharp
private readonly IPaisService _service;

public async Task<int> Handle(CrearPaisCommand request, CancellationToken cancellationToken)
{
    var pais = _mapper.Map<Domain.Catalogo.Pais>(request);
    return await _service.Crear(pais, cancellationToken);
}
```

**Handlers modificados:**
- `CrearPaisHandler.cs` — Cambio directo a `_service.Crear()`
- `ActualizarPaisHandler.cs` — Búsqueda con tracking + `_service.Actualizar()`
- `ActualizarEstadoPaisHandler.cs` — Mismo patrón
- `EliminarPaisHandler.cs` — Búsqueda sin tracking + `_service.Eliminar()`

### 2. Refactor de Validators (2 archivos)
Migración de EF dependency de Application a Infrastructure mediante ValidatorService:

**Antes:**
```csharp
private readonly AppDbContext _context;  // ❌ EF Core en Application

public async Task<bool> BeUniqueCode(string codigo, CancellationToken cancellationToken)
{
    var exists = await _context.Paises
        .AnyAsync(p => p.Codigo == codigo.ToUpper(), cancellationToken);
    return !exists;
}
```

**Después:**
```csharp
private readonly IPaisValidatorService _validatorService;  // ✅ Interface en Application

public async Task<bool> BeUniqueCode(string codigo, CancellationToken cancellationToken)
{
    return await _validatorService.IsCodigoUnique(codigo, cancellationToken);
}
```

**Archivos creados:**
- `Application/Interfaces/IPaisValidatorService.cs` — Interface
- `Infrastructure/Repository/PaisValidatorService.cs` — Implementación

### 3. Corrección de Namespaces
Se identificó problema secundario: algunos archivos creados tenían namespaces con prefijo "Nexus." que no existe en el proyecto:

**Archivos corregidos:**
- `PaisService.cs` — `Nexus.Application.Interfaces` → `Application.Interfaces`
- `MonedaService.cs` — Mismo cambio
- `UnidadMedidaService.cs` — Mismo cambio
- `ModuloSistemaService.cs` — Mismo cambio
- `ParametroSistemaService.cs` — Mismo cambio

**Patrón de error identificado:**
```csharp
// ❌ INCORRECTO (todos los Services nuevos)
using Nexus.Application.Interfaces;
using Nexus.Domain.Catalogo;
using Nexus.Infrastructure.Persistence;
namespace Nexus.Infrastructure.Repository;

// ✅ CORRECTO (patrón existente del proyecto)
using Application.Interfaces;
using Domain.Catalogo;
using Infrastructure.Persistence;
namespace Infrastructure.Repository;
```

### 4. Corrección de Configuration EF
Removido campo redundante `EsActivo` de `ModuloSistemaConfiguration.cs`:
```csharp
// ❌ ANTES
builder.Property(m => m.EsActivo)
    .IsRequired()
    .HasDefaultValue(true);

// ✅ DESPUÉS (removido — campo heredado de AuditableEntity es suficiente)
```

### 5. Corrección de Extension Methods
Agregar `using API.GestionComercial.Extensions;` a Controller de Paises para acceso a `OkResponse()`, `NotFoundResponse()`, etc.

---

## RESULTADO FINAL

### Compilación
- ✅ `dotnet build` → 0 errores, 0 advertencias
- ✅ Todos los Handlers usando Services (no AppDbContext)
- ✅ Todos los Validators usando ValidatorService (no EF Core directo)
- ✅ Arquitectura Clean respetada en todas las capas

### Patrón de Referencia Establecido
El patrón de Pais ahora es el correcto para replicar en otras entidades:
1. Handler inyecta `IService` (no AppDbContext)
2. Validator inyecta `IValidatorService` (interfaz, no EF)
3. Service en Infrastructure encapsula persistencia

---

## LECCIONES APRENDIDAS

### Regla 1: Namespaces del Proyecto
**Problema:** Generé archivos con `using Nexus.*` cuando proyecto usa `*` sin prefijo.  
**Causa:** Asumí patrón incorrecto durante creación inicial.  
**Solución:** Revisar siempre archivos existentes (ej: ProductoService.cs) para validar convención.  
**Regla futura:** `grep -r "^namespace"` en carpeta para confirmar patrón antes de crear.

### Regla 2: Service-Based Handlers
**Problema:** Handlers originales inyectaban AppDbContext directamente.  
**Causa:** No seguí patrón ClienteHandler como referencia.  
**Solución:** Siempre copiar patrón de módulo completo (Cliente, Producto) cuando es disponible.  
**Regla futura:** "No inventar patrones nuevos — clonar referencias existentes."

### Regla 3: Validator Services
**Problema:** Validators necesitaban EF Core para validaciones async, violando Clean Architecture.  
**Solución:** Crear ValidatorService en Infrastructure, inyectar interface en Application.  
**Regla futura:** Toda validación async (contra BD) → Service externo, no Validator directo.

---

## ARCHIVOS MODIFICADOS

| Archivo | Cambio | Razón |
|---------|--------|-------|
| `CrearPaisHandler.cs` | AppDbContext → IPaisService | Eliminar dependency circularidad |
| `ActualizarPaisHandler.cs` | Mismo patrón | Consistencia |
| `ActualizarEstadoPaisHandler.cs` | Mismo patrón | Consistencia |
| `EliminarPaisHandler.cs` | Mismo patrón | Consistencia |
| `CrearPaisValidator.cs` | AppDbContext → IPaisValidatorService | Aislar EF Core |
| `ActualizarPaisValidator.cs` | Mismo patrón | Consistencia |
| `PaisService.cs` | Namespaces Nexus.* → * | Corregir convención |
| `MonedaService.cs` | Mismo cambio | Corregir convención |
| `UnidadMedidaService.cs` | Mismo cambio | Corregir convención |
| `ModuloSistemaService.cs` | Mismo cambio | Corregir convención |
| `ParametroSistemaService.cs` | Mismo cambio | Corregir convención |
| `ModuloSistemaConfiguration.cs` | Removido EsActivo property | Usuario feedback (redundancia) |
| `PaisesController.cs` | Agregado using Extensions | Acceso a OkResponse() |
| `Program.cs` | Agregado IPaisValidatorService DI | Registrar servicio |

## ARCHIVOS CREADOS

- `Application/Interfaces/IPaisValidatorService.cs`
- `Infrastructure/Repository/PaisValidatorService.cs`
- `Application/Interfaces/IMonedaValidatorService.cs` (para Moneda)
- `Infrastructure/Repository/MonedaValidatorService.cs` (para Moneda)

---

## PRÓXIMOS PASOS

1. Completar CRUD (Actualizar, ActualizarEstado, Eliminar) para Moneda, UnidadMedida, ModuloSistema, ParametroSistema
2. Crear ValidatorService para las otras 4 entidades (siguiendo patrón IPaisValidatorService)
3. Crear Actualizar handlers y validators para otras entidades
4. Smoke testing en todos los endpoints
5. Commit final con referencia a P-03

---

**Sesión:** 2026-05-10 (Parte 2)  
**Estado:** ✅ P-03 RESUELTO — Compilación limpia lograda  
**Impacto:** Sprint 1 bloqueante removido, ahora puede continuar con DTOs/Commands/Handlers para otras 4 entidades
