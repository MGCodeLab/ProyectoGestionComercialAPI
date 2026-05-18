using Application.Interfaces;
using Domain.Catalogo;
using MediatR;

namespace Application.Features.Catalogo.MarcaProducto.Crear
{
    public class CrearMarcaProductoHandler : IRequestHandler<CrearMarcaProductoCommand, int>
    {
        private readonly IMarcaProductoService _service;

        public CrearMarcaProductoHandler(IMarcaProductoService service)
        {
            _service = service;
        }

        public async Task<int> Handle(CrearMarcaProductoCommand command, CancellationToken cancellationToken)
        {
            var marca = new Domain.Catalogo.MarcaProducto
            {
                Nombre = command.Nombre,
                Descripcion = command.Descripcion,
                LogoUrl = command.LogoUrl,
                Activo = true
            };

            await _service.Crear(marca);
            return marca.Id;
        }
    }
}
