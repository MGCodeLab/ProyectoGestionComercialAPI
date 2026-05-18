# Common Issues & Fixes — Troubleshooting Guide

**Propósito:** Documentar problemas encontrados, causas raíz y soluciones para evitar repetirlas.

---

## 🚨 Problemas Críticos Encontrados

### 1. UNIQUE Constraint Violation on NULL Values

**Error Message:**
```
System.Data.SqlClient.SqlException:
Violation of UNIQUE KEY constraint 'UQ_Clientes_Correo'.
Cannot insert duplicate key in object 'comercial.Clientes'.
The duplicate key value is (<NULL>).
```

**Problema:**
SQL Server trata múltiples NULLs como violación de UNIQUE constraint.
```sql
-- ❌ ESTO ES EL PROBLEMA
CONSTRAINT UQ_Clientes_Correo UNIQUE (Correo)  -- Permite NULL pero falla con dos NULLs
```

**Solución:**
Usar **filtered index** que solo aplica constraint donde IS NOT NULL:
```sql
-- ✅ SOLUCIÓN
CREATE UNIQUE INDEX UQ_Clientes_Correo
    ON comercial.Clientes(Correo)
    WHERE Correo IS NOT NULL;
```

**Aplicación:**
1. En `Database/02_Tablas/*.sql` — Cambiar constraint por index
2. En bases existentes — Ejecutar `FIX_UpdateCorreoConstraint.sql`
3. En `Entity Configuration` — Usar `.HasIndex(x => x.Correo).IsUnique()` con IsUnique

**Regla Para Futuro:**
- Todo campo nullable que sea UNIQUE → usar filtered index
- Incluir comentario SQL: `-- Razón: Permite NULLs múltiples, enforza unicidad si no NULL`

---

### 2. Computed Column Not Found in Database

**Error Message:**
```
Microsoft.Data.SqlClient.SqlException:
Invalid column name 'NombreCompleto'
```

**Problema:**
Entity Framework Config esperaba computed column, pero DDL no lo creaba.
```csharp
// ❌ PROBLEM: Config declara computed, SQL no lo tiene
.Property(x => x.NombreCompleto)
    .HasComputedColumnSql("[Nombres] + ' ' + [ApellidoPaterno]...", stored: true);
```

```sql
-- ❌ MISSING
-- La tabla no tenía la columna declarada
```

**Solución:**
1. **En Entity Configuration:**
   ```csharp
   .Property(x => x.NombreCompleto)
       .HasComputedColumnSql(
           "[Nombres] + ' ' + [ApellidoPaterno] + ' ' + ISNULL([ApellidoMaterno], '')",
           stored: true);
   ```

2. **En SQL DDL:**
   ```sql
   NombreCompleto AS [Nombres] + ' ' + [ApellidoPaterno] + ' ' + ISNULL([ApellidoMaterno], '') PERSISTED,
   ```

3. **Para bases existentes:**
   ```sql
   -- En FIX_AddNombreCompletoColumn.sql
   IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                  WHERE TABLE_NAME = 'Clientes' AND COLUMN_NAME = 'NombreCompleto')
   BEGIN
       ALTER TABLE comercial.Clientes
       ADD NombreCompleto AS [Nombres] + ' ' + [ApellidoPaterno] + ' ' + ISNULL([ApellidoMaterno], '') PERSISTED;
   END
   ```

**Regla Para Futuro:**
- Toda computed column debe estar:
  1. En Entity (`public string NombreCompleto { get; set; }`)
  2. En Configuration (`.HasComputedColumnSql(...)`)
  3. En SQL DDL (`AS [expression]`)
- No ignorar ninguno de estos tres

---

### 3. BCrypt Password Hash Verification Fails

**Error Message:**
```
BCrypt.Net.BCrypt.Verify(password, hash) → always false
```

**Síntomas:**
- Login falla incluso con credenciales correctas
- Hash contiene trailing characters o tiene longitud incorrecta (61+ en lugar de 60)

**Problema:**
Hashes generados manualmente con caracteres extra.
```
❌ Incorrecto: $2a$12$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2uheWG/igi.
✅ Correcto:  $2a$12$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2uheWG/igi
```

**Solución:**
1. **Generar hash correcto en código:**
   ```csharp
   var hash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
   // Resultado: $2a$12$... (60 caracteres exactos)
   ```

2. **Usar herramienta online:** https://bcrypt-generator.com (verificar cost factor 11-12)

3. **Verificar:**
   ```csharp
   var isValid = BCrypt.Net.BCrypt.Verify("123456", hash);
   ```

4. **En seed SQL:** Insertar hash exacto, sin trailing characters
   ```sql
   INSERT INTO seguridad.Usuarios (Email, PasswordHash, ...)
   VALUES ('admin@nexus.com', '$2a$12$...exacto...', ...)
   ```

**Regla Para Futuro:**
- Cost factor: 11-12 (balance seguridad/velocidad)
- Hash length: exactamente 60 caracteres
- Verificar siempre con `BCrypt.Verify()` antes de insertar
- Si modificas seed, regenerar hashes con herramienta confiable

---

### 3b. (2026-05-10) Application Layer Importing Infrastructure — **RESUELTO**

**ENCONTRADO EN:** Sprint 1 Catálogos Base (Handlers de Pais)  
**ESTADO:** ✅ RESUELTO (2026-05-10 14:00)

**Error Message:**
```
CS0246: El nombre del tipo o del espacio de nombres 'Infrastructure' no se encontró
CS0246: El nombre del tipo o del espacio de nombres 'AppDbContext' no se encontrado
```

**Problema:**
Handlers en Application layer importaban `Infrastructure.Persistence.AppDbContext` directamente:
```csharp
// ❌ INCORRECTO (en Application/Features/Catalogo/Pais/Crear/CrearPaisHandler.cs)
using Infrastructure.Persistence;  // Application NO debe referenciar Infrastructure

public class CrearPaisHandler : IRequestHandler<CrearPaisCommand, int>
{
    private readonly AppDbContext _context;  // Violación Clean Architecture
    // ... usa _context directamente
}
```

**Causa:**
Clean Architecture requiere que Application sea agnóstica de detalles de persistencia. Application debe comunicarse SOLO a través de interfaces (servicios).

**Solución Implementada (2026-05-10):**

1. **Refactor de Handlers → Inyectar Services:**
   ```csharp
   // ✅ CORRECTO
   using Application.Interfaces;  // Application conoce interfaces

   public class CrearPaisHandler : IRequestHandler<CrearPaisCommand, int>
   {
       private readonly IPaisService _paisService;  // Interface, no implementación

       public CrearPaisHandler(IPaisService paisService, IMapper mapper, ILogger<CrearPaisHandler> logger)
       {
           _paisService = paisService;
       }

       public async Task<int> Handle(CrearPaisCommand request, CancellationToken cancellationToken)
       {
           var pais = _mapper.Map<Pais>(request);
           return await _paisService.Crear(pais, cancellationToken);  // Usa service
       }
   }
   ```
   
   **Cambios realizados:**
   - `CrearPaisHandler.cs` ✅
   - `ActualizarPaisHandler.cs` ✅
   - `ActualizarEstadoPaisHandler.cs` ✅
   - `EliminarPaisHandler.cs` ✅

2. **Refactor de Validators → Usar ValidatorService:**
   ```csharp
   // ✅ CORRECTO (ValidatorService en Infrastructure)
   public class PaisValidatorService : IPaisValidatorService
   {
       private readonly AppDbContext _context;

       public async Task<bool> IsCodigoUnique(string codigo, CancellationToken cancellationToken)
           => !await _context.Paises.AnyAsync(p => p.Codigo == codigo, cancellationToken);
   }

   // ✅ Validator en Application (depende de interface)
   public class CrearPaisValidator : AbstractValidator<CrearPaisCommand>
   {
       private readonly IPaisValidatorService _validatorService;

       public CrearPaisValidator(IPaisValidatorService validatorService)
       {
           _validatorService = validatorService;
           RuleFor(x => x.Codigo)
               .MustAsync(BeUniqueCode).WithMessage("El código del país ya existe");
       }

       private async Task<bool> BeUniqueCode(string codigo, CancellationToken cancellationToken)
       {
           return await _validatorService.IsCodigoUnique(codigo, cancellationToken);
       }
   }
   ```
   
   **Archivos creados:**
   - `Application/Interfaces/IPaisValidatorService.cs` ✅
   - `Infrastructure/Repository/PaisValidatorService.cs` ✅
   - Registrado en `Program.cs` (DI) ✅

3. **Resultado:**
   ```
   dotnet build → 0 errores, 0 advertencias
   ✅ Clean Architecture respetada
   ✅ Compilación exitosa
   ```

**Regla Para Futuro:**
- Application NUNCA importa Infrastructure.* (clases concretas)
- Application SIEMPRE importa Application.Interfaces
- Application.Interfaces define contratos, Infrastructure implementa
- Handlers usan Services, no AppDbContext
- Si necesitas persistencia en validación, usar Service específico

**Patrón de Referencia:** 
- ClienteHandler (existente) — patrón correcto a seguir
- PaisHandler (nuevo refactorizado) — ahora sigue patrón correcto

---

### 4. AutoMapper Mapper Pattern Inconsistency

**Síntomas:**
- Algunos handlers usan `_mapper.Map()`, otros asignación manual
- DTOs y Entities se mapean de formas diferentes
- Mantenimiento inconsistente

**Problema:**
```csharp
// ❌ INCONSISTENTE (ActualizarClienteHandler antiguo)
cliente.Nombres = command.Nombres;
cliente.ApellidoPaterno = command.ApellidoPaterno;
cliente.ApellidoMaterno = command.ApellidoMaterno;
cliente.Correo = command.Correo;
// ... 15 líneas más

// ✅ CONSISTENTE (ActualizarProductoHandler)
_mapper.Map(command, cliente);
cliente.FechaActualizacion = DateTime.UtcNow;
```

**Solución:**
1. **En Profile.cs:**
   ```csharp
   CreateMap<ActualizarClienteCommand, Cliente>();  // Bidireccional
   CreateMap<ActualizarClienteCommand, Cliente>().ReverseMap();
   ```

2. **En Handler:**
   ```csharp
   _mapper.Map(command, cliente);  // Una línea, clara
   cliente.FechaActualizacion = DateTime.UtcNow;  // Solo override manual si es necesario
   await _service.Actualizar(cancellationToken);
   ```

**Regla Para Futuro:**
- Crear siempre maps bidireccionales: `.ReverseMap()`
- Usar `_mapper.Map(source, destination)` en handlers
- Nunca usar asignación manual a menos que sea lógica compleja

---

## ⚠️ Problemas de Desarrollo

### API Response Format Mismatch (Resolved, 2026-04-26)

**Situación:**
Frontend esperaba camelCase (`success`, `message`, `data`), backend devolvía PascalCase (`Success`, `Message`, `Data`).

**Descubrimiento:**
Frontend ya tenía configuración para manejar PascalCase, así que no hubo rompimiento.

**Documentación:**
Registrado en `IA_Docs/PENDING_IMPROVEMENTS.md` punto 2.

**Regla Para Futuro:**
- Usar `[JsonPropertyName("camelCase")]` si necesitas camelCase en JSON
- Verificar con frontend antes de cambiar formato de respuesta

---

### Build Lock Errors

**Error Message:**
```
The process cannot access the file because it is being used by another process
```

**Causa:**
Instancias antiguas de `dotnet run` o `dotnet watch` aún usando dlls.

**Solución:**
```powershell
# Matar procesos dotnet
Get-Process dotnet -ErrorAction SilentlyContinue | Stop-Process -Force

# Limpiar y reconstruir
dotnet clean
dotnet build
```

**Regla Para Futuro:**
- Usar `dotnet clean` antes de rebuild si ves errores de acceso
- En VS, usar "Clean Solution" antes de "Rebuild Solution"

---

### Nullable Property Warnings

**Error Message:**
```
CS8618: Non-nullable property 'Nombres' must contain a non-null value
```

**Solución:**
Usar `required` keyword en DTOs:
```csharp
// ✅ CORRECTO
public required string Nombres { get; set; }

// O con [Required] attribute
[Required(ErrorMessage = "El nombre es requerido")]
public string Nombres { get; set; }
```

**Regla Para Futuro:**
- Properties de DTOs DEBE tener `required` o nullable (`?`)
- Verificar siempre con atributos de validación

---

## 🔍 Debugging Common Scenarios

### Scenario 1: Login Always Returns 401

**Checklist:**
1. ¿Hash BCrypt es válido? (60 caracteres, sin trailing)
   ```sql
   SELECT Email, LEN(PasswordHash) as HashLen FROM seguridad.Usuarios;
   -- Debe ser 60 para todos
   ```

2. ¿Email existe en BD?
   ```sql
   SELECT * FROM seguridad.Usuarios WHERE Email = 'admin@nexus.com';
   ```

3. ¿JWT Key está configurada en appsettings.json? (mínimo 32 caracteres)
   ```json
   "Jwt": {
     "Key": "...", // Mínimo 32 chars
     "ExpirationMinutes": 60
   }
   ```

4. ¿BCrypt.Verify está en LoginHandler?
   ```csharp
   if (!BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash))
       throw new UnauthorizedException("Email o contraseña incorrectos");
   ```

---

### Scenario 2: GET Always Returns Empty Data

**Checklist:**
1. ¿Service tiene `ObtenerTodos()`?
   ```csharp
   public async Task<List<Cliente>> ObtenerTodos(CancellationToken cancellationToken)
   {
       return await _dbContext.Clientes.ToListAsync(cancellationToken);
   }
   ```

2. ¿Controller mapea correctamente?
   ```csharp
   var clientes = await _service.ObtenerTodos(HttpContext.RequestAborted);
   var result = _mapper.Map<List<ClienteDto>>(clientes);
   return this.OkResponse(result);
   ```

3. ¿DbContext está registrado? (Program.cs)
   ```csharp
   builder.Services.AddDbContext<AppDbContext>(options =>
       options.UseSqlServer(connectionString));
   ```

4. ¿Connection string es correcta?
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=.\\SQLEXPRESS;Database=NexusERP;..."
   }
   ```

---

### Scenario 3: Computed Column Shows NULL

**Problema:**
`NombreCompleto` existe pero retorna NULL o valores incorrectos.

**Checklist:**
1. ¿Columna está creada como PERSISTED?
   ```sql
   SELECT definition FROM sys.computed_columns WHERE name = 'NombreCompleto';
   ```

2. ¿Fórmula SQL es correcta?
   ```sql
   -- Debe ser exactamente esto:
   [Nombres] + ' ' + [ApellidoPaterno] + ' ' + ISNULL([ApellidoMaterno], '')
   ```

3. ¿Algunos ApellidoMaterno son NULL? (ISNULL debe manejar)
   ```sql
   SELECT Nombres, ApellidoPaterno, ApellidoMaterno, NombreCompleto
   FROM comercial.Clientes
   WHERE ApellidoMaterno IS NULL;
   ```

4. ¿Entity está mapped?
   ```csharp
   .Property(x => x.NombreCompleto)
       .HasComputedColumnSql("[Nombres] + ' ' + [ApellidoPaterno]...", stored: true);
   ```

---

## 📋 Checklist para Nuevas Features

Antes de crear nuevo módulo, verificar:

- [ ] Entity hereda `AuditableEntity`
- [ ] Configuration hereda `AuditableEntityConfiguration<T>`
- [ ] DTOs llevan validación y XMLdoc
- [ ] Commands creados para operaciones CREATE/UPDATE/DELETE
- [ ] Handlers creados con `_mapper.Map()`
- [ ] Service interface definida
- [ ] AutoMapper Profile con mappings bidireccionales
- [ ] Controller con endpoints RESTful
- [ ] SQL DDL script creado y versionado
- [ ] Seed data si es necesario
- [ ] Documentación en IA_Docs actualizada

---

## 🔄 Hotfix Workflow

Si encuentras un problema crítico:

1. **Diagnosticar** → ¿Qué falla exactamente?
2. **Aislar** → ¿Reproducible consistentemente?
3. **Documentar** → Agregar a este archivo
4. **Solucionar** → Fix en código + SQL si aplica
5. **Verificar** → Test que no rompe nada
6. **Commit** → `fix(modulo): descripción` con buena referencia
7. **Actualizar docs** → Este archivo + PROJECT_STATUS.md

---

### 5. (2026-05-10) DTO Actualizar Faltante — **RESUELTO**

**ENCONTRADO EN:** Sprint 1 Catálogos (ModuloSistema y ParametroSistema)  
**ESTADO:** ✅ RESUELTO (2026-05-10 18:00)

**Error Message:**
```
CS0246: El nombre del tipo o del espacio de nombres 'ActualizarModuloSistemaDto' no se encontró
CS0246: El nombre del tipo o del espacio de nombres 'ActualizarParametroSistemaDto' no se encontró
```

**Problema:**
Controllers creados esperaban DTOs `ActualizarXxxDto`, pero estos no fueron creados durante la fase inicial de DTOs.

```csharp
// ❌ INCORRECTO (en Controller)
public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarModuloSistemaDto dto)
// Pero ActualizarModuloSistemaDto no existía
```

**Causa:**
DTOs iniciales (Crear, Response) fueron creados, pero olvidó generar el DTO Actualizar para cada entidad. Verificación de compilación antes de crear Controllers habría detectado esto.

**Solución Implementada (2026-05-10):**

1. **Crear DTO Actualizar para cada entidad:**
   ```csharp
   // Application/Dtos/Catalogo/ActualizarModuloSistemaDto.cs
   public class ActualizarModuloSistemaDto
   {
       [Required] public required string Nombre { get; set; }
       [Required] public required string Codigo { get; set; }
       [StringLength(500)] public string? Descripcion { get; set; }
   }
   ```

2. **Aplica a entidades:**
   - `ActualizarModuloSistemaDto` ✅
   - `ActualizarParametroSistemaDto` ✅
   - `ActualizarUnidadMedidaDto` (no necesitaba, solo se usaba creación)

3. **Resultado:**
   ```
   dotnet build → 0 errores, 0 advertencias
   Controllers compilaban exitosamente
   ```

**Regla Para Futuro:**
- Checklist ANTES de crear Controller:
  1. ¿Existe CrearXxxDto? (para POST)
  2. ¿Existe ActualizarXxxDto? (para PUT)
  3. ¿Existe XxxDto? (para GET response)
  4. ¿Todos tienen validación con [Required], [StringLength]?
  5. ¿Compilación? (dotnet build antes de escribir Controller)

---

### 6. (2026-05-16) Record Parameter Ordering in Update Commands — **RESUELTO**

**ENCONTRADO EN:** Sprint 2 Organización (Actualizar Empresa, Sucursal, Almacén)  
**ESTADO:** ✅ RESUELTO (2026-05-16 14:00)

**Síntoma:**
Controller intenta usar `command with { Id = id }` para actualizar el Id del command, pero falla si el parámetro `Id` no está al final del record:

```csharp
// ❌ PROBLEMA: Id está al inicio
public record ActualizarEmpresaCommand(
    int Id,
    string RazonSocial,
    // ... más parámetros
) : IRequest<int>;

// En Controller:
command = command with { Id = id };  // Error en compilación si Id no está al final
```

**Causa:**
En C# records, cuando usas `with { }` (copy-constructor), los parámetros con valores por defecto deben estar al final del constructor. Si `Id` está primero sin valor por defecto, no puedes usar `with` sin proporcionar todos los demás parámetros.

**Solución Implementada (2026-05-16):**

1. **Reordenar parámetros en Update Commands:**
   ```csharp
   // ✅ CORRECTO: Parámetros sin default primero, Id al final con default
   public record ActualizarEmpresaCommand(
       string RazonSocial,
       string? NombreComercial,
       string NumeroDocumento,
       int TipoDocumentoId,
       int PaisId,
       int MonedaBaseId,
       string? DireccionFiscal,
       string? Telefono,
       string? Correo,
       string? LogoUrl,
       int Id = 0  // ← Al final con default
   ) : IRequest<int>;
   ```

2. **Mantener en Controller:**
   ```csharp
   var command = _mapper.Map<ActualizarEmpresaCommand>(dto);
   command = command with { Id = id };  // Ahora funciona
   ```

3. **Aplicado a:**
   - `ActualizarEmpresaCommand` ✅
   - `ActualizarSucursalCommand` ✅
   - `ActualizarAlmacenCommand` ✅

**Regla Para Futuro:**
- **Record parameter ordering:**
  - Parámetros sin default value → primero (en orden lógico)
  - Parámetros con default value → último
  - Ejemplo patrón: (nombre, email, telefono, id=0)
- Si necesitas `with { }` para actualizar un parámetro, debe tener default value
- Validación en code review: ¿Puedo compilar Controller.Update con `command with { }`?

---

### 7. (2026-05-16) SQL Table Naming Conventions — Plural Form — **RESUELTO**

**ENCONTRADO EN:** Sprint 2 Organización (Script 07_Empresas.sql)  
**ESTADO:** ✅ RESUELTO (2026-05-16 13:00)

**Síntoma:**
Foreign Key violation porque nombre de tabla en REFERENCES no coincide con nombre real:

```sql
-- ❌ PROBLEMA
CONSTRAINT FK_Empresas_TipoDocumento
    FOREIGN KEY (TipoDocumentoId)
    REFERENCES catalogo.TipoDocumento(Id),  -- ❌ Tabla no existe, es "TipoDocumentos"

-- En BD real existe:
CREATE TABLE catalogo.TipoDocumentos (...)  -- Plural
```

**Causa:**
Inconsistencia de convención: algunas tablas fueron creadas en plural (`TipoDocumentos`), pero FK reference asumió singular (`TipoDocumento`). Sin `SET FOREIGN_KEY_CHECKS OFF`, SQL Server rechaza la constraint.

**Solución Implementada (2026-05-16):**

1. **Corrección en Script:**
   ```sql
   -- ✅ CORRECTO
   CONSTRAINT FK_Empresas_TipoDocumento
       FOREIGN KEY (TipoDocumentoId)
       REFERENCES catalogo.TipoDocumentos(Id),  -- ← Plural
   ```

2. **Aplicado a:**
   - `Database/02_Tablas/07_Empresas.sql` → FK a `catalogo.TipoDocumentos` ✅

3. **Verificación post-fix:**
   ```sql
   -- Scripts ejecutados sin error
   DROP TABLE IF EXISTS organizacion.Almacenes;
   DROP TABLE IF EXISTS organizacion.Sucursales;
   DROP TABLE IF EXISTS organizacion.Empresas;
   
   -- Re-run 07_Empresas.sql → ✅ SUCCESS
   ```

**Regla Para Futuro:**
- **SQL Naming Convention:** Todas las tablas en PLURAL
  - ✅ `catalogo.Paises` (entidad: País)
  - ✅ `catalogo.Monedas` (entidad: Moneda)
  - ✅ `catalogo.TipoDocumentos` (entidad: TipoDocumento)
  - ✅ `organizacion.Empresas` (entidad: Empresa)
  - ✅ `organizacion.Sucursales` (entidad: Sucursal)
  - ✅ `organizacion.Almacenes` (entidad: Almacén)

- **Tabla → Nombre singular en código:**
  - Tabla `Paises` → Entity `Pais` (singular, no plural)
  - Tabla `Monedas` → Entity `Moneda`
  - DbSet → nombre tabla (plural): `DbSet<Pais> Paises { get; }`

- **Checklist FK creation:**
  1. ¿Existe la tabla referenciada?
  2. ¿Nombre está en PLURAL?
  3. Verificar: `SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'catalog'`
  4. Compilar script sin ejecutar primero (check sintaxis)

---

---

### 8. (2026-05-17) Sprint 3 Fiscal Entities — Compilation Fixes — **RESUELTO**

**ENCONTRADO EN:** Sprint 3 Catálogos Fiscales (TipoImpuesto, TipoComprobante, SerieDocumento)  
**ESTADO:** ✅ RESUELTO (2026-05-17 14:30) — **Compilación exitosa con 0 errores**

#### 8a. Missing AuditableEntity Using Statement

**Síntoma:**
```
CS0246: El nombre del tipo o del espacio de nombres 'AuditableEntity' no se encontró
```

**Problema:**
Domain entities (TipoImpuesto, TipoComprobante, SerieDocumento) creados sin la declaración `using Domain.Common;`:
```csharp
// ❌ INCORRECTO
namespace Domain.Catalogo;

public class TipoImpuesto : AuditableEntity  // AuditableEntity no está en scope
{
    // ...
}
```

**Solución:**
```csharp
// ✅ CORRECTO
using Domain.Common;

namespace Domain.Catalogo;

public class TipoImpuesto : AuditableEntity
{
    // Hereda: PublicId, Activo, FechaRegistro, FechaActualizacion
}
```

**Archivos corregidos:**
- `Domain/Catalogo/TipoImpuesto.cs` ✅
- `Domain/Catalogo/TipoComprobante.cs` ✅
- `Domain/Catalogo/SerieDocumento.cs` ✅

---

#### 8b. Clean Architecture Violation: Handlers Injecting Infrastructure ValidatorServices

**Síntoma:**
```
CS0246: El nombre del tipo o del espacio de nombres 'TipoImpuestoValidatorService' no se encontró
(Handlers en Application layer no pueden referenciar Infrastructure.Repository)
```

**Problema:**
Inicial: Intenté inyectar ValidatorServices directamente en Handlers:
```csharp
// ❌ INCORRECTO (violación Clean Architecture)
using Infrastructure.Repository;  // Application NO debe referenciar Infrastructure

public class CrearTipoImpuestoHandler : IRequestHandler<CrearTipoImpuestoCommand, int>
{
    private readonly TipoImpuestoValidatorService _validatorService;
    // Application layer no puede conocer implementaciones de Infrastructure
}
```

**Causa:**
Clean Architecture requiere que Application sea agnóstica de detalles de persistencia. Application comunica SOLO a través de interfaces; Infrastructure implementa esas interfaces.

**Solución Implementada (2026-05-17):**

1. **Patrón correcto: Application layer → Service interface → Infrastructure implementation**
   ```csharp
   // ✅ CORRECTO en Handler
   using Application.Interfaces;  // Application solo conoce interfaces
   
   public class CrearTipoImpuestoHandler : IRequestHandler<CrearTipoImpuestoCommand, int>
   {
       private readonly ITipoImpuestoService _service;
       
       public async Task<int> Handle(CrearTipoImpuestoCommand command, CancellationToken ct)
       {
           var entidad = new TipoImpuesto 
           { 
               Nombre = command.Nombre,
               Codigo = command.Codigo,
               Porcentaje = command.Porcentaje,
               EsIncluido = command.EsIncluido,
               Activo = true
           };
           
           await _service.Crear(entidad);  // Service valida y persiste
           return entidad.Id;
       }
   }
   ```

2. **Validación moveada a Service layer:**
   ```csharp
   // ✅ EN INFRASTRUCTURE (ITipoImpuestoService implementation)
   public class TipoImpuestoService : ITipoImpuestoService
   {
       private readonly AppDbContext _context;
       private readonly ILogger<TipoImpuestoService> _logger;
       
       public async Task Crear(TipoImpuesto entidad)
       {
           // Validación única AQUÍ (en Infrastructure)
           if (await _context.TiposImpuesto.AnyAsync(t => t.Codigo == entidad.Codigo))
               throw new InvalidOperationException($"Código {entidad.Codigo} ya existe");
           
           _context.TiposImpuesto.Add(entidad);
           await _context.SaveChangesAsync();
           _logger.LogInformation("TipoImpuesto creado: {Id}", entidad.Id);
       }
   }
   ```

3. **Archivos creados/modificados:**
   - `Application/Features/Catalogo/TipoImpuesto/Crear/CrearTipoImpuestoHandler.cs` ✅
   - `Application/Features/Catalogo/TipoComprobante/Crear/CrearTipoComprobanteHandler.cs` ✅
   - `Application/Features/Catalogo/SerieDocumento/Crear/CrearSerieDocumentoHandler.cs` ✅
   - (+ 6 handlers más para Actualizar, ActualizarEstado, Eliminar)
   - `Infrastructure/Repository/TipoImpuestoService.cs` ✅
   - `Infrastructure/Repository/TipoComprobanteService.cs` ✅
   - `Infrastructure/Repository/SerieDocumentoService.cs` ✅

**Patrón de Referencia:**
```
Catalogo (Sprint 1-2) → Patrón correcto a seguir desde ahora
Fiscal (Sprint 3) → Implementación limpia sin violaciones de capas
```

---

#### 8c. Ambiguous Type Name: SerieDocumento (Namespace vs Class)

**Síntoma:**
```
CS0118: 'SerieDocumento' es espacio de nombres pero se usa como tipo
```

**Problema:**
Namespace `Application.Features.Catalogo.SerieDocumento.Crear` conflictúa con tipo `SerieDocumento`:
```csharp
// ❌ INCORRECTO
var serie = new SerieDocumento  // Compiler confunde: ¿namespace o clase?
{
    TipoComprobanteId = command.TipoComprobanteId,
    // ...
};
```

**Causa:**
C# confunde el namespace `SerieDocumento.Crear` con la clase entity `SerieDocumento` cuando intenta instanciar sin fully qualified name.

**Solución:**
```csharp
// ✅ CORRECTO: Fully qualified type name
var serie = new Domain.Catalogo.SerieDocumento
{
    TipoComprobanteId = command.TipoComprobanteId,
    SucursalId = command.SucursalId,
    Serie = command.Serie,
    NumeroActual = 0,
    NumeroMaximo = command.NumeroMaximo,
    Activo = true
};
```

**Archivos corregidos:**
- `Application/Features/Catalogo/SerieDocumento/Crear/CrearSerieDocumentoHandler.cs` ✅

**Regla Para Futuro:**
- Si la clase entity tiene namespace con mismo nombre: usar fully qualified name
- Patrón: `new Domain.<Contexto>.<EntidadClass>` es siempre seguro
- C# resolverá primero namespaces antes de tipos, causando ambigüedad

---

#### 8d. File-Scoped Namespace Syntax Mismatch

**Síntoma:**
```
CS0103: 'OkResponse' no existe en el contexto actual
```

**Problema:**
Controllers creados con namespace tradicional (braces) en lugar de file-scoped syntax:
```csharp
// ❌ INCORRECTO
namespace API.GestionComercial.Controllers
{
    [ApiController]
    public class TiposImpuestoController : ControllerBase
    {
        // ... pero extensions requieren file-scoped namespace
    }
}
```

**Causa:**
Existing controllers en el proyecto usan file-scoped namespace (`namespace API.GestionComercial.Controllers;`), y la extensión `using API.GestionComercial.Extensions;` espera ese patrón.

**Solución:**
```csharp
// ✅ CORRECTO: File-scoped namespace (semicolon, no braces)
using API.GestionComercial.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.GestionComercial.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TiposImpuestoController : ControllerBase
{
    // Ahora OkResponse extension está en scope
}
```

**Archivos corregidos:**
- `GestionComercial/Controllers/TiposImpuestoController.cs` ✅
- `GestionComercial/Controllers/TiposComprobanteController.cs` ✅
- `GestionComercial/Controllers/SeriesDocumentoController.cs` ✅

**Patrón:**
- C# 10+: file-scoped namespace es el estándar moderno
- Ventaja: menos indentación, más clara
- Síntaxis: `namespace Name;` (con punto y coma) vs `namespace Name { ... }`

---

#### 8e. Generic Type Inference Failure: OkResponse<T> with Null Parameters

**Síntoma:**
```
CS0411: Los argumentos de tipo para el método 'ControllerExtensions.OkResponse<T>...' 
        no se pueden inferir a partir del uso
```

**Problema:**
Endpoints UPDATE/PATCH/DELETE retornan `OkResponse(null, "mensaje")`, pero el generic type `T` no puede ser inferido de `null`:
```csharp
// ❌ INCORRECTO
return this.OkResponse(null, "TipoImpuesto actualizado correctamente");
// Compiler no puede inferir T cuando primer parámetro es null
```

**Causa:**
El método `OkResponse<T>(T data, string message)` necesita tipo explícito cuando `data` es `null`. C# no puede infer `T` de `null`.

**Solución:**
Proporcionar tipo explícito cuando retornas null:
```csharp
// ✅ CORRECTO: Type explicit
return this.OkResponse<object>(null, "TipoImpuesto actualizado correctamente");
```

**Patrón general:**
```csharp
// GET (data existe)
public async Task<IActionResult> Obtener(int id)
{
    var dato = await _service.ObtenerPorIdAsync(id);
    return this.OkResponse(dato);  // ✅ Type inferred from dato
}

// PUT/PATCH/DELETE (no retornas data)
public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarDto dto)
{
    await _service.Actualizar(...);
    return this.OkResponse<object>(null, "mensaje");  // ✅ Type explicit
}

// POST (retornas ID o identificador)
public async Task<IActionResult> Crear([FromBody] CrearDto dto)
{
    var id = await _mediator.Send(command);
    return this.CreatedResponse(nameof(Obtener), new { id }, id);  // ✅ Type inferred
}
```

**Archivos corregidos (replace_all):**
- `GestionComercial/Controllers/TiposImpuestoController.cs` — 4 ocurrencias ✅
- `GestionComercial/Controllers/TiposComprobanteController.cs` — 4 ocurrencias ✅
- `GestionComercial/Controllers/SeriesDocumentoController.cs` — 4 ocurrencias ✅
- **Total: 12 líneas corregidas con tipo explícito `<object>`**

**Regla Para Futuro:**
- Si llamas `OkResponse(null, ...)` → **SIEMPRE usa `OkResponse<object>(null, ...)`**
- Esto aplica a: PUT, PATCH, DELETE que no retornan data
- Si retornas data (GET, POST): el tipo se infiere automáticamente

---

## 📊 Resumen Estadístico Sprint 3

| Métrica | Valor |
|---------|-------|
| Entidades nuevas | 3 (TipoImpuesto, TipoComprobante, SerieDocumento) |
| Archivos creados | 24+ |
| Handlers totales | 9 (3 entidades × 3 operaciones) |
| Controllers creados | 3 |
| Errores iniciales | 5 categorías |
| Errores finales | 0 |
| Advertencias (nullability) | 12 (warnings, no errores) |
| Estado compilación final | ✅ **EXITOSA** |
| Tiempo de resolución | ~2 horas |

---

### 9. (2026-05-17) SQL Server FOREIGN KEY Syntax — ON DELETE RESTRICT vs NO ACTION — **RESUELTO**

**ENCONTRADO EN:** Sprint 3 SQL Scripts (12_SeriesDocumento.sql)  
**ESTADO:** ✅ RESUELTO (2026-05-17 14:45)

**Síntoma:**
```
Incorrect syntax near the keyword 'RESTRICT'.
```

**Problema:**
Script SQL creado con sintaxis estándar SQL (ANSI), pero SQL Server no reconoce `ON DELETE RESTRICT`:

```sql
-- ❌ INCORRECTO en SQL Server
CONSTRAINT [FK_SeriesDocumento_TiposComprobante] FOREIGN KEY ([TipoComprobanteId])
    REFERENCES [catalogo].[TiposComprobante]([Id]) ON DELETE RESTRICT,
```

**Causa:**
`RESTRICT` es sintaxis estándar SQL pero **SQL Server NO la soporta**. SQL Server requiere `NO ACTION` (comportamiento equivalente).

**Diferencia:**
- `RESTRICT` → estándar ANSI SQL (funciona en PostgreSQL, MySQL, MariaDB)
- `NO ACTION` → específico de SQL Server (sintaxis equivalente a RESTRICT)

Ambos previenen eliminar registros referenciados, pero la sintaxis varía por base de datos.

**Solución Implementada (2026-05-17):**

1. **Cambio en Script:**
   ```sql
   -- ✅ CORRECTO para SQL Server
   CONSTRAINT [FK_SeriesDocumento_TiposComprobante] FOREIGN KEY ([TipoComprobanteId])
       REFERENCES [catalogo].[TiposComprobante]([Id]) ON DELETE NO ACTION,
   CONSTRAINT [FK_SeriesDocumento_Sucursales] FOREIGN KEY ([SucursalId])
       REFERENCES [organizacion].[Sucursales]([Id]) ON DELETE NO ACTION,
   ```

2. **Archivo corregido:**
   - `Database/02_Tablas/12_SeriesDocumento.sql` ✅ (2 cambios)

3. **Verificación:**
   ```powershell
   grep -n "ON DELETE RESTRICT" Database/02_Tablas/*.sql
   # (sin resultados — problema resuelto)
   ```

**Regla Para Futuro:**
- **SQL Server keywords para foreign keys:**
  - ❌ `ON DELETE RESTRICT` → no soportado
  - ✅ `ON DELETE NO ACTION` → previene delete (estándar SQL Server)
  - ✅ `ON DELETE CASCADE` → elimina registros dependientes
  - ✅ `ON DELETE SET NULL` → asigna NULL a FK
  - ✅ `ON DELETE SET DEFAULT` → asigna valor default a FK

- **Checklist para scripts SQL:**
  1. ¿Usas `RESTRICT`? → Cambiar a `NO ACTION`
  2. ¿Verificas sintaxis antes de ejecutar?
  3. ¿Testeas en SQL Server antes de commit?

- **Si necesitas trabajar multi-BD (PostgreSQL + SQL Server):**
  - Usar `NO ACTION` (compatible con ambas)
  - O generar scripts específicos por BD
  - Documentar incompatibilidades en README

**Referencia SQL Server:**
- [MS Docs: FOREIGN KEY Constraints](https://learn.microsoft.com/en-us/sql/t-sql/statements/alter-table-table-constraint-transact-sql)
- Delete actions: RESTRICT no listado → usar NO ACTION equivalente

---

### 10. (2026-05-17) FromSqlInterpolated Non-Composable with UPDATE — **RESUELTO**

**ENCONTRADO EN:** Sprint 3 (SerieDocumentoService.ObtenerProximoNumeroAsync)  
**ESTADO:** ✅ RESUELTO (2026-05-17 15:00)

**Síntoma:**
```
System.InvalidOperationException: 'FromSql' or 'SqlQuery' was called with non-composable SQL 
and with a query composing over it. Consider calling 'AsEnumerable' after the method to 
perform the composition on the client side.
```

**Problema:**
`FromSqlInterpolated` con UPDATE statements es non-composable. EF Core no permite llamar `.FirstOrDefaultAsync()` directamente después:

```csharp
// ❌ INCORRECTO
var serie = await _context.SeriesDocumento
    .FromSqlInterpolated($@"
        UPDATE catalogo.SeriesDocumento ...
        SELECT * FROM catalogo.SeriesDocumento ...
    ")
    .FirstOrDefaultAsync(ct);  // Error: intenta componer LINQ sobre raw SQL
```

**Causa:**
Cuando `FromSqlInterpolated` contiene UPDATE (no solo SELECT), el resultado es non-composable. EF Core no puede aplicar `.FirstOrDefaultAsync()` porque:
1. UPDATE statements no son component queries
2. EF Core requiere materializaralización explícita antes de aplicar LINQ

**Solución Implementada (2026-05-17):**

1. **Materializar primero con `.ToListAsync()`:**
   ```csharp
   // ✅ CORRECTO
   var resultado = await _context.SeriesDocumento
       .FromSqlInterpolated($@"
           UPDATE catalogo.SeriesDocumento WITH (ROWLOCK, UPDLOCK)
           SET NumeroActual = NumeroActual + 1
           WHERE Id = {serieDocumentoId}
               AND (NumeroMaximo IS NULL OR NumeroActual < NumeroMaximo)
           
           SELECT * FROM catalogo.SeriesDocumento
           WHERE Id = {serieDocumentoId}
       ")
       .ToListAsync(ct);  // Materializa el resultado async
   
   var serie = resultado.FirstOrDefault();  // Luego filtra en memory
   ```

2. **Archivo corregido:**
   - `Infrastructure/Repository/SerieDocumentoService.cs` (línea 66-68) ✅

3. **Verificación:**
   ```powershell
   dotnet build
   # → ✅ 0 errores, compilación exitosa
   ```

**Patrón de Referencia:**
- Siempre que usas `FromSqlInterpolated` con sentencias complejas (UPDATE, DELETE), materializa primero
- Para queries puras (SELECT), puedes componer directamente sin `.ToListAsync()`

**Regla Para Futuro:**
- **Raw SQL + LINQ composition:**
  - ❌ `.FromSqlInterpolated(...).FirstOrDefaultAsync()`
  - ✅ `.FromSqlInterpolated(...).ToListAsync()` → `.FirstOrDefault()`
  
- **Cuándo aplicar:**
  - Queries complejas con UPDATE/DELETE
  - Cuando necesitas aplicar filtros adicionales post-ejecución
  
- **Alternativa:** Si el filtro es simple, incluirlo en el SQL directo en lugar de LINQ

---

### 11. (2026-05-18) CQRS Commands Missing DTO Fields — **RESUELTO**

**ENCONTRADO EN:** Sprint 4 (CrearProductoCommand, ActualizarProductoCommand)  
**ESTADO:** ✅ RESUELTO (2026-05-18 19:30)

**Síntoma:**
```
Valores del DTO no llegan al backend (null).
PUT /api/v1/productos/{id} envía:
{
  "unidadMedidaId": 1,
  "categoriaProductoId": 2,
  "marcaProductoId": 1
}
Pero en el handler recibe todo como null.
```

**Problema:**
Cuando se agregan nuevos campos a un DTO (ej: `ActualizarProductoDto`), la correspondencia CQRS command TAMBIÉN debe actualizarse. Si el command record no tiene los campos, AutoMapper los mapea como null aunque el DTO los tenga:

```csharp
// ❌ INCORRECTO — Command incompleto
public record ActualizarProductoCommand(
    string Nombre,
    string? Descripcion,
    decimal Precio,
    int Id = 0
) : IRequest<Unit>;
// Campos Sprint 4 FALTANTES

// ✅ DTO tiene los campos
public class ActualizarProductoDto
{
    public string Nombre { get; set; }
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int? UnidadMedidaId { get; set; }      // ← Aquí están
    public int? CategoriaProductoId { get; set; }  // ← Aquí están
    public int? MarcaProductoId { get; set; }      // ← Aquí están
}
```

Cuando AutoMapper intenta mapear `ActualizarProductoDto` → `ActualizarProductoCommand`:
- Los parámetros nuevos **NO EXISTEN** en el command record
- AutoMapper ignora silenciosamente los valores que no tienen destino
- Los valores llegan como null al handler

**Causa Raíz:**
En C# records, los parámetros del constructor definen las propiedades. Si no están en el constructor, no existen como propiedades del record. AutoMapper no puede mapear a propiedades que no existen.

**Solución Implementada (2026-05-18):**

1. **Agregar parámetros faltantes al record:**
   ```csharp
   // ✅ CORRECTO — Command completo
   public record ActualizarProductoCommand(
       string Nombre,
       string? Descripcion,
       decimal Precio,
       int? UnidadMedidaId = null,        // ← Agregado
       int? CategoriaProductoId = null,   // ← Agregado
       int? MarcaProductoId = null,       // ← Agregado
       int Id = 0
   ) : IRequest<Unit>;
   ```

2. **Agregar mappings explícitos en AutoMapper:**
   ```csharp
   CreateMap<ActualizarProductoDto, ActualizarProductoCommand>()
       .ForMember(d => d.UnidadMedidaId, opt => opt.MapFrom(s => s.UnidadMedidaId))
       .ForMember(d => d.CategoriaProductoId, opt => opt.MapFrom(s => s.CategoriaProductoId))
       .ForMember(d => d.MarcaProductoId, opt => opt.MapFrom(s => s.MarcaProductoId));
   ```

3. **Archivos corregidos:**
   - `Application/Features/Productos/Actualizar/ActualizarProductoCommand.cs` ✅
   - `Application/Features/Productos/Crear/CrearProductoCommand.cs` ✅
   - `Application/Mappings/Productos/ProductoProfile.cs` ✅

4. **Verificación post-fix:**
   ```
   PUT /api/v1/productos/{id} con:
   { "unidadMedidaId": 1, "categoriaProductoId": 2, "marcaProductoId": 1 }
   ✅ Valores ahora llegan correctamente al handler
   ```

**Regla Para Futuro — CRÍTICA:**

Cuando agregas nuevos campos a un DTO, **SIEMPRE actualiza los Commands correspondientes:**

**Checklist de sincronización DTO ↔ Command:**

1. **DTO agregó nuevos campos:**
   ```csharp
   public class CrearProductoDto
   {
       public string Nombre { get; set; }
       // ... campos existentes ...
       public int? UnidadMedidaId { get; set; }  // ← NUEVO
   }
   ```

2. **Command DEBE tener esos parámetros:**
   ```csharp
   public record CrearProductoCommand(
       string Nombre,
       // ... parámetros existentes ...
       int? UnidadMedidaId = null  // ← DEBE AGREGARSE AQUÍ
   ) : IRequest<int>;
   ```

3. **AutoMapper DEBE mapearlos explícitamente:**
   ```csharp
   CreateMap<CrearProductoDto, CrearProductoCommand>()
       .ForMember(d => d.UnidadMedidaId, opt => opt.MapFrom(s => s.UnidadMedidaId));
   ```

4. **Handler usa el campo del Command:**
   ```csharp
   var producto = _mapper.Map<Producto>(request);
   // producto.UnidadMedidaId ya tiene el valor
   ```

**Anti-patrón: Silenciosa pérdida de datos**

Este problema es silencioso porque:
- ✅ Compilación exitosa (sin errores)
- ✅ Valores enviados por cliente (request válido)
- ❌ Datos no llegan al handler (null inesperado)
- ❌ Testing manual lo descubre (no compilación)

**Prevención:**

1. **Integración test:** Verificar que todos los campos del DTO se propagan
   ```csharp
   [Test]
   public void ActualizarProductoCommand_MapsAllDtoFields()
   {
       var dto = new ActualizarProductoDto { UnidadMedidaId = 1 };
       var command = _mapper.Map<ActualizarProductoCommand>(dto);
       Assert.That(command.UnidadMedidaId, Is.EqualTo(1));
   }
   ```

2. **Code Review checklist:**
   - ¿El DTO tiene nuevos campos?
   - ¿El Command record también los tiene?
   - ¿El AutoMapper mapping está actualizado?

3. **Architecture rule:**
   - **DTO → Command:** Campo a campo (1:1)
   - **Command → Entity:** Campo a campo (1:1)
   - Si falta un campo en cualquier etapa, el dato se pierde

---

## 🎯 Hallazgos Clave y Experiencias (Sprint 3)

### 1. **Importancia de Using Statements en Entity Base Classes**
Cada entity que hereda `AuditableEntity` DEBE tener `using Domain.Common;`. Esto es fácil de olvidar cuando generan múltiples archivos a la vez. **Solución:** Template o checklist para creación de entidades.

### 2. **Clean Architecture No Es Negociable**
La violación de capas (Application inyectando Infrastructure) es tentadora cuando "solo quieres validar algo", pero rompe la arquitectura. **Lección:** Validación pertenece a Service layer, nunca a Handler. Handlers solo usan Services.

### 3. **Namespace Conflicts Con Entidades**
Cuando creas feature folder con nombre igual a la entidad, pueden surgir ambigüedades. **Solución:** Fully qualified names (`Domain.Catalogo.NombreTipo`) son siempre seguros.

### 4. **File-Scoped Namespace Es Estándar en el Proyecto**
Todos los controllers existentes usan `namespace X;` (C# 10+). Mantener consistencia es crítico para que extensions funcionen. **Regla:** Copiar patrón exacto de archivo existente.

### 5. **Generic Type Inference y Null**
C# no puede infer tipo genérico de `null`. Parece obvio en retrospectiva, pero es fácil olvidar cuando retorna muchos endpoints. **Solución:** IDE warnings son amigos — escuchalos.

### 6. **Concurrencia en SerieDocumento Requiere SERIALIZABLE + ROWLOCK**
El handler `ObtenerProximoNumero` usa transaction isolation y SQL hints para evitar race conditions. **Patrón:** Copiar exactamente para cualquier operación atómica futura.

---

## 🎯 Hallazgos Clave y Experiencias (Sprint 4)

### 1. **CQRS Command Records Deben Ser Sincronizados con DTOs**
Cuando se agregan campos a un DTO, el Command record correspondiente DEBE actualizarse también. Si no, AutoMapper los mapea como null sin error de compilación. **Lección:** La arquitectura CQRS requiere sincronización manual entre DTO y Command — no hay validación automática.

### 2. **AutoMapper Mappings Son Silenciosamente Lenientes**
Si el campo no existe en el record, AutoMapper ignora el valor del DTO sin advertencia. Esto pasa desapercibido hasta testing manual. **Solución:** Mappings explícitos (`.ForMember()`) son mejor práctica que confiar en property matching automático.

### 3. **SQL Server Syntax Requiere Revisión de Documentation**
`ON DELETE RESTRICT` no funciona en SQL Server; requiere `NO ACTION`. Esto es fácil olvidar si escribes scripts basados en ANSI SQL estándar (PostgreSQL/MySQL). **Regla:** Siempre verificar sintaxis con SQL_SERVER_COMPATIBILITY.md antes de crear scripts.

### 4. **Self-Reference Foreign Keys Funcionan Correctamente con EF Core**
Configurar self-ref FK con DeleteBehavior.Restrict es directo. El patrón: `navigation.Padre` y `navigation.Subcategorias` se configura igual que FK normal. **Patrón:** Copiar exacto de CategoriaProductoConfiguration.

### 5. **Validación de Profundidad (Hierarchical Depth Limits) Va en Application, No en Database**
Limitar a 3 niveles jerárquicos no es un constraint de BD; es una rule de aplicación en el Handler/Validator. **Patrón:** CalcularProfundidadAsync() en Service, MustAsync() en Validator.

### 6. **Prevención de Ciclos en Self-Reference Requiere Graph Traversal**
No es trivial: necesitas `EsDescendienteDeAsync()` que recorre el árbol. **Patrón:** Implementar en Service, llamar desde ActualizarHandler antes de permitir cambio de padre.

---

## 🔗 Referencias Relacionadas

- **IMPLEMENTATION_PATTERNS.md** — Patrones para entidades, handlers, servicios
- **VALIDATOR_SERVICE_PATTERN.md** — Dónde y cómo validar
- **DATABASE_SETUP_INSTRUCTIONS.md** — Scripts y ejecución
- **COMMON_ISSUES_AND_FIXES.md** (este archivo) — Soluciones rápidas

---

**Última revisión:** 2026-05-18 (Sprint 4 en progreso)  
**Próxima revisión:** Después de smoke testing SQL scripts y API endpoints + validación campos DTO→Command
