# Nexus-ERP: Decisiones Arquitectónicas (ADR)

**Última actualización:** 2026-04-25  
**Formato:** Architecture Decision Record (simplificado)

> Cada decisión incluye: qué se eligió, qué se descartó y por qué.  
> Consultar este archivo antes de proponer cambios a la arquitectura.

---

## ADR-001: CQRS Pragmático (no purista)

**Fecha:** 2026-04-25  
**Estado:** ✅ Activo

**Decisión:**
- Commands (crear, actualizar, eliminar, autenticar) → MediatR
- Queries (leer lista, leer por ID) → Services directos

**Descartado:**
- CQRS completo (queries también vía MediatR)
- CQRS sin MediatR

**Razón:**
Usar MediatR para todas las operaciones agrega overhead sin beneficio real en operaciones de solo lectura simples. El patrón pragmático da los beneficios de validación, logging y separación de responsabilidades donde importa (writes), sin sobrecomplicar las lecturas.

**Impacto:** Commands en `Application/Features/`, Queries como métodos en `Application/Interfaces/I{Entity}Service.cs`.

---

## ADR-002: Sin Repository Pattern Genérico

**Fecha:** 2026-04-25  
**Estado:** ✅ Activo

**Decisión:** Services específicos por entidad (`ClienteService`, `ProductoService`, `UsuarioService`).

**Descartado:** `IRepository<T>` genérico + `IUnitOfWork`

**Razón:**
- Abstracción prematura para el estado actual del proyecto.
- Un `IRepository<T>` genérico no añade valor cuando los queries son simples y específicos.
- Oculta el comportamiento real de EF Core detrás de una capa innecesaria.

**Condición de revisión:** Cuando el módulo de Ventas requiera transacciones multi-entidad reales, evaluar `UnitOfWork` explícito (no genérico).

---

## ADR-003: Soft Delete como Auditoría, NO como Filtro

**Fecha:** 2026-04-25  
**Estado:** ✅ Activo

**Decisión:** El campo `Activo` es un flag de auditoría. Los `GET` devuelven TODOS los registros (activos e inactivos). El frontend Angular controla la presentación visual.

**Descartado:** `HasQueryFilter(x => x.Activo == true)` en AppDbContext

**Razón:**
Miguel necesita visibilidad completa de todos los registros para auditoría. Los registros inactivos no son "eliminados", son registros históricos que el frontend mostrará con tratamiento visual diferente (colores, iconos, filtros opcionales).

**Implementación:**
- `PATCH /{id}/inactivar` → `Activo = false`, registro visible
- `PATCH /{id}/activar` → `Activo = true`
- `DELETE /{id}` → hard delete real (`EF Remove()`)

**NUNCA:** Agregar `HasQueryFilter` para `Activo` en ninguna entidad.

---

## ADR-004: Sin EF Migrations — Scripts SQL Manuales

**Fecha:** 2026-04-25  
**Estado:** ✅ Activo

**Decisión:** La base de datos se gestiona con scripts SQL versionados en `Database/`.

**Descartado:** EF Core Migrations (`dotnet ef migrations add`)

**Razón:**
Miguel prefiere control total sobre el DDL de SQL Server. Los scripts manuales permiten usar features específicas de SQL Server (computed columns, specific constraints, índices filtrados) que EF Migrations no genera de forma óptima.

**Estructura de scripts:**
```
Database/
  01_Schemas/   ← CREATE SCHEMA
  02_Tablas/    ← CREATE TABLE (DDL completo, uno por entidad)
  03_Seeds/     ← INSERT de datos de prueba/referencia
  04_Procedures/ ← Stored procedures (reservado)
  05_Indices/   ← Índices adicionales
```

---

## ADR-005: AuditableEntity Obligatorio en Todas las Entidades

**Fecha:** 2026-04-25  
**Estado:** ✅ Activo

**Decisión:** Toda entidad del dominio hereda `AuditableEntity`. Las tablas pivot (join tables) son la única excepción.

**Campos obligatorios:**
```csharp
int Id              // PK interna (IDENTITY, no exponer directamente)
Guid PublicId       // Identificador externo (NEWSEQUENTIALID, exponer en API)
bool Activo         // Soft delete flag
DateTime FechaRegistro     // GETUTCDATE() automático
DateTime? FechaActualizacion // Actualizado manualmente en cada handler
```

**Razón:**
- `PublicId` separa el identificador interno del externo (seguridad, no exponer secuencias)
- `Activo` + `FechaRegistro` + `FechaActualizacion` = trazabilidad completa de ciclo de vida
- Consistencia garantizada en toda la BD

**Excepción:** Tablas pivot (`UsuarioRol`, `RolPermiso`) no heredan AuditableEntity. Son relaciones puras sin ciclo de vida propio.

---

## ADR-006: BCrypt en Infrastructure, No en Application

**Fecha:** 2026-04-25  
**Estado:** ✅ Activo

**Decisión:** La verificación y generación de hashes de password (`BCrypt.Net.BCrypt.Verify` / `HashPassword`) reside en `Infrastructure/Repository/UsuarioService.cs`.

**Descartado:** Usar BCrypt directamente en `Application/Features/Auth/Login/LoginHandler.cs`

**Razón:**
Application layer no debe tener dependencias de paquetes externos de infraestructura (BCrypt, librerías de crypto). Solo depende de abstracciones. Infrastructure implementa los detalles.

**Implementación:**
- `IUsuarioService.AutenticarUsuario(email, password)` → retorna `Usuario?` o null si credenciales inválidas
- El Handler simplemente llama al service y verifica si es null

---

## ADR-007: JWT HS256 (no RS256)

**Fecha:** 2026-04-25  
**Estado:** ✅ Activo

**Decisión:** JWT con algoritmo HS256 (clave simétrica).

**Descartado:** RS256 (clave asimétrica)

**Razón:**
El sistema no es distribuido (un solo backend, un solo frontend). HS256 es más simple de gestionar. RS256 tiene ventaja en arquitecturas donde múltiples servicios necesitan verificar tokens sin acceso a la clave privada. No aplica aquí.

**Configuración:** `appsettings.json → Jwt:Key` (mínimo 32 caracteres).

**Nota de seguridad:** En producción, `Jwt:Key` debe venir de variables de entorno o Azure Key Vault, nunca del `appsettings.json` commiteado.

---

## ADR-008: Logout Dummy (sin blacklist)

**Fecha:** 2026-04-25  
**Estado:** ✅ Activo

**Decisión:** `POST /api/v1/auth/logout` retorna 200 OK sin invalidar el token en backend.

**Descartado:** Token blacklist en BD o cache

**Razón:**
- El frontend maneja la limpieza del estado (localStorage, memoria)
- Una blacklist requiere cache distribuido (Redis) o consulta a BD en cada request
- Para v3.0.0, el riesgo de tokens "zombies" es aceptable dado el contexto
- Se evalúa en fases futuras junto con refresh tokens

**Condición de revisión:** Si el requerimiento de seguridad escala (múltiples dispositivos, revocación forzada por admin), implementar blacklist con Redis.

---

## ADR-009: /me Endpoint Consulta BD (no decodifica JWT)

**Fecha:** 2026-04-25  
**Estado:** ✅ Activo

**Decisión:** `GET /api/v1/auth/me` hace query a BD y retorna datos frescos del usuario.

**Descartado:** Decodificar claims del JWT y retornarlos directamente

**Razón:**
- Los permisos pueden cambiar en BD sin invalidar el token existente
- La consulta a BD garantiza datos actualizados (roles, permisos, estado activo)
- El costo de una query por llamada a `/me` es aceptable

---

## ADR-010: Controllers sin [Authorize] Global (por ahora)

**Fecha:** 2026-04-25  
**Estado:** ⏳ Temporal

**Decisión:** ProductosController y ClientesController no tienen `[Authorize]` actualmente.

**Razón:**
El frontend Angular todavía está en proceso de cambiar de MockAuthService a ApiAuthService. Agregar `[Authorize]` antes de que el interceptor esté conectado rompería el flujo de desarrollo.

**Acción pendiente:** Agregar `[Authorize]` en ambos controllers (y futuros) una vez que el frontend confirme que el Bearer token se envía correctamente en todos los requests.

---

## ADR-011: ListaPrecioDetalle — Abstracción Simple por ProductoId, Presentaciones Deferred

**Fecha:** 2026-05-18  
**Estado:** ✅ Activo

**Decisión:**
- `ListaPrecio` se implementa en Sprint 5 como catálogo simple
- `ListaPrecioDetalle(ListaPrecioId, ProductoId, Precio)` se diferirá al módulo Ventas (Sprint 6+)
- Sin tabla `ProductoPresentacion` en fase inicial
- Abstracción: precios por `ProductoId` solamente
- Escalabilidad futura: Si negocio requiere presentaciones (tamaños, variantes), introducir `ProductoPresentacion` como entidad separada sin romper `ListaPrecioDetalle`

**Descartado:**
- Implementar `ProductoPresentacion` ahora (especulación de requisitos)
- Crear arquitectura multi-presentación sin validación funcional

**Razón:**
1. **YAGNI:** No anticipar requisitos inciertos. Las presentaciones son especulación hasta que el negocio las pida explícitamente.
2. **Velocidad de entrega:** Opción A simple completa catálogos en Sprint 5. Opción B (con presentaciones) añade 3-4 horas de complejidad innecesaria.
3. **Escalabilidad sin acoplamiento:** Si `ProductoPresentacion` es necesario futuro:
   - Se crea como entidad independiente
   - `ListaPrecioDetalle` se refactoriza a `(ListaPrecioId, ProductoPresentacionId, Precio)`
   - Cero breaking changes en lógica actual de catálogos
4. **Validación funcional real:** Presentaciones se necesitan cuando usuario lo demande en Ventas, no antes.

**Implementación:**
```sql
-- Sprint 5: Crear solo ListaPrecio
CREATE TABLE catalogo.ListasPrecios (
    Id INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(150) NOT NULL,
    MonedaId INT NOT NULL,
    EsDefault BIT DEFAULT 0,
    ...
);

-- Sprint 6+ (Ventas): Crear ListaPrecioDetalle
CREATE TABLE catalogo.ListaPrecioDetalle (
    Id INT IDENTITY PRIMARY KEY,
    ListaPrecioId INT NOT NULL,
    ProductoId INT NOT NULL,
    Precio DECIMAL(18,2) NOT NULL,
    UNIQUE (ListaPrecioId, ProductoId)
);

-- Futuro (si se necesita): Crear ProductoPresentacion
-- Refactorizar ListaPrecioDetalle: (ProductoPresentacionId en lugar de ProductoId)
```

**Condición de revisión:** Cuando módulo Ventas requiera presentaciones explícitamente, evaluar introducción de `ProductoPresentacion`.

**Impacto:** 
- Sprint 5: Catálogos completados sin `ListaPrecioDetalle`
- Sprint 6+: `ListaPrecioDetalle` implementada en Ventas (2-3 horas)
- Futuro: `ProductoPresentacion` si negocio lo solicita (refactor reversible)

---

**Última actualización:** 2026-05-18
