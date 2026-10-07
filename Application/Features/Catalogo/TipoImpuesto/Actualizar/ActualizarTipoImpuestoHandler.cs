using Application.Handlers;
using Application.Exceptions;
using Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Application.Features.Catalogo.TipoImpuesto.Actualizar
{
    public class ActualizarTipoImpuestoHandler
        : AuditableUpdateHandlerBase<ActualizarTipoImpuestoCommand, Unit>
    {
        private readonly ITipoImpuestoService _service;

        public ActualizarTipoImpuestoHandler(
            ITipoImpuestoService service,
            IMapper mapper,
            ILogger<ActualizarTipoImpuestoHandler> logger)
            : base(mapper, logger)
        {
            _service = service;
        }

        public override async Task<Unit> Handle(ActualizarTipoImpuestoCommand request, CancellationToken cancellationToken)
        {
            Logger.LogInformation("ActualizarTipoImpuesto: ID {Id}", request.Id);

            var entity = await _service.ObtenerPorId(request.Id, isAsTracking: true, cancellationToken);
            if (entity == null)
                throw new NotFoundException($"TipoImpuesto con ID {request.Id} no encontrado");

            Mapper.Map(request, entity);

            await UpdateAuditableEntity(
                entity,
                async () => await _service.Actualizar(cancellationToken),
                () => Unit.Value,
                cancellationToken
            );

            Logger.LogInformation("TipoImpuesto {Id} actualizado correctamente", request.Id);
            return Unit.Value;
        }
    }
}
