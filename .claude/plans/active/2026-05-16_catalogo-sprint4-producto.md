# Sprint 4: Producto Enriquecido (CategoriaProducto, MarcaProducto, ALTER Productos)

**Estado:** ⏳ **PENDIENTE**  
**Fecha Estimada Inicio:** 2026-05-24  
**Duración Estimada:** 5-6 horas  
**Rama:** `catalogo-base/sprint_4`  
**Complejidad:** 🟡 **MEDIA** (ALTER TABLE + patrones conocidos)

---

## 📋 Objetivo

Enriquecer catálogo Productos con categorización jerárquica y marcas:
- **CategoriaProducto**: Árbol de categorías con self-referencia
- **MarcaProducto**: Catálogo de marcas
- **ALTER Productos**: Agregar 3 ForeignKeys nullable (migración segura)

**Dependencias:** Sprint 1 (UnidadMedida)  
**Impacta:** Productos existentes (migración)  
**Bloquea:** Módulo Ventas (Productos enriquecidos)

---

## 🎯 Entidades a Crear (2) + Migración (1)

### 1. CategoriaProducto → `catalogo.CategoriasProducto`

```
Nombre              NVARCHAR(150) NOT NULL
Descripcion         NVARCHAR(500) NULL
CategoriaPadreId    INT NULL → FK catalogo.CategoriasProducto (RESTRICT)
PublicId            GUID (via AuditableEntity)
Activo              BIT DEFAULT 1

-- Validación: Máx 3 niveles de profundidad (application rule)
```

**Características:**
- Self-referencia: Una categoría puede tener padre (crear árbol)
- Árbol jerárquico: Electrónica > Computadoras > Laptops
- Restricción: Máximo 3 niveles (prevenir árboles profundos)
- No permitir ciclos: `CategoriaPadre.CategoriaPadreId != self`

---

### 2. MarcaProducto → `catalogo.MarcasProducto`

```
Nombre              NVARCHAR(150) NOT NULL
Descripcion         NVARCHAR(500) NULL
LogoUrl             NVARCHAR(500) NULL
PublicId            GUID (via AuditableEntity)
Activo              BIT DEFAULT 1
```

**Características:**
- Catálogo simple de marcas
- CRUD estándar
- Asociación a Productos vía FK

---

### 3. ALTER TABLE Productos (Migración Segura)

```sql
-- Agregar 3 ForeignKeys nullable
ALTER TABLE catalogo.Productos ADD
    UnidadMedidaId      INT NULL → FK catalogo.UnidadesMedida (RESTRICT),
    CategoriaProductoId INT NULL → FK catalogo.CategoriasProducto (RESTRICT),
    MarcaProductoId     INT NULL → FK catalogo.MarcasProducto (RESTRICT);
```

**Características:**
- ✅ ForeignKeys NULLABLE (productos existentes sin cambios)
- ✅ Migration script idempotente (puede ejecutarse múltiples veces)
- ✅ No rompe lógica existente
- ✅ Preparado para hacer NOT NULL en futuro si negocio lo requiere

---

## ⚠️ Riesgo Crítico: ALTER TABLE Productos

### Escenario del Problema

```
Datos existentes:
┌────┬─────────┬──────────────────────────┐
│ Id │ Nombre  │ UnidadMedidaId (NUEVA)  │
├────┼─────────┼──────────────────────────┤
│ 1  │ Laptop  │ NULL                      │
│ 2  │ Mouse   │ NULL                      │
└────┴─────────┴──────────────────────────┘

PROBLEMA: Si las FKs son NOT NULL → ALTER TABLE FALLA
SOLUCIÓN: Hacer FKs NULLABLE → productos existentes conservan NULL
```

### Mitigación Requerida

**Script SQL idempotente:**
```sql
-- En: Database/02_Tablas/FIX_AddProductoFKs.sql
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Productos' 
    AND COLUMN_NAME = 'UnidadMedidaId'
)
BEGIN
    ALTER TABLE catalogo.Productos ADD
        UnidadMedidaId      INT NULL,
        CategoriaProductoId INT NULL,
        MarcaProductoId     INT NULL;
    
    ALTER TABLE catalogo.Productos ADD
        CONSTRAINT FK_Productos_UnidadMedida 
            FOREIGN KEY (UnidadMedidaId) 
            REFERENCES catalogo.UnidadesMedida(Id);
    -- ... más constraints
END
GO
```

**En código C#:**
- DTOs: `UnidadMedidaId` OPCIONAL (nullable en crear/actualizar)
- Services: No validar si está presente
- Controllers: Ignorar si null en actualización

---

## 📁 Archivos a Crear: ~18 nuevos

### Entidades Domain (2)
- `Domain/Catalogo/CategoriaProducto.cs`
- `Domain/Catalogo/MarcaProducto.cs`

### Commands (8)
- Crear (2): CrearCategoriaProductoCommand, CrearMarcaProductoCommand
- Actualizar (2): ActualizarCategoriaProductoCommand, ActualizarMarcaProductoCommand
- ActualizarEstado (2)
- Eliminar (2)

### Handlers (8)
- Crear (2): Con validación de profundidad en CategoriaProducto
- Actualizar (2): Con validación de ciclos en CategoriaProducto
- ActualizarEstado (2)
- Eliminar (2)

### Validators (4)
- Crear/Actualizar para CategoriaProducto (incluye validación de profundidad)
- Crear/Actualizar para MarcaProducto

### DTOs (6)
- CrearCategoriaProductoDto, ActualizarCategoriaProductoDto, CategoriaProductoDto
- CrearMarcaProductoDto, ActualizarMarcaProductoDto, MarcaProductoDto

### AutoMapper Profiles (2)
- CategoriaProductoProfile, MarcaProductoProfile

### Services (4)
- CategoriaProductoService, MarcaProductoService
- CategoriaProductoValidatorService, MarcaProductoValidatorService

### Entity Configurations (2)
- CategoriaProductoConfiguration (incluye self-ref navigation)
- MarcaProductoConfiguration

### Controllers (2 = 14 endpoints)
- **CategoriasProductoController** (7 endpoints)
- **MarcasProductoController** (7 endpoints)

### Database Scripts (3)
- `Database/02_Tablas/13_CategoriasProducto.sql`
- `Database/02_Tablas/14_MarcasProducto.sql`
- `Database/02_Tablas/FIX_AddProductoFKs.sql` (migration script)

---

## 🔧 Decisiones de Implementación

### 1. Validación de Profundidad (CategoriaProducto)

```csharp
// En CrearCategoriaProductoValidator
public class CrearCategoriaProductoValidator : AbstractValidator<CrearCategoriaProductoCommand>
{
    public CrearCategoriaProductoValidator(ICategoriaProductoService service)
    {
        RuleFor(x => x.CategoriaPadreId)
            .MustAsync(async (parentId, ct) =>
            {
                if (parentId == null) return true; // OK, es raíz
                
                var depth = await service.CalcularProfundidad(parentId.Value);
                return depth < 3; // Máx 3 niveles
            })
            .WithMessage("Máximo 3 niveles de profundidad permitidos");
    }
}
```

### 2. Prevención de Ciclos (Actualizar)

```csharp
// En ActualizarCategoriaProductoValidator
RuleFor(x => x.CategoriaPadreId)
    .MustAsync(async (cmd, parentId, ct) =>
    {
        if (parentId == null) return true; // OK
        
        // Validar que el padre no sea descendiente del actual
        var isDescendant = await service.IsDescendantOf(parentId.Value, cmd.Id);
        return !isDescendant; // NO permitir ciclos
    })
    .WithMessage("No se puede crear ciclo: padre no puede ser descendiente");
```

### 3. Seed Data (CategoriaProducto)

```sql
INSERT INTO catalogo.CategoriasProducto (Nombre, Descripcion, CategoriaPadreId, Activo)
VALUES
('Electrónica', 'Productos electrónicos', NULL, 1),
('Computadoras', 'Equipos de cómputo', 1, 1),  -- Padre: Electrónica
('Laptops', 'Computadoras portátiles', 2, 1),  -- Padre: Computadoras
('Accesorios', 'Accesorios tecnológicos', NULL, 1);
```

---

## 📝 Cambios en Productos Existentes

### ProductoDto (Actualizar)
```csharp
public class ProductoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public decimal Precio { get; set; }
    // Nuevas propiedades opcionales:
    public int? UnidadMedidaId { get; set; }
    public int? CategoriaProductoId { get; set; }
    public int? MarcaProductoId { get; set; }
    // ... propiedades existentes
}
```

### CrearProductoDto / ActualizarProductoDto (Actualizar)
```csharp
public class CrearProductoDto
{
    // Campos existentes
    public string Nombre { get; set; }
    public decimal Precio { get; set; }
    
    // Nuevos campos OPCIONALES
    public int? UnidadMedidaId { get; set; }
    public int? CategoriaProductoId { get; set; }
    public int? MarcaProductoId { get; set; }
}
```

---

## ✅ Checklist Pre-Implementación

- [ ] Backup de tabla Productos antes de ALTER
- [ ] Script migration idempotente probado
- [ ] Validación de profundidad implementada
- [ ] Prevención de ciclos implementada
- [ ] Seed data de categorías validado
- [ ] DTOs de Producto actualizados
- [ ] Mappers de Producto actualizados
- [ ] Testing: ALTER TABLE no rompe datos existentes

---

## 📊 Métricas Esperadas

| Item | Planeado |
|------|----------|
| Entidades nuevas | 2 |
| Commands | 8 |
| Handlers | 8 |
| Validators | 4 |
| DTOs | 6 |
| Endpoints | 14 |
| SQL Scripts | 3 |
| Cambios en Productos | DTOs + Entity Navigation |
| Compilación esperada | 0 errores |
| Tiempo estimado | 5-6 horas |

---

## 🔗 Referencias

- **Dependencias:** Sprint 1 (UnidadMedida)
- **Impacta:** Tabla Productos (migración)
- **Bloquea:** Módulo Ventas (Productos completos)
- **Riesgo crítico:** RG-03 (ALTER TABLE)
- **Pattern especial:** Self-ref CategoriaProducto + validación profundidad

---

**Siguiente paso:** Iniciar después Sprint 3 completado

*Documento creado:* 2026-05-16  
*Estado:* ⏳ Pendiente
