-- Tabla: configuracion.ParametrosSistema
-- Descripción: Parámetros de configuración del sistema

CREATE TABLE configuracion.ParametrosSistema (
    Id                  INT IDENTITY(1,1) PRIMARY KEY,
    PublicId            UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Clave               NVARCHAR(100) NOT NULL UNIQUE,
    Valor               NVARCHAR(500) NOT NULL,
    TipoDato            NVARCHAR(20) NOT NULL DEFAULT 'STRING',
    Descripcion         NVARCHAR(500) NULL,
    Activo              BIT NOT NULL DEFAULT 1,
    FechaRegistro       DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FechaActualizacion  DATETIME2 NULL,
    CONSTRAINT UQ_ParametrosSistema_PublicId UNIQUE (PublicId),
    CONSTRAINT UQ_ParametrosSistema_Clave UNIQUE (Clave)
);

CREATE INDEX IX_ParametrosSistema_Clave ON configuracion.ParametrosSistema(Clave);
