using MediatR;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Features.Organizacion.Sucursal.ActualizarEstado
{
    public class ActualizarEstadoSucursalHandler : IRequestHandler<ActualizarEstadoSucursalCommand, int>
    {
        private readonly ISucursalService _service;
        private readonly ILogger<ActualizarEstadoSucursalHandler> _logger;

        public ActualizarEstadoSucursalHandler(ISucursalService service, ILogger<ActualizarEstadoSucursalHandler> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<int> Handle(ActualizarEstadoSucursalCommand request, CancellationToken ct)
        {
            var sucursal = await _service.ObtenerPorId(request.Id, true, ct);
            if (sucursal == null)
                throw new KeyNotFoundException($"Sucursal con Id {request.Id} no encontrada");

            sucursal.Activo = request.Activo;
            await _service.Actualizar(sucursal, ct);

            _logger.LogInformation($"Sucursal {request.Id} estado actualizado a {request.Activo}");

            return sucursal.Id;
        }
    }
}
