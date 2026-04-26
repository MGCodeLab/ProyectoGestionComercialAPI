-- ============================================================================
-- Auth Seed Data - v3.0.0
-- Usuarios de prueba con roles y permisos completos
-- Passwords hasheados con BCrypt (cost factor 11)
-- Plaintext: 123456
-- ============================================================================

-- ============ ROLES ============
INSERT INTO seguridad.Roles (Nombre, Descripcion)
VALUES
    ('ADMIN',     'Administrador con acceso total al sistema'),
    ('VENDOR',    'Vendedor con acceso limitado a ventas y productos'),
    ('READ_ONLY', 'Solo lectura, sin capacidad de modificar datos');
GO

-- ============ PERMISOS ============
-- Recursos: productos, clientes, ventas, compras, inventario, reportes, usuarios, configuracion
-- Acciones:  create, edit, delete, export, activate, deactivate, audit_access, view
INSERT INTO seguridad.Permisos (Recurso, Accion, Descripcion)
VALUES
    ('productos',     'create',        'Crear productos'),
    ('productos',     'edit',          'Editar productos'),
    ('productos',     'delete',        'Eliminar productos'),
    ('productos',     'export',        'Exportar productos'),
    ('productos',     'activate',      'Activar productos'),
    ('productos',     'deactivate',    'Inactivar productos'),
    ('productos',     'audit_access',  'Auditoría de productos'),
    ('productos',     'view',          'Ver productos'),

    ('clientes',      'create',        'Crear clientes'),
    ('clientes',      'edit',          'Editar clientes'),
    ('clientes',      'delete',        'Eliminar clientes'),
    ('clientes',      'export',        'Exportar clientes'),
    ('clientes',      'activate',      'Activar clientes'),
    ('clientes',      'deactivate',    'Inactivar clientes'),
    ('clientes',      'audit_access',  'Auditoría de clientes'),
    ('clientes',      'view',          'Ver clientes'),

    ('ventas',        'create',        'Crear ventas'),
    ('ventas',        'edit',          'Editar ventas'),
    ('ventas',        'delete',        'Eliminar ventas'),
    ('ventas',        'export',        'Exportar ventas'),
    ('ventas',        'activate',      'Activar ventas'),
    ('ventas',        'deactivate',    'Inactivar ventas'),
    ('ventas',        'audit_access',  'Auditoría de ventas'),
    ('ventas',        'view',          'Ver ventas'),

    ('usuarios',      'create',        'Crear usuarios'),
    ('usuarios',      'edit',          'Editar usuarios'),
    ('usuarios',      'delete',        'Eliminar usuarios'),
    ('usuarios',      'audit_access',  'Auditoría de usuarios'),
    ('usuarios',      'view',          'Ver usuarios');
GO

-- ============ USUARIOS ============
-- BCrypt hash de "123456" con cost factor 11
INSERT INTO seguridad.Usuarios (Nombre, Email, PasswordHash)
VALUES
    ('Administrador Nexus', 'admin@nexus.com',    '$2a$12$qhYkizfFDzxkEZF3MfsmzOPbQc88N/0/DF7i626AmRPCE9sGp.qQe'),
    ('Vendedor Demo',       'vendedor@nexus.com', '$2a$12$qhYkizfFDzxkEZF3MfsmzOPbQc88N/0/DF7i626AmRPCE9sGp.qQe'),
    ('Usuario Solo Lectura','readonly@nexus.com', '$2a$12$qhYkizfFDzxkEZF3MfsmzOPbQc88N/0/DF7i626AmRPCE9sGp.qQe');
GO

-- ============ ASIGNAR ROLES A USUARIOS ============
DECLARE @AdminId     INT = (SELECT Id FROM seguridad.Usuarios WHERE Email = 'admin@nexus.com');
DECLARE @VendedorId  INT = (SELECT Id FROM seguridad.Usuarios WHERE Email = 'vendedor@nexus.com');
DECLARE @ReadOnlyId  INT = (SELECT Id FROM seguridad.Usuarios WHERE Email = 'readonly@nexus.com');

DECLARE @RolAdmin     INT = (SELECT Id FROM seguridad.Roles WHERE Nombre = 'ADMIN');
DECLARE @RolVendor    INT = (SELECT Id FROM seguridad.Roles WHERE Nombre = 'VENDOR');
DECLARE @RolReadOnly  INT = (SELECT Id FROM seguridad.Roles WHERE Nombre = 'READ_ONLY');

INSERT INTO seguridad.UsuarioRoles (UsuarioId, RolId)
VALUES
    (@AdminId,    @RolAdmin),
    (@VendedorId, @RolVendor),
    (@ReadOnlyId, @RolReadOnly);
GO

-- ============ ASIGNAR PERMISOS A ROLES ============

-- ADMIN: todos los permisos
INSERT INTO seguridad.RolPermisos (RolId, PermisoId)
SELECT
    (SELECT Id FROM seguridad.Roles WHERE Nombre = 'ADMIN'),
    Id
FROM seguridad.Permisos;
GO

-- VENDOR: create/edit en productos, clientes y ventas + view en todo
INSERT INTO seguridad.RolPermisos (RolId, PermisoId)
SELECT
    (SELECT Id FROM seguridad.Roles WHERE Nombre = 'VENDOR'),
    Id
FROM seguridad.Permisos
WHERE (Recurso IN ('productos', 'clientes', 'ventas') AND Accion IN ('create', 'edit', 'view'))
   OR (Accion = 'view');
GO

-- READ_ONLY: sin permisos (puede acceder pero sin acciones)
-- (no se insertan registros en RolPermisos para READ_ONLY)

-- ============ VERIFICACIÓN ============
PRINT '====== Auth Seed Verification ======'
SELECT 'Roles'    AS Tabla, COUNT(*) AS Total FROM seguridad.Roles
UNION ALL
SELECT 'Permisos', COUNT(*) FROM seguridad.Permisos
UNION ALL
SELECT 'Usuarios', COUNT(*) FROM seguridad.Usuarios
UNION ALL
SELECT 'UsuarioRoles', COUNT(*) FROM seguridad.UsuarioRoles
UNION ALL
SELECT 'RolPermisos', COUNT(*) FROM seguridad.RolPermisos;
GO

PRINT '✓ Auth seed completado. Usuarios de prueba:'
PRINT '  admin@nexus.com     / 123456  (ADMIN - todos los permisos)'
PRINT '  vendedor@nexus.com  / 123456  (VENDOR - create/edit/view)'
PRINT '  readonly@nexus.com  / 123456  (READ_ONLY - sin permisos)'
GO
