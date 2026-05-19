IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ListasPrecios' AND schema_id = SCHEMA_ID('catalogo'))
BEGIN
    CREATE TABLE catalogo.ListasPrecios
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        PublicId UNIQUEIDENTIFIER NOT NULL DEFAULT (NEWSEQUENTIALID()),
        Nombre NVARCHAR(150) NOT NULL,
        MonedaId INT NOT NULL,
        Descripcion NVARCHAR(500) NULL,
        EsDefault BIT NOT NULL DEFAULT 0,
        Activo BIT NOT NULL DEFAULT 1,
        FechaRegistro DATETIME2 NOT NULL DEFAULT (GETUTCDATE()),
        FechaActualizacion DATETIME2 NULL,
        UNIQUE (PublicId),
        CONSTRAINT FK_ListasPrecios_Monedas FOREIGN KEY (MonedaId) REFERENCES catalogo.Monedas(Id) ON DELETE NO ACTION
    );

    CREATE INDEX IX_ListasPrecios_MonedaId ON catalogo.ListasPrecios(MonedaId);
    CREATE INDEX IX_ListasPrecios_EsDefault ON catalogo.ListasPrecios(EsDefault);
    CREATE INDEX IX_ListasPrecios_Activo ON catalogo.ListasPrecios(Activo);
END
