using Application.Exceptions;
using Application.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Clientes.Actualizar
{
    public class ActualizarClienteHandler : IRequestHandler<ActualizarClienteCommand, Unit>
    {
        private readonly IClienteService _service;
        private readonly ILogger<ActualizarClienteHandler> _logger;
        private readonly IMapper _mapper;

        public ActualizarClienteHandler(
            IClienteService service,
            ILogger<ActualizarClienteHandler> logger,
            IMapper mapper)
        {
            _service = service;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(ActualizarClienteCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Actualizando cliente {Id}", request.Id);

            var cliente = await _service.ObtenerPorId(request.Id, isAsTracking: true, cancellationToken);

            if (cliente == null)
                throw new NotFoundException($"Cliente con id {request.Id} no encontrado");

            cliente.TipoDocumentoId = request.TipoDocumentoId;
            cliente.NumeroDocumento = request.NumeroDocumento;
            cliente.Nombres = request.Nombres;
            cliente.ApellidoPaterno = request.ApellidoPaterno;
            cliente.ApellidoMaterno = request.ApellidoMaterno;
            cliente.Correo = request.Correo;
            cliente.Telefono = request.Telefono;
            cliente.Direccion = request.Direccion;
            cliente.FechaActualizacion = DateTime.UtcNow;

            await _service.Actualizar(cancellationToken);

            _logger.LogInformation("Cliente {Id} actualizado correctamente", request.Id);

            return Unit.Value;
        }
    }
}
