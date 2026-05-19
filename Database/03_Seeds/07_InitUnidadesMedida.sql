-- Seed: Unidades de Medida

DELETE FROM catalogo.UnidadesMedida WHERE 1=1;
SET IDENTITY_INSERT catalogo.UnidadesMedida ON;

INSERT INTO catalogo.UnidadesMedida (Id, Nombre, Simbolo, Codigo, Activo)
VALUES
    (1, 'Unidad', 'UND', 'NIU', 1),
    (2, 'Kilogramo', 'KG', 'KGM', 1),
    (3, 'Litro', 'LT', 'LTR', 1),
    (4, 'Metro', 'MTR', 'MTR', 1),
    (5, 'Caja', 'CAJ', 'CAJ', 1),
    (6, 'Paquete', 'PAQ', 'PAQ', 1),
    (7, 'Docena', 'DOZ', 'DOZ', 1);

SET IDENTITY_INSERT catalogo.UnidadesMedida OFF;
