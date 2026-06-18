using Application.Exceptions;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Catalogo.SerieDocumento.Eliminar
{
    public class EliminarSerieDocumentoHandler : IRequestHandler<EliminarSerieDocumentoCommand, Unit>
    {
        private readonly ISerieDocumentoService _service;
        private readonly ILogger<EliminarSerieDocumentoHandler> _logger;

        public EliminarSerieDocumentoHandler(ISerieDocumentoService service, ILogger<EliminarSerieDocumentoHandler> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<Unit> Handle(EliminarSerieDocumentoCommand request, CancellationToken cancellationToken)
        {
            var entity = await _service.ObtenerPorId(request.Id, isAsTracking: true, cancellationToken);
            if (entity == null)
                throw new NotFoundException($"SerieDocumento con ID {request.Id} no encontrado");

            await _service.Eliminar(entity, cancellationToken);

            _logger.LogInformation("SerieDocumento eliminado con Id: {Id}", request.Id);

            return Unit.Value;
        }
    }
}
