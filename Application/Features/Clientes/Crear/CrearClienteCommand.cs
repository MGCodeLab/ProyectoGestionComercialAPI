using MediatR;

namespace Application.Features.Clientes.Crear
{
    public record CrearClienteCommand(
        int TipoDocumentoId,
        string NumeroDocumento,
        string Nombres,
        string ApellidoPaterno,
        string? ApellidoMaterno,
        string? Correo,
        string? Telefono,
        string? Direccion
    ) : IRequest<int>;
}
