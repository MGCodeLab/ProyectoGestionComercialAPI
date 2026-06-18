using Application.Exceptions;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Catalogo.TipoImpuesto.Eliminar
{
    public class EliminarTipoImpuestoHandler : IRequestHandler<EliminarTipoImpuestoCommand, Unit>
    {
        private readonly ITipoImpuestoService _service;
        private readonly ILogger<EliminarTipoImpuestoHandler> _logger;

        public EliminarTipoImpuestoHandler(
            ITipoImpuestoService service,
            ILogger<EliminarTipoImpuestoHandler> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<Unit> Handle(EliminarTipoImpuestoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("EliminarTipoImpuesto: ID {Id}", request.Id);

            var entity = await _service.ObtenerPorId(request.Id, isAsTracking: true, cancellationToken);
            if (entity == null)
                throw new NotFoundException($"TipoImpuesto con ID {request.Id} no encontrado");

            await _service.Eliminar(entity, cancellationToken);

            _logger.LogInformation("TipoImpuesto {Id} eliminado correctamente", request.Id);
            return Unit.Value;
        }
    }
}
