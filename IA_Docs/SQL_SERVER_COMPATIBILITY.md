# SQL Server Compatibility Guide

**Propósito:** Documentar diferencias entre SQL estándar (ANSI SQL) y SQL Server específicamente, para evitar errores de sintaxis.

**Última actualización:** 2026-05-17 (Sprint 3)

---

## 🚨 Errores Comunes — RESTRICT vs NO ACTION

### Problema
```sql
-- ❌ INCORRECTO en SQL Server
ON DELETE RESTRICT

-- ✅ CORRECTO en SQL Server
ON DELETE NO ACTION
```

### Contexto
- **RESTRICT** → Estándar SQL (ANSI), soportado por PostgreSQL, MySQL, MariaDB
- **NO ACTION** → Específico SQL Server, comportamiento idéntico a RESTRICT
- **Ambos** → Previenen DELETE si existen registros referenciados (integridad referencial)

### Síntoma
```
Incorrect syntax near the keyword 'RESTRICT'.
```

### Solución
Reemplazar `RESTRICT` por `NO ACTION` en:
- `CREATE TABLE` (cuando defines FKs en tabla nueva)
- `ALTER TABLE` (cuando agregas FKs a tabla existente)

---

## 📋 Palabras Clave Específicas SQL Server

### DELETE Actions (Foreign Key behavior)

| Acción | SQL Server | PostgreSQL | MySQL | Descripción |
|--------|-----------|-----------|-------|------------|
| Sin acción | `NO ACTION` ❌ (default) | `RESTRICT` ✅ (default) | `RESTRICT` ✅ (default) | Rechaza DELETE si hay refs |
| Cascada | `CASCADE` ✅ | `CASCADE` ✅ | `CASCADE` ✅ | Elimina registros dependientes |
| Set NULL | `SET NULL` ✅ | `SET NULL` ✅ | `SET NULL` ✅ | Asigna NULL a FK |
| Set Default | `SET DEFAULT` ✅ | `SET DEFAULT` ✅ | `SET DEFAULT` ✅ | Asigna default a FK |

**Recomendación:** Usar `NO ACTION` o `CASCADE` (ambas soportadas en todas las BDs).

---

## 🔍 Otros Errores Potenciales SQL Server

### 1. Sintaxis de Índices

```sql
-- ❌ INCORRECTO (PostgreSQL style)
CREATE UNIQUE INDEX idx_unique ON table(column);
-- Puede fallar en algunas versiones

-- ✅ CORRECTO (SQL Server style)
CREATE UNIQUE INDEX [idx_unique] ON [dbo].[table]([column]);
-- o simplemente:
CREATE UNIQUE INDEX idx_unique ON table(column);
```

### 2. Computed Columns

```sql
-- ✅ CORRECTO en SQL Server
ALTER TABLE [dbo].[Tabla]
ADD NombreCompleto AS [Nombres] + ' ' + [ApellidoPaterno] PERSISTED;

-- ❌ INCORRECTO (falta PERSISTED)
ALTER TABLE [dbo].[Tabla]
ADD NombreCompleto AS [Nombres] + ' ' + [ApellidoPaterno];
```

### 3. Boolean Type

```sql
-- ❌ INCORRECTO (PostgreSQL)
column BOOLEAN NOT NULL DEFAULT FALSE;

-- ✅ CORRECTO (SQL Server)
column BIT NOT NULL DEFAULT 0;
-- 0 = false, 1 = true
```

### 4. Identity Columns

```sql
-- ✅ CORRECTO en SQL Server
[Id] INT PRIMARY KEY IDENTITY(1,1)

-- ⚠️ CUIDADO al resetear
DBCC CHECKIDENT ('tabla_nombre', RESEED, 0);  -- Siguiente ID será 1
```

### 5. String Length

```sql
-- ❌ INCORRECTO (falta longitud)
column VARCHAR NOT NULL;

-- ✅ CORRECTO
column VARCHAR(100) NOT NULL;
column NVARCHAR(100) NOT NULL;  -- Para Unicode
```

### 6. NULL Handling in UNIQUE Constraints

```sql
-- ❌ PROBLEMA: Múltiples NULLs en unique constraint
CREATE TABLE tabla (
    email NVARCHAR(100) UNIQUE,  -- NULL se repite, causa error
    -- ...
);

-- ✅ SOLUCIÓN: Filtered index
CREATE UNIQUE INDEX [UQ_email] 
    ON [tabla]([email]) 
    WHERE [email] IS NOT NULL;
```

---

## ✅ Checklist SQL Server Scripts

Antes de ejecutar scripts DDL:

- [ ] ¿Usas `ON DELETE RESTRICT`? → Cambiar a `ON DELETE NO ACTION`
- [ ] ¿Usas `BOOLEAN`? → Cambiar a `BIT`
- [ ] ¿Computed columns tienen `PERSISTED`?
- [ ] ¿VARCHAR tiene longitud especificada?
- [ ] ¿UNIQUE constraints en NULL fields? → Usar filtered index
- [ ] ¿Identities correctamente configuradas?
- [ ] ¿Schemas existen? (dbo, catalogo, organizacion, etc.)

---

## 🔗 Referencias

- [MS Docs: SQL Server T-SQL](https://learn.microsoft.com/en-us/sql/t-sql/language-reference)
- [MS Docs: FOREIGN KEY Constraints](https://learn.microsoft.com/en-us/sql/t-sql/statements/alter-table-table-constraint-transact-sql)
- [MS Docs: CREATE TABLE](https://learn.microsoft.com/en-us/sql/t-sql/statements/create-table-transact-sql)

---

## 📝 Problemas Encontrados en Nexus ERP

| Fecha | Problema | Script | Solución | Status |
|-------|----------|--------|----------|--------|
| 2026-05-17 | ON DELETE RESTRICT | 12_SeriesDocumento.sql | Cambiar a NO ACTION | ✅ Resuelto |
| - | - | - | - | - |

---

**Última revisión:** 2026-05-17  
**Próxima revisión:** Cuando se agreguen nuevos scripts SQL a Sprint 4-5
