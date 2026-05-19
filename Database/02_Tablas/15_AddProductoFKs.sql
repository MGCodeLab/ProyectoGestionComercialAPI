-- Safe idempotent migration: Add FKs to Productos
-- Adds 3 nullable foreign keys to enrich Productos table
-- Must execute AFTER CategoriasProducto and MarcasProducto tables are created

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'Productos'
    AND TABLE_SCHEMA = 'catalogo'
    AND COLUMN_NAME = 'UnidadMedidaId'
)
BEGIN
    -- Add columns (nullable for existing data)
    ALTER TABLE catalogo.Productos ADD
        UnidadMedidaId      INT NULL,
        CategoriaProductoId INT NULL,
        MarcaProductoId     INT NULL;

    -- Add foreign key constraints with NO ACTION (no cascades - SQL Server compatible)
    ALTER TABLE catalogo.Productos ADD
        CONSTRAINT FK_Productos_UnidadMedida
        FOREIGN KEY (UnidadMedidaId)
        REFERENCES catalogo.UnidadesMedida(Id)
        ON DELETE NO ACTION;

    ALTER TABLE catalogo.Productos ADD
        CONSTRAINT FK_Productos_CategoriaProducto
        FOREIGN KEY (CategoriaProductoId)
        REFERENCES catalogo.CategoriasProducto(Id)
        ON DELETE NO ACTION;

    ALTER TABLE catalogo.Productos ADD
        CONSTRAINT FK_Productos_MarcaProducto
        FOREIGN KEY (MarcaProductoId)
        REFERENCES catalogo.MarcasProducto(Id)
        ON DELETE NO ACTION;

    -- Create indices for lookup performance
    CREATE INDEX IX_Productos_UnidadMedidaId ON catalogo.Productos(UnidadMedidaId);
    CREATE INDEX IX_Productos_CategoriaProductoId ON catalogo.Productos(CategoriaProductoId);
    CREATE INDEX IX_Productos_MarcaProductoId ON catalogo.Productos(MarcaProductoId);

    PRINT 'Successfully added UnidadMedidaId, CategoriaProductoId, MarcaProductoId to Productos';
END
ELSE
BEGIN
    PRINT 'Columns already exist in Productos - skipping migration';
END
GO
