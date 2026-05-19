IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Proveedores' AND schema_id = SCHEMA_ID('comercial'))
BEGIN
    CREATE TABLE comercial.Proveedores
    (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        PublicId UNIQUEIDENTIFIER NOT NULL DEFAULT (NEWSEQUENTIALID()),
        TipoDocumentoId INT NOT NULL,
        NumeroDocumento NVARCHAR(20) NOT NULL,
        RazonSocial NVARCHAR(200) NOT NULL,
        NombreComercial NVARCHAR(150) NULL,
        PaisId INT NOT NULL,
        Correo NVARCHAR(150) NULL,
        Telefono NVARCHAR(20) NULL,
        Direccion NVARCHAR(300) NULL,
        Activo BIT NOT NULL DEFAULT 1,
        FechaRegistro DATETIME2 NOT NULL DEFAULT (GETUTCDATE()),
        FechaActualizacion DATETIME2 NULL,
        UNIQUE (PublicId),
        UNIQUE (TipoDocumentoId, NumeroDocumento),
        CONSTRAINT FK_Proveedores_TipoDocumentos FOREIGN KEY (TipoDocumentoId) REFERENCES catalogo.TipoDocumentos(Id) ON DELETE NO ACTION,
        CONSTRAINT FK_Proveedores_Paises FOREIGN KEY (PaisId) REFERENCES catalogo.Paises(Id) ON DELETE NO ACTION
    );

    CREATE INDEX IX_Proveedores_TipoDocumentoId ON comercial.Proveedores(TipoDocumentoId);
    CREATE INDEX IX_Proveedores_PaisId ON comercial.Proveedores(PaisId);
    CREATE INDEX IX_Proveedores_Activo ON comercial.Proveedores(Activo);
    CREATE UNIQUE INDEX IX_Proveedores_Correo ON comercial.Proveedores(Correo) WHERE Correo IS NOT NULL;
END
