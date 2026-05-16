# Sprint 3: Fiscal (TipoImpuesto, TipoComprobante, SerieDocumento)

**Estado:** ⏳ **PENDIENTE**  
**Fecha Estimada Inicio:** 2026-05-17  
**Duración Estimada:** 8-10 horas  
**Rama:** `catalogo-base/sprint_3`  
**Complejidad:** 🔴 **ALTA** (SerieDocumento con concurrencia crítica)

---

## 📋 Objetivo

Implementar catálogos fiscales que sustentan módulo Ventas v3.1:
- **TipoImpuesto**: Porcentajes de impuesto (IGV, ISC, EXONERADO, INAFECTO)
- **TipoComprobante**: Tipos de documento (Factura, Boleta, Nota Venta)
- **SerieDocumento**: ⚠️ **CRÍTICO** — Generador de números correlativivos con manejo de concurrencia

**Dependencias:** Sprint 1 (Moneda), Sprint 2 (Sucursal)  
**Bloquea:** Módulo Ventas v3.1

---

## 🎯 Entidades a Crear (3)

### 1. TipoImpuesto → `catalogo.TiposImpuesto`

```
Nombre              NVARCHAR(100) NOT NULL
Codigo              NVARCHAR(10) NOT NULL UNIQUE      -- IGV, ISC, EXONERADO, INAFECTO
Porcentaje          DECIMAL(5,2) NOT NULL DEFAULT 0.00
EsIncluido          BIT NOT NULL DEFAULT 1             -- ¿Incluido en precio o agregado?
PublicId            GUID (via AuditableEntity)
Activo              BIT DEFAULT 1
```

**Características:**
- Catálogo de referencia (lectura dominante)
- Seed obligatorio: IGV=18%, ISC=0%, EXONERADO=0%, INAFECTO=0%
- CRUD completo (POST/PUT/DELETE permitidos a ADMIN)

---

### 2. TipoComprobante → `catalogo.TiposComprobante`

```
Nombre              NVARCHAR(100) NOT NULL
Codigo              NVARCHAR(5) NOT NULL UNIQUE        -- 01=Factura, 03=Boleta, NV=Nota
AfectaInventario    BIT NOT NULL DEFAULT 1
AfectaContable      BIT NOT NULL DEFAULT 1
PublicId            GUID (via AuditableEntity)
Activo              BIT DEFAULT 1
```

**Características:**
- Catálogo de referencia (lectura dominante)
- Flags de impacto para lógica de Ventas
- Seed obligatorio: Factura(01), Boleta(03), Nota Venta(NV)

---

### 3. SerieDocumento → `catalogo.SeriesDocumento` ⚠️ **CRÍTICO**

```
TipoComprobanteId   INT NOT NULL → FK catalogo.TiposComprobante (RESTRICT)
SucursalId          INT NOT NULL → FK organizacion.Sucursales (RESTRICT)
Serie               NVARCHAR(4) NOT NULL              -- Ej: F001, B001, T001
NumeroActual        INT NOT NULL DEFAULT 0             -- Última secuencia usada
NumeroMaximo        INT NULL                           -- NULL = sin límite
PublicId            GUID (via AuditableEntity)
Activo              BIT DEFAULT 1

-- UNIQUE (TipoComprobanteId, SucursalId, Serie)
```

**Características:**
- **Concurrencia crítica**: NumeroActual se incrementa con UPDATE `ROWLOCK + UPDLOCK`
- Handler genera número ANTES de insertar Venta (no después)
- Transacción `SERIALIZABLE` para evitar race conditions
- Seed obligatorio: F001 y B001 para sucursal principal

---

## 🚨 Riesgo Crítico: SerieDocumento Race Condition

### Escenario del Problema

```
Usuario A y B crean venta simultáneamente en misma serie

T1: Usuario A:   SELECT NumeroActual FROM SeriesDocumento (valor: 100)
T2: Usuario B:   SELECT NumeroActual FROM SeriesDocumento (valor: 100)
T3: Usuario A:   UPDATE NumeroActual = 101 → INSERT Venta con número 101
T4: Usuario B:   UPDATE NumeroActual = 101 → INSERT Venta con número 101
     
RESULTADO: ❌ Dos ventas con mismo número de comprobante (DUPLICADO)
```

### Mitigación Requerida

En `SerieDocumentoHandler.GetNextNumeroAsync()`:

```sql
-- SQL con concurrencia segura
BEGIN TRANSACTION SERIALIZABLE

UPDATE catalogo.SeriesDocumento WITH (ROWLOCK, UPDLOCK)
SET NumeroActual = NumeroActual + 1
WHERE Id = @SerieId
  AND (NumeroMaximo IS NULL OR NumeroActual < NumeroMaximo)

SELECT NumeroActual FROM catalogo.SeriesDocumento
WHERE Id = @SerieId

COMMIT
```

**En código C#:**
```csharp
public async Task<int> GetNextNumeroAsync(int serieId)
{
    using var transaction = await _dbContext.Database
        .BeginTransactionAsync(IsolationLevel.Serializable);
    
    try
    {
        // UPDATE atómico con ROWLOCK
        var serie = await _dbContext.SeriesDocumento
            .FromSqlInterpolated($@"
                UPDATE catalogo.SeriesDocumento WITH (ROWLOCK, UPDLOCK)
                SET NumeroActual = NumeroActual + 1
                WHERE Id = {serieId}
                    AND (NumeroMaximo IS NULL OR NumeroActual < NumeroMaximo)
                
                SELECT * FROM catalogo.SeriesDocumento
                WHERE Id = {serieId}
            ")
            .FirstOrDefaultAsync();
        
        await transaction.CommitAsync();
        return serie.NumeroActual;
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}
```

---

## 📁 Archivos a Crear: ~20 nuevos

### Commands (12)
- Crear (3): CrearTipoImpuestoCommand, CrearTipoComprobanteCommand, CrearSerieDocumentoCommand
- Actualizar (3): ActualizarTipoImpuestoCommand, ActualizarTipoComprobanteCommand, ActualizarSerieDocumentoCommand
- ActualizarEstado (3)
- Eliminar (3)

### Handlers (12)
- Crear (3): Con lógica estándar
- Actualizar (3): Con lógica estándar
- ActualizarEstado (3): Con lógica estándar
- Eliminar (3): Con lógica estándar
- **SerieDocumento Especial**: GetNextNumeroHandler con transacción SERIALIZABLE

### Validators (6)
- Crear/Actualizar para cada entidad

### ValidatorServices (3)
- TipoImpuestoValidatorService, TipoComprobanteValidatorService, SerieDocumentoValidatorService

### DTOs (9)

### AutoMapper Profiles (3)

### Services (6)

### Entity Configurations (3)

### Controllers (3 = 21 endpoints)

### Database Scripts (4)
- `Database/02_Tablas/10_TiposImpuesto.sql`
- `Database/02_Tablas/11_TiposComprobante.sql`
- `Database/02_Tablas/12_SeriesDocumento.sql`
- `Database/03_Seeds/08_InitTipoImpuestoComprobanteSerieDocumento.sql`

---

## 🔧 Decisiones de Implementación

### 1. GetNextNumero Pattern (No en CRUD)
SerieDocumento tendrá endpoint especial:
```
POST /api/v1/series-documento/{id}/next-numero
Response: { numero: 101 }
```

En lugar de incrementar en PUT estándar.

### 2. Validaciones Especiales
- **Límite NumeroMaximo**: No exceder en GetNextNumero
- **Combinación única**: (TipoComprobante, Sucursal, Serie)
- **Una serie principal por tipo/sucursal**: Application rule

### 3. Transaction Isolation
- `SERIALIZABLE` para GetNextNumero
- `READ_COMMITTED` para CRUD estándar

---

## ✅ Checklist Pre-Implementación

- [ ] Revisar riesgo crítico RG-02 (race condition)
- [ ] Validar transacción SERIALIZABLE en SQL Server
- [ ] Diseñar endpoint especial GET /next-numero
- [ ] Validar integridad con múltiples usuarios (Postman concurrent)
- [ ] Seed data: F001, B001, T001 con TipoImpuesto correcto

---

## 📊 Métricas Esperadas

| Item | Planeado |
|------|----------|
| Entidades | 3 |
| Commands | 12 |
| Handlers | 12 (+1 especial) |
| Validators | 6 |
| DTOs | 9 |
| Endpoints | 21 |
| SQL Scripts | 4 |
| Compilación esperada | 0 errores |
| Tiempo estimado | 8-10 horas |

---

## 🔗 Referencias

- **Dependencias:** Sprint 1, Sprint 2
- **Bloquea:** Módulo Ventas v3.1
- **Riesgo crítico:** RG-02 (race condition SerieDocumento)
- **Pattern especial:** GetNextNumero con SERIALIZABLE
- **Documentación referencia:** IA_Docs/COMMON_ISSUES_AND_FIXES.md

---

**Siguiente paso:** Iniciar implementación cuando Sprint 2 sea movido a `completed/`

*Documento creado:** 2026-05-16  
*Estado:* ⏳ Pendiente aprobación para inicio
