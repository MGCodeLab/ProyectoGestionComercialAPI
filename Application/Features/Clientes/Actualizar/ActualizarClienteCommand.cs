using MediatR;

namespace Application.Features.Clientes.Actualizar
{
    public record ActualizarClienteCommand(
        int Id,
        int TipoDocumentoId,
        string NumeroDocumento,
        string Nombres,
        string ApellidoPaterno,
        string? ApellidoMaterno,
        string? Correo,
        string? Telefono,
        string? Direccion
    ) : IRequest<Unit>;
}
