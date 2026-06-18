using MediatR;

namespace Application.Features.Catalogo.TipoComprobante.Actualizar
{
    public record ActualizarTipoComprobanteCommand(
        string Nombre,
        string Codigo,
        bool AfectaInventario,
        bool AfectaContable,
        int Id = 0) : IRequest<Unit>;
}
