using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Catalogo.MarcaProducto.Eliminar
{
    public class EliminarMarcaProductoHandler : IRequestHandler<EliminarMarcaProductoCommand, int>
    {
        private readonly IMarcaProductoService _service;
        private readonly ILogger<EliminarMarcaProductoHandler> _logger;

        public EliminarMarcaProductoHandler(IMarcaProductoService service, ILogger<EliminarMarcaProductoHandler> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<int> Handle(EliminarMarcaProductoCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("EliminarMarcaProducto: {@request}", command);

            var marca = await _service.ObtenerPorIdAsync(command.Id);
            if (marca == null)
                throw new InvalidOperationException($"MarcaProducto con ID {command.Id} no encontrada");

            await _service.Eliminar(command.Id);
            _logger.LogInformation("EliminarMarcaProducto: ID {id}", command.Id);
            return command.Id;
        }
    }
}
