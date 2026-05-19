# Sprint 5: Comercial (CondicionPago, ListaPrecio, Proveedor) — ✅ IMPLEMENTADO

**Versión:** 1.0 COMPLETADO  
**Fecha Especificación:** 2026-05-18  
**Fecha Inicio Real:** 2026-05-18  
**Fecha Completitud Real:** 2026-05-18 14:00 UTC  
**Duración Real:** ~3.5 horas (50% FASTER than estimated 6-7h)  
**Estado:** ✅ **COMPLETADO 100% — CATÁLOGOS BASE FINALIZADOS**  
**Arquitecto:** Nexus Backend Architect  
**Implementador:** Nexus Fast Builder + Miguel Gonzalez (SQL execution + testing)  
**Rama:** `catalogo-base/sprint_5`  
**Complejidad:** 🟢 **BAJA** (Patrones conocidos, clone de Cliente, sin complejidad especial)

---

## 📋 OBJETIVO

Completar catálogos comerciales base (Fase 4) que sustentan módulo Ventas v3.1:
- **CondicionPago**: Términos de crédito (Contado, 15 días, 30 días, 60 días)
- **ListaPrecio**: Catálogos de listas de precios por moneda (sin detalles de productos — PD-02 diferido)
- **Proveedor**: Maestro de proveedores (patrón idéntico a Cliente)

**Resultado:** 18 entidades de catálogos completadas (Sprint 1-5). Módulo Ventas v3.1 desbloqueado.

**Decisiones Arquitectónicas:**
- ✅ **PD-02:** ListaPrecioDetalle diferido a Ventas (Sprint 6+) — DECIDIDO 2026-05-18
- ✅ **ADR-011:** Documentado en IA_Docs/ARCHITECTURE_DECISIONS.md
- ✅ **Sin ProductoPresentacion:** Abstracción por ProductoId, escalable a presentaciones sin breaking changes

---

## 📍 ALCANCE DE IMPLEMENTACIÓN

### Entidades Nuevas (3)
- **CondicionPago** → `catalogo.CondicionesPago` (catálogo simple)
- **ListaPrecio** → `catalogo.ListasPrecios` (catálogo por moneda)
- **Proveedor** → `comercial.Proveedores` (clon de Cliente)

### Artefactos Totales
- **CQRS:** 12 Commands + 12 Handlers + 6 Validators
- **DTOs:** 9 nuevos (3 × 3 entidades)
- **Servicios:** 6 nuevos (3 Services + 3 ValidatorServices)
- **Controllers:** 3 nuevos (21 endpoints totales: 7 × 3 entidades)
- **Mappings:** 3 nuevos (AutoMapper profiles)
- **Configurations:** 3 nuevas (Entity Configurations)
- **SQL Scripts:** 4 nuevos (3 tablas + 1 seed)
- **Archivos nuevos:** ~28 totales

### Modificaciones Existentes
- **Program.cs:** +6 DI registrations (3 services + 3 validators)
- **AppDbContext.cs:** +3 DbSets (CondicionesP ago, ListasPrecios, Proveedores)
- **Appsettings/Startup:** Sin cambios

---

## 🎯 ENTIDADES & ESPECIFICACIÓN TÉCNICA

### 1. CondicionPago → `catalogo.CondicionesPago`

#### Domain Entity
```csharp
namespace Domain.Catalogo
{
    public class CondicionPago : AuditableEntity
    {
        public string Nombre { get; set; }                     // 100, obligatorio
        public int DiasCredito { get; set; }                   // 0=Contado, 15=15 días, etc.
        public string? Descripcion { get; set; }               // 500, opcional
    }
}
```

#### Configuration
```
Schema: catalogo
Table: CondicionesPago
Constraints:
- PK: Id (INT)
- Unique: Nombre (opcional)
- Indices: DiasCredito, Activo
- Default: Activo = 1, DiasCredito = 0
```

#### DTOs
- `CrearCondicionPagoDto`: Nombre, DiasCredito, Descripcion?
- `ActualizarCondicionPagoDto`: Nombre, DiasCredito, Descripcion?
- `CondicionPagoDto`: Full response con PublicId

#### Validaciones
- **Nombre:** required, max 100 chars
- **DiasCredito:** non-negative, 0-999
- **Descripcion:** optional, max 500 chars

#### Seed Data
```sql
INSERT INTO catalogo.CondicionesPago (Nombre, DiasCredito, Descripcion, Activo)
VALUES
('Contado', 0, 'Pago inmediato', 1),
('15 Días', 15, 'Crédito a 15 días', 1),
('30 Días', 30, 'Crédito a 30 días', 1),
('60 Días', 60, 'Crédito a 60 días', 1),
('90 Días', 90, 'Crédito a 90 días', 1);
```

#### Endpoints (7 estándar)
```
GET    /api/v1/condiciones-pago              → Listar (con paginación)
GET    /api/v1/condiciones-pago/{id}          → Obtener por ID
POST   /api/v1/condiciones-pago               → Crear
PUT    /api/v1/condiciones-pago/{id}          → Actualizar
PATCH  /api/v1/condiciones-pago/{id}/activar  → Activar
PATCH  /api/v1/condiciones-pago/{id}/inactivar → Inactivar
DELETE /api/v1/condiciones-pago/{id}          → Eliminar (soft)
```

---

### 2. ListaPrecio → `catalogo.ListasPrecios`

#### Domain Entity
```csharp
namespace Domain.Catalogo
{
    public class ListaPrecio : AuditableEntity
    {
        public string Nombre { get; set; }                     // 150, obligatorio
        public int MonedaId { get; set; }                      // FK Monedas (requerido)
        public string? Descripcion { get; set; }               // 500, opcional
        public bool EsDefault { get; set; }                    // Solo 1 puede ser true
        
        // Navigation property
        public virtual Moneda Moneda { get; set; }
    }
}
```

#### Configuration
```
Schema: catalogo
Table: ListasPrecios
Constraints:
- PK: Id (INT)
- FK: MonedaId → catalogo.Monedas (RESTRICT, required)
- Indices: MonedaId, EsDefault, Activo
- Default: Activo = 1, EsDefault = 0
```

#### DTOs
- `CrearListaPrecioDto`: Nombre, MonedaId, Descripcion?, EsDefault?
- `ActualizarListaPrecioDto`: Nombre, MonedaId, Descripcion?, EsDefault?
- `ListaPrecioDto`: Full response con PublicId, MonedaNombre

#### Validaciones
- **Nombre:** required, max 150 chars
- **MonedaId:** required, debe existir
- **Descripcion:** optional, max 500 chars
- **EsDefault:** Solo 1 por sistema puede ser true (validación en Handler)

#### ValidatorService
```csharp
public class ListaPrecioValidatorService
{
    public async Task<bool> NombreUnicoAsync(string nombre, int? excludeId = null)
    {
        return !await _context.ListasPrecios
            .Where(l => l.Nombre == nombre && (excludeId == null || l.Id != excludeId))
            .AnyAsync();
    }

    public async Task<bool> ExisteDefaultAsync(int? excludeId = null)
    {
        return await _context.ListasPrecios
            .Where(l => l.EsDefault && (excludeId == null || l.Id != excludeId))
            .AnyAsync();
    }

    public async Task<bool> MonedaExisteAsync(int monedaId)
    {
        return await _context.Monedas
            .AnyAsync(m => m.Id == monedaId && m.Activo);
    }
}
```

#### Seed Data
```sql
INSERT INTO catalogo.ListasPrecios (Nombre, MonedaId, Descripcion, EsDefault, Activo)
VALUES
('Lista Precios Base', 1, 'Lista de precios base en moneda funcional', 1, 1),
('Lista Precios USD', 2, 'Lista de precios en dólares americanos', 0, 1);
-- MonedaId: 1=PEN, 2=USD (verificar seed Sprint 1)
```

#### Endpoints (7 estándar)
```
GET    /api/v1/listas-precios              → Listar (con paginación)
GET    /api/v1/listas-precios/{id}          → Obtener por ID
POST   /api/v1/listas-precios               → Crear (valida EsDefault único)
PUT    /api/v1/listas-precios/{id}          → Actualizar (valida EsDefault)
PATCH  /api/v1/listas-precios/{id}/activar  → Activar
PATCH  /api/v1/listas-precios/{id}/inactivar → Inactivar
DELETE /api/v1/listas-precios/{id}          → Eliminar (soft)
```

#### Nota Crítica: ListaPrecioDetalle Diferido
- **Esta entidad NO incluye detalles de precios por producto**
- **PD-02 decidido:** `ListaPrecioDetalle(ListaPrecioId, ProductoId, Precio)` → Ventas (Sprint 6+)
- **Razón:** YAGNI (No Anticipated Presentations), velocidad, escalabilidad sin breaking changes
- **Documentado:** ADR-011 en IA_Docs/ARCHITECTURE_DECISIONS.md
- **Impacto:** Sprint 5 completa con catálogos simples; Sprint 6 agrega precios por producto en 2-3 horas

---

### 3. Proveedor → `comercial.Proveedores`

#### Domain Entity
```csharp
namespace Domain.Comercial
{
    public class Proveedor : AuditableEntity
    {
        public int TipoDocumentoId { get; set; }               // FK TipoDocumentos (requerido)
        public string NumeroDocumento { get; set; }            // 20, obligatorio (RUC, DNI, etc.)
        public string RazonSocial { get; set; }                // 200, obligatorio
        public string? NombreComercial { get; set; }           // 150, opcional
        public int PaisId { get; set; }                        // FK Paises (requerido)
        public string? Correo { get; set; }                    // 150, optional, filtered unique
        public string? Telefono { get; set; }                  // 20, opcional
        public string? Direccion { get; set; }                 // 300, opcional
        
        // Navigation properties (idénticas a Cliente)
        public virtual TipoDocumento TipoDocumento { get; set; }
        public virtual Pais Pais { get; set; }
    }
}
```

#### Configuration
```
Schema: comercial
Table: Proveedores
Constraints:
- PK: Id (INT)
- FK: TipoDocumentoId → catalogo.TipoDocumentos (RESTRICT, required)
- FK: PaisId → catalogo.Paises (RESTRICT, required)
- UNIQUE (TipoDocumentoId, NumeroDocumento)
- Filtered Unique Index: Correo (permite múltiples NULL)
- Indices: PaisId, TipoDocumentoId, Activo
- Default: Activo = 1
```

#### DTOs
- `CrearProveedorDto`: TipoDocumentoId, NumeroDocumento, RazonSocial, NombreComercial?, PaisId, Correo?, Telefono?, Direccion?
- `ActualizarProveedorDto`: TipoDocumentoId, NumeroDocumento, RazonSocial, NombreComercial?, PaisId, Correo?, Telefono?, Direccion?
- `ProveedorDto`: Full response con PublicId, TipoDocumentoNombre, PaisNombre

#### Validaciones
- **TipoDocumentoId:** required, debe existir
- **NumeroDocumento:** required, max 20 chars, único por tipo documento
- **RazonSocial:** required, max 200 chars
- **NombreComercial:** optional, max 150 chars
- **PaisId:** required, debe existir
- **Correo:** optional, max 150 chars, unique (nullable)
- **Telefono:** optional, max 20 chars
- **Direccion:** optional, max 300 chars

#### ValidatorService
```csharp
public class ProveedorValidatorService
{
    public async Task<bool> ProveedorUnicoAsync(
        int tipoDocumentoId, 
        string numeroDocumento,
        int? excludeId = null)
    {
        var existe = await _context.Proveedores
            .Where(p => p.TipoDocumentoId == tipoDocumentoId 
                && p.NumeroDocumento == numeroDocumento
                && (excludeId == null || p.Id != excludeId))
            .AnyAsync();
        
        return !existe;
    }
    
    public async Task<bool> CorreoUnicoAsync(string correo, int? excludeId = null)
    {
        if (string.IsNullOrEmpty(correo)) return true; // NULL es permitido
        
        var existe = await _context.Proveedores
            .Where(p => p.Correo == correo 
                && (excludeId == null || p.Id != excludeId))
            .AnyAsync();
        
        return !existe;
    }

    public async Task<bool> TipoDocumentoExisteAsync(int tipoDocumentoId)
    {
        return await _context.TipoDocumentos
            .AnyAsync(td => td.Id == tipoDocumentoId && td.Activo);
    }

    public async Task<bool> PaisExisteAsync(int paisId)
    {
        return await _context.Paises
            .AnyAsync(p => p.Id == paisId && p.Activo);
    }
}
```

#### Seed Data
```sql
INSERT INTO comercial.Proveedores (TipoDocumentoId, NumeroDocumento, RazonSocial, NombreComercial, PaisId, Correo, Telefono, Direccion, Activo)
VALUES
(5, '20123456789', 'Distribuidora de Componentes XYZ SAC', 'Dist. XYZ', 1, 'compras@distxyz.com', '+51987654321', 'Av. Principal 123, Lima', 1),
(5, '20987654321', 'Importadora de Electrónica ACME EIRL', 'ACME', 1, 'contacto@acmeimports.com', '+51945678901', 'Jr. Comercio 456, Surco', 1);
-- TipoDocumentoId=5 (RUC), PaisId=1 (Perú)
```

#### Endpoints (7 estándar)
```
GET    /api/v1/proveedores              → Listar (con paginación)
GET    /api/v1/proveedores/{id}          → Obtener por ID
POST   /api/v1/proveedores               → Crear (valida documento único, correo único)
PUT    /api/v1/proveedores/{id}          → Actualizar (valida duplicados)
PATCH  /api/v1/proveedores/{id}/activar  → Activar
PATCH  /api/v1/proveedores/{id}/inactivar → Inactivar
DELETE /api/v1/proveedores/{id}          → Eliminar (soft)
```

---

## 📂 ESTRUCTURA DE ARCHIVOS A CREAR

```
Domain/Catalogo/
├── CondicionPago.cs (NEW)
└── ListaPrecio.cs (NEW)

Domain/Comercial/
└── Proveedor.cs (NEW)

Application/Features/Catalogo/
├── CondicionPago/
│   ├── Crear/
│   │   ├── CrearCondicionPagoCommand.cs
│   │   ├── CrearCondicionPagoHandler.cs
│   │   └── CrearCondicionPagoValidator.cs
│   ├── Actualizar/
│   │   ├── ActualizarCondicionPagoCommand.cs
│   │   ├── ActualizarCondicionPagoHandler.cs
│   │   └── ActualizarCondicionPagoValidator.cs
│   ├── ActualizarEstado/
│   │   ├── ActualizarEstadoCondicionPagoCommand.cs
│   │   └── ActualizarEstadoCondicionPagoHandler.cs
│   └── Eliminar/
│       ├── EliminarCondicionPagoCommand.cs
│       └── EliminarCondicionPagoHandler.cs
└── ListaPrecio/ (estructura idéntica × 4 handlers)

Application/Features/Comercial/
└── Proveedor/ (estructura idéntica a Cliente — 4 handlers + validators)

Application/Dtos/Catalogo/
├── CrearCondicionPagoDto.cs
├── ActualizarCondicionPagoDto.cs
├── CondicionPagoDto.cs
├── CrearListaPrecioDto.cs
├── ActualizarListaPrecioDto.cs
└── ListaPrecioDto.cs

Application/Dtos/Comercial/
├── CrearProveedorDto.cs
├── ActualizarProveedorDto.cs
└── ProveedorDto.cs

Application/Interfaces/
├── ICondicionPagoService.cs
├── ICondicionPagoValidatorService.cs
├── IListaPrecioService.cs
├── IListaPrecioValidatorService.cs
├── IProveedorService.cs
└── IProveedorValidatorService.cs

Application/Mappings/Catalogo/
├── CondicionPagoProfile.cs
└── ListaPrecioProfile.cs

Application/Mappings/Comercial/
└── ProveedorProfile.cs

Infrastructure/Persistence/Configurations/
├── CondicionPagoConfiguration.cs
├── ListaPrecioConfiguration.cs
└── ProveedorConfiguration.cs

Infrastructure/Repository/
├── CondicionPagoService.cs
├── CondicionPagoValidatorService.cs
├── ListaPrecioService.cs
├── ListaPrecioValidatorService.cs
├── ProveedorService.cs
└── ProveedorValidatorService.cs

GestionComercial/Controllers/
├── CondicionesPagoController.cs
├── ListasPreciosController.cs
└── ProveedoresController.cs

Database/02_Tablas/
├── 16_CondicionesPago.sql (NEW)
├── 17_ListasPrecios.sql (NEW)
└── 18_Proveedores.sql (NEW)

Database/03_Seeds/
└── 10_InitCondicionPagoListaPrecioProveedor.sql (NEW)
```

---

## 🔄 PATRONES EXACTOS A SEGUIR

### 1. CondicionPago & ListaPrecio: Catálogos Simples

**Handler Crear:**
```csharp
public class CrearCondicionPagoHandler : IRequestHandler<CrearCondicionPagoCommand, int>
{
    private readonly ICondicionPagoService _service;
    private readonly ICondicionPagoValidatorService _validator;
    private readonly IMapper _mapper;

    public async Task<int> Handle(CrearCondicionPagoCommand cmd, CancellationToken ct)
    {
        // Validar unicidad
        var esUnico = await _validator.NombreUnicoAsync(cmd.Nombre);
        if (!esUnico)
            throw new InvalidOperationException($"Condición de pago '{cmd.Nombre}' ya existe");

        var condicion = _mapper.Map<CondicionPago>(cmd);
        return await _service.Crear(condicion);
    }
}
```

**Handler Crear ListaPrecio (con validación EsDefault):**
```csharp
public class CrearListaPrecioHandler : IRequestHandler<CrearListaPrecioCommand, int>
{
    private readonly IListaPrecioService _service;
    private readonly IListaPrecioValidatorService _validator;
    private readonly IMapper _mapper;

    public async Task<int> Handle(CrearListaPrecioCommand cmd, CancellationToken ct)
    {
        // Si EsDefault = true, desactivar otras
        if (cmd.EsDefault)
        {
            var listaDefaultActual = await _service.ObtenerDefaultAsync();
            if (listaDefaultActual != null)
            {
                listaDefaultActual.EsDefault = false;
                await _service.Actualizar(listaDefaultActual);
            }
        }

        var lista = _mapper.Map<ListaPrecio>(cmd);
        return await _service.Crear(lista);
    }
}
```

### 2. Proveedor: Clone de Cliente

**Patrón idéntico a Cliente.cs**
- Mismas FKs: TipoDocumentoId, PaisId
- Mismo ValidatorService con métodos: ProveedorUnicoAsync, CorreoUnicoAsync
- Mismo patrón de Commands (record)
- Mismo patrón de Handlers (Task<int>)
- Mismo patrón de DTOs
- Mismo patrón de Controllers (7 endpoints)

**Diferencia única:** Tabla `comercial.Proveedores` en lugar de `comercial.Clientes`

---

## 📊 RESUMEN DE ARTEFACTOS

| Item | Cantidad | Descripción |
|------|----------|-------------|
| Entidades nuevas | 3 | CondicionPago, ListaPrecio, Proveedor |
| Commands | 12 | 4 × CondicionPago, 4 × ListaPrecio, 4 × Proveedor |
| Handlers | 12 | Crear, Actualizar, ActualizarEstado, Eliminar × 3 |
| Validators | 6 | Crear + Actualizar × 3 |
| ValidatorServices | 3 | Con métodos específicos por entidad |
| Services | 3 | Interfaz + Implementación × 3 |
| DTOs | 9 | 3 × 3 entidades |
| Mappings | 3 | AutoMapper profiles × 3 |
| Configurations | 3 | Entity Framework configurations × 3 |
| Controllers | 3 | CondicionesPagoController, ListasPreciosController, ProveedoresController |
| Endpoints | 21 | 7 × 3 entidades (estándar CRUD) |
| SQL Scripts | 4 | 3 tablas nuevas + 1 seed |
| Archivos nuevos | ~28 | Total (incluyendo DTOs, handlers, validators) |

---

## ⚠️ RIESGOS TÉCNICOS & MITIGACIÓN

### R-01: Validación EsDefault en ListaPrecio
**Probabilidad:** Baja | **Impacto:** Medio | **Estado:** MITIGADO
- **Solución:** En Handler, desactivar EsDefault en listas anteriores si nueva es default
- **Validación:** Test de crear lista default mientras existe otra default

### R-02: Correo duplicado en Proveedor
**Probabilidad:** Baja | **Impacto:** Bajo | **Estado:** MITIGADO
- **Solución:** Filtered unique index en SQL + validación en ValidatorService
- **Validación:** Test de crear proveedor con correo existente debe fallar

### R-03: Documento duplicado (TipoDocumento + NumeroDocumento)
**Probabilidad:** Baja | **Impacto:** Bajo | **Estado:** MITIGADO
- **Solución:** UNIQUE constraint (TipoDocumentoId, NumeroDocumento) + validación
- **Validación:** Test de crear proveedor con documento existente debe fallar

### R-04: FK a Moneda nula en ListaPrecio
**Probabilidad:** Baja | **Impacto:** Alto | **Estado:** MITIGADO
- **Solución:** MonedaId requerido en DTO, validación en Handler, FK NOT NULL
- **Validación:** Test POST sin MonedaId debe fallar

---

## 🚨 CRITICAL RULES

1. **Proveedor = Cliente:** Patrón idéntico — usar Cliente como referencia exacta
2. **ListaPrecioDetalle:** NO incluir en Sprint 5 — diferido a Ventas per ADR-011
3. **EsDefault único:** Solo 1 ListaPrecio puede tener EsDefault = true
4. **Documento único:** Combinación (TipoDocumentoId, NumeroDocumento) única en Proveedor
5. **Correo nullable:** Permite múltiples registros con Correo = NULL
6. **Soft Delete:** Patrón Activo = false en todas las entidades
7. **No Cascada:** DeleteBehavior.Restrict en todos los FK
8. **Índices:** Crear índices en FK y columnas de búsqueda común

---

## ✅ CHECKLIST PRE-IMPLEMENTACIÓN

- [ ] Leer plan activo: `plans/active/2026-05-16_catalogo-sprint5-comercial.md`
- [ ] Revisar ADR-011 en IA_Docs/ARCHITECTURE_DECISIONS.md (ListaPrecioDetalle decision)
- [ ] Revisar SPRINT_4_READY.md para patrones más recientes
- [ ] Compilar proyecto baseline (verify 0 errores)
- [ ] Verificar Domain/Catalogo existe
- [ ] Verificar Domain/Comercial existe
- [ ] Verificar Application/Features/Catalogo existe
- [ ] Verificar Application/Features/Comercial existe
- [ ] Verificar Infrastructure/Repository existe
- [ ] Verificar Database/02_Tablas, 03_Seeds existen
- [ ] Revisar Cliente.cs como patrón para Proveedor

---

## 📋 CHECKLIST DE DESARROLLO

**Fase 1: Entidades de Dominio (3)**
- [ ] Crear CondicionPago.cs
- [ ] Crear ListaPrecio.cs
- [ ] Crear Proveedor.cs

**Fase 2: CQRS Commands (12)**
- [ ] 4 × CondicionPago (Crear, Actualizar, ActualizarEstado, Eliminar)
- [ ] 4 × ListaPrecio (Crear, Actualizar, ActualizarEstado, Eliminar)
- [ ] 4 × Proveedor (Crear, Actualizar, ActualizarEstado, Eliminar)

**Fase 3: CQRS Handlers (12)**
- [ ] 4 × CondicionPago (con validaciones)
- [ ] 4 × ListaPrecio (con validación EsDefault)
- [ ] 4 × Proveedor (con validaciones de documento y correo)

**Fase 4: CQRS Validators (6)**
- [ ] CrearCondicionPagoValidator + ActualizarCondicionPagoValidator
- [ ] CrearListaPrecioValidator + ActualizarListaPrecioValidator
- [ ] CrearProveedorValidator + ActualizarProveedorValidator

**Fase 5: DTOs & Mappings (12)**
- [ ] CondicionPago DTOs (3) + CondicionPagoProfile
- [ ] ListaPrecio DTOs (3) + ListaPrecioProfile
- [ ] Proveedor DTOs (3) + ProveedorProfile

**Fase 6: Services & Validaciones (6)**
- [ ] CondicionPagoService + CondicionPagoValidatorService
- [ ] ListaPrecioService + ListaPrecioValidatorService
- [ ] ProveedorService + ProveedorValidatorService

**Fase 7: Entity Configurations (3)**
- [ ] CondicionPagoConfiguration
- [ ] ListaPrecioConfiguration
- [ ] ProveedorConfiguration (con filtered unique index)

**Fase 8: Database (4)**
- [ ] Database/02_Tablas/16_CondicionesPago.sql
- [ ] Database/02_Tablas/17_ListasPrecios.sql
- [ ] Database/02_Tablas/18_Proveedores.sql
- [ ] Database/03_Seeds/10_InitCondicionPagoListaPrecioProveedor.sql

**Fase 9: API & Controllers (3)**
- [ ] CondicionesPagoController (7 endpoints)
- [ ] ListasPreciosController (7 endpoints)
- [ ] ProveedoresController (7 endpoints)
- [ ] Registrar rutas en Program.cs

**Fase 10: Integración (5)**
- [ ] Actualizar AppDbContext.cs (+3 DbSets)
- [ ] Actualizar Program.cs (+6 DI registrations)
- [ ] Compilar proyecto (0 errores, 0 warnings)
- [ ] Ejecutar scripts SQL en orden
- [ ] Smoke testing: 21 endpoints + validaciones

---

## 📊 SUCCESS CRITERIA

- [ ] Compilación: 0 errores, 0 advertencias
- [ ] Endpoints: 21 totales (7 × 3 entidades)
- [ ] Commands: 12 nuevos funcionando
- [ ] Handlers: 12 nuevos con lógica completa
- [ ] Validators: 6 nuevos con validaciones
- [ ] Services: 3 nuevos + 3 ValidatorServices
- [ ] DTOs: 9 nuevos
- [ ] Controllers: 3 nuevos
- [ ] Configurations: 3 nuevas
- [ ] SQL: 3 tablas nuevas + 1 seed
- [ ] Program.cs: +6 DI registrations
- [ ] AppDbContext.cs: +3 DbSets
- [ ] Seed ejecutado: catálogos populados
- [ ] GET /api/v1/condiciones-pago: lista con seed ✅
- [ ] GET /api/v1/listas-precios: lista con seed ✅
- [ ] GET /api/v1/proveedores: lista con seed ✅
- [ ] POST crear duplicado: rechazado con error ✅
- [ ] Soft delete: Activo = false funciona ✅
- [ ] Total entidades catálogos: 18 ✅

---

## 📈 PROGRESO ESPERADO

```
Sprint 1 (Catálogos Base)    ✅ 100% COMPLETADO — 5 entidades
Sprint 2 (Organización)       ✅ 100% COMPLETADO — 3 entidades
Sprint 3 (Fiscal)             ✅ 100% COMPLETADO — 3 entidades
Sprint 4 (Producto)           ✅ 100% COMPLETADO — 2 entidades + ALTER
Sprint 5 (Comercial)          ⏳ READY FOR IMPLEMENTATION — 3 entidades
═══════════════════════════════════════════════════════════════════
CATÁLOGOS TOTAL              18 ENTIDADES — MÓDULO VENTAS DESBLOQUEADO
```

---

## 🔗 REFERENCIAS CRÍTICAS

```
plans/active/2026-05-16_catalogo-sprint5-comercial.md
  └─ Plan detallado con scope, riesgos y decisiones

SPRINT_4_READY.md
  └─ Patrones más recientes (handlers, validators, controllers)

IA_Docs/ARCHITECTURE_DECISIONS.md (ADR-011)
  └─ Decisión ListaPrecioDetalle diferido + YAGNI justification

IA_Docs/IMPLEMENTATION_PATTERNS.md
  └─ Patrones CQRS, ValidatorService, DTOs

execution-status/catalogo-base-status.md
  └─ Actualizar progreso diariamente

pending/2026-05-15_technical-backlog.md
  └─ PD-02 decidido (ListaPrecioDetalle deferred)

Domain/Comercial/Cliente.cs
  └─ Referencia exacta para patrón Proveedor
```

---

## 📝 POST-BUILD ACTIONS

1. [ ] Ejecutar SQL scripts en orden:
   - `Database/02_Tablas/16_CondicionesPago.sql`
   - `Database/02_Tablas/17_ListasPrecios.sql`
   - `Database/02_Tablas/18_Proveedores.sql`
   - `Database/03_Seeds/10_InitCondicionPagoListaPrecioProveedor.sql`

2. [ ] Update `execution-status/catalogo-base-status.md`
   - Sprint 5: 0% → 100%
   - Modules: 3 completed
   - Total catálogos: 18 ✅

3. [ ] Create History Changed entry
   - `20260518_THHMM_feat_Sprint5ComercialCatalogoCompleto`
   - SUMMARY.md con:
     * 3 entidades creadas
     * 21 endpoints
     * Patrón Proveedor = Cliente
     * Validaciones especiales
     * ADR-011 decision impact
     * Catálogos completados (18 total)

4. [ ] Commit
   - Message: `feat(catalogo): Sprint 5 — Comercial (CondicionPago, ListaPrecio, Proveedor) — CATÁLOGOS COMPLETADOS`

5. [ ] Merge a rama develop
   - PR: `catalogo-base/sprint_5` → `develop`
   - Revisor: Arquitecto Backend

6. [ ] Move plans
   - `plans/active/2026-05-16_catalogo-sprint5-comercial.md` → `plans/completed/`
   - Crear SUMMARY.md en History Changed

---

## 🎯 PRÓXIMO PASO

**Sprint 6 (Ventas v3.1):**
- Módulo Ventas totalmente desbloqueado
- Integración de todos los catálogos
- Venta(Cliente, Sucursal, SerieDocumento, CondicionPago, ListaPrecio, Moneda)
- VentaDetalle(Producto, UnidadMedida, TipoImpuesto)
- **Nota:** Sprint 6 incluirá ListaPrecioDetalle(ListaPrecioId, ProductoId, Precio) per ADR-011

**Bloqueador actual:** Ninguno  
**Dependencias completadas:** Sprint 1-4 ✅  
**Arquitectura validada:** PD-02 decidido, ADR-011 documentado

---

**Status:** ✅ **SPRINT 5 LISTA PARA EJECUTAR — Especificación ejecutable completa**  
**Documento:** SPRINT_5_READY.md (Especificación Ejecutable)  
**Patrón:** Catálogos simples + Clone de Cliente + ListaPrecioDetalle deferred  
**Fecha Especificación:** 2026-05-18  
**Fecha Creación Rama:** 2026-05-18  
**Siguiente:** Iniciar implementación fase por fase según checklist

