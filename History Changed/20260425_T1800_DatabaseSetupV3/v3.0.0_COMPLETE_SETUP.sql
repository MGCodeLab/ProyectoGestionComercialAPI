-- ============================================================================
-- v3.0.0 Database Complete Setup
-- Execute this script in SQL Server to prepare your database for testing
-- ============================================================================

-- Step 1: Add audit columns to Productos (if missing)
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = 'catalogo' AND TABLE_NAME = 'Productos' AND COLUMN_NAME = 'PublicId'
)
BEGIN
    ALTER TABLE catalogo.Productos
    ADD PublicId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID();

    ALTER TABLE catalogo.Productos
    ADD FechaRegistro DATETIME2 NOT NULL DEFAULT GETUTCDATE();

    ALTER TABLE catalogo.Productos
    ADD FechaActualizacion DATETIME2 NULL;

    ALTER TABLE catalogo.Productos
    ADD CONSTRAINT UQ_Productos_PublicId UNIQUE (PublicId);

    PRINT 'Added audit columns to Productos table';
END
ELSE
BEGIN
    PRINT 'Audit columns already exist in Productos table';
END

-- Step 2: Add audit columns to TipoDocumentos (if missing)
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = 'catalogo' AND TABLE_NAME = 'TipoDocumentos' AND COLUMN_NAME = 'PublicId'
)
BEGIN
    ALTER TABLE catalogo.TipoDocumentos
    ADD PublicId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID();

    ALTER TABLE catalogo.TipoDocumentos
    ADD FechaRegistro DATETIME2 NOT NULL DEFAULT GETUTCDATE();

    ALTER TABLE catalogo.TipoDocumentos
    ADD FechaActualizacion DATETIME2 NULL;

    ALTER TABLE catalogo.TipoDocumentos
    ADD CONSTRAINT UQ_TipoDocumentos_PublicId UNIQUE (PublicId);

    PRINT 'Added audit columns to TipoDocumentos table';
END
ELSE
BEGIN
    PRINT 'Audit columns already exist in TipoDocumentos table';
END

-- Step 3: Seed TipoDocumentos with test data
DELETE FROM catalogo.TipoDocumentos WHERE Codigo IN ('DNI', 'RUC', 'PASSPORT');

INSERT INTO catalogo.TipoDocumentos (Codigo, Descripcion, Activo, FechaActualizacion)
VALUES
    ('DNI', 'Documento Nacional de Identidad', 1, NULL),
    ('RUC', 'Registro Único del Contribuyente', 1, NULL),
    ('PASSPORT', 'Pasaporte', 1, NULL);

PRINT 'TipoDocumentos seeded';

-- Step 4: Seed Productos with test data
DELETE FROM catalogo.Productos WHERE Nombre IN ('Laptop', 'Mouse', 'Teclado', 'Monitor');

INSERT INTO catalogo.Productos (Nombre, Descripcion, Precio, Activo, FechaActualizacion)
VALUES
    ('Laptop', 'Laptop Dell XPS 13', 1200.00, 1, NULL),
    ('Mouse', 'Logitech MX Master 3', 99.99, 1, NULL),
    ('Teclado', 'Razer Mechanical Keyboard', 149.99, 1, NULL),
    ('Monitor', 'LG UltraWide 34"', 599.99, 0, NULL);

PRINT 'Productos seeded';

-- Step 5: Seed Clientes with test data
DECLARE @DNI_ID INT;
DECLARE @RUC_ID INT;

SELECT @DNI_ID = Id FROM catalogo.TipoDocumentos WHERE Codigo = 'DNI';
SELECT @RUC_ID = Id FROM catalogo.TipoDocumentos WHERE Codigo = 'RUC';

DELETE FROM comercial.Clientes WHERE NumeroDocumento IN ('12345678', '87654321', '11111111');

INSERT INTO comercial.Clientes
    (TipoDocumentoId, NumeroDocumento, Nombres, ApellidoPaterno, ApellidoMaterno, Correo, Telefono, Direccion, Activo, FechaActualizacion)
VALUES
    (@DNI_ID, '12345678', 'Juan', 'García', 'López', 'juan.garcia@email.com', '+51987654321', 'Calle Principal 123', 1, NULL),
    (@DNI_ID, '87654321', 'María', 'Rodríguez', 'Martínez', 'maria.rodriguez@email.com', '+51987654322', 'Avenida Central 456', 1, NULL),
    (@RUC_ID, '11111111', 'Carlos', 'Fernández', 'Sánchez', 'carlos.fernandez@email.com', '+51987654323', 'Plaza Mayor 789', 0, NULL);

PRINT 'Clientes seeded';

-- Step 6: Verify data
PRINT '';
PRINT '====== VERIFICATION ======';
PRINT 'Productos:';
SELECT COUNT(*) as 'Total Productos' FROM catalogo.Productos;

PRINT 'TipoDocumentos:';
SELECT COUNT(*) as 'Total TipoDocumentos' FROM catalogo.TipoDocumentos;

PRINT 'Clientes:';
SELECT COUNT(*) as 'Total Clientes' FROM comercial.Clientes;

PRINT '';
PRINT '✓ Database v3.0.0 setup completed successfully!';
