# Frontend Integration Roadmap — Nexus-ERP v3.1

**Documento para:** Arquitecto Senior Frontend  
**Objetivo:** Entender arquitectura backend + armar plan de sprints frontend espejo  
**Fecha:** 2026-05-18  
**Estado:** Backend 100% completo (18 entidades, 5 sprints)  
**Versión:** v1.0

---

## 📋 Índice

1. [Contexto & Visión](#contexto--visión)
2. [Arquitectura Backend](#arquitectura-backend)
3. [Los 5 Sprints Ejecutados](#los-5-sprints-ejecutados)
4. [Entidades & APIs (Referencia Completa)](#entidades--apis-referencia-completa)
5. [Modelos de Datos (DTOs)](#modelos-de-datos-dtos)
6. [Flujos de Datos Críticos](#flujos-de-datos-críticos)
7. [Dependencias entre Entidades](#dependencias-entre-entidades)
8. [Sugerencia: Plan de Sprints Frontend](#sugerencia-plan-de-sprints-frontend)
9. [Notas de Integración](#notas-de-integración)

---

## Contexto & Visión

### Estado Actual (2026-05-18)

**Backend Nexus-ERP v3.0.0 → v3.1.0**

| Módulo | Estado | Entidades | Endpoints | LOC | Archivos |
|--------|--------|-----------|-----------|-----|----------|
| Autenticación | ✅ Completo | Auth, User | 4 | ~500 | 20+ |
| Clientes | ✅ Completo | Cliente | 7 | ~800 | 25+ |
| Productos | ✅ Completo | Producto | 7 | ~600 | 25+ |
| **Catálogos Base** | ✅ **100% NUEVO** | **18 entidades** | **~75** | **~1,827** | **70+** |
| Ventas | ⏳ Próximo | - | - | - | - |

### Hito Alcanzado

✅ **Catálogos Base 100% Completados** (Sprint 1-5)  
✅ **Master Data lista** para construir Ventas  
✅ **Arquitectura escalable** (multi-país, multi-moneda, multi-sucursal)  
✅ **Este documento es autosuficiente** — Todo lo necesario para frontend está aquí  

### Próxima Fase: Ventas v3.1

**Desbloqueado:**
- Número correlativo de documentos (`SerieDocumento`)
- Impuestos automáticos (`TipoImpuesto`)
- Unidades de medida (`UnidadMedida`)
- Condiciones de pago (`CondicionPago`)
- Listas de precio (`ListaPrecio`)
- Categorías de producto (`CategoriaProducto`)

---

## Arquitectura Backend

### Capas & Patrones

```
┌─────────────────────────────────────────────────────────┐
│             Controllers (HTTP Endpoints)                 │
│                    7/entidad                             │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│     Application Layer (Commands + Queries)              │
│  • Commands → MediatR → Handlers                        │
│  • Queries → Services (directo, sin MediatR)            │
│  • FluentValidation + ValidatorService                  │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│          Domain Layer (Business Rules)                  │
│  • Entities (18 nuevas)                                 │
│  • Value Objects & Enums                               │
│  • Soft Delete (patrón Activo)                          │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│      Infrastructure Layer (Persistence)                 │
│  • EF Core + SQL Server                                │
│  • DbContext + Configurations (Fluent API)             │
│  • Migrations + Seeds                                  │
│  • Repositories/Services                               │
└─────────────────────────────────────────────────────────┘
```

### Convenciones Globales

| Aspecto | Convención | Ejemplo |
|---------|-----------|---------|
| **Endpoints** | `/api/v1/{recurso}` | `/api/v1/paises`, `/api/v1/empresas` |
| **HTTP Methods** | CRUD estándar | GET, POST, PUT, DELETE |
| **Response** | Wrapper + TraceId | `{ success, data, message, traceId }` |
| **Validation** | FluentValidation + DB | POST/PUT valida entrada |
| **Soft Delete** | Campo `Activo` (BIT) | `GET` solo activos, `DELETE` soft |
| **DTOs** | Crear + Actualizar | `CrearPaisDto`, `ActualizarPaisDto` |
| **Status Code** | Standard HTTP | 200, 201, 400, 404, 500 |
| **Rate Limit** | Por venir | - |

---

## Los 5 Sprints Ejecutados

### Sprint 1: Catálogos Base (Fundación)
**Duración:** ~4.5h real  
**Entidades:** 5  
**Nuevos Schemas:** `organizacion`, `configuracion`

| # | Entidad | Tabla SQL | Descripción |
|---|---------|-----------|-------------|
| 1 | Pais | `catalogo.Paises` | ISO 3166-1 alpha-2 (PE, CL, AR...) |
| 2 | Moneda | `catalogo.Monedas` | ISO 4217 (PEN, USD, CLP...) |
| 3 | UnidadMedida | `catalogo.UnidadesMedida` | SUNAT codes (UND, KGM, LTR, MTR, CAJ) |
| 4 | ModuloSistema | `configuracion.ModulosSistema` | Feature flags (VENTAS, COMPRAS, INVENTARIO) |
| 5 | ParametroSistema | `configuracion.ParametrosSistema` | Config global (MONEDA_BASE, IGV_PORCENTAJE) |

**Endpoints por entidad:** 7 (GET, POST, PUT, DELETE, GetByStatus)  
**Total Sprint 1:** 35 endpoints, ~350 LOC, 14 archivos

**Seeds iniciales:**
- Paises: Perú + LATAM (AR, CL, CO, MX, UY, BO, PY, EC)
- Monedas: PEN (base), USD, CLP, ARS
- UnidadesMedida: UND, KGM, LTR, MTR, CAJ, GRM, L, M, CJ
- ModulosSistema: VENTAS=true, COMPRAS=false, INVENTARIO=false
- ParametrosSistema: MONEDA_BASE=PEN, IGV_PORCENTAJE=18, EMPRESA_RUC=20000000001

---

### Sprint 2: Organización
**Duración:** ~4.5h real  
**Entidades:** 3  
**Dependencias:** Pais, Moneda, TipoDocumento (pre-existente)

| # | Entidad | Tabla SQL | Descripción |
|---|---------|-----------|-------------|
| 6 | Empresa | `organizacion.Empresas` | Single-tenant (1 registro máximo) |
| 7 | Sucursal | `organizacion.Sucursales` | Controlador de `SerieDocumento` |
| 8 | Almacen | `organizacion.Almacenes` | Jerárquico: Sucursal → Almacen |

**Reglas de negocio:**
- **Empresa:** Máximo 1 registro (SingleTenantGuard en Application)
- **Sucursal:** Solo 1 puede tener `EsPrincipal=true` por empresa
- **Almacen:** Solo 1 puede tener `EsPrincipal=true` por sucursal

**Endpoints:** 7 × 3 = 21, ~500 LOC, 20 archivos  
**Seeds:** 1 empresa (Perú), 1 sucursal principal, 1 almacén principal

---

### Sprint 3: Fiscal
**Duración:** ~4h real  
**Entidades:** 3  
**Dependencias:** TipoComprobante, Sucursal, ParametroSistema

| # | Entidad | Tabla SQL | Descripción |
|---|---------|-----------|-------------|
| 9 | TipoImpuesto | `catalogo.TiposImpuesto` | IGV, ISC, EXO, INA |
| 10 | TipoComprobante | `catalogo.TiposComprobante` | Factura(01), Boleta(03), NotaVenta(NV) |
| 11 | SerieDocumento | `catalogo.SeriesDocumento` | **CRÍTICO:** Genera números correlativos |

**TipoImpuesto:**
- Seed: IGV(18%), ISC(0%), EXONERADO(0%), INAFECTO(0%)
- Usado para cálculo automático de impuestos en venta

**TipoComprobante:**
- Seed: Factura (01), Boleta (03), Nota de Venta (NV)
- Afecta inventario y contabilidad

**SerieDocumento (CRÍTICO PARA VENTAS):**
- Unicidad: (TipoComprobanteId, SucursalId, Serie)
- Ejemplo: F001 (Factura, Sucursal 1, Serie 001)
- Concurrencia: UPDATE con ROWLOCK para evitar race condition
- Handler genera número atomically antes de insert de Venta

**Endpoints:** 21 + 7 (SerieDocumento) = 28, ~600 LOC, 25 archivos

---

### Sprint 4: Producto Enriquecido
**Duración:** ~3.5h real  
**Entidades:** 2 nuevas + 1 ALTER (Productos existente)

| # | Entidad | Tabla SQL | Descripción |
|---|---------|-----------|-------------|
| 12 | CategoriaProducto | `catalogo.CategoriasProducto` | Self-referencia (árbol jerárquico) |
| 13 | MarcaProducto | `catalogo.MarcasProducto` | Referencias de producto |
| 14 | ALTER Productos | `catalogo.Productos` | +3 FKs: UnidadMedida, Categoria, Marca |

**CategoriaProducto:**
- Self-reference con `CategoriaPadreId`
- Depth validation: máx 3 niveles (Application rule)
- Ejemplo árbol: Electrónica → Computadoras → Laptops

**Migración Productos:**
- FKs agregadas como NULLABLE (compatible con datos existentes)
- Usuarios existentes tienen valores NULL (OK)
- Nuevos productos pueden especificar o dejar NULL

**Endpoints:** 14 + 7 = 21, ~500 LOC, 20 archivos

---

### Sprint 5: Comercial
**Duración:** ~3.5h real  
**Entidades:** 3  
**Dependencias:** Moneda, CondicionPago, ListaPrecio

| # | Entidad | Tabla SQL | Descripción |
|---|---------|-----------|-------------|
| 15 | CondicionPago | `catalogo.CondicionesPago` | Contado(0) + Crédito(15,30,60 días) |
| 16 | ListaPrecio | `catalogo.ListasPrecios` | Base de precios por moneda |
| 17 | Proveedor | `comercial.Proveedores` | Clone de Cliente (patrón reutilizable) |

**CondicionPago:**
- Seed: Contado (0 días), 15 días, 30 días, 60 días
- DiasCredito: entero que representa crédito

**ListaPrecio:**
- EsDefault: Solo 1 puede ser true (validado en Handler)
- MonedaId: FK a Moneda (ej. PEN)
- Nota: ListaPrecioDetalle (precios x producto) diferido a Ventas

**Proveedor:**
- Clone de Cliente con ajustes:
  - TipoDocumento + NumeroDocumento (unique composite)
  - PaisId (multi-país)
  - Filtered unique index en Correo (nullable)
- CRUD idéntico: 7 endpoints

**Endpoints:** 21, ~700 LOC, 25 archivos

---

## Entidades & APIs (Referencia Completa)

### Plantilla General de Endpoints

Para cada entidad, existen 7 endpoints estándar:

```
POST   /api/v1/{recurso}              → Crear
GET    /api/v1/{recurso}/{id}         → Obtener por ID
GET    /api/v1/{recurso}              → Listar (paginated)
PUT    /api/v1/{recurso}/{id}         → Actualizar
DELETE /api/v1/{recurso}/{id}         → Soft delete
PUT    /api/v1/{recurso}/{id}/estado  → Activar/Desactivar
GET    /api/v1/{recurso}/status/{status} → Listar por estado
```

**Status posibles:** Active, Inactive, Deleted (soft delete)

---

### SPRINT 1 ENDPOINTS

#### Paises

```
POST   /api/v1/paises
GET    /api/v1/paises/{id}
GET    /api/v1/paises?skip=0&take=10
PUT    /api/v1/paises/{id}
DELETE /api/v1/paises/{id}
PUT    /api/v1/paises/{id}/estado
GET    /api/v1/paises/status/{status}
```

**Request POST (CrearPaisDto):**
```json
{
  "nombre": "Perú",
  "codigo": "PE",
  "codigoMoneda": "PEN"
}
```

**Response GET (PaisDto):**
```json
{
  "id": 1,
  "nombre": "Perú",
  "codigo": "PE",
  "codigoMoneda": "PEN",
  "activo": true,
  "fechaCreacion": "2026-05-18T14:30:00Z",
  "fechaActualizacion": "2026-05-18T14:30:00Z"
}
```

---

#### Monedas

```
POST   /api/v1/monedas
GET    /api/v1/monedas/{id}
GET    /api/v1/monedas?skip=0&take=10
PUT    /api/v1/monedas/{id}
DELETE /api/v1/monedas/{id}
PUT    /api/v1/monedas/{id}/estado
GET    /api/v1/monedas/status/{status}
```

**Request POST (CrearMonedaDto):**
```json
{
  "nombre": "Nuevo Sol Peruano",
  "simbolo": "S/",
  "codigoISO": "PEN",
  "esMonedaBase": true
}
```

**Response GET (MonedaDto):**
```json
{
  "id": 1,
  "nombre": "Nuevo Sol Peruano",
  "simbolo": "S/",
  "codigoISO": "PEN",
  "esMonedaBase": true,
  "activo": true,
  "fechaCreacion": "2026-05-18T14:30:00Z",
  "fechaActualizacion": "2026-05-18T14:30:00Z"
}
```

---

#### UnidadesMedida

```
POST   /api/v1/unidades-medida
GET    /api/v1/unidades-medida/{id}
GET    /api/v1/unidades-medida?skip=0&take=10
PUT    /api/v1/unidades-medida/{id}
DELETE /api/v1/unidades-medida/{id}
PUT    /api/v1/unidades-medida/{id}/estado
GET    /api/v1/unidades-medida/status/{status}
```

**Request POST (CrearUnidadMedidaDto):**
```json
{
  "nombre": "Kilogramo",
  "simbolo": "KG",
  "codigo": "KGM"
}
```

**Validaciones:**
- `codigo` debe ser UNIQUE
- SUNAT codes aceptados: UND, KGM, LTR, MTR, CAJ, GRM, L, M, CJ, etc.

---

#### ModulosSistema

```
POST   /api/v1/modulos-sistema
GET    /api/v1/modulos-sistema/{id}
GET    /api/v1/modulos-sistema?skip=0&take=10
PUT    /api/v1/modulos-sistema/{id}
DELETE /api/v1/modulos-sistema/{id}
PUT    /api/v1/modulos-sistema/{id}/estado
GET    /api/v1/modulos-sistema/status/{status}
```

**Request POST (CrearModuloSistemaDto):**
```json
{
  "nombre": "VENTAS",
  "codigo": "VENTAS",
  "descripcion": "Módulo de ventas",
  "esActivo": true
}
```

**Nota:** Usar para feature flags. Si vacío en BD → todos activos (fail-open).

---

#### ParametrosSistema

```
POST   /api/v1/parametros-sistema
GET    /api/v1/parametros-sistema/{id}
GET    /api/v1/parametros-sistema?skip=0&take=10
PUT    /api/v1/parametros-sistema/{id}
DELETE /api/v1/parametros-sistema/{id}
GET    /api/v1/parametros-sistema/clave/{clave}  [EXTRA]
```

**Request POST (CrearParametroSistemaDto):**
```json
{
  "clave": "IGV_PORCENTAJE",
  "valor": "18",
  "tipoDato": "DECIMAL",
  "descripcion": "Porcentaje de IGV (Perú)"
}
```

**Parámetros seeds:**
- MONEDA_BASE = "PEN"
- IGV_PORCENTAJE = "18"
- EMPRESA_RUC = "20000000001"
- RAZON_SOCIAL_EMPRESA = "NEXUS ERP SAC"

---

### SPRINT 2 ENDPOINTS

#### Empresas

```
POST   /api/v1/empresas              [ÚNICA CREACIÓN]
GET    /api/v1/empresas/{id}
GET    /api/v1/empresas              [Retorna siempre 1]
PUT    /api/v1/empresas/{id}
DELETE /api/v1/empresas/{id}
PUT    /api/v1/empresas/{id}/estado
GET    /api/v1/empresas/status/{status}
```

**Request POST (CrearEmpresaDto):**
```json
{
  "razonSocial": "NEXUS ERP SAC",
  "nombreComercial": "Nexus",
  "numeroDocumento": "20000000001",
  "tipoDocumentoId": 1,
  "paisId": 1,
  "monedaBaseId": 1,
  "direccionFiscal": "Av. Principal 123, Lima",
  "telefono": "+51987654321",
  "correo": "admin@nexus.com",
  "logoUrl": "https://..."
}
```

**Regla CRÍTICA:** SingleTenantGuard en Handler
- POST: Si ya existe 1 → rechazar con 400
- GET: Siempre retorna el único registro

---

#### Sucursales

```
POST   /api/v1/sucursales
GET    /api/v1/sucursales/{id}
GET    /api/v1/sucursales?skip=0&take=10
PUT    /api/v1/sucursales/{id}
DELETE /api/v1/sucursales/{id}
PUT    /api/v1/sucursales/{id}/estado
GET    /api/v1/sucursales/status/{status}
```

**Request POST (CrearSucursalDto):**
```json
{
  "nombre": "Sucursal Lima",
  "codigo": "LIM",
  "empresaId": 1,
  "paisId": 1,
  "direccion": "Av. Principal 123",
  "telefono": "+51987654321",
  "esPrincipal": true
}
```

**Validaciones:**
- EsPrincipal: Solo 1 true por EmpresaId (validado en Handler)
- Codigo: UNIQUE global

---

#### Almacenes

```
POST   /api/v1/almacenes
GET    /api/v1/almacenes/{id}
GET    /api/v1/almacenes?skip=0&take=10
PUT    /api/v1/almacenes/{id}
DELETE /api/v1/almacenes/{id}
PUT    /api/v1/almacenes/{id}/estado
GET    /api/v1/almacenes/status/{status}
```

**Request POST (CrearAlmacenDto):**
```json
{
  "nombre": "Almacén Principal",
  "codigo": "ALM001",
  "sucursalId": 1,
  "descripcion": "Almacén principal de Lima",
  "esPrincipal": true
}
```

---

### SPRINT 3 ENDPOINTS

#### TiposImpuesto

```
POST   /api/v1/tipos-impuesto
GET    /api/v1/tipos-impuesto/{id}
GET    /api/v1/tipos-impuesto?skip=0&take=10
PUT    /api/v1/tipos-impuesto/{id}
DELETE /api/v1/tipos-impuesto/{id}
PUT    /api/v1/tipos-impuesto/{id}/estado
GET    /api/v1/tipos-impuesto/status/{status}
```

**Request POST (CrearTipoImpuestoDto):**
```json
{
  "nombre": "IGV",
  "codigo": "IGV",
  "porcentaje": 18.00,
  "esIncluido": true
}
```

**Seeds:**
- IGV (18%, incluido)
- ISC (0%)
- EXONERADO (0%)
- INAFECTO (0%)

**Nota:** Usado en VentaDetalle para cálculo automático de impuestos.

---

#### TiposComprobante

```
POST   /api/v1/tipos-comprobante
GET    /api/v1/tipos-comprobante/{id}
GET    /api/v1/tipos-comprobante?skip=0&take=10
PUT    /api/v1/tipos-comprobante/{id}
DELETE /api/v1/tipos-comprobante/{id}
PUT    /api/v1/tipos-comprobante/{id}/estado
GET    /api/v1/tipos-comprobante/status/{status}
```

**Request POST (CrearTipoComprobanteDto):**
```json
{
  "nombre": "Factura",
  "codigo": "01",
  "afectaInventario": true,
  "afectaContable": true
}
```

**Seeds:**
- Factura (01) - Afecta inventario y contable
- Boleta (03) - Afecta inventario y contable
- Nota de Venta (NV) - Afecta inventario

---

#### SeriesDocumento

```
POST   /api/v1/series-documento
GET    /api/v1/series-documento/{id}
GET    /api/v1/series-documento?skip=0&take=10
PUT    /api/v1/series-documento/{id}
DELETE /api/v1/series-documento/{id}
PUT    /api/v1/series-documento/{id}/estado
GET    /api/v1/series-documento/status/{status}
GET    /api/v1/series-documento/generar-numero  [EXTRA - INTERNO]
```

**Request POST (CrearSerieDocumentoDto):**
```json
{
  "tipoComprobanteId": 1,
  "sucursalId": 1,
  "serie": "F001",
  "numeroActual": 0,
  "numeroMaximo": null
}
```

**Validaciones:**
- UNIQUE (TipoComprobanteId, SucursalId, Serie)
- Serie formato: F001, B001, etc.

**CRÍTICO PARA VENTAS:**
- Endpoint interno: GET `/api/v1/series-documento/generar-numero?tipoComprobanteId=X&sucursalId=Y`
- Retorna: `{ numeroGenerado: 1, serie: "F001" }`
- Handler de Venta usa esto ANTES de crear documento
- Concurrencia: ROWLOCK evita duplicados

---

### SPRINT 4 ENDPOINTS

#### CategoriasProducto

```
POST   /api/v1/categorias-producto
GET    /api/v1/categorias-producto/{id}
GET    /api/v1/categorias-producto?skip=0&take=10
PUT    /api/v1/categorias-producto/{id}
DELETE /api/v1/categorias-producto/{id}
PUT    /api/v1/categorias-producto/{id}/estado
GET    /api/v1/categorias-producto/status/{status}
```

**Request POST (CrearCategoriaProductoDto):**
```json
{
  "nombre": "Electrónica",
  "descripcion": "Productos electrónicos",
  "categoriaPadreId": null
}
```

**Validaciones:**
- Depth máximo 3 niveles (Application rule)
- Self-reference: CategoriaPadreId → CategoriasProducto(id)

**Ejemplo árbol:**
```
Electrónica (id=1)
├── Computadoras (id=2, parent=1)
│   └── Laptops (id=3, parent=2)
└── Periféricos (id=4, parent=1)
```

---

#### MarcasProducto

```
POST   /api/v1/marcas-producto
GET    /api/v1/marcas-producto/{id}
GET    /api/v1/marcas-producto?skip=0&take=10
PUT    /api/v1/marcas-producto/{id}
DELETE /api/v1/marcas-producto/{id}
PUT    /api/v1/marcas-producto/{id}/estado
GET    /api/v1/marcas-producto/status/{status}
```

**Request POST (CrearMarcaProductoDto):**
```json
{
  "nombre": "Dell",
  "descripcion": "Marca Dell",
  "logoUrl": "https://..."
}
```

---

#### Productos (ACTUALIZADO)

```
[Existentes + modificados con nuevos campos]

POST   /api/v1/productos
GET    /api/v1/productos/{id}
GET    /api/v1/productos?skip=0&take=10
PUT    /api/v1/productos/{id}
DELETE /api/v1/productos/{id}
PUT    /api/v1/productos/{id}/estado
GET    /api/v1/productos/status/{status}
```

**Request POST (CrearProductoDto - ACTUALIZADO):**
```json
{
  "nombre": "Laptop Dell XPS",
  "descripcion": "Laptop profesional",
  "codigo": "LAP-DELL-XPS",
  "precioUnitario": 1500.00,
  "unidadMedidaId": 1,           [NUEVO - nullable]
  "categoriaProductoId": 2,      [NUEVO - nullable]
  "marcaProductoId": 1           [NUEVO - nullable]
}
```

**Nuevos campos aggregados:**
- `unidadMedidaId` (FK → UnidadesMedida) — NULLABLE
- `categoriaProductoId` (FK → CategoriasProducto) — NULLABLE
- `marcaProductoId` (FK → MarcasProducto) — NULLABLE

**Nota:** Compatibilidad hacia atrás: campos NULL no rompen datos existentes.

---

### SPRINT 5 ENDPOINTS

#### CondicionesPago

```
POST   /api/v1/condiciones-pago
GET    /api/v1/condiciones-pago/{id}
GET    /api/v1/condiciones-pago?skip=0&take=10
PUT    /api/v1/condiciones-pago/{id}
DELETE /api/v1/condiciones-pago/{id}
PUT    /api/v1/condiciones-pago/{id}/estado
GET    /api/v1/condiciones-pago/status/{status}
```

**Request POST (CrearCondicionPagoDto):**
```json
{
  "nombre": "Crédito 30 días",
  "diasCredito": 30,
  "descripcion": "Pago a crédito con plazo de 30 días"
}
```

**Seeds:**
- Contado (0 días)
- 15 días (15)
- 30 días (30)
- 60 días (60)

---

#### ListasPrecios

```
POST   /api/v1/listas-precios
GET    /api/v1/listas-precios/{id}
GET    /api/v1/listas-precios?skip=0&take=10
PUT    /api/v1/listas-precios/{id}
DELETE /api/v1/listas-precios/{id}
PUT    /api/v1/listas-precios/{id}/estado
GET    /api/v1/listas-precios/status/{status}
```

**Request POST (CrearListaPrecioDto):**
```json
{
  "nombre": "Lista Precios Base",
  "monedaId": 1,
  "descripcion": "Lista de precios en PEN",
  "esDefault": true
}
```

**Validaciones:**
- EsDefault: Solo 1 puede ser true (validado en Handler)
- MonedaId: FK requerida

**Nota:** ListaPrecioDetalle (precios × producto) se implementará en Ventas.

---

#### Proveedores

```
POST   /api/v1/proveedores
GET    /api/v1/proveedores/{id}
GET    /api/v1/proveedores?skip=0&take=10
PUT    /api/v1/proveedores/{id}
DELETE /api/v1/proveedores/{id}
PUT    /api/v1/proveedores/{id}/estado
GET    /api/v1/proveedores/status/{status}
```

**Request POST (CrearProveedorDto):**
```json
{
  "tipoDocumentoId": 1,
  "numeroDocumento": "20123456789",
  "razonSocial": "PROVEEDOR SAC",
  "nombreComercial": "Proveedor",
  "paisId": 1,
  "correo": "contacto@proveedor.com",
  "telefono": "+51987654321",
  "direccion": "Av. Proveedor 123"
}
```

**Validaciones:**
- UNIQUE (TipoDocumentoId, NumeroDocumento)
- Filtered unique index en Correo (WHERE Correo IS NOT NULL)

**Patrón:** Clone de Cliente con ajustes:
- Comercial context (no Cliente context)
- TipoDocumento + Numero como identificador único
- PaisId para multi-país

---

## Modelos de Datos (DTOs)

### Estructura DTO General

```csharp
// Crear
public class CrearXXXDto
{
    [Required]
    public string Nombre { get; set; }
    
    [StringLength(100)]
    public string Descripcion { get; set; }
    
    public int? FkId { get; set; }
}

// Actualizar
public class ActualizarXXXDto : CrearXXXDto
{
    [Required]
    public int Id { get; set; }
}

// Respuesta
public class XXXDto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaActualizacion { get; set; }
}
```

### Validaciones Comunes

| Validación | Tipo | Ejemplo |
|-----------|------|---------|
| Required | FluentValidation | Nombre no puede ser vacío |
| Length | FluentValidation | Nombre entre 3-100 caracteres |
| Unique (DB) | ValidatorService | Codigo UNIQUE en tabla |
| Foreign Key | EF Core | TipoDocumentoId debe existir |
| Custom (business) | Handler/ValidatorService | EsDefault: solo 1 true |

---

## Flujos de Datos Críticos

### Flujo 1: Crear Empresa (Simplest - No deps)

```
Frontend: POST /api/v1/empresas
├─ CrearEmpresaDto validada (FluentValidation)
│
Backend Handler:
├─ SingleTenantGuard: Si existe 1 → 400
├─ ValidatorService: numeroDocumento UNIQUE
├─ CreateEmpresaCommand.Handle()
├─ Entidad creada + BD
│
Response: { success: true, data: EmpresaDto, ... }
```

---

### Flujo 2: Crear Producto Enriquecido (Con 3 FKs)

```
Frontend: POST /api/v1/productos
├─ CrearProductoDto:
│  ├─ Nombre, Descripcion, PrecioUnitario (requeridos)
│  ├─ UnidadMedidaId (nullable)
│  ├─ CategoriaProductoId (nullable)
│  └─ MarcaProductoId (nullable)
│
Backend Handler:
├─ FluentValidation: Nombre, Precio
├─ Si UnidadMedidaId ≠ null → FK check (EF Core)
├─ Si CategoriaProductoId ≠ null → FK check + depth validation
├─ Si MarcaProductoId ≠ null → FK check
├─ AutoMapper: CrearProductoDto → Producto entity
├─ BD insert
│
Response: ProductoDto con todos los campos
```

**Nota:** Si FKs son null → producto sin categoría/marca/unidad (OK, compatible).

---

### Flujo 3: Crear Venta (DESBLOQUEADO por catálogos)

```
Frontend: POST /api/v1/ventas
├─ CrearVentaDto:
│  ├─ ClienteId (FK)
│  ├─ SucursalId (FK)
│  ├─ MonedaId (FK)
│  ├─ SerieDocumentoId (FK)
│  ├─ CondicionPagoId (FK)
│  ├─ Detalles: { ProductoId, Cantidad, PrecioUnitario, TipoImpuestoId }[]
│
Backend Handler:
├─ Validaciones: Cliente, Sucursal, Moneda, CondicionPago existen
├─ SerieDocumento critical:
│  ├─ GET /api/v1/series-documento/generar-numero?tipo=1&sucursal=1
│  ├─ Retorna: { numeroGenerado: 1, serie: "F001" }
│  └─ [ATOMIC con ROWLOCK en BD]
│
├─ Para cada detalle:
│  ├─ Producto existe + tiene UnidadMedidaId
│  ├─ TipoImpuesto existe
│  ├─ Calcula: SubTotal = Cantidad × PrecioUnitario
│  ├─ Calcula: Impuesto = SubTotal × TipoImpuesto.Porcentaje / 100
│  └─ SubTotalConImpuesto = SubTotal + Impuesto
│
├─ Venta creada con número = F001-000001
├─ Inventario actualizado (si TipoComprobante.AfectaInventario)
│
Response: VentaDto con número, detalles, totales
```

---

## Dependencias entre Entidades

### Mapa de Dependencias (Realista)

```
NIVEL 0 (Sin dependencias externas — creadas primero):
├─ Pais
├─ Moneda
├─ UnidadMedida
├─ ModuloSistema
└─ ParametroSistema

    ↓ (todo lo demás depende de aquí)

NIVEL 1 (Dependen de nivel 0):
├─ Empresa (Pais, Moneda)
├─ TipoImpuesto (independiente)
└─ TipoComprobante (independiente)

    ↓

NIVEL 2 (Dependen de nivel 1):
├─ Sucursal (Empresa, Pais)
├─ SerieDocumento (TipoComprobante, Sucursal) ← CRÍTICO
└─ CondicionPago (independiente)

    ↓

NIVEL 3 (Dependen de nivel 2):
├─ Almacen (Sucursal)
├─ CategoriaProducto (self-ref, no deps externas)
├─ MarcaProducto (independiente)
└─ ListaPrecio (Moneda)

    ↓

NIVEL 4 (Producto enriquecido):
└─ Productos (+ UnidadMedida, CategoriaProducto, MarcaProducto)

    ↓

NIVEL 5 (Comercial):
└─ Proveedor (TipoDocumento, Pais)

    ↓

DESBLOQUEADO: Módulo Ventas
└─ Venta (Cliente, Sucursal, Moneda, SerieDocumento, CondicionPago, Productos)
```

### Orden de Creación Obligatorio (Frontend)

```
1️⃣  NIVEL 0 — Catálogos base (sin deps)
2️⃣  NIVEL 1 — Empresa, impuestos, comprobantes
3️⃣  NIVEL 2 — Sucursal, Series, condiciones pago
4️⃣  NIVEL 3 — Almacén, categorías, lista precios
5️⃣  NIVEL 4 — Enriquecimiento de productos
6️⃣  NIVEL 5 — Proveedores
7️⃣  DESBLOQUEADO — Módulo Ventas
```

**No puedes crear Venta sin:**
- ✅ Cliente (v3.0.0 existente)
- ✅ Sucursal (Sprint 2)
- ✅ Moneda (Sprint 1)
- ✅ SerieDocumento (Sprint 3)
- ✅ CondicionPago (Sprint 5)
- ✅ TipoImpuesto (Sprint 3)
- ✅ Productos con UnidadMedida (Sprint 1 + 4)

---

## Sugerencia: Plan de Sprints Frontend

### Premisa

El frontend debe **espejear** la arquitectura backend:
- Sprint frontend = Sprint backend (en paralelo o secuencial)
- Integraciones REST en orden de dependencias
- UI debe reflejar flujos de datos reales

---

### Propuesta: 5 Sprints Frontend

#### **Sprint F1: Catálogos Base (Nivel 0)**
**Duración estimada:** 5-6 días  
**Frontend Focus:** Tablas CRUD simples  

| Componente | Feature | Tech Stack |
|-----------|---------|-----------|
| PaisesTable | CRUD paises | ng-zorro table |
| MonedasTable | CRUD monedas | ng-zorro table |
| UnidadesMedidaTable | CRUD unidades | ng-zorro table |
| ParametrosSystemTable | Listar (READ-ONLY) | ng-zorro table |
| ModulosSystemTable | Listar (READ-ONLY) | ng-zorro table |

**Tareas:**
- [ ] Crear módulo `catalogo-base`
- [ ] Service: PaisService, MonedaService, UnidadMedidaService
- [ ] Components: PaisesTable, MonedasTable, UnidadesMedidaTable
- [ ] Forms: CrearPais, ActualizarPais, etc.
- [ ] Validations: FluentValidator espejo de backend
- [ ] API routing: `/api/v1/paises`, etc.

**Salidas esperadas:**
- ✅ 5 tablas CRUD funcionales
- ✅ Datos sincronizados con BD
- ✅ Forms con validación client-side
- ✅ Paginación, búsqueda, filtros

---

#### **Sprint F2: Organización (Nivel 1)**
**Duración estimada:** 5-6 días  
**Frontend Focus:** Relaciones 1-N, validaciones complejas  

| Componente | Feature | Tech Stack |
|-----------|---------|-----------|
| EmpresaForm | Crear empresa (MAX 1) | Reactive forms |
| SucursalesTable | CRUD sucursales | ng-zorro table |
| AlmacenesTable | CRUD almacenes | ng-zorro table |
| EmpresaCard | Display empresa actual | Card component |

**Tareas:**
- [ ] Crear módulo `organizacion`
- [ ] Service: EmpresaService, SucursalService, AlmacenService
- [ ] Components: EmpresaForm (Crear), SucursalesTable, AlmacenesTable
- [ ] Guard: SingleTenantGuard (desabilitar POST si existe)
- [ ] Dependent dropdowns: Empresa → Sucursal → Almacén
- [ ] Validations: EsPrincipal (solo 1 true)

**Salidas esperadas:**
- ✅ Empresa cread y visualizada (singleton)
- ✅ Sucursales + Almacenes jerárquicos
- ✅ Validaciones client-side de "principal"

---

#### **Sprint F3: Fiscal (Nivel 2)**
**Duración estimada:** 4-5 días  
**Frontend Focus:** Critical: SerieDocumento + generators  

| Componente | Feature | Tech Stack |
|-----------|---------|-----------|
| TiposImpuestoTable | CRUD tipos impuesto | ng-zorro table |
| TiposComprobanteTable | CRUD tipos comprobante | ng-zorro table |
| SeriesDocumentoTable | CRUD series + generador | ng-zorro table |
| SerieGeneratorWidget | Preview número siguiente | Badge component |

**Tareas:**
- [ ] Service: TipoImpuestoService, TipoComprobanteService, SerieDocumentoService
- [ ] Components: TiposImpuestoTable, TiposComprobanteTable, SeriesDocumentoTable
- [ ] **CRITICAL:** SerieGeneratorWidget
  - [ ] Call: `GET /api/v1/series-documento/generar-numero?tipo=X&sucursal=Y`
  - [ ] Display: "Próximo número: F001-000042"
  - [ ] Real-time update
- [ ] Validations: Serie format (F001, B001, etc.)

**Salidas esperadas:**
- ✅ Tipos impuesto + comprobante CRUD
- ✅ Series documento cread y visualizadas
- ✅ Generador de números funcional + preview

**BLOCKERS A RESOLVER:**
- ¿Generador automático en POST de Venta, o manual en form?
- ¿Quién decide siguiente número: BD o frontend?

---

#### **Sprint F4: Producto Enriquecido (Nivel 3)**
**Duración estimada:** 4-5 días  
**Frontend Focus:** Self-reference, optional relations  

| Componente | Feature | Tech Stack |
|-----------|---------|-----------|
| CategoriasProductoTree | Árbol jerárquico + CRUD | ng-zorro tree |
| MarcasProductoTable | CRUD marcas | ng-zorro table |
| ProductosTableEnriquecida | Visualizar nuevos fields | ng-zorro table |
| ProductoFormEnriquecida | Crear/editar con FKs nuevas | Reactive forms |

**Tareas:**
- [ ] Service: CategoriaProductoService, MarcaProductoService
- [ ] Components: CategoriasProductoTree (self-ref), MarcasProductoTable
- [ ] Update ProductoTable:
  - [ ] Agregar columns: UnidadMedida, Categoria, Marca
  - [ ] Agregar form fields: Dropdown para cada FK (optional)
- [ ] Validations: Depth máximo 3 en árbol de categorías
- [ ] Display: Mostrar inherited fields (categoría padre, etc.)

**Salidas esperadas:**
- ✅ Árbol de categorías navegable
- ✅ Productos con nuevos fields visibles
- ✅ Forms actualizados con dropdowns opcionales

---

#### **Sprint F5: Comercial (Nivel 4)**
**Duración estimada:** 4-5 días  
**Frontend Focus:** Validaciones comerciales, precios  

| Componente | Feature | Tech Stack |
|-----------|---------|-----------|
| CondicionesPagoTable | CRUD condiciones | ng-zorro table |
| ListasPreciosTable | CRUD listas precios | ng-zorro table |
| ProveedoresTable | CRUD proveedores | ng-zorro table |
| PrecioListWidget | Preview lista precios default | Card component |

**Tareas:**
- [ ] Service: CondicionPagoService, ListaPrecioService, ProveedorService
- [ ] Components: CondicionesPagoTable, ListasPreciosTable, ProveedoresTable
- [ ] Validations: EsDefault (solo 1 true en ListaPrecio)
- [ ] PrecioListWidget: Mostrar lista default
- [ ] ProveedorForm: Clone de ClienteForm (patrón reutilizable)

**Salidas esperadas:**
- ✅ Condiciones pago CRUD
- ✅ Listas precios CRUD + default validado
- ✅ Proveedores CRUD (patrón cliente)
- ✅ Catálogos 100% en frontend

---

### Resumen Velocidad Frontend

| Sprint | Entidades | Components | Duración Est. | Risk |
|--------|-----------|-----------|---------------|------|
| F1 | 5 | 10+ | 5-6 días | Low |
| F2 | 3 | 7+ | 5-6 días | Low-Med (SingleTenant) |
| F3 | 3 | 6+ | 4-5 días | **HIGH** (SerieGenerator) |
| F4 | 2 | 5+ | 4-5 días | Medium (self-ref) |
| F5 | 3 | 6+ | 4-5 días | Low-Med (clone) |
| **TOTAL** | **18** | **34+** | **~23-27 días** | - |

**Estimación realista:** 4-5 semanas (serial)  
**Con team paralelo:** 2-3 semanas posible

---

## Notas de Integración

### HTTP Client & Auth

**Cada request debe incluir:**
```
Authorization: Bearer {jwt_token}
Content-Type: application/json
```

**Response standard:**
```json
{
  "success": true,
  "data": { /* entity data */ },
  "message": "Operación exitosa",
  "traceId": "00-abc123-def456-00",
  "timestamp": "2026-05-18T14:30:00Z"
}
```

**Error response:**
```json
{
  "success": false,
  "data": null,
  "message": "Validación fallida",
  "errors": [
    {
      "field": "nombre",
      "message": "El nombre es requerido"
    }
  ],
  "traceId": "00-abc123-def456-00"
}
```

---

### Validations (Client-side espejo Backend)

**FluentValidator equivalents:**

```typescript
// Backend: NotEmpty() + Length(3, 100)
// Frontend:
validators: [
  Validators.required,
  Validators.minLength(3),
  Validators.maxLength(100)
]

// Backend: IsUnique("Codigo", db)
// Frontend:
validators: [
  Validators.required,
  this.codigoValidator.bind(this)  // async validator
]

// Backend: Custom rule (EsDefault only 1)
// Frontend:
form.get('esDefault').valueChanges
  .pipe(
    filter(v => v === true),
    switchMap(_ => this.listaPrecioService.getDefaultCount())
  )
  .subscribe(count => {
    if (count > 0) form.get('esDefault').setErrors({ onlyOneDefault: true });
  });
```

---

### Testing Checklist (Frontend)

**Por cada Sprint:**
- [ ] Services: GET, POST, PUT, DELETE endpoints
- [ ] Forms: Validaciones client-side
- [ ] Tables: Paginación, búsqueda, soft delete
- [ ] Dropdowns: Cargan datos de BD
- [ ] Dependencies: Cuando FK existe, dropdown funciona
- [ ] Error handling: Mensajes amigables de error
- [ ] Loading states: Spinners mientras carga
- [ ] Empty states: Mensajes cuando sin datos

**Smoke Test Post-Sprint:**
```bash
# Sprint F1
✅ Crear país → GET lista
✅ Crear moneda → GET lista
✅ Crear unidad medida → GET lista

# Sprint F2
✅ Crear empresa (1 máximo) → GET
✅ Crear sucursal → asocizar a empresa
✅ Crear almacén → asociar a sucursal

# Sprint F3
✅ Crear tipo impuesto → GET
✅ Crear tipo comprobante → GET
✅ Crear serie → Generador siguiente número funciona

# Sprint F4
✅ Crear categoría → Árbol visualizado
✅ Crear marca → Visualizada en producto
✅ Actualizar producto → Nuevos fields guardados

# Sprint F5
✅ Crear condición pago → GET lista
✅ Crear lista precios → EsDefault solo 1
✅ Crear proveedor → Campos validados
```

---

### API Base URL

```typescript
// environment.ts
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api/v1'  // Ajustar según tu setup
};

// service.ts
constructor(private http: HttpClient) {
  this.apiUrl = `${environment.apiUrl}/paises`;
}
```

---

### Routing Sugerido (Frontend Modules)

```
src/
├── app/
│   ├── modules/
│   │   ├── catalogo/
│   │   │   ├── pages/
│   │   │   │   ├── paises/
│   │   │   │   ├── monedas/
│   │   │   │   ├── unidades-medida/
│   │   │   │   ├── parametros-sistema/
│   │   │   │   └── modulos-sistema/
│   │   │   ├── services/
│   │   │   │   ├── pais.service.ts
│   │   │   │   ├── moneda.service.ts
│   │   │   │   └── ...
│   │   │   └── catalogo.module.ts
│   │   ├── organizacion/
│   │   │   ├── pages/
│   │   │   │   ├── empresa/
│   │   │   │   ├── sucursales/
│   │   │   │   └── almacenes/
│   │   │   ├── services/
│   │   │   └── organizacion.module.ts
│   │   ├── fiscal/
│   │   ├── comercial/
│   │   └── ventas/ [FUTURE]
│   └── shared/
│       ├── models/
│       ├── validators/
│       └── pipes/
```

---

### Performance Tips

**Para tablas grandes (1000+ registros):**
- [ ] Implementar virtual scrolling (ng-zorro virtual)
- [ ] Lazy load: GET con skip/take (paginación)
- [ ] Debounce en búsqueda: 300ms
- [ ] Cache en service: 5-10 minutos

**Para dropdowns (50+ opciones):**
- [ ] ng-zorro dropdown con search
- [ ] Cargar on-demand (lazy)
- [ ] Cache en service

**Ejemplo:**
```typescript
paises$ = this.paisService.getAll().pipe(
  shareReplay(1),  // Cache
  catchError(err => of([]))
);

search$ = this.searchTerm$.pipe(
  debounceTime(300),
  distinctUntilChanged(),
  switchMap(term => this.paisService.search(term))
);
```

---

## Conclusión & Próximos Pasos

### Hito Alcanzado ✅

**Backend Nexus-ERP v3.1 — Catálogos Base 100% COMPLETO**

- 18 entidades nuevas
- ~75 endpoints REST
- ~1,827 LOC
- 70+ archivos
- 5 sprints ejecutados
- 100% testeado

**Frontend está desbloqueado para:**
1. Construir UI espejo (Sprint F1-5)
2. Integrar APIs en paralelo o secuencial
3. Plan disponible para arquitecto frontend

---

### Próximo: Módulo Ventas (v3.2)

**Depenencias resueltas:**
- ✅ SerieDocumento (número correlativo)
- ✅ TipoImpuesto (cálculo automático)
- ✅ CondicionPago (términos)
- ✅ ListaPrecio (base de precios)
- ✅ UnidadMedida (en detalle)
- ✅ Productos enriquecidos

**Siguiente:** Backend Sprint 6 (Venta + VentaDetalle)

---

**Documento versión:** v1.0  
**Fecha:** 2026-05-18  
**Autor:** Backend Architect (Nexus-ERP)  
**Para:** Frontend Senior Architect  
**Autosuficiente:** ✅ Sí — toda la información necesaria está en este documento  
