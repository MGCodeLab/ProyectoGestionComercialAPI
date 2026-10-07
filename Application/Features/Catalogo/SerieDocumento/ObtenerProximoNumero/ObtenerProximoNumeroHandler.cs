using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Catalogo.SerieDocumento.ObtenerProximoNumero
{
    public class ObtenerProximoNumeroHandler : IRequestHandler<ObtenerProximoNumeroQuery, int>
    {
        private readonly ISerieDocumentoService _service;
        private readonly ILogger<ObtenerProximoNumeroHandler> _logger;

        public ObtenerProximoNumeroHandler(ISerieDocumentoService service, ILogger<ObtenerProximoNumeroHandler> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<int> Handle(ObtenerProximoNumeroQuery request, CancellationToken ct)
        {
            _logger.LogInformation("ObtenerProximoNumero: {@request}", request);

            var numero = await _service.ObtenerProximoNumeroAsync(request.SerieDocumentoId, ct);
            _logger.LogInformation("ObtenerProximoNumero: numero {numero}", numero);
            return numero;
        }
    }
}
