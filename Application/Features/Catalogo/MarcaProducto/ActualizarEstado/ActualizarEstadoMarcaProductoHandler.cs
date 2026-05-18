using Application.Interfaces;
using MediatR;

namespace Application.Features.Catalogo.MarcaProducto.ActualizarEstado
{
    public class ActualizarEstadoMarcaProductoHandler : IRequestHandler<ActualizarEstadoMarcaProductoCommand, int>
    {
        private readonly IMarcaProductoService _service;

        public ActualizarEstadoMarcaProductoHandler(IMarcaProductoService service)
        {
            _service = service;
        }

        public async Task<int> Handle(ActualizarEstadoMarcaProductoCommand command, CancellationToken cancellationToken)
        {
            var marca = await _service.ObtenerPorIdAsync(command.Id);
            if (marca == null)
                throw new InvalidOperationException($"MarcaProducto con ID {command.Id} no encontrada");

            marca.Activo = command.Activo;
            await _service.Actualizar(marca);
            return marca.Id;
        }
    }
}
