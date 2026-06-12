using Application.Exceptions;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Catalogo.SerieDocumento.ActualizarEstado
{
    public class ActualizarEstadoSerieDocumentoHandler : IRequestHandler<ActualizarEstadoSerieDocumentoCommand, Unit>
    {
        private readonly ISerieDocumentoService _service;
        private readonly ILogger<ActualizarEstadoSerieDocumentoHandler> _logger;

        public ActualizarEstadoSerieDocumentoHandler(ISerieDocumentoService service, ILogger<ActualizarEstadoSerieDocumentoHandler> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<Unit> Handle(ActualizarEstadoSerieDocumentoCommand request, CancellationToken cancellationToken)
        {
            var entity = await _service.ObtenerPorId(request.Id, isAsTracking: true, cancellationToken);
            if (entity == null)
                throw new NotFoundException($"SerieDocumento con ID {request.Id} no encontrado");

            entity.Activo = request.Activo;
            entity.FechaActualizacion = DateTime.UtcNow;

            await _service.Actualizar(cancellationToken);

            _logger.LogInformation("SerieDocumento {Id} estado actualizado a Activo={Activo}", request.Id, request.Activo);

            return Unit.Value;
        }
    }
}
