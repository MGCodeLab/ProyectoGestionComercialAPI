-- Insert Empresa (SingleTenant)
INSERT INTO organizacion.Empresas
(RazonSocial, NombreComercial, NumeroDocumento, TipoDocumentoId, PaisId, MonedaBaseId, DireccionFiscal, Telefono, Correo, Activo)
VALUES
('Empresa Ejemplo S.A.C.', 'Empresa Ejemplo', '20000000001', 5, 1, 1, 'Av. Principal 123, Lima, Peru', '+51 1 2345678', 'info@empresa.ejemplo.com', 1);

-- Get Empresa Id for next inserts
DECLARE @EmpresaId INT = @@IDENTITY;

-- Insert Sucursal Principal
INSERT INTO organizacion.Sucursales
(Nombre, Codigo, EmpresaId, PaisId, Direccion, Telefono, EsPrincipal, Activo)
VALUES
('Sucursal Principal', 'SP001', @EmpresaId, 1, 'Av. Principal 123, Lima, Peru', '+51 1 2345678', 1, 1);

-- Get Sucursal Id for next inserts
DECLARE @SucursalId INT = @@IDENTITY;

-- Insert Almacen Principal
INSERT INTO organizacion.Almacenes
(Nombre, Codigo, SucursalId, Descripcion, EsPrincipal, Activo)
VALUES
('Almacén Principal', 'AM001', @SucursalId, 'Almacén principal de la sucursal', 1, 1);
