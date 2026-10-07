# Nexus ERP Backend - Issues por Módulo (Análisis Visual)

**Fecha:** 2026-06-27  
**Total Módulos:** 19  
**Módulos OK:** 8 (42%)  
**Módulos con Issues:** 11 (58%)

---

## MAPA DE ISSUES POR MÓDULO

### CATALOGO (13 módulos)

```
CategoriaProducto           [████████████████] CRÍTICO (4 issues)
├─ CrearHandler: sin mapper
├─ ActualizarHandler: sin mapper + sin FechaActualizacion + sin logger
├─ Todos handlers: sin ILogger
└─ ActualizarEstado/Eliminar: sin logger

MarcaProducto               [████████████████] CRÍTICO (4 issues)
├─ CrearHandler: sin mapper
├─ ActualizarHandler: sin mapper + sin FechaActualizacion
├─ Todos handlers: sin ILogger
└─ ActualizarEstado/Eliminar: sin logger

TipoDocumento               [█████████████] ALTO/CRÍTICO (3 issues)
├─ ActualizarHandler: sin mapper + sin FechaActualizacion
├─ Profile: falta .ReverseMap() en Actualizar
└─ OK: Command, Validator, Entity, Config

ParametroSistema            [██████████] ALTO (2 issues)
├─ Profile: falta mappings de Actualizar
├─ Service: ObtenerPorId ignora isAsTracking
└─ OK: Handlers, Commands, Validators

ListaPrecio                 [█████] CRÍTICO (1 issue)
├─ ActualizarHandler: sin FechaActualizacion
└─ OK: Mapper, Logger, Service

CondicionPago               [██████] ALTO/CRÍTICO (2 issues)
├─ ActualizarHandler: sin FechaActualizacion
├─ Service: ObtenerTodos sin .AsNoTracking()
└─ OK: Commands, Validators

Moneda                      [█████] ALTO (1 issue)
├─ Service: ObtenerPorId ignora isAsTracking
└─ OK: Commands, Handlers, Validators, Profile, Controller

UnidadMedida                [█████] ALTO (1 issue)
├─ Profile: Actualizar sin .ReverseMap()
└─ OK: Handlers, Service, Validators

ModuloSistema               [█████] ALTO (1 issue)
├─ Service: ObtenerPorId ignora isAsTracking
└─ OK: Commands, Handlers, Validators

TipoComprobante             [██████████████████] ✅ OK (0 issues)
├─ Todas las capas correctas
└─ Puede servir como referencia

TipoImpuesto                [██████████████████] ✅ OK (0 issues)
├─ Todas las capas correctas
└─ Puede servir como referencia

SerieDocumento              [███████████] OK (1 issue)
├─ ObtenerProximoNumero: sin logger
└─ OK: CRUD handlers y validadores

Pais                        [██████████████████] ✅ OK (0 issues)
├─ Validador service pattern implementado correctamente
└─ Excelente ejemplo de validación async

Producto                    [██████████████████] ✅ OK (0 issues)
└─ Todas las capas correctas
```

---

### ORGANIZACION (3 módulos)

```
Almacen                     [█████] CRÍTICO (1 issue)
├─ ActualizarHandler: sin FechaActualizacion
└─ OK: Commands, Handlers, Profile

Empresa                     [█████] CRÍTICO (1 issue)
├─ ActualizarHandler: sin FechaActualizacion
└─ OK: Commands, Handlers, Profile

Sucursal                    [█████] CRÍTICO (1 issue)
├─ ActualizarHandler: sin FechaActualizacion
└─ OK: Commands, Handlers, Profile
```

---

### COMERCIAL (1 módulo)

```
Proveedor                   [█████] CRÍTICO (1 issue)
├─ ActualizarHandler: sin FechaActualizacion
└─ OK: Commands, Handlers, Validators, Service
```

---

### CLIENTES (1 módulo)

```
Cliente                     [██████████████████] ✅ OK (0 issues)
└─ Estructura completa y correcta
```

---

### PRODUCTOS (1 módulo)

```
Producto                    [██████████████████] ✅ OK (0 issues)
└─ Estructura completa y correcta
```

---

## RESUMEN POR TIPO DE ISSUE

### FechaActualizacion Faltante (9 handlers)

```
┌────────────────────────────────┬────────┬─────────────┐
│ Módulo                         │ Handler│ Acción      │
├────────────────────────────────┼────────┼─────────────┤
│ CategoriaProducto              │ Actualizar│ Agregar │
│ CondicionPago                  │ Actualizar│ Agregar │
│ ListaPrecio                    │ Actualizar│ Agregar │
│ MarcaProducto                  │ Actualizar│ Agregar │
│ TipoDocumento                  │ Actualizar│ Agregar │
│ Proveedor                      │ Actualizar│ Agregar │
│ Almacen                        │ Actualizar│ Agregar │
│ Empresa                        │ Actualizar│ Agregar │
│ Sucursal                       │ Actualizar│ Agregar │
└────────────────────────────────┴────────┴─────────────┘

PATRÓN OBLIGATORIO:
entity.FechaActualizacion = DateTime.UtcNow;
```

---

### Sin IMapper (mapeo manual) - 5 handlers

```
┌────────────────────────────────┬─────────────┬─────────────────────┐
│ Módulo                         │ Handler     │ Problema            │
├────────────────────────────────┼─────────────┼─────────────────────┤
│ CategoriaProducto              │ Crear       │ new CategoriaProducto{}│
│ CategoriaProducto              │ Actualizar  │ asignaciones manuales│
│ MarcaProducto                  │ Crear       │ new MarcaProducto{} │
│ MarcaProducto                  │ Actualizar  │ asignaciones manuales│
│ TipoDocumento                  │ Actualizar  │ asignaciones manuales│
└────────────────────────────────┴─────────────┴─────────────────────┘

PATRÓN OBLIGATORIO:
_mapper.Map<Entity>(command)     // Crear
_mapper.Map(request, entity)     // Actualizar
```

---

### Sin ILogger - 9 handlers

```
CategoriaProducto:
├─ CrearCategoriaProductoHandler
├─ ActualizarCategoriaProductoHandler
├─ ActualizarEstadoCategoriaProductoHandler
└─ EliminarCategoriaProductoHandler

MarcaProducto:
├─ CrearMarcaProductoHandler
├─ ActualizarMarcaProductoHandler
├─ ActualizarEstadoMarcaProductoHandler
└─ EliminarMarcaProductoHandler

SerieDocumento:
└─ ObtenerProximoNumeroHandler

PATRÓN OBLIGATORIO:
private readonly ILogger<{HandlerType}> _logger;
_logger.LogInformation("Mensaje aquí");
```

---

### AutoMapper Profiles Incompletos - 3 profiles

```
┌────────────────────────┬──────────────────────────────────────┐
│ Profile                │ Faltante                             │
├────────────────────────┼──────────────────────────────────────┤
│ ParametroSistema       │ CreateMap<ActualizarDto, Command>()  │
│                        │ CreateMap<ActualizarCommand, Entity> │
│                        │   .ReverseMap()                      │
├────────────────────────┼──────────────────────────────────────┤
│ TipoDocumento          │ CreateMap<ActualizarCommand, Entity> │
│                        │   .ReverseMap()                      │
├────────────────────────┼──────────────────────────────────────┤
│ UnidadMedida           │ Cambiar:                             │
│                        │ CreateMap<ActualizarCommand, Entity> │
│                        │ Por: .ReverseMap()                   │
└────────────────────────┴──────────────────────────────────────┘
```

---

### ObtenerPorId No Respeta isAsTracking - 4 services

```
┌────────────────────────────────┬──────────────────────────────┐
│ Service                        │ Estado                       │
├────────────────────────────────┼──────────────────────────────┤
│ MonedaService                  │ Ignora isAsTracking          │
│ ModuloSistemaService           │ Ignora isAsTracking          │
│ ParametroSistemaService        │ Ignora isAsTracking (minificada)│
│ REFERENCIA CORRECTA:           │                              │
│ TipoDocumentoService           │ Implementación correcta ✅   │
└────────────────────────────────┴──────────────────────────────┘

PATRÓN OBLIGATORIO:
=> isAsTracking 
    ? await _context.{Entity}.FirstOrDefaultAsync(...)
    : await _context.{Entity}.AsNoTracking().FirstOrDefaultAsync(...);
```

---

### ObtenerTodos Sin AsNoTracking - 1 service

```
┌────────────────────────────────┬──────────────────────────────┐
│ Service                        │ Problema                     │
├────────────────────────────────┼──────────────────────────────┤
│ CondicionPagoService           │ Falta .AsNoTracking()        │
└────────────────────────────────┴──────────────────────────────┘

CAMBIAR:
await _context.CondicionesPago.ToListAsync(token);

POR:
await _context.CondicionesPago.AsNoTracking().ToListAsync(token);
```

---

## MATRIZ DE MÓDULOS vs ISSUES

```
                           FechaAct  Mapper  Logger  Profile AsNoTrack TOTAL
CategoriaProducto            ✓         ✓       ✓                        4
MarcaProducto                ✓         ✓       ✓                        4
TipoDocumento                ✓         ✓               ✓                 3
ParametroSistema                               ✓       ✓        ✓       2
ListaPrecio                  ✓                                          1
CondicionPago                ✓                                ✓        2
Moneda                                                         ✓       1
UnidadMedida                                         ✓                 1
ModuloSistema                                                ✓       1
Almacen                      ✓                                          1
Empresa                      ✓                                          1
Sucursal                     ✓                                          1
Proveedor                    ✓                                          1
SerieDocumento                       ✓                                 1
─────────────────────────────────────────────────────────────────────────
TipoComprobante                                                        0 ✅
TipoImpuesto                                                           0 ✅
Pais                                                                   0 ✅
Producto                                                               0 ✅
Cliente                                                                0 ✅
```

---

## REFERENCIA DE MÓDULOS LIMPIOS (Para usar como ejemplo)

### ✅ TipoComprobante (EXCELENTE)
- Ubicación: `Application/Features/Catalogo/TipoComprobante/`
- Qué está bien:
  - ✅ Validators completos
  - ✅ Handlers con IMapper y ILogger
  - ✅ FechaActualizacion en Actualizar
  - ✅ AutoMapper Profile bidireccional
  - ✅ Service con AsNoTracking correcto

### ✅ Pais (EXCELENTE)
- Ubicación: `Application/Features/Catalogo/Pais/`
- Qué está bien:
  - ✅ Implementa ValidatorService Pattern correctamente
  - ✅ Validación async sin violar Clean Architecture
  - ✅ Todos los handlers correctos
  - ✅ Profile completo

### ✅ TipoDocumento (REFERENCIA parcial)
- Ubicación: `Application/Features/Catalogo/TipoDocumento/`
- Qué está bien (excepto 2 issues):
  - ✅ Validators
  - ✅ Service implementation
  - ⚠️  Handler Actualizar (falta mapper y FechaActualizacion)
  - ⚠️  Profile (falta .ReverseMap())

### ✅ Cliente (LIMPIO)
- Ubicación: `Application/Features/Clientes/Cliente/`
- Qué está bien:
  - ✅ Estructura completa
  - ✅ Todos los handlers
  - ✅ Validators

### ✅ Producto (LIMPIO)
- Ubicación: `Application/Features/Productos/Producto/`
- Qué está bien:
  - ✅ Estructura completa
  - ✅ Todos los handlers
  - ✅ Validators

---

## INDICADORES DE CALIDAD

```
Patrón Crítico: entity.FechaActualizacion = DateTime.UtcNow;
├─ Implementado:  10/19 módulos (53%)
└─ Faltante:       9/19 módulos (47%) ❌ CRÍTICO

Patrón Crítico: ILogger en handlers
├─ Implementado:  10/19 módulos (53%)
└─ Faltante:       9/19 módulos (47%) ❌ CRÍTICO

Patrón Crítico: _mapper.Map() en handlers
├─ Implementado:  17/19 módulos (89%)
└─ Manual:         2/19 módulos (11%) ❌ CRÍTICO

Patrón Crítico: AutoMapper bidireccional
├─ Completo:     16/19 módulos (84%)
└─ Incompleto:    3/19 módulos (16%) ❌ CRÍTICO

Patrón Alto: .AsNoTracking() en queries
├─ Implementado:  18/19 módulos (95%)
└─ Faltante:       1/19 módulos (5%) ❌ ALTO
```

---

## PRÓXIMOS PASOS

### FASE 1: CRÍTICO
1. Agregar FechaActualizacion (9 handlers) - 30 min
2. Reemplazar mapeo manual (5 handlers) - 45 min
3. Completar Profiles (3 archivos) - 30 min
4. Agregar ILogger (9 handlers) - 45 min
5. Test y compilación - 30 min
**Total: 2.5 horas**

### FASE 2: ALTO
1. Corregir ObtenerPorId (4 services) - 30 min
2. Agregar AsNoTracking (1 service) - 15 min
3. Test - 15 min
**Total: 1 hora**

### FASE 3: MEDIO
1. Refactorizar Controller - 15 min
2. Reformatear ParametroSistemaService - 15 min
3. Test - 15 min
**Total: 45 min**

**TIEMPO TOTAL: 5-7 horas**

---

**Documento generado:** 2026-06-27  
**Auditor:** Nexus-Backend-Architect
