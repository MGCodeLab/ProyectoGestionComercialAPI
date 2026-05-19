using MediatR;

namespace Application.Features.Comercial.Proveedor.Crear;

public record CrearProveedorCommand(
    int TipoDocumentoId,
    string NumeroDocumento,
    string RazonSocial,
    string? NombreComercial,
    int PaisId,
    string? Correo,
    string? Telefono,
    string? Direccion
) : IRequest<int>;
