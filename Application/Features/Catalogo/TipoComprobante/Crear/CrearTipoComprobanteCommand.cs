using MediatR;

namespace Application.Features.Catalogo.TipoComprobante.Crear
{
    public record CrearTipoComprobanteCommand(
        string Nombre,
        string Codigo,
        bool AfectaInventario,
        bool AfectaContable) : IRequest<int>;
}
