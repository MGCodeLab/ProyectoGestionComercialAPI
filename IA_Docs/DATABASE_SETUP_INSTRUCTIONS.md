# Database Setup for v3.0.0

## Issue
Your database schema is missing audit columns that the v3.0.0 code expects:
- `PublicId` (UNIQUEIDENTIFIER) - External identifier for API exposure
- `FechaRegistro` (DATETIME2) - Record creation timestamp
- `FechaActualizacion` (DATETIME2) - Last modification timestamp

## Solution

### Step 1: Run the Migration Script
Execute this SQL script in your SQL Server Management Studio (SSMS):

**File:** `Database/v3.0.0_COMPLETE_SETUP.sql`

**What it does:**
1. Adds missing audit columns to `Productos` table
2. Adds missing audit columns to `TipoDocumentos` table
3. Populates `TipoDocumentos` with test data (DNI, RUC, PASSPORT)
4. Populates `Productos` with test data (4 products, 1 inactive)
5. Populates `Clientes` with test data (3 clients, 1 inactive)

### Step 2: Verify Database Setup
After running the script, verify the data was inserted:

```sql
SELECT COUNT(*) FROM catalogo.Productos;       -- Should show 4
SELECT COUNT(*) FROM catalogo.TipoDocumentos;  -- Should show 3
SELECT COUNT(*) FROM comercial.Clientes;       -- Should show 3
```

### Step 3: Restart the Application
1. Stop the current application (Ctrl+C)
2. Run it again: `dotnet run`
3. Application will start on `http://localhost:5198`

## Testing the Soft Delete Behavior

### Test Data Overview
- **3 Clientes:** 2 active (Activo=1), 1 inactive (Activo=0)
- **4 Productos:** 3 active (Activo=1), 1 inactive (Activo=0)

### Expected Behavior (Per v3.0.0 Specification)

**GET /api/v1/clientes** - Returns ALL records (active + inactive)
```
✓ Juan García (Activo: true)
✓ María Rodríguez (Activo: true)
✓ Carlos Fernández (Activo: false)  <- Still visible!
```

**GET /api/v1/productos** - Returns ALL records
```
✓ Laptop (Activo: true)
✓ Mouse (Activo: true)
✓ Teclado (Activo: true)
✓ Monitor (Activo: false)  <- Still visible!
```

### Test Endpoints

#### 1. List all Clientes (shows all records including inactive)
```bash
curl -X GET http://localhost:5198/api/v1/clientes
```

#### 2. Get specific Cliente
```bash
curl -X GET http://localhost:5198/api/v1/clientes/1
```

#### 3. Create new Cliente
```bash
curl -X POST http://localhost:5198/api/v1/clientes \
  -H "Content-Type: application/json" \
  -d '{
    "tipoDocumentoId": 1,
    "numeroDocumento": "99999999",
    "nombres": "Pedro",
    "apellidoPaterno": "López",
    "apellidoMaterno": "Gómez",
    "correo": "pedro.lopez@email.com",
    "telefono": "+51987654324",
    "direccion": "Calle Nueva 999"
  }'
```

#### 4. Update Cliente
```bash
curl -X PUT http://localhost:5198/api/v1/clientes/1 \
  -H "Content-Type: application/json" \
  -d '{
    "tipoDocumentoId": 1,
    "numeroDocumento": "12345678",
    "nombres": "Juan Updated",
    "apellidoPaterno": "García",
    "apellidoMaterno": "López",
    "correo": "juan.new@email.com",
    "telefono": "+51987654321",
    "direccion": "Calle Principal 123"
  }'
```

#### 5. Soft Delete (Inactivate) Cliente
```bash
curl -X PATCH http://localhost:5198/api/v1/clientes/2/inactivar
```

**Important:** After this, GET /api/v1/clientes should STILL show María with `Activo: false`

#### 6. Reactivate Cliente
```bash
curl -X PATCH http://localhost:5198/api/v1/clientes/2/activar
```

#### 7. Hard Delete (Permanent removal)
```bash
curl -X DELETE http://localhost:5198/api/v1/clientes/3
```

#### 8. List Productos (shows all including inactive)
```bash
curl -X GET http://localhost:5198/api/v1/productos
```

## Soft Delete Philosophy (v3.0.0)

❌ **NOT Data Hiding** - Soft delete does NOT hide records from queries
✓ **Audit Trail** - Soft delete (Activo=false) maintains a record for traceability
✓ **Frontend Control** - The Angular frontend controls visual presentation
   - Can display inactive records with different styling (strikethrough, gray, etc.)
   - Can filter/sort by Activo field
   - Can show/hide inactive records based on user preference

## Troubleshooting

### Error: "Invalid column name 'PublicId'"
- The script didn't execute properly
- Solution: Run `v3.0.0_COMPLETE_SETUP.sql` again from the Database folder

### Error: "Foreign key constraint fail"
- TipoDocumentos not seeded yet
- Solution: Ensure the script runs in order (Step 3 before Step 5)

### Port already in use (5198)
- A previous instance is still running
- Solution: Kill the process or change the port in `launchSettings.json`

---

**Status:** Application ready once database is set up!
