using Application.Exceptions;
using Application.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Catalogo.TipoImpuesto.Actualizar
{
    public class ActualizarTipoImpuestoHandler : IRequestHandler<ActualizarTipoImpuestoCommand, Unit>
    {
        private readonly ITipoImpuestoService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<ActualizarTipoImpuestoHandler> _logger;

        public ActualizarTipoImpuestoHandler(
            ITipoImpuestoService service,
            IMapper mapper,
            ILogger<ActualizarTipoImpuestoHandler> logger)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Unit> Handle(ActualizarTipoImpuestoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ActualizarTipoImpuesto: ID {Id}", request.Id);

            var entity = await _service.ObtenerPorId(request.Id, isAsTracking: true, cancellationToken);
            if (entity == null)
                throw new NotFoundException($"TipoImpuesto con ID {request.Id} no encontrado");

            _mapper.Map(request, entity);
            entity.FechaActualizacion = DateTime.UtcNow;

            await _service.Actualizar(cancellationToken);

            _logger.LogInformation("TipoImpuesto {Id} actualizado correctamente", request.Id);
            return Unit.Value;
        }
    }
}
