# PR#15 Code Review - Refinement Tasks

**Fecha:** 2026-06-27  
**PR:** #15 - CancellationToken Homogeneity  
**Analista:** Nexus-Backend-Architect  
**Asignado a:** Nexus-Fast-Builder  
**Prioridad:** 🟡 MEDIA (después del merge de PR#15)  
**Tiempo Estimado:** 2 horas

---

## RESUMEN EJECUTIVO

El PR#15 implementó exitosamente CancellationToken homogéneo en Services y Handlers. Sin embargo, el code review identificó 2 hallazgos de calidad arquitectónica:

1. **FechaActualizacion** — Repetido en 9 handlers (violación DRY + riesgo auditoría)
2. **Service interface tracking param** — Sin default, reduce discoverabilidad

Ambos son **mejoras legítimas** que harían el código más robusto y mantenible. Se proponen soluciones arquitectónicas.

---

## REFINEMENT 1: FechaActualizacion — Base Handler para Auditoría

### Problema
En 9 handlers de Actualizar, encontramos:
```csharp
_mapper.Map(command, categoria);
categoria.FechaActualizacion = DateTime.UtcNow;  // ← REPETIDO 9 veces
await _service.Actualizar(categoria, cancellationToken);
```

**Riesgos:**
- ✗ Si se agrega un 10º handler y se olvida esta línea, auditoría se rompe silenciosamente
- ✗ ~200 líneas de boilerplate innecesario
- ✗ Viola DRY
- ✗ Punto único de fallo en la auditoría

### Solución Arquitectónica

**OPCIÓN ELEGIDA: Base Handler Class**

Crear una clase base `AuditableUpdateHandlerBase` que encapsule el patrón completo de actualización. Todos los handlers que actualicen `AuditableEntity` heredan de esta clase, garantizando que `FechaActualizacion` se establezca SIEMPRE.

**Ventajas de esta solución:**
- ✅ Explícito y type-safe (no reflection mágica)
- ✅ Funciona realmente (la entidad está disponible dentro del handler)
- ✅ Fácil de debuggear
- ✅ Patrones consistentes: todos los updates usan la misma estructura
- ✅ Imposible olvidar: Si heredas de la base, el timestamp se establece automáticamente

### Implementación

**FASE 1: Crear la Base Handler Class**

Archivo: `Application/Handlers/AuditableUpdateHandlerBase.cs`

```csharp
using AutoMapper;
using Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Handlers
{
    /// <summary>
    /// Base class para handlers que actualizan entidades AuditableEntity.
    /// 
    /// Garantiza que FechaActualizacion se establezca automáticamente
    /// en TODOS los updates sin riesgo de olvido.
    /// 
    /// Patrón de uso en handler:
    /// 1. Obtener entidad con tracking=true
    /// 2. Mapear cambios
    /// 3. Llamar a UpdateAuditableEntity() que:
    ///    - Establece FechaActualizacion = DateTime.UtcNow
    ///    - Llama al servicio para guardar
    ///    - Retorna la respuesta
    /// </summary>
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

        /// <summary>
        /// Ejecuta la actualización de una entidad auditable con timestamp automático.
        /// 
        /// Patrón:
        /// 1. Establece FechaActualizacion = DateTime.UtcNow
        /// 2. Ejecuta la acción de persistencia (saveChanges)
        /// 3. Retorna la respuesta
        /// 
        /// Ejemplo:
        /// await UpdateAuditableEntity(
        ///     categoria,
        ///     async () => await _service.Actualizar(categoria, cancellationToken),
        ///     cancellationToken
        /// );
        /// </summary>
        protected async Task<TResponse> UpdateAuditableEntity<TEntity>(
            TEntity entity,
            Func<Task> persistenceAction,
            Func<TResponse> createResponse,
            CancellationToken cancellationToken)
            where TEntity : AuditableEntity
        {
            // Establecer timestamp automáticamente
            entity.FechaActualizacion = DateTime.UtcNow;

            // Ejecutar la persistencia
            await persistenceAction();

            // Retornar respuesta
            return createResponse();
        }
    }
}
```

**FASE 2: Refactorizar handlers para heredar de la base**

Ejemplo: `ActualizarCategoriaProductoHandler`

```csharp
// ANTES:
public class ActualizarCategoriaProductoHandler : IRequestHandler<ActualizarCategoriaProductoCommand, int>
{
    public async Task<int> Handle(ActualizarCategoriaProductoCommand command, CancellationToken cancellationToken)
    {
        var categoria = await _service.ObtenerPorIdAsync(command.Id, tracking: true, cancellationToken);
        if (categoria == null)
            throw new InvalidOperationException($"CategoriaProducto con ID {command.Id} no encontrada");

        _mapper.Map(command, categoria);
        categoria.FechaActualizacion = DateTime.UtcNow;  // ← ESTA LÍNEA SE ELIMINA

        await _service.Actualizar(categoria, cancellationToken);
        _logger.LogInformation("ActualizarCategoriaProducto: ID {id}", categoria.Id);
        return categoria.Id;
    }
}

// DESPUÉS:
public class ActualizarCategoriaProductoHandler 
    : AuditableUpdateHandlerBase<ActualizarCategoriaProductoCommand, int>
{
    private readonly ICategoriaProductoService _service;
    private readonly ICategoriaProductoValidatorService _validator;

    public ActualizarCategoriaProductoHandler(
        ICategoriaProductoService service,
        ICategoriaProductoValidatorService validator,
        IMapper mapper,
        ILogger<ActualizarCategoriaProductoHandler> logger)
        : base(mapper, logger)
    {
        _service = service;
        _validator = validator;
    }

    public async Task<int> Handle(ActualizarCategoriaProductoCommand command, CancellationToken cancellationToken)
    {
        Logger.LogInformation("ActualizarCategoriaProducto: {@request}", command);

        var categoria = await _service.ObtenerPorIdAsync(command.Id, tracking: true, cancellationToken);
        if (categoria == null)
            throw new InvalidOperationException($"CategoriaProducto con ID {command.Id} no encontrada");

        // Validación de ciclos
        if (command.CategoriaPadreId.HasValue &&
            command.CategoriaPadreId != categoria.CategoriaPadreId)
        {
            var esDescendiente = await _validator.EsDescendienteDeAsync(command.CategoriaPadreId.Value, command.Id);
            if (esDescendiente)
                throw new InvalidOperationException("No se puede crear ciclo: padre no puede ser descendiente");
        }

        Mapper.Map(command, categoria);

        // Usar el método base que establece FechaActualizacion automáticamente
        return await UpdateAuditableEntity(
            categoria,
            async () => await _service.Actualizar(categoria, cancellationToken),
            () => categoria.Id,
            cancellationToken
        );
    }
}
```

**FASE 3: Refactorizar los 9 handlers**

Aplicar el patrón anterior a estos handlers:

```
1. ActualizarCategoriaProductoHandler
2. ActualizarCondicionPagoHandler
3. ActualizarListaPrecioHandler
4. ActualizarMarcaProductoHandler
5. ActualizarTipoDocumentoHandler
6. ActualizarProveedorHandler
7. ActualizarAlmacenHandler
8. ActualizarEmpresaHandler
9. ActualizarSucursalHandler
```

**Pasos para cada handler:**
1. Cambiar `IRequestHandler<T, R>` a `: AuditableUpdateHandlerBase<T, R>`
2. Agregar `: base(mapper, logger)` en constructor
3. Cambiar `_mapper` a `Mapper` y `_logger` a `Logger`
4. Reemplazar `await _service.Actualizar(...); return entity.Id;` con la llamada `UpdateAuditableEntity()`
5. Eliminar la línea manual de `FechaActualizacion = DateTime.UtcNow`

**Resultado:**
- ✅ FechaActualizacion centralizado en base class
- ✅ Imposible olvidar: Los handlers herdan el comportamiento
- ✅ DRY: 0 repetición
- ✅ Auditoría garantizada
- ✅ Type-safe y explícito

---

## REFINEMENT 2: Service Interface Tracking Parameter — Extension Methods

### Problema
`ObtenerPorIdAsync(int id, bool tracking, CancellationToken cancellationToken)` requiere paso explícito del parámetro `tracking`:

```csharp
var categoria = await _service.ObtenerPorIdAsync(command.Id, tracking: true, cancellationToken);
```

**Riesgos:**
- ✗ Reduces discoverabilidad: ¿Cuál es el "por defecto"?
- ✗ Añade fricción: 20+ call sites deben pasar explícitamente
- ✗ Potencial fuente de confusión en nuevos handlers

### Solución Arquitectónica

**OPCIÓN ELEGIDA: Extension Methods**

Crear un overload vía extension methods que proporciona un default sensato:

```csharp
// Application/Extensions/ServiceExtensions.cs
public static class ServiceExtensions
{
    /// <summary>
    /// Overload de ObtenerPorIdAsync con tracking = false por defecto.
    /// Usar cuando solo necesitas LEER la entidad sin intención de actualizar.
    /// </summary>
    public static Task<CategoriaProducto?> ObtenerPorIdAsync(
        this ICategoriaProductoService service, 
        int id,
        CancellationToken cancellationToken = default)
    {
        return service.ObtenerPorIdAsync(id, tracking: false, cancellationToken);
    }
}
```

### Implementación

**FASE 1: Crear extension methods para TODOS los Services**

Archivo: `Application/Extensions/ServiceQueryExtensions.cs`

```csharp
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Catalogo;
using Domain.Comercial;
using Domain.Organizacion;

namespace Application.Extensions
{
    /// <summary>
    /// Extension methods que proporcionan overloads convenientes para queries ObtenerPorId.
    /// 
    /// Patrón:
    /// - _service.ObtenerPorIdAsync(id, ct)          → tracking = false (para leer)
    /// - _service.ObtenerPorIdAsync(id, true, ct)    → tracking = true (para actualizar)
    /// 
    /// Esto mejora discoverabilidad y reduce parámetros redundantes en la mayoría de cases.
    /// </summary>
    public static class ServiceQueryExtensions
    {
        // ======== CATALOGO Module ========
        
        public static Task<CategoriaProducto?> ObtenerPorIdAsync(
            this ICategoriaProductoService service, 
            int id,
            CancellationToken cancellationToken = default)
        {
            return service.ObtenerPorIdAsync(id, tracking: false, cancellationToken);
        }

        public static Task<MarcaProducto?> ObtenerPorIdAsync(
            this IMarcaProductoService service, 
            int id,
            CancellationToken cancellationToken = default)
        {
            return service.ObtenerPorIdAsync(id, tracking: false, cancellationToken);
        }

        // ======== ORGANIZACION Module ========
        
        public static Task<Sucursal?> ObtenerPorIdAsync(
            this ISucursalService service, 
            int id,
            CancellationToken cancellationToken = default)
        {
            return service.ObtenerPorIdAsync(id, tracking: false, cancellationToken);
        }

        public static Task<Almacen?> ObtenerPorIdAsync(
            this IAlmacenService service, 
            int id,
            CancellationToken cancellationToken = default)
        {
            return service.ObtenerPorIdAsync(id, tracking: false, cancellationToken);
        }

        public static Task<Empresa?> ObtenerPorIdAsync(
            this IEmpresaService service, 
            int id,
            CancellationToken cancellationToken = default)
        {
            return service.ObtenerPorIdAsync(id, tracking: false, cancellationToken);
        }

        // TODO: Agregar más servicios según necesario
        // Patrón: Un overload por cada IxxxService en el proyecto
    }
}
```

**FASE 2: Actualizar Handlers para usar el overload**

En handlers que SOLO LEEN (sin intención de actualizar), usar:
```csharp
// Antes:
var categoria = await _service.ObtenerPorIdAsync(command.Id, tracking: false, cancellationToken);

// Después:
var categoria = await _service.ObtenerPorIdAsync(command.Id, cancellationToken);
```

En handlers que ACTUALIZAN, mantener explícito:
```csharp
var categoria = await _service.ObtenerPorIdAsync(command.Id, tracking: true, cancellationToken);
```

**Resultado:**
- ✅ Default sensato: tracking = false para lecturas
- ✅ Explícito cuando necesario: tracking = true para updates
- ✅ Mejora discoverabilidad: El overload "simple" es el caso por defecto
- ✅ Menos parámetros redundantes

---

## CHECKLIST DE EJECUCIÓN

### Para Nexus-Fast-Builder

#### REFINEMENT 1: Base Handler Class
- [ ] Crear `Application/Handlers/AuditableUpdateHandlerBase.cs`
- [ ] Compilar y verificar 0 errores
- [ ] Refactorizar los 9 handlers para heredar de `AuditableUpdateHandlerBase<T, R>`
  - [ ] ActualizarCategoriaProductoHandler
  - [ ] ActualizarCondicionPagoHandler
  - [ ] ActualizarListaPrecioHandler
  - [ ] ActualizarMarcaProductoHandler
  - [ ] ActualizarTipoDocumentoHandler
  - [ ] ActualizarProveedorHandler
  - [ ] ActualizarAlmacenHandler
  - [ ] ActualizarEmpresaHandler
  - [ ] ActualizarSucursalHandler
- [ ] Compilar nuevamente → 0 errores
- [ ] Commit: `refactor(audit): centralizar FechaActualizacion en AuditableUpdateHandlerBase`

#### REFINEMENT 2: Extension Methods
- [ ] Crear `Application/Extensions/ServiceQueryExtensions.cs`
- [ ] Agregar overloads para TODOS los servicios actuales
- [ ] Compilar y verificar 0 errores
- [ ] Actualizar calls en handlers (donde sea aplicable)
- [ ] Commit: `refactor(services): agregar extension methods para ObtenerPorIdAsync`

#### FINAL
- [ ] `dotnet build` → 0 errores
- [ ] Crear rama: `refactor/pr15-refinements`
- [ ] Hacer PR hacia `main`
- [ ] Código review antes de merge

---

## DECISIONES ARQUITECTÓNICAS ASOCIADAS

**ADR-NEW-001: Base Handler Class para Auditoría de Updates**
- Los handlers que actualicen AuditableEntity heredan de `AuditableUpdateHandlerBase<T, R>`
- El método `UpdateAuditableEntity()` garantiza que `FechaActualizacion = DateTime.UtcNow` se establezca
- Imposible olvidar: Es parte de la clase base que los handlers heredan
- Type-safe y explícito: No hay reflection mágica
- Patrón consistente para TODOS los updates

**ADR-NEW-002: Service Extension Methods para defaults**
- Los servicios proveen overloads convenientes vía extensions
- tracking = false es el default (caso de lectura)
- tracking = true es explícito cuando necesario (updates)

---

## NOTAS IMPORTANTES

### Para Miguel (Arquitecto Principal)

Estos refinements son **mejoras de calidad**, no cambios críticos. El PR#15 está listo para producción. Estos refinements:

1. **Hacen la auditoría más robusta** (imposible olvidar FechaActualizacion)
2. **Mejoran la DX** (discoverabilidad de service methods)
3. **Reduce deuda técnica** (menos boilerplate)

Son **opcionales pero recomendados** para mantener estándares enterprise.

### Para el Equipo

- El Behavior MediatR es automático: No requiere cambios en nuevos handlers
- Los extension methods son backward-compatible: Código existente sigue funcionando
- Ambos mejoran la robustez sin romper nada

---

## ESTIMACIÓN

| Tarea | Tiempo | Notas |
|-------|--------|-------|
| AuditableUpdateBehavior | 30 min | Crear + registrar + verificar |
| Remover líneas duplicadas | 20 min | Editar 9 handlers |
| Extension methods | 30 min | Crear + agregar overloads |
| Testing + compilación | 20 min | dotnet build + verificación |
| **TOTAL** | **~2 horas** | Pueden hacerse en paralelo |

---

**Estado:** ✋ PENDIENTE DE APROBACIÓN POR MIGUEL  
**Siguiente paso:** Miguel valida el enfoque → Nexus-Fast-Builder ejecuta

