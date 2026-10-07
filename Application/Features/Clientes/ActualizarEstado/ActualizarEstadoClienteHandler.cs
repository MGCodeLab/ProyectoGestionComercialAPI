using Application.Handlers;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Application.Features.Clientes.ActualizarEstado
{
    public class ActualizarEstadoClienteHandler
        : AuditableUpdateHandlerBase<ActualizarEstadoClienteCommand, Unit>
    {
        private readonly IClienteService _service;

        public ActualizarEstadoClienteHandler(
            IClienteService service,
            ILogger<ActualizarEstadoClienteHandler> logger)
            : base(null, logger)
        {
            _service = service;
        }

        public override async Task<Unit> Handle(ActualizarEstadoClienteCommand request, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Actualizando estado de cliente {Id} a {Activo}", request.Id, request.Activo);

            var cliente = await _service.ObtenerPorId(request.Id, isAsTracking: true, cancellationToken);

            if (cliente == null)
                throw new NotFoundException($"Cliente con id {request.Id} no encontrado");

            cliente.Activo = request.Activo;

            await UpdateAuditableEntity(
                cliente,
                async () => await _service.Actualizar(cancellationToken),
                () => Unit.Value,
                cancellationToken
            );

            var accion = request.Activo ? "activado" : "inactivado";
            Logger.LogInformation("Cliente {Id} {Accion} correctamente", request.Id, accion);

            return Unit.Value;
        }
    }
}
