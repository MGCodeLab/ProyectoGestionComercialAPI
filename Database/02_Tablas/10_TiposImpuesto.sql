-- Create table TiposImpuesto in schema catalogo
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[catalogo].[TiposImpuesto]') AND type in (N'U'))
BEGIN
    CREATE TABLE [catalogo].[TiposImpuesto]
    (
        [Id] INT PRIMARY KEY IDENTITY(1,1),
        [PublicId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        [Nombre] NVARCHAR(100) NOT NULL,
        [Codigo] NVARCHAR(10) NOT NULL UNIQUE,
        [Porcentaje] DECIMAL(5,2) NOT NULL DEFAULT 0.00,
        [EsIncluido] BIT NOT NULL DEFAULT 1,
        [Activo] BIT NOT NULL DEFAULT 1,
        [FechaRegistro] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [FechaActualizacion] DATETIME2 NULL
    );

    CREATE INDEX [IX_TiposImpuesto_Codigo] ON [catalogo].[TiposImpuesto]([Codigo]);
    CREATE INDEX [IX_TiposImpuesto_Activo] ON [catalogo].[TiposImpuesto]([Activo]);
END
