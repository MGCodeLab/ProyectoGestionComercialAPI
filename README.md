# Nexus-ERP v3.0.0 — Backend

**Sistema de Gestión Comercial Empresarial**  
Arquitectura enterprise en .NET | SQL Server | Clean Architecture | CQRS pragmático | JWT Auth HS256

**Versión:** v3.0.0 (en desarrollo)  
**Última actualización:** 2026-04-29  
**Estado:** Módulos base completados. Autenticación implementada. Listo para módulos de negocio.

---

## 🏗️ Arquitectura

### Clean Architecture + CQRS Pragmático

```
Domain/                 → Entidades puras, sin dependencias externas
  ├── {Contexto}/
  └── AuditableEntity.cs

Application/            → Lógica de aplicación, DTOs, Validators
  ├── Dtos/{Contexto}/
  ├── Features/{Contexto}/
  ├── Interfaces/
  ├── Mappings/{Contexto}/
  └── Exceptions/

Infrastructure/         → EF Core, Servicios, Configurations
  ├── Repository/
  ├── Persistence/
  │   ├── AppDbContext.cs
  │   └── Configurations/
  ├── Services/
  └── Authentication/

GestionComercial/       → API REST (Controllers, Middleware, Bootstrap)
  ├── Controllers/
  ├── Middleware/
  ├── Extensions/
  ├── Program.cs
  └── appsettings.json

Database/               → Scripts SQL versionados (sin EF Migrations)
  ├── 01_Schemas/
  ├── 02_Tablas/
  └── 03_Seeds/

IA_Docs/                → Documentación técnica (para IAs, desarrolladores, auditoría)
History Changed/        → Historial de iteraciones arquitectónicas
USUARIO_DOCS/           → Resumen ejecutivo de avances por sesión
```

### Reglas de Dependencias (Inversión de Control)
```
GestionComercial (API)  →  Application  ←  Infrastructure
                            ↓
                        Domain (núcleo)
```

**Principios:**
- `Application` NUNCA referencia `Infrastructure` ni `GestionComercial`
- `Application` solo conoce interfaces (`IClienteService`, `IJwtService`, etc.)
- `Infrastructure` implementa interfaces y depende de `Domain`
- `Domain` es aislado, sin dependencias externas
- `GestionComercial` orquesta, no contiene lógica de negocio

---

## 🛠️ Stack Tecnológico

| Tecnología | Versión | Uso |
|------------|---------|-----|
| .NET | 8+ | Runtime asíncrono |
| C# | 12+ | Lenguaje (records, top-level statements) |
| Entity Framework Core | 8+ | ORM (data mapping, migrations versionadas) |
| SQL Server | 2019+ | RDBMS con índices filtered |
| MediatR | 12+ | CQRS (Commands vía mediador) |
| FluentValidation | 11+ | Validación fluida de DTOs |
| AutoMapper | 13+ | Mapeo bidireccional DTO ↔ Entity |
| BCrypt.Net-Next | 4.0+ | Hash seguro de passwords (cost factor 11-12) |
| JWT Bearer (HS256) | built-in | Autenticación stateless |
| Microsoft.AspNetCore.Authentication | built-in | Middleware de auth |

---

## 📦 Módulos Implementados

### ✅ Base (v3.0.0)
| Módulo | Endpoints | Observaciones |
|--------|-----------|---------------|
| **Productos** | GET, GET/{id}, POST, PUT, DELETE | CRUD completo con soft delete |
| **Clientes** | GET, GET/{id}, POST, PUT, PATCH activar/inactivar, DELETE | CRUD + gestión de estado. Campos: Documento, Nombres, Apellidos, Email (único si no NULL), Teléfono, Dirección |
| **Auth** | POST /login, GET /me | JWT HS256 con BCrypt. Usuarios: admin, vendor, readonly |

### 🔜 Próximos (v3.1+)
| Módulo | Prioridad | Descripción |
|--------|-----------|-------------|
| **Ventas** | Alta | Órdenes, facturas, clientes |
| **Compras** | Alta | Órdenes a proveedores, recepción |
| **Inventario** | Media | Stock, movimientos, auditoría |
| **Reportes** | Media | Consultas analíticas |

---

## 🚀 Cómo Iniciar el Proyecto

---

## Cómo Iniciar el Proyecto

### Prerrequisitos
- **.NET 8 SDK** o superior (verificar con `dotnet --version`)
- **SQL Server 2019+** (local o Azure)
- **Visual Studio 2022+** / **VS Code** con C# DevKit
- **Git** para control de versiones

### 1️⃣ Clonar el repositorio
```bash
git clone <repo-url>
cd "Proyecto Gestion Comercial/Backend"
```

### 2️⃣ Configurar la base de datos
Editar `GestionComercial/appsettings.json` con tu conexión SQL Server:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=NexusERP;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "tu-clave-secreta-minimo-32-caracteres-para-hs256",
    "Issuer": "nexus-erp-backend",
    "Audience": "nexus-erp-frontend",
    "ExpirationMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### 3️⃣ Crear esquema y tablas
Ejecutar scripts SQL en orden desde `Database/`:
```powershell
# En SSMS o Azure Data Studio:

-- Esquemas
:r "Database\01_Schemas\01_Schemas.sql"

-- Tablas
:r "Database\02_Tablas\01_Productos.sql"
:r "Database\02_Tablas\02_TipoDocumento.sql"
:r "Database\02_Tablas\03_Clientes.sql"
:r "Database\02_Tablas\04_Auth_Tablas.sql"

-- Data inicial + fixes
:r "Database\03_Seeds\01_InitProductos.sql"
:r "Database\03_Seeds\02_InitTipoDocumento.sql"
:r "Database\03_Seeds\04_Auth_Seed.sql"
:r "Database\03_Seeds\FIX_AddNombreCompletoColumn.sql"
:r "Database\03_Seeds\FIX_UpdateCorreoConstraint.sql"
```

### 4️⃣ Compilar y ejecutar
```bash
cd GestionComercial

# Restaurar paquetes
dotnet restore

# Compilar
dotnet build

# Ejecutar
dotnet run
# API disponible en http://localhost:5198
```

### 5️⃣ Verificar que funciona
```bash
# Login (obtener JWT)
curl -X POST http://localhost:5198/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@nexus.com","password":"123456"}'

# Listar clientes (sin autenticación, retorna todos)
curl http://localhost:5198/api/v1/clientes
```

### Usuarios de prueba incluidos
| Email | Password | Rol | Permisos |
|-------|----------|-----|----------|
| `admin@nexus.com` | `123456` | ADMIN | Acceso completo |
| `vendedor@nexus.com` | `123456` | VENDOR | Lectura/Escritura comercial |
| `readonly@nexus.com` | `123456` | READ_ONLY | Solo lectura |

---

## 📋 Convenciones de Desarrollo

### Commits (Conventional Commits)
Formato: `<tipo>(<contexto>): <descripción>`

```bash
feat(clientes): agregar validación de email
fix(auth): corregir expiración de JWT
refactor(productos): simplificar mapper
docs(readme): actualizar instrucciones
chore(deps): actualizar Entity Framework a v8.0.4
```

**Tipos permitidos:**
- `feat` — Nueva funcionalidad
- `fix` — Corrección de bug
- `refactor` — Mejora sin cambio de comportamiento
- `docs` — Cambios en documentación
- `chore` — Cambios en dependencias, build, infraestructura
- `test` — Adición de tests
- `perf` — Mejoras de rendimiento

### Branches (Git Flow)
```
main              ← Producción (protegida, requiere PR)
develop           ← Integración (protegida)
feature/*         ← Nuevas funcionalidades (e.g., feature/modulo-ventas)
hotfix/*          ← Correcciones urgentes en producción
```

**Flujo de trabajo:**
1. Crear rama desde `develop`: `git checkout -b feature/nombre`
2. Hacer commits semánticos
3. Crear PR hacia `develop` (descripción detallada)
4. Merge con merge commit (no squash)
5. Una vez estable en `develop`, mergear a `main`

### Endpoints REST (RESTful)
```
GET    /api/v1/{recurso}              ← Listar todos (sin filtro activo)
GET    /api/v1/{recurso}/{id}         ← Obtener por ID (específico)
POST   /api/v1/{recurso}              ← Crear nuevo
PUT    /api/v1/{recurso}/{id}         ← Actualizar completamente
PATCH  /api/v1/{recurso}/{id}/inactivar   ← Soft delete (Activo = 0)
PATCH  /api/v1/{recurso}/{id}/activar     ← Reactivar (Activo = 1)
DELETE /api/v1/{recurso}/{id}         ← Hard delete (eliminar físicamente)
```

**Notas:**
- Los `GET` retornan **TODOS** los registros (activos e inactivos)
- El soft delete es auditoría, no ocultación de datos
- Las respuestas usan siempre `ApiResponse<T>` wrapper
- Los códigos HTTP son estrictos (200, 201, 400, 401, 404, 500, etc.)

---

## ⚠️ Reglas de Oro (Non-Negotiable)

### ✅ Lo que SIEMPRE se debe hacer

1. **Toda entidad en Domain hereda `AuditableEntity`**
   ```csharp
   public class Cliente : AuditableEntity { }
   ```

2. **Toda configuración EF hereda `AuditableEntityConfiguration<T>`**
   ```csharp
   public class ClienteConfiguration : AuditableEntityConfiguration<Cliente> { }
   ```

3. **Toda respuesta HTTP usa `ApiResponse<T>` wrapper**
   ```csharp
   return this.OkResponse(data, "Mensaje");
   return this.CreatedResponse(nameof(GetById), routeValues, data, "Creado");
   ```

4. **Los Commands se envían vía MediatR**
   - Crear: `CrearClienteCommand` → `CrearClienteHandler`
   - Actualizar: `ActualizarClienteCommand` → `ActualizarClienteHandler`
   - Eliminar: `EliminarClienteCommand` → `EliminarClienteHandler`

5. **Las Queries usan Services directos** (sin MediatR)
   - `IClienteService.ObtenerTodos()`
   - `IClienteService.ObtenerPorId(id)`

6. **Los mapeos usan AutoMapper bidireccional**
   ```csharp
   CreateMap<Cliente, ClienteDto>().ReverseMap();
   CreateMap<CrearClienteDto, Cliente>();
   CreateMap<ActualizarClienteCommand, Cliente>();
   ```

7. **Los mensajes de validación van en español**
   - ✅ "El nombre es requerido"
   - ❌ "Name is required"

8. **Los DTOs llevan validación con atributos**
   ```csharp
   [Required(ErrorMessage = "El email es requerido")]
   [EmailAddress(ErrorMessage = "Email inválido")]
   public string Email { get; set; }
   ```

### ❌ Lo que NUNCA se debe hacer

1. **`HasQueryFilter(x => x.Activo)`** — Rompe soft delete
   - Los GET deben retornar todos (sin filtrar por Activo)

2. **Lógica de negocio en Controllers**
   - Controllers orquestan, no calculan ni transforman

3. **Acceder a `AppDbContext` desde Application layer**
   - Application solo conoce interfaces
   - Infrastructure implementa

4. **Agregar paquetes externos en Application**
   - BCrypt, JWT, EntityFramework, etc. van en Infrastructure
   - Application es agnóstica a implementaciones

5. **Cambios arquitectónicos sin aprobación**
   - Patrones, capas, comunicación → consultar primero

6. **Dejar código no utilizado o commented-out**
   - Git es el historial, no el código
   - Si no se usa, se elimina

7. **Crear índices o constraints sin documentar**
   - Toda decisión SQL debe estar registrada
   - Incluir `-- Razón: ...` en scripts

---

## 📡 Formato Estándar de Respuestas API

### Respuesta exitosa (200 OK)
```json
{
  "success": true,
  "message": "Cliente creado exitosamente",
  "data": {
    "id": 1,
    "publicId": "a1b2c3d4-e5f6-47g8-h9i0-j1k2l3m4n5o6",
    "nombres": "Juan",
    "apellidoPaterno": "Pérez",
    "apellidoMaterno": "García",
    "correo": "juan@example.com",
    "activo": true,
    "fechaRegistro": "2026-04-30T10:30:00Z"
  },
  "errors": null,
  "traceId": "0HNL30ONNEOHU:00000001"
}
```

### Respuesta con error (400/401/404/500)
```json
{
  "success": false,
  "message": "Validación fallida",
  "data": null,
  "errors": [
    "El email es requerido",
    "El email debe ser válido"
  ],
  "traceId": "0HNL30ONNEOHU:00000002"
}
```

### Códigos HTTP utilizados
| Código | Significado | Ejemplo |
|--------|-------------|---------|
| 200 | OK | GET exitoso, PUT exitoso |
| 201 | Created | POST de creación exitoso |
| 204 | No Content | DELETE exitoso |
| 400 | Bad Request | Validación fallida, datos inválidos |
| 401 | Unauthorized | Falta JWT o JWT inválido |
| 403 | Forbidden | JWT válido pero sin permisos |
| 404 | Not Found | Recurso no encontrado |
| 500 | Internal Server Error | Error del servidor |

---

## 📚 Documentación Complementaria

Para desarrolladores y futuras sesiones de IA:

| Documento | Ubicación | Propósito |
|-----------|-----------|----------|
| **Status Actual** | `IA_Docs/PROJECT_STATUS.md` | Estado actual de features, bugs, decisiones |
| **Patrones Obligatorios** | `IA_Docs/IMPLEMENTATION_PATTERNS.md` | Estándar exacto para todos los módulos |
| **Decisiones Arquitectónicas** | `IA_Docs/ARCHITECTURE_DECISIONS.md` | ADRs, razones, trade-offs |
| **Problemas Resueltos** | `IA_Docs/COMMON_ISSUES_AND_FIXES.md` | Bugs encontrados, soluciones, evitar repetir |
| **Knowledge Base IA** | `IA_Docs/PROJECT_KNOWLEDGE_BASE.md` | Context para LLMs, estructura técnica |
| **Base de Datos** | `IA_Docs/DATABASE_ARCHITECTURE.md` | Schema, constraints, índices, decisiones SQL |
| **Roadmap** | `IA_Docs/FUTURE_ROADMAP.md` | Próximas features, módulos pendientes |
| **Reglas para IAs** | `CLAUDE.md` | Instrucciones específicas de trabajo |
| **Historial de Cambios** | `History Changed/` | Registro de iteraciones arquitectónicas |
| **Avances por Sesión** | `USUARIO_DOCS/` | Resumen ejecutivo de cada sesión |

---

## 🔐 Seguridad

### Autenticación
- **Esquema:** JWT Bearer (HS256)
- **Duración:** 60 minutos (configurable)
- **Password Hash:** BCrypt (cost factor 11-12)
- **Algoritmo:** HMAC-SHA256

### Validación
- Todas las DTOs llevan atributos `[Required]`, `[StringLength]`, `[EmailAddress]`, etc.
- Validación fluida con FluentValidation
- Mensajes de error en español

### Base de Datos
- Constraints UNIQUE con índices filtered para NULL
- Foreign Keys con cascada configurable
- Índices en campos frecuentemente buscados
- Auditoría completa: Id, PublicId, FechaRegistro, FechaActualizacion

---

## 👨‍💻 Desarrollo Local

### Pre-requisitos para contribuir
- Entender Clean Architecture
- Familiaridad con CQRS (Commands vs Queries)
- Conocimiento de Entity Framework Core
- SQL Server / T-SQL básico
- Git y convenciones de commits

### Ambiente recomendado
- **IDE:** Visual Studio 2022 Community+ o JetBrains Rider
- **Terminal:** PowerShell / Bash
- **DB Tool:** SQL Server Management Studio o Azure Data Studio
- **Version Control:** GitKraken o Git CLI

### Testing
```bash
cd GestionComercial
dotnet test
```

---

## 📞 Autor & Contacto

**Miguel González Cuevas** (MGCodeLab)  
Full Stack Developer · .NET Enterprise · Angular · Arquitectura de Software

**Email:** gonzalezcuevasmiguelignacio@gmail.com

---

**Nexus-ERP v3.0.0** © 2026 — Producto empresarial. Diseñado para producción real.
