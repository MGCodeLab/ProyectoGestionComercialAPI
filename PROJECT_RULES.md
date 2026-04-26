# PROJECT_RULES.md

## Development Standards

## General Principles

* Arquitectura orientada a producción
* Código mantenible y escalable
* Evitar sobreingeniería innecesaria
* Evitar deuda técnica futura
* Controllers delgados
* Lógica de negocio fuera de Controllers
* Logging de negocio en Handlers
* Errores globales en Middleware

## CQRS Rule

### Commands

Usar MediatR para:

* Create
* Update
* Delete
* cambios de estado
* transacciones

### Queries

Usar Services para:

* lecturas
* listados
* consultas simples

No implementar CQRS completo si no aporta valor real.

## Database Standards

### Naming

* Tablas en plural
* Schemas por dominio

Ejemplo:

* catalogo
* comercial
* seguridad
* ventas

### Identity Strategy

Todas las entidades importantes deben usar:

* `Id` → INT interno
* `PublicId` → GUID público

Nunca exponer `Id` interno hacia cliente externo.

### Delete Strategy

Usar:

* Soft Delete

```text
Activo = false
```

Hard delete solo si el registro no tiene dependencias y aplica por negocio.

### Foreign Keys

Usar:

```text
DeleteBehavior.Restrict
```

Evitar cascadas peligrosas.

### Auditing

Obligatorio:

* FechaRegistro
* FechaActualizacion
* Activo
* PublicId

Preferiblemente mediante:

* AuditableEntity

## API Standards

Todas las respuestas deben seguir:

```json
{
  "success": true,
  "message": "",
  "data": {},
  "errors": [],
  "traceId": ""
}
```

Usar:

* ApiResponse<T>
* OkResponse
* CreatedResponse

## Git Rules

### Protected Branches

`main` protegida

Obligatorio:

* Pull Request
* No push directo
* Merge desde PR

### Branch Naming

Ejemplos:

* feature/clientes
* feature/ventas
* fix/productos-update
* hotfix/login-bug

## Commit Rules

Formato obligatorio:

```text
tipo(modulo): descripcion
```

Tipos:

* feat
* fix
* refactor
* docs
* chore

Ejemplo:

```text
feat(clientes): create cliente module
```

## Documentation Rule

Toda mejora importante debe actualizar:

* README.md
* CHANGELOG.md
* RELEASE_NOTES.md

La documentación es parte del desarrollo, no opcional.
