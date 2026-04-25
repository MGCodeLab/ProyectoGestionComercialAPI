# 📋 PLAN ACTUALIZADO - Iteración 3: Completar Módulo Cliente

**Fecha:** 2026-04-25 15:15  
**Estado:** ACTUALIZADO (Iteración 2.1 revertida)  
**Dependencias:** ✅ Iteración 1 + ~~Iteración 2~~ + Iteración 2.1 (corrección)

---

## ⚠️ IMPORTANTE - CAMBIO EN SOFT DELETE

**Requisito del negocio (aclaración Miguel):**
- Soft delete es para **trazabilidad**, NO para **ocultamiento**
- TODOS los registros (activos + inactivos) **deben mostrarse** en las listas
- El frontend maneja la presentación visual (scroll, estilos, etc)
- El usuario necesita ver el registro aunque esté "eliminado" para poder reactivarlo

**Impacto en Iter 3:**
```
GET /api/v1/clientes
// Retorna: TODOS los clientes (Activo=true AND Activo=false)
// Frontend decide cómo mostrar

PATCH /api/v1/clientes/{id}/inactivar
// Actualiza: SET Activo=false
// Resultado: Cliente aún aparece en lista, pero marcado como inactivo
```

---

## 🎯 Objetivo

Implementar **CRUD completo para Cliente** con soft delete como patrón de auditoría, no como ocultamiento de datos.

---

## 📂 Estructura de Archivos (SIN CAMBIOS)

```
APPLICATION LAYER:
  ├─ Application/Dtos/Cliente/ (CrearClienteDto, ActualizarClienteDto, ClienteDto)
  ├─ Application/Features/Clientes/ (4 Commands + Handlers + Validators)
  └─ Application/Mappings/Clientes/ (ClienteProfile)

INFRASTRUCTURE LAYER:
  └─ ClienteService (completar CRUD)

API LAYER:
  └─ ClientesController (6 endpoints)
```

---

## 🔄 Endpoints (SIN CAMBIOS)

| Método | Ruta | Acción |
|--------|------|--------|
| GET | `/api/v1/clientes` | **Listar TODOS** (incluye inactivos) |
| GET | `/api/v1/clientes/{id}` | Obtener por ID |
| POST | `/api/v1/clientes` | Crear |
| PUT | `/api/v1/clientes/{id}` | Actualizar |
| PATCH | `/api/v1/clientes/{id}/inactivar` | Soft delete (SET Activo=false) |
| PATCH | `/api/v1/clientes/{id}/activar` | Reactivar |
| DELETE | `/api/v1/clientes/{id}` | Hard delete |

---

## ✅ Validaciones (SIN CAMBIOS)

- NumeroDocumento + TipoDocumentoId = UNIQUE
- Correo = UNIQUE (si se proporciona)
- Email format validation
- MaxLength en campos
- Required fields: TipoDocumentoId, NumeroDocumento, Nombres, ApellidoPaterno

---

## 🏗️  Soft Delete Pattern (CORREGIDO)

### Iter 2 - REVERTIDA ❌
```csharp
// NO USAR: Global filter (hacía que GET retornara solo Activo=true)
AppDbContext.SetQueryFilter(e => e.Activo == true);
```

### Iteración 2.1 - CORRECCIÓN ✅
```csharp
// USAR: Sin filter en AppDbContext
// GET /clientes retorna TODOS
// Frontend controla presentación
```

---

## 🎨 Presentación Visual (Frontend - Responsabilidad del usuario)

El usuario (Miguel) maneja visualmente:
```javascript
// Posibles enfoques en Angular:

// Opción 1: Mostrar todos con indicador visual
clientes.map(c => ({
  ...c,
  cssClass: c.activo ? 'active' : 'inactive-strikethrough'
}))

// Opción 2: Separar en tabs
const activos = clientes.filter(c => c.activo);
const inactivos = clientes.filter(c => !c.activo);

// Opción 3: Scroll/virtualization con filtros
// El usuario menciona: "lo voy a mostrar igual por lo que si tienen 
// que mostrarse si o si el registro, yo visualmente eso lo manejare 
// con un scroll"
```

Backend: Proporciona datos crudos ✅  
Frontend: Decide presentación ✅

---

## 🧪 Testing (ACTUALIZADO)

```csharp
[Fact]
public async Task GetClientes_ReturnsAllRecords()
{
    // Arrange
    var activeCliente = new Cliente { Nombres = "Active", Activo = true };
    var inactiveCliente = new Cliente { Nombres = "Inactive", Activo = false };
    context.Clientes.AddRange(activeCliente, inactiveCliente);
    await context.SaveChangesAsync();

    // Act
    var result = await context.Clientes.ToListAsync();

    // Assert
    Assert.Equal(2, result.Count);  // ✅ AMBOS retornados
    Assert.Contains(result, c => c.Nombres == "Active");
    Assert.Contains(result, c => c.Nombres == "Inactive");
}

[Fact]
public async Task InactivarCliente_UpdatesActivoField()
{
    // Arrange
    var cliente = new Cliente { Nombres = "Test", Activo = true };
    context.Clientes.Add(cliente);
    await context.SaveChangesAsync();

    // Act
    await mediator.Send(new ActualizarEstadoClienteCommand(cliente.Id, false));

    // Assert
    var updated = await context.Clientes.FirstAsync(c => c.Id == cliente.Id);
    Assert.False(updated.Activo);  // ✅ Marked as inactive
    // ✅ Pero aún en BD y visible en queries
}
```

---

## 📝 Commit Message (Iter 3)

```
feat(cliente): complete cliente module with full CRUD

- Create CrearClienteCommand, ActualizarClienteCommand, ActualizarEstadoClienteCommand, EliminarClienteCommand
- Implement handlers + validators for all cliente operations
- Create ClienteDto, CrearClienteDto, ActualizarClienteDto with validations
- Implement ClienteService with full CRUD
- Create ClientesController with endpoints (GET, POST, PUT, PATCH, DELETE)
- Add ClienteProfile for AutoMapper
- GET /clientes returns ALL records (active + inactive)
- PATCH /inactivar soft-deletes (SET Activo=false, still visible)
- Frontend controls visual presentation of inactive records
- Cliente module production-ready with soft delete for auditing

Impact:
- Full CRUD for Cliente matching Producto functionality
- Soft delete pattern: Activo field for traceability, not hiding
- Backend provides complete data, frontend controls UX
```

---

## 🎓 Resumen de Cambios Arquitectónicos

### Iter 1: ✅ COMPLETADA
- Estandarización de AuditableEntity
- Base genérica AuditableEntityConfiguration<T>

### Iter 2: ✅ COMPLETADA + CORREGIDA (2.1)
- ~~Global soft delete filter (INCORRECTO)~~
- ✅ Soft delete pattern sin filtros (CORRECTO)
- ✅ Auditoría completa (Activo, FechaRegistro, etc)
- ✅ Frontend controla presentación

### Iter 3: ⏳ PRÓXIMA
- CRUD completo para Cliente
- Soft delete para trazabilidad
- Todos los registros retornados

---

## ⏱️  TIEMPO ESTIMADO

DTOs + Commands:           30 min
Handlers + Validators:     90 min
AutoMapper + Service:      30 min
Controller:                30 min
Testing + Documentation:   60 min
───────────────────────────────
TOTAL:                     240 min (4 horas)

---

**¿APROBADO PARA PROCEDER CON IMPLEMENTACIÓN?**
