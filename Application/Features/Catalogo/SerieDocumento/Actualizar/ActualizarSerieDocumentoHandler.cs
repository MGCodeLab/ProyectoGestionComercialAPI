using Application.Exceptions;
using Application.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Catalogo.SerieDocumento.Actualizar
{
    public class ActualizarSerieDocumentoHandler : IRequestHandler<ActualizarSerieDocumentoCommand, Unit>
    {
        private readonly ISerieDocumentoService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<ActualizarSerieDocumentoHandler> _logger;

        public ActualizarSerieDocumentoHandler(ISerieDocumentoService service, IMapper mapper, ILogger<ActualizarSerieDocumentoHandler> logger)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Unit> Handle(ActualizarSerieDocumentoCommand request, CancellationToken cancellationToken)
        {
            var entity = await _service.ObtenerPorId(request.Id, isAsTracking: true, cancellationToken);
            if (entity == null)
                throw new NotFoundException($"SerieDocumento con ID {request.Id} no encontrado");

            _mapper.Map(request, entity);
            entity.FechaActualizacion = DateTime.UtcNow;

            await _service.Actualizar(cancellationToken);

            _logger.LogInformation("SerieDocumento actualizado con Id: {Id}", request.Id);

            return Unit.Value;
        }
    }
}
