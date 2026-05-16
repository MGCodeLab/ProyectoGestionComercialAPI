# Technical Backlog & Pending Decisions — Nexus-ERP

**Última actualización:** 2026-05-15  
**Tipo:** Backlog técnico + arquitectónico + funcional

---

## 🔴 CRÍTICO (Bloqueante para próximas fases)

### PD-01: TipoDocumentoEnum Inconsistencia
**Prioridad:** CRÍTICO  
**Contexto:**  
Existe un enum `TipoDocumentoEnum` en Application que puede no coincidir con IDs reales en base de datos. Esto afecta a:
- Empresa.TipoDocumentoId
- Cliente.TipoDocumentoId  
- Proveedor.TipoDocumentoId (futuro)

**Problema:**  
Si enum define `DNI = 1` pero en BD el ID es `3`, habrá inconsistencia.

**Impacto:** Alto (afecta 3 entidades críticas)

**Recomendación:**  
1. Auditar tabla `catalogo.TipoDocumentos` en BD
2. Si existe: eliminar enum, usar int directo con comentarios
3. Si no existe: crear tabla + seed explícito
4. Documentar en `TIPO_DOCUMENTO_GUIDE.md`

**Próximos pasos:**  
- [ ] Auditar enum vs BD en Sprint 2 pre-work
- [ ] Refactorizar si necesario
- [ ] Crear migración si BD falta tabla

**Responsable:** Miguel (decisión) + Claude Code (implementación)

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

## 🟡 ALTO (Importante, pero no bloqueante inmediatamente)

### PD-03: Smoke Testing de Sprint 1
**Prioridad:** ALTO  
**Contexto:**  
Compilación limpia (0 errores), SQL scripts ejecutados, pero **endpoints no testeados manualmente**.

**Problemas potenciales:**
- Responses no mapean correctamente DTOs
- Validaciones no rechazan duplicados
- Estado Activo/Inactivo no togglea
- 404 en entidades no encontradas

**Recomendación:** Ejecutar smoke test antes de Sprint 2
- [ ] GET /api/v1/paises → 200 + lista
- [ ] POST /api/v1/paises → validación exitosa + 201
- [ ] POST /api/v1/paises (duplicado) → 400 Bad Request
- [ ] PUT, PATCH, DELETE en todas entidades

**Blocker para:** No bloqueante si code review OK

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

### PD-05: ALTER TABLE Productos — Migration Strategy
**Prioridad:** ALTO  
**Contexto:**  
Sprint 4 agregará 3 FKs nullable a tabla existente `Productos`:
- UnidadMedidaId
- CategoriaProductoId
- MarcaProductoId

**Riesgo:** Si ALTER ejecuta fuera de transacción, puede romper datos.

**Estrategia:**
1. Agregar 3 columnas con DEFAULT NULL
2. Crear migration script: `FIX_AddProductoFKs.sql`
3. Script idempotente (si columnas existen, no error)
4. Ejecutar ANTES de nuevo deployment

**Próximos pasos:**  
- [ ] Crear script en Sprint 4
- [ ] Testear en BD de desarrollo
- [ ] Documentar roll-back plan

**Responsable:** Claude Code (script) + Miguel (QA)

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

### PD-11: Soft Delete Global Filter
**Prioridad:** BAJO  
**Contexto:**  
Todas las entidades tienen `Activo` (soft delete), pero `GET` retorna todo (activos + inactivos).

**Decisión:** Es intencional (auditoría), pero puede cambiar.

**Idea futura:** Global filter en EF Core
```csharp
modelBuilder.Entity<AuditableEntity>().HasQueryFilter(x => x.Activo == true);
```

**Próximos pasos:**  
- [ ] Spike: impacto de query filters globales
- [ ] Postergar hasta tener más entidades

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

| ID | Asunto | Prioridad | Estado | Responsable |
|----|--------|-----------|--------|------------|
| PD-01 | TipoDocumentoEnum | 🔴 CRÍTICO | 🔍 Pending | Miguel |
| PD-02 | ListaPrecioDetalle | 🔴 CRÍTICO | ⏳ Sprint 5 | Miguel |
| PD-03 | Smoke Testing | 🟡 ALTO | ⏳ Pre-Sprint 2 | Miguel |
| PD-04 | SerieDocumento Concurrency | 🟡 ALTO | ⏳ Sprint 3 | Both |
| PD-05 | ALTER Productos Migration | 🟡 ALTO | ⏳ Sprint 4 | Both |
| PD-06 | Audit Trail | 🟢 MEDIO | ⏳ Sprint 6+ | Future |
| PD-07 | Testing Infrastructure | 🟢 MEDIO | ⏳ Post-Sprint 5 | Future |
| PD-08 | API Documentation | 🟢 MEDIO | ⏳ Sprint 2-3 | Future |
| PD-09 | Multi-Currency | 🔵 BAJO | ⏳ v3.2+ | Future |
| PD-10 | Feature Flags Enhancement | 🔵 BAJO | ⏳ Post-catálogos | Future |
| PD-11 | Soft Delete Global Filter | 🔵 BAJO | ⏳ Post-catálogos | Future |
| PD-12 | Repository Pattern | 🔵 BAJO | ⏳ Revisit later | Future |

---

**Próximos pasos:**
1. Resolver PD-01 (TipoDocumentoEnum) antes Sprint 2
2. Ejecutar smoke tests (PD-03) antes Sprint 2
3. Confirmar ListaPrecioDetalle en Sprint 5 (PD-02)
4. Planificar Sprint 6 (PD-06, PD-07)

