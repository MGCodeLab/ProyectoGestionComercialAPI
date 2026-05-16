using MediatR;

namespace Application.Features.Organizacion.Almacen.Eliminar
{
    public record EliminarAlmacenCommand(int Id) : IRequest<int>;
}
