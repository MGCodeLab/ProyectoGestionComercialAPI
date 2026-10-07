# Refactor: ActualizarEstado Handlers + Service Cleanup

Fecha: 2026-06-28
Branch: catalogo-base/validators

## Qué se hizo

### PASO 1: 7 Handlers ActualizarEstado migrados a AuditableUpdateHandlerBase

| Handler | Return | Firma service |
|---|---|---|
| ActualizarEstadoCategoriaProductoHandler | int | Actualizar(entidad, ct) |
| ActualizarEstadoMarcaProductoHandler | int | Actualizar(entidad, ct) |
| ActualizarEstadoListaPrecioHandler | int | Actualizar(entidad, ct) — migrado desde ActualizarEstado() |
| ActualizarEstadoTipoDocumentoHandler | Unit | Actualizar(ct) |
| ActualizarEstadoAlmacenHandler | int | Actualizar(entidad, ct) |
| ActualizarEstadoEmpresaHandler | int | Actualizar(entidad, ct) |
| ActualizarEstadoSucursalHandler | int | Actualizar(entidad, ct) |

Cambios por handler:
- Herencia: IRequestHandler -> AuditableUpdateHandlerBase
- Constructor: _logger como campo eliminado, ILogger pasado como base(null, logger)
- Handle: marcado override
- Persistencia: UpdateAuditableEntity(entity, persistAction, responseFactory, ct)
- Logging: string interpolation reemplazada por structured logging con placeholders

Caso especial ListaPrecio: el handler anterior llamaba _service.ActualizarEstado() (Patron B).
Migrado a Patron A: handler obtiene entidad con ObtenerPorId (tracking por defecto), establece Activo,
llama UpdateAuditableEntity -> _service.Actualizar(entidad, ct).

### PASO 2: FechaActualizacion eliminada de 2 services

- AlmacenService.Actualizar(): eliminada linea almacen.FechaActualizacion = DateTime.UtcNow
- EmpresaService.Actualizar(): eliminada linea empresa.FechaActualizacion = DateTime.UtcNow
- ListaPrecioService: Actualizar() ya no tenia FechaActualizacion (sin cambio)
- ProveedorService: Actualizar() ya no tenia FechaActualizacion (sin cambio)

## Por que

FechaActualizacion = DateTime.UtcNow estaba duplicada: el base handler la establece antes
de llamar al service. Centralizar en Application garantiza auditoria sin depender de
implementaciones en Infrastructure.

## Resultado build

0 errores. 77 warnings (pre-existentes, nullable reference types y obsolete API, no relacionados).

## Archivos modificados

Application:
- Features/Catalogo/CategoriaProducto/ActualizarEstado/ActualizarEstadoCategoriaProductoHandler.cs
- Features/Catalogo/MarcaProducto/ActualizarEstado/ActualizarEstadoMarcaProductoHandler.cs
- Features/Catalogo/ListaPrecio/ActualizarEstado/ActualizarEstadoListaPrecioHandler.cs
- Features/Catalogo/TipoDocumento/ActualizarEstado/ActualizarEstadoTipoDocumentoHandler.cs
- Features/Organizacion/Almacen/ActualizarEstado/ActualizarEstadoAlmacenHandler.cs
- Features/Organizacion/Empresa/ActualizarEstado/ActualizarEstadoEmpresaHandler.cs
- Features/Organizacion/Sucursal/ActualizarEstado/ActualizarEstadoSucursalHandler.cs

Infrastructure:
- Repository/AlmacenService.cs
- Repository/EmpresaService.cs

## NO tocados (Patron B)

- Features/Comercial/Proveedor/ActualizarEstado/ActualizarEstadoProveedorHandler.cs
- Features/Catalogo/CondicionPago/ActualizarEstado/ActualizarEstadoCondicionPagoHandler.cs

## Proximos pasos

1. Aprobacion de Miguel + testing manual de los 7 endpoints
2. Commit y merge a develop
3. Evaluar si AuditableUpdateHandlerBase debe declarar IMapper como nullable (IMapper?)
   para eliminar warning CS8625 en base(null, logger) — decision arquitectonica pendiente
4. Verificar si ListaPrecioService.ActualizarEstado() y ProveedorService.ActualizarEstado()
   siguen siendo usados por algun otro caller o pueden marcarse como obsoletos
