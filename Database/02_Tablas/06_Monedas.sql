-- Tabla: catalogo.Monedas
-- Descripción: Catálogo de monedas funcionales en el sistema

CREATE TABLE catalogo.Monedas (
    Id                  INT IDENTITY(1,1) PRIMARY KEY,
    PublicId            UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Nombre              NVARCHAR(100) NOT NULL,
    Simbolo             NVARCHAR(5) NOT NULL,
    CodigoISO           NVARCHAR(3) NOT NULL UNIQUE,
    EsMonedaBase        BIT NOT NULL DEFAULT 0,
    Activo              BIT NOT NULL DEFAULT 1,
    FechaRegistro       DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FechaActualizacion  DATETIME2 NULL,
    CONSTRAINT UQ_Monedas_PublicId UNIQUE (PublicId),
    CONSTRAINT UQ_Monedas_CodigoISO UNIQUE (CodigoISO)
);

CREATE INDEX IX_Monedas_CodigoISO ON catalogo.Monedas(CodigoISO);
CREATE INDEX IX_Monedas_EsMonedaBase ON catalogo.Monedas(EsMonedaBase);
