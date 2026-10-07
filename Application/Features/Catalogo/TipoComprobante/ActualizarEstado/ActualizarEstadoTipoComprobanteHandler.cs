using Application.Handlers;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Application.Features.Catalogo.TipoComprobante.ActualizarEstado
{
    public class ActualizarEstadoTipoComprobanteHandler
        : AuditableUpdateHandlerBase<ActualizarEstadoTipoComprobanteCommand, Unit>
    {
        private readonly ITipoComprobanteService _service;

        public ActualizarEstadoTipoComprobanteHandler(
            ITipoComprobanteService service,
            ILogger<ActualizarEstadoTipoComprobanteHandler> logger)
            : base(null, logger)
        {
            _service = service;
        }

        public override async Task<Unit> Handle(ActualizarEstadoTipoComprobanteCommand request, CancellationToken cancellationToken)
        {
            Logger.LogInformation("ActualizarEstadoTipoComprobante: ID {Id}, Activo {Activo}", request.Id, request.Activo);

            var entity = await _service.ObtenerPorId(request.Id, isAsTracking: true, cancellationToken);
            if (entity == null)
                throw new NotFoundException($"TipoComprobante con ID {request.Id} no encontrado");

            entity.Activo = request.Activo;

            await UpdateAuditableEntity(
                entity,
                async () => await _service.Actualizar(cancellationToken),
                () => Unit.Value,
                cancellationToken
            );

            Logger.LogInformation("TipoComprobante {Id} estado actualizado a {Activo}", request.Id, request.Activo);
            return Unit.Value;
        }
    }
}
