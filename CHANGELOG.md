# CHANGELOG.md

## [v3.0.0] - In Progress

### Added

* Inicio del módulo de Clientes
* Diseño de entidad Cliente con arquitectura enterprise
* Implementación de `AuditableEntity` como clase base compartida
* Separación de `Id` interno (INT) + `PublicId` externo (GUID)
* Configuración de índices únicos y auditoría obligatoria
* Preparación para integración con módulo de Ventas
* Estandarización para uso de IA (Claude Code / ChatGPT / Gemini)
* Documentación de reglas de proyecto y gobernanza técnica

### Planned

* CRUD completo de Cliente
* Módulo TipoDocumento
* Seeds iniciales de catálogos
* Integración Angular frontend
* Inicio módulo de Ventas

---

## [v2.2.1] - Completed

### Fixed

* Eliminación de `Activo` del DTO y Command de actualización de Producto
* Separación de actualización de estado mediante `ActualizarEstadoProductoCommand`
* Corrección de bug en actualización de productos

---

## [v2.2.0] - Completed

### Added

* Configuración de CORS para Angular (`AngularPolicy`)
* Preparación de integración frontend Angular + API

---

## [v2.1.0] - Completed

### Added

* Response Wrapper estándar (`ApiResponse<T>`)
* Controller Extensions (`OkResponse`, `CreatedResponse`)
* Estandarización de respuestas HTTP
* `traceId` integrado en errores
* Manejo global de `NotFound`

---

## [v2.0.0] - Completed

### Added

* Implementación de CQRS pragmático con MediatR
* Commands + Handlers
* FluentValidation
* ValidationBehaviour
* Middleware global de excepciones
* Logging en Handlers
* Features separadas por módulo

---

## [v1.0.0] - Completed

### Added

* CRUD básico de Productos
* Clean Architecture base
* EF Core + SQL Server
* AutoMapper
* Scripts SQL versionados

