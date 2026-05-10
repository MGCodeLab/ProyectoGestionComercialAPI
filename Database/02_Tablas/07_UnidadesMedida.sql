-- Tabla: catalogo.UnidadesMedida
-- Descripción: Catálogo de unidades de medida para productos

CREATE TABLE catalogo.UnidadesMedida (
    Id                  INT IDENTITY(1,1) PRIMARY KEY,
    PublicId            UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Nombre              NVARCHAR(100) NOT NULL,
    Simbolo             NVARCHAR(10) NOT NULL,
    Codigo              NVARCHAR(10) NOT NULL UNIQUE,
    Activo              BIT NOT NULL DEFAULT 1,
    FechaRegistro       DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FechaActualizacion  DATETIME2 NULL,
    CONSTRAINT UQ_UnidadesMedida_PublicId UNIQUE (PublicId),
    CONSTRAINT UQ_UnidadesMedida_Codigo UNIQUE (Codigo)
);

CREATE INDEX IX_UnidadesMedida_Codigo ON catalogo.UnidadesMedida(Codigo);
CREATE INDEX IX_UnidadesMedida_Simbolo ON catalogo.UnidadesMedida(Simbolo);
