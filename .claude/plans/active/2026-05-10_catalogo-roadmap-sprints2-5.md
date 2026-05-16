# Plan: Catálogos Base — Roadmap Sprints 2-5 (Nexus-ERP v3.1)

**Estado:** ACTIVO (pendiente aprobación Sprint 2)  
**Fecha creación:** 2026-05-10  
**Rama base:** `catalogo-base/sprint_*`  
**Objetivo final:** Completar todas las entidades de catálogo base para desbloquear módulo Ventas v3.1

---

## Objetivo General

Implementar arquitectura de catálogos con 18 entidades distribuidas en 5 sprints:
- Sprint 1: 5 entidades (sin dependencias) ✅ COMPLETADO
- Sprint 2: 3 entidades (organización)
- Sprint 3: 3 entidades (fiscal)
- Sprint 4: 3 entidades (producto enriquecido)
- Sprint 5: 4 entidades (comercial)

**Resultado:** Sistema catálogos 100% funcional que sustenta módulo Ventas.

---

## Mapa de Dependencias (Critical Path)

```
SPRINT 1 ✅ (Sin dependencias)
├─ Pais
├─ Moneda
├─ UnidadMedida
├─ ModuloSistema
└─ ParametroSistema
   ↓ (Foundation para todo lo demás)

SPRINT 2 (Organización - depende de Sprint 1)
├─ Empresa(Pais, Moneda)
├─ Sucursal(Empresa, Pais)
└─ Almacen(Sucursal)
   ↓ (Owner de datos para transacciones)

SPRINT 3 (Fiscal - depende de Sprints 1+2)
├─ TipoImpuesto (sin deps)
├─ TipoComprobante (sin deps)
└─ SerieDocumento(TipoComprobante, Sucursal) ← CRÍTICO para Ventas
   ↓ (Números de comprobantes)

SPRINT 4 (Producto Enriquecido - depende de Sprint 1)
├─ CategoriaProducto (self-ref)
├─ MarcaProducto (sin deps)
└─ ALTER Productos(UnidadMedida, Categoria, Marca)
   ↓ (Producto completo)

SPRINT 5 (Comercial - depende de Sprints 1+2+4)
├─ CondicionPago (sin deps)
├─ ListaPrecio(Moneda)
└─ Proveedor(TipoDocumento, Pais)
   ↓ (Maestros comerciales)

MÓDULO VENTAS v3.1 (Desbloqueado)
└─ Venta(Empresa,Sucursal,Cliente,SerieDoc,CondPago,Moneda)
   └─ VentaDetalle(Producto,UnidadMedida,TipoImpuesto)
```

---

## Decisiones Arquitectónicas (Aprobadas en Session 1)

| # | Decisión | Rationale |
|---|----------|-----------|
| D-01 | No hardcodear Perú — parametrizar por `Pais` | Multi-país desde inicio |
| D-02 | Single tenant hoy, arquitectura multi-tenant ready | Sin complejidad innecesaria |
| D-03 | Feature flags via `ModuloSistema` | Control comercial modular |
| D-04 | Moneda funcional única (PEN) — catálogo sin conversión aún | Multi-moneda postponed |
| D-05 | `SerieDocumento` controlada por `Sucursal` | Modelo real facturación LATAM |
| D-06 | `Empresa` enforza single record (Application, no BD) | Preparado para multi-tenant |
| D-07 | Patrón ValidatorService para todas las validaciones DB | Clean Architecture garantizada |
| D-08 | CQRS pragmático en toda la arquitectura | Separación clara Commands/Queries |

---

## Sprint 2: Organización (Pendiente Aprobación)

### Entidades (3)

#### 1. Empresa → `organizacion.Empresas`
```
RazonSocial           NVARCHAR(200) NOT NULL
NombreComercial       NVARCHAR(200) NULL
NumeroDocumento       NVARCHAR(20) NOT NULL UNIQUE (RUC en Perú)
TipoDocumentoId       INT NOT NULL → FK catalogo.TipoDocumentos
PaisId                INT NOT NULL → FK catalogo.Paises
MonedaBaseId          INT NOT NULL → FK catalogo.Monedas
DireccionFiscal       NVARCHAR(300) NULL
Telefono, Correo, LogoUrl
```
**Regla crítica:** SingleTenantGuard en CrearEmpresaHandler (Application, no BD)

#### 2. Sucursal → `organizacion.Sucursales`
```
Nombre                NVARCHAR(150) NOT NULL
Codigo                NVARCHAR(10) NOT NULL UNIQUE
EmpresaId             INT NOT NULL → FK Empresas (RESTRICT)
PaisId                INT NOT NULL → FK Paises (RESTRICT)
Direccion, Telefono
EsPrincipal           BIT NOT NULL DEFAULT 0
```
**Regla:** Solo 1 `EsPrincipal = true` por empresa (Application rule)

#### 3. Almacen → `organizacion.Almacenes`
```
Nombre                NVARCHAR(150) NOT NULL
Codigo                NVARCHAR(10) NOT NULL UNIQUE
SucursalId            INT NOT NULL → FK Sucursales (RESTRICT)
Descripcion           NVARCHAR(500) NULL
EsPrincipal           BIT NOT NULL DEFAULT 0
```

### Archivos a crear: ~20 nuevos
- 3 entidades × (Domain + Config + DTOs + Handlers + Validators + Service + Controller) = 21 archivos
- Program.cs: +3 DI registrations + 3 ValidatorServices
- SQL DDL: +3 tablas + 1 seed script
- Controllers: +3 nuevos

### Riesgos Sprint 2
| Riesgo | Mitigación |
|--------|-----------|
| SingleTenant guard fallido → múltiples empresas | Handler validación + unit test |
| FK Pais no coincide en Empresa y Sucursal | Data consistency rule, app validation |
| EsPrincipal duplicado en Sucursal/Almacen | Application guard, no DB constraint |

### Estimación
- Tiempo: ~6-8 horas
- Complejidad: Media (nuevos conceptos: single tenant, múltiples FKs)

---

## Sprint 3: Fiscal (Fase 3a)

### Entidades (3)

#### 1. TipoImpuesto → `catalogo.TiposImpuesto`
```
Nombre                NVARCHAR(100) NOT NULL
Codigo                NVARCHAR(10) NOT NULL UNIQUE
Porcentaje            DECIMAL(5,2) NOT NULL DEFAULT 0
EsIncluido            BIT NOT NULL DEFAULT 1 (incluido en precio vs agregado)
```
Seed: IGV=18%, ISC=0%, EXONERADO=0%, INAFECTO=0%

#### 2. TipoComprobante → `catalogo.TiposComprobante`
```
Nombre                NVARCHAR(100) NOT NULL
Codigo                NVARCHAR(5) NOT NULL UNIQUE
AfectaInventario      BIT NOT NULL DEFAULT 1
AfectaContable        BIT NOT NULL DEFAULT 1
```
Seed: Factura(01), Boleta(03), Nota Venta(NV)

#### 3. SerieDocumento → `catalogo.SeriesDocumento` ← CRÍTICO
```
TipoComprobanteId     INT NOT NULL → FK TiposComprobante (RESTRICT)
SucursalId            INT NOT NULL → FK Sucursales (RESTRICT)
Serie                 NVARCHAR(4) NOT NULL
NumeroActual          INT NOT NULL DEFAULT 0 (última secuencia usado)
NumeroMaximo          INT NULL (limit, NULL = unlimited)
-- UNIQUE (TipoComprobanteId, SucursalId, Serie)
```
**Concurrencia crítica:** NumeroActual se incrementa con UPDATE + ROWLOCK. Handler de Ventas genera número ANTES de insert.

### Archivos a crear: ~20 nuevos

### Riesgos Sprint 3
| Riesgo | Probabilidad | Impacto | Mitigación |
|--------|-------------|---------|-----------|
| Race condition en NumeroActual | Media | Alto | ROWLOCK + UPDLOCK en handler |
| Números de comprobante duplicados | Baja | Crítico | Transacción SERIALIZABLE |
| Límite de serie (NumeroMaximo) | Baja | Medio | Validación en handler |

### Estimación
- Tiempo: ~8-10 horas
- Complejidad: Alta (SerieDocumento con concurrencia)

---

## Sprint 4: Producto Enriquecido (Fase 3b)

### Entidades (3)

#### 1. CategoriaProducto → `catalogo.CategoriasProducto`
```
Nombre                NVARCHAR(150) NOT NULL
Descripcion           NVARCHAR(500) NULL
CategoriaPadreId      INT NULL → FK CategoriasProducto (RESTRICT)
-- Profundidad máxima: 3 niveles (Application rule)
```
**Patrón:** Self-referencial con límite de profundidad

#### 2. MarcaProducto → `catalogo.MarcasProducto`
```
Nombre                NVARCHAR(150) NOT NULL
Descripcion           NVARCHAR(500) NULL
LogoUrl               NVARCHAR(500) NULL
```

#### 3. ALTER TABLE Productos
Agregar 3 columnas nullable:
```sql
UnidadMedidaId        INT NULL → FK UnidadesMedida
CategoriaProductoId   INT NULL → FK CategoriasProducto
MarcaProductoId       INT NULL → FK MarcasProducto
```
**Riesgo R-01:** ALTER TABLE rompe datos existentes
**Mitigación:** Nullable FKs + migration script idempotente en `FIX_AddProductoFKs.sql`

### Archivos a crear: ~18 nuevos
- 2 entidades nuevas × 9 archivos = 18
- ALTER script + migration

### Riesgos Sprint 4
| Riesgo | Mitigación |
|--------|-----------|
| CategoriaProducto ciclo infinito (padre = hijo) | Validación de profundidad en handler |
| Productos existentes sin Categoria | FKs nullable, migración smooth |
| Inconsistencia con datos históricos | Script idempotente |

### Estimación
- Tiempo: ~5-6 horas
- Complejidad: Baja (patrones conocidos + ALTER safe)

---

## Sprint 5: Comercial (Fase 4)

### Entidades (4)

#### 1. CondicionPago → `catalogo.CondicionesPago`
```
Nombre                NVARCHAR(100) NOT NULL
DiasCredito           INT NOT NULL DEFAULT 0 (0=Contado)
Descripcion           NVARCHAR(500) NULL
```
Seed: Contado(0), 15 días(15), 30 días(30), 60 días(60)

#### 2. ListaPrecio → `catalogo.ListasPrecios`
```
Nombre                NVARCHAR(150) NOT NULL
MonedaId              INT NOT NULL → FK Monedas (RESTRICT)
Descripcion           NVARCHAR(500) NULL
EsDefault             BIT NOT NULL DEFAULT 0
```
**Nota:** Precios de productos en ListaPrecioDetalle (deferred a Fase Ventas)

#### 3. Proveedor → `comercial.Proveedores`
```
TipoDocumentoId       INT NOT NULL → FK TipoDocumentos
NumeroDocumento       NVARCHAR(20) NOT NULL
RazonSocial           NVARCHAR(200) NOT NULL
NombreComercial       NVARCHAR(150) NULL
PaisId                INT NOT NULL → FK Paises
Correo                NVARCHAR(150) NULL (filtered unique index)
Telefono, Direccion
-- UNIQUE (TipoDocumentoId, NumeroDocumento)
```
**Patrón:** Idéntico a Cliente (clonar exactamente)

#### 4. TipoDocumento → Verificación
Si no existe, crear aquí (puede faltar de Sprint 1)

### Archivos a crear: ~18 nuevos
- 3 entidades × 6 archivos = 18

### Riesgos Sprint 5
| Riesgo | Mitigación |
|--------|-----------|
| Proveedor duplicado | UNIQUE constraint + validador |
| ListaPrecio vacío | Seed default + app validation |

### Estimación
- Tiempo: ~6-7 horas
- Complejidad: Baja (patrones conocidos)

---

## Resumen de Entidades por Sprint

```
SPRINT 1 ✅    Pais, Moneda, UnidadMedida, ModuloSistema, ParametroSistema
SPRINT 2 ⏳    Empresa, Sucursal, Almacen
SPRINT 3 ⏳    TipoImpuesto, TipoComprobante, SerieDocumento
SPRINT 4 ⏳    CategoriaProducto, MarcaProducto, ALTER Productos
SPRINT 5 ⏳    CondicionPago, ListaPrecio, Proveedor
               ───────────────────────────────────────────────
               Total: 18 entidades + 1 ALTER = 19 cambios
```

---

## Timeline Estimado

| Sprint | Entidades | Duración | FIN Estimado |
|--------|-----------|----------|------------|
| 1 | 5 | 4-5h | ✅ 2026-05-10 |
| 2 | 3 | 6-8h | 2026-05-18 |
| 3 | 3 | 8-10h | 2026-05-26 |
| 4 | 3 | 5-6h | 2026-06-01 |
| 5 | 4 | 6-7h | 2026-06-08 |
| **TOTAL** | **18** | **29-36h** | **~2026-06-08** |

---

## Riesgos Globales (Cross-Sprint)

| # | Riesgo | Probabilidad | Impacto | Mitigación |
|---|--------|-------------|---------|-----------|
| RG-01 | TipoDocumentoEnum inconsistente (R-03 anterior) | Alta | Medio | Refactor: eliminar enum, usar int |
| RG-02 | SerieDocumento race condition (Sprint 3) | Media | Crítico | ROWLOCK + transacción SERIALIZABLE |
| RG-03 | ALTER TABLE Productos rompe datos (Sprint 4) | Baja | Alto | FKs nullable + script idempotente |
| RG-04 | Validaciones distribuidas en múltiples services | Baja | Medio | ValidatorService pattern garantiza consistencia |
| RG-05 | Seed script incompleto → entidades sin datos | Baja | Medio | Seed obligatorio por entidad, testing |

---

## Bloqueos Actuales

**Ninguno.** Sprint 1 completado, Sprint 2 listo para iniciar con aprobación.

---

## Decisiones Pendientes

| # | Decisión | Impacto | Status |
|---|----------|---------|--------|
| DP-01 | ¿Aprobación para iniciar Sprint 2? | Crítico | ⏳ Await Miguel |
| DP-02 | ¿Convertir TipoDocumentoEnum a int directo? | Medio | ⏳ Post-Sprint 2 |
| DP-03 | ¿Implementar ListaPrecioDetalle ahora o en Ventas? | Bajo | ⏳ Deferred a Ventas |

---

## Próximos Pasos

1. **Validar estructura de gobernanza** (.claude/)
2. **Revisar execution-status** actual
3. **Aprobación formal de Sprint 2** por Miguel
4. **Iniciar Sprint 2** (Empresa, Sucursal, Almacen)

---

**Última actualización:** 2026-05-15  
**Próxima revisión:** Post-Sprint 2 (revaluar timeline)

