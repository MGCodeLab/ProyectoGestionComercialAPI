-- ============================================================================
-- Fix: Replace UNIQUE constraint on Correo with filtered unique index
-- ============================================================================
-- Problem: UNIQUE constraint treats multiple NULLs as duplicates
-- Solution: Use filtered unique index that only enforces uniqueness when Correo IS NOT NULL
-- This allows multiple NULL values while ensuring non-NULL emails are unique

-- Drop the old constraint
IF EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
    WHERE TABLE_SCHEMA = 'comercial'
    AND TABLE_NAME = 'Clientes'
    AND CONSTRAINT_NAME = 'UQ_Clientes_Correo'
)
BEGIN
    ALTER TABLE comercial.Clientes
    DROP CONSTRAINT UQ_Clientes_Correo;
    PRINT '✓ Old UNIQUE constraint UQ_Clientes_Correo dropped';
END

-- Create filtered unique index (if it doesn't exist)
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'UQ_Clientes_Correo'
    AND object_id = OBJECT_ID('comercial.Clientes')
)
BEGIN
    CREATE UNIQUE INDEX UQ_Clientes_Correo
        ON comercial.Clientes(Correo)
        WHERE Correo IS NOT NULL;
    PRINT '✓ Filtered unique index UQ_Clientes_Correo created';
END
ELSE
BEGIN
    PRINT '✓ Filtered unique index UQ_Clientes_Correo already exists';
END

-- Verify
SELECT 'Clientes' AS TableName, COUNT(*) AS RowCount
FROM comercial.Clientes;
GO
