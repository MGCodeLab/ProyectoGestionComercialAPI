# AuditableUpdateHandlerBase — Patrón Centralizado de Actualización

**Última actualización:** 2026-06-28  
**Estado:** ✅ Decisión arquitectónica confirmada ADR-012  
**Alcance:** 29 handlers en total (9 completados, 20 pendientes)

---

## 🎯 Propósito

Centralizar la lógica de actualización de `FechaActualizacion` en una clase base para garantizar homogeneidad arquitectónica en todos los handlers que modifican entidades `AuditableEntity`.

**Objetivo clave:** Hacer imposible olvidar auditar la fecha de actualización.

---

## 📐 Patrón Arquitectónico

### Base Handler Class
**Ubicación:** `Application/Handlers/AuditableUpdateHandlerBase.cs`

```csharp
using AutoMapper;
using Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Handlers
{
    public abstract class AuditableUpdateHandlerBase<TCommand, TResponse>
        : IRequestHandler<TCommand, TResponse>
        where TCommand : IRequest<TResponse>
    {
        protected readonly IMapper Mapper;
        protected readonly ILogger Logger;

        protected AuditableUpdateHandlerBase(IMapper mapper, ILogger logger)
        {
            Mapper = mapper;
            Logger = logger;
        }

        public abstract Task<TResponse> Handle(TCommand request, CancellationToken cancellationToken);

        protected async Task<TResponse> UpdateAuditableEntity<TEntity>(
            TEntity entity,
            Func<Task> persistenceAction,
            Func<TResponse> createResponse,
            CancellationToken cancellationToken)
            where TEntity : AuditableEntity
        {
            entity.FechaActualizacion = DateTime.UtcNow;
            await persistenceAction();
            return createResponse();
        }
    }
}
```

### Handler Derivado (Template)

```csharp
using Application.Handlers;
using Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Application.Features.{Contexto}.{Entidad}.Actualizar
{
    public class Actualizar{Entidad}Handler
        : AuditableUpdateHandlerBase<Actualizar{Entidad}Command, int>
    {
        private readonly I{Entidad}Service _service;

        public Actualizar{Entidad}Handler(
            I{Entidad}Service service,
            IMapper mapper,
            ILogger<Actualizar{Entidad}Handler> logger)
            : base(mapper, logger)
        {
            _service = service;
        }

        public override async Task<int> Handle(Actualizar{Entidad}Command request, CancellationToken cancellationToken)
        {
            var entidad = await _service.ObtenerPorId(request.Id, true, cancellationToken);
            if (entidad == null)
                throw new KeyNotFoundException($"{Entidad} con Id {request.Id} no encontrada");

            Mapper.Map(request, entidad);

            return await UpdateAuditableEntity(
                entidad,
                async () => await _service.Actualizar(entidad, cancellationToken),
                () => entidad.Id,
                cancellationToken
            );
        }
    }
}
```

---

## ✅ Handlers Completados (9)

Estos handlers YA están refactorizados y usan `AuditableUpdateHandlerBase`:

### Catálogo
1. ✅ `Application/Features/Catalogo/CategoriaProducto/Actualizar/ActualizarCategoriaProductoHandler.cs`
2. ✅ `Application/Features/Catalogo/CondicionPago/Actualizar/ActualizarCondicionPagoHandler.cs`
3. ✅ `Application/Features/Catalogo/ListaPrecio/Actualizar/ActualizarListaPrecioHandler.cs`
4. ✅ `Application/Features/Catalogo/MarcaProducto/Actualizar/ActualizarMarcaProductoHandler.cs`
5. ✅ `Application/Features/Catalogo/TipoDocumento/Actualizar/ActualizarTipoDocumentoHandler.cs`

### Comercial
6. ✅ `Application/Features/Comercial/Proveedor/Actualizar/ActualizarProveedorHandler.cs`

### Organización
7. ✅ `Application/Features/Organizacion/Almacen/Actualizar/ActualizarAlmacenHandler.cs`
8. ✅ `Application/Features/Organizacion/Empresa/Actualizar/ActualizarEmpresaHandler.cs`
9. ✅ `Application/Features/Organizacion/Sucursal/Actualizar/ActualizarSucursalHandler.cs`

---

## 🚨 Handlers PENDIENTES (20)

Estos handlers AÚN usan `IRequestHandler` directo y asignan manualmente `FechaActualizacion = DateTime.UtcNow;`.

### Catálogo (16 handlers)

#### ModuloSistema
- [ ] `Application/Features/Catalogo/ModuloSistema/Actualizar/ActualizarModuloSistemaHandler.cs`
- [ ] `Application/Features/Catalogo/ModuloSistema/ActualizarEstado/ActualizarEstadoModuloSistemaHandler.cs`

#### Moneda
- [ ] `Application/Features/Catalogo/Moneda/Actualizar/ActualizarMonedaHandler.cs`
- [ ] `Application/Features/Catalogo/Moneda/ActualizarEstado/ActualizarEstadoMonedaHandler.cs`

#### Pais
- [ ] `Application/Features/Catalogo/Pais/Actualizar/ActualizarPaisHandler.cs`
- [ ] `Application/Features/Catalogo/Pais/ActualizarEstado/ActualizarEstadoPaisHandler.cs`

#### ParametroSistema
- [ ] `Application/Features/Catalogo/ParametroSistema/Actualizar/ActualizarParametroSistemaHandler.cs`
- [ ] `Application/Features/Catalogo/ParametroSistema/ActualizarEstado/ActualizarEstadoParametroSistemaHandler.cs`

#### SerieDocumento
- [ ] `Application/Features/Catalogo/SerieDocumento/Actualizar/ActualizarSerieDocumentoHandler.cs`
- [ ] `Application/Features/Catalogo/SerieDocumento/ActualizarEstado/ActualizarEstadoSerieDocumentoHandler.cs`

#### TipoComprobante
- [ ] `Application/Features/Catalogo/TipoComprobante/Actualizar/ActualizarTipoComprobanteHandler.cs`
- [ ] `Application/Features/Catalogo/TipoComprobante/ActualizarEstado/ActualizarEstadoTipoComprobanteHandler.cs`

#### TipoImpuesto
- [ ] `Application/Features/Catalogo/TipoImpuesto/Actualizar/ActualizarTipoImpuestoHandler.cs`
- [ ] `Application/Features/Catalogo/TipoImpuesto/ActualizarEstado/ActualizarEstadoTipoImpuestoHandler.cs`

#### UnidadMedida
- [ ] `Application/Features/Catalogo/UnidadMedida/Actualizar/ActualizarUnidadMedidaHandler.cs`
- [ ] `Application/Features/Catalogo/UnidadMedida/ActualizarEstado/ActualizarEstadoUnidadMedidaHandler.cs`

### Clientes (2 handlers)

- [ ] `Application/Features/Clientes/Actualizar/ActualizarClienteHandler.cs`
- [ ] `Application/Features/Clientes/ActualizarEstado/ActualizarEstadoClienteHandler.cs`

### Productos (2 handlers)

- [ ] `Application/Features/Productos/Actualizar/ActualizarProductoHandler.cs`
- [ ] `Application/Features/Productos/ActualizarEstado/ActualizarEstadoProductoHandler.cs`

---

## 🔄 Refactorización: Antes → Después

### ❌ ANTES (Manual)

```csharp
public class ActualizarPaisHandler : IRequestHandler<ActualizarPaisCommand, int>
{
    private readonly IPaisService _service;
    private readonly IMapper _mapper;

    public ActualizarPaisHandler(IPaisService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    public async Task<int> Handle(ActualizarPaisCommand request, CancellationToken cancellationToken)
    {
        var pais = await _service.ObtenerPorId(request.Id, true, cancellationToken);
        if (pais == null)
            throw new KeyNotFoundException($"Pais con Id {request.Id} no encontrado");

        _mapper.Map(request, pais);

        // ❌ PROBLEMA: Asignación manual, fácil olvidar
        pais.FechaActualizacion = DateTime.UtcNow;

        await _service.Actualizar(pais, cancellationToken);
        return pais.Id;
    }
}
```

### ✅ DESPUÉS (Base Handler)

```csharp
public class ActualizarPaisHandler
    : AuditableUpdateHandlerBase<ActualizarPaisCommand, int>
{
    private readonly IPaisService _service;

    public ActualizarPaisHandler(
        IPaisService service,
        IMapper mapper,
        ILogger<ActualizarPaisHandler> logger)
        : base(mapper, logger)
    {
        _service = service;
    }

    public override async Task<int> Handle(ActualizarPaisCommand request, CancellationToken cancellationToken)
    {
        var pais = await _service.ObtenerPorId(request.Id, true, cancellationToken);
        if (pais == null)
            throw new KeyNotFoundException($"Pais con Id {request.Id} no encontrado");

        Mapper.Map(request, pais);

        // ✅ CENTRALIZADO: Base handler maneja FechaActualizacion
        return await UpdateAuditableEntity(
            pais,
            async () => await _service.Actualizar(pais, cancellationToken),
            () => pais.Id,
            cancellationToken
        );
    }
}
```

### Cambios clave:
1. **Heredar de `AuditableUpdateHandlerBase<TCommand, TResponse>`** en lugar de `IRequestHandler<TCommand, TResponse>`
2. **Inyectar `ILogger<T>`** en constructor (necesario para base handler)
3. **Pasar `Mapper` y `Logger` al base handler** vía `base(mapper, logger)`
4. **Cambiar método a `public override`** para hacer override del método abstracto
5. **Envolver persistencia en `UpdateAuditableEntity()`** que centraliza la lógica de FechaActualizacion

---

## 🎯 Beneficios

| Aspecto | Antes | Después |
|--------|-------|---------|
| **Consistency** | 9 handlers sí, 20 no | 100% homogéneo |
| **Mantenibilidad** | Auditoría dispersa en 29 archivos | Centralizado en 1 base class |
| **Errores** | Fácil olvidar FechaActualizacion | Imposible olvidar |
| **Escalabilidad** | Nuevos handlers copian patrón manual | Nuevos handlers heredan automáticamente |
| **Testing** | Testear auditoría en cada handler | Test base handler, handlers heredan lógica |
| **Futuro** | Cambios en auditoría → actualizar 29 handlers | Cambios en auditoría → actualizar 1 base handler |

---

## 🛠️ Plan de Refactorización

### Fase 1: Validación (2026-06-28)
- [x] Crear ADR-012 en ARCHITECTURE_DECISIONS.md
- [x] Documentar patrón en este archivo
- [x] Documentar lista completa de 29 handlers

### Fase 2: Ejecución (2026-06-28)
- [ ] Enviar a Nexus-Fast-Builder para refactorizar 20 handlers pendientes
- [ ] Validar compilación cero errores
- [ ] Validar endpoints funcionales

### Fase 3: Testing (2026-06-28)
- [ ] Probar módulos: Catalogo, Clientes, Productos
- [ ] Verificar FechaActualizacion actualiza correctamente
- [ ] Verificar logs de auditoría

### Fase 4: Commit (2026-06-28)
- [ ] Crear commit: `feat(handlers): refactor all update handlers to use AuditableUpdateHandlerBase`
- [ ] Incluir en rama `catalogo-base/validators`
- [ ] Actualizar USUARIO_DOCS

---

## 📋 Checklist Post-Refactorización

- [ ] Compilación: `dotnet build` → 0 errores, 0 warnings
- [ ] Todos los 29 handlers implementan correctamente el patrón
- [ ] Base handler `AuditableUpdateHandlerBase` es clase abstracta con método Handle abstracto
- [ ] Todos los handlers tienen `public override` en Handle
- [ ] Inyección de dependencias: todos reciben `IMapper`, `ILogger<T>`
- [ ] `UpdateAuditableEntity` es `protected`
- [ ] Los handlers que devuelven `Unit` usan `Unit.Value` en lambda
- [ ] Los handlers que devuelven `int` usan `() => entidad.Id`
- [ ] Endpoints funcionales: GET, POST, PUT, PATCH (inactivar/activar), DELETE
- [ ] FechaActualizacion se actualiza correctamente en cada operación de actualización
- [ ] Logs auditoría presentes (ej: "Entidad actualizada: {Id}")

---

## 🔗 Referencias

- **ADR-012:** `IA_Docs/ARCHITECTURE_DECISIONS.md` (sección ADR-012)
- **IMPLEMENTATION_PATTERNS.md:** Patrones generales de módulos
- **Code example:** `Application/Features/Organizacion/Sucursal/Actualizar/ActualizarSucursalHandler.cs` (referencia completada)

---

## 📞 Contacto & Cambios Futuros

**Autor:** Nexus ERP Backend Architecture  
**Decisión:** Miguel González Cuevas (2026-06-28)  
**Revisión:** Q3 2026 si se requieren extensiones (soft delete automático, auditoría de usuario, etc.)

