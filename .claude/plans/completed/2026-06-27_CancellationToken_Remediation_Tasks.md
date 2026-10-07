# CancellationToken Homogeneity Remediation Tasks
**Fecha:** 2026-06-27  
**Auditoría:** `.claude/audit/20260627_T1730_CancellationTokenHomogeneityAudit/AUDIT_REPORT.md`  
**Asignado a:** @.claude/agents/Nexus-Fast-Builder.md  
**Estado:** ✅ COMPLETADA  
**Prioridad:** 🔴 CRÍTICA  

---

## OBJETIVO
Remediar falta de homogeneidad en CancellationToken (CT) en Services, Commands, Handlers y Validators para permitir graceful shutdown, timeout handling y mejor escalabilidad.

---

## FASE 1: ORGANIZACION Module Services (CRÍTICO)
**Estimado:** 1.5 horas  
**Prioridad:** 🔴 CRÍTICA

### Task 1.1: SucursalService - Agregar CancellationToken
**Archivo:** `Infrastructure/Repository/SucursalService.cs`

**Métodos a actualizar:**

1. **ObtenerPorId**
   ```csharp
   // Actual:
   public async Task<Sucursal?> ObtenerPorId(int id, bool tracking = false)
   
   // Debe ser:
   public async Task<Sucursal?> ObtenerPorId(int id, bool tracking, CancellationToken cancellationToken)
   ```
   - Agregar `CancellationToken cancellationToken` como último parámetro
   - Propagar CT a `.FirstOrDefaultAsync(..., cancellationToken)`

2. **ObtenerTodos**
   ```csharp
   // Actual:
   public async Task<List<SucursalDto>> ObtenerTodos()
   
   // Debe ser:
   public async Task<List<SucursalDto>> ObtenerTodos(CancellationToken cancellationToken)
   ```
   - Agregar CT como último parámetro
   - Propagar a `.AsNoTracking().ToListAsync(cancellationToken)`

3. **ObtenerTodosOptimizado**
   - Agregar CT como parámetro
   - Propagar a ToListAsync

4. **ObtenerPorIdOptimizado**
   - Agregar CT como parámetro
   - Propagar a FirstOrDefaultAsync

5. **ObtenerCombo**
   ```csharp
   // Actual:
   public async Task<List<SucursalComboDto>> ObtenerCombo()
   
   // Debe ser:
   public async Task<List<SucursalComboDto>> ObtenerCombo(CancellationToken cancellationToken)
   ```
   - Agregar CT
   - Propagar a Select().ToListAsync(cancellationToken)

6. **Crear**
   ```csharp
   // Actual:
   public async Task Crear(Sucursal sucursal)
   
   // Debe ser:
   public async Task Crear(Sucursal sucursal, CancellationToken cancellationToken)
   ```
   - Agregar CT
   - Propagar a SaveChangesAsync(cancellationToken)

7. **Actualizar**
   ```csharp
   // Actual:
   public async Task Actualizar(Sucursal sucursal)
   
   // Debe ser:
   public async Task Actualizar(Sucursal sucursal, CancellationToken cancellationToken)
   ```
   - Agregar CT
   - Propagar a SaveChangesAsync(cancellationToken)

8. **Eliminar**
   ```csharp
   // Actual:
   public async Task Eliminar(int id)
   
   // Debe ser:
   public async Task Eliminar(int id, CancellationToken cancellationToken)
   ```
   - Agregar CT
   - Propagar a SaveChangesAsync(cancellationToken)

---

### Task 1.2: AlmacenService - Agregar CancellationToken
**Archivo:** `Infrastructure/Repository/AlmacenService.cs`

**Métodos a actualizar:** (8 métodos)
- `ObtenerPorId(int id, bool tracking, CancellationToken cancellationToken)`
- `ObtenerTodos(CancellationToken cancellationToken)`
- `ObtenerTodosOptimizado(CancellationToken cancellationToken)`
- `ObtenerPorIdOptimizado(int id, CancellationToken cancellationToken)`
- `ObtenerCombo(CancellationToken cancellationToken)`
- `Crear(Almacen almacen, CancellationToken cancellationToken)`
- `Actualizar(Almacen almacen, CancellationToken cancellationToken)`
- `Eliminar(int id, CancellationToken cancellationToken)`

**Patrón:** Idéntico a SucursalService

---

### Task 1.3: EmpresaService - Agregar CancellationToken
**Archivo:** `Infrastructure/Repository/EmpresaService.cs`

**Métodos a actualizar:** (7 métodos)
- `ObtenerPorId(int id, bool tracking, CancellationToken cancellationToken)`
- `ObtenerPrimera(CancellationToken cancellationToken)`
- `ObtenerTodos(CancellationToken cancellationToken)`
- `ObtenerCombo(CancellationToken cancellationToken)`
- `Crear(Empresa empresa, CancellationToken cancellationToken)`
- `Actualizar(Empresa empresa, CancellationToken cancellationToken)`
- `Eliminar(int id, CancellationToken cancellationToken)`

**Patrón:** Idéntico a SucursalService

---

## FASE 2: CATALOGO Module Services (CRÍTICO)
**Estimado:** 1 hora  
**Prioridad:** 🔴 CRÍTICA

### Task 2.1: CategoriaProductoService - Agregar CancellationToken
**Archivo:** `Infrastructure/Repository/CategoriaProductoService.cs`

**Métodos a actualizar:** (6 métodos)
- `ObtenerTodosAsync(CancellationToken cancellationToken)` - Renombrar también si necesario
- `ObtenerPorIdAsync(int id, bool tracking, CancellationToken cancellationToken)`
- `ObtenerRaicesAsync(CancellationToken cancellationToken)`
- `Crear(CategoriaProducto categoria, CancellationToken cancellationToken)`
- `Actualizar(CategoriaProducto categoria, CancellationToken cancellationToken)`
- `Eliminar(int id, CancellationToken cancellationToken)`

**Nota:** Actualmente usa sufijo `*Async()` en algunos métodos. Mantener consistencia.

---

### Task 2.2: MarcaProductoService - Agregar CancellationToken
**Archivo:** `Infrastructure/Repository/MarcaProductoService.cs`

**Métodos a actualizar:** (5 métodos)
- `ObtenerTodosAsync(CancellationToken cancellationToken)`
- `ObtenerPorIdAsync(int id, bool tracking, CancellationToken cancellationToken)`
- `Crear(MarcaProducto marca, CancellationToken cancellationToken)`
- `Actualizar(MarcaProducto marca, CancellationToken cancellationToken)`
- `Eliminar(int id, CancellationToken cancellationToken)`

**Patrón:** Idéntico a CategoriaProductoService

---

## FASE 3: Handler CT Propagation (CRÍTICO)
**Estimado:** 1.5 horas  
**Prioridad:** 🔴 CRÍTICA

### Task 3.1: ORGANIZACION Module Handlers - Propagar CancellationToken
**Archivos a actualizar:** 12 handlers

#### CrearSucursalHandler
**Archivo:** `Application/Features/Organizacion/Sucursal/Crear/CrearSucursalHandler.cs`

Cambiar todas las llamadas a `_service`:
```csharp
// Actual:
await _service.Crear(sucursal);

// Debe ser:
await _service.Crear(sucursal, cancellationToken);
```

**Métodos a actualizar en handler:**
- `_service.Crear()` → agregar `cancellationToken`

---

#### ActualizarSucursalHandler
**Archivo:** `Application/Features/Organizacion/Sucursal/Actualizar/ActualizarSucursalHandler.cs`

```csharp
// Actual:
var sucursal = await _service.ObtenerPorId(request.Id, true);
await _service.Actualizar(sucursal);

// Debe ser:
var sucursal = await _service.ObtenerPorId(request.Id, true, cancellationToken);
await _service.Actualizar(sucursal, cancellationToken);
```

**Métodos a actualizar:**
- `_service.ObtenerPorId()` → agregar `cancellationToken`
- `_service.Actualizar()` → agregar `cancellationToken`

---

#### EliminarSucursalHandler
**Archivo:** `Application/Features/Organizacion/Sucursal/Eliminar/EliminarSucursalHandler.cs`

```csharp
// Actual:
await _service.Eliminar(request.Id);

// Debe ser:
await _service.Eliminar(request.Id, cancellationToken);
```

---

#### ActualizarEstadoSucursalHandler
**Archivo:** `Application/Features/Organizacion/Sucursal/ActualizarEstado/ActualizarEstadoSucursalHandler.cs`

Propagar CT en todas las llamadas a service methods.

---

#### CrearAlmacenHandler, ActualizarAlmacenHandler, EliminarAlmacenHandler, ActualizarEstadoAlmacenHandler
**Archivo:** `Application/Features/Organizacion/Almacen/{Crear,Actualizar,Eliminar,ActualizarEstado}/`

**Patrón:** Idéntico a Sucursal handlers

---

#### CrearEmpresaHandler, ActualizarEmpresaHandler, EliminarEmpresaHandler, ActualizarEstadoEmpresaHandler
**Archivo:** `Application/Features/Organizacion/Empresa/{Crear,Actualizar,Eliminar,ActualizarEstado}/`

**Patrón:** Idéntico a Sucursal handlers

---

### Task 3.2: CATALOGO Module Handlers - Propagar CancellationToken
**Archivos a actualizar:** CategoriaProducto y MarcaProducto handlers

#### CrearCategoriaProductoHandler, ActualizarCategoriaProductoHandler, ActualizarEstadoCategoriaProductoHandler, EliminarCategoriaProductoHandler
**Archivo:** `Application/Features/Catalogo/CategoriaProducto/{Crear,Actualizar,ActualizarEstado,Eliminar}/`

```csharp
// Actual:
await _service.Crear(categoria);

// Debe ser:
await _service.Crear(categoria, cancellationToken);
```

**Patrón:** Propagar CT en todas las llamadas a `_service.*`

---

#### CrearMarcaProductoHandler, ActualizarMarcaProductoHandler, ActualizarEstadoMarcaProductoHandler, EliminarMarcaProductoHandler
**Archivo:** `Application/Features/Catalogo/MarcaProducto/{Crear,Actualizar,ActualizarEstado,Eliminar}/`

**Patrón:** Idéntico a CategoriaProductoHandler

---

## FASE 4: Validación Final (30 minutos)
**Estimado:** 30 minutos

### Task 4.1: Compilación
```bash
dotnet build
# Debe resultar en 0 errores, 0 advertencias
```

### Task 4.2: Verificar Patrón Consistente
- [ ] Todos los servicios tienen CT en todos los métodos públicos async
- [ ] Todos los handlers propagan CT a todas las llamadas de service
- [ ] Todos los `.FirstOrDefaultAsync()`, `.ToListAsync()`, `.SaveChangesAsync()` reciben CT
- [ ] No hay métodos async sin CT (excepto JwtService que es utility)

---

## CHECKLIST DE REMEDIATION

### FASE 1: ORGANIZACION Services
- [x] SucursalService: 8 métodos actualizados + CT propagado ✅ (Completada 2026-06-27)
- [x] AlmacenService: 8 métodos actualizados + CT propagado ✅ (Completada 2026-06-27)
- [x] EmpresaService: 7 métodos actualizados + CT propagado ✅ (Completada 2026-06-27)
- [x] Compilación sin errores ✅ (Commit f590d37)

### FASE 2: CATALOGO Services
- [x] CategoriaProductoService: 6 métodos actualizados + CT propagado ✅ (Completada 2026-06-27)
- [x] MarcaProductoService: 5 métodos actualizados + CT propagado ✅ (Completada 2026-06-27)
- [x] Compilación sin errores ✅ (Commit 5c7473c)

### FASE 3: Handlers CT Propagation
- [x] 12 ORGANIZACION handlers: CT propagado ✅ (Completada 2026-06-27)
- [x] 8 CATALOGO handlers (CategoriaProducto + MarcaProducto): CT propagado ✅ (Completada 2026-06-27)
- [x] 5 Controllers actualizado con CT ✅ (Completada 2026-06-27)
- [x] Compilación sin errores ✅ (Verificado 2026-06-27)

### FASE 4: Validación
- [x] `dotnet build` → 0 errores ✅ (Verificado 2026-06-27)
- [x] Patrón consistente en todo el codebase ✅ (Verified 2026-06-27)
- [x] Todos los métodos async tienen CT ✅ (Spot-check passed 2026-06-27)

---

## DETALLES IMPORTANTES

### Patrón de parámetro CancellationToken

**Posición:** Siempre como ÚLTIMO parámetro
```csharp
public async Task<T> Metodo(param1, param2, CancellationToken cancellationToken)
```

**En métodos ObtenerPorId con `bool isAsTracking`:**
```csharp
public async Task<T?> ObtenerPorId(int id, bool isAsTracking, CancellationToken cancellationToken)
```

### Propagación en EF Core

**Para queries:**
```csharp
await _context.Tabla.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
await _context.Tabla.ToListAsync(cancellationToken)
```

**Para cambios:**
```csharp
await _context.SaveChangesAsync(cancellationToken)
```

### Propagar en servicios

Si un servicio llama a otro servicio:
```csharp
public async Task MetodoA(CancellationToken cancellationToken)
{
    // Propagar a otro servicio
    await _otroService.Metodo(cancellationToken);
}
```

---

## RIESGOS Y MITIGACIONES

### Riesgo 1: Breaking Changes en Interfaces
**Mitigación:** Actualizar todas las implementaciones que usan estos servicios (handlers, validators, etc.)

### Riesgo 2: Tests fallando
**Mitigación:** Si hay unit tests, actualizar para pasar CT (usar `CancellationToken.None` en tests)

### Riesgo 3: Olvidar propagar en un handler
**Mitigación:** Después de actualizar services, verificar cada handler manualmente

---

## ORDEN DE EJECUCIÓN RECOMENDADO

1. **Primero:** Actualizar Services (FASE 1 + FASE 2)
   - SucursalService, AlmacenService, EmpresaService
   - CategoriaProductoService, MarcaProductoService
   
2. **Segundo:** Compilar para detectar errores tempranos
   - Esto identificará qué handlers rompió

3. **Tercero:** Actualizar Handlers (FASE 3)
   - ORGANIZACION handlers (12)
   - CATALOGO handlers (8)

4. **Cuarto:** Validación final (FASE 4)
   - Compilación limpia
   - Revisar patrón consistente

---

## NOTAS ESPECIALES

### JwtService
No necesita actualización (métodos sincrónicos, son utilities).

### Validators
La mayoría ya implementan CT. Revisar si necesitan actualizaciones adicionales en métodos que llamen a services.

### Controllers
No necesitan actualización. MediatR maneja CT automáticamente en pipeline.

---

## RESULTADO ESPERADO

Después de completar todas las fases:
- ✅ 100% de Services async tienen CancellationToken
- ✅ 100% de Handlers propagan CancellationToken
- ✅ Patrón homogéneo en todo el proyecto
- ✅ Graceful shutdown funcional
- ✅ Timeout handling a nivel de DB
- ✅ Compilación sin errores

---

## DOCUMENTACIÓN POST-REMEDIACIÓN

Después de completar:
1. Crear commit con mensaje:
   ```
   feat(cancellation-token): implementar homogeneidad de CancellationToken en Services y Handlers
   
   - Agregar CT a 20 métodos de Services en ORGANIZACION y CATALOGO
   - Propagar CT en 20 handlers en ORGANIZACION y CATALOGO
   - Validación: dotnet build sin errores
   ```

2. Crear History Changed:
   ```
   20260627_T1800_feat_CancellationTokenHomogeneity/
   ```

3. Actualizar `.claude/execution-status/` con estado final

---

**Asignado a:** Nexus-Fast-Builder  
**Prioridad:** 🔴 CRÍTICA  
**Tiempo Estimado:** 4-5 horas (3 fases + validación)  
**Estado:** PENDIENTE DE EJECUCIÓN  

---

*Auditoría original: `.claude/audit/20260627_T1730_CancellationTokenHomogeneityAudit/AUDIT_REPORT.md`*
