using MediatR;
using AutoMapper;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Features.Organizacion.Sucursal.Actualizar
{
    public class ActualizarSucursalHandler : IRequestHandler<ActualizarSucursalCommand, int>
    {
        private readonly ISucursalService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<ActualizarSucursalHandler> _logger;

        public ActualizarSucursalHandler(ISucursalService service, IMapper mapper, ILogger<ActualizarSucursalHandler> logger)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> Handle(ActualizarSucursalCommand request, CancellationToken ct)
        {
            var sucursal = await _service.ObtenerPorId(request.Id, true, ct);
            if (sucursal == null)
                throw new KeyNotFoundException($"Sucursal con Id {request.Id} no encontrada");

            _mapper.Map(request, sucursal);
            sucursal.FechaActualizacion = DateTime.UtcNow;
            await _service.Actualizar(sucursal, ct);

            _logger.LogInformation($"Sucursal actualizada: {sucursal.Id}");

            return sucursal.Id;
        }
    }
}
