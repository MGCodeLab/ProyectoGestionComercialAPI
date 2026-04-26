using Application.Exceptions;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Clientes.Eliminar
{
    public class EliminarClienteHandler : IRequestHandler<EliminarClienteCommand, Unit>
    {
        private readonly IClienteService _service;
        private readonly ILogger<EliminarClienteHandler> _logger;

        public EliminarClienteHandler(
            IClienteService service,
            ILogger<EliminarClienteHandler> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<Unit> Handle(EliminarClienteCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Eliminando cliente {Id}", request.Id);

            var cliente = await _service.ObtenerPorId(request.Id, isAsTracking: true, cancellationToken);

            if (cliente == null)
                throw new NotFoundException($"Cliente con id {request.Id} no encontrado");

            await _service.Eliminar(cliente, cancellationToken);

            _logger.LogInformation("Cliente {Id} eliminado correctamente", request.Id);

            return Unit.Value;
        }
    }
}
