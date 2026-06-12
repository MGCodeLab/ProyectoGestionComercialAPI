using Application.Interfaces;
using AutoMapper;
using Domain.Catalogo;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Catalogo.TipoImpuesto.Crear
{
    public class CrearTipoImpuestoHandler : IRequestHandler<CrearTipoImpuestoCommand, int>
    {
        private readonly ITipoImpuestoService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<CrearTipoImpuestoHandler> _logger;

        public CrearTipoImpuestoHandler(
            ITipoImpuestoService service,
            IMapper mapper,
            ILogger<CrearTipoImpuestoHandler> logger)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> Handle(CrearTipoImpuestoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("CrearTipoImpuesto: {@request}", request);

            var entity = _mapper.Map<Domain.Catalogo.TipoImpuesto>(request);
            var id = await _service.Crear(entity, cancellationToken);

            _logger.LogInformation("CrearTipoImpuesto: ID {id}", id);
            return id;
        }
    }
}
