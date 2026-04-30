# Project Status — Nexus-ERP v3.0.0

**Última actualización:** 2026-04-30  
**Estado global:** 🟡 En desarrollo activo — Base lista, problemas resueltos  
**Próxima fase:** Módulo Ventas

---

## 📊 Resumen Ejecutivo

Nexus-ERP es un sistema de gestión comercial empresarial en .NET 8 + SQL Server, arquitectado con Clean Architecture y CQRS pragmático.

**Logros hasta ahora:**
- ✅ Arquitectura base definida y validada
- ✅ Módulos Producto y Cliente completamente funcionales
- ✅ Autenticación JWT implementada (login, permisos)
- ✅ Patrones de código estandarizados y documentados
- ✅ Base de datos con DDL versionado (sin EF Migrations)
- ✅ Problemas críticos resueltos (constraint SQL, computed columns)

**Bloqueadores actuales:**
- Ninguno. Sistema listo para nuevo desarrollo.

---

## 🏗️ Módulos Completados

### 1. Productos (v3.0.0 ✅)
**Ubicación:** `Domain/Comercial/Producto.cs`, `Application/Features/Productos/`, `GestionComercial/Controllers/ProductosController.cs`

**Features:**
- CRUD completo (Create, Read, Update, Delete)
- Soft delete (Activo: 1/0)
- Auditoría automática (FechaRegistro, FechaActualizacion)
- Validación completa (DTOs con atributos)
- Mapeo AutoMapper bidireccional

**Endpoints:**
```
GET    /api/v1/productos              → Lista todos (activos e inactivos)
GET    /api/v1/productos/{id}         → Obtener específico
POST   /api/v1/productos              → Crear
PUT    /api/v1/productos/{id}         → Actualizar
PATCH  /api/v1/productos/{id}/inactivar → Soft delete
PATCH  /api/v1/productos/{id}/activar   → Reactivar
DELETE /api/v1/productos/{id}         → Hard delete
```

**DTOs:** Validación con atributos `[Required]`, `[StringLength]`, `[Range]`, etc.

**Patrón de referencia:** Los nuevos módulos DEBEN copiar exactamente la estructura Producto/Cliente.

---

### 2. Clientes (v3.0.0 ✅)
**Ubicación:** `Domain/Comercial/Cliente.cs`, `Application/Features/Clientes/`, `GestionComercial/Controllers/ClientesController.cs`

**Features:**
- CRUD completo
- Campos: TipoDocumento, NumeroDocumento (UNIQUE por tipo), Nombres, Apellidos, Email (UNIQUE si no NULL), Teléfono, Dirección
- **Computed Column:** `NombreCompleto` (Nombres + ApellidoPaterno + ApellidoMaterno)
- Soft delete + reactivación
- Auditoría automática

**Endpoints:**
```
GET    /api/v1/clientes              → Lista todos
GET    /api/v1/clientes/{id}         → Obtener específico
POST   /api/v1/clientes              → Crear
PUT    /api/v1/clientes/{id}         → Actualizar
PATCH  /api/v1/clientes/{id}/inactivar
PATCH  /api/v1/clientes/{id}/activar
DELETE /api/v1/clientes/{id}         → Hard delete
```

**Decisión SQL importante:** 
- Constraint `UQ_Clientes_Correo` es un **índice filtered** (no UNIQUE constraint)
- Permite múltiples NULLs sin violación
- Enforza unicidad solo cuando `Correo IS NOT NULL`
- Razón: El email es opcional, pero si existe debe ser único

**SQL:**
```sql
CREATE UNIQUE INDEX UQ_Clientes_Correo
    ON comercial.Clientes(Correo)
    WHERE Correo IS NOT NULL;
```

---

### 3. Autenticación (v3.0.0 ✅)
**Ubicación:** `Application/Features/Auth/`, `Infrastructure/Services/JwtService.cs`

**Features:**
- JWT Bearer (HS256)
- BCrypt password hashing (cost factor 11-12)
- Roles: ADMIN, VENDOR, READ_ONLY
- Endpoints: Login, Logout (dummy), /me (perfil)

**Endpoints:**
```
POST   /api/v1/auth/login             → Obtener JWT
POST   /api/v1/auth/logout            → Logout (sin estado)
GET    /api/v1/auth/me [Authorize]    → Perfil del usuario
```

**Usuarios de prueba (incluidos en seed):**
```
admin@nexus.com      | 123456 | ADMIN
vendedor@nexus.com   | 123456 | VENDOR
readonly@nexus.com   | 123456 | READ_ONLY
```

**Configuración (appsettings.json):**
```json
"Jwt": {
  "Key": "tu-clave-secreta-minimo-32-caracteres",
  "Issuer": "nexus-erp-backend",
  "Audience": "nexus-erp-frontend",
  "ExpirationMinutes": 60
}
```

---

## 🐛 Problemas Resueltos (Sesión Actual)

### Problema 1: UNIQUE Constraint on NULL Values ❌ → ✅
**Fecha:** 2026-04-30  
**Severidad:** Alta (bloqueante para crear clientes sin email)

**Síntoma:**
```
SqlException: Violation of UNIQUE KEY constraint 'UQ_Clientes_Correo'
The duplicate key value is (<NULL>)
```

**Root Cause:**
SQL Server trata múltiples NULLs como duplicados cuando hay UNIQUE constraint, aunque teóricamente NULL no debería compararse.

**Solución:**
Reemplazar `CONSTRAINT UNIQUE (Correo)` con **índice filtered**:
```sql
DROP CONSTRAINT UQ_Clientes_Correo;

CREATE UNIQUE INDEX UQ_Clientes_Correo
    ON comercial.Clientes(Correo)
    WHERE Correo IS NOT NULL;
```

**Beneficios:**
- Múltiples clientes sin email (NULL) — sin error
- Clientes con email único — enforced
- Flexible para negocio: "puede ser NULL pero si existe debe ser único"

**Scripts aplicados:**
- `Database/02_Tablas/03_Clientes.sql` — tabla update
- `Database/03_Seeds/FIX_UpdateCorreoConstraint.sql` — migration script

**Status:** ✅ RESUELTO

---

### Problema 2: NombreCompleto Computed Column Missing ❌ → ✅
**Fecha:** 2026-04-27  
**Severidad:** Alta (error de runtime)

**Síntoma:**
```
Invalid column name 'NombreCompleto'
```

**Root Cause:**
Entity Framework estaba configurado para esperar `NombreCompleto` como computed column, pero el SQL DDL no lo creaba.

**Solución:**
1. Agregar a `ClienteConfiguration.cs`:
   ```csharp
   .HasComputedColumnSql(
       "[Nombres] + ' ' + [ApellidoPaterno] + ' ' + ISNULL([ApellidoMaterno], '')",
       stored: true);
   ```

2. Actualizar `Database/02_Tablas/03_Clientes.sql`:
   ```sql
   NombreCompleto AS [Nombres] + ' ' + [ApellidoPaterno] + ' ' + ISNULL([ApellidoMaterno], '') PERSISTED,
   ```

3. Crear migration script `FIX_AddNombreCompletoColumn.sql` para bases existentes

**Status:** ✅ RESUELTO

---

### Problema 3: BCrypt Hash Verification Failure ❌ → ✅
**Fecha:** 2026-04-28  
**Severidad:** Crítica (auth rota)

**Síntoma:**
```
BCrypt.Verify("123456", hash) → false
```

**Root Cause:**
Hashes generados manualmente con trailing dot (61 chars en lugar de 60), por lo que BCrypt no podía verificarlos.

**Solución:**
Regenerar hashes con herramienta online (bcrypt-generator.com) o BCrypt.Net-Next:
```csharp
var hash = BCrypt.Net.BCrypt.HashPassword("123456", workFactor: 12);
```

**Hashes actuales:**
```
admin@nexus.com      → $2a$12$H9eDr9XO.YSWLrJlv5CKuuEYYPPvN2I2DyYCFxHRJCLQ6v6/KGnJ2
vendedor@nexus.com   → $2a$12$JaD2WqXYy0wL2p4SHz3tpOCZiSWL1KKWQSmB3cHKRTQCQ8nKqVCKC
readonly@nexus.com   → $2a$12$FV9Q8n.2U6OQ3y6VPZ7XC.7XZH5LKQ2p1OLcVjVKLjZ3r2W8U4Vci
```

**Regla:** Cost factor 11-12 (balance entre seguridad y velocidad)

**Status:** ✅ RESUELTO (commit: e87648e)

---

### Problema 4: Mapper Pattern Inconsistency ❌ → ✅
**Fecha:** 2026-04-28  
**Severidad:** Media (inconsistencia técnica)

**Síntoma:**
ActualizarClienteHandler usaba asignación manual, mientras Producto usaba `_mapper.Map()`.

**Solución:**
Refactorizar ClienteHandler para usar mapper bidireccional:
```csharp
// Antes
cliente.Nombres = command.Nombres;
cliente.ApellidoPaterno = command.ApellidoPaterno;
// ... más líneas

// Después
_mapper.Map(command, cliente);
cliente.FechaActualizacion = DateTime.UtcNow;
```

**Status:** ✅ RESUELTO

---

## 📝 DTOs y Validación (Estandarizado)

Todos los DTOs llevan validación con atributos + XMLdoc:

**Ejemplo (ClienteDto):**
```csharp
/// <summary>DTO de respuesta para Cliente</summary>
public class ClienteDto
{
    /// <summary>Identificador único</summary>
    [Required(ErrorMessage = "El ID es requerido")]
    public int Id { get; set; }

    /// <summary>Nombre completo concatenado</summary>
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(150, ErrorMessage = "Máximo 150 caracteres")]
    public required string Nombres { get; set; }

    /// <summary>Email del cliente (único si no NULL)</summary>
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string? Correo { get; set; }

    /// <summary>Estado del registro</summary>
    public bool Activo { get; set; }

    /// <summary>Fecha de creación (UTC)</summary>
    public DateTime FechaRegistro { get; set; }
}
```

---

## 🔄 Ciclo de Vida de un Cliente

```
1. POST /api/v1/clientes
   └─ CrearClienteDto (validado)
      └─ CrearClienteCommand (MediatR)
         └─ CrearClienteHandler
            └─ _mapper.Map<Cliente>(command)
               └─ _service.Crear()
                  └─ Entity Framework SaveChangesAsync()

2. GET /api/v1/clientes
   └─ IClienteService.ObtenerTodos()
      └─ Entity Framework (sin filtro, retorna todos)
         └─ _mapper.Map<List<ClienteDto>>(clientes)

3. PUT /api/v1/clientes/{id}
   └─ ActualizarClienteDto (validado)
      └─ ActualizarClienteCommand (MediatR)
         └─ ActualizarClienteHandler
            └─ _mapper.Map(command, cliente)
               └─ cliente.FechaActualizacion = DateTime.UtcNow
                  └─ _service.Actualizar()

4. PATCH /api/v1/clientes/{id}/inactivar
   └─ ActualizarEstadoClienteCommand(id, false)
      └─ cliente.Activo = false
         └─ SaveChangesAsync()

5. DELETE /api/v1/clientes/{id}
   └─ EliminarClienteCommand (MediatR)
      └─ Hard delete (eliminar del contexto)
         └─ SaveChangesAsync()
```

---

## 📦 Archivos Críticos (Modificar con cuidado)

| Archivo | Propósito | Última actualización |
|---------|-----------|---------------------|
| `Infrastructure/Persistence/AppDbContext.cs` | Registro de DbSets | — |
| `Domain/Comercial/AuditableEntity.cs` | Base para todas las entidades | — |
| `Infrastructure/Persistence/Configurations/AuditableEntityConfiguration.cs` | Base para configuraciones | — |
| `Application/Mappings/{Contexto}/Profile.cs` | Mapeos AutoMapper | 2026-04-28 |
| `Database/02_Tablas/*.sql` | DDL versionado | 2026-04-30 |
| `IA_Docs/IMPLEMENTATION_PATTERNS.md` | Estándar obligatorio | 2026-04-28 |
| `CLAUDE.md` | Reglas para IAs | — |

---

## 🎯 Próximas Prioridades

### Fase 1: Consolidar Base (ACTUAL)
- [x] Resolver SQL constraints
- [x] Estandarizar mappers
- [x] Documentar patrones
- [ ] Setup testing (unit, integration)

### Fase 2: Módulo Ventas (v3.1)
- [ ] Entidad: Venta (cabecera)
- [ ] Entidad: VentaDetalle (líneas)
- [ ] CRUD completo
- [ ] Facturación básica

### Fase 3: Módulo Compras (v3.2)
- [ ] Entidad: Compra
- [ ] Entidad: CompraDetalle
- [ ] CRUD + gestión de proveedores

### Fase 4: Inventario (v3.3)
- [ ] Stock management
- [ ] Movimientos de inventario
- [ ] Alertas de stock bajo

### Fase 5: Reportes (v3.4+)
- [ ] Análisis de ventas
- [ ] Dashboard ejecutivo
- [ ] Exportación a Excel

---

## ⚙️ Stack & Versiones

```
.NET                 8.0+
Entity Framework     8.0+
MediatR              12.0+
AutoMapper           13.0+
FluentValidation     11.0+
BCrypt.Net-Next      4.0+
SQL Server           2019+
```

---

## 🔒 Decisiones de Seguridad

1. **Passwords:** BCrypt HS256, cost factor 11-12
2. **JWT:** HS256, duración 60 min (configurable)
3. **Validación:** Obligatoria en todas las DTOs
4. **Controllers:** Sin lógica de negocio
5. **Queries:** Sin filtros globales (el soft delete es auditoría)
6. **Índices:** Filtered para NULLs (caso Correo)

---

## 📞 Notas para Próximas Sesiones

1. **No cambiar patrones sin consultar:** Nuevo módulo = copiar Producto/Cliente exactamente
2. **SQL constraints:** Usar filtered indexes para campos nullable
3. **Computed columns:** Declarar en Configuration.cs Y en SQL DDL
4. **Soft delete:** Es auditoría, no ocultación (GET retorna todos)
5. **AutoMapper:** Bidireccional siempre que sea posible
6. **DTOs:** Validación + XMLdoc + required keyword

---

**Estado:** ✅ Operativo | 🚀 Listo para nuevas features  
**Próxima sesión:** Inicio de módulo Ventas
