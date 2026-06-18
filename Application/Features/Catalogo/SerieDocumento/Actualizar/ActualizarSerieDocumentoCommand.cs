using MediatR;

namespace Application.Features.Catalogo.SerieDocumento.Actualizar
{
    public record ActualizarSerieDocumentoCommand(
        int TipoComprobanteId,
        int SucursalId,
        string Serie,
        int? NumeroMaximo = null,
        int Id = 0) : IRequest<Unit>;
}
