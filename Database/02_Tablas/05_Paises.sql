-- Tabla: catalogo.Paises
-- Descripción: Catálogo maestro de países disponibles en el sistema

CREATE TABLE catalogo.Paises (
    Id                  INT IDENTITY(1,1) PRIMARY KEY,
    PublicId            UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Nombre              NVARCHAR(100) NOT NULL,
    Codigo              NVARCHAR(2) NOT NULL UNIQUE,
    CodigoMoneda        NVARCHAR(3) NOT NULL,
    Activo              BIT NOT NULL DEFAULT 1,
    FechaRegistro       DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FechaActualizacion  DATETIME2 NULL,
    CONSTRAINT UQ_Paises_PublicId UNIQUE (PublicId),
    CONSTRAINT UQ_Paises_Codigo UNIQUE (Codigo)
);

-- Índices estratégicos
CREATE INDEX IX_Paises_Codigo ON catalogo.Paises(Codigo);
CREATE INDEX IX_Paises_CodigoMoneda ON catalogo.Paises(CodigoMoneda);
