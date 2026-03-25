using FluentValidation;

namespace Application.Features.Productos.Actualizar
{
    public class ActualizarProductoValidator : AbstractValidator<ActualizarProductoCommand>
    {
        public ActualizarProductoValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty()
                .MaximumLength(150);

            RuleFor(x => x.Precio)
                .GreaterThan(0);
        }
    }
}
