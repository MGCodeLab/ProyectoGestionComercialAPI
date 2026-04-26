CREATE TABLE catalogo.TipoDocumentos
(
    Id                  INT IDENTITY PRIMARY KEY,
    PublicId            UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Codigo              NVARCHAR(20)     NOT NULL,
    Descripcion         NVARCHAR(250)    NULL,
    Activo              BIT              NOT NULL DEFAULT 1,
    FechaRegistro       DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    FechaActualizacion  DATETIME2        NULL,

    CONSTRAINT UQ_TipoDocumentos_PublicId UNIQUE (PublicId)
);
