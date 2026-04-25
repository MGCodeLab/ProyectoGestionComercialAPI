# Nexus-ERP — Backend

Sistema de gestión comercial empresarial. Diseñado para producción real.

**Stack:** .NET 10 · SQL Server · Clean Architecture · CQRS · JWT Auth  
**Versión:** v3.0.0  
**Estado:** En desarrollo activo

---

## Arquitectura

```
Domain/              Entidades puras. Sin dependencias externas.
Application/         Casos de uso, Commands, DTOs, Validators, Interfaces.
Infrastructure/      EF Core, Services (JWT, Auth), Configurations.
GestionComercial/    API: Controllers, Middleware, Program.cs.
Database/            Scripts SQL versionados (sin EF Migrations).
IA_Docs/             Documentación técnica para IAs y desarrolladores.
History Changed/     Registro de iteraciones arquitectónicas.
```

### Regla de dependencias
```
API  →  Application  ←  Infrastructure
              ↓
           Domain
```
Application nunca referencia Infrastructure. Solo conoce interfaces.

---

## Stack Tecnológico

| Tecnología | Uso |
|------------|-----|
| .NET 10 + C# 13 | Runtime |
| Entity Framework Core 10 | ORM |
| SQL Server 2019+ | Base de datos |
| MediatR | CQRS (Commands) |
| FluentValidation | Validación de inputs |
| AutoMapper | Mapeo DTO ↔ Entity |
| BCrypt.Net-Next | Hash de passwords |
| JWT Bearer (HS256) | Autenticación |

---

## Módulos Disponibles

| Módulo | Endpoints | Estado |
|--------|-----------|--------|
| Productos | GET, GET/{id}, POST, PUT, DELETE | ✅ |
| Clientes | GET, GET/{id}, POST, PUT, PATCH inactivar/activar, DELETE | ✅ |
| Auth | POST /login, POST /logout, GET /me | ✅ |
| Ventas | — | 🔜 |
| Compras | — | 🔜 |
| Inventario | — | 🔜 |

---

## Cómo Iniciar el Proyecto

### 1. Prerrequisitos
- .NET 10 SDK
- SQL Server 2019+ (local o remoto)
- Visual Studio 2022+ o VS Code

### 2. Configurar conexión a BD
Editar `GestionComercial/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=NexusERP;Trusted_Connection=True;"
  },
  "Jwt": {
    "Key": "tu-clave-secreta-minimo-32-caracteres",
    "Issuer": "nexus-erp-backend",
    "Audience": "nexus-erp-frontend",
    "ExpirationMinutes": 60
  }
}
```

### 3. Crear la base de datos
Ejecutar los scripts en orden desde `Database/`:
```
01_Schemas/01_Schemas.sql
02_Tablas/01_Productos.sql
02_Tablas/02_TipoDocumento.sql
02_Tablas/03_Clientes.sql
02_Tablas/04_Auth_Tablas.sql
03_Seeds/01_InitProductos.sql
03_Seeds/02_InitTipoDocumento.sql
03_Seeds/04_Auth_Seed.sql
```

### 4. Ejecutar la API
```bash
cd GestionComercial
dotnet run
# API disponible en http://localhost:5198
```

### 5. Usuarios de prueba (ya incluidos en seed)
| Email | Password | Rol |
|-------|----------|-----|
| admin@nexus.com | 123456 | ADMIN |
| vendedor@nexus.com | 123456 | VENDOR |
| readonly@nexus.com | 123456 | READ_ONLY |

---

## Convenciones de Desarrollo

### Commits
```
feat(modulo): descripcion nueva funcionalidad
fix(modulo): descripcion corrección de bug
refactor(modulo): descripcion mejora sin cambio de comportamiento
chore(infra): descripcion cambio de infraestructura
docs: descripcion cambio de documentación
```

### Branches
```
main              ← producción
develop           ← integración
feature/*         ← nuevas funcionalidades
hotfix/*          ← correcciones urgentes
```

### Endpoints
```
GET    /api/v1/{recurso}           ← listar (todos, activos e inactivos)
GET    /api/v1/{recurso}/{id}      ← obtener por ID
POST   /api/v1/{recurso}           ← crear
PUT    /api/v1/{recurso}/{id}      ← actualizar completo
PATCH  /api/v1/{recurso}/{id}/inactivar  ← soft delete
PATCH  /api/v1/{recurso}/{id}/activar    ← reactivar
DELETE /api/v1/{recurso}/{id}      ← hard delete
```

---

## Reglas Importantes

### Lo que SIEMPRE se debe hacer
- Toda entidad hereda `AuditableEntity`
- Toda configuración EF hereda `AuditableEntityConfiguration<T>`
- Toda respuesta usa `ApiResponse<T>` (via `ControllerExtensions`)
- Los `GET` devuelven **todos** los registros (activos + inactivos)
- Los Commands se envían vía MediatR
- Las Queries usan Services directos
- Los mensajes de validación van en español

### Lo que NUNCA se debe hacer
- `HasQueryFilter(x => x.Activo)` — rompe el principio de soft delete del proyecto
- Lógica de negocio en Controllers
- Acceder a `AppDbContext` desde Application layer
- Agregar paquetes externos en Application (BCrypt, JWT, etc.)
- Cambios arquitectónicos sin consultar primero

---

## Respuesta Estándar de la API

```json
{
  "success": true,
  "message": "Operación exitosa",
  "data": {},
  "errors": null,
  "traceId": "0HNL30ONNEOHU:00000001"
}
```

En error:
```json
{
  "success": false,
  "message": "Credenciales inválidas",
  "data": null,
  "errors": ["Email o contraseña incorrectos"],
  "traceId": "0HNL30ONNEOHU:00000002"
}
```

---

## Documentación Adicional

| Documento | Ubicación |
|-----------|-----------|
| Knowledge Base para IAs | `IA_Docs/PROJECT_KNOWLEDGE_BASE.md` |
| Decisiones Arquitectónicas | `IA_Docs/ARCHITECTURE_DECISIONS.md` |
| Patrones de Implementación | `IA_Docs/IMPLEMENTATION_PATTERNS.md` |
| Setup de Base de Datos | `IA_Docs/DATABASE_SETUP_INSTRUCTIONS.md` |
| Reglas para IAs | `CLAUDE.md` |
| Historial de cambios | `History Changed/` |

---

## Autor

**Miguel González Cuevas** — MGCodeLab  
Full Stack Developer · .NET · Angular · Arquitectura de Software
