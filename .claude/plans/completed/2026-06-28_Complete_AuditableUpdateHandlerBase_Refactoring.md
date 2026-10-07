# EJECUTABLE: Refactorización Completa AuditableUpdateHandlerBase (20 Handlers Pendientes)

**Fecha:** 2026-06-28  
**Decisión:** OPCIÓN A confirmada — Refactorizar los 29 handlers (9 completados + 20 pendientes)  
**Prioridad:** Alta (homogeneidad arquitectónica)  
**Estado:** ✅ Aprobado para ejecución  
**Responsable:** Nexus-Fast-Builder  

---

## 📋 Especificación Exacta

### Qué se hace:
Refactorizar **20 handlers pendientes** que actualmente implementan `IRequestHandler` directamente, para que hereden de `AuditableUpdateHandlerBase<TCommand, TResponse>`.

### Por qué:
- Garantizar homogeneidad arquitectónica en todos los handlers de actualización
- Centralizar lógica de auditoría (FechaActualizacion) en un único lugar
- Hacer imposible olvidar la auditoría en futuro código
- Mantener código consistente (9 handlers ya usan este patrón, 20 no)

### Resultado esperado:
- 29/29 handlers usando el patrón base
- Compilación: 0 errores, 0 warnings
- Todos los endpoints funcionales
- FechaActualizacion actualizado correctamente en cada operación

---

## 🎯 LISTA DE 20 HANDLERS A REFACTORIZAR

### CATÁLOGO (16 handlers)

#### ModuloSistema
1. `Application/Features/Catalogo/ModuloSistema/Actualizar/ActualizarModuloSistemaHandler.cs`
2. `Application/Features/Catalogo/ModuloSistema/ActualizarEstado/ActualizarEstadoModuloSistemaHandler.cs`

#### Moneda
3. `Application/Features/Catalogo/Moneda/Actualizar/ActualizarMonedaHandler.cs`
4. `Application/Features/Catalogo/Moneda/ActualizarEstado/ActualizarEstadoMonedaHandler.cs`

#### Pais
5. `Application/Features/Catalogo/Pais/Actualizar/ActualizarPaisHandler.cs`
6. `Application/Features/Catalogo/Pais/ActualizarEstado/ActualizarEstadoPaisHandler.cs`

#### ParametroSistema
7. `Application/Features/Catalogo/ParametroSistema/Actualizar/ActualizarParametroSistemaHandler.cs`
8. `Application/Features/Catalogo/ParametroSistema/ActualizarEstado/ActualizarEstadoParametroSistemaHandler.cs`

#### SerieDocumento
9. `Application/Features/Catalogo/SerieDocumento/Actualizar/ActualizarSerieDocumentoHandler.cs`
10. `Application/Features/Catalogo/SerieDocumento/ActualizarEstado/ActualizarEstadoSerieDocumentoHandler.cs`

#### TipoComprobante
11. `Application/Features/Catalogo/TipoComprobante/Actualizar/ActualizarTipoComprobanteHandler.cs`
12. `Application/Features/Catalogo/TipoComprobante/ActualizarEstado/ActualizarEstadoTipoComprobanteHandler.cs`

#### TipoImpuesto
13. `Application/Features/Catalogo/TipoImpuesto/Actualizar/ActualizarTipoImpuestoHandler.cs`
14. `Application/Features/Catalogo/TipoImpuesto/ActualizarEstado/ActualizarEstadoTipoImpuestoHandler.cs`

#### UnidadMedida
15. `Application/Features/Catalogo/UnidadMedida/Actualizar/ActualizarUnidadMedidaHandler.cs`
16. `Application/Features/Catalogo/UnidadMedida/ActualizarEstado/ActualizarEstadoUnidadMedidaHandler.cs`

### CLIENTES (2 handlers)
17. `Application/Features/Clientes/Actualizar/ActualizarClienteHandler.cs`
18. `Application/Features/Clientes/ActualizarEstado/ActualizarEstadoClienteHandler.cs`

### PRODUCTOS (2 handlers)
19. `Application/Features/Productos/Actualizar/ActualizarProductoHandler.cs`
20. `Application/Features/Productos/ActualizarEstado/ActualizarEstadoProductoHandler.cs`

---

## 🔧 PATRÓN DE REFACTORIZACIÓN

### PASO 1: Cambiar herencia de clase

```csharp
// ❌ ANTES
public class ActualizarPaisHandler : IRequestHandler<ActualizarPaisCommand, int>

// ✅ DESPUÉS
public class ActualizarPaisHandler
    : AuditableUpdateHandlerBase<ActualizarPaisCommand, int>
```

### PASO 2: Actualizar usings

**Agregar:**
```csharp
using Application.Handlers;
using Microsoft.Extensions.Logging;
```

### PASO 3: Actualizar constructor

```csharp
// ❌ ANTES
public ActualizarPaisHandler(IPaisService service, IMapper mapper)
{
    _service = service;
    _mapper = mapper;
}

// ✅ DESPUÉS
public ActualizarPaisHandler(
    IPaisService service,
    IMapper mapper,
    ILogger<ActualizarPaisHandler> logger)
    : base(mapper, logger)
{
    _service = service;
}
```

**Cambios:**
- Agregar parámetro `ILogger<ActualizarPaisHandler> logger`
- Agregar call a `base(mapper, logger)` en constructor
- ELIMINAR la asignación manual de `_mapper = mapper` (lo hace el base handler)

### PASO 4: Cambiar firma del método Handle

```csharp
// ❌ ANTES
public async Task<int> Handle(ActualizarPaisCommand request, CancellationToken cancellationToken)

// ✅ DESPUÉS
public override async Task<int> Handle(ActualizarPaisCommand request, CancellationToken cancellationToken)
```

**Cambio:** Agregar palabra clave `override` (es implementación de método abstracto del base handler).

### PASO 5: Usar UpdateAuditableEntity

```csharp
// ❌ ANTES
pais.FechaActualizacion = DateTime.UtcNow;  // ← ELIMINAR
await _service.Actualizar(pais, cancellationToken);
return pais.Id;

// ✅ DESPUÉS
return await UpdateAuditableEntity(
    pais,
    async () => await _service.Actualizar(pais, cancellationToken),
    () => pais.Id,
    cancellationToken
);
```

**Parámetros:**
1. `pais` — la entidad a actualizar
2. `async () => await _service.Actualizar(pais, cancellationToken)` — acción de persistencia
3. `() => pais.Id` — función que retorna la respuesta (el ID)
4. `cancellationToken` — para operaciones async

### PASO 6: Usar Mapper desde base handler

```csharp
// ❌ ANTES
_mapper.Map(request, pais);

// ✅ DESPUÉS
Mapper.Map(request, pais);  // Mapper es protected en base handler
```

---

## ⚠️ CASOS ESPECIALES

### Para handlers que retornan `Unit`

En lugar de `() => pais.Id`, retornar `() => Unit.Value`:

```csharp
return await UpdateAuditableEntity(
    moneda,
    async () => await _service.Actualizar(moneda, cancellationToken),
    () => Unit.Value,  // ← Unit.Value en lugar de Id
    cancellationToken
);
```

**Handlers con Unit:**
- TipoDocumento (Actualizar, ActualizarEstado) — YA COMPLETADO
- Cualquier otro handler nuevo que devuelva Unit

### Para handlers con lógica compleja pre/post actualización

Si el handler tiene lógica adicional ANTES de llamar a UpdateAuditableEntity:
- ✅ Permitido: hacer validaciones, obtener datos, mapeos
- ✅ Permitido: logging antes de UpdateAuditableEntity
- ❌ NO permitido: actualizar FechaActualizacion DESPUÉS de UpdateAuditableEntity (UpdateAuditableEntity lo hace)

```csharp
// ✅ CORRECTO
var pais = await _service.ObtenerPorId(request.Id, true, cancellationToken);
if (pais == null) throw new NotFoundException(...);
Mapper.Map(request, pais);

return await UpdateAuditableEntity(
    pais,
    async () => await _service.Actualizar(pais, cancellationToken),
    () => pais.Id,
    cancellationToken
);
```

---

## ✅ CHECKLIST POST-REFACTORIZACIÓN

Para CADA handler refactorizado:

- [ ] **Usings correctos:** `using Application.Handlers;`, `using Microsoft.Extensions.Logging;`
- [ ] **Herencia correcta:** `: AuditableUpdateHandlerBase<TCommand, TResponse>`
- [ ] **Constructor actualizado:** Recibe `ILogger<T>`, llama `base(mapper, logger)`
- [ ] **Handle override:** `public override async Task<...> Handle(...)`
- [ ] **FechaActualizacion eliminada:** NO hay asignación manual de `FechaActualizacion = DateTime.UtcNow;`
- [ ] **UpdateAuditableEntity usado:** Envuelve la persistencia
- [ ] **Mapper.Map usa Mapper protegido:** NO `_mapper.Map(...)`, sino `Mapper.Map(...)`
- [ ] **Compilación:** `dotnet build` produce 0 errores en este handler
- [ ] **Endpoint funcional:** Se puede ejecutar PUT o PATCH sin errores

---

## 🚀 EJECUCIÓN SECUENCIAL

### Orden recomendado (agrupa por módulo):

1. **Catalogo (16):** ModuloSistema, Moneda, Pais, ParametroSistema, SerieDocumento, TipoComprobante, TipoImpuesto, UnidadMedida
2. **Clientes (2):** Cliente (Actualizar + ActualizarEstado)
3. **Productos (2):** Producto (Actualizar + ActualizarEstado)

### Validación incremental:

- Refactorizar 5 handlers → compilar → probar
- Refactorizar 10 handlers → compilar → probar
- Refactorizar 15 handlers → compilar → probar
- Refactorizar 20 handlers → compilar → probar

---

## 🧪 TESTING POST-REFACTORIZACIÓN

### Módulos a probar (antes de commit):

1. **Catalogo:**
   - Pais: PUT, PATCH inactivar, PATCH activar
   - Moneda: PUT, PATCH inactivar, PATCH activar
   - UnidadMedida: PUT, PATCH inactivar, PATCH activar
   - (Todos los 8 catálogos siguiendo el mismo patrón)

2. **Clientes:**
   - Cliente: PUT, PATCH inactivar, PATCH activar

3. **Productos:**
   - Producto: PUT, PATCH inactivar, PATCH activar

### Validaciones específicas:

- ✅ FechaActualizacion se actualiza a `DateTime.UtcNow` (verificar en BD)
- ✅ ID retorna correctamente (para handlers que retornan int)
- ✅ Unit.Value retorna correctamente (para handlers que retornan Unit)
- ✅ Validaciones de negocio funcionan (no saltadas por cambios de herencia)
- ✅ Logs se escriben correctamente
- ✅ Manejo de excepciones funciona

---

## 📊 MÉTRICAS DE ÉXITO

| Métrica | Target | Actual |
|---------|--------|--------|
| Handlers refactorizados | 20/20 | |
| Compilación limpia | 0 errores | |
| Endpoints probados | 3+ módulos | |
| FechaActualizacion correcta | 100% | |
| Tests unitarios verdes | SÍ | |

---

## 📝 DOCUMENTACIÓN RELACIONADA

- **IA_Docs/AUDITABLE_ENTITY_UPDATE_HANDLERS_PATTERN.md** — Patrón técnico completo
- **IA_Docs/ARCHITECTURE_DECISIONS.md** — ADR-012
- **CLAUDE.md** — Reglas arquitectónicas obligatorias
- **IA_Docs/IMPLEMENTATION_PATTERNS.md** — Patrones generales

---

## 🎬 SIGUIENTE PASO

Una vez completada esta refactorización:

1. Crear commit: `feat(handlers): refactor remaining 20 update handlers to use AuditableUpdateHandlerBase`
2. Crear nuevo documento en `USUARIO_DOCS/avance_XX_2026-06-28.md` resumen de cambios
3. Actualizar `History Changed/` con detalles técnicos
4. Preparar PR desde `catalogo-base/validators` hacia `main`

---

**Aprobado por:** Miguel González Cuevas  
**Fecha aprobación:** 2026-06-28  
**Estado:** LISTO PARA EJECUCIÓN
