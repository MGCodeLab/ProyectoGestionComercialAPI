using MediatR;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Features.Organizacion.Almacen.ActualizarEstado
{
    public class ActualizarEstadoAlmacenHandler : IRequestHandler<ActualizarEstadoAlmacenCommand, int>
    {
        private readonly IAlmacenService _service;
        private readonly ILogger<ActualizarEstadoAlmacenHandler> _logger;

        public ActualizarEstadoAlmacenHandler(IAlmacenService service, ILogger<ActualizarEstadoAlmacenHandler> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<int> Handle(ActualizarEstadoAlmacenCommand request, CancellationToken ct)
        {
            var almacen = await _service.ObtenerPorId(request.Id, true);
            if (almacen == null)
                throw new KeyNotFoundException($"Almacén con Id {request.Id} no encontrado");

            almacen.Activo = request.Activo;
            await _service.Actualizar(almacen);

            _logger.LogInformation($"Almacén {request.Id} estado actualizado a {request.Activo}");

            return almacen.Id;
        }
    }
}
