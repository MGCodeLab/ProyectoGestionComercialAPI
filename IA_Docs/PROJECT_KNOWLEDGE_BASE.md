# Nexus-ERP: Knowledge Base para IAs Futuras

**Última actualización:** 2026-04-25  
**Proyecto:** Nexus-ERP v3.0.0  
**Estado:** En desarrollo activo - Fase de módulos (Cliente ✅, Producto ✅)  
**Usuario:** Miguel González Cuevas

---

## 📚 Resumen Ejecutivo

**Nexus-ERP** es un sistema de gestión comercial empresarial real (no demo) diseñado para:
- Gestión integral de clientes
- Catálogo de productos
- Módulos futuros: Ventas, Compras, Inventario

**Stack Tecnológico:**
- Backend: .NET 10, C# 13, Clean Architecture
- Database: SQL Server 2019+
- Frontend: Angular 19+
- Arquitectura: Clean Architecture + CQRS Pragmático
- ORM: Entity Framework Core 10
- Validation: FluentValidation
- Mapping: AutoMapper

**Ambiente de Desarrollo:**
- Windows 11 Home
- Visual Studio (no VS Code)
- SQL Server Management Studio
- Puerto API: http://localhost:5198

---

## 🏗️ Arquitectura Implementada

### Principios Fundamentales

1. **Arquitectura Limpia (Clean Architecture)**
   - Domain (entidades, lógica de negocio)
   - Application (casos de uso, DTOs, servicios)
   - Infrastructure (persistencia, repositorios)
   - API (controladores, middleware)

2. **CQRS Pragmático (NO purista)**
   - **Commands:** Operaciones que cambian estado → MediatR
   - **Queries:** Operaciones de lectura → Services directos
   - **Razón:** Evitar over-engineering innecesario

3. **Auditoría Obligatoria (AuditableEntity)**
   - `Id` (int): Identificador interno
   - `PublicId` (GUID): Identificador externo para API
   - `Activo` (bit): Flag para soft delete
   - `FechaRegistro` (datetime2): Creación automática
   - `FechaActualizacion` (datetime2): Última modificación manual

### Patrones Clave

#### Template Method Pattern (Configuraciones)
```csharp
// Base class que todos heredan
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
```

#### Soft Delete (Auditoría, NO ocultamiento)
```csharp
// ❌ INCORRECTO: Global filter que oculta inactivos
// query.Where(x => x.Activo == true)

// ✅ CORRECTO: Retorna TODOS, frontend controla visualización
// GET /api/v1/clientes → [activos e inactivos]
// PATCH /api/v1/clientes/{id}/inactivar → Activo=false pero VISIBLE
```

#### Response Wrapper Pattern
```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public string TraceId { get; set; }
}

// Uso: return this.OkResponse(data, "mensaje");
```

#### Global Exception Middleware
```csharp
public class ExceptionMiddleware
{
    // Captura TODAS las excepciones
    // Retorna ApiResponse<T> consistente
    // Evita stacktraces en cliente
}
```

---

## ✅ Módulos Implementados

### 1. **Producto** (Completado)
- **Estado:** ✅ CRUD funcional
- **Campos:** Id, Nombre, Descripcion, Precio, Activo
- **Endpoints:** 
  - GET /api/v1/productos
  - GET /api/v1/productos/{id}
  - POST /api/v1/productos
  - PUT /api/v1/productos/{id}
  - DELETE /api/v1/productos/{id}
- **Validaciones:** Nombre required, MaxLength(150), Precio > 0
- **Auditoría:** PublicId, Activo, FechaRegistro, FechaActualizacion

### 2. **Cliente** (Completado - v3.0.0)
- **Estado:** ✅ CRUD completo con soft delete
- **Campos:** TipoDocumentoId (FK), NumeroDocumento, Nombres, ApellidoPaterno, ApellidoMaterno, Correo, Telefono, Direccion
- **Endpoints:**
  - GET /api/v1/clientes (retorna TODOS: activos + inactivos)
  - GET /api/v1/clientes/{id}
  - POST /api/v1/clientes
  - PUT /api/v1/clientes/{id}
  - PATCH /api/v1/clientes/{id}/inactivar (soft delete)
  - PATCH /api/v1/clientes/{id}/activar (reactivar)
  - DELETE /api/v1/clientes/{id} (hard delete)
- **Validaciones:** 
  - TipoDocumentoId: Required, >0
  - NumeroDocumento: Required, MaxLength(20), Unique per TipoDocumento
  - Nombres: Required, MaxLength(100)
  - ApellidoPaterno: Required, MaxLength(100)
  - Correo: Optional, EmailAddress, Unique, MaxLength(150)
  - Teléfono: Optional, MaxLength(20)
  - Dirección: Optional, MaxLength(250)
- **FK Constraint:** TipoDocumento obligatorio
- **Auditoría:** PublicId, Activo, FechaRegistro, FechaActualizacion

### 3. **TipoDocumento** (Soporte - Catálogo)
- **Estado:** ✅ Implementado como soporte para Clientes
- **Campos:** Codigo, Descripcion, Activo
- **Ejemplos:** DNI, RUC, PASSPORT
- **Uso:** FK en tabla Clientes

---

## 🔄 Patrón de Implementación Replicable

Cuando se agregue un nuevo módulo (ej: Ventas, Compras, Inventario), seguir exactamente este patrón:

### Estructura de Carpetas
```
Domain/
  ├── {Modulo}/
  │   └── {Entity}.cs           (hereda AuditableEntity)

Application/
  ├── Dtos/{Modulo}/
  │   ├── Crear{Entity}Dto.cs
  │   ├── Actualizar{Entity}Dto.cs
  │   └── {Entity}Dto.cs (response with audit fields)
  ├── Features/{Modulo}/
  │   ├── Crear/
  │   │   ├── Crear{Entity}Command.cs
  │   │   ├── Crear{Entity}Handler.cs
  │   │   └── Crear{Entity}Validator.cs
  │   ├── Actualizar/
  │   │   ├── Actualizar{Entity}Command.cs
  │   │   ├── Actualizar{Entity}Handler.cs
  │   │   └── Actualizar{Entity}Validator.cs
  │   ├── ActualizarEstado/
  │   │   ├── ActualizarEstado{Entity}Command.cs
  │   │   └── ActualizarEstado{Entity}Handler.cs
  │   └── Eliminar/
  │       ├── Eliminar{Entity}Command.cs
  │       └── Eliminar{Entity}Handler.cs
  ├── Interfaces/
  │   └── I{Entity}Service.cs
  └── Mappings/{Modulo}/
      └── {Entity}Profile.cs

Infrastructure/
  ├── Repository/
  │   └── {Entity}Service.cs    (implementa I{Entity}Service)
  └── Persistence/Configurations/
      └── {Entity}Configuration.cs (hereda AuditableEntityConfiguration<T>)

API/
  └── Controllers/
      └── {Entity}sController.cs (6 endpoints CRUD + soft delete)
```

### Checklist de Implementación

- [ ] Entity en Domain hereda `AuditableEntity`
- [ ] Configuration hereda `AuditableEntityConfiguration<T>`
- [ ] DTOs (Crear, Actualizar, Response)
- [ ] Commands y Handlers con logging
- [ ] Validators con reglas de negocio
- [ ] AutoMapper Profile
- [ ] Service Interface en Application/Interfaces
- [ ] Service Implementation en Infrastructure/Repository
- [ ] Controller con 6 endpoints (GET list, GET id, POST, PUT, PATCH inactivar, DELETE)
- [ ] DI registration en Program.cs: `builder.Services.AddScoped<IService, Service>();`
- [ ] Test data en database
- [ ] Test endpoints

---

## 📊 Decisiones Arquitectónicas y Razones

### ❌ NO hacer global soft delete filter
**Decidido:** Soft delete retorna TODOS (activos + inactivos)  
**Razón:** Miguel necesita visibilidad de registros inactivos para auditoría y manejo en frontend  
**Implementación:** Activo field + frontend visual control

### ❌ NO usar Repository Pattern genérico obligatorio
**Decidido:** Services específicos por entidad (ClienteService, ProductoService)  
**Razón:** Evitar abstracción prematura; UnitOfWork se evaluará cuando Ventas requiera multi-entity transactions  
**Patrón:** Pragmático sobre arquitectónico

### ✅ USAR MediatR para Commands
**Decidido:** Commands (POST, PUT, DELETE) vía MediatR  
**Razón:** Logging centralizado, validación consistente, fácil de testear  
**Beneficio:** Separación clara entre lectura (Services) y escritura (Commands)

### ✅ USAR Services para Queries
**Decidido:** Queries (GET) vía Services directos  
**Razón:** Evitar overhead de MediatR para operaciones simples de lectura  
**Beneficio:** Performance, sintaxis más limpia

### ✅ USAR FluentValidation en Validators
**Decidido:** Validación en Handlers vía FluentValidation  
**Razón:** Reutilizable, expresiva, integrada con MediatR  
**Beneficio:** Validaciones consistentes, mensajes de error en español

### ✅ USAR AutoMapper modular (Profiles)
**Decidido:** Un Profile por módulo/entidad  
**Razón:** Encapsulación, fácil de mantener, auto-discovery via AddMaps()  
**Beneficio:** Configuración centralizada sin duplicación

---

## 🔧 Problemas Encontrados y Soluciones

### Problema 1: Columnas de auditoría faltantes en BD
**Síntoma:** Error "Invalid column name 'FechaActualizacion'" al ejecutar queries  
**Causa:** Código v3.0.0 espera columnas que BD no tenía  
**Solución:** Script v3.0.0_COMPLETE_SETUP.sql que agrega columnas con DEFAULT values  
**Ubicación:** `History Changed/20260425_T1800_DatabaseSetupV3/v3.0.0_COMPLETE_SETUP.sql`

### Problema 2: NEWSEQUENTIALID() en INSERT
**Síntoma:** Error "NEWSEQUENTIALID() can only be used in DEFAULT expression"  
**Causa:** Intentar generar GUID en INSERT VALUES (no permitido)  
**Solución:** Omitir PublicId y FechaRegistro en INSERT, usar DEFAULT de tabla  
**Lección:** SQL Server DEFAULT values se aplican automáticamente

### Problema 3: Puerto 5198 ya en uso
**Síntoma:** "Address already in use" al iniciar aplicación  
**Causa:** Instancia anterior todavía en ejecución  
**Solución:** PowerShell `Stop-Process` o cambiar puerto en launchSettings.json  
**Comando:** `netstat -ano | grep 5198`

### Problema 4: Soft delete ocultando registros
**Síntoma:** GET /api/v1/clientes no mostraba inactivos  
**Causa:** Global filter en AppDbContext que hacía WHERE Activo=1  
**Solución:** Revertir filter, retornar TODOS, frontend maneja presentación  
**Commit:** `775a0e2` (revert global soft delete filter)

---

## 📈 Métricas del Proyecto

### Código Implementado
- **Entidades:** 3 (Producto, Cliente, TipoDocumento)
- **DTOs:** 6 (2 por módulo principal)
- **Commands:** 6 (CrearCliente, ActualizarCliente, ActualizarEstadoCliente, EliminarCliente, etc.)
- **Handlers:** 6
- **Validators:** 3 (uno por operación con lógica compleja)
- **Controllers:** 2 (ProductosController, ClientesController)
- **Endpoints:** 13 (GET list, GET id, POST, PUT, PATCH x2, DELETE para Cliente+Producto)
- **Líneas de código arquitectónico:** ~500-600

### Arquitectura
- Clean Architecture: ✅ 4 capas (Domain, Application, Infrastructure, API)
- CQRS Pragmático: ✅ Commands vía MediatR, Queries vía Services
- DI Container: ✅ Configurado completamente
- Global Middleware: ✅ Exception handling centralizado
- Auditoría: ✅ AuditableEntity en todas las entidades
- Response Wrapper: ✅ ApiResponse<T> consistente

### Testing Ready
- Validaciones en place: ✅
- Logging en handlers: ✅
- Exception handling: ✅
- Test data seeded: ✅
- Manual testing viable: ✅

---

## 🚀 Próximas Fases (Roadmap v3.0.0+)

### Fase 1: Módulo Ventas (POST v3.0.0)
**Requerimientos esperados:**
- Encabezado de venta (Cliente FK, fecha, total)
- Detalle de venta (múltiples productos)
- **Patrón:** Mismo que Cliente pero con:
  - Multi-entity FK (Cliente, Producto)
  - Posible necesidad de **UnitOfWork** para transacciones multi-tabla
  - Posible necesidad de **totales calculados**

### Fase 2: Módulo Compras
**Patrón:** Similar a Ventas pero con Proveedores

### Fase 3: Módulo Inventario
**Patrón:** Producto + MovimientosInventario

### Evaluación UnitOfWork
- Cuando: Fase Ventas
- Criterio: Si hay transacciones multi-entidad reales
- Implementación: Explícita, no genérica

---

## 📝 Reglas de Oro (SIEMPRE cumplir)

### 1. Auditoría Obligatoria
- ✅ TODA entidad hereda `AuditableEntity`
- ✅ TODAS las configuraciones heredan `AuditableEntityConfiguration<T>`
- ✅ NUNCA omitir PublicId, Activo, FechaRegistro, FechaActualizacion

### 2. Soft Delete = Auditoría, NO Ocultamiento
- ✅ GET siempre retorna TODOS (activos + inactivos)
- ✅ PATCH /{id}/inactivar pone Activo=false pero visible
- ✅ DELETE /{id} hace hard delete (Remove)
- ✅ Frontend controla presentación visual

### 3. Validación en Validators
- ✅ FluentValidation en Handlers
- ✅ Reglas de negocio en Validators
- ✅ Mensajes en español

### 4. Logging en Handlers
- ✅ Log operación ANTES de ejecutar
- ✅ Log resultado
- ✅ Log errores

### 5. Commit Convention
```
feat(modulo): descripcion
fix(modulo): descripcion
refactor(modulo): descripcion
chore(infra): descripcion
docs: descripcion
```

### 6. NUNCA hacer
- ❌ Soft delete global filter
- ❌ Repository pattern genérico sin necesidad
- ❌ DTOs sin campos de auditoría en Response
- ❌ DI sin registración
- ❌ Cambios arquitectónicos sin consultar Miguel primero
- ❌ Deuda técnica por rapidez
- ❌ Soluciones temporales en código base

---

## 🎯 Instrucciones para IAs Futuras

### Cuando se continúe el proyecto:

1. **Primero:** Lee este documento (PROJECT_KNOWLEDGE_BASE.md)
2. **Luego:** Lee CLAUDE.md (reglas del proyecto)
3. **Luego:** Revisa History Changed/ (decisiones pasadas)
4. **Luego:** Abre el código (lea Domain, Application, Infrastructure)
5. **Nunca:** Asumas que algo puede ser diferente sin consultar

### Context Window Strategy
- Este documento SIEMPRE debe estar en contexto
- Cuando la ventana cierre, save importante info en memory
- Usa memory para recordar decisiones y lecciones

### Git Workflow
- Siempre haz commits con message descriptivo
- Usa History Changed/ para cambios arquitectónicos
- IA_Docs/ es para documentación reutilizable

---

## 📞 Contacto / Clarificaciones

**Usuario:** Miguel González Cuevas  
**Email:** gonzalezcuevasmiguelignacio@gmail.com  
**Rol:** Senior Software Engineer + Arquitecto del Proyecto  

**Cuando dudes:**
- Lee CLAUDE.md (contiene instrucciones arquitectónicas)
- Lee Project Memory (si existe)
- Consulta primero, implementa después
- Nexus-ERP es producción real, no demo

---

## 📚 Referencias Rápidas

**Archivo crítico de configuración:**
- `GestionComercial/Program.cs` - DI, CORS, Database, AutoMapper

**Configuraciones base:**
- `Infrastructure/Persistence/Configurations/AuditableEntityConfiguration.cs` - Template

**Ejemplos de implementación:**
- `Application/Features/Clientes/` - Referencia de CRUD completo
- `Domain/Comercial/Cliente.cs` - Entity base
- `Application/Interfaces/IClienteService.cs` - Service interface

**Database:**
- `Database/v3.0.0_COMPLETE_SETUP.sql` - Script de setup inicial
- `IA_Docs/DATABASE_SETUP_INSTRUCTIONS.md` - Instrucciones

---

**Última actualización:** 2026-04-25 18:00  
**Versión del Proyecto:** v3.0.0 (en desarrollo)  
**Compilación:** ✅ Success (0 errors, 0 warnings)  
**Estado de Módulos:** Cliente ✅, Producto ✅, Pendientes: Ventas, Compras, Inventario
