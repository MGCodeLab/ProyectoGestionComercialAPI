# Technical Backlog & Pending Decisions — Nexus-ERP

**Última actualización:** 2026-05-18 (Sprint 4 completado)  
**Tipo:** Backlog técnico + arquitectónico + funcional  
**Estado:** Sprint 1-4 ✅ completados — Sprint 5 próximo — Decisión pendiente: PD-02 (ListaPrecioDetalle)

---

## ✅ SPRINT 4 — COMPLETADO (2026-05-18)

**Estado:** ✅ Todas las decisiones del backlog para Sprint 4 implementadas

**Decisiones ejecutadas:**
- ✅ PD-05: ALTER TABLE Productos Migration Strategy — IMPLEMENTADA
  - Decisión: Columnas nullable + script idempotente con IF NOT EXISTS
  - Razón: Migración segura, productos existentes no afectados
  - Implementación: 15_AddProductoFKs.sql ejecutado exitosamente
  - Resultado: ✅ 3 FKs (UnidadMedidaId, CategoriaProductoId, MarcaProductoId) agregadas sin romper datos

**Hallazgos nuevos (descubiertos durante Sprint 4):**
- **PD-15:** CQRS Commands Missing DTO Fields
  - Problema: `ActualizarProductoCommand` record faltaban 3 parámetros que sí tenía el DTO
  - Síntoma: AutoMapper silently pierde datos cuando Command record no tiene los parámetros
  - Solución: Sincronizar parámetros Command ↔ DTO + explicit ForMember mappings
  - Impacto: CRÍTICO — AutoMapper no valida en compilación
  - Documentación: COMMON_ISSUES_AND_FIXES.md sección 11
  - Aplicación: TODOS los Commands deben matchear estructura DTO

**Problemas documentados:** 3 (SQL syntax, script numbering, CQRS sync)

---

## ✅ SPRINT 3 — COMPLETADO (2026-05-17)

**Estado:** ✅ Todas las decisiones del backlog para Sprint 3 implementadas

**Decisiones ejecutadas:**
- ✅ PD-04: SerieDocumento Concurrency Strategy — IMPLEMENTADA
  - Decisión: SERIALIZABLE transaction + ROWLOCK + UPDLOCK (más seguro que solo ROWLOCK)
  - Razón: Máxima seguridad para operación crítica
  - Implementación: ObtenerProximoNumeroHandler con transacción atómica
  - Resultado: ✅ Concurrencia segura sin duplicados

**Hallazgos nuevos (descubiertos durante Sprint 3):**
- **PD-13:** SQL Server Syntax Compatibility
  - Problema: `ON DELETE RESTRICT` no soportado en SQL Server
  - Solución: `ON DELETE NO ACTION` (equivalente)
  - Impacto: Documentado en IA_Docs/SQL_SERVER_COMPATIBILITY.md
  - Aplicación: Próximos sprints deben usar NO ACTION

- **PD-14:** FromSqlInterpolated Materialization Pattern
  - Problema: `FromSqlInterpolated(...).FirstOrDefaultAsync()` falla con UPDATE
  - Solución: `FromSqlInterpolated(...).ToListAsync().FirstOrDefault()`
  - Impacto: Pattern debe usarse en cualquier handler con raw SQL compl
  - Documentación: COMMON_ISSUES_AND_FIXES.md sección 10

**Problemas documentados:** 7 (secciones 8-10 COMMON_ISSUES_AND_FIXES.md)

---

## 🔴 CRÍTICO (Bloqueante para próximas fases)

### ✅ PD-01: TipoDocumentoEnum — RESUELTO
**Prioridad:** CRÍTICO  
**Estado:** ✅ RESUELTO (2026-05-16)  

**Verificación completada por:** Miguel Gonzalez

**IDs Correctos en BD (catalogo.TipoDocumentos):**
```
Id  | Codigo   | Descripcion
----|----------|-------------------------------------
 3  | CE       | Carnet de Extranjeria
 4  | DNI      | Documento Nacional de Identidad
 5  | RUC      | Registro Único del Contribuyente
 6  | PASSPORT | Pasaporte
```

**Impacto:** RESUELTO — IDs verificados, no hay mismatch

**Acción tomada:**
- ✅ Tabla `catalogo.TipoDocumentos` existe en BD
- ✅ IDs auditados: CE=3, DNI=4, RUC=5, PASSPORT=6
- ✅ Sin enum problemático detectado
- ✅ Seguro para Sprint 2 (Empresa, Cliente, Proveedor)

**Próximos pasos:**
- ✅ Usar directamente los IDs: 3, 4, 5, 6
- ✅ No necesita refactorización
- ✅ Documentar en code si hay enum

**Bloqueante para Sprint 2:** ❌ NO — Resuelto

---

### PD-02: ListaPrecioDetalle Implementation
**Prioridad:** CRÍTICO  
**Contexto:**  
`ListaPrecio` será creada en Sprint 5 como catálogo. Pero los precios de cada producto por lista deben ir en tabla `ListaPrecioDetalle(ListaPrecioId, ProductoId, Precio)`.

**Decisión pendiente:**  
¿Crear `ListaPrecioDetalle` en Sprint 5 o deferir a módulo Ventas?

**Impacto:** 
- Si ahora: +1 tabla, +1 controller con CRUD
- Si deferred: Bloquea pricing en Ventas

**Recomendación:** Crear en Sprint 5 (completa catálogos)

**Próximos pasos:**  
- [ ] Confirmar incluir ListaPrecioDetalle en Sprint 5 plan
- [ ] Implementar CRUD completo
- [ ] Seed: 1 precio por producto en lista default

**Responsable:** Miguel (decisión)

---

## 🟡 ALTO (Importante, Sprint 2 — Decisión Requerida)

### PD-02.5: SingleTenant Guard en Empresa
**Prioridad:** 🟡 ALTO  
**Contexto:**  
Sprint 2 creará tabla `organizacion.Empresas`. Decisión arquitectónica: ¿cómo enforcar que solo 1 empresa exista?

**Opciones:**

**Opción A (Recomendado): Application-Level Guard**
```csharp
// En CrearEmpresaHandler
var empresaExistente = await _empresaService.ObtenerPrimera();
if (empresaExistente != null)
    throw new InvalidOperationException("Solo 1 empresa permitida en sistema");

// Crear empresa
```

**Ventajas:**
- ✅ Flexible: si cambias a multi-tenant después, solo cambias Application logic
- ✅ No requiere constraint en BD
- ✅ Preparado para evolucionar
- ✅ Controlado por código, no por BD

**Opción B: Database Constraint**
```sql
ALTER TABLE organizacion.Empresas
ADD CONSTRAINT UQ_Empresas_Single_Record
UNIQUE (Id) WHERE Id = 1;
```

**Problema:** Endurecería la BD y es más difícil cambiar si necesitas multi-tenant.

**Decisión:** ✅ OPCIÓN A APROBADA (2026-05-16)

**Aprobado por:** Miguel Gonzalez

**Implementación:** Application-Level Guard en CrearEmpresaHandler

**Tiempo implementación:** <30 min en handler

**Responsable:** Nexus-Fast-Builder (Sprint 2)

**Status:** ✅ LISTO PARA IMPLEMENTAR

---

## 🟡 ALTO (Importante, pero no bloqueante inmediatamente)

### ✅ PD-03: Smoke Testing de Sprint 1 — COMPLETADO
**Prioridad:** ALTO  
**Estado:** ✅ COMPLETADO (2026-05-16)  

**Testing ejecutado por:** Miguel Gonzalez  
**Herramienta:** Postman  
**Fecha:** 2026-05-16  

**Endpoints validados:**
- ✅ GET /api/v1/paises → 200 + lista
- ✅ GET /api/v1/monedas → 200 + lista
- ✅ GET /api/v1/unidades-medida → 200 + lista
- ✅ GET /api/v1/modulos-sistema → 200 + lista
- ✅ GET /api/v1/parametros-sistema → 200 + lista
- ✅ POST (crear) → 201 + validaciones funcionales
- ✅ POST (duplicado) → 400 Bad Request
- ✅ PUT (actualizar) → 200
- ✅ PATCH (activar/inactivar) → 200
- ✅ DELETE → 204

**Resultado:** ✅ TODOS LOS ENDPOINTS FUNCIONALES

**Blocker para Sprint 2:** ❌ NO — Sprint 1 validado en producción

**Próximos pasos:**  
- [ ] Ejecutar smoke test (2-3 horas)
- [ ] Documentar resultados en `SMOKE_TEST_RESULTS.md`
- [ ] Si fallos: fix antes Sprint 2

**Responsable:** Miguel (manual test) o Claude Code (automatizar si lo desea)

---

### PD-04: SerieDocumento Concurrency Strategy
**Prioridad:** ALTO  
**Contexto:**  
Sprint 3 implementará `SerieDocumento.NumeroActual` con incremento automático. En producción, múltiples usuarios pueden generar números simultáneamente.

**Decisión pendiente:**  
¿Qué estrategia de concurrencia?

**Opciones:**
1. **ROWLOCK + UPDLOCK** (recomendado)
   - En handler: `UPDATE SeriesDocumento SET NumeroActual = NumeroActual + 1 WITH (ROWLOCK, UPDLOCK)`
   - Garantiza incremento atómico
   - Baja contención si tablas pequeñas

2. **Transacción SERIALIZABLE**
   - Máxima seguridad pero máxima contención
   - Overkill para una tabla pequeña

3. **Sequence SQL Server**
   - SQL Server nativo, pero requiere refactorizar lógica
   - Más complejo de testear

**Recomendación:** ROWLOCK (balance seguridad/performance)

**Próximos pasos:**  
- [ ] Documentar estrategia en `SERIE_DOCUMENTO_CONCURRENCY.md`
- [ ] Implementar en Sprint 3 handler
- [ ] Load test con N usuarios simultáneos

**Responsable:** Claude Code (implementación) + Miguel (validación)

---

### ✅ PD-05: ALTER TABLE Productos — Migration Strategy
**Prioridad:** ALTO  
**Estado:** ✅ COMPLETADO (2026-05-18)  
**Contexto:**  
Sprint 4 agregó 3 FKs nullable a tabla existente `Productos`:
- UnidadMedidaId
- CategoriaProductoId
- MarcaProductoId

**Estrategia implementada:**
1. ✅ Agregadas 3 columnas con DEFAULT NULL
2. ✅ Script migration: `15_AddProductoFKs.sql` (renumerado de FIX_AddProductoFKs.sql)
3. ✅ Script idempotente con IF NOT EXISTS check
4. ✅ Ejecutado exitosamente en BD

**Resultado:**
- ✅ Migración segura: productos existentes NO fueron afectados
- ✅ Ejecución: 0 errores
- ✅ Validación: Productos existentes retienen NULL en nuevas FKs
- ✅ PUT/POST productos ahora soportan 3 nuevos campos opcionales

**Documentación:**
- History Changed: 20260518_T1400_feat_Sprint4ProductoEnriquecido_COMPLETADO.md
- COMMON_ISSUES_AND_FIXES.md: Sección 11 (CQRS Commands)

**Bloqueante para Sprint 5:** ❌ NO — Completado y validado

---

## 🟢 MEDIO (Mejoras, no bloqueantes)

### PD-06: Audit Trail Implementation
**Prioridad:** MEDIO  
**Contexto:**  
Todas las entidades tienen `FechaRegistro` y `FechaActualizacion`, pero no hay:
- UsuarioId quién hizo cambio
- Campo `MotivoCambio` (comentario)
- Tabla audit histórica

**Recomendación:** Postergar a Sprint 6 (audit separado)

**Impacto:** Auditoría limitada ahora, completa después

---

### PD-07: Testing Infrastructure
**Prioridad:** MEDIO  
**Contexto:**  
No hay tests unitarios ni integración. Clean Architecture permite testabilidad, pero no hay tests.

**Recomendación:** 
1. Post-Sprint 5 (después catálogos completos)
2. Implementar xUnit + Moq
3. Cobertura mínima: 80% de handlers

**Próximos pasos:**  
- [ ] Crear spike: testing infrastructure
- [ ] Decidir: xUnit? NUnit? MSTest?

---

### PD-08: API Documentation (Swagger)
**Prioridad:** MEDIO  
**Contexto:**  
35 endpoints en Sprint 1, sin documentación Swagger.

**Recomendación:**  
1. Agregar Swashbuckle a Program.cs
2. Decorar controllers con [ProducesResponseType]
3. Generar swagger.json automático

**Próximos pasos:**  
- [ ] Implementar post-Sprint 2

---

## 🔵 BAJO (Ideas futuras)

### PD-09: Multi-Currency Conversion
**Prioridad:** BAJO  
**Contexto:**  
Decisión D-04: Moneda funcional única (PEN), sin conversión.

**Idea:** Implementar tipo de cambio futuro
- Tabla: `catalogo.TipoCambio(MonedaOrigen, MonedaDestino, Tasa, Fecha)`
- Servicio: `ICurrencyConversionService`
- Handler en Ventas: calcular precio en múltiples monedas

**Próximos pasos:**  
- [ ] Spike: requerimientos de multi-moneda
- [ ] Postergar a v3.2+

---

### PD-10: Feature Flags Enhancement
**Prioridad:** BAJO  
**Contexto:**  
Existe `ModuloSistema` para feature flags, pero no hay middleware/service que valide.

**Idea:** 
- `IModuloSistemaService` con método `IsModuloActivo(codigo)`
- Middleware: verificar módulo antes de controller
- Result: automático 403 Forbidden si módulo inactivo

**Próximos pasos:**  
- [ ] Implementar post-catálogos completos
- [ ] Agregar middleware en Program.cs

---

### ✅ PD-11: Soft Delete Global Filter — RESUELTA
**Prioridad:** BAJO  
**Estado:** ✅ RESUELTA (ADR-003, 2026-04-25)  

**Decisión aprobada:** ADR-003 — Soft Delete como Auditoría, NO como Filtro

**Implementación actual:**
- Campo `Activo` = flag de auditoría
- `GET` retorna TODOS los registros (activos + inactivos)
- Frontend Angular controla presentación visual (filtros, colores, iconos)
- **NUNCA** agregar `HasQueryFilter(x => x.Activo == true)`

**Razón:**
Miguel necesita visibilidad completa de registros para auditoría. Los registros inactivos son históricos, no eliminados.

**Endpoints:**
- `PATCH /{id}/inactivar` → `Activo = false`
- `PATCH /{id}/activar` → `Activo = true`
- `DELETE /{id}` → Hard delete real

**Conclusión:** No es una idea futura, es una decisión arquitectónica ya tomada y validada. ✅ RESUELTA

---

### PD-12: Repository Pattern vs Direct Services
**Prioridad:** BAJO  
**Contexto:**  
Actual: Handlers inyectan IXxxService (wraps EF Core).  
Alternativa: IRepository<T> genérico.

**Ventajas Repository:**
- Abstracción de persistencia
- Swap EF Core por otra BD

**Desventajas:**
- Complejidad extra
- Ya tenemos Service por entidad

**Decisión:** Mantener actual, revisitar si hay cambio de persistencia.

---

## 📊 Summary

| ID | Asunto | Prioridad | Estado | Sprint |
|----|--------|-----------|--------|--------|
| PD-01 | TipoDocumentoEnum | 🔴 CRÍTICO | ✅ RESUELTO | Sprint 1 |
| PD-02 | ListaPrecioDetalle | 🔴 CRÍTICO | ⏳ Decidir antes Sprint 5 | Sprint 5 |
| **PD-02.5** | **SingleTenant Guard en Empresa** | **🟡 ALTO** | **✅ IMPLEMENTADO** | **Sprint 2** |
| PD-03 | Smoke Testing Sprint 1 | 🟡 ALTO | ✅ COMPLETADO | Sprint 1-2 |
| PD-04 | SerieDocumento Concurrency | 🟡 ALTO | ✅ IMPLEMENTADO | Sprint 3 |
| PD-05 | ALTER Productos Migration | 🟡 ALTO | ✅ COMPLETADO | Sprint 4 |
| **PD-13** | **SQL Server Syntax Compatibility** | **🟡 ALTO** | **✅ DOCUMENTADO** | **Sprint 3** |
| **PD-14** | **FromSqlInterpolated Materialization** | **🟡 ALTO** | **✅ DOCUMENTADO** | **Sprint 3** |
| **PD-15** | **CQRS Commands Missing DTO Fields** | **🔴 CRÍTICO** | **✅ DOCUMENTADO** | **Sprint 4** |
| PD-06 | Audit Trail | 🟢 MEDIO | ⏳ Sprint 6+ | Future |
| PD-07 | Testing Infrastructure | 🟢 MEDIO | ⏳ Post-Sprint 5 | Future |
| PD-08 | API Documentation | 🟢 MEDIO | ⏳ Sprint 5+ | Future |
| PD-09 | Multi-Currency | 🔵 BAJO | ⏳ v3.2+ | Future |
| PD-10 | Feature Flags Enhancement | 🔵 BAJO | ⏳ Post-catálogos | Future |
| PD-11 | Soft Delete Global Filter | 🔵 BAJO | ✅ RESUELTO (ADR-003) | Sprint 1 |
| PD-12 | Repository Pattern | 🔵 BAJO | ⏳ Revisit later | Future |

---

## 🚀 PRÓXIMOS PASOS

### Inmediato (Hoy — Sprint 4 completado)
1. ✅ PD-05: ALTER Productos completado y testeado
2. ✅ PD-15: CQRS Commands sync documentado
3. ⏳ Push a develop (cuando SSH disponible)

### Antes de Sprint 5
1. ⏳ **CRÍTICO:** Confirmar PD-02 (ListaPrecioDetalle) — ¿incluir en Sprint 5 o deferred a Ventas?
   - Impacto: +1 tabla + CRUD + 2-3 horas si se incluye
   - Recomendación: Incluir para completar catálogos
2. ⏳ Consultar alcance final con Miguel

### Después de Catálogos (Sprint 5 completo)
1. Planificar Sprint 6: PD-06 (Audit Trail) + PD-07 (Testing)
2. Evaluación: PD-08 (Swagger/API Docs) — ¿ahora o post-Sprint 5?

### Futuro (No bloqueante)
1. PD-09: Multi-Currency conversion (v3.2+)
2. PD-10: Feature Flags middleware enhancement (post-catálogos)
3. PD-12: Repository pattern revisit (si cambio BD)

