-- Seed data for CategoriaProducto and MarcaProducto
-- Inserts hierarchical categories and product brands

-- Seed CategoriasProducto
INSERT INTO catalogo.CategoriasProducto (PublicId, Nombre, Descripcion, CategoriaPadreId, Activo, FechaRegistro)
VALUES
(NEWID(), N'Electrónica', N'Productos electrónicos en general', NULL, 1, GETUTCDATE()),
(NEWID(), N'Computadoras', N'Equipos de cómputo', 1, 1, GETUTCDATE()),
(NEWID(), N'Laptops', N'Computadoras portátiles', 2, 1, GETUTCDATE()),
(NEWID(), N'Escritorios', N'Computadoras de escritorio', 2, 1, GETUTCDATE()),
(NEWID(), N'Accesorios', N'Accesorios tecnológicos', NULL, 1, GETUTCDATE()),
(NEWID(), N'Periféricos', N'Periféricos de computadora', 5, 1, GETUTCDATE());

-- Seed MarcasProducto
INSERT INTO catalogo.MarcasProducto (PublicId, Nombre, Descripcion, LogoUrl, Activo, FechaRegistro)
VALUES
(NEWID(), N'Dell', N'Dell Inc. - Computadoras y periféricos', NULL, 1, GETUTCDATE()),
(NEWID(), N'HP', N'Hewlett-Packard - Computadoras y accesorios', NULL, 1, GETUTCDATE()),
(NEWID(), N'Lenovo', N'Lenovo Group - Equipos de cómputo', NULL, 1, GETUTCDATE()),
(NEWID(), N'Apple', N'Apple Inc. - Computadoras premium', NULL, 1, GETUTCDATE()),
(NEWID(), N'Asus', N'Asus - Motherboards y componentes', NULL, 1, GETUTCDATE()),
(NEWID(), N'Intel', N'Intel - Procesadores', NULL, 1, GETUTCDATE());

PRINT 'Successfully seeded CategoriasProducto and MarcasProducto';
GO
