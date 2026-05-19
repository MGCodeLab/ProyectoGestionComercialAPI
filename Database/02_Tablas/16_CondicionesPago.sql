IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CondicionesPago' AND schema_id = SCHEMA_ID('catalogo'))
BEGIN
    CREATE TABLE catalogo.CondicionesPago
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        PublicId UNIQUEIDENTIFIER NOT NULL DEFAULT (NEWSEQUENTIALID()),
        Nombre NVARCHAR(100) NOT NULL,
        DiasCredito INT NOT NULL DEFAULT 0,
        Descripcion NVARCHAR(500) NULL,
        Activo BIT NOT NULL DEFAULT 1,
        FechaRegistro DATETIME2 NOT NULL DEFAULT (GETUTCDATE()),
        FechaActualizacion DATETIME2 NULL,
        UNIQUE (PublicId),
        UNIQUE (Nombre)
    );

    CREATE INDEX IX_CondicionesPago_Activo ON catalogo.CondicionesPago(Activo);
    CREATE INDEX IX_CondicionesPago_DiasCredito ON catalogo.CondicionesPago(DiasCredito);
END
