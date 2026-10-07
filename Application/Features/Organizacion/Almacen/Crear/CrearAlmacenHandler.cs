using MediatR;
using AutoMapper;
using Domain.Organizacion;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Features.Organizacion.Almacen.Crear
{
    public class CrearAlmacenHandler : IRequestHandler<CrearAlmacenCommand, int>
    {
        private readonly IAlmacenService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<CrearAlmacenHandler> _logger;

        public CrearAlmacenHandler(IAlmacenService service, IMapper mapper, ILogger<CrearAlmacenHandler> logger)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> Handle(CrearAlmacenCommand request, CancellationToken ct)
        {
            var almacen = _mapper.Map<Domain.Organizacion.Almacen>(request);

            var resultado = await _service.Crear(almacen, ct);

            _logger.LogInformation($"Almacén creado: {almacen.Id}");

            return resultado;
        }
    }
}
