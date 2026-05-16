CREATE TABLE organizacion.Almacenes
(
    Id INT IDENTITY PRIMARY KEY,
    PublicId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),

    Nombre VARCHAR(150) NOT NULL,
    Codigo VARCHAR(10) NOT NULL,

    SucursalId INT NOT NULL,

    Descripcion VARCHAR(500) NULL,

    EsPrincipal BIT NOT NULL DEFAULT 0,

    Activo BIT NOT NULL DEFAULT 1,
    FechaRegistro DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FechaActualizacion DATETIME2 NULL,

    CONSTRAINT FK_Almacenes_Sucursal
        FOREIGN KEY (SucursalId)
        REFERENCES organizacion.Sucursales(Id),

    CONSTRAINT UQ_Almacenes_Codigo
        UNIQUE (Codigo)
);

CREATE INDEX IX_Almacenes_SucursalId ON organizacion.Almacenes(SucursalId);
