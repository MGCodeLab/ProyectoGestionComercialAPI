using MediatR;

namespace Application.Features.Catalogo.SerieDocumento.ObtenerProximoNumero
{
    public class ObtenerProximoNumeroQuery : IRequest<int>
    {
        public int SerieDocumentoId { get; set; }
    }
}
