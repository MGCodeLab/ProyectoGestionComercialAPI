using Application.Interfaces;
using MediatR;

namespace Application.Features.Catalogo.MarcaProducto.Eliminar
{
    public class EliminarMarcaProductoHandler : IRequestHandler<EliminarMarcaProductoCommand, int>
    {
        private readonly IMarcaProductoService _service;

        public EliminarMarcaProductoHandler(IMarcaProductoService service)
        {
            _service = service;
        }

        public async Task<int> Handle(EliminarMarcaProductoCommand command, CancellationToken cancellationToken)
        {
            var marca = await _service.ObtenerPorIdAsync(command.Id);
            if (marca == null)
                throw new InvalidOperationException($"MarcaProducto con ID {command.Id} no encontrada");

            await _service.Eliminar(command.Id);
            return command.Id;
        }
    }
}
