-- Create table TiposComprobante in schema catalogo
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[catalogo].[TiposComprobante]') AND type in (N'U'))
BEGIN
    CREATE TABLE [catalogo].[TiposComprobante]
    (
        [Id] INT PRIMARY KEY IDENTITY(1,1),
        [PublicId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        [Nombre] NVARCHAR(100) NOT NULL,
        [Codigo] NVARCHAR(5) NOT NULL UNIQUE,
        [AfectaInventario] BIT NOT NULL DEFAULT 1,
        [AfectaContable] BIT NOT NULL DEFAULT 1,
        [Activo] BIT NOT NULL DEFAULT 1,
        [FechaRegistro] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [FechaActualizacion] DATETIME2 NULL
    );

    CREATE INDEX [IX_TiposComprobante_Codigo] ON [catalogo].[TiposComprobante]([Codigo]);
    CREATE INDEX [IX_TiposComprobante_Activo] ON [catalogo].[TiposComprobante]([Activo]);
END
