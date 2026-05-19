using MediatR;

namespace Application.Features.Organizacion.Almacen.Actualizar
{
    public record ActualizarAlmacenCommand(
        string Nombre,
        string Codigo,
        int SucursalId,
        string? Descripcion,
        bool EsPrincipal,
        int Id = 0
    ) : IRequest<int>;
}
