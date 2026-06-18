using Application.Exceptions;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Catalogo.TipoImpuesto.ActualizarEstado
{
    public class ActualizarEstadoTipoImpuestoHandler : IRequestHandler<ActualizarEstadoTipoImpuestoCommand, Unit>
    {
        private readonly ITipoImpuestoService _service;
        private readonly ILogger<ActualizarEstadoTipoImpuestoHandler> _logger;

        public ActualizarEstadoTipoImpuestoHandler(
            ITipoImpuestoService service,
            ILogger<ActualizarEstadoTipoImpuestoHandler> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<Unit> Handle(ActualizarEstadoTipoImpuestoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ActualizarEstadoTipoImpuesto: ID {Id}, Activo={Activo}", request.Id, request.Activo);

            var entity = await _service.ObtenerPorId(request.Id, isAsTracking: true, cancellationToken);
            if (entity == null)
                throw new NotFoundException($"TipoImpuesto con ID {request.Id} no encontrado");

            entity.Activo = request.Activo;
            entity.FechaActualizacion = DateTime.UtcNow;

            await _service.Actualizar(cancellationToken);

            _logger.LogInformation("TipoImpuesto {Id} estado actualizado a {Activo}", request.Id, request.Activo);
            return Unit.Value;
        }
    }
}
