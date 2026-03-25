using FluentValidation;

namespace Application.Features.Productos.Crear
{
    public class CrearProductoValidator : AbstractValidator<CrearProductoCommand>
    {
        public CrearProductoValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty()
                .MaximumLength(150);

            RuleFor(x => x.Precio)
                .GreaterThan(0);
        }
    }
}
