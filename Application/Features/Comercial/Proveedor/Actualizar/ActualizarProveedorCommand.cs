using MediatR;

namespace Application.Features.Comercial.Proveedor.Actualizar;

public record ActualizarProveedorCommand(
    int Id,
    int TipoDocumentoId,
    string NumeroDocumento,
    string RazonSocial,
    string? NombreComercial,
    int PaisId,
    string? Correo,
    string? Telefono,
    string? Direccion
) : IRequest<int>;
