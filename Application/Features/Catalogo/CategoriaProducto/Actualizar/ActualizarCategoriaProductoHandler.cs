using Application.Interfaces;
using MediatR;

namespace Application.Features.Catalogo.CategoriaProducto.Actualizar
{
    public class ActualizarCategoriaProductoHandler : IRequestHandler<ActualizarCategoriaProductoCommand, int>
    {
        private readonly ICategoriaProductoService _service;
        private readonly ICategoriaProductoValidatorService _validator;

        public ActualizarCategoriaProductoHandler(
            ICategoriaProductoService service,
            ICategoriaProductoValidatorService validator)
        {
            _service = service;
            _validator = validator;
        }

        public async Task<int> Handle(ActualizarCategoriaProductoCommand command, CancellationToken cancellationToken)
        {
            var categoria = await _service.ObtenerPorIdAsync(command.Id);
            if (categoria == null)
                throw new InvalidOperationException($"CategoriaProducto con ID {command.Id} no encontrada");

            // Prevenir ciclos cuando se cambia padre
            if (command.CategoriaPadreId.HasValue &&
                command.CategoriaPadreId != categoria.CategoriaPadreId)
            {
                var esDescendiente = await _validator.EsDescendienteDeAsync(command.CategoriaPadreId.Value, command.Id);
                if (esDescendiente)
                    throw new InvalidOperationException("No se puede crear ciclo: padre no puede ser descendiente");
            }

            categoria.Nombre = command.Nombre;
            categoria.Descripcion = command.Descripcion;
            categoria.CategoriaPadreId = command.CategoriaPadreId;

            await _service.Actualizar(categoria);
            return categoria.Id;
        }
    }
}
