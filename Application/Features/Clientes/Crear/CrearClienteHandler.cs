using Application.Interfaces;
using AutoMapper;
using Domain.Comercial;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Clientes.Crear
{
    public class CrearClienteHandler : IRequestHandler<CrearClienteCommand, int>
    {
        private readonly IClienteService _service;
        private readonly ILogger<CrearClienteHandler> _logger;
        private readonly IMapper _mapper;

        public CrearClienteHandler(
            IClienteService service,
            ILogger<CrearClienteHandler> logger,
            IMapper mapper)
        {
            _service = service;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<int> Handle(CrearClienteCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creando cliente {Nombres} {ApellidoPaterno}", request.Nombres, request.ApellidoPaterno);

            var cliente = _mapper.Map<Cliente>(request);

            var id = await _service.Crear(cliente, cancellationToken);

            _logger.LogInformation("Cliente {Nombres} creado con Id {Id}", cliente.Nombres, id);

            return id;
        }
    }
}
