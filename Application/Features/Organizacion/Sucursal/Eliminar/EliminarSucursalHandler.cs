using MediatR;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Features.Organizacion.Sucursal.Eliminar
{
    public class EliminarSucursalHandler : IRequestHandler<EliminarSucursalCommand, int>
    {
        private readonly ISucursalService _service;
        private readonly ILogger<EliminarSucursalHandler> _logger;

        public EliminarSucursalHandler(ISucursalService service, ILogger<EliminarSucursalHandler> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<int> Handle(EliminarSucursalCommand request, CancellationToken ct)
        {
            var sucursal = await _service.ObtenerPorId(request.Id, true);
            if (sucursal == null)
                throw new KeyNotFoundException($"Sucursal con Id {request.Id} no encontrada");

            await _service.Eliminar(request.Id);

            _logger.LogInformation($"Sucursal eliminada: {request.Id}");

            return request.Id;
        }
    }
}
