-- Seed: Módulos del Sistema

DELETE FROM configuracion.ModulosSistema WHERE 1=1;
SET IDENTITY_INSERT configuracion.ModulosSistema ON;

INSERT INTO configuracion.ModulosSistema (Id, Nombre, Codigo, Descripcion, Activo)
VALUES
    (1, 'Ventas', 'VENTAS', 'Módulo de gestión de ventas y facturación', 1),
    (2, 'Compras', 'COMPRAS', 'Módulo de gestión de compras', 0),
    (3, 'Inventario', 'INVENTARIO', 'Módulo de gestión de inventario y almacén', 0),
    (4, 'Contabilidad', 'CONTABILIDAD', 'Módulo de contabilidad y análisis financiero', 0),
    (5, 'Reportes', 'REPORTES', 'Módulo de reportes y análisis', 1);

SET IDENTITY_INSERT configuracion.ModulosSistema OFF;
