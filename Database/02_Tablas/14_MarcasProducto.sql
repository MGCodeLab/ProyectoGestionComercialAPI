-- Create table MarcasProducto
-- Schema: catalogo
-- Features: Simple product brand catalog

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_NAME = 'MarcasProducto'
    AND TABLE_SCHEMA = 'catalogo'
)
BEGIN
    CREATE TABLE catalogo.MarcasProducto (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        PublicId UNIQUEIDENTIFIER NOT NULL,
        Nombre NVARCHAR(150) NOT NULL,
        Descripcion NVARCHAR(500) NULL,
        LogoUrl NVARCHAR(500) NULL,
        Activo BIT NOT NULL DEFAULT 1,
        FechaRegistro DATETIME NOT NULL DEFAULT GETUTCDATE(),
        FechaActualizacion DATETIME NULL
    );

    -- Create indices
    CREATE INDEX IX_MarcasProducto_Nombre ON catalogo.MarcasProducto(Nombre);
    CREATE INDEX IX_MarcasProducto_Activo ON catalogo.MarcasProducto(Activo);

    PRINT 'Successfully created MarcasProducto table';
END
ELSE
BEGIN
    PRINT 'MarcasProducto table already exists - skipping creation';
END
GO
