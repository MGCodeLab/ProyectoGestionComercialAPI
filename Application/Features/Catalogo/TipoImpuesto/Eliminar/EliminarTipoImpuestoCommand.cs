using MediatR;

namespace Application.Features.Catalogo.TipoImpuesto.Eliminar
{
    public record EliminarTipoImpuestoCommand(int Id) : IRequest<Unit>;
}
