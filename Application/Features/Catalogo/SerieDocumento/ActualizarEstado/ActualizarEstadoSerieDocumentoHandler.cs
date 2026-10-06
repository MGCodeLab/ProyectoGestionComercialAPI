using Application.Handlers;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Application.Features.Catalogo.SerieDocumento.ActualizarEstado
{
    public class ActualizarEstadoSerieDocumentoHandler
        : AuditableUpdateHandlerBase<ActualizarEstadoSerieDocumentoCommand, Unit>
    {
        private readonly ISerieDocumentoService _service;

        public ActualizarEstadoSerieDocumentoHandler(
            ISerieDocumentoService service,
            ILogger<ActualizarEstadoSerieDocumentoHandler> logger)
            : base(null, logger)
        {
            _service = service;
        }

        public override async Task<Unit> Handle(ActualizarEstadoSerieDocumentoCommand request, CancellationToken cancellationToken)
        {
            var entity = await _service.ObtenerPorId(request.Id, isAsTracking: true, cancellationToken);
            if (entity == null)
                throw new NotFoundException($"SerieDocumento con ID {request.Id} no encontrado");

            entity.Activo = request.Activo;

            await UpdateAuditableEntity(
                entity,
                async () => await _service.Actualizar(cancellationToken),
                () => Unit.Value,
                cancellationToken
            );

            Logger.LogInformation("SerieDocumento {Id} estado actualizado a Activo={Activo}", request.Id, request.Activo);

            return Unit.Value;
        }
    }
}
