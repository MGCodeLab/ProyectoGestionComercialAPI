using Application.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Catalogo.TipoComprobante.Crear
{
    public class CrearTipoComprobanteHandler : IRequestHandler<CrearTipoComprobanteCommand, int>
    {
        private readonly ITipoComprobanteService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<CrearTipoComprobanteHandler> _logger;

        public CrearTipoComprobanteHandler(
            ITipoComprobanteService service,
            IMapper mapper,
            ILogger<CrearTipoComprobanteHandler> logger)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> Handle(CrearTipoComprobanteCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("CrearTipoComprobante: {Nombre}", request.Nombre);

            var entity = _mapper.Map<Domain.Catalogo.TipoComprobante>(request);
            var id = await _service.Crear(entity, cancellationToken);

            _logger.LogInformation("TipoComprobante creado con ID {Id}", id);
            return id;
        }
    }
}
