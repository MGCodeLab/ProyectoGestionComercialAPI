using Application.Handlers;
using Application.Exceptions;
using Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Application.Features.Clientes.Actualizar
{
    public class ActualizarClienteHandler
        : AuditableUpdateHandlerBase<ActualizarClienteCommand, Unit>
    {
        private readonly IClienteService _service;

        public ActualizarClienteHandler(
            IClienteService service,
            ILogger<ActualizarClienteHandler> logger,
            IMapper mapper)
            : base(mapper, logger)
        {
            _service = service;
        }

        public override async Task<Unit> Handle(ActualizarClienteCommand request, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Actualizando cliente {Id}", request.Id);

            var cliente = await _service.ObtenerPorId(request.Id, isAsTracking: true, cancellationToken);

            if (cliente == null)
                throw new NotFoundException($"Cliente con id {request.Id} no encontrado");

            Mapper.Map(request, cliente);

            await UpdateAuditableEntity(
                cliente,
                async () => await _service.Actualizar(cancellationToken),
                () => Unit.Value,
                cancellationToken
            );

            Logger.LogInformation("Cliente {Id} actualizado correctamente", request.Id);

            return Unit.Value;
        }
    }
}
