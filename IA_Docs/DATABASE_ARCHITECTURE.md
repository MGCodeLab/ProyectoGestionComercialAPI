# Database Architecture — SQL Server Schema & Decisions

**Propósito:** Documentar decisiones de base de datos, constraints, índices, y patrones aplicados.

---

## 🏗️ Schema Layout

```
SQL Server: NexusERP
├── catalogo/              -- Datos maestros y configuración
│   ├── TipoDocumento (Id, Nombre, Abreviatura, Activo)
│   └── Productos (Id, Nombre, Descripción, Precio, Activo)
│
├── comercial/            -- Operaciones comerciales
│   ├── Clientes (Id, TipoDocumentoId, NumeroDocumento, Nombres, Correo, Activo)
│   ├── Ventas (Id, ClienteId, Fecha, Total, Activo)
│   └── VentaDetalles (Id, VentaId, ProductoId, Cantidad, Precio)
│
└── seguridad/            -- Autenticación y autorización
    ├── Usuarios (Id, Email, PasswordHash, Activo)
    ├── Roles (Id, Nombre, Descripción, Activo)
    ├── Permisos (Id, Recurso, Accion, Activo)
    ├── UsuarioRoles (UsuarioId, RolId)
    └── RolPermisos (RolId, PermisoId)
```

---

## 🔑 Patrones Aplicados

### 1. AuditableEntity Pattern

**Definición:** Todas las tablas comparten 4 columnas de auditoría:

| Columna | Tipo | Propósito |
|---------|------|----------|
| `Id` | INT IDENTITY | Primary key |
| `PublicId` | UNIQUEIDENTIFIER | Exposición hacia APIs (no secuencial) |
| `Activo` | BIT (1/0) | Soft delete indicator |
| `FechaRegistro` | DATETIME2 UTC | Auditoría (creación) |
| `FechaActualizacion` | DATETIME2 UTC NULL | Auditoría (actualización) |

**SQL Ejemplo (Clientes):**
```sql
CREATE TABLE comercial.Clientes
(
    Id INT IDENTITY PRIMARY KEY,
    PublicId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    
    -- Datos específicos
    TipoDocumentoId INT NOT NULL,
    NumeroDocumento VARCHAR(20) NOT NULL,
    Nombres VARCHAR(100) NOT NULL,
    ApellidoPaterno VARCHAR(100) NOT NULL,
    ApellidoMaterno VARCHAR(100) NULL,
    Correo VARCHAR(150) NULL,
    Telefono VARCHAR(20) NULL,
    Direccion VARCHAR(250) NULL,
    
    -- Auditoría (patrón AuditableEntity)
    Activo BIT NOT NULL DEFAULT 1,
    FechaRegistro DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FechaActualizacion DATETIME2 NULL,
    
    CONSTRAINT FK_Clientes_TipoDocumento
        FOREIGN KEY (TipoDocumentoId) REFERENCES catalogo.TipoDocumento(Id)
);
```

**EF Core Configuration:**
```csharp
public abstract class AuditableEntityConfiguration<T> : IEntityTypeConfiguration<T>
    where T : class, IAuditableEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.PublicId).IsRequired().HasDefaultValueSql("NEWSEQUENTIALID()");
        
        builder.Property(x => x.Activo).IsRequired().HasDefaultValue(true);
        builder.Property(x => x.FechaRegistro).IsRequired().HasDefaultValueSql("GETUTCDATE()");
        builder.Property(x => x.FechaActualizacion);
    }
}
```

---

### 2. Soft Delete as Audit Trail (NOT Data Hiding)

**Definición:** `Activo = 0` marca como "eliminado", pero NO se filtra en queries.

**Justificación:**
- Auditoría: Historial completo de operaciones
- Recuperación: Reactivar si es necesario
- Reportes: Ver qué se eliminó y cuándo

**Regla de Oro:**
```csharp
// ❌ NUNCA HACER
.HasQueryFilter(x => x.Activo == true);

// ✅ CORRECTO - Sin filtro global
var clientes = await _dbContext.Clientes.ToListAsync();  // TODOS, incluyendo inactivos
```

**GET Endpoint:**
```
GET /api/v1/clientes → Retorna 100 activos + 5 inactivos = 105 registros
```

**Soft Delete vs Hard Delete:**
| Operación | Comando SQL | Reversible | Auditoría |
|-----------|------------|-----------|----------|
| Soft Delete | `UPDATE SET Activo = 0` | ✅ Sí | ✅ Historial |
| Hard Delete | `DELETE FROM` | ❌ No | ❌ Perdido |

**El proyecto usa ambos:**
- `PATCH /inactivar` → Soft delete (reversible)
- `DELETE /` → Hard delete (permanente)

---

### 3. Computed Columns (PERSISTED)

**Definición:** Columnas calculadas automáticamente por SQL Server.

**Caso: NombreCompleto (Clientes)**
```sql
NombreCompleto AS [Nombres] + ' ' + [ApellidoPaterno] + ' ' + ISNULL([ApellidoMaterno], '') PERSISTED
```

**Características:**
- `PERSISTED` → almacenada en disco (no recalculada cada vez)
- Resultado indexable para búsquedas rápidas
- NULL handling automático

**Entity Configuration:**
```csharp
modelBuilder.Entity<Cliente>()
    .Property(x => x.NombreCompleto)
    .HasComputedColumnSql(
        "[Nombres] + ' ' + [ApellidoPaterno] + ' ' + ISNULL([ApellidoMaterno], '')",
        stored: true);
```

**Ventajas:**
- Datos siempre sincronizados
- Búsqueda eficiente (indexable)
- No requiere lógica en aplicación

---

### 4. Unique Indexes with NULL Handling

**Problema:** SQL Server trata múltiples NULLs como violación de UNIQUE constraint.

**Solución:** Filtered index que solo aplica donde IS NOT NULL.

**Caso: Correo (Clientes)**
```sql
-- ❌ NO USAR (falla con múltiples NULLs)
CONSTRAINT UQ_Clientes_Correo UNIQUE (Correo)

-- ✅ USAR (permite múltiples NULLs, enforza unicidad si no NULL)
CREATE UNIQUE INDEX UQ_Clientes_Correo
    ON comercial.Clientes(Correo)
    WHERE Correo IS NOT NULL;
```

**Entity Configuration:**
```csharp
modelBuilder.Entity<Cliente>()
    .HasIndex(x => x.Correo)
    .IsUnique(name: "UQ_Clientes_Correo");
    // Nota: EF no soporta directly la cláusula WHERE, 
    // debe crearse en SQL DDL
```

**SQL DDL:**
```sql
-- En Database/02_Tablas/*.sql
-- NO incluir constraint UNIQUE en CREATE TABLE
-- Crear index filtered DESPUÉS

CREATE UNIQUE INDEX UQ_Clientes_Correo
    ON comercial.Clientes(Correo)
    WHERE Correo IS NOT NULL;
```

**Cuándo usar:**
- Campo nullable que debe ser único si no NULL
- Ejemplo: Email (opcional para clientes, único si existe)

---

### 5. Foreign Keys with Cascade

**Patrón:**
```sql
CONSTRAINT FK_Clientes_TipoDocumento
    FOREIGN KEY (TipoDocumentoId)
    REFERENCES catalogo.TipoDocumento(Id)
    -- ON DELETE SET NULL | ON DELETE CASCADE | ON DELETE RESTRICT
```

**Decisiones del proyecto:**
- `TipoDocumentoId` → RESTRICT (no permitir eliminar tipos en uso)
- `ClienteId` en Ventas → CASCADE (eliminar venta si cliente se elimina)

---

## 📊 Table Catalog

### catalogo.TipoDocumento
**Propósito:** Catálogo de tipos de documento (CI, Pasaporte, RUC, etc.)

```sql
CREATE TABLE catalogo.TipoDocumento
(
    Id INT IDENTITY PRIMARY KEY,
    PublicId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Nombre VARCHAR(100) NOT NULL UNIQUE,
    Abreviatura VARCHAR(10) NOT NULL UNIQUE,
    Activo BIT NOT NULL DEFAULT 1,
    FechaRegistro DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FechaActualizacion DATETIME2 NULL
);
```

**Seed data:**
```sql
INSERT INTO catalogo.TipoDocumento (Nombre, Abreviatura)
VALUES ('Cédula de Identidad', 'CI'),
       ('Pasaporte', 'PAS'),
       ('RUC', 'RUC'),
       ('Otro', 'OTR');
```

---

### catalogo.Productos
**Propósito:** Catálogo de productos disponibles

```sql
CREATE TABLE catalogo.Productos
(
    Id INT IDENTITY PRIMARY KEY,
    PublicId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Nombre VARCHAR(200) NOT NULL,
    Descripcion VARCHAR(500) NULL,
    Precio DECIMAL(10,2) NOT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    FechaRegistro DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FechaActualizacion DATETIME2 NULL,
    
    CONSTRAINT UQ_Productos_Nombre UNIQUE (Nombre),
    CONSTRAINT CK_Productos_Precio CHECK (Precio > 0)
);
```

---

### comercial.Clientes
**Propósito:** Base de datos de clientes

```sql
CREATE TABLE comercial.Clientes
(
    Id INT IDENTITY PRIMARY KEY,
    PublicId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    TipoDocumentoId INT NOT NULL,
    NumeroDocumento VARCHAR(20) NOT NULL,
    Nombres VARCHAR(100) NOT NULL,
    ApellidoPaterno VARCHAR(100) NOT NULL,
    ApellidoMaterno VARCHAR(100) NULL,
    Correo VARCHAR(150) NULL,
    Telefono VARCHAR(20) NULL,
    Direccion VARCHAR(250) NULL,
    NombreCompleto AS [Nombres] + ' ' + [ApellidoPaterno] + ' ' + ISNULL([ApellidoMaterno], '') PERSISTED,
    Activo BIT NOT NULL DEFAULT 1,
    FechaRegistro DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FechaActualizacion DATETIME2 NULL,
    
    CONSTRAINT FK_Clientes_TipoDocumento
        FOREIGN KEY (TipoDocumentoId) REFERENCES catalogo.TipoDocumento(Id),
    
    CONSTRAINT UQ_Clientes_NumeroDocumento
        UNIQUE (TipoDocumentoId, NumeroDocumento)
);

-- Índice filtered para email único si no NULL
CREATE UNIQUE INDEX UQ_Clientes_Correo
    ON comercial.Clientes(Correo)
    WHERE Correo IS NOT NULL;
```

---

### seguridad.Usuarios
**Propósito:** Usuarios del sistema con credenciales

```sql
CREATE TABLE seguridad.Usuarios
(
    Id INT IDENTITY PRIMARY KEY,
    PublicId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Email VARCHAR(150) NOT NULL UNIQUE,
    PasswordHash VARCHAR(60) NOT NULL,  -- BCrypt (exactamente 60 caracteres)
    LastLogin DATETIME2 NULL,
    Activo BIT NOT NULL DEFAULT 1,
    FechaRegistro DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FechaActualizacion DATETIME2 NULL
);
```

**Constraints:**
- Email es UNIQUE (más restrictivo que Correo en Clientes)
- PasswordHash exactamente 60 chars (BCrypt HS256)

---

### seguridad.Roles
**Propósito:** Definición de roles del sistema

```sql
CREATE TABLE seguridad.Roles
(
    Id INT IDENTITY PRIMARY KEY,
    PublicId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Nombre VARCHAR(100) NOT NULL UNIQUE,
    Descripcion VARCHAR(500) NULL,
    Activo BIT NOT NULL DEFAULT 1,
    FechaRegistro DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FechaActualizacion DATETIME2 NULL
);
```

**Seed data:**
```sql
INSERT INTO seguridad.Roles (Nombre, Descripcion)
VALUES ('ADMIN', 'Administrador del sistema'),
       ('VENDOR', 'Vendedor'),
       ('READ_ONLY', 'Solo lectura');
```

---

### seguridad.UsuarioRoles (Join Table)
**Propósito:** Mapping many-to-many entre Usuarios y Roles

```sql
CREATE TABLE seguridad.UsuarioRoles
(
    UsuarioId INT NOT NULL,
    RolId INT NOT NULL,
    
    PRIMARY KEY (UsuarioId, RolId),
    CONSTRAINT FK_UR_Usuario FOREIGN KEY (UsuarioId) REFERENCES seguridad.Usuarios(Id),
    CONSTRAINT FK_UR_Rol FOREIGN KEY (RolId) REFERENCES seguridad.Roles(Id)
);
```

---

## 🔍 Índices Estratégicos

| Tabla | Índice | Tipo | Razón |
|-------|--------|------|-------|
| Clientes | `UQ_Clientes_NumeroDocumento` | UNIQUE | Búsqueda por documento |
| Clientes | `UQ_Clientes_Correo` | UNIQUE FILTERED | Email único si no NULL |
| Clientes | (implícito en FK) | FOREIGN | Join con TipoDocumento |
| Usuarios | `Email` | UNIQUE | Login por email |
| Productos | `Nombre` | UNIQUE | Búsqueda de producto |
| Ventas | `ClienteId` | FOREIGN | Historial por cliente |

---

## 🔐 Data Integrity Constraints

### Domain Constraints
```sql
-- Email válido (si no NULL)
ALTER TABLE comercial.Clientes
ADD CONSTRAINT CK_Clientes_EmailFormat 
CHECK (Correo IS NULL OR Correo LIKE '%@%.%');

-- Teléfono válido (si no NULL)
ALTER TABLE comercial.Clientes
ADD CONSTRAINT CK_Clientes_PhoneFormat
CHECK (Telefono IS NULL OR LEN(Telefono) >= 7);
```

### Referential Integrity
- Foreign keys en todas las relaciones
- No permitir orfandad de registros
- ON DELETE CASCADE donde sea seguro

---

## 📈 Performance Considerations

1. **Computed Columns:** PERSISTED para no recalcular
2. **Índices:** En FK, UNIQUE, y campos de búsqueda frecuente
3. **Filtered Indexes:** Para condiciones NULL
4. **Query Optimization:** SIN HasQueryFilter global
5. **Batch Operations:** Usar transaction scope si múltiples inserts

---

## 🔄 Schema Versioning Strategy

**Versión actual:** v3.0.0 (2026-04-30)

**Scripts:**
```
Database/
├── 01_Schemas/
│   └── 01_Schemas.sql         (v1.0.0) - Create schemas
├── 02_Tablas/
│   ├── 01_Productos.sql       (v1.0.0) - Catálogo
│   ├── 02_TipoDocumento.sql   (v1.0.0) - Catálogo
│   ├── 03_Clientes.sql        (v3.0.0) - Comercial (actualizado con índice filtered)
│   └── 04_Auth_Tablas.sql     (v2.0.0) - Seguridad
└── 03_Seeds/
    ├── 01_InitProductos.sql   (v1.0.0)
    ├── 02_InitTipoDocumento.sql (v1.0.0)
    ├── 04_Auth_Seed.sql       (v2.0.0)
    ├── FIX_AddNombreCompletoColumn.sql (v3.0.0) - Migration
    └── FIX_UpdateCorreoConstraint.sql  (v3.0.0) - Migration
```

**Regla:** 
- NO usar EF Migrations
- Versionado manual en `Database/`
- Documentar cambios en comentarios SQL

---

**Última actualización:** 2026-04-30  
**Próxima versión:** v3.1.0 (Módulo Ventas)
