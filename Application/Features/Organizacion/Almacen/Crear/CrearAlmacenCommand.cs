using MediatR;

namespace Application.Features.Organizacion.Almacen.Crear
{
    public record CrearAlmacenCommand(
        string Nombre,
        string Codigo,
        int SucursalId,
        string? Descripcion,
        bool EsPrincipal
    ) : IRequest<int>;
}
