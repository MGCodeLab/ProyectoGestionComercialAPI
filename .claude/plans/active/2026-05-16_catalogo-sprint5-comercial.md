# Sprint 5: Comercial (CondicionPago, ListaPrecio, Proveedor)

**Estado:** ⏳ **PENDIENTE**  
**Fecha Estimada Inicio:** 2026-06-01  
**Duración Estimada:** 6-7 horas  
**Rama:** `catalogo-base/sprint_5`  
**Complejidad:** 🟢 **BAJA** (Patrones conocidos, clone de Cliente)

---

## 📋 Objetivo

Completar catálogos comerciales que sustentan Ventas y Compras:
- **CondicionPago**: Términos de crédito (Contado, 15 días, 30 días, etc.)
- **ListaPrecio**: Catálogo de listas de precios por moneda
- **Proveedor**: Maestro de proveedores (clon del patrón Cliente)

**Dependencias:** Sprint 1 (Moneda), Sprint 2 (Empresa)  
**Completa catálogos:** Desbloquea Módulo Ventas v3.1  

---

## 🎯 Entidades a Crear (3)

### 1. CondicionPago → `catalogo.CondicionesPago`

```
Nombre              NVARCHAR(100) NOT NULL
DiasCredito         INT NOT NULL DEFAULT 0             -- 0=Contado, 15=15 días, etc.
Descripcion         NVARCHAR(500) NULL
PublicId            GUID (via AuditableEntity)
Activo              BIT DEFAULT 1
```

**Características:**
- Catálogo simple de términos de pago
- `DiasCredito = 0` indica compra al contado
- Seed obligatorio: Contado(0), 15 días, 30 días, 60 días
- CRUD completo

**Seed Data:**
```sql
INSERT INTO catalogo.CondicionesPago (Nombre, DiasCredito, Descripcion)
VALUES
('Contado', 0, 'Pago inmediato'),
('15 Días', 15, 'Crédito a 15 días'),
('30 Días', 30, 'Crédito a 30 días'),
('60 Días', 60, 'Crédito a 60 días');
```

---

### 2. ListaPrecio → `catalogo.ListasPrecios`

```
Nombre              NVARCHAR(150) NOT NULL
MonedaId            INT NOT NULL → FK catalogo.Monedas (RESTRICT)
Descripcion         NVARCHAR(500) NULL
EsDefault           BIT NOT NULL DEFAULT 0             -- Una default por sistema
PublicId            GUID (via AuditableEntity)
Activo              BIT DEFAULT 1
```

**Características:**
- Una lista por moneda
- Una por defecto (negocio rule)
- Seed obligatorio: ListaPreciosBase (PEN, default=true)
- **Nota:** Precios de productos (`ListaPrecioDetalle`) → deferred a módulo Ventas

**Seed Data:**
```sql
INSERT INTO catalogo.ListasPrecios (Nombre, MonedaId, EsDefault)
VALUES
('Lista Precios Base', 1, 1);  -- MonedaId=1 (PEN)
```

---

### 3. Proveedor → `comercial.Proveedores`

```
TipoDocumentoId     INT NOT NULL → FK catalogo.TipoDocumentos (RESTRICT)
NumeroDocumento     NVARCHAR(20) NOT NULL
RazonSocial         NVARCHAR(200) NOT NULL
NombreComercial     NVARCHAR(150) NULL
PaisId              INT NOT NULL → FK catalogo.Paises (RESTRICT)
Correo              NVARCHAR(150) NULL                 -- Filtered unique index
Telefono            NVARCHAR(20) NULL
Direccion           NVARCHAR(300) NULL
PublicId            GUID (via AuditableEntity)
Activo              BIT DEFAULT 1

-- UNIQUE (TipoDocumentoId, NumeroDocumento)
```

**Características:**
- **Patrón idéntico a Cliente** (clonar implementación)
- Datos de compra: RUC (TipoDocumento=5), DNI (TipoDocumento=4), etc.
- Correo: Unique index nullable (permite múltiples NULL)
- Seed: 1 proveedor de ejemplo

---

## 📁 Archivos a Crear: ~18 nuevos

### Entidades Domain (3)
- `Domain/Catalogo/CondicionPago.cs`
- `Domain/Catalogo/ListaPrecio.cs`
- `Domain/Comercial/Proveedor.cs`

### Commands (12)
- CondicionPago: Crear, Actualizar, ActualizarEstado, Eliminar (4)
- ListaPrecio: Crear, Actualizar, ActualizarEstado, Eliminar (4)
- Proveedor: Crear, Actualizar, ActualizarEstado, Eliminar (4)

### Handlers (12)
- Patrón estándar CQRS (Task<int>)
- Validaciones específicas en cada entidad

### Validators (6)
- Crear/Actualizar para cada entidad

### DTOs (9)
- Crear, Actualizar, Response para cada entidad

### AutoMapper Profiles (3)
- CondicionPagoProfile, ListaPrecioProfile, ProveedorProfile

### Services (6)
- CondicionPagoService, ListaPrecioService, ProveedorService
- CondicionPagoValidatorService, ListaPrecioValidatorService, ProveedorValidatorService

### Entity Configurations (3)
- CondicionPagoConfiguration
- ListaPrecioConfiguration
- ProveedorConfiguration (incluye Filtered Unique Index en Correo)

### Controllers (3 = 21 endpoints)
- **CondicionesPagoController** (7 endpoints)
- **ListasPreciosController** (7 endpoints)
- **ProveedoresController** (7 endpoints)

### Database Scripts (4)
- `Database/02_Tablas/15_CondicionesPago.sql`
- `Database/02_Tablas/16_ListasPrecios.sql`
- `Database/02_Tablas/17_Proveedores.sql`
- `Database/03_Seeds/09_InitCondicionPagoListaPrecioProveedor.sql`

---

## 🔧 Decisiones de Implementación

### 1. Proveedor como Clone de Cliente

Implementar idéntico a Cliente:

```csharp
// PATRÓN: Mismo que Domain/Comercial/Cliente.cs
public class Proveedor : AuditableEntity
{
    public int TipoDocumentoId { get; set; }
    public string NumeroDocumento { get; set; }
    public string RazonSocial { get; set; }
    public string? NombreComercial { get; set; }
    public int PaisId { get; set; }
    public string? Correo { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    
    // Navegaciones (igual que Cliente)
    public TipoDocumento TipoDocumento { get; set; }
    public Pais Pais { get; set; }
}
```

**Beneficio:** Reutilizar patrones validados, reducir duplicidad conceptual

---

### 2. Filtered Unique Index (Correo)

En `ProveedorConfiguration`:

```csharp
entity.HasIndex(p => p.Correo, "IX_Proveedores_Correo")
    .IsUnique()
    .HasFilter($"[Correo] IS NOT NULL");
    // Permite múltiples registros con Correo = NULL
```

---

### 3. ValidatorService Pattern (Proveedor)

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
        
        return !existe; // True si es único
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
}
```

---

### 4. Validación en Handlers

```csharp
public class CrearProveedorHandler : IRequestHandler<CrearProveedorCommand, int>
{
    public async Task<int> Handle(CrearProveedorCommand cmd, CancellationToken ct)
    {
        // Validar unicidad compuesta
        var esUnico = await _validatorService.ProveedorUnicoAsync(
            cmd.TipoDocumentoId, 
            cmd.NumeroDocumento
        );
        
        if (!esUnico)
            throw new InvalidOperationException(
                "Ya existe proveedor con este documento");
        
        // Crear
        var proveedor = new Proveedor(cmd);
        await _service.Crear(proveedor);
        
        return proveedor.Id;
    }
}
```

---

## 📝 Integración con Ventas (Post Sprint 5)

Cuando se implementa módulo Ventas v3.1:

```csharp
public class Venta : AuditableEntity
{
    // Relaciones con catálogos completados
    public int EmpresaId { get; set; }              // Sprint 2
    public int SucursalId { get; set; }             // Sprint 2
    public int ClienteId { get; set; }              // v3.0
    public int CondicionPagoId { get; set; }        // Sprint 5
    public int MonedaId { get; set; }               // Sprint 1
    public int ListaPrecioId { get; set; }          // Sprint 5
    // ... más propiedades
    
    public Empresa Empresa { get; set; }
    public Sucursal Sucursal { get; set; }
    public Cliente Cliente { get; set; }
    public CondicionPago CondicionPago { get; set; }
    public Moneda Moneda { get; set; }
    public ListaPrecio ListaPrecio { get; set; }
}
```

---

## ✅ Checklist Pre-Implementación

- [ ] Revisar patrón Cliente (usar como referencia)
- [ ] Seed data de CondicionPago validado
- [ ] ListaPrecio default configurada
- [ ] Proveedor con UNIQUE compuesta implementada
- [ ] Correo con filtered unique index
- [ ] DTOs de Proveedor idénticos a Cliente
- [ ] Controllers con mismo patrón
- [ ] DI registrations completadas
- [ ] SQL scripts ejecutados sin errores

---

## 📊 Métricas Esperadas

| Item | Planeado |
|------|----------|
| Entidades | 3 |
| Commands | 12 |
| Handlers | 12 |
| Validators | 6 |
| DTOs | 9 |
| Endpoints | 21 |
| SQL Scripts | 4 |
| Compilación esperada | 0 errores |
| Tiempo estimado | 6-7 horas |

---

## 🎯 Resultado Final de Catálogos (Sprint 5 = Fin de Fase)

### Catálogos Completados (18 entidades)
```
✅ SPRINT 1 (5):  Pais, Moneda, UnidadMedida, ModuloSistema, ParametroSistema
✅ SPRINT 2 (3):  Empresa, Sucursal, Almacen
⏳ SPRINT 3 (3):  TipoImpuesto, TipoComprobante, SerieDocumento
⏳ SPRINT 4 (3):  CategoriaProducto, MarcaProducto, (ALTER Productos)
⏳ SPRINT 5 (4):  CondicionPago, ListaPrecio, Proveedor
════════════════════════════════════════════════════════
               TOTAL: 18 entidades ✅
```

### Módulo Ventas v3.1 (Desbloqueado)
- Venta(Empresa, Sucursal, Cliente, CondicionPago, SerieDocumento, Moneda)
- VentaDetalle(Producto, UnidadMedida, TipoImpuesto)
- Flujo completo: Cotización → Venta → Factura

---

## 🔗 Referencias

- **Dependencias:** Sprint 1, Sprint 2
- **Completa catálogos:** Desbloquea Ventas v3.1
- **Patrón referencia:** Cliente (comercial.Clientes)
- **Patterns aplicados:** ValidatorService, CQRS, Filtered Unique Index

---

**Siguiente paso:** Iniciar después Sprint 4 completado

**Post-Sprint 5:** Iniciar Módulo Ventas v3.1

*Documento creado:* 2026-05-16  
*Estado:* ⏳ Pendiente
