using Application.Exceptions;
using Application.Interfaces;
using Domain.Catalogo;
using MediatR;
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

        public async Task<int> Handle(EliminarSucursalCommand request, CancellationToken cancellationToken)
        {
            var sucursal = await _service.ObtenerPorId(request.Id, true, cancellationToken);
            if (sucursal == null)
                throw new KeyNotFoundException($"Sucursal con Id {request.Id} no encontrada");

            var tieneDependencias = await _service.TieneDependencias(sucursal, cancellationToken);
            if (tieneDependencias)
                throw new BadRequestException("Sucursal en uso en Almacen. Solo se permite deshabilitar mediante PATCH /inactivar");

            await _service.Eliminar(request.Id, cancellationToken);

            _logger.LogInformation($"Sucursal eliminada: {request.Id}");

            return request.Id;
        }
    }
}
