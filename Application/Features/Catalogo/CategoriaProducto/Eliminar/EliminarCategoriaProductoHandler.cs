using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Catalogo.CategoriaProducto.Eliminar
{
    public class EliminarCategoriaProductoHandler : IRequestHandler<EliminarCategoriaProductoCommand, int>
    {
        private readonly ICategoriaProductoService _service;
        private readonly ILogger<EliminarCategoriaProductoHandler> _logger;

        public EliminarCategoriaProductoHandler(ICategoriaProductoService service, ILogger<EliminarCategoriaProductoHandler> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<int> Handle(EliminarCategoriaProductoCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("EliminarCategoriaProducto: {@request}", command);

            var categoria = await _service.ObtenerPorIdAsync(command.Id);
            if (categoria == null)
                throw new InvalidOperationException($"CategoriaProducto con ID {command.Id} no encontrada");

            await _service.Eliminar(command.Id);
            _logger.LogInformation("EliminarCategoriaProducto: ID {id}", command.Id);
            return command.Id;
        }
    }
}
