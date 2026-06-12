using MediatR;

namespace Application.Features.Catalogo.SerieDocumento.Eliminar
{
    public record EliminarSerieDocumentoCommand(int Id) : IRequest<Unit>;
}
