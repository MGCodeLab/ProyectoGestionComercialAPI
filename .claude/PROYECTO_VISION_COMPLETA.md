# NEXUS-ERP v3.1 — VISIÓN COMPLETA DEL PROYECTO

**Fecha:** 2026-05-16  
**Estado:** Sprint 1 COMPLETADO ✅ — Esperando autorización Sprint 2  
**Presentado a:** Miguel Gonzalez  
**Para:** Decisión sobre Sprint 2 + timeline + riesgos críticos

---

## 📊 ESTADO ACTUAL

### ✅ Sprint 1 — COMPLETADO 100%

| Métrica | Estado |
|---------|--------|
| Entidades implementadas | 5/5 ✅ |
| Handlers CQRS | 20/20 ✅ |
| Validators | 10/10 ✅ |
| ValidatorServices | 5/5 ✅ |
| DTOs | 15/15 ✅ |
| Endpoints | 35/35 ✅ |
| Compilación | 0 errores ✅ |
| SQL scripts | Ejecutados ✅ |
| Commit | 71e9c9a ✅ |

**Entidades completadas:**
- Pais (catalogo.Paises)
- Moneda (catalogo.Monedas)
- UnidadMedida (catalogo.UnidadesMedida)
- ModuloSistema (configuracion.ModulosSistema)
- ParametroSistema (configuracion.ParametrosSistema)

### 📈 Progreso General

```
Sprint 1 (Catálogos Base)    ████████████████████ 100% ✅ COMPLETADO
Sprint 2 (Organización)      ░░░░░░░░░░░░░░░░░░░░  0%  ⏳ Autorización pendiente
Sprint 3 (Fiscal)            ░░░░░░░░░░░░░░░░░░░░  0%  ⏳ Planeado
Sprint 4 (Producto)          ░░░░░░░░░░░░░░░░░░░░  0%  ⏳ Planeado
Sprint 5 (Comercial)         ░░░░░░░░░░░░░░░░░░░░  0%  ⏳ Planeado
────────────────────────────────────────────────────────────────────
TOTAL PROYECTO               ████████░░░░░░░░░░░░ 20% (5 de 18 entidades)
```

---

## 🗺️ ROADMAP COMPLETO (Sprints 2-5)

### SPRINT 2: Organización — Empresa, Sucursal, Almacén

**Duración estimada:** 6-8 horas  
**Complejidad:** MEDIA  
**Bloquea:** Sprint 3+  

#### Entidades a crear:

**1. Empresa** → `organizacion.Empresas`
```
RazonSocial (200 chars) - obligatorio, único
NombreComercial (200 chars)
NumeroDocumento (20 chars) - UNIQUE (RUC)
TipoDocumentoId → FK catalogo.TipoDocumentos
PaisId → FK catalogo.Paises (RESTRICT)
MonedaBaseId → FK catalogo.Monedas (RESTRICT)
DireccionFiscal, Telefono, Correo, LogoUrl
```
**Restricción crítica:** Solo 1 empresa en Application (SingleTenantGuard en handler)

**2. Sucursal** → `organizacion.Sucursales`
```
Nombre (150 chars) - obligatorio
Codigo (10 chars) - UNIQUE
EmpresaId → FK Empresas (RESTRICT)
PaisId → FK Paises (RESTRICT)
Direccion, Telefono
EsPrincipal (BIT) - solo 1 true por empresa (application rule)
```

**3. Almacén** → `organizacion.Almacenes`
```
Nombre (150 chars) - obligatorio
Codigo (10 chars) - UNIQUE
SucursalId → FK Sucursales (RESTRICT)
Descripcion (500 chars)
EsPrincipal (BIT)
```

**Patrón:** Idéntico a Sprint 1 (Pais, Moneda, UnidadMedida)
- Handlers: 3 entidades × 4 handlers = 12 nuevos handlers
- Validators: 3 × 2 = 6 nuevos validators
- ValidatorServices: 3 nuevos services
- DTOs: 3 × 3 = 9 nuevos DTOs
- Controllers: 3 nuevos + 3 DI registrations

**Archivos esperados:** ~20 nuevos + modificaciones Program.cs + SQL

---

### SPRINT 3: Fiscal — Impuestos, Comprobantes, Series (CRÍTICO)

**Duración estimada:** 8-10 horas  
**Complejidad:** ALTA (SerieDocumento con concurrencia)  
**Bloquea:** Módulo Ventas v3.1  
**Riesgo:** Race condition en NumeroActual

#### Entidades:

**1. TipoImpuesto** → `catalogo.TiposImpuesto`
```
Nombre (100 chars)
Codigo (10 chars) - UNIQUE
Porcentaje (DECIMAL 5,2)
EsIncluido (BIT) - ¿incluido en precio o agregado?
```
Seed: IGV=18%, ISC=0%, EXONERADO, INAFECTO

**2. TipoComprobante** → `catalogo.TiposComprobante`
```
Nombre (100 chars)
Codigo (5 chars) - UNIQUE (01=Factura, 03=Boleta, NV=Nota Venta)
AfectaInventario (BIT)
AfectaContable (BIT)
```

**3. SerieDocumento** → `catalogo.SeriesDocumento` ⚠️ CRÍTICO
```
TipoComprobanteId → FK TiposComprobante (RESTRICT)
SucursalId → FK Sucursales (RESTRICT)
Serie (4 chars) - ej: F001, B001, T001
NumeroActual (INT) - ÚLTIMA SECUENCIA USADA
NumeroMaximo (INT NULL) - limite o NULL=sin límite
-- UNIQUE (TipoComprobanteId, SucursalId, Serie)
```

**⚠️ CONCURRENCIA CRÍTICA:**
- Cuando se genera venta, se incrementa NumeroActual
- Con múltiples usuarios simultáneos → race condition
- **Solución:** UPDATE con ROWLOCK + UPDLOCK en handler
- Genera número ANTES de insertar Venta

**Seed inicial:** F001, B001 para Sucursal principal

---

### SPRINT 4: Producto Enriquecido — Categoría, Marca, ALTER Productos

**Duración estimada:** 5-6 horas  
**Complejidad:** BAJA (patrones conocidos)  
**Riesgo:** ALTER TABLE Productos (migración)  

#### Entidades:

**1. CategoriaProducto** → `catalogo.CategoriasProducto`
```
Nombre (150 chars)
Descripcion (500 chars)
CategoriaPadreId → FK self (RESTRICT) - árbol de categorías
```
**Restricción:** Máx 3 niveles de profundidad (application rule)

**2. MarcaProducto** → `catalogo.MarcasProducto`
```
Nombre (150 chars)
Descripcion (500 chars)
LogoUrl (500 chars)
```

**3. ALTER TABLE Productos** ⚠️ MIGRACIÓN
```sql
ALTER TABLE catalogo.Productos ADD
  UnidadMedidaId INT NULL → FK UnidadesMedida (RESTRICT),
  CategoriaProductoId INT NULL → FK CategoriasProducto (RESTRICT),
  MarcaProductoId INT NULL → FK MarcasProducto (RESTRICT);
```

**⚠️ Riesgo:** Productos existentes sin nuevas FK  
**Mitigación:** FKs NULLABLE + migration script idempotente en `FIX_AddProductoFKs.sql`

---

### SPRINT 5: Comercial — CondicionPago, ListaPrecio, Proveedor

**Duración estimada:** 6-7 horas  
**Complejidad:** BAJA (patrones conocidos)  

#### Entidades:

**1. CondicionPago** → `catalogo.CondicionesPago`
```
Nombre (100 chars)
DiasCredito (INT) - 0=Contado, 15=15 días, 30=30 días, etc
Descripcion (500 chars)
```
Seed: Contado(0), 15 días, 30 días, 60 días

**2. ListaPrecio** → `catalogo.ListasPrecios`
```
Nombre (150 chars)
MonedaId → FK Monedas (RESTRICT)
Descripcion (500 chars)
EsDefault (BIT)
```
**Nota:** Precios de productos en ListaPrecioDetalle (deferred a Ventas o incluir aquí?)

**3. Proveedor** → `comercial.Proveedores`
```
TipoDocumentoId → FK TipoDocumentos
NumeroDocumento (20 chars)
RazonSocial (200 chars)
NombreComercial (150 chars)
PaisId → FK Paises (RESTRICT)
Correo (150 chars) - filtered unique index
Telefono, Direccion
-- UNIQUE (TipoDocumentoId, NumeroDocumento)
```

**Patrón:** Clonar exactamente de Cliente (CQRS idéntico)

---

## ⏱️ TIMELINE ESTIMADO

| Sprint | Entidades | Duración | Fin Estimado | Status |
|--------|-----------|----------|------------|--------|
| 1 | 5 | 4-5h | ✅ 2026-05-10 | COMPLETADO |
| 2 | 3 | 6-8h | 2026-05-18 | ⏳ Autorización |
| 3 | 3 | 8-10h | 2026-05-26 | Planeado |
| 4 | 3 | 5-6h | 2026-06-01 | Planeado |
| 5 | 4 | 6-7h | 2026-06-08 | Planeado |
| **TOTAL** | **18** | **29-36h** | **~2026-06-08** | **Catálogos 100%** |

**Post-catálogos:** Módulo Ventas v3.1 (desbloqueado)

---

## 🏗️ DECISIONES ARQUITECTÓNICAS APROBADAS

| # | Decisión | Status |
|----|----------|--------|
| D-01 | No hardcodear Perú — parametrizar por `Pais` | ✅ Implementado |
| D-02 | Single tenant hoy, arquitectura multi-tenant ready | ✅ Implementado |
| D-03 | Feature flags via `ModuloSistema` | ✅ Implementado |
| D-04 | Moneda única (PEN) — sin conversión aún | ✅ Implementado |
| D-05 | `SerieDocumento` controlada por `Sucursal` | ✅ Planeado (Sprint 3) |
| D-06 | `Empresa` single record (Application, no BD) | ⏳ Sprint 2 |
| D-07 | ValidatorService para todas las validaciones DB | ✅ Implementado |
| D-08 | CQRS pragmático en toda arquitectura | ✅ Implementado |

---

## 🚨 RIESGOS CRÍTICOS IDENTIFICADOS

### 🔴 CRÍTICOS

| ID | Riesgo | Probabilidad | Impacto | Mitigación | Status |
|----|--------|-------------|---------|-----------|--------|
| **RG-01** | TipoDocumentoEnum inconsistente con IDs BD | Alta | Medio | Auditar antes Sprint 2 | ⏳ **BLOQUEANTE** |
| **RG-02** | SerieDocumento race condition (Sprint 3) | Media | Crítico | ROWLOCK + SERIALIZABLE | 🟡 Planned |

### 🟡 ALTOS

| ID | Riesgo | Probabilidad | Impacto | Mitigación | Status |
|----|--------|-------------|---------|-----------|--------|
| **RG-03** | ALTER TABLE Productos rompe datos (Sprint 4) | Baja | Alto | Nullable FKs + migration script | 🟡 Planned |
| **RG-04** | SingleTenant guard fallido (Sprint 2) | Media | Alto | Unit tests + application rule | 🟡 Planned |
| **RG-05** | Smoke testing de Sprint 1 no ejecutado | Baja | Medio | Opcional, compilation OK | ⏳ Optional |

---

## 📋 DECISIONES PENDIENTES (PD)

### 🔴 CRÍTICAS — Resolver ANTES Sprint 2

**✅ PD-01: TipoDocumentoEnum Inconsistencia — RESUELTO**
- **Estado:** ✅ VERIFICADO POR MIGUEL (2026-05-16)
- **IDs Correctos en BD:**
  - CE = 3
  - DNI = 4
  - RUC = 5
  - PASSPORT = 6
- **Impacto:** ✅ RESUELTO — Sin mismatch detectado
- **Bloqueante para Sprint 2:** ❌ NO — Seguro proceder
- **Acción:** Usar directamente los IDs numéricos en Empresa, Cliente, Proveedor

**PD-02: ListaPrecioDetalle — ¿Ahora o después?**
- **Problema:** ¿Incluir tabla `ListaPrecioDetalle` en Sprint 5 o deferred?
- **Opciones:**
  - A) Incluir en Sprint 5 (completa catálogos, más trabajo)
  - B) Deferred a módulo Ventas (más rápido Sprint 5)
- **Recomendación:** Opción B (deferred a Ventas, menos crítico)
- **Tiempo:** 0 si opción B, 3-4h si opción A
- **Decisión requerida:** Antes de Sprint 5

---

## 🔗 MAPA DE DEPENDENCIAS

```
SPRINT 1 ✅ (Sin dependencias)
├─ Pais
├─ Moneda  
├─ UnidadMedida
├─ ModuloSistema
└─ ParametroSistema
   ↓
SPRINT 2 ⏳ (Depende de Sprint 1)
├─ Empresa(Pais, Moneda)
├─ Sucursal(Empresa, Pais)
└─ Almacen(Sucursal)
   ↓
SPRINT 3 ⏳ (Depende de Sprints 1+2)
├─ TipoImpuesto (sin deps)
├─ TipoComprobante (sin deps)
└─ SerieDocumento(TipoComprobante, Sucursal) ← CRÍTICO
   ↓
SPRINT 4 ⏳ (Depende de Sprint 1)
├─ CategoriaProducto
├─ MarcaProducto
└─ ALTER Productos(UnidadMedida, Categoria, Marca)
   ↓
SPRINT 5 ⏳ (Depende de Sprints 1+2+4)
├─ CondicionPago (sin deps)
├─ ListaPrecio(Moneda)
└─ Proveedor(TipoDocumento, Pais)
   ↓
MÓDULO VENTAS v3.1 (DESBLOQUEADO)
└─ Venta(Empresa,Sucursal,Cliente,SerieDoc,CondPago,Moneda)
   └─ VentaDetalle(Producto,UnidadMedida,TipoImpuesto)
```

---

## 🎯 DECISIONES CLAVE PARA SPRINT 2

**Necesito tu aprobación explícita en estos 2 puntos restantes:**

### ✅ 1️⃣ PD-01: TipoDocumentoEnum — RESUELTO

**Estado:** ✅ Verificado por Miguel (2026-05-16)  
**IDs correctos en BD:** CE=3, DNI=4, RUC=5, PASSPORT=6  
**Bloqueante:** ❌ NO

No hay acción requerida. Proceder con confianza a Sprint 2.

---

### ✅ 2️⃣ Smoke Testing Sprint 1 — COMPLETADO

**Status:** ✅ Testing ejecutado con Postman por Miguel  
**Endpoints validados:** GET, POST, PUT, PATCH, DELETE en todos catálogos  
**Bloqueante:** ❌ NO

No hay acción requerida. Sprint 1 validado en producción.

---

### 3️⃣ AUTORIZACIÓN PARA SPRINT 2

**¿Procedo con Empresa, Sucursal, Almacén?**

- [ ] ✅ **SÍ — Adelante**
  - Una vez resuelva PD-01 (si Opción A)
  - Envío a Nexus-Fast-Builder
  
- [ ] ⏳ **ESPERA**
  - Necesito resolver primero: _______
  
- [ ] ❌ **NO — Postergar**
  - Razón: _______

**Tu decisión:** ________

---

### 4️⃣ ARQUITECTURA Sprint 2: SingleTenant Guard

**¿Apruebas implementación en Empresa?**

En `CrearEmpresaHandler`:
```csharp
var empresaExistente = await _empresaService.ObtenerPrimera();
if (empresaExistente != null)
    throw new InvalidOperationException("Solo 1 empresa permitida");

// Crear empresa
```

**Restricción:** Application level, NO en BD  
**Razón:** Preparado para multi-tenant futuro  

- [ ] Sí, aprobado
- [ ] Cambiar a: _______

**Tu decisión:** ________

---

## 📊 ESTADO GOBERNANZA (Ya aprobado)

```
✅ Estructura .claude/
├── plans/active/ → Roadmap Sprints 2-5
├── plans/completed/ → Sprint 1
├── execution-status/ → Estado actual
├── pending/ → Backlog técnico (12 items)
└── IA_Docs/
    ├── GOVERNANCE_STRUCTURE.md
    ├── GOVERNANCE_SUMMARY.md
    └── VALIDATOR_SERVICE_PATTERN.md
```

**Responsabilidades automáticas:**
- Claude Code (yo) actualizo status cada sesión
- Archivos en `.claude/` son fuente de verdad
- Cambios visibles en git antes de commit

---

## 🚀 PRÓXIMOS PASOS

### Si autorizas Sprint 2:

1. **Resolver PD-01** (si Opción A)
   - [ ] Auditar TipoDocumentoEnum
   - [ ] Refactorizar si necesario
   - [ ] Tiempo: 1-2 horas

2. **Enviar a Nexus-Fast-Builder**
   - [ ] Crear Empresa, Sucursal, Almacén
   - [ ] Handlers, Validators, Controllers
   - [ ] SQL DDL + seeds
   - [ ] Tiempo: 6-8 horas

3. **Smoke testing Sprint 2**
   - [ ] GET /api/v1/empresa
   - [ ] POST /api/v1/sucursales
   - [ ] Validaciones (SingleTenant, etc)

4. **Commit final Sprint 2**
   - Mensaje: `feat(catalogo): Sprint 2 — Organización (Empresa, Sucursal, Almacen)`

5. **Evaluar Timeline**
   - ¿Continuar con Sprint 3 inmediatamente?
   - ¿Descanso técnico?
   - ¿Smoke testing completo antes?

---

## 📌 RESUMEN PARA DECISIÓN

| Item | Estado | Bloqueante |
|------|--------|-----------|
| Sprint 1 | ✅ 100% completo | No |
| Gobernanza | ✅ Aprobada | No |
| Arquitectura Sprint 2 | ✅ Definida | No |
| PD-01 TipoDocumentoEnum | ✅ RESUELTO | No |
| Smoke Testing Sprint 1 | ✅ COMPLETADO | No |
| **Autorización Sprint 2** | ⏳ **AWAITING** | **SÍ** |
| **SingleTenant en Empresa** | ⏳ **AWAITING** | **SÍ** |

---

## ✉️ PRÓXIMA ACCIÓN

**Cuando confirmes los 4 puntos anteriores:**
1. Presento plan detallado de Sprint 2
2. Resuelvo PD-01 (si necesario)
3. **Activo Nexus-Fast-Builder** para implementación
4. Actualizo execution-status en `.claude/`

---

**Espero tu confirmación en los 4 puntos de decisión.** 🎯

