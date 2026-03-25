using FluentValidation;

namespace Application.Features.Productos.Eliminar.Commands
{
    public class EliminarProductoValidator : AbstractValidator<EliminarProductoCommand>
    {
        public EliminarProductoValidator()
        {
            RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("El Id debe ser mayor que 0");
        }
    }
}
