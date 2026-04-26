# Nexus-ERP: Knowledge Base — Documento Principal de IA

**Última actualización:** 2026-04-25  
**Versión activa:** v3.0.0  
**Estado:** ✅ Build OK — Módulos Producto, Cliente y Auth implementados  
**Propietario:** Miguel González Cuevas

> Este documento es la fuente de verdad para cualquier IA o desarrollador que continúe el proyecto.  
> Léelo completo antes de proponer o implementar cualquier cambio.

---

## 1. ¿Qué es este proyecto?

**Nexus-ERP** es un sistema de gestión comercial empresarial real, destinado a producción con clientes reales. No es un demo ni una práctica.

Cubre gestión de:
- Clientes
- Catálogo de productos
- Autenticación y control de acceso (JWT + roles + permisos)
- Ventas, Compras, Inventario (próximas fases)

**Frontend:** Angular 19+, completamente separado del backend. Se comunica vía API REST con Bearer token.  
**Backend:** Este repositorio. .NET 10 + SQL Server.

---

## 2. Stack Tecnológico

| Capa | Tecnología |
|------|-----------|
| Runtime | .NET 10, C# 13 |
| ORM | Entity Framework Core 10 |
| Base de Datos | SQL Server 2019+ |
| Autenticación | JWT Bearer (HS256) |
| Password Hashing | BCrypt.Net-Next (cost 11) |
| CQRS | MediatR |
| Validación | FluentValidation |
| Mapping | AutoMapper (modular Profiles) |
| Frontend | Angular 19+ (repositorio separado) |
| IDE | Visual Studio (no VS Code) |
| Puerto Dev | http://localhost:5198 |

---

## 3. Arquitectura

### Capas (Clean Architecture)

```
Domain/           ← Entidades puras. Sin dependencias externas.
Application/      ← Casos de uso, DTOs, Commands, Validators, Interfaces
Infrastructure/   ← EF Core, Configurations, Services (JWT, Usuario, etc.)
GestionComercial/ ← API: Controllers, Middleware, Extensions, Program.cs
Database/         ← Scripts SQL versionados (no usa EF Migrations)
```

### Regla de dependencias
```
API → Application ← Infrastructure
         ↓
       Domain
```
Application NUNCA referencia Infrastructure. Infrastructure sí referencia Application (implementa sus interfaces).

### CQRS Pragmático
- **Commands** (crear, actualizar, eliminar, autenticar) → **MediatR**
- **Queries** (leer lista, leer por id) → **Services directos**
- No se usa MediatR para queries: es overhead innecesario para lecturas simples.

---

## 4. Entidades del Dominio

### Base obligatoria: `AuditableEntity` (`Domain/Common/AuditableEntity.cs`)
```csharp
public abstract class AuditableEntity
{
    public int Id { get; set; }                    // PK interna (int, IDENTITY)
    public Guid PublicId { get; private set; }     // GUID externo (NEWSEQUENTIALID)
    public bool Activo { get; set; } = true;       // Soft delete flag
    public DateTime FechaRegistro { get; set; }    // GETUTCDATE() automático
    public DateTime? FechaActualizacion { get; set; } // Actualizado manualmente en handlers
}
```

**TODA entidad hereda `AuditableEntity` sin excepción.**

### Entidades existentes

| Entidad | Namespace | Schema BD | Notas |
|---------|-----------|-----------|-------|
| `Producto` | `Domain.Catalogo` | `catalogo` | CRUD completo |
| `TipoDocumento` | `Domain.Catalogo` | `catalogo` | Catálogo soporte para Cliente |
| `Cliente` | `Domain.Comercial` | `comercial` | CRUD + soft delete + FK TipoDocumento |
| `Usuario` | `Domain.Seguridad` | `seguridad` | Auth, tiene UsuarioRoles |
| `Rol` | `Domain.Seguridad` | `seguridad` | Tiene RolPermisos y UsuarioRoles |
| `Permiso` | `Domain.Seguridad` | `seguridad` | Recurso + Accion, tiene RolPermisos |
| `UsuarioRol` | `Domain.Seguridad` | `seguridad` | Pivot, sin herencia AuditableEntity |
| `RolPermiso` | `Domain.Seguridad` | `seguridad` | Pivot, sin herencia AuditableEntity |

---

## 5. Módulos Implementados

### 5.1 Producto ✅
**Endpoints:** `GET /api/v1/productos`, `GET /{id}`, `POST`, `PUT /{id}`, `DELETE /{id}`  
**Soft delete:** Solo `Activo` flag, sin PATCH de estado propio (usar PUT)

### 5.2 Cliente ✅
**Endpoints:** `GET /api/v1/clientes`, `GET /{id}`, `POST`, `PUT /{id}`, `PATCH /{id}/inactivar`, `PATCH /{id}/activar`, `DELETE /{id}`  
**FK:** TipoDocumentoId obligatorio  
**Unique constraints:** (TipoDocumentoId, NumeroDocumento) y Correo  
**NombreCompleto:** Columna computada en BD = Nombres + ApellidoPaterno + ApellidoMaterno

### 5.3 Auth ✅ (v3.0.0)
**Endpoints:**
- `POST /api/v1/auth/login` — [AllowAnonymous] — devuelve JWT + perfil + permisos
- `POST /api/v1/auth/logout` — [AllowAnonymous] — respuesta dummy OK (limpieza en frontend)
- `GET /api/v1/auth/me` — [Authorize] — perfil del usuario autenticado

**JWT Claims:** sub (userId), email, nombre, roles (ClaimTypes.Role), iat, jti  
**Algoritmo:** HS256  
**Expiración:** 60 minutos (configurable en `appsettings.json → Jwt:ExpirationMinutes`)  
**Password hashing:** BCrypt (cost 11) en `UsuarioService.AutenticarUsuario()`

**Usuarios de prueba:**
| Email | Password | Rol |
|-------|----------|-----|
| admin@nexus.com | 123456 | ADMIN (todos los permisos) |
| vendedor@nexus.com | 123456 | VENDOR (create/edit/view en productos, clientes, ventas) |
| readonly@nexus.com | 123456 | READ_ONLY (sin permisos) |

---

## 6. Patrones y Convenciones

### 6.1 Configuraciones EF Core
```csharp
// TODOS heredan esto:
public abstract class AuditableEntityConfiguration<T> : IEntityTypeConfiguration<T>
    where T : AuditableEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.PublicId).HasDefaultValueSql("NEWSEQUENTIALID()");
        builder.Property(e => e.Activo).HasDefaultValue(true);
        builder.Property(e => e.FechaRegistro).HasDefaultValueSql("GETUTCDATE()");
        builder.HasIndex(e => e.PublicId).IsUnique();
    }
}
// Configuración específica de entidad hereda y llama base.Configure(builder)
// + builder.ToTable("Nombre", schema: "esquema")
```

### 6.2 Response Wrapper
```csharp
// TODA respuesta de la API usa ApiResponse<T>
return this.OkResponse(data, "mensaje");        // HTTP 200
return this.CreatedResponse(...);               // HTTP 201
return this.NotFoundResponse("mensaje");        // HTTP 404
return this.FailResponse("mensaje", errors);    // HTTP 400
return this.UnauthorizedResponse("mensaje");    // HTTP 401
// Formato JSON:
{ "success": true/false, "message": "...", "data": {}, "errors": [], "traceId": "..." }
```

### 6.3 Soft Delete
**REGLA CRÍTICA:** Soft delete es un flag de auditoría, NO es ocultamiento de datos.
- `GET /api/v1/clientes` → devuelve activos E inactivos
- `PATCH /{id}/inactivar` → pone `Activo=false`, registro sigue visible
- `DELETE /{id}` → hard delete real (EF `Remove()`)
- El frontend Angular controla la presentación visual (colores, filtros, scroll)
- **NUNCA** agregar `HasQueryFilter(x => x.Activo)` en AppDbContext

### 6.4 Estructura de un Comando (patrón)
```
Application/Features/{Modulo}/{Operacion}/
  ├── {Operacion}{Entidad}Command.cs  ← record : IRequest<T>
  ├── {Operacion}{Entidad}Handler.cs  ← IRequestHandler<Command, T>
  └── {Operacion}{Entidad}Validator.cs ← AbstractValidator<Command>
```

### 6.5 Estructura de un Service (patrón)
```
Application/Interfaces/I{Entidad}Service.cs   ← contrato (solo lectura + operaciones de infra)
Infrastructure/Repository/{Entidad}Service.cs ← implementación con AppDbContext
```

### 6.6 Convención de commits
```
feat(modulo): descripcion
fix(modulo): descripcion
refactor(modulo): descripcion
chore(infra): descripcion
docs: descripcion
```

---

## 7. Estructura de Archivos Críticos

| Archivo | Propósito |
|---------|-----------|
| `GestionComercial/Program.cs` | DI completo, JWT, CORS, Middleware pipeline |
| `GestionComercial/appsettings.json` | ConnectionString + Jwt config |
| `Domain/Common/AuditableEntity.cs` | Base class de todas las entidades |
| `Infrastructure/Persistence/AppDbContext.cs` | EF DbContext con todos los DbSets |
| `Infrastructure/Persistence/Configurations/AuditableEntityConfiguration.cs` | Template base |
| `Application/Common/Models/ApiResponse.cs` | Wrapper de respuesta |
| `GestionComercial/Middleware/ExceptionMiddleware.cs` | Manejo global de errores |
| `GestionComercial/Extensions/ControllerExtensions.cs` | Métodos de respuesta |
| `GestionComercial/DependencyInjection.cs` | AddApplication() — MediatR + FluentValidation |

---

## 8. Base de Datos

**Sin EF Migrations.** La BD se gestiona manualmente con scripts SQL en `Database/`.

### Schemas
| Schema | Propósito |
|--------|-----------|
| `catalogo` | Productos, TipoDocumentos |
| `comercial` | Clientes |
| `seguridad` | Usuarios, Roles, Permisos, tablas pivot |

### Scripts a ejecutar (orden obligatorio)
```
1. Database/01_Schemas/01_Schemas.sql
2. Database/02_Tablas/01_Productos.sql
3. Database/02_Tablas/02_TipoDocumento.sql
4. Database/02_Tablas/03_Clientes.sql
5. Database/02_Tablas/04_Auth_Tablas.sql
6. Database/03_Seeds/01_InitProductos.sql
7. Database/03_Seeds/02_InitTipoDocumento.sql
8. Database/03_Seeds/04_Auth_Seed.sql
```

### Nota sobre NEWSEQUENTIALID()
`NEWSEQUENTIALID()` solo funciona en cláusulas `DEFAULT` de DDL, NO en `INSERT VALUES`.  
En seeds: omitir `PublicId` y `FechaRegistro` para que SQL Server los genere con el DEFAULT.

---

## 9. Decisiones Arquitectónicas Registradas

| Decisión | Elegido | Rechazado | Razón |
|----------|---------|-----------|-------|
| Queries | Services directos | MediatR para queries | Overhead innecesario para lecturas simples |
| Repository pattern | Services específicos | Repository genérico | Evitar abstracción prematura; se evalúa UoW cuando llegue Ventas |
| Soft delete | Flag `Activo` sin filter global | HasQueryFilter en DbContext | Frontend necesita ver todos los registros; soft delete = auditoría |
| JWT algoritmo | HS256 | RS256 | Entorno no distribuido, simplicidad suficiente |
| BCrypt | En Infrastructure (UsuarioService) | En Application | Application no puede depender de paquetes externos de hashing |
| Logout | Endpoint dummy OK | Blacklist de tokens | Frontend maneja estado; blacklist se evalúa en fases futuras |
| /me endpoint | Query a BD | Decodificar JWT en handler | Datos frescos; permisos actualizados en cada request |

---

## 10. Problemas Conocidos y Soluciones

| Problema | Causa | Solución |
|----------|-------|----------|
| `Invalid column name 'PublicId'` | Scripts DDL viejos sin columnas audit | Ejecutar script de migration o recrear tablas |
| `NEWSEQUENTIALID()` en INSERT | Uso incorrecto en VALUES | Omitir columna en INSERT; el DEFAULT la genera |
| Puerto 5198 en uso | Instancia anterior viva | `Get-NetTCPConnection -LocalPort 5198 \| Stop-Process` |
| IService not registered | Falta `AddScoped` en Program.cs | Agregar `builder.Services.AddScoped<IService, Impl>()` |
| Soft delete filtraba datos | `HasQueryFilter` mal aplicado | Revertido; retornar todos los registros |

---

## 11. Roadmap

### v3.0.0 (actual — en cierre)
- ✅ Módulo Producto (CRUD)
- ✅ Módulo Cliente (CRUD + soft delete)
- ✅ Módulo Auth (JWT + roles + permisos)
- ✅ AuditableEntity estandarizado
- ✅ Base de datos normalizada

### Post v3.0.0
1. **Módulo Ventas** — Encabezado + Detalle, FK Cliente + Producto, calcular totales. Evaluar UnitOfWork aquí.
2. **Módulo Compras** — Similar a Ventas con Proveedores.
3. **Módulo Inventario** — Movimientos de stock.
4. **Refresh Token** — Actualmente tokens expiran y user re-loguea. Implementar refresh en fase posterior.
5. **[Authorize] en otros controllers** — Actualmente los endpoints de Productos y Clientes no requieren auth. Se habilitará cuando el frontend esté conectado.

---

## 12. Reglas de Oro (no negociables)

1. **Toda entidad hereda AuditableEntity** — sin excepciones.
2. **Toda configuración hereda AuditableEntityConfiguration<T>** — sin excepciones.
3. **GET siempre devuelve todos los registros** — activos e inactivos. El frontend filtra.
4. **Los cambios arquitectónicos se consultan primero** — Miguel decide, la IA propone.
5. **Sin deuda técnica por rapidez** — este es un producto real para producción.
6. **Sin Repository genérico sin justificación** — services específicos es suficiente.
7. **BCrypt en Infrastructure** — Application no depende de librerías de hashing.
8. **ApiResponse en TODA respuesta** — sin excepciones.

---

## 13. Cómo Continuar el Proyecto

### Checklist para nueva sesión
1. Leer este archivo
2. Leer `CLAUDE.md` (reglas del proyecto para IAs)
3. Revisar `History Changed/` para ver iteraciones pasadas
4. Verificar estado de build: `dotnet build`
5. Consultar con Miguel antes de proponer cambios arquitectónicos

### Siguiente acción recomendada
Conectar `[Authorize]` en ProductosController y ClientesController una vez que el frontend esté usando la API de auth real. Esto activa la protección de rutas en el backend.

---

**Versión:** v3.0.0  
**Compilación:** ✅ 0 errores  
**Última actualización:** 2026-04-25
