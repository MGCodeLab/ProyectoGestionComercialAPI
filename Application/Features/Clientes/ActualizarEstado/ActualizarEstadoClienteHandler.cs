using Application.Exceptions;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Clientes.ActualizarEstado
{
    public class ActualizarEstadoClienteHandler : IRequestHandler<ActualizarEstadoClienteCommand, Unit>
    {
        private readonly IClienteService _service;
        private readonly ILogger<ActualizarEstadoClienteHandler> _logger;

        public ActualizarEstadoClienteHandler(
            IClienteService service,
            ILogger<ActualizarEstadoClienteHandler> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<Unit> Handle(ActualizarEstadoClienteCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Actualizando estado de cliente {Id} a {Activo}", request.Id, request.Activo);

            var cliente = await _service.ObtenerPorId(request.Id, isAsTracking: true, cancellationToken);

            if (cliente == null)
                throw new NotFoundException($"Cliente con id {request.Id} no encontrado");

            cliente.Activo = request.Activo;
            cliente.FechaActualizacion = DateTime.UtcNow;

            await _service.Actualizar(cancellationToken);

            var accion = request.Activo ? "activado" : "inactivado";
            _logger.LogInformation("Cliente {Id} {Accion} correctamente", request.Id, accion);

            return Unit.Value;
        }
    }
}
