# Namespace Conventions & Common Pitfalls

**Última actualización:** 2026-05-10  
**Relevancia:** Crítica para nuevas features y módulos

---

## Convención Correcta del Proyecto

Este proyecto **NO usa prefijo "Nexus"** en sus namespaces. La convención es usar los nombres de la capa/carpeta directamente:

```csharp
// ✅ CORRECTO
using Application.Interfaces;
using Application.Features.Clientes;
using Domain.Comercial;
using Infrastructure.Persistence;
using Infrastructure.Repository;

namespace Application.Features.Catalogo.Pais.Crear;
namespace Infrastructure.Repository;
namespace Domain.Catalogo;
```

```csharp
// ❌ INCORRECTO (NO HACER)
using Nexus.Application.Interfaces;
using Nexus.Domain.Comercial;
using Nexus.Infrastructure.Persistence;

namespace Nexus.Infrastructure.Repository;
namespace Nexus.Domain.Catalogo;
```

---

## Raíz de Este Problema (Hallazgo 2026-05-10)

### Cuándo Sucedió
Durante creación inicial de servicios para catálogos base (Pais, Moneda, UnidadMedida, etc.) en Sprint 1.

### Por Qué Sucedió
Se asumió incorrectamente que el proyecto usaba prefijo "Nexus" en namespaces, posiblemente porque:
1. Early proyecto tenía `Nexus.ERP` como nombre
2. Confusión con el nombre de la solución vs. convención interna
3. No se revisó archivo de referencia antes de crear (ej: ProductoService.cs)

### Impacto
5 archivos Service creados con namespaces incorrectos:
```csharp
namespace Nexus.Infrastructure.Repository;  // ❌ INCORRECTO
```

Esto causó errores de compilación:
```
CS0234: El tipo o el nombre del espacio de nombres 'Application' 
no existe en el espacio de nombres 'Nexus'
```

### Solución Aplicada (2026-05-10, 14:00)
Bulk replacement en 5 archivos:
- `PaisService.cs`
- `MonedaService.cs`
- `UnidadMedidaService.cs`
- `ModuloSistemaService.cs`
- `ParametroSistemaService.cs`

```bash
# Cambio aplicado en cada archivo
using Nexus.Application.Interfaces  →  using Application.Interfaces
using Nexus.Domain.*                →  using Domain.*
using Nexus.Infrastructure.*        →  using Infrastructure.*
namespace Nexus.Infrastructure.*    →  namespace Infrastructure.*
```

---

## Cómo Evitarlo en Futuro

### Antes de Crear Nuevos Archivos

1. **Buscar referencia existente en la misma capa:**
   ```bash
   grep -r "^namespace" Infrastructure/Repository/*.cs | head -1
   # Output: namespace Infrastructure.Repository;
   ```

2. **Verificar imports en archivo de referencia:**
   ```bash
   head -10 Infrastructure/Repository/ProductoService.cs
   # Verifica: using Application.Interfaces; (sin Nexus)
   ```

3. **Copiar encabezado de archivo existente** cuando generes nuevo:
   ```csharp
   // Copiar desde ProductoService.cs
   using Microsoft.EntityFrameworkCore;
   using Application.Interfaces;
   using Domain.Comercial;  // ← Nota: sin Nexus
   using Infrastructure.Persistence;

   namespace Infrastructure.Repository;  // ← Sin Nexus
   ```

### Si Generaste Incorrectamente

```bash
# Encontrar todos los archivos problemáticos
grep -r "using Nexus\." Application/ Infrastructure/ Domain/ | cut -d: -f1 | sort -u

# Luego usar sed para bulk fix
sed -i 's/using Nexus\./using /g' archivo.cs
sed -i 's/namespace Nexus\./namespace /g' archivo.cs
```

---

## Patrón de Carpetas vs. Namespaces

La convención es que **namespace siga la estructura de carpetas exactamente**:

```
Domain/
├── Comercial/
│   ├── Cliente.cs           → namespace Domain.Comercial;
│   └── Producto.cs          → namespace Domain.Comercial;
├── Catalogo/
│   ├── Pais.cs              → namespace Domain.Catalogo;
│   └── Moneda.cs            → namespace Domain.Catalogo;
└── Configuracion/
    └── ModuloSistema.cs     → namespace Domain.Configuracion;

Application/
├── Dtos/
│   └── Catalogo/
│       └── PaisDto.cs       → namespace Application.Dtos.Catalogo;
├── Features/
│   ├── Clientes/
│   │   └── ...              → namespace Application.Features.Clientes.Crear;
│   └── Catalogo/
│       └── Pais/
│           └── Crear/
│               └── CrearPaisHandler.cs  → namespace Application.Features.Catalogo.Pais.Crear;
├── Interfaces/
│   ├── IClienteService.cs   → namespace Application.Interfaces;
│   └── IPaisService.cs      → namespace Application.Interfaces;
└── Mappings/
    └── Catalogo/
        └── PaisProfile.cs   → namespace Application.Mappings.Catalogo;

Infrastructure/
├── Repository/
│   ├── ClienteService.cs    → namespace Infrastructure.Repository;
│   └── PaisService.cs       → namespace Infrastructure.Repository;
└── Persistence/
    ├── AppDbContext.cs      → namespace Infrastructure.Persistence;
    └── Configurations/
        └── PaisConfiguration.cs  → namespace Infrastructure.Persistence.Configurations;

GestionComercial/
└── Controllers/
    ├── ClientesController.cs   → namespace API.GestionComercial.Controllers;
    └── PaisesController.cs     → namespace API.GestionComercial.Controllers;
```

**Regla simple:** `namespace = Raiz/Carpeta1/Carpeta2/...`

---

## Checklist para Nuevos Módulos

- [ ] ¿Verifiqué namespace en archivo de referencia similar?
- [ ] ¿Mi nuevo archivo usa `using Application.*` (sin Nexus)?
- [ ] ¿Mi nuevo archivo usa `using Domain.*` (sin Nexus)?
- [ ] ¿Mi namespace NO tiene prefijo Nexus?
- [ ] ¿Mi namespace coincide con estructura de carpetas?
- [ ] ¿Compilación exitosa? (`dotnet build` → 0 errores)

---

## Referencia de Archivos Correctos

Usar estos como plantilla:
- `Infrastructure/Repository/ProductoService.cs` — Correcto namespace/imports
- `Infrastructure/Repository/ClienteService.cs` — Patrón estándar
- `Application/Features/Clientes/Crear/CrearClienteHandler.cs` — Handler correcto
- `Domain/Comercial/Cliente.cs` — Entity correcto

