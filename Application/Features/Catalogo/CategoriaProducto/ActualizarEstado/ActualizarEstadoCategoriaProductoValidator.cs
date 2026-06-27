using FluentValidation;

namespace Application.Features.Catalogo.CategoriaProducto.ActualizarEstado.CategoriaProducto;

public class ActualizarEstadoCategoriaProductoValidator : AbstractValidator<ActualizarEstadoCategoriaProductoCommand>
{
    public ActualizarEstadoCategoriaProductoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El id debe ser mayor a 0");
    }
}
