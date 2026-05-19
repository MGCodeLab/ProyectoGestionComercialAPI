-- Tabla: configuracion.ModulosSistema
-- Descripción: Feature flags - módulos del sistema con estado de activación
-- Nota: El campo Activo heredado de AuditableEntity controla si el módulo está habilitado

CREATE TABLE configuracion.ModulosSistema (
    Id                  INT IDENTITY(1,1) PRIMARY KEY,
    PublicId            UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Nombre              NVARCHAR(100) NOT NULL,
    Codigo              NVARCHAR(50) NOT NULL UNIQUE,
    Descripcion         NVARCHAR(500) NULL,
    Activo              BIT NOT NULL DEFAULT 1,
    FechaRegistro       DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FechaActualizacion  DATETIME2 NULL,
    CONSTRAINT UQ_ModulosSistema_PublicId UNIQUE (PublicId),
    CONSTRAINT UQ_ModulosSistema_Codigo UNIQUE (Codigo)
);

CREATE INDEX IX_ModulosSistema_Codigo ON configuracion.ModulosSistema(Codigo);
