CREATE TABLE comercial.Clientes
(
    Id INT IDENTITY PRIMARY KEY,
    PublicId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    TipoDocumentoId INT NOT NULL,
    NumeroDocumento VARCHAR(20) NOT NULL,

    Nombres VARCHAR(100) NOT NULL,
    ApellidoPaterno VARCHAR(100) NOT NULL,
    ApellidoMaterno VARCHAR(100) NULL,

    Correo VARCHAR(150) NULL,
    Telefono VARCHAR(20) NULL,
    Direccion VARCHAR(250) NULL,

    NombreCompleto AS [Nombres] + ' ' + [ApellidoPaterno] + ' ' + ISNULL([ApellidoMaterno], '') PERSISTED,

    Activo BIT NOT NULL DEFAULT 1,
    FechaRegistro DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FechaActualizacion DATETIME2 NULL,

    CONSTRAINT FK_Clientes_TipoDocumento
        FOREIGN KEY (TipoDocumentoId)
        REFERENCES catalogo.TipoDocumento(Id),

    CONSTRAINT UQ_Clientes_NumeroDocumento
        UNIQUE (TipoDocumentoId, NumeroDocumento)
);

-- Filtered unique index: allows multiple NULLs, enforces uniqueness for non-NULL Correo values
CREATE UNIQUE INDEX UQ_Clientes_Correo
    ON comercial.Clientes(Correo)
    WHERE Correo IS NOT NULL;