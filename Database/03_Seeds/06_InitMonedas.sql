-- Seed: Monedas

DELETE FROM catalogo.Monedas WHERE 1=1;
SET IDENTITY_INSERT catalogo.Monedas ON;

INSERT INTO catalogo.Monedas (Id, Nombre, Simbolo, CodigoISO, EsMonedaBase, Activo)
VALUES
    (1, 'Nuevo Sol Peruano', 'S/', 'PEN', 1, 1),
    (2, 'Dólar Estadounidense', '$', 'USD', 0, 1),
    (3, 'Peso Chileno', '$', 'CLP', 0, 1),
    (4, 'Peso Colombiano', '$', 'COP', 0, 1),
    (5, 'Peso Argentino', '$', 'ARS', 0, 1);

SET IDENTITY_INSERT catalogo.Monedas OFF;
