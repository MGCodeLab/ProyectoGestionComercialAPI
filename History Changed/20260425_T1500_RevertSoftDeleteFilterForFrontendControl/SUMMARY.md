# Corrección Arquitectónica: Revertir Global Soft Delete Filter

**Fecha:** 2026-04-25 15:00  
**Tipo:** Architecture Correction  
**Rama:** Modulos/Cliente_01  
**Referencia:** Iteración 2 (revertida parcialmente)

---

## 📋 Qué cambió

### Removido:
- ✅ `ApplySoftDeleteFilters()` method de AppDbContext
- ✅ Global `HasQueryFilter()` application
- ✅ `Infrastructure/Persistence/Extensions/SoftDeleteQueryExtensions.cs`

### Mantenido:
- ✅ `Activo` field en todas las AuditableEntity (para trazabilidad)
- ✅ Soft delete como patrón (SET Activo = false)
- ✅ Auditoría completa (FechaRegistro, FechaActualizacion, PublicId)

---

## 🎯 Por qué

### Requisito del Negocio (Aclaración Miguel):
1. **Soft delete es para trazabilidad, no ocultamiento**
2. **TODOS los registros (activos + inactivos) deben mostrarse en listas**
3. **Frontend maneja visualmente los inactivos** (con scroll, estilos, etc)
4. **Usuario necesita ver el registro aunque esté "eliminado"** para poder reactivarlo o manejar el flujo completo

### Problema con Iter 2:
La implementación de global filter hacía que:
- ❌ `GET /productos` solo retornara Activo=true
- ❌ Registros inactivos eran **ocultos automáticamente** por EF Core
- ❌ Frontend no podía mostrar ni controlar inactivos
- ❌ No alineado con requisito del negocio

### Solución Correcta:
- ✅ `GET /productos` retorna TODOS (activos + inactivos)
- ✅ Frontend decide cómo presentar visualmente
- ✅ `Activo` field mantiene trazabilidad
- ✅ Soft delete sigue siendo patrón de auditoría
- ✅ Flexible: frontend puede filtrar o no según necesidad

---

## 🔄 Cambio en Comportamiento de Queries

### Antes (Iter 2 - INCORRECTO):
```csharp
var productos = await context.Productos.ToListAsync();
// Retorna: SOLO activos (Activo=true)
// SQL: SELECT ... WHERE Activo = 1
// ❌ Frontend no ve los inactivos
```

### Después (CORRECTO):
```csharp
var productos = await context.Productos.ToListAsync();
// Retorna: TODOS (activos + inactivos)
// SQL: SELECT ... (sin WHERE Activo)
// ✅ Frontend decide cómo mostrar
```

---

## 🏗️ Arquitectura Resultante

```
SOFT DELETE PATTERN (Correcto):
├─ Domain: Activo field (auditoría)
├─ Infrastructure: SET Activo = false (soft delete)
├─ Application: Sin filtros automáticos
└─ API: Retorna TODOS, frontend elige presentación

NO ES:
├─ ❌ Invisible hiding en BD
├─ ❌ Automatic filtering en queries
├─ ❌ Backend impone presentación visual
```

---

## 📊 Impacto en Iteración 3

### Cliente CRUD - Comportamiento esperado:

**GET /api/v1/clientes**
```json
[
  { "id": 1, "nombre": "Juan", "activo": true },    // ✅ Visible
  { "id": 2, "nombre": "María", "activo": false },   // ✅ También visible
  { "id": 3, "nombre": "Carlos", "activo": true }    // ✅ Visible
]
```

Frontend decide:
- Mostrar todos (default)
- Filtrar solo activos
- Mostrar separados (tabs, colores, etc)
- User experience: scroll, visual indicators

**PATCH /api/v1/clientes/{id}/inactivar**
```
Request: { id: 2 }
Action: SET Activo = false
Result: Cliente aún aparece en lista (pero con Activo=false)
```

---

## ✅ Beneficios de esta Arquitectura

### Para Backend:
- ✅ Auditoría completa (nunca se pierden datos)
- ✅ Trazabilidad (FechaActualizacion, Activo, PublicId)
- ✅ Sin lógica de presentación
- ✅ Flexible para múltiples frontends

### Para Frontend:
- ✅ Control total sobre presentación
- ✅ Datos completos (no filtering oculto)
- ✅ UX flexibility (puede mostrar/ocultar según contexto)
- ✅ Reactivación: registros aún disponibles

### Para Negocio:
- ✅ Cumplimiento legal (auditoría completa)
- ✅ Recuperación de datos (soft delete reversible)
- ✅ Experiencia usuario mejorada
- ✅ Control en presentación, no en datos

---

## 🚨 Cambios en Iteración 2

**Nota:** Iter 2 ahora es solo documentación histórica.

La implementación de AppDbContext en Iter 2 ha sido **revertida**, pero:
- **KEEP:** Conceptual entendimiento de global filters
- **REMOVE:** Implementación práctica en este proyecto
- **KEEP:** Auditoría + soft delete flag como patrón

---

## 📝 Comparativa: Soft Delete Patterns

### Patrón 1: Backend-Hidden (❌ NO USAR - Iter 2)
```
DB: DELETE (soft)
EF: Global filter
API: Retorna solo activos
Frontend: No ve inactivos
```

### Patrón 2: Frontend-Controlled (✅ USAR - Este proyecto)
```
DB: UPDATE Activo=false (soft)
EF: Sin filtros
API: Retorna todos
Frontend: Controla presentación
```

Este proyecto implementa **Patrón 2** (correcto para tu caso).

---

## 🔧 Testing

### Verificar comportamiento:

```csharp
[Fact]
public async Task GetProductos_ReturnsBothActiveAndInactive()
{
    // Arrange
    var activeProduct = new Producto { Nombre = "Active", Activo = true };
    var inactiveProduct = new Producto { Nombre = "Inactive", Activo = false };
    context.Productos.AddRange(activeProduct, inactiveProduct);
    await context.SaveChangesAsync();

    // Act
    var result = await context.Productos.ToListAsync();

    // Assert
    Assert.Equal(2, result.Count);  // ✅ AMBOS
    Assert.Contains(result, p => p.Nombre == "Active");
    Assert.Contains(result, p => p.Nombre == "Inactive");
}
```

---

## 📋 Proximos Pasos

### Iteración 3 (Completar Cliente):
- ✅ Crear CRUD completo
- ✅ GET /clientes retorna TODOS (activos + inactivos)
- ✅ PATCH /inactivar solo actualiza Activo=false
- ✅ Frontend maneja presentación visual

---

## 🎓 Lección Aprendida

**Soft Delete tiene dos enfoques:**

1. **Backend-Hidden:** DB oculta inactivos automáticamente
   - Pros: Backend simple
   - Cons: Frontend limitado, menos flexible

2. **Frontend-Controlled:** Backend expone todos, frontend decide
   - Pros: Flexible, auditable, mejor UX
   - Cons: Requiere frontend inteligente ✅ (Angular ready)

**Nexus-ERP implementa opción 2** (correcta para tu caso).

---

**Estado:** ✅ **LISTO PARA COMMIT**

Build Status: ✅ Exitoso (0 errores, 0 advertencias)

Commit message:
```
fix(architecture): remove global soft delete filter for frontend control

BREAKING CHANGE: Revert global HasQueryFilter() from AppDbContext

- Remove ApplySoftDeleteFilters() method
- Remove SoftDeleteQueryExtensions (not needed)
- Queries now return ALL records (Activo=true and Activo=false)
- Frontend handles visual presentation and filtering
- Soft delete remains as audit/traceability pattern (SET Activo=false)

Rationale:
- Soft delete is for traceability, not data hiding
- Frontend needs complete data to control presentation
- User must see inactive records to reactivate or manage flow
- Backend should not impose presentation logic

Impact:
- GET /productos, /clientes, etc now return all records
- Frontend can filter, hide, style inactive records as needed
- Auditing remains complete (Activo, FechaActualizacion, PublicId)
- Enables flexible UX without backend changes

This aligns with business requirement: soft delete for record management, not hiding.
```
