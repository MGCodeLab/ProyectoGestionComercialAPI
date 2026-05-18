using Application.Interfaces;
using Domain.Catalogo;
using MediatR;

namespace Application.Features.Catalogo.CategoriaProducto.Crear
{
    public class CrearCategoriaProductoHandler : IRequestHandler<CrearCategoriaProductoCommand, int>
    {
        private readonly ICategoriaProductoService _service;
        private readonly ICategoriaProductoValidatorService _validator;

        public CrearCategoriaProductoHandler(
            ICategoriaProductoService service,
            ICategoriaProductoValidatorService validator)
        {
            _service = service;
            _validator = validator;
        }

        public async Task<int> Handle(CrearCategoriaProductoCommand command, CancellationToken cancellationToken)
        {
            // Validar profundidad si tiene padre
            if (command.CategoriaPadreId.HasValue)
            {
                var profundidad = await _validator.CalcularProfundidadAsync(command.CategoriaPadreId.Value);
                if (profundidad >= 3)
                    throw new InvalidOperationException("Máximo 3 niveles de profundidad permitidos");
            }

            var categoria = new Domain.Catalogo.CategoriaProducto
            {
                Nombre = command.Nombre,
                Descripcion = command.Descripcion,
                CategoriaPadreId = command.CategoriaPadreId,
                Activo = true
            };

            await _service.Crear(categoria);
            return categoria.Id;
        }
    }
}
