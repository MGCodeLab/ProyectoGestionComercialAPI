using Application.Interfaces;
using MediatR;

namespace Application.Features.Catalogo.MarcaProducto.Actualizar
{
    public class ActualizarMarcaProductoHandler : IRequestHandler<ActualizarMarcaProductoCommand, int>
    {
        private readonly IMarcaProductoService _service;

        public ActualizarMarcaProductoHandler(IMarcaProductoService service)
        {
            _service = service;
        }

        public async Task<int> Handle(ActualizarMarcaProductoCommand command, CancellationToken cancellationToken)
        {
            var marca = await _service.ObtenerPorIdAsync(command.Id);
            if (marca == null)
                throw new InvalidOperationException($"MarcaProducto con ID {command.Id} no encontrada");

            marca.Nombre = command.Nombre;
            marca.Descripcion = command.Descripcion;
            marca.LogoUrl = command.LogoUrl;

            await _service.Actualizar(marca);
            return marca.Id;
        }
    }
}
