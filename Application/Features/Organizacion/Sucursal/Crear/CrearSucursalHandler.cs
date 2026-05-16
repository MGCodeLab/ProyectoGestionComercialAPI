using MediatR;
using AutoMapper;
using Domain.Organizacion;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Features.Organizacion.Sucursal.Crear
{
    public class CrearSucursalHandler : IRequestHandler<CrearSucursalCommand, int>
    {
        private readonly ISucursalService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<CrearSucursalHandler> _logger;

        public CrearSucursalHandler(ISucursalService service, IMapper mapper, ILogger<CrearSucursalHandler> logger)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> Handle(CrearSucursalCommand request, CancellationToken ct)
        {
            var sucursal = _mapper.Map<Domain.Organizacion.Sucursal>(request);

            var resultado = await _service.Crear(sucursal);

            _logger.LogInformation($"Sucursal creada: {sucursal.Id}");

            return resultado;
        }
    }
}
