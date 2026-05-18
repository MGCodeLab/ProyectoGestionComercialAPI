-- Seed data for TiposImpuesto
IF NOT EXISTS (SELECT 1 FROM [catalogo].[TiposImpuesto] WHERE [Codigo] = 'IGV')
BEGIN
    INSERT INTO [catalogo].[TiposImpuesto] ([Nombre], [Codigo], [Porcentaje], [EsIncluido], [Activo])
    VALUES
    (N'Impuesto General a las Ventas', 'IGV', 18.00, 1, 1),
    (N'Impuesto Selectivo al Consumo', 'ISC', 0.00, 1, 1),
    (N'Exonerado', 'EXONERADO', 0.00, 1, 1),
    (N'Inafecto', 'INAFECTO', 0.00, 1, 1);
END

-- Seed data for TiposComprobante
IF NOT EXISTS (SELECT 1 FROM [catalogo].[TiposComprobante] WHERE [Codigo] = '01')
BEGIN
    INSERT INTO [catalogo].[TiposComprobante] ([Nombre], [Codigo], [AfectaInventario], [AfectaContable], [Activo])
    VALUES
    (N'Factura', '01', 1, 1, 1),
    (N'Boleta', '03', 1, 1, 1),
    (N'Nota de Venta', 'NV', 0, 0, 1);
END

-- Seed data for SeriesDocumento (assuming Sucursal with ID=1 and TipoComprobante IDs: 1=Factura, 2=Boleta, 3=NotaVenta)
IF NOT EXISTS (SELECT 1 FROM [catalogo].[SeriesDocumento] WHERE [Serie] = 'F001')
BEGIN
    INSERT INTO [catalogo].[SeriesDocumento] ([TipoComprobanteId], [SucursalId], [Serie], [NumeroActual], [NumeroMaximo], [Activo])
    VALUES
    (1, 1, 'F001', 0, NULL, 1),
    (2, 1, 'B001', 0, NULL, 1),
    (3, 1, 'NV', 0, NULL, 1);
END
