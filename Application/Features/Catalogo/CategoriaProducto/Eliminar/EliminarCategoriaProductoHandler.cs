using Application.Interfaces;
using MediatR;

namespace Application.Features.Catalogo.CategoriaProducto.Eliminar
{
    public class EliminarCategoriaProductoHandler : IRequestHandler<EliminarCategoriaProductoCommand, int>
    {
        private readonly ICategoriaProductoService _service;

        public EliminarCategoriaProductoHandler(ICategoriaProductoService service)
        {
            _service = service;
        }

        public async Task<int> Handle(EliminarCategoriaProductoCommand command, CancellationToken cancellationToken)
        {
            var categoria = await _service.ObtenerPorIdAsync(command.Id);
            if (categoria == null)
                throw new InvalidOperationException($"CategoriaProducto con ID {command.Id} no encontrada");

            await _service.Eliminar(command.Id);
            return command.Id;
        }
    }
}
