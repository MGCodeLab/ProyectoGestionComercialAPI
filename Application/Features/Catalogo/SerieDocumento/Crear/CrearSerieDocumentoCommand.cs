using MediatR;

namespace Application.Features.Catalogo.SerieDocumento.Crear
{
    public record CrearSerieDocumentoCommand(
        int TipoComprobanteId,
        int SucursalId,
        string Serie,
        int? NumeroMaximo = null) : IRequest<int>;
}
