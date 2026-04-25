# Iteración: Database Setup para v3.0.0

**Fecha:** 2026-04-25 18:00  
**Tipo:** Infrastructure / Database  
**Rama:** Modulos/Cliente_01  
**Estado:** ✅ Completada

---

## 📋 Qué se implementó

### Problema Identificado
La base de datos no tenía las columnas de auditoría que la arquitectura v3.0.0 requiere:
- `PublicId` (UNIQUEIDENTIFIER) - Identificador externo para exposición en API
- `FechaRegistro` (DATETIME2) - Timestamp de creación automático
- `FechaActualizacion` (DATETIME2) - Timestamp de última modificación

### Solución Implementada

#### Script Principal: `v3.0.0_COMPLETE_SETUP.sql`
- Agrega columnas de auditoría a `catalogo.Productos`
- Agrega columnas de auditoría a `catalogo.TipoDocumentos`
- Inserta datos de prueba en `catalogo.TipoDocumentos` (DNI, RUC, PASSPORT)
- Inserta datos de prueba en `catalogo.Productos` (4 productos, 1 inactivo)
- Inserta datos de prueba en `comercial.Clientes` (3 clientes, 1 inactivo)
- Incluye verificación de datos al final

#### Datos de Prueba Creados

**TipoDocumentos (3 registros):**
- DNI - Documento Nacional de Identidad (Activo)
- RUC - Registro Único del Contribuyente (Activo)
- PASSPORT - Pasaporte (Activo)

**Productos (4 registros):**
- Laptop - Laptop Dell XPS 13, $1200.00 (Activo)
- Mouse - Logitech MX Master 3, $99.99 (Activo)
- Teclado - Razer Mechanical Keyboard, $149.99 (Activo)
- Monitor - LG UltraWide 34", $599.99 (INACTIVO - para testing soft delete)

**Clientes (3 registros):**
- Juan García López (DNI: 12345678) - Activo
- María Rodríguez Martínez (DNI: 87654321) - Activo
- Carlos Fernández Sánchez (RUC: 11111111) - INACTIVO (para testing soft delete)

---

## 🏗️ Cambios en Estructura

### Eliminados (Redundantes)
- `Database/02_Tablas/01_Productos_Migration.sql` ❌
- `Database/02_Tablas/02_TipoDocumento_Migration.sql` ❌
- `Database/03_Seeds/01_InitProductos_v3.sql` ❌
- `Database/03_Seeds/02_InitTipoDocumento_v3.sql` ❌
- `Database/03_Seeds/03_InitClientes_v3.sql` ❌

**Razón:** El script `v3.0.0_COMPLETE_SETUP.sql` contiene toda la funcionalidad. Los individuales eran solo referencia modular.

### Documentación Reorganizada
- `DATABASE_SETUP_INSTRUCTIONS.md` → Movido a `IA_Docs/` (instrucción reutilizable)
- `v3.0.0_COMPLETE_SETUP.sql` → En esta carpeta History Changed (registro arquiterctónico)

---

## ✅ Validación

### Ejecución del Script
```sql
-- Ejecutar en SQL Server Management Studio
Database/v3.0.0_COMPLETE_SETUP.sql
```

### Verificación de Datos
```sql
SELECT COUNT(*) FROM catalogo.Productos;       -- 4
SELECT COUNT(*) FROM catalogo.TipoDocumentos;  -- 3
SELECT COUNT(*) FROM comercial.Clientes;       -- 3
```

### Verificación de Soft Delete
```sql
-- Verificar que registros inactivos aún están presentes
SELECT Id, Nombre, Activo FROM catalogo.Productos WHERE Activo = 0;   -- 1 (Monitor)
SELECT Id, Nombres, Activo FROM comercial.Clientes WHERE Activo = 0;  -- 1 (Carlos)
```

---

## 🚀 Testing Endpoints

Con los datos seeded, puedes verificar el comportamiento de soft delete:

### GET /api/v1/clientes
```bash
curl -X GET http://localhost:5198/api/v1/clientes
```
**Resultado esperado:** 3 clientes (2 activos + 1 inactivo) - TODOS visibles

### GET /api/v1/productos
```bash
curl -X GET http://localhost:5198/api/v1/productos
```
**Resultado esperado:** 4 productos (3 activos + 1 inactivo) - TODOS visibles

### PATCH /api/v1/clientes/{id}/inactivar
```bash
curl -X PATCH http://localhost:5198/api/v1/clientes/2/inactivar
```
**Resultado esperado:** Cliente inactivado pero SIGUE visible en GET

---

## 📊 Impacto

### Infrastructure
- ✅ Base de datos alineada con código v3.0.0
- ✅ Datos de prueba para all CRUD operations
- ✅ Soft delete validable (1 producto inactivo, 1 cliente inactivo)

### Documentation
- ✅ Instrucciones centralizadas en `IA_Docs/DATABASE_SETUP_INSTRUCTIONS.md`
- ✅ Script único y definitivo: `v3.0.0_COMPLETE_SETUP.sql`
- ✅ Reducción de redundancia

### Escalabilidad
- ✅ Script reutilizable para setup en nuevos ambientes
- ✅ Test data para validar soft delete en próximos módulos

---

## 🔧 Tecnología Usada

- **DBMS:** SQL Server 2019+
- **Características SQL:** IDENTITY, UNIQUEIDENTIFIER, DEFAULT, CONSTRAINT, IF NOT EXISTS
- **Patrones:** Audit columns (PublicId, FechaRegistro, FechaActualizacion), Soft Delete

---

## 📝 Referencia

Para futuras iteraciones que requieran ajustes de base de datos:
- Script de referencia: `History Changed/20260425_T1800_DatabaseSetupV3/v3.0.0_COMPLETE_SETUP.sql`
- Instrucciones completas: `IA_Docs/DATABASE_SETUP_INSTRUCTIONS.md`

---

**Estado:** ✅ **SETUP COMPLETADO Y VALIDADO**

La aplicación v3.0.0 está lista para testing con datos de prueba que validan:
- ✅ CRUD operations en Cliente
- ✅ CRUD operations en Producto  
- ✅ Soft delete behavior (Activo field)
- ✅ Auditoría completa (PublicId, FechaRegistro, FechaActualizacion)
