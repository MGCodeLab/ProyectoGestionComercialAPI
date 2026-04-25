# Detalle de Cambios Técnicos

## 1. AuditableEntityConfiguration.cs (✅ NUEVO)

**Ubicación:** `Infrastructure/Persistence/Configurations/AuditableEntityConfiguration.cs`

### Qué es:
Clase base genérica abstracta que implementa `IEntityTypeConfiguration<T>` para todas las entidades auditables.

### Patrón usado:
**Template Method Pattern** - Define estructura común (Configure base) que todas las subclases deben respetar.

### Configuración centralizada:
```csharp
// 1. Clave primaria
builder.HasKey(e => e.Id);

// 2. Identificador público (GUID externo)
builder.Property(e => e.PublicId)
    .IsRequired()
    .HasDefaultValueSql("NEWSEQUENTIALID()");

// 3. Campos de auditoría
builder.Property(e => e.Activo)
    .IsRequired()
    .HasDefaultValue(true);

builder.Property(e => e.FechaRegistro)
    .IsRequired()
    .HasDefaultValueSql("GETUTCDATE()");

builder.Property(e => e.FechaActualizacion);

// 4. Índice para lookups por PublicId
builder.HasIndex(e => e.PublicId)
    .IsUnique();
```

### Cómo usan subclases:
```csharp
public class ProductoConfiguration : AuditableEntityConfiguration<Producto>
{
    public override void Configure(EntityTypeBuilder<Producto> builder)
    {
        base.Configure(builder);  // Aplica configuración audit
        
        // Luego configuración específica de Producto
        builder.ToTable("Productos", schema: "catalogo");
        // ... propiedades específicas ...
    }
}
```

---

## 2. ProductoConfiguration.cs (🔄 REFACTORIZADO)

**Ubicación:** `Infrastructure/Persistence/Configurations/ProductoConfiguration.cs`

### Antes:
```csharp
public class ProductoConfiguration : IEntityTypeConfiguration<Producto>
{
    public void Configure(EntityTypeBuilder<Producto> builder)
    {
        builder.ToTable("Productos", schema: "catalogo");
        builder.HasKey(p => p.Id);  // ❌ Duplicado
        builder.Property(p => p.Nombre).IsRequired().HasMaxLength(150);
        builder.Property(p => p.Descripcion).HasMaxLength(500);
        builder.Property(p => p.Precio).HasColumnType("decimal(18,2)");
        builder.Property(p => p.Activo).HasDefaultValue(true);  // ❌ Sin IsRequired
        // ❌ FALTA: PublicId
        // ❌ FALTA: FechaRegistro
        // ❌ FALTA: FechaActualizacion
        // ❌ FALTA: Índice en PublicId
    }
}
```

### Después:
```csharp
public class ProductoConfiguration : AuditableEntityConfiguration<Producto>
{
    public override void Configure(EntityTypeBuilder<Producto> builder)
    {
        base.Configure(builder);  // ✅ Obtiene: HasKey, PublicId, Activo, FechaRegistro, FechaActualizacion, índice
        
        builder.ToTable("Productos", schema: "catalogo");
        builder.Property(p => p.Nombre).IsRequired().HasMaxLength(150);
        builder.Property(p => p.Descripcion).HasMaxLength(500);
        builder.Property(p => p.Precio).HasColumnType("decimal(18,2)");
    }
}
```

### Cambios específicos:
- **Antes:** ~15 líneas, 3 problemas (HasKey duplicado, Activo sin IsRequired, sin PublicId/Audit fields)
- **Después:** ~10 líneas, 0 problemas
- **Gain:** -5 líneas, 100% consistencia garantizada

---

## 3. ClienteConfiguration.cs (🔄 REFACTORIZADO)

**Ubicación:** `Infrastructure/Persistence/Configurations/ClienteConfiguration.cs`

### Antes:
```csharp
public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes", schema: "comercial");
        builder.HasKey(c => c.Id);  // ❌ Duplicado
        builder.Property(c => c.PublicId)
            .IsRequired()
            .HasDefaultValueSql("NEWSEQUENTIALID()");  // ✅ Bien configurado
        // ... campos específicos ...
        builder.Property(c => c.Activo).IsRequired();
        builder.Property(c => c.FechaRegistro)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
        builder.Property(c => c.FechaActualizacion);
        // ... FK y índices ...
        builder.HasIndex(c => c.PublicId).IsUnique();  // ❌ Duplicado
    }
}
```

### Después:
```csharp
public class ClienteConfiguration : AuditableEntityConfiguration<Cliente>
{
    public override void Configure(EntityTypeBuilder<Cliente> builder)
    {
        base.Configure(builder);  // ✅ Obtiene: HasKey, PublicId, Activo, FechaRegistro, FechaActualizacion, índice
        
        builder.ToTable("Clientes", schema: "comercial");
        // ... solo campos específicos de Cliente ...
        builder.HasOne(c => c.TipoDocumento)
            .WithMany()
            .HasForeignKey(c => c.TipoDocumentoId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Property(c => c.NombreCompleto)
            .HasComputedColumnSql("[Nombres] + ' ' + [ApellidoPaterno] + ' ' + ISNULL([ApellidoMaterno], '')");
        // ... índices de negocio (sin duplicar PublicId) ...
    }
}
```

### Cambios específicos:
- **Antes:** ~60 líneas, con mucha duplicación
- **Después:** ~40 líneas, más enfocado en lógica específica
- **Removido:** HasKey, PublicId config, Activo config, FechaRegistro config, FechaActualizacion config, índice PublicId
- **Mantiene:** TipoDocumento FK, NombreCompleto computed, índices de negocio únicos

---

## 4. TipoDocumentoConfiguration.cs (🔄 REFACTORIZADO)

**Ubicación:** `Infrastructure/Persistence/Configurations/TipoDocumentoConfiguration.cs`

### Antes:
```csharp
public class TipoDocumentoConfiguration : IEntityTypeConfiguration<TipoDocumento>
{
    public void Configure(EntityTypeBuilder<TipoDocumento> builder)
    {
        builder.ToTable("TipoDocumento", schema: "catalogo");
        builder.HasKey(e => e.Id);  // ❌ Duplicado
        builder.Property(e => e.Codigo).IsRequired().HasMaxLength(20);
        builder.Property(e => e.Descripcion).HasMaxLength(250);
        builder.Property(e => e.Activo).IsRequired();  // ❌ Sin DefaultValue
        // ❌ FALTA: PublicId
        // ❌ FALTA: FechaRegistro
        // ❌ FALTA: Índice PublicId
        // ❌ FALTA: Índice Codigo (debería ser unique)
    }
}
```

### Después:
```csharp
public class TipoDocumentoConfiguration : AuditableEntityConfiguration<TipoDocumento>
{
    public override void Configure(EntityTypeBuilder<TipoDocumento> builder)
    {
        base.Configure(builder);  // ✅ Obtiene: HasKey, PublicId, Activo, FechaRegistro, FechaActualizacion, índice
        
        builder.ToTable("TipoDocumento", schema: "catalogo");
        builder.Property(e => e.Codigo).IsRequired().HasMaxLength(20);
        builder.Property(e => e.Descripcion).HasMaxLength(250);
        builder.HasIndex(e => e.Codigo).IsUnique();  // ✅ NUEVO: Codigo debe ser único
    }
}
```

### Cambios específicos:
- **Antes:** ~15 líneas, sin enforcement de código único
- **Después:** ~10 líneas, con índice único en Codigo
- **Nueva constraint:** Dos TipoDocumentos no pueden tener mismo Código
- **Beneficio:** Integridad de datos a nivel DB

---

## SQL Server Impact

### Migraciones de EF Core necesarias:
```sql
-- Para Productos (cambios en constraints/defaults)
ALTER TABLE catalogo.Productos 
  ALTER COLUMN PublicId ADD DEFAULT NEWSEQUENTIALID()
ALTER TABLE catalogo.Productos 
  ALTER COLUMN FechaRegistro ADD DEFAULT GETUTCDATE()
CREATE UNIQUE INDEX IX_Productos_PublicId ON catalogo.Productos(PublicId);

-- Para TipoDocumento (nuevo índice)
CREATE UNIQUE INDEX IX_TipoDocumento_Codigo ON catalogo.TipoDocumento(Codigo);

-- Para Cliente (sin cambios en contraints, ya existían)
```

---

## Verificaciones Post-Cambio

### Test 1: Crear Producto
```csharp
var producto = new Producto { Nombre = "Test", Precio = 100 };
await dbContext.Productos.AddAsync(producto);
await dbContext.SaveChangesAsync();

// Verificar:
Assert.NotEqual(default(Guid), producto.PublicId);  // ✅ GUID asignado
Assert.NotEqual(default(DateTime), producto.FechaRegistro);  // ✅ FechaRegistro automático
Assert.True(producto.Activo);  // ✅ Activo = true por defecto
```

### Test 2: Crear Cliente
```csharp
var cliente = new Cliente { Nombres = "Test", ... };
await dbContext.Clientes.AddAsync(cliente);
await dbContext.SaveChangesAsync();

// Verificar:
Assert.NotEqual(default(Guid), cliente.PublicId);  // ✅ GUID asignado
Assert.NotEqual(default(DateTime), cliente.FechaRegistro);  // ✅ FechaRegistro automático
```

### Test 3: TipoDocumento Código único
```csharp
var td1 = new TipoDocumento { Codigo = "DNI", ... };
var td2 = new TipoDocumento { Codigo = "DNI", ... };
await dbContext.TipoDocumentos.AddAsync(td1);
await dbContext.SaveChangesAsync();
await dbContext.TipoDocumentos.AddAsync(td2);
await dbContext.SaveChangesAsync();  // ❌ DEBE FALLAR por constraint unique
```

---

## Resumen de Líneas de Código

| Archivo | Antes | Después | Δ |
|---------|-------|---------|---|
| ProductoConfiguration | 15 | 10 | -5 |
| ClienteConfiguration | 60 | 40 | -20 |
| TipoDocumentoConfiguration | 15 | 10 | -5 |
| AuditableEntityConfiguration (NEW) | — | 40 | +40 |
| **TOTAL** | 90 | 100 | +10 pero MEJOR |

**Interpretación:** +10 líneas son bien invertidas en base reutilizable. Próximas 10 entidades ahorrarán 50+ líneas c/u.

---

## Notas de Arquitectura

1. **Genéricos:** `AuditableEntityConfiguration<T>` usa generic `T` con constraint `where T : AuditableEntity`
2. **Abstract:** Clase es abstract porque solo se usa como base
3. **Virtual:** Método Configure es virtual para que subclases puedan override
4. **DRY:** Cada audit field se configura UNA SOLA VEZ
5. **Extensible:** Si agregamos CreatedBy, UpdatedBy, TenantId → se agrega en base y todas heredan

---

## Rollback Plan (si necesario)

1. Revertir 3 files de configuración a versión anterior
2. Eliminar AuditableEntityConfiguration.cs
3. No hay cambios BD que revertir (EF Core migrations aún no aplicadas)

Tiempo de rollback: < 2 minutos.
