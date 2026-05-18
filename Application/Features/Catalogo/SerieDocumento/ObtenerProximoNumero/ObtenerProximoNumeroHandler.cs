using Application.Interfaces;
using MediatR;

namespace Application.Features.Catalogo.SerieDocumento.ObtenerProximoNumero
{
    public class ObtenerProximoNumeroHandler : IRequestHandler<ObtenerProximoNumeroQuery, int>
    {
        private readonly ISerieDocumentoService _service;

        public ObtenerProximoNumeroHandler(ISerieDocumentoService service)
        {
            _service = service;
        }

        public async Task<int> Handle(ObtenerProximoNumeroQuery request, CancellationToken ct)
        {
            return await _service.ObtenerProximoNumeroAsync(request.SerieDocumentoId, ct);
        }
    }
}
