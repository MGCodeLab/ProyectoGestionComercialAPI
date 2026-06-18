using Application.Exceptions;
using Application.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Catalogo.TipoComprobante.Actualizar
{
    public class ActualizarTipoComprobanteHandler : IRequestHandler<ActualizarTipoComprobanteCommand, Unit>
    {
        private readonly ITipoComprobanteService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<ActualizarTipoComprobanteHandler> _logger;

        public ActualizarTipoComprobanteHandler(
            ITipoComprobanteService service,
            IMapper mapper,
            ILogger<ActualizarTipoComprobanteHandler> logger)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Unit> Handle(ActualizarTipoComprobanteCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ActualizarTipoComprobante: ID {Id}", request.Id);

            var entity = await _service.ObtenerPorId(request.Id, isAsTracking: true, cancellationToken);
            if (entity == null)
                throw new NotFoundException($"TipoComprobante con ID {request.Id} no encontrado");

            _mapper.Map(request, entity);
            entity.FechaActualizacion = DateTime.UtcNow;

            await _service.Actualizar(cancellationToken);

            _logger.LogInformation("TipoComprobante {Id} actualizado correctamente", request.Id);
            return Unit.Value;
        }
    }
}
