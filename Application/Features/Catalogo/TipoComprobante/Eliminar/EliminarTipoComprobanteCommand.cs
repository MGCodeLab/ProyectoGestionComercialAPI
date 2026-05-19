using MediatR;

namespace Application.Features.Catalogo.TipoComprobante.Eliminar
{
    public record EliminarTipoComprobanteCommand(int Id) : IRequest<int>;
}
