using Application.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Catalogo.SerieDocumento.Crear
{
    public class CrearSerieDocumentoHandler : IRequestHandler<CrearSerieDocumentoCommand, int>
    {
        private readonly ISerieDocumentoService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<CrearSerieDocumentoHandler> _logger;

        public CrearSerieDocumentoHandler(ISerieDocumentoService service, IMapper mapper, ILogger<CrearSerieDocumentoHandler> logger)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> Handle(CrearSerieDocumentoCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Domain.Catalogo.SerieDocumento>(request);
            entity.Activo = true;
            entity.NumeroActual = 0;

            var id = await _service.Crear(entity, cancellationToken);

            _logger.LogInformation("SerieDocumento creado con Id: {Id}", id);

            return id;
        }
    }
}
