using Application.Handlers;
using Application.Exceptions;
using Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Application.Features.Catalogo.TipoComprobante.Actualizar
{
    public class ActualizarTipoComprobanteHandler
        : AuditableUpdateHandlerBase<ActualizarTipoComprobanteCommand, Unit>
    {
        private readonly ITipoComprobanteService _service;

        public ActualizarTipoComprobanteHandler(
            ITipoComprobanteService service,
            IMapper mapper,
            ILogger<ActualizarTipoComprobanteHandler> logger)
            : base(mapper, logger)
        {
            _service = service;
        }

        public override async Task<Unit> Handle(ActualizarTipoComprobanteCommand request, CancellationToken cancellationToken)
        {
            Logger.LogInformation("ActualizarTipoComprobante: ID {Id}", request.Id);

            var entity = await _service.ObtenerPorId(request.Id, isAsTracking: true, cancellationToken);
            if (entity == null)
                throw new NotFoundException($"TipoComprobante con ID {request.Id} no encontrado");

            Mapper.Map(request, entity);

            await UpdateAuditableEntity(
                entity,
                async () => await _service.Actualizar(cancellationToken),
                () => Unit.Value,
                cancellationToken
            );

            Logger.LogInformation("TipoComprobante {Id} actualizado correctamente", request.Id);
            return Unit.Value;
        }
    }
}
