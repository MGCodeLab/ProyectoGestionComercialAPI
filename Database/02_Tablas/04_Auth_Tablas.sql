-- ============================================================================
-- Auth Tables - Schema: seguridad
-- v3.0.0 - Módulo de Autenticación
-- ============================================================================

CREATE TABLE seguridad.Roles
(
    Id                  INT IDENTITY PRIMARY KEY,
    PublicId            UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Nombre              NVARCHAR(50)     NOT NULL,
    Descripcion         NVARCHAR(200)    NULL,
    Activo              BIT              NOT NULL DEFAULT 1,
    FechaRegistro       DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    FechaActualizacion  DATETIME2        NULL,

    CONSTRAINT UQ_Roles_PublicId UNIQUE (PublicId),
    CONSTRAINT UQ_Roles_Nombre   UNIQUE (Nombre)
);
GO

CREATE TABLE seguridad.Permisos
(
    Id                  INT IDENTITY PRIMARY KEY,
    PublicId            UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Recurso             NVARCHAR(50)     NOT NULL,
    Accion              NVARCHAR(50)     NOT NULL,
    Descripcion         NVARCHAR(200)    NULL,
    Activo              BIT              NOT NULL DEFAULT 1,
    FechaRegistro       DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    FechaActualizacion  DATETIME2        NULL,

    CONSTRAINT UQ_Permisos_PublicId          UNIQUE (PublicId),
    CONSTRAINT UQ_Permisos_Recurso_Accion    UNIQUE (Recurso, Accion)
);
GO

CREATE TABLE seguridad.Usuarios
(
    Id                  INT IDENTITY PRIMARY KEY,
    PublicId            UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Nombre              NVARCHAR(100)    NOT NULL,
    Email               NVARCHAR(150)    NOT NULL,
    PasswordHash        NVARCHAR(256)    NOT NULL,
    LastLogin           DATETIME2        NULL,
    Activo              BIT              NOT NULL DEFAULT 1,
    FechaRegistro       DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
    FechaActualizacion  DATETIME2        NULL,

    CONSTRAINT UQ_Usuarios_PublicId UNIQUE (PublicId),
    CONSTRAINT UQ_Usuarios_Email    UNIQUE (Email)
);
GO

CREATE TABLE seguridad.RolPermisos
(
    RolId       INT NOT NULL,
    PermisoId   INT NOT NULL,

    CONSTRAINT PK_RolPermisos      PRIMARY KEY (RolId, PermisoId),
    CONSTRAINT FK_RolPermisos_Rol  FOREIGN KEY (RolId)     REFERENCES seguridad.Roles(Id)    ON DELETE CASCADE,
    CONSTRAINT FK_RolPermisos_Perm FOREIGN KEY (PermisoId) REFERENCES seguridad.Permisos(Id) ON DELETE CASCADE
);
GO

CREATE TABLE seguridad.UsuarioRoles
(
    UsuarioId   INT NOT NULL,
    RolId       INT NOT NULL,

    CONSTRAINT PK_UsuarioRoles      PRIMARY KEY (UsuarioId, RolId),
    CONSTRAINT FK_UsuarioRoles_User FOREIGN KEY (UsuarioId) REFERENCES seguridad.Usuarios(Id) ON DELETE CASCADE,
    CONSTRAINT FK_UsuarioRoles_Rol  FOREIGN KEY (RolId)     REFERENCES seguridad.Roles(Id)    ON DELETE CASCADE
);
GO
