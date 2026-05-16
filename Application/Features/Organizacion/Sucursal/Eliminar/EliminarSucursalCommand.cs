using MediatR;

namespace Application.Features.Organizacion.Sucursal.Eliminar
{
    public record EliminarSucursalCommand(int Id) : IRequest<int>;
}
