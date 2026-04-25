# Iteración: Módulo de Autenticación JWT — v3.0.0

**Fecha:** 2026-04-25 20:00  
**Tipo:** Feature / Security  
**Rama:** Modulos/Cliente_01  
**Dependencias:** Iteración 3 (Cliente CRUD) ✅

---

## 📋 Qué se implementó

### DOMAIN — `Domain/Seguridad/`
- ✅ `Usuario.cs` — Hereda AuditableEntity + Nombre, Email, PasswordHash, LastLogin
- ✅ `Rol.cs` — Hereda AuditableEntity + Nombre, Descripcion
- ✅ `Permiso.cs` — Hereda AuditableEntity + Recurso, Accion
- ✅ `UsuarioRol.cs` — Tabla pivot (UsuarioId, RolId)
- ✅ `RolPermiso.cs` — Tabla pivot (RolId, PermisoId)

### APPLICATION
- ✅ `Dtos/Auth/LoginRequestDto.cs`
- ✅ `Dtos/Auth/LoginResponseDto.cs`
- ✅ `Dtos/Auth/UserProfileDto.cs`
- ✅ `Dtos/Auth/PermissionDto.cs`
- ✅ `Exceptions/UnauthorizedException.cs` — HTTP 401
- ✅ `Features/Auth/Login/LoginCommand.cs`
- ✅ `Features/Auth/Login/LoginHandler.cs` — BCrypt.Verify en UsuarioService
- ✅ `Features/Auth/Login/LoginValidator.cs`
- ✅ `Interfaces/IJwtService.cs`
- ✅ `Interfaces/IUsuarioService.cs` — AutenticarUsuario, ObtenerPorId, ActualizarLastLogin
- ✅ `Mappings/Auth/AuthProfile.cs`

### INFRASTRUCTURE
- ✅ `Configurations/UsuarioConfiguration.cs` → tabla `seguridad.Usuarios`
- ✅ `Configurations/RolConfiguration.cs` → tabla `seguridad.Roles`
- ✅ `Configurations/PermisoConfiguration.cs` → tabla `seguridad.Permisos`
- ✅ `Configurations/UsuarioRolConfiguration.cs` → tabla `seguridad.UsuarioRoles`
- ✅ `Configurations/RolPermisoConfiguration.cs` → tabla `seguridad.RolPermisos`
- ✅ `Repository/UsuarioService.cs` — BCrypt.Verify + queries con Include
- ✅ `Services/JwtService.cs` — HS256, claims: sub, email, nombre, roles, iat, jti
- ✅ `Persistence/AppDbContext.cs` — +DbSet<Usuario>, DbSet<Rol>, DbSet<Permiso>
- ✅ `Infrastructure.csproj` — +BCrypt.Net-Next, +JwtBearer

### API
- ✅ `Controllers/AuthController.cs`
  - `POST /api/v1/auth/login` [AllowAnonymous]
  - `POST /api/v1/auth/logout` [AllowAnonymous]
  - `GET /api/v1/auth/me` [Authorize]
- ✅ `Middleware/ExceptionMiddleware.cs` — +catch UnauthorizedException → 401
- ✅ `Extensions/ControllerExtensions.cs` — +UnauthorizedResponse()
- ✅ `Program.cs` — +AddAuthentication(JwtBearer), +UseAuthentication()
- ✅ `appsettings.json` — +sección Jwt
- ✅ `API.GestionComercial.csproj` — +JwtBearer package

### DATABASE
- ✅ `01_Schemas/01_Schemas.sql` — +CREATE SCHEMA seguridad
- ✅ `02_Tablas/04_Auth_Tablas.sql` — 5 tablas del schema seguridad
- ✅ `03_Seeds/04_Auth_Seed.sql` — 3 usuarios de prueba + roles + permisos

---

## 🔐 Especificación JWT

- **Algoritmo:** HS256
- **Claims:** sub, email, nombre, roles (ClaimTypes.Role), iat, jti
- **Expiración:** 60 minutos (configurable en appsettings.json)
- **Issuer:** nexus-erp-backend
- **Audience:** nexus-erp-frontend

---

## 👤 Usuarios de Prueba

| Email | Password | Rol | Permisos |
|-------|----------|-----|----------|
| admin@nexus.com | 123456 | ADMIN | Todos los permisos |
| vendedor@nexus.com | 123456 | VENDOR | create/edit/view en productos, clientes, ventas |
| readonly@nexus.com | 123456 | READ_ONLY | Sin permisos |

---

## 🔄 Decisiones Técnicas

1. **BCrypt en Infrastructure** (no en Application): La verificación de password está en `UsuarioService` (Infrastructure) para no agregar dependencias externas a la capa Application.

2. **LoginHandler accede via IUsuarioService**: Patrón consistente con el resto del proyecto (no accede a AppDbContext directamente desde Application).

3. **Logout es dummy**: El frontend maneja limpieza de estado. El endpoint retorna 200 OK sin lógica de blacklist (fase futura si se requiere).

4. **/me lee desde BD**: En vez de decodificar el JWT en el handler, hace query a BD para obtener datos frescos con los permisos actuales.

5. **Email normalizado**: Siempre se hace `.Trim().ToLower()` antes de buscar en BD.

---

## 🧪 Endpoints

### POST /api/v1/auth/login
```bash
curl -X POST http://localhost:5198/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{ "email": "admin@nexus.com", "password": "123456" }'
```

### POST /api/v1/auth/logout
```bash
curl -X POST http://localhost:5198/api/v1/auth/logout
```

### GET /api/v1/auth/me
```bash
curl -X GET http://localhost:5198/api/v1/auth/me \
  -H "Authorization: Bearer <token>"
```

---

## 📊 Impacto

- **Archivos nuevos:** 22
- **Archivos modificados:** 7
- **Build:** ✅ 0 errores, 3 warnings preexistentes (DTOs Producto nullable)

---

## 🚀 Estado

**Build:** ✅ Exitoso  
**Scripts BD listos:** ✅  
**Frontend compatible:** ✅ (sigue contrato API exacto del frontend spec)  
**v3.0.0:** ✅ DESBLOQUEADA
