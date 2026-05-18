-- Create table CategoriasProducto with self-reference
-- Schema: catalogo
-- Features: Hierarchical tree structure (max 3 levels), self-reference FK

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_NAME = 'CategoriasProducto'
    AND TABLE_SCHEMA = 'catalogo'
)
BEGIN
    CREATE TABLE catalogo.CategoriasProducto (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        PublicId UNIQUEIDENTIFIER NOT NULL,
        Nombre NVARCHAR(150) NOT NULL,
        Descripcion NVARCHAR(500) NULL,
        CategoriaPadreId INT NULL,
        Activo BIT NOT NULL DEFAULT 1,
        FechaRegistro DATETIME NOT NULL DEFAULT GETUTCDATE(),
        FechaActualizacion DATETIME NULL
    );

    -- Add self-referencing foreign key
    ALTER TABLE catalogo.CategoriasProducto ADD
        CONSTRAINT FK_CategoriasProducto_CategoriaPadre
        FOREIGN KEY (CategoriaPadreId)
        REFERENCES catalogo.CategoriasProducto(Id)
        ON DELETE RESTRICT;

    -- Create indices
    CREATE INDEX IX_CategoriasProducto_CategoriaPadreId ON catalogo.CategoriasProducto(CategoriaPadreId);
    CREATE INDEX IX_CategoriasProducto_Activo ON catalogo.CategoriasProducto(Activo);
    CREATE INDEX IX_CategoriasProducto_Nombre ON catalogo.CategoriasProducto(Nombre);

    PRINT 'Successfully created CategoriasProducto table with self-reference';
END
ELSE
BEGIN
    PRINT 'CategoriasProducto table already exists - skipping creation';
END
GO
