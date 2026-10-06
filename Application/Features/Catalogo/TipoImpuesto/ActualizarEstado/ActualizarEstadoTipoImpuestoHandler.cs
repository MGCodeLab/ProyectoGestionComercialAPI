using Application.Handlers;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Application.Features.Catalogo.TipoImpuesto.ActualizarEstado
{
    public class ActualizarEstadoTipoImpuestoHandler
        : AuditableUpdateHandlerBase<ActualizarEstadoTipoImpuestoCommand, Unit>
    {
        private readonly ITipoImpuestoService _service;

        public ActualizarEstadoTipoImpuestoHandler(
            ITipoImpuestoService service,
            ILogger<ActualizarEstadoTipoImpuestoHandler> logger)
            : base(null, logger)
        {
            _service = service;
        }

        public override async Task<Unit> Handle(ActualizarEstadoTipoImpuestoCommand request, CancellationToken cancellationToken)
        {
            Logger.LogInformation("ActualizarEstadoTipoImpuesto: ID {Id}, Activo={Activo}", request.Id, request.Activo);

            var entity = await _service.ObtenerPorId(request.Id, isAsTracking: true, cancellationToken);
            if (entity == null)
                throw new NotFoundException($"TipoImpuesto con ID {request.Id} no encontrado");

            entity.Activo = request.Activo;

            await UpdateAuditableEntity(
                entity,
                async () => await _service.Actualizar(cancellationToken),
                () => Unit.Value,
                cancellationToken
            );

            Logger.LogInformation("TipoImpuesto {Id} estado actualizado a {Activo}", request.Id, request.Activo);
            return Unit.Value;
        }
    }
}
