CREATE TABLE organizacion.Empresas
(
    Id INT IDENTITY PRIMARY KEY,
    PublicId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),

    RazonSocial VARCHAR(200) NOT NULL,
    NombreComercial VARCHAR(200) NULL,
    NumeroDocumento VARCHAR(20) NOT NULL,

    TipoDocumentoId INT NOT NULL,
    PaisId INT NOT NULL,
    MonedaBaseId INT NOT NULL,

    DireccionFiscal VARCHAR(300) NULL,
    Telefono VARCHAR(20) NULL,
    Correo VARCHAR(150) NULL,
    LogoUrl VARCHAR(500) NULL,

    Activo BIT NOT NULL DEFAULT 1,
    FechaRegistro DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FechaActualizacion DATETIME2 NULL,

    CONSTRAINT FK_Empresas_TipoDocumento
        FOREIGN KEY (TipoDocumentoId)
        REFERENCES catalogo.TipoDocumentos(Id),

    CONSTRAINT FK_Empresas_Pais
        FOREIGN KEY (PaisId)
        REFERENCES catalogo.Paises(Id),

    CONSTRAINT FK_Empresas_Moneda
        FOREIGN KEY (MonedaBaseId)
        REFERENCES catalogo.Monedas(Id),

    CONSTRAINT UQ_Empresas_NumeroDocumento
        UNIQUE (NumeroDocumento)
);

CREATE INDEX IX_Empresas_TipoDocumentoId ON organizacion.Empresas(TipoDocumentoId);
CREATE INDEX IX_Empresas_PaisId ON organizacion.Empresas(PaisId);
CREATE INDEX IX_Empresas_MonedaBaseId ON organizacion.Empresas(MonedaBaseId);
