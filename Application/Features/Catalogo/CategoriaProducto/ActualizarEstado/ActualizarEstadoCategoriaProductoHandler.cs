using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Catalogo.CategoriaProducto.ActualizarEstado
{
    public class ActualizarEstadoCategoriaProductoHandler : IRequestHandler<ActualizarEstadoCategoriaProductoCommand, int>
    {
        private readonly ICategoriaProductoService _service;
        private readonly ILogger<ActualizarEstadoCategoriaProductoHandler> _logger;

        public ActualizarEstadoCategoriaProductoHandler(ICategoriaProductoService service, ILogger<ActualizarEstadoCategoriaProductoHandler> logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task<int> Handle(ActualizarEstadoCategoriaProductoCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ActualizarEstadoCategoriaProducto: {@request}", command);

            var categoria = await _service.ObtenerPorIdAsync(command.Id);
            if (categoria == null)
                throw new InvalidOperationException($"CategoriaProducto con ID {command.Id} no encontrada");

            categoria.Activo = command.Activo;
            await _service.Actualizar(categoria);
            _logger.LogInformation("ActualizarEstadoCategoriaProducto: ID {id}", categoria.Id);
            return categoria.Id;
        }
    }
}
