using MediatR;

namespace Application.Features.Catalogo.SerieDocumento.ActualizarEstado
{
    public record ActualizarEstadoSerieDocumentoCommand(
        bool Activo,
        int Id = 0) : IRequest<Unit>;
}
