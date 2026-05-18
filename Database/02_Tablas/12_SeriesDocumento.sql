-- Create table SeriesDocumento in schema catalogo
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[catalogo].[SeriesDocumento]') AND type in (N'U'))
BEGIN
    CREATE TABLE [catalogo].[SeriesDocumento]
    (
        [Id] INT PRIMARY KEY IDENTITY(1,1),
        [PublicId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        [TipoComprobanteId] INT NOT NULL,
        [SucursalId] INT NOT NULL,
        [Serie] NVARCHAR(4) NOT NULL,
        [NumeroActual] INT NOT NULL DEFAULT 0,
        [NumeroMaximo] INT NULL,
        [Activo] BIT NOT NULL DEFAULT 1,
        [FechaRegistro] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [FechaActualizacion] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [FK_SeriesDocumento_TiposComprobante] FOREIGN KEY ([TipoComprobanteId])
            REFERENCES [catalogo].[TiposComprobante]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_SeriesDocumento_Sucursales] FOREIGN KEY ([SucursalId])
            REFERENCES [organizacion].[Sucursales]([Id]) ON DELETE NO ACTION,
        CONSTRAINT [UQ_SeriesDocumento_Composite] UNIQUE([TipoComprobanteId], [SucursalId], [Serie])
    );

    CREATE INDEX [IX_SeriesDocumento_TipoComprobanteId] ON [catalogo].[SeriesDocumento]([TipoComprobanteId]);
    CREATE INDEX [IX_SeriesDocumento_SucursalId] ON [catalogo].[SeriesDocumento]([SucursalId]);
    CREATE INDEX [IX_SeriesDocumento_Serie] ON [catalogo].[SeriesDocumento]([Serie]);
    CREATE INDEX [IX_SeriesDocumento_Activo] ON [catalogo].[SeriesDocumento]([Activo]);
END
