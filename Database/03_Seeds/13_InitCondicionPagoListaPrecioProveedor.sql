-- ============================================================
-- SPRINT 5 - SEED DATA: CondicionPago, ListaPrecio, Proveedor
-- ============================================================

-- ========== CONDICIONES DE PAGO ==========
IF NOT EXISTS (SELECT 1 FROM catalogo.CondicionesPago WHERE Nombre = 'Contado')
BEGIN
    INSERT INTO catalogo.CondicionesPago (Nombre, DiasCredito, Descripcion, Activo)
    VALUES
        ('Contado', 0, 'Pago inmediato', 1),
        ('15 Días', 15, 'Crédito a 15 días', 1),
        ('30 Días', 30, 'Crédito a 30 días', 1),
        ('60 Días', 60, 'Crédito a 60 días', 1),
        ('90 Días', 90, 'Crédito a 90 días', 1);
END

-- ========== LISTAS DE PRECIOS ==========
IF NOT EXISTS (SELECT 1 FROM catalogo.ListasPrecios WHERE Nombre = 'Lista Precios Base')
BEGIN
    -- Asume que Monedas ya existen (PEN = Id 1)
    INSERT INTO catalogo.ListasPrecios (Nombre, MonedaId, Descripcion, EsDefault, Activo)
    VALUES
        ('Lista Precios Base', 1, 'Lista de precios base en moneda funcional (PEN)', 1, 1),
        ('Lista Precios USD', 2, 'Lista de precios en dólares americanos', 0, 1);
END

-- ========== PROVEEDORES ==========
IF NOT EXISTS (SELECT 1 FROM comercial.Proveedores WHERE NumeroDocumento = '20123456789')
BEGIN
    -- Asume que TipoDocumentos (Id 5 = RUC) y Paises (Id 1 = Perú) existen
    INSERT INTO comercial.Proveedores (TipoDocumentoId, NumeroDocumento, RazonSocial, NombreComercial, PaisId, Correo, Telefono, Direccion, Activo)
    VALUES
        (5, '20123456789', 'Distribuidora de Componentes XYZ SAC', 'Dist. XYZ', 1, 'compras@distxyz.com', '+51987654321', 'Av. Principal 123, Lima', 1),
        (5, '20987654321', 'Importadora de Electrónica ACME EIRL', 'ACME', 1, 'contacto@acmeimports.com', '+51945678901', 'Jr. Comercio 456, Surco', 1);
END
