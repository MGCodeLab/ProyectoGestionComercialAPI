using Application.Handlers;
using Application.Exceptions;
using Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Application.Features.Catalogo.SerieDocumento.Actualizar
{
    public class ActualizarSerieDocumentoHandler
        : AuditableUpdateHandlerBase<ActualizarSerieDocumentoCommand, Unit>
    {
        private readonly ISerieDocumentoService _service;

        public ActualizarSerieDocumentoHandler(
            ISerieDocumentoService service,
            IMapper mapper,
            ILogger<ActualizarSerieDocumentoHandler> logger)
            : base(mapper, logger)
        {
            _service = service;
        }

        public override async Task<Unit> Handle(ActualizarSerieDocumentoCommand request, CancellationToken cancellationToken)
        {
            var entity = await _service.ObtenerPorId(request.Id, isAsTracking: true, cancellationToken);
            if (entity == null)
                throw new NotFoundException($"SerieDocumento con ID {request.Id} no encontrado");

            Mapper.Map(request, entity);

            await UpdateAuditableEntity(
                entity,
                async () => await _service.Actualizar(cancellationToken),
                () => Unit.Value,
                cancellationToken
            );

            Logger.LogInformation("SerieDocumento actualizado con Id: {Id}", request.Id);

            return Unit.Value;
        }
    }
}
