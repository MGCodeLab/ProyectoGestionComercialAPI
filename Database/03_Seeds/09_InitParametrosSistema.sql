-- Seed: Parámetros del Sistema

DELETE FROM configuracion.ParametrosSistema WHERE 1=1;
SET IDENTITY_INSERT configuracion.ParametrosSistema ON;

INSERT INTO configuracion.ParametrosSistema (Id, Clave, Valor, TipoDato, Descripcion, Activo)
VALUES
    (1, 'MONEDA_BASE', 'PEN', 'STRING', 'Moneda funcional del sistema', 1),
    (2, 'IGV_PORCENTAJE', '18', 'DECIMAL', 'Porcentaje de Impuesto General a las Ventas (Perú)', 1),
    (3, 'EMPRESA_RUC', '20000000001', 'STRING', 'RUC de la empresa principal', 1),
    (4, 'MAX_CATEGORIA_PROFUNDIDAD', '3', 'INT', 'Profundidad máxima de categorías de productos', 1),
    (5, 'VENTAS_REQUIERE_SERIE_DOCUMENTO', '1', 'BOOL', 'Activar validación de serie de documento en ventas', 1);

SET IDENTITY_INSERT configuracion.ParametrosSistema OFF;
