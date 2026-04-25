CREATE TABLE catalogo.Productos
(
    Id                  INT IDENTITY PRIMARY KEY,
    PublicId            UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Nombre              NVARCHAR(150)    NOT NULL,
    Descripcion         NVARCHAR(250)    NULL,
    Precio              DECIMAL(18, 2)   NOT NULL,
    Activo              BIT              NOT NULL DEFAULT 1,
    FechaRegistro       DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    FechaActualizacion  DATETIME2        NULL,

    CONSTRAINT UQ_Productos_PublicId UNIQUE (PublicId)
);
