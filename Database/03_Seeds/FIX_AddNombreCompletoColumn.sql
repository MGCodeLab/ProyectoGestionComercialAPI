-- ============================================================================
-- Fix: Add NombreCompleto Computed Column to Clientes table
-- ============================================================================
-- If the column doesn't exist, add it as a computed column
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
              WHERE TABLE_NAME = 'Clientes'
              AND COLUMN_NAME = 'NombreCompleto')
BEGIN
    ALTER TABLE comercial.Clientes
    ADD NombreCompleto AS [Nombres] + ' ' + [ApellidoPaterno] + ' ' + ISNULL([ApellidoMaterno], '') PERSISTED;

    PRINT '✓ Column NombreCompleto added successfully';
END
ELSE
BEGIN
    PRINT '✓ Column NombreCompleto already exists';
END
GO

-- Verify
SELECT 'Clientes' AS TableName, COUNT(*) AS RowCount
FROM comercial.Clientes;
GO
