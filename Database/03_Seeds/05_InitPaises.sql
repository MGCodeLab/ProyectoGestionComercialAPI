-- Seed: Países LATAM

DELETE FROM catalogo.Paises WHERE 1=1;
SET IDENTITY_INSERT catalogo.Paises ON;

INSERT INTO catalogo.Paises (Id, Nombre, Codigo, CodigoMoneda, Activo)
VALUES
    (1, 'Perú', 'PE', 'PEN', 1),
    (2, 'Chile', 'CL', 'CLP', 1),
    (3, 'Colombia', 'CO', 'COP', 1),
    (4, 'Argentina', 'AR', 'ARS', 1),
    (5, 'Bolivia', 'BO', 'BOB', 1);

SET IDENTITY_INSERT catalogo.Paises OFF;
