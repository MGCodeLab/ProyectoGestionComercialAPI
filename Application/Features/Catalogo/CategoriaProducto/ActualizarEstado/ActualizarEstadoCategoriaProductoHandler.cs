using Application.Interfaces;
using MediatR;

namespace Application.Features.Catalogo.CategoriaProducto.ActualizarEstado
{
    public class ActualizarEstadoCategoriaProductoHandler : IRequestHandler<ActualizarEstadoCategoriaProductoCommand, int>
    {
        private readonly ICategoriaProductoService _service;

        public ActualizarEstadoCategoriaProductoHandler(ICategoriaProductoService service)
        {
            _service = service;
        }

        public async Task<int> Handle(ActualizarEstadoCategoriaProductoCommand command, CancellationToken cancellationToken)
        {
            var categoria = await _service.ObtenerPorIdAsync(command.Id);
            if (categoria == null)
                throw new InvalidOperationException($"CategoriaProducto con ID {command.Id} no encontrada");

            categoria.Activo = command.Activo;
            await _service.Actualizar(categoria);
            return categoria.Id;
        }
    }
}
