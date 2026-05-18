using MediatR;

namespace Application.Features.Catalogo.TipoComprobante.ActualizarEstado
{
    public record ActualizarEstadoTipoComprobanteCommand(
        bool Activo,
        int Id = 0) : IRequest<int>;
}
