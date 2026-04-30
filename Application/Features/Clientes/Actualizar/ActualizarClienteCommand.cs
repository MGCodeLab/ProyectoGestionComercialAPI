using MediatR;

namespace Application.Features.Clientes.Actualizar
{
    public record ActualizarClienteCommand(
        int TipoDocumentoId,
        string NumeroDocumento,
        string Nombres,
        string ApellidoPaterno,
        string? ApellidoMaterno,
        string? Correo,
        string? Telefono,
        string? Direccion,
        int Id = 0
    ) : IRequest<Unit>;
}
