using Application.Exceptions;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Catalogo.TipoComprobante.ActualizarEstado
{
    public class ActualizarEstadoTipoComprobanteHandler : IRequestHandler<ActualizarEstadoTipoComprobanteCommand, Unit>
    {
        private readonly ITipoComprobanteService _service;
        private readonly ILogger<ActualizarEstadoTipoComprobanteHandler> _logger;

        public ActualizarEstadoTipoComprobanteHandler(
            ITipoComprobanteService service,
            ILogger<ActualizarEstadoTipoComprobanteHandler> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<Unit> Handle(ActualizarEstadoTipoComprobanteCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ActualizarEstadoTipoComprobante: ID {Id}, Activo {Activo}", request.Id, request.Activo);

            var entity = await _service.ObtenerPorId(request.Id, isAsTracking: true, cancellationToken);
            if (entity == null)
                throw new NotFoundException($"TipoComprobante con ID {request.Id} no encontrado");

            entity.Activo = request.Activo;
            entity.FechaActualizacion = DateTime.UtcNow;

            await _service.Actualizar(cancellationToken);

            _logger.LogInformation("TipoComprobante {Id} estado actualizado a {Activo}", request.Id, request.Activo);
            return Unit.Value;
        }
    }
}
