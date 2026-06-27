using Application.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Catalogo.MarcaProducto.Actualizar
{
    public class ActualizarMarcaProductoHandler : IRequestHandler<ActualizarMarcaProductoCommand, int>
    {
        private readonly IMarcaProductoService _service;
        private readonly IMapper _mapper;
        private readonly ILogger<ActualizarMarcaProductoHandler> _logger;

        public ActualizarMarcaProductoHandler(IMarcaProductoService service, IMapper mapper, ILogger<ActualizarMarcaProductoHandler> logger)
        {
            _service = service;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> Handle(ActualizarMarcaProductoCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ActualizarMarcaProducto: {@request}", command);

            var marca = await _service.ObtenerPorIdAsync(command.Id);
            if (marca == null)
                throw new InvalidOperationException($"MarcaProducto con ID {command.Id} no encontrada");

            _mapper.Map(command, marca);
            marca.FechaActualizacion = DateTime.UtcNow;

            await _service.Actualizar(marca);
            _logger.LogInformation("ActualizarMarcaProducto: ID {id}", marca.Id);
            return marca.Id;
        }
    }
}
