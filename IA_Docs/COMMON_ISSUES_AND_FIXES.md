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

**Última revisión:** 2026-04-30  
**Próxima revisión:** Después de implementar Ventas
