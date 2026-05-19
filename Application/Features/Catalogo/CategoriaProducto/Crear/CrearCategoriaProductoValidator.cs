using Application.Interfaces;
using FluentValidation;

namespace Application.Features.Catalogo.CategoriaProducto.Crear
{
    public class CrearCategoriaProductoValidator : AbstractValidator<CrearCategoriaProductoCommand>
    {
        private readonly ICategoriaProductoValidatorService _validator;

        public CrearCategoriaProductoValidator(ICategoriaProductoValidatorService validator)
        {
            _validator = validator;

            RuleFor(x => x.Nombre)
                .NotEmpty().WithMessage("Nombre es requerido")
                .MaximumLength(150).WithMessage("Nombre no puede exceder 150 caracteres");

            RuleFor(x => x.Descripcion)
                .MaximumLength(500).WithMessage("Descripción no puede exceder 500 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Descripcion));

            RuleFor(x => x.CategoriaPadreId)
                .MustAsync(async (parentId, ct) =>
                {
                    if (parentId == null) return true;
                    var existe = await _validator.ObtenerPorIdAsync(parentId.Value) != null;
                    return existe;
                })
                .WithMessage("Categoría padre no existe")
                .When(x => x.CategoriaPadreId.HasValue);
        }
    }
}
