using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Catalogo.MarcaProducto.ActualizarEstado
{
    public class ActualizarEstadoMarcaProductoHandler : IRequestHandler<ActualizarEstadoMarcaProductoCommand, int>
    {
        private readonly IMarcaProductoService _service;
        private readonly ILogger<ActualizarEstadoMarcaProductoHandler> _logger;

        public ActualizarEstadoMarcaProductoHandler(IMarcaProductoService service, ILogger<ActualizarEstadoMarcaProductoHandler> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<int> Handle(ActualizarEstadoMarcaProductoCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ActualizarEstadoMarcaProducto: {@request}", command);

            var marca = await _service.ObtenerPorIdAsync(command.Id);
            if (marca == null)
                throw new InvalidOperationException($"MarcaProducto con ID {command.Id} no encontrada");

            marca.Activo = command.Activo;
            await _service.Actualizar(marca);
            _logger.LogInformation("ActualizarEstadoMarcaProducto: ID {id}", marca.Id);
            return marca.Id;
        }
    }
}
