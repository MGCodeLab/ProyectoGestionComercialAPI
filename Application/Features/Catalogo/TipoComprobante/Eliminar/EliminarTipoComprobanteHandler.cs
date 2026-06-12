using Application.Exceptions;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Catalogo.TipoComprobante.Eliminar
{
    public class EliminarTipoComprobanteHandler : IRequestHandler<EliminarTipoComprobanteCommand, Unit>
    {
        private readonly ITipoComprobanteService _service;
        private readonly ILogger<EliminarTipoComprobanteHandler> _logger;

        public EliminarTipoComprobanteHandler(
            ITipoComprobanteService service,
            ILogger<EliminarTipoComprobanteHandler> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<Unit> Handle(EliminarTipoComprobanteCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("EliminarTipoComprobante: ID {Id}", request.Id);

            var entity = await _service.ObtenerPorId(request.Id, isAsTracking: true, cancellationToken);
            if (entity == null)
                throw new NotFoundException($"TipoComprobante con ID {request.Id} no encontrado");

            await _service.Eliminar(entity, cancellationToken);

            _logger.LogInformation("TipoComprobante {Id} eliminado correctamente", request.Id);
            return Unit.Value;
        }
    }
}
