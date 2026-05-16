CREATE TABLE organizacion.Sucursales
(
    Id INT IDENTITY PRIMARY KEY,
    PublicId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),

    Nombre VARCHAR(150) NOT NULL,
    Codigo VARCHAR(10) NOT NULL,

    EmpresaId INT NOT NULL,
    PaisId INT NOT NULL,

    Direccion VARCHAR(300) NULL,
    Telefono VARCHAR(20) NULL,

    EsPrincipal BIT NOT NULL DEFAULT 0,

    Activo BIT NOT NULL DEFAULT 1,
    FechaRegistro DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FechaActualizacion DATETIME2 NULL,

    CONSTRAINT FK_Sucursales_Empresa
        FOREIGN KEY (EmpresaId)
        REFERENCES organizacion.Empresas(Id),

    CONSTRAINT FK_Sucursales_Pais
        FOREIGN KEY (PaisId)
        REFERENCES catalogo.Paises(Id),

    CONSTRAINT UQ_Sucursales_Codigo
        UNIQUE (Codigo)
);

CREATE INDEX IX_Sucursales_EmpresaId ON organizacion.Sucursales(EmpresaId);
CREATE INDEX IX_Sucursales_PaisId ON organizacion.Sucursales(PaisId);
